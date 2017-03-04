using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace MetroFramework.Drawing
{
	internal sealed class MetroBackBuffer
	{
		private Bitmap backBuffer;

		public MetroBackBuffer(Size bufferSize)
		{
			this.backBuffer = new Bitmap(bufferSize.Width, bufferSize.Height, PixelFormat.Format32bppArgb);
		}

		public Graphics CreateGraphics()
		{
			Graphics graphic = Graphics.FromImage(this.backBuffer);
			graphic.CompositingMode = CompositingMode.SourceOver;
			graphic.CompositingQuality = CompositingQuality.HighQuality;
			graphic.InterpolationMode = InterpolationMode.High;
			graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphic.SmoothingMode = SmoothingMode.AntiAlias;
			graphic.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			return graphic;
		}

		public void Draw(Graphics g)
		{
			g.DrawImageUnscaled(this.backBuffer, Point.Empty);
		}
	}
}