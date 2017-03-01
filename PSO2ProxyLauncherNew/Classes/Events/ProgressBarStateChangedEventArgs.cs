using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class ProgressBarStateChangedEventArgs : EventArgs
    {
        public object Properties { get; }
        public Forms.MyMainMenu.ProgressBarVisibleState ProgressBarState { get; }
        public ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState _state, object _properties) : base()
        {
            this.ProgressBarState = _state;
            this.Properties = _properties;
        }

        public ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState _state) : this(_state, null) { }
    }
}
