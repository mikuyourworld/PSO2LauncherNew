using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Collections.Concurrent;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using PSO2ProxyLauncherNew.Classes.Events;
using static PSO2ProxyLauncherNew.Classes.PSO2.PSO2UpdateManager;

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

        public AnotherSmallThreadPool(string _pso2Path, ConcurrentDictionary<string, PSO2File> PSO2filesList, int _MaxThreadCount)
        {
            this._bwList = new BackgroundWorkerManager();
            this._bwList.WorkerAdded += this._bwList_WorkerAdded;
            if (_MaxThreadCount < Environment.ProcessorCount)
            {
                if (_MaxThreadCount < 1)
                    _MaxThreadCount = 1;
                this.MaxThreadCount = _MaxThreadCount;
            }
            else
                this.MaxThreadCount = Environment.ProcessorCount;
            this.SynchronizationContextObject = WebClientPool.SynchronizationContext;
            this.IsBusy = false;
            this.PSO2Path = _pso2Path;
            this.ResetWork(PSO2filesList);
        }

        public AnotherSmallThreadPool(string _pso2Path, ConcurrentDictionary<string, PSO2File> PSO2filesList) : this(_pso2Path, PSO2filesList, Environment.ProcessorCount) { }

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
            this._keys = new ConcurrentQueue<string>(PSO2filesList.Keys);
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
                            //WebClientPool.SynchronizationContext.Send(new SendOrPostCallback(delegate { System.Windows.Forms.MessageBox.Show("IT'S A FAIL", "Update"); }), null);
                            if ((myPSO2filesList.Count - this.DownloadedFileCount) < 3)
                                this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.MissingSomeFiles, this._failedList, null, this.token));
                            else
                                this.OnKaboomFinished(new KaboomFinishedEventArgs(UpdateResult.Failed, this._failedList, null, this.token));
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
            KeyValuePair<string, PSO2File> _keypair;
            if (_keys.TryDequeue(out _key))
                if (myPSO2filesList.TryGetValue(_key, out _value))
                {
                    _keypair = new KeyValuePair<string, PSO2File>(_key, _value);
                    currentfilepath = Infos.CommonMethods.PathConcat(this.PSO2Path, _keypair.Key);
                    filemd5 = Infos.CommonMethods.FileToMD5Hash(currentfilepath);
                    if (!string.IsNullOrEmpty(filemd5))
                    {
                        if (_keypair.Value.MD5Hash == filemd5)
                            Interlocked.Increment(ref this._DownloadedFileCount);
                        else
                        {
                            this.OnStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingFile", "Downloading file {0}"), _keypair.Value.SafeFilename)));
                            if (bworker.WebClient.DownloadFile(_keypair.Value.Url, currentfilepath))
                                Interlocked.Increment(ref this._DownloadedFileCount);
                            else
                                _failedList.Add(_keypair.Key);
                        }
                    }
                    else
                    {
                        this.OnStepChanged(new StepEventArgs(string.Format(LanguageManager.GetMessageText("PSO2UpdateManager_DownloadingFile", "Downloading file {0}"), _keypair.Value.SafeFilename)));
                        if (bworker.WebClient.DownloadFile(_keypair.Value.Url, currentfilepath))
                            Interlocked.Increment(ref this._DownloadedFileCount);
                        else
                            _failedList.Add(_keypair.Key);
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

        public void StartWork()
        {
            this.StartWork(null);
        }

        private object token;

        public void StartWork(object argument)
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
            this.CancelWork();
        }
    }
}
