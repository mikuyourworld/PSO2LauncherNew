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
                        AIDA.PSO2Dir = result;
                        return System.IO.Path.GetFullPath(result);
                    }
                    else
                        return string.Empty;
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

        public static int GameClientUpdateThreads
        {
            get { return System.Math.Min(ConfigManager.Instance.GetInt(DefaultValues.MyInfo.Registries.GameClientUpdateThreads, 1), Infos.CommonMethods.MaxThreadsCount); }
            set { ConfigManager.Instance.SetInt(DefaultValues.MyInfo.Registries.GameClientUpdateThreads, value); GameClientUpdateThreadsChanged?.Invoke(null, new IntEventArgs(value)); }
        }
        public static event EventHandler<IntEventArgs> GameClientUpdateThreadsChanged;

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
    }
}
