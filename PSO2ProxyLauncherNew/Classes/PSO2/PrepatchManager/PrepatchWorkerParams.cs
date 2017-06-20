namespace PSO2ProxyLauncherNew.Classes.PSO2.PrepatchManager
{
    internal class PrepatchWorkerParams
    {
        public string PSO2Path { get; }
        public PrepatchVersion NewVersion { get; }
        public bool Force { get; set; }
        public PrepatchWorkerParams(string _pso2path, PrepatchVersion latestversion, bool _force)
        {
            this.PSO2Path = _pso2path;
            this.NewVersion = latestversion;
            this.Force = _force;
        }
        public PrepatchWorkerParams(string _pso2path, PrepatchVersion latestversion) : this(_pso2path, latestversion, false) { }
        public PrepatchWorkerParams(string _pso2path) : this(_pso2path, new PrepatchVersion()) { }
        public PrepatchWorkerParams(string _pso2path, bool _force) : this(_pso2path, new PrepatchVersion(), _force) { }
    }
}
