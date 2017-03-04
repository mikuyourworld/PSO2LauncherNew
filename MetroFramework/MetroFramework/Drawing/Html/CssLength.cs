using System;
using System.Globalization;

namespace MetroFramework.Drawing.Html
{
	public class CssLength
	{
		private float _number;

		private bool _isRelative;

		private CssLength.CssUnit _unit;

		private string _length;

		private bool _isPercentage;

		private bool _hasError;

		public bool HasError
		{
			get
			{
				return this._hasError;
			}
		}

		public bool IsPercentage
		{
			get
			{
				return this._isPercentage;
			}
		}

		public bool IsRelative
		{
			get
			{
				return this._isRelative;
			}
		}

		public string Length
		{
			get
			{
				return this._length;
			}
		}

		public float Number
		{
			get
			{
				return this._number;
			}
		}

		public CssLength.CssUnit Unit
		{
			get
			{
				return this._unit;
			}
		}

		public CssLength(string length)
		{
			this._length = length;
			this._number = 0f;
			this._unit = CssLength.CssUnit.None;
			this._isPercentage = false;
			if (string.IsNullOrEmpty(length) || length == "0")
			{
				return;
			}
			if (length.EndsWith("%"))
			{
				this._number = CssValue.ParseNumber(length, 1f);
				this._isPercentage = true;
				return;
			}
			if (length.Length < 3)
			{
				float.TryParse(length, out this._number);
				this._hasError = true;
				return;
			}
			string str = length.Substring(length.Length - 2, 2);
			string str1 = length.Substring(0, length.Length - 2);
			string str2 = str;
			string str3 = str2;
			if (str2 != null)
			{
				switch (str3)
				{
					case "em":
					{
						this._unit = CssLength.CssUnit.Ems;
						this._isRelative = true;
						break;
					}
					case "ex":
					{
						this._unit = CssLength.CssUnit.Ex;
						this._isRelative = true;
						break;
					}
					case "px":
					{
						this._unit = CssLength.CssUnit.Pixels;
						this._isRelative = true;
						break;
					}
					case "mm":
					{
						this._unit = CssLength.CssUnit.Milimeters;
						break;
					}
					case "cm":
					{
						this._unit = CssLength.CssUnit.Centimeters;
						break;
					}
					case "in":
					{
						this._unit = CssLength.CssUnit.Inches;
						break;
					}
					case "pt":
					{
						this._unit = CssLength.CssUnit.Points;
						break;
					}
					case "pc":
					{
						this._unit = CssLength.CssUnit.Picas;
						break;
					}
					default:
					{
						this._hasError = true;
						return;
					}
				}
				if (!float.TryParse(str1, NumberStyles.Number, (IFormatProvider)NumberFormatInfo.InvariantInfo, out this._number))
				{
					this._hasError = true;
				}
				return;
			}
			this._hasError = true;
		}

		public CssLength ConvertEmToPixels(float pixelFactor)
		{
			if (this.HasError)
			{
				throw new InvalidOperationException("Invalid length");
			}
			if (this.Unit != CssLength.CssUnit.Ems)
			{
				throw new InvalidOperationException("Length is not in ems");
			}
			float single = Convert.ToSingle(this.Number * pixelFactor);
			return new CssLength(string.Format("{0}px", single.ToString("0.0", NumberFormatInfo.InvariantInfo)));
		}

		public CssLength ConvertEmToPoints(float emSize)
		{
			if (this.HasError)
			{
				throw new InvalidOperationException("Invalid length");
			}
			if (this.Unit != CssLength.CssUnit.Ems)
			{
				throw new InvalidOperationException("Length is not in ems");
			}
			float single = Convert.ToSingle(this.Number * emSize);
			return new CssLength(string.Format("{0}pt", single.ToString("0.0", NumberFormatInfo.InvariantInfo)));
		}

		public override string ToString()
		{
			object[] number;
			NumberFormatInfo invariantInfo;
			if (this.HasError)
			{
				return string.Empty;
			}
			if (this.IsPercentage)
			{
				NumberFormatInfo numberFormatInfo = NumberFormatInfo.InvariantInfo;
				object[] objArray = new object[] { this.Number };
				return string.Format(numberFormatInfo, "{0}%", objArray);
			}
			string empty = string.Empty;
			switch (this.Unit)
			{
				case CssLength.CssUnit.None:
				{
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				case CssLength.CssUnit.Ems:
				{
					empty = "em";
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				case CssLength.CssUnit.Pixels:
				{
					empty = "px";
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				case CssLength.CssUnit.Ex:
				{
					empty = "ex";
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				case CssLength.CssUnit.Inches:
				{
					empty = "in";
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				case CssLength.CssUnit.Centimeters:
				{
					empty = "cm";
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				case CssLength.CssUnit.Milimeters:
				{
					empty = "mm";
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				case CssLength.CssUnit.Points:
				{
					empty = "pt";
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				case CssLength.CssUnit.Picas:
				{
					empty = "pc";
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
				default:
				{
					invariantInfo = NumberFormatInfo.InvariantInfo;
					number = new object[] { this.Number, empty };
					return string.Format(invariantInfo, "{0}{1}", number);
				}
			}
		}

		public enum CssUnit
		{
			None,
			Ems,
			Pixels,
			Ex,
			Inches,
			Centimeters,
			Milimeters,
			Points,
			Picas
		}
	}
}