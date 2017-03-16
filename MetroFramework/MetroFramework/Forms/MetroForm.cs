using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;

namespace MetroFramework.Forms
{
	public class MetroForm : Form, IMetroForm, IDisposable
	{
		private const int borderWidth = 5;

		private const int CS_DROPSHADOW = 131072;

		private const int WS_MINIMIZEBOX = 131072;

		private MetroColorStyle metroStyle = MetroColorStyle.Blue;

		private MetroThemeStyle metroTheme = MetroThemeStyle.Light;

		private MetroStyleManager metroStyleManager;

		private MetroFormTextAlign textAlign;

		private MetroFormBorderStyle formBorderStyle;

		private bool isMovable = true;

		private bool displayHeader = true;

		private bool isResizable = true;

		private MetroFormShadowType shadowType = MetroFormShadowType.Flat;

		private Forms.BackLocation backLocation;

		private Dictionary<MetroForm.WindowButtons, MetroForm.MetroFormButton> windowButtonList;

		private Form shadowForm;

        private Leayal.DirectBitmap backbuffer, backbgbuffer;

        [Browsable(false)]
        public override Color BackColor { get { return MetroPaint.BackColor.Form(this.Theme); } }

		[Category("Metro Appearance")]
        [DefaultValue(Forms.BackLocation.TopLeft)]
        public Forms.BackLocation BackLocation
        {
            get { return this.backLocation; }
            set
            {
                this.backLocation = value;
                this.Refresh();
            }
        }

		[Browsable(true)]
        [Category("Metro Appearance")]
        [DefaultValue(MetroFormBorderStyle.None)]
        public MetroFormBorderStyle BorderStyle
        {
            get { return this.formBorderStyle; }
            set { this.formBorderStyle = value; }
        }

		protected override System.Windows.Forms.CreateParams CreateParams
		{
			get
			{
				System.Windows.Forms.CreateParams createParams = base.CreateParams;
				System.Windows.Forms.CreateParams style = createParams;
				style.Style = style.Style | 131072;
				if (this.ShadowType == MetroFormShadowType.SystemShadow)
				{
					System.Windows.Forms.CreateParams classStyle = createParams;
					classStyle.ClassStyle = classStyle.ClassStyle | 131072;
				}
				return createParams;
			}
		}

		protected override System.Windows.Forms.Padding DefaultPadding { get { return new System.Windows.Forms.Padding(20, (this.DisplayHeader ? 60 : 20), 20, 20); } }

		[Category("Metro Appearance")]
        [DefaultValue(true)]
        public bool DisplayHeader
        {
            get { return this.displayHeader; }
            set
            {
                if (value != this.displayHeader)
                {
                    System.Windows.Forms.Padding padding = base.Padding;
                    padding.Top = padding.Top + (value ? 30 : -30);
                    base.Padding = padding;
                }
                this.displayHeader = value;
            }
        }

		[Browsable(false)]
        public new System.Windows.Forms.FormBorderStyle FormBorderStyle
        {
            get { return base.FormBorderStyle; }
            set { base.FormBorderStyle = value; }
        }

		public new Form MdiParent
        {
            get { return base.MdiParent; }
            set
            {
                if (value != null)
                {
                    this.RemoveShadow();
                    this.shadowType = MetroFormShadowType.None;
                }
                base.MdiParent = value;
            }
        }

		[Category("Metro Appearance")]
        public bool Movable
        {
            get { return this.isMovable; }
            set { this.isMovable = value; }
        }

		public new System.Windows.Forms.Padding Padding
        {
            get { return base.Padding; }
            set
            {
                value.Top = Math.Max(value.Top, (this.DisplayHeader ? 60 : 30));
                base.Padding = value;
            }
        }

		[Category("Metro Appearance")]
        public bool Resizable
        {
            get { return this.isResizable; }
            set { this.isResizable = value; }
        }

		[Category("Metro Appearance")]
        [DefaultValue(MetroFormShadowType.Flat)]
        public MetroFormShadowType ShadowType
        {
            get
            {
                if (base.IsMdiChild)
                    return MetroFormShadowType.None;
                return this.shadowType;
            }
            set { this.shadowType = value; }
        }

		[Category("Metro Appearance")]
        public MetroColorStyle Style
        {
            get
            {
                if (this.StyleManager == null)
                    return this.metroStyle;
                return this.StyleManager.Style;
            }
            set { this.metroStyle = value; }
        }

		[Browsable(false)]
        public MetroStyleManager StyleManager
        {
            get { return this.metroStyleManager; }
            set { this.metroStyleManager = value; }
        }

		[Browsable(true)]
		[Category("Metro Appearance")]
		public MetroFormTextAlign TextAlign
		{
			get { return this.textAlign; }
			set { this.textAlign = value; }
		}

		[Category("Metro Appearance")]
		public MetroThemeStyle Theme
		{
			get
			{
				if (this.StyleManager == null)
                    return this.metroTheme;
                return this.StyleManager.Theme;
			}
			set
            { this.metroTheme = value; this.Invalidate(this.ClientRectangle, false); }
		}

