using System;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Events;
using PSO2ProxyLauncherNew.Forms.MyMainMenuCode;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    public enum PatchType : short
    {
        English,
        LargeFiles,
        Story
    }
    public enum Task : short
    {
        None,
        LaunchGame,
        PSO2Update,
        UninstallAllPatches,
        EnglishPatch,
        LargeFilesPatch,
        StoryPatch
    }
    class PSO2Controller
    {
        private System.Threading.SynchronizationContext syncContext;
        private Patches.EnglishPatchManager englishManager;
        private Patches.StoryPatchManager storyManager;
        private Patches.LargeFilesPatchManager largefilesManager;
        private PSO2UpdateManager mypso2updater;
        private BackgroundWorker bWorker_GameStart;
        //private BackgroundWorker bWorker_PatchesVersionCheck;

        public bool IsBusy { get; private set; }
        public Task CurrentTask { get; private set; }
        public PSO2Controller(System.Threading.SynchronizationContext _SyncContext)
        {
            this.IsBusy = false;
            this.CurrentTask = Task.None;
            this.syncContext = _SyncContext;
            this.englishManager = CreateEnglishManager();
            this.largefilesManager = CreateLargeFilesManager();
            this.storyManager = CreateStoryManager();
            this.mypso2updater = CreatePSO2UpdateManager();
            this.bWorker_GameStart = CreateBworkerGameStart();
        }

        #region "All Patches"
        public void CancelOperation()
        {
            switch (this.CurrentTask)
            {
                case Task.EnglishPatch:
                    this.CurrentTask = Task.None;
                    this.englishManager.CancelAsync();
                    break;
                case Task.LargeFilesPatch:
                    this.CurrentTask = Task.None;
                    this.largefilesManager.CancelAsync();
                    break;
                case Task.StoryPatch:
                    this.CurrentTask = Task.None;
                    this.storyManager.CancelAsync();
                    break;
                case Task.PSO2Update:
                    this.CurrentTask = Task.None;
                    this.mypso2updater.CancelAsync();
                    break;
                case Task.UninstallAllPatches:
                    this.CurrentTask = Task.None;
                    this.storyManager.CancelAsync();
                    this.largefilesManager.CancelAsync();
                    this.englishManager.CancelAsync();
                    break;
            }
        }

        public VersionsCheckResults CheckForPatchesVersionsAndWait()
        {
            VersionsCheckResults result = new VersionsCheckResults();
            this.NotifyPatches();
            string curEngVer = this.englishManager.VersionString, curLargeFilesVer = this.largefilesManager.VersionString, curStoryVer = this.storyManager.VersionString;
            bool eng = (curEngVer != Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString), largefiles = (curLargeFilesVer != Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString), story = (curStoryVer != Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString);
            if (eng || largefiles || story)
            {
                string returnFromWeb = WebClientManger.WebClientPool.GetWebClient_AIDA().DownloadString(Classes.AIDA.WebPatches.PatchesInfos);
                if (!string.IsNullOrWhiteSpace(returnFromWeb))
                {
                    string jsonPropertyName, tmpstring;
                    using (var sr = new System.IO.StringReader(returnFromWeb))
                    using (var jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
                        while (jsonReader.Read())
                            if (jsonReader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                            {
                                tmpstring = string.Empty;
                                jsonPropertyName = (jsonReader.Value as string).ToLower();
                                if (jsonPropertyName == Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.ENPatchOverrideURL.ToLower())
                                {
                                    if (eng)
                                    {
                                        tmpstring = jsonReader.ReadAsString();
                                        result.Versions.Add(PatchType.English, new Infos.VersionCheckResult(System.IO.Path.GetFileNameWithoutExtension(tmpstring), curEngVer));
                                    }
                                }
                                else if (jsonPropertyName == Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesTransAmDate.ToLower())
                                {
                                    if (largefiles)
                                        result.Versions.Add(PatchType.LargeFiles, new Infos.VersionCheckResult(jsonReader.ReadAsString(), curLargeFilesVer));
                                }
                                else if (jsonPropertyName == Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.StoryDate.ToLower())
                                {
                                    if (story)
                                        result.Versions.Add(PatchType.Story, new Infos.VersionCheckResult(jsonReader.ReadAsString(), curStoryVer));
                                }
                                else
                                    jsonReader.Skip();
                            }
                }
            }
            return result;
        }

        public void NotifyPatches()
        {
            this.OnEnglishPatchNotify(new PatchNotifyEventArgs(this.englishManager.VersionString));
            this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(this.largefilesManager.VersionString));
            this.OnStoryPatchNotify(new PatchNotifyEventArgs(this.storyManager.VersionString));
        }
        #endregion

        #region "English Patch"
        private Patches.EnglishPatchManager CreateEnglishManager()
        {
            Patches.EnglishPatchManager result = new Patches.EnglishPatchManager();
            result.ProgressBarStateChanged += result_ProgressBarStateChanged;
            result.CurrentProgressChanged += result_CurrentProgressChanged;
            result.CurrentTotalProgressChanged += result_CurrentTotalProgressChanged;
            result.CurrentStepChanged += EnglishPatchManager_CurrentStepChanged;
            result.PatchInstalled += EnglishPatchManager_PatchInstalled;
            result.PatchUninstalled += EnglishPatchManager_PatchUninstalled;
            result.HandledException += EnglishPatchManager_HandledException;
            return result;
        }

        private void EnglishPatchManager_CurrentStepChanged(object sender, StepEventArgs e)
        {
            this.OnStepChanged(new StepChangedEventArgs("[English Patch] " + e.Step));
        }

        public void InstallEnglishPatch()
        {
            if (CurrentTask == Task.None)
            {
                this.CurrentTask = Task.EnglishPatch;
                this.englishManager.InstallPatch();
            }
        }

        public void UninstallEnglishPatch()
        {
            if (CurrentTask == Task.None)
            {
                this.CurrentTask = Task.EnglishPatch;
                this.englishManager.UninstallPatch();
            }
        }

        private void EnglishPatchManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask));
            switch (this.CurrentTask)
            {
                case Task.EnglishPatch:
                    this.OnEnglishPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.EnglishVersion));
                    CurrentTask = Task.None;
                    break;
                case Task.UninstallAllPatches:
                    this.OnEnglishPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.EnglishVersion));
                    this.largefilesManager.RestoreBackup();
                    break;
            }
        }

        private void EnglishPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs("[English Patch] " + LanguageManager.GetMessageText("UninstalledEnglishPatch", "English Patch has been uninstalled successfully"), true));
                MySettings.Patches.EnglishVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnEnglishPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
            switch (this.CurrentTask)
            {
                case Task.EnglishPatch:
                    CurrentTask = Task.None;
                    break;
                case Task.UninstallAllPatches:
                    this.largefilesManager.RestoreBackup();
                    break;
            }
        }

        private void EnglishPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.EnglishVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs("[English Patch] " + LanguageManager.GetMessageText("InstalledEnglishPatch", "English Patch has been installed successfully"), true));
                this.OnEnglishPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }
            if (CurrentTask == Task.EnglishPatch)
                CurrentTask = Task.None;
        }
        #endregion

        #region "LargeFiles Patch"
        private Patches.LargeFilesPatchManager CreateLargeFilesManager()
        {
            Patches.LargeFilesPatchManager result = new Patches.LargeFilesPatchManager();
            result.ProgressBarStateChanged += result_ProgressBarStateChanged;
            result.CurrentProgressChanged += result_CurrentProgressChanged;
            result.CurrentTotalProgressChanged += result_CurrentTotalProgressChanged;
            result.CurrentStepChanged += LargeFilesPatchManager_CurrentStepChanged;
            result.PatchInstalled += LargeFilesPatchManager_PatchInstalled;
            result.PatchUninstalled += LargeFilesPatchManager_PatchUninstalled;
            result.HandledException += LargeFilesPatchManager_HandledException;
            return result;
        }

        private void LargeFilesPatchManager_CurrentStepChanged(object sender, StepEventArgs e)
        {
            this.OnStepChanged(new StepChangedEventArgs("[LargeFiles Patch] " + e.Step));
        }

        public void InstallLargeFilesPatch()
        {
            if (CurrentTask == Task.None)
            {
                this.CurrentTask = Task.LargeFilesPatch;
                this.largefilesManager.InstallPatch();
            }
        }

        public void UninstallLargeFilesPatch()
        {
            if (CurrentTask == Task.None)
            {
                this.CurrentTask = Task.LargeFilesPatch;
                this.largefilesManager.UninstallPatch();
            }
        }

        private void LargeFilesPatchManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask));
            switch (this.CurrentTask)
            {
                case Task.LargeFilesPatch:
                    this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.LargeFilesVersion));
                    CurrentTask = Task.None;
                    break;
                case Task.UninstallAllPatches:
                    this.storyManager.RestoreBackup();
                    break;
            }
        }

        private void LargeFilesPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs("[LargeFiles Patch] " + LanguageManager.GetMessageText("UninstalledLargeFilesPatch", "LargeFiles Patch has been uninstalled successfully"), true));
                MySettings.Patches.LargeFilesVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
            switch (this.CurrentTask)
            {
                case Task.LargeFilesPatch:
                    CurrentTask = Task.None;
                    break;
                case Task.UninstallAllPatches:
                    this.storyManager.RestoreBackup();
                    break;
            }
        }

        private void LargeFilesPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.LargeFilesVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs("[LargeFiles Patch] " + LanguageManager.GetMessageText("InstalledLargeFilesPatch", "LargeFiles Patch has been installed successfully"), true));
                this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }
            if (CurrentTask == Task.LargeFilesPatch)
                CurrentTask = Task.None;
        }
        #endregion

        #region "Story Patch"
        private Patches.StoryPatchManager CreateStoryManager()
        {
            Patches.StoryPatchManager result = new Patches.StoryPatchManager();
            result.ProgressBarStateChanged += result_ProgressBarStateChanged;
            result.CurrentProgressChanged += result_CurrentProgressChanged;
            result.CurrentTotalProgressChanged += result_CurrentTotalProgressChanged;
            result.CurrentStepChanged += StoryPatchManager_CurrentStepChanged;
            result.PatchInstalled += StoryPatchManager_PatchInstalled;
            result.PatchUninstalled += StoryPatchManager_PatchUninstalled;
            result.HandledException += StoryPatchManager_HandledException;
            return result;
        }

        private void StoryPatchManager_CurrentStepChanged(object sender, StepEventArgs e)
        {
            this.OnStepChanged(new StepChangedEventArgs("[Story Patch] " + e.Step));
        }

        private void result_CurrentTotalProgressChanged(object sender, ProgressEventArgs e)
        {
            this.OnCurrentTotalProgressChanged(e);
        }

        private void result_CurrentProgressChanged(object sender, ProgressEventArgs e)
        {
            this.OnCurrentProgressChanged(e);
        }

        private void result_ProgressBarStateChanged(object sender, ProgressBarStateChangedEventArgs e)
        {
            this.OnProgressBarStateChanged(e);
        }

        public void InstallStoryPatch()
        {
            if (CurrentTask == Task.None)
            {
                this.CurrentTask = Task.StoryPatch;
                this.storyManager.InstallPatch();
            }
        }

        public void UninstallStoryPatch()
        {
            if (CurrentTask == Task.None)
            {
                this.CurrentTask = Task.StoryPatch;
                this.storyManager.UninstallPatch();
            }
        }

        private void StoryPatchManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask));
            switch (this.CurrentTask)
            {
                case Task.StoryPatch:
                    this.OnStoryPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.StoryVersion));
                    this.CurrentTask = Task.None;
                    break;
                case Task.UninstallAllPatches:
                    this.CurrentTask = Task.PSO2Update;
                    this.mypso2updater.UpdateGame();
                    break;
            }
        }

        private void StoryPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs("[Story Patch] " + LanguageManager.GetMessageText("UninstalledStoryPatch", "Story Patch has been uninstalled successfully"), true));
                MySettings.Patches.StoryVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnStoryPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
            switch (this.CurrentTask)
            {
                case Task.StoryPatch:
                    this.CurrentTask = Task.None;
                    break;
                case Task.UninstallAllPatches:
                    this.CurrentTask = Task.PSO2Update;
                    this.mypso2updater.UpdateGame();
                    break;
            }
        }

        private void StoryPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.StoryVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs("[Story Patch] " + LanguageManager.GetMessageText("InstalledStoryPatch", "Story Patch has been installed successfully"), true));
                this.OnStoryPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }
            if (this.CurrentTask == Task.StoryPatch)
                this.CurrentTask = Task.None;
        }
        #endregion

        #region "PSO2"

        #region "Launch Game"
        public void LaunchPSO2Game()
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.CurrentTask = Task.LaunchGame;
                this.bWorker_GameStart.RunWorkerAsync(false);
            }
            else
                this.OnHandledException(new Components.PSO2Controller.PSO2HandledExceptionEventArgs(new System.ComponentModel.InvalidAsynchronousStateException(), this.CurrentTask));
        }
        public void LaunchPSO2GameAndWait()
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.CurrentTask = Task.LaunchGame;
                this.bWorker_GameStart.RunWorkerAsync(true);
            }
            else
                this.OnHandledException(new Components.PSO2Controller.PSO2HandledExceptionEventArgs(new System.ComponentModel.InvalidAsynchronousStateException(), this.CurrentTask));
        }

        private BackgroundWorker CreateBworkerGameStart()
        {
            BackgroundWorker bWorkerLaunchPSO2 = new BackgroundWorker();
            bWorkerLaunchPSO2.WorkerReportsProgress = false;
            bWorkerLaunchPSO2.WorkerSupportsCancellation = false;
            bWorkerLaunchPSO2.DoWork += BWorkerLaunchPSO2_DoWork;
            bWorkerLaunchPSO2.RunWorkerCompleted += BWorkerLaunchPSO2_RunWorkerCompleted;
            return bWorkerLaunchPSO2;
        }

        private void BWorkerLaunchPSO2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsBusy = false;
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            this.OnPSO2Launched(new PSO2LaunchedEventArgs(e.Error));
            this.CurrentTask = Task.None;
        }

        private void BWorkerLaunchPSO2_DoWork(object sender, DoWorkEventArgs e)
        {
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite, new InfiniteProgressBarProperties(false)));
            if (PSO2.CommonMethods.IsPSO2Folder(MySettings.PSO2Dir))
            {
                AIDA.ActivatePSO2Plugin();
                PSO2.CommonMethods.LaunchPSO2Ex((bool)e.Argument);
                AIDA.DeactivatePSO2Plugin();
            }
            else
                throw new System.IO.FileNotFoundException("PSO2 executable file is not found");
        }
        #endregion

        #region "PSO2 Update"
        private PSO2UpdateManager CreatePSO2UpdateManager()
        {
            PSO2UpdateManager result = new PSO2UpdateManager();
            result.HandledException += Mypso2updater_HandledException;
            result.CurrentTotalProgressChanged += Mypso2updater_CurrentTotalProgressChanged;
            result.CurrentProgressChanged += Mypso2updater_CurrentProgressChanged;
            result.CurrentStepChanged += Mypso2updater_CurrentStepChanged;
            result.ProgressBarStateChanged += Mypso2updater_ProgressBarStateChanged;
            result.PSO2Installed += Mypso2updater_PSO2Installed;
            return result;
        }

        private void Mypso2updater_ProgressBarStateChanged(object sender, ProgressBarStateChangedEventArgs e)
        {
            this.OnProgressBarStateChanged(e);
        }

        private void Mypso2updater_PSO2Installed(object sender, PSO2UpdateManager.PSO2NotifyEventArgs e)
        {
            if (!e.Cancelled)
                if (e.FailedList == null || e.FailedList.Count < 3)
                {
                    MySettings.Patches.EnglishVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                    MySettings.Patches.LargeFilesVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                    MySettings.Patches.StoryVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                    this.OnEnglishPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
                    this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
                    this.OnStoryPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
                }
            this.OnPSO2Installed(e);
        }

        private void Mypso2updater_CurrentStepChanged(object sender, StepEventArgs e)
        {
            this.OnStepChanged(new StepChangedEventArgs(e.Step));
        }

        private void Mypso2updater_CurrentProgressChanged(object sender, ProgressEventArgs e)
        {
            this.OnCurrentProgressChanged(e);
        }

        private void Mypso2updater_CurrentTotalProgressChanged(object sender, ProgressEventArgs e)
        {
            this.OnCurrentTotalProgressChanged(e);
        }

        private void Mypso2updater_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask));
            if (this.CurrentTask == Task.PSO2Update)
            {
                this.OnStoryPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.StoryVersion));
                this.CurrentTask = Task.None;
            }
        }

        private void UninstallAllEnglishPatches()
        {
            if (CurrentTask == Task.None)
            {
                this.CurrentTask = Task.UninstallAllPatches;
                this.englishManager.RestoreBackup();
            }
        }

        public Infos.VersionCheckResult CheckForPSO2Updates()
        {
            return this.mypso2updater.CheckForUpdates();
        }

        public void UpdatePSO2Client()
        {
            this.UninstallAllEnglishPatches();
            //this.mypso2updater.UpdateGame();
        }

        public void InstallPSO2(string newPSO2Path)
        {
            this.mypso2updater.InstallPSO2To(newPSO2Path);
        }
        #endregion
        #endregion

        #region "Properties"
        public string EnglishPatchVersion { get { return this.englishManager.VersionString; } }
        public bool IsEnglishPatchInstalled { get { return this.englishManager.IsInstalled; } }
        public string LargeFilesPatchVersion { get { return this.largefilesManager.VersionString; } }
        public bool IsLargeFilesPatchInstalled { get { return this.largefilesManager.IsInstalled; } }
        public string StoryPatchVersion { get { return this.storyManager.VersionString; } }
        public bool IsStoryPatchInstalled { get { return this.storyManager.IsInstalled; } }
        public bool IsPSO2Installed
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MySettings.PSO2Dir))
                    return false;
                else
                    return PSO2.CommonMethods.IsPSO2Folder(MySettings.PSO2Dir);
            }
        }
        #endregion

        #region "Events"
        public event EventHandler<PSO2LaunchedEventArgs> PSO2Launched;
        protected void OnPSO2Launched(PSO2LaunchedEventArgs e)
        {
            if (this.PSO2Launched != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PSO2Launched.Invoke(this, e); }), null);
        }
        public event EventHandler<PSO2UpdateManager.PSO2NotifyEventArgs> PSO2Installed;
        protected void OnPSO2Installed(PSO2UpdateManager.PSO2NotifyEventArgs e)
        {
            if (this.PSO2Installed != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PSO2Installed.Invoke(this, e); }), null);
        }
        public event EventHandler<PatchNotifyEventArgs> EnglishPatchNotify;
        protected void OnEnglishPatchNotify(PatchNotifyEventArgs e)
        {
            if (this.EnglishPatchNotify != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.EnglishPatchNotify.Invoke(this, e); }), null);
        }
        public event EventHandler<PatchNotifyEventArgs> LargeFilesPatchNotify;
        protected void OnLargeFilesPatchNotify(PatchNotifyEventArgs e)
        {
            if (this.LargeFilesPatchNotify != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.LargeFilesPatchNotify.Invoke(this, e); }), null);
        }
        public event EventHandler<PatchNotifyEventArgs> StoryPatchNotify;
        protected void OnStoryPatchNotify(PatchNotifyEventArgs e)
        {
            if (this.StoryPatchNotify != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.StoryPatchNotify.Invoke(this, e); }), null);
        }
        /*public event ProgressChangedEventHandler ProgressChanged;
        protected void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (this.ProgressChanged != null)
                this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressChanged.Invoke(this, e); }), null);
        }
        public delegate void RingNotifyEventHandler(object sender, VisibleNotifyEventArgs e);
        public event RingNotifyEventHandler RingNotify;
        protected void OnRingNotify(VisibleNotifyEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.RingNotify?.Invoke(this, e); }), null);
        }
        public delegate void ProgressBarNotifyEventHandler(object sender, VisibleNotifyEventArgs e);
        public event ProgressBarNotifyEventHandler ProgressBarNotify;
        protected void OnProgressBarNotify(VisibleNotifyEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressBarNotify?.Invoke(this, e); }), null);
        }*/
        public event EventHandler<StepChangedEventArgs> StepChanged;
        protected void OnStepChanged(StepChangedEventArgs e)
        {
            if (this.StepChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.StepChanged.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressBarStateChangedEventArgs> ProgressBarStateChanged;
        protected void OnProgressBarStateChanged(ProgressBarStateChangedEventArgs e)
        {
            if (this.ProgressBarStateChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressBarStateChanged.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressEventArgs> CurrentProgressChanged;
        protected void OnCurrentProgressChanged(ProgressEventArgs e)
        {
            if (this.CurrentProgressChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentProgressChanged.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressEventArgs> CurrentTotalProgressChanged;
        protected void OnCurrentTotalProgressChanged(ProgressEventArgs e)
        {
            if (this.CurrentTotalProgressChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentTotalProgressChanged.Invoke(this, e); }), null);
        }
        public event EventHandler<PSO2HandledExceptionEventArgs> HandledException;
        protected void OnHandledException(PSO2HandledExceptionEventArgs e)
        {
            if (this.HandledException != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.HandledException.Invoke(this, e); }), null);
        }
        #endregion

        #region "Internal Classes"
        public class VersionsCheckResults
        {
            public Dictionary<PatchType, Infos.VersionCheckResult> Versions { get; }
            public VersionsCheckResults()
            {
                this.Versions = new Dictionary<PatchType, Infos.VersionCheckResult>(3);
            }

            public VersionsCheckResults(IDictionary<PatchType, Infos.VersionCheckResult> list)
            {
                this.Versions = new Dictionary<PatchType, Infos.VersionCheckResult>(list);
            }
        }

        public class StepChangedEventArgs : EventArgs
        {
            public string Step { get; }

            public bool Final { get; }

            public StepChangedEventArgs(string msg) : this(msg, false) { }

            public StepChangedEventArgs(string msg, bool isFinal) : base()
            {
                this.Step = msg;
                this.Final = isFinal;
            }
        }

        public class PSO2HandledExceptionEventArgs : EventArgs
        {
            public Task LastTask { get; }
            public Exception Error { get; }
            public object UserToken { get; }
            public PSO2HandledExceptionEventArgs(Exception ex, Task task, object token) : base()
            {
                this.Error = ex;
                this.LastTask = task;
                this.UserToken = token;
            }

            public PSO2HandledExceptionEventArgs(Exception ex, Task task) : this(ex, task, null) { }

            public override string ToString()
            {
                return this.Error.Message;
            }
        }
        #endregion
    }
}
