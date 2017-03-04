using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PSO2ProxyLauncherNew.Classes.Components.Proxifier
{
    class ProxifierProxyServer
    {
        /*
         * <Proxy id="100" type="SOCKS5">
      <Address>45.32.45.35</Address>
      <Port>1080</Port>
      <Options>48</Options>
      <Authentication enabled="true">
        <Username>socks-user</Username>
        <Password>PleaseDontDownloadOrUpload</Password>
      </Authentication>
    </Proxy>
         */
        public int ID { get; }
        public ProxyType Type { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public int Options { get; set; }
        public bool Authentication { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ProxifierProxyServer() : this(-1) { }

        public ProxifierProxyServer(int _id)
        {
            this.ID = _id;
            this.Authentication = false;
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.Options = 48;
            this.Port = 1080;
            this.Type = ProxyType.SOCKS5;
        }

        public void WriteTo(XDocument xml)
        {
            bool found = false;
            var nodeProxyList = xml.XPathSelectElement("/ProxifierProfile/ProxyList");
            if (nodeProxyList != null)
                foreach (var node in nodeProxyList.Elements())
                    if (!string.IsNullOrWhiteSpace(node.Attribute("id").Value) && node.Attribute("id").Value == this.ID.ToString())
                    {
                        found = true;
                        node.SetValue(new XAttribute("type", this.Type.ToString()));
                        node.SetElementValue("Address", this.Address.ToString());
                        node.SetElementValue("Port", this.Port.ToString());
                        node.SetElementValue("Options", this.Options.ToString());
                        node.SetAttributeValue("Authentication", new XAttribute("enabled", this.Authentication.ToString().ToLower()));
                    }
            if (!found)
            {
                xml.Add(
                    new XElement("Proxy",
                    new XAttribute("id", this.ID.ToString()),
                    new XAttribute("type", this.Type.ToString()),
                        new XElement("Address", this.Address.ToString()),
                        new XElement("Port", this.Port.ToString()),
                        new XElement("Options", this.Options.ToString())
                    )
                );
            }
        }
    }
}
