using System;
using System.Drawing;

namespace MetroFramework
{
	public static class MetroFonts
	{
		private static MetroFonts.IMetroFontResolver FontResolver;

		public static Font Subtitle
		{
			get
			{
				return MetroFonts.Default(14f);
			}
		}

		public static Font TileCount
		{
			get
			{
				return MetroFonts.Default(44f);
			}
		}

		public static Font Title
		{
			get
			{
				return MetroFonts.DefaultLight(24f);
			}
		}

		static MetroFonts()
		{
			try
			{
				Type type = Type.GetType("MetroFramework.Fonts.FontResolver, MetroFramework.Fonts, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a");
				if (type != null)
				{
					MetroFonts.FontResolver = (MetroFonts.IMetroFontResolver)Activator.CreateInstance(type);
					if (MetroFonts.FontResolver != null)
					{
						return;
					}
				}
			}
			catch (Exception exception)
			{
			}
			MetroFonts.FontResolver = new MetroFonts.DefaultFontResolver();
		}

		public static Font Button(MetroButtonSize linkSize, MetroButtonWeight linkWeight)
		{
			if (linkSize == MetroButtonSize.Small)
			{
				if (linkWeight == MetroButtonWeight.Light)
				{
					return MetroFonts.DefaultLight(11f);
				}
				if (linkWeight == MetroButtonWeight.Regular)
				{
					return MetroFonts.Default(11f);
				}
				if (linkWeight == MetroButtonWeight.Bold)
				{
					return MetroFonts.DefaultBold(11f);
				}
			}
			else if (linkSize == MetroButtonSize.Medium)
			{
				if (linkWeight == MetroButtonWeight.Light)
				{
					return MetroFonts.DefaultLight(13f);
				}
				if (linkWeight == MetroButtonWeight.Regular)
				{
					return MetroFonts.Default(13f);
				}
				if (linkWeight == MetroButtonWeight.Bold)
				{
					return MetroFonts.DefaultBold(13f);
				}
			}
			else if (linkSize == MetroButtonSize.Tall)
			{
				if (linkWeight == MetroButtonWeight.Light)
				{
					return MetroFonts.DefaultLight(16f);
				}
				if (linkWeight == MetroButtonWeight.Regular)
				{
					return MetroFonts.Default(16f);
				}
				if (linkWeight == MetroButtonWeight.Bold)
				{
					return MetroFonts.DefaultBold(16f);
				}
			}
			return MetroFonts.Default(11f);
		}

