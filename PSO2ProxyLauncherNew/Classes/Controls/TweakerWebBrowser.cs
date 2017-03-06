using System;
using System.Threading;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class TweakerWebBrowser : System.Windows.Forms.WebBrowser
    {
        private SynchronizationContext syncContext;

        public TweakerWebBrowser() : base()
        {
            this.syncContext = SynchronizationContext.Current;
            this.LockNavigate = false;
            this.EnglishPatchStatus = PatchStatus.Unknown;
            this.ItemPatchStatus = PatchStatus.Unknown;
        }

        public bool LockNavigate
        { get; set; }

        public new void Navigate(string address)
        {
            this.syncContext.Send(new System.Threading.SendOrPostCallback(this._NavigateAsync), address);
        }

        public void NavigateAsync(string address)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(this._NavigateAsync), address);
        }

        private void _NavigateAsync(object address)
        {
            base.Navigate(address as string);
        }

        public void LoadHTML(string htmlContent)
        {
            this.syncContext.Send(new SendOrPostCallback(this._LoadHTML), htmlContent);
        }

        public void LoadHTMLAsync(string htmlContent)
        {
            this.syncContext.Post(new SendOrPostCallback(this._LoadHTML), htmlContent);
        }

        public void LoadHTML(System.IO.Stream _stream)
        {
            var aaaaa = this.DocumentStream;
            this.DocumentStream = _stream;
            if (aaaaa != null)
                aaaaa.Dispose();
        }

        private void _LoadHTML(object htmlContent)
        {
            string content = htmlContent as string;
            if (!string.IsNullOrWhiteSpace(content))
            {
                if (this.Document == null)
                    this.DocumentText = content;
                else
                {
                    this.Document.OpenNew(true);
                    this.Document.Write(content);
                }
            }
            else
                this.Navigate("about:blank");
        }

        public void EnglishPatch_SetStatus(string StatusString)
        {
            StatusString = StatusString.ToLower();
            switch (StatusString)
            {
                case "compatible":
                    this.EnglishPatchStatus = PatchStatus.Compatible;
                    break;
                case "incompatible":
                    this.EnglishPatchStatus = PatchStatus.Incompatible;
                    break;
                case "unknown":
                    this.EnglishPatchStatus = PatchStatus.Unknown;
                    break;
            }
        }

        public void ItemPatch_SetStatus(string StatusString)
        {
            StatusString = StatusString.ToLower();
            switch (StatusString)
            {
                case "compatible":
                    this.ItemPatchStatus = PatchStatus.Compatible;
                    break;
                case "incompatible":
                    this.ItemPatchStatus = PatchStatus.Incompatible;
                    break;
                case "unknown":
                    this.ItemPatchStatus = PatchStatus.Unknown;
                    break;
            }
        }

        public PatchStatus EnglishPatchStatus
        { get; set; }

        public PatchStatus ItemPatchStatus
        { get; set; }

        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (this.LockNavigate)
            {
                e.Cancel = true;
                this.OnLockedNavigating(e);
                Thread cacheThread = new Thread(new ThreadStart(delegate {
                    WebBrowser cacher = new WebBrowser();
                    cacher.Navigated += Cacher_Navigated;
                    cacher.Navigate(e.Url);
                }));
                cacheThread.IsBackground = true;
                cacheThread.SetApartmentState(ApartmentState.STA);
                cacheThread.Start();
            }
            else
                base.OnNavigating(e);
        }

        public event WebBrowserNavigatingEventHandler LockedNavigating;
        protected virtual void OnLockedNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (this.LockedNavigating != null)
                this.syncContext?.Post(new SendOrPostCallback(delegate { this.LockedNavigating.Invoke(this, e); }), null);
        }

        private void Cacher_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            WebBrowser un = sender as WebBrowser;
            if (un != null)
            {
                un.Stop();
                un.Navigated -= Cacher_Navigated;
                un.Dispose();
            }
        }
    }

    public enum PatchStatus : short
    {
        Unknown = 0,
        Compatible = 1,
        Incompatible = 2
    }
}
