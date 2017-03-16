using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PSO2ProxyLauncherNew.Classes.Components.Animator;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class TransfromNeededEventArg : EventArgs
    {
        public TransfromNeededEventArg()
        {
            Matrix = new Matrix(1, 0, 0, 1, 0, 0);
        }

        public Matrix Matrix { get; set; }
        public float CurrentTime { get; internal set; }
        public Rectangle ClientRectangle { get; internal set; }
        public Rectangle ClipRectangle { get; internal set; }
        public Animation Animation { get; set; }
        public Control Control { get; internal set; }
        public AnimateMode Mode { get; internal set; }
        public bool UseDefaultMatrix { get; set; }
    }
}
