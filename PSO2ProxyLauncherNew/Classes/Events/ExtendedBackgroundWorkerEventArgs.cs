using System;
using PSO2ProxyLauncherNew.Classes.PSO2;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class ExtendedBackgroundWorkerEventArgs : EventArgs
    {
        public ExtendedBackgroundWorker Worker { get; }
        public ExtendedBackgroundWorkerEventArgs(ExtendedBackgroundWorker _worker) : base()
        {
            this.Worker = _worker;
        }
    }
}
