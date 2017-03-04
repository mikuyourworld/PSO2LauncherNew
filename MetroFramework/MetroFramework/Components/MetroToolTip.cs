using MetroFramework;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework.Components
{
	[ToolboxBitmap(typeof(ToolTip))]
	public class MetroToolTip : ToolTip, IMetroComponent
	{
		private MetroColorStyle metroStyle = MetroColorStyle.Blue;

		private MetroThemeStyle metroTheme = MetroThemeStyle.Light;

		private MetroStyleManager metroStyleManager;

		[Browsable(false)]
		public new Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[Browsable(false)]
		public new Color ForeColor
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

		[Browsable(false)]
		public new bool IsBalloon
		{
			get
			{
				return base.IsBalloon;
			}
			set
			{
				base.IsBalloon = false;
			}
		}

		[Browsable(false)]
		[DefaultValue(true)]
		public new bool OwnerDraw
		{
			get
			{
				return base.OwnerDraw;
			}
			set
			{
				base.OwnerDraw = true;
			}
		}

		[Browsable(false)]
		[DefaultValue(true)]
		public new bool ShowAlways
		{
			get
			{
				return base.ShowAlways;
			}
			set
			{
				base.ShowAlways = true;
			}
		}

		[Category("Metro Appearance")]
		public MetroColorStyle Style
		{
			get
			{
				if (this.StyleManager == null)
				{
					return this.metroStyle;
				}
				return this.StyleManager.Style;
			}
			set
			{
				this.metroStyle = value;
			}
		}

		[Browsable(false)]
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
		public MetroThemeStyle Theme
		{
			get
			{
				if (this.StyleManager == null)
				{
					return this.metroTheme;
				}
				return this.StyleManager.Theme;
			}
			set
			{
				this.metroTheme = value;
			}
		}

		[Browsable(false)]
		public new System.Windows.Forms.ToolTipIcon ToolTipIcon
		{
			get
			{
				return base.ToolTipIcon;
			}
			set
			{
				base.ToolTipIcon = System.Windows.Forms.ToolTipIcon.None;
			}
		}

		[Browsable(false)]
		public new string ToolTipTitle
		{
			get
			{
				return base.ToolTipTitle;
			}
			set
			{
				base.ToolTipTitle = "";
			}
		}

		public MetroToolTip()
		{
			this.OwnerDraw = true;
			this.ShowAlways = true;
			base.Draw += new DrawToolTipEventHandler(this.MetroToolTip_Draw);
			base.Popup += new PopupEventHandler(this.MetroToolTip_Popup);
		}

		private void MetroToolTip_Draw(object sender, DrawToolTipEventArgs e)
		{
			MetroThemeStyle metroThemeStyle = (this.Theme == MetroThemeStyle.Light ? MetroThemeStyle.Dark : MetroThemeStyle.Light);
			Color color = MetroPaint.BackColor.Form(metroThemeStyle);
			Color color1 = MetroPaint.BorderColor.Button.Normal(metroThemeStyle);
			Color color2 = MetroPaint.ForeColor.Label.Normal(metroThemeStyle);
			using (SolidBrush solidBrush = new SolidBrush(color))
			{
				e.Graphics.FillRectangle(solidBrush, e.Bounds);
			}
			using (Pen pen = new Pen(color1))
			{
				Graphics graphics = e.Graphics;
				int x = e.Bounds.X;
				int y = e.Bounds.Y;
				Rectangle bounds = e.Bounds;
				Rectangle rectangle = e.Bounds;
				graphics.DrawRectangle(pen, new Rectangle(x, y, bounds.Width - 1, rectangle.Height - 1));
			}
			Font font = MetroFonts.Default(13f);
			TextRenderer.DrawText(e.Graphics, e.ToolTipText, font, e.Bounds, color2, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
		}

		private void MetroToolTip_Popup(object sender, PopupEventArgs e)
		{
			if (e.AssociatedWindow is IMetroForm)
			{
				this.Style = ((IMetroForm)e.AssociatedWindow).Style;
				this.Theme = ((IMetroForm)e.AssociatedWindow).Theme;
				this.StyleManager = ((IMetroForm)e.AssociatedWindow).StyleManager;
			}
			else if (e.AssociatedControl is IMetroControl)
			{
				this.Style = ((IMetroControl)e.AssociatedControl).Style;
				this.Theme = ((IMetroControl)e.AssociatedControl).Theme;
				this.StyleManager = ((IMetroControl)e.AssociatedControl).StyleManager;
			}
			Size toolTipSize = e.ToolTipSize;
			Size size = e.ToolTipSize;
			e.ToolTipSize = new Size(toolTipSize.Width + 24, size.Height + 9);
		}

		public new void SetToolTip(Control control, string caption)
		{
			base.SetToolTip(control, caption);
			if (control is IMetroControl)
			{
				foreach (Control control1 in control.Controls)
				{
					this.SetToolTip(control1, caption);
				}
			}
		}
	}
}