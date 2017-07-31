using PSO2ProxyLauncherNew.Classes.PSO2.PSO2Plugin;
using System;
using System.Collections.Generic;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class CheckForPluginCompletedEventArgs : EventArgs
    {
        public Exception Error { get; }
        public List<PSO2Plugin> PluginUpdatedList { get; }

        public CheckForPluginCompletedEventArgs(Exception ex) : base()
        {
            this.Error = ex;
            this.PluginUpdatedList = null;
        }

        public CheckForPluginCompletedEventArgs(List<PSO2Plugin> pluginCount) : base()
        {
            this.Error = null;
            this.PluginUpdatedList = pluginCount;
        }

        public CheckForPluginCompletedEventArgs() : base()
        {
            this.Error = null;
            this.PluginUpdatedList = null;
        }
    }
}
