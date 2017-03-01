using System.Windows.Forms;
using System.Drawing;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class RelativeButton : Button, Components.ReserveRelativeLocation
    {
        private Point _RelativeLocation;
        public Point RelativeLocation
        {
            get { return this._RelativeLocation; }
            set { this._RelativeLocation = value; }
        }

        public new Point Location
        {
            get { return base.Location; }
            set
            {
                if (this.RelativeLocation == null)
                    this.RelativeLocation = value;
                base.Location = value;
            }
        }
    }
}
