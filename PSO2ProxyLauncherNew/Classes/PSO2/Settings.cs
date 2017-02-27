using PSO2ProxyLauncherNew.Classes;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    public static class Settings
    {
        private static string cache_DocumentWorkSpace;
        private static string cache_pso2verpath;
        private static string cache_pso2precedeverpath;

        public static string DocumentWorkSpace
        {
            get
            {
                if (string.IsNullOrEmpty(cache_DocumentWorkSpace))
                    cache_DocumentWorkSpace = Path.Combine(Infos.ApplicationInfo.MyDocument, "SEGA", "PHANTASYSTARONLINE2");
                return cache_DocumentWorkSpace;
            }
        }
        public static string VersionString
        {
            get
            {
                if (string.IsNullOrEmpty(cache_pso2verpath))
                    cache_pso2verpath = Path.Combine(DocumentWorkSpace, "version.ver");
                if (File.Exists(cache_pso2verpath))
                {
                    string result = File.ReadAllText(cache_pso2verpath);
                    if (string.IsNullOrWhiteSpace(result))
                        return string.Empty;
                    else
                        return result.Trim();
                }
                else
                    return string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(cache_pso2verpath))
                    cache_pso2verpath = Path.Combine(DocumentWorkSpace, "version.ver");
                Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(DocumentWorkSpace);
                File.WriteAllText(cache_pso2verpath, value);
            }
        }

        public static string PrecedeVersionString
        {
            get
            {
                if (string.IsNullOrEmpty(cache_pso2precedeverpath))
                    cache_pso2precedeverpath = Path.Combine(DocumentWorkSpace, "version_precede.ver");
                if (File.Exists(cache_pso2precedeverpath))
                {
                    string result = File.ReadAllText(cache_pso2precedeverpath);
                    if (string.IsNullOrWhiteSpace(result))
                        return string.Empty;
                    else
                        return result.Trim();
                }
                else
                    return string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(cache_pso2precedeverpath))
                    cache_pso2precedeverpath = Path.Combine(DocumentWorkSpace, "version_precede.ver");
                Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(DocumentWorkSpace);
                File.WriteAllText(cache_pso2precedeverpath, value);
            }
        }

        
    }
}
