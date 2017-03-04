using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MetroFramework.Drawing.Html
{
	public class HtmlTag
	{
		private string _tagName;

		private bool _isClosing;

		private Dictionary<string, string> _attributes;

		public Dictionary<string, string> Attributes
		{
			get
			{
				return this._attributes;
			}
		}

		public bool IsClosing
		{
			get
			{
				return this._isClosing;
			}
		}

		public bool IsSingle
		{
			get
			{
				if (this.TagName.StartsWith("!"))
				{
					return true;
				}
				string[] strArrays = new string[] { "area", "base", "basefont", "br", "col", "frame", "hr", "img", "input", "isindex", "link", "meta", "param" };
				return (new List<string>(strArrays)).Contains(this.TagName);
			}
		}

		public string TagName
		{
			get
			{
				return this._tagName;
			}
		}

		private HtmlTag()
		{
			this._attributes = new Dictionary<string, string>();
		}

		public HtmlTag(string tag) : this()
		{
			tag = tag.Substring(1, tag.Length - 2);
			int num = tag.IndexOf(" ");
			if (num >= 0)
			{
				this._tagName = tag.Substring(0, num);
			}
			else
			{
				this._tagName = tag;
			}
			if (this._tagName.StartsWith("/"))
			{
				this._isClosing = true;
				this._tagName = this._tagName.Substring(1);
			}
			this._tagName = this._tagName.ToLower();
			foreach (Match match in Parser.Match("[^\\s]*\\s*=\\s*(\"[^\"]*\"|[^\\s]*)", tag))
			{
				string[] strArrays = match.Value.Split(new char[] { '=' });
				if ((int)strArrays.Length != 1)
				{
					if ((int)strArrays.Length != 2)
					{
						continue;
					}
					string str = strArrays[0].Trim();
					string str1 = strArrays[1].Trim();
					if (str1.StartsWith("\"") && str1.EndsWith("\"") && str1.Length > 2)
					{
						str1 = str1.Substring(1, str1.Length - 2);
					}
					if (this.Attributes.ContainsKey(str))
					{
						continue;
					}
					this.Attributes.Add(str, str1);
				}
				else
				{
					if (this.Attributes.ContainsKey(strArrays[0]))
					{
						continue;
					}
					this.Attributes.Add(strArrays[0].ToLower(), string.Empty);
				}
			}
		}

		private void ApplyTableBorder(CssBox table, string border)
		{
			foreach (CssBox box in table.Boxes)
			{
				foreach (CssBox cssBox in box.Boxes)
				{
					cssBox.BorderWidth = this.TranslateLength(border);
				}
			}
		}

		private void ApplyTablePadding(CssBox table, string padding)
		{
			foreach (CssBox box in table.Boxes)
			{
				foreach (CssBox cssBox in box.Boxes)
				{
					cssBox.Padding = this.TranslateLength(padding);
				}
			}
		}

		public bool HasAttribute(string attribute)
		{
			return this.Attributes.ContainsKey(attribute);
		}

		public override string ToString()
		{
			return string.Format("<{1}{0}>", this.TagName, (this.IsClosing ? "/" : string.Empty));
		}

		internal void TranslateAttributes(CssBox box)
		{
			int num;
			string upper = this.TagName.ToUpper();
			foreach (string key in this.Attributes.Keys)
			{
				string item = this.Attributes[key];
				string str = key;
				string str1 = str;
				if (str == null)
				{
					continue;
				}
				if (PrivateImplementationDetails_57814A66_940D_4455_B30A_E2997453B959._method0x6000752_1 == null)
				{
					PrivateImplementationDetails_57814A66_940D_4455_B30A_E2997453B959._method0x6000752_1 = new Dictionary<string, int>(17)
					{
						{ "align", 0 },
						{ "background", 1 },
						{ "bgcolor", 2 },
						{ "border", 3 },
						{ "bordercolor", 4 },
						{ "cellspacing", 5 },
						{ "cellpadding", 6 },
						{ "color", 7 },
						{ "dir", 8 },
						{ "face", 9 },
						{ "height", 10 },
						{ "hspace", 11 },
						{ "nowrap", 12 },
						{ "size", 13 },
						{ "valign", 14 },
						{ "vspace", 15 },
						{ "width", 16 }
					};
				}
				if (!PrivateImplementationDetails_57814A66_940D_4455_B30A_E2997453B959._method0x6000752_1.TryGetValue(str1, out num))
				{
					continue;
				}
				switch (num)
				{
					case 0:
					{
						if (item == "left" || item == "center" || item == "right" || item == "justify")
						{
							box.TextAlign = item;
							continue;
						}
						else
						{
							box.VerticalAlign = item;
							continue;
						}
					}
					case 1:
					{
						box.BackgroundImage = item;
						continue;
					}
					case 2:
					{
						box.BackgroundColor = item;
						continue;
					}
					case 3:
					{
						box.BorderWidth = this.TranslateLength(item);
						if (upper != "TABLE")
						{
							box.BorderStyle = "solid";
							continue;
						}
						else
						{
							this.ApplyTableBorder(box, item);
							continue;
						}
					}
					case 4:
					{
						box.BorderColor = item;
						continue;
					}
					case 5:
					{
						box.BorderSpacing = this.TranslateLength(item);
						continue;
					}
					case 6:
					{
						this.ApplyTablePadding(box, item);
						continue;
					}
					case 7:
					{
						box.Color = item;
						continue;
					}
					case 8:
					{
						box.Direction = item;
						continue;
					}
					case 9:
					{
						box.FontFamily = item;
						continue;
					}
					case 10:
					{
						box.Height = this.TranslateLength(item);
						continue;
					}
					case 11:
					{
						string str2 = this.TranslateLength(item);
						string str3 = str2;
						box.MarginLeft = str2;
						box.MarginRight = str3;
						continue;
					}
					case 12:
					{
						box.WhiteSpace = "nowrap";
						continue;
					}
					case 13:
					{
						if (upper != "HR")
						{
							continue;
						}
						box.Height = this.TranslateLength(item);
						continue;
					}
					case 14:
					{
						box.VerticalAlign = item;
						continue;
					}
					case 15:
					{
						string str4 = this.TranslateLength(item);
						string str5 = str4;
						box.MarginBottom = str4;
						box.MarginTop = str5;
						continue;
					}
					case 16:
					{
						box.Width = this.TranslateLength(item);
						continue;
					}
					default:
					{
						continue;
					}
				}
			}
		}

		private string TranslateLength(string htmlLength)
		{
			if (!(new CssLength(htmlLength)).HasError)
			{
				return htmlLength;
			}
			return string.Concat(htmlLength, "px");
		}
	}
}