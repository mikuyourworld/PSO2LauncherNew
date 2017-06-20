using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.Net;
using PSO2ProxyLauncherNew.Classes.Events;
using Leayal.IO;
using Leayal.Log;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PrepatchManager
{
    class PrepatchManager : IDisposable
    {
        public const string PrepatchFolderName = "_precede";
        public const string PrepatchFilelistFilename = "patchlist{0}.txt";

        private System.Threading.SynchronizationContext syncContext;
        private CustomWebClient myWebClient;
        private BackgroundWorker bWorker;
        private MemoryFileCollection myFileList;
        private bool _isbusy;

        private PrepatchSmallThreadPool anothersmallthreadpool;
        private string _LastKnownLatestVersion;
        public string LastKnownLatestVersion { get { return this._LastKnownLatestVersion; } }

        public PrepatchManager()
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

        public PrepatchVersionCheckResult CheckForUpdates()
        {
            PrepatchVersionCheckResult result;
            try
            {
                string latestver = this.myWebClient.DownloadString(DefaultValues.PatchInfo.PrecedeVersionLink);
                if (string.IsNullOrWhiteSpace(latestver))
                    throw new NullReferenceException("Latest version is null. Something bad happened.");
                else
                {
                    this._LastKnownLatestVersion = latestver;
                    if (PrepatchVersion.TryParse(latestver, out var latestversion))
                        result = new PrepatchVersionCheckResult(latestversion, MySettings.PSO2PrecedeVersion);
                    else
                        result = new PrepatchVersionCheckResult(latestver, MySettings.PSO2PrecedeVersion);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Response != null)
                {
                    HttpWebResponse webRep = webEx.Response as HttpWebResponse;
                    if (webRep != null && webRep.StatusCode == HttpStatusCode.NotFound)
                        result = new PrepatchVersionCheckResult(new NoPrepatchExistedException());
                    else
                        result = new PrepatchVersionCheckResult(webEx);
                }
                else
                    result = new PrepatchVersionCheckResult(webEx);
            }
            catch (Exception ex)
            {
                result = new PrepatchVersionCheckResult(ex);
            }
            return result;
        }

        public void UpdatePrepatch()
        {
            this.UpdatePrepatch(MySettings.PSO2Dir);
        }

        public void UpdatePrepatch(string _pso2path)
        {
            this.UpdatePrepatch(new PrepatchWorkerParams(_pso2path));
        }

        public void UpdatePrepatch(string _pso2path, PrepatchVersion latestver)
        {
            this.UpdatePrepatch(new PrepatchWorkerParams(_pso2path, latestver));
        }

        private void UpdatePrepatch(PrepatchWorkerParams wp)
        {
            this.bWorker.RunWorkerAsync(wp);
        }

        protected virtual int GetFilesList(int prelistCount)
        {
            if (!this.IsBusy)
            {
                int filecount = 0;
                this.myFileList.Clear();
                this.ProgressTotal = prelistCount;
                RecyclableMemoryStream memStream;
                if (MySettings.MinimizeNetworkUsage)
                    this.myWebClient.CacheStorage = Components.CacheStorage.DefaultStorage;
                else
                    this.myWebClient.CacheStorage = null;
                string thefilename = null;
                for (int i = 0; i < prelistCount; i++)
                {
                    try
                    {
                        memStream = null;
                        thefilename = i.ToString();
                        this.ProgressCurrent = i + 1;
                        this.CurrentStep = string.Format(LanguageManager.GetMessageText("PrepatchManager_DownloadingPatchList", "Downloading {0} list"), thefilename);
                        memStream = this.myWebClient.DownloadToMemory(DefaultValues.PatchInfo.GetPrecedeDownloadLink(string.Format(PrepatchFilelistFilename, thefilename)), thefilename);
                        if (memStream != null && memStream.Length > 0)
                        {
                            filecount++;
                            this.myFileList.Add(thefilename, memStream);
                        }
                    }
                    catch (WebException webEx)
                    {
                        if (webEx.Response != null)
                        {
                            HttpWebResponse webrep = webEx.Response as HttpWebResponse;
                            if (webrep != null && webrep.StatusCode == HttpStatusCode.NotFound)
                                break;
                        }
                    }
                }
                return filecount;
            }
            else
                return -2;
        }

        protected virtual System.Collections.Concurrent.ConcurrentDictionary<string, PSO2File> ParseFilelist(MemoryFileCollection filelist)
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
                    // Can't use Using block because in .NET 4.0, StreamReader will also close underlying stream.
                    // We need the RecyclableMemoryStream in tact because we will use it once more later.
                    // According to RecyclableMemoryStream documents, once the RecyclableMemoryStream is closed, all underlying buffer will be returned to the pool
                    //which is why we can't close the stream.
                    // For .NET 4.5 or higher, there is param "keep Open" in the StreamReader constructor which is safe for using block.
                    StreamReader sr = new StreamReader(_pair.Value);
                    while (!sr.EndOfStream)
                    {
                        linebuffer = sr.ReadLine();
                        if (!string.IsNullOrWhiteSpace(linebuffer))
                            if (PSO2File.TryParse(linebuffer, DefaultValues.Web.PrecedeDownloadLink, out pso2filebuffer))
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
            PrepatchWorkerParams wp = e.Argument as PrepatchWorkerParams;
            // Let's get to see how many patchlist{0}.txt there are
            var remoteresult = CheckForUpdates();
            if (remoteresult.IsPrepatchExisted)
            {
                if (wp.Force || remoteresult.IsNewVersion)
                {
                    if (GetFilesList(remoteresult.Latest.ListCount) > 0)
                    {
                        string pso2Path = Path.Combine(wp.PSO2Path, "_precede");
                        System.Collections.Concurrent.ConcurrentDictionary<string, PSO2File> myPSO2filesList = ParseFilelist(this.myFileList);
                        if (!myPSO2filesList.IsEmpty)
                        {
                            this.ProgressTotal = myPSO2filesList.Count;
                            this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent, new Forms.MyMainMenuCode.CircleProgressBarProperties(true)));
                            PrepatchVersion verstring = wp.NewVersion;
                            if (string.IsNullOrWhiteSpace(verstring.Version))
                                verstring = remoteresult.Latest;
                            /*if (string.IsNullOrWhiteSpace(verstring))
                                verstring = this.myWebClient.DownloadString(DefaultValues.PatchInfo.VersionLink);//*/
                            anothersmallthreadpool = new PrepatchSmallThreadPool(pso2Path, myPSO2filesList);
                            anothersmallthreadpool.StepChanged += Anothersmallthreadpool_StepChanged;
                            anothersmallthreadpool.ProgressChanged += Anothersmallthreadpool_ProgressChanged;
                            anothersmallthreadpool.KaboomFinished += Anothersmallthreadpool_KaboomFinished;
                            anothersmallthreadpool.StartWork(new PrepatchWorkerParams(pso2Path, verstring, wp.Force));
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
            }
            else
                throw remoteresult.Error;
        }

        private void WritePatchListOut(string pso2path)
        {
            foreach (var memStream in this.myFileList.Values)
                using (FileStream fs = File.Create(pso2path))
                    memStream.WriteTo(fs);
        }

        private void Anothersmallthreadpool_KaboomFinished(object sender, KaboomFinishedEventArgs e)
        {
            this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            if (e.Error != null)
            {
                this.OnHandledException(e.Error);
                if (this.myFileList.Count > 0)
                    this.myFileList.Clear();
                anothersmallthreadpool.Dispose();
            }
            else
            {
                LogManager.GeneralLog.Print(PSO2UpdateResult.GetMsg(e.Result, e.FailedList == null ? 0 : e.FailedList.Count), LogLevel.Info);
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate
                {
                    switch (e.Result)
                    {
                        case UpdateResult.Cancelled:
                            if (e.UserToken != null && e.UserToken is PrepatchWorkerParams)
                            {
                                PrepatchWorkerParams wp = e.UserToken as PrepatchWorkerParams;
                                this.OnPrepatchDownloaded(new PSO2NotifyEventArgs(true, false, e.FailedList));
                            }
                            break;
                        case UpdateResult.Failed:
                            if (e.UserToken != null && e.UserToken is PrepatchWorkerParams)
                            {
                                PrepatchWorkerParams wp = e.UserToken as PrepatchWorkerParams;
                                this.OnPrepatchDownloaded(new PSO2NotifyEventArgs(wp.NewVersion.ToString(" revision "), false, e.FailedList));
                            }
                            break;
                        default:
                            if (e.UserToken != null && e.UserToken is PrepatchWorkerParams)
                            {
                                PrepatchWorkerParams wp = e.UserToken as PrepatchWorkerParams;
                                this.WritePatchListOut(wp.PSO2Path);
                                MySettings.PSO2PrecedeVersion = wp.NewVersion;
                                this.OnPrepatchDownloaded(new PSO2NotifyEventArgs(wp.NewVersion.ToString(" revision "), false));
                            }
                            break;
                    } 
                    if (this.myFileList.Count > 0)
                        this.myFileList.Clear();
                    anothersmallthreadpool.Dispose();
                }));
            }
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
                this.OnHandledException(e.Error);
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
                if (this._totalprogress != value)
                {
                    this._totalprogress = value;
                    this.OnCurrentTotalProgressChanged(new ProgressEventArgs(value));
                }
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
                if (this._currentprogress != value)
                {
                    this._currentprogress = value;
                    this.OnCurrentProgressChanged(new ProgressEventArgs(value));
                }
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
        public event EventHandler<PSO2NotifyEventArgs> PrepatchDownloaded;
        protected void OnPrepatchDownloaded(PSO2NotifyEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PrepatchDownloaded?.Invoke(this, e); }), null);
        }
        public event EventHandler<PSO2NotifyEventArgs> PrepatchDownloadedCancelled;
        protected void OnPrepatchDownloadedCancelled(PSO2NotifyEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PrepatchDownloadedCancelled?.Invoke(this, e); }), null);
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
