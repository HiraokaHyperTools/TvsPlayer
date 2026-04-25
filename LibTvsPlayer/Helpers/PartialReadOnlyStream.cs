using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public class PartialReadOnlyStream : Stream
    {
        private int _remain;
        private readonly int _size;
        private readonly Stream _stream;

        public PartialReadOnlyStream(Stream stream, int size)
        {
            _remain = size;
            _size = size;
            _stream = stream;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _size;

        public override long Position
        {
            get => _size - _remain;
            set => throw new NotImplementedException();
        }

        public override void Flush()
        {
            // do nothing, read only stream
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int toRead = Math.Min(count, _remain);
            int read = _stream.Read(buffer, offset, toRead);
            if (0 < read)
            {
                _remain -= read;
            }
            return read;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int toRead = Math.Min(buffer.Length, _remain);
            int read = await _stream.ReadAsync(buffer.Slice(0, toRead), cancellationToken);
            if (0 < read)
            {
                _remain -= read;
            }
            return read;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seek is not supported on PartialReadOnlyStream");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength is not supported on PartialReadOnlyStream");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Write is not supported on PartialReadOnlyStream");
        }
    }
}
