using System;
using System.Collections.Generic;
using System.Drawing;

namespace MetroFramework.Drawing.Html
{
	internal class CssTable
	{
		private CssBox _tableBox;

		private int _rowCount;

		private int _columnCount;

		private List<CssBox> _bodyrows;

		private CssBox _caption;

		private List<CssBox> _columns;

		private CssBox _headerBox;

		private CssBox _footerBox;

		private List<CssBox> _allRows;

		private float[] _columnWidths;

		private bool _widthSpecified;

		private float[] _columnMinWidths;

		public List<CssBox> AllRows
		{
			get
			{
				return this._allRows;
			}
		}

		public List<CssBox> BodyRows
		{
			get
			{
				return this._bodyrows;
			}
		}

		public CssBox Caption
		{
			get
			{
				return this._caption;
			}
		}

		public int ColumnCount
		{
			get
			{
				return this._columnCount;
			}
		}

		public float[] ColumnMinWidths
		{
			get
			{
				if (this._columnMinWidths == null)
				{
					this._columnMinWidths = new float[(int)this.ColumnWidths.Length];
					foreach (CssBox allRow in this.AllRows)
					{
						foreach (CssBox box in allRow.Boxes)
						{
							int colSpan = this.GetColSpan(box);
							int cellRealColumnIndex = this.GetCellRealColumnIndex(allRow, box);
							int num = cellRealColumnIndex + colSpan - 1;
							float spannedMinWidth = this.GetSpannedMinWidth(allRow, box, cellRealColumnIndex, colSpan) + (float)(colSpan - 1) * this.HorizontalSpacing;
							this._columnMinWidths[num] = Math.Max(this._columnMinWidths[num], box.GetMinimumWidth() - spannedMinWidth);
						}
					}
				}
				return this._columnMinWidths;
			}
		}

		public List<CssBox> Columns
		{
			get
			{
				return this._columns;
			}
		}

		public float[] ColumnWidths
		{
			get
			{
				return this._columnWidths;
			}
		}

		public CssBox FooterBox
		{
			get
			{
				return this._footerBox;
			}
		}

		public CssBox HeaderBox
		{
			get
			{
				return this._headerBox;
			}
		}

		public float HorizontalSpacing
		{
			get
			{
				if (this.TableBox.BorderCollapse == "collapse")
				{
					return -1f;
				}
				return this.TableBox.ActualBorderSpacingHorizontal;
			}
		}

		public int RowCount
		{
			get
			{
				return this._rowCount;
			}
		}

		public CssBox TableBox
		{
			get
			{
				return this._tableBox;
			}
		}

		public float VerticalSpacing
		{
			get
			{
				if (this.TableBox.BorderCollapse == "collapse")
				{
					return -1f;
				}
				return this.TableBox.ActualBorderSpacingVertical;
			}
		}

		public bool WidthSpecified
		{
			get
			{
				return this._widthSpecified;
			}
		}

		private CssTable()
		{
			this._bodyrows = new List<CssBox>();
			this._columns = new List<CssBox>();
			this._allRows = new List<CssBox>();
		}

		public CssTable(CssBox tableBox, Graphics g) : this()
		{
			if (!(tableBox.Display == "table") && !(tableBox.Display == "inline-table"))
			{
				throw new ArgumentException("Box is not a table", "tableBox");
			}
			this._tableBox = tableBox;
			this.MeasureWords(tableBox, g);
			this.Analyze(g);
		}

