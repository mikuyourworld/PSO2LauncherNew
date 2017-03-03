using System;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using PSO2ProxyLauncherNew.Classes.Events;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Proxy
{
    class PSO2ProxyInstaller
    {
        private static System.Collections.Generic.Dictionary<string, string> _urlinside;
        public static System.Collections.Generic.Dictionary<string, string> URLInside
        {
            get
            {
                if (_urlinside == null)
                {
                    //http://cloud02.cyberkitsune.net:8080/config.json
                    _urlinside = new System.Collections.Generic.Dictionary<string, string>();
                    _urlinside.Add("http://cloud02.cyberkitsune.net:8080/config.json", "http://alam.srb2.org/PSO2/public_proxy/config-alt.json");
                }
                return _urlinside;
            }
        }
        private static PSO2ProxyInstaller _instance;
        public static PSO2ProxyInstaller Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PSO2ProxyInstaller();
                return _instance;
            }
        }

        private ExtendedWebClient myWebClient;
        private BackgroundWorker bWorker;
        public PSO2ProxyInstaller()
        {
            this.myWebClient = new ExtendedWebClient();
            this.myWebClient.DownloadStringCompleted += MyWebClient_DownloadStringCompleted;
            this.bWorker = new BackgroundWorker();
            this.bWorker.WorkerReportsProgress = false;
            this.bWorker.WorkerSupportsCancellation = false;
            this.bWorker.DoWork += BWorker_DoWork;
            this.bWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
            this._isBusy = false;
        }

        private void MyWebClient_DownloadStringCompleted(object sender, ExtendedWebClient.DownloadStringFinishedEventArgs e)
        {
            if (e.Error != null)
            {
                this._isBusy = false;
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
            }
            else
                this.bWorker.RunWorkerAsync(new InstallMeta(e.Result));
        }

        public bool _isBusy;
        public bool IsBusy { get { return this._isBusy; } }
        public bool IsProxyInstalled { get; }

        private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this._isBusy = false;
            if (e.Error != null)
                this.OnHandledException(new HandledExceptionEventArgs(e.Error));
            else
            {
                if (e.Result is UninstallMeta)
                    this.OnProxyUninstalled(new ProxyUninstalledEventArgs());
                else
                {
                    PSO2ProxyConfiguration resultConfig = e.Result as PSO2ProxyConfiguration;
                    if (resultConfig != null)
                        this.OnProxyInstalled(new ProxyInstalledEventArgs(resultConfig));
                }
            }
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            InstallMeta _installmeta = e.Argument as InstallMeta;
            if (_installmeta != null)
            {
                JObject jo = JObject.Parse(_installmeta.Result);
                PSO2ProxyConfigurationJsonObject pso2proxyjo = jo.ToObject<PSO2ProxyConfigurationJsonObject>();
                PSO2ProxyConfiguration pso2proxyConfig = pso2proxyjo.ToPSO2ProxyConfiguration();
                if (pso2proxyConfig.Version is Versions.Telepipe)
                    Versions.Telepipe.Install(pso2proxyConfig);
                else if (pso2proxyConfig.Version is Versions.PSO2Proxy)
                    Versions.PSO2Proxy.Install(pso2proxyConfig);
                e.Result = pso2proxyConfig;
            }
            else
            {
                UninstallMeta _uninstallmeta = e.Argument as UninstallMeta;
                if (_uninstallmeta != null)
                {
                    Versions.PSO2Proxy.Uninstall();
                    e.Result = _uninstallmeta;
                }
            }
        }

        public void Install(Uri proxy)
        {
            if (!this.IsBusy && proxy != null)
            {
                this._isBusy = true;
                this.myWebClient.DownloadStringAsync(proxy, proxy);
            }
        }

        public void Uninstall()
        {
            if (!this.IsBusy)
            {
                this._isBusy = true;
                this.bWorker.RunWorkerAsync(new UninstallMeta());
            }
        }

        public event EventHandler<HandledExceptionEventArgs> HandledException;
        protected virtual void OnHandledException(HandledExceptionEventArgs e)
        {
            if (this.HandledException != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.HandledException.Invoke(this, e); }), null);
        }

        public event EventHandler<ProxyInstalledEventArgs> ProxyInstalled;
        protected virtual void OnProxyInstalled(ProxyInstalledEventArgs e)
        {
            if (this.ProxyInstalled != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.ProxyInstalled.Invoke(this, e); }), null);
        }

        public event EventHandler<ProxyUninstalledEventArgs> ProxyUninstalled;
        protected virtual void OnProxyUninstalled(ProxyUninstalledEventArgs e)
        {
            if (this.ProxyUninstalled != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.ProxyUninstalled.Invoke(this, e); }), null);
        }

        private class InstallMeta
        {
            public string Result { get; }
            public InstallMeta(string _result)
            {
                this.Result = _result;
                //this.Proxy = _proxy;
            }
        }

        private class UninstallMeta
        {
            public UninstallMeta() { }
        }
    }
}
