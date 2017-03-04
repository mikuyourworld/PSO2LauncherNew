using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;

namespace MetroFramework.Drawing.Html
{
	[CLSCompliant(false)]
	public class HtmlPanel : ScrollableControl
	{
		protected InitialContainer htmlContainer;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool AutoScroll
		{
			get
			{
				return base.AutoScroll;
			}
			set
			{
				base.AutoScroll = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
		}

		public InitialContainer HtmlContainer
		{
			get
			{
				return this.htmlContainer;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Localizable(true)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				this.CreateFragment();
				this.MeasureBounds();
				base.Invalidate();
			}
		}

		public HtmlPanel()
		{
			this.htmlContainer = new InitialContainer();
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.Opaque, true);
			this.DoubleBuffered = true;
			this.BackColor = SystemColors.Window;
			this.AutoScroll = true;
			HtmlRenderer.AddReference(Assembly.GetCallingAssembly());
		}

		protected virtual void CreateFragment()
		{
			this.htmlContainer = new InitialContainer(this.Text);
		}

		public virtual void MeasureBounds()
		{
			this.htmlContainer.SetBounds((this is HtmlLabel ? new Rectangle(0, 0, 10, 10) : base.ClientRectangle));
			using (Graphics graphic = base.CreateGraphics())
			{
				this.htmlContainer.MeasureBounds(graphic);
			}
			base.AutoScrollMinSize = System.Drawing.Size.Round(this.htmlContainer.MaximumSize);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			base.Focus();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			foreach (CssBox key in this.htmlContainer.LinkRegions.Keys)
			{
				if (!Rectangle.Round(this.htmlContainer.LinkRegions[key]).Contains(e.X, e.Y))
				{
					continue;
				}
				CssValue.GoLink(key.GetAttribute("href", string.Empty));
				return;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			foreach (CssBox key in this.htmlContainer.LinkRegions.Keys)
			{
				if (!Rectangle.Round(this.htmlContainer.LinkRegions[key]).Contains(e.X, e.Y))
				{
					continue;
				}
				this.Cursor = Cursors.Hand;
				return;
			}
			this.Cursor = Cursors.Default;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (!(this is HtmlLabel))
			{
				e.Graphics.Clear(SystemColors.Window);
			}
			this.htmlContainer.ScrollOffset = base.AutoScrollPosition;
			e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			this.htmlContainer.Paint(e.Graphics);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			this.MeasureBounds();
		}
	}
}