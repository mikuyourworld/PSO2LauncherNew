using System;

namespace PSO2ProxyLauncherNew.Classes.Components.Proxifier
{
    class ProxifierRule
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Applications { get; set; }
        public string Ports { get; set; }
        public string Targets { get; set; }
        public ProxifierRuleAction Action { get; }

        public ProxifierRule(bool _enabled) : this(_enabled, string.Empty) { }
        public ProxifierRule() : this(true, string.Empty) { }
        public ProxifierRule(bool _enabled, string _name)
        {
            this.Enabled = _enabled;
            this.Name = _name;
            this.Applications = string.Empty;
            this.Ports = string.Empty;
            this.Targets = string.Empty;
            this.Action = new ProxifierRuleAction();
        }

    }

    public class ProxifierRuleAction
    {
        public ActionType Type { get; set; }
        public int ServerID { get; set; }
        public ProxifierRuleAction(string _type, int _id) : this((ActionType)Enum.Parse(typeof(ActionType), _type, true), _id) { }
        public ProxifierRuleAction() : this(ActionType.Direct, 100) { }
        public ProxifierRuleAction(ActionType _type, int _id)
        {
            this.Type = _type;
            this.ServerID = _id;
        }
    }
}
