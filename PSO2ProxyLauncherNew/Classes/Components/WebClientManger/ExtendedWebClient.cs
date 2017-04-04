namespace PSO2ProxyLauncherNew.Classes.Components.WebClientManger
{
    class ExtendedWebClient : Leayal.Net.ExtendedWebClient
    {
        public ExtendedWebClient() : base(new BaseWebClient()) { }
    }
}
