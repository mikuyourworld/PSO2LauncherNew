using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Collections.Concurrent;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using PSO2ProxyLauncherNew.Classes.Events;
using static PSO2ProxyLauncherNew.Classes.PSO2.PSO2UpdateManager;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using Microsoft.VisualBasic;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class AnotherSmallThreadPool : IDisposable
    {
        private int _DownloadedFileCount;
        public int DownloadedFileCount { get { return this._DownloadedFileCount; } }
        public int MaxThreadCount { get { return this._bwList.MaxCount; } set { this._bwList.MaxCount = value; } }

        public int CurrentThreadCount
        {
            get
            {
                if (this._bwList == null)
                    return 0;
                else
                    return this._bwList.GetNumberOfRunning();
            }
        }

        private int _FileCount;
        public int FileCount { get { return this._FileCount; } }
        public int FileTotal { get { return this.myPSO2filesList.Count; } }
        public string PSO2Path { get; }

        ConcurrentBag<string> _failedList;
        ConcurrentQueue<string> _keys;
        ConcurrentDictionary<string, PSO2File> myPSO2filesList;
        ConcurrentDictionary<string, PSO2FileChecksum> myCheckSumList;

        BackgroundWorkerManager _bwList;

        private bool _IsBusy;
        public bool IsBusy
        {
            get
            {
                return (this._IsBusy || this._bwList.GetNumberOfRunning() > 0);
            }
            private set
            {
                this._IsBusy = value;
            }
        }

        public AnotherSmallThreadPool(string _pso2Path, ConcurrentDictionary<string, PSO2File> PSO2filesList)
        {
            this._bwList = new BackgroundWorkerManager();
            this._bwList.WorkerAdded += this._bwList_WorkerAdded;
            this.MaxThreadCount = MySettings.GameClientUpdateThreads;
            MySettings.GameClientUpdateThreadsChanged += MySettings_GameClientUpdateThreadsChanged;
            this.SynchronizationContextObject = WebClientPool.SynchronizationContext;
            this.IsBusy = false;
            this.PSO2Path = _pso2Path;
            this.myCheckSumList = new ConcurrentDictionary<string, PSO2FileChecksum>();
            this.ResetWork(PSO2filesList);
        }

        private void MySettings_GameClientUpdateThreadsChanged(object sender, EventArgs e)
        {
            this.MaxThreadCount = MySettings.GameClientUpdateThreads;
        }

        private void _bwList_WorkerAdded(object sender, Events.ExtendedBackgroundWorkerEventArgs e)
        {
            e.Worker.DoWork += this.Bworker_DoWork;
            e.Worker.RunWorkerCompleted += this.Bworker_RunWorkerCompleted;
        }

        private void ResetWork(ConcurrentDictionary<string, PSO2File> PSO2filesList)
        {
            if (this.IsBusy)
                this.CancelWork();
            this._DownloadedFileCount = 0;
            this._FileCount = 0;
            this._failedList = new ConcurrentBag<string>();
            this.myPSO2filesList = PSO2filesList;
            this.ReadChecksumCache();
            this._keys = new ConcurrentQueue<string>(PSO2filesList.Keys);
        }

        private void ReadChecksumCache()
        {
            this.myCheckSumList.Clear();
            if (MySettings.GameClientUpdateCache)
            {
                string checksumpath = Infos.DefaultValues.MyInfo.Filename.PSO2ChecksumListPath;
                if (File.Exists(checksumpath))
                {
                    using (FileStream fs = File.OpenRead(checksumpath))
                        if (fs.Length > 0)
                            try
                            {
                                using (DeflateStream gs = new DeflateStream(fs, CompressionMode.Decompress))
                                using (StreamReader sr = new StreamReader(gs, Encoding.UTF8))
                                {
                                    PSO2FileChecksum hohoho;
                                    if (!sr.EndOfStream)
                                    {
                                        string tmpline = null;
                                        string[] tmpsplit;
                                        string checksumver = sr.ReadLine();
                                        if (checksumver == MySettings.PSO2Version)
                                            while (!sr.EndOfStream)
                                            {
                                                tmpline = sr.ReadLine();
                                                if (!string.IsNullOrWhiteSpace(tmpline))
                                                {
                                                    tmpsplit = tmpline.Split('\t');
                                                    hohoho = new PSO2FileChecksum(tmpsplit[0], long.Parse(tmpsplit[1]), tmpsplit[2]);
                                                    this.myCheckSumList.TryAdd(hohoho.RelativePath.ToLower(), hohoho);
                                                }
                                            }
                                    }
                                }
                            }
                            catch (InvalidDataException dataEx)
                            { this.myCheckSumList.Clear(); Log.LogManager.GeneralLog.Print(dataEx); }
                }
            }
        }

        private void WriteChecksumCache(string pso2version)
        {
            if (MySettings.GameClientUpdateCache && !this.myCheckSumList.IsEmpty)
            {
                using (FileStream fs = File.Create(Infos.DefaultValues.MyInfo.Filename.PSO2ChecksumListPath))
                using (DeflateStream gs = new DeflateStream(fs, CompressionMode.Compress))
                using (StreamWriter sr = new StreamWriter(gs, Encoding.UTF8))
                {
                    sr.Write(pso2version);
                    PSO2FileChecksum aaa;
                    foreach (var _key in this.myCheckSumList.Keys)
                        if (this.myCheckSumList.TryGetValue(_key, out aaa))
                            sr.Write(string.Concat('\n', aaa.RelativePath, '\t', aaa.FileSize, '\t', aaa.MD5));
                    sr.Flush();
                }
            }
        }

        private bool SeekNextMove()
        {
            if (!_keys.IsEmpty)
            {
                var asdasd = this._bwList.GetRestingWorker();
                if (asdasd != null)
                    asdasd.RunWorkerAsync();
                return true;
            }
            else
                return false;
        }

        private void Bworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (this._bwList.GetNumberOfRunning() == 0)
                {
                    if (this.cancelling)
                    {
                        string asfw;
                        while (_keys.TryDequeue(out asfw))
                            this._failedList.Add(asfw);
                        this.WriteChecksumCache(MySettings.PSO2Version);
                        this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Cancelled, this._failedList, null, this.token));
                        this.cancelling = false;
                        if (_disposed)
                            (sender as ExtendedBackgroundWorker).Dispose();
                    }
                }
            }
            else if (!this.SeekNextMove())
            {
                if (this._bwList.GetNumberOfRunning() == 0)
                {
                    if (e.Error != null)
                    {
                        this.WriteChecksumCache(MySettings.PSO2Version);
                        this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Failed, null, e.Error, this.token));
                    }
                    else if (e.Cancelled)
                    { }
                    else
                    {
                        if (myPSO2filesList.Count == this.DownloadedFileCount)
                        {
                            this.WriteChecksumCache(this.token.NewVersionString);
                            this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Success, null, null, this.token));
                        }
                        else if (this.DownloadedFileCount > myPSO2filesList.Count)
                        {
                            this.WriteChecksumCache(this.token.NewVersionString);
                            this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Success, null, null, this.token));
                        }
                        else
                        {
                            //WebClientPool.SynchronizationContext.Send(new SendOrPostCallback(delegate { System.Windows.Forms.MessageBox.Show("IT'S A FAIL", "Update"); }), null);
                            if ((myPSO2filesList.Count - this.DownloadedFileCount) < 3)
                            {
                                this.WriteChecksumCache(this.token.NewVersionString);
                                this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.MissingSomeFiles, this._failedList, null, this.token));
                            }
                            else
                            {
                                this.WriteChecksumCache(MySettings.PSO2Version);
                                this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Failed, this._failedList, null, this.token));
                            }
                        }
                    }
                }
            }
        }

        private void Bworker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExtendedBackgroundWorker bworker = sender as ExtendedBackgroundWorker;
            string currentfilepath, filemd5;
            string _key;
            PSO2File _value;
            PSO2FileChecksum checksumobj;
            if (_keys.TryDequeue(out _key))
                if (myPSO2filesList.TryGetValue(_key, out _value))
                {
                    currentfilepath = null;
                    filemd5 = null;
                    if (this.myCheckSumList.TryGetValue(_key, out checksumobj))
                    {
                        currentfilepath = Infos.CommonMethods.PathConcat(this.PSO2Path, checksumobj.RelativePath);
                        using (FileStream fs = File.Open(currentfilepath, FileMode.Open))
                        {
                            if (fs.Length == checksumobj.FileSize)
                                filemd5 = checksumobj.MD5;
                            else
                                currentfilepath = null;
                        }
                    }
                    if (string.IsNullOrEmpty(currentfilepath))
                    {
                        currentfilepath = Infos.CommonMethods.PathConcat(this.PSO2Path, _key);
                        checksumobj = PSO2FileChecksum.FromFile(this.PSO2Path, currentfilepath);
                        filemd5 = checksumobj.MD5;
                        if (!this.myCheckSumList.TryAdd(checksumobj.RelativePath, checksumobj))
                            this.myCheckSumList[checksumobj.RelativePath] = checksumobj;
                    }
                    if (!string.IsNullOrEmpty(filemd5))
                    {
                        if (_value.MD5Hash == filemd5)
                            Interlocked.Increment(ref this._DownloadedFileCount);
                        else
                        {
                            this.OnStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingFile", "Downloading file {0}"), _value.SafeFilename)));
                            if (bworker.WebClient.DownloadFile(_value.Url, currentfilepath))
                            {
                                this.myCheckSumList.TryUpdate(checksumobj.RelativePath, PSO2FileChecksum.FromFile(this.PSO2Path, currentfilepath), checksumobj);
                                Interlocked.Increment(ref this._DownloadedFileCount);
                            }
                            else
                                _failedList.Add(_key);
                        }
                    }
                    else
                    {
                        this.OnStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingFile", "Downloading file {0}"), _value.SafeFilename)));
                        if (bworker.WebClient.DownloadFile(_value.Url, currentfilepath))
                        {
                            this.myCheckSumList.TryUpdate(checksumobj.RelativePath, PSO2FileChecksum.FromFile(this.PSO2Path, currentfilepath), checksumobj);
                            Interlocked.Increment(ref this._DownloadedFileCount);
                        }
                        else
                            _failedList.Add(_key);
                    }
                }
            Interlocked.Increment(ref this._FileCount);
            this.OnProgressChanged(new DetailedProgressChangedEventArgs(this.FileCount, this.FileTotal));
            if (bworker.CancellationPending)
                e.Cancel = true;
        }

        public SynchronizationContext SynchronizationContextObject { get; set; }

        public event EventHandler<StepEventArgs> StepChanged;
        protected virtual void OnStepChanged(StepEventArgs e)
        {
            if (this.StepChanged != null)
                this.SynchronizationContextObject?.Post(new SendOrPostCallback(delegate { this.StepChanged.Invoke(this, e); }), null);
        }

        public event EventHandler<KaboomFinishedEventArgs> KaboomFinished;
        protected virtual void OnKaboomFinished(KaboomFinishedEventArgs e)
        {
            if (this.KaboomFinished != null)
                this.SynchronizationContextObject?.Post(new SendOrPostCallback(delegate { this.KaboomFinished.Invoke(this, e); }), null);
        }

        public event EventHandler<DetailedProgressChangedEventArgs> ProgressChanged;
        protected virtual void OnProgressChanged(DetailedProgressChangedEventArgs e)
        {
            if (this.ProgressChanged != null)
                this.SynchronizationContextObject?.Post(new SendOrPostCallback(delegate { try { this.ProgressChanged.Invoke(this, e); } catch (InvalidOperationException) { } }), null);
        }
        private bool cancelling;
        public void CancelWork()
        {
            if (this.IsBusy)
            {
                this.cancelling = true;
                this._bwList.CancelAsync();
            }
            else
            {
                this._bwList.Dispose();
            }
        }

        private WorkerParams token;

        public void StartWork(WorkerParams argument)
        {
            if (!this.IsBusy)
            {
                this.token = argument;
                if (!myPSO2filesList.IsEmpty)
                {
                    var asdasd = this._bwList.GetRestingWorker();
                    if (asdasd != null)
                        asdasd.RunWorkerAsync();
                }
            }
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            MySettings.GameClientUpdateThreadsChanged -= MySettings_GameClientUpdateThreadsChanged;
            this.CancelWork();
        }

        private class PSO2FileChecksum
        {
            public static PSO2FileChecksum FromFile(string folder, string filepath)
            {
                string result = string.Empty;
                long len = 0;
                if (File.Exists(filepath))
                {
                    StringBuilder _stringBuilder = new StringBuilder(32);
                    using (MD5 md5engine = System.Security.Cryptography.MD5.Create())
                    using (FileStream fs = File.OpenRead(filepath))
                    {
                        len = fs.Length;
                        byte[] arrbytHashValue = md5engine.ComputeHash(fs);
                        for (int i = 0; i < arrbytHashValue.Length; i++)
                            _stringBuilder.Append(arrbytHashValue[i].ToString("X2"));
                        md5engine.Clear();
                    }
                    result = _stringBuilder.ToString();
                }
                if (Path.IsPathRooted(filepath))
                    return new PSO2FileChecksum(filepath.Remove(0, folder.Length), len, result);
                else
                    return new PSO2FileChecksum(filepath, len, result);
            }

            public PSO2FileChecksum(string _relativePath, long filelength, string filemd5)
            {
                _relativePath = Infos.CommonMethods.PathTrim(_relativePath);
                this.RelativePath = _relativePath.TrimStart('\\');
                this.FileSize = filelength;
                this.MD5 = filemd5;
            }

            public string RelativePath { get; }
            public long FileSize { get; }
            public string MD5 { get; }
        }
    }
}
