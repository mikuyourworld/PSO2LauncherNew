using System;
using PSO2ProxyLauncherNew.Classes.Infos;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using PSO2ProxyLauncherNew.Classes.Events;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using MetroFramework;
using PSO2ProxyLauncherNew.Classes;
using PSO2ProxyLauncherNew.Forms.MyMainMenuCode;
using Leayal.Forms;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class MyMainMenu : Classes.Controls.PagingForm
    {
        private SynchronizationContext SyncContext;
        private BackgroundWorker bWorker_tweakerWebBrowser_load, bWorker_Boot;
        private Classes.Components.PSO2Controller _pso2controller;
        private Classes.Components.SelfUpdate _selfUpdater;
        private Control[] targetedButtons;

        public MyMainMenu()
        {
            InitializeComponent();
            this.Icon = Properties.Resources._1;

            if (!DesignMode)
            {
                this.LoadAppearenceSetting();
                this.SelectedTab = this.panelMainMenu;
                this.panelMainMenu.SplitterRatio = MySettings.MainMenuSplitter;
                this.splitContainer1.SplitterRatio = MySettings.BottomSplitterRatio;
                this.optionSliderFormScale.ValueAvailableRange = new AvailableIntRange(Convert.ToInt32(Leayal.Forms.FormWrapper.ScalingFactor * 100), optionSliderFormScale.Maximum);
            }

            this.targetedButtons = new Control[] { this.EnglishPatchButton, this.LargeFilesPatchButton, this.StoryPatchButton, this.RaiserPatchButton,
                this.buttonPluginManager, this.buttonOptionPSO2, this.launcherOption
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
            PSO2PluginManager.FormInfo.FormLoaded += FormInfo_FormLoaded;

            this.OptionPanel_Load();
            Leayal.Forms.SystemEvents.ScalingFactorChanged += SystemEvents_ScalingFactorChanged;
        }

        #region "SelfUpdate"
        private Classes.Components.SelfUpdate CreateSelfUpdate(SynchronizationContext _syncContext)
        {
            Classes.Components.SelfUpdate result = new Classes.Components.SelfUpdate(_syncContext);
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

        private void _selfUpdater_FoundNewVersion(object sender, Classes.Components.SelfUpdate.NewVersionEventArgs e)
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
            this.englishPatchContext.Show(myself, 0, myself.Height);
        }
        #endregion

        #region "Story Patch"
        private void StoryPatchButton_Click(object sender, EventArgs e)
        {
            Control myself = sender as Control;
            this.englishPatchContext.Tag = Classes.Components.PatchType.Story;
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
        private void installToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var formPSO2ProxyInstallForm = new PSO2ProxyInstallForm())
            {
                if (formPSO2ProxyInstallForm.ShowDialog(this) == DialogResult.OK)
                    if (formPSO2ProxyInstallForm.ConfigURL != null)
                        Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.Install(formPSO2ProxyInstallForm.ConfigURL);
            }
        }

        private void uninstallToolStripMenuItem1_Click(object sender, EventArgs e)
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
        }

        private void Result_PSO2Launched(object sender, PSO2LaunchedEventArgs e)
        {
            if (e.Error != null)
                this.PrintText(string.Format(LanguageManager.GetMessageText("PSO2Launched_FailedToLaunch", "Failed to launch PSO2. Reason: {0}"), e.Error.Message), RtfColor.Red);
            else
            {
                this.PrintText(LanguageManager.GetMessageText("PSO2Launched_Launched", "GAME STARTED!!!"), RtfColor.Green);
                this.Close();
            }
        }
        #endregion

        #region "Handle Events"
        private void Result_HandledException(object sender, Classes.Components.PSO2Controller.PSO2HandledExceptionEventArgs e)
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
            Leayal.Forms.SystemEvents.ScalingFactorChanged -= SystemEvents_ScalingFactorChanged;
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.HandledException -= this.PSO2ProxyInstaller_HandledException;
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.ProxyInstalled -= this.PSO2ProxyInstaller_ProxyInstalled;
            Classes.PSO2.PSO2Proxy.PSO2ProxyInstaller.Instance.ProxyUninstalled -= this.PSO2ProxyInstaller_ProxyUninstalled;
        }

        private void checkForOldmissingFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
                if (MetroMessageBox.Show(this, LanguageManager.GetMessageText("MyMainMenu_ConfirmCheckFiles", "Are you sure you want to perform files check?\n(This task may take awhile)"), "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    this._pso2controller.UpdatePSO2Client();
        }

        private void splitContainer1_SplitterRatioChanged(object sender, EventArgs e)
        {
            MySettings.BottomSplitterRatio = this.splitContainer1.SplitterRatio;
        }

        private void panelMainMenu_SplitterMoved(object sender, SplitterEventArgs e)
        {
            //let's do nothing here
            return; //this line increase the compiled executable for NOTHING
        }

        private void panelMainMenu_SplitterRatioChanged(object sender, EventArgs e)
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
        }

        private void launcherOption_Click(object sender, EventArgs e)
        {
            this.RefreshOptionPanel();
            this.SelectedTab = this.panelOption;
        }

        private void optionButtonOK_Click(object sender, EventArgs e)
        {
            this.SaveOptionSettings();
            this.SelectedTab = this.panelMainMenu;
        }

        private void buttonOptionPSO2_Click(object sender, EventArgs e)
        {
            this.contextMenuPSO2GameOption.Show(buttonOptionPSO2, 0, buttonOptionPSO2.Height);
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
            this.launchCache();
            foreach (Control c in panel1.Controls)
                this.ReverseResize(c);
            base.OnClientSizeChanged(e);
        }

        private void buttonPluginManager_Click(object sender, EventArgs e)
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

        private void panel1_SizeChanged(object sender, EventArgs e)
        {

        }

        private void SetReverse(Control c)
        {
            Classes.Interfaces.ReserveRelativeLocation cc = c as Classes.Interfaces.ReserveRelativeLocation;
            if (cc != null)
            {
                if (c.MinimumSize.IsEmpty)
                    c.MinimumSize = c.Size;
                if (cc.RelativeLocation.IsEmpty)
                    cc.RelativeLocation = c.Location;
            }
        }

        private void ReverseResize(Control c)
        {
            Classes.Interfaces.ReserveRelativeLocation cc = c as Classes.Interfaces.ReserveRelativeLocation;
            if (cc != null)
            {
                if (!c.MinimumSize.IsEmpty)
                {
                    Size newSize = new Size(Convert.ToInt32(c.MinimumSize.Width * CommonMethods.ScalingFactor), Convert.ToInt32(c.MinimumSize.Height * CommonMethods.ScalingFactor));
                    c.Size = newSize;
                }
                if (!cc.RelativeLocation.IsEmpty)
                {
                    Point newPoint = new Point(Convert.ToInt32(cc.RelativeLocation.X * CommonMethods.ScalingFactor), Convert.ToInt32(cc.RelativeLocation.Y * CommonMethods.ScalingFactor));
                    c.Location = newPoint;
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
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
                    Classes.Components.TaskbarItemInfo.TaskbarProgress.SetState(this, Classes.Components.TaskbarItemInfo.TaskbarProgress.TaskbarStates.Normal);
                    CircleProgressBarProperties _circleProgressBarProperties = _properties as CircleProgressBarProperties;
                    if (_circleProgressBarProperties != null)
                        mainProgressBar.ShowSmallText = _circleProgressBarProperties.ShowSmallText;
                    this.ProgressBarPercent_Visible(true);
                    this.ProgressBarInfinite_Visible(false);
                    if (this.buttonCancel.Tag is string)
                        if (_circleProgressBarProperties == null || _circleProgressBarProperties.ShowCancel)
                            this.buttonCancel_Visible(true);
                        else
                            this.buttonCancel_Visible(false);
                    else
                        this.buttonCancel_Visible(false);
                    this.gameStartButton1.Visible = false;
                    this.Buttons_Visible(false);
                    break;
                case ProgressBarVisibleState.Infinite:
                    Classes.Components.TaskbarItemInfo.TaskbarProgress.SetState(this, Classes.Components.TaskbarItemInfo.TaskbarProgress.TaskbarStates.Indeterminate);
                    InfiniteProgressBarProperties _infiniteProgressBarProperties = _properties as InfiniteProgressBarProperties;
                    this.ProgressBarPercent_Visible(false);
                    mainProgressBar.ShowSmallText = false;
                    this.ProgressBarInfinite_Visible(true);
                    if (this.buttonCancel.Tag is string)
                        if (_infiniteProgressBarProperties == null || _infiniteProgressBarProperties.ShowCancel)
                            this.buttonCancel_Visible(true);
                        else
                            this.buttonCancel_Visible(false);
                    else
                        this.buttonCancel_Visible(false);
                    this.gameStartButton1.Visible = false;
                    this.Buttons_Visible(false);
                    break;
                default:
                    Classes.Components.TaskbarItemInfo.TaskbarProgress.SetState(this, Classes.Components.TaskbarItemInfo.TaskbarProgress.TaskbarStates.NoProgress);
                    this.gameStartButton1.Visible = true;
                    this.ProgressBarPercent_Visible(false);
                    mainProgressBar.ShowSmallText = false;
                    this.buttonCancel_Visible(false);
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

        private void buttonCancel_Visible(bool myBool)
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
                mainFormLoadingHost.SendToBack();//*/
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //AeroControl.EnableBlur(this);
            this.mainFormLoading.SetRingColor(Color.DarkRed);
            LanguageManager.TranslateForm(this);
        }

        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.OnBackgroundImageChanged(e);
            this.launchCache();
        }

        private void launchCache(bool val)
        {
            if (this._disposed) return;
            this.SuspendLayout();
            this.panelMainMenu.BackColor = Color.Transparent;
            this.panelMainMenu.GetNewCache();
            this.panelMainMenu.Panel1.BackColor = Color.Transparent;
            this.panel1.BackColor = Color.Transparent;
            this.panel1.GetNewCache();
            this.panelMainMenu.Panel1.BackColor = this.BackColor;
            this.gameStartButton1.GetNewCache();
            this.splitContainer1.Panel2.BackColor = Color.Transparent;
            this.tweakerWebBrowserLoading.BackColor = Color.Transparent;
            this.tweakerWebBrowserLoading.GetNewCache();
            if (val)
            {
                Color _color = this.BackColor;
                this.panelMainMenu.BackColor = _color;
                this.panel1.BackColor = _color;
                this.splitContainer1.Panel2.BackColor = _color;
                this.gameStartButton1.BackColor = _color;
                this.tweakerWebBrowserLoading.BackColor = _color;
            }
            else
            {
                this.panelMainMenu.BackColor = Color.Transparent;
                this.panel1.BackColor = Color.Transparent;
                this.splitContainer1.Panel2.BackColor = Color.Transparent;
                this.gameStartButton1.BackColor = Color.Transparent;
                this.tweakerWebBrowserLoading.BackColor = Color.Transparent;
            }
            this.ResumeLayout(false);
        }

        private void launchCache()
        {
            this.launchCache(true);
        }

        private void relaunchCache()
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
            this.launchCache();
            this.ChangeProgressBarStatus(ProgressBarVisibleState.Infinite);
            this.bWorker_Boot.RunWorkerAsync();
        }
