using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class TroubleshootingCompletedEventArgs : EventArgs
    {
        public object Result { get; }
        public Components.TroubleshootingType TroubleshootingType { get; }
        public TroubleshootingCompletedEventArgs(Components.TroubleshootingType tt, object _result) : base()
        {
            this.TroubleshootingType = tt;
            this.Result = _result;
        }

        public TroubleshootingCompletedEventArgs(Components.TroubleshootingType tt) : this(tt, null) { }
    }
}
