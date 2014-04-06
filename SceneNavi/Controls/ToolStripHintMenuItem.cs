using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SceneNavi.Controls
{
    public class ToolStripHintMenuItem : ToolStripMenuItem
    {
        public event EventHandler Hint, Unhint;

        public override bool Selected
        {
            get
            {
                if (base.Selected) OnHint(EventArgs.Empty);
                else OnUnhint(EventArgs.Empty);
                return base.Selected;
            }
        }

        public string HelpText { get; set; }

        public ToolStripHintMenuItem() :
            base(null, null, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripHintMenuItem(string text)
            : base(text, null, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripHintMenuItem(Image image)
            : base(null, image, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripHintMenuItem(string text, Image image)
            : base(text, image, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripHintMenuItem(string text, Image image, EventHandler onClick)
            : base(text, image, onClick)
        {
            Initialize();
        }

        public ToolStripHintMenuItem(string text, Image image, EventHandler onClick, string name)
            : base(text, image, onClick, name)
        {
            Initialize();
        }

        public ToolStripHintMenuItem(string text, Image image, params ToolStripItem[] dropDownItems)
            : base(text, image, dropDownItems)
        {
            Initialize();
        }

        public ToolStripHintMenuItem(string text, Image image, EventHandler onClick, Keys shortcutKeys)
            : base(text, image, onClick)
        {
            Initialize();
        }

        private void Initialize()
        {
            HelpText = null;
        }

        protected virtual void OnHint(EventArgs e)
        {
            if (Hint != null) Hint(this, e);
        }

        protected virtual void OnUnhint(EventArgs e)
        {
            if (Unhint != null) Unhint(this, e);
        }
    }
}
