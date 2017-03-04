using System;
using System.Drawing;

namespace MetroFramework.Drawing.Html
{
	public class CssRectangle
	{
		private float _left;

		private float _top;

		private float _width;

		private float _height;

		public float Bottom
		{
			get
			{
				return this.Bounds.Bottom;
			}
			set
			{
				this.Height = value - this.Top;
			}
		}

		public RectangleF Bounds
		{
			get
			{
				return new RectangleF(this.Left, this.Top, this.Width, this.Height);
			}
			set
			{
				this.Left = value.Left;
				this.Top = value.Top;
				this.Width = value.Width;
				this.Height = value.Height;
			}
		}

		public float Height
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

		public float Left
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

		public PointF Location
		{
			get
			{
				return new PointF(this.Left, this.Top);
			}
			set
			{
				this.Left = value.X;
				this.Top = value.Y;
			}
		}

		public float Right
		{
			get
			{
				return this.Bounds.Right;
			}
			set
			{
				this.Width = value - this.Left;
			}
		}

		public SizeF Size
		{
			get
			{
				return new SizeF(this.Width, this.Height);
			}
			set
			{
				this.Width = value.Width;
				this.Height = value.Height;
			}
		}

		public float Top
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

		public float Width
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

		public CssRectangle()
		{
		}
	}
}