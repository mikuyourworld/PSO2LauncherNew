using MetroFramework;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework.Drawing
{
	public sealed class MetroPaint
	{
		public MetroPaint()
		{
		}

		public static StringFormat GetStringFormat(ContentAlignment textAlign)
		{
			StringFormat stringFormat = new StringFormat()
			{
				Trimming = StringTrimming.EllipsisCharacter
			};
			ContentAlignment contentAlignment = textAlign;
			if (contentAlignment <= ContentAlignment.MiddleCenter)
			{
				switch (contentAlignment)
				{
					case ContentAlignment.TopLeft:
					{
						stringFormat.Alignment = StringAlignment.Near;
						stringFormat.LineAlignment = StringAlignment.Near;
						break;
					}
					case ContentAlignment.TopCenter:
					{
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Near;
						break;
					}
					case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
					{
						break;
					}
					case ContentAlignment.TopRight:
					{
						stringFormat.Alignment = StringAlignment.Far;
						stringFormat.LineAlignment = StringAlignment.Near;
						break;
					}
					default:
					{
						if (contentAlignment == ContentAlignment.MiddleLeft)
						{
							stringFormat.Alignment = StringAlignment.Center;
							stringFormat.LineAlignment = StringAlignment.Near;
							break;
						}
						else if (contentAlignment == ContentAlignment.MiddleCenter)
						{
							stringFormat.Alignment = StringAlignment.Center;
							stringFormat.LineAlignment = StringAlignment.Center;
							break;
						}
						else
						{
							break;
						}
					}
				}
			}
			else if (contentAlignment <= ContentAlignment.BottomLeft)
			{
				if (contentAlignment == ContentAlignment.MiddleRight)
				{
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				else if (contentAlignment == ContentAlignment.BottomLeft)
				{
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Near;
				}
			}
			else if (contentAlignment == ContentAlignment.BottomCenter)
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (contentAlignment == ContentAlignment.BottomRight)
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			return stringFormat;
		}

		public static SolidBrush GetStyleBrush(MetroColorStyle style)
		{
			switch (style)
			{
				case MetroColorStyle.Black:
				{
					return MetroBrushes.Black;
				}
				case MetroColorStyle.White:
				{
					return MetroBrushes.White;
				}
				case MetroColorStyle.Silver:
				{
					return MetroBrushes.Silver;
				}
				case MetroColorStyle.Blue:
				{
					return MetroBrushes.Blue;
				}
				case MetroColorStyle.Green:
				{
					return MetroBrushes.Green;
				}
				case MetroColorStyle.Lime:
				{
					return MetroBrushes.Lime;
				}
				case MetroColorStyle.Teal:
				{
					return MetroBrushes.Teal;
				}
				case MetroColorStyle.Orange:
				{
					return MetroBrushes.Orange;
				}
				case MetroColorStyle.Brown:
				{
					return MetroBrushes.Brown;
				}
				case MetroColorStyle.Pink:
				{
					return MetroBrushes.Pink;
				}
				case MetroColorStyle.Magenta:
				{
					return MetroBrushes.Magenta;
				}
				case MetroColorStyle.Purple:
				{
					return MetroBrushes.Purple;
				}
				case MetroColorStyle.Red:
				{
					return MetroBrushes.Red;
				}
				case MetroColorStyle.Yellow:
				{
					return MetroBrushes.Yellow;
				}
			}
			return MetroBrushes.Blue;
		}

		public static Color GetStyleColor(MetroColorStyle style)
		{
			switch (style)
			{
				case MetroColorStyle.Black:
				{
					return MetroColors.Black;
				}
				case MetroColorStyle.White:
				{
					return MetroColors.White;
				}
				case MetroColorStyle.Silver:
				{
					return MetroColors.Silver;
				}
				case MetroColorStyle.Blue:
				{
					return MetroColors.Blue;
				}
				case MetroColorStyle.Green:
				{
					return MetroColors.Green;
				}
				case MetroColorStyle.Lime:
				{
					return MetroColors.Lime;
				}
				case MetroColorStyle.Teal:
				{
					return MetroColors.Teal;
				}
				case MetroColorStyle.Orange:
				{
					return MetroColors.Orange;
				}
				case MetroColorStyle.Brown:
				{
					return MetroColors.Brown;
				}
				case MetroColorStyle.Pink:
				{
					return MetroColors.Pink;
				}
				case MetroColorStyle.Magenta:
				{
					return MetroColors.Magenta;
				}
				case MetroColorStyle.Purple:
				{
					return MetroColors.Purple;
				}
				case MetroColorStyle.Red:
				{
					return MetroColors.Red;
				}
				case MetroColorStyle.Yellow:
				{
					return MetroColors.Yellow;
				}
			}
			return MetroColors.Blue;
		}

		public static Pen GetStylePen(MetroColorStyle style)
		{
			switch (style)
			{
				case MetroColorStyle.Black:
				{
					return MetroPens.Black;
				}
				case MetroColorStyle.White:
				{
					return MetroPens.White;
				}
				case MetroColorStyle.Silver:
				{
					return MetroPens.Silver;
				}
				case MetroColorStyle.Blue:
				{
					return MetroPens.Blue;
				}
				case MetroColorStyle.Green:
				{
					return MetroPens.Green;
				}
				case MetroColorStyle.Lime:
				{
					return MetroPens.Lime;
				}
				case MetroColorStyle.Teal:
				{
					return MetroPens.Teal;
				}
				case MetroColorStyle.Orange:
				{
					return MetroPens.Orange;
				}
				case MetroColorStyle.Brown:
				{
					return MetroPens.Brown;
				}
				case MetroColorStyle.Pink:
				{
					return MetroPens.Pink;
				}
				case MetroColorStyle.Magenta:
				{
					return MetroPens.Magenta;
				}
				case MetroColorStyle.Purple:
				{
					return MetroPens.Purple;
				}
				case MetroColorStyle.Red:
				{
					return MetroPens.Red;
				}
				case MetroColorStyle.Yellow:
				{
					return MetroPens.Yellow;
				}
			}
			return MetroPens.Blue;
		}

		public static TextFormatFlags GetTextFormatFlags(ContentAlignment textAlign)
		{
			return MetroPaint.GetTextFormatFlags(textAlign, false);
		}

		public static TextFormatFlags GetTextFormatFlags(ContentAlignment textAlign, bool WrapToLine)
		{
			TextFormatFlags textFormatFlag = TextFormatFlags.Default;
			switch (WrapToLine)
			{
				case false:
				{
					textFormatFlag = TextFormatFlags.EndEllipsis;
					break;
				}
				case true:
				{
					textFormatFlag = TextFormatFlags.WordBreak;
					break;
				}
			}
			ContentAlignment contentAlignment = textAlign;
			if (contentAlignment <= ContentAlignment.MiddleCenter)
			{
				switch (contentAlignment)
				{
					case ContentAlignment.TopLeft:
					{
						textFormatFlag = textFormatFlag;
						break;
					}
					case ContentAlignment.TopCenter:
					{
						textFormatFlag = textFormatFlag | TextFormatFlags.HorizontalCenter;
						break;
					}
					case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
					{
						break;
					}
					case ContentAlignment.TopRight:
					{
						textFormatFlag = textFormatFlag | TextFormatFlags.Right;
						break;
					}
					default:
					{
						if (contentAlignment == ContentAlignment.MiddleLeft)
						{
							textFormatFlag = textFormatFlag | TextFormatFlags.VerticalCenter;
							break;
						}
						else if (contentAlignment == ContentAlignment.MiddleCenter)
						{
							textFormatFlag = textFormatFlag | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
							break;
						}
						else
						{
							break;
						}
					}
				}
			}
			else if (contentAlignment <= ContentAlignment.BottomLeft)
			{
				if (contentAlignment == ContentAlignment.MiddleRight)
				{
					textFormatFlag = textFormatFlag | TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
				}
				else if (contentAlignment == ContentAlignment.BottomLeft)
				{
					textFormatFlag = textFormatFlag | TextFormatFlags.Bottom;
				}
			}
			else if (contentAlignment == ContentAlignment.BottomCenter)
			{
				textFormatFlag = textFormatFlag | TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
			}
			else if (contentAlignment == ContentAlignment.BottomRight)
			{
				textFormatFlag = textFormatFlag | TextFormatFlags.Bottom | TextFormatFlags.Right;
			}
			return textFormatFlag;
		}

		public sealed class BackColor
		{
			public BackColor()
			{
			}

			public static Color Form(MetroThemeStyle theme)
			{
				if (theme == MetroThemeStyle.Light)
				{
                    return Color.FromArgb(255, 255, 255);
                }
                return Color.FromArgb(17, 17, 17);
			}

			public sealed class Button
			{
				public Button()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Light)
                    {
                        return Color.FromArgb(204, 204, 204);
                    }
                    return Color.FromArgb(80, 80, 80);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Light)
					{
                        return Color.FromArgb(102, 102, 102);
                    }
                    return Color.FromArgb(170, 170, 170);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
                    if (theme == MetroThemeStyle.Light)
                    {
                        return Color.FromArgb(238, 238, 238);
                    }
                    return Color.FromArgb(34, 34, 34);
                }

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Light)
					{
                        return Color.FromArgb(51, 51, 51);
                    }
                    return Color.FromArgb(238, 238, 238);
				}
			}

			public sealed class ProgressBar
			{
				public ProgressBar()
				{
				}

				public sealed class Bar
				{
					public Bar()
					{
					}

					public static Color Disabled(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(51, 51, 51);
						}
						return Color.FromArgb(221, 221, 221);
					}

					public static Color Hover(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(38, 38, 38);
						}
						return Color.FromArgb(234, 234, 234);
					}

					public static Color Normal(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(38, 38, 38);
						}
						return Color.FromArgb(234, 234, 234);
					}

					public static Color Press(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(38, 38, 38);
						}
						return Color.FromArgb(234, 234, 234);
					}
				}
			}

			public sealed class ScrollBar
			{
				public ScrollBar()
				{
				}

				public sealed class Bar
				{
					public Bar()
					{
					}

					public static Color Disabled(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(38, 38, 38);
						}
						return Color.FromArgb(234, 234, 234);
					}

					public static Color Hover(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(38, 38, 38);
						}
						return Color.FromArgb(234, 234, 234);
					}

					public static Color Normal(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(38, 38, 38);
						}
						return Color.FromArgb(234, 234, 234);
					}

					public static Color Press(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(38, 38, 38);
						}
						return Color.FromArgb(234, 234, 234);
					}
				}

				public sealed class Thumb
				{
					public Thumb()
					{
					}

					public static Color Disabled(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(51, 51, 51);
						}
						return Color.FromArgb(221, 221, 221);
					}

					public static Color Hover(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(204, 204, 204);
						}
						return Color.FromArgb(96, 96, 96);
					}

					public static Color Normal(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(51, 51, 51);
						}
						return Color.FromArgb(221, 221, 221);
					}

					public static Color Press(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(204, 204, 204);
						}
						return Color.FromArgb(96, 96, 96);
					}
				}
			}

			public sealed class TrackBar
			{
				public TrackBar()
				{
				}

				public sealed class Bar
				{
					public Bar()
					{
					}

					public static Color Disabled(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(34, 34, 34);
						}
						return Color.FromArgb(230, 230, 230);
					}

					public static Color Hover(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(51, 51, 51);
						}
						return Color.FromArgb(204, 204, 204);
					}

					public static Color Normal(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(51, 51, 51);
						}
						return Color.FromArgb(204, 204, 204);
					}

					public static Color Press(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(51, 51, 51);
						}
						return Color.FromArgb(204, 204, 204);
					}
				}

				public sealed class Thumb
				{
					public Thumb()
					{
					}

					public static Color Disabled(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(85, 85, 85);
						}
						return Color.FromArgb(179, 179, 179);
					}

					public static Color Hover(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(204, 204, 204);
						}
						return Color.FromArgb(17, 17, 17);
					}

					public static Color Normal(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(153, 153, 153);
						}
						return Color.FromArgb(102, 102, 102);
					}

					public static Color Press(MetroThemeStyle theme)
					{
						if (theme == MetroThemeStyle.Dark)
						{
							return Color.FromArgb(204, 204, 204);
						}
						return Color.FromArgb(17, 17, 17);
					}
				}
			}
		}

		public sealed class BorderColor
		{
			public BorderColor()
			{
			}

			public static Color Form(MetroThemeStyle theme)
			{
				if (theme == MetroThemeStyle.Light)
				{
                    return Color.FromArgb(204, 204, 204);
                }
                return Color.FromArgb(68, 68, 68);
			}

			public static class Button
			{
				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(109, 109, 109);
					}
					return Color.FromArgb(155, 155, 155);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(170, 170, 170);
					}
					return Color.FromArgb(102, 102, 102);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(68, 68, 68);
					}
					return Color.FromArgb(204, 204, 204);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(238, 238, 238);
					}
					return Color.FromArgb(51, 51, 51);
				}
			}

			public static class CheckBox
			{
				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(85, 85, 85);
					}
					return Color.FromArgb(204, 204, 204);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(204, 204, 204);
					}
					return Color.FromArgb(51, 51, 51);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(153, 153, 153);
					}
					return Color.FromArgb(153, 153, 153);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(153, 153, 153);
					}
					return Color.FromArgb(153, 153, 153);
				}
			}

			public static class ComboBox
			{
				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(85, 85, 85);
					}
					return Color.FromArgb(204, 204, 204);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(204, 204, 204);
					}
					return Color.FromArgb(51, 51, 51);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(153, 153, 153);
					}
					return Color.FromArgb(153, 153, 153);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(153, 153, 153);
					}
					return Color.FromArgb(153, 153, 153);
				}
			}

			public static class ProgressBar
			{
				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(109, 109, 109);
					}
					return Color.FromArgb(155, 155, 155);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(68, 68, 68);
					}
					return Color.FromArgb(204, 204, 204);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(68, 68, 68);
					}
					return Color.FromArgb(204, 204, 204);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(68, 68, 68);
					}
					return Color.FromArgb(204, 204, 204);
				}
			}

			public static class TabControl
			{
				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(109, 109, 109);
					}
					return Color.FromArgb(155, 155, 155);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(68, 68, 68);
					}
					return Color.FromArgb(204, 204, 204);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(68, 68, 68);
					}
					return Color.FromArgb(204, 204, 204);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(68, 68, 68);
					}
					return Color.FromArgb(204, 204, 204);
				}
			}
		}

		public sealed class ForeColor
		{
			public ForeColor()
			{
			}

			public static Color Title(MetroThemeStyle theme)
			{
				if (theme == MetroThemeStyle.Dark)
				{
					return Color.FromArgb(255, 255, 255);
				}
				return Color.FromArgb(0, 0, 0);
			}

			public sealed class Button
			{
				public Button()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Light)
					{
                        return Color.FromArgb(136, 136, 136);
                    }
                    return Color.FromArgb(109, 109, 109);                    
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Light)
					{
                        return Color.FromArgb(255, 255, 255);
                    }
                    return Color.FromArgb(17, 17, 17);                    
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Light)
					{
                        return Color.FromArgb(0, 0, 0);
                    }
                    return Color.FromArgb(204, 204, 204);
                    
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Light)
					{
                        return Color.FromArgb(255, 255, 255);
                    }
                    return Color.FromArgb(17, 17, 17);                    
				}
			}

			public sealed class CheckBox
			{
				public CheckBox()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(93, 93, 93);
					}
					return Color.FromArgb(136, 136, 136);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(153, 153, 153);
					}
					return Color.FromArgb(153, 153, 153);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(170, 170, 170);
					}
					return Color.FromArgb(17, 17, 17);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(153, 153, 153);
					}
					return Color.FromArgb(153, 153, 153);
				}
			}

			public sealed class ComboBox
			{
				public ComboBox()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(93, 93, 93);
					}
					return Color.FromArgb(136, 136, 136);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(170, 170, 170);
					}
					return Color.FromArgb(17, 17, 17);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(153, 153, 153);
					}
					return Color.FromArgb(153, 153, 153);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(153, 153, 153);
					}
					return Color.FromArgb(153, 153, 153);
				}
			}

			public sealed class Label
			{
				public Label()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(51, 51, 51);
					}
					return Color.FromArgb(209, 209, 209);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(170, 170, 170);
					}
					return Color.FromArgb(0, 0, 0);
				}
			}

			public sealed class Link
			{
				public Link()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(51, 51, 51);
					}
					return Color.FromArgb(209, 209, 209);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(93, 93, 93);
					}
					return Color.FromArgb(128, 128, 128);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(170, 170, 170);
					}
					return Color.FromArgb(0, 0, 0);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(93, 93, 93);
					}
					return Color.FromArgb(128, 128, 128);
				}
			}

			public sealed class ProgressBar
			{
				public ProgressBar()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(51, 51, 51);
					}
					return Color.FromArgb(209, 209, 209);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(170, 170, 170);
					}
					return Color.FromArgb(0, 0, 0);
				}
			}

			public sealed class TabControl
			{
				public TabControl()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(51, 51, 51);
					}
					return Color.FromArgb(209, 209, 209);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(170, 170, 170);
					}
					return Color.FromArgb(0, 0, 0);
				}
			}

			public sealed class Tile
			{
				public Tile()
				{
				}

				public static Color Disabled(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(209, 209, 209);
					}
					return Color.FromArgb(209, 209, 209);
				}

				public static Color Hover(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(255, 255, 255);
					}
					return Color.FromArgb(255, 255, 255);
				}

				public static Color Normal(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(255, 255, 255);
					}
					return Color.FromArgb(255, 255, 255);
				}

				public static Color Press(MetroThemeStyle theme)
				{
					if (theme == MetroThemeStyle.Dark)
					{
						return Color.FromArgb(255, 255, 255);
					}
					return Color.FromArgb(255, 255, 255);
				}
			}
		}
	}
}