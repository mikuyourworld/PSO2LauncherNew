using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    public enum Task : short
    {
        None,
        EnglishPatch,
        LargeFilesPatch,
        StoryPatch
    }
    class PSO2Controller
    {
        private System.Threading.SynchronizationContext syncContext;
        private Patches.EnglishPatchManager englishManager;
        private Patches.StoryPatchManager storyManager;
        private Patches.LargeFilesPatchManager largefilesManager;
        public bool IsBusy { get; private set; }
        public Task CurrentTask { get; private set; }
        public PSO2Controller()
        {
            this.IsBusy = false;
            this.CurrentTask = Task.None;
            this.syncContext = System.Threading.SynchronizationContext.Current;
            this.englishManager = CreateEnglishManager();
            this.largefilesManager = CreateLargeFilesManager();
            this.storyManager = CreateStoryManager();
            this.OnEnglishPatchNotify(new PatchNotifyEventArgs(this.englishManager.VersionString));
            //this.largefilesManager = CreateLargeFilesManager();
            //this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(this.largefilesManager.VersionString));
        }

        #region "English Patch"
        private Patches.EnglishPatchManager CreateEnglishManager()
        {
            Patches.EnglishPatchManager result = new Patches.EnglishPatchManager();
            result.ProgressChanged += EnglishPatchManager_ProgressChanged;
            result.PatchInstalled += EnglishPatchManager_PatchInstalled;
            result.PatchUninstalled += EnglishPatchManager_PatchUninstalled;
            result.HandledException += EnglishPatchManager_HandledException;
            return result;
        }

        public void InstallEnglishPatch()
        {
            this.CurrentTask = Task.EnglishPatch;
            this.englishManager.InstallPatch();
        }

        public void UninstallEnglishPatch()
        {
            this.CurrentTask = Task.EnglishPatch;
            this.englishManager.UninstallPatch();
        }

        private void EnglishPatchManager_HandledException(object sender, Infos.HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask));
            if (this.CurrentTask == Task.EnglishPatch)
                this.OnEnglishPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.EnglishVersion));
            this.CurrentTask = Task.None;
        }

        private void EnglishPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs("[English Patch] " + LanguageManager.GetMessageText("UninstalledEnglishPatch", "English Patch has been uninstalled successfully"), true));
                MySettings.Patches.EnglishVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnEnglishPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
        }

        private void EnglishPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.EnglishVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs("[English Patch] " + LanguageManager.GetMessageText("InstalledEnglishPatch", "English Patch has been installed successfully"), true));
                this.OnEnglishPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }
        }

        private void EnglishPatchManager_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
                this.OnStepChanged(new StepChangedEventArgs("[English Patch] " + e.UserState as string));
            else if (e.ProgressPercentage == 1)
                this.OnProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, null));
            else if (e.ProgressPercentage == 2)
                this.OnProgressBarNotify(new VisibleNotifyEventArgs((bool)e.UserState));
            else if (e.ProgressPercentage == 3)
                this.OnRingNotify(new VisibleNotifyEventArgs((bool)e.UserState));
        }
        #endregion

        #region "LargeFiles Patch"
        private Patches.LargeFilesPatchManager CreateLargeFilesManager()
        {
            Patches.LargeFilesPatchManager result = new Patches.LargeFilesPatchManager();
            result.ProgressChanged += LargeFilesPatchManager_ProgressChanged;
            result.PatchInstalled += LargeFilesPatchManager_PatchInstalled;
            result.PatchUninstalled += LargeFilesPatchManager_PatchUninstalled;
            result.HandledException += LargeFilesPatchManager_HandledException;
            return result;
        }

        public void InstallLargeFilesPatch()
        {
            this.CurrentTask = Task.LargeFilesPatch;
            this.largefilesManager.InstallPatch();
        }

        public void UninstallLargeFilesPatch()
        {
            this.CurrentTask = Task.LargeFilesPatch;
            this.largefilesManager.UninstallPatch();
        }

        private void LargeFilesPatchManager_HandledException(object sender, Infos.HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask));
            if (this.CurrentTask == Task.LargeFilesPatch)
                this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.LargeFilesVersion));
            this.CurrentTask = Task.None;
        }

        private void LargeFilesPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs("[LargeFiles Patch] " + LanguageManager.GetMessageText("UninstalledLargeFilesPatch", "LargeFiles Patch has been uninstalled successfully"), true));
                MySettings.Patches.LargeFilesVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
        }

        private void LargeFilesPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.LargeFilesVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs("[LargeFiles Patch] " + LanguageManager.GetMessageText("InstalledLargeFilesPatch", "LargeFiles Patch has been installed successfully"), true));
                this.OnLargeFilesPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }
        }

        private void LargeFilesPatchManager_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
                this.OnStepChanged(new StepChangedEventArgs("[LargeFiles Patch] " + e.UserState as string));
            else if (e.ProgressPercentage == 1)
                this.OnProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, null));
            else if (e.ProgressPercentage == 2)
                this.OnProgressBarNotify(new VisibleNotifyEventArgs((bool)e.UserState));
            else if (e.ProgressPercentage == 3)
                this.OnRingNotify(new VisibleNotifyEventArgs((bool)e.UserState));
        }
        #endregion

        #region "Story Patch"
        private Patches.StoryPatchManager CreateStoryManager()
        {
            Patches.StoryPatchManager result = new Patches.StoryPatchManager();
            result.ProgressChanged += StoryPatchManager_ProgressChanged;
            result.PatchInstalled += StoryPatchManager_PatchInstalled;
            result.PatchUninstalled += StoryPatchManager_PatchUninstalled;
            result.HandledException += StoryPatchManager_HandledException;
            return result;
        }

        public void InstallStoryPatch()
        {
            this.CurrentTask = Task.StoryPatch;
            this.storyManager.InstallPatch();
        }

        public void UninstallStoryPatch()
        {
            this.CurrentTask = Task.StoryPatch;
            this.storyManager.UninstallPatch();
        }

        private void StoryPatchManager_HandledException(object sender, Infos.HandledExceptionEventArgs e)
        {
            this.OnHandledException(new PSO2HandledExceptionEventArgs(e.Error, this.CurrentTask));
            if (this.CurrentTask == Task.StoryPatch)
                this.OnStoryPatchNotify(new PatchNotifyEventArgs(MySettings.Patches.StoryVersion));
            this.CurrentTask = Task.None;
        }

        private void StoryPatchManager_PatchUninstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                this.OnStepChanged(new StepChangedEventArgs("[Story Patch] " + LanguageManager.GetMessageText("UninstalledStoryPatch", "Story Patch has been uninstalled successfully"), true));
                MySettings.Patches.StoryVersion = Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString;
                this.OnStoryPatchNotify(new PatchNotifyEventArgs(Infos.DefaultValues.AIDA.Tweaker.Registries.NoPatchString));
            }
        }

        private void StoryPatchManager_PatchInstalled(object sender, Patches.PatchManager.PatchFinishedEventArgs e)
        {
            if (e.Success)
            {
                MySettings.Patches.StoryVersion = e.PatchVersion;
                this.OnStepChanged(new StepChangedEventArgs("[Story Patch] " + LanguageManager.GetMessageText("InstalledStoryPatch", "Story Patch has been installed successfully"), true));
                this.OnStoryPatchNotify(new PatchNotifyEventArgs(e.PatchVersion));
            }
        }

        private void StoryPatchManager_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
                this.OnStepChanged(new StepChangedEventArgs("[Story Patch] " + e.UserState as string));
            else if (e.ProgressPercentage == 1)
                this.OnProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, null));
            else if (e.ProgressPercentage == 2)
                this.OnProgressBarNotify(new VisibleNotifyEventArgs((bool)e.UserState));
            else if (e.ProgressPercentage == 3)
                this.OnRingNotify(new VisibleNotifyEventArgs((bool)e.UserState));
        }
        #endregion

        #region "Properties"
        public string EnglishPatchVersion { get { return this.englishManager.VersionString; } }
        public bool IsEnglishPatchInstalled { get { return this.englishManager.IsInstalled; } }
        public string LargeFilesPatchVersion { get { return this.largefilesManager.VersionString; } }
        public bool IsLargeFilesPatchInstalled { get { return this.largefilesManager.IsInstalled; } }
        public string StoryPatchVersion { get { return this.storyManager.VersionString; } }
        public bool IsStoryPatchInstalled { get { return this.storyManager.IsInstalled; } }
        #endregion

        #region "Events"
        public delegate void PatchNotifyEventHandler(object sender, PatchNotifyEventArgs e);
        public event PatchNotifyEventHandler EnglishPatchNotify;
        protected void OnEnglishPatchNotify(PatchNotifyEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.EnglishPatchNotify?.Invoke(this, e); }), null);
        }
        public event PatchNotifyEventHandler LargeFilesPatchNotify;
        protected void OnLargeFilesPatchNotify(PatchNotifyEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.LargeFilesPatchNotify?.Invoke(this, e); }), null);
        }
        public event PatchNotifyEventHandler StoryPatchNotify;
        protected void OnStoryPatchNotify(PatchNotifyEventArgs e)
        {
            this.syncContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.StoryPatchNotify?.Invoke(this, e); }), null);
        }
        public event ProgressChangedEventHandler ProgressChanged;
        protected void OnProgressChanged(ProgressChangedEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressChanged?.Invoke(this, e); }), null);
        }
        public delegate void StepChangedEventHandler(object sender, StepChangedEventArgs e);
        public event StepChangedEventHandler StepChanged;
        protected void OnStepChanged(StepChangedEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.StepChanged?.Invoke(this, e); }), null);
        }
        public delegate void HandledExceptionEventHandler(object sender, PSO2HandledExceptionEventArgs e);
        public event HandledExceptionEventHandler HandledException;
        protected void OnHandledException(PSO2HandledExceptionEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.HandledException?.Invoke(this, e); }), null);
        }
        public delegate void RingNotifyEventHandler(object sender, VisibleNotifyEventArgs e);
        public event RingNotifyEventHandler RingNotify;
        protected void OnRingNotify(VisibleNotifyEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.RingNotify?.Invoke(this, e); }), null);
        }
        public delegate void ProgressBarNotifyEventHandler(object sender, VisibleNotifyEventArgs e);
        public event ProgressBarNotifyEventHandler ProgressBarNotify;
        protected void OnProgressBarNotify(VisibleNotifyEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.ProgressBarNotify?.Invoke(this, e); }), null);
        }
        #endregion

        #region "Internal Classes"
        public class PatchNotifyEventArgs : EventArgs
        {
            public string PatchVer { get; }

            public PatchNotifyEventArgs(string verString) : base()
            {
                this.PatchVer = verString;
            }
        }
        public class VisibleNotifyEventArgs : EventArgs
        {
            public bool Visible { get; }

            public VisibleNotifyEventArgs(bool mybool) : base()
            {
                this.Visible = mybool;
            }
        }
        public class StepChangedEventArgs : EventArgs
        {
            public string Step { get; }

            public bool Final { get; }

            public StepChangedEventArgs(string msg) : this(msg, false) { }

            public StepChangedEventArgs(string msg, bool isFinal) : base()
            {
                this.Step = msg;
                this.Final = isFinal;
            }
        }

        public class PSO2HandledExceptionEventArgs : EventArgs
        {
            public Task LastTask { get; }
            public Exception Error { get; }
            public object UserToken { get; }
            public PSO2HandledExceptionEventArgs(Exception ex, Task task, object token) : base()
            {
                this.Error = ex;
                this.LastTask = task;
                this.UserToken = token;
            }

            public PSO2HandledExceptionEventArgs(Exception ex, Task task) : this(ex, task, null) { }

            public override string ToString()
            {
                return this.Error.Message;
            }
        }
        #endregion
    }
}
