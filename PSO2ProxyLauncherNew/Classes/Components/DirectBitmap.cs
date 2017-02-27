using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    class DirectBitmap : IDisposable
    {
        public System.Drawing.Bitmap Bitmap { get; }
        public byte[] Bits { get; }
        private bool _disposed;
        public bool Disposed { get { return this._disposed; } }
        public int Height { get; }
        public int Width { get; }
        public System.Drawing.Graphics Graphics { get; }

        protected GCHandle BitsHandle { get; }

        public DirectBitmap(int width, int height)
        {
            _disposed = false;
            Width = width;
            Height = height;
            Bits = new byte[width * height * 4];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new System.Drawing.Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
            Graphics = System.Drawing.Graphics.FromImage(Bitmap);
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
