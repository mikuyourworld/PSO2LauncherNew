using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    public static class CommonMethods
    {

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap : int
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90,
        }

        private static float _ScalingFactor;
        public static float ScalingFactor { get { return _ScalingFactor; } }

#if DEBUG
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions, System.Security.SecurityCritical]
        public static float GetResolutionScale()
        {
            _ScalingFactor = 1.25F;
            return _ScalingFactor;
            try
            {
                System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
                IntPtr desktop = g.GetHdc();
                int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
                int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
                int logpixelsy = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
                float screenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
                float dpiScalingFactor = (float)logpixelsy / (float)96;

                g.ReleaseHdc();
                
                if (dpiScalingFactor > 1)
                    _ScalingFactor = dpiScalingFactor;
                else if (screenScalingFactor > 1)
                    _ScalingFactor = screenScalingFactor;
                else
                    _ScalingFactor = 1F;
            }
            catch { _ScalingFactor = 1F; }
            return _ScalingFactor;
        }
#else
        public static float GetResolutionScale()
        {
            try
            {
                System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
                IntPtr desktop = g.GetHdc();
                int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
                int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
                int logpixelsy = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
                float screenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
                float dpiScalingFactor = (float)logpixelsy / (float)96;

                g.ReleaseHdc();
                
                if (dpiScalingFactor > 1)
                    _ScalingFactor = dpiScalingFactor;
                else if (screenScalingFactor > 1)
                    _ScalingFactor = screenScalingFactor;
                else
                    _ScalingFactor = 1F;
            }
            catch { _ScalingFactor = 1F; }
            return _ScalingFactor;
        }
#endif

        public static string URLConcat(string url1, string url2)
        {
            if (string.IsNullOrWhiteSpace(url1) | string.IsNullOrWhiteSpace(url2))
                return string.Empty;
            else
                return url1.URLtrim() + "/" + url2.URLtrim();
        }

        public static string URLtrim(this string url)
        { return url.Trim('\\', '/', ' '); }

        public static string PathConcat(string path1, string path2)
        { return Path.Combine(path1.PathTrim(), path2.PathTrim()); }

        public static string PathTrim(this string url)
        {
            url = url.TrimStart('/', ' ');
            url = url.TrimEnd('\\', '/', ' ');
            return url;
        }

        private static Dictionary<string, List<Process>> processHostPool = new Dictionary<string, List<Process>>();
        public static string FileToMD5Hash(string filepath)
        {
            string result = string.Empty;
            if (File.Exists(filepath))
            {
                StringBuilder _stringBuilder = new StringBuilder(32);
                using (MD5 md5engine = MD5.Create())
                using (FileStream fs = File.OpenRead(filepath))
                {
                    byte[] arrbytHashValue = md5engine.ComputeHash(fs);
                    for (int i = 0; i < arrbytHashValue.Length; i++)
                        _stringBuilder.Append(arrbytHashValue[i].ToString("X2"));
                }
                //result = BitConverter.ToString(arrbytHashValue);
                result = _stringBuilder.ToString(); // result.Replace("-", "");
            }
            return result;
        }
        public static string StringToMD5(string source)
        {
            StringBuilder _stringBuilder = new StringBuilder(32);
            using (MD5 _md5 = MD5.Create())
            {
                byte[] numArray = _md5.ComputeHash(Encoding.UTF8.GetBytes(source));
                for (int i = 0; i < numArray.Length; i++)
                    _stringBuilder.Append(numArray[i].ToString("X2"));
            }
            
            return _stringBuilder.ToString();
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

        public static void EmptyFolder(string directoryPath)
        {
            foreach (string filename in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
            {
                try
                { File.Delete(filename); }
                catch { }
            }
            foreach (string filename in Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories))
            {
                try
                { Directory.Delete(filename, true); }
                catch { }
            }
        }
        public static void EmptyFolder(DirectoryInfo directory)
        {
            EmptyFolder(directory.FullName);
        }

        public static string TableStringToArgs(List<string> arg) { return TableStringToArgs(arg.ToArray()); }

        public static string TableStringToArgs(string[] arg)
        {
            if ((arg != null) && (arg.Length > 0))
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                string tmp;
                for (int i = 0; i < arg.Length; i++)
                {
                    tmp = arg[i];
                    if (!string.IsNullOrWhiteSpace(tmp))
                    {
                        if (i == 0)
                        {
                            if (tmp.IndexOf(" ") > -1)
                            {
                                builder.Append("\"" + tmp + "\"");
                            }
                            else
                            {
                                builder.Append(tmp);
                            }
                        }
                        else
                        {
                            if (tmp.IndexOf(" ") > -1)
                            {
                                builder.Append(" \"" + tmp + "\"");
                            }
                            else
                            {
                                builder.Append(" " + tmp);
                            }
                        }
                    }
                }
                return builder.ToString();
            }
            else { return string.Empty; }
        }
    }
}
