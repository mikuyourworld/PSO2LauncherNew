using System;
using System.Collections.Generic;

namespace MetroFramework.Drawing.Html
{
	[CLSCompliant(false)]
	public class CssAnonymousBlockBox : CssBox
	{
		public CssAnonymousBlockBox(CssBox parent) : base(parent)
		{
			base.Display = "block";
		}

		public CssAnonymousBlockBox(CssBox parent, CssBox insertBefore) : this(parent)
		{
			int num = parent.Boxes.IndexOf(insertBefore);
			if (num < 0)
			{
				throw new Exception("insertBefore box doesn't exist on parent");
			}
			parent.Boxes.Remove(this);
			parent.Boxes.Insert(num, this);
		}
	}
}