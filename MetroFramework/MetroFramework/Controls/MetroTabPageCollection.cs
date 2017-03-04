using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[Editor("MetroFramework.Design.MetroTabPageCollectionEditor, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a", typeof(UITypeEditor))]
	[ToolboxItem(false)]
	public class MetroTabPageCollection : TabControl.TabPageCollection
	{
		public MetroTabPageCollection(MetroTabControl owner) : base(owner)
		{
		}
	}
}