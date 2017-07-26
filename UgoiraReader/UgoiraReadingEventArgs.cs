using System;

namespace Leayal.Ugoira.WPF
{
    public class UgoiraReadingEventArgs : EventArgs
    {
        private int current;
        public int CurrentProgress => this.current;

        private int total;
        public int TotalProgress => this.total;

        internal void SetCurrent(int value)
        {
            this.current = value;
        }
        internal void SetTotal(int value)
        {
            this.total = value;
        }

        public bool Cancel { get; set; }

        internal UgoiraReadingEventArgs() : this(0) { }
        internal UgoiraReadingEventArgs(int totalprogress) : base()
        {
            this.Cancel = false;
            this.total = totalprogress;
        }
    }
}
