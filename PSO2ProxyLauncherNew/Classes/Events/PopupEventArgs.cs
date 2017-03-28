using System.Drawing;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class PopupEventArgs : System.Windows.Forms.PopupEventArgs
    {
        public Point Location { get; set; }
        public PopupEventArgs(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size, Point p) : base(associatedWindow, associatedControl, isBalloon, size)
        {
            this.Location = p;
        }
    }
}
