using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	public class MetroDataGridHelper
	{
		private MetroScrollBar _scrollbar;

		private DataGridView _grid;

		private int _ignoreScrollbarChange;

		private bool _ishorizontal;

		private HScrollBar hScrollbar;

		private VScrollBar vScrollbar;

		public MetroDataGridHelper(MetroScrollBar scrollbar, DataGridView grid)
		{
			MetroDataGridHelper metroDataGridHelper = new MetroDataGridHelper(scrollbar, grid, true);
		}

		public MetroDataGridHelper(MetroScrollBar scrollbar, DataGridView grid, bool vertical)
		{
			this._scrollbar = scrollbar;
			this._scrollbar.UseBarColor = true;
			this._grid = grid;
			this._ishorizontal = !vertical;
			foreach (object control in this._grid.Controls)
			{
				if (control.GetType() == typeof(VScrollBar))
				{
					this.vScrollbar = (VScrollBar)control;
				}
				if (control.GetType() != typeof(HScrollBar))
				{
					continue;
				}
				this.hScrollbar = (HScrollBar)control;
			}
			this._grid.RowsAdded += new DataGridViewRowsAddedEventHandler(this._grid_RowsAdded);
			this._grid.UserDeletedRow += new DataGridViewRowEventHandler(this._grid_UserDeletedRow);
			this._grid.Scroll += new ScrollEventHandler(this._grid_Scroll);
			this._grid.Resize += new EventHandler(this._grid_Resize);
			this._scrollbar.Scroll += new ScrollEventHandler(this._scrollbar_Scroll);
			this._scrollbar.ScrollbarSize = 17;
			this.UpdateScrollbar();
		}

		private void _grid_AfterDataRefresh(object sender, ListChangedEventArgs e)
		{
			this.UpdateScrollbar();
		}

		private void _grid_Resize(object sender, EventArgs e)
		{
			this.UpdateScrollbar();
		}

		private void _grid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			this.UpdateScrollbar();
		}

		private void _grid_Scroll(object sender, ScrollEventArgs e)
		{
			this.UpdateScrollbar();
		}

		private void _grid_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
			this.UpdateScrollbar();
		}

		private void _scrollbar_Scroll(object sender, ScrollEventArgs e)
		{
			if (this._ignoreScrollbarChange > 0)
			{
				return;
			}
			if (this._ishorizontal)
			{
				try
				{
					this.hScrollbar.Value = this._scrollbar.Value;
					this._grid.HorizontalScrollingOffset = this._scrollbar.Value;
				}
				catch
				{
				}
			}
			else if (this._scrollbar.Value < 0 || this._scrollbar.Value >= this._grid.Rows.Count)
			{
				this._grid.FirstDisplayedScrollingRowIndex = this._scrollbar.Value - 1;
			}
			else
			{
				this._grid.FirstDisplayedScrollingRowIndex = (this._scrollbar.Value + (this._scrollbar.Value == 1 ? -1 : 1) >= this._grid.Rows.Count ? this._grid.Rows.Count - 1 : this._scrollbar.Value + (this._scrollbar.Value == 1 ? -1 : 1));
			}
			this._grid.Invalidate();
		}

		private void BeginIgnoreScrollbarChangeEvents()
		{
			MetroDataGridHelper metroDataGridHelper = this;
			metroDataGridHelper._ignoreScrollbarChange = metroDataGridHelper._ignoreScrollbarChange + 1;
		}

		private void EndIgnoreScrollbarChangeEvents()
		{
			if (this._ignoreScrollbarChange > 0)
			{
				MetroDataGridHelper metroDataGridHelper = this;
				metroDataGridHelper._ignoreScrollbarChange = metroDataGridHelper._ignoreScrollbarChange - 1;
			}
		}

		public void UpdateScrollbar()
		{
			if (this._grid == null)
			{
				return;
			}
			try
			{
				this.BeginIgnoreScrollbarChangeEvents();
				if (!this._ishorizontal)
				{
					int num = this.VisibleFlexGridRows();
					this._scrollbar.Maximum = this._grid.RowCount;
					this._scrollbar.Minimum = 1;
					this._scrollbar.SmallChange = 1;
					this._scrollbar.LargeChange = Math.Max(1, num - 1);
					this._scrollbar.Value = this._grid.FirstDisplayedScrollingRowIndex;
					if (this._grid.RowCount > 0 && this._grid.Rows[this._grid.RowCount - 1].Cells[0].Displayed)
					{
						this._scrollbar.Value = this._grid.RowCount;
					}
					this._scrollbar.Location = new Point(this._grid.Width - this._scrollbar.ScrollbarSize, 0);
					this._scrollbar.Height = this._grid.Height - (this.hScrollbar.Visible ? this._scrollbar.ScrollbarSize : 0);
					this._scrollbar.BringToFront();
					this._scrollbar.Visible = this.vScrollbar.Visible;
				}
				else
				{
					this.VisibleFlexGridCols();
					this._scrollbar.Maximum = this.hScrollbar.Maximum;
					this._scrollbar.Minimum = this.hScrollbar.Minimum;
					this._scrollbar.SmallChange = this.hScrollbar.SmallChange;
					this._scrollbar.LargeChange = this.hScrollbar.LargeChange;
					this._scrollbar.Location = new Point(0, this._grid.Height - this._scrollbar.ScrollbarSize);
					this._scrollbar.Width = this._grid.Width - (this.vScrollbar.Visible ? this._scrollbar.ScrollbarSize : 0);
					this._scrollbar.BringToFront();
					this._scrollbar.Visible = this.hScrollbar.Visible;
					this._scrollbar.Value = (this.hScrollbar.Value == 0 ? 1 : this.hScrollbar.Value);
				}
			}
			finally
			{
				this.EndIgnoreScrollbarChangeEvents();
			}
		}

		private int VisibleFlexGridCols()
		{
			return this._grid.DisplayedColumnCount(true);
		}

		private int VisibleFlexGridRows()
		{
			return this._grid.DisplayedRowCount(true);
		}

		public bool VisibleHorizontalScroll()
		{
			bool flag = false;
			if (this._grid.DisplayedColumnCount(true) < this._grid.ColumnCount + (this._grid.ColumnHeadersVisible ? 1 : 0))
			{
				flag = true;
			}
			return flag;
		}

		public bool VisibleVerticalScroll()
		{
			bool flag = false;
			if (this._grid.DisplayedRowCount(true) < this._grid.RowCount + (this._grid.RowHeadersVisible ? 1 : 0))
			{
				flag = true;
			}
			return flag;
		}
	}
}