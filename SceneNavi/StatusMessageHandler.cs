using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SceneNavi
{
    public class StatusMessageHandler
    {
        public delegate void MessageChangedEvent(object sender, MessageChangedEventArgs e);
        public class MessageChangedEventArgs : EventArgs
        {
            public string Message { get; set; }
        }
        public event MessageChangedEvent MessageChanged;

        string lastmsg;
        public string Message
        {
            get { return lastmsg; }
            set
            {
                lastmsg = value;
                var ev = MessageChanged;
                if (ev != null) ev(this, new MessageChangedEventArgs() { Message = lastmsg });
            }
        }
    }
}
