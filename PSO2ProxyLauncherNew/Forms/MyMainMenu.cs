using System;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes.Infos;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
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
        private void Result_LargeFilesPatchNotify(object sender, Classes.Components.PSO2Controller.PatchNotifyEventArgs e)
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
        private void Result_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.mainProgressBar.Value = e.ProgressPercentage;
        }
        private void Result_ProgressBarNotify(object sender, Classes.Components.PSO2Controller.VisibleNotifyEventArgs e)
        {
            ProgressBar_Visible(e.Visible);
        }
        private void Result_RingNotify(object sender, Classes.Components.PSO2Controller.VisibleNotifyEventArgs e)
        {
            Ring_Visible(e.Visible);
        }
        #endregion

        #region "Form Codes"
        private void ProgressBar_Visible(bool myBool)
        {
            mainProgressBarHost.Visible = myBool;
            if (myBool)
                mainProgressBarHost.BringToFront();
            else
                mainProgressBarHost.SendToBack();
        }
        private void Ring_Visible(bool myBool)
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
            Ring_Visible(true);
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
            result.ProgressChanged += Result_ProgressChanged;
            result.StepChanged += Result_StepChanged;
            result.ProgressBarNotify += Result_ProgressBarNotify;
            result.RingNotify += Result_RingNotify;

            result.EnglishPatchNotify += PSO2Controller_EnglishPatchNotify;
            result.LargeFilesPatchNotify += Result_LargeFilesPatchNotify;
            //result.StoryPatchNotify += PSO2Controller_StoryPatchNotify;
            return result;
        }

        private void installToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
            {
                Classes.Components.Task currentTask = (Classes.Components.Task)this.englishPatchContext.Tag;
                if (currentTask == Classes.Components.Task.EnglishPatch)
                {
                    if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskInstallEnglish", "Do you want to create backups and install the English Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                        this.EnglishPatchButton.Text = "English Patch: Installing";
                        this._pso2controller.InstallEnglishPatch();
                    }
                }
                else if (currentTask == Classes.Components.Task.LargeFilesPatch)
                {
                    if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskInstallLargeFiles", "Do you want to create backups and install the LargeFiles Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                        this.LargeFilesPatchButton.Text = "LargeFiles Patch: Installing";
                        //this._pso2controller.InstallLargeFilesPatch();
                    }
                }
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this._pso2controller.IsBusy)
            {
                Classes.Components.Task currentTask = (Classes.Components.Task)this.englishPatchContext.Tag;
                if (currentTask == Classes.Components.Task.EnglishPatch)
                {
                    if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskUninstallEnglish", "Do you want to uninstall the English Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.EnglishPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                        this.EnglishPatchButton.Text = "English Patch: Uninstalling";
                        this._pso2controller.UninstallEnglishPatch();
                    }
                }
                else if (currentTask == Classes.Components.Task.LargeFilesPatch)
                {   
                    if (MetroMessageBox.Show(this, Classes.LanguageManager.GetMessageText("AskUninstallLargeFiles", "Do you want to uninstall the LargeFiles Patch ?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.LargeFilesPatchButton.FlatAppearance.BorderColor = Color.Yellow;
                        this.LargeFilesPatchButton.Text = "LargeFiles Patch: Uninstalling";
                        //this._pso2controller.UninstallLargeFilesPatch();
                    }
                }

            }
        }
        #endregion

        #region "Startup Codes"
        private void BWorker_Boot_DoWork(object sender, DoWorkEventArgs e)
        {
            //Ping the 7z
            string libPath = DefaultValues.MyInfo.Filename.SevenZip.SevenZipLibPath;
            this.PrintText(Classes.LanguageManager.GetMessageText("RARLibLoaded", "RAR library loaded successfully"));
            if (!DefaultValues.MyInfo.Filename.SevenZip.IsValid)
            {
                this.PrintText(Classes.LanguageManager.GetMessageText("InvalidSevenZipLib", "SevenZip library is invalid or not existed. Redownloading"));
                //WakeUpCall for 7z
                string url = DefaultValues.MyServer.Web.GetDownloadLink + "/" + System.IO.Path.GetFileNameWithoutExtension(DefaultValues.MyInfo.Filename.SevenZip.SevenZipLibName) + ".rar";
                WebClientPool.GetWebClient(DefaultValues.MyServer.Web.GetDownloadLink).DownloadFile(url, libPath + ".rar");
                using (SharpCompress.Archives.Rar.RarArchive libPathArchive = SharpCompress.Archives.Rar.RarArchive.Open(libPath + ".rar"))
                    Classes.Components.AbstractExtractor.Unrar(libPathArchive, ApplicationInfo.ApplicationDirectory, null);
                try { System.IO.File.Delete(libPath + ".rar"); } catch { }
            }
            Classes.Components.AbstractExtractor.SetSevenZipLib(libPath);
            this.PrintText(Classes.LanguageManager.GetMessageText("SevenZipLibLoaded", "SevenZip library loaded successfully"));
            //Ping AIDA for the server
            Classes.AIDA.GetIdeaServer();

        }

        private void BWorker_Boot_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Classes.Log.LogManager.GeneralLog.Print(e.Error);
                MetroMessageBox.Show(this, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            else
            {
                this.refreshToolStripMenuItem.PerformClick();
                this._pso2controller = CreatePSO2Controller();
                Ring_Visible(false);
            }
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
