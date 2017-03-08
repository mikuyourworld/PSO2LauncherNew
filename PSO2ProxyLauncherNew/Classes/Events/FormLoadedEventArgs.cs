using System;
using System.Threading;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class FormLoadedEventArgs : EventArgs
    {
        public SynchronizationContext SyncContext { get; }
        public FormLoadedEventArgs(SynchronizationContext _sync)
        {
            this.SyncContext = _sync;
        }
    }
}
