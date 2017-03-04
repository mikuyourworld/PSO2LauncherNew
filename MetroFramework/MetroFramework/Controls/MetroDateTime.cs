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
	[ToolboxBitmap(typeof(DateTimePicker))]
	public class MetroDateTime : DateTimePicker, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private bool displayFocusRectangle;

		private MetroDateTimeSize metroDateTimeSize = MetroDateTimeSize.Medium;

		private MetroDateTimeWeight metroDateTimeWeight = MetroDateTimeWeight.Regular;

		private bool isHovered;

		private bool isPressed;

		private bool isFocused;

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool DisplayFocus
		{
			get
			{
				return this.displayFocusRectangle;
			}
			set
			{
				this.displayFocusRectangle = value;
			}
		}

		[Browsable(false)]
		public override System.Drawing.Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroDateTimeSize.Medium)]
		public MetroDateTimeSize FontSize
		{
			get
			{
				return this.metroDateTimeSize;
			}
			set
			{
				this.metroDateTimeSize = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroDateTimeWeight.Regular)]
		public MetroDateTimeWeight FontWeight
		{
			get
			{
				return this.metroDateTimeWeight;
			}
			set
			{
				this.metroDateTimeWeight = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(false)]
		public new bool ShowUpDown
		{
			get
			{
				return base.ShowUpDown;
			}
			set
			{
				base.ShowUpDown = false;
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
		[DefaultValue(true)]
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

		public MetroDateTime()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
		}

		public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
		{
			System.Drawing.Size height;
			base.GetPreferredSize(proposedSize);
			using (Graphics graphic = base.CreateGraphics())
			{
				string str = (this.Text.Length > 0 ? this.Text : "MeasureText");
				proposedSize = new System.Drawing.Size(2147483647, 2147483647);
				height = TextRenderer.MeasureText(graphic, str, MetroFonts.DateTime(this.metroDateTimeSize, this.metroDateTimeWeight), proposedSize, TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
				height.Height = height.Height + 10;
			}
			return height;
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

		protected override void OnEnter(EventArgs e)
		{
			this.isFocused = true;
			this.isHovered = true;
			base.Invalidate();
			base.OnEnter(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			this.isFocused = true;
			this.isHovered = true;
			base.Invalidate();
			base.OnGotFocus(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				this.isHovered = true;
				this.isPressed = true;
				base.Invalidate();
			}
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.Invalidate();
			base.OnKeyUp(e);
		}

		protected override void OnLeave(EventArgs e)
		{
			this.isFocused = false;
			this.isHovered = false;
			this.isPressed = false;
			base.Invalidate();
			base.OnLeave(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			this.isFocused = false;
			this.isHovered = false;
			this.isPressed = false;
			base.Invalidate();
			base.OnLostFocus(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this.isPressed = true;
				base.Invalidate();
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			this.isHovered = true;
			base.Invalidate();
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (!this.isFocused)
			{
				this.isHovered = false;
			}
			base.Invalidate();
			base.OnMouseLeave(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.isPressed = false;
			base.Invalidate();
			base.OnMouseUp(e);
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
					backColor = MetroPaint.BackColor.Form(this.Theme);
				}
				if (backColor.A != 255 || this.BackgroundImage != null)
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
			Color color;
			System.Drawing.Size preferredSize = this.GetPreferredSize(System.Drawing.Size.Empty);
			this.MinimumSize = new System.Drawing.Size(0, preferredSize.Height);
			if (this.isHovered && !this.isPressed && base.Enabled)
			{
				color = MetroPaint.ForeColor.ComboBox.Hover(this.Theme);
				styleColor = MetroPaint.GetStyleColor(this.Style);
			}
			else if (this.isHovered && this.isPressed && base.Enabled)
			{
				color = MetroPaint.ForeColor.ComboBox.Press(this.Theme);
				styleColor = MetroPaint.GetStyleColor(this.Style);
			}
			else if (base.Enabled)
			{
				color = MetroPaint.ForeColor.ComboBox.Normal(this.Theme);
				styleColor = MetroPaint.BorderColor.ComboBox.Normal(this.Theme);
			}
			else
			{
				color = MetroPaint.ForeColor.ComboBox.Disabled(this.Theme);
				styleColor = MetroPaint.BorderColor.ComboBox.Disabled(this.Theme);
			}
			using (Pen pen = new Pen(styleColor))
			{
				Rectangle rectangle = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
				e.Graphics.DrawRectangle(pen, rectangle);
			}
			using (SolidBrush solidBrush = new SolidBrush(color))
			{
				Graphics graphics = e.Graphics;
				Point[] point = new Point[] { new Point(base.Width - 20, base.Height / 2 - 2), new Point(base.Width - 9, base.Height / 2 - 2), new Point(base.Width - 15, base.Height / 2 + 4) };
				graphics.FillPolygon(solidBrush, point);
			}
			int num = 0;
			if (base.ShowCheckBox)
			{
				num = 15;
				using (Pen pen1 = new Pen(styleColor))
				{
					Rectangle rectangle1 = new Rectangle(3, base.Height / 2 - 6, 12, 12);
					e.Graphics.DrawRectangle(pen1, rectangle1);
				}
				if (!base.Checked)
				{
					color = MetroPaint.ForeColor.ComboBox.Disabled(this.Theme);
				}
				else
				{
					using (SolidBrush solidBrush1 = new SolidBrush(MetroPaint.GetStyleColor(this.Style)))
					{
						Rectangle rectangle2 = new Rectangle(5, base.Height / 2 - 4, 9, 9);
						e.Graphics.FillRectangle(solidBrush1, rectangle2);
					}
				}
			}
			Rectangle rectangle3 = new Rectangle(2 + num, 2, base.Width - 20, base.Height - 4);
			TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.DateTime(this.metroDateTimeSize, this.metroDateTimeWeight), rectangle3, color, TextFormatFlags.VerticalCenter);
			this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, color, e.Graphics));
			if (this.displayFocusRectangle && this.isFocused)
			{
				ControlPaint.DrawFocusRectangle(e.Graphics, base.ClientRectangle);
			}
		}

		protected override void OnValueChanged(EventArgs eventargs)
		{
			base.OnValueChanged(eventargs);
			base.Invalidate();
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
	}
}