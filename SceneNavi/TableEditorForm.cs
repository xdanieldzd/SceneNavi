using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SceneNavi
{
    public partial class TableEditorForm : Form
    {
        ROMHandler.ROMHandler ROM;

        public TableEditorForm(ROMHandler.ROMHandler rom)
        {
            InitializeComponent();

            ROM = rom;

            InitializeDataGridViews();
            dgvExitTable.Select();
        }

        private void InitializeDataGridViews()
        {
            /* Enable double-buffering to prevent scroll flicker */
            dgvExitTable.DoubleBuffered(true);
            dgvSceneTable.DoubleBuffered(true);

            /* Bind data & configure exit table */
            dgvExitTable.DataSource = new BindingSource() { DataSource = ROM.Exits };
            dgvExitTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvExitTable.Columns["SceneNumber"].DefaultCellStyle.Format = "X2";
            dgvExitTable.Columns["SceneNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvExitTable.Columns["EntranceNumber"].DefaultCellStyle.Format = "X2";
            dgvExitTable.Columns["EntranceNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvExitTable.Columns["Variable"].DefaultCellStyle.Format = "X2";
            dgvExitTable.Columns["Variable"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvExitTable.Columns["Fade"].DefaultCellStyle.Format = "X2";
            dgvExitTable.Columns["Fade"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvExitTable.Columns["SceneName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            foreach (DataGridViewColumn dcc in dgvExitTable.Columns) if (dcc.ReadOnly) dcc.DefaultCellStyle.ForeColor = SystemColors.GrayText;

            /* Bind data & configure scene table */
            dgvSceneTable.DataSource = new BindingSource() { DataSource = ROM.Scenes };
            dgvSceneTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSceneTable.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSceneTable.Columns["LabelStartAddress"].DefaultCellStyle.Format = "X8";
            dgvSceneTable.Columns["LabelStartAddress"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["LabelEndAddress"].DefaultCellStyle.Format = "X8";
            dgvSceneTable.Columns["LabelEndAddress"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["Unknown1"].DefaultCellStyle.Format = "X2";
            dgvSceneTable.Columns["Unknown1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["ConfigurationNo"].DefaultCellStyle.Format = "X2";
            dgvSceneTable.Columns["ConfigurationNo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["ConfigurationNo"].ToolTipText = "Configuration specifies ex. camera effects, dynamic textures, etc.";
            dgvSceneTable.Columns["Unknown3"].DefaultCellStyle.Format = "X2";
            dgvSceneTable.Columns["Unknown3"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["Unknown4"].DefaultCellStyle.Format = "X2";
            dgvSceneTable.Columns["Unknown4"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            foreach (DataGridViewColumn dcc in dgvSceneTable.Columns) if (dcc.ReadOnly) dcc.DefaultCellStyle.ForeColor = SystemColors.GrayText;
        }

        #region DGV Exit table events

        private void dgvExitTable_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.Columns[e.ColumnIndex].Name == "SceneNumber" || dgv.Columns[e.ColumnIndex].Name == "EntranceNumber" || dgv.Columns[e.ColumnIndex].Name == "Variable" || dgv.Columns[e.ColumnIndex].Name == "Fade")
            {
                if (e != null && e.Value != null && e.DesiredType.Equals(typeof(string)))
                {
                    try
                    {
                        e.Value = string.Format("0x{0:X2}", e.Value);
                        e.FormattingApplied = true;
                    }
                    catch
                    {
                        /* Not hexadecimal */
                    }
                }
            }
        }

        private void dgvExitTable_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.Columns[e.ColumnIndex].Name == "SceneNumber" || dgv.Columns[e.ColumnIndex].Name == "EntranceNumber" || dgv.Columns[e.ColumnIndex].Name == "Variable" || dgv.Columns[e.ColumnIndex].Name == "Fade")
            {
                if (e != null && e.Value != null && e.DesiredType.Equals(typeof(byte)))
                {
                    string str = (e.Value as string);
                    bool ishex = str.StartsWith("0x");

                    byte val = 0;
                    if (byte.TryParse((ishex ? str.Substring(2) : str), (ishex ? System.Globalization.NumberStyles.AllowHexSpecifier : System.Globalization.NumberStyles.None),
                        System.Globalization.CultureInfo.InvariantCulture, out val))
                    {
                        e.Value = val;
                        e.ParsingApplied = true;
                    }
                }
            }
        }

        private void dgvExitTable_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            int column = dgv.CurrentCell.ColumnIndex;
            string name = dgv.Columns[column].DataPropertyName;

            if (name.Equals("SceneName") && e.Control is TextBox)
            {
                TextBox tb = e.Control as TextBox;
                tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tb.AutoCompleteCustomSource = ROM.SceneNameACStrings;
                tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }

        private void dgvExitTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is FormatException) System.Media.SystemSounds.Hand.Play();
        }

        #endregion

        #region DGV Scene table events

        private void dgvSceneTable_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.Columns[e.ColumnIndex].Name == "LabelStartAddress" || dgv.Columns[e.ColumnIndex].Name == "LabelEndAddress" ||
                dgv.Columns[e.ColumnIndex].Name == "Unknown1" || dgv.Columns[e.ColumnIndex].Name == "ConfigurationNo" || dgv.Columns[e.ColumnIndex].Name == "Unknown3" || dgv.Columns[e.ColumnIndex].Name == "Unknown4")
            {
                if (e != null && e.Value != null && e.DesiredType.Equals(typeof(string)))
                {
                    try
                    {
                        if (dgv.Columns[e.ColumnIndex].ValueType == typeof(byte))
                            e.Value = string.Format("0x{0:X2}", e.Value);
                        else if (dgv.Columns[e.ColumnIndex].ValueType == typeof(uint))
                            e.Value = string.Format("0x{0:X8}", e.Value);
                        else
                            e.Value = string.Format("{0:X}", e.Value);

                        e.FormattingApplied = true;
                    }
                    catch
                    {
                        /* Not hexadecimal */
                    }
                }
            }
        }

        private void dgvSceneTable_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.Columns[e.ColumnIndex].Name == "LabelStartAddress" || dgv.Columns[e.ColumnIndex].Name == "LabelEndAddress" ||
                dgv.Columns[e.ColumnIndex].Name == "Unknown1" || dgv.Columns[e.ColumnIndex].Name == "ConfigurationNo" || dgv.Columns[e.ColumnIndex].Name == "Unknown3" || dgv.Columns[e.ColumnIndex].Name == "Unknown4")
            {
                if (e != null && e.Value != null)
                {
                    string str = (e.Value as string);
                    bool ishex = str.StartsWith("0x");

                    if (e.DesiredType.Equals(typeof(byte)))
                    {
                        byte val = 0;
                        if (byte.TryParse((ishex ? str.Substring(2) : str), (ishex ? System.Globalization.NumberStyles.AllowHexSpecifier : System.Globalization.NumberStyles.None),
                            System.Globalization.CultureInfo.InvariantCulture, out val))
                        {
                            e.Value = val;
                            e.ParsingApplied = true;
                        }
                    }
                    else if (e.DesiredType.Equals(typeof(uint)))
                    {
                        uint val = 0;
                        if (uint.TryParse((ishex ? str.Substring(2) : str), (ishex ? System.Globalization.NumberStyles.AllowHexSpecifier : System.Globalization.NumberStyles.None),
                            System.Globalization.CultureInfo.InvariantCulture, out val))
                        {
                            e.Value = val;
                            e.ParsingApplied = true;
                        }
                    }
                }
            }
        }

        private void dgvSceneTable_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //
        }

        private void dgvSceneTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is FormatException) System.Media.SystemSounds.Hand.Play();
        }

        #endregion
    }
}
