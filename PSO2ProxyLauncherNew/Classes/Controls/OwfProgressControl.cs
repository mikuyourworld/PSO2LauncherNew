using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using PSO2ProxyLauncherNew.Classes.Components;
using PSO2ProxyLauncherNew.Classes.Interfaces;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class OwfProgressControl : Interfaces.LazyPaint
    {
        private const int SideMargin = 2;
        private int _angle = 0;
        private float maxDiameter, center;

        private QuickBitmap backbuffer, backbgbuffer;
        SolidBrush innerSolidBrush;
        Pen innerPen;

        private string _titileText = "Loading..";
        [Browsable(true), DefaultValue("Loading..")]
        public string TitileText
        {
            get { return _titileText; }
            set { _titileText = value; this.OnTextChanged(EventArgs.Empty); }
        }

        private int _noOfCircles = 5;
        [Browsable(true), DefaultValue(5)]
        public int NoOfCircles
        {
            get { return _noOfCircles; }
            set { _noOfCircles = value; }
        }

        private Int16 _animationSpeed = 75;
        [Browsable(true), DefaultValue(75)]
        public Int16 AnimationSpeed
        {
            get { return _animationSpeed; }
            set
            {
                if (value > 100) value = 100;
                if (value < 1) value = 1;

                _animationSpeed = value;
                this.drawTimer.Interval = 101 - _animationSpeed;
            }
        }

        private Color _circlesColor = Color.Black;

        [Browsable(true), DefaultValue("Color.Black")]
        public Color CirclesColor
        {
            get { return _circlesColor; }
            set { _circlesColor = value; if (this.innerPen != null) this.innerPen.Dispose(); this.innerPen = new Pen(this._circlesColor); }
        }

        private System.Timers.Timer drawTimer;

        public OwfProgressControl() : base()
        {
            this.drawTimer = new System.Timers.Timer();
            AnimationSpeed = 75;
            this.drawTimer.Elapsed += DrawTimer_Elapsed;
            if (DesignMode)
                this.drawTimer.Stop();
            else
                this.drawTimer.Start();
            SetStyles();
        }

        public OwfProgressControl(IContainer container) : this()
        {
            container.Add(this);
        }
        private void DrawTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!DesignMode)
            {
                _angle = (_angle + 5) % 360;
                Invalidate(false);
            }
        }

        private void SetStyles()
        {
            SetStyle(ControlStyles.CacheText, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.backbuffer != null)
                this.backbuffer.Dispose();
            this.backbuffer = new QuickBitmap(this.Width, this.Height);
            this.backbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (this.backbgbuffer != null)
                this.backbgbuffer.Dispose();
            this.backbgbuffer = new QuickBitmap(this.Width, this.Height);
            this.backbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            this.maxDiameter = Math.Min(this.Height, this.Width) - 2 * SideMargin;
            this.center = Math.Min(this.Height, this.Width) / 2;
            base.OnSizeChanged(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            if (this.innerSolidBrush != null)
                this.innerSolidBrush.Dispose();
            this.innerSolidBrush = new SolidBrush(this.ForeColor);
            base.OnForeColorChanged(e);
        }

        /*protected override void OnPaintBackground(PaintEventArgs p)
        {
            if (backbgbuffer != null)
            {
                backbgbuffer.Graphics.Clear(this.BackColor);
                if (this.BackColor != Color.Transparent || Parent == null)
                    base.OnPaintBackground(new PaintEventArgs(backbgbuffer.Graphics, p.ClipRectangle));
                else
                    RadioButtonRenderer.DrawParentBackground(backbgbuffer.Graphics, p.ClipRectangle, this);
                p.Graphics.DrawImage(backbgbuffer.Bitmap, p.ClipRectangle, p.ClipRectangle, GraphicsUnit.Pixel);
            }
            else
            {
                base.OnPaintBackground(p);
                if (this.BackColor == Color.Transparent && Parent != null)
                    RadioButtonRenderer.DrawParentBackground(p.Graphics, p.ClipRectangle, this);
            }
        }*/

        float a1;
        RectangleF rect;
        SizeF stringSize;

        protected override void OnTextChanged(EventArgs e)
        {
            stringSize = TextRenderer.MeasureText(_titileText, this.Font);
            base.OnTextChanged(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            stringSize = TextRenderer.MeasureText(_titileText, this.Font);
            base.OnFontChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (this.backbuffer == null)
            {
                this.backbuffer = new QuickBitmap(this.Width, this.Height);
                this.backbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            if (this.innerPen == null)
                this.innerPen = new Pen(this._circlesColor);

            if (this.innerSolidBrush==null)
                this.innerSolidBrush = new SolidBrush(this.ForeColor);

            this.backbuffer.Graphics.Clear(Color.Transparent);

            // Draw circles
            for (int i = 1; i <= _noOfCircles; i++)
            {
                a1 = ((maxDiameter / (float)_noOfCircles) * i) / 2.0F;

                rect = new RectangleF(center - a1, center - a1, 2.0F * a1, 2.0F * a1);
                if (i % 4 == 0)
                    this.backbuffer.Graphics.DrawArc(this.innerPen, rect, _angle, 300);
                else if (i % 4 == 1)
                    this.backbuffer.Graphics.DrawArc(this.innerPen, rect, 360 - _angle + 90, 300);
                else if (i % 4 == 2)
                    this.backbuffer.Graphics.DrawArc(this.innerPen, rect, 360 - _angle + 180, 300);
                else
                    this.backbuffer.Graphics.DrawArc(this.innerPen, rect, 360 - _angle + 270, 300);
            }
            // Draw Text
            //this.backbuffer.Graphics.DrawString(this._titileText, this.Font, this.innerSolidBrush, maxDiameter + 3 * SideMargin, ((float)this.Height - stringSize.Height) / 2);
            e.Graphics.DrawImage(this.backbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.backbuffer != null)
                    this.backbuffer.Dispose();
                if (this.backbgbuffer != null)
                    this.backbgbuffer.Dispose();
                if (this.innerPen != null)
                    this.innerPen.Dispose();
                if (this.innerSolidBrush != null)
                    this.innerSolidBrush.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!DesignMode && this.drawTimer != null)
            {
                if (this.Visible)
                    this.drawTimer.Start();
                else
                    this.drawTimer.Stop();
            }
            base.OnVisibleChanged(e);
        }
    }

}
