using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Localization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[Designer("MetroFramework.Design.Controls.MetroToggleDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(CheckBox))]
	public class MetroToggle : CheckBox, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private bool displayFocusRectangle;

		private MetroLocalize metroLocalize;

		private MetroLinkSize metroLinkSize;

		private MetroLinkWeight metroLinkWeight = MetroLinkWeight.Regular;

		private bool displayStatus = true;

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
		[DefaultValue(true)]
		public bool DisplayStatus
		{
			get
			{
				return this.displayStatus;
			}
			set
			{
				this.displayStatus = value;
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
		[DefaultValue(MetroLinkSize.Small)]
		public MetroLinkSize FontSize
		{
			get
			{
				return this.metroLinkSize;
			}
			set
			{
				this.metroLinkSize = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroLinkWeight.Regular)]
		public MetroLinkWeight FontWeight
		{
			get
			{
				return this.metroLinkWeight;
			}
			set
			{
				this.metroLinkWeight = value;
			}
		}

		[Browsable(false)]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
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

		[Browsable(false)]
		public override string Text
		{
			get
			{
				if (base.Checked)
				{
					return this.metroLocalize.translate("StatusOn");
				}
				return this.metroLocalize.translate("StatusOff");
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

		public MetroToggle()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			base.Name = "MetroToggle";
			this.metroLocalize = new MetroLocalize(this);
		}

		public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
		{
			System.Drawing.Size preferredSize = base.GetPreferredSize(proposedSize);
			preferredSize.Width = (this.DisplayStatus ? 80 : 50);
			return preferredSize;
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
			Color color1;
			int num;
			int num1;
			int num2;
			int num3;
			int width;
			int width1;
			if (this.isHovered && !this.isPressed && base.Enabled)
			{
				color1 = MetroPaint.ForeColor.CheckBox.Hover(this.Theme);
				color = MetroPaint.BorderColor.CheckBox.Hover(this.Theme);
			}
			else if (this.isHovered && this.isPressed && base.Enabled)
			{
				color1 = MetroPaint.ForeColor.CheckBox.Press(this.Theme);
				color = MetroPaint.BorderColor.CheckBox.Press(this.Theme);
			}
			else if (base.Enabled)
			{
				color1 = (!this.useStyleColors ? MetroPaint.ForeColor.CheckBox.Normal(this.Theme) : MetroPaint.GetStyleColor(this.Style));
				color = MetroPaint.BorderColor.CheckBox.Normal(this.Theme);
			}
			else
			{
				color1 = MetroPaint.ForeColor.CheckBox.Disabled(this.Theme);
				color = MetroPaint.BorderColor.CheckBox.Disabled(this.Theme);
			}
			using (Pen pen = new Pen(color))
			{
				num1 = (this.DisplayStatus ? 30 : 0);
				int width2 = base.ClientRectangle.Width;
				num = (this.DisplayStatus ? 31 : 1);
				Rectangle clientRectangle = base.ClientRectangle;
				Rectangle rectangle = new Rectangle(num1, 0, width2 - num, clientRectangle.Height - 1);
				e.Graphics.DrawRectangle(pen, rectangle);
			}
			using (SolidBrush solidBrush = new SolidBrush((base.Checked ? MetroPaint.GetStyleColor(this.Style) : MetroPaint.BorderColor.CheckBox.Normal(this.Theme))))
			{
				num3 = (this.DisplayStatus ? 32 : 2);
				int width3 = base.ClientRectangle.Width;
				num2 = (this.DisplayStatus ? 34 : 4);
				Rectangle clientRectangle1 = base.ClientRectangle;
				Rectangle rectangle1 = new Rectangle(num3, 2, width3 - num2, clientRectangle1.Height - 4);
				e.Graphics.FillRectangle(solidBrush, rectangle1);
			}
			Color backColor = this.BackColor;
			if (!this.useCustomBackColor)
			{
				backColor = MetroPaint.BackColor.Form(this.Theme);
			}
			using (SolidBrush solidBrush1 = new SolidBrush(backColor))
			{
				if (base.Checked)
				{
					width = base.Width - 11;
				}
				else
				{
					width = (this.DisplayStatus ? 30 : 0);
				}
				int num4 = width;
				Rectangle clientRectangle2 = base.ClientRectangle;
				Rectangle rectangle2 = new Rectangle(num4, 0, 11, clientRectangle2.Height);
				e.Graphics.FillRectangle(solidBrush1, rectangle2);
			}
			using (SolidBrush solidBrush2 = new SolidBrush(MetroPaint.BorderColor.CheckBox.Hover(this.Theme)))
			{
				if (base.Checked)
				{
					width1 = base.Width - 10;
				}
				else
				{
					width1 = (this.DisplayStatus ? 30 : 0);
				}
				int num5 = width1;
				Rectangle clientRectangle3 = base.ClientRectangle;
				Rectangle rectangle3 = new Rectangle(num5, 0, 10, clientRectangle3.Height);
				e.Graphics.FillRectangle(solidBrush2, rectangle3);
			}
			if (this.DisplayStatus)
			{
				Rectangle clientRectangle4 = base.ClientRectangle;
				Rectangle rectangle4 = new Rectangle(0, 0, 30, clientRectangle4.Height);
				TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.Link(this.metroLinkSize, this.metroLinkWeight), rectangle4, color1, MetroPaint.GetTextFormatFlags(this.TextAlign));
			}
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