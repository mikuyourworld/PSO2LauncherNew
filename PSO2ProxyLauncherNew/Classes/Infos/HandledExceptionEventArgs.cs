using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    public class HandledExceptionEventArgs : EventArgs
    {
        public Exception Error { get; private set; }
        public HandledExceptionEventArgs(Exception ex):base()
        {
            this.Error = ex;
        }

        public override string ToString()
        {
            return this.Error.Message;
        }
    }
}
