using System;
using System.Threading;
using System.Diagnostics;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Events;
using System.IO;
using System.Net;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    public class SelfUpdate : IDisposable
    {
        private ExtendedWebClient withEventsField_myWebClient;
        private ExtendedWebClient myWebClient
        {
            get { return withEventsField_myWebClient; }
            set
            {                
                if (withEventsField_myWebClient != null)
                {
                    withEventsField_myWebClient.DownloadDataCompleted -= myWebClient_DownloadDataCompleted;
                    withEventsField_myWebClient.DownloadFileCompleted -= myWebClient_DownloadFileCompleted;
                    withEventsField_myWebClient.DownloadProgressChanged -= myWebClient_DownloadProgressChanged;
                }
                withEventsField_myWebClient = value;
                if (withEventsField_myWebClient != null)
                {
                    withEventsField_myWebClient.DownloadDataCompleted += myWebClient_DownloadDataCompleted;
                    withEventsField_myWebClient.DownloadFileCompleted += myWebClient_DownloadFileCompleted;
                    withEventsField_myWebClient.DownloadProgressChanged += myWebClient_DownloadProgressChanged;
                }
            }
        }
        private SynchronizationContext syncContext;
        private string LatestVersionStep = LanguageManager.GetMessageText("SelfUpdate_UsingLatestVersion", "You're using latest PSO2Launcher version");
        private string NewerVersionFoundStep = LanguageManager.GetMessageText("SelfUpdate_NewVersionFound", "Found newer version");
        public SelfUpdate() : this(SynchronizationContext.Current) { }
        public SelfUpdate(SynchronizationContext _syncContext)
        {
            this.myWebClient = new ExtendedWebClient();
            this.myWebClient.TimeOut = 5000;
            this.myWebClient.Proxy = null;
            this.syncContext = _syncContext;
            this._IsBusy = false;
            this.VersionUri = null;
            this.UpdateUri = null;
            this._CurrentStep = null;
            this.UpdaterUri = null;
            this._NewVersion = null;
            this._IsNewVersion = false;
            this.UpdaterPath = Path.Combine(Leayal.AppInfo.AssemblyInfo.DirectoryPath, "updater.exe");
        }

        #region "Properties"
        public string UpdaterPath { get; }
        private bool _IsBusy;
        public bool IsBusy { get { return this._IsBusy; } }

        public Uri VersionUri { get; set; }
        public Uri UpdateUri { get; set; }
        public Uri UpdaterUri { get; set; }
        private string _CurrentStep;
        public string CurrentStep { get { return this._CurrentStep; } }
        private int _CurrentProgress;
        public int CurrentProgress { get { return this._CurrentProgress; } }

        private bool _IsNewVersion;
        public bool IsNewVersion { get { return this._IsNewVersion; } }

        private Version _NewVersion;
        public Version NewVersion { get { return this._NewVersion; } }
        #endregion

        public void CheckForUpdates()
        {
            if ((!this._IsBusy) && (this.VersionUri != null))
            {
                this._IsBusy = true;
                //this.RaiseEventStepChanged(LanguageManager.GetMessageText("SelfUpdate_CheckingForUpdates", "Checking for Updates"));
                this.OnProgressBarStateChanged(new Events.ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                this.myWebClient.DownloadDataAsync(VersionUri, "check");
            }
        }

        public void ForceUpdate()
        {
            if ((!this.IsBusy) && (this.IsNewVersion))
                this.OnPreDownloadUpdate(this.NewVersion);
        }

        private void myWebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if ((e.Error != null))
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
            else
            {
                if ((e.UserState is string))
                {
                    string state = e.UserState as string;
                    if (state == "check")
                    {
                        if ((e.Result != null) && (e.Result.Length > 1))
                        {
                            NewVersionEventArgs myeventarg = null;
                            if ((e.Result.Length == 4))
                                myeventarg = new NewVersionEventArgs(e.Result[0], e.Result[1], e.Result[2], e.Result[3]);
                            else
                                myeventarg = new NewVersionEventArgs(e.Result[0], e.Result[1]);
                            this.OnCheckVersion(myeventarg);
                        }
                    }
                }
            }
        }

        protected virtual void OnCheckVersion(NewVersionEventArgs e)
        {
            if ((e.Version.CompareTo(Leayal.AppInfo.AssemblyInfo.Version) == 0))
                this.RaiseEventCheckCompleted();
            else
                this.OnNewVersion(e);
        }

        protected virtual void OnDownloadUpdate(Version ver)
        {
            this._CurrentProgress = -1;
            string thePath = Path.ChangeExtension(Leayal.AppInfo.ApplicationFilename, ".update-" + ver.ToString());
            this.RaiseEventStepChanged(LanguageManager.GetMessageText("SelfUpdate_ExtractingUpdates", "Extracting Updates"));
            if (SharpCompress.Archives.SevenZip.SevenZipArchive.IsSevenZipFile(thePath + ".7z"))
            {
                using (SharpCompress.Archives.SevenZip.SevenZipArchive archive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(thePath + ".7z"))
                using (SharpCompress.Readers.IReader reader = archive.ExtractAllEntries())
                    if (reader.MoveToNextEntry())
                        using (FileStream fs = File.Create(thePath))
                            reader.WriteEntryTo(fs);
                try
                { File.Delete(thePath + ".7z"); }
                catch
                { }
                this.OnProgressBarStateChanged(new Events.ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                this.RaiseEventStepChanged(LanguageManager.GetMessageText("SelfUpdate_RestartToUpdate", "Restarting application to perform update."));
                using (Process theProcess = new Process())
                {
                    theProcess.StartInfo.FileName = this.UpdaterPath;
                    var alwigh = new System.Collections.Generic.List<string>(3);
                    alwigh.Add("-leayal");
                    alwigh.Add("-patch:" + thePath);
                    alwigh.Add("-destination:" + Leayal.AppInfo.ApplicationFilename);
                    theProcess.StartInfo.Arguments = Leayal.ProcessHelper.TableStringToArgs(alwigh);
                    if ((Leayal.OSVersionInfo.Name.ToLower() != "windows xp"))
                        theProcess.StartInfo.Verb = "runas";
                    theProcess.Start();
                }
                Environment.Exit(0);
            }
            else
                this.OnHandledException(new HandledExceptionEventArgs(new FileNotFoundException("Update content not found", thePath)));
        }

        protected virtual void OnPreDownloadUpdate(Version ver)
        {
            if (!this.myWebClient.IsBusy && (this.UpdateUri != null))
            {
                this.OnProgressBarStateChanged(new Events.ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                this.RaiseEventBeginDownloadPatch();
                if (File.Exists(this.UpdaterPath))
                {
                    this.RaiseEventStepChanged("Downloading new version");
                    this.myWebClient.DownloadFileAsync(this.UpdateUri, Path.ChangeExtension(Leayal.AppInfo.ApplicationFilename, ".update-" + ver.ToString()) + ".7z", ver);
                }
                else
                {
                    if (this.UpdaterUri != null)
                    {
                        this.RaiseEventStepChanged("Downloading updater");
                        this.myWebClient.DownloadFileAsync(this.UpdaterUri, this.UpdaterPath + ".7z", new DownloadFileMeta("downloadupdater", ver));
                    }
                }
            }
        }

        private void myWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if ((e.Error != null))
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
            else
            {
                if (e.UserState is Version)
                    this.OnDownloadUpdate((Version)e.UserState);
                else if (e.UserState is DownloadFileMeta)
                {
                    DownloadFileMeta state = (DownloadFileMeta)e.UserState;
                    if ((state.State == "downloadupdater"))
                    {
                        this.RaiseEventStepChanged("Downloading new version");
                        if (SharpCompress.Archives.SevenZip.SevenZipArchive.IsSevenZipFile(this.UpdaterPath + ".7z"))
                        {
                            using (SharpCompress.Archives.SevenZip.SevenZipArchive archive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(this.UpdaterPath + ".7z"))
                            using (SharpCompress.Readers.IReader reader = archive.ExtractAllEntries())
                                if (reader.MoveToNextEntry())
                                    using (FileStream fs = File.Create(this.UpdaterPath))
                                        reader.WriteEntryTo(fs);
                            try
                            { File.Delete(this.UpdaterPath + ".7z"); }
                            catch { }
                            this.OnProgressBarStateChanged(new Events.ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                            this.myWebClient.DownloadFileAsync(this.UpdateUri, Path.ChangeExtension(Leayal.AppInfo.ApplicationFilename, ".update-" + state.Ver.ToString()) + ".7z", state.Ver);
                        }
                        else
                            this.OnHandledException(new HandledExceptionEventArgs(new FileNotFoundException("Updater not found", this.UpdaterPath)));
                    }
                }
            }
        }

        private void myWebClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, null));
        }

        public void Dispose()
        {
            myWebClient.Dispose();
        }

        #region "Events"
        public event EventHandler CheckCompleted;
        private void RaiseEventCheckCompleted()
        {
            this._IsBusy = false;
            this.OnCheckCompleted(EventArgs.Empty);
            this.RaiseEventStepChanged(LatestVersionStep);
        }
        private void OnCheckCompleted(EventArgs e)
        {
            if (this.CheckCompleted != null)
                this.syncContext?.Post(new SendOrPostCallback(delegate { this.CheckCompleted.Invoke(this, e); }), null);
        }

        public event EventHandler BeginDownloadPatch;
        private void RaiseEventBeginDownloadPatch()
        {
            this.OnBeginDownloadPatch(EventArgs.Empty);
        }
        private void OnBeginDownloadPatch(EventArgs e)
        {
            if (this.BeginDownloadPatch != null)
                this.syncContext?.Post(new SendOrPostCallback(delegate { this.BeginDownloadPatch.Invoke(this, e); }), null);
        }
        
        public event EventHandler<HandledExceptionEventArgs> HandledException;
        private void OnHandledException(HandledExceptionEventArgs e)
        {
            this._IsBusy = this.myWebClient.IsBusy;
            if (this.HandledException != null)
                this.syncContext?.Post(new SendOrPostCallback(delegate { this.HandledException.Invoke(this, e); }), null);
            else
            {
                Leayal.Log.LogManager.GeneralLog.Print(e.Error);
                System.Windows.Forms.MessageBox.Show(string.Format(LanguageManager.GetMessageText("MyMainMenu_FailedCheckLauncherUpdates", "Failed to check for PSO2Launcher updates. Reason: {0}"), e.Error.Message), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        protected void RaiseEventStepChanged(string _step)
        {
            this.OnCurrentStepChanged(new StepEventArgs(_step));
        }
        public event EventHandler<StepEventArgs> CurrentStepChanged;
        protected void OnCurrentStepChanged(StepEventArgs e)
        {
            if (this.CurrentStepChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentStepChanged.Invoke(this, e); }), null);
        }

        public event ProgressChangedEventHandler ProgressChanged;
        private void OnProgressChanged(ProgressChangedEventArgs e)
        {
            this._CurrentProgress = e.ProgressPercentage;
            if (ProgressChanged != null)
                this.syncContext?.Post(new SendOrPostCallback(delegate { this.ProgressChanged.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressBarStateChangedEventArgs> ProgressBarStateChanged;
        protected void OnProgressBarStateChanged(ProgressBarStateChangedEventArgs e)
        {
            if (this.ProgressBarStateChanged != null)
                this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressBarStateChanged.Invoke(this, e); }), null);
        }
        public event EventHandler<NewVersionEventArgs> FoundNewVersion;
        private void OnNewVersion(NewVersionEventArgs e)
        {
            this._IsNewVersion = true;
            this._NewVersion = e.Version;
            if (FoundNewVersion != null)
                this.syncContext?.Send(new SendOrPostCallback(delegate { this.FoundNewVersion.Invoke(this, e); }), null);
            if (e.AllowUpdate)
            {
                this.OnPreDownloadUpdate(e.Version);
            }
            else
            {
                this._IsBusy = false;
                this.RaiseEventStepChanged(NewerVersionFoundStep + ": " + e.Version.ToString());
            }//*/
        }
        #endregion

        #region "Internal Classes"
        protected class DownloadFileMeta
        {
            public string State { get; }
            public Version Ver { get; }
            public DownloadFileMeta(string sstate, Version over)
            {
                this.State = sstate;
                this.Ver = over;
            }
        }
        public class NewVersionEventArgs : System.EventArgs
        {
            public Version Version { get; }
            public bool AllowUpdate { get; set; }
            public NewVersionEventArgs(byte major, byte minor) : this(new Version(major, minor)) { }
            public NewVersionEventArgs(byte major, byte minor, byte build, byte revision) : this(new Version(major, minor, build, revision)) { }
            public NewVersionEventArgs(string verstring) : this(new Version(verstring)) { }
            public NewVersionEventArgs(Version ver) : base()
            {
                this.Version = ver;
                this.AllowUpdate = false;
            }
        }
        #endregion
    }
}