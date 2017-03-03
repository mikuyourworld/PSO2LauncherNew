using Newtonsoft.Json;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Proxy
{
    class PSO2ProxyConfigurationJsonObject
    {
        public string host { get; set; }
        public int version { get; set; }
        public string name { get; set; }
        public string publickeyurl { get; set; }

        public PSO2ProxyConfiguration ToPSO2ProxyConfiguration()
        {
            return new PSO2ProxyConfiguration(this.host, this.version, this.name, this.publickeyurl);
        }
    }
}
