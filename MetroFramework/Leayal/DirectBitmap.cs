using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Leayal
{
    class DirectBitmap : IDisposable
    {
        public System.Drawing.Bitmap Bitmap { get; }
        public byte[] Bits { get; }
        private bool _disposed;
        public bool Disposed { get { return this._disposed; } }
        public int Height { get; }
        public int Width { get; }
        public Graphics Graphics { get; }

        protected GCHandle BitsHandle { get; }

        public DirectBitmap(int width, int height)
        {
            _disposed = false;
            Width = width;
            Height = height;
            Bits = new byte[width * height * 4];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new System.Drawing.Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
            Graphics = Graphics.FromImage(Bitmap);
        }

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
