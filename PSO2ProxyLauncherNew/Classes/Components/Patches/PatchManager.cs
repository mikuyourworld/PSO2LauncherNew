using PSO2ProxyLauncherNew.Classes.Events;
using System;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;

namespace PSO2ProxyLauncherNew.Classes.Components.Patches
{
    internal abstract class PatchManager
    {
        public PatchManager()
        {
            this.IsBusy = false;
            this.syncContext = System.Threading.SynchronizationContext.Current;
            this.bworker_RestoreBackup = new BackgroundWorker();
            this.bworker_RestoreBackup.WorkerSupportsCancellation = true;
            this.bworker_RestoreBackup.WorkerReportsProgress = false;
            this.bWorker_install = new BackgroundWorker();
            this.bWorker_install.WorkerSupportsCancellation = true;
            this.bWorker_install.WorkerReportsProgress = false;
            this.bWorker_uninstall = new BackgroundWorker();
            this.bWorker_uninstall.WorkerSupportsCancellation = true;
            this.bWorker_uninstall.WorkerReportsProgress = false;
            this.myWebClient_ForAIDA = new ExtendedWebClient();
            this.myWebClient_ForAIDA.UserAgent = Infos.DefaultValues.AIDA.Web.UserAgent;
            this.myWebClient_ForPSO2 = new ExtendedWebClient();
            this.myWebClient_ForPSO2.UserAgent = PSO2.DefaultValues.Web.UserAgent;
        }
        protected System.Threading.SynchronizationContext syncContext { get; set; }
        protected BackgroundWorker bWorker_install;
        protected BackgroundWorker bWorker_uninstall;
        protected BackgroundWorker bworker_RestoreBackup;
        protected ExtendedWebClient myWebClient_ForAIDA;
        protected ExtendedWebClient myWebClient_ForPSO2;
        public string VersionString { get; internal set; }
        public bool IsBusy { get; internal set; }
        public bool IsInstalled
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VersionString))
                    return false;
                else if (VersionString.ToLower() == Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString.ToLower())
                    return false;
                else
                    return true;
            }
        }
        public virtual void InstallPatch() { this.OnHandledException(new HandledExceptionEventArgs(new System.NotImplementedException())); }
        public virtual void UninstallPatch() { this.OnHandledException(new HandledExceptionEventArgs(new System.NotImplementedException())); }
        public virtual void ReinstallPatch() { this.OnHandledException(new HandledExceptionEventArgs(new System.NotImplementedException())); }
        public virtual void CheckUpdate() { this.OnHandledException(new HandledExceptionEventArgs(new System.NotImplementedException())); }
        public virtual void RestoreBackup() { this.OnHandledException(new HandledExceptionEventArgs(new System.NotImplementedException())); }


        #region "Events"
        public event EventHandler<StepEventArgs> CurrentStepChanged;
        protected void OnCurrentStepChanged(StepEventArgs e)
        {
            if (this.CurrentStepChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentStepChanged.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressEventArgs> CurrentProgressChanged;
        protected void OnCurrentProgressChanged(ProgressEventArgs e)
        {
            if (this.CurrentProgressChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentProgressChanged.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressEventArgs> CurrentTotalProgressChanged;
        protected void OnCurrentTotalProgressChanged(ProgressEventArgs e)
        {
            if (this.CurrentTotalProgressChanged != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CurrentTotalProgressChanged.Invoke(this, e); }), null);
        }

        /*public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        protected void OnProgressChanged(ProgressChangedEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressChanged?.Invoke(this, e); }), null);
        }*/
        
        public event EventHandler<PatchNotificationEventArgs> PatchNotification;
        protected void OnPatchNotification(PatchNotificationEventArgs e)
        {
            if (this.PatchNotification != null)
                this.syncContext?.Send(new System.Threading.SendOrPostCallback(delegate { this.PatchNotification.Invoke(this, e); }), null);
        }
        public event EventHandler<PatchFinishedEventArgs> PatchInstalled;
        protected void OnPatchInstalled(PatchFinishedEventArgs e)
        {
            this.IsBusy = false;
            if (this.PatchInstalled != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PatchInstalled.Invoke(this, e); }), null);
        }
        public event EventHandler<PatchFinishedEventArgs> PatchUninstalled;
        protected void OnPatchUninstalled(PatchFinishedEventArgs e)
        {
            this.IsBusy = false;
            if (this.PatchUninstalled != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PatchUninstalled.Invoke(this, e); }), null);
        }
        public event EventHandler<HandledExceptionEventArgs> HandledException;
        protected void OnHandledException(HandledExceptionEventArgs e)
        {
            if (this.HandledException != null)
                this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.HandledException.Invoke(this, e); }), null);
        }
        public event EventHandler<ProgressBarStateChangedEventArgs> ProgressBarStateChanged;
        protected void OnProgressBarStateChanged(ProgressBarStateChangedEventArgs e)
        {
            if (this.ProgressBarStateChanged != null)
                this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressBarStateChanged.Invoke(this, e); }), null);
        }
        #endregion

        #region "Internal Classes"
        internal class TransarmWorkerInfo
        {
            public bool Backup { get; }
            public object Params { get; }
            public System.DateTime Date { get; }
            public string Path { get; }
            public TransarmWorkerInfo(object p, string local, System.DateTime da, bool createBackup)
            {
                this.Params = p;
                this.Path = local;
                this.Date = da;
                this.Backup = createBackup;
            }
        }

        internal class WorkerInfo
        {
            public string Step { get; }
            public bool Backup { get; }
            public object Params { get; }
            public System.Uri URL { get; }
            public string Path { get; }
            public WorkerInfo(string step, object p, string local, System.Uri link, bool createBackup)
            {
                this.Params = p;
                this.Path = local;
                this.URL = link;
                this.Backup = createBackup;
                this.Step = step;
            }
            public WorkerInfo(string step, object p, string local) : this(step, p, local, null, true) { }
            public WorkerInfo(string step, object p) : this(step, p, string.Empty, null, true) { }
            public WorkerInfo(string step) : this(step, null) { }
        }

        public class PatchNotificationEventArgs : System.EventArgs
        {
            public bool NewPatch { get; private set; }
            public string NewPatchVersion { get; private set; }
            public string CurrentPatchVersion { get; private set; }
            public bool Continue { get; set; }
            public bool Backup { get; set; }
            public PatchNotificationEventArgs(bool isnew, string newpatchstring, string currentPatch, bool createBackup)
            {
                this.NewPatch = isnew;
                this.NewPatchVersion = newpatchstring;
                this.CurrentPatchVersion = currentPatch;
                this.Continue = true;
                this.Backup = createBackup;
            }

            public PatchNotificationEventArgs(bool isnew, string newpatchstring, string currentPatch) : this(isnew, newpatchstring, currentPatch, true) { }

            public PatchNotificationEventArgs(string currentPatch) : this(false, string.Empty, currentPatch, false) { }
        }

        public class PatchFinishedEventArgs : System.EventArgs
        {
            public bool Success { get; }
            public string PatchVersion { get; }
            public PatchFinishedEventArgs(bool yes, string currentPatch)
            {
                this.Success = yes;
                this.PatchVersion = currentPatch;
            }

            public PatchFinishedEventArgs(string currentPatch) : this(true, currentPatch) { }
        }

        internal class InstallingMeta
        {
            public bool Force { get; }
            public bool Backup { get; }
            public string NewVersionString { get; }
            public string InstallParamString { get; }
            public System.DateTime NewVersionDate { get; }
            public InstallingMeta(bool bBackup, bool bForce) : this(bBackup, bForce, null) { }
            public InstallingMeta(bool bBackup, bool bForce, System.DateTime dt) : this(bBackup, bForce, null)
            {
                this.NewVersionString = dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + dt.Year.ToString();
                this.InstallParamString = dt.Month.ToString() + "-" + dt.Day.ToString() + "-" + dt.Year.ToString();
                this.NewVersionDate = dt;
            }
            public InstallingMeta(bool bBackup, bool bForce, string newversion)
            {
                this.Force = bForce;
                this.Backup = bBackup;
                this.NewVersionString = newversion;
            }
        }

        internal class WebClientInstallingMetaWrapper
        {
            public InstallingMeta Meta { get; }
            public short Step { get; }
            public WebClientInstallingMetaWrapper(short sStep, InstallingMeta oMeta)
            {
                this.Step = sStep;
                this.Meta = oMeta;
            }
        }
        #endregion
    }
}
