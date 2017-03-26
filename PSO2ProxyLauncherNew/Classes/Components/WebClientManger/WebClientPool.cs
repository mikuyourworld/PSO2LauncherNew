using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.Components.WebClientManger
{
    internal sealed class WebClientPool : IDisposable
    {
        private static WebClientPool defaultInstance = new WebClientPool();
        public static WebClientPool Instance
        {
            get
            {
                return defaultInstance;
            }
        }

        public static System.Threading.SynchronizationContext SynchronizationContext { get { return Instance._SynchronizationContext; } }

        public System.Threading.SynchronizationContext _SynchronizationContext { get; }

        public Dictionary<string, WebClientCollection> dict_WebClientPool { get; private set; }

        public WebClientPool()
        {
            this._SynchronizationContext = System.Threading.SynchronizationContext.Current;
            this.dict_WebClientPool = new Dictionary<string, WebClientCollection>();
        }

        public static CustomWebClient GetWebClient_AIDA(bool exclusive = false)
        {
            return GetWebClient("aida.moe", Infos.DefaultValues.AIDA.Web.UserAgent, exclusive);
        }

        public static CustomWebClient GetWebClient_PSO2Download(bool exclusive = false)
        {
            return GetWebClient("download.pso2.jp", PSO2.DefaultValues.Web.UserAgent, exclusive);
        }

        public static CustomWebClient GetWebClient(string host, bool exclusive = false)
        {
            return GetWebClient(host, 10000, exclusive);
        }

        public static CustomWebClient GetWebClient(string host, string userAgent, bool exclusive = false)
        {
            return GetWebClient(host, 10000, userAgent, exclusive);
        }

        public static CustomWebClient GetWebClient(string host, double lifeTime, bool exclusive = false)
        {
            return GetWebClient(host, lifeTime, string.Empty, exclusive);
        }

        public static CustomWebClient GetWebClient(string host, double lifeTime, string userAgent, bool exclusive = false)
        {
            if (host.IndexOf(@"://") == -1)
                host = "http://" + host;
            return GetWebClient(new Uri(host), lifeTime, userAgent, 5000, exclusive);
        }

        public static CustomWebClient GetWebClient(Uri host, bool exclusive = false)
        {
            return GetWebClient(host, 10000, string.Empty, 5000, exclusive);
        }

        public static CustomWebClient GetWebClient(Uri host, double lifeTime, string userAgent, int iTimeOut, bool exclusive)
        {
            if (!Instance.dict_WebClientPool.ContainsKey(host.Host))
                Instance.dict_WebClientPool.Add(host.Host, new WebClientCollection(host.Host));
            CustomWebClient result;
            if (exclusive)
                result = Instance.dict_WebClientPool[host.Host].GetExclusive();
            else
                result = Instance.dict_WebClientPool[host.Host].GetFree(lifeTime);
            if (!string.IsNullOrEmpty(userAgent))
                result.UserAgent = userAgent;
            result.TimeOut = iTimeOut;
            return result;
        }

        public void StopAll()
        {
            foreach (var webClientNode in this.dict_WebClientPool.Values)
            {
                webClientNode.StopAll();
            }
        }

        public void Dispose()
        {
            this.ForceCleanUp();
        }

        public void ForceCleanUp()
        {
            foreach (var webClientNode in this.dict_WebClientPool.Values)
            {
                webClientNode.Dispose();
            }
            this.dict_WebClientPool.Clear();
        }
    }
}
