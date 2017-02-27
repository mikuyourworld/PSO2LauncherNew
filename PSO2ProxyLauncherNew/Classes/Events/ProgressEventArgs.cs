using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class ProgressEventArgs : EventArgs
    {
        public int Progress { get; }
        public ProgressEventArgs(int _progress) : base()
        {
            this.Progress = _progress;
        }
    }
}
