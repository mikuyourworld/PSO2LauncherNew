using System;

namespace Leayal.Ugoira.WPF
{
    public class UgoiraPlayerCreatedEventArgs : EventArgs
    {
        public Exception Error { get; }
        public bool Cancelled { get; }
        public UgoiraPlayer UgoiraPlayer { get; }
        internal UgoiraPlayerCreatedEventArgs(Exception ex, bool cancel, UgoiraPlayer _player) : base()
        {
            this.Cancelled = cancel;
            this.UgoiraPlayer = _player;
            this.Error = ex;
        }
        public UgoiraPlayerCreatedEventArgs(UgoiraPlayer _player) : this(null, false, _player) { }
        public UgoiraPlayerCreatedEventArgs(Exception ex) : this(ex, false, null) { }
        public UgoiraPlayerCreatedEventArgs(bool cancel) : this(null, cancel, null) { }
    }
}
