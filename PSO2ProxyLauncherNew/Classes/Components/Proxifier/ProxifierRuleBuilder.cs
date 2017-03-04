using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes.Components.Proxifier
{
    class ProxifierRuleBuilder
    {
        private ProxifierProfile innerProfile;

        public bool AutoModeDetection
        {
            get { return this.innerProfile.AutoModeDetection; }
            set { this.innerProfile.AutoModeDetection = value; }
        }

        public bool ViaProxy
        {
            get { return this.innerProfile.ViaProxy; }
            set { this.innerProfile.ViaProxy = value; }
        }

        public bool TryLocalDnsFirst
        {
            get { return this.innerProfile.TryLocalDnsFirst; }
            set { this.innerProfile.TryLocalDnsFirst = value; }
        }

        public string ExclusionList
        {
            get { return this.innerProfile.ExclusionList; }
            set { this.innerProfile.ExclusionList = value; }
        }

        public EncryptionMode Encryption
        {
            get { return this.innerProfile.Encryption; }
            set { this.innerProfile.Encryption = value; }
        }

        public bool HttpProxiesSupport
        {
            get { return this.innerProfile.HttpProxiesSupport; }
            set { this.innerProfile.HttpProxiesSupport = value; }
        }

        public bool HandleDirectConnections
        {
            get { return this.innerProfile.HandleDirectConnections; }
            set { this.innerProfile.HandleDirectConnections = value; }
        }

        public bool ConnectionLoopDetection
        {
            get { return this.innerProfile.ConnectionLoopDetection; }
            set { this.innerProfile.ConnectionLoopDetection = value; }
        }

        public bool ProcessServices
        {
            get { return this.innerProfile.ProcessServices; }
            set { this.innerProfile.ProcessServices = value; }
        }

        public bool ProcessOtherUsers
        {
            get { return this.innerProfile.ProcessOtherUsers; }
            set { this.innerProfile.ProcessOtherUsers = value; }
        }

        public ProxifierRuleBuilder()
        {
            this.innerProfile = new ProxifierProfile();            
        }

        public void Load(string filepath)
        {
            using (StreamReader sr = new StreamReader(filepath))
                this.Load(sr);
        }

        public void Load(Stream _stream)
        {
            using (StreamReader sr = new StreamReader(_stream))
                this.Load(sr);
        }

        public void Load(TextReader xmlReader)
        {
            XDocument innerXML =  XDocument.Load(xmlReader);
            this.innerProfile.Clear();
            if (innerXML != null)
            {
                XElement ProxifierProfile = innerXML.Element("ProxifierProfile");
                if (ProxifierProfile != null)
                {
                    XElement currentE;
                    var b = ProxifierProfile.Element("Options");
                    if (b != null)
                    {
                        currentE = b.Element("Resolve");
                        if (currentE != null)
                        {
                            this.AutoModeDetection = Parse(currentE.Element("AutoModeDetection").Attribute("enabled"), false);
                            this.ViaProxy = Parse(currentE.Element("ViaProxy").Attribute("enabled"), false);
                            this.TryLocalDnsFirst = Parse(currentE.Element("ViaProxy").Element("TryLocalDnsFirst").Attribute("enabled"), false);
                            this.ExclusionList = Parse(currentE.Element("ExclusionList"), "%ComputerName%; localhost; *.local");
                        }
                        this.Encryption = Parse<EncryptionMode>(b.Element("Encryption").Attribute("mode"), EncryptionMode.disabled);
                        this.HttpProxiesSupport = Parse(b.Element("HttpProxiesSupport").Attribute("enabled"), false);
                        this.HandleDirectConnections = Parse(b.Element("HandleDirectConnections").Attribute("enabled"), false);
                        this.ConnectionLoopDetection = Parse(b.Element("ConnectionLoopDetection").Attribute("enabled"), true);
                        this.ProcessServices = Parse(b.Element("ProcessServices").Attribute("enabled"), true);
                        this.ProcessOtherUsers = Parse(b.Element("ProcessOtherUsers").Attribute("enabled"), true);
                    }

                    Dictionary<int, int> transition = new Dictionary<int, int>();

                    XElement _ProxyList = ProxifierProfile.Element("ProxyList");
                    ProxifierProxyServer currentServer;

                    int newIDServer;

                    if (_ProxyList != null)
                        foreach (var c in _ProxyList.Elements())
                        {
                            newIDServer = -1;
                            currentServer = new ProxifierProxyServer(Parse(c.Attribute("id"), -1));
                            currentServer.Type = Parse<ProxyType>(c.Attribute("type"), ProxyType.SOCKS5);
                            currentServer.Address = Parse(c.Element("Address"), string.Empty);
                            currentServer.Port = Parse(c.Element("Port"), 1080);
                            currentServer.Options = Parse(c.Element("Options"), 48);
                            currentE = c.Element("Authentication");
                            if (currentE != null)
                            {
                                currentServer.Authentication = Parse(currentE.Attribute("enabled"), false);
                                currentServer.Username = Parse(currentE.Element("Username"), string.Empty);
                                currentServer.Password = Parse(currentE.Element("Password"), string.Empty);
                            }
                            newIDServer = this.innerProfile.AddProxy(currentServer);
                            if (currentServer.ID >= 100)
                                if (currentServer.ID != newIDServer)
                                    transition.Add(currentServer.ID, newIDServer);
                        }

                    XElement _RuleList = ProxifierProfile.Element("RuleList");
                    ProxifierRule currentRule;
                    if (_RuleList != null)
                        foreach (var c in _RuleList.Elements())
                        {
                            currentRule = new ProxifierRule(Parse(c.Attribute("enabled"), true));
                            currentRule.Name = Parse(c.Element("ProxifierRule"), string.Empty);
                            currentRule.Ports = Parse(c.Element("Ports"), string.Empty);
                            currentRule.Applications = Parse(c.Element("Applications"), string.Empty);
                            currentRule.Targets = Parse(c.Element("Targets"), string.Empty);
                            currentRule.Action.Type = Parse<ActionType>(c.Element("Action").Attribute("type"), ActionType.Direct);
                            if (currentRule.Action.Type != ActionType.Direct && currentRule.Action.Type != ActionType.Block)
                            {
                                newIDServer = Parse(c.Element("Action"), -1);
                                if (transition.ContainsKey(newIDServer))
                                    currentRule.Action.ServerID = transition[newIDServer];
                                else
                                    currentRule.Action.ServerID = newIDServer;
                            }
                            this.innerProfile.AddRule(currentRule);
                        }
                }
            }
        }

        private T Parse<T>(XAttribute c, T defaultValue)
        {
            if (c != null)
            {
                try
                {
                    
                    T result = (T)Enum.Parse(typeof(T), c.Value, true);
                    return result;
                }
                catch
                { return defaultValue; }
            }
            else
                return defaultValue;
        }

        private T Parse<T>(XElement c, T defaultValue)
        {
            if (c != null)
            {
                try
                {
                    T result = (T)Enum.Parse(typeof(T), c.Value, true);
                    return result;
                }
                catch
                { return defaultValue; }
            }
            else
                return defaultValue;
        }

        private int Parse(XElement c, int defaultValue)
        {
            if (c != null)
            {
                int result = defaultValue;
                if (int.TryParse(c.Value, out result))
                    return result;
                else
                    return defaultValue;
            }
            else
                return defaultValue;
        }

        private bool Parse(XElement c, bool defaultValue)
        {
            if (c != null)
            {
                bool result = defaultValue;
                if (bool.TryParse(c.Value, out result))
                    return result;
                else
                    return defaultValue;
            }
            else
                return defaultValue;
        }

        private string Parse(XElement c, string defaultValue)
        {
            if (c != null)
                return c.Value;
            else
                return defaultValue;
        }

        private bool Parse(XAttribute c, bool defaultValue)
        {
            if (c != null)
            {
                bool result = defaultValue;
                if (bool.TryParse(c.Value, out result))
                    return result;
                else
                    return defaultValue;
            }
            else
                return defaultValue;
        }

        private string Parse(XAttribute c, string defaultValue)
        {
            if (c != null)
                return c.Value;
            else
                return defaultValue;
        }

        public List<ProxifierRule> RuleList { get { return this.innerProfile.RuleList; } }
        public Dictionary<int, ProxifierProxyServer> ProxyList { get { return this.innerProfile.ProxyList; } }

        public void AddProxy(ProxifierProxyServer proxy)
        {
            this.innerProfile.AddProxy(proxy);
        }

        public void AddProxy(string address, int port, ProxyType proxyType)
        {
            this.innerProfile.AddProxy(address, port, proxyType);
        }

        public void AddProxy(ProxifierRule rule)
        {
            this.innerProfile.AddRule(rule);
        }

        public bool AddRule(bool enabled, string ruleName, string targetAddress, string _applications, string _ports, ActionType actionType, int proxyID = 100)
        {
            return this.innerProfile.AddRule(enabled, ruleName, targetAddress, _applications, _ports, actionType, proxyID);
        }

        public void Clear()
        {
            this.innerProfile.Clear();
        }

        public override string ToString()
        {
            return this.innerProfile.ToString();
        }

        public void WriteTo(string path)
        {
            this.innerProfile.SaveAs(path);
        }

        public void WriteTo(Stream _stream)
        {
            using (StreamWriter sw = new StreamWriter(_stream))
                this.WriteTo(sw);
        }

        public void WriteTo(TextWriter xmlWriter)
        {
            this.innerProfile.SaveAs(xmlWriter);
        }

        public ProxifierProfile ToProfile()
        {
            return this.innerProfile;
        }
    }
}
