using System;
using System.Collections.Generic;
using System.Drawing;

namespace MetroFramework.Drawing.Html
{
	internal static class CssLayoutEngine
	{
		private static CssBoxWord _lastTreatedWord;

		static CssLayoutEngine()
		{
		}

		private static void ApplyAlignment(Graphics g, CssLineBox lineBox)
		{
			string textAlign = lineBox.OwnerBox.TextAlign;
			string str = textAlign;
			if (textAlign != null)
			{
				if (str == "right")
				{
					CssLayoutEngine.ApplyRightAlignment(g, lineBox);
					CssLayoutEngine.ApplyVerticalAlignment(g, lineBox);
					return;
				}
				else if (str == "center")
				{
					CssLayoutEngine.ApplyCenterAlignment(g, lineBox);
					CssLayoutEngine.ApplyVerticalAlignment(g, lineBox);
					return;
				}
				else
				{
					if (str != "justify")
					{
						CssLayoutEngine.ApplyLeftAlignment(g, lineBox);
						CssLayoutEngine.ApplyVerticalAlignment(g, lineBox);
						return;
					}
					CssLayoutEngine.ApplyJustifyAlignment(g, lineBox);
					CssLayoutEngine.ApplyVerticalAlignment(g, lineBox);
					return;
				}
			}
			CssLayoutEngine.ApplyLeftAlignment(g, lineBox);
			CssLayoutEngine.ApplyVerticalAlignment(g, lineBox);
		}

		public static void ApplyCellVerticalAlignment(Graphics g, CssBox cell)
		{
			if (cell.VerticalAlign == "top" || cell.VerticalAlign == "baseline")
			{
				return;
			}
			float clientTop = cell.ClientTop;
			float clientBottom = cell.ClientBottom;
			float maximumBottom = cell.GetMaximumBottom(cell, 0f);
			float single = 0f;
			if (cell.VerticalAlign == "bottom")
			{
				single = clientBottom - maximumBottom;
			}
			else if (cell.VerticalAlign == "middle")
			{
				single = (clientBottom - maximumBottom) / 2f;
			}
			foreach (CssBox box in cell.Boxes)
			{
				box.OffsetTop(single);
			}
		}

		private static void ApplyCenterAlignment(Graphics g, CssLineBox line)
		{
			if (line.Words.Count == 0)
			{
				return;
			}
			CssBoxWord item = line.Words[line.Words.Count - 1];
			float actualRight = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
			float right = actualRight - item.Right;
			PointF lastMeasureOffset = item.LastMeasureOffset;
			float x = right - lastMeasureOffset.X - item.OwnerBox.ActualBorderRightWidth - item.OwnerBox.ActualPaddingRight;
			x = x / 2f;
			if (x <= 0f)
			{
				return;
			}
			foreach (CssBoxWord word in line.Words)
			{
				CssBoxWord left = word;
				left.Left = left.Left + x;
			}
			foreach (CssBox key in line.Rectangles.Keys)
			{
				RectangleF rectangleF = key.Rectangles[line];
				key.Rectangles[line] = new RectangleF(rectangleF.X + x, rectangleF.Y, rectangleF.Width, rectangleF.Height);
			}
		}

		private static void ApplyJustifyAlignment(Graphics g, CssLineBox lineBox)
		{
			if (lineBox.Equals(lineBox.OwnerBox.LineBoxes[lineBox.OwnerBox.LineBoxes.Count - 1]))
			{
				return;
			}
			float single = (lineBox.Equals(lineBox.OwnerBox.LineBoxes[0]) ? lineBox.OwnerBox.ActualTextIndent : 0f);
			float width = 0f;
			float single1 = 0f;
			float width1 = lineBox.OwnerBox.ClientRectangle.Width - single;
			foreach (CssBoxWord word in lineBox.Words)
			{
				width = width + word.Width;
				single1 = single1 + 1f;
			}
			if (single1 <= 0f)
			{
				return;
			}
			float single2 = (width1 - width) / single1;
			float clientLeft = lineBox.OwnerBox.ClientLeft + single;
			foreach (CssBoxWord clientRight in lineBox.Words)
			{
				clientRight.Left = clientLeft;
				clientLeft = clientRight.Right + single2;
				if (clientRight != lineBox.Words[lineBox.Words.Count - 1])
				{
					continue;
				}
				clientRight.Left = lineBox.OwnerBox.ClientRight - clientRight.Width;
			}
		}

