﻿using System;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.IO;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.ComponentModel;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes.Events;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;

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
            this.bworker_RestoreBackup.DoWork += this.bworker_RestoreBackup_DoWork;
            this.bworker_RestoreBackup.RunWorkerCompleted += this.OnUninstalled;
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
                if (MySettings.MinimizeNetworkUsage)
                {
                    this.myWebClient_ForAIDA.CacheStorage = CacheStorage.DefaultStorage;
                    this.myWebClient_ForPSO2.CacheStorage = CacheStorage.DefaultStorage;
                }
                else
                {
                    this.myWebClient_ForAIDA.CacheStorage = null;
                    this.myWebClient_ForPSO2.CacheStorage = null;
                }
                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Checking0PatchUpdate", "Checking for {0} updates"), Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled)));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
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
                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Downloading0Patch", "Downloading new {0} version"), Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled)));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                this.OnCurrentTotalProgressChanged(new ProgressEventArgs(100));
                this.myWebClient_ForAIDA.DownloadFileAsync(url, filePath, new WorkerInfo("InstallPatchEx_callback", myEventArgs, filePath, url, myEventArgs.Backup));
            }
            catch (Exception ex) { this.OnHandledException(new HandledExceptionEventArgs(ex)); }
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
                            this.myWebClient_ForAIDA.Credentials = null;
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as PatchNotificationEventArgs).NewPatchVersion));
                            this.OnHandledException(new HandledExceptionEventArgs(e.Error));
                            break;
                        case "Uninstall_RedownloadCallback":
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnHandledException(new HandledExceptionEventArgs(e.Error));
                            this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
                            break;
                        default:
                            this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
            else if (e.Cancelled)
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "InstallPatchEx_callback":
                            this.myWebClient_ForAIDA.Credentials = null;
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as PatchNotificationEventArgs).NewPatchVersion));
                            break;
                        case "Uninstall_RedownloadCallback":
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
                            break;
                        default:
                            this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
            else
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "InstallPatchEx_callback":
                            this.myWebClient_ForAIDA.Credentials = null;
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.bWorker_install.RunWorkerAsync(state);
                            break;
                        case "Uninstall_RedownloadCallback":
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnPatchUninstalled(new PatchFinishedEventArgs(true, string.Empty));
                            break;
                        default:
                            this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
        }

        private void InstallPatchEx_DownloadProgress(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.OnCurrentProgressChanged(new ProgressEventArgs(e.ProgressPercentage));
        }
        
        protected virtual void OnInstalling(object sender, DoWorkEventArgs e)
        {
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
            WorkerInfo seed = e.Argument as WorkerInfo;
            PatchNotificationEventArgs seedEvent = seed.Params as PatchNotificationEventArgs;
            using (FileStream fs = File.OpenRead(seed.Path))
            using (var archive = SharpCompress.Archives.ArchiveFactory.Open(fs))
            {
                string pso2datafolder = DefaultValues.Directory.PSO2Win32Data;
                if (seed.Backup)
                {
                    string tmppath;
                    string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.English);
                    string backuppath;
                    this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Creating0PatchBackup", "Creating backup for {0} files"),Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled)));
                    int total = System.Linq.Enumerable.Count(archive.Entries);
                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(total));
                    int index = 0;
                    Directory.CreateDirectory(englishBackupFolder);
                    foreach (var entry in archive.Entries)
                        if (!entry.IsDirectory)
                        {

                            tmppath = Path.Combine(pso2datafolder, entry.Key);
                            backuppath = Path.Combine(englishBackupFolder, entry.Key);
                            File.Copy(tmppath, backuppath, true);
                            index++;
                            this.OnCurrentProgressChanged(new ProgressEventArgs(index + 1));
                        }
                }
                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Installing0Patch", "Installing {0}"), Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled)));
                var result = AbstractExtractor.Extract(archive, pso2datafolder, extract_callback);
                if (!result.IsSuccess)
                {
                    if (seed.Backup)
                    {
                        string tmppath;
                        string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.English);
                        var rollbackList = Directory.GetFiles(englishBackupFolder, "*", SearchOption.TopDirectoryOnly);
                        int total = rollbackList.Length;
                        this.OnCurrentTotalProgressChanged(new ProgressEventArgs(total));
                        int index = 0;
                        this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Rollback0Patch", "Rolling back the {0} installation"), Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled)));
                        foreach (string dundun in rollbackList)
                        {
                            tmppath = Path.Combine(pso2datafolder, Path.GetFileName(dundun));
                            File.Delete(tmppath);
                            File.Move(dundun, tmppath);
                            index++;
                            this.OnCurrentProgressChanged(new ProgressEventArgs(index + 1));
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

        private void extract_callback(object sender, AbstractExtractor.ExtractProgress e)
        {
            this.OnCurrentTotalProgressChanged(new ProgressEventArgs(e.Total));
            this.OnCurrentProgressChanged(new ProgressEventArgs(e.CurrentIndex + 1));
        }

        protected virtual void OnInstalled(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsBusy = false;
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));;
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
            //Classes.AIDA.WebPatches.PatchesInfos
            this.UninstallPatch(new Uri(Classes.AIDA.WebPatches.PatchesFileListInfos));
        }

        protected virtual void UninstallPatch(Uri address)
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                if (MySettings.MinimizeNetworkUsage)
                {
                    this.myWebClient_ForAIDA.CacheStorage = CacheStorage.DefaultStorage;
                    this.myWebClient_ForPSO2.CacheStorage = CacheStorage.DefaultStorage;
                }
                else
                {
                    this.myWebClient_ForAIDA.CacheStorage = null;
                    this.myWebClient_ForPSO2.CacheStorage = null;
                }
                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("BeginRestoring0PatchFiles", "Getting {0} filelist"), Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled)));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                this.myWebClient_ForAIDA.Credentials = null;
                this.myWebClient_ForAIDA.DownloadStringAsync(address, new WorkerInfo("UninstallPatchEx"));
            }
        }

        private void MyWebClient_ForAIDA_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
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
                if (state != null)
                {
                    this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
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
                }
                else if (meta != null)
                {
                    switch (meta.Step)
                    {
                        case 0:
                            if (!string.IsNullOrEmpty(e.Result))
                            {
                                try
                                {
                                    if (Classes.AIDA.FlatJsonFetch<string>(e.Result, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.ENPatchOverride).BoolAIDASettings(false))
                                    {
                                        this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                                        string newverstring = Classes.AIDA.FlatJsonFetch<string>(e.Result, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.ENPatchOverrideURL);
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
                                    else
                                    {
                                        if (MySettings.MinimizeNetworkUsage)
                                        {
                                            this.myWebClient_ForAIDA.CacheStorage = CacheStorage.DefaultStorage;
                                            this.myWebClient_ForPSO2.CacheStorage = CacheStorage.DefaultStorage;
                                        }
                                        else
                                        {
                                            this.myWebClient_ForAIDA.CacheStorage = null;
                                            this.myWebClient_ForPSO2.CacheStorage = null;
                                        }
                                        //this.myWebClient_ForAIDA.Credentials = Infos.DefaultValues.Arghlex.Web.AccountArghlex;
                                        var asdasdasdasdasd = new Uri(ACF.EnglishPatchManualHome);
                                        this.myWebClient_ForAIDA.AutoUserAgent = false;
                                        this.myWebClient_ForAIDA.UserAgent = "";
                                        this.myWebClient_ForAIDA.DownloadStringAsync(asdasdasdasdasd, new WebClientInstallingMetaWrapper(2, meta.Meta));
                                    }
                                }
                                catch (UriFormatException uriEx) { this.OnHandledException(new HandledExceptionEventArgs(uriEx)); }
                            }
                            else
                                this.OnHandledException(new HandledExceptionEventArgs(new Exception("Failed to check for patch.\r\n")));
                            break;
                        case 2:
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.myWebClient_ForAIDA.AutoUserAgent = true;
                            if (!string.IsNullOrWhiteSpace(e.Result))
                            {
                                string newverstring = GetNewestENPatch(e.Result);
                                if (!string.IsNullOrEmpty(newverstring))
                                {
                                    System.Uri url = new Uri(newverstring);
                                    newverstring = ACF.GetVersionFromURL(newverstring);
                                    InstallingMeta asd = new InstallingMeta(meta.Meta.Backup, meta.Meta.Force, newverstring);
                                    if (VersionString != newverstring)
                                    {
                                        this.myWebClient_ForAIDA.Credentials = Infos.DefaultValues.Arghlex.Web.AccountArghlex;
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
                            else
                                this.OnHandledException(new HandledExceptionEventArgs(new Exception("Failed to check for patch.\r\n")));
                            break;
                        case 1:
                            // Discarded Code
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            return;
                            if (!string.IsNullOrWhiteSpace(e.Result))
                            {
                                string newverstring = GetNewestENPatch(e.Result);
                                if (!string.IsNullOrEmpty(newverstring))
                                {
                                    System.Uri url = new System.Uri(Leayal.UriHelper.URLConcat(Infos.DefaultValues.Arghlex.Web.PatchesFolder, newverstring));
                                    newverstring = Path.GetFileNameWithoutExtension(newverstring);
                                    InstallingMeta asd = new InstallingMeta(meta.Meta.Backup, meta.Meta.Force, newverstring);
                                    if (VersionString != newverstring)
                                    {
                                        this.myWebClient_ForAIDA.Credentials = Infos.DefaultValues.Arghlex.Web.AccountArghlex;
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
                            else
                                this.OnHandledException(new HandledExceptionEventArgs(new Exception("Failed to check for patch.\r\n")));
                            break;
                    }
                }
            }
        }

        public static string GetNewestENPatch(string jobjstring)
        {
            //return GetNewestENPatch(JObject.Parse(jobjstring));
            return ACF.ParseTheWeb(jobjstring);
        }

        public static string GetNewestENPatch(JObject jobj)
        {
            JToken jt = jobj.SelectToken("files");
            if (jt != null)
            {
                IEnumerable<JToken> tokenlist = jt.Children();
                if (tokenlist != null)
                {
                    tokenlist = tokenlist.Where((token) => {
                        return (token["name"].Value<object>().ToString().IndexOf("largefiles") == -1);
                    });
                    if (tokenlist != null)
                    {
                        tokenlist = tokenlist.OrderByDescending((token) => {
                            return token["modtime"].Value<int>();
                        });
                        if (tokenlist != null)
                        {
                            jt = tokenlist.First();
                            if (jt != null)
                                return jt["name"].Value<object>().ToString();
                        }
                    }
                }
            }
            return null;
        }

        protected void OnUninstalling(object sender, DoWorkEventArgs e)
        {
            WorkerInfo wi = e.Argument as WorkerInfo;
            this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Restoring0PatchFiles", "Restoring {0} files"), Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled)));
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
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
            if (backup_files.Count > 0)
            {
                if (tbl_files.Length > backup_files.Count)
                {
                    int total = tbl_files.Length;
                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(tbl_files.Length));
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
                        using (ExtendedWebClient downloader = new ExtendedWebClient())
                        {
                            downloader.UserAgent = PSO2.DefaultValues.Web.UserAgent;
                            Dictionary<string, string> downloadlist = new Dictionary<string, string>();
                            for (int i = 0; i < nonExist.Count; i++)
                            {
                                currentStringIndex = nonExist[i];
                                downloadlist.Add("data/win32/" + currentStringIndex + DefaultValues.Web.FakeFileExtension, Path.Combine(pso2datafolder, currentStringIndex));
                            }
                            // PSO2UpdateManager.RedownloadFiles(downloader, downloadlist, Downloader_StepProgressChanged, Downloader_DownloadFileProgressChanged, this.Uninstall_RedownloadCallback);
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

        protected void Downloader_StepProgressChanged(object sender, StringEventArgs e)
        {
            this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("RedownloadingMissingOriginalFiles_0", "Redownloading file {0}"), Path.GetFileName(e.Value))));
        }

        private bool Downloader_DownloadFileProgressChanged(int arg0, int arg1)
        {
            this.OnCurrentProgressChanged(new ProgressEventArgs(arg0));
            return true;
        }

        protected void OnUninstalled(object sender, RunWorkerCompletedEventArgs e)
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

        #region "Reinstall Patch"
        public override void ReinstallPatch()
        {
            this.CheckUpdate(new Uri(Classes.AIDA.WebPatches.PatchesInfos), true);
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
            string englishBackupFolder = Path.Combine(pso2datafolder, DefaultValues.Directory.PSO2Win32DataBackup, DefaultValues.Directory.Backup.English);
            if (Directory.Exists(englishBackupFolder))
            {
                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Restoring0PatchFiles", "Restoring {0} files"), Infos.DefaultValues.AIDA.Strings.EnglishPatchCalled)));
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
        }
        #endregion
    }
}
