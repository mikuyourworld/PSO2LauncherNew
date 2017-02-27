using System;
using System.Diagnostics;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.IO;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.ComponentModel;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes.Events;


namespace PSO2ProxyLauncherNew.Classes.Components.Patches
{
    class StoryPatchManager : PatchManager
    {
        public new string VersionString { get { return MySettings.Patches.StoryVersion; } private set { MySettings.Patches.StoryVersion = value; } }

        public StoryPatchManager() : base()
        {
            this.bWorker_uninstall.DoWork += this.OnUninstalling;
            this.bWorker_uninstall.RunWorkerCompleted += this.OnUninstalled;

            this.bworker_RestoreBackup.DoWork += this.bworker_RestoreBackup_DoWork;
            this.bworker_RestoreBackup.RunWorkerCompleted += this.OnUninstalled;
            this.myWebClient_ForAIDA.DownloadStringCompleted += this.myWebClient_ForAIDA_DownloadStringCompleted;
            //this.myWebClient_ForAIDA.DownloadFileCompleted += this.MyWebClient_ForAIDA_DownloadFileCompleted;
            //this.myWebClient_ForAIDA.DownloadFileProgressChanged += this.MyWebClient_ForAIDA_DownloadFileProgressChanged;
            //this.myWebClient_ForAIDA.DownloadProgressChanged += this.MyWebClient_ForAIDA_DownloadProgressChanged;
        }

        #region "Install Patch"

