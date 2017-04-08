using System;
using System.Collections;
using System.Collections.Generic;
using Leayal;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2UserConfiguration
{
    public class ValuesCollection
    {
        private Dictionary<string, string> innerdict;
        internal ValuesCollection()
        {
            this.innerdict = new Dictionary<string, string>();
        }

        public string this[string propertyName]
        {
            get
            {
                if (this.Count > 0)
                    foreach (string key in this.Keys)
                        if (key.IsEqual(propertyName, true))
                            return this.innerdict[key];
                return null;
            }
            set
            {
                this.innerdict[propertyName] = value;
            }
        }

        public ICollection<string> Keys => this.innerdict.Keys;

        public ICollection<string> Values => this.innerdict.Values;

        public int Count => this.innerdict.Count;

        public bool IsReadOnly => false;

        public void Clear()
        {
            this.innerdict.Clear();
        }

        public bool ContainsKey(string key)
        {
            return this.innerdict.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.innerdict.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return this.innerdict.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return this.innerdict.TryGetValue(key, out value);
        }
    }
}
