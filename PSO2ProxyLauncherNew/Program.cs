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

            var asdawfawf = new SingleInstanceController();
            asdawfawf.Run(Environment.GetCommandLineArgs());
            AppDomain.CurrentDomain.AssemblyResolve -= ev;
        }

        private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            LogManager.GeneralLog.Print((Exception)e.ExceptionObject, Logger.LogLevel.Critical);
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
                this.last_f = 1F;
            }

            protected override bool OnUnhandledException(Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs e)
            {
                if (e.ExitApplication)
                    LogManager.GeneralLog.Print(e.Exception, Logger.LogLevel.Critical);
                else
                    LogManager.GeneralLog.Print(e.Exception, Logger.LogLevel.Error);
                return base.OnUnhandledException(e);
            }

            bool shutdowww;
            protected override void OnShutdown()
            {
                if (this.shutdowww) return;
                this.shutdowww = true;
                Microsoft.Win32.SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
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
                    this.WriteVersionFile(CommonMethods.PathConcat(MyApp.AssemblyInfo.DirectoryPath, "PSO2LauncherNewVersion.dat"));
                    eventArgs.Cancel = true;
                    Application.Exit();
                }
                else
                {
                    Application_CreateFolder();
                    Microsoft.Win32.SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

                    var myForm = new Forms.MyMainMenu();
                    myForm.LetsSetReverse();
                    this.MainForm = myForm;
                    this.LetsScale();
                }

                return base.OnStartup(eventArgs);
            }

            float last_f;
            private void SystemEvents_UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
            {
                this.LetsScale();
            }

            private void LetsScale()
            {
                if (this.MainForm != null)
                {
                    float f = PSO2ProxyLauncherNew.Classes.Infos.CommonMethods.GetResolutionScale();
                    if (this.last_f != f)
                    {
                        this.last_f = f;
                        if (f == 1F)
                            this.MainForm.Size = this.MainForm.MinimumSize;
                        else
                            this.MainForm.Size = new System.Drawing.Size(Convert.ToInt32(this.MainForm.Width * f), Convert.ToInt32(this.MainForm.Height * f));
                    }
                }
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
