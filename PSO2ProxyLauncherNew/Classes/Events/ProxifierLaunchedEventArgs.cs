using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class ProxifierLaunchedEventArgs : EventArgs
    {
        public Exception Error { get; }
        public ProxifierLaunchedEventArgs(Exception ex)
        {
            this.Error = ex;
        }
    }
}
