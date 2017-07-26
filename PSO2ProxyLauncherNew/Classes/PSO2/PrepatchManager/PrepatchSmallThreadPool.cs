using System;
using System.ComponentModel;
using System.Threading;
using System.Collections.Concurrent;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using PSO2ProxyLauncherNew.Classes.Events;
using System.IO;
using System.Text;
using System.IO.Compression;
using Leayal.Log;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PrepatchManager
{
    class PrepatchSmallThreadPool : IDisposable
    {
        private int _DownloadedFileCount;
        public int DownloadedFileCount { get { return this._DownloadedFileCount; } }

        private int _throttlecachespeed;
        public int ThrottleCacheSpeed { get { return this._throttlecachespeed; } }
        private int _FileCount;
        public int FileCount { get { return this._FileCount; } }
        public int FileTotal { get { return this.myPSO2filesList.Count; } }
        public string PSO2Path { get; }

        ConcurrentBag<string> _failedList;
        ConcurrentQueue<string> _keys;
        ConcurrentDictionary<string, PSO2File> myPSO2filesList;

        ExtendedBackgroundWorker bWorker;

        private bool _IsBusy;
        public bool IsBusy
        {
            get
            {
                return (this._IsBusy || this.bWorker.IsBusy);
            }
            private set
            {
                this._IsBusy = value;
            }
        }

        public PrepatchSmallThreadPool(string _pso2Path, ConcurrentDictionary<string, PSO2File> PSO2filesList)
        {
            this._throttlecachespeed = MySettings.GameClientUpdateThrottleCache;
            MySettings.GameClientUpdateThrottleCacheChanged += this.MySettings_GameClientUpdateThrottleCacheChanged;
            this.bWorker = new ExtendedBackgroundWorker();
            this.bWorker.DoWork += this.Bworker_DoWork;
            this.bWorker.RunWorkerCompleted += this.Bworker_RunWorkerCompleted;
            this.SynchronizationContextObject = WebClientPool.SynchronizationContext;
            this.IsBusy = false;
            this.PSO2Path = _pso2Path;
            this.ResetWork(PSO2filesList);
        }

        private void MySettings_GameClientUpdateThrottleCacheChanged(object sender, IntEventArgs e)
        {
            // Limit because of reasons
            this._throttlecachespeed = Math.Min(e.Value, 4);
        }

        private void ResetWork(ConcurrentDictionary<string, PSO2File> PSO2filesList)
        {
            if (this.IsBusy)
                this.CancelWork();
            this._DownloadedFileCount = 0;
            this._FileCount = 0;
            this._failedList = new ConcurrentBag<string>();
            this.myPSO2filesList = PSO2filesList;
            this._keys = new ConcurrentQueue<string>(PSO2filesList.Keys);
        }

        private bool SeekNextMove()
        {
            if (!_keys.IsEmpty)
            {
                this.bWorker.RunWorkerAsync();
                return true;
            }
            else
                return false;
        }
        
        private void Bworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (this.cancelling)
                {
                    string asfw;
                    while (_keys.TryDequeue(out asfw))
                        this._failedList.Add(asfw);
                    this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Cancelled, this._failedList, null, this.token));
                    this.cancelling = false;
                    if (_disposed)
                        (sender as ExtendedBackgroundWorker).Dispose();
                }
            }
            else if (!this.SeekNextMove())
            {
                if (e.Error != null)
                    this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Failed, null, e.Error, this.token));
                else if (e.Cancelled)
                { }
                else
                {
                    if (myPSO2filesList.Count == this.DownloadedFileCount)
                        this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Success, null, null, this.token));
                    else if (this.DownloadedFileCount > myPSO2filesList.Count)
                        this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Success, null, null, this.token));
                    else
                    {
                        if ((myPSO2filesList.Count - this.DownloadedFileCount) < 3)
                            this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.MissingSomeFiles, this._failedList, null, this.token));
                        else
                            this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Failed, this._failedList, null, this.token));
                    }
                }
            }
        }
        private void Bworker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExtendedBackgroundWorker bworker = sender as ExtendedBackgroundWorker;
            string currentfilepath, filemd5, _key;
            PSO2File _value;
            PSO2FileChecksum checksumobj;
            if (_keys.TryDequeue(out _key))
                if (myPSO2filesList.TryGetValue(_key, out _value))
                {
                    currentfilepath = null;
                    filemd5 = null;

                    if (string.IsNullOrEmpty(currentfilepath))
                    {
                        currentfilepath = Infos.CommonMethods.PathConcat(this.PSO2Path, _key);
                        checksumobj = PSO2FileChecksum.FromFile(this.PSO2Path, currentfilepath);
                        filemd5 = checksumobj.MD5;
                    }
                    if (!string.IsNullOrEmpty(filemd5))
                    {
                        if (_value.MD5Hash == filemd5)
                            Interlocked.Increment(ref this._DownloadedFileCount);
                        else
                        {
                            this.OnStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingFile", "Downloading file {0}"), _value.SafeFilename)));
                            try
                            {
                                if (bworker.WebClient.DownloadFile(_value.Url, currentfilepath))
                                    Interlocked.Increment(ref this._DownloadedFileCount);
                                else
                                    _failedList.Add(_key);
                            }
                            catch (System.Net.WebException)
                            {
                                _failedList.Add(_key);
                            }
                        }
                    }
                    else
                    {
                        this.OnStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingFile", "Downloading file {0}"), _value.SafeFilename)));
                        try
                        {
                            if (bworker.WebClient.DownloadFile(_value.Url, currentfilepath))
                                Interlocked.Increment(ref this._DownloadedFileCount);
                            else
                                _failedList.Add(_key);
                        }
                        catch (System.Net.WebException)
                        {
                            _failedList.Add(_key);
                        }
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
                this.bWorker.CancelAsync();
            }
        }

        private PrepatchWorkerParams token;

        public void StartWork(PrepatchWorkerParams argument)
        {
            if (!this.IsBusy)
            {
                this.token = argument;
                if (!myPSO2filesList.IsEmpty)
                {
                    this.bWorker.RunWorkerAsync();
                    /*ExtendedBackgroundWorker asdasd;
                    asdasd = this._bwList.GetRestingWorker();
                    if (asdasd != null && !asdasd.IsBusy)
                        asdasd.RunWorkerAsync();
                    while (this._bwList.GetNumberOfRunning() < this._bwList.MaxCount)
                    {
                        asdasd = this._bwList.GetRestingWorker();
                        if (asdasd != null && !asdasd.IsBusy)
                            asdasd.RunWorkerAsync();
                    }//*/
                }
            }
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            MySettings.GameClientUpdateThrottleCacheChanged -= this.MySettings_GameClientUpdateThrottleCacheChanged;
            this.CancelWork();
        }

        private class PSO2FileChecksum
        {
            public static PSO2FileChecksum FromFile(string folder, string filepath)
            {
                string result = string.Empty;
                long len = 0;
                if (File.Exists(filepath))
                    using (FileStream fs = File.OpenRead(filepath))
                    {
                        len = fs.Length;
                        result = Leayal.Security.Cryptography.MD5Wrapper.FromStream(fs);
                    }
                if (Path.IsPathRooted(filepath))
                    return new PSO2FileChecksum(filepath.Remove(0, folder.Length), len, result);
                else
                    return new PSO2FileChecksum(filepath, len, result);
            }

            public PSO2FileChecksum(string _relativePath, long filelength, string filemd5)
            {
                _relativePath = Leayal.IO.PathHelper.PathTrim(_relativePath);
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
