using MetroFramework.Controls;
using MetroFramework.Native;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MetroFramework.Design.Controls
{
	internal class MetroTabControlDesigner : ParentControlDesigner
	{
		private readonly DesignerVerbCollection designerVerbs = new DesignerVerbCollection();

		private IDesignerHost designerHost;

		private ISelectionService selectionService;

		public IDesignerHost DesignerHost
		{
			get
			{
				IDesignerHost designerHost = this.designerHost;
				if (designerHost == null)
				{
					IDesignerHost service = (IDesignerHost)this.GetService(typeof(IDesignerHost));
					IDesignerHost designerHost1 = service;
					this.designerHost = service;
					designerHost = designerHost1;
				}
				return designerHost;
			}
		}

		public override System.Windows.Forms.Design.SelectionRules SelectionRules
		{
			get
			{
				if (this.Control.Dock == DockStyle.Fill)
				{
					return System.Windows.Forms.Design.SelectionRules.Visible;
				}
				return base.SelectionRules;
			}
		}

		public ISelectionService SelectionService
		{
			get
			{
				ISelectionService selectionService = this.selectionService;
				if (selectionService == null)
				{
					ISelectionService service = (ISelectionService)this.GetService(typeof(ISelectionService));
					ISelectionService selectionService1 = service;
					this.selectionService = service;
					selectionService = selectionService1;
				}
				return selectionService;
			}
		}

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this.designerVerbs.Count == 2)
				{
					MetroTabControl control = (MetroTabControl)this.Control;
					this.designerVerbs[1].Enabled = control.TabCount != 0;
				}
				return this.designerVerbs;
			}
		}

		public MetroTabControlDesigner()
		{
			DesignerVerb designerVerb = new DesignerVerb("Add Tab", new EventHandler(this.OnAddPage));
			DesignerVerb designerVerb1 = new DesignerVerb("Remove Tab", new EventHandler(this.OnRemovePage));
			this.designerVerbs.AddRange(new DesignerVerb[] { designerVerb, designerVerb1 });
		}

		protected override bool GetHitTest(Point point)
		{
			if (this.SelectionService.PrimarySelection == this.Control)
			{
				WinApi.TCHITTESTINFO tCHITTESTINFO = new WinApi.TCHITTESTINFO()
				{
					pt = this.Control.PointToClient(point),
					flags = 0
				};
				WinApi.TCHITTESTINFO tCHITTESTINFO1 = tCHITTESTINFO;
				Message message = new Message()
				{
					HWnd = this.Control.Handle,
					Msg = 4883
				};
				Message message1 = message;
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(tCHITTESTINFO1));
				Marshal.StructureToPtr(tCHITTESTINFO1, intPtr, false);
				message1.LParam = intPtr;
				base.WndProc(ref message1);
				Marshal.FreeHGlobal(intPtr);
				if (message1.Result.ToInt32() != -1)
				{                    
					return tCHITTESTINFO1.flags != 1;
				}
			}
			return false;
		}

		private void OnAddPage(object sender, EventArgs e)
		{
			MetroTabControl control = (MetroTabControl)this.Control;
			System.Windows.Forms.Control.ControlCollection controls = control.Controls;
			base.RaiseComponentChanging(TypeDescriptor.GetProperties(control)["TabPages"]);
			MetroTabPage name = (MetroTabPage)this.DesignerHost.CreateComponent(typeof(MetroTabPage));
			name.Text = name.Name;
			control.TabPages.Add(name);
			base.RaiseComponentChanged(TypeDescriptor.GetProperties(control)["TabPages"], controls, control.TabPages);
			control.SelectedTab = name;
			this.SetVerbs();
		}

		private void OnRemovePage(object sender, EventArgs e)
		{
			MetroTabControl control = (MetroTabControl)this.Control;
			System.Windows.Forms.Control.ControlCollection controls = control.Controls;
			if (control.SelectedIndex < 0)
			{
				return;
			}
			base.RaiseComponentChanging(TypeDescriptor.GetProperties(control)["TabPages"]);
			this.DesignerHost.DestroyComponent(control.TabPages[control.SelectedIndex]);
			base.RaiseComponentChanged(TypeDescriptor.GetProperties(control)["TabPages"], controls, control.TabPages);
			this.SelectionService.SetSelectedComponents(new IComponent[] { control }, SelectionTypes.Auto);
			this.SetVerbs();
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

		private void SetVerbs()
		{
			if (((MetroTabControl)this.Control).TabPages.Count == 0)
			{
				this.Verbs[1].Enabled = false;
				return;
			}
			this.Verbs[1].Enabled = true;
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg != 132)
			{
				return;
			}
			if (m.Result.ToInt32() == -1)
			{
				m.Result = (IntPtr)((long)1);
			}
		}
	}
}