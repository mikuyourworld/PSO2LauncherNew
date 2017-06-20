using System;

namespace PSO2ProxyLauncherNew.Classes.Events
{
    class ValidPrepatchPromptEventArgs : EventArgs
    {
        public bool Use { get; set; }
        public ValidPrepatchPromptEventArgs(bool _use) : base()
        {
            this.Use = _use;
        }

        public ValidPrepatchPromptEventArgs() : this(true) { }
    }

    class InvalidPrepatchPromptEventArgs : EventArgs
    {
        public bool Delete { get; set; }
        public InvalidPrepatchPromptEventArgs(bool _delete) : base()
        {
            this.Delete = _delete;
        }

        public InvalidPrepatchPromptEventArgs() : this(false) { }
    }
}
