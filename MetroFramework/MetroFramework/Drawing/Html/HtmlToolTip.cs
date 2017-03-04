using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MetroFramework.Drawing.Html
{
	public class HtmlToolTip : ToolTip
	{
		private InitialContainer container;

		public HtmlToolTip()
		{
			base.OwnerDraw = true;
			base.Popup += new PopupEventHandler(this.HtmlToolTip_Popup);
			base.Draw += new DrawToolTipEventHandler(this.HtmlToolTip_Draw);
		}

		private void HtmlToolTip_Draw(object sender, DrawToolTipEventArgs e)
		{
			e.Graphics.Clear(Color.White);
			if (this.container != null)
			{
				this.container.Paint(e.Graphics);
			}
		}

		private void HtmlToolTip_Popup(object sender, PopupEventArgs e)
		{
			string toolTip = base.GetToolTip(e.AssociatedControl);
			NumberFormatInfo invariantInfo = NumberFormatInfo.InvariantInfo;
			object[] size = new object[] { e.AssociatedControl.Font.Size, e.AssociatedControl.Font.FontFamily.Name };
			string str = string.Format(invariantInfo, "font: {0}pt {1}", size);
			string[] strArrays = new string[] { "<table class=htmltooltipbackground cellspacing=5 cellpadding=0 style=\"", str, "\"><tr><td style=border:0px>", toolTip, "</td></tr></table>" };
			this.container = new InitialContainer(string.Concat(strArrays));
			this.container.SetBounds(new Rectangle(0, 0, 10, 10));
			this.container.AvoidGeometryAntialias = true;
			using (Graphics graphic = e.AssociatedControl.CreateGraphics())
			{
				this.container.MeasureBounds(graphic);
			}
			e.ToolTipSize = Size.Round(this.container.MaximumSize);
		}
	}
}