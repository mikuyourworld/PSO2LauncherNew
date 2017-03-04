using System.Collections;
using System.Windows.Forms.Design;

namespace MetroFramework.Design.Controls
{
	internal class MetroButtonDesigner : ControlDesigner
	{
		public override SelectionRules SelectionRules { get { return base.SelectionRules; } }

        public MetroButtonDesigner() { }

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
			properties.Remove("UseVisualStyleBackColor");
			properties.Remove("Font");
			properties.Remove("RightToLeft");
			base.PreFilterProperties(properties);
		}
	}
}