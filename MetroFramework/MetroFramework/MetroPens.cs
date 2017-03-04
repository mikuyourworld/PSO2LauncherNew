using System;
using System.Collections.Generic;
using System.Drawing;

namespace MetroFramework
{
	public sealed class MetroPens
	{
		private static Dictionary<string, Pen> metroPens;

		public static Pen Black
		{
			get
			{
				return MetroPens.GetSavePen("Black", MetroColors.Black);
			}
		}

		public static Pen Blue
		{
			get
			{
				return MetroPens.GetSavePen("Blue", MetroColors.Blue);
			}
		}

		public static Pen Brown
		{
			get
			{
				return MetroPens.GetSavePen("Brown", MetroColors.Brown);
			}
		}

		public static Pen Green
		{
			get
			{
				return MetroPens.GetSavePen("Green", MetroColors.Green);
			}
		}

		public static Pen Lime
		{
			get
			{
				return MetroPens.GetSavePen("Lime", MetroColors.Lime);
			}
		}

		public static Pen Magenta
		{
			get
			{
				return MetroPens.GetSavePen("Magenta", MetroColors.Magenta);
			}
		}

		public static Pen Orange
		{
			get
			{
				return MetroPens.GetSavePen("Orange", MetroColors.Orange);
			}
		}

		public static Pen Pink
		{
			get
			{
				return MetroPens.GetSavePen("Pink", MetroColors.Pink);
			}
		}

		public static Pen Purple
		{
			get
			{
				return MetroPens.GetSavePen("Purple", MetroColors.Purple);
			}
		}

		public static Pen Red
		{
			get
			{
				return MetroPens.GetSavePen("Red", MetroColors.Red);
			}
		}

		public static Pen Silver
		{
			get
			{
				return MetroPens.GetSavePen("Silver", MetroColors.Silver);
			}
		}

		public static Pen Teal
		{
			get
			{
				return MetroPens.GetSavePen("Teal", MetroColors.Teal);
			}
		}

		public static Pen White
		{
			get
			{
				return MetroPens.GetSavePen("White", MetroColors.White);
			}
		}

		public static Pen Yellow
		{
			get
			{
				return MetroPens.GetSavePen("Yellow", MetroColors.Yellow);
			}
		}

		static MetroPens()
		{
			MetroPens.metroPens = new Dictionary<string, Pen>();
		}

		public MetroPens()
		{
		}

		private static Pen GetSavePen(string key, Color color)
		{
			Pen pen;
			lock (MetroPens.metroPens)
			{
				if (!MetroPens.metroPens.ContainsKey(key))
				{
					MetroPens.metroPens.Add(key, new Pen(color, 1f));
				}
				pen = MetroPens.metroPens[key].Clone() as Pen;
			}
			return pen;
		}
	}
}