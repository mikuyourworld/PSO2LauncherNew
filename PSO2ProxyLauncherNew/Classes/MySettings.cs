﻿using PSO2ProxyLauncherNew.Classes.Infos;
using System;

namespace PSO2ProxyLauncherNew.Classes
{
    public static class MySettings
    {
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
            set { ConfigManager.Instance.SetInt(DefaultValues.MyInfo.Registries.GameClientUpdateThreads, value); GameClientUpdateThreadsChanged?.Invoke(null, System.EventArgs.Empty); }
        }
        public static event EventHandler GameClientUpdateThreadsChanged;

        public static bool GameClientUpdateCache
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.GameClientUpdateCache, true); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.GameClientUpdateCache, value); }
        }

        public static bool MinimizeNetworkUsage
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.MinimizeNetworkUsage, true); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.MinimizeNetworkUsage, value); MinimizeNetworkUsageChanged?.Invoke(null, System.EventArgs.Empty); }
        }
        public static event EventHandler MinimizeNetworkUsageChanged;

        public static bool LaunchAsAdmin
        {
            get { return ConfigManager.Instance.GetBool(DefaultValues.MyInfo.Registries.LaunchAsAdmin, false); }
            set { ConfigManager.Instance.SetBool(DefaultValues.MyInfo.Registries.LaunchAsAdmin, value); }
        }
    }
}
