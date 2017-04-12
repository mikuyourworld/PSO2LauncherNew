using System;

namespace Leayal.Forms
{
    public delegate void FakeControlEventHandler(object sender, FakeControlEventArgs e);
    public class FakeControlEventArgs : EventArgs
    {
        private FakeControl _control;
        public FakeControl Control => this._control;
        public FakeControlEventArgs(FakeControl control):base()
        {
            this._control = control;
        }
    }
}
