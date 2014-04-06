using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

namespace SceneNavi
{
    class GUIHelpers
    {
        /// <summary>
        /// Show a file save dialog
        /// </summary>
        /// <param name="filter">File filters to use</param>
        /// <param name="filteridx">Default filter index (optional)</param>
        /// <returns>Path to selected file</returns>
        public static string ShowSaveFileDialog(string filter, int filteridx = -1)
        {
            string sfile = string.Empty;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = filter;
            sfd.FilterIndex = filteridx;
            sfd.CheckPathExists = true;

            if (sfd.ShowDialog() == DialogResult.OK) sfile = sfd.FileName;

            return sfile;
        }

        public static string LoadLocalizedString(string libraryName, uint ident, string defaultText)
        {
            IntPtr libraryHandle = LoadLibrary(libraryName);
            if (libraryHandle != IntPtr.Zero)
            {
                StringBuilder sb = new StringBuilder(1024);
                int size = LoadString(libraryHandle, ident, sb, 1024);
                if (size > 0) return sb.ToString();
            }
            FreeLibrary(libraryHandle);
            return defaultText;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string path);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hInst);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        [System.Drawing.ToolboxBitmap(typeof(ToolStripButton))]
        [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.StatusStrip)]
        public class ButtonStripItem : ToolStripButton
        {
        }

        [System.Drawing.ToolboxBitmap(typeof(ToolStripSeparator))]
        [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.StatusStrip)]
        public class SeparatorStripItem : ToolStripSeparator
        {
        }
    }
}
