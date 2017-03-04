using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[Designer("MetroFramework.Design.Controls.MetroLabelDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	[ToolboxBitmap(typeof(Label))]
	public class MetroLabel : Label, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private MetroLabel.DoubleBufferedTextBox baseTextBox;

		private MetroLabelSize metroLabelSize = MetroLabelSize.Medium;

		private MetroLabelWeight metroLabelWeight;

		private MetroLabelMode labelMode;

		private bool wrapToLine;

		private bool firstInitialization = true;

		[Category("Metro Appearance")]
		[DefaultValue(MetroLabelSize.Medium)]
		public MetroLabelSize FontSize
		{
			get
			{
				return this.metroLabelSize;
			}
			set
			{
				this.metroLabelSize = value;
				this.Refresh();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroLabelWeight.Light)]
		public MetroLabelWeight FontWeight
		{
			get
			{
				return this.metroLabelWeight;
			}
			set
			{
				this.metroLabelWeight = value;
				this.Refresh();
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroLabelMode.Default)]
		public MetroLabelMode LabelMode
		{
			get
			{
				return this.labelMode;
			}
			set
			{
				this.labelMode = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(MetroColorStyle.Default)]
		public MetroColorStyle Style
		{
			get
			{
				if (base.DesignMode || this.metroStyle != MetroColorStyle.Default)
				{
					return this.metroStyle;
				}
				if (this.StyleManager != null && this.metroStyle == MetroColorStyle.Default)
				{
					return this.StyleManager.Style;
				}
				if (this.StyleManager == null && this.metroStyle == MetroColorStyle.Default)
				{
					return MetroColorStyle.Blue;
				}
				return this.metroStyle;
			}
			set
			{
				this.metroStyle = value;
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
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool UseCustomBackColor
		{
			get
			{
				return this.useCustomBackColor;
			}
			set
			{
				this.useCustomBackColor = value;
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool UseCustomForeColor
		{
			get
			{
				return this.useCustomForeColor;
			}
			set
			{
				this.useCustomForeColor = value;
			}
		}

		[Browsable(false)]
		[Category("Metro Behaviour")]
		[DefaultValue(false)]
		public bool UseSelectable
		{
			get
			{
				return base.GetStyle(ControlStyles.Selectable);
			}
			set
			{
				base.SetStyle(ControlStyles.Selectable, value);
			}
		}

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool UseStyleColors
		{
			get
			{
				return this.useStyleColors;
			}
			set
			{
				this.useStyleColors = value;
			}
		}

		[Category("Metro Behaviour")]
		[DefaultValue(false)]
		public bool WrapToLine
		{
			get
			{
				return this.wrapToLine;
			}
			set
			{
				this.wrapToLine = value;
				this.Refresh();
			}
		}

		public MetroLabel()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
			this.baseTextBox = new MetroLabel.DoubleBufferedTextBox()
			{
				Visible = false
			};
			base.Controls.Add(this.baseTextBox);
		}

		[SecuritySafeCritical]
		private void BaseTextBoxOnClick(object sender, EventArgs eventArgs)
		{
			WinCaret.HideCaret(this.baseTextBox.Handle);
		}

		[SecuritySafeCritical]
		private void BaseTextBoxOnDoubleClick(object sender, EventArgs eventArgs)
		{
			this.baseTextBox.SelectAll();
			WinCaret.HideCaret(this.baseTextBox.Handle);
		}

		private void CreateBaseTextBox()
		{
			if (this.baseTextBox.Visible && !this.firstInitialization)
			{
				return;
			}
			if (!this.firstInitialization)
			{
				return;
			}
			this.firstInitialization = false;
			if (!base.DesignMode)
			{
				Form form = base.FindForm();
				if (form != null)
				{
					form.ResizeBegin += new EventHandler(this.parentForm_ResizeBegin);
					form.ResizeEnd += new EventHandler(this.parentForm_ResizeEnd);
				}
			}
			this.baseTextBox.BackColor = Color.Transparent;
			this.baseTextBox.Visible = true;
			this.baseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.baseTextBox.Font = MetroFonts.Label(this.metroLabelSize, this.metroLabelWeight);
			this.baseTextBox.Location = new Point(1, 0);
			this.baseTextBox.Text = this.Text;
			this.baseTextBox.ReadOnly = true;
			this.baseTextBox.Size = this.GetPreferredSize(System.Drawing.Size.Empty);
			this.baseTextBox.Multiline = true;
			this.baseTextBox.DoubleClick += new EventHandler(this.BaseTextBoxOnDoubleClick);
			this.baseTextBox.Click += new EventHandler(this.BaseTextBoxOnClick);
			base.Controls.Add(this.baseTextBox);
		}

		private void DestroyBaseTextbox()
		{
			if (!this.baseTextBox.Visible)
			{
				return;
			}
			this.baseTextBox.DoubleClick -= new EventHandler(this.BaseTextBoxOnDoubleClick);
			this.baseTextBox.Click -= new EventHandler(this.BaseTextBoxOnClick);
			this.baseTextBox.Visible = false;
		}

		public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
		{
			System.Drawing.Size size;
			base.GetPreferredSize(proposedSize);
			using (Graphics graphic = base.CreateGraphics())
			{
				proposedSize = new System.Drawing.Size(2147483647, 2147483647);
				size = TextRenderer.MeasureText(graphic, this.Text, MetroFonts.Label(this.metroLabelSize, this.metroLabelWeight), proposedSize, MetroPaint.GetTextFormatFlags(this.TextAlign));
			}
			return size;
		}

		private void HideBaseTextBox()
		{
			this.baseTextBox.Visible = false;
		}

		protected virtual void OnCustomPaint(MetroPaintEventArgs e)
		{
			if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaint != null)
			{
				this.CustomPaint(this, e);
			}
		}

		protected virtual void OnCustomPaintBackground(MetroPaintEventArgs e)
		{
			if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaintBackground != null)
			{
				this.CustomPaintBackground(this, e);
			}
		}

		protected virtual void OnCustomPaintForeground(MetroPaintEventArgs e)
		{
			if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaintForeground != null)
			{
				this.CustomPaintForeground(this, e);
			}
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			base.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				if (base.GetStyle(ControlStyles.AllPaintingInWmPaint))
				{
					this.OnPaintBackground(e);
				}
				this.OnCustomPaint(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
				this.OnPaintForeground(e);
			}
			catch
			{
				base.Invalidate();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			try
			{
				Color backColor = this.BackColor;
				if (!this.useCustomBackColor)
				{
					backColor = MetroPaint.BackColor.Form(this.Theme);
					if (base.Parent is MetroTile)
					{
						backColor = MetroPaint.GetStyleColor(this.Style);
					}
				}
				if (backColor.A != 255 || this.BackgroundImage != null)
				{
					base.OnPaintBackground(e);
					this.OnCustomPaintBackground(new MetroPaintEventArgs(backColor, Color.Empty, e.Graphics));
				}
				else
				{
					e.Graphics.Clear(backColor);
				}
			}
			catch
			{
				base.Invalidate();
			}
		}

		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			Color foreColor;
			if (this.useCustomForeColor)
			{
				foreColor = this.ForeColor;
			}
			else if (!base.Enabled)
			{
				if (base.Parent == null)
				{
					foreColor = MetroPaint.ForeColor.Label.Disabled(this.Theme);
				}
				else
				{
					foreColor = (!(base.Parent is MetroTile) ? MetroPaint.ForeColor.Label.Normal(this.Theme) : MetroPaint.ForeColor.Tile.Disabled(this.Theme));
				}
			}
			else if (base.Parent == null)
			{
				foreColor = (!this.useStyleColors ? MetroPaint.ForeColor.Label.Normal(this.Theme) : MetroPaint.GetStyleColor(this.Style));
			}
			else if (!(base.Parent is MetroTile))
			{
				foreColor = (!this.useStyleColors ? MetroPaint.ForeColor.Label.Normal(this.Theme) : MetroPaint.GetStyleColor(this.Style));
			}
			else
			{
				foreColor = MetroPaint.ForeColor.Tile.Normal(this.Theme);
			}
			if (this.LabelMode != MetroLabelMode.Selectable)
			{
				this.DestroyBaseTextbox();
				TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.Label(this.metroLabelSize, this.metroLabelWeight), base.ClientRectangle, foreColor, MetroPaint.GetTextFormatFlags(this.TextAlign, this.wrapToLine));
				this.OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, foreColor, e.Graphics));
			}
			else
			{
				this.CreateBaseTextBox();
				this.UpdateBaseTextBox();
				if (!this.baseTextBox.Visible)
				{
					TextRenderer.DrawText(e.Graphics, this.Text, MetroFonts.Label(this.metroLabelSize, this.metroLabelWeight), base.ClientRectangle, foreColor, MetroPaint.GetTextFormatFlags(this.TextAlign));
					return;
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			if (this.LabelMode == MetroLabelMode.Selectable)
			{
				this.HideBaseTextBox();
			}
			base.OnResize(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (this.LabelMode == MetroLabelMode.Selectable)
			{
				this.ShowBaseTextBox();
			}
		}

		private void parentForm_ResizeBegin(object sender, EventArgs e)
		{
			if (this.LabelMode == MetroLabelMode.Selectable)
			{
				this.HideBaseTextBox();
			}
		}

		private void parentForm_ResizeEnd(object sender, EventArgs e)
		{
			if (this.LabelMode == MetroLabelMode.Selectable)
			{
				this.ShowBaseTextBox();
			}
		}

		public override void Refresh()
		{
			if (this.LabelMode == MetroLabelMode.Selectable)
			{
				this.UpdateBaseTextBox();
			}
			base.Refresh();
		}

		private void ShowBaseTextBox()
		{
			this.baseTextBox.Visible = true;
		}

		private void UpdateBaseTextBox()
		{
			if (!this.baseTextBox.Visible)
			{
				return;
			}
			base.SuspendLayout();
			this.baseTextBox.SuspendLayout();
			if (!this.useCustomBackColor)
			{
				this.baseTextBox.BackColor = MetroPaint.BackColor.Form(this.Theme);
			}
			else
			{
				this.baseTextBox.BackColor = this.BackColor;
			}
			if (!base.Enabled)
			{
				if (base.Parent == null)
				{
					if (!this.useStyleColors)
					{
						this.baseTextBox.ForeColor = MetroPaint.ForeColor.Label.Disabled(this.Theme);
					}
					else
					{
						this.baseTextBox.ForeColor = MetroPaint.GetStyleColor(this.Style);
					}
				}
				else if (base.Parent is MetroTile)
				{
					this.baseTextBox.ForeColor = MetroPaint.ForeColor.Tile.Disabled(this.Theme);
				}
				else if (!this.useStyleColors)
				{
					this.baseTextBox.ForeColor = MetroPaint.ForeColor.Label.Disabled(this.Theme);
				}
				else
				{
					this.baseTextBox.ForeColor = MetroPaint.GetStyleColor(this.Style);
				}
			}
			else if (base.Parent == null)
			{
				if (!this.useStyleColors)
				{
					this.baseTextBox.ForeColor = MetroPaint.ForeColor.Label.Normal(this.Theme);
				}
				else
				{
					this.baseTextBox.ForeColor = MetroPaint.GetStyleColor(this.Style);
				}
			}
			else if (base.Parent is MetroTile)
			{
				this.baseTextBox.ForeColor = MetroPaint.ForeColor.Tile.Normal(this.Theme);
			}
			else if (!this.useStyleColors)
			{
				this.baseTextBox.ForeColor = MetroPaint.ForeColor.Label.Normal(this.Theme);
			}
			else
			{
				this.baseTextBox.ForeColor = MetroPaint.GetStyleColor(this.Style);
			}
			this.baseTextBox.Font = MetroFonts.Label(this.metroLabelSize, this.metroLabelWeight);
			this.baseTextBox.Text = this.Text;
			this.baseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			base.Size = this.GetPreferredSize(System.Drawing.Size.Empty);
			this.baseTextBox.ResumeLayout();
			base.ResumeLayout();
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

		private class DoubleBufferedTextBox : TextBox
		{
			public DoubleBufferedTextBox()
			{
				base.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
			}
		}
	}
}