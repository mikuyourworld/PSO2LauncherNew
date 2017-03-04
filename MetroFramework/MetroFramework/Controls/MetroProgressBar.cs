using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[Designer("MetroFramework.Design.Controls.MetroProgressBarDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(ProgressBar))]
	public class MetroProgressBar : ProgressBar, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors = true;

		private MetroProgressBarSize metroLabelSize = MetroProgressBarSize.Medium;

		private MetroProgressBarWeight metroLabelWeight;

		private ContentAlignment textAlign = ContentAlignment.MiddleRight;

		private bool hideProgressText = true;

		private System.Windows.Forms.ProgressBarStyle progressBarStyle = System.Windows.Forms.ProgressBarStyle.Continuous;

		private int marqueeX;

		private Timer marqueeTimer;

		[Category("Metro Appearance")]
		[DefaultValue(MetroProgressBarSize.Medium)]
		public MetroProgressBarSize FontSize
		{
			get
			{
				return this.metroLabelSize;
			}
			set
			{
				this.metroLabelSize = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroProgressBarWeight.Light)]
		public MetroProgressBarWeight FontWeight
		{
			get
			{
				return this.metroLabelWeight;
			}
			set
			{
				this.metroLabelWeight = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(true)]
		public bool HideProgressText
		{
			get
			{
				return this.hideProgressText;
			}
			set
			{
				this.hideProgressText = value;
			}
		}

		private bool marqueeTimerEnabled
		{
			get
			{
				if (this.marqueeTimer == null)
				{
					return false;
				}
				return this.marqueeTimer.Enabled;
			}
		}

		private int ProgressBarMarqueeWidth
		{
			get
			{
				return base.ClientRectangle.Width / 3;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(System.Windows.Forms.ProgressBarStyle.Continuous)]
		public System.Windows.Forms.ProgressBarStyle ProgressBarStyle
		{
			get
			{
				return this.progressBarStyle;
			}
			set
			{
				this.progressBarStyle = value;
			}
		}

		private double ProgressBarWidth
		{
			get
			{
				return (double)this.Value / (double)base.Maximum * (double)base.ClientRectangle.Width;
			}
		}

		[Browsable(false)]
		public string ProgressPercentText
		{
			get
			{
				return string.Format("{0}%", Math.Round(this.ProgressTotalPercent));
			}
		}

		[Browsable(false)]
		public double ProgressTotalPercent
		{
			get
			{
				return (1 - (double)(base.Maximum - this.Value) / (double)base.Maximum) * 100;
			}
		}

		[Browsable(false)]
		public double ProgressTotalValue
		{
			get
			{
				return 1 - (double)(base.Maximum - this.Value) / (double)base.Maximum;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroColorStyle.Default)]
		public new MetroColorStyle Style
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
		[DefaultValue(ContentAlignment.MiddleRight)]
		public ContentAlignment TextAlign
		{
			get
			{
				return this.textAlign;
			}
			set
			{
				this.textAlign = value;
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

		[Browsable(false)]
		[Category("Metro Appearance")]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[Browsable(false)]
		[Category("Metro Appearance")]
		[DefaultValue(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		public new int Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				if (value > base.Maximum)
				{
					return;
				}
				base.Value = value;
				base.Invalidate();
			}
		}

		public MetroProgressBar()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
		}

		private void DrawProgressContinuous(Graphics graphics)
		{
			SolidBrush styleBrush = MetroPaint.GetStyleBrush(this.Style);
			int progressBarWidth = (int)this.ProgressBarWidth;
			Rectangle clientRectangle = base.ClientRectangle;
			graphics.FillRectangle(styleBrush, 0, 0, progressBarWidth, clientRectangle.Height);
		}

		private void DrawProgressMarquee(Graphics graphics)
		{
			SolidBrush styleBrush = MetroPaint.GetStyleBrush(this.Style);
			int num = this.marqueeX;
			int progressBarMarqueeWidth = this.ProgressBarMarqueeWidth;
			Rectangle clientRectangle = base.ClientRectangle;
			graphics.FillRectangle(styleBrush, num, 0, progressBarMarqueeWidth, clientRectangle.Height);
		}

		private void DrawProgressText(Graphics graphics)
		{
			Color color;
			if (this.HideProgressText)
			{
				return;
			}
			color = (base.Enabled ? MetroPaint.ForeColor.ProgressBar.Normal(this.Theme) : MetroPaint.ForeColor.ProgressBar.Disabled(this.Theme));
			TextRenderer.DrawText(graphics, this.ProgressPercentText, MetroFonts.ProgressBar(this.metroLabelSize, this.metroLabelWeight), base.ClientRectangle, color, MetroPaint.GetTextFormatFlags(this.TextAlign));
		}

		public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
		{
			System.Drawing.Size size;
			base.GetPreferredSize(proposedSize);
			using (Graphics graphic = base.CreateGraphics())
			{
				proposedSize = new System.Drawing.Size(2147483647, 2147483647);
				size = TextRenderer.MeasureText(graphic, this.ProgressPercentText, MetroFonts.ProgressBar(this.metroLabelSize, this.metroLabelWeight), proposedSize, MetroPaint.GetTextFormatFlags(this.TextAlign));
			}
			return size;
		}

		private void marqueeTimer_Tick(object sender, EventArgs e)
		{
			MetroProgressBar metroProgressBar = this;
			metroProgressBar.marqueeX = metroProgressBar.marqueeX + 1;
			if (this.marqueeX > base.ClientRectangle.Width)
			{
				this.marqueeX = -this.ProgressBarMarqueeWidth;
			}
			base.Invalidate();
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
					backColor = (base.Enabled ? MetroPaint.BackColor.ProgressBar.Bar.Normal(this.Theme) : MetroPaint.BackColor.ProgressBar.Bar.Disabled(this.Theme));
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
			if (this.progressBarStyle == System.Windows.Forms.ProgressBarStyle.Continuous)
			{
				if (!base.DesignMode)
				{
					this.StopTimer();
				}
				this.DrawProgressContinuous(e.Graphics);
			}
			else if (this.progressBarStyle == System.Windows.Forms.ProgressBarStyle.Blocks)
			{
				if (!base.DesignMode)
				{
					this.StopTimer();
				}
				this.DrawProgressContinuous(e.Graphics);
			}
			else if (this.progressBarStyle == System.Windows.Forms.ProgressBarStyle.Marquee)
			{
				if (!base.DesignMode && base.Enabled)
				{
					this.StartTimer();
				}
				if (!base.Enabled)
				{
					this.StopTimer();
				}
				if (this.Value != base.Maximum)
				{
					this.DrawProgressMarquee(e.Graphics);
				}
				else
				{
					this.StopTimer();
					this.DrawProgressContinuous(e.Graphics);
				}
			}
			this.DrawProgressText(e.Graphics);
			using (Pen pen = new Pen(MetroPaint.BorderColor.ProgressBar.Normal(this.Theme)))
			{
				Rectangle rectangle = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
				e.Graphics.DrawRectangle(pen, rectangle);
			}
			this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
		}

		private void StartTimer()
		{
			if (this.marqueeTimerEnabled)
			{
				return;
			}
			if (this.marqueeTimer == null)
			{
				this.marqueeTimer = new Timer()
				{
					Interval = 10
				};
				this.marqueeTimer.Tick += new EventHandler(this.marqueeTimer_Tick);
			}
			this.marqueeX = -this.ProgressBarMarqueeWidth;
			this.marqueeTimer.Stop();
			this.marqueeTimer.Start();
			this.marqueeTimer.Enabled = true;
			base.Invalidate();
		}

		private void StopTimer()
		{
			if (this.marqueeTimer == null)
			{
				return;
			}
			this.marqueeTimer.Stop();
			base.Invalidate();
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
	}
}