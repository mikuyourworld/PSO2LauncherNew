using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
	[DefaultEvent("Scroll")]
	[ToolboxBitmap(typeof(TrackBar))]
	public class MetroTrackBar : Control, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private bool displayFocusRectangle;

		private int trackerValue = 50;

		private int barMinimum;

		private int barMaximum = 100;

		private int smallChange = 1;

		private int largeChange = 5;

		private int mouseWheelBarPartitions = 10;

		private bool isHovered;

		private bool isPressed;

		private bool isFocused;

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool DisplayFocus
		{
			get
			{
				return this.displayFocusRectangle;
			}
			set
			{
				this.displayFocusRectangle = value;
			}
		}

		[DefaultValue(5)]
		public int LargeChange
		{
			get
			{
				return this.largeChange;
			}
			set
			{
				this.largeChange = value;
			}
		}

		[DefaultValue(100)]
		public int Maximum
		{
			get
			{
				return this.barMaximum;
			}
			set
			{
				if (value <= this.barMinimum)
				{
					throw new ArgumentOutOfRangeException("Maximal value is lower than minimal one");
				}
				this.barMaximum = value;
				if (this.trackerValue > this.barMaximum)
				{
					this.trackerValue = this.barMaximum;
					if (this.ValueChanged != null)
					{
						this.ValueChanged(this, new EventArgs());
					}
				}
				base.Invalidate();
			}
		}

		[DefaultValue(0)]
		public int Minimum
		{
			get
			{
				return this.barMinimum;
			}
			set
			{
				if (value >= this.barMaximum)
				{
					throw new ArgumentOutOfRangeException("Minimal value is greather than maximal one");
				}
				this.barMinimum = value;
				if (this.trackerValue < this.barMinimum)
				{
					this.trackerValue = this.barMinimum;
					if (this.ValueChanged != null)
					{
						this.ValueChanged(this, new EventArgs());
					}
				}
				base.Invalidate();
			}
		}

		[DefaultValue(10)]
		public int MouseWheelBarPartitions
		{
			get
			{
				return this.mouseWheelBarPartitions;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("MouseWheelBarPartitions has to be greather than zero");
				}
				this.mouseWheelBarPartitions = value;
			}
		}

		[DefaultValue(1)]
		public int SmallChange
		{
			get
			{
				return this.smallChange;
			}
			set
			{
				this.smallChange = value;
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

		[Browsable(false)]
		[Category("Metro Appearance")]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
		[DefaultValue(true)]
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

		[Browsable(false)]
		[Category("Metro Appearance")]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[DefaultValue(50)]
		public int Value
		{
			get
			{
				return this.trackerValue;
			}
			set
			{
				if (!(value >= this.barMinimum & value <= this.barMaximum))
				{
					throw new ArgumentOutOfRangeException("Value is outside appropriate range (min, max)");
				}
				this.trackerValue = value;
				this.OnValueChanged();
				base.Invalidate();
			}
		}

		public MetroTrackBar(int min, int max, int value)
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.UserMouse | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			this.BackColor = Color.Transparent;
			this.Minimum = min;
			this.Maximum = max;
			this.Value = value;
		}

		public MetroTrackBar() : this(0, 100, 50)
		{
		}

		private void DrawTrackBar(Graphics g, Color thumbColor, Color barColor)
		{
			int width = (this.trackerValue - this.barMinimum) * (base.Width - 6) / (this.barMaximum - this.barMinimum);
			using (SolidBrush solidBrush = new SolidBrush(thumbColor))
			{
				Rectangle rectangle = new Rectangle(0, base.Height / 2 - 2, width, 4);
				g.FillRectangle(solidBrush, rectangle);
				Rectangle rectangle1 = new Rectangle(width, base.Height / 2 - 8, 6, 16);
				g.FillRectangle(solidBrush, rectangle1);
			}
			using (SolidBrush solidBrush1 = new SolidBrush(barColor))
			{
				Rectangle rectangle2 = new Rectangle(width + 7, base.Height / 2 - 2, base.Width - width + 7, 4);
				g.FillRectangle(solidBrush1, rectangle2);
			}
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

		protected override void OnEnter(EventArgs e)
		{
			this.isFocused = true;
			base.Invalidate();
			base.OnEnter(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			this.isFocused = true;
			base.Invalidate();
			base.OnGotFocus(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			this.isHovered = true;
			this.isPressed = true;
			base.Invalidate();
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			this.isHovered = false;
			this.isPressed = false;
			base.Invalidate();
			base.OnKeyUp(e);
			switch (e.KeyCode)
			{
				case Keys.Prior:
				{
					this.SetProperValue(this.Value + this.largeChange);
					this.OnScroll(ScrollEventType.LargeIncrement, this.Value);
					break;
				}
				case Keys.Next:
				{
					this.SetProperValue(this.Value - this.largeChange);
					this.OnScroll(ScrollEventType.LargeDecrement, this.Value);
					break;
				}
				case Keys.End:
				{
					this.Value = this.barMaximum;
					break;
				}
				case Keys.Home:
				{
					this.Value = this.barMinimum;
					break;
				}
				case Keys.Left:
				case Keys.Down:
				{
					this.SetProperValue(this.Value - this.smallChange);
					this.OnScroll(ScrollEventType.SmallDecrement, this.Value);
					break;
				}
				case Keys.Up:
				case Keys.Right:
				{
					this.SetProperValue(this.Value + this.smallChange);
					this.OnScroll(ScrollEventType.SmallIncrement, this.Value);
					break;
				}
			}
			if (this.Value == this.barMinimum)
			{
				this.OnScroll(ScrollEventType.First, this.Value);
			}
			if (this.Value == this.barMaximum)
			{
				this.OnScroll(ScrollEventType.Last, this.Value);
			}
			Point client = base.PointToClient(System.Windows.Forms.Cursor.Position);
			this.OnMouseMove(new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, client.X, client.Y, 0));
		}

		protected override void OnLeave(EventArgs e)
		{
			this.isFocused = false;
			this.isHovered = false;
			this.isPressed = false;
			base.Invalidate();
			base.OnLeave(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			this.isFocused = false;
			this.isHovered = false;
			this.isPressed = false;
			base.Invalidate();
			base.OnLostFocus(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this.isPressed = true;
				base.Invalidate();
			}
			base.OnMouseDown(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				base.Capture = true;
				this.OnScroll(ScrollEventType.ThumbTrack, this.trackerValue);
				this.OnValueChanged();
				this.OnMouseMove(e);
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			this.isHovered = true;
			base.Invalidate();
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this.isHovered = false;
			base.Invalidate();
			base.OnMouseLeave(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (base.Capture & e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				ScrollEventType scrollEventType = ScrollEventType.ThumbPosition;
				int x = e.Location.X;
				float single = (float)(this.barMaximum - this.barMinimum);
				System.Drawing.Size clientSize = base.ClientSize;
				float width = single / (float)(clientSize.Width - 3);
				this.trackerValue = (int)((float)x * width + (float)this.barMinimum);
				if (this.trackerValue <= this.barMinimum)
				{
					this.trackerValue = this.barMinimum;
					scrollEventType = ScrollEventType.First;
				}
				else if (this.trackerValue >= this.barMaximum)
				{
					this.trackerValue = this.barMaximum;
					scrollEventType = ScrollEventType.Last;
				}
				this.OnScroll(scrollEventType, this.trackerValue);
				this.OnValueChanged();
				base.Invalidate();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.isPressed = false;
			base.Invalidate();
			base.OnMouseUp(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			int delta = e.Delta / 120 * (this.barMaximum - this.barMinimum) / this.mouseWheelBarPartitions;
			this.SetProperValue(this.Value + delta);
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
				}
				if (backColor.A != 255)
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
			Color color;
			Color color1;
			if (this.isHovered && !this.isPressed && base.Enabled)
			{
				color = MetroPaint.BackColor.TrackBar.Thumb.Hover(this.Theme);
				color1 = MetroPaint.BackColor.TrackBar.Bar.Hover(this.Theme);
			}
			else if (this.isHovered && this.isPressed && base.Enabled)
			{
				color = MetroPaint.BackColor.TrackBar.Thumb.Press(this.Theme);
				color1 = MetroPaint.BackColor.TrackBar.Bar.Press(this.Theme);
			}
			else if (base.Enabled)
			{
				color = MetroPaint.BackColor.TrackBar.Thumb.Normal(this.Theme);
				color1 = MetroPaint.BackColor.TrackBar.Bar.Normal(this.Theme);
			}
			else
			{
				color = MetroPaint.BackColor.TrackBar.Thumb.Disabled(this.Theme);
				color1 = MetroPaint.BackColor.TrackBar.Bar.Disabled(this.Theme);
			}
			this.DrawTrackBar(e.Graphics, color, color1);
			if (this.displayFocusRectangle && this.isFocused)
			{
				ControlPaint.DrawFocusRectangle(e.Graphics, base.ClientRectangle);
			}
		}

		private void OnScroll(ScrollEventType scrollType, int newValue)
		{
			if (this.Scroll != null)
			{
				this.Scroll(this, new ScrollEventArgs(scrollType, newValue));
			}
		}

		private void OnValueChanged()
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, EventArgs.Empty);
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Tab | Control.ModifierKeys == Keys.Shift)
			{
				return base.ProcessDialogKey(keyData);
			}
			this.OnKeyDown(new KeyEventArgs(keyData));
			return true;
		}

		private void SetProperValue(int val)
		{
			if (val < this.barMinimum)
			{
				this.Value = this.barMinimum;
				return;
			}
			if (val <= this.barMaximum)
			{
				this.Value = val;
				return;
			}
			this.Value = this.barMaximum;
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

		public event ScrollEventHandler Scroll;

		public event EventHandler ValueChanged;
	}
}