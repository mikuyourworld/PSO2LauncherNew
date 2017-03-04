using MetroFramework;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework.Components
{
	[ProvideProperty("ApplyMetroTheme", typeof(Control))]
	public sealed class MetroStyleExtender : Component, IExtenderProvider, IMetroComponent
	{
		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private readonly List<Control> extendedControls = new List<Control>();

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MetroColorStyle Style
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MetroStyleManager StyleManager
		{
			get
			{
				return this.metroStyleManager;
			}
			set
			{
				this.metroStyleManager = value;
				this.UpdateTheme();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroThemeStyle.Default)]
		public MetroThemeStyle Theme
		{
			get
			{
				if (base.DesignMode || this.metroTheme != MetroThemeStyle.Default)
				{
					return this.metroTheme;
				}
				if (this.StyleManager != null && this.metroTheme == MetroThemeStyle.Default)
				{
					return this.StyleManager.Theme;
				}
				if (this.StyleManager == null && this.metroTheme == MetroThemeStyle.Default)
				{
					return MetroThemeStyle.Light;
				}
				return this.metroTheme;
			}
			set
			{
				this.metroTheme = value;
				this.UpdateTheme();
			}
		}

		public MetroStyleExtender()
		{
		}

		public MetroStyleExtender(IContainer parent) : this()
		{
			if (parent != null)
			{
				parent.Add(this);
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		[Description("Apply Metro Theme BackColor and ForeColor.")]
		public bool GetApplyMetroTheme(Control control)
		{
			if (control == null)
			{
				return false;
			}
			return this.extendedControls.Contains(control);
		}

		public void SetApplyMetroTheme(Control control, bool value)
		{
			if (control == null)
			{
				return;
			}
			if (this.extendedControls.Contains(control))
			{
				if (!value)
				{
					this.extendedControls.Remove(control);
					return;
				}
			}
			else if (value)
			{
				this.extendedControls.Add(control);
			}
		}

		bool System.ComponentModel.IExtenderProvider.CanExtend(object target)
		{
			if (!(target is Control))
			{
				return false;
			}
			if (target is IMetroControl)
			{
				return false;
			}
			return !(target is IMetroForm);
		}

		private void UpdateTheme()
		{
			Color color = MetroPaint.BackColor.Form(this.Theme);
			Color color1 = MetroPaint.ForeColor.Label.Normal(this.Theme);
			foreach (Control extendedControl in this.extendedControls)
			{
				if (extendedControl == null)
				{
					continue;
				}
				try
				{
					extendedControl.BackColor = color;
				}
				catch
				{
				}
				try
				{
					extendedControl.ForeColor = color1;
				}
				catch
				{
				}
			}
		}
	}
}