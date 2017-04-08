using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Leayal.Drawing
{
    class ImageHelper
    {
        internal static readonly byte[] bmpSig = Encoding.ASCII.GetBytes("BM");
        internal static readonly byte[] gifSig = Encoding.ASCII.GetBytes("GIF");
        internal static readonly byte[] pngSig = { 137, 80, 78, 71 };
        internal static readonly byte[] tiffStg = { 73, 73, 42 };
        internal static readonly byte[] tiff2Sig = { 77, 77, 42 };
        internal static readonly byte[] jpegSig = { 255, 216, 255, 224 };
        internal static readonly byte[] jpeg2Sig = { 255, 216, 255, 225 };

        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            if (jpegSig.SequenceEqual(bytes.Take(jpegSig.Length)))
                return ImageFormat.Jpeg;

            if (jpeg2Sig.SequenceEqual(bytes.Take(jpeg2Sig.Length)))
                return ImageFormat.Jpeg;

            if (pngSig.SequenceEqual(bytes.Take(pngSig.Length)))
                return ImageFormat.Png;

            if (bmpSig.SequenceEqual(bytes.Take(bmpSig.Length)))
                return ImageFormat.Bmp;

            if (gifSig.SequenceEqual(bytes.Take(gifSig.Length)))
                return ImageFormat.Gif;

            if (tiffStg.SequenceEqual(bytes.Take(tiffStg.Length)))
                return ImageFormat.Tiff;

            if (tiff2Sig.SequenceEqual(bytes.Take(tiff2Sig.Length)))
                return ImageFormat.Tiff;

            return null;
        }
    }
}
