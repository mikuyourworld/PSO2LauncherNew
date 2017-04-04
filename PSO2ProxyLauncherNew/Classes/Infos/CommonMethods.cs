using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Leayal.Security.Cryptography;
using System.Runtime.InteropServices;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    public static class CommonMethods
    {        
        public static float ScalingFactor { get { return GetResolutionScale(); } }

        public static int MaxThreadsCount
        {
            get
            {
                //Limit the threads to only 4
                return Math.Min(4, Environment.ProcessorCount);
            }
        }

#if DEBUG
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions, System.Security.SecurityCritical]
        public static float GetResolutionScale()
        {
            return 1.25F;
        }
#else
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions, System.Security.SecurityCritical]
        public static float GetResolutionScale()
        {
            return Leayal.Forms.FormWrapper.ScalingFactor;
        }
#endif

        public static string SHA256FromString(string value)
        {
            return SHA256Wrapper.FromString(value);
        }

        public static string PathConcat(string path1, string path2)
        {
            return Leayal.IO.PathHelper.Combine(path1, path2);
        }

        public static bool DetermineBool(string _boolString)
        {
            if (string.IsNullOrWhiteSpace(_boolString))
                return true;
            else
            {
                _boolString = _boolString.ToLower();
                if (_boolString == "yes" || _boolString == "true")
                    return true;
                else if (_boolString == "no" || _boolString == "false")
                    return false;
                else
                    return true;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        public static IntPtr GetMethodAddress(IntPtr hModule, string lpProcName)
        {
            return GetProcAddress(hModule, lpProcName);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string libname);
        public static IntPtr LoadLib(string libname)
        {
            return LoadLibrary(libname);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FreeLibrary(IntPtr hModule);
        public static bool FreeLib(IntPtr hModule)
        {
            return FreeLibrary(hModule);
        }

        private static Dictionary<string, List<Process>> processHostPool = new Dictionary<string, List<Process>>();
        public static string FileToMD5Hash(string filepath)
        {
            return MD5Wrapper.FromFile(filepath);
        }
        public static string StringToMD5(string source)
        {
            return MD5Wrapper.FromString(source);
        }

        public static Process MakeProcess(ProcessStartInfo info)
        {
            Process result = new Process();
            result.StartInfo = info;
            string key = Path.GetFileName(info.FileName).ToLower();
            if (!processHostPool.ContainsKey(key))
                processHostPool.Add(key, new List<Process>());
            processHostPool[key].Add(result);
            return result;
        }

        public static Process MakeProcess(string filename)
        {
            Process result = new Process();
            result.StartInfo.FileName = filename;
            string key = Path.GetFileName(filename).ToLower();
            if (!processHostPool.ContainsKey(key))
                processHostPool.Add(key, new List<Process>());
            processHostPool[key].Add(result);
            return result;
        }

        public static Process MakeProcess(string filename, string args)
        {
            Process result = new Process();
            result.StartInfo.FileName = filename;
            result.StartInfo.Arguments = args;
            string key = Path.GetFileName(filename).ToLower();
            if (!processHostPool.ContainsKey(key))
                processHostPool.Add(key, new List<Process>());
            processHostPool[key].Add(result);
            return result;
        }

        public static Process[] GetProcess(string processName)
        {
            return processHostPool[processName.ToLower()].ToArray();
        }

        public static void ExitAllProcesses()
        {
            foreach (List<Process> procList in processHostPool.Values)
                foreach (Process proc in procList)
                    if (!proc.HasExited)
                    {
                        proc.CloseMainWindow();
                        proc.WaitForExit(1000);
                        if (!proc.HasExited)
                            proc.Kill();
                        proc.Close();
                    }
        }

        public static void KillAllProcesses()
        {
            foreach (List<Process> procList in processHostPool.Values)
                foreach (Process proc in procList)
                    if (!proc.HasExited)
                    {
                        proc.Kill();
                        proc.Close();
                    }
        }
    }
}
