using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.Components.WebClientManger
{
    class ExtendedWebClient : IDisposable
    {
        private System.Threading.SynchronizationContext syncContext;
        private BaseWebClient innerWebClient;
        private short retried;
        private string downloadfileLocalPath;

        public ExtendedWebClient()
        {
            this.syncContext = WebClientPool.SynchronizationContext;
            this.innerWebClient = new BaseWebClient();
            this.innerWebClient.DownloadStringCompleted += InnerWebClient_DownloadStringCompleted;
            this.innerWebClient.DownloadDataCompleted += InnerWebClient_DownloadDataCompleted;
            this.innerWebClient.DownloadFileCompleted += InnerWebClient_DownloadFileCompleted;
            this.innerWebClient.DownloadProgressChanged += InnerWebClient_DownloadProgressChanged;
            this.retried = 0;
            this.Retry = 4;
            this.LastURL = null;
            this.downloadfileLocalPath = null;
            this.IsBusy = false;
        }

        private void InnerWebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.UserState is DownloadAsyncWrapper)
            { }
            else
            {
                this.DownloadProgressChanged?.Invoke(sender, e);
                //this.OnDownloadProgressChanged(e);
            }
        }

        #region "Properties"
        public bool AutoCredentials { get { return this.innerWebClient.AutoCredentials; } set { this.innerWebClient.AutoCredentials = value; } }
        public ICredentials Credentials { get { return this.innerWebClient.Credentials; } set { this.innerWebClient.Credentials = value; } }
        public bool AutoUserAgent { get { return this.innerWebClient.AutoUserAgent; } set { this.innerWebClient.AutoUserAgent = value; } }
        public CacheStorage CacheStorage { get { return this.innerWebClient.CacheStorage; } set { this.innerWebClient.CacheStorage = value; } }
        private short Retry { get; set; }
        public bool IsBusy { get; private set; }
        public Uri LastURL { get; private set; }
        public WebHeaderCollection Headers { get { return this.innerWebClient.Headers; } set { this.innerWebClient.Headers = value; } }
        public IWebProxy Proxy { get { return this.innerWebClient.Proxy; } set { this.innerWebClient.Proxy = value; } }
        public System.Net.Cache.RequestCachePolicy CachePolicy { get { return this.innerWebClient.CachePolicy; } set { this.innerWebClient.CachePolicy = value; } }
        public int TimeOut { get { return this.innerWebClient.TimeOut; } set { this.innerWebClient.TimeOut = value; } }
        public string UserAgent { get { return this.innerWebClient.UserAgent; } set { this.innerWebClient.UserAgent = value; } }
        public WebHeaderCollection ResponseHeaders { get { return this.innerWebClient.ResponseHeaders; } }
        #endregion

        #region "Open"
        public WebRequest CreateRequest(Uri url, string _method, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout, System.Net.Cache.RequestCachePolicy _cachePolicy)
        {
            return innerWebClient.CreateRequest(url, _method, _headers, _proxy, _timeout, _cachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout, System.Net.Cache.RequestCachePolicy _cachePolicy)
        {
            return this.CreateRequest(url, string.Empty, _headers, _proxy, _timeout, _cachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout)
        {
            return this.CreateRequest(url, _headers, _proxy, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers, IWebProxy _proxy)
        {
            return this.CreateRequest(url, _headers, _proxy, this.TimeOut, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers, int _timeout)
        {
            return this.CreateRequest(url, _headers, null, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, IWebProxy _proxy, int _timeout)
        {
            return this.CreateRequest(url, this.Headers, _proxy, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, IWebProxy _proxy)
        {
            return this.CreateRequest(url, this.Headers, _proxy, this.TimeOut, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers)
        {
            return this.CreateRequest(url, _headers, null, this.TimeOut, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, int _timeout)
        {
            return this.CreateRequest(url, this.Headers, null, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, string _method, int _timeout)
        {
            return this.CreateRequest(url, _method, this.Headers, null, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, string _method)
        {
            return this.CreateRequest(url, _method, this.TimeOut);
        }

        public WebRequest CreateRequest(Uri url)
        {
            return this.CreateRequest(url, this.TimeOut);
        }

        public WebRequest CreateRequest(string url, string _method, int _timeout)
        {
            return this.CreateRequest(new Uri(url), _method, this.Headers, null, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(string url, string _method)
        {
            return this.CreateRequest(new Uri(url), _method, this.TimeOut);
        }

        public WebRequest CreateRequest(string url)
        {
            return this.CreateRequest(new Uri(url));
        }

        public WebRequest CreateRequest(string url, int _timeout)
        {
            return this.CreateRequest(new Uri(url), _timeout);
        }

        public WebRequest CreateRequest(string url, IWebProxy _proxy)
        {
            return this.CreateRequest(new Uri(url), _proxy);
        }

        public WebRequest CreateRequest(string url, IWebProxy _proxy, int _timeout)
        {
            return this.CreateRequest(new Uri(url), _proxy, _timeout);
        }

        public WebRequest CreateRequest(string url, WebHeaderCollection _headers)
        {
            return this.CreateRequest(new Uri(url), _headers);
        }

        public WebRequest CreateRequest(string url, WebHeaderCollection _headers, int _timeout)
        {
            return this.CreateRequest(new Uri(url), _headers, null, _timeout, this.CachePolicy);
        }

        private WebResponse Open(WebRequest request)
        {
            this.LastURL = request.RequestUri;
            return this.innerWebClient.Open(request);
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout, System.Net.Cache.RequestCachePolicy _cachePolicy)
        {
            return this.Open(this.CreateRequest(url, _headers, _proxy, _timeout, _cachePolicy));
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout)
        {
            return this.Open(url, _headers, _proxy, _timeout, this.CachePolicy);
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers, IWebProxy _proxy)
        {
            return this.Open(url, _headers, _proxy, this.TimeOut, this.CachePolicy);
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers, int _timeout)
        {
            return this.Open(url, _headers, null, _timeout, this.CachePolicy);
        }

        public WebResponse Open(Uri url, IWebProxy _proxy, int _timeout)
        {
            return this.Open(url, this.Headers, _proxy, _timeout, this.CachePolicy);
        }

        public WebResponse Open(Uri url, IWebProxy _proxy)
        {
            return this.Open(url, this.Headers, _proxy, this.TimeOut, this.CachePolicy);
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers)
        {
            return this.Open(url, _headers, null, this.TimeOut, this.CachePolicy);
        }

        public WebResponse Open(Uri url, int _timeout)
        {
            return this.Open(url, this.Headers, null, _timeout, this.CachePolicy);
        }

        public WebResponse Open(Uri url)
        {
            return this.Open(url, this.Headers, null, this.TimeOut, this.CachePolicy);
        }

        public WebResponse Open(string url)
        {
            return this.Open(new Uri(url));
        }

        public WebResponse Open(string url, int _timeout)
        {
            return this.Open(new Uri(url), _timeout);
        }

        public WebResponse Open(string url, IWebProxy _proxy)
        {
            return this.Open(new Uri(url), _proxy);
        }

        public WebResponse Open(string url, IWebProxy _proxy, int _timeout)
        {
            return this.Open(new Uri(url), _proxy, _timeout);
        }

        public WebResponse Open(string url, WebHeaderCollection _headers)
        {
            return this.Open(new Uri(url), _headers);
        }

        public WebResponse Open(string url, WebHeaderCollection _headers, int _timeout)
        {
            return this.Open(new Uri(url), _headers, null, _timeout, this.CachePolicy);
        }
        #endregion

        #region "OpenRead"
        public Stream OpenRead(Uri url)
        {
            this.LastURL = url;
            return this.innerWebClient.OpenRead(url);
        }

        public Stream OpenRead(string url)
        {
            return this.OpenRead(new Uri(url));
        }
        #endregion

        #region "DownloadString"
        public string DownloadString(string address)
        {
            return this.DownloadString(new Uri(address));
        }

        public string DownloadString(Uri address)
        {
            string result = string.Empty;
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                for (short i = 0; i < Retry; i++)
                {
                    try
                    {
                        result = this.innerWebClient.DownloadString(address);
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (ex.Response is HttpWebResponse && !WorthRetry(ex.Response as HttpWebResponse))
                                throw ex;
                        }
                        else
                            throw ex;
                    }
                }
                OnWorkFinished();
            }
            return result;
        }

        public void DownloadStringAsync(Uri address)
        {
            this.DownloadStringAsync(address, null);
        }

        public void DownloadStringAsync(Uri address, object userToken)
        {
            this.retried = 0;
            this.LastURL = address;
            this.IsBusy = true;
            this.innerWebClient.DownloadStringAsync(address, userToken);
        }

        private void InnerWebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is WebException)
                {
                    WebException ex = e.Error as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL, e.UserState);
                        else
                            this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(ex, e.Cancelled, e.Result, e.UserState));
                    }
                    else
                        this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState));
                }
                else if (e.Error.InnerException is WebException)
                {
                    WebException ex = e.Error.InnerException as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL);
                        else
                            this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(ex, e.Cancelled, e.Result, e.UserState));
                    }
                    else
                        this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(e.Error.InnerException, e.Cancelled, e.Result, e.UserState));
                }
            }
            else if (e.Cancelled)
            { this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState)); }
            else
            {
                this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState));
            }
        }
        #endregion

        #region "DownloadData"
        public byte[] DownloadData(string address)
        {
            return this.DownloadData(new Uri(address));
        }

        public byte[] DownloadData(Uri address)
        {
            byte[] result = null;
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                for (short i = 0; i < Retry; i++)
                {
                    try
                    {
                        result = this.innerWebClient.DownloadData(address);
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (ex.Response is HttpWebResponse && !WorthRetry(ex.Response as HttpWebResponse))
                                throw ex;
                        }
                        else
                            throw ex;
                    }
                }
                OnWorkFinished();
            }
            return result;
        }

        public void DownloadDataAsync(Uri address)
        {
            this.DownloadDataAsync(address, null);
        }

        public void DownloadDataAsync(Uri address, object userToken)
        {
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                this.IsBusy = true;
                this.innerWebClient.DownloadDataAsync(address, userToken);
            }
        }

        private void InnerWebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is WebException)
                {
                    WebException ex = e.Error as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadDataAsync(this.LastURL, e.UserState);
                        else
                            this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(ex, e.Cancelled, e.Result, e.UserState));
                    }
                    else
                        this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState));
                }
                else if (e.Error.InnerException is WebException)
                {
                    WebException ex = e.Error.InnerException as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL);
                        else
                            this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(ex, e.Cancelled, e.Result, e.UserState));
                    }
                    else
                        this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(e.Error.InnerException, e.Cancelled, e.Result, e.UserState));
                }
            }
            else if (e.Cancelled)
            { this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState)); }
            else
            { this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState)); }
        }
        #endregion

        #region "DownloadFile"
        public void DownloadFile(string address, string localpath)
        {
            this.DownloadFile(new Uri(address), localpath);
        }

        public bool DownloadFile(Uri address, string localpath)
        {
            bool result = false;
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                FileInfo asd = new FileInfo(localpath + ".dtmp");
                for (short i = 0; i < Retry; i++)
                    try
                    {
                        if (asd.Exists)
                            asd.Open(FileMode.Open, FileAccess.ReadWrite).Close();
                        else
                            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(asd.DirectoryName);
                        this.innerWebClient.DownloadFile(address, asd.FullName);
                        if (IsHTTP(address))
                            if (!string.IsNullOrEmpty(this.innerWebClient.ResponseHeaders[HttpResponseHeader.ContentLength]))
                                if (long.Parse(this.innerWebClient.ResponseHeaders[HttpResponseHeader.ContentLength]) != asd.Length)
                                    throw new WebException($"Session '{address.OriginalString}' aborted.", WebExceptionStatus.RequestCanceled);
                        File.Delete(localpath);
                        asd.MoveTo(localpath);
                        //File.Move(asd.FullName, localpath);
                        result = true;
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (ex.Response is HttpWebResponse)
                                if (!WorthRetry(ex.Response as HttpWebResponse))
                                    throw ex;
                        }
                        else
                            throw ex;
                    }
                OnWorkFinished();
            }
            return result;
        }

        public void DownloadFileAsync(Uri address, string localPath)
        {
            this.DownloadFileAsync(address, localPath, null);
        }

        public void DownloadFileAsync(Uri address, string localPath, object userToken)
        {
            if (!this.IsBusy)
            {
                OnWorkStarted();
                DownloadFileAsyncEx(address, localPath, userToken);
            }
        }

        private void DownloadFileAsyncEx(Uri address, string localPath, object userToken)
        {
            this.retried = 0;
            this.downloadfileLocalPath = localPath;
            this.LastURL = address;
            this.IsBusy = true;
            if (File.Exists(localPath))
                File.Open(localPath, FileMode.Open).Close();
            else
                Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(Path.GetDirectoryName(localPath));
            this.innerWebClient.DownloadFileAsync(address, localPath + ".dtmp", userToken);
        }

        public void DownloadFileListAsync(Dictionary<Uri, string> fileList, object userToken)
        {
            if (fileList.Count > 0)
            {
                DownloadInfoCollection list = new DownloadInfoCollection();
                foreach (var fileNode in fileList)
                    list.Add(new DownloadInfo(fileNode.Key, fileNode.Value));
                this.DownloadFileListAsync(list, userToken);
            }
        }

        public void DownloadFileListAsync(List<DownloadInfo> filelist, object userToken)
        {
            this.DownloadFileListAsync(new DownloadInfoCollection(filelist), userToken);
        }

        public void DownloadFileListAsync(DownloadInfo[] filelist, object userToken)
        {
            this.DownloadFileListAsync(new DownloadInfoCollection(filelist), userToken);
        }

        public void DownloadFileListAsync(DownloadInfoCollection filelist, object userToken)
        {
            if (!this.IsBusy)
            {
                if (!filelist.IsEmpty)
                {
                    DownloadInfo item = filelist.TakeFirst();
                    this.DownloadFileAsyncEx(item.URL, item.Filename, new DownloadAsyncWrapper(filelist, userToken));
                }
            }
        }

        private void InnerWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            object token = null;
            DownloadAsyncWrapper info = null;
            if (e.UserState != null)
            {
                if (e.UserState is DownloadAsyncWrapper)
                {
                    info = e.UserState as DownloadAsyncWrapper;
                    token = info.userToken;
                }
                else
                    token = e.UserState;
            }
            if (e.Error != null)
            {
                if (e.Error is WebException)
                {
                    WebException ex = e.Error as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadFileAsync(this.LastURL, this.downloadfileLocalPath + ".dtmp", e.UserState);
                        else
                            this.SeekActionDerpian(info, e, token);
                    }
                    else
                        this.SeekActionDerpian(info, e, token);
                }
                else if (e.Error.InnerException is WebException)
                {

                    WebException ex = e.Error.InnerException as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL);
                        else
                            this.SeekActionDerpian(info, e, token);
                    }
                    else
                        this.SeekActionDerpian(info, e, token);
                }
                else
                {
                    this.SeekActionDerpian(info, e, token);
                }
            }
            else if (e.Cancelled)
            { this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, token)); }
            else
            {
                try
                {
                    File.Delete(this.downloadfileLocalPath);
                    File.Move(this.downloadfileLocalPath + ".dtmp", this.downloadfileLocalPath);
                }
                catch (Exception ex) { Log.LogManager.GeneralLog.Print(ex); }
                this.SeekActionDerpian(info, e, token);
            }
        }

        private void SeekActionDerpian(DownloadAsyncWrapper info, AsyncCompletedEventArgs e, object token)
        {
            if ((info != null) && (!info.filelist.IsEmpty))
            {
                DownloadInfo item = info.filelist.TakeFirst();
                if (item == null)
                {
                    this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, token));
                    return;
                }
                this.OnDownloadFileProgressChanged(new DownloadFileProgressChangedEventArgs(info.filelist.CurrentItemCount, info.filelist.Count));
                this.DownloadFileAsyncEx(item.URL, item.Filename, info);
            }
            else
            { this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, token)); }
        }
        #endregion

        #region "Public Methods"
        public void Dispose()
        {
            this.innerWebClient.Dispose();
        }

        public void CancelAsync()
        {
            this.innerWebClient.CancelAsync();
        }

        public void ClearProgressEvents()
        {
            DownloadProgressChangedEventHandler myDe = this.DownloadProgressChanged;
            while (myDe != null)
            {
                this.DownloadProgressChanged -= myDe;
                myDe = this.DownloadProgressChanged;
            }
            /*
            FieldInfo f1 = typeof(ExtendedWebClient).GetField("DownloadProgressChanged", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(this);
            PropertyInfo pi = this.GetType().GetProperty("Events",
                BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(this, null);
            list.RemoveHandler(obj, list[obj]);
            */
        }
        #endregion

        #region "Private Methods"
        private bool IsHTTP(Uri url)
        {
            if (url == null)
                return false;
            else
            {
                if ((url.Scheme == Uri.UriSchemeHttp) || (url.Scheme == Uri.UriSchemeHttps))
                    return true;
                else
                    return false;
            }
        }

        private bool WorthRetry(HttpWebResponse rep)
        {
            if (retried > this.Retry)
                return false;
            else
            {
                retried++;
                switch (rep.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return false;
                    case HttpStatusCode.NotAcceptable:
                        return false;
                    case HttpStatusCode.Unauthorized:
                        return false;
                    case HttpStatusCode.ServiceUnavailable:
                        return false;
                    case HttpStatusCode.NotImplemented:
                        return false;
                    case HttpStatusCode.PaymentRequired:
                        return false;
                    case HttpStatusCode.PreconditionFailed:
                        return false;
                    case HttpStatusCode.ProxyAuthenticationRequired:
                        return false;
                    default:
                        return true;
                }
            }
        }
        #endregion

        #region "Events"
        internal event EventHandler WorkFinished;
        protected void OnWorkFinished()
        {
            this.IsBusy = false;
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.WorkFinished?.Invoke(this, EventArgs.Empty); }), null);
        }
        internal event EventHandler WorkStarted;
        protected void OnWorkStarted()
        {
            this.IsBusy = true;
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.WorkStarted?.Invoke(this, EventArgs.Empty); }), null);
        }

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        protected void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadProgressChanged.Invoke(this, e); }), null);
        }

        public event AsyncCompletedEventHandler DownloadFileCompleted;
        protected void OnDownloadFileCompleted(AsyncCompletedEventArgs e)
        {
            OnWorkFinished();
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadFileCompleted?.Invoke(this, e); }), null);
        }

        public delegate void DownloadDataFinishedEventHandler(object sender, DownloadDataFinishedEventArgs e);
        public event DownloadDataFinishedEventHandler DownloadDataCompleted;
        protected void OnDownloadDataFinished(DownloadDataFinishedEventArgs e)
        {
            OnWorkFinished();
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadDataCompleted?.Invoke(this, e); }), null);
        }
        public class DownloadDataFinishedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
        {
            public byte[] Result { get; }
            public DownloadDataFinishedEventArgs(Exception ex, bool cancel, byte[] taskresult, object userToken) : base(ex, cancel, userToken)
            {
                this.Result = taskresult;
            }
        }

        public delegate void DownloadStringFinishedEventHandler(object sender, DownloadStringFinishedEventArgs e);
        public event DownloadStringFinishedEventHandler DownloadStringCompleted;
        protected void OnDownloadStringFinished(DownloadStringFinishedEventArgs e)
        {
            OnWorkFinished();
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadStringCompleted?.Invoke(this, e); }), null);
        }
        public class DownloadStringFinishedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
        {
            public string Result { get; }
            public DownloadStringFinishedEventArgs(Exception ex, bool cancel, string taskresult, object userToken) : base(ex, cancel, userToken)
            {
                this.Result = taskresult;
            }
        }

        public delegate void DownloadFileProgressChangedEventHandler(object sender, DownloadFileProgressChangedEventArgs e);
        public event DownloadFileProgressChangedEventHandler DownloadFileProgressChanged;
        protected void OnDownloadFileProgressChanged(DownloadFileProgressChangedEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadFileProgressChanged?.Invoke(this, e); }), null);
        }
        internal class DownloadFileProgressChangedEventArgs : System.EventArgs
        {
            public int CurrentFileIndex { get; }
            public int TotalFileCount { get; }
            public int Percent { get; }
            public DownloadFileProgressChangedEventArgs(int current, int total)
            {
                this.CurrentFileIndex = current;
                this.TotalFileCount = total;
                this.Percent = (int)((CurrentFileIndex * 100) / TotalFileCount);
            }
        }
        #endregion

        #region "Private Class"
        private class DownloadAsyncWrapper
        {
            public object userToken { get; }
            public DownloadInfoCollection filelist;
            public DownloadAsyncWrapper(DownloadInfoCollection list, object token)
            {
                this.userToken = token;
                this.filelist = list;
            }
            public DownloadAsyncWrapper(DownloadInfoCollection list) : this(list, null) { }
        }
        internal class DownloadInfo
        {
            public Uri URL
            { get; private set; }
            public string Filename
            { get; private set; }

            public DownloadInfo(Uri uUrl, string sFilename)
            {
                this.URL = uUrl;
                this.Filename = sFilename;
            }
        }

        internal class DownloadInfoCollection
        {
            private ConcurrentQueue<DownloadInfo> myList;
            public bool IsEmpty
            { get { return this.myList.IsEmpty; } }
            public int Count
            { get; private set; }
            private int _currentItemCount;
            public int CurrentItemCount
            { get { return this._currentItemCount; } }
            public DownloadInfoCollection(IEnumerable<DownloadInfo> list)
            {
                this.myList = new ConcurrentQueue<DownloadInfo>(list);
                this.Count = 0;
                this._currentItemCount = 0;
            }
            public DownloadInfoCollection()
            {
                this.myList = new ConcurrentQueue<DownloadInfo>();
                this.Count = 0;
                this._currentItemCount = 0;
            }

            public void Add(string sUrl, string sFilename)
            {
                this.Add(new DownloadInfo(new Uri(sUrl), sFilename));
            }

            public void Add(Uri uUrl, string sFilename)
            {
                this.Add(new DownloadInfo(uUrl, sFilename));
            }

            public void Add(DownloadInfo item)
            {
                this.Count++;
                this.myList.Enqueue(item);
            }

            public DownloadInfo TakeFirst()
            {
                if (this.IsEmpty)
                    return null;
                else
                {
                    DownloadInfo result;
                    if (this.myList.TryDequeue(out result))
                    {
                        this._currentItemCount++;
                        return result;
                    }
                    else
                        return null;
                }
            }

            public void Dispose()
            {
                this.myList = null;
            }
        }
        #endregion
    }
}
