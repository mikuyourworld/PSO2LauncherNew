using System;
using System.Drawing;

namespace MetroFramework
{
	internal class MetroImage
	{
		public MetroImage()
		{
		}

		public static Image ResizeImage(Image imgToResize, Rectangle maxOffset)
		{
			int width = imgToResize.Width;
			int height = imgToResize.Height;
			float single = 0f;
			float width1 = 0f;
			float height1 = 0f;
			width1 = (float)maxOffset.Width / (float)width;
			height1 = (float)maxOffset.Height / (float)height;
			single = (height1 < width1 ? height1 : width1);
			int num = (int)((float)width * single);
			int num1 = (int)((float)height * single);
			return imgToResize.GetThumbnailImage(num, num1, null, IntPtr.Zero);
		}
	}
}