using System;
using System.Drawing;

namespace MetroFramework.Drawing.Html
{
	internal class CssBoxWord : CssRectangle
	{
		private string _word;

		private PointF _lastMeasureOffset;

		private CssBox _ownerBox;

		private System.Drawing.Image _image;

		public float FullWidth
		{
			get
			{
				return base.Width;
			}
		}

		public System.Drawing.Image Image
		{
			get
			{
				return this._image;
			}
			set
			{
				this._image = value;
				if (value != null)
				{
					CssLength cssLength = new CssLength(this.OwnerBox.Width);
					CssLength cssLength1 = new CssLength(this.OwnerBox.Height);
					if (cssLength.Number <= 0f || cssLength.Unit != CssLength.CssUnit.Pixels)
					{
						base.Width = (float)value.Width;
					}
					else
					{
						base.Width = cssLength.Number;
					}
					if (cssLength1.Number <= 0f || cssLength1.Unit != CssLength.CssUnit.Pixels)
					{
						base.Height = (float)value.Height;
					}
					else
					{
						base.Height = cssLength1.Number;
					}
					CssBoxWord height = this;
					height.Height = height.Height + (this.OwnerBox.ActualBorderBottomWidth + this.OwnerBox.ActualBorderTopWidth + this.OwnerBox.ActualPaddingTop + this.OwnerBox.ActualPaddingBottom);
				}
			}
		}

		public bool IsImage
		{
			get
			{
				return this.Image != null;
			}
		}

		public bool IsLineBreak
		{
			get
			{
				return this.Text == "\n";
			}
		}

		public bool IsSpaces
		{
			get
			{
				return string.IsNullOrEmpty(this.Text.Trim());
			}
		}

		public bool IsTab
		{
			get
			{
				return this.Text == "\t";
			}
		}

		internal PointF LastMeasureOffset
		{
			get
			{
				return this._lastMeasureOffset;
			}
			set
			{
				this._lastMeasureOffset = value;
			}
		}

		public CssBox OwnerBox
		{
			get
			{
				return this._ownerBox;
			}
		}

		public string Text
		{
			get
			{
				return this._word;
			}
		}

		internal CssBoxWord(CssBox owner)
		{
			this._ownerBox = owner;
			this._word = string.Empty;
		}

		public CssBoxWord(CssBox owner, System.Drawing.Image image) : this(owner)
		{
			this.Image = image;
		}

		internal void AppendChar(char c)
		{
			CssBoxWord cssBoxWord = this;
			cssBoxWord._word = string.Concat(cssBoxWord._word, c);
		}

		internal void ReplaceLineBreaksAndTabs()
		{
			this._word = this._word.Replace('\n', ' ');
			this._word = this._word.Replace('\t', ' ');
		}

		public override string ToString()
		{
			return string.Format("{0} ({1} char{2})", this.Text.Replace(' ', '-').Replace("\n", "\\n"), this.Text.Length, (this.Text.Length != 1 ? "s" : string.Empty));
		}
	}
}