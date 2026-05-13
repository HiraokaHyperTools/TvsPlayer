using kenjiuno.AutoHourglass;
using LibTvsPlayer.DataTypes;
using LibTvsPlayer.Helpers;
using LibTvsPlayer.Services;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Compression;
using System.Windows.Forms;
using System.Xml.Serialization;
using VideoConv4Win.DataTypes;
using VideoConv4Win.Properties;

namespace VideoConv4Win
{
    public partial class Form1 : Form
    {
        private AVFormats _avFormats;
        private readonly DecodeMouseRecord _decodeMouseRecord;
        private readonly ParseKey _parseKey;
        private readonly ParseTvsStruc _parseTvsStruc;
        private readonly DecodeKeyRecord _decodeKeyRecord;
        private readonly Func<ConvertProgressForm> _newConvertProgressForm;
        private readonly DetectVideoFrameSize _detectVideoFrameSize;
        private string _ffmpegOptions = "";

        public Form1(
            DetectVideoFrameSize detectVideoFrameSize,
            Func<ConvertProgressForm> newConvertProgressForm,
            DecodeKeyRecord decodeKeyRecord,
            ParseTvsStruc parseTvsStruc,
            ParseKey parseKey,
            DecodeMouseRecord decodeMouseRecord)
        {
            _decodeMouseRecord = decodeMouseRecord;
            _parseKey = parseKey;
            _parseTvsStruc = parseTvsStruc;
            _decodeKeyRecord = decodeKeyRecord;
            _newConvertProgressForm = newConvertProgressForm;
            _detectVideoFrameSize = detectVideoFrameSize;

            InitializeComponent();

            _ffmpegExe.Text = Settings.Default.FFMPEGEXE;
            _avFormats = new AVFormats();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using var stream = File.OpenRead(Path.Combine(Application.StartupPath, "AVFormats.xml"));
            _avFormats = (AVFormats)(new XmlSerializer(typeof(AVFormats))
                .Deserialize(
                    stream
                )
                ?? throw new Exception()
            );

            _videoFormat.ValueMember = "";
            _videoFormat.DisplayMember = "Display";
            _videoFormat.DataSource = _avFormats
                .FFmpegVideo;

            _videoFormat_TextChanged(sender, e);
        }

        private async void _proceed_Click(object sender, EventArgs e)
        {
            Settings.Default.FFMPEGEXE = _ffmpegExe.Text;
            Settings.Default.Save();

            if (true
                && File.Exists(_saveVideoTo.Text)
                && MessageBox.Show(this, $"The following file exists, overwrite it anyway?\n\n{_saveVideoTo.Text}", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) != DialogResult.Yes
            )
            {
                return;
            }

            var form = _newConvertProgressForm();
            form.Text = _saveVideoTo.Text;
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            form._cancel.Click += (_, __) => cts.Cancel();

            form.FormClosed += (_, __) =>
            {
                cts.Cancel();
                cts.Dispose();
            };

            var timestampByFps = new TimestampByFps(_fps.Value);

            var cxScreen = Convert.ToInt32(_cx.Value);
            var cyScreen = Convert.ToInt32(_cy.Value);
            var offscreen = new byte[4 * cxScreen * cyScreen];
            var mouseSave = new byte[4 * 32 * 32];
            var mouseBitmap = new byte[4 * 32 * 32];
            var mouseX = -1;
            var mouseY = -1;

            var writeToOffScreen = new WriteToOffScreen(
                offscreen,
                cxScreen,
                cyScreen
            );

            Process LaunchFFmpeg()
            {
                var psi = new ProcessStartInfo(
                    _ffmpegExe.Text,
                    $"-y -f rawvideo -vcodec rawvideo -pix_fmt rgba -s {cxScreen}x{cyScreen} -framerate {_fps.Value} -i - {_ffmpegOptions} \"{_saveVideoTo.Text}\""
                )
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                };
                return Process.Start(psi) ?? throw new Exception();
            }

            var ffmpegProcess = LaunchFFmpeg();
            using var ffmpegPipe0 = ffmpegProcess.StandardInput.BaseStream;

            form.Show(this);

            IFormattable playingAt = $"";

