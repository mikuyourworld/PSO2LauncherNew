using System;
using PSO2ProxyLauncherNew.Classes.Infos;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using PSO2ProxyLauncherNew.Classes.Log;
using Microsoft.VisualBasic.ApplicationServices;

namespace PSO2ProxyLauncherNew
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyLoader.AssemblyResolve);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application_CreateFolder();

            var asdawfawf = new SingleInstanceController();
            asdawfawf.Run(Environment.GetCommandLineArgs());
        }

        private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            LogManager.GeneralLog.Print(e.ExceptionObject as Exception);
            //MsgBox.Error("Critical unhandled exception occured. Application will now exit.\n" + Logger.ExeptionParser(e.ExceptionObject as Exception));

            Application.Exit();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogManager.GeneralLog.Print(e.Exception);
            //Logger.Critical(e.Exception);
            //MsgBox.Error("Critical thread exception occured. Application will now exit.\n" + Logger.ExeptionParser(e.Exception));

            Application.Exit();
        }

        private static void Application_CreateFolder()
        {
            Directory.CreateDirectory(DefaultValues.MyInfo.Directory.LanguageFolder);
            Directory.CreateDirectory(DefaultValues.MyInfo.Directory.LogFolder);
        }

        private class SingleInstanceController : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
        {
            public SingleInstanceController() : base(Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.Windows)
            {
                this.ShutdownStyle = Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
                this.IsSingleInstance = true;
                this.EnableVisualStyles = true;
                this.SaveMySettingsOnExit = false;
            }

            protected override void OnShutdown()
            {
                PSO2ProxyLauncherNew.Classes.PSO2.PSO2UrlDatabase.Save();
                Classes.Infos.CommonMethods.ExitAllProcesses();
                base.OnShutdown();
            }

            protected override void OnCreateMainForm()
            {
                this.MainForm = new Forms.MyMainMenu();
            }

            protected override bool OnStartup(StartupEventArgs eventArgs)
            {
                if (IsSetArg(eventArgs.CommandLine, "dumpversionout"))
                {
                    this.WriteVersionFile(CommonMethods.PathConcat(MyApp.AssemblyInfo.DirectoryPath, "PSO2LauncherNewVersion.dat"));
                    eventArgs.Cancel = true;
                    Environment.Exit(0);
                }
                return base.OnStartup(eventArgs);
            }

            protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
            {
                if (IsSetArg(eventArgs.CommandLine, "dumpversionout"))
                    this.WriteVersionFile(CommonMethods.PathConcat(MyApp.AssemblyInfo.DirectoryPath, "PSO2LauncherNewVersion.dat"));
                base.OnStartupNextInstance(eventArgs);
            }

            private void WriteVersionFile(string path)
            {
                using (FileStream fs = File.Create(path))
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    var _with1 = (this.Info.Version);
                    bw.Write(Convert.ToByte(_with1.Major));
                    bw.Write(Convert.ToByte(_with1.Minor));
                    bw.Write(Convert.ToByte(_with1.Build));
                    bw.Write(Convert.ToByte(_with1.Revision));
                }
            }

            private bool IsSetArg(System.Collections.Generic.IEnumerable<string> args, string argName)
            {
                bool functionReturnValue = false;
                functionReturnValue = false;
                foreach (string arg in args)
                    if ((arg.ToLower() == argName.ToLower()))
                        functionReturnValue = true;
                return functionReturnValue;
            }
            private bool IsSetArg(string[] args, string argName)
            {
                bool functionReturnValue = false;
                functionReturnValue = false;
                if ((args != null) && (args.Length > 0))
                    for (int i = 0; i <= args.Length - 1; i++)
                        if ((args[i].ToLower() == argName.ToLower()))
                            functionReturnValue = true;
                return functionReturnValue;
            }
        }
    }
}
