namespace PSO2ProxyLauncherNew.Classes.Components.Proxifier
{
    public enum EncryptionMode { disabled, basic };
    public enum ProxyType { HTTP, HTTPS, SOCKS4, SOCKS5 };
    public enum ActionType { Direct, Block, Proxy, Chain };
    public enum ChainType { simple, redundancy, load_balancing };
}