using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2UserConfiguration
{
    public class PSO2ConfigToken : IDisposable
    {
        private Dictionary<string, PSO2ConfigToken> myDictionary;

        internal PSO2ConfigToken()
        {
            this.myDictionary = new Dictionary<string, PSO2ConfigToken>();
            this.Values = new ValuesCollection();
        }

        public PSO2ConfigToken this[string key]
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("PSO2ConfigToken");
                if (!this.myDictionary.ContainsKey(key))
                    this.myDictionary.Add(key, new PSO2ConfigToken());
                return this.myDictionary[key];
            }
            set
            {
                if (_disposed) throw new ObjectDisposedException("PSO2ConfigToken");
                this.myDictionary[key] = value;
            }
        }

        public bool Remove(string key)
        {
            if (_disposed) throw new ObjectDisposedException("PSO2ConfigToken");
            return this.myDictionary.Remove(key);
        }

        protected void Clear()
        {
            if (_disposed) throw new ObjectDisposedException("PSO2ConfigToken");
            this.myDictionary.Clear();
        }

        public ValuesCollection Values { get; }

        protected Dictionary<string, PSO2ConfigToken> InnerArray => this.myDictionary;

        public IEnumerable<KeyValuePair<string, PSO2ConfigToken>> GetChildren()
        {
            if (_disposed) throw new ObjectDisposedException("PSO2ConfigToken");
            return this.myDictionary.AsEnumerable();
        }

        public override string ToString()
        {
            if (_disposed) throw new ObjectDisposedException("PSO2ConfigToken");
            if (this.Values == null || this.Values.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            bool firstline = true;
            foreach (var item in this.Values)
                if (firstline)
                {
                    firstline = false;
                    sb.AppendFormat("{0} = {1},", item.Key, item.Value);
                }
                else
                    sb.AppendFormat("\r\n{0} = {1},", item.Key, item.Value);
            return sb.ToString();
        }

        private bool _disposed;
        public virtual void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            this.myDictionary.Clear();
        }
    }
}
