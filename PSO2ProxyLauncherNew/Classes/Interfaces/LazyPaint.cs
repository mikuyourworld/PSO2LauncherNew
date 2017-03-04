using PSO2ProxyLauncherNew.Classes.Components;
using System;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Interfaces
{
    abstract class LazyPaint : System.Windows.Forms.Control
    {
        private bool _CacheBackground;
        public bool CacheBackground
        {
            get { return this._CacheBackground; }
            set
            {
                this._CacheBackground = value;
                this.GetNewCache();
            }
        }

        public LazyPaint() : base() { this._CacheBackground = false; }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.GetNewCache();
        }

        protected override void OnParentBackgroundImageChanged(EventArgs e)
        {
            base.OnParentBackgroundImageChanged(e);
            this.GetNewCache();
        }

        public void GetNewCache()
        {
            if (this.CacheBackground)
            {
                this.BackgroundImage = null;
                if (myBGCache != null)
                    myBGCache.Dispose();
                myBGCache = new DirectBitmap(this.Width, this.Height);
                ButtonRenderer.DrawParentBackground(myBGCache.Graphics, this.ClientRectangle, this);
                this.BackgroundImage = myBGCache.Bitmap;
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            if (this.myBGCache != null)
                myBGCache.Dispose();
        }

        private DirectBitmap myBGCache;
    }
}