		private static void ApplyLeftAlignment(Graphics g, CssLineBox line)
		{
		}

		private static void ApplyRightAlignment(Graphics g, CssLineBox line)
		{
			if (line.Words.Count == 0)
			{
				return;
			}
			CssBoxWord item = line.Words[line.Words.Count - 1];
			float actualRight = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
			float right = actualRight - item.Right;
			PointF lastMeasureOffset = item.LastMeasureOffset;
			float x = right - lastMeasureOffset.X - item.OwnerBox.ActualBorderRightWidth - item.OwnerBox.ActualPaddingRight;
			if (x <= 0f)
			{
				return;
			}
			foreach (CssBoxWord word in line.Words)
			{
				CssBoxWord left = word;
				left.Left = left.Left + x;
			}
			foreach (CssBox key in line.Rectangles.Keys)
			{
				RectangleF rectangleF = key.Rectangles[line];
				key.Rectangles[line] = new RectangleF(rectangleF.X + x, rectangleF.Y, rectangleF.Width, rectangleF.Height);
			}
		}

		private static void ApplyRightToLeft(CssLineBox line)
		{
			float clientLeft = line.OwnerBox.ClientLeft;
			float clientRight = line.OwnerBox.ClientRight;
			foreach (CssBoxWord word in line.Words)
			{
				float left = word.Left - clientLeft;
				word.Left = clientRight - left - word.Width;
			}
		}

		private static void ApplyVerticalAlignment(Graphics g, CssLineBox lineBox)
		{
			float maxWordBottom = lineBox.GetMaxWordBottom() - CssLayoutEngine.GetDescent(lineBox.OwnerBox.ActualFont) - 2f;
			foreach (CssBox cssBox in new List<CssBox>(lineBox.Rectangles.Keys))
			{
				CssLayoutEngine.GetAscent(cssBox.ActualFont);
				CssLayoutEngine.GetDescent(cssBox.ActualFont);
				string verticalAlign = cssBox.VerticalAlign;
				string str = verticalAlign;
				if (verticalAlign != null)
				{
					switch (str)
					{
						case "sub":
						{
							RectangleF item = lineBox.Rectangles[cssBox];
							lineBox.SetBaseLine(g, cssBox, maxWordBottom + item.Height * 0.2f);
							continue;
						}
						case "super":
						{
							RectangleF rectangleF = lineBox.Rectangles[cssBox];
							lineBox.SetBaseLine(g, cssBox, maxWordBottom - rectangleF.Height * 0.2f);
							continue;
						}
						case "text-top":
						case "text-bottom":
						case "top":
						case "bottom":
						case "middle":
						{
							continue;
						}
					}
				}
				lineBox.SetBaseLine(g, cssBox, maxWordBottom);
			}
		}

		private static void BubbleRectangles(CssBox box, CssLineBox line)
		{
			if (box.Words.Count <= 0)
			{
				foreach (CssBox cssBox in box.Boxes)
				{
					CssLayoutEngine.BubbleRectangles(cssBox, line);
				}
			}
			else
			{
				float single = float.MaxValue;
				float single1 = float.MaxValue;
				float single2 = float.MinValue;
				float single3 = float.MinValue;
				List<CssBoxWord> cssBoxWords = line.WordsOf(box);
				if (cssBoxWords.Count > 0)
				{
					foreach (CssBoxWord cssBoxWord in cssBoxWords)
					{
						single = Math.Min(single, cssBoxWord.Left);
						single2 = Math.Max(single2, cssBoxWord.Right);
						single1 = Math.Min(single1, cssBoxWord.Top);
						single3 = Math.Max(single3, cssBoxWord.Bottom);
					}
					line.UpdateRectangle(box, single, single1, single2, single3);
					return;
				}
			}
		}

