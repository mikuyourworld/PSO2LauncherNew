namespace PSO2ProxyLauncherNew.Forms.MyMainMenuCode
{
    public class CircleProgressBarProperties : ProgressBarProperties
    {
        public bool ShowCancel { get; }
        public bool ShowSmallText { get; }
        public CircleProgressBarProperties(bool _showsmalltext, bool _cancel)
        {
            this.ShowCancel = _cancel;
            this.ShowSmallText = _showsmalltext;
        }

        public CircleProgressBarProperties(bool _showsmalltext) : this(_showsmalltext, true) { }
    }

    public class InfiniteProgressBarProperties : ProgressBarProperties
    {
        public bool ShowCancel { get; }
        public InfiniteProgressBarProperties(bool _cancel)
        {
            this.ShowCancel = _cancel;
        }
    }

    public interface ProgressBarProperties
    {
        bool ShowCancel { get; }
    }
}
