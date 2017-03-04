using System;

namespace MetroFramework.Drawing.Html
{
	[CLSCompliant(false)]
	public class CssAnonymousSpaceBlockBox : CssAnonymousBlockBox
	{
		public CssAnonymousSpaceBlockBox(CssBox parent) : base(parent)
		{
			base.Display = "none";
		}

		public CssAnonymousSpaceBlockBox(CssBox parent, CssBox insertBefore) : base(parent, insertBefore)
		{
			base.Display = "none";
		}
	}
}