using System.Drawing;

namespace Leayal.Forms
{
    public struct WrapStringResult
    {
        public string Result { get; }
        public Size Size { get; }
        internal WrapStringResult(string re, Size _size)
        {
            this.Result = re;
            this.Size = _size;
        }
    }
}
