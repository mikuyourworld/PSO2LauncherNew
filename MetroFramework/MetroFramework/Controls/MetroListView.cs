using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	public class MetroListView : ListView, IMetroControl
	{
		private const uint WM_VSCROLL = 277;

		private const uint WM_NCCALCSIZE = 131;

		private const uint LVM_FIRST = 4096;

		private const uint LVM_INSERTITEMA = 4103;

		private const uint LVM_INSERTITEMW = 4173;

		private const uint LVM_DELETEITEM = 4104;

		private const uint LVM_DELETEALLITEMS = 4105;

		private const int GWL_STYLE = -16;

		private const int WS_VSCROLL = 2097152;

		private ListViewColumnSorter lvwColumnSorter;

		private System.Drawing.Font stdFont = new System.Drawing.Font("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Pixel);

		private float _offset = 0.2f;

		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private int _disableChangeEvents;

		private MetroScrollBar _vScrollbar = new MetroScrollBar();

		private bool allowSorting;

		[Category("Metro Behaviour")]
		[DefaultValue(false)]
		public bool AllowSorting
		{
			get
			{
				return this.allowSorting;
			}
			set
			{
				this.allowSorting = value;
				if (!value)
				{
					this.lvwColumnSorter = null;
					base.ListViewItemSorter = null;
					return;
				}
				this.lvwColumnSorter = new ListViewColumnSorter();
				base.ListViewItemSorter = this.lvwColumnSorter;
			}
		}

		[Browsable(false)]
		[Description("Set the font of the button caption")]
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

		public MetroListView()
		{
			this.Font = new System.Drawing.Font("Segoe UI", 12f);
			base.HideSelection = true;
			base.OwnerDraw = true;
			base.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(this.MetroListView_DrawColumnHeader);
			base.DrawItem += new DrawListViewItemEventHandler(this.MetroListView_DrawItem);
			base.DrawSubItem += new DrawListViewSubItemEventHandler(this.MetroListView_DrawSubItem);
			base.Resize += new EventHandler(this.MetroListView_Resize);
			base.ColumnClick += new ColumnClickEventHandler(this.MetroListView_ColumnClick);
			base.SelectedIndexChanged += new EventHandler(this.MetroListView_SelectedIndexChanged);
			base.FullRowSelect = true;
			base.Controls.Add(this._vScrollbar);
			this._vScrollbar.Visible = false;
			this._vScrollbar.Width = 15;
			this._vScrollbar.Dock = DockStyle.Right;
			this._vScrollbar.ValueChanged += new MetroScrollBar.ScrollValueChangedDelegate(this._vScrollbar_ValueChanged);
		}

		private void _vScrollbar_ValueChanged(object sender, int newValue)
		{
			if (this._disableChangeEvents > 0)
			{
				return;
			}
			this.SetScrollPosition(this._vScrollbar.Value);
		}

		private void BeginDisableChangeEvents()
		{
			MetroListView metroListView = this;
			metroListView._disableChangeEvents = metroListView._disableChangeEvents + 1;
		}

		private void EndDisableChangeEvents()
		{
			if (this._disableChangeEvents > 0)
			{
				MetroListView metroListView = this;
				metroListView._disableChangeEvents = metroListView._disableChangeEvents - 1;
			}
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref MetroListView.SCROLLINFO lpsi);

		public void GetScrollPosition(out int min, out int max, out int pos, out int smallchange, out int largechange)
		{
			MetroListView.SCROLLINFO sCROLLINFO = new MetroListView.SCROLLINFO()
			{
				cbSize = (uint)Marshal.SizeOf(typeof(MetroListView.SCROLLINFO)),
				fMask = 23
			};
			if (!MetroListView.GetScrollInfo(base.Handle, 1, ref sCROLLINFO))
			{
				min = 0;
				max = 0;
				pos = 0;
				smallchange = 0;
				largechange = 0;
				return;
			}
			min = sCROLLINFO.nMin;
			max = sCROLLINFO.nMax;
			pos = sCROLLINFO.nPos + 1;
			smallchange = 1;
			largechange = (int)sCROLLINFO.nPage;
		}

		public static int GetWindowLong(IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size == 4)
			{
				return (int)MetroListView.GetWindowLong32(hWnd, nIndex);
			}
			return (int)((long)MetroListView.GetWindowLongPtr64(hWnd, nIndex));
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, EntryPoint="GetWindowLong", ExactSpelling=false)]
		public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet=CharSet.Auto, EntryPoint="GetWindowLongPtr", ExactSpelling=false)]
		public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

		private void MetroListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (this.lvwColumnSorter == null)
			{
				return;
			}
			if (e.Column != this.lvwColumnSorter.SortColumn)
			{
				this.lvwColumnSorter.SortColumn = e.Column;
				this.lvwColumnSorter.Order = SortOrder.Ascending;
			}
			else if (this.lvwColumnSorter.Order != SortOrder.Ascending)
			{
				this.lvwColumnSorter.Order = SortOrder.Ascending;
			}
			else
			{
				this.lvwColumnSorter.Order = SortOrder.Descending;
			}
			base.Sort();
		}

		private void MetroListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			Color color = MetroPaint.ForeColor.Button.Press(this.Theme);
			e.Graphics.FillRectangle(new SolidBrush(MetroPaint.GetStyleColor(this.Style)), e.Bounds);
			using (StringFormat stringFormat = new StringFormat())
			{
				stringFormat.Alignment = StringAlignment.Center;
				e.Graphics.DrawString(e.Header.Text, this.stdFont, new SolidBrush(color), e.Bounds, stringFormat);
			}
		}

		private void MetroListView_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			Rectangle bounds;
			Color white = MetroPaint.ForeColor.Button.Disabled(this.Theme);
			if (base.View == System.Windows.Forms.View.Details | base.View == System.Windows.Forms.View.List | base.View == System.Windows.Forms.View.SmallIcon)
			{
				Color styleColor = MetroPaint.GetStyleColor(this.Style);
				if (e.Item.Selected)
				{
					e.Graphics.FillRectangle(new SolidBrush(ControlPaint.Light(MetroPaint.GetStyleColor(this.Style), this._offset)), e.Bounds);
					white = Color.White;
					styleColor = Color.White;
				}
				TextFormatFlags textFormatFlag = TextFormatFlags.Default;
				int width = 0;
				int num = 0;
				if (base.CheckBoxes)
				{
					width = 12;
					num = 14;
					bounds = e.Bounds;
					int height = bounds.Height / 2 - 6;
					using (Pen pen = new Pen(white))
					{
						Rectangle rectangle = e.Bounds;
						Rectangle bounds1 = e.Bounds;
						Rectangle rectangle1 = new Rectangle(rectangle.X + 2, bounds1.Y + height, 12, 12);
						e.Graphics.DrawRectangle(pen, rectangle1);
					}
					if (e.Item.Checked)
					{
						using (SolidBrush solidBrush = new SolidBrush(styleColor))
						{
							height = e.Bounds.Height / 2 - 4;
							Rectangle bounds2 = e.Bounds;
							Rectangle rectangle2 = e.Bounds;
							Rectangle rectangle3 = new Rectangle(bounds2.X + 4, rectangle2.Y + height, 9, 9);
							e.Graphics.FillRectangle(solidBrush, rectangle3);
						}
					}
				}
				if (base.SmallImageList != null)
				{
					int height1 = 0;
					Image item = null;
					if (e.Item.ImageIndex > -1)
					{
						item = base.SmallImageList.Images[e.Item.ImageIndex];
					}
					if (e.Item.ImageKey != "")
					{
						item = base.SmallImageList.Images[e.Item.ImageKey];
					}
					if (item != null)
					{
						num = num + (num > 0 ? 4 : 2);
						Rectangle bounds3 = e.Item.Bounds;
						height1 = (bounds3.Height - item.Height) / 2;
						Graphics graphics = e.Graphics;
						Rectangle bounds4 = e.Item.Bounds;
						Rectangle rectangle4 = e.Item.Bounds;
						graphics.DrawImage(item, new Rectangle(bounds4.Left + num, rectangle4.Top + height1, item.Width, item.Height));
						num = num + base.SmallImageList.ImageSize.Width;
						width = width + base.SmallImageList.ImageSize.Width;
					}
				}
				if (base.View == System.Windows.Forms.View.Details)
				{
					return;
				}
				int width1 = e.Item.Bounds.Width;
				if (base.View == System.Windows.Forms.View.Details)
				{
					width1 = base.Columns[0].Width;
				}
				Rectangle bounds5 = e.Bounds;
				int y = e.Bounds.Y;
				Rectangle rectangle5 = e.Item.Bounds;
				Rectangle rectangle6 = new Rectangle(bounds5.X + num, y, width1 - width, rectangle5.Height);
				TextRenderer.DrawText(e.Graphics, e.Item.Text, this.stdFont, rectangle6, white, textFormatFlag | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
				return;
			}
			if (base.View != System.Windows.Forms.View.Tile)
			{
				if (base.CheckBoxes)
				{
					int num1 = e.Bounds.Height / 2 - 6;
					using (Pen pen1 = new Pen(Color.Black))
					{
						bounds = e.Bounds;
						int x = bounds.X + 6;
						bounds = e.Bounds;
						Rectangle rectangle7 = new Rectangle(x, bounds.Y + num1, 12, 12);
						e.Graphics.DrawRectangle(pen1, rectangle7);
					}
					if (e.Item.Checked)
					{
						Color color = MetroPaint.GetStyleColor(this.Style);
						if (e.Item.Selected)
						{
							color = Color.White;
						}
						using (SolidBrush solidBrush1 = new SolidBrush(color))
						{
							bounds = e.Bounds;
							num1 = bounds.Height / 2 - 4;
							bounds = e.Bounds;
							int x1 = bounds.X + 8;
							bounds = e.Bounds;
							Rectangle rectangle8 = new Rectangle(x1, bounds.Y + num1, 9, 9);
							e.Graphics.FillRectangle(solidBrush1, rectangle8);
						}
					}
					bounds = e.Bounds;
					int x2 = bounds.X + 23;
					bounds = e.Bounds;
					int y1 = bounds.Y + 1;
					int width2 = e.Bounds.Width;
					bounds = e.Bounds;
					Rectangle rectangle9 = new Rectangle(x2, y1, width2, bounds.Height);
					e.Graphics.DrawString(e.Item.Text, this.stdFont, new SolidBrush(white), rectangle9);
				}
				this.Font = this.stdFont;
				e.DrawDefault = true;
			}
			else
			{
				int num2 = 0;
				if (base.LargeImageList != null)
				{
					int height2 = 0;
					num2 = base.LargeImageList.ImageSize.Width + 2;
					Image image = null;
					if (e.Item.ImageIndex > -1)
					{
						image = base.LargeImageList.Images[e.Item.ImageIndex];
					}
					if (e.Item.ImageKey != "")
					{
						image = base.LargeImageList.Images[e.Item.ImageKey];
					}
					if (image != null)
					{
						Rectangle bounds6 = e.Item.Bounds;
						height2 = (bounds6.Height - image.Height) / 2;
						Graphics graphic = e.Graphics;
						Rectangle bounds7 = e.Item.Bounds;
						Rectangle bounds8 = e.Item.Bounds;
						graphic.DrawImage(image, new Rectangle(bounds7.Left + num2, bounds8.Top + height2, image.Width, image.Height));
					}
				}
				if (e.Item.Selected)
				{
					Rectangle bounds9 = e.Item.Bounds;
					int y2 = e.Item.Bounds.Y;
					int width3 = e.Item.Bounds.Width;
					Rectangle bounds10 = e.Item.Bounds;
					Rectangle rectangle10 = new Rectangle(bounds9.X + num2, y2, width3, bounds10.Height);
					e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(250, 194, 87)), rectangle10);
				}
				int num3 = 0;
				foreach (ListViewItem.ListViewSubItem subItem in e.Item.SubItems)
				{
					if (num3 > 0 && !e.Item.Selected)
					{
						white = Color.Silver;
					}
					int y3 = e.Item.Bounds.Y;
					Rectangle bounds11 = e.Item.Bounds;
					int height3 = (bounds11.Height - e.Item.SubItems.Count * 15) / 2;
					Rectangle rectangle11 = e.Item.Bounds;
					Rectangle bounds12 = e.Item.Bounds;
					int width4 = e.Item.Bounds.Width;
					Rectangle rectangle12 = e.Item.Bounds;
					Rectangle rectangle13 = new Rectangle(rectangle11.X + num2, bounds12.Y + num3, width4, rectangle12.Height);
					TextFormatFlags textFormatFlag1 = TextFormatFlags.Default;
					TextRenderer.DrawText(e.Graphics, subItem.Text, new System.Drawing.Font("Segoe UI", 9f), rectangle13, white, textFormatFlag1 | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis);
					num3 = num3 + 15;
				}
			}
		}

		private void MetroListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			double num;
			Color white = MetroPaint.ForeColor.Button.Disabled(this.Theme);
			if (base.View != System.Windows.Forms.View.Details)
			{
				e.DrawDefault = true;
			}
			else
			{
				if (e.Item.Selected)
				{
					e.Graphics.FillRectangle(new SolidBrush(ControlPaint.Light(MetroPaint.GetStyleColor(this.Style), this._offset)), e.Bounds);
					white = Color.White;
				}
				TextFormatFlags textFormatFlag = TextFormatFlags.Default;
				int width = 0;
				int width1 = 0;
				if (base.CheckBoxes && e.ColumnIndex == 0)
				{
					width = 12;
					width1 = 14;
					int height = e.Bounds.Height / 2 - 6;
					using (Pen pen = new Pen(white))
					{
						Rectangle bounds = e.Bounds;
						Rectangle rectangle = e.Bounds;
						Rectangle rectangle1 = new Rectangle(bounds.X + 2, rectangle.Y + height, 12, 12);
						e.Graphics.DrawRectangle(pen, rectangle1);
					}
					if (e.Item.Checked)
					{
						Color styleColor = MetroPaint.GetStyleColor(this.Style);
						if (e.Item.Selected)
						{
							styleColor = Color.White;
						}
						using (SolidBrush solidBrush = new SolidBrush(styleColor))
						{
							height = e.Bounds.Height / 2 - 4;
							Rectangle bounds1 = e.Bounds;
							Rectangle bounds2 = e.Bounds;
							Rectangle rectangle2 = new Rectangle(bounds1.X + 4, bounds2.Y + height, 9, 9);
							e.Graphics.FillRectangle(solidBrush, rectangle2);
						}
					}
				}
				if (base.SmallImageList != null)
				{
					int height1 = 0;
					Image item = null;
					if (e.Item.ImageIndex > -1)
					{
						item = base.SmallImageList.Images[e.Item.ImageIndex];
					}
					if (e.Item.ImageKey != "")
					{
						item = base.SmallImageList.Images[e.Item.ImageKey];
					}
					if (item != null)
					{
						width1 = width1 + (width1 > 0 ? 4 : 2);
						Rectangle bounds3 = e.Item.Bounds;
						height1 = (bounds3.Height - item.Height) / 2;
						Graphics graphics = e.Graphics;
						Rectangle rectangle3 = e.Item.Bounds;
						Rectangle bounds4 = e.Item.Bounds;
						graphics.DrawImage(item, new Rectangle(rectangle3.Left + width1, bounds4.Top + height1, item.Width, item.Height));
						width1 = width1 + base.SmallImageList.ImageSize.Width;
						width = width + base.SmallImageList.ImageSize.Width;
					}
				}
				int num1 = e.Item.Bounds.Width;
				if (base.View == System.Windows.Forms.View.Details)
				{
					num1 = base.Columns[0].Width;
				}
				using (StringFormat stringFormat = new StringFormat())
				{
					switch (e.Header.TextAlign)
					{
						case HorizontalAlignment.Right:
						{
							stringFormat.Alignment = StringAlignment.Far;
							break;
						}
						case HorizontalAlignment.Center:
						{
							stringFormat.Alignment = StringAlignment.Center;
							break;
						}
					}
					if (e.ColumnIndex > 0 && double.TryParse(e.SubItem.Text, NumberStyles.Currency, (IFormatProvider)NumberFormatInfo.CurrentInfo, out num))
					{
						stringFormat.Alignment = StringAlignment.Far;
					}
					Rectangle rectangle4 = e.Bounds;
					int y = e.Bounds.Y;
					Rectangle bounds5 = e.Item.Bounds;
					Rectangle rectangle5 = new Rectangle(rectangle4.X + width1, y, num1 - width, bounds5.Height);
					TextRenderer.DrawText(e.Graphics, e.SubItem.Text, this.stdFont, rectangle5, white, textFormatFlag | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
				}
			}
		}

		private void MetroListView_Resize(object sender, EventArgs e)
		{
			int count = base.Columns.Count;
		}

		private void MetroListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateScrollbar();
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

		protected void OnItemAdded()
		{
			if (this._disableChangeEvents > 0)
			{
				return;
			}
			this.UpdateScrollbar();
			if (this.ItemAdded != null)
			{
				this.ItemAdded(this);
			}
		}

		protected void OnItemsRemoved()
		{
			if (this._disableChangeEvents > 0)
			{
				return;
			}
			this.UpdateScrollbar();
			if (this.ItemsRemoved != null)
			{
				this.ItemsRemoved(this);
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (this._vScrollbar != null)
			{
				MetroScrollBar value = this._vScrollbar;
				value.Value = value.Value - 3 * Math.Sign(e.Delta);
			}
		}

		public void SetScrollPosition(int pos)
		{
			pos = Math.Min(base.Items.Count - 1, pos);
			if (pos < 0 || pos >= base.Items.Count)
			{
				return;
			}
			base.SuspendLayout();
			base.EnsureVisible(pos);
			for (int i = 0; i < 10; i++)
			{
				if (base.TopItem != null && base.TopItem.Index != pos)
				{
					base.TopItem = base.Items[pos];
				}
			}
			base.ResumeLayout();
		}

		public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
		{
			if (IntPtr.Size == 4)
			{
				return (int)MetroListView.SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
			}
			return (int)((long)MetroListView.SetWindowLongPtr64(hWnd, nIndex, dwNewLong));
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, EntryPoint="SetWindowLong", ExactSpelling=false)]
		public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet=CharSet.Auto, EntryPoint="SetWindowLongPtr", ExactSpelling=false)]
		public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

		public void UpdateScrollbar()
		{
			int num;
			int num1;
			int num2;
			int num3;
			int num4;
			if (this._vScrollbar != null)
			{
				this.GetScrollPosition(out num1, out num, out num2, out num3, out num4);
				this.BeginDisableChangeEvents();
				this._vScrollbar.Value = num2;
				this._vScrollbar.Maximum = num - num4 + 1;
				this._vScrollbar.Minimum = num1;
				this._vScrollbar.SmallChange = num3;
				this._vScrollbar.LargeChange = num4;
				this._vScrollbar.Visible = this._vScrollbar.Maximum != 101;
				this.EndDisableChangeEvents();
			}
		}

		protected override void WndProc(ref Message m)
		{
			int num;
			int num1;
			int num2;
			int num3;
			int num4;
			if ((long)m.Msg == (long)277)
			{
				this.GetScrollPosition(out num1, out num, out num2, out num3, out num4);
				if (this.ScrollPositionChanged != null)
				{
					this.ScrollPositionChanged(this, num2);
				}
				if (this._vScrollbar != null)
				{
					this._vScrollbar.Value = num2;
				}
			}
			else if ((long)m.Msg == (long)131)
			{
				int windowLong = MetroListView.GetWindowLong(base.Handle, -16);
				if ((windowLong & 2097152) == 2097152)
				{
					MetroListView.SetWindowLong(base.Handle, -16, windowLong & -2097153);
				}
			}
			else if ((long)m.Msg == (long)4103 || (long)m.Msg == (long)4173)
			{
				this.OnItemAdded();
			}
			else if ((long)m.Msg == (long)4104 || (long)m.Msg == (long)4105)
			{
				this.OnItemsRemoved();
			}
			base.WndProc(ref m);
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

		public event Action<MetroListView> ItemAdded;

		public event Action<MetroListView> ItemsRemoved;

		public event MetroListView.ScrollPositionChangedDelegate ScrollPositionChanged;

		private enum LPCSCROLLINFO
		{
			SIF_RANGE = 1,
			SIF_PAGE = 2,
			SIF_POS = 4,
			SIF_DISABLENOSCROLL = 8,
			SIF_TRACKPOS = 16,
			SIF_ALL = 23
		}

		private struct LVITEM
		{
			public uint mask;

			public int iItem;

			public int iSubItem;

			public uint state;

			public uint stateMask;

			public IntPtr pszText;

			public int cchTextMax;

			public int iImage;

			public IntPtr lParam;
		}

		private enum SBTYPES
		{
			SB_HORZ,
			SB_VERT,
			SB_CTL,
			SB_BOTH
		}

		public enum ScrollBarCommands
		{
			SB_LINELEFT = 0,
			SB_LINEUP = 0,
			SB_LINEDOWN = 1,
			SB_LINERIGHT = 1,
			SB_PAGELEFT = 2,
			SB_PAGEUP = 2,
			SB_PAGEDOWN = 3,
			SB_PAGERIGHT = 3,
			SB_THUMBPOSITION = 4,
			SB_THUMBTRACK = 5,
			SB_LEFT = 6,
			SB_TOP = 6,
			SB_BOTTOM = 7,
			SB_RIGHT = 7,
			SB_ENDSCROLL = 8
		}

		private enum ScrollBarDirection
		{
			SB_HORZ,
			SB_VERT,
			SB_CTL,
			SB_BOTH
		}

		private struct SCROLLINFO
		{
			public uint cbSize;

			public uint fMask;

			public int nMin;

			public int nMax;

			public uint nPage;

			public int nPos;

			public int nTrackPos;
		}

		private enum ScrollInfoMask
		{
			SIF_RANGE = 1,
			SIF_PAGE = 2,
			SIF_POS = 4,
			SIF_DISABLENOSCROLL = 8,
			SIF_TRACKPOS = 16,
			SIF_ALL = 23
		}

		public delegate void ScrollPositionChangedDelegate(MetroListView listview, int pos);
	}
}