        public new FormWindowState WindowState
        {
            get { return base.WindowState; }
            set
            {
                if (base.WindowState != value)
                {
                    MetroFramework.Forms.WindowStateEventArgs ee = new MetroFramework.Forms.WindowStateEventArgs(base.WindowState);
                    this.OnWindowStateChanging(new MetroFramework.Forms.WindowStateEventArgs(value));
                    base.WindowState = value;
                    this.OnWindowStateChanged(ee);
                }
            }
        }

        public event EventHandler<MetroFramework.Forms.WindowStateEventArgs> WindowStateChanging;
        protected virtual void OnWindowStateChanging(MetroFramework.Forms.WindowStateEventArgs e)
        {
            this.WindowStateChanging?.Invoke(this, e);
        }

        public event EventHandler<MetroFramework.Forms.WindowStateEventArgs> WindowStateChanged;
        protected virtual void OnWindowStateChanged(MetroFramework.Forms.WindowStateEventArgs e)
        {
            this.WindowStateChanged?.Invoke(this, e);
        }

        public MetroForm() : base()
		{
            base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "MetroForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			base.TransparencyKey = Color.Lavender;
            this.DrawBackground = true;
            base.UpdateStyles();
		}

		private void AddWindowButton(MetroForm.WindowButtons button)
		{
			if (this.windowButtonList == null)
                this.windowButtonList = new Dictionary<MetroForm.WindowButtons, MetroForm.MetroFormButton>();
            if (this.windowButtonList.ContainsKey(button)) return;
            MetroForm.MetroFormButton metroFormButton = new MetroForm.MetroFormButton();
            switch (button)
            {
                case WindowButtons.Close:
                    metroFormButton.Text = "r";
                    break;
                case WindowButtons.Minimize:
                    metroFormButton.Text = "0";
                    break;
                case WindowButtons.Maximize:
                    if (this.WindowState != FormWindowState.Normal)
                        metroFormButton.Text = "2";
                    else
                        metroFormButton.Text = "1";
                    break;
            }
			metroFormButton.Style = this.Style;
			metroFormButton.Theme = this.Theme;
			metroFormButton.Tag = button;
			metroFormButton.Size = new System.Drawing.Size(25, 20);
			metroFormButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			metroFormButton.TabStop = false;
			metroFormButton.Click += new EventHandler(this.WindowButton_Click);
			base.Controls.Add(metroFormButton);
			this.windowButtonList.Add(button, metroFormButton);
		}

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (e.Control is MetroForm.MetroFormButton)
            { }
            else
                base.OnControlAdded(e);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if (e.Control is MetroForm.MetroFormButton)
            { }
            else
                base.OnControlRemoved(e);
        }

        public Bitmap ApplyInvert(Bitmap bitmapImage)
		{
			for (int i = 0; i < bitmapImage.Height; i++)
                for (int j = 0; j < bitmapImage.Width; j++)
                {
                    Color pixel = bitmapImage.GetPixel(j, i);
                    byte a = pixel.A;
                    byte r = (byte)(255 - pixel.R);
                    byte g = (byte)(255 - pixel.G);
                    byte b = (byte)(255 - pixel.B);
                    if (r <= 0)
                        r = 17;
                    if (g <= 0)
                        g = 17;
                    if (b <= 0)
                        b = 17;
                    bitmapImage.SetPixel(j, i, Color.FromArgb((int)r, (int)g, (int)b));
                }
            return bitmapImage;
		}

