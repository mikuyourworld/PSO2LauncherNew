using System;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls.PagingForm
{
    public class DerpedTabPage : TabPage
    {
        public DerpedTabPage() : base() { }
        public DerpedTabPage(string text) : base(text) { }

        public DerpedTabPage(TabPage tp) : base(tp.Text)
        {
            this.Name = tp.Name;
            this.ImageIndex = tp.ImageIndex;
            this.ImageKey = tp.ImageKey;
            this.BackgroundImageLayout = tp.BackgroundImageLayout;
            this.BackgroundImage = tp.BackgroundImage;
            this.BackColor = tp.BackColor;
            this.AutoScroll = tp.AutoScroll;
            this.Font = tp.Font;
            this.ForeColor = tp.ForeColor;
            this.Size = tp.Size;
            this.ToolTipText = tp.ToolTipText;
            this.Tag = tp.Tag;
            this.RightToLeft = tp.RightToLeft;
            this.Padding = tp.Padding;
            this.Margin = tp.Margin;
            this.ImeMode = tp.ImeMode;
            this.CausesValidation = tp.CausesValidation;
            this.ContextMenu = tp.ContextMenu;
            this.ContextMenuStrip = tp.ContextMenuStrip;
            foreach (Control ctl in tp.Controls)
                this.Controls.Add(ctl);
            tp.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Prepaint != null)
                this.Prepaint.Invoke(this, e);
            else
                base.OnPaint(e);
        }

        public event EventHandler<PaintEventArgs> Prepaint;
    }
}
