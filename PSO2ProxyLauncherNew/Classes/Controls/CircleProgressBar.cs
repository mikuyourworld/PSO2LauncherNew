using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private long _Value;
        private string _ValuePercentString, _ValueString, _MaximumString;
        private long _Maximum;
        private Color _ProgressColor1 = Color.FromArgb(92, 92, 92);
        private Color _ProgressColor2 = Color.FromArgb(92, 92, 92);
        private _ProgressShape ProgressShapeVal;

        #endregion
        #region Custom Properties

        public long Value
        {
            get { return _Value; }
            set
            {
                if (value > _Maximum)
                    value = _Maximum;
                _Value = value;
                _ValueString = Convert.ToString(value);
                _ValuePercentString = Convert.ToString(Convert.ToInt32(_Value * 100 / _Maximum)) + "%";
                Invalidate();
            }
        }
        
        public bool ShowSmallText { get; set; }

        public long Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value < 1)
                    value = 1;
                _Maximum = value;
                _MaximumString = Convert.ToString(value);
                Invalidate();
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

        public Font SmallTextFont { get; set; }

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
            base.OnPaintBackground(p);
        }

        #endregion

        public CircleProgressBar()
        {
            Size = new Size(130, 130);
            Font = new Font("Segoe UI", 15);
            SmallTextFont = new Font("Segoe UI", 10);
            MinimumSize = new Size(100, 100);
            DoubleBuffered = true;
            _Maximum = 100;
            _MaximumString = "100";
            ShowSmallText = false;
            Value = 0;
        }

        private void SetStandardSize()
        {
            int _Size = Math.Max(Width, Height);
            Size = new Size(_Size, _Size);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Bitmap bitmap = new Bitmap(this.Width, this.Height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(this.BackColor);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
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
                    graphics.DrawArc(pen, 0x12, 0x12, (this.Width - 0x23) - 2, (this.Height - 0x23) - 2, -90, (int)Math.Round((double)((360.0 / ((double)this._Maximum)) * this._Value)));
                }
                using (LinearGradientBrush brush2 = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(0x34, 0x34, 0x34), Color.FromArgb(0x34, 0x34, 0x34), LinearGradientMode.Vertical))
                    graphics.FillEllipse(brush2, 0x18, 0x18, (this.Width - 0x30) - 1, (this.Height - 0x30) - 1);
                SizeF MS = TextRenderer.MeasureText(this._ValuePercentString, this.Font);
                float textHalfwidth = MS.Width / 2;
                double X = Width * 0.4;
                TextRenderer.DrawText(graphics, this._ValuePercentString, this.Font, new Point(Convert.ToInt32(X - textHalfwidth), Convert.ToInt32(Height / 2 - MS.Height / 2)), this.ForeColor);
                //graphics.DrawString(this._ValueString, this.Font, Brushes.White, Convert.ToInt32(Width / 2 - MS.Width / 2), Convert.ToInt32(Height / 2 - MS.Height / 2));
                if (this.ShowSmallText)
                {
                    int smallX = Convert.ToInt32(X + textHalfwidth + 1);

                    MS = TextRenderer.MeasureText(this._ValueString, this.SmallTextFont);
                    float smalltextHalfwidth = MS.Width / 2;
                    double h = Height * 0.35;
                    TextRenderer.DrawText(graphics, this._ValueString, this.SmallTextFont, new Point(Convert.ToInt32(smallX - smalltextHalfwidth), Convert.ToInt32(h - MS.Height / 2)), this.ForeColor);

                    MS = TextRenderer.MeasureText(this._MaximumString, this.SmallTextFont);
                    smalltextHalfwidth = MS.Width / 2;
                    TextRenderer.DrawText(graphics, this._MaximumString, this.SmallTextFont, new Point(Convert.ToInt32(smallX - smalltextHalfwidth), Convert.ToInt32((Height - h) - MS.Height / 2)), this.ForeColor);
                }
                e.Graphics.DrawImage(bitmap, 0, 0);
            }
        }
    }
}
