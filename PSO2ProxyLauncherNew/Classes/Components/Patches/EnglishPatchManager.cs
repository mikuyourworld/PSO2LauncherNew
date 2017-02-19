using System;
using SharpCompress.Archives.Rar;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.IO;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.ComponentModel;
using System.Collections.Generic;

namespace PSO2ProxyLauncherNew.Classes.Components.Patches
{
    class EnglishPatchManager : PatchManager
    {
        public new string VersionString { get { return MySettings.Patches.EnglishVersion; } private set { MySettings.Patches.EnglishVersion = value; } }
        
        public EnglishPatchManager() : base()
        {
            this.bWorker_install.DoWork += this.OnInstalling;
            this.bWorker_install.RunWorkerCompleted += this.OnInstalled;
            this.bWorker_uninstall.DoWork += this.OnUninstalling;
            this.bWorker_uninstall.RunWorkerCompleted += this.OnUninstalled;
            this.myWebClient_ForAIDA.DownloadProgressChanged += this.InstallPatchEx_DownloadProgress;
            this.myWebClient_ForAIDA.DownloadFileCompleted += this.MyWebClient_ForAIDA_DownloadFileCompleted;
            this.myWebClient_ForAIDA.DownloadStringCompleted += this.MyWebClient_ForAIDA_DownloadStringCompleted;
        }

        #region "Install Patch"
        public override void InstallPatch()
        {
            this.CheckUpdate(new Uri(Classes.AIDA.WebPatches.PatchesInfos), true);
        }
        public override void CheckUpdate()
        {
            this.CheckUpdate(new Uri(Classes.AIDA.WebPatches.PatchesInfos), false);
        }

