using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualBasic.ApplicationServices;

namespace PSO2ProxyLauncherNew
{
    public class MyApp
    {
        private static ComputerInfo _compInfo = new ComputerInfo();
        public static ComputerInfo ComputerInfo
        { get { return _compInfo; } }
        private static AssemblyInfo _assemblyInfo = new AssemblyInfo(System.Reflection.Assembly.GetExecutingAssembly());
        public static AssemblyInfo AssemblyInfo
        { get { return _assemblyInfo; } }

        private static string _appFilename;
        public static string ApplicationFilename
        {
            get
            {
                if (string.IsNullOrEmpty(_appFilename))
                    using (Process myProcess = Process.GetCurrentProcess())
                        _appFilename = myProcess.MainModule.FileName;
                return _appFilename;
            }
        }
    }
}
