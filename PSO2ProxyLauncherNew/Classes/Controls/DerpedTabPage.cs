using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    public class DerpedTabPage : TabPage
    {
        public DerpedTabPage() : base() { }
        public DerpedTabPage(string text) : base(text) { }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.Parent != null)
            {
                e.ClipRectangle.Offset(this.Left, this.Top);
                this.InvokePaintBackground(this.Parent, e);
            }
            else
                base.OnPaintBackground(e);
        }
    }
}
