using System;
using System.Drawing;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    class ExtendedToolTip : ToolTip
    {
        public ExtendedToolTip() : base() { this.SettingUp(); }
        public ExtendedToolTip(System.ComponentModel.IContainer cont) : base(cont) { this.SettingUp(); }
        public Size PreferedSize { get; set; }
        public Font Font { get; set; }

        private void SettingUp()
        {
            this.Font = Form.DefaultFont;
            this.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
            this.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.AutoPopDelay = 0;
            this.InitialDelay = 0;
            this.StripAmpersands = false;
            this.UseAnimation = true;
            this.UseFading = true;
            this.ReshowDelay = 0;
            this.ShowAlways = true;
        }

        public void StopTime()
        {
            this.StopTimer();
        }
    }
}
