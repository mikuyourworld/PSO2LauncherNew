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
        public ExtendedTableLayoutPanel() : base() { }

        public void GetNewCache()
        {
            if (!this.Size.IsEmpty)
            {
                base.BackgroundImage = null;
                if (myBGCache != null)
                    myBGCache.Dispose();
                myBGCache = new QuickBitmap(this.Width, this.Height);
                ButtonRenderer.DrawParentBackground(myBGCache.Graphics, this.DisplayRectangle, this);
                base.BackgroundImage = myBGCache.Bitmap;
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            if (this.myBGCache != null)
                myBGCache.Dispose();
        }

        private QuickBitmap myBGCache;

        //public event EventHandler ParentBackgroundImageChanged;
    }
}
