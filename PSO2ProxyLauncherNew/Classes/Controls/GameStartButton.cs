using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Timers;
using Leayal.Drawing;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class GameStartButton : Interfaces.LazyPaint, Interfaces.ReserveRelativeLocation
    {
        LinearGradientBrush innerBrush, OuterBrush, textBrush;
        Pen OuterPen, penWhite, penYellow, penBlue, penRed;
        DirectBitmap innerbuffer, innerbgbuffer;
        private Color _ProgressColor1, _ProgressColor2, _ProgressColor3;
        ColorMatrix matrix;
        ImageAttributes attributes, darkAttribute;
        float halfwidth, halfheight;
        Size halfsize_game, halfsize_start;
        Point loc_start;
        Random _random;
        GraphicsPath innerGrpath;
        Region innerRegion;

        const string str_game = "GAME", str_start = "START";

        System.Timers.Timer drawTimer;

        public Color MainColor
        {
            get { return _ProgressColor1; }
            set
            {
                if (value != _ProgressColor1)
                {
                    _ProgressColor1 = value;
                    if (innerBrush != null)
                        innerBrush.Dispose();
                    if (!this.ClientRectangle.IsEmpty)
                        innerBrush = new LinearGradientBrush(new Rectangle(2, 2, this.Width - 4, this.Height - 4), this._ProgressColor1, this._ProgressColor1, LinearGradientMode.Vertical);
                    Invalidate();
                }
            }
        }

        public void PerformClick()
        {
            this.OnClick(EventArgs.Empty);
        }

        public Color SubColor1
        {
            get { return _ProgressColor2; }
            set
            {
                if (value != _ProgressColor2)
                {
                    _ProgressColor2 = value;
                    if (OuterPen != null)
                        OuterPen.Dispose();
                    if (OuterBrush != null)
                        OuterBrush.Dispose();
                    if (!this.ClientRectangle.IsEmpty)
                    {
                        OuterBrush = new LinearGradientBrush(new Rectangle(1, 1, this.Width - 2, this.Height - 2), this._ProgressColor2, this._ProgressColor3, LinearGradientMode.ForwardDiagonal);
                        OuterPen = new Pen(OuterBrush, 14f);
                    }
                    Invalidate();
                }
            }
        }

        public Color SubColor2
        {
            get { return _ProgressColor3; }
            set
            {
                if (value != _ProgressColor3)
                {
                    _ProgressColor3 = value;
                    if (OuterPen != null)
                        OuterPen.Dispose();
                    if (OuterBrush != null)
                        OuterBrush.Dispose();
                    if (!this.ClientRectangle.IsEmpty)
                    {
                        OuterBrush = new LinearGradientBrush(new Rectangle(1, 1, this.Width - 2, this.Height - 2), this._ProgressColor2, this._ProgressColor3, LinearGradientMode.ForwardDiagonal);
                        OuterPen = new Pen(OuterBrush, 14f);
                    }
                    Invalidate();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.drawTimer != null)
                {
                    this.drawTimer.Stop();
                    this.drawTimer.Dispose();
                }
                if (innerbuffer != null)
                    innerbuffer.Dispose();
                if (innerbgbuffer != null)
                    innerbgbuffer.Dispose();
                if (this.OuterPen != null)
                    this.OuterPen.Dispose();
                if (this.innerBrush != null)
                    this.innerBrush.Dispose();
                if (this.OuterBrush != null)
                    this.OuterBrush.Dispose();
                if (this.penRed != null)
                    this.penRed.Dispose();
                if (this.penYellow != null)
                    this.penYellow.Dispose();
                if (this.penBlue != null)
                    this.penBlue.Dispose();
                if (this.penWhite != null)
                    this.penWhite.Dispose();
                if (this.innerGrpath != null)
                    this.innerGrpath.Dispose();
                if (this.innerRegion != null)
                    this.innerRegion.Dispose();
                if (this.attributes != null)
                    this.attributes.Dispose();
                if (this.darkAttribute != null)
                    this.darkAttribute.Dispose();
            }
        }

        public GameStartButton() : base()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.CacheText, true);
            this.SetStyle(ControlStyles.UserMouse, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            this.cur_innerrotate = 360;
            this.cur_rotate = 0;

            this.matrix = new ColorMatrix();
            this.attributes = new ImageAttributes();

            this.MainColor = Color.Brown;
            this.SubColor1 = Color.Brown;
            this.SubColor2 = Color.Green;

            base.MinimumSize = new Size(100, 100);
            //this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            this.innerGrpath = new GraphicsPath();
            this.darkAttribute = GetBrightnessAdjuster(0.7F);

            this._random = new Random();
            this.mouseCaptured = false;
            this.mouseDown = false;

            //this.halfsize_game = TextRenderer.MeasureText(str_game, this.Font);
            this.halfsize_start = TextRenderer.MeasureText(str_start, this.Font);

            this.penWhite = new Pen(Brushes.White, 2F);
            this.penBlue = new Pen(Brushes.Blue, 2F);
            this.penYellow = new Pen(Brushes.Yellow, 2F);
            this.penRed = new Pen(Brushes.Red, 2F);

            this.drawTimer = new System.Timers.Timer();
            this.drawTimer.Elapsed += this.DrawTimer_Elapsed;
            this.FPS = 75;
            if (!DesignMode)
                this.drawTimer.Start();
        }

        public new Size MinimumSize
        {
            get { return base.MinimumSize; }
            set
            {
                if (value.Height < 100 || value.Width < 100)
                    base.MinimumSize = new Size(100, 100);
                else
                    base.MinimumSize = value;
            }
        }

        private int _Opacity;
        public int Opacity
        {
            get { return this._Opacity; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;
                if (value != this._Opacity)
                {
                    this._Opacity = value;
                    matrix.Matrix33 = (value / 100F);
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    this.Invalidate();
                }
            }
        }

        private double _animationSpeed;
        [Browsable(true), DefaultValue(60)]
        public double FPS
        {
            get { return _animationSpeed; }
            set
            {
                if (value > 120) value = 120;
                if (value < 1) value = 1;

                _animationSpeed = value;
                this.drawTimer.Interval = 1000 / _animationSpeed;
                //Invalidate();
            }
        }

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

        protected override void OnFontChanged(EventArgs e)
        {
            this.recalText();
            base.OnFontChanged(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            this.recalText();
            base.OnTextChanged(e);
        }

        private void recalText()
        {
            this.halfsize_game = TextRenderer.MeasureText(str_game, this.Font);
            this.halfsize_start = TextRenderer.MeasureText(this.Text, this.Font);
            this.loc_start = new Point(Convert.ToInt32(halfwidth - (halfsize_start.Width / 2F)), Convert.ToInt32(halfheight - (halfsize_start.Height / 2F)));
            if (!this.halfsize_start.IsEmpty)
                this.textBrush = new LinearGradientBrush(new Rectangle(this.loc_start, this.halfsize_start), Color.Gray, Color.Ivory, LinearGradientMode.BackwardDiagonal);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.drawTimer != null)
            {
                if (this.Visible)
                    this.drawTimer.Start();
                else
                    this.drawTimer.Stop();
            }

            base.OnVisibleChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.Width == this.Height)
            {
                this.SuspendLayout();
                if (innerGrpath == null)
                    innerGrpath = new GraphicsPath();
                this.innerGrpath.ClearMarkers();
                this.innerGrpath.Reset();
                Rectangle anotherRect = this.ClientRectangle;
                anotherRect.Inflate(2, 2);
                anotherRect.Offset(-1, -1);

                this.innerGrpath.AddEllipse(anotherRect);

                //innerbuffer.Graphics.DrawArc(this.OuterPen, 0x12, 0x12, (this.Width - 0x23) - 2, (this.Height - 0x23) - 2, 0, 360);
                if (this.innerRegion != null)
                    this.innerRegion.Dispose();
                this.innerRegion = new Region(this.innerGrpath);
                this.Region = this.innerRegion;
                this.halfwidth = this.Width / 2F;
                this.halfheight = this.Height / 2F;
                this.loc_start = new Point(Convert.ToInt32(halfwidth - (halfsize_start.Width / 2F)), Convert.ToInt32(halfheight - (halfsize_start.Height / 2F)));
                if (!this.halfsize_start.IsEmpty)
                {
                    if (this.textBrush != null)
                        this.textBrush.Dispose();
                    this.textBrush = new LinearGradientBrush(new Rectangle(this.loc_start, this.halfsize_start), Color.Gray, Color.Ivory, LinearGradientMode.BackwardDiagonal);
                }

                if (OuterBrush != null)
                    OuterBrush.Dispose();
                OuterBrush = new LinearGradientBrush(new Rectangle(1, 1, this.Width - 2, this.Height - 2), this._ProgressColor2, this._ProgressColor3, LinearGradientMode.ForwardDiagonal);
                if (OuterPen != null)
                    OuterPen.Dispose();
                OuterPen = new Pen(OuterBrush, 14f);
                if (innerBrush != null)
                    innerBrush.Dispose();
                innerBrush = new LinearGradientBrush(new Rectangle(2, 2, this.Width - 4, this.Height - 4), this._ProgressColor1, this._ProgressColor1, LinearGradientMode.Vertical);

                if (innerbgbuffer != null)
                    innerbgbuffer.Dispose();
                if (!Size.IsEmpty)
                {
                    innerbgbuffer = new DirectBitmap(Size.Width, Size.Height);
                    innerbgbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    innerbgbuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    innerbgbuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    innerbgbuffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                }
                else
                    innerbgbuffer = null;

                if (innerbuffer != null)
                    innerbuffer.Dispose();
                if (!Size.IsEmpty)
                {
                    innerbuffer = new DirectBitmap(Size.Width, Size.Height);
                    //innerbuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
                    innerbuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    innerbuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    innerbuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    innerbuffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                }
                else
                    innerbuffer = null;
                this.ResumeLayout();
                //this.Update();
                this.Refresh();
            }
            else
            {
                int _Size = Math.Max(Width, Height);
                //this.SuspendLayout();
                Size = new Size(_Size, _Size);
                //this.ResumeLayout();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs p)
        {
            if (innerbgbuffer != null)
            {
                innerbgbuffer.Graphics.Clear(this.BackColor);
                if (this.BackgroundImage != null || Parent == null)
                    base.OnPaintBackground(new PaintEventArgs(innerbgbuffer.Graphics, p.ClipRectangle));
                else
                    RadioButtonRenderer.DrawParentBackground(innerbgbuffer.Graphics, p.ClipRectangle, this);
                p.Graphics.DrawImage(innerbgbuffer.Bitmap, p.ClipRectangle, p.ClipRectangle, GraphicsUnit.Pixel);
            }
            else
            {
                base.OnPaintBackground(p);
                if (this.BackColor == Color.Transparent && Parent != null)
                    RadioButtonRenderer.DrawParentBackground(p.Graphics, p.ClipRectangle, this);
            }
        }

        private int cur_rotate;
        private int cur_innerrotate;

        private float IncreaseAngle()
        {
            if (cur_rotate < 0)
                cur_rotate = 0;
            else if (cur_rotate >= 360)
                cur_rotate = 0;
            else
                cur_rotate = cur_rotate + 1;
            if (cur_innerrotate <= 0)
                cur_innerrotate = 360;
            else if (cur_innerrotate > 360)
                cur_innerrotate = 360;
            else
            {
                if (mouseCaptured)
                    cur_innerrotate = cur_innerrotate - 6;
                else
                    cur_innerrotate = cur_innerrotate - 2;
            }
            return cur_rotate;
        }

        private ImageAttributes GetBrightnessAdjuster(float brightness)
        {
            float b = brightness;
            ColorMatrix cm = new ColorMatrix(new float[][]
                {
            new float[] {b, 0, 0, 0, 0},
            new float[] {0, b, 0, 0, 0},
            new float[] {0, 0, b, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1},
                });
            ImageAttributes _attributes = new ImageAttributes();
            _attributes.SetColorMatrix(cm);
            return _attributes;
        }

        private bool mouseCaptured, mouseDown;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.mouseCaptured = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.mouseCaptured = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.mouseCaptured = true;
                this.mouseDown = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                this.mouseDown = false;
            }
        }

        private void DrawTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IncreaseAngle();
            this.Invalidate(false);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (innerbuffer != null)
            {
                if (OuterBrush == null)
                    OuterBrush = new LinearGradientBrush(new Rectangle(1, 1, this.Width - 2, this.Height - 2), this._ProgressColor2, this._ProgressColor3, LinearGradientMode.ForwardDiagonal);
                if (OuterPen == null)
                    OuterPen = new Pen(OuterBrush, 14f);
                if (innerBrush == null)
                    innerBrush = new LinearGradientBrush(new Rectangle(2, 2, this.Width - 4, this.Height - 4), this._ProgressColor1, this._ProgressColor1, LinearGradientMode.Vertical);
                if (this.textBrush == null && !this.halfsize_start.IsEmpty && !this.loc_start.IsEmpty)
                    this.textBrush = new LinearGradientBrush(new Rectangle(this.loc_start, this.halfsize_start), Color.Gray, Color.Ivory, LinearGradientMode.BackwardDiagonal);
                innerbuffer.Graphics.Clear(Color.Transparent);
                //base.OnPaint(new PaintEventArgs(this.innerbuffer.Graphics, pevent.ClipRectangle));
                innerbuffer.Graphics.DrawArc(this.OuterPen, 7, 7, this.Width - 14, this.Height - 14, 0, 360);
                //using (LinearGradientBrush brus = new LinearGradientBrush(this.ClientRectangle, this._ProgressColor1, this._ProgressColor1, LinearGradientMode.Vertical))
                innerbuffer.Graphics.FillEllipse(this.innerBrush, 14, 14, this.Width - 28, this.Height - 28);

                innerbuffer.Graphics.DrawArc(Pens.DarkRed, 1, 1, this.Width - 2, this.Height - 2, 0, 360);

                //pevent.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                innerbuffer.Graphics.DrawString(this.Text, this.Font, this.textBrush, this.loc_start);
                //TextRenderer.DrawText(innerbuffer.Graphics, this.Text, this.Font, this.loc_start, this.ForeColor);

                if (this.mouseCaptured)
                {
                    if (this.mouseDown)
                    {
                        innerbuffer.Graphics.DrawArc(this.penBlue, 1, 1, this.Width - 2, this.Height - 2, -45 + this.cur_rotate, 90);
                        innerbuffer.Graphics.DrawArc(this.penBlue, 1, 1, this.Width - 2, this.Height - 2, 135 + this.cur_rotate, 90);
                        innerbuffer.Graphics.DrawArc(this.penYellow, 14, 14, this.Width - 28, this.Height - 28, this.cur_innerrotate, 35);
                    }
                    else
                    {
                        innerbuffer.Graphics.DrawArc(this.penYellow, 1, 1, this.Width - 2, this.Height - 2, -45 + this.cur_rotate, 90);
                        innerbuffer.Graphics.DrawArc(this.penYellow, 1, 1, this.Width - 2, this.Height - 2, 135 + this.cur_rotate, 90);
                        innerbuffer.Graphics.DrawArc(this.penWhite, 14, 14, this.Width - 28, this.Height - 28, this.cur_innerrotate, 35);
                    }
                }
                else
                {
                    innerbuffer.Graphics.DrawArc(this.penWhite, 1, 1, this.Width - 2, this.Height - 2, -45 + this.cur_rotate, 90);
                    innerbuffer.Graphics.DrawArc(this.penWhite, 1, 1, this.Width - 2, this.Height - 2, 135 + this.cur_rotate, 90);
                    innerbuffer.Graphics.DrawArc(this.penRed, 14, 14, this.Width - 28, this.Height - 28, this.cur_innerrotate, 35);
                }

                //innerbuffer.Graphics.FillEllipse(Brushes.Brown, 0, 0, this.Width, this.Height);
                /*if (this.innerRegion != null)
                {
                    innerbuffer.Graphics.Clear(Color.Transparent);
                    innerbuffer.Graphics.FillRegion(Brushes.White, this.innerRegion);
                }//*/
                if (this.mouseCaptured || this.Opacity == 100)
                {
                    if (this.mouseDown && this.darkAttribute != null)
                        pevent.Graphics.DrawImage(innerbuffer.Bitmap, pevent.ClipRectangle, pevent.ClipRectangle.X, pevent.ClipRectangle.Y, pevent.ClipRectangle.Width, pevent.ClipRectangle.Height, GraphicsUnit.Pixel, this.darkAttribute);
                    else
                        pevent.Graphics.DrawImage(innerbuffer.Bitmap, pevent.ClipRectangle, pevent.ClipRectangle, GraphicsUnit.Pixel);
                }
                else
                    pevent.Graphics.DrawImage(innerbuffer.Bitmap, pevent.ClipRectangle, pevent.ClipRectangle.X, pevent.ClipRectangle.Y, pevent.ClipRectangle.Width, pevent.ClipRectangle.Height, GraphicsUnit.Pixel, attributes);
            }
            else
                base.OnPaint(pevent);
        }
    }
}
