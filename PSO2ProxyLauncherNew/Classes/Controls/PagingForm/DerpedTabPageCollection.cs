using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PSO2ProxyLauncherNew.Classes.Controls.PagingForm
{
    public class DerpedTabPageCollection : TabControl.TabPageCollection
    {
        public DerpedTabPageCollection(TabControl owner) : base(owner) { }

        public new void Add(TabPage item)
        {
            base.Add(new DerpedTabPage(item));
        }

        public new void Add(string text)
        {
            base.Add(new DerpedTabPage(text));
        }

        public new void Add(string key, string text)
        {
            base.Add(new DerpedTabPage(text) { Name = key });
        }

        public new void Add(string key, string text, string _imagekey)
        {
            base.Add(new DerpedTabPage(text) { Name = key, ImageKey = _imagekey });
        }

        public new void Add(string key, string text, int _imageIndex)
        {
            base.Add(new DerpedTabPage(text) { Name = key, ImageIndex = _imageIndex });
        }

        public new DerpedTabPage this[int index] { get { return (base[index] as DerpedTabPage); } }
        public new DerpedTabPage this[string key] { get { return (base[key] as DerpedTabPage); } }
    }
}
