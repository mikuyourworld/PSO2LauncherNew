using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MetroFramework.Drawing.Html
{
	internal static class CssDrawingHelper
	{
		private static GraphicsPath CreateCorner(CssBox b, RectangleF r, int cornerIndex)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF empty = RectangleF.Empty;
			RectangleF x = RectangleF.Empty;
			float single = 0f;
			float single1 = 0f;
			switch (cornerIndex)
			{
				case 1:
				{
					empty = new RectangleF(r.Left, r.Top, b.ActualCornerNW, b.ActualCornerNW);
					x = RectangleF.FromLTRB(empty.Left + b.ActualBorderLeftWidth, empty.Top + b.ActualBorderTopWidth, empty.Right, empty.Bottom);
					single = 180f;
					single1 = 270f;
					break;
				}
				case 2:
				{
					empty = new RectangleF(r.Right - b.ActualCornerNE, r.Top, b.ActualCornerNE, b.ActualCornerNE);
					x = RectangleF.FromLTRB(empty.Left, empty.Top + b.ActualBorderTopWidth, empty.Right - b.ActualBorderRightWidth, empty.Bottom);
					empty.X = empty.X - empty.Width;
					x.X = x.X - x.Width;
					single = -90f;
					single1 = 0f;
					break;
				}
				case 3:
				{
					empty = RectangleF.FromLTRB(r.Right - b.ActualCornerSE, r.Bottom - b.ActualCornerSE, r.Right, r.Bottom);
					x = new RectangleF(empty.Left, empty.Top, empty.Width - b.ActualBorderRightWidth, empty.Height - b.ActualBorderBottomWidth);
					empty.X = empty.X - empty.Width;
					empty.Y = empty.Y - empty.Height;
					x.X = x.X - x.Width;
					x.Y = x.Y - x.Height;
					single = 0f;
					single1 = 90f;
					break;
				}
				case 4:
				{
					empty = new RectangleF(r.Left, r.Bottom - b.ActualCornerSW, b.ActualCornerSW, b.ActualCornerSW);
					x = RectangleF.FromLTRB(r.Left + b.ActualBorderLeftWidth, empty.Top, empty.Right, empty.Bottom - b.ActualBorderBottomWidth);
					single = 90f;
					single1 = 180f;
					empty.Y = empty.Y - empty.Height;
					x.Y = x.Y - x.Height;
					break;
				}
			}
			if (empty.Width <= 0f)
			{
				empty.Width = 1f;
			}
			if (empty.Height <= 0f)
			{
				empty.Height = 1f;
			}
			if (x.Width <= 0f)
			{
				x.Width = 1f;
			}
			if (x.Height <= 0f)
			{
				x.Height = 1f;
			}
			empty.Width = empty.Width * 2f;
			empty.Height = empty.Height * 2f;
			x.Width = x.Width * 2f;
			x.Height = x.Height * 2f;
			empty = CssDrawingHelper.RoundR(empty, b);
			x = CssDrawingHelper.RoundR(x, b);
			graphicsPath.AddArc(empty, single, 90f);
			graphicsPath.AddArc(x, single1, -90f);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		public static Color Darken(Color c)
		{
			return Color.FromArgb(c.R / 2, c.G / 2, c.B / 2);
		}

		public static GraphicsPath GetBorderPath(CssDrawingHelper.Border border, CssBox b, RectangleF r, bool isLineStart, bool isLineEnd)
		{
			PointF[] x = new PointF[4];
			float actualBorderTopWidth = 0f;
			GraphicsPath graphicsPath = null;
			switch (border)
			{
				case CssDrawingHelper.Border.Top:
				{
					actualBorderTopWidth = b.ActualBorderTopWidth;
					x[0] = CssDrawingHelper.RoundP(new PointF(r.Left + b.ActualCornerNW, r.Top), b);
					x[1] = CssDrawingHelper.RoundP(new PointF(r.Right - b.ActualCornerNE, r.Top), b);
					x[2] = CssDrawingHelper.RoundP(new PointF(r.Right - b.ActualCornerNE, r.Top + actualBorderTopWidth), b);
					x[3] = CssDrawingHelper.RoundP(new PointF(r.Left + b.ActualCornerNW, r.Top + actualBorderTopWidth), b);
					if (isLineEnd && b.ActualCornerNE == 0f)
					{
						x[2].X = x[2].X - b.ActualBorderRightWidth;
					}
					if (isLineStart && b.ActualCornerNW == 0f)
					{
						x[3].X = x[3].X + b.ActualBorderLeftWidth;
					}
					if (b.ActualCornerNW <= 0f)
					{
						break;
					}
					graphicsPath = CssDrawingHelper.CreateCorner(b, r, 1);
					break;
				}
				case CssDrawingHelper.Border.Right:
				{
					actualBorderTopWidth = b.ActualBorderRightWidth;
					x[0] = CssDrawingHelper.RoundP(new PointF(r.Right - actualBorderTopWidth, r.Top + b.ActualCornerNE), b);
					x[1] = CssDrawingHelper.RoundP(new PointF(r.Right, r.Top + b.ActualCornerNE), b);
					x[2] = CssDrawingHelper.RoundP(new PointF(r.Right, r.Bottom - b.ActualCornerSE), b);
					x[3] = CssDrawingHelper.RoundP(new PointF(r.Right - actualBorderTopWidth, r.Bottom - b.ActualCornerSE), b);
					if (b.ActualCornerNE == 0f)
					{
						x[0].Y = x[0].Y + b.ActualBorderTopWidth;
					}
					if (b.ActualCornerSE == 0f)
					{
						x[3].Y = x[3].Y - b.ActualBorderBottomWidth;
					}
					if (b.ActualCornerNE <= 0f)
					{
						break;
					}
					graphicsPath = CssDrawingHelper.CreateCorner(b, r, 2);
					break;
				}
				case CssDrawingHelper.Border.Bottom:
				{
					actualBorderTopWidth = b.ActualBorderBottomWidth;
					x[0] = CssDrawingHelper.RoundP(new PointF(r.Left + b.ActualCornerSW, r.Bottom - actualBorderTopWidth), b);
					x[1] = CssDrawingHelper.RoundP(new PointF(r.Right - b.ActualCornerSE, r.Bottom - actualBorderTopWidth), b);
					x[2] = CssDrawingHelper.RoundP(new PointF(r.Right - b.ActualCornerSE, r.Bottom), b);
					x[3] = CssDrawingHelper.RoundP(new PointF(r.Left + b.ActualCornerSW, r.Bottom), b);
					if (isLineStart && b.ActualCornerSW == 0f)
					{
						x[0].X = x[0].X + b.ActualBorderLeftWidth;
					}
					if (isLineEnd && b.ActualCornerSE == 0f)
					{
						x[1].X = x[1].X - b.ActualBorderRightWidth;
					}
					if (b.ActualCornerSE <= 0f)
					{
						break;
					}
					graphicsPath = CssDrawingHelper.CreateCorner(b, r, 3);
					break;
				}
				case CssDrawingHelper.Border.Left:
				{
					actualBorderTopWidth = b.ActualBorderLeftWidth;
					x[0] = CssDrawingHelper.RoundP(new PointF(r.Left, r.Top + b.ActualCornerNW), b);
					x[1] = CssDrawingHelper.RoundP(new PointF(r.Left + actualBorderTopWidth, r.Top + b.ActualCornerNW), b);
					x[2] = CssDrawingHelper.RoundP(new PointF(r.Left + actualBorderTopWidth, r.Bottom - b.ActualCornerSW), b);
					x[3] = CssDrawingHelper.RoundP(new PointF(r.Left, r.Bottom - b.ActualCornerSW), b);
					if (b.ActualCornerNW == 0f)
					{
						x[1].Y = x[1].Y + b.ActualBorderTopWidth;
					}
					if (b.ActualCornerSW == 0f)
					{
						x[2].Y = x[2].Y - b.ActualBorderBottomWidth;
					}
					if (b.ActualCornerSW <= 0f)
					{
						break;
					}
					graphicsPath = CssDrawingHelper.CreateCorner(b, r, 4);
					break;
				}
			}
			GraphicsPath graphicsPath1 = new GraphicsPath(x, new byte[] { 1, 1, 1, 1 });
			if (graphicsPath != null)
			{
				graphicsPath1.AddPath(graphicsPath, true);
			}
			return graphicsPath1;
		}

		public static GraphicsPath GetRoundRect(RectangleF rect, float nwRadius, float neRadius, float seRadius, float swRadius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			nwRadius = nwRadius * 2f;
			neRadius = neRadius * 2f;
			seRadius = seRadius * 2f;
			swRadius = swRadius * 2f;
			graphicsPath.AddLine(rect.X + nwRadius, rect.Y, rect.Right - neRadius, rect.Y);
			if (neRadius > 0f)
			{
				graphicsPath.AddArc(RectangleF.FromLTRB(rect.Right - neRadius, rect.Top, rect.Right, rect.Top + neRadius), -90f, 90f);
			}
			graphicsPath.AddLine(rect.Right, rect.Top + neRadius, rect.Right, rect.Bottom - seRadius);
			if (seRadius > 0f)
			{
				graphicsPath.AddArc(RectangleF.FromLTRB(rect.Right - seRadius, rect.Bottom - seRadius, rect.Right, rect.Bottom), 0f, 90f);
			}
			graphicsPath.AddLine(rect.Right - seRadius, rect.Bottom, rect.Left + swRadius, rect.Bottom);
			if (swRadius > 0f)
			{
				graphicsPath.AddArc(RectangleF.FromLTRB(rect.Left, rect.Bottom - swRadius, rect.Left + swRadius, rect.Bottom), 90f, 90f);
			}
			graphicsPath.AddLine(rect.Left, rect.Bottom - swRadius, rect.Left, rect.Top + nwRadius);
			if (nwRadius > 0f)
			{
				graphicsPath.AddArc(RectangleF.FromLTRB(rect.Left, rect.Top, rect.Left + nwRadius, rect.Top + nwRadius), 180f, 90f);
			}
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		private static PointF RoundP(PointF p, CssBox b)
		{
			return p;
		}

		private static RectangleF RoundR(RectangleF r, CssBox b)
		{
			return Rectangle.Round(r);
		}

		internal enum Border
		{
			Top,
			Right,
			Bottom,
			Left
		}
	}
}