		public static Font CheckBox(MetroCheckBoxSize linkSize, MetroCheckBoxWeight linkWeight)
		{
			if (linkSize == MetroCheckBoxSize.Small)
			{
				if (linkWeight == MetroCheckBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (linkWeight == MetroCheckBoxWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (linkWeight == MetroCheckBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (linkSize == MetroCheckBoxSize.Medium)
			{
				if (linkWeight == MetroCheckBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (linkWeight == MetroCheckBoxWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (linkWeight == MetroCheckBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (linkSize == MetroCheckBoxSize.Tall)
			{
				if (linkWeight == MetroCheckBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (linkWeight == MetroCheckBoxWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (linkWeight == MetroCheckBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.Default(12f);
		}

		public static Font ComboBox(MetroComboBoxSize linkSize, MetroComboBoxWeight linkWeight)
		{
			if (linkSize == MetroComboBoxSize.Small)
			{
				if (linkWeight == MetroComboBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (linkWeight == MetroComboBoxWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (linkWeight == MetroComboBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (linkSize == MetroComboBoxSize.Medium)
			{
				if (linkWeight == MetroComboBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (linkWeight == MetroComboBoxWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (linkWeight == MetroComboBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (linkSize == MetroComboBoxSize.Tall)
			{
				if (linkWeight == MetroComboBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (linkWeight == MetroComboBoxWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (linkWeight == MetroComboBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.Default(12f);
		}

		public static Font DateTime(MetroDateTimeSize linkSize, MetroDateTimeWeight linkWeight)
		{
			if (linkSize == MetroDateTimeSize.Small)
			{
				if (linkWeight == MetroDateTimeWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (linkWeight == MetroDateTimeWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (linkWeight == MetroDateTimeWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (linkSize == MetroDateTimeSize.Medium)
			{
				if (linkWeight == MetroDateTimeWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (linkWeight == MetroDateTimeWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (linkWeight == MetroDateTimeWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (linkSize == MetroDateTimeSize.Tall)
			{
				if (linkWeight == MetroDateTimeWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (linkWeight == MetroDateTimeWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (linkWeight == MetroDateTimeWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.Default(12f);
		}

		public static Font Default(float size)
		{
			return MetroFonts.FontResolver.ResolveFont("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Pixel);
		}

		public static Font DefaultBold(float size)
		{
			return MetroFonts.FontResolver.ResolveFont("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Pixel);
		}

		public static Font DefaultItalic(float size)
		{
			return MetroFonts.FontResolver.ResolveFont("Segoe UI", size, FontStyle.Italic, GraphicsUnit.Pixel);
		}

		public static Font DefaultLight(float size)
		{
			return MetroFonts.FontResolver.ResolveFont("Segoe UI Light", size, FontStyle.Regular, GraphicsUnit.Pixel);
		}

		public static Font Label(MetroLabelSize labelSize, MetroLabelWeight labelWeight)
		{
			if (labelSize == MetroLabelSize.Small)
			{
				if (labelWeight == MetroLabelWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (labelWeight == MetroLabelWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (labelWeight == MetroLabelWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (labelSize == MetroLabelSize.Medium)
			{
				if (labelWeight == MetroLabelWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (labelWeight == MetroLabelWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (labelWeight == MetroLabelWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (labelSize == MetroLabelSize.Tall)
			{
				if (labelWeight == MetroLabelWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (labelWeight == MetroLabelWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (labelWeight == MetroLabelWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.DefaultLight(14f);
		}

		public static Font Link(MetroLinkSize linkSize, MetroLinkWeight linkWeight)
		{
			if (linkSize == MetroLinkSize.Small)
			{
				if (linkWeight == MetroLinkWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (linkWeight == MetroLinkWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (linkWeight == MetroLinkWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (linkSize == MetroLinkSize.Medium)
			{
				if (linkWeight == MetroLinkWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (linkWeight == MetroLinkWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (linkWeight == MetroLinkWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (linkSize == MetroLinkSize.Tall)
			{
				if (linkWeight == MetroLinkWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (linkWeight == MetroLinkWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (linkWeight == MetroLinkWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.Default(12f);
		}

		public static Font ProgressBar(MetroProgressBarSize labelSize, MetroProgressBarWeight labelWeight)
		{
			if (labelSize == MetroProgressBarSize.Small)
			{
				if (labelWeight == MetroProgressBarWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (labelWeight == MetroProgressBarWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (labelWeight == MetroProgressBarWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (labelSize == MetroProgressBarSize.Medium)
			{
				if (labelWeight == MetroProgressBarWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (labelWeight == MetroProgressBarWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (labelWeight == MetroProgressBarWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (labelSize == MetroProgressBarSize.Tall)
			{
				if (labelWeight == MetroProgressBarWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (labelWeight == MetroProgressBarWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (labelWeight == MetroProgressBarWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.DefaultLight(14f);
		}

		public static Font TabControl(MetroTabControlSize labelSize, MetroTabControlWeight labelWeight)
		{
			if (labelSize == MetroTabControlSize.Small)
			{
				if (labelWeight == MetroTabControlWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (labelWeight == MetroTabControlWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (labelWeight == MetroTabControlWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (labelSize == MetroTabControlSize.Medium)
			{
				if (labelWeight == MetroTabControlWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (labelWeight == MetroTabControlWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (labelWeight == MetroTabControlWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (labelSize == MetroTabControlSize.Tall)
			{
				if (labelWeight == MetroTabControlWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (labelWeight == MetroTabControlWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (labelWeight == MetroTabControlWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.DefaultLight(14f);
		}

		public static Font TextBox(MetroTextBoxSize linkSize, MetroTextBoxWeight linkWeight)
		{
			if (linkSize == MetroTextBoxSize.Small)
			{
				if (linkWeight == MetroTextBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (linkWeight == MetroTextBoxWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (linkWeight == MetroTextBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (linkSize == MetroTextBoxSize.Medium)
			{
				if (linkWeight == MetroTextBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (linkWeight == MetroTextBoxWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (linkWeight == MetroTextBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (linkSize == MetroTextBoxSize.Tall)
			{
				if (linkWeight == MetroTextBoxWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (linkWeight == MetroTextBoxWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (linkWeight == MetroTextBoxWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.Default(12f);
		}

		public static Font Tile(MetroTileTextSize labelSize, MetroTileTextWeight labelWeight)
		{
			if (labelSize == MetroTileTextSize.Small)
			{
				if (labelWeight == MetroTileTextWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (labelWeight == MetroTileTextWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (labelWeight == MetroTileTextWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
			}
			else if (labelSize == MetroTileTextSize.Medium)
			{
				if (labelWeight == MetroTileTextWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (labelWeight == MetroTileTextWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (labelWeight == MetroTileTextWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
			}
			else if (labelSize == MetroTileTextSize.Tall)
			{
				if (labelWeight == MetroTileTextWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (labelWeight == MetroTileTextWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (labelWeight == MetroTileTextWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
			}
			return MetroFonts.DefaultLight(14f);
		}

		public static Font WaterMark(MetroLabelSize labelSize, MetroWaterMarkWeight labelWeight)
		{
			if (labelSize == MetroLabelSize.Small)
			{
				if (labelWeight == MetroWaterMarkWeight.Light)
				{
					return MetroFonts.DefaultLight(12f);
				}
				if (labelWeight == MetroWaterMarkWeight.Regular)
				{
					return MetroFonts.Default(12f);
				}
				if (labelWeight == MetroWaterMarkWeight.Bold)
				{
					return MetroFonts.DefaultBold(12f);
				}
				if (labelWeight == MetroWaterMarkWeight.Italic)
				{
					return MetroFonts.DefaultItalic(12f);
				}
			}
			else if (labelSize == MetroLabelSize.Medium)
			{
				if (labelWeight == MetroWaterMarkWeight.Light)
				{
					return MetroFonts.DefaultLight(14f);
				}
				if (labelWeight == MetroWaterMarkWeight.Regular)
				{
					return MetroFonts.Default(14f);
				}
				if (labelWeight == MetroWaterMarkWeight.Bold)
				{
					return MetroFonts.DefaultBold(14f);
				}
				if (labelWeight == MetroWaterMarkWeight.Italic)
				{
					return MetroFonts.DefaultItalic(14f);
				}
			}
			else if (labelSize == MetroLabelSize.Tall)
			{
				if (labelWeight == MetroWaterMarkWeight.Light)
				{
					return MetroFonts.DefaultLight(18f);
				}
				if (labelWeight == MetroWaterMarkWeight.Regular)
				{
					return MetroFonts.Default(18f);
				}
				if (labelWeight == MetroWaterMarkWeight.Bold)
				{
					return MetroFonts.DefaultBold(18f);
				}
				if (labelWeight == MetroWaterMarkWeight.Italic)
				{
					return MetroFonts.DefaultItalic(18f);
				}
			}
			return MetroFonts.DefaultLight(14f);
		}

		private class DefaultFontResolver : MetroFonts.IMetroFontResolver
		{
			public DefaultFontResolver()
			{
			}

			public Font ResolveFont(string familyName, float emSize, FontStyle fontStyle, GraphicsUnit unit)
			{
				return new Font(familyName, emSize, fontStyle, unit);
			}
		}

		public interface IMetroFontResolver
		{
			Font ResolveFont(string familyName, float emSize, FontStyle fontStyle, GraphicsUnit unit);
		}
	}
}