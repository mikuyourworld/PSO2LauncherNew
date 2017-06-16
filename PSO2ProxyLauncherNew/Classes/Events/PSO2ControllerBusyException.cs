using System.ComponentModel;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class PSO2ControllerBusyException : InvalidAsynchronousStateException
    {
        public PSO2ControllerBusyException() : base() { }
        public PSO2ControllerBusyException(string message) : base(message) { }
    }
}
