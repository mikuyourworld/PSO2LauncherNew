using PSO2ProxyLauncherNew.Classes.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls
{
    class ExtendedToolTip : IDisposable
    {

        private DerpedToolTipForm currentTooltip;
        private Control currentControl;
        private Dictionary<Control, string> innerToolTipText;

        public Font Font { get; set; }
        public Size PreferedSize { get; set; }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color FormColor { get; set; }
        public double Opacity { get; set; }

        public ExtendedToolTip()
        {
            this.Font = Form.DefaultFont;
            this.BackColor = Form.DefaultBackColor;
            this.ForeColor = Form.DefaultForeColor;
            this.Opacity = 100F;
            this.FormColor = Color.Lavender;
            this.innerToolTipText = new Dictionary<Control, string>();
        }

        private void ShowToolTip(DerpedToolTipForm _form)
        {
            _form.Show();
        }

        public void SetToolTip(Control c, string tooltipText)
        {
            if (this.innerToolTipText.ContainsKey(c)) return;
            this.innerToolTipText.Add(c, tooltipText);
            c.MouseEnter += C_MouseEnter;
            c.MouseLeave += C_MouseLeave;
            c.VisibleChanged += C_VisibleChanged;
        }

        private void C_MouseLeave(object sender, EventArgs e)
        {
            this.CloseCurrentTip();
        }

        private void C_MouseEnter(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (c != null && c != this.currentControl)
            {
                this.currentControl = c;
                this.currentTooltip = this.SetupAnotherTip();
                this.currentTooltip.Text = this.innerToolTipText[c];
                this.currentTooltip.Tag = c;
                PopupEventArgs arrrrgggg = new PopupEventArgs(this.currentTooltip, c, false, this.currentTooltip.Size);
                this.Popup?.Invoke(this.currentTooltip, arrrrgggg);
                this.currentTooltip.ClientSize = arrrrgggg.ToolTipSize;
                Point awgkaugw = System.Windows.Forms.Cursor.Position;
                awgkaugw.Offset(3, 3);
                this.currentTooltip.DesktopLocation = awgkaugw;
                this.currentTooltip.Opacity = this.Opacity;
                this.ShowToolTip(this.currentTooltip);
            }
        }

        private DerpedToolTipForm SetupAnotherTip()
        {
            var result = new DerpedToolTipForm();
            result.Paint += CurrentTooltip_Paint;
            //result.PaintBackground += CurrentTooltip_PaintBackground;
            result.FormBorderStyle = FormBorderStyle.None;
            result.BackColor = this.FormColor;
            result.Font = this.Font;
            result.StartPosition = FormStartPosition.Manual;
            result.ForeColor = this.ForeColor;
            result.FormClosed += Result_FormClosed;
            return result;
        }

        private void Result_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.currentTooltip = null;
            this.currentControl = null;
        }

        private void C_VisibleChanged(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (c != null && c == this.currentControl && !c.Visible)
                this.CloseCurrentTip();
        }

        private void CloseCurrentTip()
        {
            if (this.currentTooltip != null)
                this.currentTooltip.Close();
            this.currentTooltip = null;
            this.currentControl = null;
        }

        private void CurrentTooltip_Paint(object sender, PaintEventArgs e)
        {
            DerpedToolTipForm fffff = sender as DerpedToolTipForm;
            if (fffff != null)
                this.Draw?.Invoke(sender, new DrawToolTipEventArgs(e.Graphics, fffff, fffff.Tag as Control, fffff.ClientRectangle, fffff.Text, this.BackColor, fffff.ForeColor, fffff.Font));
        }

        private void CurrentTooltip_PaintBackground(object sender, PaintEventArgs e)
        {
            DerpedToolTipForm fffff = sender as DerpedToolTipForm;
            if (fffff != null)
                this.BackgroundDraw?.Invoke(sender, new DrawToolTipEventArgs(e.Graphics, fffff, fffff.Tag as Control, fffff.ClientRectangle, fffff.Text, fffff.BackColor, fffff.ForeColor, fffff.Font));
        }

        public void RemoveToolTip(Control c)
        {
            if (!this.innerToolTipText.ContainsKey(c)) return;
            if (c == this.currentControl)
                this.CloseCurrentTip();
            c.MouseEnter -= C_MouseEnter;
            c.MouseLeave -= C_MouseLeave;
            this.innerToolTipText.Remove(c);
        }

        public void Hide()
        {
            this.CloseCurrentTip();
        }

        bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            this.Hide();
            this.innerToolTipText.Clear();
            this.innerToolTipText = null;
        }

        public event DrawToolTipEventHandler Draw;
        public event DrawToolTipEventHandler BackgroundDraw;
        public event PopupEventHandler Popup;

        private class DerpedToolTipForm : Form
        {
            public DerpedToolTipForm() : base() { this.DoubleBuffered = true; }

            protected override bool ShowWithoutActivation { get { return true; } }
        }
    }
}
