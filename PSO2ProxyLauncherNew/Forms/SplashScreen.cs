// Copyright ｩ 2002-2004 Rui Godinho Lopes <rui@ruilopes.com>
// All rights reserved.
//
// This source file(s) may be redistributed unmodified by any means
// PROVIDING they are not sold for profit without the authors expressed
// written consent, and providing that this notice and the authors name
// and all copyright notices remain intact.
//
// Any use of the software in source or binary forms, with or without
// modification, must include, in the user documentation ("About" box and
// printed documentation) and internal comments to the code, notices to
// the end user as follows:
//
// "Portions Copyright ｩ 2002-2004 Rui Godinho Lopes"
//
// An email letting me know that you are using it would be nice as well.
// That's not much to ask considering the amount of work that went into
// this.
//
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED. USE IT AT YOUT OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;


// class that exposes needed win32 gdi functions.
class Win32
{
    public enum Bool
    {
        False = 0,
        True
    };


    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public Int32 x;
        public Int32 y;

        public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public Int32 cx;
        public Int32 cy;

        public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ARGB
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }


    public const Int32 ULW_COLORKEY = 0x00000001;
    public const Int32 ULW_ALPHA = 0x00000002;
    public const Int32 ULW_OPAQUE = 0x00000004;

    public const byte AC_SRC_OVER = 0x00;
    public const byte AC_SRC_ALPHA = 0x01;


    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool DeleteObject(IntPtr hObject);
}



/// <para>Your PerPixel form should inherit this class</para>
/// <author><name>Rui Godinho Lopes</name><email>rui@ruilopes.com</email></author>
namespace PSO2ProxyLauncherNew.Forms
{
    class SplashScreen : Form
    {
        private System.Windows.Forms.Timer myTimer;
        private IntPtr screenDc, memDc, hBitmap, oldBitmap;
        private Win32.Size drawsize;
        private Win32.Point pointSource, topPos;
        private Win32.BLENDFUNCTION blend;

        public int DestinationOpacity { get; set; }
        private double _cacheimageopacity = 0;
        private double imageopacity
        {
            get { return _cacheimageopacity; }
            set
            {
                if (_cacheimageopacity != value)
                {
                    _cacheimageopacity = value;
                    int abc = Convert.ToInt32(_cacheimageopacity * 255);
                    if (abc > 255)
                        abc = 255;
                    else if (abc < 0)
                        abc = 0;
                    this.DrawBitmap((byte)abc);
                }
            }
        }

        private bool _isActivating;
        public bool IsActivating => this._isActivating;
        protected override void OnActivated(EventArgs e)
        {
            this._isActivating = true;
            base.OnActivated(e);
        }
        protected override void OnDeactivate(EventArgs e)
        {
            this._isActivating = false;
            base.OnDeactivate(e);
        }

        private Bitmap mySplashImage;
        private SynchronizationContext _synccontext;
        private SynchronizationContext syncContext
        {
            get
            {
                if (this._synccontext == null)
                    this._synccontext = SynchronizationContext.Current;
                return this._synccontext;
            }
        }
        private int opaSetTimes;
        private bool closingTime, codeclosing;

