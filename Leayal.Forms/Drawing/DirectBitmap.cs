using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Leayal.Drawing
{
    public sealed class DirectBitmap : IDisposable
    {
        public System.Drawing.Bitmap Bitmap { get; }
        public byte[] Bits { get; }
        private bool _disposed;
        public bool Disposed { get { return this._disposed; } }
        public int Height { get; }
        public int Width { get; }
        public System.Drawing.Graphics Graphics { get; }

        internal GCHandle BitsHandle { get; }

        public DirectBitmap(Size _size, PixelFormat _pixelformat) : this(_size.Width, _size.Height, _pixelformat) { }

        public DirectBitmap(int _width, int _height, PixelFormat _pixelformat)
        {
            if (_width == 0 || _height == 0) throw new NotSupportedException("Empty image not supported");
            _disposed = false;
            Width = _width;
            Height = _height;
            switch (_pixelformat)
            {
                case PixelFormat.Format24bppRgb:
                    Bits = new byte[_width * _height * 3];
                    break;
                case PixelFormat.Format48bppRgb:
                    Bits = new byte[_width * _height * 6];
                    break;
                case PixelFormat.Format64bppArgb | PixelFormat.Format64bppPArgb:
                    Bits = new byte[_width * _height * 8];
                    break;
                default:
                    Bits = new byte[_width * _height * 4];
                    break;
            }
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new System.Drawing.Bitmap(_width, _height, _width * 4, _pixelformat, BitsHandle.AddrOfPinnedObject());
            Graphics = System.Drawing.Graphics.FromImage(Bitmap);
        }

        public DirectBitmap(Size _size) : this(_size.Width, _size.Height) { }

        public DirectBitmap(int _width, int _height) : this(_width, _height, PixelFormat.Format32bppPArgb) { }

        //public int upX = 114;
        //public int upY = 28;
        //public Size size = new Size(1137, 640);
        //public Color rosa = Color.FromArgb(255, 102, 153);
        //const int tollerance = 20;
        public void deleteColors(Color colorToSave)
        {
            this.deleteColors(colorToSave, 200);
        }
        public void deleteColors(Color colorToSave, int similiar)
        {
            if (similiar > 255)
                similiar = 255;
            else if (similiar < 0)
                similiar = 0;
            //Bitmap.LockBits(
            for (int i = 0; i < Bitmap.Width; i++)
                for (int j = 0; j < Bitmap.Height; j++)
                {
                    Color c = Bitmap.GetPixel(i, j);
                    if (c != Color.Transparent && !colorsAreSimilar(c, colorToSave, similiar))
                        Bitmap.SetPixel(i, j, Color.White);
                }
        }

        public DirectBitmap Clone()
        {
            if (this.Disposed) return null;
            DirectBitmap result = new DirectBitmap(this.Bitmap.Size);
            result.Graphics.DrawImageUnscaled(this.Bitmap, 0, 0);
            return result;
        }

        private bool colorsAreSimilar(Color a, Color b, int similiar)
        {
            if (Math.Abs(a.R - b.R) < similiar && Math.Abs(a.G - b.G) < similiar && Math.Abs(a.B - b.B) < similiar)
                return true;
            return false;
        }

        public void Dispose()
        {
            if (Disposed) return;
            _disposed = true;
            Graphics.Dispose();
            Bitmap.Dispose();
            BitsHandle.Free();
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
