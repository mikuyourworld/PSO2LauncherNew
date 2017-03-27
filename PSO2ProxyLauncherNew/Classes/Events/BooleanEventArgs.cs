using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class BooleanEventArgs : EventArgs
    {
        public bool Value { get; }
        public BooleanEventArgs(bool _val) : base()
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
