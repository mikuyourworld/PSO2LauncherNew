using System.Windows.Forms;
using System.Drawing;
using PSO2ProxyLauncherNew.Classes.Components;
using System;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class RelativeButton : Button, Interfaces.ReserveRelativeLocation
    {
        private Point _RelativeLocation;
        public Point RelativeLocation
        {
            get { return this._RelativeLocation; }
            set { this._RelativeLocation = value; }
        }

        public new Point Location
        {
            get { return base.Location; }
            set
            {
                if (this.RelativeLocation == null)
                    this.RelativeLocation = value;
                base.Location = value;
            }
        }

        public RelativeButton() : base() { }

        public void GetNewCache()
        {
            if (!this.Size.IsEmpty)
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
