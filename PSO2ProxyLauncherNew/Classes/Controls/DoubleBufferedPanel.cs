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

        /*protected override void OnSizeChanged(EventArgs e)
        {
            if (innerbgbuffer != null)
                innerbgbuffer.Dispose();
            if (!Size.IsEmpty)
            {
                innerbgbuffer = new Components.DirectBitmap(Size.Width, Size.Height);
                innerbgbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                innerbgbuffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }
            else
                innerbgbuffer = null;

            if (innerbuffer != null)
                innerbuffer.Dispose();
            if (!Size.IsEmpty)
            {
                innerbuffer = new Components.DirectBitmap(Size.Width, Size.Height);
                innerbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                innerbuffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }
            else
                innerbuffer = null;
        }

        public new void Dispose()
        {
            base.Dispose();
            if (innerbuffer != null)
                innerbuffer.Dispose();
            if (innerbgbuffer != null)
                innerbgbuffer.Dispose();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (innerbgbuffer != null)
            {
                innerbgbuffer.Graphics.Clear(this.BackColor);
                if (this.BackColor != Color.Transparent)
                    base.OnPaintBackground(new PaintEventArgs(innerbgbuffer.Graphics, e.ClipRectangle));
                e.Graphics.DrawImage(innerbgbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
            else
                base.OnPaintBackground(e);
            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (innerbuffer != null)
            {
                innerbuffer.Graphics.Clear(this.BackColor);
                if (this.BackColor != Color.Transparent)
                    base.OnPaint(new PaintEventArgs(innerbuffer.Graphics, e.ClipRectangle));
                e.Graphics.DrawImage(innerbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
            else
                base.OnPaint(e);
            base.OnPaint(e);
        }//*/
    }
}
