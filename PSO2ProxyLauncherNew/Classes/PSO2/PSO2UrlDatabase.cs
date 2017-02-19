using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class PSO2UrlDatabase
    {
        private char[] _onlyNullChar = { '\0' };
        private static PSO2UrlDatabase _instance;
        public static PSO2UrlDatabase Instance
        {
            get
            {
                
                if (_instance == null)
                    _instance = new PSO2UrlDatabase(Classes.Infos.CommonMethods.PathConcat(MyApp.AssemblyInfo.DirectoryPath, "PSO2UrlDatabase.dbLea"));
                return _instance;
            }
        }

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

        private Dictionary<string, Uri> innerDictionary;
        private string workingDirectory;
        private bool commited;

        public PSO2UrlDatabase(string databasepath)
        {
            if (string.IsNullOrWhiteSpace(databasepath))
                throw new ArgumentNullException("databasepath", "The PSO2UrlDatabase's path cannot be empty.");
            this.workingDirectory = Path.GetFullPath(databasepath);
            this.innerDictionary = this.Read();
            this.commited = false;
        }

        public bool ContainsEx(string relativePath)
        {
            return this.innerDictionary.ContainsKey(relativePath);
        }

        public bool AddEx(string relativePath, Uri uri)
        {
            if (!this.ContainsEx(relativePath))
            {
                this.innerDictionary.Add(relativePath, uri);
                this.commited = true;
                return true;
            }
            return false;
        }

        public bool AddEx(string relativePath, string uri)
        {
            return AddEx(relativePath, new Uri(uri));
        }

        public Uri FetchEx(string relativePath)
        {
            if (this.ContainsEx(relativePath))
            { return this.innerDictionary[relativePath]; }
            return null;
        }

        public Uri this[string relativePath]
        { get { return this.FetchEx(relativePath); } }

        public bool UpdateEx(string relativePath, Uri uri)
        {
            if (this.ContainsEx(relativePath))
            {
                if (this.innerDictionary[relativePath].AbsoluteUri == uri.AbsoluteUri)
                    return false;
                else
                {
                    this.innerDictionary.Remove(relativePath);
                    this.innerDictionary.Add(relativePath, uri);
                    this.commited = true;
                    return true;
                }
            }
            else
            {
                this.innerDictionary.Add(relativePath, uri);
                this.commited = true;
                return false;
            }
        }

        public bool UpdateEx(string relativePath, string uri)
        {
            return this.UpdateEx(relativePath, new Uri(uri));
        }

        private Dictionary<string, Uri> Read()
        {
            Dictionary<string, Uri> result = new Dictionary<string, Uri>();
            if (File.Exists(this.workingDirectory))
            {
                string bufferline;
                string[] splitline;
                using (StreamReader sr = new StreamReader(this.workingDirectory, System.Text.Encoding.ASCII))
                    while (!sr.EndOfStream)
                    {
                        bufferline = string.Empty;
                        bufferline = sr.ReadLine();
                        splitline = null;
                        if (!string.IsNullOrWhiteSpace(bufferline))
                            if (bufferline.IndexOf('\0') > -1)
                            {
                                splitline = bufferline.Split(_onlyNullChar, 2);
                                result.Add(splitline[0], new Uri(splitline[1]));
                            }
                    }
            }
            return result;
        }

        public bool SaveEx()
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
                                sw.Write(asd.Key + '\0' + asd.Value.AbsoluteUri + '\n');
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
}
