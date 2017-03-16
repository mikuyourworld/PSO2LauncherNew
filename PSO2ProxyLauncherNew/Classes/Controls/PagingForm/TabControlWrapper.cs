using System;
using System.Drawing;
using PSO2ProxyLauncherNew.Classes.Events;
using System.Windows.Forms;
using System.ComponentModel;
using PSO2ProxyLauncherNew.Classes.Components;

namespace PSO2ProxyLauncherNew.Classes.Controls.PagingForm
{
    public class TabControlWrapper : TabControl
    {
        private System.Timers.Timer innerTimer;

        public bool IsInTransition { get { return this.innerTimer.Enabled; } }
        private QuickBitmap currentQB, nextQB;
        private int transitionCount, jump, offset;
        private DerpedTabPage innerTP;

        public TabControlWrapper() : base()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.innerTimer = new System.Timers.Timer(10);
            this.innerTimer.Enabled = false;
            this.innerTimer.AutoReset = true;
            this.innerTimer.SynchronizingObject = this;
            this.innerTimer.Elapsed += this.InnerTimer_Elapsed;
            this.innerTP = new DerpedTabPage();
            this.innerTP.Prepaint += InnerTP_Prepaint;
            this.Controls.Add(this.innerTP);
            //this.Appearance = TabAppearance.FlatButtons;
            //this.ItemSize = new Size(0, 1);
            //this.SizeMode = TabSizeMode.Fixed;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control != this.innerTP && this.TabPages[this.TabCount - 1] != this.innerTP)
            {
                this.Controls.Remove(this.innerTP);
                this.Controls.Add(this.innerTP);
            }
        }

        [Description("Occurs as a tab is being changed.")]
        public event EventHandler<TabPageChangeEventArgs> SelectedIndexChanging;

        protected override void WndProc(ref Message m)
        {
            // Hide tabs by trapping the TCM_ADJUSTRECT message
            if (m.Msg == 0x1328 && !DesignMode)
                m.Result = one;
            else
            {
                base.WndProc(ref m);
            }
        }

        public new TabPage SelectedTab
        {
            get { return base.SelectedTab; }
            set
            {
                if (this.innerTP != value)
                    base.SelectedTab = value;
            }
        }

        public new int SelectedIndex
        {
            get { return base.SelectedIndex; }
            set
            {
                if (base.SelectedIndex != value)
                    if (this.TabPages[value] != this.innerTP)
                        OnSelectedIndexChanging(new TabPageChangeEventArgs(SelectedTab, this.TabPages[value]));
                base.SelectedIndex = value;
            }
        }

