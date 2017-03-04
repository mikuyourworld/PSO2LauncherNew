using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MetroFramework.Drawing.Html
{
	[CLSCompliant(false)]
	public class CssBox
	{
		internal readonly static CssBox Empty;

		internal static Dictionary<string, PropertyInfo> _properties;

		private static Dictionary<string, string> _defaults;

		private static List<PropertyInfo> _inheritables;

		private static List<PropertyInfo> _cssproperties;

		private string _backgroundColor;

		private string _backgroundGradient;

		private string _backgroundGradientAngle;

		private string _BackgroundImage;

		private string _backgroundRepeat;

		private string _borderTopWidth;

		private string _borderRightWidth;

		private string _borderBottomWidth;

		private string _borderLeftWidth;

		private string _borderWidth;

		private string _borderTopColor;

		private string _borderRightColor;

		private string _borderBottomColor;

		private string _borderLeftColor;

		private string _borderColor;

		private string _borderTopStyle;

		private string _borderRightStyle;

		private string _borderBottomStyle;

		private string _borderLeftStyle;

		private string _borderStyle;

		private string _borderBottom;

		private string _borderLeft;

		private string _borderRight;

		private string _borderTop;

		private string _borderSpacing;

		private string _borderCollapse;

		private string _border;

		private string _color;

		private string _cornerNWRadius;

		private string _cornerNERadius;

		private string _cornerSERadius;

		private string _cornerSWRadius;

		private string _cornerRadius;

		private string _emptyCells;

		private string _direction;

		private string _display;

		private string _font;

		private string _fontFamily;

		private string _fontSize;

		private string _fontStyle;

		private string _fontVariant;

		private string _fontWeight;

		private string _float;

		private string _height;

		private string _marginBottom;

		private string _marginLeft;

		private string _marginRight;

		private string _marginTop;

		private string _margin;

		private string _left;

		private string _lineHeight;

		private string _listStyleType;

		private string _listStyleImage;

		private string _listStylePosition;

		private string _listStyle;

		private string _paddingLeft;

		private string _paddingBottom;

		private string _paddingRight;

		private string _paddingTop;

		private string _padding;

		private string _text;

		private string _textAlign;

		private string _textDecoration;

		private string _textIndent;

		private string _top;

		private string _position;

		private string _verticalAlign;

		private string _width;

		private string _wordSpacing;

		private string _whiteSpace;

		internal bool TableFixed;

		private List<CssBoxWord> _boxWords;

		private List<CssBox> _boxes;

		private CssBox _parentBox;

		private bool _wordsSizeMeasured;

		private SizeF _size;

		private PointF _location;

		private List<CssLineBox> _lineBoxes;

		private List<CssLineBox> _parentLineBoxes;

		private float _fontAscent = float.NaN;

		private float _fontDescent = float.NaN;

		private float _fontLineSpacing = float.NaN;

		private HtmlTag _htmltag;

		private Dictionary<CssLineBox, RectangleF> _rectangles;

		protected InitialContainer _initialContainer;

		private CssBox _listItemBox;

		private CssLineBox _firstHostingLineBox;

		private CssLineBox _lastHostingLineBox;

		private float _actualCornerNW = float.NaN;

		private float _actualCornerNE = float.NaN;

		private float _actualCornerSW = float.NaN;

		private float _actualCornerSE = float.NaN;

		private System.Drawing.Color _actualColor = System.Drawing.Color.Empty;

		private float _actualBackgroundGradientAngle = float.NaN;

		private float _actualPaddingTop = float.NaN;

		private float _actualPaddingBottom = float.NaN;

		private float _actualPaddingRight = float.NaN;

		private float _actualPaddingLeft = float.NaN;

		private float _actualMarginTop = float.NaN;

		private float _actualMarginBottom = float.NaN;

		private float _actualMarginRight = float.NaN;

		private float _actualMarginLeft = float.NaN;

		private float _actualBorderTopWidth = float.NaN;

		private float _actualBorderLeftWidth = float.NaN;

		private float _actualBorderBottomWidth = float.NaN;

		private float _actualBorderRightWidth = float.NaN;

		private System.Drawing.Color _actualBackgroundGradient = System.Drawing.Color.Empty;

		private System.Drawing.Color _actualBorderTopColor = System.Drawing.Color.Empty;

		private System.Drawing.Color _actualBorderLeftColor = System.Drawing.Color.Empty;

		private System.Drawing.Color _actualBorderBottomColor = System.Drawing.Color.Empty;

		private System.Drawing.Color _actualBorderRightColor = System.Drawing.Color.Empty;

		private float _actualWordSpacing = float.NaN;

		private System.Drawing.Color _actualBackgroundColor = System.Drawing.Color.Empty;

		private System.Drawing.Font _actualFont;

		private float _actualTextIndent = float.NaN;

		private float _actualBorderSpacingHorizontal = float.NaN;

		private float _actualBorderSpacingVertical = float.NaN;

		public System.Drawing.Color ActualBackgroundColor
		{
			get
			{
				if (this._actualBackgroundColor.IsEmpty)
				{
					this._actualBackgroundColor = CssValue.GetActualColor(this.BackgroundColor);
				}
				return this._actualBackgroundColor;
			}
		}

		public System.Drawing.Color ActualBackgroundGradient
		{
			get
			{
				if (this._actualBackgroundGradient.IsEmpty)
				{
					this._actualBackgroundGradient = CssValue.GetActualColor(this.BackgroundGradient);
				}
				return this._actualBackgroundGradient;
			}
		}

		public float ActualBackgroundGradientAngle
		{
			get
			{
				if (float.IsNaN(this._actualBackgroundGradientAngle))
				{
					this._actualBackgroundGradientAngle = CssValue.ParseNumber(this.BackgroundGradientAngle, 360f);
				}
				return this._actualBackgroundGradientAngle;
			}
		}

		public System.Drawing.Color ActualBorderBottomColor
		{
			get
			{
				if (this._actualBorderBottomColor.IsEmpty)
				{
					this._actualBorderBottomColor = CssValue.GetActualColor(this.BorderBottomColor);
				}
				return this._actualBorderBottomColor;
			}
		}

		public float ActualBorderBottomWidth
		{
			get
			{
				if (float.IsNaN(this._actualBorderBottomWidth))
				{
					this._actualBorderBottomWidth = CssValue.GetActualBorderWidth(this.BorderBottomWidth, this);
					if (string.IsNullOrEmpty(this.BorderBottomStyle) || this.BorderBottomStyle == "none")
					{
						this._actualBorderBottomWidth = 0f;
					}
				}
				return this._actualBorderBottomWidth;
			}
		}

		public System.Drawing.Color ActualBorderLeftColor
		{
			get
			{
				if (this._actualBorderLeftColor.IsEmpty)
				{
					this._actualBorderLeftColor = CssValue.GetActualColor(this.BorderLeftColor);
				}
				return this._actualBorderLeftColor;
			}
		}

		public float ActualBorderLeftWidth
		{
			get
			{
				if (float.IsNaN(this._actualBorderLeftWidth))
				{
					this._actualBorderLeftWidth = CssValue.GetActualBorderWidth(this.BorderLeftWidth, this);
					if (string.IsNullOrEmpty(this.BorderLeftStyle) || this.BorderLeftStyle == "none")
					{
						this._actualBorderLeftWidth = 0f;
					}
				}
				return this._actualBorderLeftWidth;
			}
		}

		public System.Drawing.Color ActualBorderRightColor
		{
			get
			{
				if (this._actualBorderRightColor.IsEmpty)
				{
					this._actualBorderRightColor = CssValue.GetActualColor(this.BorderRightColor);
				}
				return this._actualBorderRightColor;
			}
		}

		public float ActualBorderRightWidth
		{
			get
			{
				if (float.IsNaN(this._actualBorderRightWidth))
				{
					this._actualBorderRightWidth = CssValue.GetActualBorderWidth(this.BorderRightWidth, this);
					if (string.IsNullOrEmpty(this.BorderRightStyle) || this.BorderRightStyle == "none")
					{
						this._actualBorderRightWidth = 0f;
					}
				}
				return this._actualBorderRightWidth;
			}
		}

		public float ActualBorderSpacingHorizontal
		{
			get
			{
				if (float.IsNaN(this._actualBorderSpacingHorizontal))
				{
					MatchCollection matchCollections = Parser.Match("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", this.BorderSpacing);
					if (matchCollections.Count == 0)
					{
						this._actualBorderSpacingHorizontal = 0f;
					}
					else if (matchCollections.Count > 0)
					{
						this._actualBorderSpacingHorizontal = CssValue.ParseLength(matchCollections[0].Value, 1f, this);
					}
				}
				return this._actualBorderSpacingHorizontal;
			}
		}

		public float ActualBorderSpacingVertical
		{
			get
			{
				if (float.IsNaN(this._actualBorderSpacingVertical))
				{
					MatchCollection matchCollections = Parser.Match("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", this.BorderSpacing);
					if (matchCollections.Count == 0)
					{
						this._actualBorderSpacingVertical = 0f;
					}
					else if (matchCollections.Count != 1)
					{
						this._actualBorderSpacingVertical = CssValue.ParseLength(matchCollections[1].Value, 1f, this);
					}
					else
					{
						this._actualBorderSpacingVertical = CssValue.ParseLength(matchCollections[0].Value, 1f, this);
					}
				}
				return this._actualBorderSpacingVertical;
			}
		}

		public System.Drawing.Color ActualBorderTopColor
		{
			get
			{
				if (this._actualBorderTopColor.IsEmpty)
				{
					this._actualBorderTopColor = CssValue.GetActualColor(this.BorderTopColor);
				}
				return this._actualBorderTopColor;
			}
		}

		public float ActualBorderTopWidth
		{
			get
			{
				if (float.IsNaN(this._actualBorderTopWidth))
				{
					this._actualBorderTopWidth = CssValue.GetActualBorderWidth(this.BorderTopWidth, this);
					if (string.IsNullOrEmpty(this.BorderTopStyle) || this.BorderTopStyle == "none")
					{
						this._actualBorderTopWidth = 0f;
					}
				}
				return this._actualBorderTopWidth;
			}
		}

		public float ActualBottom
		{
			get
			{
				return this.Location.Y + this.Size.Height;
			}
			set
			{
				float width = this.Size.Width;
				PointF location = this.Location;
				this.Size = new SizeF(width, value - location.Y);
			}
		}

		public System.Drawing.Color ActualColor
		{
			get
			{
				if (this._actualColor.IsEmpty)
				{
					this._actualColor = CssValue.GetActualColor(this.Color);
				}
				return this._actualColor;
			}
		}

		public float ActualCornerNE
		{
			get
			{
				if (float.IsNaN(this._actualCornerNE))
				{
					this._actualCornerNE = CssValue.ParseLength(this.CornerNERadius, 0f, this);
				}
				return this._actualCornerNE;
			}
		}

		public float ActualCornerNW
		{
			get
			{
				if (float.IsNaN(this._actualCornerNW))
				{
					this._actualCornerNW = CssValue.ParseLength(this.CornerNWRadius, 0f, this);
				}
				return this._actualCornerNW;
			}
		}

		public float ActualCornerSE
		{
			get
			{
				if (float.IsNaN(this._actualCornerSE))
				{
					this._actualCornerSE = CssValue.ParseLength(this.CornerSERadius, 0f, this);
				}
				return this._actualCornerSE;
			}
		}

		public float ActualCornerSW
		{
			get
			{
				if (float.IsNaN(this._actualCornerSW))
				{
					this._actualCornerSW = CssValue.ParseLength(this.CornerSWRadius, 0f, this);
				}
				return this._actualCornerSW;
			}
		}

		public System.Drawing.Font ActualFont
		{
			get
			{
				float fontSize;
				float size;
				if (this._actualFont == null)
				{
					if (string.IsNullOrEmpty(this.FontFamily))
                        this.FontFamily = CssDefaults.FontSerif;
                    if (string.IsNullOrEmpty(this.FontSize))
                        this.FontSize = string.Concat(CssDefaults.FontSize, "pt");
                    System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
					if (this.FontStyle == "italic" || this.FontStyle == "oblique")
                        fontStyle = fontStyle | System.Drawing.FontStyle.Italic;
                    if (this.FontWeight != "normal" && this.FontWeight != "lighter" && !string.IsNullOrEmpty(this.FontWeight))
                        fontStyle = fontStyle | System.Drawing.FontStyle.Bold;
                    fontSize = 0f;
					size = CssDefaults.FontSize;
					if (this.ParentBox != null)
                        size = this.ParentBox.ActualFont.Size;
                    string str = this.FontSize;
					string str1 = str;
                    if (str != null)
                        switch (str1)
                        {
                            case "medium":
                                fontSize = CssDefaults.FontSize;
                                break;
                            case "xx-small":
                                fontSize = CssDefaults.FontSize - 4f;
                                break;
                            case "x-small":
                                fontSize = CssDefaults.FontSize - 3f;
                                break;
                            case "small":
                                fontSize = CssDefaults.FontSize - 2f;
                                break;
                            case "large":
                                fontSize = CssDefaults.FontSize + 2f;
                                break;
                            case "x-large":
                                fontSize = CssDefaults.FontSize + 3f;
                                break;
                            case "xx-large":
                                fontSize = CssDefaults.FontSize + 4f;
                                break;
                            case "smaller":
                                fontSize = size - 2f;
                                break;
                            case "larger":
                                fontSize = size + 2f;
                                break;
                            default:
                                fontSize = CssValue.ParseLength(this.FontSize, size, this, size, true);
                                break;
                        }
                    else
                        fontSize = CssValue.ParseLength(this.FontSize, size, this, size, true);
                    //_Label1:
                    if (fontSize <= 1f)
                        fontSize = CssDefaults.FontSize;
                    this._actualFont = new System.Drawing.Font(this.FontFamily, fontSize, fontStyle);
				}
				return this._actualFont;
                //Label0:
                //fontSize = CssValue.ParseLength(this.FontSize, size, this, size, true);
                //goto _Label1;
			}
		}

		public float ActualMarginBottom
		{
			get
			{
				if (float.IsNaN(this._actualMarginBottom))
				{
					if (this.MarginBottom == "auto")
					{
						this.MarginBottom = "0";
					}
					string marginBottom = this.MarginBottom;
					SizeF size = this.Size;
					this._actualMarginBottom = CssValue.ParseLength(marginBottom, size.Width, this);
				}
				return this._actualMarginBottom;
			}
		}

		public float ActualMarginLeft
		{
			get
			{
				if (float.IsNaN(this._actualMarginLeft))
				{
					if (this.MarginLeft == "auto")
					{
						this.MarginLeft = "0";
					}
					string marginLeft = this.MarginLeft;
					SizeF size = this.Size;
					this._actualMarginLeft = CssValue.ParseLength(marginLeft, size.Width, this);
				}
				return this._actualMarginLeft;
			}
		}

		public float ActualMarginRight
		{
			get
			{
				if (float.IsNaN(this._actualMarginRight))
				{
					if (this.MarginRight == "auto")
					{
						this.MarginRight = "0";
					}
					string marginRight = this.MarginRight;
					SizeF size = this.Size;
					this._actualMarginRight = CssValue.ParseLength(marginRight, size.Width, this);
				}
				return this._actualMarginRight;
			}
		}

		public float ActualMarginTop
		{
			get
			{
				if (float.IsNaN(this._actualMarginTop))
				{
					if (this.MarginTop == "auto")
					{
						this.MarginTop = "0";
					}
					string marginTop = this.MarginTop;
					SizeF size = this.Size;
					this._actualMarginTop = CssValue.ParseLength(marginTop, size.Width, this);
				}
				return this._actualMarginTop;
			}
		}

		public float ActualPaddingBottom
		{
			get
			{
				if (float.IsNaN(this._actualPaddingBottom))
				{
					string paddingBottom = this.PaddingBottom;
					SizeF size = this.Size;
					this._actualPaddingBottom = CssValue.ParseLength(paddingBottom, size.Width, this);
				}
				return this._actualPaddingBottom;
			}
		}

		public float ActualPaddingLeft
		{
			get
			{
				if (float.IsNaN(this._actualPaddingLeft))
				{
					string paddingLeft = this.PaddingLeft;
					SizeF size = this.Size;
					this._actualPaddingLeft = CssValue.ParseLength(paddingLeft, size.Width, this);
				}
				return this._actualPaddingLeft;
			}
		}

		public float ActualPaddingRight
		{
			get
			{
				if (float.IsNaN(this._actualPaddingRight))
				{
					string paddingRight = this.PaddingRight;
					SizeF size = this.Size;
					this._actualPaddingRight = CssValue.ParseLength(paddingRight, size.Width, this);
				}
				return this._actualPaddingRight;
			}
		}

		public float ActualPaddingTop
		{
			get
			{
				if (float.IsNaN(this._actualPaddingTop))
				{
					string paddingTop = this.PaddingTop;
					SizeF size = this.Size;
					this._actualPaddingTop = CssValue.ParseLength(paddingTop, size.Width, this);
				}
				return this._actualPaddingTop;
			}
		}

		public System.Drawing.Font ActualParentFont
		{
			get
			{
				if (this.ParentBox == null)
				{
					return this.ActualFont;
				}
				return this.ParentBox.ActualFont;
			}
		}

		public float ActualRight
		{
			get
			{
				return this.Location.X + this.Size.Width;
			}
			set
			{
				PointF location = this.Location;
				this.Size = new SizeF(value - location.X, this.Size.Height);
			}
		}

		public float ActualTextIndent
		{
			get
			{
				if (float.IsNaN(this._actualTextIndent))
				{
					string textIndent = this.TextIndent;
					SizeF size = this.Size;
					this._actualTextIndent = CssValue.ParseLength(textIndent, size.Width, this);
				}
				return this._actualTextIndent;
			}
		}

		public float ActualWordSpacing
		{
			get
			{
				if (float.IsNaN(this._actualWordSpacing))
				{
					throw new Exception("Space must be calculated before using this property");
				}
				return this._actualWordSpacing;
			}
		}

		public float AvailableWidth
		{
			get
			{
				SizeF size = this.Size;
				return size.Width - this.ActualBorderLeftWidth - this.ActualPaddingLeft - this.ActualPaddingRight - this.ActualBorderRightWidth;
			}
		}

		[CssProperty("background-color")]
		[DefaultValue("transparent")]
		public string BackgroundColor
		{
			get
			{
				return this._backgroundColor;
			}
			set
			{
				this._backgroundColor = value;
			}
		}

		[CssProperty("background-gradient")]
		[DefaultValue("none")]
		public string BackgroundGradient
		{
			get
			{
				return this._backgroundGradient;
			}
			set
			{
				this._backgroundGradient = value;
			}
		}

		[CssProperty("background-gradient-angle")]
		[DefaultValue("90")]
		public string BackgroundGradientAngle
		{
			get
			{
				return this._backgroundGradientAngle;
			}
			set
			{
				this._backgroundGradientAngle = value;
			}
		}

		[CssProperty("background-image")]
		[DefaultValue("none")]
		public string BackgroundImage
		{
			get
			{
				return this._BackgroundImage;
			}
			set
			{
				this._BackgroundImage = value;
			}
		}

		[CssProperty("background-repeat")]
		[DefaultValue("repeat")]
		public string BackgroundRepeat
		{
			get
			{
				return this._backgroundRepeat;
			}
			set
			{
				this._backgroundRepeat = value;
			}
		}

		[CssProperty("border")]
		[DefaultValue("")]
		public string Border
		{
			get
			{
				return this._border;
			}
			set
			{
				this._border = value;
				string str = Parser.Search("(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|thin|medium|thick)", value);
				string str1 = Parser.Search("(none|hidden|dotted|dashed|solid|double|groove|ridge|inset|outset)", value);
				string str2 = Parser.Search("(#\\S{6}|#\\S{3}|rgb\\(\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)", value);
				if (str != null)
				{
					this.BorderWidth = str;
				}
				if (str1 != null)
				{
					this.BorderStyle = str1;
				}
				if (str2 != null)
				{
					this.BorderColor = str2;
				}
			}
		}

		[CssProperty("border-bottom")]
		[DefaultValue("")]
		public string BorderBottom
		{
			get
			{
				return this._borderBottom;
			}
			set
			{
				this._borderBottom = value;
				string str = Parser.Search("(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|thin|medium|thick)", value);
				string str1 = Parser.Search("(none|hidden|dotted|dashed|solid|double|groove|ridge|inset|outset)", value);
				string str2 = Parser.Search("(#\\S{6}|#\\S{3}|rgb\\(\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)", value);
				if (str != null)
				{
					this.BorderBottomWidth = str;
				}
				if (str1 != null)
				{
					this.BorderBottomStyle = str1;
				}
				if (str2 != null)
				{
					this.BorderBottomColor = str2;
				}
			}
		}

		[CssProperty("border-bottom-color")]
		[DefaultValue("black")]
		public string BorderBottomColor
		{
			get
			{
				return this._borderBottomColor;
			}
			set
			{
				this._borderBottomColor = value;
			}
		}

		[CssProperty("border-bottom-style")]
		[DefaultValue("none")]
		public string BorderBottomStyle
		{
			get
			{
				return this._borderBottomStyle;
			}
			set
			{
				this._borderBottomStyle = value;
			}
		}

		[CssProperty("border-bottom-width")]
		[DefaultValue("medium")]
		public string BorderBottomWidth
		{
			get
			{
				return this._borderBottomWidth;
			}
			set
			{
				this._borderBottomWidth = value;
			}
		}

		[CssProperty("border-collapse")]
		[CssPropertyInherited]
		[DefaultValue("separate")]
		public string BorderCollapse
		{
			get
			{
				return this._borderCollapse;
			}
			set
			{
				this._borderCollapse = value;
			}
		}

		[CssProperty("border-color")]
		[DefaultValue("black")]
		public string BorderColor
		{
			get
			{
				return this._borderColor;
			}
			set
			{
				this._borderColor = value;
				MatchCollection matchCollections = Parser.Match("(#\\S{6}|#\\S{3}|rgb\\(\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)", value);
				string[] strArrays = new string[matchCollections.Count];
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					strArrays[i] = matchCollections[i].Value;
				}
				switch ((int)strArrays.Length)
				{
					case 1:
					{
						string str = strArrays[0];
						string str1 = str;
						this.BorderBottomColor = str;
						string str2 = str1;
						string str3 = str2;
						this.BorderRightColor = str2;
						string str4 = str3;
						string str5 = str4;
						this.BorderLeftColor = str4;
						this.BorderTopColor = str5;
						return;
					}
					case 2:
					{
						string str6 = strArrays[0];
						string str7 = str6;
						this.BorderBottomColor = str6;
						this.BorderTopColor = str7;
						string str8 = strArrays[1];
						string str9 = str8;
						this.BorderRightColor = str8;
						this.BorderLeftColor = str9;
						return;
					}
					case 3:
					{
						this.BorderTopColor = strArrays[0];
						string str10 = strArrays[1];
						string str11 = str10;
						this.BorderRightColor = str10;
						this.BorderLeftColor = str11;
						this.BorderBottomColor = strArrays[2];
						return;
					}
					case 4:
					{
						this.BorderTopColor = strArrays[0];
						this.BorderRightColor = strArrays[1];
						this.BorderBottomColor = strArrays[2];
						this.BorderLeftColor = strArrays[3];
						return;
					}
					default:
					{
						return;
					}
				}
			}
		}

		[CssProperty("border-left")]
		[DefaultValue("")]
		public string BorderLeft
		{
			get
			{
				return this._borderLeft;
			}
			set
			{
				this._borderLeft = value;
				string str = Parser.Search("(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|thin|medium|thick)", value);
				string str1 = Parser.Search("(none|hidden|dotted|dashed|solid|double|groove|ridge|inset|outset)", value);
				string str2 = Parser.Search("(#\\S{6}|#\\S{3}|rgb\\(\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)", value);
				if (str != null)
				{
					this.BorderLeftWidth = str;
				}
				if (str1 != null)
				{
					this.BorderLeftStyle = str1;
				}
				if (str2 != null)
				{
					this.BorderLeftColor = str2;
				}
			}
		}

		[CssProperty("border-left-color")]
		[DefaultValue("black")]
		public string BorderLeftColor
		{
			get
			{
				return this._borderLeftColor;
			}
			set
			{
				this._borderLeftColor = value;
			}
		}

		[CssProperty("border-left-style")]
		[DefaultValue("none")]
		public string BorderLeftStyle
		{
			get
			{
				return this._borderLeftStyle;
			}
			set
			{
				this._borderLeftStyle = value;
			}
		}

		[CssProperty("border-left-width")]
		[DefaultValue("medium")]
		public string BorderLeftWidth
		{
			get
			{
				return this._borderLeftWidth;
			}
			set
			{
				this._borderLeftWidth = value;
			}
		}

		[CssProperty("border-right")]
		[DefaultValue("")]
		public string BorderRight
		{
			get
			{
				return this._borderRight;
			}
			set
			{
				this._borderRight = value;
				string str = Parser.Search("(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|thin|medium|thick)", value);
				string str1 = Parser.Search("(none|hidden|dotted|dashed|solid|double|groove|ridge|inset|outset)", value);
				string str2 = Parser.Search("(#\\S{6}|#\\S{3}|rgb\\(\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)", value);
				if (str != null)
				{
					this.BorderRightWidth = str;
				}
				if (str1 != null)
				{
					this.BorderRightStyle = str1;
				}
				if (str2 != null)
				{
					this.BorderRightColor = str2;
				}
			}
		}

		[CssProperty("border-right-color")]
		[DefaultValue("black")]
		public string BorderRightColor
		{
			get
			{
				return this._borderRightColor;
			}
			set
			{
				this._borderRightColor = value;
			}
		}

		[CssProperty("border-right-style")]
		[DefaultValue("none")]
		public string BorderRightStyle
		{
			get
			{
				return this._borderRightStyle;
			}
			set
			{
				this._borderRightStyle = value;
			}
		}

		[CssProperty("border-right-width")]
		[DefaultValue("medium")]
		public string BorderRightWidth
		{
			get
			{
				return this._borderRightWidth;
			}
			set
			{
				this._borderRightWidth = value;
			}
		}

		[CssProperty("border-spacing")]
		[CssPropertyInherited]
		[DefaultValue("0")]
		public string BorderSpacing
		{
			get
			{
				return this._borderSpacing;
			}
			set
			{
				this._borderSpacing = value;
			}
		}

		[CssProperty("border-style")]
		[DefaultValue("")]
		public string BorderStyle
		{
			get
			{
				return this._borderStyle;
			}
			set
			{
				this._borderStyle = value;
				string[] strArrays = CssValue.SplitValues(value);
				switch ((int)strArrays.Length)
				{
					case 1:
					{
						string str = strArrays[0];
						string str1 = str;
						this.BorderBottomStyle = str;
						string str2 = str1;
						string str3 = str2;
						this.BorderRightStyle = str2;
						string str4 = str3;
						string str5 = str4;
						this.BorderLeftStyle = str4;
						this.BorderTopStyle = str5;
						return;
					}
					case 2:
					{
						string str6 = strArrays[0];
						string str7 = str6;
						this.BorderBottomStyle = str6;
						this.BorderTopStyle = str7;
						string str8 = strArrays[1];
						string str9 = str8;
						this.BorderRightStyle = str8;
						this.BorderLeftStyle = str9;
						return;
					}
					case 3:
					{
						this.BorderTopStyle = strArrays[0];
						string str10 = strArrays[1];
						string str11 = str10;
						this.BorderRightStyle = str10;
						this.BorderLeftStyle = str11;
						this.BorderBottomStyle = strArrays[2];
						return;
					}
					case 4:
					{
						this.BorderTopStyle = strArrays[0];
						this.BorderRightStyle = strArrays[1];
						this.BorderBottomStyle = strArrays[2];
						this.BorderLeftStyle = strArrays[3];
						return;
					}
					default:
					{
						return;
					}
				}
			}
		}

		[CssProperty("border-top")]
		[DefaultValue("")]
		public string BorderTop
		{
			get
			{
				return this._borderTop;
			}
			set
			{
				this._borderTop = value;
				string str = Parser.Search("(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|thin|medium|thick)", value);
				string str1 = Parser.Search("(none|hidden|dotted|dashed|solid|double|groove|ridge|inset|outset)", value);
				string str2 = Parser.Search("(#\\S{6}|#\\S{3}|rgb\\(\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)", value);
				if (str != null)
				{
					this.BorderTopWidth = str;
				}
				if (str1 != null)
				{
					this.BorderTopStyle = str1;
				}
				if (str2 != null)
				{
					this.BorderTopColor = str2;
				}
			}
		}

		[CssProperty("border-top-color")]
		[DefaultValue("black")]
		public string BorderTopColor
		{
			get
			{
				return this._borderTopColor;
			}
			set
			{
				this._borderTopColor = value;
			}
		}

		[CssProperty("border-top-style")]
		[DefaultValue("none")]
		public string BorderTopStyle
		{
			get
			{
				return this._borderTopStyle;
			}
			set
			{
				this._borderTopStyle = value;
			}
		}

		[CssProperty("border-top-width")]
		[DefaultValue("medium")]
		public string BorderTopWidth
		{
			get
			{
				return this._borderTopWidth;
			}
			set
			{
				this._borderTopWidth = value;
			}
		}

		[CssProperty("border-width")]
		[DefaultValue("")]
		public string BorderWidth
		{
			get
			{
				return this._borderWidth;
			}
			set
			{
				this._borderWidth = value;
				string[] strArrays = CssValue.SplitValues(value);
				switch ((int)strArrays.Length)
				{
					case 1:
					{
						string str = strArrays[0];
						string str1 = str;
						this.BorderBottomWidth = str;
						string str2 = str1;
						string str3 = str2;
						this.BorderRightWidth = str2;
						string str4 = str3;
						string str5 = str4;
						this.BorderLeftWidth = str4;
						this.BorderTopWidth = str5;
						return;
					}
					case 2:
					{
						string str6 = strArrays[0];
						string str7 = str6;
						this.BorderBottomWidth = str6;
						this.BorderTopWidth = str7;
						string str8 = strArrays[1];
						string str9 = str8;
						this.BorderRightWidth = str8;
						this.BorderLeftWidth = str9;
						return;
					}
					case 3:
					{
						this.BorderTopWidth = strArrays[0];
						string str10 = strArrays[1];
						string str11 = str10;
						this.BorderRightWidth = str10;
						this.BorderLeftWidth = str11;
						this.BorderBottomWidth = strArrays[2];
						return;
					}
					case 4:
					{
						this.BorderTopWidth = strArrays[0];
						this.BorderRightWidth = strArrays[1];
						this.BorderBottomWidth = strArrays[2];
						this.BorderLeftWidth = strArrays[3];
						return;
					}
					default:
					{
						return;
					}
				}
			}
		}

		public RectangleF Bounds
		{
			get
			{
				return new RectangleF(this.Location, this.Size);
			}
		}

		public List<CssBox> Boxes
		{
			get
			{
				return this._boxes;
			}
		}

		public float ClientBottom
		{
			get
			{
				return this.ActualBottom - this.ActualPaddingBottom - this.ActualBorderBottomWidth;
			}
		}

		public float ClientLeft
		{
			get
			{
				PointF location = this.Location;
				return location.X + this.ActualBorderLeftWidth + this.ActualPaddingLeft;
			}
		}

		public RectangleF ClientRectangle
		{
			get
			{
				return RectangleF.FromLTRB(this.ClientLeft, this.ClientTop, this.ClientRight, this.ClientBottom);
			}
		}

		public float ClientRight
		{
			get
			{
				return this.ActualRight - this.ActualPaddingRight - this.ActualBorderRightWidth;
			}
		}

		public float ClientTop
		{
			get
			{
				PointF location = this.Location;
				return location.Y + this.ActualBorderTopWidth + this.ActualPaddingTop;
			}
		}

		[CssProperty("color")]
		[CssPropertyInherited]
		[DefaultValue("black")]
		public string Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
				this._actualColor = System.Drawing.Color.Empty;
			}
		}

		public CssBox ContainingBlock
		{
			get
			{
				if (this.ParentBox == null)
				{
					return this;
				}
				CssBox parentBox = this.ParentBox;
				while (parentBox.Display != "block" && parentBox.Display != "table" && parentBox.Display != "table-cell" && parentBox.ParentBox != null)
				{
					parentBox = parentBox.ParentBox;
				}
				if (parentBox == null)
				{
					throw new Exception("There's no containing block on the chain");
				}
				return parentBox;
			}
		}

		[CssProperty("corner-ne-radius")]
		[DefaultValue("0")]
		public string CornerNERadius
		{
			get
			{
				return this._cornerNERadius;
			}
			set
			{
				this._cornerNERadius = value;
			}
		}

		[CssProperty("corner-nw-radius")]
		[DefaultValue("0")]
		public string CornerNWRadius
		{
			get
			{
				return this._cornerNWRadius;
			}
			set
			{
				this._cornerNWRadius = value;
			}
		}

		[CssProperty("corner-radius")]
		[DefaultValue("0")]
		public string CornerRadius
		{
			get
			{
				return this._cornerRadius;
			}
			set
			{
				MatchCollection matchCollections = Parser.Match("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", value);
				switch (matchCollections.Count)
				{
					case 1:
					{
						this.CornerNERadius = matchCollections[0].Value;
						this.CornerNWRadius = matchCollections[0].Value;
						this.CornerSERadius = matchCollections[0].Value;
						this.CornerSWRadius = matchCollections[0].Value;
						break;
					}
					case 2:
					{
						this.CornerNERadius = matchCollections[0].Value;
						this.CornerNWRadius = matchCollections[0].Value;
						this.CornerSERadius = matchCollections[1].Value;
						this.CornerSWRadius = matchCollections[1].Value;
						break;
					}
					case 3:
					{
						this.CornerNERadius = matchCollections[0].Value;
						this.CornerNWRadius = matchCollections[1].Value;
						this.CornerSERadius = matchCollections[2].Value;
						break;
					}
					case 4:
					{
						this.CornerNERadius = matchCollections[0].Value;
						this.CornerNWRadius = matchCollections[1].Value;
						this.CornerSERadius = matchCollections[2].Value;
						this.CornerSWRadius = matchCollections[3].Value;
						break;
					}
				}
				this._cornerRadius = value;
			}
		}

		[CssProperty("corner-se-radius")]
		[DefaultValue("0")]
		public string CornerSERadius
		{
			get
			{
				return this._cornerSERadius;
			}
			set
			{
				this._cornerSERadius = value;
			}
		}

		[CssProperty("corner-sw-radius")]
		[DefaultValue("0")]
		public string CornerSWRadius
		{
			get
			{
				return this._cornerSWRadius;
			}
			set
			{
				this._cornerSWRadius = value;
			}
		}

		[CssProperty("direction")]
		[DefaultValue("ltr")]
		public string Direction
		{
			get
			{
				return this._direction;
			}
			set
			{
				this._direction = value;
			}
		}

		[CssProperty("display")]
		[DefaultValue("inline")]
		public string Display
		{
			get
			{
				return this._display;
			}
			set
			{
				this._display = value;
			}
		}

		[CssProperty("empty-cells")]
		[CssPropertyInherited]
		[DefaultValue("show")]
		public string EmptyCells
		{
			get
			{
				return this._emptyCells;
			}
			set
			{
				this._emptyCells = value;
			}
		}

		internal CssLineBox FirstHostingLineBox
		{
			get
			{
				return this._firstHostingLineBox;
			}
			set
			{
				this._firstHostingLineBox = value;
			}
		}

		internal CssBoxWord FirstWord
		{
			get
			{
				return this.Words[0];
			}
		}

		[CssProperty("float")]
		[DefaultValue("none")]
		public string Float
		{
			get
			{
				return this._float;
			}
			set
			{
				this._float = value;
			}
		}

		[CssProperty("font")]
		[CssPropertyInherited]
		[DefaultValue("")]
		public string Font
		{
			get
			{
				return this._font;
			}
			set
			{
				int num;
				this._font = value;
				string str = Parser.Search("(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)(\\/(normal|{[0-9]+|[0-9]*\\.[0-9]+}|([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%))?(\\s|$)", value, out num);
				if (!string.IsNullOrEmpty(str))
				{
					str = str.Trim();
					string str1 = value.Substring(0, num);
					string str2 = Parser.Search("(normal|italic|oblique)", str1);
					string str3 = Parser.Search("(normal|small-caps)", str1);
					string str4 = Parser.Search("(normal|bold|bolder|lighter|100|200|300|400|500|600|700|800|900)", str1);
					string str5 = value.Substring(num + str.Length);
					string str6 = str5.Trim();
					string str7 = str;
					string empty = string.Empty;
					if (str.Contains("/") && str.Length > str.IndexOf("/") + 1)
					{
						int num1 = str.IndexOf("/");
						str7 = str.Substring(0, num1);
						empty = str.Substring(num1 + 1);
					}
					if (!string.IsNullOrEmpty(str2))
					{
						this.FontStyle = str2;
					}
					if (!string.IsNullOrEmpty(str3))
					{
						this.FontVariant = str3;
					}
					if (!string.IsNullOrEmpty(str4))
					{
						this.FontWeight = str4;
					}
					if (!string.IsNullOrEmpty(str6))
					{
						this.FontFamily = str6;
					}
					if (!string.IsNullOrEmpty(str7))
					{
						this.FontSize = str7;
					}
					if (!string.IsNullOrEmpty(empty))
					{
						this.LineHeight = empty;
					}
				}
			}
		}

		public float FontAscent
		{
			get
			{
				if (float.IsNaN(this._fontAscent))
				{
					this._fontAscent = CssLayoutEngine.GetAscent(this.ActualFont);
				}
				return this._fontAscent;
			}
		}

		public float FontDescent
		{
			get
			{
				if (float.IsNaN(this._fontDescent))
				{
					this._fontDescent = CssLayoutEngine.GetDescent(this.ActualFont);
				}
				return this._fontDescent;
			}
		}

		[CssProperty("font-family")]
		[CssPropertyInherited]
		[DefaultValue("serif")]
		public string FontFamily
		{
			get
			{
				return this._fontFamily;
			}
			set
			{
				string str = value;
				string str1 = str;
				if (str != null)
				{
					if (str1 == "serif")
					{
						this._fontFamily = CssDefaults.FontSerif;
						return;
					}
					if (str1 == "sans-serif")
					{
						this._fontFamily = CssDefaults.FontSansSerif;
						return;
					}
					if (str1 == "cursive")
					{
						this._fontFamily = CssDefaults.FontCursive;
						return;
					}
					if (str1 == "fantasy")
					{
						this._fontFamily = CssDefaults.FontFantasy;
						return;
					}
					if (str1 == "monospace")
					{
						this._fontFamily = CssDefaults.FontMonospace;
						return;
					}
				}
				this._fontFamily = value;
			}
		}

		public float FontLineSpacing
		{
			get
			{
				if (float.IsNaN(this._fontLineSpacing))
				{
					this._fontLineSpacing = CssLayoutEngine.GetLineSpacing(this.ActualFont);
				}
				return this._fontLineSpacing;
			}
		}

		[CssProperty("font-size")]
		[CssPropertyInherited]
		[DefaultValue("medium")]
		public string FontSize
		{
			get
			{
				return this._fontSize;
			}
			set
			{
				string str = Parser.Search("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", value);
				if (str == null)
				{
					this._fontSize = value;
					return;
				}
				string empty = string.Empty;
				CssLength cssLength = new CssLength(str);
				if (!cssLength.HasError)
				{
					empty = (cssLength.Unit != CssLength.CssUnit.Ems || this.ParentBox == null ? cssLength.ToString() : cssLength.ConvertEmToPoints(this.ParentBox.ActualFont.SizeInPoints).ToString());
				}
				else
				{
					empty = CssBox._defaults["font-size"];
				}
				this._fontSize = empty;
			}
		}

		[CssProperty("font-style")]
		[CssPropertyInherited]
		[DefaultValue("normal")]
		public string FontStyle
		{
			get
			{
				return this._fontStyle;
			}
			set
			{
				this._fontStyle = value;
			}
		}

		[CssProperty("font-variant")]
		[CssPropertyInherited]
		[DefaultValue("normal")]
		public string FontVariant
		{
			get
			{
				return this._fontVariant;
			}
			set
			{
				this._fontVariant = value;
			}
		}

		[CssProperty("font-weight")]
		[CssPropertyInherited]
		[DefaultValue("normal")]
		public string FontWeight
		{
			get
			{
				return this._fontWeight;
			}
			set
			{
				this._fontWeight = value;
			}
		}

		[CssProperty("height")]
		[DefaultValue("auto")]
		public string Height
		{
			get
			{
				return this._height;
			}
			set
			{
				this._height = value;
			}
		}

		public HtmlTag HtmlTag
		{
			get
			{
				return this._htmltag;
			}
		}

		public InitialContainer InitialContainer
		{
			get
			{
				return this._initialContainer;
			}
		}

		public bool IsImage
		{
			get
			{
				if (this.Words.Count != 1)
				{
					return false;
				}
				return this.Words[0].IsImage;
			}
		}

		public bool IsRounded
		{
			get
			{
				if (this.ActualCornerNE > 0f || this.ActualCornerNW > 0f || this.ActualCornerSE > 0f)
				{
					return true;
				}
				return this.ActualCornerSW > 0f;
			}
		}

		public bool IsSpaceOrEmpty
		{
			get
			{
				bool flag;
				if (this.Words.Count == 0 && this.Boxes.Count == 0 || this.Words.Count == 1 && this.Words[0].IsSpaces || this.Boxes.Count == 1 && this.Boxes[0] is CssAnonymousSpaceBlockBox)
				{
					return true;
				}
				List<CssBoxWord>.Enumerator enumerator = this.Words.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsSpaces)
						{
							continue;
						}
						flag = false;
						return flag;
					}
					return true;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
		}

		internal CssLineBox LastHostingLineBox
		{
			get
			{
				return this._lastHostingLineBox;
			}
			set
			{
				this._lastHostingLineBox = value;
			}
		}

		internal CssBoxWord LastWord
		{
			get
			{
				return this.Words[this.Words.Count - 1];
			}
		}

		[CssProperty("left")]
		[DefaultValue("auto")]
		public string Left
		{
			get
			{
				return this._left;
			}
			set
			{
				this._left = value;
			}
		}

		internal List<CssLineBox> LineBoxes
		{
			get
			{
				return this._lineBoxes;
			}
		}

		[CssProperty("line-height")]
		[DefaultValue("normal")]
		public string LineHeight
		{
			get
			{
				return this._lineHeight;
			}
			set
			{
				this._lineHeight = this.NoEms(value);
			}
		}

		public CssBox ListItemBox
		{
			get
			{
				return this._listItemBox;
			}
		}

		[CssProperty("list-style")]
		[CssPropertyInherited]
		[DefaultValue("")]
		public string ListStyle
		{
			get
			{
				return this._listStyle;
			}
			set
			{
				this._listStyle = value;
			}
		}

		[CssProperty("list-style-image")]
		[CssPropertyInherited]
		[DefaultValue("")]
		public string ListStyleImage
		{
			get
			{
				return this._listStyleImage;
			}
			set
			{
				this._listStyleImage = value;
			}
		}

		[CssProperty("list-style-position")]
		[CssPropertyInherited]
		[DefaultValue("outside")]
		public string ListStylePosition
		{
			get
			{
				return this._listStylePosition;
			}
			set
			{
				this._listStylePosition = value;
			}
		}

		[CssProperty("list-style-type")]
		[CssPropertyInherited]
		[DefaultValue("disc")]
		public string ListStyleType
		{
			get
			{
				return this._listStyleType;
			}
			set
			{
				this._listStyleType = value;
			}
		}

		public PointF Location
		{
			get
			{
				return this._location;
			}
			set
			{
				this._location = value;
			}
		}

		[CssProperty("margin")]
		[DefaultValue("")]
		public string Margin
		{
			get
			{
				return this._margin;
			}
			set
			{
				this._margin = value;
				string[] strArrays = CssValue.SplitValues(value);
				switch ((int)strArrays.Length)
				{
					case 1:
					{
						string str = strArrays[0];
						string str1 = str;
						this.MarginBottom = str;
						string str2 = str1;
						string str3 = str2;
						this.MarginRight = str2;
						string str4 = str3;
						string str5 = str4;
						this.MarginLeft = str4;
						this.MarginTop = str5;
						return;
					}
					case 2:
					{
						string str6 = strArrays[0];
						string str7 = str6;
						this.MarginBottom = str6;
						this.MarginTop = str7;
						string str8 = strArrays[1];
						string str9 = str8;
						this.MarginRight = str8;
						this.MarginLeft = str9;
						return;
					}
					case 3:
					{
						this.MarginTop = strArrays[0];
						string str10 = strArrays[1];
						string str11 = str10;
						this.MarginRight = str10;
						this.MarginLeft = str11;
						this.MarginBottom = strArrays[2];
						return;
					}
					case 4:
					{
						this.MarginTop = strArrays[0];
						this.MarginRight = strArrays[1];
						this.MarginBottom = strArrays[2];
						this.MarginLeft = strArrays[3];
						return;
					}
					default:
					{
						return;
					}
				}
			}
		}

		[CssProperty("margin-bottom")]
		[DefaultValue("0")]
		public string MarginBottom
		{
			get
			{
				return this._marginBottom;
			}
			set
			{
				this._marginBottom = value;
			}
		}

		[CssProperty("margin-left")]
		[DefaultValue("0")]
		public string MarginLeft
		{
			get
			{
				return this._marginLeft;
			}
			set
			{
				this._marginLeft = value;
			}
		}

		[CssProperty("margin-right")]
		[DefaultValue("0")]
		public string MarginRight
		{
			get
			{
				return this._marginRight;
			}
			set
			{
				this._marginRight = value;
			}
		}

		[CssProperty("margin-top")]
		[DefaultValue("0")]
		public string MarginTop
		{
			get
			{
				return this._marginTop;
			}
			set
			{
				this._marginTop = value;
			}
		}

		[CssProperty("padding")]
		[DefaultValue("")]
		public string Padding
		{
			get
			{
				return this._padding;
			}
			set
			{
				this._padding = value;
				string[] strArrays = CssValue.SplitValues(value);
				switch ((int)strArrays.Length)
				{
					case 1:
					{
						string str = strArrays[0];
						string str1 = str;
						this.PaddingBottom = str;
						string str2 = str1;
						string str3 = str2;
						this.PaddingRight = str2;
						string str4 = str3;
						string str5 = str4;
						this.PaddingLeft = str4;
						this.PaddingTop = str5;
						return;
					}
					case 2:
					{
						string str6 = strArrays[0];
						string str7 = str6;
						this.PaddingBottom = str6;
						this.PaddingTop = str7;
						string str8 = strArrays[1];
						string str9 = str8;
						this.PaddingRight = str8;
						this.PaddingLeft = str9;
						return;
					}
					case 3:
					{
						this.PaddingTop = strArrays[0];
						string str10 = strArrays[1];
						string str11 = str10;
						this.PaddingRight = str10;
						this.PaddingLeft = str11;
						this.PaddingBottom = strArrays[2];
						return;
					}
					case 4:
					{
						this.PaddingTop = strArrays[0];
						this.PaddingRight = strArrays[1];
						this.PaddingBottom = strArrays[2];
						this.PaddingLeft = strArrays[3];
						return;
					}
					default:
					{
						return;
					}
				}
			}
		}

		[CssProperty("padding-bottom")]
		[DefaultValue("0")]
		public string PaddingBottom
		{
			get
			{
				return this._paddingBottom;
			}
			set
			{
				this._paddingBottom = value;
				this._actualPaddingBottom = float.NaN;
			}
		}

		[CssProperty("padding-left")]
		[DefaultValue("0")]
		public string PaddingLeft
		{
			get
			{
				return this._paddingLeft;
			}
			set
			{
				this._paddingLeft = value;
				this._actualPaddingLeft = float.NaN;
			}
		}

		[CssProperty("padding-right")]
		[DefaultValue("0")]
		public string PaddingRight
		{
			get
			{
				return this._paddingRight;
			}
			set
			{
				this._paddingRight = value;
				this._actualPaddingRight = float.NaN;
			}
		}

		[CssProperty("padding-top")]
		[DefaultValue("0")]
		public string PaddingTop
		{
			get
			{
				return this._paddingTop;
			}
			set
			{
				this._paddingTop = value;
				this._actualPaddingTop = float.NaN;
			}
		}

		public CssBox ParentBox
		{
			get
			{
				return this._parentBox;
			}
			set
			{
				if (this._parentBox != null && this._parentBox.Boxes.Contains(this))
				{
					this._parentBox.Boxes.Remove(this);
				}
				this._parentBox = value;
				if (value != null && !value.Boxes.Contains(this))
				{
					this._parentBox.Boxes.Add(this);
					this._initialContainer = value.InitialContainer;
				}
			}
		}

		internal List<CssLineBox> ParentLineBoxes
		{
			get
			{
				return this._parentLineBoxes;
			}
		}

		[CssProperty("position")]
		[DefaultValue("static")]
		public string Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		internal Dictionary<CssLineBox, RectangleF> Rectangles
		{
			get
			{
				return this._rectangles;
			}
		}

		public SizeF Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
				this.UpdateWords();
			}
		}

		[CssProperty("text-align")]
		[CssPropertyInherited]
		[DefaultValue("")]
		public string TextAlign
		{
			get
			{
				return this._textAlign;
			}
			set
			{
				this._textAlign = value;
			}
		}

		[CssProperty("text-decoration")]
		[DefaultValue("")]
		public string TextDecoration
		{
			get
			{
				return this._textDecoration;
			}
			set
			{
				this._textDecoration = value;
			}
		}

		[CssProperty("text-indent")]
		[CssPropertyInherited]
		[DefaultValue("0")]
		public string TextIndent
		{
			get
			{
				return this._textIndent;
			}
			set
			{
				this._textIndent = this.NoEms(value);
			}
		}

		[CssProperty("top")]
		[DefaultValue("auto")]
		public string Top
		{
			get
			{
				return this._top;
			}
			set
			{
				this._top = value;
			}
		}

		[CssProperty("vertical-align")]
		[CssPropertyInherited]
		[DefaultValue("baseline")]
		public string VerticalAlign
		{
			get
			{
				return this._verticalAlign;
			}
			set
			{
				this._verticalAlign = value;
			}
		}

		[CssProperty("white-space")]
		[CssPropertyInherited]
		[DefaultValue("normal")]
		public string WhiteSpace
		{
			get
			{
				return this._whiteSpace;
			}
			set
			{
				this._whiteSpace = value;
			}
		}

		[CssProperty("width")]
		[DefaultValue("auto")]
		public string Width
		{
			get
			{
				return this._width;
			}
			set
			{
				this._width = value;
			}
		}

		internal List<CssBoxWord> Words
		{
			get
			{
				return this._boxWords;
			}
		}

		[CssProperty("word-spacing")]
		[DefaultValue("normal")]
		public string WordSpacing
		{
			get
			{
				return this._wordSpacing;
			}
			set
			{
				this._wordSpacing = this.NoEms(value);
			}
		}

		static CssBox()
		{
			CssBox._properties = new Dictionary<string, PropertyInfo>();
			CssBox._defaults = new Dictionary<string, string>();
			CssBox._inheritables = new List<PropertyInfo>();
			CssBox._cssproperties = new List<PropertyInfo>();
			PropertyInfo[] properties = typeof(CssBox).GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				CssPropertyAttribute customAttribute = Attribute.GetCustomAttribute(properties[i], typeof(CssPropertyAttribute)) as CssPropertyAttribute;
				if (customAttribute != null)
				{
					CssBox._properties.Add(customAttribute.Name, properties[i]);
					CssBox._defaults.Add(customAttribute.Name, CssBox.GetDefaultValue(properties[i]));
					CssBox._cssproperties.Add(properties[i]);
					if (Attribute.GetCustomAttribute(properties[i], typeof(CssPropertyInheritedAttribute)) is CssPropertyInheritedAttribute)
					{
						CssBox._inheritables.Add(properties[i]);
					}
				}
			}
			CssBox.Empty = new CssBox();
		}

		protected CssBox()
		{
			this._boxWords = new List<CssBoxWord>();
			this._boxes = new List<CssBox>();
			this._lineBoxes = new List<CssLineBox>();
			this._parentLineBoxes = new List<CssLineBox>();
			this._rectangles = new Dictionary<CssLineBox, RectangleF>();
			foreach (string key in CssBox._properties.Keys)
			{
				CssBox._properties[key].SetValue(this, CssBox._defaults[key], null);
			}
		}

		public CssBox(CssBox parentBox) : this()
		{
			this.ParentBox = parentBox;
		}

		internal CssBox(CssBox parentBox, HtmlTag tag) : this(parentBox)
		{
			this._htmltag = tag;
		}

		internal bool ContainsInlinesOnly()
		{
			bool flag;
			List<CssBox>.Enumerator enumerator = this.Boxes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Display == "inline")
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private void CreateListItemBox(Graphics g)
		{
			if (this.Display == "list-item")
			{
				if (this._listItemBox == null)
				{
					this._listItemBox = new CssBox();
					this._listItemBox.InheritStyle(this, false);
					this._listItemBox.Display = "inline";
					this._listItemBox.SetInitialContainer(this.InitialContainer);
					if (this.ParentBox == null || !(this.ListStyleType == "decimal"))
					{
						this._listItemBox.Text = "•";
					}
					else
					{
						CssBox cssBox = this._listItemBox;
						int indexForList = this.GetIndexForList();
						cssBox.Text = string.Concat(indexForList.ToString(), ".");
					}
					this._listItemBox.MeasureBounds(g);
					this._listItemBox.Size = new SizeF(this._listItemBox.Words[0].Width, this._listItemBox.Words[0].Height);
				}
				CssBoxWord item = this._listItemBox.Words[0];
				float x = this.Location.X;
				SizeF size = this._listItemBox.Size;
				item.Left = x - size.Width - 5f;
				this._listItemBox.Words[0].Top = this.Location.Y + this.ActualPaddingTop;
			}
		}

		internal CssBoxWord FirstWordOccourence(CssBox b, CssLineBox line)
		{
			CssBoxWord cssBoxWord;
			if (b.Words.Count == 0 && b.Boxes.Count == 0)
			{
				return null;
			}
			if (b.Words.Count <= 0)
			{
				foreach (CssBox box in b.Boxes)
				{
					CssBoxWord cssBoxWord1 = this.FirstWordOccourence(box, line);
					if (cssBoxWord1 == null)
					{
						continue;
					}
					cssBoxWord = cssBoxWord1;
					return cssBoxWord;
				}
				return null;
			}
			else
			{
				foreach (CssBoxWord word in b.Words)
				{
					if (!line.Words.Contains(word))
					{
						continue;
					}
					cssBoxWord = word;
					return cssBoxWord;
				}
				return null;
			}
			return cssBoxWord;
		}

		internal string GetAttribute(string attribute)
		{
			return this.GetAttribute(attribute, string.Empty);
		}

		internal string GetAttribute(string attribute, string defaultValue)
		{
			if (this.HtmlTag == null)
			{
				return defaultValue;
			}
			if (!this.HtmlTag.HasAttribute(attribute))
			{
				return defaultValue;
			}
			return this.HtmlTag.Attributes[attribute];
		}

		private static string GetDefaultValue(PropertyInfo prop)
		{
			DefaultValueAttribute customAttribute = Attribute.GetCustomAttribute(prop, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
			if (customAttribute == null)
			{
				return string.Empty;
			}
			string str = Convert.ToString(customAttribute.Value);
			if (!string.IsNullOrEmpty(str))
			{
				return str;
			}
			return string.Empty;
		}

		public float GetEmHeight()
		{
			return this.ActualFont.GetHeight();
		}

		internal float GetFullWidth(Graphics g)
		{
			float single = 0f;
			float single1 = 0f;
			this.GetFullWidth_WordsWith(this, g, ref single, ref single1);
			return single1 + single;
		}

		private void GetFullWidth_WordsWith(CssBox b, Graphics g, ref float sum, ref float paddingsum)
		{
			if (b.Display != "inline")
			{
				sum = 0f;
			}
			paddingsum = paddingsum + (b.ActualBorderLeftWidth + b.ActualBorderRightWidth + b.ActualPaddingRight + b.ActualPaddingLeft);
			if (b.Words.Count <= 0)
			{
				foreach (CssBox box in b.Boxes)
				{
					this.GetFullWidth_WordsWith(box, g, ref sum, ref paddingsum);
				}
			}
			else
			{
				foreach (CssBoxWord word in b.Words)
				{
					sum = sum + word.FullWidth;
				}
			}
		}

		private int GetIndexForList()
		{
			int num;
			int num1 = 0;
			List<CssBox>.Enumerator enumerator = this.ParentBox.Boxes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CssBox current = enumerator.Current;
					if (current.Display == "list-item")
					{
						num1++;
					}
					if (!current.Equals(this))
					{
						continue;
					}
					num = num1;
					return num;
				}
				return num1;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return num;
		}

		internal float GetMaximumBottom(CssBox startBox, float currentMaxBottom)
		{
			foreach (CssLineBox key in startBox.Rectangles.Keys)
			{
				RectangleF item = startBox.Rectangles[key];
				currentMaxBottom = Math.Max(currentMaxBottom, item.Bottom);
			}
			foreach (CssBox box in startBox.Boxes)
			{
				currentMaxBottom = Math.Max(currentMaxBottom, box.ActualBottom);
				currentMaxBottom = Math.Max(currentMaxBottom, this.GetMaximumBottom(box, currentMaxBottom));
			}
			return currentMaxBottom;
		}

		internal float GetMinimumWidth()
		{
			float single = 0f;
			float single1 = 0f;
			CssBoxWord cssBoxWord = null;
			this.GetMinimumWidth_LongestWord(this, ref single, ref cssBoxWord);
			if (cssBoxWord != null)
			{
				this.GetMinimumWidth_BubblePadding(cssBoxWord.OwnerBox, this, ref single1);
			}
			return single + single1;
		}

		private void GetMinimumWidth_BubblePadding(CssBox box, CssBox endbox, ref float sum)
		{
			float actualBorderLeftWidth = box.ActualBorderLeftWidth + box.ActualPaddingLeft + box.ActualBorderRightWidth + box.ActualPaddingRight;
			sum = sum + actualBorderLeftWidth;
			if (!box.Equals(endbox))
			{
				this.GetMinimumWidth_BubblePadding(box.ParentBox, endbox, ref sum);
			}
		}

		private void GetMinimumWidth_LongestWord(CssBox b, ref float maxw, ref CssBoxWord word)
		{
			if (b.Words.Count <= 0)
			{
				foreach (CssBox box in b.Boxes)
				{
					this.GetMinimumWidth_LongestWord(box, ref maxw, ref word);
				}
			}
			else
			{
				foreach (CssBoxWord cssBoxWord in b.Words)
				{
					if (cssBoxWord.FullWidth <= maxw)
					{
						continue;
					}
					maxw = cssBoxWord.FullWidth;
					word = cssBoxWord;
				}
			}
		}

		private CssBox GetNextSibling()
		{
			if (this.ParentBox == null)
			{
				return null;
			}
			int num = this.ParentBox.Boxes.IndexOf(this);
			if (num < 0)
			{
				throw new Exception("Box doesn't exist on parent's Box list");
			}
			if (num == this.ParentBox.Boxes.Count - 1)
			{
				return null;
			}
			return this.ParentBox.Boxes[num + 1];
		}

		private CssBox GetPreviousSibling(CssBox b)
		{
			CssBox i;
			int num = 0;
			if (b.ParentBox == null)
			{
				return null;
			}
			int num1 = b.ParentBox.Boxes.IndexOf(this);
			if (num1 < 0)
			{
				throw new Exception("Box doesn't exist on parent's Box list");
			}
			if (num1 == 0)
			{
				return null;
			}
			int num2 = 1;
			for (i = b.ParentBox.Boxes[num1 - num2]; (i.Display == "none" || i.Position == "absolute") && num1 - num2 - 1 >= 0; i = b.ParentBox.Boxes[num1 - num])
			{
				num = num2 + 1;
				num2 = num;
			}
			if (i.Display != "none")
			{
				return i;
			}
			return null;
		}

		internal bool HasJustInlineSiblings()
		{
			if (this.ParentBox == null)
			{
				return false;
			}
			return this.ParentBox.ContainsInlinesOnly();
		}

		internal void InheritStyle()
		{
			this.InheritStyle(this.ParentBox, false);
		}

		internal void InheritStyle(CssBox godfather, bool everything)
		{
			if (godfather != null)
			{
				foreach (PropertyInfo propertyInfo in (everything ? CssBox._cssproperties : CssBox._inheritables))
				{
					propertyInfo.SetValue(this, propertyInfo.GetValue(godfather, null), null);
				}
			}
		}

		private float MarginCollapse(CssBox a, CssBox b)
		{
			return Math.Max((a == null ? 0f : a.ActualMarginBottom), (b == null ? 0f : b.ActualMarginTop));
		}

		public virtual void MeasureBounds(Graphics g)
		{
			if (this.Display == "none")
			{
				return;
			}
			this.RectanglesReset();
			this.MeasureWordsSize(g);
			if (this.Display == "block" || this.Display == "list-item" || this.Display == "table" || this.Display == "inline-table" || this.Display == "table-cell" || this.Display == "none")
			{
				if (this.Display != "table-cell")
				{
					CssBox previousSibling = this.GetPreviousSibling(this);
					PointF location = this.ContainingBlock.Location;
					float x = location.X + this.ContainingBlock.ActualPaddingLeft + this.ActualMarginLeft + this.ContainingBlock.ActualBorderLeftWidth;
					float single = (previousSibling != null || this.ParentBox == null ? 0f : this.ParentBox.ClientTop) + this.MarginCollapse(previousSibling, this) + (previousSibling != null ? previousSibling.ActualBottom + previousSibling.ActualBorderBottomWidth : 0f);
					this.Location = new PointF(x, single);
					this.ActualBottom = single;
				}
				if (this.Display != "table-cell" && this.Display != "table")
				{
					float minimumWidth = this.GetMinimumWidth();
					SizeF size = this.ContainingBlock.Size;
					float width = size.Width - this.ContainingBlock.ActualPaddingLeft - this.ContainingBlock.ActualPaddingRight - this.ContainingBlock.ActualBorderLeftWidth - this.ContainingBlock.ActualBorderRightWidth - this.ActualMarginLeft - this.ActualMarginRight - this.ActualBorderLeftWidth - this.ActualBorderRightWidth;
					if (this.Width != "auto" && !string.IsNullOrEmpty(this.Width))
					{
						width = CssValue.ParseLength(this.Width, width, this);
					}
					if (width < minimumWidth)
					{
						width = minimumWidth;
					}
					this.Size = new SizeF(width, this.Size.Height);
				}
				if (this.Display == "table" || this.Display == "inline-table")
				{
					CssTable cssTable = new CssTable(this, g);
				}
				else if (this.Display != "none")
				{
					if (!this.ContainsInlinesOnly())
					{
						CssBox cssBox = null;
						foreach (CssBox box in this.Boxes)
						{
							if (box.Display == "none")
							{
								continue;
							}
							box.MeasureBounds(g);
							cssBox = box;
						}
						if (cssBox != null)
						{
							this.ActualBottom = Math.Max(this.ActualBottom, cssBox.ActualBottom + cssBox.ActualMarginBottom + this.ActualPaddingBottom);
						}
					}
					else
					{
						this.ActualBottom = this.Location.Y;
						CssLayoutEngine.CreateLineBoxes(g, this);
					}
				}
			}
			if (this.InitialContainer != null)
			{
				InitialContainer initialContainer = this.InitialContainer;
				SizeF maximumSize = this.InitialContainer.MaximumSize;
				float single1 = Math.Max(maximumSize.Width, this.ActualRight);
				SizeF sizeF = this.InitialContainer.MaximumSize;
				initialContainer.MaximumSize = new SizeF(single1, Math.Max(sizeF.Height, this.ActualBottom));
			}
		}

		private void MeasureWordSpacing(Graphics g)
		{
			this._actualWordSpacing = CssLayoutEngine.WhiteSpace(g, this);
			if (this.WordSpacing != "normal")
			{
				string str = Parser.Search("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", this.WordSpacing);
				CssBox cssBox = this;
				cssBox._actualWordSpacing = cssBox._actualWordSpacing + CssValue.ParseLength(str, 1f, this);
			}
		}

		internal void MeasureWordsSize(Graphics g)
		{
			if (this._wordsSizeMeasured)
			{
				return;
			}
			if (float.IsNaN(this._actualWordSpacing))
			{
				this.MeasureWordSpacing(g);
			}
			if (this.HtmlTag == null || !this.HtmlTag.TagName.Equals("img", StringComparison.CurrentCultureIgnoreCase))
			{
				bool flag = false;
				foreach (CssBoxWord word in this.Words)
				{
					bool flag1 = CssBoxWordSplitter.CollapsesWhiteSpaces(this);
					if (CssBoxWordSplitter.EliminatesLineBreaks(this))
					{
						word.ReplaceLineBreaksAndTabs();
					}
					if (!word.IsSpaces)
					{
						string text = word.Text;
						CharacterRange[] characterRange = new CharacterRange[] { new CharacterRange(0, text.Length) };
						CharacterRange[] characterRangeArray = characterRange;
						StringFormat stringFormat = new StringFormat();
						stringFormat.SetMeasurableCharacterRanges(characterRangeArray);
						Region[] regionArray = g.MeasureCharacterRanges(text, this.ActualFont, new RectangleF(0f, 0f, float.MaxValue, float.MaxValue), stringFormat);
						SizeF size = regionArray[0].GetBounds(g).Size;
						PointF location = regionArray[0].GetBounds(g).Location;
						word.LastMeasureOffset = new PointF(location.X, location.Y);
						word.Width = size.Width;
						word.Height = size.Height;
						flag = false;
					}
					else
					{
						word.Height = this.FontLineSpacing;
						if (word.IsTab)
						{
							word.Width = this.ActualWordSpacing * 4f;
						}
						else if (word.IsLineBreak)
						{
							word.Width = 0f;
						}
						else if (!flag || !flag1)
						{
							word.Width = this.ActualWordSpacing * (float)((flag1 ? 1 : word.Text.Length));
						}
						flag = true;
					}
				}
			}
			else
			{
				CssBoxWord cssBoxWord = new CssBoxWord(this, CssValue.GetImage(this.GetAttribute("src")));
				this.Words.Clear();
				this.Words.Add(cssBoxWord);
			}
			this._wordsSizeMeasured = true;
		}

		private string NoEms(string length)
		{
			CssLength cssLength = new CssLength(length);
			if (cssLength.Unit == CssLength.CssUnit.Ems)
			{
				length = cssLength.ConvertEmToPixels(this.GetEmHeight()).ToString();
			}
			return length;
		}

		internal void OffsetRectangle(CssLineBox lineBox, float gap)
		{
			if (this.Rectangles.ContainsKey(lineBox))
			{
				RectangleF item = this.Rectangles[lineBox];
				this.Rectangles[lineBox] = new RectangleF(item.X, item.Y + gap, item.Width, item.Height);
			}
		}

		internal void OffsetTop(float amount)
		{
			List<CssLineBox> cssLineBoxes = new List<CssLineBox>();
			foreach (CssLineBox key in this.Rectangles.Keys)
			{
				cssLineBoxes.Add(key);
			}
			foreach (CssLineBox cssLineBox in cssLineBoxes)
			{
				RectangleF item = this.Rectangles[cssLineBox];
				this.Rectangles[cssLineBox] = new RectangleF(item.X, item.Y + amount, item.Width, item.Height);
			}
			foreach (CssBoxWord word in this.Words)
			{
				CssBoxWord top = word;
				top.Top = top.Top + amount;
			}
			foreach (CssBox box in this.Boxes)
			{
				box.OffsetTop(amount);
			}
			float x = this.Location.X;
			PointF location = this.Location;
			this.Location = new PointF(x, location.Y + amount);
		}

		public void Paint(Graphics g)
		{
			if (this.Display == "none")
			{
				return;
			}
			if (this.Display == "table-cell" && this.EmptyCells == "hide" && this.IsSpaceOrEmpty)
			{
				return;
			}
			RectangleF[] array = ((this.Rectangles.Count == 0 ? new List<RectangleF>(new RectangleF[] { this.Bounds }) : new List<RectangleF>(this.Rectangles.Values))).ToArray();
			PointF pointF = (this.InitialContainer != null ? this.InitialContainer.ScrollOffset : PointF.Empty);
			for (int i = 0; i < (int)array.Length; i++)
			{
				RectangleF rectangleF = array[i];
				rectangleF.Offset(pointF);
				if (this.InitialContainer != null && this.HtmlTag != null && this.HtmlTag.TagName.Equals("a", StringComparison.CurrentCultureIgnoreCase))
				{
					if (this.InitialContainer.LinkRegions.ContainsKey(this))
					{
						this.InitialContainer.LinkRegions.Remove(this);
					}
					this.InitialContainer.LinkRegions.Add(this, rectangleF);
				}
				this.PaintBackground(g, rectangleF);
				this.PaintBorder(g, rectangleF, i == 0, i == (int)array.Length - 1);
			}
			if (!this.IsImage)
			{
				System.Drawing.Font actualFont = this.ActualFont;
				using (SolidBrush solidBrush = new SolidBrush(CssValue.GetActualColor(this.Color)))
				{
					foreach (CssBoxWord word in this.Words)
					{
						string text = word.Text;
						float left = word.Left;
						PointF lastMeasureOffset = word.LastMeasureOffset;
						g.DrawString(text, actualFont, solidBrush, left - lastMeasureOffset.X + pointF.X, word.Top + pointF.Y);
					}
				}
			}
			else
			{
				RectangleF bounds = this.Words[0].Bounds;
				bounds.Offset(pointF);
				bounds.Height = bounds.Height - (this.ActualBorderTopWidth + this.ActualBorderBottomWidth + this.ActualPaddingTop + this.ActualPaddingBottom);
				bounds.Y = bounds.Y + (this.ActualBorderTopWidth + this.ActualPaddingTop);
				g.DrawImage(this.Words[0].Image, Rectangle.Round(bounds));
			}
			for (int j = 0; j < (int)array.Length; j++)
			{
				RectangleF rectangleF1 = array[j];
				rectangleF1.Offset(pointF);
				this.PaintDecoration(g, rectangleF1, j == 0, j == (int)array.Length - 1);
			}
			foreach (CssBox box in this.Boxes)
			{
				box.Paint(g);
			}
			this.CreateListItemBox(g);
			if (this.ListItemBox != null)
			{
				this.ListItemBox.Paint(g);
			}
		}

		private void PaintBackground(Graphics g, RectangleF rectangle)
		{
			if (this.ContainingBlock.TextAlign == "justify")
			{
				return;
			}
			GraphicsPath roundRect = null;
			Brush solidBrush = null;
			SmoothingMode smoothingMode = g.SmoothingMode;
			if (this.IsRounded)
			{
				roundRect = CssDrawingHelper.GetRoundRect(rectangle, this.ActualCornerNW, this.ActualCornerNE, this.ActualCornerSE, this.ActualCornerSW);
			}
			if (!(this.BackgroundGradient != "none") || rectangle.Width <= 0f || rectangle.Height <= 0f)
			{
				solidBrush = new SolidBrush(this.ActualBackgroundColor);
			}
			else
			{
				solidBrush = new LinearGradientBrush(rectangle, this.ActualBackgroundColor, this.ActualBackgroundGradient, this.ActualBackgroundGradientAngle);
			}
			if (this.InitialContainer != null && !this.InitialContainer.AvoidGeometryAntialias && this.IsRounded)
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;
			}
			if (roundRect == null)
			{
				g.FillRectangle(solidBrush, rectangle);
			}
			else
			{
				g.FillPath(solidBrush, roundRect);
			}
			g.SmoothingMode = smoothingMode;
			if (roundRect != null)
			{
				roundRect.Dispose();
			}
			if (solidBrush != null)
			{
				solidBrush.Dispose();
			}
		}

		private void PaintBorder(Graphics g, RectangleF rectangle, bool isFirst, bool isLast)
		{
			SmoothingMode smoothingMode = g.SmoothingMode;
			if (this.InitialContainer != null && !this.InitialContainer.AvoidGeometryAntialias && this.IsRounded)
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;
			}
			if (!string.IsNullOrEmpty(this.BorderTopStyle) && !(this.BorderTopStyle == "none"))
			{
				using (SolidBrush solidBrush = new SolidBrush(this.ActualBorderTopColor))
				{
					if (this.BorderTopStyle == "inset")
					{
						solidBrush.Color = CssDrawingHelper.Darken(this.ActualBorderTopColor);
					}
					g.FillPath(solidBrush, CssDrawingHelper.GetBorderPath(CssDrawingHelper.Border.Top, this, rectangle, isFirst, isLast));
				}
			}
			if (isLast && !string.IsNullOrEmpty(this.BorderRightStyle) && !(this.BorderRightStyle == "none"))
			{
				using (SolidBrush solidBrush1 = new SolidBrush(this.ActualBorderRightColor))
				{
					if (this.BorderRightStyle == "outset")
					{
						solidBrush1.Color = CssDrawingHelper.Darken(this.ActualBorderRightColor);
					}
					g.FillPath(solidBrush1, CssDrawingHelper.GetBorderPath(CssDrawingHelper.Border.Right, this, rectangle, isFirst, isLast));
				}
			}
			if (!string.IsNullOrEmpty(this.BorderBottomStyle) && !(this.BorderBottomStyle == "none"))
			{
				using (SolidBrush solidBrush2 = new SolidBrush(this.ActualBorderBottomColor))
				{
					if (this.BorderBottomStyle == "outset")
					{
						solidBrush2.Color = CssDrawingHelper.Darken(this.ActualBorderBottomColor);
					}
					g.FillPath(solidBrush2, CssDrawingHelper.GetBorderPath(CssDrawingHelper.Border.Bottom, this, rectangle, isFirst, isLast));
				}
			}
			if (isFirst && !string.IsNullOrEmpty(this.BorderLeftStyle) && !(this.BorderLeftStyle == "none"))
			{
				using (SolidBrush solidBrush3 = new SolidBrush(this.ActualBorderLeftColor))
				{
					if (this.BorderLeftStyle == "inset")
					{
						solidBrush3.Color = CssDrawingHelper.Darken(this.ActualBorderLeftColor);
					}
					g.FillPath(solidBrush3, CssDrawingHelper.GetBorderPath(CssDrawingHelper.Border.Left, this, rectangle, isFirst, isLast));
				}
			}
			g.SmoothingMode = smoothingMode;
		}

		private void PaintDecoration(Graphics g, RectangleF rectangle, bool isFirst, bool isLast)
		{
			if (string.IsNullOrEmpty(this.TextDecoration) || this.TextDecoration == "none" || this.IsImage)
			{
				return;
			}
			float descent = CssLayoutEngine.GetDescent(this.ActualFont);
			float ascent = CssLayoutEngine.GetAscent(this.ActualFont);
			float bottom = 0f;
			if (this.TextDecoration == "underline")
			{
				bottom = rectangle.Bottom - descent;
			}
			else if (this.TextDecoration == "line-through")
			{
				bottom = rectangle.Bottom - descent - ascent / 2f;
			}
			else if (this.TextDecoration == "overline")
			{
				bottom = rectangle.Bottom - descent - ascent - 2f;
			}
			bottom = bottom - (this.ActualPaddingBottom - this.ActualBorderBottomWidth);
			float x = rectangle.X;
			float right = rectangle.Right;
			if (isFirst)
			{
				x = x + (this.ActualPaddingLeft + this.ActualBorderLeftWidth);
			}
			if (isLast)
			{
				right = right - (this.ActualPaddingRight + this.ActualBorderRightWidth);
			}
			g.DrawLine(new Pen(this.ActualColor), x, bottom, right, bottom);
		}

		internal void RectanglesReset()
		{
			this._rectangles.Clear();
		}

		internal void RemoveAnonymousSpaces()
		{
			for (int i = 0; i < this.Boxes.Count; i++)
			{
				if (this.Boxes[i] is CssAnonymousSpaceBlockBox || this.Boxes[i] is CssAnonymousSpaceBox)
				{
					this.Boxes.RemoveAt(i);
					i--;
				}
			}
		}

		public void SetBounds(Rectangle r)
		{
			this.SetBounds(new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height));
		}

		public void SetBounds(RectangleF rectangle)
		{
			this.Size = rectangle.Size;
			this.Location = rectangle.Location;
		}

		private void SetInitialContainer(InitialContainer b)
		{
			this._initialContainer = b;
		}

		public override string ToString()
		{
			string name = this.GetType().Name;
			if (this.HtmlTag != null)
			{
				name = string.Format("<{0}>", this.HtmlTag.TagName);
			}
			if (this.ParentBox == null)
			{
				return "Initial Container";
			}
			if (this.Display == "block")
			{
				return string.Format("{0} BlockBox {2}, Children:{1}", name, this.Boxes.Count, this.FontSize);
			}
			if (this.Display == "none")
			{
				return string.Format("{0} None", name);
			}
			return string.Format("{0} {2}: {1}", name, this.Text, this.Display);
		}

		private void UpdateWords()
		{
			this.Words.Clear();
			CssBoxWordSplitter cssBoxWordSplitter = new CssBoxWordSplitter(this, this.Text);
			cssBoxWordSplitter.SplitWords();
			this.Words.AddRange(cssBoxWordSplitter.Words);
		}
	}
}