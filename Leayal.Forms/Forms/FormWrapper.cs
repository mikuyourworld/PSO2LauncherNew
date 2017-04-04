using System.Collections.Generic;

namespace Leayal.Forms
{
    public static class FormWrapper
    {
        public static IEnumerable<System.Windows.Forms.Control> GetControls(System.Windows.Forms.Form _container)
        {
            List<System.Windows.Forms.Control> cl = new List<System.Windows.Forms.Control>();
            if (_container.Controls != null && _container.Controls.Count > 0)
                foreach (System.Windows.Forms.Control c in _container.Controls)
                    cl.AddRange(ControlWrapper.GetControls(c));
            return cl;
        }

        internal static float _ScalingFactor = -1f;
        public static float ScalingFactor { get
            {
                if (_ScalingFactor < 0)
                    return SystemEvents.InnerSystemEvents.GetResolutionScale();
                return _ScalingFactor;
            }
        }
    }
}
