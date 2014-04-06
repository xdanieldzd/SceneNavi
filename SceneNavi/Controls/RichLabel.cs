using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SceneNavi.Controls
{
    class RichLabel : RichTextBox
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        public RichLabel()
        {
            //this.ReadOnly = true;
            //this.TabStop = false;
            //this.SetStyle(ControlStyles.Selectable, false);
        }

        protected override void OnEnter(EventArgs e)
        {
            //if (!DesignMode) this.Parent.SelectNextControl(this, true, true, true, true);
            base.OnEnter(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg < 0x201 || m.Msg > 0x20e || m.Msg == 0x20a) base.WndProc(ref m);

            HideCaret(this.Handle);
        }
    }
}
