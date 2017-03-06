using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    class QuickBitmap : IDisposable
    {
        public Bitmap Bitmap { get; }
        private Graphics _Graphics;
        public Graphics Graphics { get { return this._Graphics; } }
        private bool _disposed;
        public bool Disposed { get { return this._disposed; } }
        public QuickBitmap(Size _size) : this(_size.Width, _size.Height) { }

        public QuickBitmap(int _width, int _height)
        {
            this.Bitmap = new Bitmap(_width, _height);
            this.CreateGraphics();
        }

        public QuickBitmap(Bitmap _bitmap)
        {
            this.Bitmap = (Bitmap)_bitmap.Clone();
            this.CreateGraphics();
        }

        private void CreateGraphics()
        {
            if (this.Bitmap != null)
                this._Graphics = Graphics.FromImage(this.Bitmap);
        }

        public void Dispose()
        {
            if (this._disposed) return;
            this._disposed = true;
            this.Graphics.Dispose();
            this.Bitmap.Dispose();
        }
    }
}
