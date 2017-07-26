using System;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    class NotRecommendedException : Exception
    {
        public NotRecommendedException() : base() { }
        public NotRecommendedException(string message) : base(message) { }
    }
}
