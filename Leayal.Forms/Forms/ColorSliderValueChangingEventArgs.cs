using System;

namespace Leayal.Forms
{
    public class ColorSliderValueChangingEventArgs : EventArgs
    {
        public int TrackerValue { get; }
        public ColorSliderValueChangingEventArgs(int val) : base()
        {
            this.TrackerValue = val;
        }
    }
}
