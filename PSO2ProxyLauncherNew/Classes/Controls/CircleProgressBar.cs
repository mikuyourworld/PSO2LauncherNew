using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class CircleProgressBar : Control
    {
        #region Enums

        public enum _ProgressShape
        {
            Round,
            Flat
        }

        #endregion
        #region Variables

        private long _Value = -1;
        private string _ValuePercentString, _ValueString, _MaximumString;
        private long _Maximum;
        private Color _ProgressColor1 = Color.FromArgb(92, 92, 92);
        private Color _ProgressColor2 = Color.FromArgb(92, 92, 92);
        private _ProgressShape ProgressShapeVal;
        private Components.DirectBitmap innerbuffer, innerbgbuffer;
        ColorMatrix matrix;
        ImageAttributes attributes;

        #endregion
        #region Custom Properties

        public long Value
        {
            get { return _Value; }
            set
            {
                if (value > _Maximum)
                    value = _Maximum;
                if (value != _Value)
                {
                    _Value = value;
                    _ValueString = Convert.ToString(value);
                    _ValuePercentString = Convert.ToString(Convert.ToInt32(_Value * 100 / _Maximum)) + "%";
                    Invalidate();
                }
            }
        }
        
        public bool ShowSmallText { get; set; }

        public long Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value != _Maximum)
                {
                    if (value < 1)
                        value = 1;
                    _Maximum = value;
                    _MaximumString = Convert.ToString(value);
                    Invalidate();
                }
            }
        }

        public Color ProgressColor1
        {
            get { return _ProgressColor1; }
            set
            {
                _ProgressColor1 = value;
                Invalidate();
            }
        }

        public Color ProgressColor2
        {
            get { return _ProgressColor2; }
            set
            {
                _ProgressColor2 = value;
                Invalidate();
            }
        }

        public _ProgressShape ProgressShape
        {
            get { return ProgressShapeVal; }
            set
            {
                ProgressShapeVal = value;
                Invalidate();
            }
        }

        public Font _SmallTextFont;
        public Font SmallTextFont
        {
            get { return this._SmallTextFont; }
            set
            {
                if (value != _SmallTextFont)
                {
                    _SmallTextFont = value;
                    Invalidate();
                }
            }
        }

        #endregion
        #region EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetStandardSize();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetStandardSize();
        }

        protected override void OnPaintBackground(PaintEventArgs p)
        {
            if (innerbgbuffer != null)
            {
                innerbgbuffer.Graphics.Clear(this.BackColor);
                if (this.BackColor != Color.Transparent || Parent == null)
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

        #endregion

        public CircleProgressBar() : base()
        {            
            Size = new Size(130, 130);
            Font = new Font(this.Font.FontFamily, 15);
            SmallTextFont = new Font(this.Font.FontFamily, 10);
            MinimumSize = new Size(100, 100);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserMouse, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            DoubleBuffered = true;
            Maximum = 100;
            ShowSmallText = false;
            Value = 0;
            matrix = new ColorMatrix();
            attributes = new ImageAttributes();
            this.Opacity = 100;
            this.UpdateStyles();
        }

        public new void Dispose()
        {
            base.Dispose();
            _ValuePercentString = null;
            _ValueString = null;
            _MaximumString = null;
            if (innerbuffer != null)
                innerbuffer.Dispose();
            if (innerbgbuffer != null)
                innerbgbuffer.Dispose();
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

        private void SetStandardSize()
        {
            if (this.Width == this.Height)
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
            else
            {
                int _Size = Math.Max(Width, Height);
                Size = new Size(_Size, _Size);
            }
            
        }

        public void Increment(int Val)
        {
            this._Value += Val;
            Invalidate();
        }

        public void Decrement(int Val)
        {
            this._Value -= Val;
            Invalidate();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (innerbuffer != null)
            {
                innerbuffer.Graphics.Clear(this.BackColor);
                base.OnPaint(new PaintEventArgs(innerbuffer.Graphics, e.ClipRectangle));
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, this._ProgressColor1, this._ProgressColor2, LinearGradientMode.ForwardDiagonal))
                using (Pen pen = new Pen(brush, 14f))
                {
                    switch (this.ProgressShapeVal)
                    {
                        case _ProgressShape.Round:
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;
                            break;

                        case _ProgressShape.Flat:
                            pen.StartCap = LineCap.Flat;
                            pen.EndCap = LineCap.Flat;
                            break;
                    }
                    innerbuffer.Graphics.DrawArc(pen, 0x12, 0x12, (this.Width - 0x23) - 2, (this.Height - 0x23) - 2, -90, (int)Math.Round((double)((360.0 / ((double)this._Maximum)) * this._Value)));
                }
                //System.Windows.Media.BitmapCache
                using (LinearGradientBrush brush2 = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(0x34, 0x34, 0x34), Color.FromArgb(0x34, 0x34, 0x34), LinearGradientMode.Vertical))
                    innerbuffer.Graphics.FillEllipse(brush2, 0x18, 0x18, (this.Width - 0x30) - 1, (this.Height - 0x30) - 1);
                //graphics.DrawString(this._ValueString, this.Font, Brushes.White, Convert.ToInt32(Width / 2 - MS.Width / 2), Convert.ToInt32(Height / 2 - MS.Height / 2));
                if (this.ShowSmallText)
                {
                    SizeF MS = TextRenderer.MeasureText(this._ValuePercentString, this.Font);
                    float textHalfwidth = MS.Width / 2;
                    double X = Width * 0.4;
                    TextRenderer.DrawText(innerbuffer.Graphics, this._ValuePercentString, this.Font, new Point(Convert.ToInt32(X - textHalfwidth), Convert.ToInt32(Height / 2 - MS.Height / 2)), this.ForeColor);

                    int smallX = Convert.ToInt32(X + textHalfwidth + 1);

                    MS = TextRenderer.MeasureText(this._ValueString, this.SmallTextFont);
                    float smalltextHalfwidth = MS.Width / 2;
                    double h = Height * 0.35;
                    TextRenderer.DrawText(innerbuffer.Graphics, this._ValueString, this.SmallTextFont, new Point(Convert.ToInt32(smallX - smalltextHalfwidth), Convert.ToInt32(h - MS.Height / 2)), this.ForeColor);

                    MS = TextRenderer.MeasureText(this._MaximumString, this.SmallTextFont);
                    smalltextHalfwidth = MS.Width / 2;
                    TextRenderer.DrawText(innerbuffer.Graphics, this._MaximumString, this.SmallTextFont, new Point(Convert.ToInt32(smallX - smalltextHalfwidth), Convert.ToInt32((Height - h) - MS.Height / 2)), this.ForeColor);
                }
                else
                {
                    SizeF MS = TextRenderer.MeasureText(this._ValuePercentString, this.Font);
                    TextRenderer.DrawText(innerbuffer.Graphics, this._ValuePercentString, this.Font, new Point(Convert.ToInt32(Width / 2 - MS.Width / 2), Convert.ToInt32(Height / 2 - MS.Height / 2)), this.ForeColor);
                }
                
                if (this.Opacity == 100)
                    e.Graphics.DrawImage(innerbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
                else
                    e.Graphics.DrawImage(innerbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height, GraphicsUnit.Pixel, attributes);
            }
        }
    }
}
