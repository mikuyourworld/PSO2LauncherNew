using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.Net;
using PSO2ProxyLauncherNew.Classes.Events;
using Leayal.IO;
using Leayal.Log;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class PSO2UpdateManager : IDisposable
    {
        private static readonly char[] bangonly = { '=' };
        private System.Threading.SynchronizationContext syncContext;
        private CustomWebClient myWebClient;
        private BackgroundWorker bWorker;
        private MemoryFileCollection myFileList;
        private bool _isbusy;

        private AnotherSmallThreadPool anothersmallthreadpool;
        private string _LastKnownLatestVersion;
        public string LastKnownLatestVersion { get { return this._LastKnownLatestVersion; } }

        public PSO2UpdateManager()
        {
            this._LastKnownLatestVersion = string.Empty;
            this._isbusy = false;
            this.syncContext = System.Threading.SynchronizationContext.Current;
            this.myWebClient = WebClientPool.GetWebClient_PSO2Download(true);
            this.myFileList = new MemoryFileCollection();
            this.bWorker = new BackgroundWorker();
            this.bWorker.WorkerReportsProgress = true;
            this.bWorker.WorkerSupportsCancellation = true;
            this.bWorker.DoWork += BWorker_DoWork;
            this.bWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
        }

        /*public void CheckForUpdatesAsync()
        {

        }*/

        public Infos.PSO2VersionCheckResult CheckForUpdates()
        {
            Infos.PSO2VersionCheckResult result;
            try
            {
                string management = this.myWebClient.DownloadString(DefaultValues.PatchInfo.PatchManagement);
                if (MySettings.MinimizeNetworkUsage)
                    this.myWebClient.CacheStorage = Components.CacheStorage.DefaultStorage;
                else
                    this.myWebClient.CacheStorage = null;
                if (string.IsNullOrWhiteSpace(management))
                    throw new NullReferenceException("Latest version is null. Something bad happened.");
                else
                {
                    string currentline, master = null, patch = null;
                    string[] splitedline;
                    
                    using (StringReader sr = new StringReader(management))
                        while (sr.Peek() > -1)
                        {
                            currentline = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(currentline))
                            {
                                splitedline = currentline.Split(bangonly, 2, StringSplitOptions.RemoveEmptyEntries);
                                if (Leayal.StringHelper.IsEqual(splitedline[0], "MasterURL", true))
                                    master = splitedline[1];
                                else if (Leayal.StringHelper.IsEqual(splitedline[0], "PatchURL", true))
                                    patch = splitedline[1];
                                if (!string.IsNullOrWhiteSpace(master) && !string.IsNullOrWhiteSpace(patch))
                                    break;
                            }
                        }

                    if (string.IsNullOrWhiteSpace(master))
                        result = new Infos.PSO2VersionCheckResult(new ArgumentNullException("MasterURL is not found"));
                    else if (string.IsNullOrWhiteSpace(patch))
                        result = new Infos.PSO2VersionCheckResult(new ArgumentNullException("PatchURL is not found"));
                    else
                    {
                        string latestver = this.myWebClient.DownloadString(Leayal.UriHelper.URLConcat(patch, "version.ver"));

                        if (string.IsNullOrWhiteSpace(latestver))
                            result = new Infos.PSO2VersionCheckResult(new ArgumentNullException("Latest version file is not found"));
                        else
                        {
                            this._LastKnownLatestVersion = latestver;
                            result = new Infos.PSO2VersionCheckResult(latestver, MySettings.PSO2Version, master, patch);
                        }
                    }
                }
                this.myWebClient.CacheStorage = null;
            }
            catch (Exception ex)
            {
                result = new Infos.PSO2VersionCheckResult(ex);
            }
            return result;
        }

        public void UpdateGame()
        {
            this.UpdateGame(MySettings.PSO2Dir);
        }

        public void UpdateGame(string _pso2path)
        {
            this.UpdateGame(new WorkerParams(_pso2path));
        }

        public void UpdateGame(string _pso2path, string latestver)
        {
            this.UpdateGame(new WorkerParams(_pso2path, latestver, false, false));
        }

        private void UpdateGame(WorkerParams wp)
        {
            this.bWorker.RunWorkerAsync(wp);
        }

        public void InstallPSO2To(string path)
        {
            this.UpdateGame(new WorkerParams(path, true));
        }

        public void InstallPSO2To(string path, string latestver)
        {
            this.UpdateGame(new WorkerParams(path, latestver, true, true));
        }

        public void CheckLocalFiles(string _pso2path, string latestver)
        {
            this.UpdateGame(new WorkerParams(_pso2path, latestver, false, true));
        }

        public void CheckLocalFiles(string _pso2path)
        {
            this.UpdateGame(new WorkerParams(_pso2path, MySettings.PSO2Version, false, true));
        }

        public void CheckLocalFiles()
        {
            this.UpdateGame(new WorkerParams(MySettings.PSO2Dir, MySettings.PSO2Version, false, true));
        }
        

        protected virtual bool GetFilesList(Infos.PSO2VersionCheckResult patchinfo)
        {
            if (!this.IsBusy)
            {
                this.myFileList.Clear();
                int i = 0;
                // this.ProgressTotal = DefaultValues.PatchInfo.PatchListFiles.Count;
                this.ProgressTotal = 2;
                // patchurl
                RecyclableMemoryStream memStream;
                if (MySettings.MinimizeNetworkUsage)
                    this.myWebClient.CacheStorage = Components.CacheStorage.DefaultStorage;
                else
                    this.myWebClient.CacheStorage = null;
                
                this.ProgressCurrent = 1;
                this.CurrentStep = string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingPatchList", "Downloading {0} list"), DefaultValues.PatchInfo.called_masterlist);
                memStream = this.myWebClient.DownloadToMemory(Leayal.UriHelper.URLConcat(patchinfo.MasterURL, DefaultValues.PatchInfo.file_patch), DefaultValues.PatchInfo.called_masterlist);
                if (memStream != null && memStream.Length > 0)
                    this.myFileList.Add(DefaultValues.PatchInfo.called_masterlist, memStream);

                this.ProgressCurrent = 2;
                this.CurrentStep = string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingPatchList", "Downloading {0} list"), DefaultValues.PatchInfo.called_patchlist);
                memStream = this.myWebClient.DownloadToMemory(Leayal.UriHelper.URLConcat(patchinfo.PatchURL, DefaultValues.PatchInfo.file_patch), DefaultValues.PatchInfo.called_patchlist);
                if (memStream != null && memStream.Length > 0)
                    this.myFileList.Add(DefaultValues.PatchInfo.called_patchlist, memStream);
                
                this.myWebClient.CacheStorage = null;
                if (this.myFileList.Count == 2)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        protected virtual System.Collections.Concurrent.ConcurrentDictionary<string, PSO2File> ParseFilelist(MemoryFileCollection filelist, Infos.PSO2VersionCheckResult patchinfo)
        {
            Dictionary<string, PSO2File> result = new Dictionary<string, PSO2File>();
            if (filelist != null && filelist.Count > 0)
            {
                string linebuffer;
                PSO2File pso2filebuffer;
                this.ProgressTotal = filelist.Count;
                this.CurrentStep = LanguageManager.GetMessageText("PSO2UpdateManager_BuildingFileList", "Building file list");
                int i = 0;
                foreach (var _pair in filelist.GetEnumerator())
                {
                    i++;
                    using (StreamReader sr = new StreamReader(_pair.Value))
                        while (!sr.EndOfStream)
                        {
                            linebuffer = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(linebuffer))
                                if (PSO2File.TryParse(linebuffer, patchinfo, out pso2filebuffer))
                                {
                                    if (!result.ContainsKey(pso2filebuffer.WindowFilename))
                                        result.Add(pso2filebuffer.WindowFilename, pso2filebuffer);
                                    else
                                    {
                                        if (_pair.Key == DefaultValues.PatchInfo.file_patch)
                                            result[pso2filebuffer.WindowFilename] = pso2filebuffer;
                                    }
                                }
                        }
                    this.ProgressCurrent = i;
                }
            }
            return new System.Collections.Concurrent.ConcurrentDictionary<string, PSO2File>(result);
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));

            WorkerParams wp = e.Argument as WorkerParams;
            string pso2Path = wp.PSO2Path;

            // Check if there is any prepatch files
            if (!wp.IgnorePrepatch)
            {
                string prepatchFolderData = Path.Combine(pso2Path, PrepatchManager.PrepatchManager.PrepatchFolderName, "data");
                if (!DirectoryHelper.IsFolderEmpty(prepatchFolderData))
                {
                    // Ignore prepatch files if it's older than the current client version
                    PSO2Version currentVersion = PSO2Version.Parse(MySettings.PSO2Version);
                    PSO2Version prepatchVersion = PSO2Version.Parse(MySettings.PSO2PrecedeVersion.Version);
                    if (prepatchVersion.CompareTo(currentVersion) > 0)
                    {
                        this.CurrentStep = LanguageManager.GetMessageText("PSO2Updater_FoundValidPrepatch", "Found prepatch files which are ready to be used.");
                        ValidPrepatchPromptEventArgs myEventArgs = new ValidPrepatchPromptEventArgs();
                        this.OnValidPrepatchPrompt(myEventArgs);
                        if (myEventArgs.Use)
                        {
                            string[] filenames = Directory.GetFiles(prepatchFolderData, "*", SearchOption.AllDirectories);
                            this.CurrentStep = LanguageManager.GetMessageText("PSO2Updater_MovingPrepatchFiles", "Applying prepatch files.");
                            this.ProgressTotal = filenames.Length;
                            this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
                            string str = null, maindatafolder = Path.Combine(pso2Path, "data"), targetfile = null;
                            for (int i = 0; i < filenames.Length; i++)
                            {
                                str = filenames[i];
                                targetfile = maindatafolder + str.Remove(0, prepatchFolderData.Length);
                                File.Delete(targetfile);
                                File.Move(str, targetfile);
                                this.ProgressCurrent = i + 1;
                            }

                            // Check if it's empty again to remove it
                            if (DirectoryHelper.IsFolderEmpty(prepatchFolderData))
                            {
                                string prepatchfolder = Path.Combine(pso2Path, PrepatchManager.PrepatchManager.PrepatchFolderName);
                                try
                                {
                                    Directory.Delete(prepatchfolder, true);
                                }
                                catch { }
                            }
                        }
                    }
                    else
                    {
                        this.CurrentStep = LanguageManager.GetMessageText("PSO2Updater_FoundInvalidPrepatch", "Found out-dated prepatch files which will be ignored. These files shouldn't be used and should be deleted.");
                        InvalidPrepatchPromptEventArgs myEventArgs = new InvalidPrepatchPromptEventArgs();
                        this.OnInvalidPrepatchPrompt(myEventArgs);
                        if (myEventArgs.Delete)
                        {
                            this.CurrentStep = LanguageManager.GetMessageText("PSO2Updater_DeletingInvalidPrepatch", "Deleting out-dated prepatch files.");
                            string prepatchfolder = Path.Combine(pso2Path, PrepatchManager.PrepatchManager.PrepatchFolderName);
                            try
                            {
                                Directory.Delete(prepatchfolder, true);
                            }
                            catch { }
                        }
                    }
                }
            }
            
            this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));

            var patchinfo = this.CheckForUpdates();

            if (this.GetFilesList(patchinfo))
            {
                System.Collections.Concurrent.ConcurrentDictionary<string, PSO2File> myPSO2filesList = ParseFilelist(this.myFileList, patchinfo);
                if (!myPSO2filesList.IsEmpty)
                {
                    this.ProgressTotal = myPSO2filesList.Count;
                    this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent, new Forms.MyMainMenuCode.CircleProgressBarProperties(true)));
                    string verstring = wp.NewVersionString;
                    if (string.IsNullOrWhiteSpace(verstring))
                        verstring = this.myWebClient.DownloadString(Leayal.UriHelper.URLConcat(patchinfo.PatchURL, "version.ver"));
                    if (!string.IsNullOrWhiteSpace(verstring))
                        verstring = verstring.Trim();
                    anothersmallthreadpool = new AnotherSmallThreadPool(pso2Path, myPSO2filesList);
                    anothersmallthreadpool.StepChanged += Anothersmallthreadpool_StepChanged;
                    anothersmallthreadpool.ProgressChanged += Anothersmallthreadpool_ProgressChanged;
                    anothersmallthreadpool.KaboomFinished += Anothersmallthreadpool_KaboomFinished;

                    this.CurrentStep = LanguageManager.GetMessageText("PSO2Updater_BeginFileCheckAndDownload", "Begin the file check and download...");

                    anothersmallthreadpool.StartWork(new WorkerParams(pso2Path, verstring, wp.Installation, wp.IgnorePrepatch));
                    e.Result = null;
                }
                else
                    e.Result = new PSO2UpdateResult(UpdateResult.Failed);
            }
            else
            {
                e.Result = new PSO2UpdateResult(UpdateResult.Unknown);
                throw new PSO2UpdateException(LanguageManager.GetMessageText("PSO2UpdateManager_GetPatchListFailed", "Failed to get PSO2's file list."));
            }
        }

        private void Anothersmallthreadpool_KaboomFinished(object sender, KaboomFinishedEventArgs e)
        {
            this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            if (e.Error != null)
                this.OnHandledException(e.Error);
            else
            {
                LogManager.GeneralLog.Print(PSO2UpdateResult.GetMsg(e.Result, e.FailedList == null ? 0 : e.FailedList.Count), LogLevel.Info);
                switch (e.Result)
                {
                    case UpdateResult.Cancelled:
                        if (e.UserToken != null && e.UserToken is WorkerParams)
                        {
                            WorkerParams wp = e.UserToken as WorkerParams;
                            if (wp.Installation)
                            {
                                this.OnPSO2Installed(new PSO2NotifyEventArgs(true, wp.PSO2Path, e.FailedList));
                            }
                            else
                                this.OnPSO2Installed(new PSO2NotifyEventArgs(true, false, e.FailedList));
                        }
                        break;
                    case UpdateResult.Failed:
                        if (e.UserToken != null && e.UserToken is WorkerParams)
                        {
                            WorkerParams wp = e.UserToken as WorkerParams;
                            if (wp.Installation)
                            {
                                MySettings.PSO2Dir = wp.PSO2Path;
                                this.OnPSO2Installed(new PSO2NotifyEventArgs(wp.NewVersionString, wp.PSO2Path, e.FailedList));
                            }
                            else
                                this.OnPSO2Installed(new PSO2NotifyEventArgs(wp.NewVersionString, false, e.FailedList));
                        }
                        break;
                    default:
                        if (e.UserToken != null && e.UserToken is WorkerParams)
                        {
                            WorkerParams wp = e.UserToken as WorkerParams;
                            if (!string.IsNullOrWhiteSpace(wp.NewVersionString))
                                MySettings.PSO2Version = wp.NewVersionString;
                            if (wp.Installation)
                            {
                                MySettings.PSO2Dir = wp.PSO2Path;
                                this.OnPSO2Installed(new PSO2NotifyEventArgs(wp.NewVersionString, wp.PSO2Path));
                            }
                            else
                                this.OnPSO2Installed(new PSO2NotifyEventArgs(wp.NewVersionString, false));
                        }
                        break;
                }
            }
            anothersmallthreadpool.Dispose();
        }

        private void Anothersmallthreadpool_ProgressChanged(object sender, DetailedProgressChangedEventArgs e)
        {
            this.ProgressCurrent = e.Current;
        }

        private void Anothersmallthreadpool_StepChanged(object sender, StepEventArgs e)
        {
            this.CurrentStep = e.Step;
        }

        private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.OnHandledException(e.Error);
                this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            }
            else if (e.Cancelled)
            { }
            else
            {
                if (e.Result is PSO2UpdateResult)
                {
                    PSO2UpdateResult updateresult = e.Result as PSO2UpdateResult;
                    switch (updateresult.StatusCode)
                    {
                        case UpdateResult.Failed:
                            LogManager.GeneralLog.Print(updateresult.StatusMessage, LogLevel.Error);
                            break;
                        case UpdateResult.MissingSomeFiles:
                            LogManager.GeneralLog.Print(updateresult.StatusMessage, LogLevel.Error);
                            break;
                        default:
                            LogManager.GeneralLog.Print(updateresult.StatusMessage);
                            break;
                    }
                    this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
                }
            }
        }

        public bool RedownloadFiles(Dictionary<string, string> fileList, EventHandler<StringEventArgs> stepReport, Func<int, int, bool> downloadprogressReport, RunWorkerCompletedEventHandler downloadFinished_CallBack)
        {
            return RedownloadFiles(this.myWebClient, fileList, stepReport, downloadprogressReport, downloadFinished_CallBack);
        }

        /// <summary>
        /// Redownload files with given relative filenames.
        /// </summary>
        /// <returns>RunWorkerCompletedEventArgs. True if the download is succeeded, otherwise false.</returns>
        public static RunWorkerCompletedEventArgs RedownloadFile(ExtendedWebClient _webClient, string relativeFilename, string destinationFullfilename, Func<int, bool> progress_callback)
        {
            throw new Infos.NotRecommendedException(LanguageManager.GetMessageText("RedownloadFiles_NotRecommended", "RedownloadFile Method is deprecated, please consider use check for old/missing files instead."));
        }

        /// <summary>
        /// Redownload files with given relative filenames.
        /// </summary>
        /// <param name="stepReport">This method will be invoked everytime the download proceed to tell the filename. This is thread-safe invoke.</param>
        /// <param name="downloadprogressReport">This method will be invoked everytime the download proceed. This is thread-safe invoke.</param>
        /// <param name="downloadFinished_CallBack">This method will be invoked when the download is finished. This is thread-safe invoke.</param>
        /// <returns>Bool. True if the download is succeeded, otherwise false.</returns>
        public static bool RedownloadFiles(ExtendedWebClient _webClient, Dictionary<string,string> fileList, EventHandler<StringEventArgs> stepReport, Func<int, int, bool> downloadprogressReport, RunWorkerCompletedEventHandler downloadFinished_CallBack)
        {
            throw new Infos.NotRecommendedException(LanguageManager.GetMessageText("RedownloadFiles_NotRecommended", "RedownloadFile Method is deprecated, please consider use check for old/missing files instead."));
        }

        #region "Properties"
        public bool IsInstalled
        {
            get
            {
                string path = MySettings.PSO2Dir;
                if (!string.IsNullOrWhiteSpace(path))
                    if (File.Exists(Path.Combine(path, "pso2.exe")))
                        if (Directory.Exists(Path.Combine(path, "data", "win32")))
                            return true;
                return false;
            }
        }

        public bool IsBusy
        {
            get
            {
                return (this._isbusy || myWebClient.IsBusy);
            }
        }

        private int _totalprogress;
        public int ProgressTotal
        {
            get
            {
                return _totalprogress;
            }
            internal set
            {
                this._totalprogress = value;
                this.OnCurrentTotalProgressChanged(new ProgressEventArgs(value));
            }
        }

        private int _currentprogress;
        public int ProgressCurrent
        {
            get
            {
                return _currentprogress;
            }
            internal set
            {
                this._currentprogress = value;
                this.OnCurrentProgressChanged(new ProgressEventArgs(value));
            }
        }

        private string _currentstep;
        public string CurrentStep
        {
            get
            {
                return _currentstep;
            }
            internal set
            {
                this._currentstep = value;
                this.OnCurrentStepChanged(new StepEventArgs(value));
            }
        }
        #endregion

        #region "Events"
        public event EventHandler<ValidPrepatchPromptEventArgs> ValidPrepatchPrompt;
        protected void OnValidPrepatchPrompt(ValidPrepatchPromptEventArgs e)
        {
            if (this.ValidPrepatchPrompt != null)
                this.syncContext?.Send(new System.Threading.SendOrPostCallback(delegate { this.ValidPrepatchPrompt.Invoke(this, e); }), null);
        }

        public event EventHandler<InvalidPrepatchPromptEventArgs> InvalidPrepatchPrompt;
        protected void OnInvalidPrepatchPrompt(InvalidPrepatchPromptEventArgs e)
        {
            if (this.InvalidPrepatchPrompt != null)
                this.syncContext?.Send(new System.Threading.SendOrPostCallback(delegate { this.InvalidPrepatchPrompt.Invoke(this, e); }), null);
        }

        public event EventHandler<ProgressBarStateChangedEventArgs> ProgressBarStateChanged;
        protected void OnProgressStateChanged(ProgressBarStateChangedEventArgs e)
        {
            if (this.ProgressBarStateChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressBarStateChanged.Invoke(this, e); }), null);
        }

        public event EventHandler<HandledExceptionEventArgs> HandledException;
        protected void OnHandledException(System.Exception ex)
        {
            if (this.HandledException != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.HandledException.Invoke(this, new HandledExceptionEventArgs(ex)); }), null);
        }
        public event EventHandler<PSO2NotifyEventArgs> PSO2Installed;
        protected void OnPSO2Installed(PSO2NotifyEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PSO2Installed?.Invoke(this, e); }), null);
        }
        public event EventHandler<PSO2NotifyEventArgs> PSO2InstallCancelled;
        protected void OnPSO2InstallCancelled(PSO2NotifyEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PSO2InstallCancelled?.Invoke(this, e); }), null);
        }
        public event EventHandler<StepEventArgs> CurrentStepChanged;
        protected void OnCurrentStepChanged(StepEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentStepChanged?.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressEventArgs> CurrentProgressChanged;
        protected void OnCurrentProgressChanged(ProgressEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentProgressChanged?.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressEventArgs> CurrentTotalProgressChanged;
        protected void OnCurrentTotalProgressChanged(ProgressEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentTotalProgressChanged?.Invoke(this, e); }), null);
        }
        #endregion

        #region "Internal Classes"
        public class PSO2UpdateResult
        {
            public UpdateResult StatusCode { get; }
            public string StatusMessage { get; }
            public object UserToken { get; }

            public PSO2UpdateResult(UpdateResult code, int missingfilecount) : this(code, missingfilecount, null) { }
            public PSO2UpdateResult(UpdateResult code) : this(code, -1, null) { }

            public PSO2UpdateResult(UpdateResult code, int missingfilecount, object _userToken)
            {
                this.StatusCode = code;
                this.StatusMessage = GetMsg(code, missingfilecount);
                this.UserToken = _userToken;
            }

            public static string GetMsg(UpdateResult msgCode, int missingfilecount)
            {
                string result;
                switch (msgCode)
                {
                    case UpdateResult.Failed:
                        result = LanguageManager.GetMessageText("PSO2UpdateResult_Failed", "Failed to download game updates");
                        break;
                    case UpdateResult.Success:
                        result = LanguageManager.GetMessageText("PSO2UpdateResult_Success", "The game has been updated successfully");
                        break;
                    case UpdateResult.MissingSomeFiles:
                        result = string.Format(LanguageManager.GetMessageText("PSO2UpdateResult_MissingSomeFiles", "The game has been updated but {0} files were not downloaded"), missingfilecount);
                        break;
                    default:
                        result = LanguageManager.GetMessageText("PSO2UpdateResult_UnknownError", "Unknown error while updating game");
                        break;
                }
                return result;
            }
        }
        #endregion

        #region "Cancel Operation"
        public void CancelAsync()
        {
            myWebClient.CancelAsync();
            if (anothersmallthreadpool != null)
                anothersmallthreadpool.CancelWork();
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            this._disposed = true;
            if (myWebClient != null)
                myWebClient.Dispose();
            if (bWorker != null)
                bWorker.Dispose();
            if (anothersmallthreadpool != null)
                anothersmallthreadpool.Dispose();
            if (myFileList != null)
                myFileList.Dispose();
        }
        #endregion
    }
}
