using System;
using System.Collections.Concurrent;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class Discarded_PSO2UrlDatabase
    {
        private char[] _onlyNullChar = { '\0' };

        private static Discarded_PSO2UrlDatabase Instance = null;

        public static bool Add(string relativePath, Uri uri)
        {
            return Instance.AddEx(relativePath, uri);
        }

        public static bool Add(string relativePath, string uri)
        {
            return Instance.AddEx(relativePath, uri);
        }

        public static bool Contains(string relativePath)
        {
            return Instance.ContainsEx(relativePath);
        }

        public static Uri Fetch(string relativePath)
        {
            return Instance.FetchEx(relativePath);
        }

        public static bool Update(string relativePath, Uri uri)
        {
            return Instance.UpdateEx(relativePath, uri);
        }

        public static bool Update(string relativePath, string uri)
        {
            return Instance.UpdateEx(relativePath, uri);
        }

        public static bool Save()
        {
            return Instance.SaveEx();
        }

        private ConcurrentDictionary<string, Uri> innerDictionary;
        //private ConcurrentBag<string> indexing;
        private string workingDirectory;
        private bool commited;

        internal Discarded_PSO2UrlDatabase(string databasepath)
        {
            if (string.IsNullOrWhiteSpace(databasepath))
                throw new ArgumentNullException("databasepath", "The PSO2UrlDatabase's path cannot be empty.");
            this.workingDirectory = Path.GetFullPath(databasepath);
            this.innerDictionary = this.Read();
            this.commited = false;
        }

        internal bool ContainsEx(string relativePath)
        {
            return this.innerDictionary.ContainsKey(relativePath);
        }

        internal bool AddEx(string relativePath, Uri uri)
        {
            if (!this.ContainsEx(relativePath))
            {
                if (!this.innerDictionary.TryAdd(relativePath, uri))
                    this.innerDictionary[relativePath] = uri;
                this.commited = true;
                return true;
            }
            return false;
        }

        internal bool AddEx(string relativePath, string uri)
        {
            return AddEx(relativePath, new Uri(uri));
        }

        internal Uri FetchEx(string relativePath)
        {
            Uri result = null;
            if (!this.innerDictionary.TryGetValue(relativePath, out result))
                return null;
            return result;
        }

        public Uri this[string relativePath]
        { get { return this.FetchEx(relativePath); } }

        internal bool UpdateEx(string relativePath, Uri uri)
        {
            if (this.ContainsEx(relativePath))
            {
                if (this.innerDictionary[relativePath].AbsoluteUri == uri.AbsoluteUri)
                    return false;
                else
                {
                    Uri asd;
                    this.innerDictionary.TryRemove(relativePath, out asd);
                    this.innerDictionary.TryAdd(relativePath, uri);
                    this.commited = true;
                    return true;
                }
            }
            else
            {
                if (!this.innerDictionary.TryAdd(relativePath, uri))
                    this.innerDictionary[relativePath] = uri;
                this.commited = true;
                return false;
            }
        }

        internal bool UpdateEx(string relativePath, string uri)
        {
            return this.UpdateEx(relativePath, new Uri(uri));
        }

        private ConcurrentDictionary<string, Uri> Read()
        {
            ConcurrentDictionary<string, Uri> result = new ConcurrentDictionary<string, Uri>();
            if (File.Exists(this.workingDirectory))
            {
                string bufferline;
                string[] splitline;
                Uri asd;
                using (StreamReader sr = new StreamReader(this.workingDirectory, System.Text.Encoding.ASCII))
                    while (!sr.EndOfStream)
                    {
                        asd = null; ;
                        bufferline = sr.ReadLine();
                        splitline = null;
                        if (!string.IsNullOrWhiteSpace(bufferline))
                            if (bufferline.IndexOf('\0') > -1)
                            {
                                splitline = bufferline.Split(_onlyNullChar, 2);
                                asd = new Uri(splitline[1]);
                                if (!result.TryAdd(splitline[0], asd))
                                    result[splitline[0]] = asd;
                            }
                    }
            }
            return result;
        }

        internal bool SaveEx()
        {
            if (this.commited)
            {
                try
                {
                    if (this.innerDictionary.Count > 0)
                    {
                        FileSystem.CreateDirectory(FileSystem.GetParentPath(this.workingDirectory));
                        using (StreamWriter sw = new StreamWriter(this.workingDirectory, false, System.Text.Encoding.ASCII))
                            foreach (var asd in this.innerDictionary)
                                sw.Write(string.Concat(asd.Key, '\0', asd.Value.AbsoluteUri, '\n'));
                        this.commited = false;
                        return true;
                    }
                    else
                        return false;
                }
                catch
                { return false; }
            }
            else
                return false;
        }
    }

    public class PSO2FileUrl
    {
        public Uri MainUrl { get; }
        public Uri OldUrl { get; }
        public PSO2FileUrl(string _newUri, string _oldUrl)
        {
            this.MainUrl = new Uri(_newUri);
            this.OldUrl = new Uri(_oldUrl);
        }

        public Uri GetTheOtherOne(string url)
        {
            if (OldUrl.OriginalString == url)
                return MainUrl;
            else if (MainUrl.OriginalString == url)
                return OldUrl;
            else
                return null;
        }
    }
}
