using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    public class CacheStorage : IDisposable
    {
        private static CacheStorage _defaultstorage;
        public static CacheStorage DefaultStorage
        {
            get
            {
                if (_defaultstorage == null)
                    _defaultstorage = new CacheStorage(Infos.DefaultValues.MyInfo.Directory.Cache);
                return _defaultstorage;
            }
        }
        private ConcurrentDictionary<string, CacheInfo> innerDict;
        public string StorageLocation { get; }
        public CacheStorage(string path)
        {
            this.StorageLocation = Path.GetFullPath(path);
            this.innerDict = new ConcurrentDictionary<string, CacheInfo>();
            //Set it as 250MB ???
            this.MaximumCacheSize = 250 * 1024 * 1024;
        }

        public long MaximumCacheSize { get; set; }

        public CacheInfo GetCacheFromURL(Uri _url)
        {
            if (disposed) throw new ObjectDisposedException("CacheStorage");
            var cacheID = Infos.CommonMethods.SHA256FromString(_url.AbsoluteUri);
            if (string.IsNullOrWhiteSpace(cacheID)) return null;
            cacheID = cacheID.ToLower();
            if (this.innerDict.ContainsKey(cacheID))
            {
                CacheInfo ci;
                if (this.innerDict.TryGetValue(cacheID, out ci))
                    return ci;
                else
                    return null;
            }
            else
            {
                var cache = GetCacheFromID(cacheID);
                if (this.innerDict.TryAdd(cacheID, cache))
                    this.innerDict[cacheID] = cache;
                //this.innerDict.Add(cacheID, cache);
                return cache;
            }
        }

        private CacheInfo GetCacheFromID(string cacheID)
        {
            var asdasd = CacheInfo.FromFile(Path.Combine(this.StorageLocation, cacheID));
            asdasd.CacheCreateCompleted += CacheInfo_CacheCreateCompleted;
            return asdasd;
        }

        private void CacheInfo_CacheCreateCompleted(object sender, EventArgs e)
        {
            CacheInfo ci = sender as CacheInfo;
            if (ci != null)
                this.Shrink();
        }

        public void Shrink()
        {
            string cacheID = null;
            long maxcacheSize = Math.Max(this.MaximumCacheSize, 10 * 1024 * 1024);
            CacheInfo cache;
            long TotalSize = 0L;
            foreach (string filename in Directory.EnumerateFiles(this.StorageLocation, "*", SearchOption.TopDirectoryOnly))
            {
                cacheID = Path.GetFileName(filename.ToLower());
                if (!this.innerDict.ContainsKey(cacheID))
                {
                    cache = GetCacheFromID(cacheID);
                    if (this.innerDict.TryAdd(cacheID, cache))
                        this.innerDict[cacheID] = cache;
                }
                else
                    cache = this.innerDict[cacheID];
                TotalSize += cache.FileSize;
            }
            if (this.innerDict.Count > 5 && TotalSize > maxcacheSize)
            {
                var ienum = this.innerDict.Values.Where(fi => fi.Exists && !fi.IsInUse).OrderBy(fi => fi.LastModifiedDate);
                if (ienum.Count() < 6) return;
                cache = ienum.First();
                if (cache == null) return;
                try
                { cache.Delete(); }
                catch { }
                cache.Dispose();
                CacheInfo aaaa;
                this.innerDict.TryRemove(cache.ID, out aaaa);
                if (this.innerDict.Count > 5 && TotalSize > 0 && TotalSize > maxcacheSize)
                    this.Shrink();
            }
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed) return;
            this.disposed = true;
            if (this.innerDict.Count > 0)
                foreach (CacheInfo ci in this.innerDict.Values)
                    ci.Dispose();
            this.innerDict.Clear();
        }
    }

    public class CacheInfo : IDisposable
    {
        public static CacheInfo FromFile(string path)
        {
            return new CacheInfo(new FileInfo(path));
        }

        public static CacheInfo FromFileInfo(FileInfo fi)
        {
            return new CacheInfo(fi);
        }

        private FileInfo innerfi;
        public string FilePath { get; }
        public FileStream OpenRead()
        {
            if (this.innerfi.Exists)
                return this.innerfi.OpenRead();
            else
                return null;
        }

        public bool IsInUse
        {
            get
            {
                this.innerfi.Refresh();
                if (!this.innerfi.Exists) return false;
                bool result = false;
                if ((this.innerfi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    try { this.innerfi.Open(FileMode.Open, FileAccess.Read, FileShare.None).Close(); }
                    catch (IOException) { result = true; }
                else
                    try { this.innerfi.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None).Close(); }
                    catch (IOException) { result = true; }

                return result;
            }
        }

        public string ID { get { return this.innerfi.Name.ToLower(); } }

        public FileStream OpenWrite()
        {
            if (this.innerfi.Exists)
                return this.innerfi.OpenWrite();
            else
                return null;
        }

        public FileStream Open(FileMode mode)
        {
            if (this.innerfi.Exists)
                return this.innerfi.Open(mode);
            else
                return null;
        }

        public void CreateFromStream(Stream _stream, DateTime _cacheTime)
        {
            this.CreateFromStream(_stream, _cacheTime, null);
        }

        public void CreateFromStream(Stream _stream, DateTime _cacheTime, EventHandler<Events.CacheWriteProgressChangedEventArgs> progressCallback)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(this.innerfi.DirectoryName);
            bool cancel = false;
            using (BufferedStream bufferStream = new BufferedStream(_stream, 1024))
            using (FileStream localfile = this.innerfi.Create())
            {
                long totalread = 0;
                byte[] arr = new byte[1024];
                int readbyte = bufferStream.Read(arr, 0, arr.Length);
                while (readbyte > 0)
                {
                    if (cancel)
                    {
                        localfile.Flush();
                        localfile.Close();
                        this.innerfi.Delete();
                        return;
                    }
                    localfile.Write(arr, 0, readbyte);
                    totalread += readbyte;
                    if (progressCallback != null)
                    {
                        var myEvent = new Events.CacheWriteProgressChangedEventArgs(totalread);
                        progressCallback.Invoke(this, myEvent);
                        cancel = myEvent.Cancel;
                    }
                    readbyte = bufferStream.Read(arr, 0, arr.Length);
                }
                localfile.Flush();
            }

            this.innerfi.CreationTimeUtc = _cacheTime;
            this.innerfi.LastWriteTimeUtc = _cacheTime;
            if (!this._disposed)
                this.CacheCreateCompleted?.Invoke(this, System.EventArgs.Empty);
        }

        public void CreateFromBytes(byte[] bytes, DateTime _cacheTime)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(this.innerfi.DirectoryName);
            using (FileStream fs = this.innerfi.Create())
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }
            this.innerfi.CreationTimeUtc = _cacheTime;
            this.innerfi.LastWriteTimeUtc = _cacheTime;
            if (!this._disposed)
                this.CacheCreateCompleted?.Invoke(this, System.EventArgs.Empty);
        }

        public void AppendText(string str, Encoding encode)
        {
            using (var fs = this.Open(FileMode.Append))
            using (StreamWriter sw = new StreamWriter(fs, encode))
            {
                sw.Write(str);
                sw.Flush();
            }
            if (!this._disposed)
                this.CacheCreateCompleted?.Invoke(this, System.EventArgs.Empty);
        }

        public void WriteText(string str, Encoding encode)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(this.innerfi.DirectoryName);
            using (var fs = this.innerfi.Create())
            using (StreamWriter  sw = new StreamWriter(fs, encode))
            {
                sw.Write(str);
                sw.Flush();
            }
            if (!this._disposed)
                this.CacheCreateCompleted?.Invoke(this, System.EventArgs.Empty);
        }

        public Uri LocalURI
        {
            get
            {
                if (this.innerfi.Exists)
                    return new Uri(this.innerfi.FullName);
                else
                    return null;
            }
        }

        public bool Exists
        {
            get
            {
                this.innerfi.Refresh();
                return this.innerfi.Exists;
            }
        }

        public DateTime LastModifiedDate
        {
            get
            {
                if (this.innerfi.Exists)
                {
                    this.innerfi.Refresh();
                    return this.innerfi.LastWriteTimeUtc;
                }
                else
                    return DateTime.MinValue;
            }
        }

        public long FileSize
        {
            get
            {
                this.innerfi.Refresh();
                if (this.innerfi.Exists)
                    return this.innerfi.Length;
                else
                    return 0L;
            }
        }

        internal event EventHandler CacheCreateCompleted;

        private CacheInfo(FileInfo fi)
        {
            this.innerfi = fi;
            this.FilePath = fi.FullName;
        }

        public void Delete()
        {
            this.innerfi.Delete();
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            this._disposed = true;
        }
    }
}