            async Task ProceedAsync()
            {
                using var tvsStream = File.OpenRead(_tvsFile.Text);
                using var buffered = new BufferedStream(tvsStream);

                async Task readAsync(Memory<byte> buf, long position)
                {
                    buffered.Position = position;
                    await buffered.ReadExactlyAsync(buf)
                        .ConfigureAwait(false);
                }

                var struc = await _parseTvsStruc.ParseAsync(readAsync);

                var state = new DecodeKeyRecord.State();
                var mouseState = new DecodeMouseRecord.State();

                form._status.Text = "Rendering frames...";
                form._progress.Value = 0;
                form._progress.Maximum = struc.TvsChunks.Count;

                var numFramesEmitted = 0;

                async Task EmitScreenBufferAsync(int nTimes)
                {
                    if (0 <= mouseX)
                    {
                        writeToOffScreen.SaveTo(mouseSave, 32, 32, mouseX, mouseY);
                        writeToOffScreen.BitbltSrcAlpha(mouseBitmap, 32, 32, mouseX, mouseY);
                    }

                    for (int x = 0; x < nTimes; x++)
                    {
                        await ffmpegPipe0.WriteAsync(offscreen);
                        numFramesEmitted++;
                    }

                    if (0 <= mouseX)
                    {
                        writeToOffScreen.Bitblt(mouseSave, 32, 32, mouseX, mouseY);
                    }
                }

                var startTime = DateTime.Now;
                var nextUpdatedTime = startTime.AddSeconds(1);
                var maxFileSize = tvsStream.Length;

                foreach (var chunkRef in struc.TvsChunks)
                {
                    var compressed = new byte[chunkRef.CompressedSize];
                    await readAsync(compressed, chunkRef.ChunkPosition);
                    using var ms = new MemoryStream(compressed, false);
                    using var zlib = new ZLibStream(ms, CompressionMode.Decompress);
                    var keyFile = new byte[chunkRef.UncompressedSize];
                    await zlib.ReadExactlyAsync(keyFile);

                    var tvsPositionPercent = chunkRef.ChunkPosition * 1.0 / Math.Max(1, maxFileSize);

                    var keyRecords = _parseKey.Parse(keyFile);
                    foreach (var keyRecord in keyRecords)
                    {
                        playingAt = $"ChunkPosition {chunkRef.ChunkPosition} Timestamp {keyRecord.Timestamp}";

                        ct.ThrowIfCancellationRequested();

                        {
                            var now = DateTime.Now;
                            if (nextUpdatedTime <= now)
                            {
                                nextUpdatedTime = now.AddSeconds(1);

                                var eta = (0 < tvsPositionPercent)
                                    ? startTime.AddTicks(Convert.ToInt64(now.Subtract(startTime).Ticks / tvsPositionPercent))
                                    : now;
                                form._status.Text = $"Rendering frames {numFramesEmitted}\n\nSince: {startTime}\nNow: {now}\nTvsFilePosition: {chunkRef.ChunkPosition:#,##0} / {maxFileSize:#,##0} ({tvsPositionPercent:P1})\nETA: {eta}";
                            }
                        }

                        var numFramesGenerated = timestampByFps.Reach(keyRecord.Timestamp);
                        if (numFramesGenerated != 0)
                        {
                            await EmitScreenBufferAsync(numFramesGenerated);
                        }

                        {
                            var changes = _decodeKeyRecord.Consume(keyRecord, state);

                            if (changes.Render32bppBitmaps != null)
                            {
                                foreach (var cmd in changes.Render32bppBitmaps)
                                {
                                    writeToOffScreen.Bitblt(
                                        cmd.Bits,
                                        cmd.Width,
                                        cmd.Height,
                                        cmd.Tx,
                                        cmd.Ty
                                    );
                                }
                            }
                            if (changes.FillColors != null)
                            {
                                foreach (var cmd in changes.FillColors)
                                {
                                    writeToOffScreen.FillColor(
                                        cmd.R,
                                        cmd.G,
                                        cmd.B,
                                        cmd.Width,
                                        cmd.Height,
                                        cmd.Tx,
                                        cmd.Ty
                                    );
                                }
                            }
                        }

                        {
                            var changes = _decodeMouseRecord.Consume(keyRecord, mouseState);
                            if (changes.SetMousePos is Point pt)
                            {
                                mouseX = pt.X;
                                mouseY = pt.Y;
                            }
                            if (changes.SetMouseBitmap is ReadOnlyMemory<byte> setMouseBitmap)
                            {
                                CopyMouseBitmap(
                                    setMouseBitmap.Span,
                                    mouseBitmap
                                );
                            }
                        }
                    }

                    form._progress.Value += 1;
                }

                await EmitScreenBufferAsync(1);
                ffmpegPipe0.Close();

                form._status.Text = "Waiting for termination of FFmpeg.";

                await ffmpegProcess.WaitForExitAsync();
            }

