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
            dgvEntranceTable.Select();
        }

        private void InitializeDataGridViews()
        {
            /* Enable double-buffering to prevent scroll flicker */
            dgvEntranceTable.DoubleBuffered(true);
            dgvSceneTable.DoubleBuffered(true);

            /* Bind data & configure entrance table */
            dgvEntranceTable.DataSource = new BindingSource() { DataSource = ROM.Entrances };
            dgvEntranceTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvEntranceTable.Columns["Number"].DefaultCellStyle.Format = "X4";
            dgvEntranceTable.Columns["Number"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvEntranceTable.Columns["SceneNumber"].DefaultCellStyle.Format = "X2";
            dgvEntranceTable.Columns["SceneNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvEntranceTable.Columns["SceneNumber"].ToolTipText = "Scene number to load";
            dgvEntranceTable.Columns["EntranceNumber"].DefaultCellStyle.Format = "X2";
            dgvEntranceTable.Columns["EntranceNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvEntranceTable.Columns["EntranceNumber"].ToolTipText = "Entrance within scene to spawn at";
            dgvEntranceTable.Columns["Variable"].DefaultCellStyle.Format = "X2";
            dgvEntranceTable.Columns["Variable"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvEntranceTable.Columns["Variable"].ToolTipText = "Controls certain behaviors when transitioning, ex. stopping music";
            dgvEntranceTable.Columns["Fade"].DefaultCellStyle.Format = "X2";
            dgvEntranceTable.Columns["Fade"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvEntranceTable.Columns["Fade"].ToolTipText = "Animation used when transitioning";
            dgvEntranceTable.Columns["SceneName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            foreach (DataGridViewColumn dcc in dgvEntranceTable.Columns) if (dcc.ReadOnly) dcc.DefaultCellStyle.ForeColor = SystemColors.GrayText;

            /* Bind data & configure scene table */
            dgvSceneTable.DataSource = new BindingSource() { DataSource = ROM.Scenes };
            dgvSceneTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSceneTable.Columns["Number"].DefaultCellStyle.Format = "X4";
            dgvSceneTable.Columns["Number"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSceneTable.Columns["LabelStartAddress"].DefaultCellStyle.Format = "X8";
            dgvSceneTable.Columns["LabelStartAddress"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["LabelStartAddress"].ToolTipText = "Start address of area title card";
            dgvSceneTable.Columns["LabelEndAddress"].DefaultCellStyle.Format = "X8";
            dgvSceneTable.Columns["LabelEndAddress"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["LabelEndAddress"].ToolTipText = "End address of area title card";
            dgvSceneTable.Columns["Unknown1"].DefaultCellStyle.Format = "X2";
            dgvSceneTable.Columns["Unknown1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["Unknown1"].ToolTipText = "Unknown; either 0x01 or 0x02 for some dungeons, otherwise 0x00";
            dgvSceneTable.Columns["ConfigurationNo"].DefaultCellStyle.Format = "X2";
            dgvSceneTable.Columns["ConfigurationNo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["ConfigurationNo"].ToolTipText = "Specifies ex. camera effects, dynamic textures, etc.";
            dgvSceneTable.Columns["Unknown3"].DefaultCellStyle.Format = "X2";
            dgvSceneTable.Columns["Unknown3"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["Unknown3"].ToolTipText = "Unknown; unique value between 0x02 and 0x0A for some dungeons";
            dgvSceneTable.Columns["Unknown4"].DefaultCellStyle.Format = "X2";
            dgvSceneTable.Columns["Unknown4"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvSceneTable.Columns["Unknown4"].ToolTipText = "Unknown; always 0x00, unused or padding?";
            foreach (DataGridViewColumn dcc in dgvSceneTable.Columns) if (dcc.ReadOnly) dcc.DefaultCellStyle.ForeColor = SystemColors.GrayText;
        }

        #region DGV Entrance table events

        private void dgvEntranceTable_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.Columns[e.ColumnIndex].Name == "Number" || dgv.Columns[e.ColumnIndex].Name == "SceneNumber" || dgv.Columns[e.ColumnIndex].Name == "EntranceNumber" ||
                dgv.Columns[e.ColumnIndex].Name == "Variable" || dgv.Columns[e.ColumnIndex].Name == "Fade")
            {
                if (e != null && e.Value != null && e.DesiredType.Equals(typeof(string)))
                {
                    try
                    {
                        e.Value = (dgv.Columns[e.ColumnIndex].Name == "Number" ? string.Format("0x{0:X4}", e.Value) : string.Format("0x{0:X2}", e.Value));
                        e.FormattingApplied = true;
                    }
                    catch
                    {
                        /* Not hexadecimal */
                    }
                }
            }
        }

        private void dgvEntranceTable_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
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

        private void dgvEntranceTable_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
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

        private void dgvEntranceTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is FormatException) System.Media.SystemSounds.Hand.Play();
        }

        #endregion

        #region DGV Scene table events

        private void dgvSceneTable_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.Columns[e.ColumnIndex].Name == "Number" || dgv.Columns[e.ColumnIndex].Name == "LabelStartAddress" || dgv.Columns[e.ColumnIndex].Name == "LabelEndAddress" ||
                dgv.Columns[e.ColumnIndex].Name == "Unknown1" || dgv.Columns[e.ColumnIndex].Name == "ConfigurationNo" || dgv.Columns[e.ColumnIndex].Name == "Unknown3" || dgv.Columns[e.ColumnIndex].Name == "Unknown4")
            {
                if (e != null && e.Value != null && e.DesiredType.Equals(typeof(string)))
                {
                    try
                    {
                        if (dgv.Columns[e.ColumnIndex].ValueType == typeof(byte))
                            e.Value = string.Format("0x{0:X2}", e.Value);
                        else if (dgv.Columns[e.ColumnIndex].ValueType == typeof(ushort))
                            e.Value = string.Format("0x{0:X4}", e.Value);
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
