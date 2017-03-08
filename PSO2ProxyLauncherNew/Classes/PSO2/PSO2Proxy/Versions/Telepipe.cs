namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Proxy
{
    public partial class Versions
    {
        public static Versions VersionTelepipe = new Telepipe();
        public class Telepipe : Versions, IPSO2Proxy
        {
            public override string ToString() { return "Telepipe"; }

            private static string[] IncompatiblePluginList = { "PSO2Proxy.dll", "PSO2TitleTranslator.dll", "translator.dll" };
            private static string[] RequiredPluginList = { "TelepipeProxy.dll" };

            public void Install(PSO2ProxyConfiguration config)
            {
                PSO2Proxy.Install(RequiredPluginList, IncompatiblePluginList, config);
            }

            public void Uninstall()
            {
                PSO2Proxy.Uninstall(RequiredPluginList);
            }
        }
    }
}
