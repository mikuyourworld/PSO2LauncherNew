using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework
{
	public class MetroMessageBoxProperties
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MetroMessageBoxControl _owner;

		public MessageBoxButtons Buttons
		{
			get;
			set;
		}

		public MessageBoxDefaultButton DefaultButton
		{
			get;
			set;
		}

		public MessageBoxIcon Icon
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public MetroMessageBoxControl Owner
		{
			get
			{
				return this._owner;
			}
		}

		public string Title
		{
			get;
			set;
		}

		public MetroMessageBoxProperties(MetroMessageBoxControl owner)
		{
			this._owner = owner;
		}
	}
}