        /*protected override void OnKeyDown(KeyEventArgs e)
        {

            if (m.Msg == 0x201)//WM_MOUSEDOWN
                {
                    TabPage tp = TestTab(new Point(m.LParam.ToInt32()));
                    if (tp != null)
                    {
                        if (tp.Enabled == false)
                            return;
                        else
                        {
                            TabPageChangeEventArgs ev = new TabPageChangeEventArgs(SelectedTab, tp);
                            OnSelectedIndexChanging(ev);
                            return;
                        }
                    }
                }

            if (e.KeyValue >= 37 && e.KeyValue <= 40)
            {
                e.Handled = true;

                TabPage tp = null;
                Rectangle r = GetTabRect(SelectedIndex);
                Point pt;

                bool foundNextTab = false;
                do
                {
                    if (e.KeyCode == Keys.Left)
                        pt = new Point(r.Left - 3, r.Top);
                    else if (e.KeyCode == Keys.Up)
                        pt = new Point(r.Left, r.Top - 3);
                    else if (e.KeyCode == Keys.Right)
                        pt = new Point(r.Right + 3, r.Top);
                    else
                        pt = new Point(r.Left, r.Bottom + 3);

                    tp = TestTab(pt);
                    if (tp != null)
                    {
                        foundNextTab = tp.Enabled;
                        r = GetTabRect(TabPages.IndexOf(tp));
                    }

                } while (tp != null && !foundNextTab);

                if (tp == null)
                    tp = GetNextEnabledTab(e.KeyValue > 38, false);

                if (tp != null && tp != SelectedTab)
                {
                    TabPageChangeEventArgs ev = new TabPageChangeEventArgs(SelectedTab, tp);
                    OnSelectedIndexChanging(ev);
                }//
            }

            base.OnKeyDown(e);
        }//*/

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.IsInTransition || (keyData == (Keys.Tab | Keys.Control)) || (keyData == (Keys.Tab | Keys.Shift | Keys.Control)))
            {
                return true;
                /*TabPage tp = GetNextEnabledTab((keyData & Keys.Shift) == Keys.None, true);
                if (tp != null)
                    if (tp != SelectedTab)
                    {
                        TabPageChangeEventArgs ev = new TabPageChangeEventArgs(SelectedTab, tp);
                        OnSelectedIndexChanging(ev);
                    }
                return true;//*/
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        protected virtual void OnSelectedIndexChanging(TabPageChangeEventArgs e)
        {
            SelectedIndexChanging?.Invoke(this, e);
            if (!e.Cancel)
            {
                if (!this.ClientSize.IsEmpty)
                {
                    if (this.currentQB != null)
                        this.currentQB.Dispose();
                    if (this.nextQB != null)
                        this.nextQB.Dispose();
                    this.transitionCount = 0;
                    if (SelectedTab != null)
                    {
                        this.currentQB = new QuickBitmap(SelectedTab.Size);
                        SelectedTab.DrawToBitmap(this.currentQB.Bitmap, SelectedTab.ClientRectangle);
                    }
                    this.nextQB = new QuickBitmap(e.NextTab.Size);
                    e.NextTab.DrawToBitmap(this.nextQB.Bitmap, e.NextTab.ClientRectangle);
                    this.jump = this.Width / 10;
                    this.destinationTab = e.NextTab;
                    this.innerTimer.Start();
                    base.SelectedTab = this.innerTP;
                }
                else
                    this.SelectedTab = e.NextTab;
            }
        }

        private TabPage destinationTab;

        private void InnerTP_Prepaint(object sender, PaintEventArgs e)
        {
            if (this.IsInTransition && this.SelectedTab != null)
            {
                this.offset = jump * this.transitionCount;
                if (this.currentQB != null)
                    e.Graphics.DrawImageUnscaled(this.currentQB.Bitmap, new Point(0 - this.offset, 0));
                e.Graphics.DrawImageUnscaled(this.nextQB.Bitmap, new Point(this.ClientSize.Width - this.offset, 0));
            }
        }

        private void InnerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.nextQB != null && !this.nextQB.Disposed)
                this.innerTP.Invalidate();
            this.transitionCount++;
            if (this.transitionCount >= 10)
            {
                if (this.currentQB != null)
                    this.currentQB.Dispose();
                if (this.nextQB != null)
                    this.nextQB.Dispose();
                this.Invalidate(false);
                this.SelectedTab = this.destinationTab;
                this.innerTimer.Stop();
            }
        }

        public new void Dispose()
        {
            if (this.innerTimer != null)
            {
                this.innerTimer.Stop();
                this.innerTimer.Dispose();
            }
            base.Dispose();
        }

        private TabPage TestTab(Point pt)
        {
            for (int index = 0; index <= TabCount - 1; index++)
                if (GetTabRect(index).Contains(pt.X, pt.Y))
                    return TabPages[index];
            return null;
        }


        private TabPage GetNextEnabledTab(bool forward, bool wrap)
        {
            if (forward)
            {
                for (int index = SelectedIndex + 1; index < TabCount; index++)
                    if (TabPages[index].Enabled)
                        return TabPages[index];

                if (wrap)
                    for (int index = 0; index < SelectedIndex; index++)
                        if (TabPages[index].Enabled)
                            return TabPages[index];
            }
            else
            {
                for (int index = SelectedIndex - 1; index >= 0; index--)
                    if (TabPages[index].Enabled)
                        return TabPages[index];

                if (wrap)
                    for (int index = TabCount - 1; index > SelectedIndex; index--)
                        if (TabPages[index].Enabled)
                            return TabPages[index];
            }
            return null;
        }
    }
}