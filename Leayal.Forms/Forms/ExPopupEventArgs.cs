using System.Drawing;
using System.Windows.Forms;

namespace Leayal.Forms
{
    public class ExPopupEventArgs : System.Windows.Forms.PopupEventArgs
    {
        public Point Location { get; set; }
        public ExPopupEventArgs(IWin32Window associatedWindow, System.Windows.Forms.Control associatedControl, bool isBalloon, Size size, Point p) : base(associatedWindow, associatedControl, isBalloon, size)
        {
            this.Location = p;
        }
    }
}
