using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace MetroFramework.Design.Components
{
	internal class MetroStyleManagerDesigner : ComponentDesigner
	{
		private DesignerVerbCollection designerVerbs;

		private IDesignerHost designerHost;

		private IComponentChangeService componentChangeService;

		public IComponentChangeService ComponentChangeService
		{
			get
			{
				if (this.componentChangeService != null)
				{
					return this.componentChangeService;
				}
				this.componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
				return this.componentChangeService;
			}
		}

		public IDesignerHost DesignerHost
		{
			get
			{
				if (this.designerHost != null)
				{
					return this.designerHost;
				}
				this.designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
				return this.designerHost;
			}
		}

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this.designerVerbs != null)
				{
					return this.designerVerbs;
				}
				this.designerVerbs = new DesignerVerbCollection();
				this.designerVerbs.Add(new DesignerVerb("Reset Styles to Default", new EventHandler(this.OnResetStyles)));
				return this.designerVerbs;
			}
		}

		public MetroStyleManagerDesigner()
		{
		}

		private void OnResetStyles(object sender, EventArgs args)
		{
			MetroStyleManager component = base.Component as MetroStyleManager;
			if (component == null || component.Owner != null)
			{
				this.ResetStyles(component, component.Owner);
				return;
			}
			MessageBox.Show("StyleManager needs the Owner property assigned to before it can reset styles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private void ResetProperty(Control control, string name, object newValue)
		{
			PropertyDescriptor item = TypeDescriptor.GetProperties(control)[name];
			if (item == null)
			{
				return;
			}
			object value = item.GetValue(control);
			if (newValue.Equals(value))
			{
				return;
			}
			this.ComponentChangeService.OnComponentChanging(control, item);
			item.SetValue(control, newValue);
			this.ComponentChangeService.OnComponentChanged(control, item, value, newValue);
		}

		private void ResetStyles(MetroStyleManager styleManager, Control control)
		{
			IMetroForm metroForm = control as IMetroForm;
			if (metroForm != null && !object.ReferenceEquals(styleManager, metroForm.StyleManager))
			{
				return;
			}
			if (control is IMetroControl)
			{
				this.ResetProperty(control, "Style", MetroColorStyle.Default);
				this.ResetProperty(control, "Theme", MetroThemeStyle.Default);
			}
			else if (control is IMetroComponent)
			{
				this.ResetProperty(control, "Style", MetroColorStyle.Default);
				this.ResetProperty(control, "Theme", MetroThemeStyle.Default);
			}
			if (control.ContextMenuStrip != null)
			{
				this.ResetStyles(styleManager, control.ContextMenuStrip);
			}
			TabControl tabControl = control as TabControl;
			if (tabControl != null)
			{
				foreach (TabPage tabPage in tabControl.TabPages)
				{
					this.ResetStyles(styleManager, tabPage);
				}
			}
			if (control.Controls != null)
			{
				foreach (Control control1 in control.Controls)
				{
					this.ResetStyles(styleManager, control1);
				}
			}
		}
	}
}