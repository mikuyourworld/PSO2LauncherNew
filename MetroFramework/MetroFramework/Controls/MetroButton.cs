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
	[DefaultEvent("Click")]
	[Designer("MetroFramework.Design.Controls.MetroButtonDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(Button))]
	public class MetroButton : Button, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private bool displayFocusRectangle;

		private bool highlight;

		private MetroButtonSize metroButtonSize;

		private MetroButtonWeight metroButtonWeight = MetroButtonWeight.Bold;

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

		[Category("Metro Appearance")]
		[DefaultValue(MetroButtonSize.Small)]
		public MetroButtonSize FontSize
		{
			get
			{
				return this.metroButtonSize;
			}
			set
			{
				this.metroButtonSize = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroButtonWeight.Bold)]
		public MetroButtonWeight FontWeight
		{
			get
			{
				return this.metroButtonWeight;
			}
			set
			{
				this.metroButtonWeight = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool Highlight
		{
			get
			{
				return this.highlight;
			}
			set
			{
				this.highlight = value;
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

		public MetroButton()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
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
				if (this.isHovered && !this.isPressed && base.Enabled)
				{
					backColor = MetroPaint.BackColor.Button.Hover(this.Theme);
				}
				else if (this.isHovered && this.isPressed && base.Enabled)
				{
					backColor = MetroPaint.BackColor.Button.Press(this.Theme);
				}
				else if (!base.Enabled)
				{
					backColor = MetroPaint.BackColor.Button.Disabled(this.Theme);
				}
				else if (!this.useCustomBackColor)
				{
					backColor = MetroPaint.BackColor.Button.Normal(this.Theme);
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
			Color color;
			Color foreColor;
			if (this.isHovered && !this.isPressed && base.Enabled)
			{
				color = MetroPaint.BorderColor.Button.Hover(this.Theme);
				foreColor = MetroPaint.ForeColor.Button.Hover(this.Theme);
			}
			else if (this.isHovered && this.isPressed && base.Enabled)
			{
				color = MetroPaint.BorderColor.Button.Press(this.Theme);
				foreColor = MetroPaint.ForeColor.Button.Press(this.Theme);
			}
			else if (base.Enabled)
			{
				color = MetroPaint.BorderColor.Button.Normal(this.Theme);
				if (!this.useCustomForeColor)
				{
					foreColor = (!this.useStyleColors ? MetroPaint.ForeColor.Button.Normal(this.Theme) : MetroPaint.GetStyleColor(this.Style));
				}
				else
				{
					foreColor = this.ForeColor;
				}
			}
			else
			{
				color = MetroPaint.BorderColor.Button.Disabled(this.Theme);
				foreColor = MetroPaint.ForeColor.Button.Disabled(this.Theme);
			}
			using (Pen pen = new Pen(color))
			{
				Rectangle rectangle = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
				e.Graphics.DrawRectangle(pen, rectangle);
			}
			if (this.Highlight && !this.isHovered && !this.isPressed && base.Enabled)
			{
				using (Pen stylePen = MetroPaint.GetStylePen(this.Style))
				{
					Rectangle rectangle1 = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
					e.Graphics.DrawRectangle(stylePen, rectangle1);
					rectangle1 = new Rectangle(1, 1, base.Width - 3, base.Height - 3);
					e.Graphics.DrawRectangle(stylePen, rectangle1);
				}
			}
			TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.Button(this.metroButtonSize, this.metroButtonWeight), base.ClientRectangle, foreColor, MetroPaint.GetTextFormatFlags(this.TextAlign));
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