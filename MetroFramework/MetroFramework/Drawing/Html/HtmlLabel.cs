using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework.Drawing.Html
{
	[CLSCompliant(false)]
	public class HtmlLabel : HtmlPanel
	{
		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Automatically sets the size of the label by measuring the content")]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
				if (value)
				{
					this.MeasureBounds();
				}
			}
		}

		public HtmlLabel()
		{
			base.SetStyle(ControlStyles.Opaque, false);
			this.AutoScroll = false;
		}

		protected override void CreateFragment()
		{
			string text = this.Text;
			string str = string.Format("font: {0}pt {1}", this.Font.Size, this.Font.FontFamily.Name);
			string[] strArrays = new string[] { "<table border=0 cellspacing=5 cellpadding=0 style=\"", str, "\"><tr><td>", text, "</td></tr></table>" };
			this.htmlContainer = new InitialContainer(string.Concat(strArrays));
		}

		public override void MeasureBounds()
		{
			base.MeasureBounds();
			if (this.htmlContainer != null && this.AutoSize)
			{
				base.Size = System.Drawing.Size.Round(this.htmlContainer.MaximumSize);
			}
		}
	}
}