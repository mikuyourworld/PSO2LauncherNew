using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MetroFramework.Native
{
	[SuppressUnmanagedCodeSecurity]
	internal sealed class WinCaret
	{
		private IntPtr controlHandle;

		public WinCaret(IntPtr ownerHandle)
		{
			this.controlHandle = ownerHandle;
		}

		public bool Create(int width, int height)
		{
			return WinCaret.CreateCaret(this.controlHandle, 0, width, height);
		}

		[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool CreateCaret(IntPtr hWnd, int hBitmap, int nWidth, int nHeight);

		public void Destroy()
		{
			WinCaret.DestroyCaret();
		}

		[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool DestroyCaret();

		public void Hide()
		{
			WinCaret.HideCaret(this.controlHandle);
		}

		[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool HideCaret(IntPtr hWnd);

		[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool SetCaretPos(int x, int y);

		public bool SetPosition(int x, int y)
		{
			return WinCaret.SetCaretPos(x, y);
		}

		public void Show()
		{
			WinCaret.ShowCaret(this.controlHandle);
		}

		[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool ShowCaret(IntPtr hWnd);
	}
}