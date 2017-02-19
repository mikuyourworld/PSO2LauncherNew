using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.Net;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class PSO2UpdateManager
    {

        public enum UpdateResult : short
        {
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

        public PSO2UpdateManager()
        {
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
                    bytes = this.myWebClient.DownloadData(item.Value.PatchListURL);
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

        protected virtual Dictionary<string, PSO2File> ParseFilelist(MemoryFileCollection filelist)
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
                                    result.Add(pso2filebuffer.WindowFilename, pso2filebuffer);
                                }
                        }
                    this.ProgressCurrent = 1 + i;
                }
                if (needwrite)
                    PSO2UrlDatabase.Save();
            }
            return result;
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (GetFilesList())
            {
                string pso2Path = e.Argument as string;
                Dictionary<string, PSO2File> myPSO2filesList = ParseFilelist(this.myFileList);
                List<string> failedList = new List<string>();
                int downloadedfilecount = 0;
                int filecount = 0;
                this.ProgressTotal = myPSO2filesList.Count;
                string currentfilepath, filemd5;
                foreach (var _keypair in myPSO2filesList)
                {
                    currentfilepath = Classes.Infos.CommonMethods.PathConcat(pso2Path, _keypair.Key);
                    filemd5 = Infos.CommonMethods.FileToMD5Hash(currentfilepath);
                    if (!string.IsNullOrEmpty(filemd5))
                    {
                        if (_keypair.Value.MD5Hash != filemd5)
                        {
                            this.CurrentStep = string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingFile", "Downloading file {0}"), _keypair.Value.SafeFilename);
                            if (this.myWebClient.DownloadFile(_keypair.Value.Url, currentfilepath))
                                downloadedfilecount++;
                            else
                                failedList.Add(_keypair.Key);
                        }
                    }
                    else
                    {
                        this.CurrentStep = string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingFile", "Downloading file {0}"), _keypair.Value.SafeFilename);
                        if (this.myWebClient.DownloadFile(_keypair.Value.Url, currentfilepath))
                            downloadedfilecount++;
                        else
                            failedList.Add(_keypair.Key);
                    }
                    filecount++;
                    this.ProgressCurrent = filecount;
                }
                if (myPSO2filesList.Count == downloadedfilecount)
                    e.Result = new PSO2UpdateResult(UpdateResult.Success);
                else
                {
                    if ((myPSO2filesList.Count - downloadedfilecount) < 3)
                        e.Result = new PSO2UpdateResult(UpdateResult.MissingSomeFiles);
                    else
                        e.Result = new PSO2UpdateResult(UpdateResult.Failed);
                }
            }
            else
            {
                
                e.Result = new PSO2UpdateResult(UpdateResult.Unknown);
                throw new PSO2UpdateException(LanguageManager.GetMessageText("PSO2UpdateManager_GetPatchListFailed", "Failed to get PSO2's file list."));
            }
        }

        public bool RedownloadFiles(Dictionary<string, string> fileList, EventHandler<StringEventArgs> stepReport, Func<long, long, bool> downloadprogressReport, RunWorkerCompletedEventHandler downloadFinished_CallBack)
        {
            return RedownloadFiles(this.myWebClient, fileList, stepReport, downloadprogressReport, downloadFinished_CallBack);
        }

        /// <summary>
        /// Redownload files with given relative filenames.
        /// </summary>
        /// <param name="stepReport">This method will be invoked everytime the download proceed to tell the filename. This is thread-safe invoke.</param>
        /// <param name="downloadprogressReport">This method will be invoked everytime the download proceed. This is thread-safe invoke.</param>
        /// <param name="downloadFinished_CallBack">This method will be invoked when the download is finished. This is thread-safe invoke.</param>
        /// <returns>Bool. True if the download is succeeded, otherwise false.</returns>
        public static bool RedownloadFiles(ExtendedWebClient _webClient, Dictionary<string,string> fileList, EventHandler<StringEventArgs> stepReport, Func<long, long, bool> downloadprogressReport, RunWorkerCompletedEventHandler downloadFinished_CallBack)
        {
            bool continueDownload = true;
            Exception Myex = null;
            int filecount = 0;
            Uri currenturl;
            PSO2UrlDatabase.PSO2FileUrl _pso22fileurl;
            List<string> failedfiles = new List<string>();
            try
            {
                HttpStatusCode lastCode;
                byte[] buffer = new byte[1024];
                long byteprocessed, filelength;
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
                        byteprocessed = 0;
                        filelength = 0;
                        try
                        {
                            using (HttpWebResponse theRep = _webClient.Open(currenturl) as HttpWebResponse)
                            {
                                if (theRep.StatusCode == HttpStatusCode.NotFound)
                                    throw new WebException("File not found", null, WebExceptionStatus.ReceiveFailure, theRep);
                                else if (theRep.StatusCode == HttpStatusCode.Forbidden)
                                    throw new WebException("Access denied", null, WebExceptionStatus.ReceiveFailure, theRep);
                                if (theRep.ContentLength > 0)
                                    filelength = theRep.ContentLength;
                                else
                                    WebClientPool.SynchronizationContext.Send(new System.Threading.SendOrPostCallback(delegate { System.Windows.Forms.MessageBox.Show($"{_keypair.Key} has the length {filelength}", "Filelength"); }), null);
                                using (var theRepStream = theRep.GetResponseStream())
                                {
                                    int count = theRepStream.Read(buffer, 0, buffer.Length);
                                    while (count > 0)
                                    {
                                        local.Write(buffer, 0, count);
                                        byteprocessed += count;
                                        if (downloadprogressReport != null && filelength > 0)
                                            WebClientPool.SynchronizationContext.Send(new System.Threading.SendOrPostCallback(delegate { continueDownload = downloadprogressReport.Invoke(byteprocessed, filelength); }), null);
                                        count = theRepStream.Read(buffer, 0, buffer.Length);
                                    }
                                }
                            }
                            currenturl = null;
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
                                    if (theRep.ContentLength > 0)
                                        filelength = theRep.ContentLength;
                                    else
                                        WebClientPool.SynchronizationContext.Send(new System.Threading.SendOrPostCallback(delegate { System.Windows.Forms.MessageBox.Show($"{_keypair.Key} has the length {filelength}", "Filelength"); }), null);
                                    using (var theRepStream = theRep.GetResponseStream())
                                    {
                                        int count = theRepStream.Read(buffer, 0, buffer.Length);
                                        while (count > 0)
                                        {
                                            local.Write(buffer, 0, count);
                                            byteprocessed += count;
                                            if (downloadprogressReport != null && filelength > 0)
                                                WebClientPool.SynchronizationContext.Send(new System.Threading.SendOrPostCallback(delegate { continueDownload = downloadprogressReport.Invoke(byteprocessed, filelength); }), null);
                                            count = theRepStream.Read(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }
                            catch (WebException webEx)
                            {
                                if (webEx.Response != null)
                                {
                                    HttpWebResponse rep = webEx.Response as HttpWebResponse;
                                    if (rep.StatusCode != HttpStatusCode.NotFound)
                                    {
                                        failedfiles.Add(_keypair.Key);
                                        currenturl = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            failedfiles.Add(_keypair.Key);
                            currenturl = null;
                        }
                        if (currenturl != null)
                        { PSO2UrlDatabase.Update(_keypair.Key, currenturl); }
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
            var myevent = new RunWorkerCompletedEventArgs(failedfiles, Myex, !continueDownload);
            if (downloadFinished_CallBack != null)
                WebClientPool.SynchronizationContext.Post(new System.Threading.SendOrPostCallback(delegate { downloadFinished_CallBack.Invoke(_webClient, myevent); }), null);

            if (myevent.Error != null && !myevent.Cancelled)
                if (failedfiles.Count == 0)
                    return true;
            return false;
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
        public delegate void HandledExceptionEventHandler(object sender, Infos.HandledExceptionEventArgs e);
        public event HandledExceptionEventHandler HandledException;
        protected void OnHandledException(System.Exception ex)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.HandledException?.Invoke(this, new Infos.HandledExceptionEventArgs(ex)); }), null);
        }
        public delegate void PSO2NotifyEventHandler(object sender, PSO2NotifyEventArgs e);
        public event PSO2NotifyEventHandler PSO2Installed;
        protected void OnPSO2Installed(PSO2NotifyEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.PSO2Installed?.Invoke(this, e); }), null);
        }

        public event EventHandler<StepEventArgs> CurrentStepChanged;
        protected void OnCurrentStepChanged(StepEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentStepChanged?.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressEventArgs> CurrentProgressChanged;
        protected void OnCurrentProgressChanged(ProgressEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentProgressChanged?.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressEventArgs> CurrentTotalProgressChanged;
        protected void OnCurrentTotalProgressChanged(ProgressEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentTotalProgressChanged?.Invoke(this, e); }), null);
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

        public class ProgressEventArgs : EventArgs
        {
            public int Progress { get; }
            public ProgressEventArgs(int _progress) : base()
            {
                this.Progress = _progress;
            }
        }
        public class StepEventArgs : EventArgs
        {
            public string Step { get; }
            public StepEventArgs(string _step) : base()
            {
                this.Step = _step;
            }
        }
        public class PSO2NotifyEventArgs : EventArgs
        {
            public PSO2NotifyEventArgs() : base()
            {

            }
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
                switch (code)
                {
                    case UpdateResult.Failed:
                        this.StatusMessage = LanguageManager.GetMessageText("PSO2UpdateResult_MissingSomeFiles", "Failed to download game updates");
                        break;
                    case UpdateResult.Success:
                        this.StatusMessage = LanguageManager.GetMessageText("PSO2UpdateResult_MissingSomeFiles", "The game has been updated successfully");
                        break;
                    case UpdateResult.MissingSomeFiles:
                        this.StatusMessage = string.Format(LanguageManager.GetMessageText("PSO2UpdateResult_MissingSomeFiles", "The game has been updated but {0} files were not downloaded"), missingfilecount);
                        break;
                    default:
                        this.StatusMessage = LanguageManager.GetMessageText("PSO2UpdateResult_UnknownError", "Unknown error while updating game");
                        break;
                }
                this.UserToken = _userToken;
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
        #endregion
    }
}
