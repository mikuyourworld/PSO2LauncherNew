using System.Drawing;

namespace PSO2ProxyLauncherNew.Classes.Infos
{
    public class WrapStringResult
    {
        public string Result { get; }
        public Size Size { get; }
        public WrapStringResult(string re, Size _size)
        {
            this.Result = re;
            this.Size = _size;
        }
    }
}
