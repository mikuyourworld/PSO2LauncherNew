using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class IntEventArgs : EventArgs
    {
        public int Value { get; }
        public IntEventArgs(int _val) : base()
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
