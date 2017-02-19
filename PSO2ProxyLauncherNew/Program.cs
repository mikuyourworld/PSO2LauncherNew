using System;
using PSO2ProxyLauncherNew.Classes.Infos;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using PSO2ProxyLauncherNew.Classes.Log;

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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyLoader.AssemblyResolve);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application_CreateFolder();
            new SingleInstanceController().Run(Environment.GetCommandLineArgs());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
        }
    }
}
