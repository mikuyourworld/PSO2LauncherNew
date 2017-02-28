using System;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes.Infos;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using PSO2ProxyLauncherNew.Classes.Events;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using MetroFramework;
using MetroFramework.Forms;

namespace PSO2ProxyLauncherNew.Forms
{
    public partial class MyMainMenu : MetroForm
    {
        private WindowsFormsSynchronizationContext SyncContext;
        private BackgroundWorker bWorker_tweakerWebBrowser_load;
        private BackgroundWorker bWorker_Boot;
        private Classes.Components.PSO2Controller _pso2controller;
        Classes.Components.DirectBitmap bgImage;

        public MyMainMenu()
        {
            InitializeComponent();
            this.SyncContext = WindowsFormsSynchronizationContext.Current as WindowsFormsSynchronizationContext;
            this.Icon = Properties.Resources._1;

            //BackgroundWorker for tweakerWebBrowser Load
            this.bWorker_Boot = new BackgroundWorker();
            this.bWorker_Boot.WorkerSupportsCancellation = false;
            this.bWorker_Boot.WorkerReportsProgress = false;
            this.bWorker_Boot.DoWork += BWorker_Boot_DoWork;
            this.bWorker_Boot.RunWorkerCompleted += BWorker_Boot_RunWorkerCompleted;

            //BackgroundWorker for tweakerWebBrowser Load
            this.bWorker_tweakerWebBrowser_load = new BackgroundWorker();
            this.bWorker_tweakerWebBrowser_load.WorkerSupportsCancellation = false;
            this.bWorker_tweakerWebBrowser_load.WorkerReportsProgress = false;
            this.bWorker_tweakerWebBrowser_load.DoWork += BWorker_tweakerWebBrowser_load_DoWork;
            this.bWorker_tweakerWebBrowser_load.RunWorkerCompleted += BWorker_tweakerWebBrowser_load_RunWorkerCompleted;

            this._pso2controller = CreatePSO2Controller();

            Bitmap asfas = PSO2ProxyLauncherNew.Properties.Resources._bgimg;
            this.bgImage = new Classes.Components.DirectBitmap(asfas.Width, asfas.Height);
            this.bgImage.Graphics.DrawImage(asfas, 0, 0);
            //db.Bitmap.MakeTransparent(Color.Black);
            //panel1 .i.SizeMode = PictureBoxSizeMode.Zoom;//*/
            panel1.BackgroundImage = this.bgImage.Bitmap;

            Classes.Components.AbstractExtractor.SetSyncContext(this.SyncContext);
        }

        #region "English Patch"
        private void PSO2Controller_EnglishPatchNotify(object sender, Classes.Components.PSO2Controller.PatchNotifyEventArgs e)
        {
            this.EnglishPatchButton.Text = "English Patch: " + e.PatchVer;
            if (e.PatchVer == DefaultValues.AIDA.Tweaker.Registries.NoPatchString)
                this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Red;
            else
                this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Green;
        }
        private void EnglishPatchButton_Click(object sender, EventArgs e)
        {
            Control myself = sender as Control;
            this.englishPatchContext.Tag = Classes.Components.Task.EnglishPatch;
            this.englishPatchContext.Show(myself, 0, myself.Height);
        }
        #endregion

        #region "LargeFiles Patch"
        private void PSO2Controller_LargeFilesPatchNotify(object sender, Classes.Components.PSO2Controller.PatchNotifyEventArgs e)
        {
            this.LargeFilesPatchButton.Text = "LargeFiles Patch: " + e.PatchVer;
            if (e.PatchVer == DefaultValues.AIDA.Tweaker.Registries.NoPatchString)
                this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Red;
            else
                this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Green;
        }
        private void LargeFilesPatchButton_Click(object sender, EventArgs e)
        {
            Control myself = sender as Control;
            this.englishPatchContext.Tag = Classes.Components.Task.LargeFilesPatch;
            this.englishPatchContext.Show(myself, 0, myself.Height);
        }
        #endregion

        #region "Story Patch"
        private void StoryPatchButton_Click(object sender, EventArgs e)
        {
            Control myself = sender as Control;
            this.englishPatchContext.Tag = Classes.Components.Task.StoryPatch;
            this.englishPatchContext.Show(myself, 0, myself.Height);
        }

        private void PSO2Controller_StoryPatchNotify(object sender, Classes.Components.PSO2Controller.PatchNotifyEventArgs e)
        {
            this.StoryPatchButton.Text = "Story Patch: " + e.PatchVer;
            if (e.PatchVer == DefaultValues.AIDA.Tweaker.Registries.NoPatchString)
                this.StoryPatchButton.FlatAppearance.BorderColor = Color.Red;
            else
                this.StoryPatchButton.FlatAppearance.BorderColor = Color.Green;
        }
        #endregion

