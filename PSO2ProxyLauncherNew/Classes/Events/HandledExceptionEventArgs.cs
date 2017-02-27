using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class HandledExceptionEventArgs : EventArgs
    {
        public Exception Error { get; private set; }
        public HandledExceptionEventArgs(Exception ex):base()
        {
            this.Error = ex;
        }

        public override string ToString()
        {
            return this.Error.Message;
        }
    }
}