            try
            {
                await ProceedAsync();
                form._status.Text = "Rendering finished.";

                form._cancel.Text = "Close";
                form._cancel.Click += (_, __) => form.Close();
            }
            catch (Exception ex)
            {
                form._status.Text = $"At {playingAt}, fatal error: {ex}";
            }

        }

        private void CopyMouseBitmap(ReadOnlySpan<byte> readerSpan, Span<byte> writerSpan)
        {
            var readerTopSpan = readerSpan;
            // bottom up to top down, BGRA not RGBA
            for (int y = 0; y < 32; y++)
            {
                readerSpan = readerTopSpan.Slice(4 * 32 * (31 - y));
                for (int x = 0; x < 32; x++)
                {
                    // BGRA to RGBA
                    writerSpan[2] = readerSpan[0];
                    writerSpan[1] = readerSpan[1];
                    writerSpan[0] = readerSpan[2];
                    writerSpan[3] = readerSpan[3];

                    writerSpan = writerSpan.Slice(4);
                    readerSpan = readerSpan.Slice(4);
                }
            }
        }

        private void _tvsFileRef_Click(object sender, EventArgs e)
        {
            _ofdTvs.FileName = _tvsFile.Text;

            if (_ofdTvs.ShowDialog(this) == DialogResult.OK)
            {
                _tvsFile.Text = _ofdTvs.FileName;
            }
        }

        private void _ffmpegExeRef_Click(object sender, EventArgs e)
        {
            _ofdFFmpegExe.FileName = _ffmpegExe.Text;

            if (_ofdFFmpegExe.ShowDialog(this) == DialogResult.OK)
            {
                _ffmpegExe.Text = _ofdFFmpegExe.FileName;
            }
        }

        private void _saveVideoToRef_Click(object sender, EventArgs e)
        {
            _sfdVideo.FileName = _saveVideoTo.Text;

            if (_sfdVideo.ShowDialog(this) == DialogResult.OK)
            {
                _saveVideoTo.Text = _sfdVideo.FileName;
            }
        }

        private void _videoFormat_TextChanged(object sender, EventArgs e)
        {
            if (_videoFormat.SelectedItem is FFmpegVideo video)
            {
                _ffmpegDesc.Text = video.Description;
                _sfdVideo.DefaultExt = video.FileExtension;
                _sfdVideo.Filter = $"{video.Description} (*.{video.FileExtension})|*.{video.FileExtension}";
                _ffmpegOptions = video.Options ?? "";
            }
            else
            {
                _ffmpegDesc.Text = "";
            }
        }

        private void _detectVideoFrameSize_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_tvsFile.Text) || !File.Exists(_tvsFile.Text))
            {
                _tvsFile.Focus();
                _tvsFile.SelectAll();
                return;
            }

            using var _ = new AH();
            using var tvsStream = File.OpenRead(_tvsFile.Text);
            using var buffered = new BufferedStream(tvsStream);

            var sizeOrNull = _detectVideoFrameSize.DetectAsync(
                readAsync: async (buf, position) =>
                {
                    buffered.Position = position;
                    await buffered.ReadExactlyAsync(buf)
                        .ConfigureAwait(false);
                }
            )
                .GetAwaiter()
                .GetResult();

            if (sizeOrNull is Size size)
            {
                _cx.Value = size.Width;
                _cy.Value = size.Height;

                MessageBox.Show($"Detected as {size.Width}x{size.Height}");
            }
            else
            {
                MessageBox.Show("Unfortunaetly, detected nothing.");
            }
        }
    }
}
