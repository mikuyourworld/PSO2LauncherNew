using System;
using PSO2ProxyLauncherNew.Classes.Events;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;

namespace PSO2ProxyLauncherNew.Classes.Components.Proxifier
{
    class ProxifierController
    {
        public static string ProxifierPath
        {
            get
            {
                string result = string.Empty;
                string _ProxifierPath = MySettings.ProxifierPath;
                if (!string.IsNullOrWhiteSpace(_ProxifierPath))
                {
                    if (File.Exists(Infos.CommonMethods.PathConcat(_ProxifierPath, Infos.DefaultValues.MyInfo.Filename.ProxifierExecutable)))
                        return _ProxifierPath;
                    else
                    {
                        _ProxifierPath = Infos.CommonMethods.PathConcat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Proxifier");
                        if (File.Exists(Infos.CommonMethods.PathConcat(_ProxifierPath, Infos.DefaultValues.MyInfo.Filename.ProxifierExecutable)))
                        {
                            if (string.IsNullOrWhiteSpace(MySettings.ProxifierPath))
                                MySettings.ProxifierPath = _ProxifierPath;
                            return _ProxifierPath;
                        }
                        else
                        {
                            string regPath = Path.Combine("SOFTWARE", "Microsoft", "Windows", "CurrentVersion", "Uninstall", "Proxifier_is1");
                            if (Environment.Is64BitProcess)
                                regPath = Path.Combine("SOFTWARE", "WOW6432Node", "Microsoft", "Windows", "CurrentVersion", "Uninstall", "Proxifier_is1");
                            RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath, false);
                            if (key != null)
                            {
                                _ProxifierPath = key.GetValue("InstallLocation", string.Empty) as string;
                                if (!string.IsNullOrWhiteSpace(_ProxifierPath))
                                    if (File.Exists(Infos.CommonMethods.PathConcat(_ProxifierPath, Infos.DefaultValues.MyInfo.Filename.ProxifierExecutable)))
                                        if (string.IsNullOrWhiteSpace(MySettings.ProxifierPath))
                                            MySettings.ProxifierPath = _ProxifierPath;
                                key.Close();
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(_ProxifierPath))
                    result = _ProxifierPath;
                return result;
            }
        }

        public string ProxifierProfileName
        {
            get
            {
                string result = string.Empty;
                RegistryKey key = Registry.LocalMachine.OpenSubKey(Path.Combine("HKEY_CURRENT_USER", "SOFTWARE", "Initex", "Proxifier", "Settings"), false);
                if (key != null)
                {
                    string _ActiveProfile = key.GetValue("ActiveProfile", string.Empty) as string;
                    if (!string.IsNullOrWhiteSpace(_ActiveProfile))
                        if (File.Exists(Infos.CommonMethods.PathConcat(Infos.ApplicationInfo.ProxifierProfileDocument, _ActiveProfile)))
                            result = _ActiveProfile;
                    key.Close();
                }
                return result;
            }
        }

        public void LaunchProxifier()
        {
            if (!this.bWorker.IsBusy)
                this.bWorker.RunWorkerAsync();
        }

        public void LaunchProxifierWithProfile(string profilePath)
        {
            if (!this.bWorker.IsBusy)
                this.bWorker.RunWorkerAsync(profilePath);
        }

        private BackgroundWorker bWorker;

        public ProxifierController()
        {
            this.bWorker = new BackgroundWorker();
            this.bWorker.WorkerSupportsCancellation = false;
            this.bWorker.WorkerReportsProgress = false;
            this.bWorker.DoWork += this.BWorker_DoWork;
            this.bWorker.RunWorkerCompleted += this.BWorker_RunWorkerCompleted;
        }

        private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnProxifierLaunched(new ProxifierLaunchedEventArgs(e.Error));
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string profilePath = e.Argument as string;
            string _proxifierPath = Infos.CommonMethods.PathConcat(ProxifierPath, Infos.DefaultValues.MyInfo.Filename.ProxifierExecutable);
            if (File.Exists(_proxifierPath))
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = _proxifierPath;
                    if (!string.IsNullOrWhiteSpace(profilePath))
                    {
                        string newPath = Path.Combine(Infos.ApplicationInfo.ProxifierProfileDocument, Path.GetFileName(profilePath));
                        File.Copy(profilePath, newPath, true);
                        var asd = new System.Collections.Generic.List<string>();
                        asd.Add(newPath);
                        asd.Add("silent-load");
                        proc.StartInfo.Arguments = Leayal.ProcessHelper.TableStringToArgs(asd);
                    }
                }
            }
            else
                throw new FileNotFoundException("Can not find Proxifier.exe", _proxifierPath);
        }

        public event EventHandler<ProxifierLaunchedEventArgs> ProxifierLaunched;
        protected virtual void OnProxifierLaunched(ProxifierLaunchedEventArgs e)
        {
            if (this.ProxifierLaunched != null)
                WebClientManger.WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.ProxifierLaunched.Invoke(this, e); }), null);
        }
    }
}
