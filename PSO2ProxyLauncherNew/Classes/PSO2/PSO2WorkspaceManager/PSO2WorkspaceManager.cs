using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Threading;
using PSO2ProxyLauncherNew.Classes.Events;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2WorkspaceManager
{
    class PSO2WorkspaceManager : IDisposable
    {
        private SynchronizationContext synccontext;
        private BackgroundWorker bWorker;

        public PSO2WorkspaceManager()
        {
            this.synccontext = SynchronizationContext.Current;
            this.bWorker = new BackgroundWorker();
            this.bWorker.WorkerReportsProgress = false;
            this.bWorker.WorkerSupportsCancellation = true;
            this.bWorker.DoWork += BWorker_DoWork;
            this.bWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
        }
        
        public void CleanUp(Forms.PSO2WorkspaceCleanupDialog options)
        {
            if (this.disposed)
                throw new ObjectDisposedException("PSO2WorkspaceManager");
            if (this.IsBusy)
                throw new InvalidOperationException();
            else
            {
                if (Directory.Exists(DefaultValues.Directory.DocumentWorkSpace))
                {
                    this.bWorker.RunWorkerAsync(options);
                    this.OnCleanupStarted(EventArgs.Empty);
                }
                else
                    this.OnHandledException(new HandledExceptionEventArgs(new DirectoryNotFoundException($"'{DefaultValues.Directory.DocumentWorkSpace}' was not found.")));
            }
        }

        private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.None));
            this.OnCleanupFinished(e);
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Infinite));
            Forms.PSO2WorkspaceCleanupDialog requestForm = e.Argument as Forms.PSO2WorkspaceCleanupDialog;

            // Predict how many files <FullPath, RelativePath>
            this.OnStepChanged(new StepEventArgs(LanguageManager.GetMessageText("WorkspaceManager_PreparingFileList", "Preparing file list")));
            Dictionary<string, string> fileList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (requestForm.StrictClean)
            {
                string[] characterPresetExts = CharacterPreset.GetFileExts();
                string tmpCurrentPath;
                foreach (string filename in Directory.EnumerateFiles(DefaultValues.Directory.DocumentWorkSpace, "*", SearchOption.TopDirectoryOnly))
                {
                    if (filename.EndsWith(".pso2", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".ver", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        fileList.Add(filename, filename.Remove(0, DefaultValues.Directory.DocumentWorkSpace.Length + 1));
                    else
                    {
                        for (int i = 0; i < characterPresetExts.Length; i++)
                            if (filename.EndsWith($".{characterPresetExts[i]}"))
                            {
                                fileList.Add(filename, filename.Remove(0, DefaultValues.Directory.DocumentWorkSpace.Length + 1));
                                break;
                            }
                    }
                }
                tmpCurrentPath = Path.Combine(DefaultValues.Directory.DocumentWorkSpace, "log");
                if (Directory.Exists(tmpCurrentPath))
                    foreach (string filename in Directory.EnumerateFiles(tmpCurrentPath, "*.txt", SearchOption.TopDirectoryOnly))
                        fileList.Add(filename, filename.Remove(0, DefaultValues.Directory.DocumentWorkSpace.Length + 1));
                tmpCurrentPath = Path.Combine(DefaultValues.Directory.DocumentWorkSpace, "download");
                if (Directory.Exists(tmpCurrentPath))
                    foreach (string filename in Directory.EnumerateFiles(tmpCurrentPath, "*", SearchOption.TopDirectoryOnly))
                    if (filename.EndsWith(".pat", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        fileList.Add(filename, filename.Remove(0, DefaultValues.Directory.DocumentWorkSpace.Length + 1));
                tmpCurrentPath = Path.Combine(DefaultValues.Directory.DocumentWorkSpace, "temp");
                if (Directory.Exists(tmpCurrentPath))
                    foreach (string filename in Directory.EnumerateFiles(tmpCurrentPath, "*", SearchOption.TopDirectoryOnly))
                        fileList.Add(filename, filename.Remove(0, DefaultValues.Directory.DocumentWorkSpace.Length + 1));
                tmpCurrentPath = Path.Combine(DefaultValues.Directory.DocumentWorkSpace, "pictures");
                if (Directory.Exists(tmpCurrentPath))
                    foreach (string filename in Directory.EnumerateFiles(tmpCurrentPath, "*", SearchOption.TopDirectoryOnly))
                        if (filename.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                            fileList.Add(filename, filename.Remove(0, DefaultValues.Directory.DocumentWorkSpace.Length + 1));
                tmpCurrentPath = Path.Combine(DefaultValues.Directory.DocumentWorkSpace, "symbolarts");
                if (Directory.Exists(tmpCurrentPath))
                    foreach (string filename in Directory.EnumerateFiles(tmpCurrentPath, "*.sar", SearchOption.AllDirectories))
                        fileList.Add(filename, filename.Remove(0, DefaultValues.Directory.DocumentWorkSpace.Length + 1));
            }
            else
            {
                foreach (string filename in Directory.EnumerateFiles(DefaultValues.Directory.DocumentWorkSpace, "*", SearchOption.AllDirectories))
                    fileList.Add(filename, filename.Remove(0, DefaultValues.Directory.DocumentWorkSpace.Length + 1));
            }

            this.OnProgressTotalChanged(new ProgressEventArgs(fileList.Count));
            this.OnProgressBarStateChanged(new ProgressBarStateChangedEventArgs(Forms.MyMainMenu.ProgressBarVisibleState.Percent));
            int counting = 0;
            this.OnProgressCurrentChanged(new ProgressEventArgs(counting));
            if (requestForm.CreateBackup)
            {
                this.OnStepChanged(new StepEventArgs(LanguageManager.GetMessageText("WorkspaceManager_BeginPSO2WorkSpaceBackup", "Backing up datas")));
                SharpCompress.Writers.Zip.ZipWriterOptions asdasd = new SharpCompress.Writers.Zip.ZipWriterOptions(requestForm.CompressionType);
                asdasd.LeaveStreamOpen = false;
                if (requestForm.CompressionType == SharpCompress.Common.CompressionType.Deflate)
                    asdasd.DeflateCompressionLevel = requestForm.CompressionLevel;
                using (FileStream fs = new FileStream(requestForm.BackupPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                using (SharpCompress.Writers.Zip.ZipWriter zw = new SharpCompress.Writers.Zip.ZipWriter(fs, asdasd))
                {
                    foreach (var _keypair in fileList)
                    {
                        if (this.bWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                        counting++;
                        this.OnProgressCurrentChanged(new ProgressEventArgs(counting));
                        using (FileStream fss = File.OpenRead(_keypair.Key))
                            zw.Write(_keypair.Value, fss, File.GetLastWriteTime(_keypair.Key));
                    }                  
                }
            }
            if (!this.bWorker.CancellationPending)
            {
                counting = 0;
                this.OnStepChanged(new StepEventArgs(LanguageManager.GetMessageText("WorkspaceManager_BeginPSO2WorkSpaceCleanup", "Cleaning up datas")));
                foreach (string fullpath in fileList.Keys)
                {
                    if (this.bWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    if (fullpath.EndsWith("version.ver", StringComparison.OrdinalIgnoreCase) || fullpath.EndsWith("precede.txt", StringComparison.OrdinalIgnoreCase))
                    { }
                    else
                        File.Delete(fullpath);
                    counting++;
                    this.OnProgressCurrentChanged(new ProgressEventArgs(counting));
                }
                foreach (string folderpath in Directory.EnumerateDirectories(DefaultValues.Directory.DocumentWorkSpace, "*", SearchOption.TopDirectoryOnly))
                    if (Leayal.IO.DirectoryHelper.IsFolderEmpty(folderpath))
                        Directory.Delete(folderpath, true);
            }
        }
        public bool IsBusy => this.bWorker.IsBusy;

        private bool disposed;
        public void Dispose()
        {
            if (this.disposed) return;
            this.disposed = true;
            this.bWorker.Dispose();
        }

        public void CancelAsync()
        {
            if (this.bWorker.IsBusy)
                this.bWorker.CancelAsync();
        }

        public event EventHandler<HandledExceptionEventArgs> HandledException;
        protected virtual void OnHandledException(HandledExceptionEventArgs e)
        {
            if (this.HandledException != null)
                this.synccontext?.Post(new SendOrPostCallback(delegate { this.HandledException.Invoke(this, e); }), null);
        }

        public event EventHandler CleanupStarted;
        protected virtual void OnCleanupStarted(EventArgs e)
        {
            if (this.CleanupStarted != null)
                this.synccontext?.Post(new SendOrPostCallback(delegate { this.CleanupStarted.Invoke(this, e); }), null);
        }

        public event RunWorkerCompletedEventHandler CleanupFinished;
        protected virtual void OnCleanupFinished(RunWorkerCompletedEventArgs e)
        {
            if (this.CleanupFinished != null)
                this.synccontext?.Post(new SendOrPostCallback(delegate { this.CleanupFinished.Invoke(this, e); }), null);
        }

        public event EventHandler<StepEventArgs> StepChanged;
        protected virtual void OnStepChanged(StepEventArgs e)
        {
            if (this.StepChanged != null)
                this.synccontext?.Post(new SendOrPostCallback(delegate { this.StepChanged.Invoke(this, e); }), null);
        }

        public event EventHandler<ProgressEventArgs> ProgressTotalChanged;
        protected virtual void OnProgressTotalChanged(ProgressEventArgs e)
        {
            if (this.ProgressTotalChanged != null)
                this.synccontext?.Post(new SendOrPostCallback(delegate { this.ProgressTotalChanged.Invoke(this, e); }), null);
        }

        public event EventHandler<ProgressEventArgs> ProgressCurrentChanged;
        protected virtual void OnProgressCurrentChanged(ProgressEventArgs e)
        {
            if (this.ProgressCurrentChanged != null)
                this.synccontext?.Post(new SendOrPostCallback(delegate { this.ProgressCurrentChanged.Invoke(this, e); }), null);
        }

        public event EventHandler<ProgressBarStateChangedEventArgs> ProgressBarStateChanged;
        protected void OnProgressBarStateChanged(ProgressBarStateChangedEventArgs e)
        {
            if (this.ProgressBarStateChanged != null)
                this.synccontext?.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressBarStateChanged.Invoke(this, e); }), null);
        }
    }
}
