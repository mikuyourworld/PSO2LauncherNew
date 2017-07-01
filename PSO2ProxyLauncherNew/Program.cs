using System;
using PSO2ProxyLauncherNew.Classes.Infos;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Microsoft.VisualBasic.ApplicationServices;
using Leayal.Log;
using System.Linq;
using System.Collections.ObjectModel;

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
            {
                string[] cmds = Environment.GetCommandLineArgs();
                launchedbysteam = IsSetArg(cmds, "steam", true) ||
                        IsSetArg(cmds, "-steam", true) ||
                        IsSetArg(cmds, "/steam", true) ||
                        Classes.Infos.CommonMethods.IsLaunchedBySteam();
                var mymainmenu = new Forms.MyMainMenu();
                if (launchedbysteam)
                    mymainmenu.PrintText(Classes.LanguageManager.GetMessageText("launchedbysteam", "Launcher has been launched by Steam or has launched with steam switch. Auto enable steam mode."), Leayal.Forms.RtfColor.Green);
                this.MainForm = mymainmenu;
            }

            public void DisposeSplashScreen()
            {
                if (this.SplashScreen != null)
                {
                    this.HideSplashScreen();
                    if (this.SplashScreen != null)
                        this.SplashScreen.Dispose();
                }
            }

            protected override void OnCreateSplashScreen()
            {
                this.SplashScreen = new Forms.SplashScreen();
            }

            protected override bool OnInitialize(ReadOnlyCollection<string> commandLineArgs)
            {
                this.EnableVisualStyles = true;
                this.SaveMySettingsOnExit = false;
                return base.OnInitialize(commandLineArgs);
            }

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
