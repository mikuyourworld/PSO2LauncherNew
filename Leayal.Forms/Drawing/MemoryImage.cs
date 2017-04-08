using System;
using System.Drawing;
using System.IO;

namespace Leayal.Drawing
{
    public class MemoryImage : IDisposable
    {
        private bool _disposed, leaveStreamOpen;
        private Stream innerStream;
        public Image Image { get; }

        public static MemoryImage FromStream(Stream _stream, bool leaveOpen)
        {
            return new MemoryImage(_stream, leaveOpen);
        }

        public static MemoryImage FromFile(string filepath, bool leaveOpen)
        {
            return new MemoryImage(filepath, leaveOpen);
        }

        internal MemoryImage(Stream _stream, bool leaveOpen)
        {
            this.leaveStreamOpen = leaveOpen;
            this.innerStream = _stream;
            this.Image = Image.FromStream(this.innerStream);
        }

        internal MemoryImage(string filepath, bool leaveOpen) : this(File.OpenRead(filepath), leaveOpen) { }
        
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (this.Image != null)
                this.Image.Dispose();
            if (this.innerStream!= null && !this.leaveStreamOpen)
                this.innerStream.Dispose();
        }
    }
}
