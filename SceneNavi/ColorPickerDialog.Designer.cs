namespace SceneNavi
{
    partial class ColorPickerDialog
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblRed = new System.Windows.Forms.Label();
            this.lblGreen = new System.Windows.Forms.Label();
            this.lblBlue = new System.Windows.Forms.Label();
            this.lblAlpha = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudRed = new System.Windows.Forms.NumericUpDown();
            this.nudGreen = new System.Windows.Forms.NumericUpDown();
            this.nudBlue = new System.Windows.Forms.NumericUpDown();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.pbColorGradientAlpha = new System.Windows.Forms.PictureBox();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.pbColorGradientBlue = new System.Windows.Forms.PictureBox();
            this.pbColorGradientGreen = new System.Windows.Forms.PictureBox();
            this.pbColorGradientRed = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorGradientAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorGradientBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorGradientGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorGradientRed)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(357, 167);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(276, 167);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblRed
            // 
            this.lblRed.Location = new System.Drawing.Point(12, 9);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(53, 20);
            this.lblRed.TabIndex = 0;
            this.lblRed.Text = "Red";
            this.lblRed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblGreen
            // 
            this.lblGreen.Location = new System.Drawing.Point(12, 35);
            this.lblGreen.Name = "lblGreen";
            this.lblGreen.Size = new System.Drawing.Size(53, 20);
            this.lblGreen.TabIndex = 2;
            this.lblGreen.Text = "Green";
            this.lblGreen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBlue
            // 
            this.lblBlue.Location = new System.Drawing.Point(12, 61);
            this.lblBlue.Name = "lblBlue";
            this.lblBlue.Size = new System.Drawing.Size(53, 20);
            this.lblBlue.TabIndex = 4;
            this.lblBlue.Text = "Blue";
            this.lblBlue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAlpha
            // 
            this.lblAlpha.Location = new System.Drawing.Point(12, 87);
            this.lblAlpha.Name = "lblAlpha";
            this.lblAlpha.Size = new System.Drawing.Size(53, 20);
            this.lblAlpha.TabIndex = 6;
            this.lblAlpha.Text = "Alpha";
            this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 40);
            this.label1.TabIndex = 8;
            this.label1.Text = "Preview";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nudRed
            // 
            this.nudRed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudRed.Location = new System.Drawing.Point(389, 9);
            this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.Name = "nudRed";
            this.nudRed.Size = new System.Drawing.Size(43, 20);
            this.nudRed.TabIndex = 1;
            this.nudRed.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.ValueChanged += new System.EventHandler(this.nudRed_ValueChanged);
            // 
            // nudGreen
            // 
            this.nudGreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudGreen.Location = new System.Drawing.Point(389, 35);
            this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Size = new System.Drawing.Size(43, 20);
            this.nudGreen.TabIndex = 3;
            this.nudGreen.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.ValueChanged += new System.EventHandler(this.nudGreen_ValueChanged);
            // 
            // nudBlue
            // 
            this.nudBlue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudBlue.Location = new System.Drawing.Point(389, 61);
            this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Size = new System.Drawing.Size(43, 20);
            this.nudBlue.TabIndex = 5;
            this.nudBlue.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.ValueChanged += new System.EventHandler(this.nudBlue_ValueChanged);
            // 
            // nudAlpha
            // 
            this.nudAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudAlpha.Location = new System.Drawing.Point(389, 87);
            this.nudAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Size = new System.Drawing.Size(43, 20);
            this.nudAlpha.TabIndex = 7;
            this.nudAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAlpha.ValueChanged += new System.EventHandler(this.nudAlpha_ValueChanged);
            // 
            // pbColorGradientAlpha
            // 
            this.pbColorGradientAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbColorGradientAlpha.BackgroundImage = global::SceneNavi.Properties.Resources.bg_empty_3;
            this.pbColorGradientAlpha.Location = new System.Drawing.Point(72, 87);
            this.pbColorGradientAlpha.Name = "pbColorGradientAlpha";
            this.pbColorGradientAlpha.Size = new System.Drawing.Size(311, 20);
            this.pbColorGradientAlpha.TabIndex = 18;
            this.pbColorGradientAlpha.TabStop = false;
            this.pbColorGradientAlpha.Paint += new System.Windows.Forms.PaintEventHandler(this.pbColorGradientAlpha_Paint);
            this.pbColorGradientAlpha.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbColorGradientAlpha_MouseDown);
            this.pbColorGradientAlpha.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbColorGradientAlpha_MouseMove);
            // 
            // pbPreview
            // 
            this.pbPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbPreview.BackgroundImage = global::SceneNavi.Properties.Resources.bg_empty_3;
            this.pbPreview.Location = new System.Drawing.Point(71, 116);
            this.pbPreview.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(361, 40);
            this.pbPreview.TabIndex = 16;
            this.pbPreview.TabStop = false;
            this.pbPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPreview_Paint);
            this.pbPreview.DoubleClick += new System.EventHandler(this.pbPreview_DoubleClick);
            // 
            // pbColorGradientBlue
            // 
            this.pbColorGradientBlue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbColorGradientBlue.BackgroundImage = global::SceneNavi.Properties.Resources.bg_empty_3;
            this.pbColorGradientBlue.Location = new System.Drawing.Point(72, 61);
            this.pbColorGradientBlue.Name = "pbColorGradientBlue";
            this.pbColorGradientBlue.Size = new System.Drawing.Size(311, 20);
            this.pbColorGradientBlue.TabIndex = 4;
            this.pbColorGradientBlue.TabStop = false;
            this.pbColorGradientBlue.Paint += new System.Windows.Forms.PaintEventHandler(this.pbColorGradientBlue_Paint);
            this.pbColorGradientBlue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbColorGradientBlue_MouseDown);
            this.pbColorGradientBlue.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbColorGradientBlue_MouseMove);
            // 
            // pbColorGradientGreen
            // 
            this.pbColorGradientGreen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbColorGradientGreen.BackgroundImage = global::SceneNavi.Properties.Resources.bg_empty_3;
            this.pbColorGradientGreen.Location = new System.Drawing.Point(72, 35);
            this.pbColorGradientGreen.Name = "pbColorGradientGreen";
            this.pbColorGradientGreen.Size = new System.Drawing.Size(311, 20);
            this.pbColorGradientGreen.TabIndex = 3;
            this.pbColorGradientGreen.TabStop = false;
            this.pbColorGradientGreen.Paint += new System.Windows.Forms.PaintEventHandler(this.pbColorGradientGreen_Paint);
            this.pbColorGradientGreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbColorGradientGreen_MouseDown);
            this.pbColorGradientGreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbColorGradientGreen_MouseMove);
            // 
            // pbColorGradientRed
            // 
            this.pbColorGradientRed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbColorGradientRed.BackgroundImage = global::SceneNavi.Properties.Resources.bg_empty_3;
            this.pbColorGradientRed.Location = new System.Drawing.Point(72, 9);
            this.pbColorGradientRed.Name = "pbColorGradientRed";
            this.pbColorGradientRed.Size = new System.Drawing.Size(311, 20);
            this.pbColorGradientRed.TabIndex = 2;
            this.pbColorGradientRed.TabStop = false;
            this.pbColorGradientRed.Paint += new System.Windows.Forms.PaintEventHandler(this.pbColorGradientRed_Paint);
            this.pbColorGradientRed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbColorGradientRed_MouseDown);
            this.pbColorGradientRed.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbColorGradientRed_MouseMove);
            // 
            // ColorPickerDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(444, 202);
            this.Controls.Add(this.nudAlpha);
            this.Controls.Add(this.nudBlue);
            this.Controls.Add(this.nudGreen);
            this.Controls.Add(this.nudRed);
            this.Controls.Add(this.pbColorGradientAlpha);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbPreview);
            this.Controls.Add(this.lblAlpha);
            this.Controls.Add(this.lblBlue);
            this.Controls.Add(this.lblGreen);
            this.Controls.Add(this.lblRed);
            this.Controls.Add(this.pbColorGradientBlue);
            this.Controls.Add(this.pbColorGradientGreen);
            this.Controls.Add(this.pbColorGradientRed);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorPickerDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Color Picker";
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorGradientAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorGradientBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorGradientGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbColorGradientRed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pbColorGradientRed;
        private System.Windows.Forms.PictureBox pbColorGradientGreen;
        private System.Windows.Forms.PictureBox pbColorGradientBlue;
        private System.Windows.Forms.Label lblRed;
        private System.Windows.Forms.Label lblGreen;
        private System.Windows.Forms.Label lblBlue;
        private System.Windows.Forms.Label lblAlpha;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbColorGradientAlpha;
        private System.Windows.Forms.NumericUpDown nudRed;
        private System.Windows.Forms.NumericUpDown nudGreen;
        private System.Windows.Forms.NumericUpDown nudBlue;
        private System.Windows.Forms.NumericUpDown nudAlpha;
    }
}