using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class PSO2LaunchedEventArgs : EventArgs
    {
        public Exception Error { get; }
        public PSO2LaunchedEventArgs(Exception ex) : base()
        {
            this.Error = ex;
        }
    }
}