        public SplashScreen(Bitmap splashImage) : base()
        {
            if (splashImage == null)
                throw new ArgumentNullException();
            if (splashImage.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            this.UseFadeIn = true;
            this.UseFadeOut = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DoubleBuffered = true;

            this.mySplashImage = splashImage;
            // This form should not have a border or else Windows will clip it.
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            this.MaximizeBox = false;
            this.Name = "SplashScreen";
            this.DestinationOpacity = 0;
            this.opaSetTimes = 0;
            this.closingTime = false;
            this.codeclosing = false;

            // Force it to be at Center of the screen
            /*var sizeofImage = Properties.Resources.splashimage.Size;
            Rectangle myBound = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            if (myBound.Size.Width < sizeofImage.Width || myBound.Size.Height < sizeofImage.Height)
            {
                this.DesktopBounds = new Rectangle(0, 0,
                    myBound.Size.Width < sizeofImage.Width ? myBound.Size.Width : sizeofImage.Width,
                    myBound.Size.Height < sizeofImage.Height ? myBound.Size.Height : sizeofImage.Height);
            }
            else
                this.DesktopBounds = new Rectangle(
                    (myBound.Size.Width / 2) - (sizeofImage.Width / 2),
                    (myBound.Size.Height / 2) - (sizeofImage.Height / 2),
                    sizeofImage.Width,
                    sizeofImage.Height
                    );
            //*/

            this.ClientSize = this.mySplashImage.Size;

            this.screenDc = Win32.GetDC(IntPtr.Zero);
            this.memDc = Win32.CreateCompatibleDC(screenDc);
            this.hBitmap = IntPtr.Zero;
            this.oldBitmap = IntPtr.Zero;

            try
            {
                this.hBitmap = this.mySplashImage.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
                this.oldBitmap = Win32.SelectObject(this.memDc, this.hBitmap);

                this.drawsize = new Win32.Size(this.mySplashImage.Width, this.mySplashImage.Height);
                this.pointSource = new Win32.Point(0, 0);
                this.blend = new Win32.BLENDFUNCTION();
                this.blend.BlendOp = Win32.AC_SRC_OVER;
                this.blend.BlendFlags = 0;
                this.blend.AlphaFormat = Win32.AC_SRC_ALPHA;
            }
            catch (Exception ex)
            {
                Win32.ReleaseDC(IntPtr.Zero, this.screenDc);
                if (this.hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(this.memDc, this.oldBitmap);
                    // Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                    Win32.DeleteObject(this.hBitmap);
                }
                Win32.DeleteDC(this.memDc);
                throw ex;
            }

            this.myTimer = new System.Windows.Forms.Timer();
            this.myTimer.Enabled = false;
            this.myTimer.Tick += MyTimer_Tick;
            this.myTimer.Interval = 20;
        }

        public bool UseFadeIn { get; set; }
        public bool UseFadeOut { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            this.Text = "PSO2Launcher - Loading";
            this.Icon = Properties.Resources._1;

            this.CenterToScreen();
            this.topPos = new Win32.Point(this.DesktopLocation.X, this.DesktopLocation.Y);

            if (!this.UseFadeIn)
                this.DrawBitmap(255);
            base.OnLoad(e);
        }

        protected override bool ShowFocusCues => false;

        protected override void OnShown(EventArgs e)
        {
            this.FadeIn();
        }

        public void FadeIn()
        {
            if (this.UseFadeIn)
            {
                this.DestinationOpacity = 1;
                this.myTimer.Start();
            }
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            if (this.opaSetTimes > 10)
            {
                this.myTimer.Stop();
                this.imageopacity = this.DestinationOpacity;
                this.opaSetTimes = 0;
                if (this.imageopacity == 0)
                    if (this.closingTime)
                        this.Close();
                return;
            }
            else
            {
                this.opaSetTimes++;
            }
            if (this.imageopacity < this.DestinationOpacity)
                this.imageopacity += 0.1;
            else if (this.imageopacity > this.DestinationOpacity)
                this.imageopacity -= 0.1;
            else
                this.myTimer.Stop();
            if (this.imageopacity == 0)
                if (this.closingTime)
                    this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Win32.ReleaseDC(IntPtr.Zero, this.screenDc);
            if (this.hBitmap != IntPtr.Zero)
            {
                Win32.SelectObject(this.memDc, this.oldBitmap);
                // Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                Win32.DeleteObject(this.hBitmap);
            }
            Win32.DeleteDC(this.memDc);
            this.mySplashImage.Dispose();
            base.OnFormClosed(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!this.codeclosing)
            {
                e.Cancel = true;
                base.OnFormClosing(e);
            }
            else
            {
                if (this.UseFadeOut)
                {
                    if (this.imageopacity != 0)
                    {
                        e.Cancel = true;
                        this.FadeOut();
                    }
                    else
                        base.OnFormClosing(e);
                }
                else
                    base.OnFormClosing(e);
            }
        }

        public void FadeOut()
        {
            this.codeclosing = true;
            this.closingTime = true;
            if (this.UseFadeOut)
            {
                this.imageopacity = 1;
                this.DestinationOpacity = 0;
                this.myTimer.Start();
            }
            else
                this.Close();
        }

        private void DrawBitmap(byte opacity)
        {
            try
            {
                this.blend.SourceConstantAlpha = opacity;
                Win32.UpdateLayeredWindow(this.Handle, this.screenDc, ref this.topPos, ref this.drawsize, this.memDc, ref this.pointSource, 0, ref this.blend, Win32.ULW_ALPHA);
            }
            catch { }
        }

        /// <para>Changes the current bitmap.</para>
        public void SetBitmap(Bitmap bitmap)
        {
            this.SetBitmap(bitmap, 255);
        }


        /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
        public void SetBitmap(Bitmap bitmap, byte opacity)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                Win32.Size size = new Win32.Size(this.Width, this.Height);
                Win32.Point pointSource = new Win32.Point(0, 0);
                Win32.Point topPos = new Win32.Point(this.DesktopLocation.X, this.DesktopLocation.Y);
                Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;

                Win32.UpdateLayeredWindow(this.Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }
    }
}
