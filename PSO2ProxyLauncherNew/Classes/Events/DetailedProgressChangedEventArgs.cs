using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class DetailedProgressChangedEventArgs : EventArgs
    {
        public int Current { get; }
        public int Total { get; }
        public DetailedProgressChangedEventArgs(int _current, int _total) : base()
        {
            this.Total = _total;
            this.Current = _current;
        }

        public DetailedProgressChangedEventArgs(int _current) : this(_current, 0) { }
    }
}
