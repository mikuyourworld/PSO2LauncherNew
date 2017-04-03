namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    public enum UpdateResult : short
    {
        Cancelled = -2,
        Failed = -1,
        Unknown = 0,
        Success = 1,
        MissingSomeFiles = 2
    }

    public enum PREPATCH_STATUS : byte
    {
        NONE,
        DOWNLOADING,
        APPLIED,
        UNKNOWN
    }
}
