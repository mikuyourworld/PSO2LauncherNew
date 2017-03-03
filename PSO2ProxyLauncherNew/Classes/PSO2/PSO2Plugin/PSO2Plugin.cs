using System.Collections.ObjectModel;
using System.Collections.Generic;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using PSO2ProxyLauncherNew.Classes.Events;
using System;
using System.ComponentModel;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Plugin
{
    class PSO2PluginPath
    {
        public string EnabledPath { get; }
        public string DisabledPath { get; }
        public PSO2PluginPath(string enabled, string disabled)
        {
            this.EnabledPath = enabled;
            this.DisabledPath = disabled;
        }

        public PSO2PluginPath(string enabled) : this(enabled, null) { }
    }

    class PSO2Plugin
    {
        public enum Status : short
        {

            NotExisted,
            Enabled,
            Disabled,
            EnabledInvalid,
            DisabledInvalid
        }
        private BackgroundWorker BWorker;
        public string PluginID { get; }
        public string Name { get; }
        public string Filename { get; }
        public string Description { get; }
        public string Author { get; }
        public string Homepage { get; }
        public string Version { get; }
        public string MD5Hash { get; }
        public bool Toggleable { get; }
        public PSO2Plugin(string _id, string _name, string _filename, string _desc, string _author, string _homepage, string _version, string _md5, string _toggleable)
        {
            this.PluginID = _id;
            this.Name = _name;
            this.Filename = _filename;
            this.Description = _desc;
            this.Author = _author;
            this.Homepage = _homepage;
            this.Version = _version;
            this.MD5Hash = _md5;
            if (string.IsNullOrWhiteSpace(_toggleable))
                this.Toggleable = true;
            else
            {
                _toggleable = _toggleable.ToLower();
                if (_toggleable == "yes" || _toggleable == "true")
                    this.Toggleable = true;
                else if (_toggleable == "no" || _toggleable == "false")
                    this.Toggleable = false;
                else
                    this.Toggleable = true;
            }
            this.BWorker = new BackgroundWorker();
            this.BWorker.DoWork += BWorker_DoWork;
            this.BWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
            this.BWorker.WorkerReportsProgress = false;
            this.BWorker.WorkerSupportsCancellation = false;
        }

        //public PSO2Plugin(string _id, string _name, string _filename, string _desc, string _author, string _version, string _md5, string _toggleable) : this(_id, _name, _filename, _desc, _author, string.Empty, _version, _md5, _toggleable) { }
        public bool Enabled
        {
            get { return this.CheckIfPluginIsEnabled(); }
            set
            {
                if (this.Toggleable && !this.BWorker.IsBusy)
                    this.BWorker.RunWorkerAsync(value);
            }
        }

        public PSO2PluginPath FullPath
        {
            get
            {
                if (this.Toggleable)
                    return new PSO2PluginPath(Infos.CommonMethods.PathConcat(PSO2.DefaultValues.Directory.PSO2Plugins, this.Filename), Infos.CommonMethods.PathConcat(PSO2.DefaultValues.Directory.PSO2PluginsDisabled, this.Filename));
                else
                    return new PSO2PluginPath(Infos.CommonMethods.PathConcat(MySettings.PSO2Dir, this.Filename));
            }
        }

        private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                this.OnHandledException(new Events.HandledExceptionEventArgs(e.Error));
            else
                this.OnEnableChanged(System.EventArgs.Empty);
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.SetPluginIsEnabled((bool)e.Argument);
        }

        public event EventHandler<HandledExceptionEventArgs> HandledException;
        protected virtual void OnHandledException(HandledExceptionEventArgs e)
        {
            if (this.HandledException != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.HandledException.Invoke(this, e); }), null);
        }

        public event EventHandler EnableChanged;
        protected virtual void OnEnableChanged(EventArgs e)
        {
            if (this.EnableChanged != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.EnableChanged.Invoke(this, e); }), null);
        }

        public Status IsValid()
        {
            var myPath = this.FullPath;
            bool existDisabled = File.Exists(myPath.DisabledPath), existEnabled = File.Exists(myPath.EnabledPath);
            if (existEnabled)
            {
                if (this.MD5Hash == Infos.CommonMethods.FileToMD5Hash(myPath.EnabledPath))
                {
                    if (existDisabled)
                        try { File.Delete(myPath.DisabledPath); } catch { }
                    return Status.Enabled;
                }
                else
                {
                    if (this.MD5Hash == Infos.CommonMethods.FileToMD5Hash(myPath.DisabledPath))
                    {
                        try { File.Delete(myPath.EnabledPath); } catch { }
                        return Status.Disabled;
                    }
                    else
                        return Status.EnabledInvalid;
                }
            }
            else
            {
                if (existDisabled)
                {
                    if (this.MD5Hash == Infos.CommonMethods.FileToMD5Hash(myPath.DisabledPath))
                        return Status.Disabled;
                    else
                        return Status.DisabledInvalid;
                }
                else
                    return Status.NotExisted;
            }
        }

        public void Delete()
        {
            var laiwhgliahwg = this.FullPath;
            File.Delete(laiwhgliahwg.EnabledPath);
            if (!string.IsNullOrWhiteSpace(laiwhgliahwg.DisabledPath))
                File.Delete(laiwhgliahwg.DisabledPath);
        }

        private void SetPluginIsEnabled(bool val)
        {
            var myPath = this.FullPath;
            if (val)
            {
                if (File.Exists(myPath.DisabledPath))
                {
                    if (File.Exists(myPath.EnabledPath))
                    {
                        if (this.MD5Hash == Infos.CommonMethods.FileToMD5Hash(myPath.EnabledPath))
                            try { File.Delete(myPath.DisabledPath); } catch { }
                        else if (this.MD5Hash == Infos.CommonMethods.FileToMD5Hash(myPath.DisabledPath))
                        {
                            try { File.Delete(myPath.EnabledPath); } catch { }
                            File.Move(myPath.DisabledPath, myPath.EnabledPath);
                        }
                        else
                            try { File.Delete(myPath.EnabledPath); } catch { }
                    }
                    else
                        File.Move(myPath.DisabledPath, myPath.EnabledPath);
                }
            }
            else
            {
                if (File.Exists(myPath.EnabledPath))
                {
                    if (File.Exists(myPath.DisabledPath))
                    {
                        if (this.MD5Hash == Infos.CommonMethods.FileToMD5Hash(myPath.EnabledPath))
                        {
                            try { File.Delete(myPath.DisabledPath); } catch { }
                            File.Move(myPath.EnabledPath, myPath.DisabledPath);
                        }
                        else
                            try { File.Delete(myPath.EnabledPath); } catch { }
                    }
                    else
                        File.Move(myPath.EnabledPath, myPath.DisabledPath);
                }
            }
        }

        private Uri _downloadlink;
        public Uri DownloadLink
        {
            get
            {
                if (string.IsNullOrEmpty(this.PluginID))
                    return null;
                else
                {
                    string aaa = Infos.CommonMethods.URLConcat(AIDA.TweakerWebPanel.PluginURL, this.PluginID);
                    if (_downloadlink == null || _downloadlink.OriginalString != aaa)
                        _downloadlink = new Uri(aaa);
                    return _downloadlink;
                }
            }
        }

        private bool CheckIfPluginIsEnabled()
        {
            if (this.Toggleable)
            {
                if (File.Exists(Infos.CommonMethods.PathConcat(PSO2.DefaultValues.Directory.PSO2Plugins, this.Filename)))
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
    }
}
