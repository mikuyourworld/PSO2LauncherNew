using System;
using PSO2ProxyLauncherNew.Classes;
using PSO2ProxyLauncherNew.Forms.MyMainMenuCode;
using PSO2ProxyLauncherNew.Classes.Components;
using PSO2ProxyLauncherNew.Classes.Infos;
using PSO2ProxyLauncherNew.Classes.Events;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using MetroFramework;
using Leayal.Forms;
using System.IO;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class MyMainMenu : Classes.Controls.PagingForm
    {
        private SynchronizationContext SyncContext;
        private BackgroundWorker bWorker_tweakerWebBrowser_load, bWorker_Boot;
        private PSO2Controller _pso2controller;
        private SelfUpdate _selfUpdater;
        private Leayal.WMI.ProcessWatcher pso2processwatcher;
        private Control[] targetedButtons;

        public MyMainMenu()
        {
            InitializeComponent();
            this.labelLauncherVersion.Text = $"Version: {Leayal.AppInfo.AssemblyInfo.Version.Major}.{Leayal.AppInfo.AssemblyInfo.Version.Minor}.{Leayal.AppInfo.AssemblyInfo.Version.Build}.{Leayal.AppInfo.AssemblyInfo.Version.Revision}";
            this.Icon = Properties.Resources._1;

            if (!DesignMode)
            {
                this.LetsSetReverse();
                this.SelectedTab = this.panelMainMenu;
                this.optionSliderFormScale.ValueAvailableRange = new AvailableIntRange(Convert.ToInt32(Leayal.Forms.FormWrapper.ScalingFactor * 100), optionSliderFormScale.Maximum);
            }

            this.targetedButtons = new Control[] { this.EnglishPatchButton, this.LargeFilesPatchButton, this.StoryPatchButton, this.RaiserPatchButton,
                this.buttonPluginManager, this.buttonAllFunctions, this.launcherOption, this.buttonPSO2Option
            };

            this.SyncContext = SynchronizationContext.Current;
            Classes.Components.AbstractExtractor.SetSyncContext(this.SyncContext);

            this._selfUpdater = this.CreateSelfUpdate(this.SyncContext);
            this.bWorker_Boot = CreateBootUpWorker();
            this.bWorker_tweakerWebBrowser_load = CreatetweakerWebBrowser();
            this._pso2controller = CreatePSO2Controller();
            
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.HandledException += this.PSO2ProxyInstaller_HandledException;
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.ProxyInstalled += this.PSO2ProxyInstaller_ProxyInstalled;
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.ProxyUninstalled += this.PSO2ProxyInstaller_ProxyUninstalled;
            
            Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.CheckForPluginCompleted += this.PSO2PluginManager_CheckForPluginCompleted;

            PSO2PluginManager.FormInfo.FormLoaded += FormInfo_FormLoaded;

            if (!DesignMode)
            {
                this.OptionPanel_Load();
                this.PSO2OptionPanel_Initialize();
            }

            Leayal.Forms.SystemEvents.ScalingFactorChanged += SystemEvents_ScalingFactorChanged;
        }

        #region "SelfUpdate"
        private Classes.Components.SelfUpdate CreateSelfUpdate(SynchronizationContext _syncContext)
        {
            SelfUpdate result = new SelfUpdate(_syncContext);
            result.UpdaterUri = new Uri(DefaultValues.MyServer.Web.SelfUpdate_UpdaterUri);
            result.UpdateUri = new Uri(DefaultValues.MyServer.Web.SelfUpdate_UpdateUri);
            result.VersionUri = new Uri(DefaultValues.MyServer.Web.SelfUpdate_VersionUri);
            result.ProgressChanged += this._selfUpdater_ProgressChanged;
            result.HandledException += this._selfUpdater_HandledException;
            result.FoundNewVersion += this._selfUpdater_FoundNewVersion;
            result.CurrentStepChanged += this._selfUpdater_CurrentStepChanged;
            result.CheckCompleted += this._selfUpdater_CheckCompleted;
            result.BeginDownloadPatch += this._selfUpdater_BeginDownloadPatch;
            result.ProgressBarStateChanged += this.Result_ProgressBarStateChanged;
            return result;
        }

        private void _selfUpdater_CheckCompleted(object sender, EventArgs e)
        {
            this.ChangeProgressBarStatus(ProgressBarVisibleState.Infinite);
            this.bWorker_Boot.RunWorkerAsync();
        }

        private void _selfUpdater_CurrentStepChanged(object sender, StepEventArgs e)
        {
            this.PrintText(e.Step);
        }

        private void _selfUpdater_FoundNewVersion(object sender, SelfUpdate.NewVersionEventArgs e)
        {
            if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("SelfUpdater_PromptToUpdateMsg", "Found new {0} version {1}.\nDo you want to update?\n\nYes=Update (Yes, please)\nNo=Skip (STRONGLY NOT RECOMMENDED)"), Leayal.AppInfo.AssemblyInfo.AssemblyName, e.Version), "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                e.AllowUpdate = true;
                this.PrintText(string.Format(LanguageManager.GetMessageText("SelfUpdater_BeginUpdate", "Begin download update version {0}"), e.Version), RtfColor.Blue);
            }
            else
            {
                e.AllowUpdate = false;
                this.ChangeProgressBarStatus(ProgressBarVisibleState.Infinite);
                this.bWorker_Boot.RunWorkerAsync();
            }
        }

        private void _selfUpdater_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.PrintText(e.Error.Message, RtfColor.Red);
            MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("MyMainMenu_FailedCheckLauncherUpdates","Failed to check for PSO2Launcher updates. Reason: {0}"), e.Error.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            //this.Close();
        }

        private void _selfUpdater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.mainProgressBar.Value = e.ProgressPercentage;
        }

        private void _selfUpdater_BeginDownloadPatch(object sender, EventArgs e)
        {
            this.mainProgressBar.Maximum = 100;
        }
        #endregion

        #region "English Patch"
        private void PSO2Controller_EnglishPatchNotify(object sender, PatchNotifyEventArgs e)
        {
            this.EnglishPatchButton.Text = $"{Classes.Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled}: " + e.PatchVer;
            if (e.PatchVer.ToLower() == DefaultValues.AIDA.Tweaker.Registries.NoPatchString.ToLower())
                this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Red;
            else
                this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Green;
        }
        private void EnglishPatchButton_Click(object sender, EventArgs e)
        {
            Control myself = sender as Control;
            this.englishPatchContext.Tag = Classes.Components.PatchType.English;
            this.installToolStripMenuItem.Visible = true;
            this.raiserInstallToolStripMenuItem.Visible = false;
            this.englishPatchContext.Show(myself, 0, myself.Height);
        }
        #endregion

        #region "LargeFiles Patch"
        private void PSO2Controller_LargeFilesPatchNotify(object sender, PatchNotifyEventArgs e)
        {
            this.LargeFilesPatchButton.Text = $"{DefaultValues.AIDA.Strings.LargeFilesPatchCalled}: " + e.PatchVer;
            if (e.PatchVer.ToLower() == DefaultValues.AIDA.Tweaker.Registries.NoPatchString.ToLower())
                this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Red;
            else
                this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Green;
        }
        private void LargeFilesPatchButton_Click(object sender, EventArgs e)
        {
            Control myself = sender as Control;
            this.englishPatchContext.Tag = Classes.Components.PatchType.LargeFiles;
            this.installToolStripMenuItem.Visible = true;
            this.raiserInstallToolStripMenuItem.Visible = false;
            this.englishPatchContext.Show(myself, 0, myself.Height);
        }
        #endregion

        #region "Story Patch"
        private void StoryPatchButton_Click(object sender, EventArgs e)
        {
            Control myself = sender as Control;
            this.englishPatchContext.Tag = Classes.Components.PatchType.Story;
            this.installToolStripMenuItem.Visible = true;
            this.raiserInstallToolStripMenuItem.Visible = false;
            this.englishPatchContext.Show(myself, 0, myself.Height);
        }

        private void PSO2Controller_StoryPatchNotify(object sender, PatchNotifyEventArgs e)
        {
            this.StoryPatchButton.Text = $"{DefaultValues.AIDA.Strings.StoryPatchCalled}: " + e.PatchVer;
            if (e.PatchVer.ToLower() == DefaultValues.AIDA.Tweaker.Registries.NoPatchString.ToLower())
                this.StoryPatchButton.FlatAppearance.BorderColor = Color.Red;
            else
                this.StoryPatchButton.FlatAppearance.BorderColor = Color.Green;
        }
        #endregion

        #region "Raiser Patch"
        private void RaiserPatchButton_Click(object sender, EventArgs e)
        {
            Control myself = sender as Control;
            this.englishPatchContext.Tag = Classes.Components.PatchType.Raiser;
            if (!this.raiserInstallToolStripMenuItem.HasDropDownItems)
            {
                for (int i = 1; i < (int)Classes.Components.Patches.RaiserLanguageName.AllPatch; i++)
                {
                    Classes.Components.Patches.RaiserLanguageName castingObj = (Classes.Components.Patches.RaiserLanguageName)i;
                    this.raiserInstallToolStripMenuItem.DropDownItems.Add(castingObj.ToString(), null, new EventHandler((exsender, exevent) => 
                    {
                        ToolStripItem item = exsender as ToolStripItem;
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskInstallPatchRaiser", "Do you want to install the {0} patch?"), item.Text) + "\r\n" + string.Format(LanguageManager.GetMessageText("0PatchNotCompatibleWithOthers", "{0} may be NOT compatible with other patches. It is advised to NOT install any other patches beside {0}."), DefaultValues.AIDA.Strings.RaiserPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.RaiserPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.RaiserPatchButton.Text = $"{DefaultValues.AIDA.Strings.RaiserPatchCalled}: Installing";
                            this._pso2controller.InstallRaiserPatch(castingObj);
                        }
                    }));
                }
            }
            this.installToolStripMenuItem.Visible = false;
            this.raiserInstallToolStripMenuItem.Visible = true;
            this.englishPatchContext.Show(myself, 0, myself.Height);
        }

        private void PSO2Controller_RaiserPatchNotify(object sender, PatchNotifyEventArgs e)
        {
            this.RaiserPatchButton.Text = $"{DefaultValues.AIDA.Strings.RaiserPatchCalled}: " + e.PatchVer;
            if (e.PatchVer.ToLower() == DefaultValues.AIDA.Tweaker.Registries.NoPatchString.ToLower())
                this.RaiserPatchButton.FlatAppearance.BorderColor = Color.Red;
            else if (e.PatchVer == LanguageManager.GetMessageText("PluginsNotEnabled", "Plugin(s) not enabled"))
                this.RaiserPatchButton.FlatAppearance.BorderColor = Color.DarkGoldenrod;
            else
                this.RaiserPatchButton.FlatAppearance.BorderColor = Color.Green;
        }
        #endregion

        #region "PSO2"
        private void Result_TroubleshootingCompleted(object sender, TroubleshootingCompletedEventArgs e)
        {
            RunWorkerCompletedEventArgs args;
            switch (e.TroubleshootingType)
            {
                case TroubleshootingType.ResetGameGuard:
                    args = e.Result as RunWorkerCompletedEventArgs;
                    if (args != null)
                    {
                        if (args.Error != null)
                            this.PrintText(args.Error.Message, RtfColor.Red);
                        else if (args.Cancelled)
                        { }
                        else
                        {
                            this.PrintText(LanguageManager.GetMessageText("MyMainMenu_ResetGameGuard", "GameGuard has been reseted successfully."));
                        }
                    }
                    break;
                case TroubleshootingType.EnableCensor:
                    args = e.Result as RunWorkerCompletedEventArgs;
                    if (args != null)
                    {
                        if (args.Error != null)
                            this.PrintText(string.Format(LanguageManager.GetMessageText("MyMainMenu_EnableCensor_Error", "Error while restoring censorship file.\nReason: {0}"), args.Error.Message), RtfColor.Red);
                    }
                    else
                    {
                        bool enablelaiwghlawhg = (bool)e.Result;
                        if (enablelaiwghlawhg)
                        {
                            this.enableChatCensorshipToolStripMenuItem.Checked = true;
                            this.PrintText(LanguageManager.GetMessageText("MyMainMenu_EnableCensor", "Chat censorship has been restored."));
                        }
                    }
                    break;
                case TroubleshootingType.DisableCensor:
                    bool disableLAWighliasd = (bool)e.Result;
                    if (disableLAWighliasd)
                    {
                        this.enableChatCensorshipToolStripMenuItem.Checked = false;
                        this.PrintText(LanguageManager.GetMessageText("MyMainMenu_DisableCensor", "Chat censorship has been removed."));
                    }
                    break;
                case TroubleshootingType.CleanupWorkspace:
                    this.PrintText(LanguageManager.GetMessageText("MyMainMenu_CleanupWorkspace", "All PSO2 caches has been deleted and your PSO2 Settings has been reseted."));
                    break;
                case TroubleshootingType.FixFullPermission:
                    this.PrintText(LanguageManager.GetMessageText("MyMainMenu_FixFullPermission", "All the files and folders in the game folder has been changed."));
                    break;
                case TroubleshootingType.FixPermission:
                    this.PrintText(LanguageManager.GetMessageText("MyMainMenu_FixFullPermission", "All the executable files and folders in the game folder has been changed."));
                    break;
            }
        }

        private void PSO2PluginManager_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.PrintText("[PSO2PluginManager] " + e.Error.Message, RtfColor.Red);
        }

        private void resetGameGuardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("PSO2Controller_RequestGameguardReset", "Are you sure you want to reset Gameguard?\n(The PSO2 game and the GameGuard of any games must NOT be running)"), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    this._pso2controller.RequestGameguardReset();
        }

        private void removeChatCensorshipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._pso2controller.RequestToggleCensorFile(!this.enableChatCensorshipToolStripMenuItem.Checked);
        }

        private void InstallToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var formPSO2ProxyInstallForm = new PSO2ProxyInstallForm())
            {
                if (formPSO2ProxyInstallForm.ShowDialog(this) == DialogResult.OK)
                    if (formPSO2ProxyInstallForm.ConfigURL != null)
                        Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.Install(formPSO2ProxyInstallForm.ConfigURL);
            }
        }

        private void UninstallToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.Uninstall();
        }

        private void Result_PSO2Installed(object sender, PSO2NotifyEventArgs e)
        {
            if (e.FailedList != null && e.FailedList.Count > 0)
            {
                if (e.Installation && Classes.PSO2.CommonMethods.IsPSO2Folder(e.InstalledLocation))
                    this.SetGameStartState(GameStartState.GameInstalled);
                if (e.Cancelled)
                    this.PrintText(string.Format(LanguageManager.GetMessageText("Mypso2updater_InstallationCancelled", "Updating PSO2 client {0} has been cancelled. The download still have {1} files left."), e.NewClientVersion, e.FailedList.Count), RtfColor.Red);
                else
                    this.PrintText(string.Format(LanguageManager.GetMessageText("Mypso2updater_InstallationFailure", "PSO2 client version {0} has been downloaded but missing {1} files."), e.NewClientVersion, e.FailedList.Count), RtfColor.Red);
            }
            else
            {
                if (e.Installation)
                {
                    this.SetGameStartState(GameStartState.GameInstalled);
                    this.PrintText(string.Format(LanguageManager.GetMessageText("Mypso2updater_InstalledSuccessfully", "PSO2 client version {0} has been installed successfully"), e.NewClientVersion));
                }
                else
                    this.PrintText(string.Format(LanguageManager.GetMessageText("Mypso2updater_UpdatedSuccessfully", "PSO2 client has been updated to version {0} successfully"), e.NewClientVersion));
            }
            this.enableChatCensorshipToolStripMenuItem.Checked = Classes.PSO2.CommonMethods.IsCensorFileExist();
        }


        private void Result_PSO2Launched(object sender, PSO2LaunchedEventArgs e)
        {
            if (e.Error != null)
                this.PrintText(string.Format(LanguageManager.GetMessageText("PSO2Launched_FailedToLaunch", "Failed to launch PSO2. Reason: {0}"), e.Error.Message), RtfColor.Red);
            else
            {
                this.PrintText(LanguageManager.GetMessageText("PSO2Launched_Launched", "GAME STARTED!!!"), RtfColor.Green);
                if (MySettings.SteamSwitch || MySettings.SteamMode)
                {
                    if (!string.IsNullOrEmpty(e.GameFolder))
                    {
                        this.pso2processwatcher = new Leayal.WMI.ProcessWatcher(Path.Combine(e.GameFolder, "pso2.exe"), new EventHandler(this.Pso2processwatcher_ProcessLaunched));
                        this.pso2processwatcher.ProcessExited += Pso2processwatcher_ProcessExited;
                    }
                    this.PrintText(LanguageManager.GetMessageText("PSO2Launched_WithSteamMode", "This launcher will not close itself because of Steam Mode. Please close the launcher manually whenever you want to."), RtfColor.Green);
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void Pso2processwatcher_ProcessLaunched(object sender, EventArgs e)
        {
            this.Result_ProgressBarStateChanged(this, new ProgressBarStateChangedEventArgs(ProgressBarVisibleState.Infinite, new InfiniteProgressBarProperties(false)));
        }

        private void Pso2processwatcher_ProcessExited(object sender, EventArgs e)
        {
            this.pso2processwatcher.Dispose();
            this.pso2processwatcher = null;
            this.PrintText(LanguageManager.GetMessageText("PSO2Exited", "Game closed. Launcher is back to ready state."), RtfColor.Green);
            this.Result_ProgressBarStateChanged(this, new ProgressBarStateChangedEventArgs(ProgressBarVisibleState.None));
        }

        private void PSO2Controller_PSO2PrepatchDownloaded(object sender, PSO2NotifyEventArgs e)
        {
            if (e.FailedList != null && e.FailedList.Count > 0)
            {
                if (e.Cancelled)
                    this.PrintText(string.Format(LanguageManager.GetMessageText("PSO2Prepatch_DownloadCancelled", "Downloading pre-patch {0} has been cancelled. The download still have {1} files left."), e.NewClientVersion, e.FailedList.Count), RtfColor.Red);
                else
                    this.PrintText(string.Format(LanguageManager.GetMessageText("PSO2Prepatch_DownloadFailed", "PSO2 pre-patch version {0} has been downloaded but missing {1} files."), e.NewClientVersion, e.FailedList.Count), RtfColor.Red);
            }
            else
                this.PrintText(string.Format(LanguageManager.GetMessageText("PSO2Prepatch_DownloadedSuccess", "PSO2 pre-patch version {0} has been downloaded successfully."), e.NewClientVersion), RtfColor.Green);
        }

        private void PSO2Controller_InvalidPrepatchPrompt(object sender, InvalidPrepatchPromptEventArgs e)
        {
            if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("PSO2Prepatch_InvalidPrepatchPrompt", "Do you want to delete the out-dated or invalid prepatch files???"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                e.Delete = true;
            else
                e.Delete = false;
        }

        private void PSO2Controller_ValidPrepatchPrompt(object sender, ValidPrepatchPromptEventArgs e)
        {
            if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("PSO2Prepatch_ValidPrepatchPrompt", "Do you want to apply the downloaded prepatch files???"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                e.Use = true;
            else
                e.Use = false;
        }
        #endregion

        #region "Handle Events"
        private void PSO2PluginManager_CheckForPluginCompleted(object sender, CheckForPluginCompletedEventArgs e)
        {
            if (e.Error == null)
                this.PrintText(string.Format(LanguageManager.GetMessageText("PSO2PluginManager_CheckForPluginCompleted", "[PSO2PluginManager] Updated {0} plugin(s)."), e.PluginUpdatedCount), RtfColor.Green);
        }

        private void Result_HandledException(object sender, Classes.Components.PSO2Controller.PSO2HandledExceptionEventArgs e)
        {
            if (e.Error is PSO2ControllerBusyException)
            {
                this.PrintText(e.Error.Message, RtfColor.Red);
            }
            else
            {
                Leayal.Log.LogManager.GeneralLog.Print(e.Error);
                if (e.LastTask == Classes.Components.Task.InstallPatches || e.LastTask == Classes.Components.Task.UninstallPatches)
                    switch (e.LastPatch)
                    {
                        case Classes.Components.PatchType.English:
                            this.PrintText($"[{DefaultValues.AIDA.Strings.EnglishPatchCalled}]" + e.Error.Message, RtfColor.Red);
                            break;
                        case Classes.Components.PatchType.LargeFiles:
                            this.PrintText($"[{DefaultValues.AIDA.Strings.LargeFilesPatchCalled}]" + e.Error.Message, RtfColor.Red);
                            break;
                        case Classes.Components.PatchType.Story:
                            this.PrintText($"[{DefaultValues.AIDA.Strings.StoryPatchCalled}]" + e.Error.Message, RtfColor.Red);
                            break;
                    }
                else
                    this.PrintText(e.Error.Message, RtfColor.Red);
            }
        }
        private void Result_StepChanged(object sender, Classes.Components.PSO2Controller.StepChangedEventArgs e)
        {
            if (e.Final)
                this.PrintText(e.Step, RtfColor.Green);
            else
                this.PrintText(e.Step);
        }
        private void Result_ProgressBarStateChanged(object sender, ProgressBarStateChangedEventArgs e)
        {
            this.ChangeProgressBarStatus(e.ProgressBarState, e.Properties);
        }
        #endregion

        #region "Form Codes"
        private void MyMainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.optionToolTip.Dispose();
            if (this.pso2configFile != null)
            {
                this.pso2configFile.Dispose();
                this.pso2configFile = null;
            }
            if (this.pso2processwatcher != null)
            {

                this.pso2processwatcher.Dispose();
            }
            Leayal.Forms.SystemEvents.ScalingFactorChanged -= SystemEvents_ScalingFactorChanged;
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.HandledException -= this.PSO2ProxyInstaller_HandledException;
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.ProxyInstalled -= this.PSO2ProxyInstaller_ProxyInstalled;
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.ProxyUninstalled -= this.PSO2ProxyInstaller_ProxyUninstalled;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            this.optionToolTip.Hide();
            this.pso2optionToolTip.Hide();
        }

        private void checkForPSO2UpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("MyMainMenu_ConfirmUpdate", "Are you sure you want to perform updates check?\n(This task may take awhile)"), "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    this._pso2controller.UpdatePSO2Client();
        }

        private void checkForPrepatchUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("MyMainMenu_ConfirmPrepatchUpdate", "Are you sure you want to perform pre-patch check?\n(This task may take awhile)"), "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    this._pso2controller.CheckForPrepatchUpdates();
        }

        private void CheckForOldmissingFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("MyMainMenu_ConfirmCheckFiles", "Are you sure you want to perform files check?\n(This task may take awhile)"), "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    this._pso2controller.CheckPSO2ClientFiles();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!this.bWorker_Boot.IsBusy && !this._pso2controller.IsBusy)
                if (e.Button == MouseButtons.Right)
                    this.contextMenuAllFunctions.Show(this.panel1, e.Location);
        }

        private void FixPermissionQuickFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("PSO2Controller_RequestFixPermission", "Are you sure you want to quick repair files permission??\nThis will set neccessary file permission to all the .EXEs files in the game folders."), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    this._pso2controller.RequestFixPermission(false);
        }

        private void FixPermissionThroughoutFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("PSO2Controller_RequestFixFullPermission", "Are you sure you want to fully repair files permission??\nThis will set neccessary file permission to all the files in the game folders."), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    this._pso2controller.RequestFixPermission(true);
        }

        private void createShortcutForThisLauncherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var scs = Leayal.Shell.Shortcut.Load(@"C:\Users\Dramiel Leayal\Desktop\NohBoard.lnk");
            //MessageBox.Show(scs.TargetPath);
            //return;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = false;
                sfd.SupportMultiDottedExtensions = false;
                sfd.Title = "Select location to save the shortcut";
                sfd.CheckFileExists = false;
                sfd.CheckPathExists = true;
                sfd.DefaultExt = "lnk";
                sfd.Filter = "Shortcut File|*.lnk";
                sfd.AddExtension = true;
                sfd.FileName = "PSO2 Launcher.lnk";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    if (Leayal.Shell.Shortcut.CreateOverwrite(sfd.FileName, Leayal.AppInfo.ApplicationFilename, null, null, null, 0, "Launch Leayal's PSO2Launcher."))
                        MetroMessageBox.Show(this, LanguageManager.GetMessageText("MyMainMenu_ShortcutCreated", "The shortcut has been created successfully."), "Shortcut created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MetroMessageBox.Show(this, LanguageManager.GetMessageText("MyMainMenu_ShortcutCreateFailed", "Failed to create shortcut file."), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void openGameFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string pso2dir = MySettings.PSO2Dir, pso2exe = Path.Combine(pso2dir, "pso2.exe");
                if (File.Exists(pso2exe))
                    Leayal.Shell.Explorer.ShowAndHighlightItem(pso2exe);
                else
                    Leayal.Shell.Explorer.ShowAndHighlightItem(pso2dir);
            }
            catch (Exception ex)
            { MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void openGamesDocumentsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            { Leayal.Shell.Explorer.OpenFolder(Classes.PSO2.DefaultValues.Directory.DocumentWorkSpace); }
            catch (Exception ex)
            { MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void openGamesScreenshotFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            { Leayal.Shell.Explorer.OpenFolder(Path.Combine(Classes.PSO2.DefaultValues.Directory.DocumentWorkSpace, "pictures")); }
            catch (Exception ex)
            { MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void SplitContainer1_SplitterRatioChanged(object sender, EventArgs e)
        {
            MySettings.BottomSplitterRatio = this.splitContainer1.SplitterRatio;
        }

        private void resetPSO2SettingsAndClearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                this._pso2controller.RequestWorkspaceCleanup(this);
        }

        private void PanelMainMenu_SplitterRatioChanged(object sender, EventArgs e)
        {
            this.panelMainMenu.Panel1.BackColor = Color.Transparent;
            this.panel1.BackColor = Color.Transparent;
            this.panel1.GetNewCache();
            this.panel1.BackColor = this.BackColor;
            this.panelMainMenu.Panel1.BackColor = this.BackColor;
            this.gameStartButton1.BackColor = Color.Transparent;
            this.gameStartButton1.GetNewCache();
            this.gameStartButton1.BackColor = this.BackColor;
            MySettings.MainMenuSplitter = this.panelMainMenu.SplitterRatio;
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                _disposed = true;
            }
            base.Dispose(disposing);
            if (disposing)
            {
                if (mi != null)
                    mi.Dispose();
            }
        }        

        private void ButtonOptionPSO2_Click(object sender, EventArgs e)
        {
            this.contextMenuAllFunctions.Show(buttonAllFunctions, 0, buttonAllFunctions.Height);
        }

        private void PSO2ProxyInstaller_ProxyUninstalled(object sender, ProxyUninstalledEventArgs e)
        {
            this.PrintText("[Proxy] " + LanguageManager.GetMessageText("PSO2Proxy_UninstallSuccessfully", "Proxy has been uninstalled successfully"), RtfColor.Green);
        }

        private void PSO2ProxyInstaller_ProxyInstalled(object sender, ProxyInstalledEventArgs e)
        {
            this.PrintText($"[{e.Proxy.Version.ToString()}-Proxy] " + string.Format(LanguageManager.GetMessageText("PSO2Proxy_InstallSuccessfully", "{0} has been installed successfully"), e.Proxy.Name), RtfColor.Green);
        }

        private void PSO2ProxyInstaller_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.PrintText("[Proxy] " + LanguageManager.GetMessageText("PSO2Proxy_Failed", "Error while processing Proxy"), RtfColor.Red);
            MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("PSO2Proxy_FailedWithError", "Error while processing Proxy.\nAre you sure you gave correct URL which point to proxy config?\nError Message:\n{0}"), e.Error.Message), "Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            //this.LaunchCache();
            foreach (Control c in panel1.Controls)
                this.ReverseResize(c);
            base.OnClientSizeChanged(e);
        }

        private void ButtonPluginManager_Click(object sender, EventArgs e)
        {
            PSO2PluginManager.FormInfo.Show();
        }

        private void FormInfo_FormLoaded(object sender, FormLoadedEventArgs e)
        {
            e.SyncContext.Post(new SendOrPostCallback(delegate {
                var newPoint = this.DesktopLocation;
                newPoint.Offset(this.Width / 2, this.Height / 2);
                newPoint.Offset((PSO2PluginManager.FormInfo.Form.Width / 2) * -1, (PSO2PluginManager.FormInfo.Form.Height / 2) * -1);
                PSO2PluginManager.FormInfo.Form.DesktopLocation = newPoint;
            }), null);
        }

        public void LetsSetReverse()
        {
            foreach (Control c in panel1.Controls)
                this.SetReverse(c);
        }

        private void SetReverse(Control c)
        {
            Classes.Interfaces.ReserveRelativeLocation rrlc = c as Classes.Interfaces.ReserveRelativeLocation;
            if (rrlc != null)
            {
                if (c.MinimumSize.IsEmpty)
                    c.MinimumSize = c.Size;
                if (rrlc.RelativeLocation.IsEmpty)
                    rrlc.RelativeLocation = c.Location;
            }
        }

        private void ReverseResize(Control c)
        {
            Classes.Interfaces.ReserveRelativeLocation rrlc = c as Classes.Interfaces.ReserveRelativeLocation;
            if (rrlc != null)
            {
                if (!c.MinimumSize.IsEmpty)
                {
                    Size newSize = new Size(Convert.ToInt32(c.MinimumSize.Width * CommonMethods.ScaleFactor), Convert.ToInt32(c.MinimumSize.Height * CommonMethods.ScaleFactor));
                    c.Size = newSize;
                }
                if (!rrlc.RelativeLocation.IsEmpty)
                {
                    Point newPoint = new Point(Convert.ToInt32(rrlc.RelativeLocation.X * CommonMethods.ScaleFactor), Convert.ToInt32(rrlc.RelativeLocation.Y * CommonMethods.ScaleFactor));
                    c.Location = newPoint;
                }
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("MyMainMenu_CancelConfirm", "Are you sure you want to cancel current operation?.\nThis is highly NOT RECOMMENDED."), "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                this._pso2controller.CancelOperation();
        }

        private void ChangeProgressBarStatus(ProgressBarVisibleState val)
        { this.ChangeProgressBarStatus(val, null); }
        private void ChangeProgressBarStatus(ProgressBarVisibleState val, object _properties)
        {
            if (this._disposed) return;
            switch (val)
            {
                case ProgressBarVisibleState.Percent:
                    TaskbarItemInfo.TaskbarProgress.SetState(this, TaskbarItemInfo.TaskbarProgress.TaskbarStates.Normal);
                    CircleProgressBarProperties _circleProgressBarProperties = _properties as CircleProgressBarProperties;
                    if (_circleProgressBarProperties != null)
                        mainProgressBar.ShowSmallText = _circleProgressBarProperties.ShowSmallText;
                    this.ProgressBarPercent_Visible(true);
                    this.ProgressBarInfinite_Visible(false);
                    if (this.buttonCancel.Tag is string)
                        if (_circleProgressBarProperties == null || _circleProgressBarProperties.ShowCancel)
                            this.ButtonCancel_Visible(true);
                        else
                            this.ButtonCancel_Visible(false);
                    else
                        this.ButtonCancel_Visible(false);
                    this.gameStartButton1.Visible = false;
                    this.Buttons_Visible(false);
                    break;
                case ProgressBarVisibleState.Infinite:
                    TaskbarItemInfo.TaskbarProgress.SetState(this, TaskbarItemInfo.TaskbarProgress.TaskbarStates.Indeterminate);
                    InfiniteProgressBarProperties _infiniteProgressBarProperties = _properties as InfiniteProgressBarProperties;
                    this.ProgressBarPercent_Visible(false);
                    mainProgressBar.ShowSmallText = false;
                    this.ProgressBarInfinite_Visible(true);
                    if (this.buttonCancel.Tag is string)
                        if (_infiniteProgressBarProperties == null || _infiniteProgressBarProperties.ShowCancel)
                            this.ButtonCancel_Visible(true);
                        else
                            this.ButtonCancel_Visible(false);
                    else
                        this.ButtonCancel_Visible(false);
                    this.gameStartButton1.Visible = false;
                    this.Buttons_Visible(false);
                    break;
                default:
                    TaskbarItemInfo.TaskbarProgress.SetState(this, TaskbarItemInfo.TaskbarProgress.TaskbarStates.NoProgress);
                    this.gameStartButton1.Visible = true;
                    this.ProgressBarPercent_Visible(false);
                    mainProgressBar.ShowSmallText = false;
                    this.ButtonCancel_Visible(false);
                    this.ProgressBarInfinite_Visible(false);
                    this.Buttons_Visible(true);
                    break;
            }
        }

        private void Buttons_Visible(bool val)
        {
            for (int i = 0; i < this.targetedButtons.Length; i++)
                this.targetedButtons[i].Visible = val;
        }

        private void ButtonCancel_Visible(bool myBool)
        {
            buttonCancel.Visible = myBool;
            if (myBool)
                buttonCancel.BringToFront();
            else
                buttonCancel.SendToBack();
        }

        private void ProgressBarPercent_Visible(bool myBool)
        {
            mainProgressBar.Visible = myBool;
            if (myBool)
                mainProgressBar.BringToFront();
            else
                mainProgressBar.SendToBack();
        }
        private void ProgressBarInfinite_Visible(bool myBool)
        {
            mainFormLoadingHost.Visible = myBool;
            if (myBool)
                mainFormLoadingHost.BringToFront();
            else
                mainFormLoadingHost.SendToBack();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.mainFormLoading.SetRingColor(Color.DarkRed);
            LanguageManager.TranslateForm(this, this.gameStartButton1, this.EnglishPatchButton, this.LargeFilesPatchButton, this.StoryPatchButton, this.RaiserPatchButton);
            if (!DesignMode)
                this.LoadAppearenceSetting();
        }

        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.OnBackgroundImageChanged(e);
            this.LaunchCache();
        }

        public void LaunchCache(bool val)
        {
            if (this._disposed) return;
            this.SuspendLayout();

            this.panelMainMenu.BackColor = Color.Transparent;
            this.panelMainMenu.GetNewCache();
            this.panelMainMenu.Panel1.BackColor = Color.Transparent;
            this.panel1.BackColor = Color.Transparent;
            this.panel1.GetNewCache();
            this.gameStartButton1.GetNewCache();
            this.splitContainer1.Panel2.BackColor = Color.Transparent;
            this.tweakerWebBrowserLoading.BackColor = Color.Transparent;
            this.tweakerWebBrowserLoading.GetNewCache();
            
            //this.panelPSO2Option.BackColor = Color.Transparent;
            //this.panelPSO2Option.GetNewCache();

            if (val)
            {
                Color _color = this.BackColor;
                this.panelMainMenu.BackColor = _color;
                this.panel1.BackColor = _color;
                this.splitContainer1.Panel2.BackColor = _color;
                this.gameStartButton1.BackColor = _color;
                this.tweakerWebBrowserLoading.BackColor = _color;
                //this.panelPSO2Option.BackColor = _color;
                this.panelMainMenu.Panel1.BackColor = _color;
            }
            else
            {
                this.panelMainMenu.BackColor = Color.Transparent;
                this.panel1.BackColor = Color.Transparent;
                this.splitContainer1.Panel2.BackColor = Color.Transparent;
                this.gameStartButton1.BackColor = Color.Transparent;
                this.tweakerWebBrowserLoading.BackColor = Color.Transparent;
                //this.panelPSO2Option.BackColor = Color.Transparent;
                this.panelMainMenu.Panel1.BackColor = Color.Transparent;
            }

            this.ResumeLayout(false);

        }

        public void LaunchCache()
        {
            this.LaunchCache(true);
        }

        private void RelaunchCache()
        {
            return;
            this.panelMainMenu.BackColor = Color.Transparent;
            //this.panelMainMenu.GetNewCache();
            this.panel1.BackColor = Color.Transparent;
            this.panel1.GetNewCache();
            this.gameStartButton1.GetNewCache();
            this.panelMainMenu.BackColor = Color.FromArgb(17, 17, 17);
            this.panel1.BackColor = Color.FromArgb(17, 17, 17);
            this.gameStartButton1.BackColor = Color.FromArgb(17, 17, 17);
        }

        private void SetCacheBackgroundControls(Form _container)
        {
            if (_container.Controls != null && _container.Controls.Count > 0)
                foreach (Control c in _container.Controls)
                    SetCacheBackgroundControls(c);
        }

        private void SetCacheBackgroundControls(Control _container)
        {
            Classes.Interfaces.CacheBackground cccc;
            if (_container.Controls != null && _container.Controls.Count > 0)
            {
                cccc = _container as Classes.Interfaces.CacheBackground;
                if (cccc != null)
                {
                    _container.BackColor = this.BackColor;
                    _container.BackgroundImageLayout = ImageLayout.None;
                    cccc.CacheBackground = true;
                }
                foreach (Control c in _container.Controls)
                    SetCacheBackgroundControls(c);
            }
            else
            {
                cccc = _container as Classes.Interfaces.CacheBackground;
                if (cccc != null)
                {
                    _container.BackColor = this.BackColor;
                    _container.BackgroundImageLayout = ImageLayout.None;
                    cccc.CacheBackground = true;
                }
            }
        }

