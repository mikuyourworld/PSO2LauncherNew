using System;
using System.Collections.Concurrent;
using System.IO;
using SharpCompress.Archives.Zip;
using System.Reflection;
using Leayal.IO;
using Leayal;
using PSO2ProxyLauncherNew.Classes.PSO2;

namespace PSO2ProxyLauncherNew
{
    public class AddonManager
    {
        private string rootDir;
        private ConcurrentDictionary<string, Assembly> assemblyList;

        public AddonManager(string dir)
        {
            this.rootDir = dir;
            this.assemblyList = new ConcurrentDictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
        }

        public void LoadAddons()
        {
            foreach (string path in Directory.EnumerateFiles(this.rootDir, "*.zip", SearchOption.TopDirectoryOnly))
                LoadAddon(path);

        }

        public AddonLoadResult LoadAddon(string path)
        {
            AddonLoadResult result = null;
            string filename = Path.GetFileName(path);
            using (ZipArchive archive = ZipArchive.Open(path))
            using (var reader = archive.ExtractAllEntries())
            using (MemoryFileCollection mfc = new MemoryFileCollection())
            {
                RecyclableMemoryStream rms;
                Leayal.Ini.IniFile ini = null;
                //seek for the header file while store any files have been read (which are not the header) to memorystream for later use.
                while (reader.MoveToNextEntry())
                    if (!reader.Entry.IsDirectory)
                    {
                        if (reader.Entry.Key.IsEqual("header.ini", true))
                        {
                            using (StreamReader sr = new StreamReader(reader.OpenEntryStream()))
                                ini = new Leayal.Ini.IniFile(sr);
                        }
                        else
                        {
                            rms = new RecyclableMemoryStream(reader.Entry.Key, reader.Entry.Size > int.MaxValue ? int.MaxValue : (int)reader.Entry.Size);
                            reader.WriteEntryTo(rms);
                            rms.Position = 0;
                            mfc.Add(reader.Entry.Key, rms);
                        }
                    }
                if (ini != null)
                    try
                    {
                        string req = ini.GetValue("Libraries", "Dependencies", string.Empty);
                        if (!string.IsNullOrWhiteSpace(req))
                        {
                            string[] reqList = req.Split(',');
                            if (reqList != null && reqList.Length > 0)
                                for (int i = 0; i < reqList.Length; i++)
                                    AssemblyLoader.myDict.Add(reqList[i], Assembly.Load(mfc[reqList[i]].ToArray()));
                        }
                        string mainDll = ini.GetValue("Libraries", "MainLib", string.Empty);
                        if (!string.IsNullOrWhiteSpace(mainDll))
                        {
                            if (!AssemblyLoader.myDict.ContainsKey(mainDll))
                                AssemblyLoader.myDict.Add(mainDll, Assembly.Load(mfc[mainDll].ToArray()));
                        }
                        result = new AddonLoadResult(filename);
                    }
                    catch (Exception ex)
                    {
                        Leayal.Log.LogManager.GetLogDefaultPath("AddonManager", true).Print(string.Format("Failed to load the add-on in archive '{0}'.\r\n{1}", filename, ex));
                        result = new AddonLoadResult(filename, ex);
                    }
            }
            return result;
        }

        public void LoadAddonAsync()
        {

        }
    }

    public class AddonLoadResultCollection
    {

    }

    public class AddonLoadResult
    {
        public string Filename { get; }
        public Exception Error { get; }
        public AddonLoadResult(string file)
        {
            this.Filename = file;
        }

        public AddonLoadResult(string file, Exception ex)
        {
            this.Filename = file;
            this.Error = ex;
        }
    }
}
