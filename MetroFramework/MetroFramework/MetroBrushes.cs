using System;
using System.Collections.Generic;
using System.Drawing;

namespace MetroFramework
{
	public sealed class MetroBrushes
	{
		private static Dictionary<string, SolidBrush> metroBrushes;

		public static SolidBrush Black
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Black", MetroColors.Black);
			}
		}

		public static SolidBrush Blue
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Blue", MetroColors.Blue);
			}
		}

		public static SolidBrush Brown
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Brown", MetroColors.Brown);
			}
		}

		public static SolidBrush Green
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Green", MetroColors.Green);
			}
		}

		public static SolidBrush Lime
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Lime", MetroColors.Lime);
			}
		}

		public static SolidBrush Magenta
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Magenta", MetroColors.Magenta);
			}
		}

		public static SolidBrush Orange
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Orange", MetroColors.Orange);
			}
		}

		public static SolidBrush Pink
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Pink", MetroColors.Pink);
			}
		}

		public static SolidBrush Purple
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Purple", MetroColors.Purple);
			}
		}

		public static SolidBrush Red
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Red", MetroColors.Red);
			}
		}

		public static SolidBrush Silver
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Silver", MetroColors.Silver);
			}
		}

		public static SolidBrush Teal
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Teal", MetroColors.Teal);
			}
		}

		public static SolidBrush White
		{
			get
			{
				return MetroBrushes.GetSaveBrush("White", MetroColors.White);
			}
		}

		public static SolidBrush Yellow
		{
			get
			{
				return MetroBrushes.GetSaveBrush("Yellow", MetroColors.Yellow);
			}
		}

		static MetroBrushes()
		{
			MetroBrushes.metroBrushes = new Dictionary<string, SolidBrush>();
		}

		public MetroBrushes()
		{
		}

		private static SolidBrush GetSaveBrush(string key, Color color)
		{
			SolidBrush solidBrush;
			lock (MetroBrushes.metroBrushes)
			{
				if (!MetroBrushes.metroBrushes.ContainsKey(key))
				{
					MetroBrushes.metroBrushes.Add(key, new SolidBrush(color));
				}
				solidBrush = MetroBrushes.metroBrushes[key].Clone() as SolidBrush;
			}
			return solidBrush;
		}
	}
}