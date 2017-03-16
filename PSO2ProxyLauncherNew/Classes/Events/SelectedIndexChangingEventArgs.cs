using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class SelectedIndexChangingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
        public int IndexBefore { get; }
        public int IndexAfter { get; }
        public SelectedIndexChangingEventArgs(int before, int after)
        {
            this.Cancel = false;
            this.IndexBefore = before;
            this.IndexAfter = after;
        }
    }
}
