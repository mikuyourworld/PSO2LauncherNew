using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.Components.Proxifier
{
    class ProxifierProfile
    {
        private int _proxyCount;
        private ActionType defaultRuleActionType;
        private int defaultRuleProxyID;
        // List
        private List<ProxifierRule> innerRuleList;
        private Dictionary<int, ProxifierProxyServer> innerProxyList;
        private List<object[]> innerChainList;
        // Parameters
        private bool _AutoModeDetection;
        private bool _ViaProxy;
        private bool _TryLocalDnsFirst;
        private string _ExclusionList;
        private EncryptionMode _Encryption;
        private bool _HttpProxiesSupport;
        private bool _HandleDirectConnections;
        private bool _ConnectionLoopDetection;
        private bool _ProcessServices;
        private bool _ProcessOtherUsers;

        private StringBuilder sb_ProxyList, sb_ProxyChainList, sb_RuleList, sb_Options, sbOut;

        public List<ProxifierRule> RuleList { get { return this.innerRuleList; } }
        public Dictionary<int, ProxifierProxyServer> ProxyList { get { return this.innerProxyList; } }

        public int proxyCount
        {
            get { return _proxyCount; }
            set { _proxyCount = value; }
        }

        public bool AutoModeDetection
        {
            get { return _AutoModeDetection; }
            set { _AutoModeDetection = value; }
        }

        public bool ViaProxy
        {
            get { return _ViaProxy; }
            set { _ViaProxy = value; }
        }

        public bool TryLocalDnsFirst
        {
            get { return _TryLocalDnsFirst; }
            set { _TryLocalDnsFirst = value; }
        }

        public string ExclusionList
        {
            get { return _ExclusionList; }
            set { _ExclusionList = value; }
        }

        public EncryptionMode Encryption
        {
            get { return _Encryption; }
            set { _Encryption = value; }
        }

        public bool HttpProxiesSupport
        {
            get { return _HttpProxiesSupport; }
            set { _HttpProxiesSupport = value; }
        }

        public bool HandleDirectConnections
        {
            get { return _HandleDirectConnections; }
            set { _HandleDirectConnections = value; }
        }

        public bool ConnectionLoopDetection
        {
            get { return _ConnectionLoopDetection; }
            set { _ConnectionLoopDetection = value; }
        }

        public bool ProcessServices
        {
            get { return _ProcessServices; }
            set { _ProcessServices = value; }
        }

        public bool ProcessOtherUsers
        {
            get { return _ProcessOtherUsers; }
            set { _ProcessOtherUsers = value; }
        }

        public ProxifierProfile()
        {
            this.sb_ProxyList = new StringBuilder();
            this.sb_ProxyChainList = new StringBuilder();
            this.sb_RuleList = new StringBuilder();
            this.sbOut = new StringBuilder();
            this.sb_Options = new StringBuilder();
            proxyCount = 100;
            AutoModeDetection = false;
            ViaProxy = false;
            TryLocalDnsFirst = false;
            ExclusionList = "%ComputerName%; localhost; *.local";
            Encryption = EncryptionMode.disabled;
            HttpProxiesSupport = false;
            HandleDirectConnections = false;
            ConnectionLoopDetection = true;
            ProcessServices = true;
            ProcessOtherUsers = true;
            defaultRuleActionType = ActionType.Direct;
            defaultRuleProxyID = 100;
            innerRuleList = new List<ProxifierRule>();
            innerProxyList = new Dictionary<int, ProxifierProxyServer>();
            innerChainList = new List<object[]>();
            this.AddRule(true, "Localhost", "localhost; 127.0.0.1; %ComputerName%;", null, null, ActionType.Direct);
        }

        public void Clear()
        {
            proxyCount = 100;
            AutoModeDetection = false;
            ViaProxy = true;
            TryLocalDnsFirst = false;
            ExclusionList = "%ComputerName%; localhost; *.local";
            Encryption = EncryptionMode.disabled;
            HttpProxiesSupport = false;
            HandleDirectConnections = false;
            ConnectionLoopDetection = true;
            ProcessServices = true;
            ProcessOtherUsers = true;
            defaultRuleActionType = ActionType.Direct;
            defaultRuleProxyID = 100;
            innerRuleList.Clear();
            innerProxyList.Clear();
            innerChainList.Clear();
            this.AddRule(true, "Localhost", "localhost; 127.0.0.1; %ComputerName%;", null, null, ActionType.Direct);
        }

        public int AddProxy(ProxifierProxyServer proxy)
        {
            this.innerProxyList.Add(this.proxyCount, proxy);
            this.proxyCount++;
            return (this.proxyCount - 1);
        }

        public int AddProxy(string address, int port, ProxyType proxyType)
        {
            return this.AddProxy(new ProxifierProxyServer() { Address = address, Port = port, Type = proxyType });
        }

        public bool AddRule(bool enabled, string ruleName, string targetAddress, string Applications, string Ports, ActionType actionType, ProxifierProxyServer proxy)
        {
            if (this.innerProxyList.ContainsValue(proxy))
                return this.AddRule(enabled, ruleName, targetAddress, Applications, Ports, actionType, proxy.ID);
            else
                return this.AddRule(enabled, ruleName, targetAddress, Applications, Ports, actionType);
        }

        public bool AddRule(ProxifierRule rule)
        {
            if (!this.innerRuleList.Contains(rule))
            {
                this.innerRuleList.Add(rule);
                return true;
            }
            return false;
        }

        public bool AddRule(bool enabled, string ruleName, string targetAddress, string _applications, string _ports, ActionType actionType, int proxyID = 100)
        {
            if (ruleName.ToLower() != "default" && this.innerProxyList.ContainsKey(proxyID))
            {
                var currentRule = new ProxifierRule(enabled, ruleName) { Targets = targetAddress, Applications = _applications, Ports = _ports };
                currentRule.Action.Type = actionType;
                currentRule.Action.ServerID = proxyID;                
                this.innerRuleList.Add(currentRule);
                return true;
            }
            else
                return false;
        }

        public int AddProxyChain(ChainType chainType, string chainName, List<KeyValuePair<int, bool>> proxyIDList, int RedundancyTimeout = 30, bool RedundancyTryDirect = false)
        {
            this.proxyCount += 1;
            List<KeyValuePair<int, bool>> proxyPairs = new List<KeyValuePair<int, bool>>();
            foreach (KeyValuePair<int, bool> proxyPair in proxyIDList)
                proxyPairs.Add(new KeyValuePair<int, bool>(proxyPair.Key, proxyPair.Value));
            this.innerChainList.Add(new object[]{
                this.proxyCount.ToString(), // [0] ID Chain
                chainType.ToString(),       // [1] Chain type
                chainName,                  // [2] Chain name
                proxyPairs,                 // [3] Proxies + status
                RedundancyTimeout,          // [4] RedundancyTimeout
                RedundancyTryDirect         // [5] RedundancyTryDirect
            });
            return this.proxyCount;
        }

        private string profileOptions()
        {
            this.sb_Options.Clear();
            sb_Options.Append("  <Options>");
            sb_Options.Append("\r\n    <Resolve>");
            sb_Options.Append("\r\n      <AutoModeDetection enabled=\"" + this.AutoModeDetection.ToString().ToLower() + "\" />");
            sb_Options.Append("\r\n      <ViaProxy enabled=\"" + this.ViaProxy.ToString().ToLower() + "\">");
            sb_Options.Append("\r\n        <TryLocalDnsFirst enabled=\"" + this.TryLocalDnsFirst.ToString().ToLower() + "\" />");
            sb_Options.Append("\r\n      </ViaProxy>");
            sb_Options.Append("\r\n      <ExclusionList>" + this.ExclusionList + "</ExclusionList>");
            sb_Options.Append("\r\n    </Resolve>");
            sb_Options.Append("\r\n    <Encryption mode=\"" + this.Encryption.ToString() + "\" />");
            sb_Options.Append("\r\n    <HttpProxiesSupport enabled=\"" + this.HttpProxiesSupport.ToString().ToLower() + "\" />");
            sb_Options.Append("\r\n    <HandleDirectConnections enabled=\"" + this.HandleDirectConnections.ToString().ToLower() + "\" />");
            sb_Options.Append("\r\n    <ConnectionLoopDetection enabled=\"" + this.ConnectionLoopDetection.ToString().ToLower() + "\" />");
            sb_Options.Append("\r\n    <ProcessServices enabled=\"" + this.ProcessServices.ToString().ToLower() + "\" />");
            sb_Options.Append("\r\n    <ProcessOtherUsers enabled=\"" + this.ProcessOtherUsers.ToString().ToLower() + "\" />");
            sb_Options.Append("\r\n  </Options>");
            return this.sb_Options.ToString();
        }

        private string profileProxyList()
        {
            this.sb_ProxyList.Clear();
            this.sb_ProxyList.Append("  <ProxyList>");
            foreach (var proxyData in this.innerProxyList)
            {
                this.sb_ProxyList.Append("\r\n    <Proxy id=\"" + proxyData.Key.ToString() + "\" type=\"" + proxyData.Value.Type.ToString() + "\">");
                this.sb_ProxyList.Append("\r\n      <Address>" + proxyData.Value.Address + "</Address>");
                this.sb_ProxyList.Append("\r\n      <Port>" + proxyData.Value.Port + "</Port>");
                this.sb_ProxyList.Append("\r\n      <Options>48</Options>");
                if (proxyData.Value.Authentication)
                {
                    this.sb_ProxyList.Append("\r\n      <Authentication enabled=\"" + proxyData.Value.Authentication.ToString().ToLower() + "\">");
                    this.sb_ProxyList.Append("\r\n         <Username>" + proxyData.Value.Username + "</Username>");
                    this.sb_ProxyList.Append("\r\n         <Password>" + proxyData.Value.Password + "</Password>");
                    this.sb_ProxyList.Append("\r\n      </Authentication>");
                }
                this.sb_ProxyList.Append("\r\n    </Proxy>");
            }
            this.sb_ProxyList.Append("\r\n  </ProxyList>");
            return this.sb_ProxyList.ToString();
        }

        private string profileChain()
        {
            this.sb_ProxyChainList.Clear();
            this.sb_ProxyChainList.Append("  <ChainList>");
            foreach (object[] chainData in this.innerChainList)
            {
                this.sb_ProxyChainList.Append("\r\n    <Chain id=\"" + chainData[0].ToString() + "\" type=\"" + chainData[1].ToString() + "\">");
                this.sb_ProxyChainList.Append("\r\n      <Name>" + chainData[2].ToString() + "</Name>");
                foreach (KeyValuePair<int, bool> proxyPair in chainData[3] as List<KeyValuePair<int, bool>>)
                {
                    this.sb_ProxyChainList.Append("\r\n      <Proxy enabled=\"" + proxyPair.Value.ToString().ToLower() + "\">" + proxyPair.Key.ToString() + "</Proxy>");
                }
                if (chainData[1].ToString() == ChainType.redundancy.ToString())
                {
                    this.sb_ProxyChainList.Append("\r\n      <RedundancyTimeout>" + chainData[4].ToString() + "</RedundancyTimeout>");
                    this.sb_ProxyChainList.Append("\r\n      <RedundancyTryDirect>" + chainData[5].ToString().ToLower() + "</RedundancyTryDirect>");
                }
                this.sb_ProxyChainList.Append("\r\n    </Chain>");
            }
            this.sb_ProxyChainList.Append("\r\n  </ChainList>");
            return this.sb_ProxyChainList.ToString();
        }

        private string profileRuleList()
        {
            this.sb_RuleList.Clear();
            foreach (ProxifierRule rulesOpt in this.innerRuleList)
            {
                this.sb_RuleList.Append("\r\n    <Rule enabled=\"" + rulesOpt.Enabled.ToString().ToLower() + "\">");
                this.sb_RuleList.Append("\r\n      <Name>" + rulesOpt.Name + "</Name>");
                if (string.IsNullOrWhiteSpace(rulesOpt.Targets))
                    this.sb_RuleList.Append("\r\n      <Targets>" + rulesOpt.Targets + "</Targets>");
                if (string.IsNullOrWhiteSpace(rulesOpt.Applications))
                    this.sb_RuleList.Append("\r\n      <Applications>" + rulesOpt.Applications + "</Applications>");
                if (string.IsNullOrWhiteSpace(rulesOpt.Ports))
                    this.sb_RuleList.Append("\r\n      <Ports>" + rulesOpt.Ports + "</Ports>");
                switch (rulesOpt.Action.Type)
                {
                    case ActionType.Proxy:
                        if (rulesOpt.Action.ServerID == -1)
                            this.sb_RuleList.Append("\r\n      <Action type=\"" + this.defaultRuleActionType.ToString() + "\" />");
                        else
                            this.sb_RuleList.Append("\r\n      <Action type=\"" + rulesOpt.Action.Type.ToString() + "\">" + rulesOpt.Action.ServerID.ToString() + "</Action>");
                        break;
                    case ActionType.Chain:
                        if (rulesOpt.Action.ServerID == -1)
                            this.sb_RuleList.Append("\r\n      <Action type=\"" + this.defaultRuleActionType.ToString() + "\" />");
                        else
                            this.sb_RuleList.Append("\r\n      <Action type=\"" + rulesOpt.Action.Type.ToString() + "\">" + rulesOpt.Action.ServerID.ToString() + "</Action>");
                        break;
                    default:
                        this.sb_RuleList.Append("\r\n      <Action type=\"" + rulesOpt.Action.Type.ToString() + "\" />");
                        break;
                }
            }
            return this.sb_RuleList.ToString();
        }

        public void setDefaultRule(ActionType action_type, int proxyID = 0)
        {
            defaultRuleActionType = action_type;
            defaultRuleProxyID = proxyID;
        }

        public int getProxyID(string proxyAddress, int proxyPort)
        {
            int proxyID = -1;
            foreach (ProxifierProxyServer proxy in innerProxyList.Values)
            {
                if (proxy.Address == proxyAddress && proxy.Port == proxyPort)
                    proxyID = proxy.ID;
            }
            return proxyID;
        }

        public ProxifierProxyServer getProxy(int ID)
        {
            ProxifierProxyServer result = null;
            if (this.innerProxyList.TryGetValue(ID, out result))
                return result;
            else
                return null;
        }

        public override string ToString()
        {
            this.sbOut.Clear();
            this.AddRule(true, "Default", null, null, null, defaultRuleActionType, defaultRuleProxyID);
            this.sbOut.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            this.sbOut.Append("\r\n<ProxifierProfile version=\"101\" platform=\"Windows\" product_id=\"1\" product_minver=\"310\">");
            this.sbOut.Append("\r\n" + profileOptions());
            if (this.innerProxyList.Count > 0)
                this.sbOut.Append("\r\n" + profileProxyList());
            else
                this.sbOut.Append("\r\n  <ProxyList />");
            if (this.innerChainList.Count > 0)
                this.sbOut.Append("\r\n" + profileChain());
            else
                this.sbOut.Append("\r\n  <ChainList />");
            this.sbOut.Append("\r\n  <RuleList>");
            if (this.innerRuleList.Count > 0)
                this.sbOut.Append("\r\n" + profileRuleList());
            this.sbOut.Append("\r\n  </RuleList>");
            this.sbOut.Append("\r\n</ProxifierProfile>");
            return this.sbOut.ToString();
        }

        public void SaveAs(string filepath)
        {
            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(filepath, false, Encoding.UTF8))
                this.SaveAs(sr);
        }

        public void SaveAs(System.IO.TextWriter _textwriter)
        {
            this.AddRule(true, "Default", null, null, null, defaultRuleActionType, defaultRuleProxyID);
            _textwriter.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            _textwriter.Write("\r\n<ProxifierProfile version=\"101\" platform=\"Windows\" product_id=\"1\" product_minver=\"310\">");
            _textwriter.Write("\r\n" + profileOptions());
            if (this.innerProxyList.Count > 0)
                _textwriter.Write("\r\n" + profileProxyList());
            else
                _textwriter.Write("\r\n  <ProxyList />");
            if (this.innerChainList.Count > 0)
                _textwriter.Write("\r\n" + profileChain());
            else
                _textwriter.Write("\r\n  <ChainList />");
            _textwriter.Write("\r\n  <RuleList>");
            if (this.innerRuleList.Count > 0)
                _textwriter.Write("\r\n" + profileRuleList());
            _textwriter.Write("\r\n  </RuleList>");
            _textwriter.Write("\r\n</ProxifierProfile>");
            _textwriter.Flush();
        }
    }
}
