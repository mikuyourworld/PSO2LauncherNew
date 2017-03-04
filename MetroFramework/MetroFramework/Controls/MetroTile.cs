using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[Designer("MetroFramework.Design.Controls.MetroTileDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(Button))]
	public class MetroTile : Button, IContainerControl, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private Control activeControl;

		private bool paintTileCount = true;

		private int tileCount;

		private System.Drawing.Image tileImage;

		private bool useTileImage;

		private ContentAlignment tileImageAlign = ContentAlignment.TopLeft;

		private MetroTileTextSize tileTextFontSize = MetroTileTextSize.Medium;

		private MetroTileTextWeight tileTextFontWeight;

		private bool isHovered;

		private bool isPressed;

		private bool isFocused;

		[Browsable(false)]
		public Control ActiveControl
		{
			get
			{
				return this.activeControl;
			}
			set
			{
				this.activeControl = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(true)]
		public bool PaintTileCount
		{
			get
			{
				return this.paintTileCount;
			}
			set
			{
				this.paintTileCount = value;
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

		[DefaultValue(ContentAlignment.BottomLeft)]
		public new ContentAlignment TextAlign
		{
			get
			{
				return base.TextAlign;
			}
			set
			{
				base.TextAlign = value;
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

		[DefaultValue(0)]
		public int TileCount
		{
			get
			{
				return this.tileCount;
			}
			set
			{
				this.tileCount = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(null)]
		public System.Drawing.Image TileImage
		{
			get
			{
				return this.tileImage;
			}
			set
			{
				this.tileImage = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(ContentAlignment.TopLeft)]
		public ContentAlignment TileImageAlign
		{
			get
			{
				return this.tileImageAlign;
			}
			set
			{
				this.tileImageAlign = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroTileTextSize.Medium)]
		public MetroTileTextSize TileTextFontSize
		{
			get
			{
				return this.tileTextFontSize;
			}
			set
			{
				this.tileTextFontSize = value;
				this.Refresh();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroTileTextWeight.Light)]
		public MetroTileTextWeight TileTextFontWeight
		{
			get
			{
				return this.tileTextFontWeight;
			}
			set
			{
				this.tileTextFontWeight = value;
				this.Refresh();
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
		[DefaultValue(false)]
		public bool UseTileImage
		{
			get
			{
				return this.useTileImage;
			}
			set
			{
				this.useTileImage = value;
			}
		}

		public MetroTile()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			this.TextAlign = ContentAlignment.BottomLeft;
		}

		public bool ActivateControl(Control ctrl)
		{
			if (!base.Controls.Contains(ctrl))
			{
				return false;
			}
			ctrl.Select();
			this.activeControl = ctrl;
			return true;
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
					backColor = MetroPaint.GetStyleColor(this.Style);
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
			Color foreColor;
			Rectangle rectangle;
			Color color = MetroPaint.BorderColor.Button.Normal(this.Theme);
			if (this.isHovered && !this.isPressed && base.Enabled)
			{
				foreColor = MetroPaint.ForeColor.Tile.Hover(this.Theme);
			}
			else if (!this.isHovered || !this.isPressed || !base.Enabled)
			{
				foreColor = (base.Enabled ? MetroPaint.ForeColor.Tile.Normal(this.Theme) : MetroPaint.ForeColor.Tile.Disabled(this.Theme));
			}
			else
			{
				foreColor = MetroPaint.ForeColor.Tile.Press(this.Theme);
			}
			if (this.useCustomForeColor)
			{
				foreColor = this.ForeColor;
			}
			if (this.isPressed || this.isHovered || this.isFocused)
			{
				using (Pen pen = new Pen(color))
				{
					pen.Width = 3f;
					Rectangle rectangle1 = new Rectangle(1, 1, base.Width - 3, base.Height - 3);
					e.Graphics.DrawRectangle(pen, rectangle1);
				}
			}
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
			if (this.useTileImage && this.tileImage != null)
			{
				ContentAlignment contentAlignment = this.tileImageAlign;
				if (contentAlignment <= ContentAlignment.MiddleCenter)
                    switch (contentAlignment)
                    {
                        case ContentAlignment.TopLeft:
                            rectangle = new Rectangle(new Point(0, 0), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                            break;
                        case ContentAlignment.TopCenter:
                            rectangle = new Rectangle(new Point(base.Width / 2 - this.TileImage.Width / 2, 0), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                            break;
                        case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
                            rectangle = new Rectangle(new Point(0, 0), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                            break;
                        case ContentAlignment.TopRight:
                            rectangle = new Rectangle(new Point(base.Width - this.TileImage.Width, 0), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                            break;
                        default:
                            if (contentAlignment == ContentAlignment.MiddleLeft)
                            {
                                rectangle = new Rectangle(new Point(0, base.Height / 2 - this.TileImage.Height / 2), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                                break;
                            }
                            else if (contentAlignment == ContentAlignment.MiddleCenter)
                            {
                                rectangle = new Rectangle(new Point(base.Width / 2 - this.TileImage.Width / 2, base.Height / 2 - this.TileImage.Height / 2), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                                break;
                            }
                            else
                                rectangle = new Rectangle(new Point(0, 0), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                            break;
                    }
				else if (contentAlignment <= ContentAlignment.BottomLeft)
				{
					if (contentAlignment == ContentAlignment.MiddleRight)
					{
						rectangle = new Rectangle(new Point(base.Width - this.TileImage.Width, base.Height / 2 - this.TileImage.Height / 2), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
					}
					else
					{
                        if (contentAlignment != ContentAlignment.BottomLeft)
                            rectangle = new Rectangle(new Point(0, 0), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                        else
                            rectangle = new Rectangle(new Point(0, base.Height - this.TileImage.Height), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
					}
				}
				else if (contentAlignment == ContentAlignment.BottomCenter)
				{
					rectangle = new Rectangle(new Point(base.Width / 2 - this.TileImage.Width / 2, base.Height - this.TileImage.Height), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
				}
				else
				{
                    if (contentAlignment != ContentAlignment.BottomRight)
                        rectangle = new Rectangle(new Point(0, 0), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
                    else
                        rectangle = new Rectangle(new Point(base.Width - this.TileImage.Width, base.Height - this.TileImage.Height), new System.Drawing.Size(this.TileImage.Width, this.TileImage.Height));
				}
				e.Graphics.DrawImage(this.TileImage, rectangle);
			}
			if (this.TileCount > 0 && this.paintTileCount)
			{
				int tileCount = this.TileCount;
				System.Drawing.Size size = TextRenderer.MeasureText(tileCount.ToString(), MetroFonts.TileCount);
				e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				Graphics graphics = e.Graphics;
				int num = this.TileCount;
				TextRenderer.DrawText(graphics, num.ToString(), MetroFonts.TileCount, new Point(base.Width - size.Width, 0), foreColor);
				e.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
			}
			TextRenderer.MeasureText(this.Text, MetroFonts.Tile(this.tileTextFontSize, this.tileTextFontWeight));
			TextFormatFlags textFormatFlags = MetroPaint.GetTextFormatFlags(this.TextAlign) | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;
			Rectangle clientRectangle = base.ClientRectangle;
			if (!this.isPressed)
			{
				clientRectangle.Inflate(-2, -10);
			}
			else
			{
				clientRectangle.Inflate(-4, -12);
			}
			TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.Tile(this.tileTextFontSize, this.tileTextFontWeight), clientRectangle, foreColor, textFormatFlags);
			return;
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
	}
}