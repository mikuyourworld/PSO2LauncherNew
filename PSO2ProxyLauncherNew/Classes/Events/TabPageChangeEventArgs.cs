using System;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    public class TabPageChangeEventArgs : EventArgs
    {
        private TabPage _Selected = null;
        private TabPage _PreSelected = null;
        public bool Cancel = false;

        public TabPage CurrentTab
        {
            get { return _Selected; }
        }


        public TabPage NextTab
        {
            get { return _PreSelected; }
        }


        public override String ToString()
        {
            return String.Format("CurrentTab: {0}, NextTab: {1}", _Selected.ToString(), _PreSelected.ToString());
        }

        public TabPageChangeEventArgs(TabPage CurrentTab, TabPage NextTab)
        {
            _Selected = CurrentTab;
            _PreSelected = NextTab;
        }

    }
}
