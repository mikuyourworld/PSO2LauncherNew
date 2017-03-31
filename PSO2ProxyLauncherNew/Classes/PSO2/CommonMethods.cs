using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    public static class CommonMethods
    {
        public static bool IsPSO2Installed
        {
            get
            {
                string dir = MySettings.PSO2Dir;
                if (string.IsNullOrWhiteSpace(dir))
                    return false;
                else
                    return IsPSO2Folder(dir);
            }
        }

        public static bool IsPSO2RootFolder(string path)
        {
            return IsPSO2Folder(Path.Combine(path, "pso2_bin"));
        }

        public static bool IsPSO2Folder(string path)
        {
            if (File.Exists(Path.Combine(path, "pso2launcher.exe")))
                if (File.Exists(Path.Combine(path, "pso2.exe")))
                    if (Directory.Exists(Path.Combine(path, "data", "win32")))
                        return true;
            return false;
        }

        public static Exception LaunchPSO2(bool waitForExit = true)
        {
            return LaunchPSO2(MySettings.PSO2Dir, waitForExit);
        }
        public static Exception LaunchPSO2(string path, bool waitForExit = true)
        {
            Exception result = null;
            try
            {
                if (LaunchPSO2Ex(path, waitForExit))
                    result = null;
                else
                    result = new FileNotFoundException("PSO2 executable file is not found", path);
            }
            catch (Exception ex) { result = ex; }
            return result;
        }

        public static bool LaunchPSO2Ex(bool waitForExit = true)
        {
            return LaunchPSO2Ex(MySettings.PSO2Dir, waitForExit);
        }

        public static bool LaunchPSO2Ex(string path, bool waitForExit = true)
        {
            string pso2exefullpath = Path.Combine(path, "pso2.exe");
            if (File.Exists(pso2exefullpath))
            {
                PSO2HexStartInfo hexInfo = DefaultValues.RandomStartGameHexCode;
                ProcessStartInfo myProcInfo = new ProcessStartInfo(pso2exefullpath, hexInfo.ImageString);
                myProcInfo.UseShellExecute = false;
                myProcInfo.WorkingDirectory = path;
                var myProcInfo_env = myProcInfo.EnvironmentVariables;
                if (myProcInfo_env.ContainsKey(DefaultValues.EnviromentKey))
                    myProcInfo_env.Add(DefaultValues.EnviromentKey, hexInfo.EnviromentString);
                else if (myProcInfo_env[DefaultValues.EnviromentKey] != hexInfo.EnviromentString)
                    myProcInfo_env[DefaultValues.EnviromentKey] = hexInfo.EnviromentString;

                //if ((Infos.OSVersionInfo.Name.ToLower() != "windows xp") && Classes.MySettings.LaunchPSO2AsAdmin)
                if ((Infos.OSVersionInfo.Name.ToLower() != "windows xp"))
                    myProcInfo.Verb = "runas";

                if (myProcInfo_env.ContainsKey("COMPLUS_NoGuiFromShim"))
                    myProcInfo_env.Remove("COMPLUS_NoGuiFromShim");
                if (myProcInfo_env.ContainsKey("_NO_DEBUG_HEAP"))
                    myProcInfo_env.Remove("_NO_DEBUG_HEAP");
                if (myProcInfo_env.ContainsKey("VS110COMNTOOLS"))
                    myProcInfo_env.Remove("VS110COMNTOOLS");
                if (myProcInfo_env.ContainsKey("VS120COMNTOOLS"))
                    myProcInfo_env.Remove("VS120COMNTOOLS");
                if (myProcInfo_env.ContainsKey("VS140COMNTOOLS"))
                    myProcInfo_env.Remove("VS140COMNTOOLS");
                if (myProcInfo_env.ContainsKey("MSBuildLoadMicrosoftTargetsReadOnly"))
                    myProcInfo_env.Remove("MSBuildLoadMicrosoftTargetsReadOnly");
                if (myProcInfo_env.ContainsKey("VisualStudioDir"))
                    myProcInfo_env.Remove("VisualStudioDir");
                if (myProcInfo_env.ContainsKey("VisualStudioEdition"))
                    myProcInfo_env.Remove("VisualStudioEdition");
                if (myProcInfo_env.ContainsKey("VisualStudioVersion"))
                    myProcInfo_env.Remove("VisualStudioVersion");
                if (myProcInfo_env.ContainsKey("VSLANG"))
                    myProcInfo_env.Remove("VSLANG");
                using (Process myProc = new Process())
                {
                    myProc.StartInfo = myProcInfo;
                    myProc.Start();
                    if (waitForExit)
                        myProc.WaitForExit();
                }
                return true;
            }
            return false;
        }

        public static RunWorkerCompletedEventArgs FixGameGuardError(string ggLocation)
        {
            return FixGameGuardError(false, ggLocation, null);
        }

        public static RunWorkerCompletedEventArgs FixGameGuardError(string ggLocation, Func<int, bool> progress_callback)
        {
            return FixGameGuardError(false, ggLocation, progress_callback);
        }

        public static RunWorkerCompletedEventArgs FixGameGuardError(bool cleanFix, string ggLocation, Func<int, bool> progress_callback)
        {
            RunWorkerCompletedEventArgs result = null;
            using (var webc = Components.WebClientManger.WebClientPool.GetWebClient_PSO2Download(true))
            {
                webc.CacheStorage = Components.CacheStorage.DefaultStorage;
                result = PSO2UpdateManager.RedownloadFile(webc, DefaultValues.Filenames.GameGuardDes + DefaultValues.Web.FakeFileExtension, ggLocation, progress_callback);
                webc.CacheStorage = null;
            }
            if (cleanFix)
            {
                string dir = MySettings.PSO2Dir;
                if (IsPSO2Folder(dir))
                    Directory.Delete(Path.Combine(dir, "GameGuard"), true);
            }
            return result;
        }
    }
}
