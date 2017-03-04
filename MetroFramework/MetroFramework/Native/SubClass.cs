using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;

namespace MetroFramework.Native
{
	[SuppressUnmanagedCodeSecurity]
	internal class SubClass : NativeWindow
	{
		private bool IsSubClassed;

		public bool SubClassed
		{
			get
			{
				return this.IsSubClassed;
			}
			set
			{
				this.IsSubClassed = value;
			}
		}

		public SubClass(IntPtr Handle, bool _SubClass)
		{
			base.AssignHandle(Handle);
			this.IsSubClassed = _SubClass;
		}

		public void CallDefaultWndProc(ref Message m)
		{
			base.WndProc(ref m);
		}

		public int HiWord(int Number)
		{
			return Number >> 16 & 65535;
		}

		public int LoWord(int Number)
		{
			return Number & 65535;
		}

		public int MakeLong(int LoWord, int HiWord)
		{
			return HiWord << 16 | LoWord & 65535;
		}

		public IntPtr MakeLParam(int LoWord, int HiWord)
		{
			return (IntPtr)(HiWord << 16 | LoWord & 65535);
		}

		private int OnSubClassedWndProc(ref Message m)
		{
			if (this.SubClassedWndProc == null)
			{
				return 0;
			}
			return this.SubClassedWndProc(ref m);
		}

		protected override void WndProc(ref Message m)
		{
			if (this.IsSubClassed && this.OnSubClassedWndProc(ref m) != 0)
			{
				return;
			}
			base.WndProc(ref m);
		}

		public event SubClass.SubClassWndProcEventHandler SubClassedWndProc;

		public delegate int SubClassWndProcEventHandler(ref Message m);
	}
}