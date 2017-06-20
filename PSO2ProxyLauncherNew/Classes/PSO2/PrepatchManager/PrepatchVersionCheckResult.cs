using System;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PrepatchManager
{
    class PrepatchVersionCheckResult
    {
        public Exception Error { get; }

        public PrepatchVersion Current { get; }
        public PrepatchVersion Latest { get; }

        public bool IsNewVersion => !this.Latest.Equals(this.Current);

        public bool IsPrepatchExisted
        {
            get
            {
                if (this.Error != null && this.Error is NoPrepatchExistedException)
                    return false;
                else
                    return true;
            }
        }

        public PrepatchVersionCheckResult(string latest, PrepatchVersion current) : this(latest, current, 1) { }

        public PrepatchVersionCheckResult(string latest, PrepatchVersion current, int prepatchListCount) :this(new PrepatchVersion(latest, prepatchListCount), current) { }

        public PrepatchVersionCheckResult(PrepatchVersion _latest, PrepatchVersion _current)
        {
            this.Current = _current;
            this.Latest = _latest;
            this.Error = null;
        }

        public PrepatchVersionCheckResult(Exception ex)
        {
            this.Error = ex;
        }
    }
}
