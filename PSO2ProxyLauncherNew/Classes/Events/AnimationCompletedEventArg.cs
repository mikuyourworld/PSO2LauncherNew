using System;
using PSO2ProxyLauncherNew.Classes.Components.Animator;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class AnimationCompletedEventArg : EventArgs
    {
        public AnimationCompletedEventArg(Control c, AnimateMode m) : base()
        {
            this.Control = c;
            this.Mode = m;
        }
        public Animation Animation { get; set; }
        public Control Control { get; }
        public AnimateMode Mode { get; }
    }
}
