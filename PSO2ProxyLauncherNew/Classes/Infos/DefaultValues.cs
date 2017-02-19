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
                //public const string TweakerSidePanelLiner = "/launcher/liner.txt";
                public static string GetDownloadLink
                {
                    get { return GetWebLink + "/site/pso2proxypri"; }
                    //https://sites.google.com/site/pso2proxypri/PSO2Proxy.7z?attredirects=0&d=1
                    //leayal.000webhostapp.com/launcher/files/7za.rar
                }
            }
        }

        public static class MyInfo
        {
            public static class Registries
            {
                public const string LaunchPSO2AsAdmin = "LaunchPSO2AsAdmin";
                public const string Language = "Language";
            }

            public static class Directory
            {
                public static string LanguageFolder { get { return Path.Combine(ApplicationInfo.ApplicationDirectory, "lang"); } }
                public static string LogFolder { get { return Path.Combine(ApplicationInfo.ApplicationDirectory, "logs"); } }
                public static string Patches { get { return Path.Combine(ApplicationInfo.ApplicationDirectory, "patches"); } }
                public static class Folders
                {
                    public static string EnglishPatch { get { return Path.Combine(Patches, "english"); } }
                    public static string LargeFilesPatch { get { return Path.Combine(Patches, "largefiles"); } }
                    public static string StoryPatch { get { return Path.Combine(Patches, "story"); } }
                }
            }
            public static class Filename
            {
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
                                return Path.Combine(ApplicationInfo.ApplicationDirectory, SevenZipx64);
                            else
                                return Path.Combine(ApplicationInfo.ApplicationDirectory, SevenZipx86);
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

            public static class Tweaker
            {
                public static class TransArmThingiesOrWatever
                {
                    public const string VEDA_Thingie = "2a439ffb17d16ef2d7188158de55d58b";
                    public const string VEDA_Filename = "tweaker.bin";
                    public const string paramNodeForOutput = "-h";
                    //-i "OutputBackup/" -h largefiles-10-7-2016 lf.stripped.db "OutputFiles"
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
                    public const string ENPatchVersion = "ENPatchVersion";
                    public const string LargeFilesVersion = "LargeFilesVersion";
                    public const string StoryPatchVersion = "StoryPatchVersion";
                    public const string NoPatchString = "Not Installed";
                }
            }
        }
    }
}
