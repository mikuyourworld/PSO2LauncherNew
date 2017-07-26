using System;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    class PSO2VersionCheckResult : VersionCheckResult
    {
        public string PatchURL { get; }
        public string MasterURL { get; }

        public PSO2VersionCheckResult(string latest, string current, string _masterurl, string _patchurl) : base(latest, current)
        {
            this.PatchURL = _patchurl;
            this.MasterURL = _masterurl;
        }

        public PSO2VersionCheckResult(Exception ex) : base(ex) { }
    }
}
