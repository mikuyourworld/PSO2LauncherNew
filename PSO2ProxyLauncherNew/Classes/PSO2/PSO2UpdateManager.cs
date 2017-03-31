using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.Net;
using PSO2ProxyLauncherNew.Classes.Events;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class PSO2UpdateManager
    {

        public enum UpdateResult : short
        {
            Cancelled = -2,
            Failed = -1,
            Unknown = 0,
            Success = 1,
            MissingSomeFiles = 2
        }

        private static char[] _spaceonly = { ' ' };
        private static char[] _tabonly = { Microsoft.VisualBasic.ControlChars.Tab };
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

        public Infos.VersionCheckResult CheckForUpdates()
        {
            Infos.VersionCheckResult result;
            try
            {
                string latestver = this.myWebClient.DownloadString(DefaultValues.PatchInfo.VersionLink);
                if (string.IsNullOrWhiteSpace(latestver))
                    throw new NullReferenceException("Latest version is null. Something bad happened.");
                else
                {
                    this._LastKnownLatestVersion = latestver;
                    result = new Infos.VersionCheckResult(latestver, MySettings.PSO2Version);
                }
            }
            catch (Exception ex)
            {
                result = new Infos.VersionCheckResult(ex);
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
            this.UpdateGame(new WorkerParams(_pso2path, latestver));
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
            this.UpdateGame(new WorkerParams(path, latestver, true));
        }

        protected virtual bool GetFilesList()
        {
            if (!this.IsBusy)
            {
                this.myFileList.Clear();
                int i = 0;
                this.ProgressTotal = DefaultValues.PatchInfo.PatchListFiles.Count;
                byte[] bytes;
                foreach (var item in DefaultValues.PatchInfo.PatchListFiles)
                {
                    i++;
                    this.ProgressCurrent = i;
                    this.CurrentStep = string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingPatchList", "Downloading {0} list"), item.Key);
                    if (MySettings.MinimizeNetworkUsage)
                        this.myWebClient.CacheStorage = Components.CacheStorage.DefaultStorage;
                    else
                        this.myWebClient.CacheStorage = null;
                    bytes = this.myWebClient.DownloadData(item.Value.PatchListURL);
                    this.myWebClient.CacheStorage = null;
                    if (bytes != null && bytes.Length > 0)
                    {
                        this.myFileList.Add(item.Key, bytes);
                    }
                }
                if (this.myFileList.Count == DefaultValues.PatchInfo.PatchListFiles.Count)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        protected virtual System.Collections.Concurrent.ConcurrentDictionary<string, PSO2File> ParseFilelist(MemoryFileCollection filelist)
        {
            Dictionary<string, PSO2File> result = new Dictionary<string, PSO2File>();
            if (filelist != null && filelist.Count > 0)
            {
                bool needwrite = false;
                string linebuffer;
                PSO2File pso2filebuffer;
                this.ProgressTotal = filelist.Count;
                KeyValuePair<string, MemoryStream> _keypair;
                string currentBaseUrl;
                this.CurrentStep = LanguageManager.GetMessageText("PSO2UpdateManager_BuildingFileList", "Building file list");
                for (int i = 0; i < filelist.Count; i++)
                {
                    _keypair = filelist[i];
                    currentBaseUrl = DefaultValues.PatchInfo.PatchListFiles[_keypair.Key].BaseURL;
                    using (StreamReader sr = new StreamReader(_keypair.Value))
                        while (!sr.EndOfStream)
                        {
                            linebuffer = string.Empty;
                            linebuffer = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(linebuffer))
                                if (PSO2File.TryParse(linebuffer, currentBaseUrl, out pso2filebuffer))
                                {
                                    if (!needwrite)
                                    { needwrite = PSO2UrlDatabase.Update(pso2filebuffer.OriginalFilename, pso2filebuffer.Url); }
                                    else
                                    { PSO2UrlDatabase.Update(pso2filebuffer.OriginalFilename, pso2filebuffer.Url); }
                                    if (!result.ContainsKey(pso2filebuffer.WindowFilename))
                                        result.Add(pso2filebuffer.WindowFilename, pso2filebuffer);
                                    else
                                    {
                                        if (_keypair.Key == DefaultValues.PatchInfo.file_patch)
                                            result[pso2filebuffer.WindowFilename] = pso2filebuffer;
                                    }
                                }
                        }
                    this.ProgressCurrent = 1 + i;
                }
                if (needwrite)
                    PSO2UrlDatabase.Save();
            }
            return new System.Collections.Concurrent.ConcurrentDictionary<string, PSO2File>(result);
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
            if (GetFilesList())
            {
                WorkerParams wp = e.Argument as WorkerParams;
                string pso2Path = wp.PSO2Path;
                System.Collections.Concurrent.ConcurrentDictionary<string, PSO2File> myPSO2filesList = ParseFilelist(this.myFileList);
                if (!myPSO2filesList.IsEmpty)
                {
                    this.ProgressTotal = myPSO2filesList.Count;
                    this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent, new Forms.MyMainMenuCode.CircleProgressBarProperties(true)));
                    string verstring = wp.NewVersionString;
                    if (string.IsNullOrWhiteSpace(verstring))
                        verstring = this.myWebClient.DownloadString(DefaultValues.PatchInfo.VersionLink);
                    if (!string.IsNullOrWhiteSpace(verstring))
                        verstring = verstring.Trim();
                    anothersmallthreadpool = new AnotherSmallThreadPool(pso2Path, myPSO2filesList);
                    anothersmallthreadpool.StepChanged += Anothersmallthreadpool_StepChanged;
                    anothersmallthreadpool.ProgressChanged += Anothersmallthreadpool_ProgressChanged;
                    anothersmallthreadpool.KaboomFinished += Anothersmallthreadpool_KaboomFinished;
                    anothersmallthreadpool.StartWork(new WorkerParams(pso2Path, verstring, wp.Installation));
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
                Log.LogManager.GeneralLog.Print(PSO2UpdateResult.GetMsg(e.Result, e.FailedList == null ? 0 : e.FailedList.Count), Log.Logger.LogLevel.Error);
                switch (e.Result)
                {
                    case UpdateResult.Cancelled:
                        if (e.UserToken != null && e.UserToken is WorkerParams)
                        {
                            WorkerParams wp = e.UserToken as WorkerParams;
                            if (wp.Installation)
                                this.OnPSO2Installed(new PSO2NotifyEventArgs(true, wp.PSO2Path, e.FailedList));
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
                                AIDA.PSO2Dir = wp.PSO2Path;
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
                                AIDA.PSO2Dir = wp.PSO2Path;
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
                            Log.LogManager.GeneralLog.Print(updateresult.StatusMessage, Log.Logger.LogLevel.Error);
                            break;
                        case UpdateResult.MissingSomeFiles:
                            Log.LogManager.GeneralLog.Print(updateresult.StatusMessage, Log.Logger.LogLevel.Error);
                            break;
                        default:
                            Log.LogManager.GeneralLog.Print(updateresult.StatusMessage);
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
            bool continueDownload = true;
            Exception Myex = null;
            Uri currenturl;
            PSO2UrlDatabase.PSO2FileUrl _pso22fileurl;
            DownloadProgressChangedEventHandler ooooo = null;
            if (progress_callback != null)
                ooooo = new DownloadProgressChangedEventHandler(delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    if (progress_callback.Invoke(e.ProgressPercentage))
                    {
                        continueDownload = false;
                        _webClient.CancelAsync();
                    }
                });
            if (ooooo != null)
                _webClient.DownloadProgressChanged += ooooo;
            try
            {
                HttpStatusCode lastCode;
                _pso22fileurl = new PSO2UrlDatabase.PSO2FileUrl(Infos.CommonMethods.URLConcat(DefaultValues.Web.MainDownloadLink, relativeFilename), Infos.CommonMethods.URLConcat(DefaultValues.Web.OldDownloadLink, relativeFilename));
                currenturl = PSO2UrlDatabase.Fetch(relativeFilename);
                if (currenturl == null)
                    currenturl = _pso22fileurl.MainUrl;
                lastCode = HttpStatusCode.ServiceUnavailable;
                try
                {
                    _webClient.DownloadFile(currenturl, destinationFullfilename);
                    PSO2UrlDatabase.Update(relativeFilename, currenturl);
                }
                catch (WebException webEx)
                {
                    if (webEx.Response != null)
                    {
                        HttpWebResponse rep = webEx.Response as HttpWebResponse;
                        lastCode = rep.StatusCode;
                    }
                    else
                        throw webEx;
                }
                if (lastCode == HttpStatusCode.NotFound)
                {
                    currenturl = _pso22fileurl.GetTheOtherOne(currenturl.OriginalString);
                    try
                    {
                        _webClient.DownloadFile(currenturl, destinationFullfilename);
                        PSO2UrlDatabase.Update(relativeFilename, currenturl);
                    }
                    catch (WebException webEx)
                    {
                        if (webEx.Response != null)
                        {
                            HttpWebResponse rep = webEx.Response as HttpWebResponse;
                            if (rep.StatusCode != HttpStatusCode.NotFound)
                                throw webEx;
                        }
                        else
                            throw webEx;
                    }
                }
            }
            catch (Exception ex) { Myex = ex; }
            if (ooooo != null)
                _webClient.DownloadProgressChanged -= ooooo;
            PSO2UrlDatabase.Save();
            return new RunWorkerCompletedEventArgs(null, Myex, !continueDownload);
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
            bool continueDownload = true;
            Exception Myex = null;
            int filecount = 0;
            Uri currenturl;
            var asdasdads = _webClient.CacheStorage;
            _webClient.CacheStorage = null;
            PSO2UrlDatabase.PSO2FileUrl _pso22fileurl;
            List<string> failedfiles = new List<string>();
            try
            {
                HttpStatusCode lastCode;
                byte[] buffer = new byte[1024];
                //long byteprocessed, filelength;
                foreach (var _keypair in fileList)
                {
                    if (stepReport != null)
                        WebClientPool.SynchronizationContext.Send(new System.Threading.SendOrPostCallback(delegate { stepReport.Invoke(_webClient, new StringEventArgs(_keypair.Key)); }), null);
                    using (FileStream local = File.Create(_keypair.Value, 1024))
                    {
                        _pso22fileurl = new PSO2UrlDatabase.PSO2FileUrl(Infos.CommonMethods.URLConcat(DefaultValues.Web.MainDownloadLink, _keypair.Key), Infos.CommonMethods.URLConcat(DefaultValues.Web.OldDownloadLink, _keypair.Key));
                        currenturl = PSO2UrlDatabase.Fetch(_keypair.Key);
                        if (currenturl == null)
                            currenturl = _pso22fileurl.MainUrl;
                        lastCode = HttpStatusCode.ServiceUnavailable;
                        //byteprocessed = 0;
                        //filelength = 0;
                        try
                        {
                            using (HttpWebResponse theRep = _webClient.Open(currenturl) as HttpWebResponse)
                            {
                                if (theRep.StatusCode == HttpStatusCode.NotFound)
                                    throw new WebException("File not found", null, WebExceptionStatus.ReceiveFailure, theRep);
                                else if (theRep.StatusCode == HttpStatusCode.Forbidden)
                                    throw new WebException("Access denied", null, WebExceptionStatus.ReceiveFailure, theRep);
                                /*if (theRep.ContentLength > 0)
                                    filelength = theRep.ContentLength;
                                else
                                {
                                    HttpWebRequest headReq = _webClient.CreateRequest(currenturl, "HEAD") as HttpWebRequest;
                                    headReq.AutomaticDecompression = DecompressionMethods.None;
                                    HttpWebResponse headRep = headReq.GetResponse() as HttpWebResponse;
                                    if (headRep != null)
                                    {
                                        if (headRep.ContentLength > 0)
                                            filelength = headRep.ContentLength;
                                        headRep.Close();
                                    }
                                }*/
                                using (var theRepStream = theRep.GetResponseStream())
                                {
                                    int count = theRepStream.Read(buffer, 0, buffer.Length);
                                    while (count > 0)
                                    {
                                        local.Write(buffer, 0, count);
                                        //byteprocessed += count;
                                        count = theRepStream.Read(buffer, 0, buffer.Length);
                                    }
                                }
                            }
                            PSO2UrlDatabase.Update(_keypair.Key, currenturl);
                        }
                        catch (WebException webEx)
                        {
                            if (webEx.Response != null)
                            {
                                HttpWebResponse rep = webEx.Response as HttpWebResponse;
                                lastCode = rep.StatusCode;
                            }
                        }
                        if (lastCode == HttpStatusCode.NotFound)
                        {
                            currenturl = _pso22fileurl.GetTheOtherOne(currenturl.OriginalString);
                            try
                            {
                                using (HttpWebResponse theRep = _webClient.Open(currenturl) as HttpWebResponse)
                                {
                                    if (theRep.StatusCode == HttpStatusCode.NotFound)
                                        throw new WebException("File not found", null, WebExceptionStatus.ReceiveFailure, theRep);
                                    else if (theRep.StatusCode == HttpStatusCode.Forbidden)
                                        throw new WebException("Access denied", null, WebExceptionStatus.ReceiveFailure, theRep);
                                    /*if (theRep.ContentLength > 0)
                                        filelength = theRep.ContentLength;
                                    else
                                    {
                                        HttpWebRequest headReq = _webClient.CreateRequest(currenturl, "HEAD") as HttpWebRequest;
                                        headReq.AutomaticDecompression = DecompressionMethods.None;
                                        HttpWebResponse headRep = headReq.GetResponse() as HttpWebResponse;
                                        if (headRep != null)
                                        {
                                            if (headRep.ContentLength > 0)
                                                filelength = headRep.ContentLength;
                                            headRep.Close();
                                        }
                                    }*/
                                    using (var theRepStream = theRep.GetResponseStream())
                                    {
                                        int count = theRepStream.Read(buffer, 0, buffer.Length);
                                        while (count > 0)
                                        {
                                            local.Write(buffer, 0, count);
                                            //byteprocessed += count;
                                            count = theRepStream.Read(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                                PSO2UrlDatabase.Update(_keypair.Key, currenturl);
                            }
                            catch (WebException webEx)
                            {
                                if (webEx.Response != null)
                                {
                                    HttpWebResponse rep = webEx.Response as HttpWebResponse;
                                    if (rep.StatusCode != HttpStatusCode.NotFound)
                                        failedfiles.Add(_keypair.Key);
                                }
                            }
                        }
                        else
                            failedfiles.Add(_keypair.Key);
                    }
                    //fileList[filecount].IndexOfAny(' ');
                    if (downloadprogressReport != null)
                        WebClientPool.SynchronizationContext.Send(new System.Threading.SendOrPostCallback(delegate { continueDownload = downloadprogressReport.Invoke(filecount, fileList.Count); }), null);
                    filecount++;
                }
            }
            catch (Exception ex)
            { Myex = ex; }
            PSO2UrlDatabase.Save();
            _webClient.CacheStorage = asdasdads;
            var myevent = new RunWorkerCompletedEventArgs(failedfiles, Myex, !continueDownload);
            if (downloadFinished_CallBack != null)
                WebClientPool.SynchronizationContext.Post(new System.Threading.SendOrPostCallback(delegate { downloadFinished_CallBack.Invoke(_webClient, myevent); }), null);

            if (myevent.Error != null && !myevent.Cancelled)
                if (failedfiles.Count == 0)
                    return true;
            return false;
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
        internal class InstallationMeta
        {
            public string GamePath { get; }
            public string Step { get; }
            public InstallationMeta(string step) : this(step, string.Empty) { }
            public InstallationMeta(string step, string path)
            {
                this.Step = step;
                this.GamePath = path;
            }
        }
        internal class PSO2File
        {
            public static bool TryParse(string rawdatastring, string baseUrl, out PSO2File _pso2file)
            {
                string[] splitbuffer = null;
                if (rawdatastring.IndexOf(Microsoft.VisualBasic.ControlChars.Tab) > -1)
                { splitbuffer = rawdatastring.Split(_tabonly, 3, StringSplitOptions.RemoveEmptyEntries); }
                else if (rawdatastring.IndexOf(" ") > -1)
                { splitbuffer = rawdatastring.Split(_spaceonly, 3, StringSplitOptions.RemoveEmptyEntries); }
                if (splitbuffer != null && splitbuffer.Length == 3)
                {
                    _pso2file = new PSO2File(splitbuffer[0], splitbuffer[1], splitbuffer[2], baseUrl);
                    return true;
                }
                else
                {
                    _pso2file = null;
                    return false;
                }
            }
            public string SafeFilename { get; }
            public string WindowFilename { get; }
            public string Filename { get; }
            public string OriginalFilename { get; }
            public Uri Url { get; }
            public long Length { get; }
            public string MD5Hash { get; }
            public PSO2File(string _filename, string _length, string _md5, string _baseurl)
            {
                this.Filename = trimPat(_filename);
                this.OriginalFilename = _filename;
                this.WindowFilename = switchToWindowsPath(this.Filename);
                this.SafeFilename = Path.GetFileName(this.Filename);
                long filelength;
                if (!long.TryParse(_length, out filelength))
                    filelength = -1;
                this.Length = filelength;
                this.MD5Hash = _md5.ToUpper();
                this.Url = new Uri(Classes.Infos.CommonMethods.URLConcat(_baseurl, _filename));
                    //new PSO2FileUrl(Classes.Infos.CommonMethods.URLConcat(DefaultValues.Web.MainDownloadLink, _filename), Classes.Infos.CommonMethods.URLConcat(DefaultValues.Web.OldDownloadLink, _filename));
            }

            private string switchToWindowsPath(string _path)
            {
                if (_path.IndexOf("//") > -1)
                    _path = _path.Replace("//", "/");
                if (_path.IndexOf(@"\\") > -1)
                    _path = _path.Replace(@"\\", @"\");
                if (_path.IndexOf("/") > -1)
                    _path = _path.Replace("/", "\\");
                return _path;
            }

            private string trimPat(string _path)
            {
                if (_path.EndsWith(DefaultValues.Web.FakeFileExtension))
                {
                    return _path.Substring(0, _path.Length - DefaultValues.Web.FakeFileExtension.Length);
                }
                else
                    return _path;
            }
        }
        public class PSO2NotifyEventArgs : EventArgs
        {
            public string NewClientVersion { get; }
            public bool Installation { get; }
            public System.Collections.ObjectModel.ReadOnlyCollection<string> FailedList { get; }
            public bool Cancelled { get; }
            public string InstalledLocation { get; }
            public PSO2NotifyEventArgs(string _ver, bool install, System.Collections.ObjectModel.ReadOnlyCollection<string> _failedlist) : base()
            {
                this.NewClientVersion = _ver;
                this.Installation = install;
                this.InstalledLocation = string.Empty;
                this.Cancelled = false;
                this.FailedList = _failedlist;
            }

            public PSO2NotifyEventArgs(string _ver, string _installedlocation) : this(_ver, _installedlocation, null) { }

            public PSO2NotifyEventArgs(string _ver, string _installedlocation, System.Collections.ObjectModel.ReadOnlyCollection<string> _failedlist) : base()
            {
                this.NewClientVersion = _ver;
                this.Installation = true;
                this.InstalledLocation = _installedlocation;
                this.Cancelled = false;
                this.FailedList = _failedlist;
            }

            public PSO2NotifyEventArgs(bool _cancel, string _installedlocation, System.Collections.ObjectModel.ReadOnlyCollection<string> _failedlist) : base()
            {
                this.NewClientVersion = string.Empty;
                this.Installation = true;
                this.InstalledLocation = _installedlocation;
                this.Cancelled = _cancel;
                this.FailedList = _failedlist;
            }

            public PSO2NotifyEventArgs(bool _cancel, bool install, System.Collections.ObjectModel.ReadOnlyCollection<string> _failedlist) : base()
            {
                this.NewClientVersion = string.Empty;
                this.Installation = install;
                this.InstalledLocation = string.Empty;
                this.Cancelled = _cancel;
                this.FailedList = _failedlist;
            }

            public PSO2NotifyEventArgs(string _ver, bool install) : this(_ver, install, null) {}
        }

        public class PSO2UpdateResult
        {
            public UpdateResult StatusCode { get; }
            public string StatusMessage { get; }
            public object UserToken { get; }

            public PSO2UpdateResult(UpdateResult code, int missingfilecount) : this(code, missingfilecount, null) { }
            public PSO2UpdateResult(UpdateResult code) : this(code, -1, null) { }

            public PSO2UpdateResult(UpdateResult code, int missingfilecount, object _userToken)
            {
                this.StatusMessage = GetMsg(code, missingfilecount);
                this.UserToken = _userToken;
            }

            public static string GetMsg(UpdateResult msgCode, int missingfilecount)
            {
                string result;
                switch (msgCode)
                {
                    case UpdateResult.Failed:
                        result = LanguageManager.GetMessageText("PSO2UpdateResult_MissingSomeFiles", "Failed to download game updates");
                        break;
                    case UpdateResult.Success:
                        result = LanguageManager.GetMessageText("PSO2UpdateResult_MissingSomeFiles", "The game has been updated successfully");
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

        public class StringEventArgs : EventArgs
        {
            public string UserToken { get; }
            public StringEventArgs(string token) : base()
            {
                this.UserToken = token;
            }
        }

        public class PSO2UpdateException : Exception
        {
            public PSO2UpdateException(string msg) : base(msg)
            { }
        }
        
        internal class WorkerParams
        {
            public string PSO2Path { get; }
            public string NewVersionString { get; }
            public bool Installation { get; set; }
            public WorkerParams(string _pso2path, string latestversionstring, bool install)
            {
                this.PSO2Path = _pso2path;
                this.NewVersionString = latestversionstring;
                this.Installation = install;
            }
            public WorkerParams(string _pso2path, string latestversionstring) : this(_pso2path, latestversionstring, false) { }
            public WorkerParams(string _pso2path) : this(_pso2path, string.Empty) { }
            public WorkerParams(string _pso2path, bool install) : this(_pso2path, string.Empty, install) { }
        }
        #endregion

        #region "Cancel Operation"
        public void CancelAsync()
        {
            myWebClient.CancelAsync();
            if (anothersmallthreadpool != null)
                anothersmallthreadpool.CancelWork();
        }
        #endregion
    }
}
