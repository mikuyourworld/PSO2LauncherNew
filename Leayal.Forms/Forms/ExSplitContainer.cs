using Leayal.Drawing;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Leayal.Forms
{
    public class ExSplitContainer : SplitContainer
    {
        private int lastRatio = -1;
        public ExSplitContainer() : base()
        {
            this.SplitterMoved += this_SplitterMoved;
        }

        [Category("Layout")]
        public int SplitterRatio
        {
            get { return this.lastRatio; }
            set
            {
                if (!this.IsSplitterFixed)
                    switch (this.Orientation)
                    {
                        case Orientation.Horizontal:
                            base.SplitterDistance = this.GetValueFromPercent(value, this.ClientSize.Height);
                            break;
                        case Orientation.Vertical:
                            base.SplitterDistance = this.GetValueFromPercent(value, this.ClientSize.Width);
                            break;
                    }
            }
        }

        private int _panel1MinRatio;
        [Category("Layout"), DefaultValue(-1)]
        public int Panel1MinSizeRatio
        {
            get { return this._panel1MinRatio; }
            set
            {
                if (this._panel1MinRatio != value)
                {
                    this._panel1MinRatio = value;
                    if (value != -1)
                        switch (this.Orientation)
                        {
                            case Orientation.Horizontal:
                                base.Panel1MinSize = this.GetValueFromPercent(value, this.ClientSize.Height);
                                break;
                            case Orientation.Vertical:
                                base.Panel1MinSize = this.GetValueFromPercent(value, this.ClientSize.Width);
                                break;
                        }
                }
            }
        }



        private int _panel2MinRatio;
        [Category("Layout"), DefaultValue(-1)]
        public int Panel2MinSizeRatio
        {
            get { return this._panel2MinRatio; }
            set
            {
                if (this._panel2MinRatio != value)
                {
                    this._panel2MinRatio = value;
                    if (value != -1)
                        switch (this.Orientation)
                        {
                            case Orientation.Horizontal:
                                base.Panel2MinSize = this.GetValueFromPercent(value, this.ClientSize.Height);
                                break;
                            case Orientation.Vertical:
                                base.Panel2MinSize = this.GetValueFromPercent(value, this.ClientSize.Width);
                                break;
                        }
                }
            }
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            switch (this.Orientation)
            {
                case Orientation.Horizontal:
                    if (this._panel1MinRatio != -1)
                        base.Panel1MinSize = this.GetValueFromPercent(this._panel1MinRatio, this.ClientSize.Height);
                    if (this._panel2MinRatio != -1)
                        base.Panel2MinSize = this.GetValueFromPercent(this._panel2MinRatio, this.ClientSize.Height);
                    break;
                case Orientation.Vertical:
                    if (this._panel1MinRatio != -1)
                        base.Panel1MinSize = this.GetValueFromPercent(this._panel1MinRatio, this.ClientSize.Width);
                    if (this._panel2MinRatio != -1)
                        base.Panel2MinSize = this.GetValueFromPercent(this._panel2MinRatio, this.ClientSize.Width);
                    break;
            }
            base.OnClientSizeChanged(e);
        }

        private int GetValueFromPercent(int percent, int length)
        {
            return (int)((length / 100F) * percent);
        }

        private void this_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!this.IsSplitterFixed)
            {
                switch (this.Orientation)
                {
                    case Orientation.Horizontal:
                        this.CalcRatio(e.SplitY, this.ClientSize.Height);
                        break;
                    case Orientation.Vertical:
                        this.CalcRatio(e.SplitX, this.ClientSize.Width);
                        break;
                }
                
            }
        }

        private void CalcRatio(int num, int length)
        {
            int newnum = ((int)((100F * num) / length));
            if (newnum != this.lastRatio)
            {
                this.lastRatio = newnum;
                this.OnSplitterRatioChanged(System.EventArgs.Empty);
            }
        }

        public event EventHandler SplitterRatioChanged;
        protected virtual void OnSplitterRatioChanged(EventArgs e)
        {
            if (this.CanRaiseEvents)
                this.SplitterRatioChanged?.Invoke(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SplitterMoved -= this_SplitterMoved;
            }
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.myBGCache != null)
                    myBGCache.Dispose();
            }
        }

        public void GetNewCache()
        {
            if (!this.Size.IsEmpty)
            {
                this.BackgroundImage = null;
                if (myBGCache != null)
                    myBGCache.Dispose();
                myBGCache = new QuickBitmap(this.Width, this.Height);
                ButtonRenderer.DrawParentBackground(myBGCache.Graphics, this.DisplayRectangle, this);
                this.BackgroundImage = myBGCache.Bitmap;
            }
        }
        
        private QuickBitmap myBGCache;
    }
}
