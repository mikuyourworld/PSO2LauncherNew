namespace PSO2ProxyLauncherNew.Classes.Components
{
    public static class CacheStorage
    {
        private static Leayal.Net.CacheStorage _defaultstorage;
        public static Leayal.Net.CacheStorage DefaultStorage
        {
            get
            {
                if (_defaultstorage == null)
                    _defaultstorage = new Leayal.Net.CacheStorage(Infos.DefaultValues.MyInfo.Directory.Cache);
                return _defaultstorage;
            }
        }
    }
}
