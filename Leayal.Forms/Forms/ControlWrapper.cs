using System.Collections.Generic;

namespace Leayal.Forms
{
    public static class ControlWrapper
    {
        public static IEnumerable<System.Windows.Forms.Control> GetControls(System.Windows.Forms.Control _container)
        {
            List<System.Windows.Forms.Control> cl = new List<System.Windows.Forms.Control>();
            if (_container.Controls != null && _container.Controls.Count > 0)
            {
                cl.Add(_container);
                foreach (System.Windows.Forms.Control c in _container.Controls)
                    cl.AddRange(GetControls(c));
            }
            else
                cl.Add(_container);
            return cl;
        }

        public static System.Windows.Forms.Form FindFreakingForm(this System.Windows.Forms.Control c)
        {
            System.Windows.Forms.Form result = null;
            if (c.Parent != null)
            {
                if (c.Parent is System.Windows.Forms.Form)
                    result = (System.Windows.Forms.Form)c.Parent;
                else
                    result = FindFreakingForm(c.Parent);
            }
            return result;
        }
    }
}
