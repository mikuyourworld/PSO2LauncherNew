using PSO2ProxyLauncherNew.Classes.Components;
using PSO2ProxyLauncherNew.Classes.Infos;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class DoubleBufferedPanel : Panel
    {
        //Components.DirectBitmap innerbuffer, innerbgbuffer;

        public DoubleBufferedPanel() : base()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.UpdateStyles();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        protected override void OnParentBackgroundImageChanged(EventArgs e)
        {
            base.OnParentBackgroundImageChanged(e);
        }

        public void GetNewCache()
        {
            if (!this.Size.IsEmpty)
            {
                base.BackgroundImage = null;
                if (myBGCache != null)
                    myBGCache.Dispose();
                myBGCache = new QuickBitmap(this.Width, this.Height);
                ButtonRenderer.DrawParentBackground(myBGCache.Graphics, this.ClientRectangle, this);
                /*var myForm = this.FindForm();
                if (myForm != null)
                    using (QuickBitmap qb = new QuickBitmap(myForm.Size))
                    {
                        Point pa = relative(myForm);
                        this.InvokePaintBackground(myForm, new PaintEventArgs(qb.Graphics, new Rectangle(pa, this.Size)));
                        myBGCache.Graphics.DrawImageUnscaled(qb.Bitmap, pa.X * -1, pa.Y * -1);// new Rectangle(relative(myForm), this.ClientSize), asd(myForm), GraphicsUnit.Pixel);
                    }//*/
                base.BackgroundImage = myBGCache.Bitmap;
            }
        }

        /*Rectangle geh;
        Point pa;
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.myForm != null)
            {
                using (QuickBitmap qb = new QuickBitmap(myForm.Size))
                {
                    pa = relative(this.myForm);
                    geh = new Rectangle(pa.X + e.ClipRectangle.X, pa.Y + e.ClipRectangle.Y, e.ClipRectangle.Size.Width, e.ClipRectangle.Size.Height);
                    //MessageBox.Show(geh.ToString());
                    this.InvokePaintBackground(myForm, new PaintEventArgs(qb.Graphics, geh));
                    e.Graphics.DrawImage(qb.Bitmap, e.ClipRectangle, geh, GraphicsUnit.Pixel);
                }
            }
            else
            {
                //MessageBox.Show("lahwigilhawg");
                base.OnPaintBackground(e);
            }
        }

        //Form myForm = null;
        public Form myForm { get; set; }
        private Point relative(Form onForm)
        {
            Point controlLoc = onForm.PointToScreen(this.Location);
            return new Point(controlLoc.X - onForm.Location.X, controlLoc.Y - onForm.Location.Y);
        }//*/

        public new void Dispose()
        {
            base.Dispose();
            if (this.myBGCache != null)
                myBGCache.Dispose();
        }

        private QuickBitmap myBGCache;
    }
}