        protected virtual void CheckUpdate(Uri url, bool force)
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("CheckingEnglishPatchUpdate", "Checking for English Patch updates")));
                this.OnProgressChanged(new ProgressChangedEventArgs(3, true));
                this.OnProgressChanged(new ProgressChangedEventArgs(2, true));
                this.myWebClient_ForAIDA.DownloadStringAsync(url, new WebClientInstallingMetaWrapper(0, new InstallingMeta(true, force)));
            }
        }

        protected virtual void InstallPatchEx(PatchNotificationEventArgs myEventArgs, System.Uri url)
        {
            try
            {
                if (myEventArgs == null)
                {
                    string patchversion = Path.GetFileNameWithoutExtension(url.OriginalString);
                    myEventArgs = new PatchNotificationEventArgs(true, patchversion, this.VersionString);
                }
                string filePath = Path.Combine(Infos.DefaultValues.MyInfo.Directory.Patches, myEventArgs.NewPatchVersion);
                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("DownloadingEnglishPatch", "Downloading new English Patch version")));
                this.OnProgressChanged(new ProgressChangedEventArgs(2, true));
                this.myWebClient_ForAIDA.DownloadFileAsync(url, filePath, new WorkerInfo("InstallPatchEx_callback", myEventArgs, filePath, url, myEventArgs.Backup));
            }
            catch (Exception ex) { this.OnHandledException(new Infos.HandledExceptionEventArgs(ex)); }
        }

        private void MyWebClient_ForAIDA_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            WorkerInfo state = null;
            if (e.UserState != null && e.UserState is WorkerInfo)
            {
                state = e.UserState as WorkerInfo;
            }
            if (e.Error != null)
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "InstallPatchEx_callback":
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as PatchNotificationEventArgs).NewPatchVersion));
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(e.Error));
                            break;
                        case "Uninstall_RedownloadCallback":
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnProgressChanged(new ProgressChangedEventArgs(3, false));
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(e.Error));
                            this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
                            break;
                        default:
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
            else if (e.Cancelled)
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "InstallPatchEx_callback":
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as PatchNotificationEventArgs).NewPatchVersion));
                            break;
                        case "Uninstall_RedownloadCallback":
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnProgressChanged(new ProgressChangedEventArgs(3, false));
                            this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
                            break;
                        default:
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
            else
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "InstallPatchEx_callback":
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.bWorker_install.RunWorkerAsync(state);
                            break;
                        case "Uninstall_RedownloadCallback":
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnProgressChanged(new ProgressChangedEventArgs(3, false));
                            this.OnPatchUninstalled(new PatchFinishedEventArgs(true, string.Empty));
                            break;
                        default:
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
        }

        private void InstallPatchEx_DownloadProgress(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(1, e.ProgressPercentage));
        }
        
        protected virtual void OnInstalling(object sender, DoWorkEventArgs e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(2, true));
            WorkerInfo seed = e.Argument as WorkerInfo;
            PatchNotificationEventArgs seedEvent = seed.Params as PatchNotificationEventArgs;
            using (FileStream fs = File.OpenRead(seed.Path))
            using (RarArchive archive = RarArchive.Open(fs))
            {
                string pso2datafolder = DefaultValues.Directory.PSO2Win32Data;
                if (seed.Backup)
                {
                    string tmppath;
                    string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.English);
                    string backuppath;
                    this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("CreatingEnglishPatchBackup", "Creating backup for English Patch files")));
                    int total = archive.Entries.Count;
                    int index = 0;
                    Directory.CreateDirectory(englishBackupFolder);
                    foreach (RarArchiveEntry entry in archive.Entries)
                    {
                        tmppath = Path.Combine(pso2datafolder, entry.Key);
                        backuppath = Path.Combine(englishBackupFolder, entry.Key);
                        File.Copy(tmppath, backuppath, true);
                        index++;
                        this.OnProgressChanged(new ProgressChangedEventArgs(1, (int)(Math.Round((double)(index / total), 2) * 100)));
                    }
                }
                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("InstallingEnglishPatch", "Installing English Patch")));
                var result = AbstractExtractor.Unrar(archive, pso2datafolder, extract_callback);
                if (!result.IsSuccess)
                {
                    if (seed.Backup)
                    {
                        string tmppath;
                        string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.English);
                        var rollbackList = Directory.GetFiles(englishBackupFolder, "*", SearchOption.TopDirectoryOnly);
                        int total = rollbackList.Length;
                        int index = 0;
                        this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("RollbackEnglishPatch", "Rolling back the English Patch installation")));
                        foreach (string dundun in rollbackList)
                        {
                            tmppath = Path.Combine(pso2datafolder, Path.GetFileName(dundun));
                            File.Delete(tmppath);
                            File.Move(dundun, tmppath);
                            index++;
                            this.OnProgressChanged(new ProgressChangedEventArgs(1, (int)(System.Math.Round((double)(index / total), 2) * 100)));
                        }
                    }
                    throw new Exception("Extract failed.");
                }
            }
            try
            { File.Delete(seed.Path); }
            catch { }
            e.Result = seedEvent.NewPatchVersion;
        }

        private void extract_callback(object sender, AbstractExtractor.RARExtractProgress e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(1, e.Percent));
        }

        protected virtual void OnInstalled(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
            this.OnProgressChanged(new ProgressChangedEventArgs(3, false));
            if (e.Error != null)
            {
                this.OnHandledException(new Infos.HandledExceptionEventArgs(e.Error));
                this.OnPatchInstalled(new PatchFinishedEventArgs(false, null));
            }
            else if (e.Cancelled)
                this.OnPatchInstalled(new PatchFinishedEventArgs(false, e.Result as string));
            else
            {
                string patchver = e.Result as string;
                this.VersionString = patchver;
                this.OnPatchInstalled(new PatchFinishedEventArgs(true, patchver));
            }
        }
        #endregion

        #region "Uninstall Patch"
        public override void UninstallPatch()
        {
            //Classes.AIDA.WebPatches.PatchesInfos
            this.UninstallPatch(new Uri(Classes.AIDA.WebPatches.PatchesFileListInfos));
        }

        protected virtual void UninstallPatch(Uri address)
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("BeginRestoringEnglishPatchFiles", "Getting English Patch filelist")));
                this.OnProgressChanged(new ProgressChangedEventArgs(3, true));
                this.OnProgressChanged(new ProgressChangedEventArgs(2, true));
                this.myWebClient_ForAIDA.DownloadStringAsync(address, new WorkerInfo("UninstallPatchEx"));
            }
        }

        private void MyWebClient_ForAIDA_DownloadStringCompleted(object sender, ExtendedWebClient.DownloadStringFinishedEventArgs e)
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
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(e.Error));
                            break;
                        default:
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(e.Error));
                            break;
                    }
            }
            else if (e.Cancelled)
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "UninstallPatchEx":
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
                            break;
                        default:
                            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
            else
            {
                this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
                if (state != null)
                    switch (state.Step)
                    {
                        case "UninstallPatchEx":
                            WorkerInfo wi = new WorkerInfo(string.Empty, e.Result, null, null, true);
                            this.bWorker_uninstall.RunWorkerAsync(wi);
                            break;
                        default:
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
                else if (meta != null)
                {
                    if (meta.Step == 0)
                    {
                        if (!string.IsNullOrEmpty(e.Result))
                        {
                            try
                            {
                                string newverstring = Classes.AIDA.FlatJsonFetch<string>(e.Result, "ENPatchOverrideURL");
                                if (!string.IsNullOrWhiteSpace(newverstring))
                                {
                                    System.Uri url = new System.Uri(newverstring);
                                    newverstring = Path.GetFileNameWithoutExtension(newverstring);
                                    InstallingMeta asd = new InstallingMeta(meta.Meta.Backup, meta.Meta.Force, newverstring);
                                    if (VersionString != newverstring)
                                    {
                                        PatchNotificationEventArgs theevent = new PatchNotificationEventArgs(true, newverstring, VersionString);
                                        this.OnPatchNotification(theevent);
                                        if (meta.Meta.Force || theevent.Continue)
                                            InstallPatchEx(theevent, url);
                                    }
                                    else
                                        this.OnPatchInstalled(new PatchFinishedEventArgs(VersionString));
                                }
                                else
                                    this.OnPatchInstalled(new PatchFinishedEventArgs(VersionString));
                            }
                            catch (UriFormatException uriEx) { this.OnHandledException(new Infos.HandledExceptionEventArgs(uriEx)); }
                        }
                        else
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(new System.Exception("Failed to check for patch.\r\n")));
                    }
                }
            }
        }

        protected void OnUninstalling(object sender, DoWorkEventArgs e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(2, true));
            WorkerInfo wi = e.Argument as WorkerInfo;
            this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("RestoringEnglishPatchFiles", "Restoring English Patch files")));
            string sourceTable = string.Empty;
            using (var theTextReader = new StringReader(wi.Params as string))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(theTextReader))
                while (jsonReader.Read())
                    if (jsonReader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                        if (jsonReader.Value is string && (jsonReader.Value as string).ToLower() == "enpatchlist")
                        {
                            sourceTable = jsonReader.ReadAsString();
                        }

            string[] tbl_files = AIDA.StringToTableString(sourceTable);
            string pso2datafolder = DefaultValues.Directory.PSO2Win32Data;
            string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.English);
            List<string> backup_files = new List<string>();
            if (Directory.Exists(englishBackupFolder))
                foreach (string derp in Directory.GetFiles(englishBackupFolder, "*", SearchOption.TopDirectoryOnly))
                    backup_files.Add(Path.GetFileName(derp).ToLower());
            string backedup;
            string data;
            string currentStringIndex;
            if (backup_files.Count > 0)
            {
                if (tbl_files.Length > backup_files.Count)
                {
                    int total = tbl_files.Length;
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
                            this.OnProgressChanged(new ProgressChangedEventArgs(1, (int)(Math.Round((double)(count / total), 2) * 100)));
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
                                this.OnProgressChanged(new ProgressChangedEventArgs(1, (int)(Math.Round((double)(count / total), 2) * 100)));
                            }
                        }
                    }
                    Directory.Delete(englishBackupFolder, true);
                    if (nonExist.Count > 0)
                    {
                        this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("RedownloadingMissingOriginalFiles", "Redownloading missing original files")));
                        using (ExtendedWebClient downloader = new ExtendedWebClient())
                        {
                            downloader.UserAgent = PSO2.DefaultValues.Web.UserAgent;
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
                    for (int i = 0; i < backup_files.Count; i++)
                    {
                        currentStringIndex = backup_files[i];
                        data = Path.Combine(pso2datafolder, currentStringIndex);
                        backedup = Path.Combine(englishBackupFolder, currentStringIndex);
                        if (File.Exists(backedup))
                        {
                            File.Delete(data);
                            File.Move(backedup, data);
                            this.OnProgressChanged(new ProgressChangedEventArgs(1, (int)(Math.Round((double)((i + 1) / total), 2) * 100)));
                        }
                    }
                    Directory.Delete(englishBackupFolder, true);
                    e.Result = true;
                }
            }
            else if (tbl_files.Length > 0)
            {
                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("RedownloadingMissingOriginalFiles", "Redownloading missing original files")));
                using (ExtendedWebClient downloader = new ExtendedWebClient())
                {
                    downloader.UserAgent = PSO2.DefaultValues.Web.UserAgent;
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
            this.OnProgressChanged(new ProgressChangedEventArgs(0, string.Format(LanguageManager.GetMessageText("RedownloadingMissingOriginalFiles_0", "Redownloading file {0}"), Path.GetFileName(e.UserToken))));
        }

        private bool Downloader_DownloadFileProgressChanged(long arg0, long arg1)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(1, (int)(Math.Round((double)(arg0 / arg1), 2) * 100)));
            return true;
        }

        protected void OnUninstalled(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
            this.OnProgressChanged(new ProgressChangedEventArgs(3, false));
            if (e.Error != null)
            {
                this.OnHandledException(new Infos.HandledExceptionEventArgs(e.Error));
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            }
            else if (e.Cancelled)
            {
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            }
            else
            {
                bool myBool = (bool)e.Result;
                this.OnPatchUninstalled(new PatchFinishedEventArgs(myBool, string.Empty));
            }

        }

        private void Uninstall_RedownloadCallback(object sender, AsyncCompletedEventArgs e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(2, false));
            this.OnProgressChanged(new ProgressChangedEventArgs(3, false));
            if (e.Error != null)
            {
                this.OnHandledException(new Infos.HandledExceptionEventArgs(e.Error));
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

        #region "Reinstall Patch"
        public override void ReinstallPatch()
        {
            this.CheckUpdate(new Uri(Classes.AIDA.WebPatches.PatchesInfos), true);
        }
        #endregion
    }
}
