namespace VideoConv4Win
{
    partial class ConvertProgressForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _cancel = new Button();
            _status = new Label();
            _progress = new ProgressBar();
            _panel = new TableLayoutPanel();
            _panel.SuspendLayout();
            SuspendLayout();
            // 
            // _cancel
            // 
            _cancel.Location = new Point(10, 244);
            _cancel.Name = "_cancel";
            _cancel.Size = new Size(75, 23);
            _cancel.TabIndex = 0;
            _cancel.Text = "Cancel";
            _cancel.UseVisualStyleBackColor = true;
            // 
            // _status
            // 
            _status.Dock = DockStyle.Fill;
            _status.Location = new Point(10, 7);
            _status.Name = "_status";
            _status.Size = new Size(509, 193);
            _status.TabIndex = 1;
            _status.Text = "...";
            // 
            // _progress
            // 
            _progress.Dock = DockStyle.Top;
            _progress.Location = new Point(10, 209);
            _progress.Margin = new Padding(3, 9, 3, 9);
            _progress.Name = "_progress";
            _progress.Size = new Size(509, 23);
            _progress.TabIndex = 2;
            // 
            // _panel
            // 
            _panel.ColumnCount = 1;
            _panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _panel.Controls.Add(_status, 0, 0);
            _panel.Controls.Add(_cancel, 0, 2);
            _panel.Controls.Add(_progress, 0, 1);
            _panel.Dock = DockStyle.Fill;
            _panel.Location = new Point(0, 0);
            _panel.Name = "_panel";
            _panel.Padding = new Padding(7);
            _panel.RowCount = 3;
            _panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _panel.RowStyles.Add(new RowStyle());
            _panel.RowStyles.Add(new RowStyle());
            _panel.Size = new Size(529, 277);
            _panel.TabIndex = 3;
            // 
            // ConvertProgressForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancel;
            ClientSize = new Size(529, 277);
            Controls.Add(_panel);
            Name = "ConvertProgressForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ConvertProgressForm";
            _panel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        internal Button _cancel;
        internal Label _status;
        internal ProgressBar _progress;
        private TableLayoutPanel _panel;
    }
}