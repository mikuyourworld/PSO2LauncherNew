using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Proxy
{
    public interface IPSO2Proxy
    {
        void Install(PSO2ProxyConfiguration config);
        void Uninstall();
    }
}
