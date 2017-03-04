using MetroFramework.Controls;
using System;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MetroFramework.Design.Controls
{
	internal class MetroTileDesigner : ParentControlDesigner
	{
		public override System.Windows.Forms.Design.SelectionRules SelectionRules
		{
			get
			{
				return base.SelectionRules;
			}
		}

		public MetroTileDesigner()
		{
		}

		public override bool CanParent(System.Windows.Forms.Control control)
		{
			if (control is MetroLabel)
			{
				return true;
			}
			return control is MetroProgressSpinner;
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			properties.Remove("ImeMode");
			properties.Remove("Padding");
			properties.Remove("FlatAppearance");
			properties.Remove("FlatStyle");
			properties.Remove("AutoEllipsis");
			properties.Remove("UseCompatibleTextRendering");
			properties.Remove("Image");
			properties.Remove("ImageAlign");
			properties.Remove("ImageIndex");
			properties.Remove("ImageKey");
			properties.Remove("ImageList");
			properties.Remove("TextImageRelation");
			properties.Remove("BackgroundImage");
			properties.Remove("BackgroundImageLayout");
			properties.Remove("UseVisualStyleBackColor");
			properties.Remove("Font");
			properties.Remove("RightToLeft");
			base.PreFilterProperties(properties);
		}
	}
}