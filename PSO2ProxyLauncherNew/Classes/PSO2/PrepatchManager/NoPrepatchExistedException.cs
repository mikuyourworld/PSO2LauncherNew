using System;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PrepatchManager
{
    class NoPrepatchExistedException : Exception
    {
        public NoPrepatchExistedException() : base(LanguageManager.GetMessageText("NoPrepatchExistedException", "There is no pre-patch existed.")) { }
    }
}
