using System;
using System.Collections.Generic;
using System.Drawing;

namespace MetroFramework.Drawing.Html
{
	internal class CssLineBox
	{
		private List<CssBoxWord> _words;

		private CssBox _ownerBox;

		private Dictionary<CssBox, RectangleF> _rects;

		private List<CssBox> _relatedBoxes;

		public CssBox OwnerBox
		{
			get
			{
				return this._ownerBox;
			}
		}

		public Dictionary<CssBox, RectangleF> Rectangles
		{
			get
			{
				return this._rects;
			}
		}

		public List<CssBox> RelatedBoxes
		{
			get
			{
				return this._relatedBoxes;
			}
		}

		public List<CssBoxWord> Words
		{
			get
			{
				return this._words;
			}
		}

		public CssLineBox(CssBox ownerBox)
		{
			this._rects = new Dictionary<CssBox, RectangleF>();
			this._relatedBoxes = new List<CssBox>();
			this._words = new List<CssBoxWord>();
			this._ownerBox = ownerBox;
			this._ownerBox.LineBoxes.Add(this);
		}

		internal void AssignRectanglesToBoxes()
		{
			foreach (CssBox key in this.Rectangles.Keys)
			{
				key.Rectangles.Add(this, this.Rectangles[key]);
			}
		}

		internal void DrawRectangles(Graphics g)
		{
			foreach (CssBox key in this.Rectangles.Keys)
			{
				if (float.IsInfinity(this.Rectangles[key].Width))
				{
					continue;
				}
				g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Black)), Rectangle.Round(this.Rectangles[key]));
				g.DrawRectangle(Pens.Red, Rectangle.Round(this.Rectangles[key]));
			}
		}

		public float GetBaseLineHeight(CssBox b, Graphics g)
		{
			Font actualFont = b.ActualFont;
			FontFamily fontFamily = actualFont.FontFamily;
			FontStyle style = actualFont.Style;
			return actualFont.GetHeight(g) * (float)fontFamily.GetCellAscent(style) / (float)fontFamily.GetLineSpacing(style);
		}

		public float GetMaxWordBottom()
		{
			float single = float.MinValue;
			foreach (CssBoxWord word in this.Words)
			{
				single = Math.Max(single, word.Bottom);
			}
			return single;
		}

		internal void ReportExistanceOf(CssBoxWord word)
		{
			if (!this.Words.Contains(word))
			{
				this.Words.Add(word);
			}
			if (!this.RelatedBoxes.Contains(word.OwnerBox))
			{
				this.RelatedBoxes.Add(word.OwnerBox);
			}
		}

		internal void SetBaseLine(Graphics g, CssBox b, float baseline)
		{
			List<CssBoxWord> cssBoxWords = this.WordsOf(b);
			if (!this.Rectangles.ContainsKey(b))
			{
				return;
			}
			RectangleF item = this.Rectangles[b];
			float top = 0f;
			if (cssBoxWords.Count <= 0)
			{
				CssBoxWord cssBoxWord = b.FirstWordOccourence(b, this);
				if (cssBoxWord != null)
				{
					top = cssBoxWord.Top - item.Top;
				}
			}
			else
			{
				top = cssBoxWords[0].Top - item.Top;
			}
			float single = baseline - this.GetBaseLineHeight(b, g);
			if (b.ParentBox != null && b.ParentBox.Rectangles.ContainsKey(this) && item.Height < b.ParentBox.Rectangles[this].Height)
			{
				float single1 = single - top;
				RectangleF rectangleF = new RectangleF(item.X, single1, item.Width, item.Height);
				this.Rectangles[b] = rectangleF;
				b.OffsetRectangle(this, top);
			}
			foreach (CssBoxWord cssBoxWord1 in cssBoxWords)
			{
				if (cssBoxWord1.IsImage)
				{
					continue;
				}
				cssBoxWord1.Top = single;
			}
		}

		public override string ToString()
		{
			string[] text = new string[this.Words.Count];
			for (int i = 0; i < (int)text.Length; i++)
			{
				text[i] = this.Words[i].Text;
			}
			return string.Join(" ", text);
		}

		internal void UpdateRectangle(CssBox box, float x, float y, float r, float b)
		{
			float actualBorderLeftWidth = box.ActualBorderLeftWidth + box.ActualPaddingLeft;
			float actualBorderRightWidth = box.ActualBorderRightWidth + box.ActualPaddingRight;
			float actualBorderTopWidth = box.ActualBorderTopWidth + box.ActualPaddingTop;
			float actualBorderBottomWidth = box.ActualBorderBottomWidth + box.ActualPaddingTop;
			if (box.FirstHostingLineBox != null && box.FirstHostingLineBox.Equals(this) || box.IsImage)
			{
				x = x - actualBorderLeftWidth;
			}
			if (box.LastHostingLineBox != null && box.LastHostingLineBox.Equals(this) || box.IsImage)
			{
				r = r + actualBorderRightWidth;
			}
			if (!box.IsImage)
			{
				y = y - actualBorderTopWidth;
				b = b + actualBorderBottomWidth;
			}
			if (this.Rectangles.ContainsKey(box))
			{
				RectangleF item = this.Rectangles[box];
				this.Rectangles[box] = RectangleF.FromLTRB(Math.Min(item.X, x), Math.Min(item.Y, y), Math.Max(item.Right, r), Math.Max(item.Bottom, b));
			}
			else
			{
				this.Rectangles.Add(box, RectangleF.FromLTRB(x, y, r, b));
			}
			if (box.ParentBox != null && box.ParentBox.Display == "inline")
			{
				this.UpdateRectangle(box.ParentBox, x, y, r, b);
			}
		}

		internal List<CssBoxWord> WordsOf(CssBox box)
		{
			List<CssBoxWord> cssBoxWords = new List<CssBoxWord>();
			foreach (CssBoxWord word in this.Words)
			{
				if (!word.OwnerBox.Equals(box))
				{
					continue;
				}
				cssBoxWords.Add(word);
			}
			return cssBoxWords;
		}
	}
}