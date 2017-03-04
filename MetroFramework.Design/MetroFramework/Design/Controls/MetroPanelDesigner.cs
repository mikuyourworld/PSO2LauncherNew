using MetroFramework.Controls;
using System;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace MetroFramework.Design.Controls
{
	internal class MetroPanelDesigner : ParentControlDesigner
	{
		public MetroPanelDesigner()
		{
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			MetroPanel control = this.Control as MetroPanel;
		}
	}
}