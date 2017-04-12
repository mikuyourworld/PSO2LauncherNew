using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Leayal.Forms
{
    public static class ControlWrapper
    {
        public static IEnumerable<System.Windows.Forms.Control> GetControls(System.Windows.Forms.Control _container)
        {
            List<System.Windows.Forms.Control> cl = new List<System.Windows.Forms.Control>();
            cl.Add(_container);
            if (_container.HasChildren)
            {
                if (_container.HasChildren)
                    foreach (System.Windows.Forms.Control c in _container.Controls)
                        cl.AddRange(GetControls(c));
            }
            return cl;
        }

        public static IEnumerable<object> GetAllControls(object _container)
        {
            List<object> cl = new List<object>();
            if (_container is Control || _container is FakeControl)
                cl.Add(_container);
            IFakeControlContainer fccccc = _container as IFakeControlContainer;
            if (fccccc != null)
            {
                foreach (FakeControl c in fccccc.Controls)
                    cl.AddRange(GetAllControls(c));
            }
            else
            {
                Control ccccc = _container as Control;
                if (ccccc != null)
                {
                    if (ccccc.HasChildren)
                        foreach (System.Windows.Forms.Control c in ccccc.Controls)
                            cl.AddRange(GetAllControls(c));
                }
            }
            return cl;
        }

        public static IEnumerable<FakeControl> GetFakeControls(System.Windows.Forms.Control _container)
        {
            List<FakeControl> cl = new List<FakeControl>();
            IFakeControlContainer fccontainer = _container as IFakeControlContainer;
            if (fccontainer != null)
            {
                foreach (FakeControl c in fccontainer.Controls)
                    cl.AddRange(GetFakeControls(c));
            }
            else
            {
                if (_container.HasChildren)
                    foreach (System.Windows.Forms.Control c in _container.Controls)
                        cl.AddRange(GetFakeControls(c));
            }
            return cl;
        }

        public static IEnumerable<FakeControl> GetFakeControls(FakeControl _container)
        {
            List<FakeControl> cl = new List<FakeControl>();
            IFakeControlContainer fccontainer = _container as IFakeControlContainer;
            cl.Add(_container);
            if (fccontainer != null)
            {
                foreach (FakeControl c in fccontainer.Controls)
                    cl.AddRange(GetFakeControls(c));
            }
            return cl;
        }

        public static void SetNewCache(System.Windows.Forms.Control c)
        {
            if (!c.Size.IsEmpty)
            {
                Image myBGCache;
                if (c.BackgroundImage != null)
                {
                    myBGCache = c.BackgroundImage;
                    c.BackgroundImage = null;
                    myBGCache.Dispose();
                }
                myBGCache = new Bitmap(c.Width, c.Height);
                using (Graphics gr = Graphics.FromImage(myBGCache))
                    ButtonRenderer.DrawParentBackground(gr, c.DisplayRectangle, c);
                c.BackgroundImage = myBGCache;
            }
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
