using System;
using System.Drawing;
using System.Windows.Forms;
using PSO2ProxyLauncherNew.Classes.Components.Animator;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class NonLinearTransfromNeededEventArg : EventArgs
    {
        public float CurrentTime { get; internal set; }

        public Rectangle ClientRectangle { get; internal set; }
        public byte[] Pixels { get; internal set; }
        public int Stride { get; internal set; }

        public Rectangle SourceClientRectangle { get; internal set; }
        public byte[] SourcePixels { get; internal set; }
        public int SourceStride { get; set; }

        public Animation Animation { get; set; }
        public Control Control { get; internal set; }
        public AnimateMode Mode { get; internal set; }
        public bool UseDefaultTransform { get; set; }
    }
}
