namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Proxy
{
    partial class Versions
    {
        public static Versions VersionTelepipe = new Telepipe();
        public class Telepipe : Versions
        {
            public override string ToString() { return "Telepipe Proxy"; }

            private static string[] IncompatiblePluginList = { "PSO2TitleTranslator.dll", "translator.dll" };
            private static string[] RequiredPluginList = { "TelepipeProxy.dll" };

            public static void Install(PSO2ProxyConfiguration config)
            {
                PSO2Proxy.Install(RequiredPluginList, IncompatiblePluginList, config);
            }

            public static void Uninstall()
            {
                PSO2Proxy.Uninstall(RequiredPluginList);
            }
        }
    }
}
