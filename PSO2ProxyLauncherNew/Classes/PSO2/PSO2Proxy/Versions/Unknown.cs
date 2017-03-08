using System;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Proxy
{
    public partial class Versions
    {
        public static Versions VersionUnknown = new Unknown();
        public class Unknown : Versions, IPSO2Proxy
        {
            public override string ToString() { return "Unknown"; }

            private static string[] IncompatiblePluginList = { };
            private static string[] RequiredPluginList = { };

            public void Install(PSO2ProxyConfiguration config)
            {
                throw new NotImplementedException(LanguageManager.GetMessageText("PSO2Proxy_UnknownVersionInstall", "This proxy version is not supported"));
            }

            public void Uninstall()
            {
                //throw new NotImplementedException(LanguageManager.GetMessageText("PSO2Proxy_UnknownVersionInstall", "This proxy version is not supported"));
            }
        }
    }
}
