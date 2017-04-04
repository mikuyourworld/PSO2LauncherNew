using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using System.Net;
using PSO2ProxyLauncherNew.Classes.Events;
using Microsoft.VisualBasic;
using Microsoft.IO;
using Leayal.Log;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class PSO2PrecedeManager : IDisposable
    {
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

        public PSO2PrecedeManager()
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

        /*public PSO2PrecedeResult CheckForPrepatch(bool IsThroughtout)
        {
            PREPATCH_STATUS arr1 = PREPATCH_STATUS.UNKNOWN;
            LastResult(1) = 0;
            //Apply
            bool isApplied = false;

            PSO2PrecedeVersion remotePrecedeVer = GetPrecedeVersion();
            

            string BufferLine = null;
            string[] TheSplitting = null;
            string BufferVersion = null;
            string BufferListNumber = null;
            bool BufferPrecede = false;
            while ((prepatch_vanilaver == null))
            {
                try
                {
                    HttpWebRequest prepatch_vanilaver_request = HttpWebRequest.Create(new Uri("http://patch01.pso2gs.net/patch_prod/patches/management_beta.txt?" + String_RandomNumberWithMask(4000, 5000)));
                    //prepatch_vanilaver_request.Method = "HEAD"
                    prepatch_vanilaver_request.Timeout = 1500;
                    prepatch_vanilaver_request.UserAgent = "AQUA_HTTP";
                    prepatch_vanilaver_request.Proxy = null;
                    HttpWebResponse prepatch_vanilaver_response = prepatch_vanilaver_request.GetResponse();
                    if (((prepatch_vanilaver_response != null)))
                    {
                        if ((prepatch_vanilaver_response.StatusCode == HttpStatusCode.OK))
                        {
                            using (System.IO.StreamReader TheStreamReader = new System.IO.StreamReader(prepatch_vanilaver_response.GetResponseStream(), System.Text.Encoding.UTF8))
                            {
                                while ((TheStreamReader.Peek() > 0))
                                {
                                    BufferLine = TheStreamReader.ReadLine();
                                    if ((BufferLine.IndexOf("=") > -1))
                                    {
                                        TheSplitting = BufferLine.Split(new char[] { '=' });
                                        if ((TheSplitting(0).ToLower() == "isleaveprecede"))
                                        {
                                            BufferPrecede = TheSplitting(1) == "1" ? true : false;
                                        }
                                        else if ((TheSplitting(0).ToLower() == "precedeversion"))
                                        {
                                            prepatch_vanilaver = TheSplitting(1);
                                        }
                                        else if ((TheSplitting(0).ToLower() == "precedecurrent"))
                                        {
                                            BufferListNumber = TheSplitting(1);
                                        }
                                    }
                                }
                            }
                            if ((BufferPrecede == true))
                            {
                                if ((string.IsNullOrWhiteSpace(BufferVersion) == false) & (string.IsNullOrWhiteSpace(BufferListNumber) == false))
                                {
                                    arr1 = PREPATCH_STATUS.DOWNLOADING;
                                }
                                else
                                {
                                    arr1 = PREPATCH_STATUS.APPLIED;
                                    return LastResult;
                                }
                            }
                            else
                            {
                                arr1 = PREPATCH_STATUS.NONE;
                                return LastResult;
                            }
                        }
                        else if ((prepatch_vanilaver_response.StatusCode == HttpStatusCode.NotFound))
                        {
                            arr1 = PREPATCH_STATUS.NONE;
                            return LastResult;
                        }
                    }
                }
                catch (WebException ex)
                {
                    if ((ex.Response == null))
                    {
                        Create_log(Error_Log_for_general, ex.Message + Constants.vbNewLine + "Detail:" + ex.StackTrace, true);
                    }
                    else
                    {
                        HttpWebResponse TheRes = (HttpWebResponse)ex.Response;
                        if ((TheRes.StatusCode == HttpStatusCode.NotFound))
                        {
                            arr1 = PREPATCH_STATUS.NONE;
                            return LastResult;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Create_log(Error_Log_for_general, ex.Message + Constants.vbNewLine + "Detail:" + ex.StackTrace, true);
                }
                //prepatch_vanilaver = DoRequest("http://download.pso2.jp/patch_prod/patches_precede/version.ver")
            }
            //Dim PATH_MYDOCUMENTS As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            bool IsPrepatchExist = false;
            if ((IsThroughtout == false))
            {
                string Documents_PSO2 = _MyDocuments + "\\SEGA\\PHANTASYSTARONLINE2";
                string TheLocalVersion = null;
                if ((My.Application.String_IsNullOrWhiteSpace(TheLocalVersion) == true))
                {
                    if ((My.Computer.FileSystem.FileExists(Documents_PSO2 + "\\precede.txt") == true))
                    {
                        string TheTemper = System.IO.File.ReadAllText(Documents_PSO2 + "\\precede.txt");
                        if ((TheTemper.Contains(Constants.vbTab) == true))
                        {
                            TheLocalVersion = TheTemper.Split(new char[] { Constants.vbTab })(0);
                        }
                        else
                        {
                            TheLocalVersion = TheTemper;
                        }
                    }
                }
                if ((My.Application.String_IsNullOrWhiteSpace(TheLocalVersion) == true))
                {
                    if ((My.Computer.FileSystem.FileExists(Documents_PSO2 + "\\version.ver") == true))
                        TheLocalVersion = System.IO.File.ReadAllText(Documents_PSO2 + "\\version.ver");
                }
                if ((My.Application.String_IsNullOrWhiteSpace(TheLocalVersion) == true))
                {
                    TheLocalVersion = Registry_GetValue(My.Computer.Registry.CurrentUser.Name() + "\\Software\\AIDA", "PSO2RemoteVersion", "");
                }
                if ((My.Application.String_IsNullOrWhiteSpace(TheLocalVersion) == true))
                    return LastResult;
                string[] TheSplitUpPrePatch = prepatch_vanilaver.Split(new char[] { "_" });
                string[] TheSplitUpLocal = TheLocalVersion.Split(new char[] { "_" });
                int MainVersionPrepatch = Int32.Parse(TheSplitUpPrePatch(0).Replace("v", ""));
                int MainVersionLocal = Int32.Parse(TheSplitUpLocal(0).Replace("v", ""));
                if ((MainVersionPrepatch == MainVersionLocal))
                {
                    int SubVersionPrepatch = Int32.Parse(TheSplitUpPrePatch(2));
                    int SubVersionLocal = Int32.Parse(TheSplitUpLocal(2));
                    if ((SubVersionPrepatch > SubVersionLocal))
                    {
                        IsPrepatchExist = true;
                        isApplied = true;
                    }
                    else
                    {
                        isApplied = false;
                    }
                }
                else if ((MainVersionPrepatch > MainVersionLocal))
                {
                    IsPrepatchExist = true;
                    isApplied = true;
                }
                else
                {
                    isApplied = false;
                }
            }
            else
            {
                IsPrepatchExist = true;
                isApplied = true;
            }
            if ((IsPrepatchExist == true))
            {
                short IsPrepatchList = -1;
                while ((IsPrepatchList == -1))
                {
                    try
                    {
                        HttpWebRequest request = HttpWebRequest.Create(new Uri("http://download.pso2.jp/patch_prod/patches_precede/patchlist0.txt"));
                        request.Method = "HEAD";
                        request.UserAgent = "AQUA_HTTP";
                        request.Proxy = null;
                        HttpWebResponse response = request.GetResponse();
                        if (((response != null)))
                        {
                            if ((response.StatusCode == HttpStatusCode.OK))
                            {
                                IsPrepatchList = 1;
                            }
                            else if ((response.StatusCode == HttpStatusCode.NotFound))
                            {
                                IsPrepatchList = 0;
                            }
                            else
                            {
                                IsPrepatchList = -1;
                            }
                        }
                    }
                    catch (System.Net.WebException ex)
                    {
                        if ((ex.Response == null))
                        {
                            IsPrepatchList = -1;
                            Create_log(Error_Log_for_general, ex.Message + Constants.vbNewLine + "Detail:" + ex.StackTrace, true);
                        }
                        else
                        {
                            HttpWebResponse TheRes = (HttpWebResponse)ex.Response;
                            if ((TheRes.StatusCode == HttpStatusCode.NotFound))
                            {
                                IsPrepatchList = 0;
                            }
                            else if ((TheRes.StatusCode == HttpStatusCode.OK))
                            {
                                IsPrepatchList = 1;
                            }
                            else
                            {
                                IsPrepatchList = -1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        IsPrepatchList = -1;
                        Create_log(Error_Log_for_general, ex.Message + Constants.vbNewLine + "Detail:" + ex.StackTrace, true);
                    }
                }
                if ((IsPrepatchList == 1))
                {
                    bool Errored = true;
                    for (int TheCount = 1; TheCount <= 10; TheCount++)
                    {
                        Errored = true;
                        while ((Errored == true))
                        {
                            try
                            {
                                HttpWebRequest Additionrequest = HttpWebRequest.Create(new Uri("http://download.pso2.jp/patch_prod/patches_precede/patchlist" + Convert.ToString(TheCount) + ".txt"));
                                Additionrequest.Method = "HEAD";
                                Additionrequest.UserAgent = "AQUA_HTTP";
                                Additionrequest.Proxy = null;
                                HttpWebResponse Additionresponse = Additionrequest.GetResponse();
                                if (((Additionresponse != null)))
                                {
                                    if ((Additionresponse.StatusCode == HttpStatusCode.OK))
                                    {
                                        Errored = false;
                                        IsPrepatchList += 1;
                                    }
                                    else if ((Additionresponse.StatusCode == HttpStatusCode.NotFound))
                                    {
                                        Errored = false;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                            }
                            catch (System.Net.WebException ex)
                            {
                                Errored = true;
                                if (((ex.Response != null)))
                                {
                                    HttpWebResponse TheRes = (HttpWebResponse)ex.Response;
                                    if ((TheRes.StatusCode == HttpStatusCode.NotFound))
                                    {
                                        Errored = false;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Errored = true;
                            }
                        }
                    }
                }
                if ((IsPrepatchList > 0))
                {
                    arr1 = PREPATCH_STATUS.DOWNLOADING;
                    LastResult(1) = IsPrepatchList;
                }
                else if ((IsPrepatchList == 0))
                {
                    arr1 = PREPATCH_STATUS.APPLIED;
                }
            }
            else
            {
                if (!isApplied)
                {
                    arr1 = PREPATCH_STATUS.DOWNLOADING;
                }
                else
                {
                    arr1 = PREPATCH_STATUS.NONE;
                }
            }
            return LastResult;
        }//*/

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
            { result = new Infos.VersionCheckResult(ex); }
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

        public PSO2PrecedeVersion GetPrecedeVersion()
        {
            string prepatch_vanilaver = null;
            try
            {
                if (!this.myWebClient.IsBusy)
                    prepatch_vanilaver = this.myWebClient.DownloadString(DefaultValues.PatchInfo.PrecedeVersionLink);
                else
                    prepatch_vanilaver = WebClientPool.GetWebClient_PSO2Download().DownloadString(DefaultValues.PatchInfo.PrecedeVersionLink);
            }
            catch (WebException webEx)
            {
                if (webEx.Response != null)
                {
                    HttpWebResponse res = webEx.Response as HttpWebResponse;
                    if (res == null)
                        throw webEx;
                    else if (res.StatusCode == HttpStatusCode.NotFound)
                        return null;
                }
                else throw webEx;
            }
            if (string.IsNullOrWhiteSpace(prepatch_vanilaver))
                return null;
            else
                return PSO2PrecedeVersion.Parse(prepatch_vanilaver);
        }

        protected virtual Dictionary<string, DefaultValues.PatchInfo.PatchList> GetFilesList(PSO2PrecedeVersion precede)
        {
            if (!this.IsBusy)
            {
                this.myFileList.Clear();
                int i = 0;

                Dictionary<string, DefaultValues.PatchInfo.PatchList> precedePatchList = new Dictionary<string, DefaultValues.PatchInfo.PatchList>();
                string precedelistName;
                for (int iii = 0; iii < precede.ListCount; iii++)
                {
                    precedelistName = string.Format(DefaultValues.PatchInfo.file_precedelist, iii.ToString());
                    precedePatchList.Add(precedelistName, new DefaultValues.PatchInfo.PatchList(DefaultValues.Web.MainDownloadLink, precedelistName));
                }

                this.ProgressTotal = precedePatchList.Count;
                byte[] bytes;
                foreach (var item in precedePatchList)
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
                        this.myFileList.Add(item.Key, bytes);
                }

                return precedePatchList;
            }
            else
                return null;
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
                string currentBaseUrl;
                KeyValuePair<string, MemoryStream> _pair;
                this.CurrentStep = LanguageManager.GetMessageText("PSO2UpdateManager_BuildingFileList", "Building file list");
                for (int i = 0; i < filelist.Count; i++)
                {
                    _pair = filelist[i];
                    currentBaseUrl = DefaultValues.PatchInfo.PatchListFiles[_pair.Key].BaseURL;
                    using (StreamReader sr = new StreamReader(_pair.Value))
                        while (!sr.EndOfStream)
                        {
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
                                        result[pso2filebuffer.WindowFilename] = pso2filebuffer;
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
            PSO2PrecedeVersion vgetPrecedeVersion = this.GetPrecedeVersion();
            if (vgetPrecedeVersion != null)
            {
                string currentClientVerString = MySettings.PSO2Version;
                PSO2Version currentClientVer = null;
                if (!string.IsNullOrEmpty(currentClientVerString))
                    currentClientVer = new PSO2Version(currentClientVerString);
                if (currentClientVer != null)
                {
                    if (vgetPrecedeVersion.IsValid(currentClientVer))
                    {
                        Dictionary<string, DefaultValues.PatchInfo.PatchList> listoffiles = GetFilesList(vgetPrecedeVersion);
                        if (listoffiles != null)
                        {
                            WorkerParams wp = e.Argument as WorkerParams;
                            string pso2Path = Path.Combine(wp.PSO2Path, DefaultValues.Directory.PrecedeFolderName);
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
                                e.Result = new PSO2PrecedeResult(UpdateResult.Failed, PREPATCH_STATUS.UNKNOWN);
                        }
                        else
                        {
                            e.Result = new PSO2PrecedeResult(UpdateResult.Unknown, PREPATCH_STATUS.UNKNOWN);
                            throw new PSO2UpdateException(PSO2PrecedeResult.GetMsg(PREPATCH_STATUS.UNKNOWN));
                        }
                    }
                    else
                    {
                        e.Result = new PSO2PrecedeResult(PREPATCH_STATUS.NONE);
                        throw new PSO2UpdateException(PSO2PrecedeResult.GetMsg(PREPATCH_STATUS.NONE));
                    }
                }
                else
                {
                    e.Result = new PSO2PrecedeResult(UpdateResult.Unknown, PREPATCH_STATUS.UNKNOWN);
                    throw new PSO2UpdateException(LanguageManager.GetMessageText("PSO2NotInstalled", "PSO2 Client is not installed or recognized yet."));
                }
            }
            else
            {
                e.Result = new PSO2PrecedeResult(PREPATCH_STATUS.NONE);
                throw new PSO2UpdateException(PSO2PrecedeResult.GetMsg(PREPATCH_STATUS.NONE));
            }
        }

        private void Anothersmallthreadpool_KaboomFinished(object sender, KaboomFinishedEventArgs e)
        {
            this.OnProgressStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            if (e.Error != null)
                this.OnHandledException(e.Error);
            else
            {
                LogManager.GeneralLog.Print(PSO2PrecedeResult.GetMsg(e.Result, e.FailedList == null ? 0 : e.FailedList.Count), LogLevel.Error);
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
                if (e.Result is PSO2PrecedeResult)
                {
                    PSO2PrecedeResult updateresult = e.Result as PSO2PrecedeResult;
                    switch (updateresult.Code)
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
        public bool IsPrepatchExist { get { return !Leayal.IO.DirectoryHelper.IsFolderEmpty(DefaultValues.Directory.PrecedeFolder); } }

        public bool IsBusy { get { return (this._isbusy || myWebClient.IsBusy); } }

        private int _totalprogress;
        public int ProgressTotal
        {
            get { return _totalprogress; }
            internal set
            {
                this._totalprogress = value;
                this.OnCurrentTotalProgressChanged(new ProgressEventArgs(value));
            }
        }

        private int _currentprogress;
        public int ProgressCurrent
        {
            get { return _currentprogress; }
            internal set
            {
                this._currentprogress = value;
                this.OnCurrentProgressChanged(new ProgressEventArgs(value));
            }
        }

        private string _currentstep;
        public string CurrentStep
        {
            get { return _currentstep; }
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
        public class PSO2PrecedeVersion
        {
            public static readonly char[] TabOnly = { ControlChars.Tab };
            public static readonly char[] SpaceOnly = { ' ' };
            public static PSO2PrecedeVersion Parse(string rawstring)
            {
                string[] splitted = null;
                if (rawstring.IndexOf(TabOnly[0]) > -1)
                {
                    splitted = rawstring.Split(TabOnly);
                    return new PSO2PrecedeVersion(splitted[0], TabOnly[0], int.Parse(splitted[1]) + 1);
                }
                else if (rawstring.IndexOf(SpaceOnly[0]) > -1)
                {
                    splitted = rawstring.Split(SpaceOnly);
                    return new PSO2PrecedeVersion(splitted[0], SpaceOnly[0], int.Parse(splitted[1]) + 1);
                }
                else
                    return new PSO2PrecedeVersion(rawstring, ControlChars.Tab, 1);
            }

            /// <summary>
            /// Return the number of missing patchlist. If the version is mismatch, return all lists as missing. If the cache is older than current version, return -1
            /// </summary>
            /// <param name="ver">The version to compare</param>
            /// <returns>int. Return the number of missing patchlist</returns>
            public int CompareTo(PSO2PrecedeVersion ver)
            {
                int re = this.Version.CompareTo(ver.Version);
                if (re == 0)
                    return (ver.ListCount - this.ListCount);
                else if (re > 1)
                    return ver.ListCount;
                else
                    return -1;
            }

            /// <summary>
            /// Check if this version is newer for given version.
            /// </summary>
            /// <param name="ver">PSO2Version. The version the be checked with</param>
            /// <returns>Bool</returns>
            public bool IsValid(PSO2Version ver)
            {
                return (this.Version.CompareTo(ver) > 0);
            }

            /// <summary>
            /// Check if this version is newer or equal for given version.
            /// </summary>
            /// <param name="ver">PSO2PrecedeVersion. The version the be checked with</param>
            /// <returns>Bool</returns>
            public bool IsValid(PSO2PrecedeVersion ver)
            {
                return (this.Version.CompareTo(ver.Version) >= 0);
            }

            /// <summary>
            /// Check if this version is equal for given version.
            /// </summary>
            /// <param name="ver">PSO2PrecedeVersion. The version the be checked with</param>
            /// <returns>Bool</returns>
            public bool IsEqual(PSO2PrecedeVersion ver)
            {
                if ((this.Version.CompareTo(ver.Version) == 0) && (this.ListCount == ver.ListCount))
                    return true;
                else
                    return false;
            }

            public PSO2Version Version { get; }

            public PREPATCH_STATUS Status { get; internal set; }
            public int ListCount { get; }
            private char splitchar;
            private PSO2PrecedeVersion(string ver, char _splitchar, int countFromZero)
            {
                this.Version = new PSO2Version(ver);
                this.ListCount = countFromZero;
                this.splitchar = _splitchar;
            }

            public override string ToString()
            {
                return string.Concat(this.Version, this.splitchar, (this.ListCount - 1).ToString());
            }
        }

        public class PSO2PrecedeResult
        {
            public UpdateResult Code { get; }
            public PREPATCH_STATUS Status { get; }
            public string StatusMessage { get; }
            public object UserToken { get; }

            public PSO2PrecedeResult(UpdateResult code, PREPATCH_STATUS _statuscode, int missingfilecount) : this(code, _statuscode, missingfilecount, null) { }
            public PSO2PrecedeResult(UpdateResult code, PREPATCH_STATUS _statuscode) : this(code, _statuscode, -1, null) { }
            public PSO2PrecedeResult(PREPATCH_STATUS _statuscode, int missingfilecount, object _userToken) : this(UpdateResult.Success, _statuscode, missingfilecount, _userToken) { }
            public PSO2PrecedeResult(PREPATCH_STATUS _statuscode) : this(UpdateResult.Success, _statuscode) { }
            public PSO2PrecedeResult(UpdateResult code, PREPATCH_STATUS _statuscode, int missingfilecount, object _userToken)
            {
                this.Code = code;
                this.Status = _statuscode;
                this.StatusMessage = GetMsg(code, missingfilecount);
                this.UserToken = _userToken;
            }

            public static string GetMsg(UpdateResult msgCode, int missingfilecount)
            {
                string result;
                switch (msgCode)
                {
                    case UpdateResult.Failed:
                        result = LanguageManager.GetMessageText("PSO2PrecedeResult_Failed", "Failed to download pre-patch files.");
                        break;
                    case UpdateResult.Success:
                        result = LanguageManager.GetMessageText("PSO2PrecedeResult_Success", "The pre-patches have been downloaded successfully.");
                        break;
                    case UpdateResult.MissingSomeFiles:
                        result = string.Format(LanguageManager.GetMessageText("PSO2PrecedeResult_MissingSomeFiles", "The pre-patches have been updated but {0} files were not downloaded"), missingfilecount);
                        break;
                    default:
                        result = LanguageManager.GetMessageText("PSO2PrecedeResult_UnknownError", "Unknown error while updating game");
                        break;
                }
                return result;
            }

            public static string GetMsg(PREPATCH_STATUS msgCode)
            {
                string result;
                switch (msgCode)
                {
                    case PREPATCH_STATUS.APPLIED:
                        result = LanguageManager.GetMessageText("PSO2PrecedeResult_Applied", "The pre-patches are ready to be applied.");
                        break;
                    case PREPATCH_STATUS.NONE:
                        result = LanguageManager.GetMessageText("PSO2PrecedeResult_None", "There are no pre-patches currently.");
                        break;
                    case PREPATCH_STATUS.DOWNLOADING:
                        result = string.Format(LanguageManager.GetMessageText("PSO2PrecedeResult_Dowloading", "There are pre-patches now."));
                        break;
                    default:
                        result = LanguageManager.GetMessageText("PSO2PrecedeResult_Unknown", "Unable to check for pre-patches.");
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
