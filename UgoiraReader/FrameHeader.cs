namespace Leayal.Ugoira.WPF
{
    public class FrameHeader
    {
        private string _filename;
        public void SetFilename(string filename)
        {
            this._filename = filename;
        }
        public string Filename => this._filename;
        private int _framedelay;
        public void SetDelay(int framedelay)
        {
            this._framedelay = framedelay;
        }
        public int FrameDelay => this._framedelay;

        internal FrameHeader() : this(string.Empty) { }
        internal FrameHeader(string filename) : this(filename, 60) { }
        internal FrameHeader(int delay) : this(string.Empty, delay) { }
        internal FrameHeader(string filename, int delay)
        {
            this._filename = filename;
            this._framedelay = delay;
        }
    }
}
