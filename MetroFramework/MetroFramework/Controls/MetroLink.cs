using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[DefaultEvent("Click")]
	[Designer("MetroFramework.Design.Controls.MetroLinkDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(LinkLabel))]
	public class MetroLink : Button, IMetroControl
	{
		private bool displayFocusRectangle;

		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private System.Drawing.Image _image;

		private System.Drawing.Image _nofocus;

		private int _imagesize = 16;

		private MetroLinkSize metroLinkSize;

		private MetroLinkWeight metroLinkWeight = MetroLinkWeight.Bold;

		private bool isHovered;

		private bool isPressed;

		private bool isFocused;

		private Color foreColor;

		private System.Drawing.Image _lightlightimg;

		private System.Drawing.Image _darklightimg;

		private System.Drawing.Image _lightimg;

		private System.Drawing.Image _darkimg;

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
		[DefaultValue(MetroLinkWeight.Bold)]
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

		[Category("Metro Appearance")]
		[DefaultValue(null)]
		public new virtual System.Drawing.Image Image
		{
			get
			{
				return this._image;
			}
			set
			{
				this._image = value;
				this.createimages();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(16)]
		public int ImageSize
		{
			get
			{
				return this._imagesize;
			}
			set
			{
				this._imagesize = value;
				base.Invalidate();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(null)]
		public System.Drawing.Image NoFocusImage
		{
			get
			{
				return this._nofocus;
			}
			set
			{
				this._nofocus = value;
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

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				if (this.AutoSize && this._image != null)
				{
					System.Drawing.Size size = TextRenderer.MeasureText(value, MetroFonts.Link(this.metroLinkSize, this.metroLinkWeight));
					base.Width = size.Width;
					MetroLink width = this;
					width.Width = width.Width + this._imagesize + 2;
				}
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

		public MetroLink()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
		}

		public Bitmap ApplyInvert(Bitmap bitmapImage)
		{
			for (int i = 0; i < bitmapImage.Height; i++)
			{
				for (int j = 0; j < bitmapImage.Width; j++)
				{
					Color pixel = bitmapImage.GetPixel(j, i);
					byte a = pixel.A;
					byte r = (byte)(255 - pixel.R);
					byte g = (byte)(255 - pixel.G);
					byte b = (byte)(255 - pixel.B);
					bitmapImage.SetPixel(j, i, Color.FromArgb((int)a, (int)r, (int)g, (int)b));
				}
			}
			return bitmapImage;
		}

		public Bitmap ApplyLight(Bitmap bitmapImage)
		{
			for (int i = 0; i < bitmapImage.Height; i++)
			{
				for (int j = 0; j < bitmapImage.Width; j++)
				{
					Color pixel = bitmapImage.GetPixel(j, i);
					byte a = pixel.A;
					if (pixel.A <= 255 && pixel.A >= 100)
					{
						a = 90;
					}
					byte r = pixel.R;
					byte g = pixel.G;
					byte b = pixel.B;
					bitmapImage.SetPixel(j, i, Color.FromArgb((int)a, (int)r, (int)g, (int)b));
				}
			}
			return bitmapImage;
		}

		private void createimages()
		{
			if (this._image != null)
			{
				this._lightimg = this._image;
				this._darkimg = this.ApplyInvert(new Bitmap(this._image));
				this._darklightimg = this.ApplyLight(new Bitmap(this._darkimg));
				this._lightlightimg = this.ApplyLight(new Bitmap(this._lightimg));
			}
		}

		private void DrawIcon(Graphics g)
		{
			if (this.Image != null)
			{
				int width = this._imagesize;
				int height = this._imagesize;
				if (this._imagesize == 0)
				{
					width = this._image.Width;
					height = this._image.Height;
				}
				Rectangle clientRectangle = base.ClientRectangle;
				Point point = new Point(2, (clientRectangle.Height - this._imagesize) / 2);
				int num = 0;
				ContentAlignment imageAlign = base.ImageAlign;
				if (imageAlign <= ContentAlignment.MiddleCenter)
				{
					switch (imageAlign)
					{
						case ContentAlignment.TopLeft:
						{
							point = new Point(num, num);
							break;
						}
						case ContentAlignment.TopCenter:
						{
							Rectangle rectangle = base.ClientRectangle;
							point = new Point((rectangle.Width - width) / 2, num);
							break;
						}
						case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
						{
							break;
						}
						case ContentAlignment.TopRight:
						{
							Rectangle clientRectangle1 = base.ClientRectangle;
							point = new Point(clientRectangle1.Width - width - num, num);
							break;
						}
						default:
						{
							if (imageAlign == ContentAlignment.MiddleLeft)
							{
								Rectangle rectangle1 = base.ClientRectangle;
								point = new Point(num, (rectangle1.Height - height) / 2);
								break;
							}
							else if (imageAlign == ContentAlignment.MiddleCenter)
							{
								int width1 = (base.ClientRectangle.Width - width) / 2;
								Rectangle clientRectangle2 = base.ClientRectangle;
								point = new Point(width1, (clientRectangle2.Height - height) / 2);
								break;
							}
							else
							{
								break;
							}
						}
					}
				}
				else if (imageAlign <= ContentAlignment.BottomLeft)
				{
					if (imageAlign == ContentAlignment.MiddleRight)
					{
						Rectangle rectangle2 = base.ClientRectangle;
						Rectangle clientRectangle3 = base.ClientRectangle;
						point = new Point(rectangle2.Width - width - num, (clientRectangle3.Height - height) / 2);
					}
					else if (imageAlign == ContentAlignment.BottomLeft)
					{
						Rectangle rectangle3 = base.ClientRectangle;
						point = new Point(num, rectangle3.Height - height - num);
					}
				}
				else if (imageAlign == ContentAlignment.BottomCenter)
				{
					int num1 = (base.ClientRectangle.Width - width) / 2;
					Rectangle clientRectangle4 = base.ClientRectangle;
					point = new Point(num1, clientRectangle4.Height - height - num);
				}
				else if (imageAlign == ContentAlignment.BottomRight)
				{
					Rectangle rectangle4 = base.ClientRectangle;
					Rectangle clientRectangle5 = base.ClientRectangle;
					point = new Point(rectangle4.Width - width - num, clientRectangle5.Height - height - num);
				}
				point.Y = point.Y + 1;
				if (this._nofocus == null)
				{
					if (this.Theme == MetroThemeStyle.Dark)
					{
						g.DrawImage((!this.isHovered || this.isPressed ? this._darklightimg : this._darkimg), new Rectangle(point, new System.Drawing.Size(width, height)));
						return;
					}
					g.DrawImage((!this.isHovered || this.isPressed ? this._lightlightimg : this._lightimg), new Rectangle(point, new System.Drawing.Size(width, height)));
					return;
				}
				if (this.Theme == MetroThemeStyle.Dark)
				{
					g.DrawImage((!this.isHovered || this.isPressed ? this._nofocus : this._darkimg), new Rectangle(point, new System.Drawing.Size(width, height)));
					return;
				}
				g.DrawImage((!this.isHovered || this.isPressed ? this._nofocus : this._image), new Rectangle(point, new System.Drawing.Size(width, height)));
			}
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
			this.isPressed = true;
			base.Invalidate();
			base.OnEnter(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			this.isFocused = true;
			this.isPressed = false;
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
			if (!this.isFocused)
			{
				this.isHovered = false;
				this.isPressed = false;
			}
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
				if (base.Name == "lnkClear" && base.Parent.GetType().Name == "MetroTextBox")
				{
					base.PerformClick();
				}
				if (base.Name == "lnkClear" && base.Parent.GetType().Name == "SearchControl")
				{
					base.PerformClick();
				}
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
			this.isHovered = false;
			this.isPressed = false;
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
			if (this.useCustomForeColor)
			{
				this.foreColor = this.ForeColor;
			}
			else if (this.isHovered && !this.isPressed && base.Enabled)
			{
				this.foreColor = MetroPaint.ForeColor.Link.Normal(this.Theme);
			}
			else if (this.isHovered && this.isPressed && base.Enabled)
			{
				this.foreColor = MetroPaint.ForeColor.Link.Press(this.Theme);
			}
			else if (base.Enabled)
			{
				this.foreColor = (!this.useStyleColors ? MetroPaint.ForeColor.Link.Hover(this.Theme) : MetroPaint.GetStyleColor(this.Style));
			}
			else
			{
				this.foreColor = MetroPaint.ForeColor.Link.Disabled(this.Theme);
			}
			TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.Link(this.metroLinkSize, this.metroLinkWeight), base.ClientRectangle, this.foreColor, MetroPaint.GetTextFormatFlags(this.TextAlign));
			this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, this.foreColor, e.Graphics));
			if (this.displayFocusRectangle && this.isFocused)
			{
				ControlPaint.DrawFocusRectangle(e.Graphics, base.ClientRectangle);
			}
			if (this._image != null)
			{
				this.DrawIcon(e.Graphics);
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