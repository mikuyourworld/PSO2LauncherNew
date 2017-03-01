using System;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    class VersionCheckResult
    {
        public string LatestVersion { get; }
        public string CurrentVersion { get; }
        public bool IsNewVersionFound { get; }
        public Exception Error { get; }
        public VersionCheckResult(string latest, string current)
        {
            this.LatestVersion = latest;
            this.CurrentVersion = current;
            if (latest.ToLower() == current.ToLower())
                this.IsNewVersionFound = false;
            else
                this.IsNewVersionFound = true;
            this.Error = null;
        }

        public VersionCheckResult(Exception ex) : this(string.Empty, string.Empty)
        {
            this.Error = ex;
        }
    }
}
