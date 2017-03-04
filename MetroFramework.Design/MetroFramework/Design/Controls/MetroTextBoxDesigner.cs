using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace MetroFramework.Design.Controls
{
	internal class MetroTextBoxDesigner : ControlDesigner
	{
		public override System.Windows.Forms.Design.SelectionRules SelectionRules
		{
			get
			{
				PropertyDescriptor item = TypeDescriptor.GetProperties(base.Component)["Multiline"];
				if (item == null)
				{
					return base.SelectionRules;
				}
				if ((bool)item.GetValue(base.Component))
				{
					return System.Windows.Forms.Design.SelectionRules.Moveable | System.Windows.Forms.Design.SelectionRules.Visible | System.Windows.Forms.Design.SelectionRules.TopSizeable | System.Windows.Forms.Design.SelectionRules.BottomSizeable | System.Windows.Forms.Design.SelectionRules.LeftSizeable | System.Windows.Forms.Design.SelectionRules.RightSizeable | System.Windows.Forms.Design.SelectionRules.AllSizeable;
				}
				return System.Windows.Forms.Design.SelectionRules.Moveable | System.Windows.Forms.Design.SelectionRules.Visible | System.Windows.Forms.Design.SelectionRules.LeftSizeable | System.Windows.Forms.Design.SelectionRules.RightSizeable;
			}
		}

		public MetroTextBoxDesigner()
		{
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			properties.Remove("BackgroundImage");
			properties.Remove("ImeMode");
			properties.Remove("Padding");
			properties.Remove("BackgroundImageLayout");
			properties.Remove("Font");
			base.PreFilterProperties(properties);
		}
	}
}