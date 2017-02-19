using System;
using System.Diagnostics;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.IO;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.ComponentModel;
using System.Collections.Generic;


namespace PSO2ProxyLauncherNew.Classes.Components.Patches
{
    class LargeFilesPatchManager : PatchManager
    {
        public new string VersionString { get { return MySettings.Patches.LargeFilesVersion; } private set { MySettings.Patches.LargeFilesVersion = value; } }

        public LargeFilesPatchManager() : base()
        {
            this.bWorker_install.DoWork += this.OnInstalling;
            this.bWorker_install.RunWorkerCompleted += this.OnInstalled;
            this.bWorker_uninstall.DoWork += this.OnUninstalling;
            this.bWorker_uninstall.RunWorkerCompleted += this.OnUninstalled;
            this.myWebClient_ForAIDA.DownloadStringCompleted += this.myWebClient_ForAIDA_DownloadStringCompleted;
        }

        #region "Install Patch"
        /*public override void InstallPatch()
        {
            this.CheckUpdate(new Uri(Infos.DefaultValues.AIDA.GetWebLink + Infos.DefaultValues.AIDA.Web.Patches.LargeFilesPatchLink), true);
        }
        public override void CheckUpdate()
        {
            this.CheckUpdate(new Uri(Infos.DefaultValues.AIDA.GetWebLink + Infos.DefaultValues.AIDA.Web.Patches.LargeFilesPatchLink), false);
        }

        protected virtual void CheckUpdate(Uri url, bool force)
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("CheckingLargeFilesPatchUpdate", "Checking for LargeFiles Patch updates")));
                this.myWebClient_ForAIDA.DownloadStringAsync(url, new WebClientInstallingMetaWrapper(0, new InstallingMeta(true, force)));
            }
        }*/

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
                            DateTime dt = AIDA.StringToDateTime(e.Result, "MM-dd-yyyy", '-');
                            InstallingMeta newMeta = new InstallingMeta(meta.Meta.Backup, meta.Meta.Force, dt);
                            if (VersionString != newMeta.NewVersionString)
                            {
                                PatchNotificationEventArgs theevent = new PatchNotificationEventArgs(true, newMeta.NewVersionString, VersionString);
                                this.OnPatchNotification(theevent);
                                if (meta.Meta.Force || theevent.Continue)
                                {
                                    this.OnHandledException(new Infos.HandledExceptionEventArgs(new NotImplementedException("This function is not available yet.")));
                                    //InstallPatchEx(theevent, dt);
                                }
                            }
                            else
                                this.OnPatchInstalled(new PatchFinishedEventArgs(newMeta.NewVersionString));
                        }
                        else
                            this.OnHandledException(new Infos.HandledExceptionEventArgs(new System.Exception("Failed to check for patch.\r\n" + e.Result)));
                    }
                }
            }
        }
        
        protected virtual void InstallPatchEx(PatchNotificationEventArgs myEventArgs, DateTime newver)
        {
            try
            {
                if (myEventArgs == null)
                {
                    myEventArgs = new PatchNotificationEventArgs(true, newver.ToVersionString(), this.VersionString);
                }
                string filePath = Infos.DefaultValues.MyInfo.Directory.Folders.LargeFilesPatch;
                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("DownloadingLargeFilesPatch", "Downloading new LargeFiles Patch version")));
                CustomWebClient.DownloadInfoCollection aaay = new CustomWebClient.DownloadInfoCollection();
                //aaay.Add(Infos.DefaultValues.AIDA.GetWebLink + Infos.DefaultValues.AIDA.Web.Patches.LargeFiles.LargeFilesPatcherLink, Path.Combine(filePath, "pso2-transam.exe"));
                //aaay.Add(Infos.DefaultValues.AIDA.GetWebLink + Infos.DefaultValues.AIDA.Web.Patches.LargeFiles.LargeFilesDatabaseLink, Path.Combine(filePath, "largefiledb.7zdb"));
                this.myWebClient_ForAIDA.DownloadFileListAsync(aaay, new TransarmWorkerInfo(myEventArgs, filePath, newver, myEventArgs.Backup));
            }
            catch (Exception ex) { this.OnHandledException(new Infos.HandledExceptionEventArgs(ex)); }
        }

        private void InstallPatchEx_DownloadProgress(object sender, ProgressChangedEventArgs e)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(1, e.ProgressPercentage));
        }
        private void InstallPatchEx_callback(object sender, AsyncCompletedEventArgs e)
        {
            TransarmWorkerInfo state = e.UserState as TransarmWorkerInfo;
            if (e.Error != null)
            {
                this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as PatchNotificationEventArgs).NewPatchVersion));
                this.OnHandledException(new Infos.HandledExceptionEventArgs(e.Error));
            }
            else if (e.Cancelled)
                this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as PatchNotificationEventArgs).NewPatchVersion));
            else
                this.bWorker_install.RunWorkerAsync(state);
        }

        protected virtual void OnInstalling(object sender, DoWorkEventArgs e)
        {
            return;
            TransarmWorkerInfo seed = e.Argument as TransarmWorkerInfo;
            PatchNotificationEventArgs seedEvent = seed.Params as PatchNotificationEventArgs;

            string pso2datadir = DefaultValues.Directory.PSO2Win32Data;
            string largefilesBackupFolder = DefaultValues.Directory.Backup.LargeFiles;
            this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("BeginLargeFilesPatchFiles", "Extracting LargeFiles Patch data")));
            string myPatcher = Path.Combine(seed.Path, "pso2-transam.exe");
            string my7zDB = Path.Combine(seed.Path, "largefiledb.7zdb");
            string myDB = string.Empty;
            bool isOkay = false;
            using (SevenZip.SevenZipExtractor archive = new SevenZip.SevenZipExtractor(my7zDB))
            {
                AbstractExtractor.SevenZipExtractResult result = AbstractExtractor.Extract7z(archive, seed.Path, null);
                isOkay = result.IsSuccess;
                myDB = result.SuccessItems[0].FileName;
            }
            if (isOkay)
            {
                if (seed.Backup)
                {
                    this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("BeginRestoringLargeFilesPatchFiles", "Getting LargeFiles Patch filelist")));
                    string rawtbl = this.myWebClient_ForAIDA.DownloadString("");
                    string[] tbl_files = AIDA.StringToTableString(rawtbl);
                    string originalFile, backupFile, currentIndexString;
                    this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("CreatingLargeFilesPatchBackup", "Creating backup for LargeFiles Patch files")));
                    int total = tbl_files.Length;
                    Directory.CreateDirectory(largefilesBackupFolder);
                    for (int i = 0; i < tbl_files.Length; i++)
                    {
                        currentIndexString = tbl_files[i];
                        originalFile = Path.Combine(pso2datadir, currentIndexString);
                        backupFile = Path.Combine(largefilesBackupFolder, currentIndexString);
                        File.Copy(originalFile, backupFile, true);
                        this.OnProgressChanged(new ProgressChangedEventArgs(1, (int)(Math.Round((double)(((i + 1) * 100) / total)))));
                    }
                }

                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("CallTransarmPatcherBackup", "Call patcher and wait for patcher finish the job")));
                this.OnProgressChanged(new ProgressChangedEventArgs(3, true));
                Process patcher = Infos.CommonMethods.MakeProcess(myPatcher);
                //-i "Backup/" -h largefiles-10-7-2016 lf.stripped.db "Out"
                string MyBaseDateString = "largefiles-" + seed.Date.Month.ToString() + "-" + seed.Date.Day.ToString() + "-" + seed.Date.Year.ToString();
                //lf.stripped.db
                //Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesBackupFolder

                List<string> myParams = new List<string>();
                myParams.Add(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.paramNodeForBackupOutput);
                myParams.Add(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesBackupFolder);

                myParams.Add(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.paramNodeForOutput);
                myParams.Add(MyBaseDateString);
                myParams.Add(myDB);
                myParams.Add(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.ValidPath(DefaultValues.Directory.PSO2Win32Data));

                string veda = Path.Combine(DefaultValues.Directory.PSO2Dir, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.VEDA_Filename);
                File.WriteAllText(veda, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.VEDA_Thingie);
                string asdadasd = Infos.CommonMethods.TableStringToArgs(myParams);
                Log.LogManager.GetLog("asdasd.txt", true).Print(asdadasd);
                patcher.StartInfo.Arguments = asdadasd;
                patcher.StartInfo.WorkingDirectory = seed.Path;
                patcher.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                if (Infos.OSVersionInfo.Name.ToLower() != "windows xp")
                    patcher.StartInfo.Verb = "runas";
                patcher.StartInfo.UseShellExecute = false;
                patcher.Start();
                patcher.WaitForExit();
                File.Delete(veda);
                this.OnProgressChanged(new ProgressChangedEventArgs(3, false));
                //Log.LogManager.GetLogDefaultPath("LargeFile.txt", true).Print("LargeFile Exit COde: " + patcher.ExitCode.ToString());
                try
                {
                    if ((patcher != null) && (patcher.ExitCode == 0))
                    {
                        e.Result = seed.Date.ToVersionString();
                    }
                    else
                    {
                        if (seed.Backup)
                            if (Directory.Exists(largefilesBackupFolder))
                            {
                                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("RollbackLargeFilesPatch", "Rolling back the LargeFiles Patch installation")));
                                string[] tbl_backup = Directory.GetFiles(largefilesBackupFolder, "*", SearchOption.TopDirectoryOnly);
                                string originalFile, backupFile, currentIndexString;
                                int total = tbl_backup.Length;
                                for (int i = 0; i < tbl_backup.Length; i++)
                                {
                                    currentIndexString = Path.GetFileName(tbl_backup[i]);
                                    originalFile = Path.Combine(pso2datadir, currentIndexString);
                                    backupFile = Path.Combine(largefilesBackupFolder, currentIndexString);
                                    File.Delete(originalFile);
                                    File.Move(backupFile, originalFile);
                                    this.OnProgressChanged(new ProgressChangedEventArgs(1, (int)(Math.Round((double)(((i + 1) * 100) / total)))));
                                }
                            }
                        throw new Exception(LanguageManager.GetMessageText("CancelLargeFilesPatchFiles", "User cancelled or the patcher closed with Error(s)."));
                    }
                }
                catch (System.Net.WebException) { }
            }
            else
            {
                throw new Exception(LanguageManager.GetMessageText("ErrorBeginLargeFilesPatchFiles", "Bad archive file or unknown error happened while") + " " + LanguageManager.GetMessageText("BeginLargeFilesPatchFiles", "Extracting LargeFiles Patch data"));
            }
            try
            { Directory.Delete(seed.Path, true); }
            catch (IOException)
            {
                try
                {
                    Infos.CommonMethods.EmptyFolder(seed.Path);
                    Directory.Delete(seed.Path, true);
                }
                catch { }
            }
            e.Result = seedEvent.NewPatchVersion;
        }

        protected virtual void OnInstalled(object sender, RunWorkerCompletedEventArgs e)
        {
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
            this.UninstallPatch(new Uri(Classes.AIDA.WebPatches.PatchesFileListInfos));
        }

        protected virtual void UninstallPatch(Uri address)
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("BeginRestoringLargeFilesPatchFiles", "Getting LargeFiles Patch filelist")));
                this.OnProgressChanged(new ProgressChangedEventArgs(3, true));
                this.OnProgressChanged(new ProgressChangedEventArgs(2, true));
                this.myWebClient_ForAIDA.DownloadStringAsync(address, new WorkerInfo("UninstallPatchEx"));
            }
        }

        protected void OnUninstalling(object sender, DoWorkEventArgs e)
        {
            WorkerInfo wi = e.Argument as WorkerInfo;
            this.OnProgressChanged(new ProgressChangedEventArgs(0, LanguageManager.GetMessageText("RestoringLargeFilesPatchFiles", "Restoring LargeFiles Patch files")));
            string sourceTable = string.Empty;
            using (var theTextReader = new StringReader(wi.Params as string))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(theTextReader))
                while (jsonReader.Read())
                    if (jsonReader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                        if (jsonReader.Value is string && (jsonReader.Value as string).ToLower() == "largefileslist")
                        {
                            sourceTable = jsonReader.ReadAsString();
                        }

            string[] tbl_files = AIDA.StringToTableString(sourceTable);
            string pso2datafolder = DefaultValues.Directory.PSO2Win32Data;
            string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.LargeFiles);
            List<string> backup_files = new List<string>();
            if (Directory.Exists(englishBackupFolder))
                foreach (string derp in Directory.GetFiles(englishBackupFolder,"*", SearchOption.TopDirectoryOnly))
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
    }
}
