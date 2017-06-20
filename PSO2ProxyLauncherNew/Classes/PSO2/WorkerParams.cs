namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    internal class WorkerParams
    {
        public string PSO2Path { get; }
        public string NewVersionString { get; }
        public bool Installation { get; set; }
        public bool IgnorePrepatch { get; set; }

        public WorkerParams(string _pso2path, string latestversionstring, bool install, bool ignoreprepatch)
        {
            this.PSO2Path = _pso2path;
            this.NewVersionString = latestversionstring;
            this.Installation = install;
            this.IgnorePrepatch = IgnorePrepatch;
        }
        public WorkerParams(string _pso2path, string latestversionstring) : this(_pso2path, latestversionstring, false, false) { }
        public WorkerParams(string _pso2path) : this(_pso2path, string.Empty) { }
        public WorkerParams(string _pso2path, bool install) : this(_pso2path, string.Empty, install, false) { }
        public WorkerParams(string _pso2path, bool install, bool ignoreprepatch) : this(_pso2path, string.Empty, install, ignoreprepatch) { }
    }
}
