namespace VideoConv4Win
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            _tvsFile = new TextBox();
            label3 = new Label();
            _ffmpegExe = new TextBox();
            _tvsFileRef = new Button();
            _ffmpegExeRef = new Button();
            label4 = new Label();
            _videoFormat = new ComboBox();
            label5 = new Label();
            _saveVideoTo = new TextBox();
            _saveVideoToRef = new Button();
            label6 = new Label();
            _cx = new NumericUpDown();
            _cy = new NumericUpDown();
            _detectVideoFrameSizeBtn = new Button();
            _proceed = new Button();
            label7 = new Label();
            _ffmpegDesc = new Label();
            _ofdTvs = new OpenFileDialog();
            _ofdFFmpegExe = new OpenFileDialog();
            _sfdVideo = new SaveFileDialog();
            label8 = new Label();
            _fps = new NumericUpDown();
            label9 = new Label();
            ((System.ComponentModel.ISupportInitialize)_cx).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_cy).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_fps).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(278, 15);
            label1.TabIndex = 0;
            label1.Text = "Convert .tvs file to video format using your FFmpeg.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 49);
            label2.Name = "label2";
            label2.Size = new Size(44, 15);
            label2.TabIndex = 1;
            label2.Text = ".tvs file";
            // 
            // _tvsFile
            // 
            _tvsFile.Location = new Point(12, 67);
            _tvsFile.Name = "_tvsFile";
            _tvsFile.Size = new Size(248, 23);
            _tvsFile.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 122);
            label3.Name = "label3";
            label3.Size = new Size(48, 15);
            label3.TabIndex = 4;
            label3.Text = "FFmpeg";
            // 
            // _ffmpegExe
            // 
            _ffmpegExe.Location = new Point(12, 140);
            _ffmpegExe.Name = "_ffmpegExe";
            _ffmpegExe.Size = new Size(248, 23);
            _ffmpegExe.TabIndex = 5;
            // 
            // _tvsFileRef
            // 
            _tvsFileRef.Location = new Point(266, 67);
            _tvsFileRef.Name = "_tvsFileRef";
            _tvsFileRef.Size = new Size(75, 23);
            _tvsFileRef.TabIndex = 3;
            _tvsFileRef.Text = "Browse...";
            _tvsFileRef.UseVisualStyleBackColor = true;
            _tvsFileRef.Click += _tvsFileRef_Click;
            // 
            // _ffmpegExeRef
            // 
            _ffmpegExeRef.Location = new Point(266, 140);
            _ffmpegExeRef.Name = "_ffmpegExeRef";
            _ffmpegExeRef.Size = new Size(75, 23);
            _ffmpegExeRef.TabIndex = 6;
            _ffmpegExeRef.Text = "Browse...";
            _ffmpegExeRef.UseVisualStyleBackColor = true;
            _ffmpegExeRef.Click += _ffmpegExeRef_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 198);
            label4.Name = "label4";
            label4.Size = new Size(132, 15);
            label4.TabIndex = 7;
            label4.Text = "Convert to video format";
            // 
            // _videoFormat
            // 
            _videoFormat.FormattingEnabled = true;
            _videoFormat.Location = new Point(12, 216);
            _videoFormat.Name = "_videoFormat";
            _videoFormat.Size = new Size(329, 23);
            _videoFormat.TabIndex = 8;
            _videoFormat.Text = "mpeg4";
            _videoFormat.TextChanged += _videoFormat_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 401);
            label5.Name = "label5";
            label5.Size = new Size(96, 15);
            label5.TabIndex = 17;
            label5.Text = "Save video file to";
            // 
            // _saveVideoTo
            // 
            _saveVideoTo.Location = new Point(12, 419);
            _saveVideoTo.Name = "_saveVideoTo";
            _saveVideoTo.Size = new Size(248, 23);
            _saveVideoTo.TabIndex = 18;
            // 
            // _saveVideoToRef
            // 
            _saveVideoToRef.Location = new Point(266, 419);
            _saveVideoToRef.Name = "_saveVideoToRef";
            _saveVideoToRef.Size = new Size(75, 23);
            _saveVideoToRef.TabIndex = 19;
            _saveVideoToRef.Text = "Browse...";
            _saveVideoToRef.UseVisualStyleBackColor = true;
            _saveVideoToRef.Click += _saveVideoToRef_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 285);
            label6.Name = "label6";
            label6.Size = new Size(92, 15);
            label6.TabIndex = 11;
            label6.Text = "Video frame size";
            // 
            // _cx
            // 
            _cx.Location = new Point(12, 303);
            _cx.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
            _cx.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _cx.Name = "_cx";
            _cx.Size = new Size(92, 23);
            _cx.TabIndex = 12;
            _cx.TextAlign = HorizontalAlignment.Right;
            _cx.Value = new decimal(new int[] { 1920, 0, 0, 0 });
            // 
            // _cy
            // 
            _cy.Location = new Point(110, 303);
            _cy.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
            _cy.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _cy.Name = "_cy";
            _cy.Size = new Size(92, 23);
            _cy.TabIndex = 13;
            _cy.TextAlign = HorizontalAlignment.Right;
            _cy.Value = new decimal(new int[] { 1080, 0, 0, 0 });
            // 
            // _detectVideoFrameSizeBtn
            // 
            _detectVideoFrameSizeBtn.Location = new Point(266, 303);
            _detectVideoFrameSizeBtn.Name = "_detectVideoFrameSizeBtn";
            _detectVideoFrameSizeBtn.Size = new Size(75, 23);
            _detectVideoFrameSizeBtn.TabIndex = 14;
            _detectVideoFrameSizeBtn.Text = "Detect !";
            _detectVideoFrameSizeBtn.UseVisualStyleBackColor = true;
            _detectVideoFrameSizeBtn.Click += _detectVideoFrameSize_Click;
            // 
            // _proceed
            // 
            _proceed.Location = new Point(12, 498);
            _proceed.Name = "_proceed";
            _proceed.Size = new Size(75, 23);
            _proceed.TabIndex = 20;
            _proceed.Text = "Proceed";
            _proceed.UseVisualStyleBackColor = true;
            _proceed.Click += _proceed_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 242);
            label7.Name = "label7";
            label7.Size = new Size(70, 15);
            label7.TabIndex = 9;
            label7.Text = "Description:";
            // 
            // _ffmpegDesc
            // 
            _ffmpegDesc.AutoSize = true;
            _ffmpegDesc.Location = new Point(88, 242);
            _ffmpegDesc.Name = "_ffmpegDesc";
            _ffmpegDesc.Size = new Size(16, 15);
            _ffmpegDesc.TabIndex = 10;
            _ffmpegDesc.Text = "...";
            // 
            // _ofdTvs
            // 
            _ofdTvs.DefaultExt = "tvs";
            _ofdTvs.Filter = "*.tvs|*.tvs";
            // 
            // _ofdFFmpegExe
            // 
            _ofdFFmpegExe.DefaultExt = "exe";
            _ofdFFmpegExe.Filter = "ffmpeg.exe|ffmpeg.exe";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 329);
            label8.Name = "label8";
            label8.Size = new Size(23, 15);
            label8.TabIndex = 15;
            label8.Text = "fps";
            // 
            // _fps
            // 
            _fps.DecimalPlaces = 2;
            _fps.Location = new Point(12, 347);
            _fps.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
            _fps.Name = "_fps";
            _fps.Size = new Size(92, 23);
            _fps.TabIndex = 16;
            _fps.TextAlign = HorizontalAlignment.Right;
            _fps.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 467);
            label9.Name = "label9";
            label9.Size = new Size(279, 15);
            label9.TabIndex = 21;
            label9.Text = "Note: current quality of TVS decoder is not so good.";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(406, 543);
            Controls.Add(label9);
            Controls.Add(_fps);
            Controls.Add(label8);
            Controls.Add(_ffmpegDesc);
            Controls.Add(label7);
            Controls.Add(_proceed);
            Controls.Add(_cy);
            Controls.Add(_cx);
            Controls.Add(label6);
            Controls.Add(_saveVideoTo);
            Controls.Add(label5);
            Controls.Add(_videoFormat);
            Controls.Add(label4);
            Controls.Add(_saveVideoToRef);
            Controls.Add(_detectVideoFrameSizeBtn);
            Controls.Add(_ffmpegExeRef);
            Controls.Add(_tvsFileRef);
            Controls.Add(_ffmpegExe);
            Controls.Add(label3);
            Controls.Add(_tvsFile);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "VideoConv4Win";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)_cx).EndInit();
            ((System.ComponentModel.ISupportInitialize)_cy).EndInit();
            ((System.ComponentModel.ISupportInitialize)_fps).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox _tvsFile;
        private Label label3;
        private TextBox _ffmpegExe;
        private Button _tvsFileRef;
        private Button _ffmpegExeRef;
        private Label label4;
        private ComboBox _videoFormat;
        private Label label5;
        private TextBox _saveVideoTo;
        private Button _saveVideoToRef;
        private Label label6;
        private NumericUpDown _cx;
        private NumericUpDown _cy;
        private Button _detectVideoFrameSizeBtn;
        private Button _proceed;
        private Label label7;
        private Label _ffmpegDesc;
        private OpenFileDialog _ofdTvs;
        private OpenFileDialog _ofdFFmpegExe;
        private SaveFileDialog _sfdVideo;
        private Label label8;
        private NumericUpDown _fps;
        private Label label9;
    }
}
