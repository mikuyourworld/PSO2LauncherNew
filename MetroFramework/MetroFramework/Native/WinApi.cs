using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace MetroFramework.Native
{
	[SuppressUnmanagedCodeSecurity]
	public static class WinApi
	{
		public const int Autohide = 1;

		public const int AlwaysOnTop = 2;

		public const int MfByposition = 1024;

		public const int MfRemove = 4096;

		public const int TCM_HITTEST = 4883;

		public const int ULW_COLORKEY = 1;

		public const int ULW_ALPHA = 2;

		public const int ULW_OPAQUE = 4;

		public const byte AC_SRC_OVER = 0;

		public const byte AC_SRC_ALPHA = 1;

		public const int GW_HWNDFIRST = 0;

		public const int GW_HWNDLAST = 1;

		public const int GW_HWNDNEXT = 2;

		public const int GW_HWNDPREV = 3;

		public const int GW_OWNER = 4;

		public const int GW_CHILD = 5;

		public const int HC_ACTION = 0;

		public const int WH_CALLWNDPROC = 4;

		public const int GWL_WNDPROC = -4;

		[DllImport("gdi32.dll", CharSet=CharSet.None, ExactSpelling=true, SetLastError=true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport("gdi32.dll", CharSet=CharSet.None, ExactSpelling=true, SetLastError=true)]
		public static extern WinApi.Bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll", CharSet=CharSet.None, ExactSpelling=true, SetLastError=true)]
		public static extern WinApi.Bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool DrawMenuBar(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("User32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern int GetClassName(IntPtr hwnd, char[] className, int maxCount);

		[DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern int GetClientRect(IntPtr hwnd, ref WinApi.RECT lpRect);

		[DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern int GetClientRect(IntPtr hwnd, [In][Out] ref Rectangle rect);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=true, SetLastError=true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr hrgnclip, uint fdwOptions);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int GetMenuItemCount(IntPtr hMenu);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("User32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern IntPtr GetWindow(IntPtr hwnd, int uCmd);

		[DllImport("User32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern IntPtr GetWindowDC(IntPtr handle);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		internal static extern bool GetWindowRect(IntPtr hWnd, [In][Out] ref Rectangle rect);

		public static int HiWord(int dwValue)
		{
			return dwValue >> 16 & 65535;
		}

		[DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern bool InvalidateRect(IntPtr hwnd, ref Rectangle rect, bool bErase);

		[DllImport("User32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern bool IsWindowVisible(IntPtr hwnd);

		public static int LoWord(int dwValue)
		{
			return dwValue & 65535;
		}

		[DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool ReleaseCapture();

		[DllImport("User32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hDC);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

		[DllImport("gdi32.dll", CharSet=CharSet.None, ExactSpelling=true)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int SendMessage(IntPtr wnd, int msg, bool param, int lparam);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr SetCapture(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int W, int H, uint uFlags);

		[DllImport("shell32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr SHAppBarMessage(WinApi.ABM dwMessage, [In] ref WinApi.APPBARDATA pData);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool ShowScrollBar(IntPtr hWnd, int bar, int cmd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=true, SetLastError=true)]
		public static extern WinApi.Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref WinApi.POINT pptDst, ref WinApi.SIZE psize, IntPtr hdcSrc, ref WinApi.POINT pprSrc, int crKey, ref WinApi.BLENDFUNCTION pblend, int dwFlags);

		[DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern bool UpdateWindow(IntPtr hwnd);

		[DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern bool ValidateRect(IntPtr hwnd, ref Rectangle rect);

		public enum ABE : uint
		{
			Left,
			Top,
			Right,
			Bottom
		}

		public enum ABM : uint
		{
			New,
			Remove,
			QueryPos,
			SetPos,
			GetState,
			GetTaskbarPos,
			Activate,
			GetAutoHideBar,
			SetAutoHideBar,
			WindowPosChanged,
			SetState
		}

		public struct APPBARDATA
		{
			public uint cbSize;

			public IntPtr hWnd;

			public uint uCallbackMessage;

			public WinApi.ABE uEdge;

			public WinApi.RECT rc;

			public int lParam;
		}

		public struct ARGB
		{
			public byte Blue;

			public byte Green;

			public byte Red;

			public byte Alpha;
		}

		public struct BLENDFUNCTION
		{
			public byte BlendOp;

			public byte BlendFlags;

			public byte SourceConstantAlpha;

			public byte AlphaFormat;
		}

		public enum Bool
		{
			False,
			True
		}

		public enum HitTest
		{
			HTTRANSPARENT = -1,
			HTNOWHERE = 0,
			HTCLIENT = 1,
			HTCAPTION = 2,
			HTGROWBOX = 4,
			HTSIZE = 4,
			HTMINBUTTON = 8,
			HTREDUCE = 8,
			HTMAXBUTTON = 9,
			HTZOOM = 9,
			HTLEFT = 10,
			HTSIZEFIRST = 10,
			HTRIGHT = 11,
			HTTOP = 12,
			HTTOPLEFT = 13,
			HTTOPRIGHT = 14,
			HTBOTTOM = 15,
			HTBOTTOMLEFT = 16,
			HTBOTTOMRIGHT = 17,
			HTSIZELAST = 17
		}

		public enum Messages : uint
		{
			WM_NULL = 0,
			WM_CREATE = 1,
			WM_DESTROY = 2,
			WM_MOVE = 3,
			WM_SIZE = 5,
			WM_ACTIVATE = 6,
			WM_SETFOCUS = 7,
			WM_KILLFOCUS = 8,
			WM_ENABLE = 10,
			WM_SETREDRAW = 11,
			WM_SETTEXT = 12,
			WM_GETTEXT = 13,
			WM_GETTEXTLENGTH = 14,
			WM_PAINT = 15,
			WM_CLOSE = 16,
			WM_QUERYENDSESSION = 17,
			WM_QUIT = 18,
			WM_QUERYOPEN = 19,
			WM_ERASEBKGND = 20,
			WM_SYSCOLORCHANGE = 21,
			WM_ENDSESSION = 22,
			WM_SHOWWINDOW = 24,
			WM_CTLCOLOR = 25,
			WM_SETTINGCHANGE = 26,
			WM_WININICHANGE = 26,
			WM_DEVMODECHANGE = 27,
			WM_ACTIVATEAPP = 28,
			WM_FONTCHANGE = 29,
			WM_TIMECHANGE = 30,
			WM_CANCELMODE = 31,
			WM_SETCURSOR = 32,
			WM_MOUSEACTIVATE = 33,
			WM_CHILDACTIVATE = 34,
			WM_QUEUESYNC = 35,
			WM_GETMINMAXINFO = 36,
			WM_PAINTICON = 38,
			WM_ICONERASEBKGND = 39,
			WM_NEXTDLGCTL = 40,
			WM_SPOOLERSTATUS = 42,
			WM_DRAWITEM = 43,
			WM_MEASUREITEM = 44,
			WM_DELETEITEM = 45,
			WM_VKEYTOITEM = 46,
			WM_CHARTOITEM = 47,
			WM_SETFONT = 48,
			WM_GETFONT = 49,
			WM_SETHOTKEY = 50,
			WM_GETHOTKEY = 51,
			WM_QUERYDRAGICON = 55,
			WM_COMPAREITEM = 57,
			WM_GETOBJECT = 61,
			WM_COMPACTING = 65,
			WM_COMMNOTIFY = 68,
			WM_WINDOWPOSCHANGING = 70,
			WM_WINDOWPOSCHANGED = 71,
			WM_POWER = 72,
			WM_COPYDATA = 74,
			WM_CANCELJOURNAL = 75,
			WM_NOTIFY = 78,
			WM_INPUTLANGCHANGEREQUEST = 80,
			WM_INPUTLANGCHANGE = 81,
			WM_TCARD = 82,
			WM_HELP = 83,
			WM_USERCHANGED = 84,
			WM_NOTIFYFORMAT = 85,
			WM_CONTEXTMENU = 123,
			WM_STYLECHANGING = 124,
			WM_STYLECHANGED = 125,
			WM_DISPLAYCHANGE = 126,
			WM_GETICON = 127,
			WM_SETICON = 128,
			WM_NCCREATE = 129,
			WM_NCDESTROY = 130,
			WM_NCCALCSIZE = 131,
			WM_NCHITTEST = 132,
			WM_NCPAINT = 133,
			WM_NCACTIVATE = 134,
			WM_GETDLGCODE = 135,
			WM_SYNCPAINT = 136,
			WM_NCMOUSEMOVE = 160,
			WM_NCLBUTTONDOWN = 161,
			WM_NCLBUTTONUP = 162,
			WM_NCLBUTTONDBLCLK = 163,
			WM_NCRBUTTONDOWN = 164,
			WM_NCRBUTTONUP = 165,
			WM_NCRBUTTONDBLCLK = 166,
			WM_NCMBUTTONDOWN = 167,
			WM_NCMBUTTONUP = 168,
			WM_NCMBUTTONDBLCLK = 169,
			WM_NCXBUTTONDOWN = 171,
			WM_NCXBUTTONUP = 172,
			WM_NCXBUTTONDBLCLK = 173,
			WM_INPUT = 255,
			WM_KEYDOWN = 256,
			WM_KEYFIRST = 256,
			WM_KEYUP = 257,
			WM_CHAR = 258,
			WM_DEADCHAR = 259,
			WM_SYSKEYDOWN = 260,
			WM_SYSKEYUP = 261,
			WM_SYSCHAR = 262,
			WM_SYSDEADCHAR = 263,
			WM_KEYLAST = 264,
			WM_UNICHAR = 265,
			WM_IME_STARTCOMPOSITION = 269,
			WM_IME_ENDCOMPOSITION = 270,
			WM_IME_COMPOSITION = 271,
			WM_IME_KEYLAST = 271,
			WM_INITDIALOG = 272,
			WM_COMMAND = 273,
			WM_SYSCOMMAND = 274,
			WM_TIMER = 275,
			WM_HSCROLL = 276,
			WM_VSCROLL = 277,
			WM_INITMENU = 278,
			WM_INITMENUPOPUP = 279,
			WM_MENUSELECT = 287,
			WM_MENUCHAR = 288,
			WM_ENTERIDLE = 289,
			WM_MENURBUTTONUP = 290,
			WM_MENUDRAG = 291,
			WM_MENUGETOBJECT = 292,
			WM_UNINITMENUPOPUP = 293,
			WM_MENUCOMMAND = 294,
			WM_CHANGEUISTATE = 295,
			WM_UPDATEUISTATE = 296,
			WM_QUERYUISTATE = 297,
			WM_CTLCOLORMSGBOX = 306,
			WM_CTLCOLOREDIT = 307,
			WM_CTLCOLORLISTBOX = 308,
			WM_CTLCOLORBTN = 309,
			WM_CTLCOLORDLG = 310,
			WM_CTLCOLORSCROLLBAR = 311,
			WM_CTLCOLORSTATIC = 312,
			WM_MOUSEFIRST = 512,
			WM_MOUSEMOVE = 512,
			WM_LBUTTONDOWN = 513,
			WM_LBUTTONUP = 514,
			WM_LBUTTONDBLCLK = 515,
			WM_RBUTTONDOWN = 516,
			WM_RBUTTONUP = 517,
			WM_RBUTTONDBLCLK = 518,
			WM_MBUTTONDOWN = 519,
			WM_MBUTTONUP = 520,
			WM_MBUTTONDBLCLK = 521,
			WM_MOUSEWHEEL = 522,
			WM_XBUTTONDOWN = 523,
			WM_XBUTTONUP = 524,
			WM_MOUSELAST = 525,
			WM_XBUTTONDBLCLK = 525,
			WM_PARENTNOTIFY = 528,
			WM_ENTERMENULOOP = 529,
			WM_EXITMENULOOP = 530,
			WM_NEXTMENU = 531,
			WM_SIZING = 532,
			WM_CAPTURECHANGED = 533,
			WM_MOVING = 534,
			WM_POWERBROADCAST = 536,
			WM_DEVICECHANGE = 537,
			WM_MDICREATE = 544,
			WM_MDIDESTROY = 545,
			WM_MDIACTIVATE = 546,
			WM_MDIRESTORE = 547,
			WM_MDINEXT = 548,
			WM_MDIMAXIMIZE = 549,
			WM_MDITILE = 550,
			WM_MDICASCADE = 551,
			WM_MDIICONARRANGE = 552,
			WM_MDIGETACTIVE = 553,
			WM_MDISETMENU = 560,
			WM_ENTERSIZEMOVE = 561,
			WM_EXITSIZEMOVE = 562,
			WM_DROPFILES = 563,
			WM_MDIREFRESHMENU = 564,
			WM_IME_SETCONTEXT = 641,
			WM_IME_NOTIFY = 642,
			WM_IME_CONTROL = 643,
			WM_IME_COMPOSITIONFULL = 644,
			WM_IME_SELECT = 645,
			WM_IME_CHAR = 646,
			WM_IME_REQUEST = 648,
			WM_IME_KEYDOWN = 656,
			WM_IME_KEYUP = 657,
			WM_MOUSEHOVER = 673,
			WM_NCMOUSELEAVE = 674,
			WM_MOUSELEAVE = 675,
			WM_WTSSESSION_CHANGE = 689,
			WM_TABLET_FIRST = 704,
			WM_TABLET_LAST = 735,
			WM_CUT = 768,
			WM_COPY = 769,
			WM_PASTE = 770,
			WM_CLEAR = 771,
			WM_UNDO = 772,
			WM_RENDERFORMAT = 773,
			WM_RENDERALLFORMATS = 774,
			WM_DESTROYCLIPBOARD = 775,
			WM_DRAWCLIPBOARD = 776,
			WM_PAINTCLIPBOARD = 777,
			WM_VSCROLLCLIPBOARD = 778,
			WM_SIZECLIPBOARD = 779,
			WM_ASKCBFORMATNAME = 780,
			WM_CHANGECBCHAIN = 781,
			WM_HSCROLLCLIPBOARD = 782,
			WM_QUERYNEWPALETTE = 783,
			WM_PALETTEISCHANGING = 784,
			WM_PALETTECHANGED = 785,
			WM_HOTKEY = 786,
			WM_PRINT = 791,
			WM_PRINTCLIENT = 792,
			WM_APPCOMMAND = 793,
			WM_THEMECHANGED = 794,
			WM_DWMCOMPOSITIONCHANGED = 798,
			WM_HANDHELDFIRST = 856,
			WM_HANDHELDLAST = 863,
			WM_AFXFIRST = 864,
			WM_AFXLAST = 895,
			WM_PENWINFIRST = 896,
			WM_PENWINLAST = 911,
			WM_USER = 1024,
			WM_REFLECT = 8192,
			WM_APP = 32768,
			SC_MOVE = 61456,
			SC_MINIMIZE = 61472,
			SC_MAXIMIZE = 61488,
			SC_RESTORE = 61728
		}

		public struct MINMAXINFO
		{
			public WinApi.POINT ptReserved;

			public WinApi.POINT ptMaxSize;

			public WinApi.POINT ptMaxPosition;

			public WinApi.POINT ptMinTrackSize;

			public WinApi.POINT ptMaxTrackSize;
		}

		public struct NCCALCSIZE_PARAMS
		{
			public WinApi.RECT rect0;

			public WinApi.RECT rect1;

			public WinApi.RECT rect2;

			public IntPtr lppos;
		}

		public struct POINT
		{
			public int x;

			public int y;

			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		public struct RECT
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;

			public RECT(Rectangle rc)
			{
				this.Left = rc.Left;
				this.Top = rc.Top;
				this.Right = rc.Right;
				this.Bottom = rc.Bottom;
			}

			public Rectangle ToRectangle()
			{
				return Rectangle.FromLTRB(this.Left, this.Top, this.Right, this.Bottom);
			}
		}

		public enum ScrollBar
		{
			SB_HORZ,
			SB_VERT,
			SB_CTL,
			SB_BOTH
		}

		public struct SIZE
		{
			public int cx;

			public int cy;

			public SIZE(int cx, int cy)
			{
				this.cx = cx;
				this.cy = cy;
			}
		}

		public enum TabControlHitTest
		{
			TCHT_NOWHERE = 1
		}

		public struct TCHITTESTINFO
		{
			public Point pt;

			public uint flags;
		}

		public struct WindowPos
		{
			public int hwnd;

			public int hWndInsertAfter;

			public int x;

			public int y;

			public int cx;

			public int cy;

			public int flags;
		}
	}
}