        private void myWebClient_ForAIDA_DownloadStringCompleted(object sender, ExtendedWebClient.DownloadStringFinishedEventArgs e)
        {
            WorkerInfo state = null;
            WebClientInstallingMetaWrapper meta = null;
            if (e.UserState != null)
            {
                if (e.UserState is WorkerInfo)
                    state = e.UserState as WorkerInfo;
                else if (e.UserState is WebClientInstallingMetaWrapper)
                    meta = e.UserState as WebClientInstallingMetaWrapper;
            }
            if (e.Error != null)
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "UninstallPatchEx":
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnHandledException(new HandledExceptionEventArgs(e.Error));
                            break;
                        default:
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnHandledException(new HandledExceptionEventArgs(e.Error));
                            break;
                    }
            }
            else if (e.Cancelled)
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "UninstallPatchEx":
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
                            break;
                        default:
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
            else
            {
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                if (state != null)
                    switch (state.Step)
                    {
                        case "UninstallPatchEx":
                            WorkerInfo wi = new WorkerInfo(string.Empty, e.Result, null, null, true);
                            this.bWorker_uninstall.RunWorkerAsync(wi);
                            break;
                        default:
                            this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
                else if (meta != null)
                {
                    if (meta.Step == 0)
                    {
                        if (!string.IsNullOrEmpty(e.Result))
                        {
                            DateTime dt = AIDA.StringToDateTime(e.Result, "MM/dd/yyyy", '/');
                            InstallingMeta newMeta = new InstallingMeta(meta.Meta.Backup, meta.Meta.Force, dt);
                            if (VersionString != newMeta.NewVersionString)
                            {
                                PatchNotificationEventArgs theevent = new PatchNotificationEventArgs(true, newMeta.NewVersionString, VersionString);
                                this.OnPatchNotification(theevent);
                                if (meta.Meta.Force || theevent.Continue)
                                {
                                    this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException("This function is not available yet.")));
                                    //InstallPatchEx(theevent, dt);
                                }
                            }
                            else
                                this.OnPatchInstalled(new PatchFinishedEventArgs(newMeta.NewVersionString));
                        }
                        else
                            this.OnHandledException(new HandledExceptionEventArgs(new System.Exception("Failed to check for patch.\r\n" + e.Result)));
                    }
                }
            }
        }
        #endregion

        #region "Uninstall Patch"
        public override void UninstallPatch()
        {
            this.UninstallPatch(new Uri(Classes.AIDA.WebPatches.PatchesFileListInfos));
        }

        protected virtual void UninstallPatch(Uri address)
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("BeginRestoringLargeFilesPatchFiles", "Getting LargeFiles Patch filelist")));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                this.myWebClient_ForAIDA.DownloadStringAsync(address, new WorkerInfo("UninstallPatchEx"));
            }
        }

        protected void OnUninstalling(object sender, DoWorkEventArgs e)
        {
            WorkerInfo wi = e.Argument as WorkerInfo;
            this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("RestoringStoryPatchFiles", "Restoring Story Patch files")));
            string sourceTable = string.Empty;
            using (var theTextReader = new StringReader(wi.Params as string))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(theTextReader))
                while (jsonReader.Read())
                    if (jsonReader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                        if (jsonReader.Value is string && (jsonReader.Value as string).ToLower() == "storylist")
                        {
                            sourceTable = jsonReader.ReadAsString();
                        }

            string[] tbl_files = AIDA.StringToTableString(sourceTable);
            string pso2datafolder = DefaultValues.Directory.PSO2Win32Data;
            string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.LargeFiles);
            List<string> backup_files = new List<string>();
            if (Directory.Exists(englishBackupFolder))
                foreach (string derp in Directory.GetFiles(englishBackupFolder, "*", SearchOption.TopDirectoryOnly))
                    backup_files.Add(Path.GetFileName(derp).ToLower());
            string backedup;
            string data;
            string currentStringIndex;
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
            if (backup_files.Count > 0)
            {
                if (tbl_files.Length > backup_files.Count)
                {
                    int total = tbl_files.Length;
                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(total));
                    int count = 0;
                    List<string> nonExist = new List<string>();
                    for (int i = 0; i < tbl_files.Length; i++)
                    {
                        currentStringIndex = tbl_files[i].ToLower();
                        data = Path.Combine(pso2datafolder, currentStringIndex);
                        backedup = Path.Combine(englishBackupFolder, currentStringIndex);
                        if (File.Exists(backedup))
                        {
                            backup_files.Remove(currentStringIndex);
                            File.Delete(data);
                            File.Move(backedup, data);
                            count++;
                            this.OnCurrentProgressChanged(new ProgressEventArgs(count + 1));
                        }
                        else
                        {
                            nonExist.Add(currentStringIndex);
                        }
                    }
                    if (backup_files.Count > 0)
                    {
                        for (int i = 0; i < backup_files.Count; i++)
                        {
                            currentStringIndex = backup_files[i];
                            data = Path.Combine(pso2datafolder, currentStringIndex);
                            backedup = Path.Combine(englishBackupFolder, currentStringIndex);
                            if (File.Exists(backedup))
                            {
                                File.Delete(data);
                                File.Move(backedup, data);
                                count++;
                                this.OnCurrentProgressChanged(new ProgressEventArgs(count + 1));
                            }
                        }
                    }
                    Directory.Delete(englishBackupFolder, true);
                    if (nonExist.Count > 0)
                    {
                        this.OnCurrentTotalProgressChanged(new ProgressEventArgs(nonExist.Count));
                        this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("RedownloadingMissingOriginalFiles", "Redownloading missing original files")));
                        using (CustomWebClient downloader = WebClientPool.GetWebClient_PSO2Download())
                        {
                            Dictionary<string, string> downloadlist = new Dictionary<string, string>();
                            for (int i = 0; i < nonExist.Count; i++)
                            {
                                currentStringIndex = nonExist[i];
                                downloadlist.Add("data/win32/" + currentStringIndex + DefaultValues.Web.FakeFileExtension, Path.Combine(pso2datafolder, currentStringIndex));
                            }
                            PSO2UpdateManager.RedownloadFiles(downloader, downloadlist, Downloader_StepProgressChanged, Downloader_DownloadFileProgressChanged, this.Uninstall_RedownloadCallback);
                            e.Result = false;
                        }
                    }
                    else
                    {
                        e.Result = true;
                    }
                }
                else
                {
                    int total = backup_files.Count;
                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(total));
                    for (int i = 0; i < backup_files.Count; i++)
                    {
                        currentStringIndex = backup_files[i];
                        data = Path.Combine(pso2datafolder, currentStringIndex);
                        backedup = Path.Combine(englishBackupFolder, currentStringIndex);
                        if (File.Exists(backedup))
                        {
                            File.Delete(data);
                            File.Move(backedup, data);
                            this.OnCurrentProgressChanged(new ProgressEventArgs(i + 1));
                        }
                    }
                    Directory.Delete(englishBackupFolder, true);
                    e.Result = true;
                }
            }
            else if (tbl_files.Length > 0)
            {
                this.OnCurrentTotalProgressChanged(new ProgressEventArgs(Convert.ToInt32(tbl_files.Length)));
                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("RedownloadingMissingOriginalFiles", "Redownloading missing original files")));
                using (CustomWebClient downloader = WebClientPool.GetWebClient_PSO2Download())
                {
                    Dictionary<string, string> downloadlist = new Dictionary<string, string>();
                    for (int i = 0; i < tbl_files.Length; i++)
                    {
                        currentStringIndex = tbl_files[i];
                        downloadlist.Add("data/win32/" + currentStringIndex + DefaultValues.Web.FakeFileExtension, Path.Combine(pso2datafolder, currentStringIndex));
                    }
                    PSO2UpdateManager.RedownloadFiles(downloader, downloadlist, Downloader_StepProgressChanged, Downloader_DownloadFileProgressChanged, this.Uninstall_RedownloadCallback);
                    e.Result = false;
                }
            }
            else
            {
                throw new Exception("Unknown Error");
                //Failed
            }
        }

        protected void Downloader_StepProgressChanged(object sender, PSO2UpdateManager.StringEventArgs e)
        {
            this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("RedownloadingMissingOriginalFiles_0", "Redownloading file {0}"), Path.GetFileName(e.UserToken))));
        }

        private bool Downloader_DownloadFileProgressChanged(int arg0, int arg1)
        {
            this.OnCurrentProgressChanged(new ProgressEventArgs(Convert.ToInt32(arg0)));
            return true;
        }

        protected void OnUninstalled(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            if (e.Error != null)
            {
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            }
            else if (e.Cancelled)
            {
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            }
            else
            {
                if (e.Result != null && e.Result is bool)
                {
                    bool myBool = (bool)e.Result;
                    if (myBool)
                    {
                        this.IsBusy = false;
                        this.OnPatchUninstalled(new PatchFinishedEventArgs(true, string.Empty));
                    }
                }
            }
        }

        private void Uninstall_RedownloadCallback(object sender, AsyncCompletedEventArgs e)
        {
            this.IsBusy = false;
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            if (e.Error != null)
            {
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            }
            else if (e.Cancelled)
            {
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            }
            else
            {
                this.OnPatchUninstalled(new PatchFinishedEventArgs(true, string.Empty));
            }
        }
        #endregion

        #region "Restore Patch"
        public override void RestoreBackup()
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.bworker_RestoreBackup.RunWorkerAsync();
            }
        }
        private void bworker_RestoreBackup_DoWork(object sender, DoWorkEventArgs e)
        {
            this.OnProgressBarStateChanged(new Events.ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
            string pso2datafolder = DefaultValues.Directory.PSO2Win32Data;
            string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.Story);
            if (Directory.Exists(englishBackupFolder))
            {
                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("RestoringStoryPatchFiles", "Restoring Story Patch files")));
                string currentStringIndex, data, backedup;
                string[] derp = Directory.GetFiles(englishBackupFolder, "*", SearchOption.TopDirectoryOnly);
                this.OnCurrentTotalProgressChanged(new ProgressEventArgs(derp.Length));
                for (int i = 0; i < derp.Length; i++)
                {
                    backedup = derp[i];
                    currentStringIndex = System.IO.Path.GetFileName(backedup);
                    data = Path.Combine(pso2datafolder, currentStringIndex);
                    if (File.Exists(backedup))
                    {
                        File.Delete(data);
                        File.Move(backedup, data);
                        this.OnCurrentProgressChanged(new ProgressEventArgs(i + 1));
                    }
                }
                e.Result = true;
            }
            else
                e.Result = false;
        }
        #endregion
    }
}
