using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Leayal.Forms
{
    public class ExtendedToolTip : Component
    {

        private DerpedToolTipForm currentTooltip;
        private System.Windows.Forms.Control currentControl;
        private Dictionary<System.Windows.Forms.Control, string> innerToolTipText;

        public Font Font { get; set; }
        public Size PreferedSize { get; set; }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color FormColor { get; set; }
        public double Opacity { get; set; }
        public bool UseFading { get; set; }

        public ExtendedToolTip()
        {
            this.Font = System.Windows.Forms.Form.DefaultFont;
            this.BackColor = System.Windows.Forms.Form.DefaultBackColor;
            this.ForeColor = System.Windows.Forms.Form.DefaultForeColor;
            this.Opacity = 100F;
            this.FormColor = Color.Lavender;
            this.UseFading = true;
            this.innerToolTipText = new Dictionary<System.Windows.Forms.Control, string>();
        }

        private void ShowToolTip(DerpedToolTipForm _form)
        {
            _form.Show();
        }

        public void SetToolTip(System.Windows.Forms.Control c, string tooltipText)
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
            System.Windows.Forms.Control c = sender as System.Windows.Forms.Control;
            if (c != null && c != this.currentControl)
            {
                this.CloseCurrentTip();
                this.currentControl = c;
                this.currentTooltip = this.SetupAnotherTip();
                var resultinfo = TextRendererWrapper.WrapString(this.innerToolTipText[c], this.PreferedSize.Width, this.Font, TextFormatFlags.Left);
                this.currentTooltip.ExTooltipText = resultinfo.Result;
                this.currentTooltip.Tag = c;

                Point awgkaugw = System.Windows.Forms.Cursor.Position;
                awgkaugw.Offset(3, 3);

                ExPopupEventArgs arrrrgggg = new ExPopupEventArgs(this.currentTooltip, c, false, new Size(resultinfo.Size.Width + 2, resultinfo.Size.Height + 2), awgkaugw);
                this.Popup?.Invoke(this.currentTooltip, arrrrgggg);
                this.currentTooltip.ClientSize = arrrrgggg.ToolTipSize;
                
                this.currentTooltip.DesktopLocation = arrrrgggg.Location;
                this.currentTooltip.Opacity = this.Opacity;
                this.ShowToolTip(this.currentTooltip);
            }
        }

        public string GetToolTipText(System.Windows.Forms.Control c)
        {
            string result = string.Empty;
            if (this.innerToolTipText.ContainsKey(c))
                result = this.innerToolTipText[c];
            return result;
        }

        private DerpedToolTipForm SetupAnotherTip()
        {
            var result = new DerpedToolTipForm(this.UseFading);
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
            System.Windows.Forms.Control c = sender as System.Windows.Forms.Control;
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
            {
                fffff.ClientRectangle.Offset(1, 1);
                fffff.ClientRectangle.Inflate(-2, -2);
                var eargs = new DrawToolTipEventArgs(e.Graphics, fffff, (System.Windows.Forms.Control)fffff.Tag, fffff.ClientRectangle, fffff.ExTooltipText, this.BackColor, fffff.ForeColor, fffff.Font);
                if (this.Draw == null)
                {
                    eargs.DrawBorder();
                    System.Windows.Forms.TextRenderer.DrawText(e.Graphics, fffff.ExTooltipText, fffff.Font, fffff.ClientRectangle.Location, this.ForeColor);
                }
                else
                    this.Draw.Invoke(sender, eargs);
            }
        }

        private void CurrentTooltip_PaintBackground(object sender, PaintEventArgs e)
        {
            DerpedToolTipForm fffff = sender as DerpedToolTipForm;
            if (fffff != null)
                this.BackgroundDraw?.Invoke(sender, new DrawToolTipEventArgs(e.Graphics, fffff, (System.Windows.Forms.Control)fffff.Tag, fffff.ClientRectangle, fffff.ExTooltipText, fffff.BackColor, fffff.ForeColor, fffff.Font));
        }

        public void RemoveToolTip(System.Windows.Forms.Control c)
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

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_disposed) return;
                _disposed = true;
                this.Hide();
                while (this.innerToolTipText.Count > 0)
                    this.RemoveToolTip(this.innerToolTipText.Keys.First());
                this.innerToolTipText = null;
            }
        }

        public event DrawToolTipEventHandler Draw;
        public event DrawToolTipEventHandler BackgroundDraw;
        public event EventHandler<ExPopupEventArgs> Popup;

        private class DerpedToolTipForm : System.Windows.Forms.Form
        {
            //private static int SW_SHOWNOACTIVATE = 4;
            int nopeCount;
            bool _useFading;

            public string ExTooltipText { get; set; }

            public DerpedToolTipForm(bool useFading) : base()
            {
                this.ExTooltipText = string.Empty;
                this.DoubleBuffered = true;
                this.ControlBox = false;
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this._useFading = useFading;
                if (useFading)
                {
                    this.myInnerTimer = new System.Timers.Timer(30);
                    this.myInnerTimer.SynchronizingObject = this;
                    this.myInnerTimer.Enabled = false;
                    this.myInnerTimer.AutoReset = true;
                    this.myInnerTimer.Elapsed += MyInnerTimer_Elapsed;
                    this.Opacity = 0;
                    base.Opacity = 0;
                    this.nopeCount = 0;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.myInnerTimer != null)
                    {
                        this.myInnerTimer.Stop();
                        this.myInnerTimer.Dispose();
                    }
                }
                base.Dispose(disposing);
            }

            protected override void OnShown(EventArgs e)
            {
                if (this.myInnerTimer != null)
                    this.myInnerTimer.Start();
                base.OnShown(e);
                ShowInactiveTopmost();
            }

            private void ShowInactiveTopmost()
            {
                //ShowWindow(this.Handle, SW_SHOWNOACTIVATE);
                SafeNativeMethods.SetWindowPos(this.Handle, SafeNativeMethods.HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, SafeNativeMethods.SWP_NOACTIVATE);
            }

            protected override void OnFormClosing(FormClosingEventArgs e)
            {
                if (this.myInnerTimer != null)
                {
                    this.myInnerTimer.Stop();
                    this.myInnerTimer.Dispose();
                }
                base.OnFormClosing(e);
            }

            private void MyInnerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                if (this.t_Opacity == base.Opacity)
                {
                    this.myInnerTimer.Stop();
                    this.nopeCount = 0;
                }
                else
                {
                    if (this.t_Opacity < base.Opacity)
                        this.RealOpacity -= jump;
                    else
                        this.RealOpacity += jump;
                    Interlocked.Increment(ref this.nopeCount);
                    if (this.nopeCount > 10)
                    {
                        this.RealOpacity = this.Opacity;
                        this.nopeCount = 0;
                    }
                }
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    var Params = base.CreateParams;
                    Params.ExStyle |= 0x80;
                    return Params;
                }
            }

            private double RealOpacity
            {
                get { return base.Opacity; }
                set { base.Opacity = value; }
            }

            protected override bool ShowWithoutActivation { get { return true; } }

            System.Timers.Timer myInnerTimer;
            private double jump;

            private double t_Opacity = 0;
            public new double Opacity
            {
                get
                {
                    if (_useFading)
                        return this.t_Opacity;
                    else
                        return this.RealOpacity;
                }
                set
                {
                    if (_useFading)
                    {
                        this.t_Opacity = value;
                        this.jump = Math.Abs(value - base.Opacity) / 10;
                        this.nopeCount = 0;
                        this.myInnerTimer.Start();
                    }
                    else
                        this.RealOpacity = value;
                }
            }
        }
    }
}
