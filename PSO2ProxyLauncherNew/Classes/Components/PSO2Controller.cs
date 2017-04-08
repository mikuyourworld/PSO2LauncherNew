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
        LargeFiles = 1 << 2,
        Story = 1 << 3,
        Raiser = 1 << 4
    }

    [Flags]
    public enum Task : byte
    {
        None = 0,
        LaunchGame = 1 << 0,
        PSO2Update = 1 << 1,
        InstallPSO2 = 1 << 2,
        UninstallPatches = 1 << 3,
        InstallPatches = 1 << 4,
        RestorePatches = 1 << 5
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
            this.raisermanager = CreateRaiserOrWateverPatchManager();
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
                case Task.UninstallPatches:
                    this.CurrentTask = Task.None;
                    this.WorkingPatch = PatchType.None;
                    this.storyManager.CancelAsync();
                    this.largefilesManager.CancelAsync();
                    this.englishManager.CancelAsync();
                    this.raisermanager.CancelAsync();
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
            else if ((this.WorkingPatch & PatchType.Raiser) == PatchType.Raiser)
                return PatchType.Raiser;
            else
                return PatchType.None;
        }

        private void DoTaskWork(bool checkBusy, PatchType patch)
        {
            this.DoTaskWork(checkBusy, patch, string.Empty);
        }

        private void DoTaskWork(bool checkBusy, PatchType patch, string installPSO2Location)
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
                        break;
                    case PatchType.Raiser:
                        this.raisermanager.InstallPatch();
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
            if ((this.CurrentTask & Task.InstallPSO2) == Task.InstallPSO2)
            {
                if (!string.IsNullOrWhiteSpace(installPSO2Location) && System.IO.Path.IsPathRooted(installPSO2Location))
                    this.mypso2updater.InstallPSO2To(installPSO2Location);
                else
                    this.mypso2updater.UpdateGame();
                return;
            }
        }

        public void OrderWork(Task installOrUninstall, PatchType AllWork, string pso2location)
        {
            this.WorkingPatch = AllWork;
            this.CurrentTask = installOrUninstall;
            this.DoTaskWork(false, this.GetNextPatchWork(PatchType.None), pso2location);
        }

        public void OrderWork(Task installOrUninstall, PatchType AllWork)
        {
            this.OrderWork(installOrUninstall, AllWork, string.Empty);
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
                                string arghlexpatchjson = theWebClient.DownloadString(Infos.DefaultValues.Arghlex.Web.PatchesJson);
                                if (!string.IsNullOrEmpty(arghlexpatchjson))
                                {
                                    arghlexpatchjson = EnglishPatchManager.GetNewestENPatch(arghlexpatchjson);
                                    if (!string.IsNullOrEmpty(arghlexpatchjson))
                                        result.Versions.Add(PatchType.English, new Infos.VersionCheckResult(System.IO.Path.GetFileNameWithoutExtension(arghlexpatchjson), curEngVer));
                                }
                            }
                        }

                        if (largefiles)
                            result.Versions.Add(PatchType.LargeFiles, new Infos.VersionCheckResult(jobj[Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesTransAmDate].Value<object>().ToString(), curLargeFilesVer));
                        if (story)
                            result.Versions.Add(PatchType.Story, new Infos.VersionCheckResult(jobj[Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.StoryDate].Value<object>().ToString(), curStoryVer));
                        if (raiser)
                        {
                            string raiserjson = theWebClient.DownloadString(jobj[Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.RaiserURL].Value<object>().ToString());
                            if (!string.IsNullOrWhiteSpace(raiserjson))
                                result.Versions.Add(PatchType.Raiser, new Infos.VersionCheckResult(AIDA.FlatJsonFetch<string>(raiserjson, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.RaiserPatchMD5), curRaiserVer));
                        }
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
                        this.OnRaiserPatchNotify(new PatchNotifyEventArgs(LanguageManager.GetMessageText("Installed", "Installed")));
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

        public void InstallRaiserPatch()
        {
            if (!this.IsBusy)
                this.OrderWork(Task.InstallPatches, PatchType.Raiser);
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
                this.OnStepChanged(new StepChangedEventArgs($"[{Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled}] " + string.Format(LanguageManager.GetMessageText("Installed0Patch", "{0} has been installed successfully"), Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled), true));
                this.OnRaiserPatchNotify(new PatchNotifyEventArgs(LanguageManager.GetMessageText("Installed", "Installed")));
            }
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
            bWorkerLaunchPSO2.WorkerSupportsCancellation = true;
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
                System.IO.FileInfo fi = new System.IO.FileInfo(System.IO.Path.Combine(pso2dir, DefaultValues.Filenames.GameGuardDes));
                if (!fi.Exists || fi.Length == 0)
                {
                    this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent, new CircleProgressBarProperties(false, true)));
                    this.OnStepChanged(new StepChangedEventArgs(string.Format(LanguageManager.GetMessageText("Invalid0fileAtLaunchGame","Invalid {0} file. Redownloading"), DefaultValues.Filenames.GameGuardDes)));
                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(100));
                    RunWorkerCompletedEventArgs ex = CommonMethods.FixGameGuardError(fi.FullName, (progress) => { this.OnCurrentProgressChanged(new ProgressEventArgs(progress)); return this.bWorker_GameStart.CancellationPending; });
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
                    AIDA.ActivatePSO2Plugin(pso2dir);
                    CommonMethods.LaunchPSO2Ex((bool)e.Argument);
                    AIDA.DeactivatePSO2Plugin(pso2dir);
                }
                else
                    e.Cancel = true;
            }
            else
                throw new System.IO.FileNotFoundException("PSO2 executable file is not found");
        }

        private void Webc_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.OnCurrentProgressChanged(new ProgressEventArgs(e.ProgressPercentage));
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
            if (!this.IsBusy)
                using (FolderBrowseDialogEx fbe = new FolderBrowseDialogEx())
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
        public event EventHandler<PSO2NotifyEventArgs> PSO2Installed;
        protected void OnPSO2Installed(PSO2NotifyEventArgs e)
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
