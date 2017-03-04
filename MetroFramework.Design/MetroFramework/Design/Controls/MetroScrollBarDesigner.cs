using MetroFramework.Controls;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace MetroFramework.Design.Controls
{
	[Designer(typeof(ScrollableControlDesigner), typeof(ParentControlDesigner))]
	internal class MetroScrollBarDesigner : ControlDesigner
	{
		public override System.Windows.Forms.Design.SelectionRules SelectionRules
		{
			get
			{
				PropertyDescriptor item = TypeDescriptor.GetProperties(base.Component)["Orientation"];
				if (item == null)
				{
					return base.SelectionRules;
				}
				if ((MetroScrollOrientation)item.GetValue(base.Component) == MetroScrollOrientation.Vertical)
				{
					return System.Windows.Forms.Design.SelectionRules.Moveable | System.Windows.Forms.Design.SelectionRules.Visible | System.Windows.Forms.Design.SelectionRules.TopSizeable | System.Windows.Forms.Design.SelectionRules.BottomSizeable;
				}
				return System.Windows.Forms.Design.SelectionRules.Moveable | System.Windows.Forms.Design.SelectionRules.Visible | System.Windows.Forms.Design.SelectionRules.LeftSizeable | System.Windows.Forms.Design.SelectionRules.RightSizeable;
			}
		}

		public MetroScrollBarDesigner()
		{
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			properties.Remove("Text");
			properties.Remove("BackgroundImage");
			properties.Remove("ForeColor");
			properties.Remove("ImeMode");
			properties.Remove("Padding");
			properties.Remove("BackgroundImageLayout");
			properties.Remove("BackColor");
			properties.Remove("Font");
			properties.Remove("RightToLeft");
			base.PreFilterProperties(properties);
		}
	}
}