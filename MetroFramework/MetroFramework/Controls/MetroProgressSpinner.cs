using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[Designer("MetroFramework.Design.Controls.MetroProgressSpinnerDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(ProgressBar))]
	public class MetroProgressSpinner : Control, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private Timer timer;

		private int progress;

		private float angle = 270f;

		private int minimum;

		private int maximum = 100;

		private bool ensureVisible = true;

		private float speed;

		private bool backwards;

		private bool useCustomBackground;

		[Category("Metro Behaviour")]
		[DefaultValue(false)]
		public bool Backwards
		{
			get
			{
				return this.backwards;
			}
			set
			{
				this.backwards = value;
				this.Refresh();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool CustomBackground
		{
			get
			{
				return this.useCustomBackground;
			}
			set
			{
				this.useCustomBackground = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(true)]
		public bool EnsureVisible
		{
			get
			{
				return this.ensureVisible;
			}
			set
			{
				this.ensureVisible = value;
				this.Refresh();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(0)]
		public int Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				if (value <= this.minimum)
				{
					throw new ArgumentOutOfRangeException("Maximum value must be > Minimum.", (Exception)null);
				}
				this.maximum = value;
				if (this.progress > this.maximum)
				{
					this.progress = this.maximum;
				}
				this.Refresh();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(0)]
		public int Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Minimum value must be >= 0.", (Exception)null);
				}
				if (value >= this.maximum)
				{
					throw new ArgumentOutOfRangeException("Minimum value must be < Maximum.", (Exception)null);
				}
				this.minimum = value;
				if (this.progress != -1 && this.progress < this.minimum)
				{
					this.progress = this.minimum;
				}
				this.Refresh();
			}
		}

		[Category("Metro Behaviour")]
		[DefaultValue(1f)]
		public float Speed
		{
			get
			{
				return this.speed;
			}
			set
			{
				if (value <= 0f || value > 10f)
				{
					throw new ArgumentOutOfRangeException("Speed value must be > 0 and <= 10.", (Exception)null);
				}
				this.speed = value;
			}
		}

		[Category("Metro Behaviour")]
		[DefaultValue(true)]
		public bool Spinning
		{
			get
			{
				return this.timer.Enabled;
			}
			set
			{
				this.timer.Enabled = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroColorStyle.Default)]
		public MetroColorStyle Style
		{
			get
			{
				if (base.DesignMode || this.metroStyle != MetroColorStyle.Default)
				{
					return this.metroStyle;
				}
				if (this.StyleManager != null && this.metroStyle == MetroColorStyle.Default)
				{
					return this.StyleManager.Style;
				}
				if (this.StyleManager == null && this.metroStyle == MetroColorStyle.Default)
				{
					return MetroColorStyle.Blue;
				}
				return this.metroStyle;
			}
			set
			{
				this.metroStyle = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MetroStyleManager StyleManager
		{
			get
			{
				return this.metroStyleManager;
			}
			set
			{
				this.metroStyleManager = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroThemeStyle.Default)]
		public MetroThemeStyle Theme
		{
			get
			{
				if (base.DesignMode || this.metroTheme != MetroThemeStyle.Default)
				{
					return this.metroTheme;
				}
				if (this.StyleManager != null && this.metroTheme == MetroThemeStyle.Default)
				{
					return this.StyleManager.Theme;
				}
				if (this.StyleManager == null && this.metroTheme == MetroThemeStyle.Default)
				{
					return MetroThemeStyle.Light;
				}
				return this.metroTheme;
			}
			set
			{
				this.metroTheme = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool UseCustomBackColor
		{
			get
			{
				return this.useCustomBackColor;
			}
			set
			{
				this.useCustomBackColor = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool UseCustomForeColor
		{
			get
			{
				return this.useCustomForeColor;
			}
			set
			{
				this.useCustomForeColor = value;
			}
		}

		[Browsable(false)]
		[Category("Metro Behaviour")]
		[DefaultValue(false)]
		public bool UseSelectable
		{
			get
			{
				return base.GetStyle(ControlStyles.Selectable);
			}
			set
			{
				base.SetStyle(ControlStyles.Selectable, value);
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool UseStyleColors
		{
			get
			{
				return this.useStyleColors;
			}
			set
			{
				this.useStyleColors = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(0)]
		public int Value
		{
			get
			{
				return this.progress;
			}
			set
			{
				if (value != -1 && (value < this.minimum || value > this.maximum))
				{
					throw new ArgumentOutOfRangeException("Progress value must be -1 or between Minimum and Maximum.", (Exception)null);
				}
				this.progress = value;
				this.Refresh();
			}
		}

		public MetroProgressSpinner()
		{
			this.timer = new Timer()
			{
				Interval = 20
			};
			this.timer.Tick += new EventHandler(this.timer_Tick);
			this.timer.Enabled = true;
			base.Width = 16;
			base.Height = 16;
			this.speed = 1f;
			this.DoubleBuffered = true;
		}

		protected virtual void OnCustomPaint(MetroPaintEventArgs e)
		{
			if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaint != null)
			{
				this.CustomPaint(this, e);
			}
		}

		protected virtual void OnCustomPaintBackground(MetroPaintEventArgs e)
		{
			if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaintBackground != null)
			{
				this.CustomPaintBackground(this, e);
			}
		}

		protected virtual void OnCustomPaintForeground(MetroPaintEventArgs e)
		{
			if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaintForeground != null)
			{
				this.CustomPaintForeground(this, e);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				if (base.GetStyle(ControlStyles.AllPaintingInWmPaint))
				{
					this.OnPaintBackground(e);
				}
				this.OnCustomPaint(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
				this.OnPaintForeground(e);
			}
			catch
			{
				base.Invalidate();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			try
			{
				Color backColor = this.BackColor;
				if (!this.useCustomBackColor)
				{
					backColor = (!(base.Parent is MetroTile) ? MetroPaint.BackColor.Form(this.Theme) : MetroPaint.GetStyleColor(this.Style));
				}
				if (backColor.A != 255)
				{
					base.OnPaintBackground(e);
					this.OnCustomPaintBackground(new MetroPaintEventArgs(backColor, Color.Empty, e.Graphics));
				}
				else
				{
					e.Graphics.Clear(backColor);
				}
			}
			catch
			{
				base.Invalidate();
			}
		}

		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			Color styleColor;
			float single;
			if (!this.useCustomBackground)
			{
				styleColor = (!(base.Parent is MetroTile) ? MetroPaint.GetStyleColor(this.Style) : MetroPaint.ForeColor.Tile.Normal(this.Theme));
			}
			else
			{
				styleColor = MetroPaint.GetStyleColor(this.Style);
			}
			using (Pen pen = new Pen(styleColor, (float)base.Width / 5f))
			{
				int num = (int)Math.Ceiling((double)((float)base.Width / 10f));
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				if (this.progress == -1)
				{
					for (int i = 0; i <= 180; i = i + 15)
					{
						int num1 = 290 - i * 290 / 180;
						if (num1 > 255)
						{
							num1 = 255;
						}
						if (num1 < 0)
						{
							num1 = 0;
						}
						Color color = Color.FromArgb(num1, pen.Color);
						using (Pen pen1 = new Pen(color, pen.Width))
						{
							float single1 = this.angle + (float)((i - (this.ensureVisible ? 30 : 0)) * (this.backwards ? 1 : -1));
							float single2 = (float)(15 * (this.backwards ? 1 : -1));
							e.Graphics.DrawArc(pen1, (float)num, (float)num, (float)(base.Width - 2 * num - 1), (float)(base.Height - 2 * num - 1), single1, single2);
						}
					}
				}
				else
				{
					float single3 = (float)(this.progress - this.minimum) / (float)(this.maximum - this.minimum);
					single = (!this.ensureVisible ? 360f * single3 : 30f + 300f * single3);
					if (this.backwards)
					{
						single = -single;
					}
					e.Graphics.DrawArc(pen, (float)num, (float)num, (float)(base.Width - 2 * num - 1), (float)(base.Height - 2 * num - 1), this.angle, single);
				}
			}
			this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, styleColor, e.Graphics));
		}

		public void Reset()
		{
			this.progress = this.minimum;
			this.angle = 270f;
			this.Refresh();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (!base.DesignMode)
			{
				MetroProgressSpinner metroProgressSpinner = this;
				metroProgressSpinner.angle = metroProgressSpinner.angle + 6f * this.speed * (float)((this.backwards ? -1 : 1));
				this.Refresh();
			}
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
	}
}