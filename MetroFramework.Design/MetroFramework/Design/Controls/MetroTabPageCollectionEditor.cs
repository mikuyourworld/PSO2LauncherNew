using MetroFramework.Controls;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace MetroFramework.Design.Controls
{
	internal class MetroTabPageCollectionEditor : CollectionEditor
	{
		public MetroTabPageCollectionEditor(Type type) : base(type)
		{
		}

		protected override CollectionEditor.CollectionForm CreateCollectionForm()
		{
			CollectionEditor.CollectionForm collectionForm = base.CreateCollectionForm();
			collectionForm.Text = "MetroTabPage Collection Editor";
			return collectionForm;
		}

		protected override Type CreateCollectionItemType()
		{
			return typeof(MetroTabPage);
		}

		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(MetroTabPage) };
		}
	}
}