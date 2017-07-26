using System.Windows.Media.Imaging;

namespace Leayal.Ugoira.WPF
{
    public class Frame
    {
        public FrameHeader Header { get; }
        public BitmapImage Image { get; }
        internal Frame(BitmapImage _image) : this(new FrameHeader(), _image) { }
        internal Frame(FrameHeader header, BitmapImage _image)
        {
            this.Header = header;
            this.Image = _image;
        }
    }
}