		private void CreateShadow()
		{
			switch (this.ShadowType)
			{
				case MetroFormShadowType.Flat:
					this.shadowForm = new MetroForm.MetroFlatDropShadow(this);
					return;
				case MetroFormShadowType.DropShadow:
					this.shadowForm = new MetroForm.MetroRealisticDropShadow(this);
					return;
				default:
                    return;
            }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.RemoveShadow();
                if (this.backbuffer != null)
                    this.backbuffer.Dispose();
                if (this.backbgbuffer != null)
                    this.backbgbuffer.Dispose();
            }
			base.Dispose(disposing);
		}

		[SecuritySafeCritical]
		public bool FocusMe()
		{
			return WinApi.SetForegroundWindow(base.Handle);
		}

		private TextFormatFlags GetTextFormatFlags()
		{
			switch (this.TextAlign)
			{
				case MetroFormTextAlign.Left:
                    return TextFormatFlags.Default;
                case MetroFormTextAlign.Center:
                    return TextFormatFlags.HorizontalCenter;
                case MetroFormTextAlign.Right:
                    return TextFormatFlags.Right;
            }
			throw new InvalidOperationException();
		}

		private WinApi.HitTest HitTestNCA(IntPtr hwnd, IntPtr wparam, IntPtr lparam)
		{
			Point point = new Point((short)((int)lparam), (short)((int)lparam >> 16));
			int num = Math.Max(this.Padding.Right, this.Padding.Bottom);
			if (this.Resizable)
			{
				Rectangle clientRectangle = base.ClientRectangle;
				Rectangle rectangle = base.ClientRectangle;
                if (base.RectangleToScreen(new Rectangle(clientRectangle.Width - num, rectangle.Height - num, num, num)).Contains(point))
                    return WinApi.HitTest.HTBOTTOMRIGHT;
			}
			if (base.RectangleToScreen(new Rectangle(5, 5, base.ClientRectangle.Width - 10, 50)).Contains(point))
                return WinApi.HitTest.HTCAPTION;
            return WinApi.HitTest.HTCLIENT;
		}

		[SecuritySafeCritical]
		private static bool IsAeroThemeEnabled()
		{
			bool flag;
			if (Environment.OSVersion.Version.Major <= 5)
                return false;
            DwmApi.DwmIsCompositionEnabled(out flag);
			return flag;
		}

		private static bool IsDropShadowSupported()
		{
			if (Environment.OSVersion.Version.Major > 5)
                return SystemInformation.IsDropShadowEnabled;
            return false;
		}

		private Rectangle MeasureText(Graphics g, Rectangle clientRectangle, System.Drawing.Font font, string text, TextFormatFlags flags)
		{
			System.Drawing.Size size = new System.Drawing.Size(2147483647, -2147483648);
			System.Drawing.Size size1 = TextRenderer.MeasureText(g, text, font, size, flags);
			return new Rectangle(clientRectangle.X, clientRectangle.Y, size1.Width, size1.Height);
		}

		[SecuritySafeCritical]
		private void MoveControl()
		{
			WinApi.ReleaseCapture();
			WinApi.SendMessage(base.Handle, 161, 2, 0);
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (this.shadowType == MetroFormShadowType.AeroShadow && MetroForm.IsAeroThemeEnabled() && MetroForm.IsDropShadowSupported())
			{
				int num = 2;
				DwmApi.DwmSetWindowAttribute(base.Handle, 2, ref num, 4);
				DwmApi.MARGINS mARGIN = new DwmApi.MARGINS()
				{
					cyBottomHeight = 1,
					cxLeftWidth = 0,
					cxRightWidth = 0,
					cyTopHeight = 0
				};
				DwmApi.MARGINS mARGIN1 = mARGIN;
				DwmApi.DwmExtendFrameIntoClientArea(base.Handle, ref mARGIN1);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			if (base.Owner != null)
                base.Owner = null;
            this.RemoveShadow();
			base.OnClosed(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!(this is MetroTaskWindow))
                MetroTaskWindow.ForceClose();
            base.OnClosing(e);
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			base.Invalidate();
		}

		[SecuritySafeCritical]
		private unsafe void OnGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
		{
			WinApi.MINMAXINFO* width = (WinApi.MINMAXINFO*)((void*)lParam);
			Screen screen = Screen.FromHandle(hwnd);
			Rectangle workingArea = screen.WorkingArea;
			(*width).ptMaxSize.x = workingArea.Width;
			Rectangle rectangle = screen.WorkingArea;
			(*width).ptMaxSize.y = rectangle.Height;
			int left = screen.WorkingArea.Left;
			Rectangle bounds = screen.Bounds;
			(*width).ptMaxPosition.x = Math.Abs(left - bounds.Left);
			int top = screen.WorkingArea.Top;
			Rectangle bounds1 = screen.Bounds;
			(*width).ptMaxPosition.y = Math.Abs(top - bounds1.Top);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (base.DesignMode) return;
            FormStartPosition startPosition = base.StartPosition;
			if (startPosition != FormStartPosition.CenterScreen)
			{
				if (startPosition == FormStartPosition.CenterParent)
                    base.CenterToParent();
            }
			else if (!base.IsMdiChild)
                base.CenterToScreen();
            else
                base.CenterToParent();
            this.RemoveCloseButton();
			if (base.ControlBox)
			{
				this.AddWindowButton(MetroForm.WindowButtons.Close);
				if (base.MaximizeBox)
                    this.AddWindowButton(MetroForm.WindowButtons.Maximize);
                if (base.MinimizeBox)
                    this.AddWindowButton(MetroForm.WindowButtons.Minimize);
                this.UpdateWindowButtonPosition();
			}
			this.CreateShadow();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (this.WindowState != FormWindowState.Maximized && e.Button == System.Windows.Forms.MouseButtons.Left && this.Movable)
			{
                Point location = e.Location;
				if (base.Width - 5 > location.X && e.Location.X > 5 && e.Location.Y > 5)
                    this.MoveControl();
            }
		}

        protected bool DrawBackground { get; set; }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!this.DrawBackground) return;
            if (this.backbgbuffer == null)
                this.backbgbuffer = new Leayal.DirectBitmap(this.Width, this.Height);
            this.backbgbuffer.Graphics.Clear(MetroPaint.BackColor.Form(this.Theme));
            base.OnPaintBackground(new PaintEventArgs(this.backbgbuffer.Graphics, e.ClipRectangle));
            //e.Graphics.DrawImage(this.backbgbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.backbuffer == null)
                this.backbuffer = new Leayal.DirectBitmap(this.Width, this.Height);
            //Image image;
            Color color1 = MetroPaint.ForeColor.Title(this.Theme);
            if (this.backbgbuffer != null && !this.backbgbuffer.Disposed)
                this.backbuffer.Graphics.DrawImage(this.backbgbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            else
                this.backbuffer.Graphics.Clear(Color.Transparent);

            this.OnPainfulPaint(new PaintEventArgs(this.backbuffer.Graphics, e.ClipRectangle));

            //base.OnPaint(new PaintEventArgs(this.backbuffer.Graphics, e.ClipRectangle));
            using (SolidBrush styleBrush = MetroPaint.GetStyleBrush(this.Style))
                this.backbuffer.Graphics.FillRectangle(styleBrush, new Rectangle(0, 0, base.Width, 5));
            if (this.BorderStyle != MetroFormBorderStyle.None)
                using (Pen pen = new Pen(MetroPaint.BorderColor.Form(this.Theme)))
                {
                    Point[] point = new Point[] { new Point(0, 5), new Point(0, base.Height - 1), new Point(base.Width - 1, base.Height - 1), new Point(base.Width - 1, 5) };
                    this.backbuffer.Graphics.DrawLines(pen, point);
                }
            /*if (this.backImage != null && this.backMaxSize != 0)
            {
                Image image1 = MetroImage.ResizeImage(this.backImage, new Rectangle(0, 0, this.backMaxSize, this.backMaxSize));
                if (this._imageinvert)
                {
                    if (this.Theme == MetroThemeStyle.Dark)
                        image = this._image;
                    else
                        image = this.backImage;
                    image1 = MetroImage.ResizeImage(image, new Rectangle(0, 0, this.backMaxSize, this.backMaxSize));
                }
                switch (this.backLocation)
                {
                    case Forms.BackLocation.TopLeft:
                        derped.Graphics.DrawImage(image1, this.backImagePadding.Left, this.backImagePadding.Top);
                        break;
                    case Forms.BackLocation.TopRight:
                        Rectangle clientRectangle = base.ClientRectangle;
                        derped.Graphics.DrawImage(image1, clientRectangle.Right - (this.backImagePadding.Right + image1.Width), this.backImagePadding.Top);
                        break;
                    case Forms.BackLocation.BottomLeft:
                        int left = this.backImagePadding.Left;
                        Rectangle clientRectangle1 = base.ClientRectangle;
                        derped.Graphics.DrawImage(image1, left, clientRectangle1.Bottom - (image1.Height + this.backImagePadding.Bottom));
                        break;
                    case Forms.BackLocation.BottomRight:
                        Rectangle rectangle1 = base.ClientRectangle;
                        int right = rectangle1.Right - (this.backImagePadding.Right + image1.Width);
                        Rectangle clientRectangle2 = base.ClientRectangle;
                        derped.Graphics.DrawImage(image1, right, clientRectangle2.Bottom - (image1.Height + this.backImagePadding.Bottom));
                        break;
                }
            }//*/
            if (this.displayHeader)
                TextRenderer.DrawText(this.backbuffer.Graphics, this.Text, MetroFonts.Title, new Rectangle(20, 20, base.ClientRectangle.Width - 40, 40), color1, TextFormatFlags.EndEllipsis | this.GetTextFormatFlags());
            if (this.Resizable && (base.SizeGripStyle == System.Windows.Forms.SizeGripStyle.Auto || base.SizeGripStyle == System.Windows.Forms.SizeGripStyle.Show))
                using (SolidBrush solidBrush = new SolidBrush(MetroPaint.ForeColor.Button.Disabled(this.Theme)))
                {
                    System.Drawing.Size size = new System.Drawing.Size(2, 2);
                    Rectangle[] rectangleArray = new Rectangle[6];
                    rectangleArray[0] = new Rectangle(new Point(base.ClientRectangle.Width - 6, base.ClientRectangle.Height - 6), size);
                    rectangleArray[1] = new Rectangle(new Point(base.ClientRectangle.Width - 10, base.ClientRectangle.Height - 10), size);
                    rectangleArray[2] = new Rectangle(new Point(base.ClientRectangle.Width - 10, base.ClientRectangle.Height - 6), size);
                    rectangleArray[3] = new Rectangle(new Point(base.ClientRectangle.Width - 6, base.ClientRectangle.Height - 10), size);
                    rectangleArray[4] = new Rectangle(new Point(base.ClientRectangle.Width - 14, base.ClientRectangle.Height - 6), size);
                    rectangleArray[5] = new Rectangle(new Point(base.ClientRectangle.Width - 6, base.ClientRectangle.Height - 14), size);
                    this.backbuffer.Graphics.FillRectangles(solidBrush, rectangleArray);
                }
            e.Graphics.DrawImage(this.backbuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        public event PaintEventHandler PainfulPaint;
        protected virtual void OnPainfulPaint(PaintEventArgs e)
        {
            this.PainfulPaint?.Invoke(this, e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (this.backbgbuffer != null)
                this.backbgbuffer.Dispose();
            this.backbgbuffer = new Leayal.DirectBitmap(this.Width, this.Height);
            if (this.backbuffer != null)
                this.backbuffer.Dispose();
            this.backbuffer = new Leayal.DirectBitmap(this.Width, this.Height);
        }

        protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);
            this.UpdateWindowButtonPosition();
		}

		[SecuritySafeCritical]
		public void RemoveCloseButton()
		{
			IntPtr systemMenu = WinApi.GetSystemMenu(base.Handle, false);
			if (systemMenu == IntPtr.Zero)
			{
				return;
			}
			int menuItemCount = WinApi.GetMenuItemCount(systemMenu);
			if (menuItemCount <= 0)
			{
				return;
			}
			WinApi.RemoveMenu(systemMenu, (uint)(menuItemCount - 1), 5120);
			WinApi.RemoveMenu(systemMenu, (uint)(menuItemCount - 2), 5120);
			WinApi.DrawMenuBar(base.Handle);
		}

		private void RemoveShadow()
		{
			if (this.shadowForm == null || this.shadowForm.IsDisposed)
			{
				return;
			}
			this.shadowForm.Visible = false;
			base.Owner = this.shadowForm.Owner;
			this.shadowForm.Owner = null;
			this.shadowForm.Dispose();
			this.shadowForm = null;
		}

		private void UpdateWindowButtonPosition()
		{
			if (!base.ControlBox)
			{
				return;
			}
			Dictionary<int, MetroForm.WindowButtons> nums = new Dictionary<int, MetroForm.WindowButtons>(3)
			{
				{ 0, MetroForm.WindowButtons.Close },
				{ 1, MetroForm.WindowButtons.Maximize },
				{ 2, MetroForm.WindowButtons.Minimize }
			};
			Dictionary<int, MetroForm.WindowButtons> nums1 = nums;
			Rectangle clientRectangle = base.ClientRectangle;
			Point point = new Point(clientRectangle.Width - 5 - 25, 5);
			int x = point.X - 25;
			MetroForm.MetroFormButton item = null;
			if (this.windowButtonList.Count != 1)
			{
				foreach (KeyValuePair<int, MetroForm.WindowButtons> keyValuePair in nums1)
				{
					bool flag = this.windowButtonList.ContainsKey(keyValuePair.Value);
					if (item != null || !flag)
					{
						if (item == null || !flag)
						{
							continue;
						}
						this.windowButtonList[keyValuePair.Value].Location = new Point(x, 5);
						x = x - 25;
					}
					else
					{
						item = this.windowButtonList[keyValuePair.Value];
						item.Location = point;
					}
				}
			}
			else
			{
				foreach (KeyValuePair<MetroForm.WindowButtons, MetroForm.MetroFormButton> keyValuePair1 in this.windowButtonList)
				{
					keyValuePair1.Value.Location = point;
				}
			}
			this.Refresh();
		}

		private void WindowButton_Click(object sender, EventArgs e)
		{
			MetroForm.MetroFormButton metroFormButton = sender as MetroForm.MetroFormButton;
			if (metroFormButton != null)
			{
				MetroForm.WindowButtons tag = (MetroForm.WindowButtons)metroFormButton.Tag;
                if (tag == MetroForm.WindowButtons.Close)
                    base.Close();
                if (tag == MetroForm.WindowButtons.Minimize)
                    this.WindowState = FormWindowState.Minimized;
                if (tag == MetroForm.WindowButtons.Maximize)
				{
                    if (this.WindowState == FormWindowState.Normal)
                    {
                        this.WindowState = FormWindowState.Maximized;
                        metroFormButton.Text = "2";
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Normal;
                        metroFormButton.Text = "1";
                    }
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
			MetroForm.MetroFormButton metroFormButton;
			if (base.DesignMode)
			{
				base.WndProc(ref m);
				return;
			}
			int msg = m.Msg;
			if (msg > 163)
			{
				if (msg == 274)
				{
					int num = m.WParam.ToInt32() & 65520;
					if (num != 61456)
					{
						if (num != 61488 && num != 61728)
						{
						}
					}
					else if (!this.Movable)
					{
						return;
					}
				}
				else
				{
					if (msg == 515)
					{
                        if (!base.MaximizeBox)
                            return;
                    }
					if (msg != 798)
					{
					}
				}
			}
			else if (msg == 132)
			{
				WinApi.HitTest hitTest = this.HitTestNCA(m.HWnd, m.WParam, m.LParam);
				if (hitTest != WinApi.HitTest.HTCLIENT)
				{
					m.Result = (IntPtr)((long)hitTest);
					return;
				}
			}
			else if (msg == 163)
			{
                if (!base.MaximizeBox)
                    return;
            }
			base.WndProc(ref m);
			int msg1 = m.Msg;
			if (msg1 != 5)
			{
				if (msg1 != 36)
				{
					return;
				}
				this.OnGetMinMaxInfo(m.HWnd, m.LParam);
				return;
			}
			if (this.windowButtonList != null)
			{
				this.windowButtonList.TryGetValue(MetroForm.WindowButtons.Maximize, out metroFormButton);
				if (metroFormButton == null)
				{
					return;
				}
				if (this.WindowState == FormWindowState.Normal)
				{
					if (this.shadowForm != null)
					{
						this.shadowForm.Visible = true;
					}
					metroFormButton.Text = "1";
				}
				if (this.WindowState == FormWindowState.Maximized)
				{
					metroFormButton.Text = "2";
				}
			}
			return;
		}

		protected class MetroAeroDropShadow : MetroForm.MetroShadowBase
		{
			public MetroAeroDropShadow(Form targetForm) : base(targetForm, 0, 134217760)
			{
				base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			}

			protected override void ClearShadow()
			{
			}

			protected override void PaintShadow()
			{
				base.Visible = true;
			}

			protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
			{
				if (specified == BoundsSpecified.Size)
				{
					return;
				}
				base.SetBoundsCore(x, y, width, height, specified);
			}
		}

		protected class MetroFlatDropShadow : MetroForm.MetroShadowBase
		{
			private Point Offset;

			public MetroFlatDropShadow(Form targetForm) : base(targetForm, 6, 134742048)
			{
			}

			protected override void ClearShadow()
			{
				Bitmap bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
				Graphics graphic = Graphics.FromImage(bitmap);
				graphic.Clear(Color.Transparent);
				graphic.Flush();
				graphic.Dispose();
				this.SetBitmap(bitmap, 255);
				bitmap.Dispose();
			}

			private Bitmap DrawBlurBorder()
			{
				Color black = Color.Black;
				int width = base.ClientRectangle.Width;
				Rectangle clientRectangle = base.ClientRectangle;
				return (Bitmap)this.DrawOutsetShadow(black, new Rectangle(0, 0, width, clientRectangle.Height));
			}

			private Image DrawOutsetShadow(Color color, Rectangle shadowCanvasArea)
			{
				Rectangle rectangle = shadowCanvasArea;
				Rectangle rectangle1 = new Rectangle(shadowCanvasArea.X + (-this.Offset.X - 1), shadowCanvasArea.Y + (-this.Offset.Y - 1), shadowCanvasArea.Width - (-this.Offset.X * 2 - 1), shadowCanvasArea.Height - (-this.Offset.Y * 2 - 1));
				Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);
				Graphics graphic = Graphics.FromImage(bitmap);
				graphic.SmoothingMode = SmoothingMode.AntiAlias;
				graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
				using (Brush solidBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
				{
					graphic.FillRectangle(solidBrush, rectangle);
				}
				using (Brush brush = new SolidBrush(Color.FromArgb(60, Color.Black)))
				{
					graphic.FillRectangle(brush, rectangle1);
				}
				graphic.Flush();
				graphic.Dispose();
				return bitmap;
			}

			protected override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);
				this.PaintShadow();
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				base.Visible = true;
				this.PaintShadow();

			}

			protected override void PaintShadow()
			{
				using (Bitmap bitmap = this.DrawBlurBorder())
				{
					this.SetBitmap(bitmap, 255);
				}
			}

			[SecuritySafeCritical]
			private void SetBitmap(Bitmap bitmap, byte opacity)
			{
				if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
				{
					throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");
				}
				IntPtr dC = WinApi.GetDC(IntPtr.Zero);
				IntPtr intPtr = WinApi.CreateCompatibleDC(dC);
				IntPtr zero = IntPtr.Zero;
				IntPtr zero1 = IntPtr.Zero;
				try
				{
					zero = bitmap.GetHbitmap(Color.FromArgb(0));
					zero1 = WinApi.SelectObject(intPtr, zero);
					WinApi.SIZE sIZE = new WinApi.SIZE(bitmap.Width, bitmap.Height);
					WinApi.POINT pOINT = new WinApi.POINT(0, 0);
					WinApi.POINT pOINT1 = new WinApi.POINT(base.Left, base.Top);
					WinApi.BLENDFUNCTION bLENDFUNCTION = new WinApi.BLENDFUNCTION()
					{
						BlendOp = 0,
						BlendFlags = 0,
						SourceConstantAlpha = opacity,
						AlphaFormat = 1
					};
					WinApi.UpdateLayeredWindow(base.Handle, dC, ref pOINT1, ref sIZE, intPtr, ref pOINT, 0, ref bLENDFUNCTION, 2);
				}
				finally
				{
					WinApi.ReleaseDC(IntPtr.Zero, dC);
					if (zero != IntPtr.Zero)
					{
						WinApi.SelectObject(intPtr, zero1);
						WinApi.DeleteObject(zero);
					}
					WinApi.DeleteDC(intPtr);
				}
			}
		}

		private class MetroFormButton : Button, IMetroControl
		{
			private MetroColorStyle metroStyle;

			private MetroThemeStyle metroTheme;

			private MetroStyleManager metroStyleManager;

			private bool useCustomBackColor;

			private bool useCustomForeColor;

			private bool useStyleColors;

			private bool isHovered;

			private bool isPressed;

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

			public MetroFormButton()
			{
				base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
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

			protected override void OnMouseDown(MouseEventArgs e)
			{
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					this.isPressed = true;
					base.Invalidate();
				}
				base.OnMouseDown(e);
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

			protected override void OnMouseUp(MouseEventArgs e)
			{
				this.isPressed = false;
				base.Invalidate();
				base.OnMouseUp(e);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				Color styleColor;
				Color color;
				MetroThemeStyle theme = this.Theme;
				if (base.Parent == null)
				{
					styleColor = MetroPaint.BackColor.Form(theme);
				}
				else if (!(base.Parent is IMetroForm))
				{
					styleColor = (!(base.Parent is IMetroControl) ? base.Parent.BackColor : MetroPaint.GetStyleColor(this.Style));
				}
				else
				{
					theme = ((IMetroForm)base.Parent).Theme;
					styleColor = MetroPaint.BackColor.Form(theme);
				}
				if (this.isHovered && !this.isPressed && base.Enabled)
				{
					color = MetroPaint.ForeColor.Button.Normal(theme);
					styleColor = MetroPaint.BackColor.Button.Normal(theme);
				}
				else if (this.isHovered && this.isPressed && base.Enabled)
				{
					color = MetroPaint.ForeColor.Button.Press(theme);
					styleColor = MetroPaint.GetStyleColor(this.Style);
				}
				else if (base.Enabled)
				{
					color = MetroPaint.ForeColor.Button.Normal(theme);
				}
				else
				{
					color = MetroPaint.ForeColor.Button.Disabled(theme);
					styleColor = MetroPaint.BackColor.Button.Disabled(theme);
				}
				e.Graphics.Clear(styleColor);
				System.Drawing.Font font = new System.Drawing.Font("Webdings", 9.25f);
				TextRenderer.DrawText(e.Graphics, this.Text, font, base.ClientRectangle, color, styleColor, TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
			}

			[Category("Metro Appearance")]
			public event EventHandler<MetroPaintEventArgs> CustomPaint;

			[Category("Metro Appearance")]
			public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

			[Category("Metro Appearance")]
			public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
		}

		protected class MetroRealisticDropShadow : MetroForm.MetroShadowBase
		{
			public MetroRealisticDropShadow(Form targetForm) : base(targetForm, 15, 134742048)
			{
			}

			protected override void ClearShadow()
			{
				Bitmap bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
				Graphics graphic = Graphics.FromImage(bitmap);
				graphic.Clear(Color.Transparent);
				graphic.Flush();
				graphic.Dispose();
				this.SetBitmap(bitmap, 255);
				bitmap.Dispose();
			}

			private Bitmap DrawBlurBorder()
			{
				Color black = Color.Black;
				int width = base.ClientRectangle.Width;
				Rectangle clientRectangle = base.ClientRectangle;
				return (Bitmap)this.DrawOutsetShadow(0, 0, 40, 1, black, new Rectangle(1, 1, width, clientRectangle.Height));
			}

			private Image DrawOutsetShadow(int hShadow, int vShadow, int blur, int spread, Color color, Rectangle shadowCanvasArea)
			{
				Rectangle rectangle = shadowCanvasArea;
				Rectangle rectangle1 = shadowCanvasArea;
				rectangle1.Offset(hShadow, vShadow);
				rectangle1.Inflate(-blur, -blur);
				rectangle.Inflate(spread, spread);
				rectangle.Offset(hShadow, vShadow);
				Rectangle rectangle2 = rectangle;
				Bitmap bitmap = new Bitmap(rectangle2.Width, rectangle2.Height, PixelFormat.Format32bppArgb);
				Graphics graphic = Graphics.FromImage(bitmap);
				graphic.SmoothingMode = SmoothingMode.AntiAlias;
				graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
				int num = 0;
				do
				{
					double height = (double)(rectangle.Height - rectangle1.Height) / (double)(blur * 2 + spread * 2);
					Color color1 = Color.FromArgb((int)(200 * (height * height)), color);
					Rectangle rectangle3 = rectangle1;
					rectangle3.Offset(-rectangle2.Left, -rectangle2.Top);
					this.DrawRoundedRectangle(graphic, rectangle3, num, Pens.Transparent, color1);
					rectangle1.Inflate(1, 1);
					num = (int)((double)blur * (1 - height * height));
				}
				while (rectangle.Contains(rectangle1));
				graphic.Flush();
				graphic.Dispose();
				return bitmap;
			}

			private void DrawRoundedRectangle(Graphics g, Rectangle bounds, int cornerRadius, Pen drawPen, Color fillColor)
			{
				int num = Convert.ToInt32(Math.Ceiling((double)drawPen.Width));
				bounds = Rectangle.Inflate(bounds, -num, -num);
				GraphicsPath graphicsPath = new GraphicsPath();
				if (cornerRadius <= 0)
				{
					graphicsPath.AddRectangle(bounds);
				}
				else
				{
					graphicsPath.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180f, 90f);
					graphicsPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270f, 90f);
					graphicsPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 0f, 90f);
					graphicsPath.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90f, 90f);
				}
				graphicsPath.CloseAllFigures();
				if (cornerRadius > 5)
				{
					using (SolidBrush solidBrush = new SolidBrush(fillColor))
					{
						g.FillPath(solidBrush, graphicsPath);
					}
				}
				if (drawPen != Pens.Transparent)
				{
					using (Pen pen = new Pen(drawPen.Color))
					{
						int num1 = 2;
						LineCap lineCap = (LineCap)num1;
						pen.StartCap = (LineCap)num1;
						pen.EndCap = lineCap;
						g.DrawPath(pen, graphicsPath);
					}
				}
			}

			protected override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);
				this.PaintShadow();
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				base.Visible = true;
				this.PaintShadow();
			}

			protected override void PaintShadow()
			{
				using (Bitmap bitmap = this.DrawBlurBorder())
				{
					this.SetBitmap(bitmap, 255);
				}
			}

			[SecuritySafeCritical]
			private void SetBitmap(Bitmap bitmap, byte opacity)
			{
				if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
				{
					throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");
				}
				IntPtr dC = WinApi.GetDC(IntPtr.Zero);
				IntPtr intPtr = WinApi.CreateCompatibleDC(dC);
				IntPtr zero = IntPtr.Zero;
				IntPtr zero1 = IntPtr.Zero;
				try
				{
					zero = bitmap.GetHbitmap(Color.FromArgb(0));
					zero1 = WinApi.SelectObject(intPtr, zero);
					WinApi.SIZE sIZE = new WinApi.SIZE(bitmap.Width, bitmap.Height);
					WinApi.POINT pOINT = new WinApi.POINT(0, 0);
					WinApi.POINT pOINT1 = new WinApi.POINT(base.Left, base.Top);
					WinApi.BLENDFUNCTION bLENDFUNCTION = new WinApi.BLENDFUNCTION()
					{
						BlendOp = 0,
						BlendFlags = 0,
						SourceConstantAlpha = opacity,
						AlphaFormat = 1
					};
					WinApi.BLENDFUNCTION bLENDFUNCTION1 = bLENDFUNCTION;
					WinApi.UpdateLayeredWindow(base.Handle, dC, ref pOINT1, ref sIZE, intPtr, ref pOINT, 0, ref bLENDFUNCTION1, 2);
				}
				finally
				{
					WinApi.ReleaseDC(IntPtr.Zero, dC);
					if (zero != IntPtr.Zero)
					{
						WinApi.SelectObject(intPtr, zero1);
						WinApi.DeleteObject(zero);
					}
					WinApi.DeleteDC(intPtr);
				}
			}
		}

		protected abstract class MetroShadowBase : Form
		{
			protected const int WS_EX_TRANSPARENT = 32;

			protected const int WS_EX_LAYERED = 524288;

			protected const int WS_EX_NOACTIVATE = 134217728;

			private const int TICKS_PER_MS = 10000;

			private const long RESIZE_REDRAW_INTERVAL = 10000000L;

			private readonly int shadowSize;

			private readonly int wsExStyle;

			private bool isBringingToFront;

			private long lastResizedOn;

			protected override System.Windows.Forms.CreateParams CreateParams
			{
				get
				{
                    var Params = base.CreateParams;
                    Params.ExStyle |= this.wsExStyle;
                    Params.ExStyle |= 0x80;
                    return Params;
				}
			}

            protected override bool ShowWithoutActivation { get { return true; } }

            private bool IsResizing
			{
				get
				{
					return this.lastResizedOn > (long)0;
				}
			}

			protected Form TargetForm
			{
				get;
				private set;
			}

			protected MetroShadowBase(Form targetForm, int shadowSize, int wsExStyle)
			{
				this.TargetForm = targetForm;
				this.shadowSize = shadowSize;
				this.wsExStyle = wsExStyle;
				this.TargetForm.Activated += new EventHandler(this.OnTargetFormActivated);
				this.TargetForm.ResizeBegin += new EventHandler(this.OnTargetFormResizeBegin);
				this.TargetForm.ResizeEnd += new EventHandler(this.OnTargetFormResizeEnd);
				this.TargetForm.VisibleChanged += new EventHandler(this.OnTargetFormVisibleChanged);
				this.TargetForm.SizeChanged += new EventHandler(this.OnTargetFormSizeChanged);
				this.TargetForm.Move += new EventHandler(this.OnTargetFormMove);
				this.TargetForm.Resize += new EventHandler(this.OnTargetFormResize);
				if (this.TargetForm.Owner != null)
				{
					base.Owner = this.TargetForm.Owner;
				}
				this.TargetForm.Owner = this;
				base.MaximizeBox = false;
				base.MinimizeBox = false;
				base.ShowInTaskbar = false;
				base.ShowIcon = false;
				base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				base.Bounds = this.GetShadowBounds();
			}

			protected abstract void ClearShadow();

			private Rectangle GetShadowBounds()
			{
				Rectangle bounds = this.TargetForm.Bounds;
				bounds.Inflate(this.shadowSize, this.shadowSize);
				return bounds;
			}

			protected override void OnDeactivate(EventArgs e)
			{
				base.OnDeactivate(e);
				this.isBringingToFront = true;
			}

			private void OnTargetFormActivated(object sender, EventArgs e)
			{
				if (base.Visible)
				{
					base.Update();
				}
				if (!this.isBringingToFront)
				{
					base.BringToFront();
					return;
				}
				base.Visible = true;
				this.isBringingToFront = false;
			}

			private void OnTargetFormMove(object sender, EventArgs e)
			{
				if (!this.TargetForm.Visible || this.TargetForm.WindowState != FormWindowState.Normal)
				{
					base.Visible = false;
					return;
				}
				base.Bounds = this.GetShadowBounds();
			}

			private void OnTargetFormResize(object sender, EventArgs e)
			{
				this.ClearShadow();
			}

			private void OnTargetFormResizeBegin(object sender, EventArgs e)
			{
				this.lastResizedOn = DateTime.Now.Ticks;
			}

			private void OnTargetFormResizeEnd(object sender, EventArgs e)
			{
				this.lastResizedOn = (long)0;
				this.PaintShadowIfVisible();
			}

			private void OnTargetFormSizeChanged(object sender, EventArgs e)
			{
				base.Bounds = this.GetShadowBounds();
				if (this.IsResizing)
				{
					return;
				}
				this.PaintShadowIfVisible();
			}

			private void OnTargetFormVisibleChanged(object sender, EventArgs e)
			{
				base.Visible = (!this.TargetForm.Visible ? false : this.TargetForm.WindowState != FormWindowState.Minimized);
				base.Update();
			}

			protected abstract void PaintShadow();

			private void PaintShadowIfVisible()
			{
				if (this.TargetForm.Visible && this.TargetForm.WindowState != FormWindowState.Minimized)
				{
					this.PaintShadow();
				}
			}
		}

		private enum WindowButtons
		{
			Minimize,
			Maximize,
			Close
		}
	}
}