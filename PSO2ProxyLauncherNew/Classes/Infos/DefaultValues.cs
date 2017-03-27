using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    public static class DefaultValues
    {
        public static class MyServer
        {
            public const string ServerProtocol = "https";
            public const string ServerDomain = "sites.google.com";

            public static string GetWebLink
            {
                get { return ServerProtocol + "://" + ServerDomain; }
            }
            public static class Web
            {
                public const string SelfUpdate_UpdaterUri = "https://sites.google.com/site/a2511346854864321/WebClientLiteUpdater.7z?attredirects=0&d=1";
                public const string SelfUpdate_UpdateUri = "https://sites.google.com/site/pso2guide/PSO2LauncherNew.7z?attredirects=0&d=1";
                public const string SelfUpdate_VersionUri = "https://sites.google.com/site/pso2guide/PSO2LauncherNewVersion.dat?attredirects=0&d=1";
                //public const string TweakerSidePanelLiner = "/launcher/liner.txt";
                public static string GetDownloadLink
                {
                    get { return GetWebLink + "/site/pso2guide"; }
                    //https://sites.google.com/site/pso2proxypri/PSO2Proxy.7z?attredirects=0&d=1
                    //leayal.000webhostapp.com/launcher/files/7za.rar
                }
            }
        }

        public static class MyInfo
        {
            public static class Registries
            {
                public const string GameClientUpdateThrottleCache = "GameClientUpdateThrottleCache";
                public const string MinimizeNetworkUsage = "MinimizeNetworkUsage";
                public const string GameClientUpdateCache = "GameClientUpdateCache";
                public const string GameClientUpdateThreads = "GameClientUpdateThreads";
                public const string LaunchAsAdmin = "LaunchAsAdmin";
                public const string ProxifierPath = "ProxifierPath";
                public const string Language = "Language";
            }

            public static class Directory
            {
                public static string LanguageFolder { get { return Path.Combine(MyApp.AssemblyInfo.DirectoryPath, "lang"); } }
                public static string LogFolder { get { return Path.Combine(MyApp.AssemblyInfo.DirectoryPath, "logs"); } }
                public static string Patches { get { return Path.Combine(MyApp.AssemblyInfo.DirectoryPath, "patches"); } }
                public static class Folders
                {
                    public static string EnglishPatch { get { return Path.Combine(Patches, "english"); } }
                    public static string LargeFilesPatch { get { return Path.Combine(Patches, "largefiles"); } }
                    public static string StoryPatch { get { return Path.Combine(Patches, "story"); } }
                }
                public static string Cache { get { return Path.Combine(MyApp.AssemblyInfo.DirectoryPath, "cache"); } }

            }
            public static class Filename
            {
                public const string PSO2ChecksumList = "PSO2ChecksumList.leaCheck";
                private static string _pso2checksumlistpath = CommonMethods.PathConcat(MyApp.AssemblyInfo.DirectoryPath, PSO2ChecksumList);
                public static string PSO2ChecksumListPath { get { return _pso2checksumlistpath; } }
                public const string ProxifierExecutable = "proxifier.exe";
                public const string ddraw = "ddraw.dll";
                public const string PluginCache = "PluginsCache.json";
                public static class Log
                {
                    public const string PrintOut = "PrintOut.txt";
                }

                public static class SevenZip
                {
                    public const string SevenZipx86 = "7za.dll";
                    public const string SevenZipx64 = "7za64.dll";
                    public const string MD5x86 = "8F8C8662D50A727EB783B4B6101B1FAB";
                    public const string MD5x64 = "D1FCE3939B8F3F105D98F838B659C1B9";
                    public static string SevenZipLibName
                    {
                        get
                        {
                            if (Environment.Is64BitProcess)
                                return SevenZipx64;
                            else
                                return SevenZipx86;
                        }
                    }
                    public static string SevenZipLibPath
                    {
                        get
                        {
                            if (Environment.Is64BitProcess)
                                return Path.Combine(MyApp.AssemblyInfo.DirectoryPath, SevenZipx64);
                            else
                                return Path.Combine(MyApp.AssemblyInfo.DirectoryPath, SevenZipx86);
                        }
                    }
                    public static bool IsValid
                    {
                        get
                        {
                            string path = SevenZipLibPath;
                            if (Environment.Is64BitProcess)
                                return (CommonMethods.FileToMD5Hash(path) == MD5x64);
                            else
                                return (CommonMethods.FileToMD5Hash(path) == MD5x86);
                        }
                    }
                }
            }
        }

        public static class AIDA
        {            
            public static class Web
            {
                public const string UserAgent = "GNT-0000 00 QanT";
            }

            public static class Strings
            {
                public const string EnglishPatchCalled = "English Patch";
                public const string LargeFilesPatchCalled = "LargeFiles Patch";
                public const string StoryPatchCalled = "Story Patch";
            }

            public static class Tweaker
            {
                public static class TransArmThingiesOrWatever
                {
                    public const string ENPatchOverrideURL = "ENPatchOverrideURL";
                    public const string TransAmEXE = "pso2-transam.exe";
                    public const string LargeFilesDB = "lf.stripped.db";
                    public const string StoryDB = "pso2.stripped.zip";
                    //http://arks-layer.com/justice/lf.stripped.zip
                    public const string LargeFilesTransAmDate = "LargeFilesTransAmDate";
                    public const string StoryDate = "StoryDate";
                    public const string VEDA_MagicWord = "Request TRANSAM";
                    public const string VEDA_Password = "3SlvWqLp4vuvLBwwkfhP";
                    public const string VEDA_Filename = "tweaker.bin";
                    public const string paramNodeForOutput = "-h";
                    //-i "OutputBackup/" -h largefiles-10-7-2016 lf.stripped.db "OutputFiles"
                    //-i "OutputBackup/" -h story-eng-10-7-2016 pso2.stripped.db "OutputFiles"
                    public const string paramNodeForBackupOutput = "-i";
                    public const char PathSplitChar = '/';
                    //This English is just place here for nothing ???
                    public static string EnglishBackupFolder { get { return ValidPath(Path.Combine(PSO2.DefaultValues.Directory.PSO2Win32Data, PSO2.DefaultValues.Directory.PSO2Win32DataBackup, PSO2.DefaultValues.Directory.Backup.English)) + "/"; } }
                    //Yes, they do have extra "/" at the end
                    public static string LargeFilesBackupFolder { get { return ValidPath(Path.Combine(PSO2.DefaultValues.Directory.PSO2Win32Data, PSO2.DefaultValues.Directory.PSO2Win32DataBackup, PSO2.DefaultValues.Directory.Backup.LargeFiles)) + "/"; } }
                    //Yes, they do have extra "/" at the end
                    public static string StoryBackupFolder { get { return ValidPath(Path.Combine(PSO2.DefaultValues.Directory.PSO2Win32Data, PSO2.DefaultValues.Directory.PSO2Win32DataBackup, PSO2.DefaultValues.Directory.Backup.Story)) + "/"; } }
                    public static string ValidPath(string source) { return source.Replace(Path.DirectorySeparatorChar, PathSplitChar); }
                }
                public static class Registries
                {
                    public const string PSO2Dir = "PSO2Dir";
                    public const string PSO2RemoteVersion = "PSO2RemoteVersion";
                    public const string ENPatchVersion = "ENPatchVersion";
                    public const string LargeFilesVersion = "LargeFilesVersion";
                    public const string StoryPatchVersion = "StoryPatchVersion";
                    public const string NoPatchString = "Not Installed";
                }
            }
        }
    }
}
