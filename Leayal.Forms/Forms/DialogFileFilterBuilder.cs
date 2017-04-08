using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leayal.Forms
{
    public class DialogFileFilterBuilder : IDisposable
    {
        private List<FileFilterItem> innerList;
        private StringBuilder sb;
        StringBuilder alltypeSB;
        public DialogFileFilterBuilder()
        {
            this.innerList = new List<FileFilterItem>();
            this.sb = new StringBuilder();
            this.alltypeSB = new StringBuilder();
            this.AppendAllSupportedTypes = AppendOrder.None;
            this._lasttimeappendAllSupportedTypes = AppendOrder.None;
        }

        public DialogFileFilterBuilder(IEnumerable<FileFilterItem> preList)
        {
            if (preList != null)
                this.innerList = new List<FileFilterItem>(preList);
            else
                this.innerList = new List<FileFilterItem>();
            this.sb = new StringBuilder();
            this.AppendAllSupportedTypes = AppendOrder.None;
            this._lasttimeappendAllSupportedTypes = AppendOrder.None;
        }

        public DialogFileFilterBuilder(IDictionary<string, IEnumerable<string>> preList) : this(preList.Select((keypair) => { return new FileFilterItem(keypair.Key, keypair.Value); })) { }

        public DialogFileFilterBuilder(IDictionary<string, string[]> preList) : this(preList.Select((keypair) => { return new FileFilterItem(keypair.Key, keypair.Value); })) { }

        public void Append(string nametag, IEnumerable<string> extensions)
        {
            this.Append(new FileFilterItem(nametag, extensions));
        }

        public void Append(string nametag, params string[] extensions)
        {
            this.Append(new FileFilterItem(nametag, extensions));
        }

        public void Append(FileFilterItem ff)
        {
            if (_disposed) throw new ObjectDisposedException("DialogFileFilterBuilder");
            this.innerList.Add(ff);
        }

        public void Insert(int index, string nametag, IEnumerable<string> extensions)
        {
            this.Insert(index, new FileFilterItem(nametag, extensions));
        }

        public void Insert(int index, string nametag, params string[] extensions)
        {
            this.Insert(index, new FileFilterItem(nametag, extensions));
        }

        public void Insert(int index, FileFilterItem ff)
        {
            if (_disposed) throw new ObjectDisposedException("DialogFileFilterBuilder");
            this.innerList.Insert(index, ff);
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            this.sb.Clear();
            this.innerList.Clear();
        }

        public string ToFileFilterString()
        {
            if (_disposed) throw new ObjectDisposedException("DialogFileFilterBuilder");
            if (this.Count == 0) return string.Empty;
            this.sb.Clear();
            if (this.AppendAllSupportedTypes == AppendOrder.First)
            {
                this.alltypeSB.Clear();
                for (int i = 0; i < this.Count; i++)
                {
                    if (i == 0)
                        alltypeSB.Append(innerList[i].GetManyExts(","));
                    else
                        alltypeSB.AppendFormat(",{0}", innerList[i].GetManyExts(","));
                }
                this.sb.AppendFormat("|All Supported Types ({0})|{1}", alltypeSB.ToString(), alltypeSB.Replace(",", ";").ToString());
            }
            for (int i = 0; i < this.Count; i++)
            {
                if (i == 0)
                    this.sb.Append(innerList[i].ToString());
                else
                    this.sb.AppendFormat("|{0}", innerList[i].ToString());
            }
            if (this.AppendAllSupportedTypes == AppendOrder.Last)
            {
                this.alltypeSB.Clear();
                for (int i = 0; i < this.Count; i++)
                {
                    if (i == 0)
                        alltypeSB.Append(innerList[i].GetManyExts(","));
                    else
                        alltypeSB.AppendFormat(",{0}", innerList[i].GetManyExts(","));                    
                }
                this.sb.AppendFormat("|All Supported Types ({0})|{1}", alltypeSB.ToString(), alltypeSB.Replace(",", ";").ToString());
            }
            this._lasttimeappendAllSupportedTypes = this.AppendAllSupportedTypes;
            return this.sb.ToString();
        }

        public override string ToString()
        {
            return this.ToFileFilterString();
        }

        public int Count => this.innerList.Count;

        /// <summary>
        /// This properties should be used AFTER calling .ToString() or .ToFileFilterString()
        /// </summary>
        public int OutputCount
        {
            get
            {
                if (this._lasttimeappendAllSupportedTypes == AppendOrder.None)
                    return this.Count;
                else
                {
                    return (this.Count + 1);
                }
            }
        }

        private AppendOrder _lasttimeappendAllSupportedTypes;
        private AppendOrder _appendAllSupportedTypes;
        public AppendOrder AppendAllSupportedTypes
        {
            get { return this._appendAllSupportedTypes; }
            set { this._appendAllSupportedTypes = value; }
        }
    }

    public enum AppendOrder : byte
    {
        None,
        First,
        Last
    }

    public class FileFilterItem
    {
        public string Nametag { get; }
        public string[] FileExtension { get; }
        private StringBuilder sb;
        
        public FileFilterItem(string name, IEnumerable<string> ext) : this(name, ext.ToArray()) { }
        public FileFilterItem(string name, string[] ext)
        {
            this.Nametag = name;
            this.FileExtension = ext;
            this.sb = new StringBuilder();
        }

        internal void AppendManyExts(string splitchar)
        {
            for (int i = 0; i < this.FileExtension.Length; i++)
                if (i == 0)
                    this.sb.Append(this.FileExtension[i]);
                else
                    this.sb.AppendFormat("{0}{1}", splitchar, this.FileExtension[i]);
        }

        internal string GetManyExts(string splitchar)
        {
            this.sb.Clear();
            for (int i = 0; i < this.FileExtension.Length; i++)
                if (i == 0)
                    this.sb.Append(this.FileExtension[i]);
                else
                    this.sb.AppendFormat("{0}{1}", splitchar, this.FileExtension[i]);
            return this.sb.ToString();
        }

        public override string ToString()
        {
            this.sb.Clear();
            this.sb.Append(this.Nametag);
            this.sb.Append(" (");
            this.AppendManyExts(",");
            this.sb.Append(")|");
            this.AppendManyExts(";");
            return this.sb.ToString();
        }
    }
}
