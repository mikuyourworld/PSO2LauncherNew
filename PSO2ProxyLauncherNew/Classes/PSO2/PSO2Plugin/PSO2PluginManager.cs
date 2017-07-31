﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using PSO2ProxyLauncherNew.Classes.Components.WebClientManger;
using PSO2ProxyLauncherNew.Classes.Events;
using Leayal.Log;
using System.Collections.Concurrent;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Plugin
{
    class PSO2PluginManager
    {
        private static PSO2PluginManager _instance;
        public static PSO2PluginManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PSO2PluginManager();
                return _instance;
            }
        }

        private ExtendedWebClient myWebClient;
        private BackgroundWorker myBWorker;
        private FileInfo myCacheFileInfo;
        private KeyValuePair<string, PSO2Plugin> nullKeyPair;

        public bool IsCacheExist
        {
            get
            {
                if (this.myCacheFileInfo != null)
                    return this.myCacheFileInfo.Exists;
                else
                    return false;
            }
        }

        public bool IsBusy { get { return this.myBWorker.IsBusy; } }

        public DateTime _Version;
        public DateTime Version { get { return this._Version; } }

        private ConcurrentDictionary<string, PSO2Plugin> _PluginList;
        public ConcurrentDictionary<string, PSO2Plugin> PluginList { get { return this._PluginList; } }

        public int Count { get { return (this._PluginList != null ? this._PluginList.Count : 0); } }

        public PSO2Plugin this[string PluginID]
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PluginID))
                {

                    if (this._PluginList != null)
                    {
                        PSO2Plugin val;
                        if (this._PluginList.TryGetValue(PluginID, out val))
                            return val;
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        private void Clear()
        {
            if (this._PluginList != null && this._PluginList.Count > 0)
            {
                foreach (PSO2Plugin val in this._PluginList.Values)
                {
                    val.EnableChanged -= PSO2Plugin_EnableChanged;
                    val.HandledException -= PSO2Plugin_HandledException;
                }
                this._PluginList.Clear();
            }
        }

        private void PSO2Plugin_HandledException(object sender, HandledExceptionEventArgs e)
        {
            this.OnHandledException(e);
            this.OnPluginStatusChanged(new PSO2PluginStatusChanged(e.Error, sender as PSO2Plugin));
        }

        private void PSO2Plugin_EnableChanged(object sender, EventArgs e)
        {
            this.OnPluginStatusChanged(new PSO2PluginStatusChanged(sender as PSO2Plugin));
        }

        public PSO2PluginManager()
        {
            this.nullKeyPair = new KeyValuePair<string, PSO2Plugin>(string.Empty, null);
            this.myCacheFileInfo = new FileInfo(Infos.CommonMethods.PathConcat(Infos.DefaultValues.MyInfo.Directory.Cache, Infos.DefaultValues.MyInfo.Filename.PluginCache));
            this._Version = DateTime.MinValue;
            this.myWebClient = new ExtendedWebClient();
            //this.myWebClient.UserAgent = Infos.DefaultValues.Kaze.Web.UserAgent;
            this.myBWorker = new BackgroundWorker();
            this.myBWorker.WorkerSupportsCancellation = false;
            this.myBWorker.WorkerReportsProgress = false;
            this.myBWorker.DoWork += this.MyBWorker_DoWork;
            this.myBWorker.RunWorkerCompleted += this.MyBWorker_RunWorkerCompleted;
            this.ReadCache();
            if (this._PluginList == null)
                this._PluginList = new ConcurrentDictionary<string, PSO2Plugin>(StringComparer.OrdinalIgnoreCase);
        }

        public void GetPluginList()
        {
            if (!this.myBWorker.IsBusy)
            {
                if (MySettings.MinimizeNetworkUsage)
                    this.myWebClient.CacheStorage = Components.CacheStorage.DefaultStorage;
                else
                    this.myWebClient.CacheStorage = null;
                this.myBWorker.RunWorkerAsync();
            }
        }

        private void MyBWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.OnHandledException(new Events.HandledExceptionEventArgs(e.Error));
                this.OnCheckForPluginCompleted(new CheckForPluginCompletedEventArgs(e.Error));
            }
            else
            {
                if (e.Result != null)
                    this.OnCheckForPluginCompleted(new CheckForPluginCompletedEventArgs((List<PSO2Plugin>)e.Result));
                else
                    this.OnCheckForPluginCompleted(new CheckForPluginCompletedEventArgs());
            }
        }

        private void MyBWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (AIDA.IsPingedAIDA)
            {
                PluginGetResult returnString = GetPluginList(new Uri(Leayal.UriHelper.URLConcat(AIDA.TweakerWebPanel.PluginURL, AIDA.TweakerWebPanel.PluginJsonFilename)));
                if (returnString != null)
                {
                    try
                    {
                        ConcurrentDictionary<string, PSO2Plugin> _newpluginlist = ReadPluginList(returnString.Result);
                        this._Version = returnString.Version;
                        this.Clear();
                        this._PluginList = _newpluginlist;
                        this.WriteCache(returnString.Version, returnString.Result);
                    }
                    catch (Exception ex) { LogManager.GeneralLog.Print(ex); }
                }
            }

            if (CommonMethods.IsPSO2Installed)
            {
                if (this._PluginList.Count > 0)
                {
                    List<PSO2Plugin> pluginUpdated = new List<PSO2Plugin>(this._PluginList.Count);
                    foreach (var item in this._PluginList)
                        if (item.Value.DownloadLink != null)
                        {
                            try
                            {
                                switch (item.Value.IsValid())
                                {
                                    case PSO2Plugin.Status.NotExisted:
                                        //Down freaking load to the Enabled place
                                        this.myWebClient.DownloadFile(item.Value.DownloadLink, item.Value.FullPath.EnabledPath);
                                        pluginUpdated.Add(item.Value);
                                        break;
                                    case PSO2Plugin.Status.DisabledInvalid:
                                        this.myWebClient.DownloadFile(item.Value.DownloadLink, item.Value.FullPath.DisabledPath);
                                        pluginUpdated.Add(item.Value);
                                        break;
                                    case PSO2Plugin.Status.EnabledInvalid:
                                        this.myWebClient.DownloadFile(item.Value.DownloadLink, item.Value.FullPath.EnabledPath);
                                        pluginUpdated.Add(item.Value);
                                        break;
                                }
                            }
                            catch (IOException ex) { this.OnHandledException(new HandledExceptionEventArgs(new Exception("Failed to update the plugin '" + item.Key + "'", ex))); }
                            catch (UnauthorizedAccessException ex) { this.OnHandledException(new HandledExceptionEventArgs(new Exception("Failed to update the plugin '" + item.Key + "'", ex))); }
                        }
                    e.Result = pluginUpdated;
                }

                string filenameonly, nameonly, lowerfilenameonly;
                if (Directory.Exists(PSO2.DefaultValues.Directory.PSO2Plugins))
                    foreach (string file in Directory.EnumerateFiles(PSO2.DefaultValues.Directory.PSO2Plugins, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        filenameonly = Path.GetFileName(file);
                        lowerfilenameonly = filenameonly.ToLower();
                        nameonly = Path.ChangeExtension(filenameonly, null);
                        if (!this.PluginList.ContainsKey(lowerfilenameonly))
                            AddPlugin(this._PluginList, new PSO2Plugin(lowerfilenameonly, nameonly, filenameonly, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, true, false));
                    }

                if (Directory.Exists(PSO2.DefaultValues.Directory.PSO2PluginsDisabled))
                    foreach (string file in Directory.EnumerateFiles(PSO2.DefaultValues.Directory.PSO2PluginsDisabled, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        filenameonly = Path.GetFileName(file);
                        lowerfilenameonly = filenameonly.ToLower();
                        nameonly = Path.ChangeExtension(filenameonly, null);
                        if (!this.PluginList.ContainsKey(lowerfilenameonly))
                            AddPlugin(this._PluginList, new PSO2Plugin(lowerfilenameonly, nameonly, filenameonly, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, true, false));
                    }
            }
        }

        public event EventHandler<PSO2PluginStatusChanged> PluginStatusChanged;
        protected virtual void OnPluginStatusChanged(PSO2PluginStatusChanged e)
        {
            if (this.PluginStatusChanged != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.PluginStatusChanged.Invoke(this, e); }), null);
        }

        public event EventHandler<HandledExceptionEventArgs> HandledException;
        protected virtual void OnHandledException(HandledExceptionEventArgs e)
        {
            if (this.HandledException != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.HandledException.Invoke(this, e); }), null);
        }

        public event EventHandler<CheckForPluginCompletedEventArgs> CheckForPluginCompleted;
        protected virtual void OnCheckForPluginCompleted(CheckForPluginCompletedEventArgs e)
        {
            if (this.CheckForPluginCompleted != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.CheckForPluginCompleted.Invoke(this, e); }), null);
        }

        public event EventHandler<StepEventArgs> StepChanged;
        protected virtual void OnStepChanged(StepEventArgs e)
        {
            if (this.StepChanged != null)
                WebClientPool.SynchronizationContext?.Post(new System.Threading.SendOrPostCallback(delegate { this.StepChanged.Invoke(this, e); }), null);
        }

        protected virtual PluginGetResult GetPluginList(Uri url)
        {
            PluginGetResult result = null;
            WebResponse request = this.myWebClient.Open(url);
            HttpWebResponse http = request as HttpWebResponse;
            if (http != null)
            {
                if (http.LastModified > this.Version)
                    using (Stream rs = http.GetResponseStream())
                    using (StreamReader sr = new StreamReader(rs))
                        result = new PluginGetResult(http.LastModified, sr.ReadToEnd());
                http.Close();
            }
            Leayal.Net.CacheResponse cache = request as Leayal.Net.CacheResponse;
            if (cache != null)
            {
                if (cache.LastModified > this.Version)
                    using (Stream rs = cache.GetResponseStream())
                    using (StreamReader sr = new StreamReader(rs))
                        result = new PluginGetResult(cache.LastModified, sr.ReadToEnd());
                cache.Close();
            }
            return result;
        }

        protected virtual void WriteCache(DateTime ver, string _rawpluginlist)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(this.myCacheFileInfo.DirectoryName);
            using (StreamWriter sw = new StreamWriter(this.myCacheFileInfo.FullName, false, Encoding.UTF8))
            {
                sw.Write(_rawpluginlist);
                sw.Flush();
            }
            this.myCacheFileInfo.CreationTimeUtc = ver;
            this.myCacheFileInfo.LastWriteTimeUtc = ver;
            this.myCacheFileInfo.Refresh();
        }

        protected virtual void ReadCache()
        {
            if (this.myCacheFileInfo != null)
            {
                this.myCacheFileInfo.Refresh();
                if (this.myCacheFileInfo.Exists)
                {
                    string text;
                    using (StreamReader sw = new StreamReader(this.myCacheFileInfo.FullName, Encoding.UTF8))
                        text = sw.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(text))
                        try
                        {
                            if (this._PluginList != null)
                                this.Clear();
                            this._PluginList = ReadPluginList(text);
                            this._Version = this.myCacheFileInfo.CreationTimeUtc;
                        }
                        catch { }
                }
            }
        }

