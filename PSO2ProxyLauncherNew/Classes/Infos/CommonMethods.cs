using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    public static class CommonMethods
    {
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
            if (!File.Exists(filepath))
                result = string.Empty;
            else
            {
                byte[] arrbytHashValue;
                using (MD5 md5engine = MD5.Create())
                using (FileStream fs = File.OpenRead(filepath))
                    arrbytHashValue = md5engine.ComputeHash(fs);
                result = BitConverter.ToString(arrbytHashValue);
                arrbytHashValue = null;
                result = result.Replace("-", "");
            }
            return result;
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
