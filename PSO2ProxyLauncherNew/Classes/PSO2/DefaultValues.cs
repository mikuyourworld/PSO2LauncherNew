using System;
using System.Collections.Generic;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    public static class DefaultValues
    {
        private static PSO2HexStartInfo[] StartGameHexCode = {
            new PSO2HexStartInfo("+0x3215c415", "+0x005a9745"),
            new PSO2HexStartInfo("+0x3fdf37dd", "+0x0d90648d"),
            new PSO2HexStartInfo("+0x3fc2094e", "+0x0d8d5a1e"),
            new PSO2HexStartInfo("+0x33aca2b9", "+0x01e3f1e9")
        };
        private static Random TheRan = new Random();
        public static PSO2HexStartInfo RandomStartGameHexCode
        {
            get
            {
                if (StartGameHexCode.Length > 0)
                {
                    if (StartGameHexCode.Length == 1)
                        return StartGameHexCode[0];
                    else
                        return StartGameHexCode[TheRan.Next(0, StartGameHexCode.Length - 1)];
                }
                else
                    return new PSO2HexStartInfo("+0x33aca2b9", "+0x01e3f1e9");
            }
        }

        public const string EnviromentKey = "-pso2";

        public static class Filenames
        {
            public const string GameGuardDes = "GameGuard.des";
        }

        public static class Directory
        {
            private static string cache_DocumentWorkSpace, cache_userSettingPath;
            public static string DocumentWorkSpace
            {
                get
                {
                    if (string.IsNullOrEmpty(cache_DocumentWorkSpace))
                        cache_DocumentWorkSpace = System.IO.Path.Combine(Infos.ApplicationInfo.MyDocument, "SEGA", "PHANTASYSTARONLINE2");
                    return cache_DocumentWorkSpace;
                }
            }
            public static string UserSettingPath
            {
                get
                {
                    if (string.IsNullOrEmpty(cache_userSettingPath))
                        cache_userSettingPath = System.IO.Path.Combine(DocumentWorkSpace, "user.pso2");
                    return cache_userSettingPath;
                }
            }
            public static string PSO2Dir { get { return MySettings.PSO2Dir; } }
            public static string PSO2Win32Data { get { return System.IO.Path.Combine(MySettings.PSO2Dir, @"data\win32"); } }
            public const string PSO2Win32DataBackup = "Backup";
            public const string RaiserPatchFolderName = "patch";
            public static string RaiserPatchFolder { get { return System.IO.Path.Combine(MySettings.PSO2Dir, RaiserPatchFolderName); } }
            public const string PrecedeFolderName = "_precede";
            public static string PrecedeFolder { get { return System.IO.Path.Combine(MySettings.PSO2Dir, PrecedeFolderName); } }
            public const string PluginsFolderName = "Plugins";
            public static string PSO2Plugins { get { return System.IO.Path.Combine(MySettings.PSO2Dir, PluginsFolderName); } }
            public static string PSO2PluginsDisabled { get { return System.IO.Path.Combine(PSO2Plugins, "disabled"); } }
            public static class Backup
            {
                public const string English = "English Patch";
                public const string LargeFiles = "Large Files";
                public const string Story = "Story Patch";
            }
        }

        public static class Web
        {
            public const string UserAgent = "AQUA_HTTP";
            public const string DownloadHost = "download.pso2.jp";
            public const string MainDownloadLink = "http://" + DownloadHost + "/patch_prod/patches";
            public const string MainDataDownloadLink = "http://" + DownloadHost + "/patch_prod/patches/data/win32";
            public const string OldDownloadLink = "http://" + DownloadHost + "/patch_prod/patches_old";
            public const string OldDataDownloadLink = "http://" + DownloadHost + "/patch_prod/patches_old/data/win32";
            public const string PrecedeDownloadLink = "http://" + DownloadHost + "/patch_prod/patches_precede";
            public const string FakeFileExtension = ".pat";
        }

        public static class PatchInfo
        {
            public const string file_patchold = "patchlist_old.txt";
            public const string file_patch = "patchlist.txt";
            public const string file_launcher = "launcherlist.txt";
            private static Uri _VersionLink;
            public static Uri VersionLink
            {
                get
                {
                    if (_VersionLink == null)
                        _VersionLink = new Uri(Leayal.UriHelper.URLConcat(Web.MainDownloadLink, "version.ver"));
                    return _VersionLink;
                }
            }
            private static Dictionary<string, PatchList> _patchlistfiles;
            public static Dictionary<string, PatchList> PatchListFiles
            {
                get
                {
                    if (_patchlistfiles == null)
                    {
                        _patchlistfiles = new Dictionary<string, PatchList>();
                        _patchlistfiles.Add(file_patchold, new PatchList(Web.OldDownloadLink));
                        _patchlistfiles.Add(file_patch, new PatchList(Web.MainDownloadLink));
                        _patchlistfiles.Add(file_launcher, new PatchList(Web.MainDownloadLink, file_launcher));
                    }
                    return _patchlistfiles;
                }
            }

            public const string file_precedelist = "patchlist{0}.txt";
            private static Uri _PrecedeVersionLink;
            public static Uri PrecedeVersionLink
            {
                get
                {
                    if (_PrecedeVersionLink == null)
                        _PrecedeVersionLink = new Uri(Leayal.UriHelper.URLConcat(Web.PrecedeDownloadLink, "version.ver"));
                    return _PrecedeVersionLink;
                }
            }
            /*public static Dictionary<string, PatchList> PrecedeListFiles
            {
                get
                {
                    if (_patchlistfiles == null)
                    {
                        _patchlistfiles = new Dictionary<string, PatchList>();
                        //http://download.pso2.jp/patch_prod/patches_precede/patchlist0.txt
                        _patchlistfiles.Add(file_launcher, new PatchList(Web.PrecedeDownloadLink, file_launcher));
                    }
                    return _patchlistfiles;
                }
            }//*/
            public class PatchList
            {
                public string BaseURL { get; }
                public Uri PatchListURL { get; }
                public PatchList(string _baseURL) : this(_baseURL, file_patch) { }
                public PatchList(string _baseURL, string filename)
                {
                    this.BaseURL = _baseURL;
                    this.PatchListURL = new Uri(Leayal.UriHelper.URLConcat(_baseURL, filename));
                }
            }
        }
    }

    public class PSO2HexStartInfo
    {
        public string ImageString { get; }
        public string EnviromentString { get; }
        public PSO2HexStartInfo(string image, string enviroment)
        {
            this.ImageString = image;
            this.EnviromentString = enviroment;
        }
    }
}
