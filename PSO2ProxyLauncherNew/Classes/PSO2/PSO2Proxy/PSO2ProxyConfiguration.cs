using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Proxy
{
    class PSO2ProxyConfiguration
    {
        public string Host { get; }
        public Versions Version { get; }
        public string Name { get; }
        public Uri PublickeyURL { get; }

        public PSO2ProxyConfiguration(string _host, int _version, string _name, string _publickeyurl)
        {
            this.Host = _host;
            this.Name = _name;
            this.PublickeyURL = new Uri(_publickeyurl);
            switch (_version)
            {
                case 2:
                    this.Version = Versions.VersionTelepipe;
                    break;
                default:
                    this.Version = Versions.VersionPSO2Proxy;
                    break;
            }
        }

        public override string ToString()
        {
            return $"Host: '{this.Host}'\nVersion: '{this.Version.ToString()}'";
        }
    }
}
