using System;
using PSO2ProxyLauncherNew.Classes.Infos;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Microsoft.VisualBasic.ApplicationServices;
using Leayal.Log;
using System.Linq;
using System.Collections.ObjectModel;
using System.Security;
using System.Security.Permissions;
using Microsoft.VisualBasic.CompilerServices;

namespace PSO2ProxyLauncherNew
{
    static class Program
    {
        internal static bool launchedbysteam;
        internal static SingleInstanceController ApplicationController;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ResolveEventHandler ev = new ResolveEventHandler(AssemblyLoader.AssemblyResolve);
            AppDomain.CurrentDomain.AssemblyResolve += ev;
            AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);

            ApplicationController = new SingleInstanceController();
            ApplicationController.Run(Environment.GetCommandLineArgs());
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

        internal class SingleInstanceController : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
        {
            public SingleInstanceController() : base(Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.Windows)
            {
                this.ShutdownStyle = Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
                this.IsSingleInstance = true;
                this.SaveMySettingsOnExit = false;
                LogManager.DefaultPath = DefaultValues.MyInfo.Directory.LogFolder;
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
                Classes.Infos.CommonMethods.ExitAllProcesses();
                Classes.Components.AsyncForm.CloseAllForms();
                AssemblyLoader.UnloadAllCLI();
                base.OnShutdown();
            }

            protected override void OnCreateMainForm()
            { }

            public void HideSplashScreenEx()
            {
                this.m_SplashScreen.FadeIt();
            }

            public void DisposeSplashScreen()
            {
                object splashLock = new object();
                ObjectFlowControl.CheckForSyncLockOnValueType(splashLock);
                lock (splashLock)
                {
                    System.Threading.SynchronizationContext.Current?.Send(new SendOrPostCallback(delegate
                    {
                        if (this.MainForm != null)
                        {
                            new UIPermission(UIPermissionWindow.AllWindows).Assert();
                            this.MainForm.Activate();
                            PermissionSet.RevertAssert();
                        }
                    }), null);
                    if ((this.m_SplashScreen != null) && !this.m_SplashScreen.IsDisposed)
                        this.m_SplashScreen.Dispose();
                }
            }

            private void SplashScreen_FormClosed(object sender, FormClosedEventArgs e)
            {
                this.DisposeSplashScreen();
            }

            private bool m_DidSplashScreen = false;
            Forms.SplashScreen m_SplashScreen;

            protected void ShowSplashScreenEx()
            {
                if (!this.m_DidSplashScreen)
                {
                    this.m_DidSplashScreen = true;
                    if (this.m_SplashScreen == null)
                    {
                        new Thread(new ThreadStart(delegate 
                        {
                            this.m_SplashScreen = new Forms.SplashScreen();
                            this.m_SplashScreen.FormClosed += SplashScreen_FormClosed;
                            Application.Run(this.m_SplashScreen);
                        })).Start();
                    }
                }
            }

            protected override bool OnInitialize(ReadOnlyCollection<string> commandLineArgs)
            {
                this.EnableVisualStyles = true;
                Application.EnableVisualStyles();
                if (!commandLineArgs.Contains("/nosplash") && !this.CommandLineArgs.Contains("-nosplash"))
                {
                    this.ShowSplashScreenEx();
                }
                return true;
            }

            protected override bool OnStartup(StartupEventArgs eventArgs)
            {
                if (IsSetArg(eventArgs.CommandLine, "dumpversionout"))
                {
                    this.WriteVersionFile(CommonMethods.PathConcat(Leayal.AppInfo.AssemblyInfo.DirectoryPath, "PSO2LauncherNewVersion.dat"));
                    eventArgs.Cancel = true;
                    Application.Exit();
                    return false;
                }
                else
                {
                    Application_CreateFolder();

                    launchedbysteam = IsSetArg(eventArgs.CommandLine, "steam", true) ||
                        IsSetArg(eventArgs.CommandLine, "-steam", true) ||
                        IsSetArg(eventArgs.CommandLine, "/steam", true) ||
                        Classes.Infos.CommonMethods.IsLaunchedBySteam();
                    var mymainmenu = new Forms.MyMainMenu();
                    if (launchedbysteam)
                        mymainmenu.PrintText(Classes.LanguageManager.GetMessageText("launchedbysteam", "Launcher has been launched by Steam or has launched with steam switch. Auto enable steam mode."), Leayal.Forms.RtfColor.Green);
                    this.MainForm = mymainmenu;
                }

                return base.OnStartup(eventArgs);
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
                return args.Contains(argName);
            }

            private bool IsSetArg(System.Collections.Generic.IEnumerable<string> args, string argName, bool ignoreCase)
            {
                if (ignoreCase)
                {
                    foreach (string arg in args)
                        if (Leayal.StringHelper.IsEqual(arg, argName, true))
                            return true;
                    return false;
                }
                else
                    return args.Contains(argName);
            }
        }
    }
}
