using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    internal class MemoryFileCollection : IEnumerable, IDisposable
    {
        private Dictionary<string, MemoryStream> innerDictionary;

        public MemoryFileCollection()
        {
            this.innerDictionary = new Dictionary<string, MemoryStream>();
        }

        public int Count { get { return this.innerDictionary.Count; } }

        public MemoryStream Add(string filename, MemoryStream item)
        {
            if (_disposed) throw new ObjectDisposedException("MemoryFileCollection");
            this.innerDictionary.Add(filename, item);
            return item;
        }

        public MemoryStream Add(string filename, int capacity)
        {
            return this.Add(filename, new MemoryStream(capacity));
        }

        public MemoryStream Add(string filename, byte[] bytes, bool writable)
        {
            return this.Add(filename, new MemoryStream(bytes, writable));
        }

        public MemoryStream Add(string filename)
        {
            return this.Add(filename, new MemoryStream());
        }

        public MemoryStream Add(string filename, byte[] bytes)
        {
            return this.Add(filename, new MemoryStream(bytes));
        }

        public void Clear()
        {
            if (this.innerDictionary.Count > 0)
            {
                foreach (MemoryStream val in this.innerDictionary.Values)
                    val.Dispose();
                this.innerDictionary.Clear();
            }
        }

        public bool Contains(MemoryStream item)
        {
            return this.innerDictionary.ContainsValue(item);
        }

        public bool Contains(string filename)
        {
            return this.innerDictionary.ContainsKey(filename);
        }

        public bool Remove(string mapName)
        {
            if (this.Contains(mapName))
            {
                this.innerDictionary[mapName].Dispose();
                this.innerDictionary.Remove(mapName);
                return true;
            }
            else
                return false;
        }

        public Dictionary<string, MemoryStream>.ValueCollection Values
        { get { return this.innerDictionary.Values; } }

        public Dictionary<string, MemoryStream>.KeyCollection Keys
        { get { return this.innerDictionary.Keys; } }

        public KeyValuePair<string, MemoryStream> this[int index]
        { get { return this.innerDictionary.ElementAt(index); } }

        public MemoryStream this[string key]
        { get { return this.innerDictionary[key]; } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.innerDictionary.GetEnumerator();
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            this.Clear();
        }
    }
}
