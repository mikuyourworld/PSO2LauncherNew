using System;
using Leayal;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Events;
using PSO2ProxyLauncherNew.Forms.MyMainMenuCode;
using PSO2ProxyLauncherNew.Classes.Components.Patches;
using Newtonsoft.Json.Linq;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    [Flags]
    public enum PatchType : byte
    {
        None = 0,
        English = 1 << 0,
        LargeFiles = 1 << 1,
        Story = 1 << 2,
        Raiser = 1 << 3
    }

    public enum TroubleshootingType : byte
    {
        None = 0,
        ResetGameGuard = 1 << 0,
        EnableCensor = 1 << 1,
        DisableCensor = 1 << 2,
        CleanupWorkspace = 1 << 3,
        FixPermission = 1 << 4,
        FixFullPermission = 1 << 5,
        Filecheck = 1 << 6
    }

    [Flags]
    public enum Task : byte
    {
        None = 0,
        LaunchGame = 1 << 0,
        PSO2Update = 1 << 1,
        PrepatchUpdate = 1 << 2,
        InstallPSO2 = 1 << 3,
        UninstallPatches = 1 << 4,
        InstallPatches = 1 << 5,
        RestorePatches = 1 << 6,
        Troubleshooting = 1 << 7
    }

    class PSO2Controller
    {
        private System.Threading.SynchronizationContext syncContext;
        private EnglishPatchManager englishManager;
        private StoryPatchManager storyManager;
        private LargeFilesPatchManager largefilesManager;
        private PSO2UpdateManager mypso2updater;
        private BackgroundWorker bWorker_GameStart;
        private RaiserOrWateverPatchManager raisermanager;
        private PSO2.PSO2WorkspaceManager.PSO2WorkspaceManager _pso2workspacemanager;
        private PSO2.PrepatchManager.PrepatchManager _prepatchManager;

        public bool IsBusy { get { return (this.CurrentTask != Task.None); } }
        public Task CurrentTask { get; private set; }
        public PatchType WorkingPatch { get; private set; }
        public TroubleshootingType WorkingTroubleshooting { get; private set; }
        public PSO2Controller(System.Threading.SynchronizationContext _SyncContext)
        {
            this.CurrentTask = Task.None;
            this.syncContext = _SyncContext;
            this.englishManager = CreateEnglishManager();
            this.largefilesManager = CreateLargeFilesManager();
            this.storyManager = CreateStoryManager();
            this.mypso2updater = CreatePSO2UpdateManager();
            this.bWorker_GameStart = CreateBworkerGameStart();
            this.raisermanager = CreateRaiserOrWateverPatchManager();
            this._pso2workspacemanager = CreatePSO2WorkspaceManager();
            this._prepatchManager = CreatePrepatchManager();
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
                    this.raisermanager.CancelAsync();
                    break;
                case Task.PSO2Update:
                    this.CurrentTask = Task.None;
                    this.mypso2updater.CancelAsync();
                    break;
                case Task.InstallPSO2:
                    this.CurrentTask = Task.None;
                    this.mypso2updater.CancelAsync();
                    break;
                case Task.PrepatchUpdate:
                    this.CurrentTask = Task.None;
                    this._prepatchManager.CancelAsync();
                    break;
                case Task.UninstallPatches:
                    this.CurrentTask = Task.None;
                    this.WorkingPatch = PatchType.None;
                    this.storyManager.CancelAsync();
                    this.largefilesManager.CancelAsync();
                    this.englishManager.CancelAsync();
                    this.raisermanager.CancelAsync();
                    break;
                case Task.Troubleshooting:
                    this.CurrentTask = Task.None;
                    this.WorkingPatch = PatchType.None;
                    this._pso2workspacemanager.CancelAsync();
                    this.mypso2updater.CancelAsync();
                    break;
            }
        }
        private PatchType GetNextPatchWork(PatchType CurrentPatch)
        {
            this.WorkingPatch &= ~CurrentPatch;
            if (this.WorkingPatch == PatchType.None)
                return PatchType.None;
            else if ((this.WorkingPatch & PatchType.English) == PatchType.English)
                return PatchType.English;
            else if ((this.WorkingPatch & PatchType.LargeFiles) == PatchType.LargeFiles)
                return PatchType.LargeFiles;
            else if ((this.WorkingPatch & PatchType.Story) == PatchType.Story)
                return PatchType.Story;
            else if ((this.WorkingPatch & PatchType.Raiser) == PatchType.Raiser)
                return PatchType.Raiser;
            else
                return PatchType.None;
        }

        private TroubleshootingType GetNextTroubleshootingWork(TroubleshootingType CurrentTroubleshooting)
        {
            this.WorkingTroubleshooting &= ~CurrentTroubleshooting;
            if (this.WorkingTroubleshooting == TroubleshootingType.None)
                return TroubleshootingType.None;
            else if ((this.WorkingTroubleshooting & TroubleshootingType.ResetGameGuard) == TroubleshootingType.ResetGameGuard)
                return TroubleshootingType.ResetGameGuard;
            else if ((this.WorkingTroubleshooting & TroubleshootingType.EnableCensor) == TroubleshootingType.EnableCensor)
                return TroubleshootingType.EnableCensor;
            else if ((this.WorkingTroubleshooting & TroubleshootingType.DisableCensor) == TroubleshootingType.DisableCensor)
                return TroubleshootingType.DisableCensor;
            else if ((this.WorkingTroubleshooting & TroubleshootingType.CleanupWorkspace) == TroubleshootingType.CleanupWorkspace)
                return TroubleshootingType.CleanupWorkspace;
            else if ((this.WorkingTroubleshooting & TroubleshootingType.FixFullPermission) == TroubleshootingType.FixFullPermission)
            {
                this.WorkingTroubleshooting &= ~TroubleshootingType.FixPermission;
                return TroubleshootingType.FixFullPermission;
            }
            else if ((this.WorkingTroubleshooting & TroubleshootingType.FixPermission) == TroubleshootingType.FixPermission)
                return TroubleshootingType.FixPermission;
            else if ((this.WorkingTroubleshooting & TroubleshootingType.Filecheck) == TroubleshootingType.Filecheck)
                return TroubleshootingType.Filecheck;
            else
                return TroubleshootingType.None;
        }

        /*private Task GetNextTask(Task _currentTask)
        {
            this.CurrentTask &= ~_currentTask;
            if (this.CurrentTask == Task.None)
                return Task.None;
            else if ((this.CurrentTask & Task.InstallPatches) == Task.InstallPatches)
                return Task.InstallPatches;
            else if ((this.CurrentTask & Task.UninstallPatches) == Task.UninstallPatches)
                return Task.UninstallPatches;
            else if ((this.CurrentTask & Task.InstallPSO2) == Task.InstallPSO2)
                return Task.InstallPSO2;
            else if ((this.CurrentTask & Task.LaunchGame) == Task.LaunchGame)
                return Task.LaunchGame;
            else if ((this.CurrentTask & Task.PSO2Update) == Task.PSO2Update)
                return Task.PSO2Update;
            else if ((this.CurrentTask & Task.RestorePatches) == Task.RestorePatches)
                return Task.RestorePatches;
            else if ((this.CurrentTask & Task.Troubleshooting) == Task.Troubleshooting)
                return Task.Troubleshooting;
            else
                return Task.None;
        }//*/

        private void SeekNextWork()
        {
            this.DoTaskWork(false, this.WorkingPatch, this.WorkingTroubleshooting, this.dunduninstallPSO2Location);
        }

        private string dunduninstallPSO2Location;
        private void DoTaskWork(bool checkBusy, PatchType patch, TroubleshootingType trouleshootingtype, string installPSO2Location)
        {
            if (checkBusy && this.IsBusy) return;
            this.dunduninstallPSO2Location = installPSO2Location;
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
                        break;
                    case PatchType.Raiser:
                        if (this.pickedRaiserLanguageName == RaiserLanguageName.Auto)
                            this.raisermanager.InstallPatch();
                        else
                            this.raisermanager.InstallPatch(this.pickedRaiserLanguageName);
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
                    case PatchType.Raiser:
                        this.raisermanager.UninstallPatch();
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
            {
                this.mypso2updater.UpdateGame();
                return;
            }
            if ((this.CurrentTask & Task.PrepatchUpdate) == Task.PrepatchUpdate)
            {
                if (!string.IsNullOrWhiteSpace(installPSO2Location) && System.IO.Path.IsPathRooted(installPSO2Location))
                    this._prepatchManager.UpdatePrepatch(installPSO2Location);
                else
                    this._prepatchManager.UpdatePrepatch();
                return;
            }
            if ((this.CurrentTask & Task.Troubleshooting) == Task.Troubleshooting)
                switch (trouleshootingtype)
                {
                    case TroubleshootingType.Filecheck:
                        if (!string.IsNullOrEmpty(installPSO2Location))
                            this.mypso2updater.CheckLocalFiles(installPSO2Location);
                        else
                            this.mypso2updater.CheckLocalFiles();
                        return;
                    case TroubleshootingType.DisableCensor:
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate {
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                            CommonMethods.ToggleCensorFile(false);
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnTroubleshootingCompleted(new TroubleshootingCompletedEventArgs(TroubleshootingType.DisableCensor, true));
                            this.DoTaskWork(checkBusy, patch, this.GetNextTroubleshootingWork(TroubleshootingType.DisableCensor), installPSO2Location);
                        }));
                        return;
                    case TroubleshootingType.EnableCensor:
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate {
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                            string myDir = MySettings.PSO2Dir;
                            if (!CommonMethods.ToggleCensorFile(true, myDir))
                            {
                                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                                RunWorkerCompletedEventArgs result = CommonMethods.RedownloadCensorFile(myDir, null);
                                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                                if (result.Error != null || result.Cancelled)
                                    this.OnTroubleshootingCompleted(new TroubleshootingCompletedEventArgs(TroubleshootingType.EnableCensor, result));
                            }
                            else
                                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnTroubleshootingCompleted(new TroubleshootingCompletedEventArgs(TroubleshootingType.EnableCensor, true));
                            this.DoTaskWork(checkBusy, patch, this.GetNextTroubleshootingWork(TroubleshootingType.EnableCensor), installPSO2Location);
                        }));
                        return;
                    case TroubleshootingType.ResetGameGuard:
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate {
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                            RunWorkerCompletedEventArgs result = CommonMethods.FixGameGuardError(true, MySettings.PSO2Dir, null);
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnTroubleshootingCompleted(new TroubleshootingCompletedEventArgs(TroubleshootingType.ResetGameGuard, result));
                            this.DoTaskWork(checkBusy, patch, this.GetNextTroubleshootingWork(TroubleshootingType.ResetGameGuard), installPSO2Location);
                        }));
                        return;
                    case TroubleshootingType.CleanupWorkspace:
                        this._pso2workspacemanager.CleanUp(this._pso2workspacemanageroption);
                        return;
                    case TroubleshootingType.FixPermission:
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate {
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                            RunWorkerCompletedEventArgs result = CommonMethods.FixFilesPermission(false);
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnTroubleshootingCompleted(new TroubleshootingCompletedEventArgs(TroubleshootingType.FixPermission, result));
                            this.DoTaskWork(checkBusy, patch, this.GetNextTroubleshootingWork(TroubleshootingType.FixPermission), installPSO2Location);
                        }));
                        return;
                    case TroubleshootingType.FixFullPermission:
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate {
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                            RunWorkerCompletedEventArgs result = CommonMethods.FixFilesPermission(true);
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnTroubleshootingCompleted(new TroubleshootingCompletedEventArgs(TroubleshootingType.FixFullPermission, result));
                            this.DoTaskWork(checkBusy, patch, this.GetNextTroubleshootingWork(TroubleshootingType.FixFullPermission), installPSO2Location);
                        }));
                        return;
                    case TroubleshootingType.None:
                        this.CurrentTask &= ~Task.Troubleshooting;
                        break;
                }
            this.CurrentTask = Task.None;
            this.WorkingPatch = PatchType.None;
            this.WorkingTroubleshooting = TroubleshootingType.None;
        }

        public void OrderWork(Task installOrUninstall, PatchType AllWork, TroubleshootingType troubleshootingType, string pso2location)
        {
            this.WorkingPatch = AllWork;
            this.CurrentTask = installOrUninstall;
            this.WorkingTroubleshooting = troubleshootingType;
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.None), this.GetNextTroubleshootingWork(TroubleshootingType.None), pso2location);
        }

        private void DoTaskWork(bool checkBusy, PatchType patch)
        {
            this.DoTaskWork(checkBusy, patch, this.WorkingTroubleshooting, string.Empty);
        }

        public void OrderWork(Task installOrUninstall, PatchType AllWork)
        {
            this.OrderWork(installOrUninstall, AllWork, TroubleshootingType.None, string.Empty);
        }

        public void OrderWork(Task installOrUninstall, PatchType AllWork, string pso2location)
        {
            this.OrderWork(installOrUninstall, AllWork, TroubleshootingType.None, pso2location);
        }

        public void OrderWork(Task installOrUninstall, PatchType AllWork, TroubleshootingType troubleshootingType)
        {
            this.OrderWork(installOrUninstall, AllWork, troubleshootingType, string.Empty);
        }

        public void OrderWork(Task installOrUninstall, TroubleshootingType troubleshootingType)
        {
            this.OrderWork(installOrUninstall, PatchType.None, troubleshootingType, string.Empty);
        }

        public VersionsCheckResults CheckForPatchesVersionsAndWait()
        {
            VersionsCheckResults result = new VersionsCheckResults();
            //this.NotifyPatches();
            string curEngVer = this.englishManager.VersionString,
                curLargeFilesVer = this.largefilesManager.VersionString,
                curStoryVer = this.storyManager.VersionString,
                curRaiserVer = this.raisermanager.VersionString;
            bool eng = (!curEngVer.IsEqual(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString, true)),
                largefiles = (!curLargeFilesVer.IsEqual(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString, true)),
                story = (!curStoryVer.IsEqual(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString, true)),
                raiser = ((!string.IsNullOrWhiteSpace(curRaiserVer) && !curRaiserVer.IsEqual(Infos.DefaultValues.AIDA.Tweaker.Registries.NonePatchString, true)) || MySettings.Patches.RaiserEnabled);
            if (eng || largefiles || story || raiser)
                using (var theWebClient = WebClientManger.WebClientPool.GetWebClient_AIDA(true))
                {
                    string returnFromWeb = theWebClient.DownloadString(Classes.AIDA.WebPatches.PatchesInfos);
                    if (!string.IsNullOrWhiteSpace(returnFromWeb))
                    {
                        JObject jobj = JObject.Parse(returnFromWeb);
                        if (eng)
                        {
                            if (AIDA.BoolAIDASettings(jobj[Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.ENPatchOverride].Value<object>().ToString(), false))
                                result.Versions.Add(PatchType.English, new Infos.VersionCheckResult(System.IO.Path.GetFileNameWithoutExtension(jobj[Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.ENPatchOverrideURL].Value<object>().ToString()), curEngVer));
                            else
                            {
                                //string arghlexpatchjson = theWebClient.DownloadString(Infos.DefaultValues.Arghlex.Web.PatchesJson);
                                string arghlexpatchjson = theWebClient.DownloadString(ACF.EnglishPatchManualHome);
                                if (!string.IsNullOrEmpty(arghlexpatchjson))
                                {
                                    arghlexpatchjson = EnglishPatchManager.GetNewestENPatch(arghlexpatchjson);
                                    if (!string.IsNullOrEmpty(arghlexpatchjson))
                                        result.Versions.Add(PatchType.English, new Infos.VersionCheckResult(ACF.GetVersionFromURL(arghlexpatchjson), curEngVer));
                                }
                            }
                        }

                        if (largefiles)
                            result.Versions.Add(PatchType.LargeFiles, new Infos.VersionCheckResult(jobj[Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesTransAmDate].Value<object>().ToString(), curLargeFilesVer));
                        if (story)
                            result.Versions.Add(PatchType.Story, new Infos.VersionCheckResult(jobj[Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.StoryDate].Value<object>().ToString(), curStoryVer));
                    }
                    if (raiser)
                    {
                        string raiserjson = theWebClient.DownloadString(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.RaiserURL);
                        if (!string.IsNullOrWhiteSpace(raiserjson))
                            result.Versions.Add(PatchType.Raiser, new Infos.VersionCheckResult(RaiserOrWateverPatchManager.GetValueFromJson(raiserjson, RaiserOrWateverPatchManager.GetLangCode(MySettings.Patches.PatchLanguage)).MD5, curRaiserVer));
                    }
                }
            return result;
        }

        public void NotifyPatches(bool Sync)
        {
            this.OnEnglishPatchNotify(new PatchNotifyEventArgs(this.englishManager.VersionString));
            this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(this.largefilesManager.VersionString));
            this.OnStoryPatchNotify(new PatchNotifyEventArgs(this.storyManager.VersionString));
            if (Sync)
                this.threadOnRaiserPatchNotify();
            else
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((obj) => { this.threadOnRaiserPatchNotify(); }));
        }

        private void threadOnRaiserPatchNotify()
        {
            string pso2Dir = MySettings.PSO2Dir;
            if (!string.IsNullOrWhiteSpace(pso2Dir) && CommonMethods.IsPSO2Folder(pso2Dir))
            {
                bool ale = Leayal.IO.DirectoryHelper.IsFolderEmpty(System.IO.Path.Combine(pso2Dir, DefaultValues.Directory.RaiserPatchFolderName));
                if (!ale)
                {
                    ale = true;
                    foreach (string str in RaiserOrWateverPatchManager.RequiredPluginList)
                        if (!System.IO.File.Exists(System.IO.Path.Combine(pso2Dir, DefaultValues.Directory.PluginsFolderName, str)))
                        {
                            ale = false;
                            break;
                        }
                    if (!ale)
                        this.OnRaiserPatchNotify(new PatchNotifyEventArgs(LanguageManager.GetMessageText("PluginsNotEnabled", "Plugin(s) not enabled")));
                    else
                        this.OnRaiserPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.PatchLanguage.ToString()));
                }
                else
                    this.OnRaiserPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
            else
                this.OnRaiserPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
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
            this.OnEnglishPatchNotify(new PatchNotifyEventArgs(this.englishManager.VersionString));
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
            this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(this.largefilesManager.VersionString));
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
            this.OnStoryPatchNotify(new PatchNotifyEventArgs(this.storyManager.VersionString));
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

        #region "Raiser Patch"
        private Patches.RaiserOrWateverPatchManager CreateRaiserOrWateverPatchManager()
        {
            Patches.RaiserOrWateverPatchManager RaiserManager = new Patches.RaiserOrWateverPatchManager();
            RaiserManager.ProgressBarStateChanged += result_ProgressBarStateChanged;
            RaiserManager.CurrentProgressChanged += result_CurrentProgressChanged;
            RaiserManager.CurrentTotalProgressChanged += result_CurrentTotalProgressChanged;
            RaiserManager.CurrentStepChanged += RaiserManager_CurrentStepChanged;
            RaiserManager.PatchInstalled += RaiserManager_PatchInstalled;
            RaiserManager.PatchUninstalled += RaiserManager_PatchUninstalled;
            RaiserManager.HandledException += RaiserManager_HandledException;
            return RaiserManager;
        }

        private void RaiserManager_CurrentStepChanged(object sender, StepEventArgs e)
        {
            this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled}] " + e.Step));
        }

        RaiserLanguageName pickedRaiserLanguageName;
        public void InstallRaiserPatch()
        {
            if (!this.IsBusy)
            {
                this.pickedRaiserLanguageName = RaiserLanguageName.Auto;
                this.OrderWork(Task.InstallPatches, PatchType.Raiser);
            }
        }

        public void InstallRaiserPatch(RaiserLanguageName langName)
        {
            if (!this.IsBusy)
            {
                this.pickedRaiserLanguageName = langName;
                this.OrderWork(Task.InstallPatches, PatchType.Raiser);
            }
        }

        public void UninstallRaiserPatch()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.UninstallPatches, PatchType.Raiser);
        }

        private void RaiserManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask, PatchType.Raiser));
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.Raiser));
        }

        private void RaiserManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled}] " + string.Format(LanguageManager.GetMessageText("Uninstalled0Patch", "{0} has been uninstalled successfully"), Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled), true));
                this.OnRaiserPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.Raiser));
        }

        private void RaiserManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                if (this.pickedRaiserLanguageName == RaiserLanguageName.Auto || this.pickedRaiserLanguageName == RaiserLanguageName.AllPatch)
                    this.pickedRaiserLanguageName = MySettings.Patches.PatchLanguage;
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled}] " + string.Format(LanguageManager.GetMessageText("Installed0Patch", "{0} has been installed successfully"), Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled), true));
                this.OnRaiserPatchNotify(new PatchNotifyEventArgs(this.pickedRaiserLanguageName.ToString()));
            }
            this.pickedRaiserLanguageName = RaiserLanguageName.Auto;
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.Raiser));
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
                this.OnHandledException(new PSO2Controller.PSO2HandledExceptionEventArgs(new PSO2ControllerBusyException(LanguageManager.GetMessageText("PSO2Controller_BusyOnSomething", "The launcher is busy on something")), this.CurrentTask));
        }
        public void LaunchPSO2GameAndWait()
        {
            if (!this.IsBusy)
            {
                this.CurrentTask = Task.LaunchGame;
                this.bWorker_GameStart.RunWorkerAsync(true);
            }
            else
                this.OnHandledException(new PSO2Controller.PSO2HandledExceptionEventArgs(new PSO2ControllerBusyException(LanguageManager.GetMessageText("PSO2Controller_BusyOnSomething", "The launcher is busy on something")), this.CurrentTask));
        }

        private BackgroundWorker CreateBworkerGameStart()
        {
            BackgroundWorker bWorkerLaunchPSO2 = new BackgroundWorker();
            bWorkerLaunchPSO2.WorkerReportsProgress = false;
            bWorkerLaunchPSO2.WorkerSupportsCancellation = true;
            bWorkerLaunchPSO2.DoWork += BWorkerLaunchPSO2_DoWork;
            bWorkerLaunchPSO2.RunWorkerCompleted += BWorkerLaunchPSO2_RunWorkerCompleted;
            return bWorkerLaunchPSO2;
        }

        private void BWorkerLaunchPSO2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.CurrentTask = Task.None;
            this.WorkingPatch = PatchType.None;
            this.WorkingTroubleshooting = TroubleshootingType.None;

            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            this.OnPSO2Launched(new PSO2LaunchedEventArgs(e.Error, e.Result as string));
        }

        private void BWorkerLaunchPSO2_DoWork(object sender, DoWorkEventArgs e)
        {
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite, new InfiniteProgressBarProperties(false)));
            string pso2dir = MySettings.PSO2Dir;
            if (CommonMethods.IsPSO2Folder(pso2dir))
            {
                //Ensure Raiser is not disabled while it's being installed
                if (MySettings.Patches.RaiserEnabled)
                {
                    PSO2.PSO2Plugin.PSO2Plugin pi;
                    foreach (string guuh in RaiserOrWateverPatchManager.RequiredPluginList)
                    {
                        pi = PSO2.PSO2Plugin.PSO2PluginManager.Instance[guuh];
                        if (pi != null)
                        {
                            if (!pi.Enabled)
                                pi.Enabled = true;
                        }
                        else
                            this.OnStepChanged(new StepChangedEventArgs(string.Format(LanguageManager.GetMessageText("InstallingPatch_FailedEnableRequiredPlugin0", "Failed to find required plugin: {0}.\nPlease enable it as soon as possible. Otherwise the patch may not work correctly."), guuh)));
                    }
                }
                /* Ensure GameGuard is not deleted or empty, so that the game can be launched without CTD (no error)
                 * Store it in the cache storage, too. Because the file is rarely to be changed once in a while.
                 */
                System.IO.FileInfo fiDes = new System.IO.FileInfo(System.IO.Path.Combine(pso2dir, DefaultValues.Filenames.GameGuardDes));
                System.IO.FileInfo fiIni = new System.IO.FileInfo(System.IO.Path.Combine(pso2dir, DefaultValues.Filenames.GameGuardConfig));
                if ((!fiDes.Exists || fiDes.Length == 0) || (!fiIni.Exists || fiIni.Length == 0))
                {
                    this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent, new CircleProgressBarProperties(false, true)));
                    this.OnStepChanged(new StepChangedEventArgs(string.Format(LanguageManager.GetMessageText("Invalid0fileAtLaunchGame","Invalid {0} file. Redownloading"), DefaultValues.Filenames.GameGuardDes)));
                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(100));
                    RunWorkerCompletedEventArgs ex = CommonMethods.FixGameGuardError(fiDes.DirectoryName, (progress) => { this.OnCurrentProgressChanged(new ProgressEventArgs(progress)); return this.bWorker_GameStart.CancellationPending; });
                    this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                    if (ex.Error != null)
                        throw ex.Error;
                    else if (ex.Cancelled)
                    {
                        this.bWorker_GameStart.CancelAsync();
                        e.Cancel = true;
                    }
                }
                if (!this.bWorker_GameStart.CancellationPending)
                {
                    string exlauncherpath = MySettings.ExternalLauncherEXE;
                    if (MySettings.UseExternalLauncher && !string.IsNullOrWhiteSpace(exlauncherpath))
                    {
                        if (!System.IO.Path.IsPathRooted(exlauncherpath))
                            exlauncherpath = System.IO.Path.GetFullPath(exlauncherpath);
                        if (!MySettings.ExternalLauncherUseStrictMode)
                            AIDA.ActivatePSO2Plugin(pso2dir);
                        if (System.IO.File.Exists(exlauncherpath))
                        {
                            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                            {
                                proc.StartInfo.FileName = exlauncherpath;
                                string myargs = MySettings.ExternalLauncherArgs;
                                if (!string.IsNullOrWhiteSpace(myargs))
                                    proc.StartInfo.Arguments = myargs;
                                if (exlauncherpath.EndsWith(".bin"))
                                    proc.StartInfo.UseShellExecute = false;
                                if (!OSVersionInfo.IsVistaAndUp)
                                    proc.StartInfo.Verb = "runas";
                                proc.Start();
                                // Why wait for 1 second .... I don't know, let's just go with this for now
                                proc.WaitForExit(1000);
                            }
                        }
                        else
                        {
                            throw new System.IO.FileNotFoundException("Target launcher not found", exlauncherpath);
                        }
                    }
                    else
                    {
                        try
                        {
                            // First let virtual PSO2 Process load the Arks-Layer plugin manager
                            AIDA.ActivatePSO2Plugin(pso2dir);
                            CommonMethods.LaunchPSO2Ex(true);
                            if (MySettings.ReshadeSupport && CommonMethods.IsReshadeExists(pso2dir))
                            {
                                /* After the virtual process finished its job, replace the Arks-Layer plugin manager with reshade
                                 * This works because SEGA make PSO2 run the virtual process at first. Thank you, SEGA~
                                 */
                                CommonMethods.ActivateReshade(pso2dir);
                                // File is in use, because it's unlike the Arks-Layer's ddraw.dll
                                // CommonMethods.DeactivateReshade(pso2dir);
                            }
                            else
                            {
                                // Suppress the File-in-use exception. Should be a correct way ??? I don't really think so but let's just go along with it for now.
                                AIDA.DeactivatePSO2Plugin(pso2dir);
                            }
                        }
                        catch (System.IO.IOException ex)
                        {
                            throw new Exception(LanguageManager.GetMessageText("LaunchGameFailureIOEx", "Game is already started. Please wait for some minutes and try again later if the game doesn't show up."), ex);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            throw new Exception(LanguageManager.GetMessageText("LaunchGameFailureIOEx", "Game is already started. Please wait for some minutes and try again later if the game doesn't show up."));
                        }
                    }
                    e.Result = pso2dir;
                }
                else
                    e.Cancel = true;
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
            result.ValidPrepatchPrompt += PSO2UpdateManager_ValidPrepatchPrompt;
            result.InvalidPrepatchPrompt += PSO2UpdateManager_InvalidPrepatchPrompt;
            return result;
        }

        private void Mypso2updater_ProgressBarStateChanged(object sender, ProgressBarStateChangedEventArgs e)
        {
            this.OnProgressBarStateChanged(e);
        }

        private void Mypso2updater_PSO2Installed(object sender, PSO2NotifyEventArgs e)
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
            this.CurrentTask &= ~Task.InstallPSO2;
            this.CurrentTask &= ~Task.PSO2Update;
            this.WorkingTroubleshooting &= ~TroubleshootingType.Filecheck;
            this.OnPSO2Installed(e);
            this.SeekNextWork();
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
            this.CurrentTask &= ~Task.InstallPSO2;
            this.CurrentTask &= ~Task.PSO2Update;
            this.WorkingTroubleshooting &= ~TroubleshootingType.Filecheck;
            this.SeekNextWork();
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

        public void CheckPSO2ClientFiles()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.Troubleshooting | Task.RestorePatches, PatchType.English | PatchType.LargeFiles | PatchType.Story, TroubleshootingType.Filecheck);
            //this.mypso2updater.UpdateGame();
        }

        public void RequestInstallPSO2(System.Windows.Forms.IWin32Window parentForm)
        {
            if (!this.IsBusy)
                using (Leayal.Forms.FolderBrowseDialogEx fbe = new Leayal.Forms.FolderBrowseDialogEx())
                {
                    fbe.Description = "Select location where PSO2 will be installed";
                    string currentPSO2Dir = MySettings.PSO2Dir;
                    string lowerofparentPSO2Dir = string.Empty;
                    if (!string.IsNullOrEmpty(currentPSO2Dir))
                        lowerofparentPSO2Dir = Microsoft.VisualBasic.FileIO.FileSystem.GetParentPath(currentPSO2Dir).ToLower();
                    fbe.SelectedDirectory = currentPSO2Dir;
                    string lowerofcurrentpath, lowerofcurrentpso2dir = currentPSO2Dir.ToLower();
                    fbe.FolderBrowseDialogExSelectChanged += (sender, e) =>
                    {
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
                                    if (!string.IsNullOrWhiteSpace(currentPSO2Dir) && System.IO.Path.IsPathRooted(currentPSO2Dir) && lowerofcurrentpath.IndexOf(lowerofcurrentpso2dir) > -1)
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
                                string _pso2bindir = System.IO.Path.Combine(fbe.SelectedDirectory, "pso2_bin");
                                if (MetroFramework.MetroMessageBox.Show(parentForm, string.Format(LanguageManager.GetMessageText("RequestPSO2Install_ConfirmInstallation", "Are you sure you want to install PSO2 Client?\nInstalled Directory:\n{0}\n'pso2_bin' directory:\n{1}"), fbe.SelectedDirectory, _pso2bindir), "Question", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    this.OrderWork(Task.InstallPSO2, PatchType.None, _pso2bindir);
                                }
                                break;
                            case 0:
                                if (MetroFramework.MetroMessageBox.Show(parentForm, LanguageManager.GetMessageText("RequestPSO2Install_Repair", "Do you want to repair PSO2 Client?"), "Question", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                    this.UpdatePSO2Client();
                                break;
                            default:
                                MySettings.PSO2Dir = fbe.SelectedDirectory;
                                if (MetroFramework.MetroMessageBox.Show(parentForm, LanguageManager.GetMessageText("RequestPSO2Install_EnsureUpdated", "Do you want to perform files checking?\n(Perform checking recommended)"), "Question", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                    this.OrderWork(Task.InstallPSO2, PatchType.None, fbe.SelectedDirectory);
                                else
                                    this.OnPSO2Installed(new PSO2NotifyEventArgs(MySettings.PSO2Version, true));
                                break;
                        }
                    }
                }
        }
        #endregion

        #region "Prepatch Update"
        private PSO2.PrepatchManager.PrepatchManager CreatePrepatchManager()
        {
            PSO2.PrepatchManager.PrepatchManager PrepatchManager = new PSO2.PrepatchManager.PrepatchManager();
            PrepatchManager.CurrentProgressChanged += this.Mypso2updater_CurrentProgressChanged;
            PrepatchManager.CurrentTotalProgressChanged += this.Mypso2updater_CurrentTotalProgressChanged;
            PrepatchManager.HandledException += this.PrepatchManager_HandledException;
            PrepatchManager.ProgressBarStateChanged += this.Mypso2updater_ProgressBarStateChanged;
            PrepatchManager.CurrentStepChanged += this.Mypso2updater_CurrentStepChanged;

            PrepatchManager.PrepatchDownloaded += this.PrepatchManager_PrepatchDownloaded;

            return PrepatchManager;
        }

        public void PrepatchUpdate()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.PrepatchUpdate, PatchType.None);
        }

        public void PrepatchUpdate(string pso2path)
        {
            if (!this.IsBusy)
                this.OrderWork(Task.PrepatchUpdate, PatchType.None, pso2path);
        }

        public PSO2.PrepatchManager.PrepatchVersionCheckResult CheckForPrepatchUpdates()
        {
            return this._prepatchManager.CheckForUpdates();
        }

        private void PrepatchManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask));
            this.CurrentTask &= ~Task.PrepatchUpdate;
            this.SeekNextWork();
        }

        private void PSO2UpdateManager_InvalidPrepatchPrompt(object sender, InvalidPrepatchPromptEventArgs e)
        {
            this.OnInvalidPrepatchPrompt(e);
        }

        private void PSO2UpdateManager_ValidPrepatchPrompt(object sender, ValidPrepatchPromptEventArgs e)
        {
            this.OnValidPrepatchPrompt(e);
        }

        private void PrepatchManager_PrepatchDownloaded(object sender, PSO2NotifyEventArgs e)
        {
            this.OnPSO2PrepatchDownloaded(e);
            this.CurrentTask &= ~Task.PrepatchUpdate;
            this.SeekNextWork();
        }
        #endregion

        #region "Troubleshooting"
        public void RequestGameguardReset()
        {
            this.OrderWork(Task.Troubleshooting, TroubleshootingType.ResetGameGuard);
        }

        public void RequestToggleCensorFile(bool enable)
        {
            if (!this.IsBusy)
            {
                if (enable)
                    this.OrderWork(Task.Troubleshooting, TroubleshootingType.EnableCensor);
                else
                    this.OrderWork(Task.Troubleshooting, TroubleshootingType.DisableCensor);
            }
        }

        public void RequestFixPermission(bool fullfix)
        {
            if (!this.IsBusy)
            {
                if (fullfix)
                    this.OrderWork(Task.Troubleshooting, TroubleshootingType.FixFullPermission);
                else
                    this.OrderWork(Task.Troubleshooting, TroubleshootingType.FixPermission);
            }
        }

        private Forms.PSO2WorkspaceCleanupDialog _pso2workspacemanageroption;
        public void RequestWorkspaceCleanup(System.Windows.Forms.IWin32Window owner)
        {
            if (this._pso2workspacemanageroption != null) return;
            if (!this.IsBusy)
            {
                this._pso2workspacemanageroption = new Forms.PSO2WorkspaceCleanupDialog();
                if (this._pso2workspacemanageroption.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK)
                {
                    this.OrderWork(Task.Troubleshooting, TroubleshootingType.CleanupWorkspace);
                }
            }
        }

        private PSO2.PSO2WorkspaceManager.PSO2WorkspaceManager CreatePSO2WorkspaceManager()
        {
            PSO2.PSO2WorkspaceManager.PSO2WorkspaceManager result = new PSO2.PSO2WorkspaceManager.PSO2WorkspaceManager();
            result.CleanupFinished += Result_CleanupFinished;
            result.HandledException += PSO2WorkspaceManager_HandledException;
            result.ProgressCurrentChanged += result_CurrentProgressChanged;
            result.ProgressTotalChanged += result_CurrentTotalProgressChanged;
            result.ProgressBarStateChanged += result_ProgressBarStateChanged;
            result.StepChanged += Result_StepChanged;
            return result;
        }

        private void Result_CleanupFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!this._pso2workspacemanageroption.IsDisposed && !this._pso2workspacemanageroption.Disposing)
                this._pso2workspacemanageroption.Dispose();
            this._pso2workspacemanageroption = null;
            if (e.Error != null)
            {
                this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, Task.Troubleshooting));
                this.DoTaskWork(false, PatchType.None, this.GetNextTroubleshootingWork(TroubleshootingType.CleanupWorkspace), dunduninstallPSO2Location);
            }
            else if (e.Cancelled)
                this.CancelOperation();
            else
            {
                this.DoTaskWork(false, PatchType.None, this.GetNextTroubleshootingWork(TroubleshootingType.CleanupWorkspace), dunduninstallPSO2Location);
            }
        }

        private void Result_StepChanged(object sender, StepEventArgs e)
        {
            this.OnStepChanged(new StepChangedEventArgs(e.Step));
        }

        private void PSO2WorkspaceManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, Task.Troubleshooting));
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
        public event EventHandler<ValidPrepatchPromptEventArgs> ValidPrepatchPrompt;
        protected void OnValidPrepatchPrompt(ValidPrepatchPromptEventArgs e)
        {
            if (this.ValidPrepatchPrompt != null)
                this.syncContext?.Send(new System.Threading.SendOrPostCallback(delegate { this.ValidPrepatchPrompt.Invoke(this, e); }), null);
        }

        public event EventHandler<InvalidPrepatchPromptEventArgs> InvalidPrepatchPrompt;
        protected void OnInvalidPrepatchPrompt(InvalidPrepatchPromptEventArgs e)
        {
            if (this.InvalidPrepatchPrompt != null)
                this.syncContext?.Send(new System.Threading.SendOrPostCallback(delegate { this.InvalidPrepatchPrompt.Invoke(this, e); }), null);
        }

        public event EventHandler<PSO2LaunchedEventArgs> PSO2Launched;
        protected void OnPSO2Launched(PSO2LaunchedEventArgs e)
        {
            if (this.PSO2Launched != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PSO2Launched.Invoke(this, e); }), null);
        }
        public event EventHandler<PSO2NotifyEventArgs> PSO2Installed;
        protected void OnPSO2Installed(PSO2NotifyEventArgs e)
        {
            if (this.PSO2Installed != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PSO2Installed.Invoke(this, e); }), null);
        }
        public event EventHandler<PSO2NotifyEventArgs> PSO2PrepatchDownloaded;
        protected void OnPSO2PrepatchDownloaded(PSO2NotifyEventArgs e)
        {
            if (this.PSO2PrepatchDownloaded != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PSO2PrepatchDownloaded.Invoke(this, e); }), null);
        }
        public event EventHandler<PatchNotifyEventArgs> EnglishPatchNotify;
        protected void OnEnglishPatchNotify(PatchNotifyEventArgs e)
        {
            if (this.EnglishPatchNotify != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.EnglishPatchNotify.Invoke(this, e); }), null);
        }
        public event EventHandler<TroubleshootingCompletedEventArgs> TroubleshootingCompleted;
        protected void OnTroubleshootingCompleted(TroubleshootingCompletedEventArgs e)
        {
            if (this.TroubleshootingCompleted != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.TroubleshootingCompleted.Invoke(this, e); }), null);
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
        public event EventHandler<PatchNotifyEventArgs> RaiserPatchNotify;
        protected void OnRaiserPatchNotify(PatchNotifyEventArgs e)
        {
            if (this.RaiserPatchNotify != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.RaiserPatchNotify.Invoke(this, e); }), null);
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
                    case PatchType.Raiser:
                        return Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled;
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
