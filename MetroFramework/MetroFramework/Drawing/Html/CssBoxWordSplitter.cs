using System;
using System.Collections.Generic;

namespace MetroFramework.Drawing.Html
{
	internal class CssBoxWordSplitter
	{
		private CssBox _box;

		private string _text;

		private List<CssBoxWord> _words;

		private CssBoxWord _curword;

		public CssBox Box
		{
			get
			{
				return this._box;
			}
		}

		public string Text
		{
			get
			{
				return this._text;
			}
		}

		public List<CssBoxWord> Words
		{
			get
			{
				return this._words;
			}
		}

		private CssBoxWordSplitter()
		{
			this._words = new List<CssBoxWord>();
			this._curword = null;
		}

		public CssBoxWordSplitter(CssBox box, string text) : this()
		{
			this._box = box;
			this._text = text.Replace("\r", string.Empty);
		}

		public static bool CollapsesWhiteSpaces(CssBox b)
		{
			if (b.WhiteSpace == "normal" || b.WhiteSpace == "nowrap")
			{
				return true;
			}
			return b.WhiteSpace == "pre-line";
		}

		private void CutWord()
		{
			if (this._curword.Text.Length > 0)
			{
				this.Words.Add(this._curword);
			}
			this._curword = new CssBoxWord(this.Box);
		}

		public static bool EliminatesLineBreaks(CssBox b)
		{
			if (b.WhiteSpace == "normal")
			{
				return true;
			}
			return b.WhiteSpace == "nowrap";
		}

		private bool IsLineBreak(char c)
		{
			if (c == '\n')
			{
				return true;
			}
			return c == '\a';
		}

		private bool IsSpace(char c)
		{
			if (c == ' ' || c == '\t')
			{
				return true;
			}
			return c == '\n';
		}

		private bool IsTab(char c)
		{
			return c == '\t';
		}

		public void SplitWords()
		{
			if (string.IsNullOrEmpty(this.Text))
			{
				return;
			}
			this._curword = new CssBoxWord(this.Box);
			bool flag = this.IsSpace(this.Text[0]);
			for (int i = 0; i < this.Text.Length; i++)
			{
				if (!this.IsSpace(this.Text[i]))
				{
					if (flag)
					{
						this.CutWord();
					}
					this._curword.AppendChar(this.Text[i]);
					flag = false;
				}
				else
				{
					if (!flag)
					{
						this.CutWord();
					}
					if (this.IsLineBreak(this.Text[i]))
					{
						this._curword.AppendChar('\n');
						this.CutWord();
					}
					else if (!this.IsTab(this.Text[i]))
					{
						this._curword.AppendChar(' ');
					}
					else
					{
						this._curword.AppendChar('\t');
						this.CutWord();
					}
					flag = true;
				}
			}
			this.CutWord();
		}
	}
}