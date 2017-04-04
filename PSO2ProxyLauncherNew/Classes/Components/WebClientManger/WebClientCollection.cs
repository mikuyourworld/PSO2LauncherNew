using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PSO2ProxyLauncherNew.Classes.Components.WebClientManger
{
    class WebClientCollection
    {
        private List<CustomWebClient> list_WebClient;
        private ConcurrentStack<CustomWebClient> list_WebClient_free;

        public string Host { get; }
        public WebClientCollection(string s_host)
        {
            this.list_WebClient = new List<CustomWebClient>();
            this.Host = s_host;
        }

        public void StopAll()
        {
            foreach (var cwc in this.list_WebClient)
            {
                if (cwc.IsBusy)
                    cwc.CancelAsync();
            }
        }

        public CustomWebClient GetFree(double lifetime, bool forceNew = false)
        {
            CustomWebClient result = null;
            if ((this.list_WebClient.Count == 0) || (forceNew))
            {
                result = new CustomWebClient(lifetime);
                result.Disposed += Result_Disposed;
                this.list_WebClient.Add(result);
            }
            else
            {
                foreach (CustomWebClient myWebClient in this.list_WebClient)
                {
                    if (!myWebClient.IsBusy)
                    {
                        myWebClient.Cancel_SelfDestruct();
                        result = myWebClient;
                    }
                }
                if (result == null)
                    result = GetFree(lifetime, true);
            }
            return result;
        }

        public CustomWebClient GetExclusive()
        {
            return (new CustomWebClient());
        }

        private void Result_Disposed(object sender, EventArgs e)
        {
            this.list_WebClient.Remove(sender as CustomWebClient);
        }

        public void Dispose()
        {
            this.StopAll();
            this.ForceCleanUp();
        }

        public void ForceCleanUp()
        {
            foreach (var cwc in this.list_WebClient)
            {
                cwc.Dispose();
            }
        }
    }
}