		private void Analyze(Graphics g)
		{
			int num;
			List<CssBox>.Enumerator enumerator;
			this.GetAvailableWidth();
			float availableCellWidth = float.NaN;
			foreach (CssBox box in this.TableBox.Boxes)
			{
				box.RemoveAnonymousSpaces();
				string display = box.Display;
				string str = display;
				if (display == null)
				{
					continue;
				}

                if (PrivateImplementationDetails_57814A66_940D_4455_B30A_E2997453B959._method0x6000713_1 == null)
                {
                    PrivateImplementationDetails_57814A66_940D_4455_B30A_E2997453B959._method0x6000713_1 = new Dictionary<string, int>(7)
                    {
                        { "table-caption", 0 },
                        { "table-column", 1 },
                        { "table-column-group", 2 },
                        { "table-footer-group", 3 },
                        { "table-header-group", 4 },
                        { "table-row", 5 },
                        { "table-row-group", 6 }
                    };
                }
                if (!PrivateImplementationDetails_57814A66_940D_4455_B30A_E2997453B959._method0x6000713_1.TryGetValue(str, out num))
                {
                    continue;
                }
                switch (num)
				{
					case 0:
					{
						this._caption = box;
						continue;
					}
					case 1:
					{
						for (int i = 0; i < this.GetSpan(box); i++)
						{
							this.Columns.Add(this.CreateColumn(box));
						}
						continue;
					}
					case 2:
					{
						if (box.Boxes.Count != 0)
						{
							enumerator = box.Boxes.GetEnumerator();
							try
							{
								while (enumerator.MoveNext())
								{
									CssBox current = enumerator.Current;
									int span = this.GetSpan(current);
									for (int j = 0; j < span; j++)
									{
										this.Columns.Add(this.CreateColumn(current));
									}
								}
								continue;
							}
							finally
							{
								((IDisposable)enumerator).Dispose();
							}
						}
						else
						{
							int span1 = this.GetSpan(box);
							for (int k = 0; k < span1; k++)
							{
								this.Columns.Add(this.CreateColumn(box));
							}
							continue;
						}
						break;
					}
					case 3:
					{
						if (this.FooterBox == null)
						{
							this._footerBox = box;
							continue;
						}
						else
						{
							this.BodyRows.Add(box);
							continue;
						}
					}
					case 4:
					{
						if (this.HeaderBox == null)
						{
							this._headerBox = box;
							continue;
						}
						else
						{
							this.BodyRows.Add(box);
							continue;
						}
					}
					case 5:
					{
						this.BodyRows.Add(box);
						continue;
					}
					case 6:
					{
						enumerator = box.Boxes.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								CssBox cssBox = enumerator.Current;
								if (box.Display != "table-row")
								{
									continue;
								}
								this.BodyRows.Add(box);
							}
							continue;
						}
						finally
						{
							((IDisposable)enumerator).Dispose();
						}
						break;
					}
					default:
					{
						continue;
					}
				}
			}
			if (this.HeaderBox != null)
			{
				this._allRows.AddRange(this.HeaderBox.Boxes);
			}
			this._allRows.AddRange(this.BodyRows);
			if (this.FooterBox != null)
			{
				this._allRows.AddRange(this.FooterBox.Boxes);
			}
			if (!this.TableBox.TableFixed)
			{
				int num1 = 0;
				int num2 = 0;
				List<CssBox> bodyRows = this.BodyRows;
				foreach (CssBox bodyRow in bodyRows)
				{
					bodyRow.RemoveAnonymousSpaces();
					num2 = 0;
					for (int l = 0; l < bodyRow.Boxes.Count; l++)
					{
						CssBox item = bodyRow.Boxes[l];
						int rowSpan = this.GetRowSpan(item);
						int cellRealColumnIndex = this.GetCellRealColumnIndex(bodyRow, item);
						for (int m = num1 + 1; m < num1 + rowSpan; m++)
						{
							int num3 = 0;
							int num4 = 0;
							while (num4 <= bodyRows[m].Boxes.Count)
							{
								if (num3 != cellRealColumnIndex)
								{
									num3++;
									cellRealColumnIndex = cellRealColumnIndex - (this.GetColSpan(bodyRows[m].Boxes[num4]) - 1);
									num4++;
								}
								else
								{
									bodyRows[m].Boxes.Insert(num3, new CssTable.SpacingBox(this.TableBox, ref item, num1));
									break;
								}
							}
						}
						num2++;
					}
					num1++;
				}
				this.TableBox.TableFixed = true;
			}
			this._rowCount = this.BodyRows.Count + (this.HeaderBox != null ? this.HeaderBox.Boxes.Count : 0) + (this.FooterBox != null ? this.FooterBox.Boxes.Count : 0);
			if (this.Columns.Count <= 0)
			{
				foreach (CssBox allRow in this.AllRows)
				{
					this._columnCount = Math.Max(this._columnCount, allRow.Boxes.Count);
				}
			}
			else
			{
				this._columnCount = this.Columns.Count;
			}
			this._columnWidths = new float[this._columnCount];
			for (int n = 0; n < (int)this._columnWidths.Length; n++)
			{
				this._columnWidths[n] = float.NaN;
			}
			availableCellWidth = this.GetAvailableCellWidth();
			if (this.Columns.Count <= 0)
			{
				foreach (CssBox allRow1 in this.AllRows)
				{
					for (int o = 0; o < this._columnCount; o++)
					{
						if (float.IsNaN(this.ColumnWidths[o]) && o < allRow1.Boxes.Count && allRow1.Boxes[o].Display == "table-cell")
						{
							CssLength cssLength = new CssLength(allRow1.Boxes[o].Width);
							if (cssLength.Number > 0f)
							{
								int colSpan = this.GetColSpan(allRow1.Boxes[o]);
								float number = 0f;
								if (cssLength.IsPercentage)
								{
									number = CssValue.ParseNumber(allRow1.Boxes[o].Width, availableCellWidth);
								}
								else if (cssLength.Unit == CssLength.CssUnit.Pixels || cssLength.Unit == CssLength.CssUnit.None)
								{
									number = cssLength.Number;
								}
								number = number / Convert.ToSingle(colSpan);
								for (int p = o; p < o + colSpan; p++)
								{
									this.ColumnWidths[p] = number;
								}
							}
						}
					}
				}
			}
			else
			{
				for (int q = 0; q < this.Columns.Count; q++)
				{
					CssLength cssLength1 = new CssLength(this.Columns[q].Width);
					if (cssLength1.Number > 0f)
					{
						if (cssLength1.IsPercentage)
						{
							this.ColumnWidths[q] = CssValue.ParseNumber(this.Columns[q].Width, availableCellWidth);
						}
						else if (cssLength1.Unit == CssLength.CssUnit.Pixels || cssLength1.Unit == CssLength.CssUnit.None)
						{
							this.ColumnWidths[q] = cssLength1.Number;
						}
					}
				}
			}
			if (!this.WidthSpecified)
			{
				float[] singleArray = new float[(int)this.ColumnWidths.Length];
				foreach (CssBox cssBox1 in this.AllRows)
				{
					for (int r = 0; r < cssBox1.Boxes.Count; r++)
					{
						int cellRealColumnIndex1 = this.GetCellRealColumnIndex(cssBox1, cssBox1.Boxes[r]);
						if (float.IsNaN(this.ColumnWidths[cellRealColumnIndex1]) && r < cssBox1.Boxes.Count && this.GetColSpan(cssBox1.Boxes[r]) == 1)
						{
							singleArray[cellRealColumnIndex1] = Math.Max(singleArray[cellRealColumnIndex1], cssBox1.Boxes[r].GetFullWidth(g));
						}
					}
				}
				for (int s = 0; s < (int)this.ColumnWidths.Length; s++)
				{
					if (float.IsNaN(this.ColumnWidths[s]))
					{
						this.ColumnWidths[s] = singleArray[s];
					}
				}
			}
			else
			{
				int num5 = 0;
				float columnWidths = 0f;
				for (int t = 0; t < (int)this.ColumnWidths.Length; t++)
				{
					if (!float.IsNaN(this.ColumnWidths[t]))
					{
						columnWidths = columnWidths + this.ColumnWidths[t];
					}
					else
					{
						num5++;
					}
				}
				float single = (availableCellWidth - columnWidths) / Convert.ToSingle(num5);
				for (int u = 0; u < (int)this.ColumnWidths.Length; u++)
				{
					if (float.IsNaN(this.ColumnWidths[u]))
					{
						this.ColumnWidths[u] = single;
					}
				}
			}
			int num6 = 0;
			float single1 = 1f;
			while (this.GetWidthSum() > this.GetAvailableWidth() && this.CanReduceWidth())
			{
				while (!this.CanReduceWidth(num6))
				{
					num6++;
				}
				this.ColumnWidths[num6] = this.ColumnWidths[num6] - single1;
				num6++;
				if (num6 < (int)this.ColumnWidths.Length)
				{
					continue;
				}
				num6 = 0;
			}
			foreach (CssBox allRow2 in this.AllRows)
			{
				foreach (CssBox box1 in allRow2.Boxes)
				{
					int colSpan1 = this.GetColSpan(box1);
					int cellRealColumnIndex2 = this.GetCellRealColumnIndex(allRow2, box1);
					int num7 = cellRealColumnIndex2 + colSpan1 - 1;
					if (this.ColumnWidths[cellRealColumnIndex2] >= this.ColumnMinWidths[cellRealColumnIndex2])
					{
						continue;
					}
					float columnMinWidths = this.ColumnMinWidths[cellRealColumnIndex2] - this.ColumnWidths[cellRealColumnIndex2];
					this.ColumnWidths[num7] = this.ColumnMinWidths[num7];
					if (cellRealColumnIndex2 >= (int)this.ColumnWidths.Length - 1)
					{
						continue;
					}
					this.ColumnWidths[cellRealColumnIndex2 + 1] = this.ColumnWidths[cellRealColumnIndex2 + 1] - columnMinWidths;
				}
			}
			this.TableBox.Padding = "0";
			float clientLeft = this.TableBox.ClientLeft + this.HorizontalSpacing;
			float clientTop = this.TableBox.ClientTop + this.VerticalSpacing;
			float actualRight = clientLeft;
			float verticalSpacing = clientTop;
			float single2 = clientLeft;
			float single3 = 0f;
			int num8 = 0;
			foreach (CssBox cssBox2 in this.AllRows)
			{
				if (cssBox2 is CssAnonymousSpaceBlockBox || cssBox2 is CssAnonymousSpaceBox)
				{
					continue;
				}
				actualRight = clientLeft;
				num6 = 0;
				foreach (CssBox pointF in cssBox2.Boxes)
				{
					if (num6 >= (int)this.ColumnWidths.Length)
					{
						break;
					}
					int rowSpan1 = this.GetRowSpan(pointF);
					float cellWidth = this.GetCellWidth(this.GetCellRealColumnIndex(cssBox2, pointF), pointF);
					pointF.Location = new PointF(actualRight, verticalSpacing);
					pointF.Size = new SizeF(cellWidth, 0f);
					pointF.MeasureBounds(g);
					CssTable.SpacingBox spacingBox = pointF as CssTable.SpacingBox;
					if (spacingBox != null)
					{
						if (spacingBox.EndRow == num8)
						{
							single3 = Math.Max(single3, spacingBox.ExtendedBox.ActualBottom);
						}
					}
					else if (rowSpan1 == 1)
					{
						single3 = Math.Max(single3, pointF.ActualBottom);
					}
					single2 = Math.Max(single2, pointF.ActualRight);
					num6++;
					actualRight = pointF.ActualRight + this.HorizontalSpacing;
				}
				foreach (CssBox box2 in cssBox2.Boxes)
				{
					CssTable.SpacingBox spacingBox1 = box2 as CssTable.SpacingBox;
					if (spacingBox1 != null || this.GetRowSpan(box2) != 1)
					{
						if (spacingBox1 == null || spacingBox1.EndRow != num8)
						{
							continue;
						}
						spacingBox1.ExtendedBox.ActualBottom = single3;
						CssLayoutEngine.ApplyCellVerticalAlignment(g, spacingBox1.ExtendedBox);
					}
					else
					{
						box2.ActualBottom = single3;
						CssLayoutEngine.ApplyCellVerticalAlignment(g, box2);
					}
				}
				verticalSpacing = single3 + this.VerticalSpacing;
				num8++;
			}
			this.TableBox.ActualRight = single2 + this.HorizontalSpacing + this.TableBox.ActualBorderRightWidth;
			this.TableBox.ActualBottom = single3 + this.VerticalSpacing + this.TableBox.ActualBorderBottomWidth;
		}

		private bool CanReduceWidth()
		{
			for (int i = 0; i < (int)this.ColumnWidths.Length; i++)
			{
				if (this.CanReduceWidth(i))
				{
					return true;
				}
			}
			return false;
		}

		private bool CanReduceWidth(int columnIndex)
		{
			if ((int)this.ColumnWidths.Length >= columnIndex || (int)this.ColumnMinWidths.Length >= columnIndex)
			{
				return false;
			}
			return this.ColumnWidths[columnIndex] > this.ColumnMinWidths[columnIndex];
		}

		private CssBox CreateColumn(CssBox modelBox)
		{
			return modelBox;
		}

		private float GetAvailableCellWidth()
		{
			return this.GetAvailableWidth() - this.HorizontalSpacing * (float)(this.ColumnCount + 1) - this.TableBox.ActualBorderLeftWidth - this.TableBox.ActualBorderRightWidth;
		}

		private float GetAvailableWidth()
		{
			CssLength cssLength = new CssLength(this.TableBox.Width);
			if (cssLength.Number <= 0f)
			{
				return this.TableBox.ParentBox.AvailableWidth;
			}
			this._widthSpecified = true;
			if (!cssLength.IsPercentage)
			{
				return cssLength.Number;
			}
			return CssValue.ParseNumber(cssLength.Length, this.TableBox.ParentBox.AvailableWidth);
		}

		private int GetCellRealColumnIndex(CssBox row, CssBox cell)
		{
			int colSpan = 0;
			foreach (CssBox box in row.Boxes)
			{
				if (box.Equals(cell))
				{
					break;
				}
				colSpan = colSpan + this.GetColSpan(box);
			}
			return colSpan;
		}

		private float GetCellWidth(int column, CssBox b)
		{
			float single = Convert.ToSingle(this.GetColSpan(b));
			float columnWidths = 0f;
			for (int i = column; (float)i < (float)column + single && column < (int)this.ColumnWidths.Length && (int)this.ColumnWidths.Length > i; i++)
			{
				columnWidths = columnWidths + this.ColumnWidths[i];
			}
			columnWidths = columnWidths + (single - 1f) * this.HorizontalSpacing;
			return columnWidths;
		}

		private int GetColSpan(CssBox b)
		{
			int num;
			if (!int.TryParse(b.GetAttribute("colspan", "1"), out num))
			{
				return 1;
			}
			return num;
		}

		private int GetReductableColumns()
		{
			int num = 0;
			for (int i = 0; i < (int)this.ColumnWidths.Length; i++)
			{
				if (this.CanReduceWidth(i))
				{
					num++;
				}
			}
			return num;
		}

		private int GetRowSpan(CssBox b)
		{
			int num;
			if (!int.TryParse(b.GetAttribute("rowspan", "1"), out num))
			{
				return 1;
			}
			return num;
		}

		private int GetSpan(CssBox b)
		{
			float single = CssValue.ParseNumber(b.GetAttribute("span"), 1f);
			return Math.Max(1, Convert.ToInt32(single));
		}

		private float GetSpannedMinWidth(CssBox row, CssBox cell, int realcolindex, int colspan)
		{
			float columnMinWidths = 0f;
			for (int i = realcolindex; i < row.Boxes.Count || i < realcolindex + colspan - 1; i++)
			{
				columnMinWidths = columnMinWidths + this.ColumnMinWidths[i];
			}
			return columnMinWidths;
		}

		private float GetWidthSum()
		{
			float columnWidths = 0f;
			for (int i = 0; i < (int)this.ColumnWidths.Length; i++)
			{
				if (float.IsNaN(this.ColumnWidths[i]))
				{
					throw new Exception("CssTable Algorithm error: There's a NaN in column widths");
				}
				columnWidths = columnWidths + this.ColumnWidths[i];
			}
			columnWidths = columnWidths + this.HorizontalSpacing * (float)((int)this.ColumnWidths.Length + 1);
			columnWidths = columnWidths + (this.TableBox.ActualBorderLeftWidth + this.TableBox.ActualBorderRightWidth);
			return columnWidths;
		}

		private void Measure(CssBox b, Graphics g)
		{
			if (b == null)
			{
				return;
			}
			foreach (CssBox box in b.Boxes)
			{
				box.MeasureBounds(g);
				this.Measure(box, g);
			}
		}

		private void MeasureWords(CssBox b, Graphics g)
		{
			if (b == null)
			{
				return;
			}
			foreach (CssBox box in b.Boxes)
			{
				box.MeasureWordsSize(g);
				this.MeasureWords(box, g);
			}
		}

		public class SpacingBox : CssBox
		{
			public readonly CssBox ExtendedBox;

			private int _startRow;

			private int _endRow;

			public int EndRow
			{
				get
				{
					return this._endRow;
				}
			}

			public int StartRow
			{
				get
				{
					return this._startRow;
				}
			}

			public SpacingBox(CssBox tableBox, ref CssBox extendedBox, int startRow) : base(tableBox, new HtmlTag(string.Concat("<none colspan=", extendedBox.GetAttribute("colspan", "1"), ">")))
			{
				this.ExtendedBox = extendedBox;
				base.Display = "none";
				this._startRow = startRow;
				this._endRow = startRow + int.Parse(extendedBox.GetAttribute("rowspan", "1")) - 1;
			}
		}
	}
}