using System.Threading;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Forms
{
    class AnotherExtendedForm : Form
    {
        private bool _isclosed;
        public bool IsClosed { get { return this._isclosed; } }
        public SynchronizationContext SyncContext { get; }
        public AnotherExtendedForm() : base() { this.SyncContext = SynchronizationContext.Current; this.DoubleBuffered = true; }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            this._isclosed = true;
            base.OnFormClosed(e);
        }
    }
}
