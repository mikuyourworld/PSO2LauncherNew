using PSO2ProxyLauncherNew.Classes.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    public class PagingForm : MetroFramework.Forms.MetroForm
    {
        private Timer myTimer;
        private Leayal.Drawing.QuickBitmap tqb;
        private List<Control> _controlList;
        private bool _ready;

        public PagingForm() : base()
        {
            this._ready = false;
            this._controlList = new List<Control>();
            this.myTimer = new Timer();
            this.myTimer.Enabled = false;
            this.myTimer.Interval = 15;
            //this.myTimer.AutoReset = true;
            //this.myTimer.SynchronizingObject = this;
            this.myTimer.Tick += this.MyTimer_Tick;
            //this.myTimer.Elapsed += this.MyTimer_Elapsed;
            this._selectedindex = 0;
            this.matrix = new ColorMatrix();
            this._attribute = new ImageAttributes();
            this._attribute.SetColorMatrix(this.matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            for (int i = 0; i < this._controlList.Count; i++)
                if (i == this.SelectedIndex)
                    this._controlList[i].Visible = true;
                else
                {
                    this._controlList[i].Visible = false;
                }
            this._ready = true;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (this.IsPanel(e.Control))
                this._controlList.Add(e.Control);
            base.OnControlAdded(e);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            //if (e.Control is Panel || e.Control is FlowLayoutPanel || e.Control is TableLayoutPanel)
            if (this._controlList.Contains(e.Control))
                this._controlList.Remove(e.Control);
            base.OnControlRemoved(e);
        }

        private bool IsPanel(Control c)
        {
            if (c is Panel || c is FlowLayoutPanel || c is TableLayoutPanel)
                return true;
            else
                return false;
        }

        [Browsable(false)]
        public Control SelectedTab
        {
            get
            {
                if (this._controlList != null && this._controlList.Count > 0)
                    return this._controlList[this.SelectedIndex];
                else
                    return null;
            }
            set
            {
                if (this._controlList != null && this._controlList.Contains(value))
                    this.SelectedIndex = this._controlList.IndexOf(value);
                else
                    throw new InvalidOperationException("This control is not a panel-type or is not added to this form.");
            }
        }

        private int _selectedindex;
        [Browsable(true), DefaultValue(0), Category("Tabs")]
        public int SelectedIndex
        {
            get { return this._selectedindex; }
            set
            {
                if (this._selectedindex != value)
                {
                    if (value > 0 && value >= this._controlList.Count)
                        throw new IndexOutOfRangeException("Non-existed tab");
                    this.OnSelectedIndexChanging(new SelectedIndexChangingEventArgs(this._selectedindex, value));
                    this._selectedindex = value;
                    this.OnSelectedIndexChanged(System.EventArgs.Empty);
                }
            }
        }

        private Control destinationTab;
        private int countup;
        public const int jump = 10;
        private ColorMatrix matrix;
        private ImageAttributes _attribute;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._attribute != null)
                    this._attribute.Dispose();
            }
            base.Dispose(disposing);
        }

        public event EventHandler<SelectedIndexChangingEventArgs> SelectedIndexChanging;
        protected virtual void OnSelectedIndexChanging(SelectedIndexChangingEventArgs e)
        {
            this.SelectedIndexChanging?.Invoke(this, e);
            if (!e.Cancel)
            {
                if (this._controlList != null && this._controlList.Count > 0)
                {
                    if (!DesignMode && this._ready)
                    {
                        if (this.tqb != null)
                            this.tqb.Dispose();
                        this.destinationTab = this._controlList[e.IndexAfter];
                        this.destinationTab.Visible = false;
                        Point loc = this.destinationTab.Location;
                        this.tqb = new Leayal.Drawing.QuickBitmap(this.destinationTab.Size);
                        this.DrawBackground = false;
                        this.tqb.Graphics.Clear(Color.Transparent);
                        this.tqb.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                        this.tqb.Graphics.InterpolationMode = InterpolationMode.High;
                        this.tqb.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        this.tqb.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        this.destinationTab.Location = new Point(this.destinationTab.Size.Width * -2, this.destinationTab.Size.Height * -2);
                        this.destinationTab.Visible = true;
                        //this.RecursiveInvokePaint(this.tqb.Graphics, this.destinationTab, false);
                        this.destinationTab.DrawToBitmap(tqb.Bitmap, this.destinationTab.ClientRectangle);
                        this.destinationTab.Visible = false;
                        this.destinationTab.Location = loc;//*/
                        this.countup = 0;
                        if (this.SelectedTab != null)
                            this.SelectedTab.Visible = false;
                        this.myTimer.Start();
                    }
                    else
                    {
                        if (this.SelectedTab != null)
                            this.SelectedTab.Visible = false;
                        this._controlList[e.IndexAfter].Visible = true;
                    }
                }
            }
        }

        protected override void OnPainfulPaint(PaintEventArgs e)
        {
            base.OnPainfulPaint(e);
            if (this._ready && this.tqb != null && !this.tqb.Disposed)
            {
                /*int ad = jump - this.countup;
                double currentScale = 0.1F * this.countup;
                Size tmpSize = new Size(Convert.ToInt32(this.tqb.Bitmap.Size.Width * currentScale), Convert.ToInt32(this.tqb.Bitmap.Size.Height * currentScale));
                Point tmpPoint = this.destinationTab.Location;
                tmpPoint.Offset((this.tqb.Bitmap.Size.Width / 2) - (tmpSize.Width / 2), (this.tqb.Bitmap.Size.Height / 2) - (tmpSize.Height / 2));//*/

                /*//This is for fly-in effect
                e.Graphics.DrawImage(this.tqb.Bitmap, new Rectangle(new Point(this.destinationTab.Location.X - ad, this.destinationTab.Location.Y), this.tqb.Bitmap.Size),
                    0, 0, this.tqb.Bitmap.Size.Width, this.tqb.Bitmap.Size.Height, GraphicsUnit.Pixel, _attribute);//*/
                /*//This is for Zoom effect
                e.Graphics.DrawImage(this.tqb.Bitmap, new Rectangle(tmpPoint, tmpSize),
                    0, 0, this.tqb.Bitmap.Size.Width, this.tqb.Bitmap.Size.Height, GraphicsUnit.Pixel, _attribute);//*/
                //This is for Fading effect
                e.Graphics.DrawImage(this.tqb.Bitmap, new Rectangle(this.destinationTab.Location, this.tqb.Bitmap.Size),
                    0, 0, this.tqb.Bitmap.Size.Width, this.tqb.Bitmap.Size.Height, GraphicsUnit.Pixel, _attribute);//*/
            }
        }

        private Bitmap RecursiveInvokePaint(Control _container, bool paintBackground)
        {
            Bitmap result = new Bitmap(_container.Size.Width, _container.Size.Height);
            using (Graphics gr = Graphics.FromImage(result))
                this.RecursiveInvokePaint(gr, _container, paintBackground);
            return result;
        }

        private void RecursiveInvokePaint(Bitmap bm, Control _container, bool paintBackground)
        {
            using (Graphics gr = Graphics.FromImage(bm))
                this.RecursiveInvokePaint(gr, _container, paintBackground);
        }

        private void RecursiveInvokePaint(Graphics gr, Control _container, bool paintBackground)
        {
            foreach (Control ctl in _container.Controls)
                this.RecursiveInvokePaint(gr, ctl, ctl.Location, paintBackground);
        }

        private void RecursiveInvokePaint(Graphics gr, Control _container, Point offset, bool paintBackground)
        {
            //Currently skip RichTextBox and WebBrowser, they're not being painted by .NET
            if (!_container.Visible || _container is RichTextBox || _container is WebBrowser) return;
            if (_container.Controls != null && _container.Controls.Count > 0)
            {
                using (Leayal.Drawing.QuickBitmap qbm = new Leayal.Drawing.QuickBitmap(_container.Size))
                {
                    qbm.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    qbm.Graphics.InterpolationMode = InterpolationMode.High;
                    qbm.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    qbm.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    if (paintBackground)
                        this.InvokePaintBackground(_container, new PaintEventArgs(qbm.Graphics, new Rectangle(Point.Empty, _container.Size)));
                    else
                        qbm.Graphics.Clear(Color.Transparent);
                    this.InvokePaint(_container, new PaintEventArgs(qbm.Graphics, new Rectangle(Point.Empty, _container.Size)));
                    gr.DrawImageUnscaled(qbm.Bitmap, offset);
                }
                foreach (Control ctl in _container.Controls)
                    RecursiveInvokePaint(gr, ctl, new Point(offset.X + ctl.Location.X, offset.Y + ctl.Location.Y), paintBackground);
            }
            else
                using (Leayal.Drawing.QuickBitmap qbm = new Leayal.Drawing.QuickBitmap(_container.Size))
                {
                    qbm.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    qbm.Graphics.InterpolationMode = InterpolationMode.High;
                    qbm.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    qbm.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    if (paintBackground)
                        this.InvokePaintBackground(_container, new PaintEventArgs(qbm.Graphics, new Rectangle(Point.Empty, _container.Size)));
                    else
                        qbm.Graphics.Clear(Color.Transparent);
                    this.InvokePaint(_container, new PaintEventArgs(qbm.Graphics, new Rectangle(Point.Empty, _container.Size)));
                    gr.DrawImageUnscaled(qbm.Bitmap, offset);
                }
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            if (this.tqb != null && !this.tqb.Disposed)
            {
                this.countup++;
                matrix.Matrix33 = ((this.countup * 10) / 100F);
                _attribute.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                if (countup >= 10)
                {
                    this.DrawBackground = true;
                    this.tqb.Dispose();
                    this.Invalidate(false);
                    this.destinationTab.Visible = true;
                    this.myTimer.Stop();
                }
                else
                    this.Invalidate(false);
            }
            else
            {
                this.DrawBackground = true;
                this.Invalidate(false);
                this.destinationTab.Visible = true;
                this.myTimer.Stop();
            }
        }

        public event EventHandler SelectedIndexChanged;
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            this.SelectedIndexChanged?.Invoke(this, e);
        }
    }
}