#if DEBUG
        private void Form_Shown(object sender, EventArgs e)
        {
            Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.HandledException += this.PSO2PluginManager_HandledException;
            this.LaunchCache();
            this.ChangeProgressBarStatus(ProgressBarVisibleState.Infinite);
            this.bWorker_Boot.RunWorkerAsync();
        }
#else
        private void Form_Shown(object sender, EventArgs e)
        {
            Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.HandledException += this.PSO2PluginManager_HandledException;
            this.LaunchCache();
            this.ChangeProgressBarStatus(ProgressBarVisibleState.Infinite);
            this._selfUpdater.CheckForUpdates();
            //this.bWorker_Boot.RunWorkerAsync();
        }
#endif

        public void PrintText(string msg)
        {
            Leayal.Log.LogManager.GetLogDefaultPath(DefaultValues.MyInfo.Filename.Log.PrintOut, string.Empty, false).Print(msg);
            this.LogRichTextBox.AppendText(msg);
        }
        public void PrintText(string msg, RtfColor textColor)
        {
            Leayal.Log.LogManager.GetLogDefaultPath(DefaultValues.MyInfo.Filename.Log.PrintOut, string.Empty, false).Print(msg);
            this.LogRichTextBox.AppendText(msg, textColor);
        }
        private Classes.Components.PSO2Controller CreatePSO2Controller()
        {
            Classes.Components.PSO2Controller result = new Classes.Components.PSO2Controller(this.SyncContext);
            result.HandledException += Result_HandledException;
            result.ProgressBarStateChanged += Result_ProgressBarStateChanged;
            result.StepChanged += Result_StepChanged;
            result.CurrentProgressChanged += Result_CurrentProgressChanged;
            result.CurrentTotalProgressChanged += Result_CurrentTotalProgressChanged;

            result.PSO2Installed += Result_PSO2Installed;
            result.PSO2Launched += Result_PSO2Launched;
            result.TroubleshootingCompleted += Result_TroubleshootingCompleted;
            result.EnglishPatchNotify += PSO2Controller_EnglishPatchNotify;
            result.LargeFilesPatchNotify += PSO2Controller_LargeFilesPatchNotify;
            result.StoryPatchNotify += PSO2Controller_StoryPatchNotify;
            result.RaiserPatchNotify += PSO2Controller_RaiserPatchNotify;

            result.ValidPrepatchPrompt += PSO2Controller_ValidPrepatchPrompt;
            result.InvalidPrepatchPrompt += PSO2Controller_InvalidPrepatchPrompt;
            result.PSO2PrepatchDownloaded += PSO2Controller_PSO2PrepatchDownloaded;
            
            return result;
        }

        private void Result_CurrentTotalProgressChanged(object sender, ProgressEventArgs e)
        {
            this.mainProgressBar.Maximum = e.Progress;
        }

        private void Result_CurrentProgressChanged(object sender, ProgressEventArgs e)
        {
            if (e.Progress <= this.mainProgressBar.Maximum)
            {
                this.mainProgressBar.Value = e.Progress;
                if (!this._disposed)
                    TaskbarItemInfo.TaskbarProgress.SetValue(this, e.Progress, this.mainProgressBar.Maximum);
            }
        }

        private void InstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
            {
                Classes.Components.PatchType currentTask = (Classes.Components.PatchType)this.englishPatchContext.Tag;
                switch (currentTask)
                {
                    case Classes.Components.PatchType.English:
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskInstallPatch", "Do you want to create backups and install the {0}?"), DefaultValues.AIDA.Strings.EnglishPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.EnglishPatchButton.Text = $"{DefaultValues.AIDA.Strings.EnglishPatchCalled}: Installing";
                            this._pso2controller.InstallEnglishPatch();
                        }
                        break;
                    case Classes.Components.PatchType.LargeFiles:
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskInstallPatch", "Do you want to create backups and install the {0}?"), DefaultValues.AIDA.Strings.LargeFilesPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.LargeFilesPatchButton.Text = $"{DefaultValues.AIDA.Strings.LargeFilesPatchCalled}: Installing";
                            this._pso2controller.InstallLargeFilesPatch();
                        }
                        break;
                    case Classes.Components.PatchType.Story:
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskInstallPatch", "Do you want to create backups and install the {0}?"), DefaultValues.AIDA.Strings.StoryPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.StoryPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.StoryPatchButton.Text = $"{DefaultValues.AIDA.Strings.StoryPatchCalled}: Installing";
                            this._pso2controller.InstallStoryPatch();
                        }
                        break;
                    case Classes.Components.PatchType.Raiser:
                        break;

                        // Discard this

                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskInstallPatch", "Do you want to install the {0}?"), DefaultValues.AIDA.Strings.RaiserPatchCalled) + "\r\n" + string.Format(LanguageManager.GetMessageText("0PatchNotCompatibleWithOthers", "{0} may be NOT compatible with other patches. It is advised to NOT install any other patches beside {0}."), DefaultValues.AIDA.Strings.RaiserPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.RaiserPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.RaiserPatchButton.Text = $"{DefaultValues.AIDA.Strings.RaiserPatchCalled}: Installing";
                            this._pso2controller.InstallRaiserPatch();
                        }
                }
            }
        }

        private void UninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
            {
                Classes.Components.PatchType currentTask = (Classes.Components.PatchType)this.englishPatchContext.Tag;
                switch (currentTask)
                {
                    case Classes.Components.PatchType.English:
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskUnnstallPatch", "Do you want to uninstall the {0}?"), DefaultValues.AIDA.Strings.StoryPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.EnglishPatchButton.Text = $"{DefaultValues.AIDA.Strings.EnglishPatchCalled}: Uninstalling";
                            this._pso2controller.UninstallEnglishPatch();
                        }
                        break;
                    case Classes.Components.PatchType.LargeFiles:
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskUnnstallPatch", "Do you want to uninstall the {0}?"), DefaultValues.AIDA.Strings.LargeFilesPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.LargeFilesPatchButton.Text = $"{DefaultValues.AIDA.Strings.LargeFilesPatchCalled}: Uninstalling";
                            this._pso2controller.UninstallLargeFilesPatch();
                        }
                        break;
                    case Classes.Components.PatchType.Story:
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskUnnstallPatch", "Do you want to uninstall the {0}?"), DefaultValues.AIDA.Strings.StoryPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.StoryPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.StoryPatchButton.Text = $"{DefaultValues.AIDA.Strings.StoryPatchCalled}: Uninstalling";
                            this._pso2controller.UninstallStoryPatch();
                        }
                        break;
                    case Classes.Components.PatchType.Raiser:
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskUnnstallPatch", "Do you want to uninstall the {0}?"), DefaultValues.AIDA.Strings.RaiserPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.RaiserPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.RaiserPatchButton.Text = $"{DefaultValues.AIDA.Strings.RaiserPatchCalled}: Uninstalling";
                            this._pso2controller.UninstallRaiserPatch();
                        }
                        break;
                }
            }
        }

        private void GameStartButton1_Click(object sender, EventArgs e)
        {
            switch (this.MyCurrentState)
            {
                case GameStartState.GameNotInstalled:
                    this._pso2controller.RequestInstallPSO2(this);
                    return;
                default:
                    this._pso2controller.LaunchPSO2GameAndWait();
                    return;
            }
        }

        internal enum GameStartState : byte { GameInstalled, GameNotInstalled }
        internal GameStartState MyCurrentState { get; private set; }
        private void SetGameStartState(GameStartState state)
        {
            if (this.MyCurrentState != state)
            {
                this.MyCurrentState = state;
                switch (state)
                {
                    case GameStartState.GameInstalled:
                        this.gameStartButton1.Text = "START";
                        this.SetEnabledControls(true, this.EnglishPatchButton, this.LargeFilesPatchButton, this.StoryPatchButton, this.RaiserPatchButton,
                            this.buttonPluginManager, this.buttonAllFunctions);
                        break;
                    case GameStartState.GameNotInstalled:
                        this.SetEnabledControls(false, this.EnglishPatchButton, this.LargeFilesPatchButton, this.StoryPatchButton, this.RaiserPatchButton,
                            this.buttonPluginManager, this.buttonAllFunctions);
                        this.gameStartButton1.Text = "INSTALL";
                        break;
                }
            }
        }

        private void SetEnabledControls(bool _enabled, params FakeControl[] c)
        {
            foreach (FakeControl ccc in c)
                ccc.Enabled = _enabled;
        }

        private void SetEnabledControls(bool _enabled, params Control[] c)
        {
            foreach (Control ccc in c)
                ccc.Enabled = _enabled;
        }

        private void SelectPSO2LocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                this._pso2controller.RequestInstallPSO2(this);
        }
        #endregion

        #region "Startup Codes"
        private BackgroundWorker CreateBootUpWorker()
        {
            BackgroundWorker result = new BackgroundWorker();
            result.WorkerSupportsCancellation = false;
            result.WorkerReportsProgress = false;
            result.DoWork += this.BWorker_Boot_DoWork;
            result.RunWorkerCompleted += this.BWorker_Boot_RunWorkerCompleted;
            return result;
        }

        private void Load7z()
        {
            string libPath = DefaultValues.MyInfo.Filename.SevenZip.SevenZipLibPath;
            
            if (!DefaultValues.MyInfo.Filename.SevenZip.IsValid)
            {
                this.PrintText(LanguageManager.GetMessageText("InvalidSevenZipLib", "SevenZip library is invalid or not existed. Redownloading"), RtfColor.Red);
                //WakeUpCall for 7z
                string url = DefaultValues.MyServer.Web.GetDownloadLink + "/" + System.IO.Path.ChangeExtension(DefaultValues.MyInfo.Filename.SevenZip.SevenZipLibName, ".7z");
                //this.SyncContext?.Send(new SendOrPostCallback(delegate { MessageBox.Show(url, "alwgihawligh"); }), null);
                WebClientPool.GetWebClient(DefaultValues.MyServer.Web.GetDownloadLink).DownloadFile(url, libPath + ".7z");
                using (FileStream fsss = new FileStream(libPath + ".7z", FileMode.Open, FileAccess.Read, FileShare.Read))
                using (SharpCompress.Archives.SevenZip.SevenZipArchive libPathArchive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(fsss))
                using (var reader = libPathArchive.ExtractAllEntries())
                    if (reader.MoveToNextEntry())
                        using (var fs = System.IO.File.Create(libPath))
                            reader.WriteEntryTo(fs);
                //Classes.Components.AbstractExtractor.(libPathArchive, Leayal.AppInfo.AssemblyInfo.DirectoryPath, null);
                try { System.IO.File.Delete(libPath + ".7z"); } catch { }
            }
            Classes.Components.AbstractExtractor.SetSevenZipLib(libPath);
            this.PrintText(LanguageManager.GetMessageText("SevenZipLibLoaded", "SevenZip library loaded successfully"), RtfColor.Green);
        }

        private void BWorker_Boot_DoWork(object sender, DoWorkEventArgs e)
        {
            //this.Load7z();

            //Ping AIDA for the server
            var PingAIDA = AIDA.GetIdeaServer();
            if (!AIDA.IsPingedAIDA)
            {
                string errormsg = string.Format(LanguageManager.GetMessageText("PingAidaFailed", "Failed to connect to Arks-Layer.\nAll the patches and plugins could not be checked for updates. For your safety, please restart launcher and try again. Error code: {0}"), (int)PingAIDA);
                this.PrintText(errormsg, RtfColor.Red);
                this.SyncContext?.Post(new SendOrPostCallback(delegate { MetroMessageBox.Show(this, errormsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }), null);
            }
            this.SyncContext?.Post(new SendOrPostCallback(delegate {
                this.refreshToolStripMenuItem.PerformClick();
                if (AIDA.IsPingedAIDA)
                {
                    Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.GetPluginList();
                }
            }), null);

            bool pso2installed, pso2update = false, updatepatches = false; ;
            string pso2Dir = MySettings.PSO2Dir;
            if (string.IsNullOrWhiteSpace(pso2Dir))
                pso2installed = false;
            else
                pso2installed = Classes.PSO2.CommonMethods.IsPSO2Folder(pso2Dir);
            
            Classes.Components.PatchType whichpatch = Classes.Components.PatchType.None;
            bool _updatepPrepatch = false;
            if (pso2installed)
            {
                bool isCensorExisted = Classes.PSO2.CommonMethods.IsCensorFileExist(pso2Dir);
                this.SyncContext?.Post(new SendOrPostCallback(delegate {
                    this.enableChatCensorshipToolStripMenuItem.Checked = isCensorExisted;
                }), null);
                var pso2versions = this._pso2controller.CheckForPSO2Updates();
                if (pso2versions.IsNewVersionFound)
                {
                    string pso2updater_FoundNewLatestVersion = string.Format(LanguageManager.GetMessageText("PSO2Updater_FoundNewLatestVersion", "Found new PSO2 client version: {0}.\nYour current version: {1}"), pso2versions.LatestVersion, pso2versions.CurrentVersion);
                    this.PrintText(pso2updater_FoundNewLatestVersion);
                    DialogResult pso2updateAnswer = DialogResult.No;
                    this.SyncContext?.Send(new SendOrPostCallback(delegate { pso2updateAnswer = MetroMessageBox.Show(this, pso2updater_FoundNewLatestVersion + "\n" + Classes.LanguageManager.GetMessageText("PSO2Updater_ConfirmToUpdate", "Do you want to perform update now?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }), null);
                    if (pso2updateAnswer == DialogResult.Yes)
                        pso2update = true;
                }
                else
                    this.PrintText(string.Format(LanguageManager.GetMessageText("PSO2Updater_AlreadyLatestVersion", "PSO2 Client is already latest version: {0}"), pso2versions.CurrentVersion), RtfColor.Green);
                
                this._pso2controller.NotifyPatches(true);

                if (MySettings.CheckForPrepatch)
                {
                    var pso2prepatchversion = this._pso2controller.CheckForPrepatchUpdates();
                    if (pso2prepatchversion.IsPrepatchExisted)
                    {
                        if (pso2prepatchversion.IsNewVersion)
                        {
                            DialogResult _updatepPrepatchAnswer = DialogResult.No;
                            string pso2updater_FoundNewLatestPrepatchVersion = string.Format(LanguageManager.GetMessageText("pso2updater_FoundNewLatestPrepatchVersion", "Found new PSO2 pre-patch version: {0}.\nYour current pre-patch version: {1}"), pso2prepatchversion.Latest.ToString(" revision "), pso2prepatchversion.Current.ToString(" revision "));
                            this.PrintText(pso2updater_FoundNewLatestPrepatchVersion);
                            this.SyncContext?.Send(new SendOrPostCallback(delegate { _updatepPrepatchAnswer = MetroMessageBox.Show(this, pso2updater_FoundNewLatestPrepatchVersion + "\n" + Classes.LanguageManager.GetMessageText("PSO2Updater_ConfirmToUpdate", "Do you want to perform update now?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }), null);
                            if (_updatepPrepatchAnswer == DialogResult.Yes)
                                _updatepPrepatch = true;
                        }
                        else
                            this.PrintText(string.Format(LanguageManager.GetMessageText("PSO2Updater_AlreadyLatestPrepatchVersion", "PSO2 pre-patch files are already at latest version: {0}"), pso2prepatchversion.Current.ToString(" revision ")), RtfColor.Green);
                    }
                    else
                        this.PrintText(LanguageManager.GetMessageText("PSO2Updater_FoundNoPrepatch", "There is no pre-patch existed yet."), RtfColor.Green);
                }

                if (!pso2update)
                {
                    if (AIDA.IsPingedAIDA)
                    {
                        var patchesversions = this._pso2controller.CheckForPatchesVersionsAndWait();
                        string patchname;
                        foreach (var ver in patchesversions.Versions)
                            if (ver.Value.IsNewVersionFound)
                            {
                                patchname = patchesversions.GetPatchName(ver.Key);
                                string pso2patches_FoundNewLatestVersion = string.Format(LanguageManager.GetMessageText("PSO2Patches_FoundNewLatestVersion", "Found new {0} version: {1}.\nYour current version: {2}"), patchname, ver.Value.LatestVersion, ver.Value.CurrentVersion);
                                this.SyncContext?.Send(new SendOrPostCallback(delegate
                                {
                                    if (MetroMessageBox.Show(this, pso2patches_FoundNewLatestVersion + "\n" + LanguageManager.GetMessageText("PSO2Patches_ConfirmToUpdate", "Do you want to perform update now?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        updatepatches = true;
                                        whichpatch |= ver.Key;
                                    }
                                }), null);
                            }
                    }
                }
            }
            else
                this._pso2controller.NotifyPatches(true);

            e.Result = new BootResult(pso2installed, pso2update, _updatepPrepatch, updatepatches, whichpatch);
        }

        private void BWorker_Boot_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Leayal.Log.LogManager.GeneralLog.Print(e.Error);
                this.PrintText(e.Error.Message, RtfColor.Red);
                MetroMessageBox.Show(this, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.ExitCode = 1;
                this.Close();
            }
            else
            {
                this.buttonCancel.Tag = "R";
                if (e.Result is BootResult)
                {
                    BootResult br = e.Result as BootResult;
                    if (br.IsPSO2Installed)
                    {
                        this.ChangeProgressBarStatus(ProgressBarVisibleState.None);
                        if (br.UpdatePrepatch)
                        {
                            Task workingTask = Task.PrepatchUpdate;
                            PatchType patchtype = PatchType.None;
                            if (br.UpdatePSO2)
                            {
                                workingTask &= (Task.PSO2Update | Task.RestorePatches);
                                patchtype = PatchType.English | PatchType.LargeFiles | PatchType.Story;
                            }
                            else if (br.UpdatePSO2Patches)
                            {
                                workingTask &= Task.InstallPatches;
                                patchtype = br.PSO2Patches;
                            }
                            this._pso2controller.OrderWork(workingTask, patchtype);
                        }
                        else
                        {
                            if (br.UpdatePSO2)
                                this._pso2controller.UpdatePSO2Client();
                            else if (br.UpdatePSO2Patches)
                                this._pso2controller.InstallPatches(br.PSO2Patches);
                        }
                    }
                    else
                    {
                        this.SetGameStartState(GameStartState.GameNotInstalled);
                        this.PrintText(LanguageManager.GetMessageText("PSO2NotInstalled", "PSO2 Client is not installed or recognized yet."), RtfColor.Red);
                        this.ChangeProgressBarStatus(ProgressBarVisibleState.None);
                    }
                }
            }
        }

        
        #endregion

        #region "Private Classes"
        private class BootResult
        {
            public bool IsPSO2Installed { get; }
            public bool UpdatePSO2 { get; }
            public bool UpdatePSO2Patches { get; }
            public Classes.Components.PatchType PSO2Patches { get; }
            public bool UpdatePrepatch { get; }
            public BootResult(bool pso2installed, bool _updatepso2, bool prepatch, bool _updatepatches) : this(pso2installed, _updatepso2, prepatch, _updatepatches, Classes.Components.PatchType.None) { }
            public BootResult(bool pso2installed, bool _updatepso2, bool prepatch, bool _updatepatches, Classes.Components.PatchType whichpatchtoupdate)
            {
                this.IsPSO2Installed = pso2installed;
                this.UpdatePSO2 = _updatepso2;
                this.UpdatePSO2Patches = _updatepatches;
                this.PSO2Patches = whichpatchtoupdate;
                this.UpdatePrepatch = prepatch;
            }
        }

        public enum ProgressBarVisibleState : short
        {
            None,
            Percent,
            Infinite
        }
        #endregion

        #region "Tweaker Browser Methods"
        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.bWorker_tweakerWebBrowser_load.RunWorkerAsync();
        }

        private void BWorker_tweakerWebBrowser_load_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TweakerWebBrowser_IsLoading(false);
            if (e.Error != null)
                Leayal.Log.LogManager.GeneralLog.Print(e.Error);
        }

        private void BWorker_tweakerWebBrowser_load_DoWork(object sender, DoWorkEventArgs e)
        {
            /*string linefiletoskip = Classes.AIDA.TweakerWebPanel.CutString;
            linefiletoskip = WebClientPool.GetWebClient(DefaultValues.MyServer.GetWebLink).DownloadString(DefaultValues.MyServer.GetWebLink + DefaultValues.MyServer.Web.TweakerSidePanelLiner);
            if (string.IsNullOrWhiteSpace(linefiletoskip))
                linefiletoskip = Classes.AIDA.TweakerWebPanel.CutString;*/
            TweakerWebBrowser_IsLoading(true);
            if (!AIDA.IsPingedAIDA)
                AIDA.GetIdeaServer();
            if (AIDA.IsPingedAIDA)
            {
                this.tweakerWebBrowser.LockNavigate = false;
                string resultofgettinghtmlfile = WebClientPool.GetWebClient_AIDA().DownloadString(AIDA.TweakerWebPanel.InfoPageLink);
                if (string.IsNullOrEmpty(resultofgettinghtmlfile))
                {
                    this.tweakerWebBrowser.Navigate(AIDA.TweakerWebPanel.InfoPageLink);
                    this.tweakerWebBrowser.LockNavigate = true;
                }
                else
                {
                    this.tweakerWebBrowser.LoadHTMLAsync(resultofgettinghtmlfile, (wbsender, wbe) => { this.tweakerWebBrowser.LockNavigate = true; });
                    //this.tweakerWebBrowser.EnglishPatchStatus = result.EnglishPatch;
                    //this.tweakerWebBrowser.ItemPatchStatus = result.ItemPatch;
                }//*/
            }
            else
            {                
                this.tweakerWebBrowser.LockNavigate = false;
                this.tweakerWebBrowser.LoadHTMLAsync(
@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <style>
        div{text-align:center;width:100%;font-weight:bold;font-family:""Times New Roman"",Times,serif;font-size:17px;}
        a:visited,a{color:red;}
    </style>
</head>
<body>
    <div>
        <span>Failed to connect to Arks-Layer's server.</span><br/>
        <a href=""leayal://retry"">Click me to retry</a>
    </div>
</body>
</html>
"
, (wbsender, wbe) => { this.tweakerWebBrowser.LockNavigate = true; });
            }
        }

        private void TweakerWebBrowser_CommandLink(object sender, StepEventArgs e)
        {
            switch (e.Step)
            {
                case "retry":
                    this.refreshToolStripMenuItem.PerformClick();
                    break;
            }
        }

        private BackgroundWorker CreatetweakerWebBrowser()
        {
            BackgroundWorker result = new BackgroundWorker();
            result.WorkerSupportsCancellation = false;
            result.WorkerReportsProgress = false;
            result.DoWork += BWorker_tweakerWebBrowser_load_DoWork;
            result.RunWorkerCompleted += BWorker_tweakerWebBrowser_load_RunWorkerCompleted;
            return result;
        }

        private void TweakerWebBrowser_LockedNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate {
                try
                { System.Diagnostics.Process.Start(e.Url.OriginalString); }
                catch (Exception ex)
                { Leayal.Log.LogManager.GeneralLog.Print(ex); }
            }));
        }

        public void TweakerWebBrowser_IsLoading(bool theBool)
        {
            this.SyncContext.Post(new SendOrPostCallback(delegate {
                this.tweakerWebBrowserLoading.Visible = theBool;
                foreach (ToolStripMenuItem item in this.tweakerWebBrowserContextMenu.Items)
                    item.Enabled = !theBool;
            }), null);
        }
        #endregion
    }
}
