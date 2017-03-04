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
	[ToolboxBitmap(typeof(ComboBox))]
	public class MetroComboBox : ComboBox, IMetroControl
	{
		private const int OCM_COMMAND = 8465;

		private const int WM_PAINT = 15;

		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private bool displayFocusRectangle;

		private MetroComboBoxSize metroComboBoxSize = MetroComboBoxSize.Medium;

		private MetroComboBoxWeight metroComboBoxWeight = MetroComboBoxWeight.Regular;

		private string promptText = "";

		private bool drawPrompt;

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
		[DefaultValue(System.Windows.Forms.DrawMode.OwnerDrawFixed)]
		public new System.Windows.Forms.DrawMode DrawMode
		{
			get
			{
				return System.Windows.Forms.DrawMode.OwnerDrawFixed;
			}
			set
			{
				base.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			}
		}

		[Browsable(false)]
		[DefaultValue(ComboBoxStyle.DropDownList)]
		public new ComboBoxStyle DropDownStyle
		{
			get
			{
				return ComboBoxStyle.DropDownList;
			}
			set
			{
				base.DropDownStyle = ComboBoxStyle.DropDownList;
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
		[DefaultValue(MetroComboBoxSize.Medium)]
		public MetroComboBoxSize FontSize
		{
			get
			{
				return this.metroComboBoxSize;
			}
			set
			{
				this.metroComboBoxSize = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroComboBoxWeight.Regular)]
		public MetroComboBoxWeight FontWeight
		{
			get
			{
				return this.metroComboBoxWeight;
			}
			set
			{
				this.metroComboBoxWeight = value;
			}
		}

		[Browsable(true)]
		[Category("Metro Appearance")]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public string PromptText
		{
			get
			{
				return this.promptText;
			}
			set
			{
				this.promptText = value.Trim();
				base.Invalidate();
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

		public MetroComboBox()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
			base.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			base.DropDownStyle = ComboBoxStyle.DropDownList;
			this.drawPrompt = this.SelectedIndex == -1;
		}

		private void DrawTextPrompt()
		{
			using (Graphics graphic = base.CreateGraphics())
			{
				this.DrawTextPrompt(graphic);
			}
		}

		private void DrawTextPrompt(Graphics g)
		{
			Color backColor = this.BackColor;
			if (!this.useCustomBackColor)
			{
				backColor = MetroPaint.BackColor.Form(this.Theme);
			}
			Rectangle rectangle = new Rectangle(2, 2, base.Width - 20, base.Height - 4);
			TextRenderer.DrawText(g, this.promptText, MetroFonts.ComboBox(this.metroComboBoxSize, this.metroComboBoxWeight), rectangle, SystemColors.GrayText, backColor, TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
		}

		public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
		{
			System.Drawing.Size height;
			base.GetPreferredSize(proposedSize);
			using (Graphics graphic = base.CreateGraphics())
			{
				string str = (this.Text.Length > 0 ? this.Text : "MeasureText");
				proposedSize = new System.Drawing.Size(2147483647, 2147483647);
				height = TextRenderer.MeasureText(graphic, str, MetroFonts.ComboBox(this.metroComboBoxSize, this.metroComboBoxWeight), proposedSize, TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
				height.Height = height.Height + 4;
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

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			Color color;
			if (e.Index < 0)
			{
				base.OnDrawItem(e);
				return;
			}
			Color backColor = this.BackColor;
			if (!this.useCustomBackColor)
			{
				backColor = MetroPaint.BackColor.Form(this.Theme);
			}
			if (e.State == (DrawItemState.NoAccelerator | DrawItemState.NoFocusRect) || e.State == DrawItemState.None)
			{
				using (SolidBrush solidBrush = new SolidBrush(backColor))
				{
					Graphics graphics = e.Graphics;
					int left = e.Bounds.Left;
					int top = e.Bounds.Top;
					int width = e.Bounds.Width;
					Rectangle bounds = e.Bounds;
					graphics.FillRectangle(solidBrush, new Rectangle(left, top, width, bounds.Height));
				}
				color = MetroPaint.ForeColor.Link.Normal(this.Theme);
			}
			else
			{
				using (SolidBrush solidBrush1 = new SolidBrush(MetroPaint.GetStyleColor(this.Style)))
				{
					Graphics graphic = e.Graphics;
					int num = e.Bounds.Left;
					int top1 = e.Bounds.Top;
					int width1 = e.Bounds.Width;
					Rectangle rectangle = e.Bounds;
					graphic.FillRectangle(solidBrush1, new Rectangle(num, top1, width1, rectangle.Height));
				}
				color = MetroPaint.ForeColor.Tile.Normal(this.Theme);
			}
			int num1 = e.Bounds.Top;
			int width2 = e.Bounds.Width;
			Rectangle bounds1 = e.Bounds;
			Rectangle rectangle1 = new Rectangle(0, num1, width2, bounds1.Height);
			TextRenderer.DrawText(e.Graphics, base.GetItemText(base.Items[e.Index]), MetroFonts.ComboBox(this.metroComboBoxSize, this.metroComboBoxWeight), rectangle1, color, TextFormatFlags.VerticalCenter);
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
			Color color;
			Color color1;
			base.ItemHeight = this.GetPreferredSize(System.Drawing.Size.Empty).Height;
			if (this.isHovered && !this.isPressed && base.Enabled)
			{
				color1 = MetroPaint.ForeColor.ComboBox.Hover(this.Theme);
				color = MetroPaint.BorderColor.ComboBox.Hover(this.Theme);
			}
			else if (this.isHovered && this.isPressed && base.Enabled)
			{
				color1 = MetroPaint.ForeColor.ComboBox.Press(this.Theme);
				color = MetroPaint.BorderColor.ComboBox.Press(this.Theme);
			}
			else if (base.Enabled)
			{
				color1 = MetroPaint.ForeColor.ComboBox.Normal(this.Theme);
				color = MetroPaint.BorderColor.ComboBox.Normal(this.Theme);
			}
			else
			{
				color1 = MetroPaint.ForeColor.ComboBox.Disabled(this.Theme);
				color = MetroPaint.BorderColor.ComboBox.Disabled(this.Theme);
			}
			using (Pen pen = new Pen(color))
			{
				Rectangle rectangle = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
				e.Graphics.DrawRectangle(pen, rectangle);
			}
			using (SolidBrush solidBrush = new SolidBrush(color1))
			{
				Graphics graphics = e.Graphics;
				Point[] point = new Point[] { new Point(base.Width - 20, base.Height / 2 - 2), new Point(base.Width - 9, base.Height / 2 - 2), new Point(base.Width - 15, base.Height / 2 + 4) };
				graphics.FillPolygon(solidBrush, point);
			}
			Rectangle rectangle1 = new Rectangle(2, 2, base.Width - 20, base.Height - 4);
			TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.ComboBox(this.metroComboBoxSize, this.metroComboBoxWeight), rectangle1, color1, TextFormatFlags.VerticalCenter);
			this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, color1, e.Graphics));
			if (this.displayFocusRectangle && this.isFocused)
			{
				ControlPaint.DrawFocusRectangle(e.Graphics, base.ClientRectangle);
			}
			if (this.drawPrompt)
			{
				this.DrawTextPrompt(e.Graphics);
			}
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			this.drawPrompt = this.SelectedIndex == -1;
			base.Invalidate();
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if ((m.Msg == 15 || m.Msg == 8465) && this.drawPrompt)
			{
				this.DrawTextPrompt();
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