using PSO2ProxyLauncherNew.Classes.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class ExtendedTableLayoutPanel : TableLayoutPanel
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

        public ExtendedTableLayoutPanel() : base() { this._CacheBackground = false; }

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

        //public event EventHandler ParentBackgroundImageChanged;
    }
}
