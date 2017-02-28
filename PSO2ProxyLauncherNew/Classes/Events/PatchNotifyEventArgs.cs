using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class PatchNotifyEventArgs : EventArgs
    {
        public string PatchVer { get; }

        public PatchNotifyEventArgs(string verString) : base()
        {
            this.PatchVer = verString;
        }
    }
}
