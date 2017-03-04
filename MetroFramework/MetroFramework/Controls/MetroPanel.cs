using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[ToolboxBitmap(typeof(Panel))]
	public class MetroPanel : Panel, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private MetroScrollBar verticalScrollbar = new MetroScrollBar(MetroScrollOrientation.Vertical);

		private MetroScrollBar horizontalScrollbar = new MetroScrollBar(MetroScrollOrientation.Horizontal);

		private bool showHorizontalScrollbar;

		private bool showVerticalScrollbar;

		[Category("Metro Appearance")]
		public new bool AutoScroll
		{
			get
			{
				return base.AutoScroll;
			}
			set
			{
				this.showHorizontalScrollbar = value;
				this.showVerticalScrollbar = value;
				base.AutoScroll = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool HorizontalScrollbar
		{
			get
			{
				return this.showHorizontalScrollbar;
			}
			set
			{
				this.showHorizontalScrollbar = value;
			}
		}

		[Category("Metro Appearance")]
		public bool HorizontalScrollbarBarColor
		{
			get
			{
				return this.horizontalScrollbar.UseBarColor;
			}
			set
			{
				this.horizontalScrollbar.UseBarColor = value;
			}
		}

		[Category("Metro Appearance")]
		public bool HorizontalScrollbarHighlightOnWheel
		{
			get
			{
				return this.horizontalScrollbar.HighlightOnWheel;
			}
			set
			{
				this.horizontalScrollbar.HighlightOnWheel = value;
			}
		}

		[Category("Metro Appearance")]
		public int HorizontalScrollbarSize
		{
			get
			{
				return this.horizontalScrollbar.ScrollbarSize;
			}
			set
			{
				this.horizontalScrollbar.ScrollbarSize = value;
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

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool VerticalScrollbar
		{
			get
			{
				return this.showVerticalScrollbar;
			}
			set
			{
				this.showVerticalScrollbar = value;
			}
		}

		[Category("Metro Appearance")]
		public bool VerticalScrollbarBarColor
		{
			get
			{
				return this.verticalScrollbar.UseBarColor;
			}
			set
			{
				this.verticalScrollbar.UseBarColor = value;
			}
		}

		[Category("Metro Appearance")]
		public bool VerticalScrollbarHighlightOnWheel
		{
			get
			{
				return this.verticalScrollbar.HighlightOnWheel;
			}
			set
			{
				this.verticalScrollbar.HighlightOnWheel = value;
			}
		}

		[Category("Metro Appearance")]
		public int VerticalScrollbarSize
		{
			get
			{
				return this.verticalScrollbar.ScrollbarSize;
			}
			set
			{
				this.verticalScrollbar.ScrollbarSize = value;
			}
		}

		public MetroPanel()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
			base.Controls.Add(this.verticalScrollbar);
			base.Controls.Add(this.horizontalScrollbar);
			this.verticalScrollbar.UseBarColor = true;
			this.horizontalScrollbar.UseBarColor = true;
			this.verticalScrollbar.Visible = false;
			this.horizontalScrollbar.Visible = false;
			this.verticalScrollbar.Scroll += new ScrollEventHandler(this.VerticalScrollbarScroll);
			this.horizontalScrollbar.Scroll += new ScrollEventHandler(this.HorizontalScrollbarScroll);
		}

		private void HorizontalScrollbarScroll(object sender, ScrollEventArgs e)
		{
			base.AutoScrollPosition = new Point(e.NewValue, this.verticalScrollbar.Value);
			this.UpdateScrollBarPositions();
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

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			this.verticalScrollbar.Value = Math.Abs(base.VerticalScroll.Value);
			this.horizontalScrollbar.Value = Math.Abs(base.HorizontalScroll.Value);
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
			if (base.DesignMode)
			{
				this.horizontalScrollbar.Visible = false;
				this.verticalScrollbar.Visible = false;
				return;
			}
			this.UpdateScrollBarPositions();
			if (this.HorizontalScrollbar)
			{
				this.horizontalScrollbar.Visible = base.HorizontalScroll.Visible;
			}
			if (base.HorizontalScroll.Visible)
			{
				this.horizontalScrollbar.Minimum = base.HorizontalScroll.Minimum;
				this.horizontalScrollbar.Maximum = base.HorizontalScroll.Maximum;
				this.horizontalScrollbar.SmallChange = base.HorizontalScroll.SmallChange;
				this.horizontalScrollbar.LargeChange = base.HorizontalScroll.LargeChange;
			}
			if (this.VerticalScrollbar)
			{
				this.verticalScrollbar.Visible = base.VerticalScroll.Visible;
			}
			if (base.VerticalScroll.Visible)
			{
				this.verticalScrollbar.Minimum = base.VerticalScroll.Minimum;
				this.verticalScrollbar.Maximum = base.VerticalScroll.Maximum;
				this.verticalScrollbar.SmallChange = base.VerticalScroll.SmallChange;
				this.verticalScrollbar.LargeChange = base.VerticalScroll.LargeChange;
			}
			this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
		}

		private void UpdateScrollBarPositions()
		{
			if (base.DesignMode)
			{
				return;
			}
			if (!this.AutoScroll)
			{
				this.verticalScrollbar.Visible = false;
				this.horizontalScrollbar.Visible = false;
				return;
			}
			MetroScrollBar point = this.verticalScrollbar;
			Rectangle clientRectangle = base.ClientRectangle;
			point.Location = new Point(clientRectangle.Width - this.verticalScrollbar.Width, base.ClientRectangle.Y);
			MetroScrollBar height = this.verticalScrollbar;
			Rectangle rectangle = base.ClientRectangle;
			height.Height = rectangle.Height - this.horizontalScrollbar.Height;
			if (!this.VerticalScrollbar)
			{
				this.verticalScrollbar.Visible = false;
			}
			MetroScrollBar metroScrollBar = this.horizontalScrollbar;
			int x = base.ClientRectangle.X;
			Rectangle clientRectangle1 = base.ClientRectangle;
			metroScrollBar.Location = new Point(x, clientRectangle1.Height - this.horizontalScrollbar.Height);
			MetroScrollBar width = this.horizontalScrollbar;
			Rectangle rectangle1 = base.ClientRectangle;
			width.Width = rectangle1.Width - this.verticalScrollbar.Width;
			if (!this.HorizontalScrollbar)
			{
				this.horizontalScrollbar.Visible = false;
			}
		}

		private void VerticalScrollbarScroll(object sender, ScrollEventArgs e)
		{
			base.AutoScrollPosition = new Point(this.horizontalScrollbar.Value, e.NewValue);
			this.UpdateScrollBarPositions();
		}

		[SecuritySafeCritical]
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (!base.DesignMode)
			{
				WinApi.ShowScrollBar(base.Handle, 3, 0);
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