		public static void CreateLineBoxes(Graphics g, CssBox blockBox)
		{
			blockBox.LineBoxes.Clear();
			float actualRight = blockBox.ActualRight - blockBox.ActualPaddingRight - blockBox.ActualBorderRightWidth;
			PointF location = blockBox.Location;
			float x = location.X + blockBox.ActualPaddingLeft - 0f + blockBox.ActualBorderLeftWidth;
			PointF pointF = blockBox.Location;
			float y = pointF.Y + blockBox.ActualPaddingTop - 0f + blockBox.ActualBorderTopWidth;
			float actualTextIndent = x + blockBox.ActualTextIndent;
			float single = y;
			float single1 = y;
			float single2 = 0f;
			CssLineBox cssLineBox = new CssLineBox(blockBox);
			CssLayoutEngine.FlowBox(g, blockBox, blockBox, actualRight, single2, x, ref cssLineBox, ref actualTextIndent, ref single, ref single1);
			foreach (CssLineBox lineBox in blockBox.LineBoxes)
			{
				CssLayoutEngine.BubbleRectangles(blockBox, lineBox);
				lineBox.AssignRectanglesToBoxes();
				CssLayoutEngine.ApplyAlignment(g, lineBox);
				if (blockBox.Direction != "rtl")
				{
					continue;
				}
				CssLayoutEngine.ApplyRightToLeft(lineBox);
			}
			blockBox.ActualBottom = single1 + blockBox.ActualPaddingBottom + blockBox.ActualBorderBottomWidth;
		}

		private static void FlowBox(Graphics g, CssBox blockbox, CssBox box, float maxright, float linespacing, float startx, ref CssLineBox line, ref float curx, ref float cury, ref float maxbottom)
		{
			box.FirstHostingLineBox = line;
			foreach (CssBox cssBox in box.Boxes)
			{
				float actualMarginLeft = cssBox.ActualMarginLeft + cssBox.ActualBorderLeftWidth + cssBox.ActualPaddingLeft;
				float actualMarginRight = cssBox.ActualMarginRight + cssBox.ActualBorderRightWidth + cssBox.ActualPaddingRight;
				float actualBorderTopWidth = cssBox.ActualBorderTopWidth;
				float actualPaddingTop = cssBox.ActualPaddingTop;
				float actualBorderBottomWidth = cssBox.ActualBorderBottomWidth;
				float single = cssBox.ActualPaddingTop;
				cssBox.RectanglesReset();
				cssBox.MeasureWordsSize(g);
				curx = curx + actualMarginLeft;
				if (cssBox.Words.Count <= 0)
				{
					CssLayoutEngine.FlowBox(g, blockbox, cssBox, maxright, linespacing, startx, ref line, ref curx, ref cury, ref maxbottom);
				}
				else
				{
					foreach (CssBoxWord word in cssBox.Words)
					{
						if (cssBox.WhiteSpace != "nowrap" && curx + word.Width + actualMarginRight > maxright || word.IsLineBreak)
						{
							curx = startx;
							cury = maxbottom + linespacing;
							line = new CssLineBox(blockbox);
							if (word.IsImage || word.Equals(cssBox.FirstWord))
							{
								curx = curx + actualMarginLeft;
							}
						}
						line.ReportExistanceOf(word);
						word.Left = curx;
						word.Top = cury;
						curx = word.Right;
						maxbottom = Math.Max(maxbottom, word.Bottom);
						CssLayoutEngine._lastTreatedWord = word;
					}
				}
				curx = curx + actualMarginRight;
			}
			box.LastHostingLineBox = line;
		}

		public static float GetAscent(Font f)
		{
			float size = f.Size * (float)f.FontFamily.GetCellAscent(f.Style) / (float)f.FontFamily.GetEmHeight(f.Style);
			return size;
		}

		public static float GetDescent(Font f)
		{
			float size = f.Size * (float)f.FontFamily.GetCellDescent(f.Style) / (float)f.FontFamily.GetEmHeight(f.Style);
			return size;
		}

		public static float GetLineSpacing(Font f)
		{
			float size = f.Size * (float)f.FontFamily.GetLineSpacing(f.Style) / (float)f.FontFamily.GetEmHeight(f.Style);
			return size;
		}

		public static float WhiteSpace(Graphics g, CssBox b)
		{
			string str = " .";
			float width = 0f;
			float single = 5f;
			StringFormat stringFormat = new StringFormat();
			CharacterRange[] characterRange = new CharacterRange[] { new CharacterRange(0, 1) };
			stringFormat.SetMeasurableCharacterRanges(characterRange);
			Region[] regionArray = g.MeasureCharacterRanges(str, b.ActualFont, new RectangleF(0f, 0f, float.MaxValue, float.MaxValue), stringFormat);
			if (regionArray == null || (int)regionArray.Length == 0)
			{
				return single;
			}
			width = regionArray[0].GetBounds(g).Width;
			if (!string.IsNullOrEmpty(b.WordSpacing) && !(b.WordSpacing == "normal"))
			{
				width = width + CssValue.ParseLength(b.WordSpacing, 0f, b);
			}
			return width;
		}
	}
}