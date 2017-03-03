using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class PSO2PluginStatusChanged : EventArgs
    {
        public Exception Error { get; }
        public PSO2.PSO2Plugin.PSO2Plugin Plugin { get; }
        public PSO2PluginStatusChanged(Exception ex, PSO2.PSO2Plugin.PSO2Plugin _plugin) : base()
        {
            this.Error = ex;
            this.Plugin = _plugin;
        }

        public PSO2PluginStatusChanged(PSO2.PSO2Plugin.PSO2Plugin _plugin) : this(null, _plugin) { }

        public override string ToString()
        {
            if (this.Plugin == null)
                return string.Empty;
            else
                return this.Plugin.PluginID;
        }
    }
}
