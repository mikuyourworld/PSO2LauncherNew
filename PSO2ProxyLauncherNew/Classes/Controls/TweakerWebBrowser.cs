using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class TweakerWebBrowser : System.Windows.Forms.WebBrowser
    {
        private WindowsFormsSynchronizationContext syncContext;

        public TweakerWebBrowser() : base()
        {
            this.syncContext = WindowsFormsSynchronizationContext.Current as WindowsFormsSynchronizationContext;
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
            this.syncContext.Send(new System.Threading.SendOrPostCallback(this._LoadHTML), htmlContent);
        }

        public void LoadHTMLAsync(string htmlContent)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(this._LoadHTML), htmlContent);
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
            {
                this.Navigate("about:blank");
            }
        }

        public void EnglishPatch_SetStatus(string StatusString)
        {
            if ((StatusString == "compatible"))
            {
                this.EnglishPatchStatus = PatchStatus.Compatible;
            }
            else if ((StatusString == "incompatible"))
            {
                this.EnglishPatchStatus = PatchStatus.Incompatible;
            }
            else if ((StatusString == "unknown"))
            {
                this.EnglishPatchStatus = PatchStatus.Unknown;
            }
        }

        public void ItemPatch_SetStatus(string StatusString)
        {
            if ((StatusString == "compatible"))
            {
                this.ItemPatchStatus = PatchStatus.Compatible;
            }
            else if ((StatusString == "incompatible"))
            {
                this.ItemPatchStatus = PatchStatus.Incompatible;
            }
            else if ((StatusString == "unknown"))
            {
                this.ItemPatchStatus = PatchStatus.Unknown;
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
                this.LockedNavigating?.Invoke(this, e);
                WebBrowser cacher = new WebBrowser();
                cacher.Navigated += Cacher_Navigated;
                cacher.Navigate(e.Url);
            }
            else
                base.OnNavigating(e);
        }

        private void Cacher_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            WebBrowser un = sender as WebBrowser;
            un.Stop();
            un.Navigated -= Cacher_Navigated;
            un.Dispose();
        }

        public event WebBrowserNavigatingEventHandler LockedNavigating;
    }

    public enum PatchStatus : short
    {
        Unknown = 0,
        Compatible = 1,
        Incompatible = 2
    }
}
