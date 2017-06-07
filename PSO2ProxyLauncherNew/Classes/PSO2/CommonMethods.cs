using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Security.AccessControl;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    public static class CommonMethods
    {
        public const string CensorFilename = "ffbff2ac5b7a7948961212cefd4d402c";
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

        public static bool IsReshadeExists()
        {
            return IsReshadeExists(MySettings.PSO2Dir);
        }

        public static bool IsReshadeExists(string pso2path)
        {
            if (File.Exists(Path.Combine(pso2path, "ReShade32.dll")))
                if (File.Exists(Path.Combine(pso2path, "ReShade.fx")))
                    return true;
            return false;
        }

        public static bool ActivateReshade()
        {
            return ActivateReshade(MySettings.PSO2Dir);
        }

        public static bool ActivateReshade(string pso2path)
        {
            try
            {
                File.Copy(Path.Combine(pso2path, "ReShade32.dll"), Path.Combine(pso2path, "ddraw.dll"), true);
                return true;
            }
            catch (FileNotFoundException)
            { return false; }
        }

        public static void DeactivateReshade()
        {
            DeactivateReshade(MySettings.PSO2Dir);
        }

        public static void DeactivateReshade(string pso2path)
        {
            File.Delete(Path.Combine(pso2path, "ddraw.dll"));
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
                if (!myProcInfo_env.ContainsKey(DefaultValues.EnviromentKey))
                    myProcInfo_env.Add(DefaultValues.EnviromentKey, hexInfo.EnviromentString);
                else if (myProcInfo_env[DefaultValues.EnviromentKey] != hexInfo.EnviromentString)
                    myProcInfo_env[DefaultValues.EnviromentKey] = hexInfo.EnviromentString;

                //if ((Infos.OSVersionInfo.Name.ToLower() != "windows xp") && Classes.MySettings.LaunchPSO2AsAdmin)
                if (Leayal.OSVersionInfo.IsVistaAndUp)
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

        public static RunWorkerCompletedEventArgs FixFilesPermission()
        {
            return FixFilesPermission(MySettings.PSO2Dir, false);
        }

        public static RunWorkerCompletedEventArgs FixFilesPermission(bool fullrepair)
        {
            return FixFilesPermission(MySettings.PSO2Dir, fullrepair);
        }

        public static RunWorkerCompletedEventArgs FixFilesPermission(string gameLocation, bool fullrepair)
        {
            RunWorkerCompletedEventArgs result = null;

            FileSystemAccessRule fileRule;
            if (fullrepair)
                fileRule = new FileSystemAccessRule(Leayal.IO.Permission.CurrentUser, FileSystemRights.TakeOwnership | FileSystemRights.Synchronize | FileSystemRights.Modify, AccessControlType.Allow);
            else
                fileRule = new FileSystemAccessRule(Leayal.IO.Permission.CurrentUser, FileSystemRights.Synchronize | FileSystemRights.Modify, AccessControlType.Allow);

            FileSystemAccessRule directoryRule;
            if (fullrepair)
                directoryRule = new FileSystemAccessRule(Leayal.IO.Permission.CurrentUser, FileSystemRights.TakeOwnership | FileSystemRights.Synchronize | FileSystemRights.Modify | FileSystemRights.ListDirectory, AccessControlType.Allow);
            else
                directoryRule = new FileSystemAccessRule(Leayal.IO.Permission.CurrentUser, FileSystemRights.Synchronize | FileSystemRights.Modify | FileSystemRights.ListDirectory, AccessControlType.Allow);

            try
            {
                foreach (string folder in Directory.EnumerateDirectories(gameLocation, "*", SearchOption.AllDirectories))
                    Leayal.IO.Permission.AddDirectorySecurity(folder, directoryRule);

                if (fullrepair)
                {
                    foreach (string file in Directory.EnumerateFiles(gameLocation, "*", SearchOption.AllDirectories))
                        Leayal.IO.Permission.AddFileSecurity(file, fileRule);
                }
                else
                {
                    foreach (string file in Directory.EnumerateFiles(gameLocation, "*.exe", SearchOption.TopDirectoryOnly))
                        Leayal.IO.Permission.AddFileSecurity(file, fileRule);
                }

                result = new RunWorkerCompletedEventArgs(null, null, false);
            }
            catch (Exception ex)
            {
                result = new RunWorkerCompletedEventArgs(null, ex, false);
            }
            return result;
        }

        public static RunWorkerCompletedEventArgs FixGameGuardError(string gameLocation)
        {
            return FixGameGuardError(false, gameLocation, null);
        }

        public static RunWorkerCompletedEventArgs FixGameGuardError(string gameLocation, Func<int, bool> progress_callback)
        {
            return FixGameGuardError(false, gameLocation, progress_callback);
        }

        public static RunWorkerCompletedEventArgs FixGameGuardError(bool cleanFix, string gameLocation, Func<int, bool> progress_callback)
        {
            RunWorkerCompletedEventArgs result = null;
            using (var webc = Components.WebClientManger.WebClientPool.GetWebClient_PSO2Download(true))
            {
                webc.CacheStorage = Components.CacheStorage.DefaultStorage;
                result = PSO2UpdateManager.RedownloadFile(webc, DefaultValues.Filenames.GameGuardDes + DefaultValues.Web.FakeFileExtension, Path.Combine(gameLocation, DefaultValues.Filenames.GameGuardDes), progress_callback);
                if (result.Error != null || result.Cancelled)
                    return result;
                result = PSO2UpdateManager.RedownloadFile(webc, DefaultValues.Filenames.GameGuardConfig + DefaultValues.Web.FakeFileExtension, Path.Combine(gameLocation, DefaultValues.Filenames.GameGuardConfig), progress_callback);
                if (result.Error != null || result.Cancelled)
                    return result;
                webc.CacheStorage = null;
            }
            if (cleanFix)
            {
                // Delete the game's gameguard folder
                Directory.Delete(Path.Combine(gameLocation, "GameGuard"), true);
                // Delete the registry
                Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree("SYSTEM\\CurrentControlSet\\Services\\npggsvc", false);
                // Delete the 3 (or 6???) hidden files in SystemRoot
                string sysRoot = Environment.GetFolderPath(Environment.SpecialFolder.System);
                File.Delete(Path.Combine(sysRoot, "gamemon.des"));
                File.Delete(Path.Combine(sysRoot, "npptnt2.sys"));
                File.Delete(Path.Combine(sysRoot, "nppt9x.vxd"));
            }
            return result;
        }

        public static bool IsCensorFileExist()
        {
            return IsCensorFileExist(MySettings.PSO2Dir);
        }

        public static bool IsCensorFileExist(string pso2Dir)
        {
            return File.Exists(Path.Combine(pso2Dir, "data", "win32", CensorFilename));
        }

        /// <summary>
        /// Enable = Enable the censor file, NOT Enable = enable REMOVE censor file
        /// </summary>
        /// <param name="enable">True to recover censor file. False to remove it.</param>
        /// <returns>bool</returns>
        public static bool ToggleCensorFile(bool enable)
        {
            return ToggleCensorFile(enable, MySettings.PSO2Dir);
        }

        /// <summary>
        /// Enable = Enable the censor file, NOT Enable = enable REMOVE censor file
        /// </summary>
        /// <param name="enable">True to recover censor file. False to remove it.</param>
        /// <param name="pso2Dir"></param>
        /// <returns>bool</returns>
        public static bool ToggleCensorFile(bool enable, string pso2Dir)
        {
            bool result = false;
            //ffbff2ac5b7a7948961212cefd4d402c
            if (!string.IsNullOrWhiteSpace(pso2Dir))
            {
                string censorfilepath = Path.Combine(pso2Dir, "data", "win32", "ffbff2ac5b7a7948961212cefd4d402c");
                if (enable)
                {
                    if (!File.Exists(censorfilepath))
                    {
                        if (File.Exists(censorfilepath + ".backup"))
                        {
                            File.Move(censorfilepath + ".backup", censorfilepath);
                            result = true;
                        }
                        else
                            result = false;
                    }
                    else
                    {
                        File.Delete(censorfilepath + ".backup");
                        result = true;
                    }                    
                }
                else
                {
                    if (File.Exists(censorfilepath))
                    {
                        File.Delete(censorfilepath + ".backup");
                        File.Move(censorfilepath, censorfilepath + ".backup");
                    }
                    result = true;
                }
            }
            return result;
        }

        public static RunWorkerCompletedEventArgs RedownloadCensorFile(string pso2Dir, Func<int, bool> progress_callback)
        {
            RunWorkerCompletedEventArgs result = null;
            if (!string.IsNullOrWhiteSpace(pso2Dir))
            {
                string censorfilepath = Path.Combine(pso2Dir, "data", "win32", "ffbff2ac5b7a7948961212cefd4d402c");
                using (var webc = Components.WebClientManger.WebClientPool.GetWebClient_PSO2Download(true))
                {
                    webc.CacheStorage = Components.CacheStorage.DefaultStorage;
                    result = PSO2UpdateManager.RedownloadFile(webc, "ffbff2ac5b7a7948961212cefd4d402c" + DefaultValues.Web.FakeFileExtension, censorfilepath, progress_callback);
                    webc.CacheStorage = null;
                }
            }
            else
                result = new RunWorkerCompletedEventArgs(null, new ArgumentNullException(), false);
            return result;
        }
    }
}
