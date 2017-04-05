using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Leayal.IO;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    internal class MemoryFileCollection : IEnumerable, IDisposable
    {
        private Dictionary<string, RecyclableMemoryStream> innerDictionary;

        public MemoryFileCollection()
        {
            this.innerDictionary = new Dictionary<string, RecyclableMemoryStream>();
        }

        public int Count { get { return this.innerDictionary.Count; } }

        public RecyclableMemoryStream Add(string filename, RecyclableMemoryStream item)
        {
            if (_disposed) throw new ObjectDisposedException("MemoryFileCollection");
            this.innerDictionary.Add(filename, item);
            return item;
        }

        public RecyclableMemoryStream Add(string filename, int capacity)
        {
            return this.Add(filename, new RecyclableMemoryStream(filename, capacity));
        }

        public RecyclableMemoryStream Add(string filename)
        {
            return this.Add(filename, new RecyclableMemoryStream());
        }

        public RecyclableMemoryStream Add(string filename, byte[] bytes)
        {
            return this.Add(filename, bytes, 0, bytes.Length);
        }

        public RecyclableMemoryStream Add(string filename, byte[] bytes, int startIndex, int length)
        {
            RecyclableMemoryStream rms = new RecyclableMemoryStream(filename);
            rms.Write(bytes, startIndex, length);
            rms.Position = 0;
            return this.Add(filename, rms);
        }

        public void Clear()
        {
            if (this.innerDictionary.Count > 0)
            {
                foreach (RecyclableMemoryStream val in this.innerDictionary.Values)
                    val.Dispose();
                this.innerDictionary.Clear();
            }
        }

        public bool Contains(RecyclableMemoryStream item)
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

        public Dictionary<string, RecyclableMemoryStream>.ValueCollection Values
        { get { return this.innerDictionary.Values; } }

        public Dictionary<string, RecyclableMemoryStream>.KeyCollection Keys
        { get { return this.innerDictionary.Keys; } }

        public KeyValuePair<string, RecyclableMemoryStream> this[int index]
        { get { return this.innerDictionary.ElementAt(index); } }

        public RecyclableMemoryStream this[string key]
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
