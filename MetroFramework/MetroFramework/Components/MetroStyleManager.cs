using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace MetroFramework.Components
{
	[Designer("MetroFramework.Design.Components.MetroStyleManagerDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	public sealed class MetroStyleManager : Component, ICloneable, ISupportInitialize
	{
		private readonly IContainer parentContainer;

		private MetroColorStyle metroStyle = MetroColorStyle.Blue;

		private MetroThemeStyle metroTheme = MetroThemeStyle.Light;

		private ContainerControl owner;

		private bool isInitializing;

		public ContainerControl Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				if (this.owner != null)
				{
					this.owner.ControlAdded -= new ControlEventHandler(this.ControlAdded);
				}
				this.owner = value;
				if (value != null)
				{
					this.owner.ControlAdded += new ControlEventHandler(this.ControlAdded);
					if (!this.isInitializing)
					{
						this.UpdateControl(value);
					}
				}
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroColorStyle.Blue)]
		public MetroColorStyle Style
		{
			get
			{
				return this.metroStyle;
			}
			set
			{
				if (value == MetroColorStyle.Default)
				{
					value = MetroColorStyle.Blue;
				}
				this.metroStyle = value;
				if (!this.isInitializing)
				{
					this.Update();
				}
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroThemeStyle.Light)]
		public MetroThemeStyle Theme
		{
			get
			{
				return this.metroTheme;
			}
			set
			{
				if (value == MetroThemeStyle.Default)
				{
					value = MetroThemeStyle.Light;
				}
				this.metroTheme = value;
				if (!this.isInitializing)
				{
					this.Update();
				}
			}
		}

		public MetroStyleManager()
		{
		}

		public MetroStyleManager(IContainer parentContainer) : this()
		{
			if (parentContainer != null)
			{
				this.parentContainer = parentContainer;
				this.parentContainer.Add(this);
			}
		}

		private void ApplyTheme(IMetroControl control)
		{
			control.StyleManager = this;
		}

		private void ApplyTheme(IMetroComponent component)
		{
			component.StyleManager = this;
		}

		public object Clone()
		{
			MetroStyleManager metroStyleManager = new MetroStyleManager()
			{
				metroTheme = this.Theme,
				metroStyle = this.Style
			};
			return metroStyleManager;
		}

		public object Clone(ContainerControl owner)
		{
			MetroStyleManager metroStyleManager = this.Clone() as MetroStyleManager;
			if (owner is IMetroForm)
			{
				metroStyleManager.Owner = owner;
				((IMetroForm)owner).StyleManager = metroStyleManager;
				FieldInfo field = owner.GetType().GetField("components", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field == null)
				{
					return metroStyleManager;
				}
				IContainer value = (IContainer)field.GetValue(owner);
				if (value == null)
				{
					return metroStyleManager;
				}
				foreach (Component component in value.Components)
				{
					if (component is IMetroComponent)
					{
						this.ApplyTheme((IMetroComponent)component);
					}
					if (component.GetType() != typeof(MetroContextMenu))
					{
						continue;
					}
					this.ApplyTheme((MetroContextMenu)component);
				}
			}
			return metroStyleManager;
		}

		private void ControlAdded(object sender, ControlEventArgs e)
		{
			if (!this.isInitializing)
			{
				this.UpdateControl(e.Control);
			}
		}

		void System.ComponentModel.ISupportInitialize.BeginInit()
		{
			this.isInitializing = true;
		}

		void System.ComponentModel.ISupportInitialize.EndInit()
		{
			this.isInitializing = false;
			this.Update();
		}

		public void Update()
		{
			if (this.owner != null)
			{
				this.UpdateControl(this.owner);
			}
			if (this.parentContainer == null || this.parentContainer.Components == null)
			{
				return;
			}
			foreach (object component in this.parentContainer.Components)
			{
				if (component is IMetroComponent)
				{
					this.ApplyTheme((IMetroComponent)component);
				}
				if (component.GetType() != typeof(MetroContextMenu))
				{
					continue;
				}
				this.ApplyTheme((MetroContextMenu)component);
			}
		}

		private void UpdateControl(Control ctrl)
		{
			if (ctrl == null)
			{
				return;
			}
			IMetroControl metroControl = ctrl as IMetroControl;
			if (metroControl != null)
			{
				this.ApplyTheme(metroControl);
			}
			IMetroComponent metroComponent = ctrl as IMetroComponent;
			if (metroComponent != null)
			{
				this.ApplyTheme(metroComponent);
			}
			if (ctrl is TabControl)
			{
				foreach (TabPage tabPage in ((TabControl)ctrl).TabPages)
				{
					this.UpdateControl(tabPage);
				}
			}
			if (ctrl.Controls != null)
			{
				foreach (Control control in ctrl.Controls)
				{
					this.UpdateControl(control);
				}
			}
			if (ctrl.ContextMenuStrip != null)
			{
				this.UpdateControl(ctrl.ContextMenuStrip);
			}
			ctrl.Refresh();
		}
	}
}