using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class VisibleNotifyEventArgs : EventArgs
    {
        public bool Visible { get; }

        public VisibleNotifyEventArgs(bool mybool) : base()
        {
            this.Visible = mybool;
        }
    }
}
