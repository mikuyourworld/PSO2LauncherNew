using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MetroFramework.Drawing
{
	public class MetroPaintEventArgs : EventArgs
	{
		public Color BackColor
		{
			get;
			private set;
		}

		public Color ForeColor
		{
			get;
			private set;
		}

		public System.Drawing.Graphics Graphics
		{
			get;
			private set;
		}

		public MetroPaintEventArgs(Color backColor, Color foreColor, System.Drawing.Graphics g)
		{
			this.BackColor = backColor;
			this.ForeColor = foreColor;
			this.Graphics = g;
		}
	}
}