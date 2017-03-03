using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class ProxyInstalledEventArgs : EventArgs
    {
        public PSO2.PSO2Proxy.PSO2ProxyConfiguration Proxy { get; }
        public ProxyInstalledEventArgs(PSO2.PSO2Proxy.PSO2ProxyConfiguration _proxy) : base()
        {
            this.Proxy = _proxy;
        }
    }

    class ProxyUninstalledEventArgs : EventArgs
    {
        public ProxyUninstalledEventArgs() : base() { }
    }
}
