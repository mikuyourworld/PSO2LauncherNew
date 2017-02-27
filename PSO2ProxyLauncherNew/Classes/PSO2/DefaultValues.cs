using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    public static class DefaultValues
    {
        private static PSO2HexStartInfo[] StartGameHexCode = {
            new PSO2HexStartInfo("+0x3215c415", "+0x005a9745"),
            new PSO2HexStartInfo("+0x3fdf37dd", "+0x0d90648d"),
            new PSO2HexStartInfo("+0x3fc2094e", "+0x0d8d5a1e")
        };
        private static Random TheRan = new Random();
        public static PSO2HexStartInfo RandomStartGameHexCode
        { get { return StartGameHexCode[TheRan.Next(0, StartGameHexCode.Length - 1)]; } }

        public const string EnviromentKey = "-pso2";

        public static class Registries
        {
            
        }

        public static class Directory
        {
            public static string PSO2Dir { get { return MySettings.PSO2Dir; } }
            public static string PSO2Win32Data { get { return System.IO.Path.Combine(MySettings.PSO2Dir, @"data\win32"); } }
            public const string PSO2Win32DataBackup = "Backup";
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
            public const string MainDownloadLink = "http://download.pso2.jp/patch_prod/patches";
            public const string MainDataDownloadLink = "http://download.pso2.jp/patch_prod/patches/data/win32";
            public const string OldDownloadLink = "http://download.pso2.jp/patch_prod/patches_old";
            public const string OldDataDownloadLink = "http://download.pso2.jp/patch_prod/patches_old/data/win32";
            public const string PrecedeDownloadLink = "http://download.pso2.jp/patch_prod/patches/data/win32";
            public const string FakeFileExtension = ".pat";
        }

        public static class PatchInfo
        {
            public const string file_patchold = "patchlist_old.txt";
            public const string file_patch = "patchlist.txt";
            private static Uri _VersionLink;
            public static Uri VersionLink
            {
                get
                {
                    if (_VersionLink == null)
                        _VersionLink = new Uri(Classes.Infos.CommonMethods.URLConcat(Web.MainDownloadLink, "version.ver"));
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
                    }
                    return _patchlistfiles;
                }
            }
            public class PatchList
            {
                public string BaseURL { get; }
                public Uri PatchListURL { get; }
                public PatchList(string _baseURL)
                {
                    this.BaseURL = _baseURL;
                    this.PatchListURL = new Uri(Classes.Infos.CommonMethods.URLConcat(_baseURL, file_patch));
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
