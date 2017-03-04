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
	[DefaultEvent("Scroll")]
	[DefaultProperty("Value")]
	[Designer("MetroFramework.Design.Controls.MetroScrollBarDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
	public class MetroScrollBar : Control, IMetroControl
	{
		private MetroColorStyle metroStyle;

		private MetroThemeStyle metroTheme;

		private MetroStyleManager metroStyleManager;

		private bool useCustomBackColor;

		private bool useCustomForeColor;

		private bool useStyleColors;

		private bool isFirstScrollEventVertical = true;

		private bool isFirstScrollEventHorizontal = true;

		private bool inUpdate;

		private Rectangle clickedBarRectangle;

		private Rectangle thumbRectangle;

		private bool topBarClicked;

		private bool bottomBarClicked;

		private bool thumbClicked;

		private int thumbWidth = 6;

		private int thumbHeight;

		private int thumbBottomLimitBottom;

		private int thumbBottomLimitTop;

		private int thumbTopLimit;

		private int thumbPosition;

		private int trackPosition;

		private readonly Timer progressTimer = new Timer();

		private int mouseWheelBarPartitions = 10;

		private bool isHovered;

		private bool isPressed;

		private bool useBarColor;

		private bool highlightOnWheel;

		private MetroScrollOrientation metroOrientation = MetroScrollOrientation.Vertical;

		private ScrollOrientation scrollOrientation = ScrollOrientation.VerticalScroll;

		private int minimum;

		private int maximum = 100;

		private int smallChange = 1;

		private int largeChange = 10;

		private int curValue;

		private bool dontUpdateColor;

		private Timer autoHoverTimer;

		[Category("Metro Appearance")]
		[DefaultValue(false)]
		public bool HighlightOnWheel
		{
			get
			{
				return this.highlightOnWheel;
			}
			set
			{
				this.highlightOnWheel = value;
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
				if (value == this.largeChange || value < this.smallChange || value < 2)
				{
					return;
				}
				if (value <= this.maximum - this.minimum)
				{
					this.largeChange = value;
				}
				else
				{
					this.largeChange = this.maximum - this.minimum;
				}
				this.SetupScrollBar();
			}
		}

		public int Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				if (value == this.maximum || value < 1 || value <= this.minimum)
				{
					return;
				}
				this.maximum = value;
				if (this.largeChange > this.maximum - this.minimum)
				{
					this.largeChange = this.maximum - this.minimum;
				}
				this.SetupScrollBar();
				if (this.curValue <= value)
				{
					this.ChangeThumbPosition(this.GetThumbPosition());
					this.Refresh();
					return;
				}
				this.dontUpdateColor = true;
				this.Value = this.maximum;
			}
		}

		public int Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				if (this.minimum == value || value < 0 || value >= this.maximum)
				{
					return;
				}
				this.minimum = value;
				if (this.curValue < value)
				{
					this.curValue = value;
				}
				if (this.largeChange > this.maximum - this.minimum)
				{
					this.largeChange = this.maximum - this.minimum;
				}
				this.SetupScrollBar();
				if (this.curValue < value)
				{
					this.dontUpdateColor = true;
					this.Value = value;
					return;
				}
				this.ChangeThumbPosition(this.GetThumbPosition());
				this.Refresh();
			}
		}

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
					throw new ArgumentOutOfRangeException("value", "MouseWheelBarPartitions has to be greather than zero");
				}
				this.mouseWheelBarPartitions = value;
			}
		}

		public MetroScrollOrientation Orientation
		{
			get
			{
				return this.metroOrientation;
			}
			set
			{
				if (value == this.metroOrientation)
				{
					return;
				}
				this.metroOrientation = value;
				if (value != MetroScrollOrientation.Vertical)
				{
					this.scrollOrientation = ScrollOrientation.HorizontalScroll;
				}
				else
				{
					this.scrollOrientation = ScrollOrientation.VerticalScroll;
				}
				base.Size = new System.Drawing.Size(base.Height, base.Width);
				this.SetupScrollBar();
			}
		}

		[Category("Metro Appearance")]
		public int ScrollbarSize
		{
			get
			{
				if (this.Orientation != MetroScrollOrientation.Vertical)
				{
					return base.Height;
				}
				return base.Width;
			}
			set
			{
				if (this.Orientation == MetroScrollOrientation.Vertical)
				{
					base.Width = value;
					return;
				}
				base.Height = value;
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
				if (value == this.smallChange || value < 1 || value >= this.largeChange)
				{
					return;
				}
				this.smallChange = value;
				this.SetupScrollBar();
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
		public bool UseBarColor
		{
			get
			{
				return this.useBarColor;
			}
			set
			{
				this.useBarColor = value;
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

		[Browsable(false)]
		[DefaultValue(0)]
		public int Value
		{
			get
			{
				return this.curValue;
			}
			set
			{
				if (this.curValue == value || value < this.minimum || value > this.maximum)
				{
					return;
				}
				this.curValue = value;
				this.ChangeThumbPosition(this.GetThumbPosition());
				this.OnScroll(ScrollEventType.ThumbPosition, -1, value, this.scrollOrientation);
				if (this.dontUpdateColor || !this.highlightOnWheel)
				{
					this.dontUpdateColor = false;
				}
				else
				{
					if (!this.isHovered)
					{
						this.isHovered = true;
					}
					if (this.autoHoverTimer != null)
					{
						this.autoHoverTimer.Stop();
						this.autoHoverTimer.Start();
					}
					else
					{
						this.autoHoverTimer = new Timer()
						{
							Interval = 1000
						};
						this.autoHoverTimer.Tick += new EventHandler(this.autoHoverTimer_Tick);
						this.autoHoverTimer.Start();
					}
				}
				this.Refresh();
			}
		}

		public MetroScrollBar()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
			base.Width = 10;
			base.Height = 200;
			this.SetupScrollBar();
			this.progressTimer.Interval = 20;
			this.progressTimer.Tick += new EventHandler(this.ProgressTimerTick);
		}

		public MetroScrollBar(MetroScrollOrientation orientation) : this()
		{
			this.Orientation = orientation;
		}

		public MetroScrollBar(MetroScrollOrientation orientation, int width) : this(orientation)
		{
			base.Width = width;
		}

		private void autoHoverTimer_Tick(object sender, EventArgs e)
		{
			this.isHovered = false;
			base.Invalidate();
			this.autoHoverTimer.Stop();
		}

		[SecuritySafeCritical]
		public void BeginUpdate()
		{
			WinApi.SendMessage(base.Handle, 11, false, 0);
			this.inUpdate = true;
		}

		private void ChangeThumbPosition(int position)
		{
			if (this.Orientation == MetroScrollOrientation.Vertical)
			{
				this.thumbRectangle.Y = position;
				return;
			}
			this.thumbRectangle.X = position;
		}

		private void DrawScrollBar(Graphics g, Color backColor, Color thumbColor, Color barColor)
		{
			if (this.useBarColor)
			{
				using (SolidBrush solidBrush = new SolidBrush(barColor))
				{
					g.FillRectangle(solidBrush, base.ClientRectangle);
				}
			}
			using (SolidBrush solidBrush1 = new SolidBrush(backColor))
			{
				Rectangle rectangle = new Rectangle(this.thumbRectangle.X - 1, this.thumbRectangle.Y - 1, this.thumbRectangle.Width + 2, this.thumbRectangle.Height + 2);
				g.FillRectangle(solidBrush1, rectangle);
			}
			using (SolidBrush solidBrush2 = new SolidBrush(thumbColor))
			{
				g.FillRectangle(solidBrush2, this.thumbRectangle);
			}
		}

		private void EnableTimer()
		{
			if (this.progressTimer.Enabled)
			{
				this.progressTimer.Interval = 10;
				return;
			}
			this.progressTimer.Interval = 600;
			this.progressTimer.Start();
		}

		[SecuritySafeCritical]
		public void EndUpdate()
		{
			WinApi.SendMessage(base.Handle, 11, true, 0);
			this.inUpdate = false;
			this.SetupScrollBar();
			this.Refresh();
		}

		private int GetThumbPosition()
		{
			int num;
			if (this.thumbHeight == 0 || this.thumbWidth == 0)
			{
				return 0;
			}
			int num1 = (this.metroOrientation == MetroScrollOrientation.Vertical ? this.thumbPosition / base.Height / this.thumbHeight : this.thumbPosition / base.Width / this.thumbWidth);
			num = (this.Orientation != MetroScrollOrientation.Vertical ? base.Width - num1 : base.Height - num1);
			int num2 = this.maximum - this.minimum;
			float single = 0f;
			if (num2 != 0)
			{
				single = ((float)this.curValue - (float)this.minimum) / (float)num2;
			}
			return Math.Max(this.thumbTopLimit, Math.Min(this.thumbBottomLimitTop, Convert.ToInt32(single * (float)num)));
		}

		private int GetThumbSize()
		{
			int num = (this.metroOrientation == MetroScrollOrientation.Vertical ? base.Height : base.Width);
			if (this.maximum == 0 || this.largeChange == 0)
			{
				return num;
			}
			float single = (float)this.largeChange * (float)num / (float)this.maximum;
			return Convert.ToInt32(Math.Min((float)num, Math.Max(single, 10f)));
		}

		private int GetValue(bool smallIncrement, bool up)
		{
			int num;
			if (!up)
			{
				num = this.curValue + (smallIncrement ? this.smallChange : this.largeChange);
				if (num > this.maximum)
				{
					num = this.maximum;
				}
			}
			else
			{
				num = this.curValue - (smallIncrement ? this.smallChange : this.largeChange);
				if (num < this.minimum)
				{
					num = this.minimum;
				}
			}
			return num;
		}

		public bool HitTest(Point point)
		{
			return this.thumbRectangle.Contains(point);
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
			base.Invalidate();
			base.OnEnter(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
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
		}

		protected override void OnLeave(EventArgs e)
		{
			this.isHovered = false;
			this.isPressed = false;
			base.Invalidate();
			base.OnLeave(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
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
			base.Focus();
			if (e.Button != System.Windows.Forms.MouseButtons.Left)
			{
				if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
					this.trackPosition = (this.metroOrientation == MetroScrollOrientation.Vertical ? e.Y : e.X);
				}
				return;
			}
			Point location = e.Location;
			if (this.thumbRectangle.Contains(location))
			{
				this.thumbClicked = true;
				this.thumbPosition = (this.metroOrientation == MetroScrollOrientation.Vertical ? location.Y - this.thumbRectangle.Y : location.X - this.thumbRectangle.X);
				base.Invalidate(this.thumbRectangle);
				return;
			}
			this.trackPosition = (this.metroOrientation == MetroScrollOrientation.Vertical ? location.Y : location.X);
			if (this.trackPosition >= (this.metroOrientation == MetroScrollOrientation.Vertical ? this.thumbRectangle.Y : this.thumbRectangle.X))
			{
				this.bottomBarClicked = true;
			}
			else
			{
				this.topBarClicked = true;
			}
			this.ProgressThumb(true);
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
			this.ResetScrollStatus();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			int width;
			int x;
			base.OnMouseMove(e);
			if (e.Button != System.Windows.Forms.MouseButtons.Left)
			{
				if (!base.ClientRectangle.Contains(e.Location))
				{
					this.ResetScrollStatus();
					return;
				}
				if (e.Button == System.Windows.Forms.MouseButtons.None)
				{
					if (this.thumbRectangle.Contains(e.Location))
					{
						base.Invalidate(this.thumbRectangle);
						return;
					}
					if (base.ClientRectangle.Contains(e.Location))
					{
						base.Invalidate();
					}
				}
			}
			else if (this.thumbClicked)
			{
				int num = this.curValue;
				int num1 = (this.metroOrientation == MetroScrollOrientation.Vertical ? e.Location.Y : e.Location.X);
				int num2 = (this.metroOrientation == MetroScrollOrientation.Vertical ? num1 / base.Height / this.thumbHeight : num1 / base.Width / this.thumbWidth);
				if (num1 <= this.thumbTopLimit + this.thumbPosition)
				{
					this.ChangeThumbPosition(this.thumbTopLimit);
					this.curValue = this.minimum;
					base.Invalidate();
				}
				else if (num1 < this.thumbBottomLimitTop + this.thumbPosition)
				{
					this.ChangeThumbPosition(num1 - this.thumbPosition);
					if (this.Orientation != MetroScrollOrientation.Vertical)
					{
						width = base.Width - num2;
						x = this.thumbRectangle.X;
					}
					else
					{
						width = base.Height - num2;
						x = this.thumbRectangle.Y;
					}
					float single = 0f;
					if (width != 0)
					{
						single = (float)x / (float)width;
					}
					this.curValue = Convert.ToInt32(single * (float)(this.maximum - this.minimum) + (float)this.minimum);
				}
				else
				{
					this.ChangeThumbPosition(this.thumbBottomLimitTop);
					this.curValue = this.maximum;
					base.Invalidate();
				}
				if (num != this.curValue)
				{
					this.OnScroll(ScrollEventType.ThumbTrack, num, this.curValue, this.scrollOrientation);
					this.Refresh();
					return;
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.isPressed = false;
			base.OnMouseUp(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (this.thumbClicked)
				{
					this.thumbClicked = false;
					this.OnScroll(ScrollEventType.EndScroll, -1, this.curValue, this.scrollOrientation);
				}
				else if (this.topBarClicked)
				{
					this.topBarClicked = false;
					this.StopTimer();
				}
				else if (this.bottomBarClicked)
				{
					this.bottomBarClicked = false;
					this.StopTimer();
				}
				base.Invalidate();
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			int delta = e.Delta / 120 * (this.maximum - this.minimum) / this.mouseWheelBarPartitions;
			if (this.Orientation == MetroScrollOrientation.Vertical)
			{
				MetroScrollBar value = this;
				value.Value = value.Value - delta;
				return;
			}
			MetroScrollBar metroScrollBar = this;
			metroScrollBar.Value = metroScrollBar.Value + delta;
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
					if (base.Parent == null)
					{
						backColor = MetroPaint.BackColor.Form(this.Theme);
					}
					else
					{
						backColor = (!(base.Parent is IMetroControl) ? base.Parent.BackColor : MetroPaint.BackColor.Form(this.Theme));
					}
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
			Color backColor;
			Color color;
			Color color1;
			if (this.useCustomBackColor)
			{
				backColor = this.BackColor;
			}
			else if (base.Parent == null)
			{
				backColor = MetroPaint.BackColor.Form(this.Theme);
			}
			else
			{
				backColor = (!(base.Parent is IMetroControl) ? base.Parent.BackColor : MetroPaint.BackColor.Form(this.Theme));
			}
			if (this.isHovered && !this.isPressed && base.Enabled)
			{
				color = MetroPaint.BackColor.ScrollBar.Thumb.Hover(this.Theme);
				color1 = MetroPaint.BackColor.ScrollBar.Bar.Hover(this.Theme);
			}
			else if (this.isHovered && this.isPressed && base.Enabled)
			{
				color = MetroPaint.BackColor.ScrollBar.Thumb.Press(this.Theme);
				color1 = MetroPaint.BackColor.ScrollBar.Bar.Press(this.Theme);
			}
			else if (base.Enabled)
			{
				color = MetroPaint.BackColor.ScrollBar.Thumb.Normal(this.Theme);
				color1 = MetroPaint.BackColor.ScrollBar.Bar.Normal(this.Theme);
			}
			else
			{
				color = MetroPaint.BackColor.ScrollBar.Thumb.Disabled(this.Theme);
				color1 = MetroPaint.BackColor.ScrollBar.Bar.Disabled(this.Theme);
			}
			this.DrawScrollBar(e.Graphics, backColor, color, color1);
			this.OnCustomPaintForeground(new MetroPaintEventArgs(backColor, color, e.Graphics));
		}

		private void OnScroll(ScrollEventType type, int oldValue, int newValue, ScrollOrientation orientation)
		{
			if (oldValue != newValue && this.ValueChanged != null)
			{
				this.ValueChanged(this, this.curValue);
			}
			if (this.Scroll == null)
			{
				return;
			}
			if (orientation == ScrollOrientation.HorizontalScroll)
			{
				if (type != ScrollEventType.EndScroll && this.isFirstScrollEventHorizontal)
				{
					type = ScrollEventType.First;
				}
				else if (!this.isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
				{
					this.isFirstScrollEventHorizontal = true;
				}
			}
			else if (type != ScrollEventType.EndScroll && this.isFirstScrollEventVertical)
			{
				type = ScrollEventType.First;
			}
			else if (!this.isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
			{
				this.isFirstScrollEventVertical = true;
			}
			this.Scroll(this, new ScrollEventArgs(type, oldValue, newValue, orientation));
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			this.SetupScrollBar();
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			Keys key = Keys.Up;
			Keys key1 = Keys.Down;
			if (this.Orientation == MetroScrollOrientation.Horizontal)
			{
				key = Keys.Left;
				key1 = Keys.Right;
			}
			if (keyData == key)
			{
				MetroScrollBar value = this;
				value.Value = value.Value - this.smallChange;
				return true;
			}
			if (keyData == key1)
			{
				MetroScrollBar metroScrollBar = this;
				metroScrollBar.Value = metroScrollBar.Value + this.smallChange;
				return true;
			}
			if (keyData == Keys.Prior)
			{
				this.Value = this.GetValue(false, true);
				return true;
			}
			if (keyData == Keys.Next)
			{
				if (this.curValue + this.largeChange <= this.maximum)
				{
					MetroScrollBar value1 = this;
					value1.Value = value1.Value + this.largeChange;
				}
				else
				{
					this.Value = this.maximum;
				}
				return true;
			}
			if (keyData == Keys.Home)
			{
				this.Value = this.minimum;
				return true;
			}
			if (keyData != Keys.End)
			{
				return base.ProcessDialogKey(keyData);
			}
			this.Value = this.maximum;
			return true;
		}

		private void ProgressThumb(bool enableTimer)
		{
			int width;
			int x;
			int num = this.curValue;
			ScrollEventType scrollEventType = ScrollEventType.First;
			if (this.Orientation != MetroScrollOrientation.Vertical)
			{
				x = this.thumbRectangle.X;
				width = this.thumbRectangle.Width;
			}
			else
			{
				x = this.thumbRectangle.Y;
				width = this.thumbRectangle.Height;
			}
			if (this.bottomBarClicked && x + width < this.trackPosition)
			{
				scrollEventType = ScrollEventType.LargeIncrement;
				this.curValue = this.GetValue(false, false);
				if (this.curValue != this.maximum)
				{
					this.ChangeThumbPosition(Math.Min(this.thumbBottomLimitTop, this.GetThumbPosition()));
				}
				else
				{
					this.ChangeThumbPosition(this.thumbBottomLimitTop);
					scrollEventType = ScrollEventType.Last;
				}
			}
			else if (this.topBarClicked && x > this.trackPosition)
			{
				scrollEventType = ScrollEventType.LargeDecrement;
				this.curValue = this.GetValue(false, true);
				if (this.curValue != this.minimum)
				{
					this.ChangeThumbPosition(Math.Max(this.thumbTopLimit, this.GetThumbPosition()));
				}
				else
				{
					this.ChangeThumbPosition(this.thumbTopLimit);
					scrollEventType = ScrollEventType.First;
				}
			}
			if (num != this.curValue)
			{
				this.OnScroll(scrollEventType, num, this.curValue, this.scrollOrientation);
				base.Invalidate();
				if (enableTimer)
				{
					this.EnableTimer();
				}
			}
		}

		private void ProgressTimerTick(object sender, EventArgs e)
		{
			this.ProgressThumb(true);
		}

		private void ResetScrollStatus()
		{
			bool flag = false;
			this.topBarClicked = false;
			this.bottomBarClicked = flag;
			this.StopTimer();
			this.Refresh();
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore(x, y, width, height, specified);
			if (base.DesignMode)
			{
				this.SetupScrollBar();
			}
		}

		private void SetupScrollBar()
		{
			if (this.inUpdate)
			{
				return;
			}
			if (this.Orientation != MetroScrollOrientation.Vertical)
			{
				this.thumbHeight = (base.Height > 0 ? base.Height : 10);
				this.thumbWidth = this.GetThumbSize();
				this.clickedBarRectangle = base.ClientRectangle;
				this.clickedBarRectangle.Inflate(-1, -1);
				int x = base.ClientRectangle.X;
				Rectangle clientRectangle = base.ClientRectangle;
				this.thumbRectangle = new Rectangle(x, clientRectangle.Y, this.thumbWidth, this.thumbHeight);
				this.thumbPosition = this.thumbRectangle.Width / 2;
				this.thumbBottomLimitBottom = base.ClientRectangle.Right;
				this.thumbBottomLimitTop = this.thumbBottomLimitBottom - this.thumbRectangle.Width;
				this.thumbTopLimit = base.ClientRectangle.X;
			}
			else
			{
				this.thumbWidth = (base.Width > 0 ? base.Width : 10);
				this.thumbHeight = this.GetThumbSize();
				this.clickedBarRectangle = base.ClientRectangle;
				this.clickedBarRectangle.Inflate(-1, -1);
				int num = base.ClientRectangle.X;
				Rectangle rectangle = base.ClientRectangle;
				this.thumbRectangle = new Rectangle(num, rectangle.Y, this.thumbWidth, this.thumbHeight);
				this.thumbPosition = this.thumbRectangle.Height / 2;
				this.thumbBottomLimitBottom = base.ClientRectangle.Bottom;
				this.thumbBottomLimitTop = this.thumbBottomLimitBottom - this.thumbRectangle.Height;
				this.thumbTopLimit = base.ClientRectangle.Y;
			}
			this.ChangeThumbPosition(this.GetThumbPosition());
			this.Refresh();
		}

		private void StopTimer()
		{
			this.progressTimer.Stop();
		}

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaint;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		[Category("Metro Appearance")]
		public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

		public event ScrollEventHandler Scroll;

		public event MetroScrollBar.ScrollValueChangedDelegate ValueChanged;

		public delegate void ScrollValueChangedDelegate(object sender, int newValue);
	}
}