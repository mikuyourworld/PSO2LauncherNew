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

        private DirectBitmap backbuffer, backbgbuffer;

        private string _titileText = "Loading..";
        [Browsable(true), DefaultValue("Loading..")]
        public string TitileText
        {
            get { return _titileText; }
            set { _titileText = value; Invalidate(); }
        }

        private int _noOfCircles = 5;
        [Browsable(true), DefaultValue(5)]
        public int NoOfCircles
        {
            get { return _noOfCircles; }
            set { _noOfCircles = value; Invalidate(); }
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
            set { _circlesColor = value; Invalidate(); }
        }

        /*public new Control Parent
        {
            get { return base.Parent; }
            set
            {
                if (base.Parent != null)
                    base.Parent.BackgroundPaint -= this.drawBackground;
                base.Parent = value;
                value.BackgroundPaint -= this.drawBackground;
            }
        }

        public event PaintEventHandler BackgroundPaint;//*/
        //Let's use sh**

        /*protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (this.backbgbuffer == null)
            {
                this.backbgbuffer = new DirectBitmap(this.Width, this.Height);
                this.backbgbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }
            if (this.backbgbuffer != null)
                base.OnPaintBackground(new PaintEventArgs(this.backbgbuffer.Graphics, pevent.ClipRectangle));
        }//*/

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
                Invalidate();
            }
        }

        private void SetStyles()
        {
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
            this.backbuffer = new DirectBitmap(this.Width, this.Height);
            this.backbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (this.backbgbuffer != null)
                this.backbgbuffer.Dispose();
            this.backbgbuffer = new DirectBitmap(this.Width, this.Height);
            this.backbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (this.backbuffer == null)
            {
                this.backbuffer = new DirectBitmap(this.Width, this.Height);
                this.backbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            if (this.backbgbuffer == null)
            {
                this.backbgbuffer = new DirectBitmap(this.Width, this.Height);
                this.backbgbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            this.backbuffer.Graphics.Clear(this.BackColor);

            //if (this.backbgbuffer != null) this.backbuffer.Graphics.DrawImage(this.backbgbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);

            float maxDiameter = Math.Min(this.Height, this.Width) - 2 * SideMargin;
            float center = Math.Min(this.Height, this.Width) / 2;

            // Draw circles
            for (int i = 1; i <= _noOfCircles; i++)
            {
                float a1 = ((maxDiameter / (float)_noOfCircles) * i) / 2.0F;

                RectangleF rect = new RectangleF(center - a1, center - a1, 2.0F * a1, 2.0F * a1);
                if (i % 4 == 0)
                    this.backbuffer.Graphics.DrawArc(new Pen(this._circlesColor), rect, _angle, 300);
                else if (i % 4 == 1)
                    this.backbuffer.Graphics.DrawArc(new Pen(this._circlesColor), rect, 360 - _angle + 90, 300);
                else if (i % 4 == 2)
                    this.backbuffer.Graphics.DrawArc(new Pen(this._circlesColor), rect, 360 - _angle + 180, 300);
                else
                    this.backbuffer.Graphics.DrawArc(new Pen(this._circlesColor), rect, 360 - _angle + 270, 300);
            }

            // Draw Text
            SizeF stringSize = TextRenderer.MeasureText(_titileText, this.Font);
            this.backbuffer.Graphics.DrawString(this._titileText, this.Font, new SolidBrush(this.ForeColor),
                maxDiameter + 3 * SideMargin, ((float)this.Height - stringSize.Height) / 2);
            e.Graphics.DrawImage(this.backbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        public new void Dispose()
        {
            base.Dispose();
            if (this.backbuffer != null)
                this.backbuffer.Dispose();
            if (this.backbgbuffer != null)
                this.backbgbuffer.Dispose();
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
