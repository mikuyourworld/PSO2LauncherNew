using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    public partial class OwfProgressControl : Control
    {
        private const int SideMargin = 2;
        private int _angle = 0;

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
                Invalidate();
            }
        }

        [Browsable(true), DefaultValue(true)]
        public bool IsTransperant
        {
            get { return this.BackColor == Color.Transparent; }
            set
            {
                if (value)
                    this.BackColor = Color.Transparent;
                else
                    this.BackColor = SystemColors.Control;
            }
        }

        private Color _circlesColor = Color.Black;
        [Browsable(true), DefaultValue("Color.Black")]
        public Color CirclesColor
        {
            get { return _circlesColor; }
            set { _circlesColor = value; Invalidate(); }
        }

        public OwfProgressControl()
        {
            InitializeComponent();
            SetStyles();
        }

        public OwfProgressControl(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            SetStyles();
        }

        private void SetStyles()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private void OwfProgressControl_Paint(object sender, PaintEventArgs e)
        {
            float maxDiameter = Math.Min(this.Height, this.Width) - 2 * SideMargin;
            float center = Math.Min(this.Height, this.Width) / 2;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw circles
            for (int i = 1; i <= _noOfCircles; i++)
            {
                float a1 = ((maxDiameter / (float)_noOfCircles) * i) / 2.0F;

                RectangleF rect = new RectangleF(center - a1, center - a1, 2.0F * a1, 2.0F * a1);
                if (i % 4 == 0)
                    e.Graphics.DrawArc(new Pen(this._circlesColor), rect, _angle, 300);
                else if (i % 4 == 1)
                    e.Graphics.DrawArc(new Pen(this._circlesColor), rect, 360 - _angle + 90, 300);
                else if (i % 4 == 2)
                    e.Graphics.DrawArc(new Pen(this._circlesColor), rect, 360 - _angle + 180, 300);
                else
                    e.Graphics.DrawArc(new Pen(this._circlesColor), rect, 360 - _angle + 270, 300);
            }

            // Draw Text
            SizeF stringSize = e.Graphics.MeasureString(_titileText, this.Font);
            e.Graphics.DrawString(this._titileText, this.Font, new SolidBrush(this.ForeColor),
                maxDiameter + 3 * SideMargin, ((float)this.Height - stringSize.Height) / 2);
        }

        private void drawTimer_Tick(object sender, EventArgs e)
        {
            _angle = (_angle + 5) % 360;
            Invalidate();
        }
    }

}
