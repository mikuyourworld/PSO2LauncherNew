using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class StringEventArgs : EventArgs
    {
        public string Value { get; }
        public StringEventArgs(string val) : base()
        {
            this.Value = val;
        }
    }
}
