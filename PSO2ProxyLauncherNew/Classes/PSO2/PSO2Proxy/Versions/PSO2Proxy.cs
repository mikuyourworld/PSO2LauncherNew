namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Proxy
{
    partial class Versions
    {
        public static Versions VersionPSO2Proxy = new PSO2Proxy();
        public class PSO2Proxy : Versions
        {
            const string publickeyFilename = "publickey.blob";
            const string proxyConfigFile = "proxy.txt";
            public override string ToString() { return "PSO2Proxy Proxy"; }

            private static string[] IncompatiblePluginList = { };
            private static string[] RequiredPluginList = { "TelepipeProxy.dll" };

            public static void Install(PSO2ProxyConfiguration config)
            {
                Install(RequiredPluginList, IncompatiblePluginList, config);
            }

            internal static void Install(string[] requiredPlugins, string[] incompatiblePlugins, PSO2ProxyConfiguration config)
            {
                ModifyHostFile();
                Components.WebClientManger.WebClientPool.GetWebClient(config.PublickeyURL).DownloadFile(config.PublickeyURL, Infos.CommonMethods.PathConcat(MySettings.PSO2Dir, publickeyFilename));
                string _ipAdressString = config.Host;
                System.Net.IPAddress _ipAdress;
                if (Infos.CommonMethods.GetResolvedConnecionIPAddress(config.Host, out _ipAdress))
                    _ipAdressString = _ipAdress.ToString();
                System.IO.File.WriteAllText(Infos.CommonMethods.PathConcat(MySettings.PSO2Dir, proxyConfigFile), _ipAdressString);
                PSO2Plugin.PSO2Plugin plugin;
                if (incompatiblePlugins != null && incompatiblePlugins.Length > 0)
                    for (int i = 0; i < incompatiblePlugins.Length; i++)
                    {
                        plugin = PSO2Plugin.PSO2PluginManager.Instance[incompatiblePlugins[i]];
                        if (plugin != null)
                            plugin.Enabled = false;
                    }
                if (requiredPlugins != null && requiredPlugins.Length > 0)
                    for (int i = 0; i < requiredPlugins.Length; i++)
                    {
                        plugin = PSO2Plugin.PSO2PluginManager.Instance[requiredPlugins[i]];
                        if (plugin != null)
                            plugin.Enabled = true;
                    }
                plugin = null;
            }

            public static void Uninstall()
            {
                Uninstall(RequiredPluginList);
            }

            internal static void Uninstall(string[] requiredPlugins)
            {
                //ModifyHostFile();
                System.IO.File.Delete(Infos.CommonMethods.PathConcat(MySettings.PSO2Dir, publickeyFilename));
                System.IO.File.Delete(Infos.CommonMethods.PathConcat(MySettings.PSO2Dir, proxyConfigFile));
                PSO2Plugin.PSO2Plugin plugin;
                if (requiredPlugins != null && requiredPlugins.Length > 0)
                    for (int i = 0; i < requiredPlugins.Length; i++)
                    {
                        plugin = PSO2Plugin.PSO2PluginManager.Instance[requiredPlugins[i]];
                        if (plugin != null)
                            plugin.Enabled = false;
                    }
                plugin = null;
            }

            private static void ModifyHostFile()
            {
                ModifyHostFile(null);
            }

            private static void ModifyHostFile(PSO2ProxyConfiguration config)
            {
                bool needWrite = false;
                bool installMode = config is PSO2ProxyConfiguration;
                string _ipAdressString = string.Empty;
                System.Net.IPAddress _ipAdress;
                if (installMode)
                {
                    if (Infos.CommonMethods.GetResolvedConnecionIPAddress(config.Host, out _ipAdress))
                        _ipAdressString = _ipAdress.ToString();
                    else
                        _ipAdressString = config.Host;
                }
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                string hostFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "drivers", "etc", "hosts");

                if (System.IO.File.Exists(hostFilePath))
                {
                    string linebuffer;
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(hostFilePath))
                        while (!sr.EndOfStream)
                        {
                            linebuffer = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(linebuffer))
                            {
                                if (linebuffer.IndexOf("pso2gs.net") == -1)
                                {
                                    sb.AppendLine(linebuffer);
                                }
                                else
                                    needWrite = true;
                            }
                            else
                                sb.AppendLine(linebuffer);
                        }
                }
                if (needWrite)
                    System.IO.File.WriteAllText(hostFilePath, sb.ToString());
                sb.Clear();
                //config
            }
        }

    }
}
