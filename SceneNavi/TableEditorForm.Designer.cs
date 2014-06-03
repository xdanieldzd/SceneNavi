namespace SceneNavi
{
    partial class TableEditorForm
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
            this.dgvEntranceTable = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpExitTable = new System.Windows.Forms.TabPage();
            this.tpSceneTable = new System.Windows.Forms.TabPage();
            this.dgvSceneTable = new System.Windows.Forms.DataGridView();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntranceTable)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tpExitTable.SuspendLayout();
            this.tpSceneTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSceneTable)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvExitTable
            // 
            this.dgvEntranceTable.AllowUserToAddRows = false;
            this.dgvEntranceTable.AllowUserToDeleteRows = false;
            this.dgvEntranceTable.AllowUserToResizeColumns = false;
            this.dgvEntranceTable.AllowUserToResizeRows = false;
            this.dgvEntranceTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEntranceTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEntranceTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEntranceTable.Location = new System.Drawing.Point(0, 0);
            this.dgvEntranceTable.MultiSelect = false;
            this.dgvEntranceTable.Name = "dgvExitTable";
            this.dgvEntranceTable.RowHeadersVisible = false;
            this.dgvEntranceTable.Size = new System.Drawing.Size(562, 390);
            this.dgvEntranceTable.TabIndex = 0;
            this.dgvEntranceTable.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvEntranceTable_CellFormatting);
            this.dgvEntranceTable.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.dgvEntranceTable_CellParsing);
            this.dgvEntranceTable.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvEntranceTable_DataError);
            this.dgvEntranceTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvEntranceTable_EditingControlShowing);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpExitTable);
            this.tabControl1.Controls.Add(this.tpSceneTable);
            this.tabControl1.Location = new System.Drawing.Point(12, 45);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(570, 416);
            this.tabControl1.TabIndex = 1;
            // 
            // tpExitTable
            // 
            this.tpExitTable.Controls.Add(this.dgvEntranceTable);
            this.tpExitTable.Location = new System.Drawing.Point(4, 22);
            this.tpExitTable.Name = "tpExitTable";
            this.tpExitTable.Size = new System.Drawing.Size(562, 390);
            this.tpExitTable.TabIndex = 0;
            this.tpExitTable.Text = "Entrance Table";
            this.tpExitTable.UseVisualStyleBackColor = true;
            // 
            // tpSceneTable
            // 
            this.tpSceneTable.Controls.Add(this.dgvSceneTable);
            this.tpSceneTable.Location = new System.Drawing.Point(4, 22);
            this.tpSceneTable.Name = "tpSceneTable";
            this.tpSceneTable.Size = new System.Drawing.Size(562, 390);
            this.tpSceneTable.TabIndex = 1;
            this.tpSceneTable.Text = "Scene Table";
            this.tpSceneTable.UseVisualStyleBackColor = true;
            // 
            // dgvSceneTable
            // 
            this.dgvSceneTable.AllowUserToAddRows = false;
            this.dgvSceneTable.AllowUserToDeleteRows = false;
            this.dgvSceneTable.AllowUserToResizeColumns = false;
            this.dgvSceneTable.AllowUserToResizeRows = false;
            this.dgvSceneTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSceneTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSceneTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSceneTable.Location = new System.Drawing.Point(0, 0);
            this.dgvSceneTable.MultiSelect = false;
            this.dgvSceneTable.Name = "dgvSceneTable";
            this.dgvSceneTable.RowHeadersVisible = false;
            this.dgvSceneTable.Size = new System.Drawing.Size(562, 390);
            this.dgvSceneTable.TabIndex = 1;
            this.dgvSceneTable.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvSceneTable_CellFormatting);
            this.dgvSceneTable.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.dgvSceneTable_CellParsing);
            this.dgvSceneTable.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvSceneTable_DataError);
            this.dgvSceneTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvSceneTable_EditingControlShowing);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(497, 467);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(85, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.label1.Size = new System.Drawing.Size(569, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "Warning: Make sure you know what you\'re doing! Invalid values could potentially c" +
    "ause game crashes, and prevent SceneNavi from ex. recognizing a scene!";
            // 
            // TableEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(594, 502);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TableEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Tables";
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntranceTable)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tpExitTable.ResumeLayout(false);
            this.tpSceneTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSceneTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvEntranceTable;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpExitTable;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabPage tpSceneTable;
        private System.Windows.Forms.DataGridView dgvSceneTable;
        private System.Windows.Forms.Label label1;
    }
}