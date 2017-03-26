using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class CacheWriteProgressChangedEventArgs : EventArgs
    {
        public long BytesReceived { get; }
        public bool Cancel { get; set; }
        public CacheWriteProgressChangedEventArgs(long bytes) : base()
        {
            this.BytesReceived = bytes;
            this.Cancel = false;
        }
    }
}
