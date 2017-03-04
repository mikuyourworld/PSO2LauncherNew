using System;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    public static class ApplicationInfo
    {
        //public static string ApplicationDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static string MyDocument { get { return SpecialDirectories.MyDocuments; } }
        private static string _proxifierProfileDocument;
        public static string ProxifierProfileDocument
        {
            get
            {
                if (string.IsNullOrEmpty(_proxifierProfileDocument))
                    _proxifierProfileDocument = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Proxifier", "Profiles");
                return _proxifierProfileDocument;
            }
        }
    }
}
