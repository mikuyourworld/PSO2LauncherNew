using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class CheckForPluginCompletedEventArgs : EventArgs
    {
        public Exception Error { get; }
        public int PluginUpdatedCount { get; }

        public CheckForPluginCompletedEventArgs(Exception ex) : base()
        {
            this.Error = ex;
            this.PluginUpdatedCount = 0;
        }

        public CheckForPluginCompletedEventArgs(int pluginCount) : base()
        {
            this.Error = null;
            this.PluginUpdatedCount = pluginCount;
        }
    }
}
