using PSO2ProxyLauncherNew.Classes.Infos;
using System;
using PSO2ProxyLauncherNew.Classes.Events;
using System.Drawing;

namespace PSO2ProxyLauncherNew.Classes
{
    public static class MySettings
    {
        private static readonly char[] phayonly = { ',' };
        public static class Patches
        {
            public static string EnglishVersion
            {
                get
                {
                    string result = AIDA.LocalPatches.EnglishVersion;
                    if (string.IsNullOrEmpty(result))
                    {
                        result = ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.ENPatchVersion, DefaultValues.AIDA.Tweaker.Registries.NoPatchString);
                        if (!string.IsNullOrEmpty(result))
                            AIDA.LocalPatches.EnglishVersion = result;
                        return result;
                    }
                    else
                        return result;
                }
                set
                {
                    ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.ENPatchVersion, value);
                    AIDA.LocalPatches.EnglishVersion = value;
                }
            }
            public static string LargeFilesVersion
            {
                get
                {
                    string result = AIDA.LocalPatches.LargeFilesVersion;
                    if (string.IsNullOrEmpty(result))
                    {
                        result = ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.LargeFilesVersion, DefaultValues.AIDA.Tweaker.Registries.NoPatchString);
                        if (!string.IsNullOrEmpty(result))
                            AIDA.LocalPatches.LargeFilesVersion = result;
                        return result;
                    }
                    else
                        return result;
                }
                set
                {
                    ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.LargeFilesVersion, value);
                    AIDA.LocalPatches.LargeFilesVersion = value;
                }
            }
            public static string StoryVersion
            {
                get
                {
                    string result = AIDA.LocalPatches.StoryVersion;
                    if (string.IsNullOrEmpty(result))
                    {
                        result = ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.StoryPatchVersion, DefaultValues.AIDA.Tweaker.Registries.NoPatchString);
                        if (!string.IsNullOrEmpty(result))
                            AIDA.LocalPatches.StoryVersion = result;
                        return result;
                    }
                    else
                        return result;
                }
                set
                {
                    ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.StoryPatchVersion, value);
                    AIDA.LocalPatches.StoryVersion = value;
                }
            }

            public static string RaiserVersion
            {
                get
                {
                    string result = AIDA.LocalPatches.RaiserVersion;
                    if (string.IsNullOrEmpty(result))
                    {
                        result = ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.RaiserPatchVersion, DefaultValues.AIDA.Tweaker.Registries.NonePatchString);
                        if (!string.IsNullOrEmpty(result))
                            AIDA.LocalPatches.RaiserVersion = result;
                        return result;
                    }
                    else
                        return result;
                }
                set
                {
                    ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.RaiserPatchVersion, value);
                    AIDA.LocalPatches.RaiserVersion = value;
                }
            }
            public static bool RaiserEnabled
            {
                get
                {
                    string result = AIDA.LocalPatches.RaiserEnabled;
                    if (string.IsNullOrEmpty(result))
                    {
                        var myBoolresult = ConfigManager.Instance.GetBool(DefaultValues.AIDA.Tweaker.Registries.RaiserPatchEnabled, false);
                        AIDA.LocalPatches.RaiserEnabled = myBoolresult.ToAIDASettings();
                        return myBoolresult;
                    }
                    else
                        return result.BoolAIDASettings(false);
                }
                set
                {
                    ConfigManager.Instance.SetBool(DefaultValues.AIDA.Tweaker.Registries.RaiserPatchEnabled, value);
                    AIDA.LocalPatches.RaiserEnabled = value.ToAIDASettings();
                }
            }

            public static Components.Patches.RaiserLanguageName PatchLanguage
            {
                get
                {
                    string result = AIDA.LocalPatches.PatchLanguage;
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.PatchLanguage, string.Empty);
                        if (string.IsNullOrWhiteSpace(result))
                            return Components.Patches.RaiserLanguageName.English;
                        else
                        {
                            if (Enum.TryParse<Components.Patches.RaiserLanguageCode>(result, true, out var value))
                            {
                                AIDA.LocalPatches.PatchLanguage = result;
                                return (Components.Patches.RaiserLanguageName)((int)value);
                            }
                            else
                                return Components.Patches.RaiserLanguageName.English;
                        }
                    }
                    else
                    {
                        if (Enum.TryParse<Components.Patches.RaiserLanguageCode>(result, true, out var value))
                        {
                            return (Components.Patches.RaiserLanguageName)((int)value);
                        }
                        else
                            return Components.Patches.RaiserLanguageName.English;
                    }
                }
                set
                {
                    string theValueString = Enum.GetName(typeof(Components.Patches.RaiserLanguageCode), (Components.Patches.RaiserLanguageCode)((int)value)).ToUpper();
                    ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PatchLanguage, theValueString);
                    AIDA.LocalPatches.PatchLanguage = theValueString;
                }
            }
        }
        public static string PSO2Dir
        {
            get
            {
                //AIDA.PSO2Dir
                string result = AIDA.PSO2Dir;
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2Dir, string.Empty);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        result = System.IO.Path.GetFullPath(result);
                        AIDA.PSO2Dir = result;
                        return result;
                    }
                    else
                    {
                        result = Leayal.AppInfo.AssemblyInfo.DirectoryPath;
                        if (PSO2.CommonMethods.IsPSO2Folder(result))
                        {
                            PSO2Dir = result;
                            return result;
                        }
                        else
                        {
                            result = Leayal.IO.PathHelper.Combine(Leayal.AppInfo.AssemblyInfo.DirectoryPath, "pso2_bin");
                            if (PSO2.CommonMethods.IsPSO2Folder(result))
                            {
                                PSO2Dir = result;
                                return result;
                            }
                            else
                                return string.Empty;
                        }
                    }
                }
                else
                {
                    result = System.IO.Path.GetFullPath(result);
                    if (string.IsNullOrWhiteSpace(ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2Dir, string.Empty)))
                        ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2Dir, result);
                    return result;
                }
            }
            set
            {
                ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2Dir, value);
                AIDA.PSO2Dir = value;
            }
        }

        public static string PSO2Version
        {
            get
            {
                string result = PSO2.Settings.VersionString;
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = AIDA.PSO2RemoteVersion;
                    if (string.IsNullOrEmpty(result))
                    {
                        result = ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2RemoteVersion, string.Empty);
                        if (!string.IsNullOrEmpty(result))
                        {
                            PSO2.Settings.VersionString = result;
                            AIDA.PSO2RemoteVersion = result;
                        }
                        return result;
                    }
                    else
                    {
                        PSO2.Settings.VersionString = result;
                        ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2RemoteVersion, result);
                        return result;
                    }
                }
                else
                    return result;
            }
            set
            {
                ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2RemoteVersion, value);
                AIDA.PSO2RemoteVersion = value;
                PSO2.Settings.VersionString = value;
            }
        }

        public static PSO2.PrepatchManager.PrepatchVersion PSO2PrecedeVersion
        {
            get
            {
                string result = PSO2.Settings.PrecedeVersionString;
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = ConfigManager.Instance.GetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2PrecedeVersion, string.Empty);

                    if (!string.IsNullOrEmpty(result))
                    {
                        if (PSO2.PrepatchManager.PrepatchVersion.TryParse(result, out var sumthing))
                        {
                            PSO2.Settings.PrecedeVersionString = result;
                            return sumthing;
                        }
                        else
                            return new PSO2.PrepatchManager.PrepatchVersion();
                    }
                    else
                        return new PSO2.PrepatchManager.PrepatchVersion();
                }
                else
                {
                    if (PSO2.PrepatchManager.PrepatchVersion.TryParse(result, out var sumthing))
                        return sumthing;
                    else
                        return new PSO2.PrepatchManager.PrepatchVersion();
                }
            }
            set
            {
                ConfigManager.Instance.SetSetting(DefaultValues.AIDA.Tweaker.Registries.PSO2PrecedeVersion, value.ToString());
                PSO2.Settings.PrecedeVersionString = value.ToString();
            }
        }

        public static Uri ProxyJSONURL
        {
            get
            {
                string re = AIDA.ProxyJSONURL;
                Uri result = null;
                if (!Uri.TryCreate(re, UriKind.Absolute, out result))
                    return null;
                return result;
            }
            set
            {
                if (value != null)
                    AIDA.ProxyJSONURL = value.AbsolutePath;
                else
                    AIDA.ProxyJSONURL = string.Empty;
            }
        }

        public static string Language
        {
            get { return ConfigManager.Instance.GetSetting(DefaultValues.MyInfo.Registries.Language, "english"); }
            set { ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.Language, value); }
        }

        public static string ProxifierPath
        {
            get { return ConfigManager.Instance.GetSetting(DefaultValues.MyInfo.Registries.ProxifierPath, string.Empty); }
            set { ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.ProxifierPath, value); }
        }

        public static bool GameClientUpdateCache
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.GameClientUpdateCache, true); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.GameClientUpdateCache, value); }
        }

        public static int GameClientUpdateThrottleCache
        {
            get { return Math.Min(ConfigManager.Instance.GetInt(DefaultValues.MyInfo.Registries.GameClientUpdateThrottleCache, 0), 4); }
            set { ConfigManager.Instance.SetInt(DefaultValues.MyInfo.Registries.GameClientUpdateThrottleCache, value); GameClientUpdateThrottleCacheChanged?.Invoke(null, new IntEventArgs(value)); }
        }
        public static event EventHandler<IntEventArgs> GameClientUpdateThrottleCacheChanged;

        public static bool MinimizeNetworkUsage
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.MinimizeNetworkUsage, true); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.MinimizeNetworkUsage, value); MinimizeNetworkUsageChanged?.Invoke(null, new BooleanEventArgs(value)); }
        }
        public static event EventHandler<BooleanEventArgs> MinimizeNetworkUsageChanged;

        public static bool LaunchAsAdmin
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.LaunchAsAdmin, false); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.LaunchAsAdmin, value); }
        }

        public static int BottomSplitterRatio
        {
            get { return ConfigManager.Instance.GetInt(DefaultValues.MyInfo.Registries.BottomSplitterRatio, 50); }
            set { ConfigManager.Instance.SetInt(DefaultValues.MyInfo.Registries.BottomSplitterRatio, value); }
        }

        public static int MainMenuSplitter
        {
            get { return ConfigManager.Instance.GetInt(DefaultValues.MyInfo.Registries.MainMenuSplitter, 50); }
            set { ConfigManager.Instance.SetInt(DefaultValues.MyInfo.Registries.MainMenuSplitter, value); }
        }

        public static int LauncherSizeScale
        {
            get { return ConfigManager.Instance.GetInt(DefaultValues.MyInfo.Registries.LauncherSizeScale, Convert.ToInt32(Leayal.Forms.FormWrapper.ScalingFactor * 100)); }
            set { ConfigManager.Instance.SetInt(DefaultValues.MyInfo.Registries.LauncherSizeScale, value); }
        }

        public static string LauncherBGlocation
        {
            get { return ConfigManager.Instance.GetSetting(DefaultValues.MyInfo.Registries.LauncherBGlocation, string.Empty); }
            set { ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.LauncherBGlocation, value); }
        }

        public static Color? LauncherBGColor
        {
            get
            {
                string colorstring =  ConfigManager.Instance.GetSetting(DefaultValues.MyInfo.Registries.LauncherBGColor, string.Empty);
                if (!string.IsNullOrWhiteSpace(colorstring) && (colorstring.IndexOf(",") > -1))
                {
                    string[] dundun = colorstring.Split(phayonly, 3, StringSplitOptions.RemoveEmptyEntries);
                    if (dundun.Length > 2)
                    {
                        int red, green, blue;
                        if (Leayal.NumberHelper.TryParse(dundun[0].Trim(), out red) && Leayal.NumberHelper.TryParse(dundun[1].Trim(), out green) && Leayal.NumberHelper.TryParse(dundun[2].Trim(), out blue))
                        {
                            if (red < 256 && green < 256 && blue < 256)
                            {
                                return Color.FromArgb(red, green, blue);
                            }
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.LauncherBGColor, string.Format("{0}, {1}, {2}", value.Value.R, value.Value.G, value.Value.B));
                else
                    ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.LauncherBGColor, "17, 17, 17");
            }
        }

        public static Color? LauncherForeColor
        {
            get
            {
                string colorstring = ConfigManager.Instance.GetSetting(DefaultValues.MyInfo.Registries.LauncherForeColor, string.Empty);
                if (!string.IsNullOrWhiteSpace(colorstring) && (colorstring.IndexOf(",") > -1))
                {
                    string[] dundun = colorstring.Split(phayonly, 3, StringSplitOptions.RemoveEmptyEntries);
                    if (dundun.Length > 2)
                    {
                        int red, green, blue;
                        if (Leayal.NumberHelper.TryParse(dundun[0].Trim(), out red) && Leayal.NumberHelper.TryParse(dundun[1].Trim(), out green) && Leayal.NumberHelper.TryParse(dundun[2].Trim(), out blue))
                        {
                            if (red < 256 && green < 256 && blue < 256)
                            {
                                return Color.FromArgb(red, green, blue);
                            }
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.LauncherForeColor, string.Format("{0}, {1}, {2}", value.Value.R, value.Value.G, value.Value.B));
                else
                    ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.LauncherForeColor, "254, 254, 254");
            }
        }

        public static bool HighlightTexts
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.HighlightTexts, false); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.HighlightTexts, value); }
        }

        public static System.Windows.Forms.ImageLayout LauncherBGImgLayout
        {
            get
            {
                string setting = ConfigManager.Instance.GetSetting(DefaultValues.MyInfo.Registries.LauncherBGImgLayout, System.Windows.Forms.ImageLayout.Zoom.ToString());
                System.Windows.Forms.ImageLayout result = System.Windows.Forms.ImageLayout.Zoom;
                if (!Enum.TryParse<System.Windows.Forms.ImageLayout>(setting, out result))
                    return System.Windows.Forms.ImageLayout.Zoom;
                return result;
            }
            set { ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.LauncherBGImgLayout, value.ToString()); }
        }

        public static bool ReshadeSupport
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.ReshadeSupport, false); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.ReshadeSupport, value); }
        }

        public static bool UseExternalLauncher
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.UseExternalLauncher, false); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.UseExternalLauncher, value); }
        }

        public static bool ExternalLauncherUseStrictMode
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.ExternalLauncherUseStrictMode, false); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.ExternalLauncherUseStrictMode, value); }
        }

        public static string ExternalLauncherEXE
        {
            get { return ConfigManager.Instance.GetSetting(DefaultValues.MyInfo.Registries.ExternalLauncherEXE, string.Empty); }
            set { ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.ExternalLauncherEXE, value); }
        }

        public static string ExternalLauncherArgs
        {
            get { return ConfigManager.Instance.GetSetting(DefaultValues.MyInfo.Registries.ExternalLauncherArgs, string.Empty); }
            set { ConfigManager.Instance.SetSetting(DefaultValues.MyInfo.Registries.ExternalLauncherArgs, value); }
        }

        public static bool SteamSwitch => Program.launchedbysteam;

        public static bool SteamMode
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.SteamMode, false); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.SteamMode, value); }
        }

        public static bool CheckForPrepatch
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.CheckForPrepatch, true); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.CheckForPrepatch, value); }
        }
    }
}
