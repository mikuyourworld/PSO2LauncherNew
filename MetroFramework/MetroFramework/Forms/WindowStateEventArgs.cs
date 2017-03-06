using System;
using System.Windows.Forms;

namespace MetroFramework.MetroFramework.Forms
{
    public class WindowStateEventArgs : EventArgs
    {
        public FormWindowState StateBefore { get; }
        public WindowStateEventArgs(FormWindowState _state) : base()
        {
            this.StateBefore = _state;
        }
    }
}
