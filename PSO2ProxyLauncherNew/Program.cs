using System;
using PSO2ProxyLauncherNew.Classes.Infos;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Microsoft.VisualBasic.ApplicationServices;
using Leayal.Log;

namespace PSO2ProxyLauncherNew
{
    static class Program
    {
        private static ResolveEventHandler ev = new ResolveEventHandler(AssemblyLoader.AssemblyResolve);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.AssemblyResolve += ev;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);

            LogManager.DefaultPath = DefaultValues.MyInfo.Directory.LogFolder;

            var asdawfawf = new SingleInstanceController();
            asdawfawf.Run(Environment.GetCommandLineArgs());
            AppDomain.CurrentDomain.AssemblyResolve -= ev;
        }

        private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            LogManager.GeneralLog.Print((Exception)e.ExceptionObject, LogLevel.Critical);
            //MsgBox.Error("Critical unhandled exception occured. Application will now exit.\n" + Logger.ExeptionParser(e.ExceptionObject as Exception));
            if (e.IsTerminating)
            {
                Environment.ExitCode = 2;
                Application.Exit();
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogManager.GeneralLog.Print(e.Exception);
            //Logger.Critical(e.Exception);
            //MsgBox.Error("Critical thread exception occured. Application will now exit.\n" + Logger.ExeptionParser(e.Exception));
            //Application.Exit();
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

            protected override bool OnUnhandledException(Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs e)
            {
                if (e.ExitApplication)
                    LogManager.GeneralLog.Print(e.Exception, LogLevel.Critical);
                else
                    LogManager.GeneralLog.Print(e.Exception, LogLevel.Error);
                return base.OnUnhandledException(e);
            }

            bool shutdowww;
            protected override void OnShutdown()
            {
                if (this.shutdowww) return;
                this.shutdowww = true;
                PSO2ProxyLauncherNew.Classes.PSO2.PSO2UrlDatabase.Save();
                Classes.Infos.CommonMethods.ExitAllProcesses();
                Classes.Components.AsyncForm.CloseAllForms();
                AssemblyLoader.UnloadAllCLI();
                base.OnShutdown();
            }

            protected override void OnCreateMainForm()
            { }

            protected override bool OnStartup(StartupEventArgs eventArgs)
            {
                if (IsSetArg(eventArgs.CommandLine, "dumpversionout"))
                {
                    this.WriteVersionFile(CommonMethods.PathConcat(Leayal.AppInfo.AssemblyInfo.DirectoryPath, "PSO2LauncherNewVersion.dat"));
                    eventArgs.Cancel = true;
                    Application.Exit();
                }
                else
                {
                    Application_CreateFolder();
                    Leayal.Forms.SystemEvents.ScalingFactorChanged += SystemEvents_ScalingFactorChanged;

                    var myForm = new Forms.MyMainMenu();
                    myForm.LetsSetReverse();
                    this.MainForm = myForm;
                    this.LetsScale();
                }

                return base.OnStartup(eventArgs);
            }

            private void SystemEvents_ScalingFactorChanged(object sender, EventArgs e)
            {
                this.LetsScale();
            }

            private void LetsScale()
            {
                if (this.MainForm != null)
                {
                    float f = Classes.Infos.CommonMethods.GetResolutionScale();
                    if (f == 1F)
                        this.MainForm.Size = this.MainForm.MinimumSize;
                    else
                        this.MainForm.Size = new System.Drawing.Size(Convert.ToInt32(this.MainForm.MinimumSize.Width * f), Convert.ToInt32(this.MainForm.MinimumSize.Height * f));
                }
            }

            protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
            {
                if (IsSetArg(eventArgs.CommandLine, "dumpversionout"))
                    this.WriteVersionFile(CommonMethods.PathConcat(Leayal.AppInfo.AssemblyInfo.DirectoryPath, "PSO2LauncherNewVersion.dat"));
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
