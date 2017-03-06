using PSO2ProxyLauncherNew.Classes.Components;
using System;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Interfaces
{
    public abstract class LazyPaint : System.Windows.Forms.Control
    {

        public LazyPaint() : base() { }

        public void GetNewCache()
        {
            if (!this.Size.IsEmpty)
            {
                this.BackgroundImage = null;
                if (myBGCache != null)
                    myBGCache.Dispose();
                myBGCache = new QuickBitmap(this.Width, this.Height);
                ButtonRenderer.DrawParentBackground(myBGCache.Graphics, this.DisplayRectangle, this);
                this.BackgroundImage = myBGCache.Bitmap;
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            if (this.myBGCache != null)
                myBGCache.Dispose();
        }

        private QuickBitmap myBGCache;
    }
}
