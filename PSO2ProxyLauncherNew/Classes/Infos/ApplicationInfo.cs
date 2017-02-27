using System;
using Microsoft.VisualBasic.FileIO;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    public static class ApplicationInfo
    {
        //public static string ApplicationDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static string MyDocument { get { return SpecialDirectories.MyDocuments; } }
    }
}
