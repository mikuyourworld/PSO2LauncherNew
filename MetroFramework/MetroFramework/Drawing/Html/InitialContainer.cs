using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace MetroFramework.Drawing.Html
{
	[CLSCompliant(false)]
	public class InitialContainer : CssBox
	{
		private Dictionary<string, Dictionary<string, CssBlock>> _media_blocks;

		private string _documentSource;

		private bool _avoidGeometryAntialias;

		private SizeF _maxSize;

		private PointF _scrollOffset;

		private Dictionary<CssBox, RectangleF> _linkRegions;

		private bool _avoidTextAntialias;

		public bool AvoidGeometryAntialias
		{
			get
			{
				return this._avoidGeometryAntialias;
			}
			set
			{
				this._avoidGeometryAntialias = value;
			}
		}

		public bool AvoidTextAntialias
		{
			get
			{
				return this._avoidTextAntialias;
			}
			set
			{
				this._avoidTextAntialias = value;
			}
		}

		public string DocumentSource
		{
			get
			{
				return this._documentSource;
			}
		}

		internal Dictionary<CssBox, RectangleF> LinkRegions
		{
			get
			{
				return this._linkRegions;
			}
		}

		public SizeF MaximumSize
		{
			get
			{
				return this._maxSize;
			}
			set
			{
				this._maxSize = value;
			}
		}

		public Dictionary<string, Dictionary<string, CssBlock>> MediaBlocks
		{
			get
			{
				return this._media_blocks;
			}
		}

		public PointF ScrollOffset
		{
			get
			{
				return this._scrollOffset;
			}
			set
			{
				this._scrollOffset = value;
			}
		}

		public InitialContainer()
		{
			this._initialContainer = this;
			this._media_blocks = new Dictionary<string, Dictionary<string, CssBlock>>();
			this._linkRegions = new Dictionary<CssBox, RectangleF>();
			this.MediaBlocks.Add("all", new Dictionary<string, CssBlock>());
			base.Display = "block";
			this.FeedStyleSheet("\r\n\r\n        \r\n        html, address,\r\n        blockquote,\r\n        body, dd, div,\r\n        dl, dt, fieldset, form,\r\n        frame, frameset,\r\n        h1, h2, h3, h4,\r\n        h5, h6, noframes,\r\n        ol, p, ul, center,\r\n        dir, hr, menu, pre   { display: block }\r\n        li              { display: list-item }\r\n        head            { display: none }\r\n        table           { display: table }\r\n        tr              { display: table-row }\r\n        thead           { display: table-header-group }\r\n        tbody           { display: table-row-group }\r\n        tfoot           { display: table-footer-group }\r\n        col             { display: table-column }\r\n        colgroup        { display: table-column-group }\r\n        td, th          { display: table-cell }\r\n        caption         { display: table-caption }\r\n        th              { font-weight: bolder; text-align: center }\r\n        caption         { text-align: center }\r\n        body            { margin: 8px }\r\n        h1              { font-size: 2em; margin: .67em 0 }\r\n        h2              { font-size: 1.5em; margin: .75em 0 }\r\n        h3              { font-size: 1.17em; margin: .83em 0 }\r\n        h4, p,\r\n        blockquote, ul,\r\n        fieldset, form,\r\n        ol, dl, dir,\r\n        menu            { margin: 1.12em 0 }\r\n        h5              { font-size: .83em; margin: 1.5em 0 }\r\n        h6              { font-size: .75em; margin: 1.67em 0 }\r\n        h1, h2, h3, h4,\r\n        h5, h6, b,\r\n        strong          { font-weight: bolder; }\r\n        blockquote      { margin-left: 40px; margin-right: 40px }\r\n        i, cite, em,\r\n        var, address    { font-style: italic }\r\n        pre, tt, code,\r\n        kbd, samp       { font-family: monospace }\r\n        pre             { white-space: pre }\r\n        button, textarea,\r\n        input, select   { display: inline-block }\r\n        big             { font-size: 1.17em }\r\n        small, sub, sup { font-size: .83em }\r\n        sub             { vertical-align: sub }\r\n        sup             { vertical-align: super }\r\n        table           { border-spacing: 2px; }\r\n        thead, tbody,\r\n        tfoot           { vertical-align: middle }\r\n        td, th          { vertical-align: inherit }\r\n        s, strike, del  { text-decoration: line-through }\r\n        hr              { border: 1px inset }\r\n        ol, ul, dir,\r\n        menu, dd        { margin-left: 40px }\r\n        ol              { list-style-type: decimal }\r\n        ol ul, ul ol,\r\n        ul ul, ol ol    { margin-top: 0; margin-bottom: 0 }\r\n        u, ins          { text-decoration: underline }\r\n        br:before       { content: \"\\A\" }\r\n        :before, :after { white-space: pre-line }\r\n        center          { text-align: center }\r\n        :link, :visited { text-decoration: underline }\r\n        :focus          { outline: thin dotted invert }\r\n\r\n        /* Begin bidirectionality settings (do not change) */\r\n        BDO[DIR=\"ltr\"]  { direction: ltr; unicode-bidi: bidi-override }\r\n        BDO[DIR=\"rtl\"]  { direction: rtl; unicode-bidi: bidi-override }\r\n\r\n        *[DIR=\"ltr\"]    { direction: ltr; unicode-bidi: embed }\r\n        *[DIR=\"rtl\"]    { direction: rtl; unicode-bidi: embed }\r\n\r\n        @media print {\r\n          h1            { page-break-before: always }\r\n          h1, h2, h3,\r\n          h4, h5, h6    { page-break-after: avoid }\r\n          ul, ol, dl    { page-break-before: avoid }\r\n        }\r\n\r\n        /* Not in the specification but necessary */\r\n        a               { color:blue; text-decoration:underline }\r\n        table           { border-color:#dfdfdf; border-style:outset; }\r\n        td, th          { border-color:#dfdfdf; border-style:inset; }\r\n        style, title,\r\n        script, link,\r\n        meta, area,\r\n        base, param     { display:none }\r\n        hr              { border-color: #ccc }  \r\n        pre             { font-size:10pt }\r\n        \r\n        /*This is the background of the HtmlToolTip*/\r\n        .htmltooltipbackground {\r\n              border:solid 1px #767676;\r\n              corner-radius:3px;\r\n              background-color:#white;\r\n              background-gradient:#E4E5F0;\r\n        }\r\n\r\n        ");
		}

		public InitialContainer(string documentSource) : this()
		{
			this._documentSource = documentSource;
			this.ParseDocument();
			this.CascadeStyles(this);
			this.BlockCorrection(this);
		}

		private void BlockCorrection(CssBox startBox)
		{
			if (!startBox.ContainsInlinesOnly())
			{
				foreach (List<CssBox> cssBoxes in this.BlockCorrection_GetInlineGroups(startBox))
				{
					if (cssBoxes.Count == 0)
					{
						continue;
					}
					if (cssBoxes.Count != 1 || !(cssBoxes[0] is CssAnonymousSpaceBox))
					{
						CssAnonymousBlockBox cssAnonymousBlockBox = new CssAnonymousBlockBox(startBox, cssBoxes[0]);
						foreach (CssBox cssBox in cssBoxes)
						{
							cssBox.ParentBox = cssAnonymousBlockBox;
						}
					}
					else
					{
						CssAnonymousSpaceBlockBox cssAnonymousSpaceBlockBox = new CssAnonymousSpaceBlockBox(startBox, cssBoxes[0]);
						cssBoxes[0].ParentBox = cssAnonymousSpaceBlockBox;
					}
				}
			}
			foreach (CssBox box in startBox.Boxes)
			{
				this.BlockCorrection(box);
			}
		}

		private List<List<CssBox>> BlockCorrection_GetInlineGroups(CssBox box)
		{
			List<List<CssBox>> lists = new List<List<CssBox>>();
			List<CssBox> cssBoxes = null;
			for (int i = 0; i < box.Boxes.Count; i++)
			{
				CssBox item = box.Boxes[i];
				if (item.Display != "inline")
				{
					cssBoxes = null;
				}
				else
				{
					if (cssBoxes == null)
					{
						cssBoxes = new List<CssBox>();
						lists.Add(cssBoxes);
					}
					cssBoxes.Add(item);
				}
			}
			if (lists.Count > 0 && lists[lists.Count - 1].Count == 0)
			{
				lists.RemoveAt(lists.Count - 1);
			}
			return lists;
		}

		private void CascadeStyles(CssBox startBox)
		{
			bool flag = false;
			foreach (CssBox box in startBox.Boxes)
			{
				box.InheritStyle();
				if (box.HtmlTag != null)
				{
					if (this.MediaBlocks["all"].ContainsKey(box.HtmlTag.TagName))
					{
						this.MediaBlocks["all"][box.HtmlTag.TagName].AssignTo(box);
					}
					if (box.HtmlTag.HasAttribute("class") && this.MediaBlocks["all"].ContainsKey(string.Concat(".", box.HtmlTag.Attributes["class"])))
					{
						this.MediaBlocks["all"][string.Concat(".", box.HtmlTag.Attributes["class"])].AssignTo(box);
					}
					box.HtmlTag.TranslateAttributes(box);
					if (box.HtmlTag.HasAttribute("style"))
					{
						CssBlock cssBlock = new CssBlock(box.HtmlTag.Attributes["style"]);
						cssBlock.AssignTo(box);
					}
					if (box.HtmlTag.TagName.Equals("style", StringComparison.CurrentCultureIgnoreCase) && box.Boxes.Count == 1)
					{
						this.FeedStyleSheet(box.Boxes[0].Text);
					}
					if (box.HtmlTag.TagName.Equals("link", StringComparison.CurrentCultureIgnoreCase) && box.GetAttribute("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
					{
						this.FeedStyleSheet(CssValue.GetStyleSheet(box.GetAttribute("href", string.Empty)));
					}
				}
				this.CascadeStyles(box);
			}
			if (flag)
			{
				foreach (CssBox cssBox in startBox.Boxes)
				{
					cssBox.Display = "block";
				}
			}
		}

		private void FeedStyleBlock(string media, string block)
		{
			if (string.IsNullOrEmpty(media))
			{
				media = "all";
			}
			int num = block.IndexOf("{");
			string str = block.Substring(num).Replace("{", string.Empty).Replace("}", string.Empty);
			if (num < 0)
			{
				return;
			}
			string[] strArrays = block.Substring(0, num).Split(new char[] { ',' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str1 = strArrays[i].Trim();
				if (!string.IsNullOrEmpty(str1))
				{
					CssBlock cssBlock = new CssBlock(str);
					if (!this.MediaBlocks.ContainsKey(media))
					{
						this.MediaBlocks.Add(media, new Dictionary<string, CssBlock>());
					}
					if (this.MediaBlocks[media].ContainsKey(str1))
					{
						CssBlock item = this.MediaBlocks[media][str1];
						foreach (string key in cssBlock.Properties.Keys)
						{
							if (!item.Properties.ContainsKey(key))
							{
								item.Properties.Add(key, cssBlock.Properties[key]);
							}
							else
							{
								item.Properties[key] = cssBlock.Properties[key];
							}
						}
						item.UpdatePropertyValues();
					}
					else
					{
						this.MediaBlocks[media].Add(str1, cssBlock);
					}
				}
			}
		}

		public void FeedStyleSheet(string stylesheet)
		{
			if (string.IsNullOrEmpty(stylesheet))
			{
				return;
			}
			stylesheet = stylesheet.ToLower();
			for (MatchCollection i = Parser.Match("/\\*[^*/]*\\*/", stylesheet); i.Count > 0; i = Parser.Match("/\\*[^*/]*\\*/", stylesheet))
			{
				stylesheet = stylesheet.Remove(i[0].Index, i[0].Length);
			}
			for (MatchCollection j = Parser.Match("@.*\\{\\s*(\\s*[^\\{\\}]*\\{[^\\{\\}]*\\}\\s*)*\\s*\\}", stylesheet); j.Count > 0; j = Parser.Match("@.*\\{\\s*(\\s*[^\\{\\}]*\\{[^\\{\\}]*\\}\\s*)*\\s*\\}", stylesheet))
			{
				Match item = j[0];
				string value = item.Value;
				stylesheet = stylesheet.Remove(item.Index, item.Length);
				if (value.StartsWith("@media"))
				{
					MatchCollection matchCollections = Parser.Match("@media[^\\{\\}]*\\{", value);
					if (matchCollections.Count == 1)
					{
						string str = matchCollections[0].Value;
						if (str.StartsWith("@media") && str.EndsWith("{"))
						{
							string[] strArrays = str.Substring(6, str.Length - 7).Split(new char[] { ' ' });
							for (int k = 0; k < (int)strArrays.Length; k++)
							{
								if (!string.IsNullOrEmpty(strArrays[k].Trim()))
								{
									foreach (Match match in Parser.Match("[^\\{\\}]*\\{[^\\{\\}]*\\}", value))
									{
										this.FeedStyleBlock(strArrays[k].Trim(), match.Value);
									}
								}
							}
						}
					}
				}
			}
			foreach (Match match1 in Parser.Match("[^\\{\\}]*\\{[^\\{\\}]*\\}", stylesheet))
			{
				this.FeedStyleBlock("all", match1.Value);
			}
		}

		private CssBox FindParent(string tagName, CssBox b)
		{
			if (b == null)
			{
				return base.InitialContainer;
			}
			if (b.HtmlTag == null || !b.HtmlTag.TagName.Equals(tagName, StringComparison.CurrentCultureIgnoreCase))
			{
				return this.FindParent(tagName, b.ParentBox);
			}
			if (b.ParentBox != null)
			{
				return b.ParentBox;
			}
			return base.InitialContainer;
		}

		public override void MeasureBounds(Graphics g)
		{
			this.LinkRegions.Clear();
			base.MeasureBounds(g);
		}

		private void ParseDocument()
		{
			InitialContainer initialContainer = this;
			MatchCollection matchCollections = Parser.Match("<[^<>]*>", this.DocumentSource);
			CssBox cssBox = initialContainer;
			int index = -1;
			foreach (Match match in matchCollections)
			{
				string str = (match.Index > 0 ? this.DocumentSource.Substring(index + 1, match.Index - index - 1) : string.Empty);
				if (!string.IsNullOrEmpty(str.Trim()))
				{
					(new CssAnonymousBox(cssBox)).Text = str;
				}
				else if (str != null && str.Length > 0)
				{
					(new CssAnonymousSpaceBox(cssBox)).Text = str;
				}
				HtmlTag htmlTag = new HtmlTag(match.Value);
				if (htmlTag.IsClosing)
				{
					cssBox = this.FindParent(htmlTag.TagName, cssBox);
				}
				else if (!htmlTag.IsSingle)
				{
					cssBox = new CssBox(cssBox, htmlTag);
				}
				else
				{
					CssBox cssBox1 = new CssBox(cssBox, htmlTag);
				}
				index = match.Index + match.Length - 1;
			}
			string str1 = this.DocumentSource.Substring((index > 0 ? index + 1 : 0), this.DocumentSource.Length - index - 1 + (index == 0 ? 1 : 0));
			if (!string.IsNullOrEmpty(str1))
			{
				(new CssAnonymousBox(cssBox)).Text = str1;
			}
		}
	}
}