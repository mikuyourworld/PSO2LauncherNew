using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[Designer("MetroFramework.Design.Controls.MetroTabControlDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(TabControl))]
	public class MetroTabControl : TabControl, IMetroControl
	{
		private const int TabBottomBorderHeight = 3;

		private const int WM_SETFONT = 48;

		private const int WM_FONTCHANGE = 29;

		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private List<string> tabDisable = new List<string>();

		private List<string> tabOrder = new List<string>();

		private List<HiddenTabs> hidTabs = new List<HiddenTabs>();

		private SubClass scUpDown;

		private bool bUpDown;

		private MetroTabControlSize metroLabelSize = MetroTabControlSize.Medium;

		private MetroTabControlWeight metroLabelWeight;

		private ContentAlignment textAlign = ContentAlignment.MiddleLeft;

		private bool isMirrored;

		protected override System.Windows.Forms.CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				System.Windows.Forms.CreateParams createParams = base.CreateParams;
				if (this.isMirrored)
				{
					createParams.ExStyle = createParams.ExStyle | 4194304 | 1048576;
				}
				return createParams;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroTabControlSize.Medium)]
		public MetroTabControlSize FontSize
		{
			get
			{
				return this.metroLabelSize;
			}
			set
			{
				this.metroLabelSize = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroTabControlWeight.Light)]
		public MetroTabControlWeight FontWeight
		{
			get
			{
				return this.metroLabelWeight;
			}
			set
			{
				this.metroLabelWeight = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public new bool IsMirrored
		{
			get
			{
				return this.isMirrored;
			}
			set
			{
				if (this.isMirrored == value)
				{
					return;
				}
				this.isMirrored = value;
				base.UpdateStyles();
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

		[Editor("MetroFramework.Design.MetroTabPageCollectionEditor, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a", typeof(UITypeEditor))]
		public new TabControl.TabPageCollection TabPages
		{
			get
			{
				return base.TabPages;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(ContentAlignment.MiddleLeft)]
		public ContentAlignment TextAlign
		{
			get
			{
				return this.textAlign;
			}
			set
			{
				this.textAlign = value;
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

		public MetroTabControl()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			base.Padding = new Point(6, 8);
			base.Selecting += new TabControlCancelEventHandler(this.MetroTabControl_Selecting);
		}

		public void DisableTab(MetroTabPage tabpage)
		{
			if (!this.tabDisable.Contains(tabpage.Name))
			{
				if (base.SelectedTab == tabpage && base.TabCount == 1)
				{
					return;
				}
				if (base.SelectedTab == tabpage)
				{
					if (base.SelectedIndex != base.TabCount - 1)
					{
						MetroTabControl selectedIndex = this;
						selectedIndex.SelectedIndex = selectedIndex.SelectedIndex + 1;
					}
					else
					{
						base.SelectedIndex = 0;
					}
				}
				int num = this.TabPages.IndexOf(tabpage);
				this.tabDisable.Add(tabpage.Name);
				Graphics graphic = base.CreateGraphics();
				this.DrawTab(num, graphic);
				this.DrawTabBottomBorder(base.SelectedIndex, graphic);
				this.DrawTabSelected(base.SelectedIndex, graphic);
			}
		}

		private void DrawTab(int index, Graphics graphics)
		{
			Color defaultForeColor;
			Color backColor = this.BackColor;
			if (!this.useCustomBackColor)
			{
				backColor = MetroPaint.BackColor.Form(this.Theme);
			}
			TabPage item = this.TabPages[index];
			Rectangle tabRect = this.GetTabRect(index);
			if (!base.Enabled || this.tabDisable.Contains(item.Name))
			{
				defaultForeColor = MetroPaint.ForeColor.Label.Disabled(this.Theme);
			}
			else if (!this.useCustomForeColor)
			{
				defaultForeColor = (!this.useStyleColors ? MetroPaint.ForeColor.TabControl.Normal(this.Theme) : MetroPaint.GetStyleColor(this.Style));
			}
			else
			{
				defaultForeColor = Control.DefaultForeColor;
			}
			if (index == 0)
			{
				tabRect.X = this.DisplayRectangle.X;
			}
			Rectangle rectangle = tabRect;
			tabRect.Width = tabRect.Width + 20;
			using (Brush solidBrush = new SolidBrush(backColor))
			{
				graphics.FillRectangle(solidBrush, rectangle);
			}
			TextRenderer.DrawText(graphics, item.Text, MetroFonts.TabControl(this.metroLabelSize, this.metroLabelWeight), tabRect, defaultForeColor, backColor, MetroPaint.GetTextFormatFlags(this.TextAlign));
		}

		private void DrawTabBottomBorder(int index, Graphics graphics)
		{
			using (Brush solidBrush = new SolidBrush(MetroPaint.BorderColor.TabControl.Normal(this.Theme)))
			{
				int x = this.DisplayRectangle.X;
				Rectangle tabRect = this.GetTabRect(index);
				Rectangle displayRectangle = this.DisplayRectangle;
				Rectangle rectangle = new Rectangle(x, tabRect.Bottom + 2 - 3, displayRectangle.Width, 3);
				graphics.FillRectangle(solidBrush, rectangle);
			}
		}

		private void DrawTabSelected(int index, Graphics graphics)
		{
			using (Brush solidBrush = new SolidBrush(MetroPaint.GetStyleColor(this.Style)))
			{
				Rectangle tabRect = this.GetTabRect(index);
				int x = tabRect.X + (index == 0 ? 2 : 0);
				Rectangle rectangle = this.GetTabRect(index);
				Rectangle rectangle1 = new Rectangle(x, rectangle.Bottom + 2 - 3, tabRect.Width + (index == 0 ? 0 : 2), 3);
				graphics.FillRectangle(solidBrush, rectangle1);
			}
		}

		[SecuritySafeCritical]
		private void DrawUpDown(Graphics graphics)
		{
			Color color = (base.Parent != null ? base.Parent.BackColor : MetroPaint.BackColor.Form(this.Theme));
			Rectangle rectangle = new Rectangle();
			WinApi.GetClientRect(this.scUpDown.Handle, ref rectangle);
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.Clear(color);
			using (Brush solidBrush = new SolidBrush(MetroPaint.BorderColor.TabControl.Normal(this.Theme)))
			{
				GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
				PointF[] pointF = new PointF[] { new PointF(6f, 6f), new PointF(16f, 0f), new PointF(16f, 12f) };
				graphicsPath.AddLines(pointF);
				graphics.FillPath(solidBrush, graphicsPath);
				graphicsPath.Reset();
				PointF[] pointFArray = new PointF[] { new PointF((float)(rectangle.Width - 15), 0f), new PointF((float)(rectangle.Width - 5), 6f), new PointF((float)(rectangle.Width - 15), 12f) };
				graphicsPath.AddLines(pointFArray);
				graphics.FillPath(solidBrush, graphicsPath);
				graphicsPath.Dispose();
			}
		}

		public void EnableTab(MetroTabPage tabpage)
		{
			if (this.tabDisable.Contains(tabpage.Name))
			{
				this.tabDisable.Remove(tabpage.Name);
				int num = this.TabPages.IndexOf(tabpage);
				Graphics graphic = base.CreateGraphics();
				this.DrawTab(num, graphic);
				this.DrawTabBottomBorder(base.SelectedIndex, graphic);
				this.DrawTabSelected(base.SelectedIndex, graphic);
			}
		}

		[SecuritySafeCritical]
		private void FindUpDown()
		{
			if (!base.DesignMode)
			{
				bool flag = false;
				IntPtr window = WinApi.GetWindow(base.Handle, 5);
				while (window != IntPtr.Zero)
				{
					char[] chrArray = new char[33];
					if (new string(chrArray, 0, WinApi.GetClassName(window, chrArray, 32)) != "msctls_updown32")
					{
						window = WinApi.GetWindow(window, 2);
					}
					else
					{
						flag = true;
						if (this.bUpDown)
						{
							break;
						}
						this.scUpDown = new SubClass(window, true);
						this.scUpDown.SubClassedWndProc += new SubClass.SubClassWndProcEventHandler(this.scUpDown_SubClassedWndProc);
						this.bUpDown = true;
						break;
					}
				}
				if (!flag && this.bUpDown)
				{
					this.bUpDown = false;
				}
			}
		}

		private new Rectangle GetTabRect(int index)
		{
			if (index < 0)
			{
				return new Rectangle();
			}
			return base.GetTabRect(index);
		}

		public void HideTab(MetroTabPage tabpage)
		{
			if (this.TabPages.Contains(tabpage))
			{
				int num = this.TabPages.IndexOf(tabpage);
				this.hidTabs.Add(new HiddenTabs(num, tabpage.Name));
				this.TabPages.Remove(tabpage);
			}
		}

		public bool IsTabEnable(MetroTabPage tabpage)
		{
			return this.tabDisable.Contains(tabpage.Name);
		}

		public bool IsTabHidden(MetroTabPage tabpage)
		{
			HiddenTabs hiddenTab = this.hidTabs.Find((HiddenTabs bk) => bk.tabpage == tabpage.Name);
			return hiddenTab != null;
		}

		private System.Drawing.Size MeasureText(string text)
		{
			System.Drawing.Size size;
			using (Graphics graphic = base.CreateGraphics())
			{
				System.Drawing.Size size1 = new System.Drawing.Size(2147483647, 2147483647);
				size = TextRenderer.MeasureText(graphic, text, MetroFonts.TabControl(this.metroLabelSize, this.metroLabelWeight), size1, MetroPaint.GetTextFormatFlags(this.TextAlign) | TextFormatFlags.NoPadding);
			}
			return size;
		}

		private void MetroTabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (this.tabDisable.Count > 0 && this.tabDisable.Contains(e.TabPage.Name))
			{
				e.Cancel = true;
			}
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			this.FindUpDown();
			this.UpdateUpDown();
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			this.FindUpDown();
			this.UpdateUpDown();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			this.OnFontChanged(EventArgs.Empty);
			this.FindUpDown();
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

		[SecuritySafeCritical]
		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			IntPtr hfont = MetroFonts.TabControl(this.metroLabelSize, this.metroLabelWeight).ToHfont();
			MetroTabControl.SendMessage(base.Handle, 48, hfont, new IntPtr(-1));
			MetroTabControl.SendMessage(base.Handle, 29, IntPtr.Zero, IntPtr.Zero);
			base.UpdateStyles();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (base.SelectedIndex != -1 && !this.TabPages[base.SelectedIndex].Focused)
			{
				bool flag = false;
				foreach (Control control in this.TabPages[base.SelectedIndex].Controls)
				{
					if (!control.Focused)
					{
						continue;
					}
					flag = true;
					return;
				}
				if (!flag)
				{
					this.TabPages[base.SelectedIndex].Select();
					this.TabPages[base.SelectedIndex].Focus();
				}
			}
			base.OnMouseWheel(e);
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
			for (int i = 0; i < this.TabPages.Count; i++)
			{
				if (i != base.SelectedIndex)
				{
					this.DrawTab(i, e.Graphics);
				}
			}
			if (base.SelectedIndex <= -1)
			{
				return;
			}
			this.DrawTabBottomBorder(base.SelectedIndex, e.Graphics);
			this.DrawTab(base.SelectedIndex, e.Graphics);
			this.DrawTabSelected(base.SelectedIndex, e.Graphics);
			this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
			base.Invalidate();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			base.Invalidate();
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			this.UpdateUpDown();
			base.Invalidate();
		}

		[SecuritySafeCritical]
		private int scUpDown_SubClassedWndProc(ref Message m)
		{
			if (m.Msg != 15)
			{
				return 0;
			}
			IntPtr windowDC = WinApi.GetWindowDC(this.scUpDown.Handle);
			Graphics graphic = Graphics.FromHdc(windowDC);
			this.DrawUpDown(graphic);
			graphic.Dispose();
			WinApi.ReleaseDC(this.scUpDown.Handle, windowDC);
			m.Result = IntPtr.Zero;
			Rectangle rectangle = new Rectangle();
			WinApi.GetClientRect(this.scUpDown.Handle, ref rectangle);
			WinApi.ValidateRect(this.scUpDown.Handle, ref rectangle);
			return 1;
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		public void ShowTab(MetroTabPage tabpage)
		{
			HiddenTabs hiddenTab = this.hidTabs.Find((HiddenTabs bk) => bk.tabpage == tabpage.Name);
			if (hiddenTab != null)
			{
				this.TabPages.Insert(hiddenTab.index, tabpage);
				this.hidTabs.Remove(hiddenTab);
			}
		}

		[SecuritySafeCritical]
		private void UpdateUpDown()
		{
			if (this.bUpDown && !base.DesignMode && WinApi.IsWindowVisible(this.scUpDown.Handle))
			{
				Rectangle rectangle = new Rectangle();
				WinApi.GetClientRect(this.scUpDown.Handle, ref rectangle);
				WinApi.InvalidateRect(this.scUpDown.Handle, ref rectangle, true);
			}
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