        #region "Handle Events"
        private void Result_HandledException(object sender, Classes.Components.PSO2Controller.PSO2HandledExceptionEventArgs e)
        {
            Classes.Log.LogManager.GeneralLog.Print(e.Error);
            if (e.LastTask == Classes.Components.Task.EnglishPatch)
                this.PrintText("[English Patch]" + e.Error.Message, Classes.Controls.RtfColor.Red);
            else if (e.LastTask == Classes.Components.Task.LargeFilesPatch)
                this.PrintText("[LargeFiles Patch]" + e.Error.Message, Classes.Controls.RtfColor.Red);
            else if (e.LastTask == Classes.Components.Task.StoryPatch)
                this.PrintText("[Story Patch]" + e.Error.Message, Classes.Controls.RtfColor.Red);
            else
                this.PrintText(e.Error.Message, Classes.Controls.RtfColor.Red);
        }
        private void Result_StepChanged(object sender, Classes.Components.PSO2Controller.StepChangedEventArgs e)
        {
            if (e.Final)
                this.PrintText(e.Step, Classes.Controls.RtfColor.Green);
            else
                this.PrintText(e.Step);
        }
        private void Result_ProgressBarStateChanged(object sender, ProgressBarStateChangedEventArgs e)
        {
            this.ChangeProgressBarStatus(e.ProgressBarState, e.Properties);
        }
        #endregion

        #region "Form Codes"
        public new void Dispose()
        {
            panel1.BackgroundImage = null;
            if (this.bgImage != null)
                this.bgImage.Dispose();
            base.Dispose();
        }

        private void ChangeProgressBarStatus(ProgressBarVisibleState val)
        { this.ChangeProgressBarStatus(val, null); }
        private void ChangeProgressBarStatus(ProgressBarVisibleState val, object _properties)
        {
            switch (val)
            {
                case ProgressBarVisibleState.Percent:
                    if (_properties != null && _properties is CircleProgressBarProperties)
                    {
                        var asdasdasd = _properties as CircleProgressBarProperties;
                        mainProgressBar.ShowSmallText = asdasdasd.ShowSmallText;
                    }
                    this.ProgressBarPercent_Visible(true);
                    this.ProgressBarInfinite_Visible(false);
                    break;
                case ProgressBarVisibleState.Infinite:
                    this.ProgressBarPercent_Visible(false);
                    mainProgressBar.ShowSmallText = false;
                    this.ProgressBarInfinite_Visible(true);
                    break;
                default:
                    this.ProgressBarPercent_Visible(false);
                    mainProgressBar.ShowSmallText = false;
                    this.ProgressBarInfinite_Visible(false);
                    break;
            }
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
            //AeroControl.EnableBlur(this);
            this.mainFormLoading.SetRingColor(Color.DarkRed);
            Classes.LanguageManager.TranslateForm(this);
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            this.ChangeProgressBarStatus(ProgressBarVisibleState.Infinite);
            this.bWorker_Boot.RunWorkerAsync();
        }

        public void PrintText(string msg)
        {
            Classes.Log.LogManager.GetLogDefaultPath(DefaultValues.MyInfo.Filename.Log.PrintOut, string.Empty, false).Print(msg);
            this.LogRichTextBox.AppendText(msg);
        }
        public void PrintText(string msg, Classes.Controls.RtfColor textColor)
        {
            Classes.Log.LogManager.GetLogDefaultPath(DefaultValues.MyInfo.Filename.Log.PrintOut, string.Empty, false).Print(msg);
            this.LogRichTextBox.AppendText(msg, textColor);
        }
        private Classes.Components.PSO2Controller CreatePSO2Controller()
        {
            Classes.Components.PSO2Controller result = new Classes.Components.PSO2Controller();
            result.HandledException += Result_HandledException;
            result.ProgressBarStateChanged += Result_ProgressBarStateChanged;
            result.StepChanged += Result_StepChanged;
            result.CurrentProgressChanged += Result_CurrentProgressChanged;
            result.CurrentTotalProgressChanged += Result_CurrentTotalProgressChanged;

            result.EnglishPatchNotify += PSO2Controller_EnglishPatchNotify;
            result.LargeFilesPatchNotify += PSO2Controller_LargeFilesPatchNotify;
            result.StoryPatchNotify += PSO2Controller_StoryPatchNotify;
            return result;
        }

