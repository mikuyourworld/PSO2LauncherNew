﻿using System;
using PSO2ProxyLauncherNew.Classes.PSO2;
using System.IO;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Events;
using System.Net;

namespace PSO2ProxyLauncherNew.Classes.Components.Patches
{
    public enum RaiserLanguageName : byte
    {
        Auto = 0,
        English = 1,
        German,
        Spanish,
        France,
        Russian,
        AllPatch
    }

    public enum RaiserLanguageCode : byte
    {
        Auto = 0,
        EN = 1,
        DE,
        ES,
        FR,
        RU,
        AllPatch
    }

    class RaiserOrWateverPatchManager : PatchManager 
    {
        public new string VersionString { get { return MySettings.Patches.RaiserVersion; } private set { MySettings.Patches.RaiserVersion = value; } }
        public static readonly string[] RequiredPluginList = { "PSO2RAISERSystem.dll" };
        public override bool IsInstalled
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VersionString))
                    return false;
                else if (Leayal.StringHelper.IsEqual(VersionString, Infos.DefaultValues.AIDA.Tweaker.Registries.NonePatchString, true))
                    return false;
                else if (Leayal.IO.DirectoryHelper.IsFolderEmpty(DefaultValues.Directory.RaiserPatchFolder))
                    return false;
                else
                    return true;
            }
        }

        public RaiserOrWateverPatchManager() : base()
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
        public void InstallPatch(RaiserLanguageName lang)
        {
            //this.CheckUpdate(new Uri(AIDA.WebPatches.PatchesInfos), true);
            this.CheckUpdate(new Uri(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.RaiserURL), lang, true);
        }
        public override void InstallPatch()
        {
            this.CheckUpdate(new Uri(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.RaiserURL), true);
        }
        public override void CheckUpdate()
        {
            this.CheckUpdate(new Uri(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.RaiserURL), false);
        }

        public void CheckUpdate(RaiserLanguageName lang)
        {
            //this.CheckUpdate(new Uri(AIDA.WebPatches.PatchesInfos), false);
            this.CheckUpdate(new Uri(Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.RaiserURL), lang, false);
        }

        protected virtual void CheckUpdate(Uri url, bool force)
        {
            this.CheckUpdate(url, RaiserLanguageName.Auto, force);
        }
        protected virtual void CheckUpdate(Uri url, RaiserLanguageName langName, bool force)
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                if (MySettings.MinimizeNetworkUsage)
                    this.myWebClient_ForAIDA.CacheStorage = CacheStorage.DefaultStorage;
                else
                    this.myWebClient_ForAIDA.CacheStorage = null;

                if (langName == RaiserLanguageName.Auto || langName == RaiserLanguageName.AllPatch)
                    langName = MySettings.Patches.PatchLanguage;

                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Checking0PatchUpdate", "Checking for {0} updates"), Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled)));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                this.myWebClient_ForAIDA.DownloadStringAsync(url, new RaiserWebClientInstallingMetaWrapper(1, new InstallingMeta(true, force), langName));
            }
        }

        protected virtual void InstallPatchEx(RaiserPatchNotificationEventArgs myEventArgs, System.Uri url)
        {
            try
            {
                if (myEventArgs == null)
                {
                    string str = this.VersionString;
                    myEventArgs = new RaiserPatchNotificationEventArgs(true, str, str, RaiserLanguageName.English);
                }
                string filePath = Path.Combine(Infos.DefaultValues.MyInfo.Directory.Patches, Path.GetFileName(url.OriginalString));
                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Downloading0Patch", "Downloading new {0} version"), Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled)));
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                this.OnCurrentTotalProgressChanged(new ProgressEventArgs(100));
                this.myWebClient_ForAIDA.DownloadFileAsync(url, filePath, new WorkerInfo("InstallPatchEx_callback", myEventArgs, filePath, url, false));
            }
            catch (Exception ex) { this.OnHandledException(new HandledExceptionEventArgs(ex)); }
        }

        private void MyWebClient_ForAIDA_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            WorkerInfo state = e.UserState as WorkerInfo;
            if (e.Error != null)
            {
                if (state != null)
                    switch (state.Step)
                    {
                        case "InstallPatchEx_callback":
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as RaiserPatchNotificationEventArgs).NewPatchVersion));
                            WebException webEx = e.Error as WebException;
                            if (webEx != null && webEx.Response != null)
                            {
                                HttpWebResponse httpwebresponseasd = webEx.Response as HttpWebResponse;
                                if (httpwebresponseasd != null && httpwebresponseasd.StatusCode == HttpStatusCode.NotFound)
                                    this.OnHandledException(new HandledExceptionEventArgs(new Exception("The patch you're looking for is not existed. Contact Arks-Layer's staffs for more info.", e.Error)));
                                else
                                    this.OnHandledException(new HandledExceptionEventArgs(e.Error));
                            }
                            else
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
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnPatchInstalled(new PatchFinishedEventArgs(false, (state.Params as RaiserPatchNotificationEventArgs).NewPatchVersion));
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
            RaiserPatchNotificationEventArgs seedEvent = seed.Params as RaiserPatchNotificationEventArgs;
            string patchdestination = DefaultValues.Directory.RaiserPatchFolder;
            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(patchdestination);
            using (FileStream fs = File.OpenRead(seed.Path))
            using (var archive = SharpCompress.Archives.ArchiveFactory.Open(fs))
            {
                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("Installing0Patch", "Installing {0}"), Infos.DefaultValues.AIDA.Strings.RaiserPatchCalled)));
                bool guuuh = false;
                var result = AbstractExtractor.FlatExtract(archive, patchdestination, (insender, ine) =>
                {
                    if (!guuuh)
                    {
                        guuuh = true;
                        this.OnCurrentTotalProgressChanged(new ProgressEventArgs(ine.Total));
                    }
                    this.OnCurrentProgressChanged(new ProgressEventArgs(ine.CurrentIndex + 1));
                });
                if (!result.IsSuccess)
                {
                    System.IO.Directory.Delete(patchdestination, true);
                    /*System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    bool glaihwg = true;
                    foreach (var item in result.FailedItems)
                        if (glaihwg)
                        {
                            glaihwg = false;
                            sb.Append(item);
                        }
                        else
                            sb.Append(", " + item);
                    System.Windows.Forms.MessageBox.Show(sb.ToString());//*/
                    throw new Exception("Extract failed.");
                }
                PSO2.PSO2Plugin.PSO2Plugin plugin;
                foreach (string str in RequiredPluginList)
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        plugin = PSO2.PSO2Plugin.PSO2PluginManager.Instance[str];
                        if (plugin != null)
                        {
                            if (!plugin.Enabled)
                            {
                                this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("InstallingPatch_EnableRequiredPlugin0", "Enabling required plugin: {0}"), plugin.Name)));
                                plugin.Enabled = true;
                            }
                        }
                        else
                            this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("InstallingPatch_FailedEnableRequiredPlugin0", "Failed to find required plugin: {0}.\nPlease enable it as soon as possible. Otherwise the patch may not work correctly."), str)));
                    }
            }
            try
            { File.Delete(seed.Path); }
            catch { }
            e.Result = seedEvent.NewPatchVersion;
            MySettings.Patches.PatchLanguage = seedEvent.Language;
            MySettings.Patches.RaiserVersion = seedEvent.NewPatchVersion;
            MySettings.Patches.RaiserEnabled = true;
        }

        protected virtual void OnInstalled(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsBusy = false;
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None)); ;
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
            if (!this.IsBusy)
            {
                this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
                this.bWorker_uninstall.RunWorkerAsync();
            }
        }

        private void MyWebClient_ForAIDA_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            WorkerInfo state = e.UserState as WorkerInfo;
            RaiserWebClientInstallingMetaWrapper meta = null;
            if (state == null)
                meta = e.UserState as RaiserWebClientInstallingMetaWrapper;
            if (e.Error != null)
            {
                if (state != null)
                    switch (state.Step)
                    {
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
                        default:
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
            }
            else
            {
                if (state != null)
                    switch (state.Step)
                    {
                        default:
                            this.OnHandledException(new HandledExceptionEventArgs(new NotImplementedException()));
                            break;
                    }
                else if (meta != null)
                {
                    switch (meta.Step)
                    {
                        case 0:
                            if (!string.IsNullOrEmpty(e.Result))
                                try
                                {
                                    string raiserURL = AIDA.FlatJsonFetch<string>(e.Result, Infos.DefaultValues.AIDA.Tweaker.TransArmThingiesOrWatever.RaiserURL);
                                    if (!string.IsNullOrWhiteSpace(raiserURL))
                                        this.myWebClient_ForAIDA.DownloadStringAsync(new System.Uri(raiserURL), new RaiserWebClientInstallingMetaWrapper(1, meta.Meta, meta.LanguageName));
                                    else
                                        this.OnPatchInstalled(new PatchFinishedEventArgs(VersionString));
                                }
                                catch (UriFormatException uriEx) { this.OnHandledException(new HandledExceptionEventArgs(uriEx)); }
                            else
                                this.OnHandledException(new HandledExceptionEventArgs(new Exception("Failed to check for patch.\r\n")));
                            break;
                        case 1:
                            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                            if (!string.IsNullOrEmpty(e.Result))
                                try
                                {
                                    var langcode = GetLangCode(meta.LanguageName);
                                    var obj = GetValueFromJson(e.Result, langcode.ToString());
                                    string newverurl = obj.URL,
                                        newvermd5 = obj.MD5;
                                    if (!string.IsNullOrWhiteSpace(newverurl))
                                    {
                                        System.Uri url = new System.Uri(newverurl);
                                        InstallingMeta asd = new InstallingMeta(meta.Meta.Backup, meta.Meta.Force, newvermd5);
                                        if (this.VersionString != newvermd5 || !this.IsInstalled)
                                        {
                                            RaiserPatchNotificationEventArgs theevent = new RaiserPatchNotificationEventArgs(true, newvermd5, VersionString, meta.LanguageName);
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
                                catch (UriFormatException uriEx) { this.OnHandledException(new HandledExceptionEventArgs(uriEx)); }
                            else
                                this.OnHandledException(new HandledExceptionEventArgs(new Exception("Failed to check for patch.\r\n")));
                            break;
                    }
                }
            }
        }

        public static RaiserInfo GetValueFromJson(string jsonContent)
        {
            return GetValueFromJson(jsonContent, "en");
        }

        public static RaiserInfo GetValueFromJson(string jsonContent, RaiserLanguageCode languageCode)
        {
            return GetValueFromJson(jsonContent, languageCode.ToString());
        }

        public static RaiserInfo GetValueFromJson(string jsonContent, string language)
        {
            if (string.IsNullOrEmpty(language))
                language = "en";
            using (StringReader sr = new StringReader(jsonContent))
                return GetValueFromJson(sr, language);
        }

        public static RaiserInfo GetValueFromJson(TextReader jsonStream, string language)
        {
            RaiserInfo result = null;
            using (Newtonsoft.Json.JsonTextReader jtr = new Newtonsoft.Json.JsonTextReader(jsonStream))
            {
                bool? enable = true;
                int? count = 0;
                string md5 = string.Empty, url = string.Empty, tmp;
                while (jtr.Read())
                    if (jtr.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                    {
                        if (((string)jtr.Value).ToLower() == language.ToLower())
                        {
                            while (jtr.Read())
                                if (jtr.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                                {
                                    tmp = ((string)jtr.Value).ToLower();
                                    if (tmp == "enabled")
                                        enable = jtr.ReadAsBoolean();
                                    else if (tmp == "filecount")
                                        count = jtr.ReadAsInt32();
                                    else if (tmp == "patchmd5")
                                        md5 = jtr.ReadAsString();
                                    else if (tmp == "patchurl")
                                        url = jtr.ReadAsString();
                                    else
                                        jtr.Skip();
                                }
                        }
                        else
                            jtr.Skip();
                    }
                if (enable.HasValue && !enable.Value)
                    result = new RaiserInfo(false, count.HasValue ? count.Value : 0, md5, url);
                else
                    result = new RaiserInfo(true, count.HasValue ? count.Value : 0, md5, url);
            }
            return result;
        }

        public class RaiserInfo
        {
            internal RaiserInfo(bool _enable, int _count, string _md5, string _url)
            {
                this.Enable = _enable;
                this.FileCount = _count;
                this.MD5 = _md5;
                this.URL = _url;
            }

            public bool Enable { get; }
            public int FileCount { get; }
            public string MD5 { get; }
            public string URL { get; }

        }

        protected void OnUninstalling(object sender, DoWorkEventArgs e)
        {
            PSO2.PSO2Plugin.PSO2Plugin plugin;
            foreach (string str in RequiredPluginList)
                if (!string.IsNullOrWhiteSpace(str))
                {
                    plugin = PSO2.PSO2Plugin.PSO2PluginManager.Instance[str];
                    if (plugin != null)
                    {
                        if (plugin.Enabled)
                        {
                            this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("InstallingPatch_DisableRequiredPlugin0", "Disabling required plugin: {0}"), plugin.Name)));
                            plugin.Enabled = false;
                        }
                    }
                    else
                        this.OnCurrentStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("InstallingPatch_FailedFindRequiredPlugin0", "Failed to find required plugin: {0}"), str)));
                }
            string folder = DefaultValues.Directory.RaiserPatchFolder;
            if (Directory.Exists(DefaultValues.Directory.RaiserPatchFolder))
                Directory.Delete(folder, true);
            MySettings.Patches.RaiserVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NonePatchString;
            MySettings.Patches.RaiserEnabled = false;
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
                this.OnPatchUninstalled(new PatchFinishedEventArgs(false, string.Empty));
            else
                this.OnPatchUninstalled(new PatchFinishedEventArgs(true, string.Empty));
        }
        #endregion

        #region "Reinstall Patch"
        public override void ReinstallPatch()
        {
            throw new NotImplementedException();
        }

        public void ReinstallPatch(RaiserLanguageName langName)
        {
            this.CheckUpdate(new Uri(Classes.AIDA.WebPatches.PatchesInfos), langName, true);
        }
        #endregion

        #region "Cancel Support"
        public override void CancelAsync()
        {
            if (this.bWorker_install.IsBusy)
                this.bWorker_install.CancelAsync();
            if (this.bWorker_uninstall.IsBusy)
                this.bWorker_uninstall.CancelAsync();
            if (this.myWebClient_ForAIDA.IsBusy)
                this.myWebClient_ForAIDA.CancelAsync();
        }
        #endregion

        #region "Private classes"
        private class RaiserWebClientInstallingMetaWrapper : WebClientInstallingMetaWrapper
        {
            public RaiserLanguageName LanguageName { get; }
            public RaiserWebClientInstallingMetaWrapper(short sStep, InstallingMeta oMeta, RaiserLanguageName langName) : base(sStep, oMeta)
            { this.LanguageName = langName; }
        }

        public static RaiserLanguageCode GetLangCode(RaiserLanguageName langName)
        {
            return (RaiserLanguageCode)((int)langName);
        }

        public class RaiserPatchNotificationEventArgs : PatchNotificationEventArgs
        {
            public RaiserLanguageName Language { get; }
            public RaiserPatchNotificationEventArgs(bool isnew, string newpatchstring, string currentPatch, bool createBackup, RaiserLanguageName langName) : base(isnew, newpatchstring, currentPatch, createBackup)
            {
                this.Language = langName;
            }

            public RaiserPatchNotificationEventArgs(bool isnew, string newpatchstring, string currentPatch, RaiserLanguageName langName) : this(isnew, newpatchstring, currentPatch, true, langName) { }

            public RaiserPatchNotificationEventArgs(string currentPatch, RaiserLanguageName langName) : this(false, string.Empty, currentPatch, false, langName) { }
        }
        #endregion
    }
}
