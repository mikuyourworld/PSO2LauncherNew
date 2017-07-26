using System;
using System.Windows.Media.Imaging;

namespace Leayal.Ugoira.WPF
{
    public class FrameDrawEventArgs : EventArgs
    {
        public BitmapImage Image { get; }

        public FrameDrawEventArgs(BitmapImage _image) : base()
        {
            this.Image = _image;
        }
    }
}
