using System;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Events;
using PSO2ProxyLauncherNew.Forms.MyMainMenuCode;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    [Flags]
    public enum PatchType : byte
    {
        None = 0,
        English = 1 << 0,
        LargeFiles = 1 << 2,
        Story = 1 << 3
    }

    [Flags]
    public enum Task : byte
    {
        None = 0,
        LaunchGame = 1 << 0,
        PSO2Update = 1 << 1,
        UninstallPatches = 1 << 2,
        InstallPatches = 1 << 3,
        RestorePatches = 1 << 4
    }

    class PSO2Controller
    {
        private System.Threading.SynchronizationContext syncContext;
        private Patches.EnglishPatchManager englishManager;
        private Patches.StoryPatchManager storyManager;
        private Patches.LargeFilesPatchManager largefilesManager;
        private PSO2UpdateManager mypso2updater;
        private BackgroundWorker bWorker_GameStart;

        public bool IsBusy { get { return (this.CurrentTask != Task.None); } }
        public Task CurrentTask { get; private set; }
        public PatchType WorkingPatch { get; private set; }
        public PSO2Controller(System.Threading.SynchronizationContext _SyncContext)
        {
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
                case Task.InstallPatches:
                    this.CurrentTask = Task.None;
                    this.WorkingPatch = PatchType.None;
                    this.storyManager.CancelAsync();
                    this.largefilesManager.CancelAsync();
                    this.englishManager.CancelAsync();
                    break;
                case Task.PSO2Update:
                    this.CurrentTask = Task.None;
                    this.mypso2updater.CancelAsync();
                    break;
                case Task.UninstallPatches:
                    this.CurrentTask = Task.None;
                    this.WorkingPatch = PatchType.None;
                    this.storyManager.CancelAsync();
                    this.largefilesManager.CancelAsync();
                    this.englishManager.CancelAsync();
                    break;
            }
        }
        private PatchType GetNextPatchWork(PatchType CurrentPatch)
        {
            this.WorkingPatch &= ~CurrentPatch;
            if ((this.WorkingPatch & PatchType.English) == PatchType.English)
                return PatchType.English;
            else if ((this.WorkingPatch & PatchType.LargeFiles) == PatchType.LargeFiles)
                return PatchType.LargeFiles;
            else if ((this.WorkingPatch & PatchType.Story) == PatchType.Story)
                return PatchType.Story;
            else
                return PatchType.None;
        }

        private void DoTaskWork(bool checkBusy, PatchType patch)
        {
            if (checkBusy && this.IsBusy) return;
            if ((this.CurrentTask & Task.InstallPatches) == Task.InstallPatches)
                switch (patch)
                {
                    case PatchType.English:
                        this.englishManager.InstallPatch();
                        return;
                    case PatchType.LargeFiles:
                        this.largefilesManager.InstallPatch();
                        return;
                    case PatchType.Story:
                        this.storyManager.InstallPatch();
                        return;
                    case PatchType.None:
                        this.CurrentTask &= ~Task.InstallPatches;
                        break;
                }
            if ((this.CurrentTask & Task.UninstallPatches) == Task.UninstallPatches)
                switch (patch)
                {
                    case PatchType.English:
                        this.englishManager.UninstallPatch();
                        return;
                    case PatchType.LargeFiles:
                        this.largefilesManager.UninstallPatch();
                        return;
                    case PatchType.Story:
                        this.storyManager.UninstallPatch();
                        return;
                    case PatchType.None:
                        this.CurrentTask &= ~Task.UninstallPatches;
                        break;
                }
            if ((this.CurrentTask & Task.RestorePatches) == Task.RestorePatches)
                switch (patch)
                {
                    case PatchType.English:
                        this.englishManager.RestoreBackup();
                        return;
                    case PatchType.LargeFiles:
                        this.largefilesManager.RestoreBackup();
                        return;
                    case PatchType.Story:
                        this.storyManager.RestoreBackup();
                        return;
                    case PatchType.None:
                        this.CurrentTask &= ~Task.RestorePatches;
                        break;
                }
            if ((this.CurrentTask & Task.PSO2Update) == Task.PSO2Update)
                this.mypso2updater.UpdateGame();
        }

        public void OrderWork(Task installOrUninstall, PatchType AllWork)
        {
            this.WorkingPatch = AllWork;
            this.CurrentTask = installOrUninstall;
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.None));
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
            this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled}] " + e.Step));
        }

        public void InstallEnglishPatch()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.InstallPatches, PatchType.English);
        }

        public void UninstallEnglishPatch()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.UninstallPatches, PatchType.English);
        }

        private void EnglishPatchManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask, PatchType.English));
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.English));
        }

        private void EnglishPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled}] " + LanguageManager.GetMessageText("UninstalledEnglishPatch", "English Patch has been uninstalled successfully"), true));
                MySettings.Patches.EnglishVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnEnglishPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.English));
        }

        private void EnglishPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.EnglishVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled}] " + LanguageManager.GetMessageText("InstalledEnglishPatch", "English Patch has been installed successfully"), true));
                this.OnEnglishPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.English));
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
            this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.LargeFilesPatchCalled}] " + e.Step));
        }

        public void InstallLargeFilesPatch()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.InstallPatches, PatchType.LargeFiles);
        }

        public void UninstallLargeFilesPatch()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.UninstallPatches, PatchType.LargeFiles);
        }

        private void LargeFilesPatchManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask, PatchType.LargeFiles));
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.LargeFiles));
        }

        private void LargeFilesPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.LargeFilesPatchCalled}] " + string.Format(LanguageManager.GetMessageText("Uninstalled0Patch", "{0} has been uninstalled successfully"), Infos.DefaultValues.AIDA.Strings.LargeFilesPatchCalled), true));
                MySettings.Patches.LargeFilesVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.LargeFiles));
        }

        private void LargeFilesPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.LargeFilesVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.LargeFilesPatchCalled}] " + string.Format(LanguageManager.GetMessageText("Installed0Patch", "{0} has been installed successfully"), Infos.DefaultValues.AIDA.Strings.LargeFilesPatchCalled), true));
                this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }

            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.LargeFiles));
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
            this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.StoryPatchCalled}] " + e.Step));
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
            if (!this.IsBusy)
                this.OrderWork(Task.InstallPatches, PatchType.Story);
        }

        public void UninstallStoryPatch()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.UninstallPatches, PatchType.Story);
        }

        private void StoryPatchManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask, PatchType.Story));
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.Story));
        }

        private void StoryPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.StoryPatchCalled}] " + string.Format(LanguageManager.GetMessageText("Uninstalled0Patch", "{0} has been uninstalled successfully"), Infos.DefaultValues.AIDA.Strings.StoryPatchCalled), true));
                MySettings.Patches.StoryVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnStoryPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.Story));
        }

        private void StoryPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.StoryVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.StoryPatchCalled}] " + string.Format(LanguageManager.GetMessageText("Installed0Patch", "{0} has been installed successfully"), Infos.DefaultValues.AIDA.Strings.StoryPatchCalled), true));
                this.OnStoryPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.Story));
        }
        #endregion

        #region "PSO2"

        #region "Launch Game"
        public void LaunchPSO2Game()
        {
            if (!this.IsBusy)
            {
                this.CurrentTask = Task.LaunchGame;
                this.bWorker_GameStart.RunWorkerAsync(false);
            }
            else
                this.OnHandledException(new PSO2Controller.PSO2HandledExceptionEventArgs(new System.ComponentModel.InvalidAsynchronousStateException(), this.CurrentTask));
        }
        public void LaunchPSO2GameAndWait()
        {
            if (!this.IsBusy)
            {
                this.CurrentTask = Task.LaunchGame;
                this.bWorker_GameStart.RunWorkerAsync(true);
            }
            else
                this.OnHandledException(new PSO2Controller.PSO2HandledExceptionEventArgs(new System.ComponentModel.InvalidAsynchronousStateException(), this.CurrentTask));
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
            {
                if (e.FailedList == null || e.FailedList.Count < 3)
                {
                    MySettings.Patches.EnglishVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                    MySettings.Patches.LargeFilesVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                    MySettings.Patches.StoryVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                    this.OnEnglishPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
                    this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
                    this.OnStoryPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
                }
                if (e.Installation && AIDA.IsPingedAIDA)
                    this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.GetPluginList(); }), null);
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

        private void UninstallPatches(PatchType _patches)
        {
            if (!this.IsBusy)
                this.OrderWork(Task.UninstallPatches, _patches);
        }

        public void InstallPatches(PatchType _patches)
        {
            if (!this.IsBusy)
                this.OrderWork(Task.InstallPatches, _patches);
        }

        public Infos.VersionCheckResult CheckForPSO2Updates()
        {
            return this.mypso2updater.CheckForUpdates();
        }

        public void UpdatePSO2Client()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.PSO2Update | Task.RestorePatches, PatchType.English | PatchType.LargeFiles | PatchType.Story);
            //this.mypso2updater.UpdateGame();
        }

        public void RequestInstallPSO2(System.Windows.Forms.IWin32Window parentForm)
        {
            using (FolderBrowseDialogEx fbe = new FolderBrowseDialogEx())
            {
                fbe.Description = "Select location where PSO2 will be installed";
                string currentPSO2Dir = MySettings.PSO2Dir;
                string lowerofparentPSO2Dir = string.Empty;
                if (!string.IsNullOrEmpty(currentPSO2Dir))
                    lowerofparentPSO2Dir = Microsoft.VisualBasic.FileIO.FileSystem.GetParentPath(currentPSO2Dir).ToLower();
                fbe.SelectedDirectory = currentPSO2Dir;
                string lowerofcurrentpath, lowerofcurrentpso2dir = currentPSO2Dir.ToLower();
                fbe.FolderBrowseDialogExSelectChanged += (sender, e) => {
                    if (string.IsNullOrWhiteSpace(e.CurrentPath))
                        e.SetOKButtonText("Install");
                    else
                    {
                        lowerofcurrentpath = e.CurrentPath.ToLower();
                        if (lowerofcurrentpso2dir == lowerofcurrentpath)
                        {
                            if (CommonMethods.IsPSO2Folder(lowerofcurrentpath))
                                e.SetOKButtonText("Repair");
                            else
                                e.SetOKButtonText("Install");
                        }
                        else
                        {
                            if (CommonMethods.IsPSO2Folder(e.CurrentPath))
                                e.SetOKButtonText("Select");
                            else if (CommonMethods.IsPSO2RootFolder(e.CurrentPath))
                            {
                                if (lowerofparentPSO2Dir == lowerofcurrentpath)
                                    e.SetOKButtonText("Repair");
                                else
                                    e.SetOKButtonText("Select");
                            }
                            else
                            {
                                if (lowerofcurrentpath.IndexOf(lowerofcurrentpso2dir) > -1)
                                    e.SetOKEnabled(false);
                                else
                                    e.SetOKEnabled(true);
                                e.SetOKButtonText("Install");
                            }
                        }
                    }
                };
                if (fbe.ShowDialog(parentForm) == System.Windows.Forms.DialogResult.OK)
                {
                    byte method = 2;
                    /*0=repair
                     *1=select
                     *2=install
                     */
                    lowerofcurrentpath = fbe.SelectedDirectory.ToLower();
                    if (lowerofcurrentpso2dir == lowerofcurrentpath)
                    {
                        if (CommonMethods.IsPSO2Folder(lowerofcurrentpath))
                            method = 0;
                        else
                            method = 2;
                    }
                    else
                    {
                        if (CommonMethods.IsPSO2Folder(fbe.SelectedDirectory))
                            method = 1;
                        else if (CommonMethods.IsPSO2RootFolder(fbe.SelectedDirectory))
                        {
                            if (lowerofparentPSO2Dir == lowerofcurrentpath)
                                method = 0;
                            else
                            {
                                fbe.SelectedDirectory = System.IO.Path.Combine(fbe.SelectedDirectory, "pso2_bin");
                                method = 1;
                            }
                        }
                        else
                            method = 2;
                    }
                    switch (method)
                    {
                        case 2:
                            if (MetroFramework.MetroMessageBox.Show(parentForm, string.Format(LanguageManager.GetMessageText("RequestPSO2Install_ConfirmInstallation", "Are you sure you want to install PSO2 Client?\nPath:\n{0}"), fbe.SelectedDirectory), "Question", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                this.mypso2updater.InstallPSO2To(fbe.SelectedDirectory);
                            break;
                        case 0:
                            if (MetroFramework.MetroMessageBox.Show(parentForm, LanguageManager.GetMessageText("RequestPSO2Install_Repair", "Do you want to repair PSO2 Client?"), "Question", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                this.mypso2updater.UpdateGame();
                            break;
                        default:
                            MySettings.PSO2Dir = fbe.SelectedDirectory;
                            if (MetroFramework.MetroMessageBox.Show(parentForm, LanguageManager.GetMessageText("RequestPSO2Install_EnsureUpdated", "Do you want to perform files checking?\n(Perform checking recommended)"), "Question", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                this.mypso2updater.UpdateGame(fbe.SelectedDirectory);
                            else
                                this.OnPSO2Installed(new PSO2UpdateManager.PSO2NotifyEventArgs(MySettings.PSO2Version, true));
                            break;
                    }
                }
            }
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
        public bool IsPSO2Installed { get { return PSO2.CommonMethods.IsPSO2Installed; } }
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

            public string GetPatchName(PatchType _type)
            {
                switch (_type)
                {
                    default:
                        return Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled;
                    case PatchType.LargeFiles:
                        return Infos.DefaultValues.AIDA.Strings.LargeFilesPatchCalled;
                    case PatchType.Story:
                        return Infos.DefaultValues.AIDA.Strings.StoryPatchCalled;
                }
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
            public PatchType LastPatch { get; }
            public Exception Error { get; }
            public object UserToken { get; }
            public PSO2HandledExceptionEventArgs(Exception ex, Task task, PatchType _patch, object token) : base()
            {
                this.Error = ex;
                this.LastTask = task;
                this.UserToken = token;
                this.LastPatch = _patch;
            }

            public PSO2HandledExceptionEventArgs(Exception ex, Task task, PatchType _patch) : this(ex, task, _patch, null) { }
            public PSO2HandledExceptionEventArgs(Exception ex, Task task) : this(ex, task, PatchType.None, null) { }

            public override string ToString()
            {
                return this.Error.Message;
            }
        }
        #endregion
    }
}
