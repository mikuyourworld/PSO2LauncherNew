using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class StepEventArgs : EventArgs
    {
        public string Step { get; }
        public StepEventArgs(string _step) : base()
        {
            this.Step = _step;
        }
    }
}