        private void Result_CurrentTotalProgressChanged(object sender, ProgressEventArgs e)
        {
            
            this.mainProgressBar.Maximum = e.Progress;
        }

        private void Result_CurrentProgressChanged(object sender, ProgressEventArgs e)
        {
            if (e.Progress <= this.mainProgressBar.Maximum)
                this.mainProgressBar.Value = e.Progress;
        }

        private void installToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
            {
                Classes.Components.Task currentTask = (Classes.Components.Task)this.englishPatchContext.Tag;
                switch (currentTask)
                {
                    case Classes.Components.Task.EnglishPatch:
                        if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskInstallEnglish", "Do you want to create backups and install the English Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.EnglishPatchButton.Text = "English Patch: Installing";
                            this._pso2controller.InstallEnglishPatch();
                        }
                        break;
                    case Classes.Components.Task.LargeFilesPatch:
                        if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskInstallLargeFiles", "Do you want to create backups and install the LargeFiles Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.LargeFilesPatchButton.Text = "LargeFiles Patch: Installing";
                            this._pso2controller.InstallLargeFilesPatch();
                        }
                        break;
                    case Classes.Components.Task.StoryPatch:
                        if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskInstallStory", "Do you want to create backups and install the Story Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.StoryPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.StoryPatchButton.Text = "Story Patch: Installing";
                            this._pso2controller.InstallStoryPatch();
                        }
                        break;
                }
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
            {
                Classes.Components.Task currentTask = (Classes.Components.Task)this.englishPatchContext.Tag;
                switch (currentTask)
                {
                    case Classes.Components.Task.EnglishPatch:
                        if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskUninstallEnglish", "Do you want to uninstall the English Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.EnglishPatchButton.Text = "English Patch: Uninstalling";
                            this._pso2controller.UninstallEnglishPatch();
                        }
                        break;
                    case Classes.Components.Task.LargeFilesPatch:
                        if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskUninstallLargeFiles", "Do you want to uninstall the LargeFiles Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.LargeFilesPatchButton.Text = "LargeFiles Patch: Uninstalling";
                            this._pso2controller.UninstallLargeFilesPatch();
                        }
                        break;
                    case Classes.Components.Task.StoryPatch:
                        if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskUninstallStory", "Do you want to uninstall the Story Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.StoryPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                            this.StoryPatchButton.Text = "Story Patch: Uninstalling";
                            this._pso2controller.UninstallStoryPatch();
                        }
                        break;
                }
            }
        }

        private void gameStartButton1_Click(object sender, EventArgs e)
        {
            this._pso2controller.LaunchPSO2Game();
        }
        #endregion

        #region "Startup Codes"
        private void BWorker_Boot_DoWork(object sender, DoWorkEventArgs e)
        {
            //Ping the 7z
            string libPath = DefaultValues.MyInfo.Filename.SevenZip.SevenZipLibPath;
            //this.PrintText(Classes.LanguageManager.GetMessageText("RARLibLoaded", "RAR library loaded successfully"));
            if (!DefaultValues.MyInfo.Filename.SevenZip.IsValid)
            {
                this.PrintText(Classes.LanguageManager.GetMessageText("InvalidSevenZipLib", "SevenZip library is invalid or not existed. Redownloading"));
                //WakeUpCall for 7z
                string url = DefaultValues.MyServer.Web.GetDownloadLink + "/" + System.IO.Path.GetFileNameWithoutExtension(DefaultValues.MyInfo.Filename.SevenZip.SevenZipLibName) + ".rar";
                WebClientPool.GetWebClient(DefaultValues.MyServer.Web.GetDownloadLink).DownloadFile(url, libPath + ".rar");
                using (SharpCompress.Archives.Rar.RarArchive libPathArchive = SharpCompress.Archives.Rar.RarArchive.Open(libPath + ".rar"))
                    Classes.Components.AbstractExtractor.Unrar(libPathArchive, MyApp.AssemblyInfo.DirectoryPath, null);
                try { System.IO.File.Delete(libPath + ".rar"); } catch { }
            }
            Classes.Components.AbstractExtractor.SetSevenZipLib(libPath);
            this.PrintText(Classes.LanguageManager.GetMessageText("SevenZipLibLoaded", "SevenZip library loaded successfully"), Classes.Controls.RtfColor.Green);
            //Ping AIDA for the server
            Classes.AIDA.GetIdeaServer();
            var pso2versions = this._pso2controller.CheckForPSO2Updates();
            bool pso2update = false;
            if (pso2versions.IsNewVersionFound)
            {
                string pso2updater_FoundNewLatestVersion = string.Format(Classes.LanguageManager.GetMessageText("PSO2Updater_FoundNewLatestVersion", "Found new PSO2 client version: {0}.\nYour current version: {1}"), pso2versions.LatestVersion, pso2versions.CurrentVersion);
                this.PrintText(pso2updater_FoundNewLatestVersion);
                DialogResult pso2updateAnswer = DialogResult.No;
                this.SyncContext?.Send(new SendOrPostCallback(delegate { pso2updateAnswer = MessageBox.Show(pso2updater_FoundNewLatestVersion + "\n" + Classes.LanguageManager.GetMessageText("PSO2Updater_ConfirmToUpdate", "Do you want to perform update now?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }), null);
                if (pso2updateAnswer == DialogResult.Yes)
                    pso2update = true;
            }
            else
                this.PrintText(string.Format(Classes.LanguageManager.GetMessageText("PSO2Updater_AlreadyLatestVersion", "PSO2 Client is already latest version: {0}"), pso2versions.CurrentVersion), Classes.Controls.RtfColor.Green);
            e.Result = new BootResult(pso2update);
        }

        private void BWorker_Boot_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Classes.Log.LogManager.GeneralLog.Print(e.Error);
                this.PrintText(e.Error.Message, Classes.Controls.RtfColor.Red);
                MetroMessageBox.Show(this, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            else
            {
                this.refreshToolStripMenuItem.PerformClick();
                this.ChangeProgressBarStatus(ProgressBarVisibleState.None);
                if (e.Result != null)
                {
                    BootResult br = e.Result as BootResult;
                    if (br.UpdatePSO2)
                        this._pso2controller.UpdatePSO2Client();
                }
            }
        }
        #endregion

        #region "Private Classes"
        private class BootResult
        {
            public bool UpdatePSO2 { get; }
            public BootResult(bool _updatepso2)
            {
                this.UpdatePSO2 = _updatepso2;
            }
        }

        public class CircleProgressBarProperties
        {
            public bool ShowSmallText { get; }
            public CircleProgressBarProperties(bool _showsmalltext)
            {
                this.ShowSmallText = _showsmalltext;
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
        }

        private void BWorker_tweakerWebBrowser_load_DoWork(object sender, DoWorkEventArgs e)
        {
            this.tweakerWebBrowser.LockNavigate = false;
            tweakerWebBrowser_IsLoading(true);
            /*string linefiletoskip = Classes.AIDA.TweakerWebPanel.CutString;
            linefiletoskip = WebClientPool.GetWebClient(DefaultValues.MyServer.GetWebLink).DownloadString(DefaultValues.MyServer.GetWebLink + DefaultValues.MyServer.Web.TweakerSidePanelLiner);
            if (string.IsNullOrWhiteSpace(linefiletoskip))
                linefiletoskip = Classes.AIDA.TweakerWebPanel.CutString;*/
            string resultofgettinghtmlfile = WebClientPool.GetWebClient_AIDA().DownloadString(Classes.AIDA.TweakerWebPanel.InfoPageLink);
            if (string.IsNullOrEmpty(resultofgettinghtmlfile))
                this.tweakerWebBrowser.Navigate(Classes.AIDA.TweakerWebPanel.InfoPageLink);
            else
            {
                this.tweakerWebBrowser.LoadHTML(resultofgettinghtmlfile);
                //this.tweakerWebBrowser.EnglishPatchStatus = result.EnglishPatch;
                //this.tweakerWebBrowser.ItemPatchStatus = result.ItemPatch;
            }
            this.tweakerWebBrowser.LockNavigate = true;
        }

        private void tweakerWebBrowser_LockedNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Thread launchWebThread = new Thread(new ParameterizedThreadStart(this.launchWeb));
            launchWebThread.IsBackground = true;
            launchWebThread.Start(e.Url);
        }

        private void launchWeb(object url)
        {
            try
            {
                Uri _uri = url as Uri;
                System.Diagnostics.Process.Start(_uri.OriginalString);
            }
            catch (Exception ex)
            {
                Classes.Log.LogManager.GeneralLog.Print(ex);
            }
        }

        public void tweakerWebBrowser_IsLoading(bool theBool)
        {
            this.SyncContext.Post(new System.Threading.SendOrPostCallback(this._tweakerWebBrowser_IsLoading), theBool as object);
        }
        private void _tweakerWebBrowser_IsLoading(object theboolean)
        {
            bool bo = Convert.ToBoolean(theboolean);
            this.tweakerWebBrowserLoading.Visible = bo;
            foreach (ToolStripMenuItem item in this.tweakerWebBrowserContextMenu.Items)
                item.Enabled = !bo;
        }


        #endregion
    }
}