#else
        private void Form_Shown(object sender, EventArgs e)
        {
            this.launchCache();
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
            result.EnglishPatchNotify += PSO2Controller_EnglishPatchNotify;
            result.LargeFilesPatchNotify += PSO2Controller_LargeFilesPatchNotify;
            result.StoryPatchNotify += PSO2Controller_StoryPatchNotify;
            result.RaiserPatchNotify += PSO2Controller_RaiserPatchNotify;
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
                    Classes.Components.TaskbarItemInfo.TaskbarProgress.SetValue(this, e.Progress, this.mainProgressBar.Maximum);
            }
        }

        private void installToolStripMenuItem_Click(object sender, EventArgs e)
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
                        if (MetroMessageBox.Show(this, string.Format(LanguageManager.GetMessageText("AskInstallPatch", "Do you want to install the {0}?"), DefaultValues.AIDA.Strings.RaiserPatchCalled) + "\r\n" + string.Format(LanguageManager.GetMessageText("0PatchNotCompatibleWithOthers", "{0} may be NOT compatible with other patches. It is advised to NOT install any other patches beside {0}."), DefaultValues.AIDA.Strings.RaiserPatchCalled), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.RaiserPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.RaiserPatchButton.Text = $"{DefaultValues.AIDA.Strings.RaiserPatchCalled}: Installing";
                            this._pso2controller.InstallRaiserPatch();
                        }
                        break;
                }
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void gameStartButton1_Click(object sender, EventArgs e)
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
                            this.buttonPluginManager, this.buttonOptionPSO2);
                        break;
                    case GameStartState.GameNotInstalled:
                        this.SetEnabledControls(false, this.EnglishPatchButton, this.LargeFilesPatchButton, this.StoryPatchButton, this.RaiserPatchButton,
                            this.buttonPluginManager, this.buttonOptionPSO2);
                        this.gameStartButton1.Text = "INSTALL";
                        break;
                }
            }
        }

        private void SetEnabledControls(bool _enabled, params Control[] c)
        {
            foreach (Control ccc in c)
                ccc.Enabled = _enabled;
        }

        private void selectPSO2LocationToolStripMenuItem_Click(object sender, EventArgs e)
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
                using (SharpCompress.Archives.SevenZip.SevenZipArchive libPathArchive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(libPath + ".7z"))
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
            bool PingAIDA = AIDA.GetIdeaServer();
            this.SyncContext?.Post(new SendOrPostCallback(delegate {
                this.refreshToolStripMenuItem.PerformClick();
                if (PingAIDA)
                {
                    Classes.PSO2.PSO2Plugin.PSO2PluginManager.Instance.GetPluginList();
                }
            }), null);

            bool pso2installed = this._pso2controller.IsPSO2Installed;
            
            bool pso2update = false, updatepatches = false;
            Classes.Components.PatchType whichpatch = Classes.Components.PatchType.None;
            if (pso2installed)
            {
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
                if (!pso2update)
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
            else
                this._pso2controller.NotifyPatches(true);

            e.Result = new BootResult(pso2installed, pso2update, updatepatches, whichpatch);
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
                        if (br.UpdatePSO2)
                            this._pso2controller.UpdatePSO2Client();
                        else if (br.UpdatePSO2Patches)
                            this._pso2controller.InstallPatches(br.PSO2Patches);
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
            public BootResult(bool pso2installed, bool _updatepso2, bool _updatepatches) : this(pso2installed, _updatepso2, _updatepatches, Classes.Components.PatchType.None) { }
            public BootResult(bool pso2installed, bool _updatepso2, bool _updatepatches, Classes.Components.PatchType whichpatchtoupdate)
            {
                this.IsPSO2Installed = pso2installed;
                this.UpdatePSO2 = _updatepso2;
                this.UpdatePSO2Patches = _updatepatches;
                this.PSO2Patches = whichpatchtoupdate;
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
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.bWorker_tweakerWebBrowser_load.RunWorkerAsync();
        }

        private void BWorker_tweakerWebBrowser_load_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tweakerWebBrowser_IsLoading(false);
            if (e.Error != null)
                Leayal.Log.LogManager.GeneralLog.Print(e.Error);
        }

        private void BWorker_tweakerWebBrowser_load_DoWork(object sender, DoWorkEventArgs e)
        {
            /*string linefiletoskip = Classes.AIDA.TweakerWebPanel.CutString;
            linefiletoskip = WebClientPool.GetWebClient(DefaultValues.MyServer.GetWebLink).DownloadString(DefaultValues.MyServer.GetWebLink + DefaultValues.MyServer.Web.TweakerSidePanelLiner);
            if (string.IsNullOrWhiteSpace(linefiletoskip))
                linefiletoskip = Classes.AIDA.TweakerWebPanel.CutString;*/
            tweakerWebBrowser_IsLoading(true);
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

        private void tweakerWebBrowser_CommandLink(object sender, StepEventArgs e)
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

        private void tweakerWebBrowser_LockedNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Thread launchWebThread = new Thread(new ParameterizedThreadStart(this.launchWeb));
            launchWebThread.IsBackground = true;
            launchWebThread.SetApartmentState(ApartmentState.STA);
            launchWebThread.Start(e.Url);
        }

        private void launchWeb(object url)
        {
            try
            {
                Uri _uri = url as Uri;
                if (_uri != null)
                    System.Diagnostics.Process.Start(_uri.OriginalString);
            }
            catch (Exception ex)
            { Leayal.Log.LogManager.GeneralLog.Print(ex); }
        }

        public void tweakerWebBrowser_IsLoading(bool theBool)
        {
            this.SyncContext.Post(new SendOrPostCallback(delegate {
                this.tweakerWebBrowserLoading.Visible = theBool;
                foreach (ToolStripMenuItem item in this.tweakerWebBrowserContextMenu.Items)
                    item.Enabled = !theBool;
            }), null);
        }
        #endregion

        #region "Options"
        private ExtendedToolTip optionToolTip;
        private void OptionPanel_Load()
        {
            if (DesignMode) return;
            if (this.optionToolTip == null)
            {
                this.optionToolTip = new ExtendedToolTip();
                this.optionToolTip.UseFading = true;
                this.optionToolTip.BackColor = Color.FromArgb(17, 17, 17);
                this.optionToolTip.Font = new Font(this.Font.FontFamily, 10F);
                this.optionToolTip.ForeColor = Color.FromArgb(254, 254, 254);
                this.optionToolTip.FormColor = this.optionToolTip.BackColor;
                this.optionToolTip.PreferedSize = new Size(300, 400);
                this.optionToolTip.Opacity = 0.75F;
                this.optionToolTip.Popup += this.OptionToolTip_Popup;
                this.optionToolTip.SetToolTip(this.optionComboBoxUpdateThread, LanguageManager.GetMessageText("OptionTooltip_UpdateThreads", "This option is to determine how many threads the launcher will use to check the game files while updating your game client.\nMore threads = cost more computer resource."));
                this.optionToolTip.SetToolTip(this.optioncomboBoxThrottleCache, LanguageManager.GetMessageText("OptionTooltip_UpdateThreadsThrottle", "This option is to throttle how fast the cache process will be to reduce CPU usage. Only avaiable if using update cache.\nSlower = cost less CPU usage."));
                this.optionToolTip.SetToolTip(this.optioncheckboxpso2updatecache, LanguageManager.GetMessageText("OptionTooltip_UpdateCache", "This option is to determine if the launcher should use update cache to speed up file checking."));
                this.optionToolTip.SetToolTip(this.optioncheckBoxMinimizeNetworkUsage, LanguageManager.GetMessageText("OptionTooltip_MinimizeNetworkUsage", "This option is to determine if the launcher should reduce network usage by reading the resource from cache."));

                this.optionToolTip.SetToolTip(this.optionSliderFormScale, LanguageManager.GetMessageText("OptionTooltip_SliderFormScale", "Set the launcher size scale factor.\nThis scale factor must be equal or higher than user's font scale settings."));
                this.optionToolTip.SetToolTip(this.optionbuttonResetBG, LanguageManager.GetMessageText("OptionTooltip_ResetBG", "Reset background image and background color to default."));
                this.optionToolTip.SetToolTip(this.optioncomboBoxBGImgMode, LanguageManager.GetMessageText("OptionTooltip_ImgMode", "Set the image layout for the custom background image."));
            }
        }

        private void OptionToolTip_Popup(object sender, ExPopupEventArgs e)
        {
            if (e.AssociatedControl is ComboBox)
                e.Location = new Point(e.AssociatedControl.PointToScreen(new Point(e.AssociatedControl.Width, 0)).X, e.Location.Y);
        }

        private void RefreshOptionPanel()
        {
            if (this.optionComboBoxUpdateThread.Items.Count != CommonMethods.MaxThreadsCount)
            {
                this.optionComboBoxUpdateThread.Items.Clear();
                if (CommonMethods.MaxThreadsCount == 1)
                {
                    this.optionComboBoxUpdateThread.Items.Add("1");
                    this.optionComboBoxUpdateThread.Enabled = false;
                }
                else
                {
                    for (int i = 1; i <= CommonMethods.MaxThreadsCount; i++)
                        this.optionComboBoxUpdateThread.Items.Add(i.ToString());
                    this.optionComboBoxUpdateThread.Enabled = true;
                }
            }
            this.optionComboBoxUpdateThread.SelectedItem = MySettings.GameClientUpdateThreads.ToString();
            int _threadspeedcount = (int)ThreadSpeed.ThreadSpeedCount;
            if (this.optioncomboBoxThrottleCache.Items.Count != _threadspeedcount)
            {
                this.optioncomboBoxThrottleCache.Items.Clear();
                for (int i = 0; i < _threadspeedcount; i++)
                    this.optioncomboBoxThrottleCache.Items.Add(((ThreadSpeed)i).ToString());
            }
            this.optioncomboBoxThrottleCache.SelectedItem = ((ThreadSpeed)MySettings.GameClientUpdateThrottleCache).ToString();

            this.optioncheckboxpso2updatecache.Checked = MySettings.GameClientUpdateCache;
            this.optioncheckBoxMinimizeNetworkUsage.Checked = MySettings.MinimizeNetworkUsage;

            this.optionSliderFormScale.MouseWheelBarPartitions = ((optionSliderFormScale.Maximum - optionSliderFormScale.Minimum) / 25);
        }

        private bool _appearenceChanged;
        private void SaveOptionSettings()
        {
            this.optionToolTip.Hide();
            MySettings.GameClientUpdateThreads = int.Parse(this.optionComboBoxUpdateThread.SelectedItem.ToString());
            MySettings.GameClientUpdateThrottleCache = (int)(Enum.Parse(typeof(ThreadSpeed), (string)this.optioncomboBoxThrottleCache.SelectedItem));
            MySettings.GameClientUpdateCache = this.optioncheckboxpso2updatecache.Checked;
            MySettings.MinimizeNetworkUsage = this.optioncheckBoxMinimizeNetworkUsage.Checked;
            if (this._appearenceChanged)
            {
                this._appearenceChanged = false;
                MySettings.LauncherBGColor = new Nullable<Color>(optionbuttonPickBackColor.BackColor);
                MySettings.LauncherBGlocation = optiontextBoxBGlocation.Text;
                MySettings.LauncherSizeScale = optionSliderFormScale.Value;
                MySettings.LauncherBGImgLayout = (ImageLayout)Enum.Parse(typeof(ImageLayout), (string)this.optioncomboBoxBGImgMode.SelectedItem, true);
                MetroMessageBox.Show(this, LanguageManager.GetMessageText("OptionAppearenceApplyNextBoot", "The appearence changes in your settings will be applied at next startup."), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool LoadingAppearenceOption;
        private void LoadAppearenceSetting()
        {
            LoadingAppearenceOption = true;
            string[] names = Enum.GetNames(typeof(ImageLayout));
            if (this.optioncomboBoxBGImgMode.Items.Count != names.Length)
            {
                this.optioncomboBoxBGImgMode.Items.Clear();
                for (int i = 0; i < names.Length; i++)
                    this.optioncomboBoxBGImgMode.Items.Add(names[i]);
            }
            var myImglayout = MySettings.LauncherBGImgLayout;
            this.optioncomboBoxBGImgMode.SelectedItem = myImglayout.ToString();

            string bgloc = MySettings.LauncherBGlocation;
            Color? bgcolor = MySettings.LauncherBGColor;
            if (!string.IsNullOrWhiteSpace(bgloc) && System.IO.File.Exists(bgloc))
            {
                Leayal.Drawing.MemoryImage mi = null;
                try
                {
                    mi = Leayal.Drawing.MemoryImage.FromFile(bgloc, false);
                    this.BackgroundImageLayout = myImglayout;
                    if (this.BackgroundImage != null)
                    {
                        Image asd = this.BackgroundImage;
                        this.BackgroundImage = mi.Image;
                        asd.Dispose();
                    }
                    else
                        this.BackgroundImage = mi.Image;
                    if (bgcolor!= null && bgcolor.HasValue)
                        this.BackColor = bgcolor.Value;
                }
                catch (Exception ex)
                {
                    if (mi != null)
                        mi.Dispose();
                    Leayal.Log.LogManager.GeneralLog.Print(ex);
                }
            }
            optiontextBoxBGlocation.Text = bgloc;
            if (bgcolor!= null && bgcolor.HasValue)
                optionbuttonPickBackColor.BackColor = bgcolor.Value;
            else
                optionbuttonPickBackColor.BackColor = this.BackColor;
            optionSliderFormScale.Value = Convert.ToInt32(Classes.Infos.CommonMethods.GetResolutionScale() * 100);
            LoadingAppearenceOption = false;
        }

        private void optioncheckboxpso2updatecache_CheckedChanged(object sender, EventArgs e)
        {
            this.optioncomboBoxThrottleCache.Enabled = this.optioncheckboxpso2updatecache.Checked;
        }

        private void SystemEvents_ScalingFactorChanged(object sender, EventArgs e)
        {
            this.optionSliderFormScale.ValueAvailableRange = new AvailableIntRange(Convert.ToInt32(Leayal.Forms.FormWrapper.ScalingFactor * 100), optionSliderFormScale.Maximum);
        }

        private void optioncomboBoxBGImgMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!LoadingAppearenceOption)
                this._appearenceChanged = true;
        }

        private void optionbuttonResetBG_Click(object sender, EventArgs e)
        {
            this._appearenceChanged = true;
            optiontextBoxBGlocation.Text = string.Empty;
            optionbuttonPickBackColor.BackColor = Color.FromArgb(17, 17, 17);
        }

        private void optionbuttonBrowseBG_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select background image location";
                ofd.SupportMultiDottedExtensions = false;
                ofd.RestoreDirectory = true;
                ofd.AutoUpgradeEnabled = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = false;
                if (!string.IsNullOrWhiteSpace(optiontextBoxBGlocation.Text) && System.IO.File.Exists(optiontextBoxBGlocation.Text))
                    ofd.FileName = optiontextBoxBGlocation.Text;
                using (Leayal.Forms.DialogFileFilterBuilder dffb = new DialogFileFilterBuilder())
                {
                    dffb.AppendAllSupportedTypes = AppendOrder.Last;
                    dffb.Append("Portable Network Graphics", "*.png");
                    dffb.Append("Bitmap Image", "*.bmp");
                    dffb.Append("JPEG Image", "*jpg", "*.jpeg");
                    ofd.Filter = dffb.ToFileFilterString();
                    ofd.FilterIndex = dffb.OutputCount;
                }
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    this._appearenceChanged = true;
                    optiontextBoxBGlocation.Text = ofd.FileName;
                }
            }
        }

        private void optionbuttonPickBackColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.AllowFullOpen = true;
                cd.AnyColor = true;
                cd.SolidColorOnly = true;
                cd.Color = optionbuttonPickBackColor.BackColor;
                if (cd.ShowDialog(this) == DialogResult.OK)
                {
                    this._appearenceChanged = true;
                    optionbuttonPickBackColor.BackColor = cd.Color;
                }
            }
        }

        private void colorSlider1_ValueChanged(object sender, EventArgs e)
        {
            if (!LoadingAppearenceOption)
                this._appearenceChanged = true;
        }

        private enum ThreadSpeed : int { Fastest, Faster, Normal, Slower, Slowest, ThreadSpeedCount }
        #endregion
    }
}
