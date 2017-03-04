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
	public class MetroContextMenu : System.Windows.Forms.ContextMenuStrip, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

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
				this.settheme();
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

		public MetroContextMenu(IContainer Container)
		{
			if (Container != null)
			{
				Container.Add(this);
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

		private void settheme()
		{
			base.BackColor = MetroPaint.BackColor.Form(this.Theme);
			base.ForeColor = MetroPaint.ForeColor.Button.Normal(this.Theme);
			base.Renderer = new MetroContextMenu.MetroCTXRenderer(this.Theme, this.Style);
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

		private class contextcolors : ProfessionalColorTable
		{
			private MetroThemeStyle _theme;

			private MetroColorStyle _style;

			public override Color ImageMarginGradientBegin
			{
				get
				{
					return MetroPaint.BackColor.Form(this._theme);
				}
			}

			public override Color ImageMarginGradientEnd
			{
				get
				{
					return MetroPaint.BackColor.Form(this._theme);
				}
			}

			public override Color ImageMarginGradientMiddle
			{
				get
				{
					return MetroPaint.BackColor.Form(this._theme);
				}
			}

			public override Color MenuBorder
			{
				get
				{
					return MetroPaint.BackColor.Form(this._theme);
				}
			}

			public override Color MenuItemBorder
			{
				get
				{
					return MetroPaint.GetStyleColor(this._style);
				}
			}

			public override Color MenuItemSelected
			{
				get
				{
					return MetroPaint.GetStyleColor(this._style);
				}
			}

			public override Color ToolStripBorder
			{
				get
				{
					return MetroPaint.GetStyleColor(this._style);
				}
			}

			public override Color ToolStripDropDownBackground
			{
				get
				{
					return MetroPaint.BackColor.Form(this._theme);
				}
			}

			public contextcolors(MetroThemeStyle Theme, MetroColorStyle Style)
			{
				this._theme = Theme;
				this._style = Style;
			}
		}

		private class MetroCTXRenderer : ToolStripProfessionalRenderer
		{
			private MetroThemeStyle _theme;

			public MetroCTXRenderer(MetroThemeStyle Theme, MetroColorStyle Style) : base(new MetroContextMenu.contextcolors(Theme, Style))
			{
				this._theme = Theme;
			}

			protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
			{
				e.TextColor = MetroPaint.ForeColor.Button.Normal(this._theme);
				base.OnRenderItemText(e);
			}
		}
	}
}