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
    class LargeFilesPatchManager : PatchManager
    {
        public new string VersionString { get { return MySettings.Patches.LargeFilesVersion; } private set { MySettings.Patches.LargeFilesVersion = value; } }
        private Process patcherProcess;

        public LargeFilesPatchManager() : base()
        {
            this.bWorker_install.DoWork += this.OnInstalling;
            this.bWorker_install.RunWorkerCompleted += this.OnInstalled;
            this.bWorker_uninstall.DoWork += this.OnUninstalling;
            this.bWorker_uninstall.RunWorkerCompleted += this.OnUninstalled;
            this.bworker_RestoreBackup.DoWork += this.bworker_RestoreBackup_DoWork;
            this.bworker_RestoreBackup.RunWorkerCompleted += this.OnUninstalled;
            this.myWebClient_ForAIDA.DownloadStringCompleted += this.myWebClient_ForAIDA_DownloadStringCompleted;
            this.myWebClient_ForAIDA.DownloadFileCompleted += this.MyWebClient_ForAIDA_DownloadFileCompleted;
            this.myWebClient_ForAIDA.DownloadFileProgressChanged += this.MyWebClient_ForAIDA_DownloadFileProgressChanged;
            this.myWebClient_ForAIDA.DownloadProgressChanged += this.MyWebClient_ForAIDA_DownloadProgressChanged;
            this.patcherProcess = null;
        }

        #region "Install Patch"
        public override void InstallPatch()
        {
            this.CheckUpdate(new Uri(AIDA.WebPatches.PatchesInfos), true);
        }
        public override void CheckUpdate()
        {
            this.CheckUpdate(new Uri(AIDA.WebPatches.PatchesInfos), false);
        }

        protected virtual void CheckUpdate(Uri url, bool force)
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("CheckingLargeFilesPatchUpdate", "Checking for LargeFiles Patch updates")));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                this.myWebClient_ForAIDA.DownloadStringAsync(url, new WebClientInstallingMetaWrapper(0, new InstallingMeta(true, force)));
            }
        }

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
                //this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
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
                            string hue = AIDA.FlatJsonFetch<string>(e.Result, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesTransAmDate);
                            //System.Windows.Forms.MessageBox.Show(hue, "awghalwihgaliwhglaihwg");
                            if (!string.IsNullOrWhiteSpace(hue))
                            {
                                DateTime dt = AIDA.StringToDateTime(hue, "M/d/yyyy", '/');
                                if (dt != DateTime.MinValue)
                                {
                                    InstallingMeta newMeta = new InstallingMeta(meta.Meta.Backup, meta.Meta.Force, dt);
                                    if (VersionString != newMeta.NewVersionString)
                                    {
                                        PatchNotificationEventArgs theevent = new PatchNotificationEventArgs(true, newMeta.NewVersionString, VersionString);
                                        this.OnPatchNotification(theevent);
                                        if (meta.Meta.Force || theevent.Continue)
                                        {
                                            //this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException("This function is not available yet.")));
                                            InstallPatchEx(theevent, dt);
                                        }
                                    }
                                    else
                                        this.OnPatchInstalled(new PatchFinishedEventArgs(newMeta.NewVersionString));
                                }
                                else
                                    this.OnHandledException(new HandledExceptionEventArgs(new System.Exception("Failed to check for patch.\r\n" + e.Result)));
                            }
                            else
                                this.OnPatchInstalled(new PatchFinishedEventArgs(hue));
                        }
                        else
                            this.OnHandledException(new HandledExceptionEventArgs(new System.Exception("Failed to check for patch.\r\n" + e.Result)));
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
                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("DownloadingLargeFilesPatch", "Downloading new LargeFiles Patch version")));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                CustomWebClient.DownloadInfoCollection aaay = new CustomWebClient.DownloadInfoCollection();
                aaay.Add(AIDA.WebPatches.TransAmEXE, Path.Combine(filePath, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.TransAmEXE));
                aaay.Add(AIDA.WebPatches.LargeFilesDB, Path.Combine(filePath, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesDB + "zip"));
                this.myWebClient_ForAIDA.DownloadFileListAsync(aaay, new TransarmWorkerInfo(myEventArgs, filePath, newver, myEventArgs.Backup));
            }
            catch (Exception ex) { this.OnHandledException(new HandledExceptionEventArgs(ex)); }
        }

        private void MyWebClient_ForAIDA_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            TransarmWorkerInfo meta = null;
            if (e.UserState != null)
            {
                if (e.UserState is TransarmWorkerInfo)
                    meta = e.UserState as TransarmWorkerInfo;
            }
            if (e.Error != null)
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
            else if (e.Cancelled)
            { }
            else
            {
                if (meta != null)
                    this.bWorker_install.RunWorkerAsync(meta);
            }
        }

        private void MyWebClient_ForAIDA_DownloadFileProgressChanged(object sender, ExtendedWebClient.DownloadFileProgressChangedEventArgs e)
        {
            this.OnCurrentTotalProgressChanged(new Events.ProgressEventArgs(e.TotalFileCount));
            this.OnCurrentProgressChanged(new Events.ProgressEventArgs(e.CurrentFileIndex + 1));
        }

        private void MyWebClient_ForAIDA_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.OnCurrentProgressChanged(new ProgressEventArgs(e.ProgressPercentage));
        }

        private void InstallPatchEx_callback(object sender, AsyncCompletedEventArgs e)
        {
            TransarmWorkerInfo state = e.UserState as TransarmWorkerInfo;
            if (e.Error != null)
            {
                this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as PatchNotificationEventArgs).NewPatchVersion));
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
            }
            else if (e.Cancelled)
                this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as PatchNotificationEventArgs).NewPatchVersion));
            else
                this.bWorker_install.RunWorkerAsync(state);
        }

        protected virtual void OnInstalling(object sender, DoWorkEventArgs e)
        {
            //return;
            TransarmWorkerInfo seed = e.Argument as TransarmWorkerInfo;
            PatchNotificationEventArgs seedEvent = seed.Params as PatchNotificationEventArgs;

            string pso2datadir = DefaultValues.Directory.PSO2Win32Data;
            string largefilesBackupFolder = Path.Combine(pso2datadir, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.LargeFiles);
            this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("BeginLargeFilesPatchFiles", "Extracting LargeFiles Patch data")));
            string myPatcher = Path.Combine(seed.Path, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.TransAmEXE);
            string my7zDB = Path.Combine(seed.Path, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.LargeFilesDB + "zip");
            string myDB = string.Empty;
            bool isOkay = false;
            var result = AbstractExtractor.ExtractZip(my7zDB, seed.Path, null);
            isOkay = result.IsSuccess;
            myDB = result.SuccessItems[0].Key;
            File.Delete(my7zDB);
            if (isOkay)
            {
                if (false)
                {
                    this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("BeginRestoringLargeFilesPatchFiles", "Getting LargeFiles Patch filelist")));
                    string rawtbl = this.myWebClient_ForAIDA.DownloadString(Classes.AIDA.WebPatches.PatchesFileListInfos);
                    string sourceTable = string.Empty;
                    using (var theTextReader = new StringReader(rawtbl))
                    using (var jsonReader = new Newtonsoft.Json.JsonTextReader(theTextReader))
                        while (jsonReader.Read())
                            if (jsonReader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                                if (jsonReader.Value is string && (jsonReader.Value as string).ToLower() == "largefileslist")
                                {
                                    sourceTable = jsonReader.ReadAsString();
                                }

                    string[] tbl_files = AIDA.StringToTableString(sourceTable);
                    string originalFile, backupFile, currentIndexString;
                    this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("CreatingLargeFilesPatchBackup", "Creating backup for LargeFiles Patch files")));
                    int total = tbl_files.Length;
                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(total));
                    Directory.CreateDirectory(largefilesBackupFolder);
                    for (int i = 0; i < tbl_files.Length; i++)
                    {
                        currentIndexString = tbl_files[i];
                        originalFile = Path.Combine(pso2datadir, currentIndexString);
                        backupFile = Path.Combine(largefilesBackupFolder, currentIndexString);
                        File.Copy(originalFile, backupFile, true);
                        this.OnCurrentProgressChanged(new ProgressEventArgs(i + 1));
                    }
                }

                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("CallTransarmPatcherBackup", "Call patcher and wait for patcher finish the job")));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                patcherProcess = Infos.CommonMethods.MakeProcess(myPatcher);
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
                string asdadasd = Infos.CommonMethods.TableStringToArgs(myParams);
                //Log.LogManager.GetLog("asdasd.txt", true).Print(asdadasd);
                patcherProcess.StartInfo.Arguments = asdadasd;
                patcherProcess.StartInfo.WorkingDirectory = seed.Path;
                patcherProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                if (Infos.OSVersionInfo.Name.ToLower() != "windows xp")
                    patcherProcess.StartInfo.Verb = "runas";
                Exception exVeda = AIDA.TransarmOrVedaOrWhatever.VEDA_Activate();
                if (exVeda == null)
                {
                    patcherProcess.StartInfo.UseShellExecute = false;
                    patcherProcess.Start();
                    patcherProcess.WaitForExit();
                    File.Delete(veda);
                    this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                    //Log.LogManager.GetLogDefaultPath("LargeFile.txt", true).Print("LargeFile Exit COde: " + patcher.ExitCode.ToString());
                    try
                    {
                        if ((patcherProcess != null) && (patcherProcess.ExitCode == 0))
                        {
                            patcherProcess = null;
                            e.Result = seed.Date.ToVersionString();
                        }
                        else
                        {
                            patcherProcess = null;
                            if (seed.Backup)
                                if (Directory.Exists(largefilesBackupFolder))
                                {
                                    this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                                    this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("RollbackLargeFilesPatch", "Rolling back the LargeFiles Patch installation")));
                                    string[] tbl_backup = Directory.GetFiles(largefilesBackupFolder, "*", SearchOption.TopDirectoryOnly);
                                    string originalFile, backupFile, currentIndexString;
                                    int total = tbl_backup.Length;
                                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(total));
                                    for (int i = 0; i < tbl_backup.Length; i++)
                                    {
                                        currentIndexString = Path.GetFileName(tbl_backup[i]);
                                        originalFile = Path.Combine(pso2datadir, currentIndexString);
                                        backupFile = Path.Combine(largefilesBackupFolder, currentIndexString);
                                        File.Delete(originalFile);
                                        File.Move(backupFile, originalFile);
                                        this.OnCurrentProgressChanged(new ProgressEventArgs(i + 1));
                                    }
                                }
                            throw new Exception(LanguageManager.GetMessageText("CancelLargeFilesPatchFiles", "User cancelled or the patcher closed with Error(s)."));
                        }
                        File.Delete(Path.Combine(seed.Path, myDB));
                        File.Delete(myPatcher);
                    }
                    catch (System.Net.WebException) { }
                }
                else
                    throw exVeda;
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
            this.IsBusy = false;
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            if (e.Error != null)
            {
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
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
                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("BeginRestoringLargeFilesPatchFiles", "Getting LargeFiles Patch filelist")));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                this.myWebClient_ForAIDA.DownloadStringAsync(address, new WorkerInfo("UninstallPatchEx"));
            }
        }

        protected void OnUninstalling(object sender, DoWorkEventArgs e)
        {
            WorkerInfo wi = e.Argument as WorkerInfo;
            this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("RestoringLargeFilesPatchFiles", "Restoring LargeFiles Patch files")));
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
                            nonExist.Add(currentStringIndex);
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
                this.OnCurrentTotalProgressChanged(new ProgressEventArgs(tbl_files.Length));
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
            this.OnCurrentProgressChanged(new ProgressEventArgs(arg0));
            return true;
        }

        protected void OnUninstalled(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            if (e.Error != null)
            {
                this.IsBusy = false;
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            }
            else if (e.Cancelled)
            {
                this.IsBusy = false;
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            }
            else
            {
                if (e.Result != null && e.Result is bool)
                {
                    bool myBool = (bool)e.Result;
                    if (myBool)
                        this.OnPatchUninstalled(new PatchFinishedEventArgs(true, string.Empty));
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
            string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.LargeFiles);
            if (Directory.Exists(englishBackupFolder))
            {
                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("RestoringLargeFilesPatchFiles", "Restoring LargeFiles Patch files")));
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
            }
            try
            { System.IO.Directory.Delete(englishBackupFolder, true); }
            catch { }
            e.Result = true;
        }
        #endregion

        #region "Cancel Support"
        public override void CancelAsync()
        {
            //this._cancelling = true;
            if (this.bWorker_install.IsBusy)
                this.bWorker_install.CancelAsync();
            if (this.bWorker_uninstall.IsBusy)
                this.bWorker_uninstall.CancelAsync();
            if (this.bworker_RestoreBackup.IsBusy)
                this.bworker_RestoreBackup.CancelAsync();
            if (this.myWebClient_ForAIDA.IsBusy)
                this.myWebClient_ForAIDA.CancelAsync();
            /*string filePath = Infos.DefaultValues.MyInfo.Directory.Folders.LargeFilesPatch;
                this.OnCurrentStepChanged(new StepEventArgs(LanguageManager.GetMessageText("DownloadingLargeFilesPatch", "Downloading new LargeFiles Patch version")));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                CustomWebClient.DownloadInfoCollection aaay = new CustomWebClient.DownloadInfoCollection();
                aaay.Add(AIDA.WebPatches.TransAmEXE, Path.Combine(filePath, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.TransAmEXE));
                */
            if (patcherProcess != null)
            {
                if (!patcherProcess.HasExited)
                {
                    patcherProcess.CloseMainWindow();
                    patcherProcess.WaitForExit(300);
                    if (!patcherProcess.HasExited)
                        patcherProcess.Kill();
                }

            }
            //Infos.CommonMethods.KillAllProcesses();
        }
        #endregion
    }
}
