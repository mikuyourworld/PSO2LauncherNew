using PSO2ProxyLauncherNew.Classes.Components;
using PSO2ProxyLauncherNew.Classes.Components.Animator;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    public class AnimatedTabControl : TabControl
    {
        private static IntPtr one = new IntPtr(1);

        Animator animator;
        QuickBitmap bgbuffer, myBGCache;

        public AnimatedTabControl()
        {
            animator = new Animator(this);
            animator.Interval = 10F;
            animator.AnimationType = AnimationType.VertSlide;
            animator.DefaultAnimation.TimeCoeff = 1.5f;
            animator.DefaultAnimation.TransparencyCoeff = 1.5f;

            animator.DefaultAnimation.AnimateOnlyDifferences = false;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();
        }

#if !DEBUG
        protected override void WndProc(ref Message m)
        {
            // Hide tabs by trapping the TCM_ADJUSTRECT message
            if (m.Msg == 0x1328 && !DesignMode)
                m.Result = one;
            else
                base.WndProc(ref m);
        }
#endif

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == (Keys.Tab | Keys.Control)) || (keyData == (Keys.Tab | Keys.Shift | Keys.Control)))
            {
                return true;
                /*TabPage tp = GetNextEnabledTab((keyData & Keys.Shift) == Keys.None, true);
                if (tp != null)
                    if (tp != SelectedTab)
                    {
                        TabPageChangeEventArgs ev = new TabPageChangeEventArgs(SelectedTab, tp);
                        OnSelectedIndexChanging(ev);
                    }
                return true;//*/
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.bgbuffer != null)
                this.bgbuffer.Dispose();
            if (!this.Size.IsEmpty)
                this.bgbuffer = new QuickBitmap(this.Size);
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.bgbuffer != null && !this.bgbuffer.Disposed)
            {
                this.bgbuffer.Graphics.Clear(this.BackColor);
                if (this.BackColor == Color.Transparent)
                    ButtonRenderer.DrawParentBackground(this.bgbuffer.Graphics, e.ClipRectangle, this);
                else
                    base.OnPaint(new PaintEventArgs(this.bgbuffer.Graphics, e.ClipRectangle));
                e.Graphics.DrawImage(this.bgbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
            else
                base.OnPaint(e);
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Animation Animation
        {
            get { return animator.DefaultAnimation; }
            set { animator.DefaultAnimation = value; }
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

        protected override void OnSelecting(TabControlCancelEventArgs e)
        {
            base.OnSelecting(e);
            if (!DesignMode)
            {
                animator.BeginUpdate(this, false, null, new Rectangle(0, ItemSize.Height + 3, Width, Height - ItemSize.Height - 3));
                BeginInvoke(new MethodInvoker(() => animator.EndUpdate(this)));
            }
        }
    }
}
