using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class PSO2LaunchedEventArgs : EventArgs
    {
        public string GameFolder { get; }
        public Exception Error { get; }
        public PSO2LaunchedEventArgs(Exception ex, string _gamefolder) : base()
        {
            this.Error = ex;
            this.GameFolder = _gamefolder;
        }

        public PSO2LaunchedEventArgs(Exception ex) : this(ex, string.Empty) { }
    }
}
