using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace MetroFramework.Native
{
	internal class Taskbar
	{
		private const string ClassName = "Shell_TrayWnd";

		private Rectangle bounds = Rectangle.Empty;

		private TaskbarPosition position = TaskbarPosition.Unknown;

		private bool alwaysOnTop;

		private bool autoHide;

		public bool AlwaysOnTop
		{
			get
			{
				return this.alwaysOnTop;
			}
			private set
			{
				this.alwaysOnTop = value;
			}
		}

		public bool AutoHide
		{
			get
			{
				return this.autoHide;
			}
			private set
			{
				this.autoHide = value;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
			private set
			{
				this.bounds = value;
			}
		}

		public Point Location
		{
			get
			{
				return this.Bounds.Location;
			}
		}

		public TaskbarPosition Position
		{
			get
			{
				return this.position;
			}
			private set
			{
				this.position = value;
			}
		}

		public System.Drawing.Size Size
		{
			get
			{
				return this.Bounds.Size;
			}
		}

		[SecuritySafeCritical]
		public Taskbar()
		{
			IntPtr intPtr = WinApi.FindWindow("Shell_TrayWnd", null);
			WinApi.APPBARDATA aPPBARDATum = new WinApi.APPBARDATA()
			{
				cbSize = (uint)Marshal.SizeOf(typeof(WinApi.APPBARDATA)),
				hWnd = intPtr
			};
			if (WinApi.SHAppBarMessage(WinApi.ABM.GetTaskbarPos, ref aPPBARDATum) == IntPtr.Zero)
			{
				throw new InvalidOperationException();
			}
			this.Position = (TaskbarPosition)aPPBARDATum.uEdge;
			this.Bounds = Rectangle.FromLTRB(aPPBARDATum.rc.Left, aPPBARDATum.rc.Top, aPPBARDATum.rc.Right, aPPBARDATum.rc.Bottom);
			aPPBARDATum.cbSize = (uint)Marshal.SizeOf(typeof(WinApi.APPBARDATA));
			IntPtr intPtr1 = WinApi.SHAppBarMessage(WinApi.ABM.GetState, ref aPPBARDATum);
			int num = intPtr1.ToInt32();
			this.AlwaysOnTop = (num & 2) == 2;
			this.AutoHide = (num & 1) == 1;
		}
	}
}