#if DEBUG
        private ConcurrentDictionary<string, PSO2Plugin> ReadPluginList(string jsonString)
        {
            ConcurrentDictionary<string, PSO2Plugin> result = new ConcurrentDictionary<string, PSO2Plugin>(StringComparer.OrdinalIgnoreCase);
            JObject _jObject = JObject.Parse(jsonString);
            PSO2PluginJsonObject jo;
            PSO2Plugin roooaar;
            foreach (JToken _id in _jObject.Children())
                foreach (JToken _property in _id.Children())
                {
                    roooaar = null;
                    jo = _property.ToObject<PSO2PluginJsonObject>();
                    if (jo.MD5Hash.ToLower() != "no")
                    {
                        using (var jr = _id.CreateReader())
                            if (jr.Read())
                                if (jr.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                                    jo.ID = jr.Value as string;
                        roooaar = jo.ToPSO2Plugin();
                        if (result.TryAdd(jo.ID, roooaar))
                            result[jo.ID] = roooaar;
                        roooaar.EnableChanged += PSO2Plugin_EnableChanged;
                        roooaar.HandledException += PSO2Plugin_HandledException;
                    }
                }
            return result;
        }
#else
        private ConcurrentDictionary<string, PSO2Plugin> ReadPluginList(string jsonString)
        {
            ConcurrentDictionary<string, PSO2Plugin> result = new ConcurrentDictionary<string, PSO2Plugin>(StringComparer.OrdinalIgnoreCase);
            JObject _jObject = JObject.Parse(jsonString);
            PSO2PluginJsonObject jo;
            //PSO2Plugin roooaar;
            foreach (JToken _id in _jObject.Children())
                foreach (JToken _property in _id.Children())
                {
                    try
                    {
                        jo = _property.ToObject<PSO2PluginJsonObject>();
                        if (jo.MD5Hash.ToLower() != "no")
                        {
                            using (var jr = _id.CreateReader())
                                if (jr.Read())
                                    if (jr.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                                        jo.ID = jr.Value as string;
                            AddPlugin(result, jo.ToPSO2Plugin());
                        }
                        //WebClientPool.SynchronizationContext?.Send(new System.Threading.SendOrPostCallback(delegate { System.Windows.Forms.MessageBox.Show(jo.ToString(), "awgliahwg"); }), null);
                    }
                    catch { }
                }
            return result;
        }
#endif

        private void AddPlugin(ConcurrentDictionary<string, PSO2Plugin> dict, PSO2Plugin roooaar)
        {
            if (!dict.TryAdd(roooaar.PluginID.ToLower(), roooaar))
                dict[roooaar.PluginID] = roooaar;
            roooaar.EnableChanged += PSO2Plugin_EnableChanged;
            roooaar.HandledException += PSO2Plugin_HandledException;
        }

        public IEnumerator<KeyValuePair<string, PSO2Plugin>> GetEnumerator()
        {
            return _PluginList.GetEnumerator();
        }

        internal class PluginCheckResult
        {
            public Dictionary<string, PSO2Plugin> Result { get; }
            public DateTime Version { get; }

            public PluginCheckResult(DateTime _ver, Dictionary<string, PSO2Plugin> _re)
            {
                this.Version = _ver;
                this.Result = _re;
            }
        }

        internal class PluginGetResult
        {
            public DateTime Version { get; }
            public string Result { get; }

            public PluginGetResult(DateTime _ver, string _re)
            {
                this.Version = _ver;
                this.Result = _re;
            }
        }
    }
}
