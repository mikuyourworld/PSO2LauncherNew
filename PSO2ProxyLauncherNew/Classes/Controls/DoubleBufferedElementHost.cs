using System;
using System.Drawing;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class DoubleBufferedElementHost : System.Windows.Forms.Integration.ElementHost, Interfaces.ReserveRelativeLocation
    {
        public DoubleBufferedElementHost() : base()
        {
            this.SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
            this.UpdateStyles();
        }

        public Point RelativeLocation { get; set; }
    }
}
