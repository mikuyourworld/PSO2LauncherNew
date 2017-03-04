using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace MetroFramework.Drawing.Html
{
	[CLSCompliant(false)]
	public static class CssValue
	{
		private static object DetectSource(string path)
		{
			if (!path.StartsWith("method:", StringComparison.CurrentCultureIgnoreCase))
			{
				if (!path.StartsWith("property:", StringComparison.CurrentCultureIgnoreCase))
				{
					if (Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
					{
						return new Uri(path);
					}
					return new FileInfo(path);
				}
				string empty = string.Empty;
				Type typeInfo = CssValue.GetTypeInfo(path.Substring(9), ref empty);
				if (typeInfo == null)
				{
					return null;
				}
				return typeInfo.GetProperty(empty);
			}
			string str = string.Empty;
			Type type = CssValue.GetTypeInfo(path.Substring(7), ref str);
			if (type == null)
			{
				return null;
			}
			MethodInfo method = type.GetMethod(str);
			if (method.IsStatic && (int)method.GetParameters().Length <= 0)
			{
				return method;
			}
			return null;
		}

		public static float GetActualBorderWidth(string borderValue, CssBox b)
		{
			if (string.IsNullOrEmpty(borderValue))
			{
				return CssValue.GetActualBorderWidth("medium", b);
			}
			string str = borderValue;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "thin")
				{
					return 1f;
				}
				if (str1 == "medium")
				{
					return 2f;
				}
				if (str1 == "thick")
				{
					return 4f;
				}
			}
			return Math.Abs(CssValue.ParseLength(borderValue, 1f, b));
		}

		public static Color GetActualColor(string colorValue)
		{
			int r = 0;
			int g = 0;
			int b = 0;
			Color empty = Color.Empty;
			if (string.IsNullOrEmpty(colorValue))
			{
				return empty;
			}
			colorValue = colorValue.ToLower().Trim();
			if (colorValue.StartsWith("#"))
			{
				string str = colorValue.Substring(1);
				if (str.Length != 6)
				{
					if (str.Length != 3)
					{
						return empty;
					}
					r = int.Parse(new string(str.Substring(0, 1)[0], 2), NumberStyles.HexNumber);
					g = int.Parse(new string(str.Substring(1, 1)[0], 2), NumberStyles.HexNumber);
					b = int.Parse(new string(str.Substring(2, 1)[0], 2), NumberStyles.HexNumber);
				}
				else
				{
					r = int.Parse(str.Substring(0, 2), NumberStyles.HexNumber);
					g = int.Parse(str.Substring(2, 2), NumberStyles.HexNumber);
					b = int.Parse(str.Substring(4, 2), NumberStyles.HexNumber);
				}
			}
			else if (!colorValue.StartsWith("rgb(") || !colorValue.EndsWith(")"))
			{
				string empty1 = string.Empty;
				string str1 = colorValue;
				string str2 = str1;
				if (str1 != null)
				{
					switch (str2)
					{
						case "maroon":
						{
							empty1 = "#800000";
							break;
						}
						case "red":
						{
							empty1 = "#ff0000";
							break;
						}
						case "orange":
						{
							empty1 = "#ffA500";
							break;
						}
						case "olive":
						{
							empty1 = "#808000";
							break;
						}
						case "purple":
						{
							empty1 = "#800080";
							break;
						}
						case "fuchsia":
						{
							empty1 = "#ff00ff";
							break;
						}
						case "white":
						{
							empty1 = "#ffffff";
							break;
						}
						case "lime":
						{
							empty1 = "#00ff00";
							break;
						}
						case "green":
						{
							empty1 = "#008000";
							break;
						}
						case "navy":
						{
							empty1 = "#000080";
							break;
						}
						case "blue":
						{
							empty1 = "#0000ff";
							break;
						}
						case "aqua":
						{
							empty1 = "#00ffff";
							break;
						}
						case "teal":
						{
							empty1 = "#008080";
							break;
						}
						case "black":
						{
							empty1 = "#000000";
							break;
						}
						case "silver":
						{
							empty1 = "#c0c0c0";
							break;
						}
						case "gray":
						{
							empty1 = "#808080";
							break;
						}
						case "yellow":
						{
							empty1 = "#FFFF00";
							break;
						}
					}
				}
				if (string.IsNullOrEmpty(empty1))
				{
					return empty;
				}
				Color actualColor = CssValue.GetActualColor(empty1);
				r = actualColor.R;
				g = actualColor.G;
				b = actualColor.B;
			}
			else
			{
				string str3 = colorValue.Substring(4, colorValue.Length - 5);
				string[] strArrays = str3.Split(new char[] { ',' });
				if ((int)strArrays.Length != 3)
				{
					return empty;
				}
				r = Convert.ToInt32(CssValue.ParseNumber(strArrays[0].Trim(), 255f));
				g = Convert.ToInt32(CssValue.ParseNumber(strArrays[1].Trim(), 255f));
				b = Convert.ToInt32(CssValue.ParseNumber(strArrays[2].Trim(), 255f));
			}
			return Color.FromArgb(r, g, b);
		}

		public static Image GetImage(string path)
		{
			Image value;
			object obj = CssValue.DetectSource(path);
			FileInfo fileInfo = obj as FileInfo;
			PropertyInfo propertyInfo = obj as PropertyInfo;
			MethodInfo methodInfo = obj as MethodInfo;
			try
			{
				if (fileInfo != null)
				{
					if (fileInfo.Exists)
					{
						value = Image.FromFile(fileInfo.FullName);
					}
					else
					{
						value = null;
					}
				}
				else if (propertyInfo != null)
				{
					if (propertyInfo.PropertyType.IsSubclassOf(typeof(Image)) || propertyInfo.PropertyType.Equals(typeof(Image)))
					{
						value = propertyInfo.GetValue(null, null) as Image;
					}
					else
					{
						value = null;
					}
				}
				else if (methodInfo == null)
				{
					value = null;
				}
				else if (methodInfo.ReturnType.IsSubclassOf(typeof(Image)))
				{
					value = methodInfo.Invoke(null, null) as Image;
				}
				else
				{
					value = null;
				}
			}
			catch
			{
				value = new Bitmap(50, 50);
			}
			return value;
		}

		public static string GetStyleSheet(string path)
		{
			string value;
			object obj = CssValue.DetectSource(path);
			FileInfo fileInfo = obj as FileInfo;
			PropertyInfo propertyInfo = obj as PropertyInfo;
			MethodInfo methodInfo = obj as MethodInfo;
			try
			{
				if (fileInfo != null)
				{
					if (fileInfo.Exists)
					{
						StreamReader streamReader = new StreamReader(fileInfo.FullName);
						string end = streamReader.ReadToEnd();
						streamReader.Dispose();
						value = end;
					}
					else
					{
						value = null;
					}
				}
				else if (propertyInfo != null)
				{
					if (propertyInfo.PropertyType.Equals(typeof(string)))
					{
						value = propertyInfo.GetValue(null, null) as string;
					}
					else
					{
						value = null;
					}
				}
				else if (methodInfo == null)
				{
					value = string.Empty;
				}
				else if (methodInfo.ReturnType.Equals(typeof(string)))
				{
					value = methodInfo.Invoke(null, null) as string;
				}
				else
				{
					value = null;
				}
			}
			catch
			{
				value = string.Empty;
			}
			return value;
		}

		private static Type GetTypeInfo(string path, ref string moreInfo)
		{
			Type type;
			int num = path.LastIndexOf('.');
			if (num < 0)
			{
				return null;
			}
			string str = path.Substring(0, num);
			moreInfo = path.Substring(num + 1);
			moreInfo = moreInfo.Replace("(", string.Empty).Replace(")", string.Empty);
			List<Assembly>.Enumerator enumerator = HtmlRenderer.References.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Type type1 = enumerator.Current.GetType(str, false, true);
					if (type1 == null)
					{
						continue;
					}
					type = type1;
					return type;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return type;
		}

		public static void GoLink(string href)
		{
			object obj = CssValue.DetectSource(href);
			FileInfo fileInfo = obj as FileInfo;
			MethodInfo methodInfo = obj as MethodInfo;
			Uri uri = obj as Uri;
			try
			{
				if (fileInfo != null || uri != null)
				{
					ProcessStartInfo processStartInfo = new ProcessStartInfo(href)
					{
						UseShellExecute = true
					};
					Process.Start(processStartInfo);
				}
				else if (methodInfo != null)
				{
					methodInfo.Invoke(null, null);
				}
			}
			catch
			{
				throw;
			}
		}

		public static float ParseLength(string length, float hundredPercent, CssBox box)
		{
			return CssValue.ParseLength(length, hundredPercent, box, box.GetEmHeight(), false);
		}

		public static float ParseLength(string length, float hundredPercent, CssBox box, float emFactor, bool returnPoints)
		{
			if (string.IsNullOrEmpty(length) || length == "0")
			{
				return 0f;
			}
			if (length.EndsWith("%"))
			{
				return CssValue.ParseNumber(length, hundredPercent);
			}
			if (length.Length < 3)
			{
				return 0f;
			}
			string str = length.Substring(length.Length - 2, 2);
			float single = 1f;
			string str1 = length.Substring(0, length.Length - 2);
			string str2 = str;
			string str3 = str2;
			if (str2 != null)
			{
				switch (str3)
				{
					case "em":
					{
						single = emFactor;
						break;
					}
					case "px":
					{
						single = 1f;
						break;
					}
					case "mm":
					{
						single = 3f;
						break;
					}
					case "cm":
					{
						single = 37f;
						break;
					}
					case "in":
					{
						single = 96f;
						break;
					}
					case "pt":
					{
						single = 1.33333337f;
						if (!returnPoints)
						{
							break;
						}
						return CssValue.ParseNumber(str1, hundredPercent);
					}
					case "pc":
					{
						single = 16f;
						break;
					}
					default:
					{
						single = 0f;
						return single * CssValue.ParseNumber(str1, hundredPercent);
					}
				}
			}
			else
			{
				single = 0f;
				return single * CssValue.ParseNumber(str1, hundredPercent);
			}
			return single * CssValue.ParseNumber(str1, hundredPercent);
		}

		public static float ParseNumber(string number, float hundredPercent)
		{
			if (string.IsNullOrEmpty(number))
			{
				return 0f;
			}
			string str = number;
			bool flag = number.EndsWith("%");
			float single = 0f;
			if (flag)
			{
				str = number.Substring(0, number.Length - 1);
			}
			if (!float.TryParse(str, NumberStyles.Number, (IFormatProvider)NumberFormatInfo.InvariantInfo, out single))
			{
				return 0f;
			}
			if (flag)
			{
				single = single / 100f * hundredPercent;
			}
			return single;
		}

		public static string[] SplitValues(string value)
		{
			return CssValue.SplitValues(value, ' ');
		}

		public static string[] SplitValues(string value, char separator)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new string[0];
			}
			string[] strArrays = value.Split(new char[] { separator });
			List<string> strs = new List<string>();
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i].Trim();
				if (!string.IsNullOrEmpty(str))
				{
					strs.Add(str);
				}
			}
			return strs.ToArray();
		}
	}
}