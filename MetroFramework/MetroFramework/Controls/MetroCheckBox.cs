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
	[Designer("MetroFramework.Design.Controls.MetroCheckBoxDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(CheckBox))]
	public class MetroCheckBox : CheckBox, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private bool displayFocusRectangle;

		private MetroCheckBoxSize metroCheckBoxSize;

		private MetroCheckBoxWeight metroCheckBoxWeight = MetroCheckBoxWeight.Regular;

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
		[DefaultValue(MetroCheckBoxSize.Small)]
		public MetroCheckBoxSize FontSize
		{
			get
			{
				return this.metroCheckBoxSize;
			}
			set
			{
				this.metroCheckBoxSize = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroCheckBoxWeight.Regular)]
		public MetroCheckBoxWeight FontWeight
		{
			get
			{
				return this.metroCheckBoxWeight;
			}
			set
			{
				this.metroCheckBoxWeight = value;
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

		public MetroCheckBox()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
		}

		public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
		{
			System.Drawing.Size width;
			base.GetPreferredSize(proposedSize);
			using (Graphics graphic = base.CreateGraphics())
			{
				proposedSize = new System.Drawing.Size(2147483647, 2147483647);
				width = TextRenderer.MeasureText(graphic, this.Text, MetroFonts.CheckBox(this.metroCheckBoxSize, this.metroCheckBoxWeight), proposedSize, MetroPaint.GetTextFormatFlags(this.TextAlign));
				width.Width = width.Width + 16;
				if (base.CheckAlign == ContentAlignment.TopCenter || base.CheckAlign == ContentAlignment.BottomCenter)
				{
					width.Height = width.Height + 16;
				}
			}
			return width;
		}

		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);
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

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			base.Invalidate();
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
					if (base.Parent is MetroTile)
					{
						backColor = MetroPaint.GetStyleColor(this.Style);
					}
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
			Color color;
			Color foreColor;
			if (this.useCustomForeColor)
			{
				foreColor = this.ForeColor;
				if (this.isHovered && !this.isPressed && base.Enabled)
				{
					color = MetroPaint.BorderColor.CheckBox.Hover(this.Theme);
				}
				else if (!this.isHovered || !this.isPressed || !base.Enabled)
				{
					color = (base.Enabled ? MetroPaint.BorderColor.CheckBox.Normal(this.Theme) : MetroPaint.BorderColor.CheckBox.Disabled(this.Theme));
				}
				else
				{
					color = MetroPaint.BorderColor.CheckBox.Press(this.Theme);
				}
			}
			else if (this.isHovered && !this.isPressed && base.Enabled)
			{
				foreColor = MetroPaint.ForeColor.CheckBox.Hover(this.Theme);
				color = MetroPaint.BorderColor.CheckBox.Hover(this.Theme);
			}
			else if (this.isHovered && this.isPressed && base.Enabled)
			{
				foreColor = MetroPaint.ForeColor.CheckBox.Press(this.Theme);
				color = MetroPaint.BorderColor.CheckBox.Press(this.Theme);
			}
			else if (base.Enabled)
			{
				foreColor = (!this.useStyleColors ? MetroPaint.ForeColor.CheckBox.Normal(this.Theme) : MetroPaint.GetStyleColor(this.Style));
				color = MetroPaint.BorderColor.CheckBox.Normal(this.Theme);
			}
			else
			{
				foreColor = MetroPaint.ForeColor.CheckBox.Disabled(this.Theme);
				color = MetroPaint.BorderColor.CheckBox.Disabled(this.Theme);
			}
			Rectangle rectangle = new Rectangle(16, 0, base.Width - 16, base.Height);
			Rectangle rectangle1 = new Rectangle(0, base.Height / 2 - 6, 12, 12);
			using (Pen pen = new Pen(color))
			{
				ContentAlignment checkAlign = base.CheckAlign;
				if (checkAlign <= ContentAlignment.MiddleCenter)
				{
					switch (checkAlign)
					{
						case ContentAlignment.TopLeft:
						{
							rectangle1 = new Rectangle(0, 0, 12, 12);
							break;
						}
						case ContentAlignment.TopCenter:
						{
							rectangle1 = new Rectangle(base.Width / 2 - 6, 0, 12, 12);
							rectangle = new Rectangle(16, rectangle1.Top + rectangle1.Height - 5, base.Width - 8, base.Height);
							break;
						}
						case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
						{
							break;
						}
						case ContentAlignment.TopRight:
						{
							rectangle1 = new Rectangle(base.Width - 13, 0, 12, 12);
							rectangle = new Rectangle(0, 0, base.Width - 16, base.Height);
							break;
						}
						default:
						{
							if (checkAlign == ContentAlignment.MiddleLeft)
							{
								rectangle1 = new Rectangle(0, base.Height / 2 - 6, 12, 12);
								break;
							}
							else if (checkAlign == ContentAlignment.MiddleCenter)
							{
								rectangle1 = new Rectangle(base.Width / 2 - 6, base.Height / 2 - 6, 12, 12);
								break;
							}
							else
							{
								break;
							}
						}
					}
				}
				else if (checkAlign <= ContentAlignment.BottomLeft)
				{
					if (checkAlign == ContentAlignment.MiddleRight)
					{
						rectangle1 = new Rectangle(base.Width - 13, base.Height / 2 - 6, 12, 12);
						rectangle = new Rectangle(0, 0, base.Width - 16, base.Height);
					}
					else if (checkAlign == ContentAlignment.BottomLeft)
					{
						rectangle1 = new Rectangle(0, base.Height - 13, 12, 12);
					}
				}
				else if (checkAlign == ContentAlignment.BottomCenter)
				{
					rectangle1 = new Rectangle(base.Width / 2 - 6, base.Height - 13, 12, 12);
					rectangle = new Rectangle(16, -10, base.Width - 8, base.Height);
				}
				else if (checkAlign == ContentAlignment.BottomRight)
				{
					rectangle1 = new Rectangle(base.Width - 13, base.Height - 13, 12, 12);
					rectangle = new Rectangle(0, 0, base.Width - 16, base.Height);
				}
				e.Graphics.DrawRectangle(pen, rectangle1);
			}
			if (base.Checked)
			{
				using (SolidBrush solidBrush = new SolidBrush((base.CheckState == System.Windows.Forms.CheckState.Indeterminate ? color : MetroPaint.GetStyleColor(this.Style))))
				{
					Rectangle rectangle2 = new Rectangle(rectangle1.Left + 2, rectangle1.Top + 2, 9, 9);
					e.Graphics.FillRectangle(solidBrush, rectangle2);
				}
			}
			TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.CheckBox(this.metroCheckBoxSize, this.metroCheckBoxWeight), rectangle, foreColor, MetroPaint.GetTextFormatFlags(this.TextAlign));
			this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, foreColor, e.Graphics));
			if (this.displayFocusRectangle && this.isFocused)
			{
				ControlPaint.DrawFocusRectangle(e.Graphics, base.ClientRectangle);
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