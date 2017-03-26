using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace PSO2ProxyLauncherNew.Classes.Components.WebClientManger
{
    internal class BaseWebClient : WebClient
    {
        private BackgroundWorker worker;
        public BaseWebClient(CookieContainer cookies = null, bool autoRedirect = true) : base()
        {
            this.CookieContainer = cookies ?? new CookieContainer();
            this.AutoRedirect = autoRedirect;
            this.UserAgent = "Mozilla/4.0";
            this.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            this.Proxy = null;
            this.TimeOut = 5000;
            this.ReadTimeOut = 1800000;
            this._response = null;
            this.CacheStorage = null;
            this.worker = new BackgroundWorker();
            this.worker.WorkerSupportsCancellation = true;
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += this.Worker_DoWork;
            this.worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
            this.worker.ProgressChanged += this.Worker_ProgressChanged;
        }

        public BaseWebClient(int iTimeOut, CookieContainer cookies = null, bool autoRedirect = true) : this(cookies, autoRedirect)
        {
            this.TimeOut = iTimeOut;
        }
        public string UserAgent { get; set; }
        public int TimeOut { get; set; }
        public int ReadTimeOut { get; set; }
        public Uri CurrentURL { get; private set; }
        private WebResponse _response;

        public new bool IsBusy { get { return (base.IsBusy || this.worker.IsBusy); } }

        /// Initializes a new instance of the BetterWebClient class.  <pa...

        /// Gets or sets a value indicating whether to automatically redi...
        public bool AutoRedirect { get; set; }

        /// Gets or sets the cookie container. This contains all the cook...
        public CookieContainer CookieContainer { get; set; }

        /// Gets the cookies header (Set-Cookie) of the last request.
        public string Cookies
        {
            get { return GetHeaderValue("Set-Cookie"); }
        }

        public CacheStorage CacheStorage { get; set; }

        /// Gets the location header for the last request.
        public string Location
        {
            get { return GetHeaderValue("Location"); }
        }

        /// Gets the status code. When no request is present, <see cref="...
        public HttpStatusCode StatusCode
        {
            get
            {
                var result = HttpStatusCode.Gone;
                if (_response != null && this.IsHTTP())
                {
                    try
                    {
                        var rep = _response as HttpWebResponse;
                        result = rep.StatusCode;
                    }
                    catch
                    { result = HttpStatusCode.Gone; }
                }
                return result;
            }
        }

        /// Gets or sets the setup that is called before the request is d...
        public Action<HttpWebRequest> Setup { get; set; }

        /// Gets the header value.
        public string GetHeaderValue(string headerName)
        {
            if (_response == null)
                return null;
            else
            {
                string result = null;
                result = _response.Headers?[headerName];
                return result;
            }
        }

        public string GetHeaderValue(HttpResponseHeader headerenum)
        {
            if (_response == null)
                return null;
            else
            {
                string result = null;
                result = _response.Headers?[headerenum];
                return result;
            }
        }

        /// Returns a <see cref="T:System.Net.WebRequest" /> object for t...
        protected override WebRequest GetWebRequest(Uri address)
        {
            this.CurrentURL = address;
            var request = base.GetWebRequest(address);
            if (this.IsHTTP())
            {
                HttpWebRequest httpRequest = request as HttpWebRequest;
                if (request != null)
                {
                    httpRequest.AllowAutoRedirect = AutoRedirect;
                    httpRequest.CookieContainer = CookieContainer;
                    httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    httpRequest.Timeout = this.TimeOut;
                    if ((this.Headers != null) && (this.Headers.HasKeys()))
                        httpRequest.Headers = this.Headers;
                    if (!string.IsNullOrEmpty(UserAgent))
                        httpRequest.UserAgent = this.UserAgent;
                    Setup?.Invoke(httpRequest);
                }
            }
            else
            {
                WebRequest Request = request as WebRequest;
                Request.Timeout = this.TimeOut;
                if ((this.Headers != null) && (this.Headers.HasKeys()))
                    Request.Headers = this.Headers;
            }
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            this._response = base.GetWebResponse(request);
            string lastmod = this._response.Headers[HttpResponseHeader.LastModified];
            if ((this._response.Headers != null) && (this._response.Headers.HasKeys()))
            {
                this.ResponseHeaders.Clear();
                foreach (string s in this._response.Headers.AllKeys)
                    this.ResponseHeaders[s] = this._response.Headers.Get(s);
                if (!string.IsNullOrEmpty(lastmod))
                    this.ResponseHeaders.Add(HttpResponseHeader.LastModified, lastmod);
            }

            if (this.CacheStorage != null && this.IsHTTP())
                if (!request.RequestUri.IsFile && !request.RequestUri.IsLoopback && request.RequestUri.IsAbsoluteUri)
                {
                    if (this._response != null && !string.IsNullOrWhiteSpace(lastmod))
                    {
                        DateTime remoteLastModified = DateTime.MinValue;
                        if (lastmod.EndsWith("UTC"))
                            remoteLastModified = DateTime.ParseExact(lastmod, "ddd, dd MMM yyyy HH:mm:ss 'UTC'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
                        else if (lastmod.EndsWith("GMT"))
                            remoteLastModified = DateTime.ParseExact(lastmod, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
                        else
                            remoteLastModified = DateTime.ParseExact(lastmod, "ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
                        CacheInfo _cacheinfo = this.CacheStorage.GetCacheFromURL(request.RequestUri);
                        if (remoteLastModified != DateTime.MinValue)
                        {
                            if (_cacheinfo.Exists && remoteLastModified == _cacheinfo.LastModifiedDate)
                            {
                                //System.Windows.Forms.MessageBox.Show(lastmod + "\n\n" + remoteLastModified.ToString() + "\n\n" + _cacheinfo.LastModifiedDate.ToString());
                                request.Abort();
                                this._response.Close();
                                var filerequest = FileWebRequest.Create(_cacheinfo.LocalURI);
                                filerequest.Proxy = null;
                                filerequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                                filerequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                                this._response = filerequest.GetResponse();
                            }
                            else
                            {
                                using (Stream s = this._response.GetResponseStream())
                                {
                                    try { s.ReadTimeout = this.ReadTimeOut; }
                                    catch (InvalidOperationException) { }
                                    if (this._response.ContentLength > 0)
                                        _cacheinfo.CreateFromStream(s, remoteLastModified, (sender, e) => {
                                            e.Cancel = this.worker.CancellationPending;
                                            this.OnDownloadProgressChanged(this.GetDownloadProgressChangedEventArgs(null, e.BytesReceived, this._response.ContentLength));
                                        });
                                    else
                                        _cacheinfo.CreateFromStream(s, remoteLastModified);
                                }
                                if (this.worker.CancellationPending)
                                {
                                    this._response.Close();
                                    throw new WebException("User cancelled the request.", WebExceptionStatus.RequestCanceled);
                                }
                                else if (this._response.ContentLength > 0 && this._response.ContentLength > _cacheinfo.FileSize)
                                {
                                    this._response.Close();
                                    throw new WebException("The request has been closed unexpectedly.", WebExceptionStatus.ConnectionClosed);
                                }
                                var filerequest = FileWebRequest.Create(_cacheinfo.LocalURI);
                                filerequest.Proxy = null;
                                filerequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                                filerequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                                this._response = filerequest.GetResponse();
                            }
                        }
                    }
                }

            return this._response;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            Console.WriteLine("Noooooo~! You can't be here. It must be wrong so that you can reach here.");

            //DownloadDataCompletedEventArgs(byte[] result, System.Exception exception, bool cancelled, object userToken);
            this._response = base.GetWebResponse(request, result);
            string lastmod = this._response.Headers[HttpResponseHeader.LastModified];
            if ((this._response.Headers != null) && (this._response.Headers.HasKeys()))
            {
                this.ResponseHeaders.Clear();
                foreach (string s in this._response.Headers.AllKeys)
                    this.ResponseHeaders[s] = this._response.Headers.Get(s);
                if (!string.IsNullOrEmpty(lastmod))
                    this.ResponseHeaders.Add(HttpResponseHeader.LastModified, lastmod);
            }

            if (this.CacheStorage != null && this.IsHTTP())
                if (!request.RequestUri.IsFile && !request.RequestUri.IsLoopback && request.RequestUri.IsAbsoluteUri)
                {
                    if (this._response != null && !string.IsNullOrWhiteSpace(lastmod))
                    {
                        DateTime remoteLastModified = DateTime.MinValue;
                        if (lastmod.EndsWith("UTC"))
                            remoteLastModified = DateTime.ParseExact(lastmod, "ddd, dd MMM yyyy HH:mm:ss 'UTC'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
                        else if (lastmod.EndsWith("GMT"))
                            remoteLastModified = DateTime.ParseExact(lastmod, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
                        else
                            remoteLastModified = DateTime.ParseExact(lastmod, "ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
                        CacheInfo _cacheinfo = this.CacheStorage.GetCacheFromURL(request.RequestUri);
                        if (remoteLastModified != DateTime.MinValue)
                        {
                            if (remoteLastModified == _cacheinfo.LastModifiedDate)
                            {
                                //System.Windows.Forms.MessageBox.Show(lastmod + "\n\n" + remoteLastModified.ToString() + "\n\n" + _cacheinfo.LastModifiedDate.ToString());
                                request.Abort();
                                this._response.Close();
                                var filerequest = FileWebRequest.Create(_cacheinfo.LocalURI);
                                filerequest.Proxy = null;
                                filerequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                                filerequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                                this._response = filerequest.GetResponse();
                            }
                            else
                            {
                                using (Stream s = this._response.GetResponseStream())
                                {
                                    if (this._response.ContentLength > 0)
                                        _cacheinfo.CreateFromStream(s, remoteLastModified, (sender, e) => {
                                            e.Cancel = this.worker.CancellationPending;
                                            this.OnDownloadProgressChanged(this.GetDownloadProgressChangedEventArgs(null, e.BytesReceived, this._response.ContentLength));
                                        });
                                    else
                                        _cacheinfo.CreateFromStream(s, remoteLastModified);
                                }
                                if (this.worker.CancellationPending)
                                {
                                    this._response.Close();
                                    throw new WebException("User cancelled the request.", WebExceptionStatus.RequestCanceled);
                                }
                                else if (this._response.ContentLength > 0 && this._response.ContentLength > _cacheinfo.FileSize)
                                {
                                    this._response.Close();
                                    throw new WebException("The request has been closed unexpectedly.", WebExceptionStatus.ConnectionClosed);
                                }
                                var filerequest = FileWebRequest.Create(_cacheinfo.LocalURI);
                                filerequest.Proxy = null;
                                filerequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                                filerequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                                this._response = filerequest.GetResponse();
                            }
                        }
                    }
                }
            return this._response;
        }

        public new void CancelAsync()
        {
            base.CancelAsync();
            this.worker.CancelAsync();
        }

        public new void DownloadFileAsync(Uri address, string filename)
        {
            this.DownloadFileAsync(address, filename, null);
        }

        public new void DownloadFileAsync(Uri address, string filename, object UserToken)
        {
            WebRequest myRequest = this.GetWebRequest(address);
            this.CurrentTask = Task.DownloadFile;
            this.innerusertoken = UserToken;
            this.worker.RunWorkerAsync(new filerequestmeta(myRequest, filename));
        }

        public new void DownloadStringAsync(Uri address)
        {
            this.DownloadStringAsync(address, null);
        }

        public new void DownloadStringAsync(Uri address, object UserToken)
        {
            WebRequest myRequest = this.GetWebRequest(address);
            this.CurrentTask = Task.DownloadString;
            this.innerusertoken = UserToken;
            this.worker.RunWorkerAsync(new requestmeta(myRequest));
        }

        public new void DownloadDataAsync(Uri address)
        {
            this.DownloadDataAsync(address, null);
        }

        public new void DownloadDataAsync(Uri address, object UserToken)
        {
            WebRequest myRequest = this.GetWebRequest(address);
            this.CurrentTask = Task.DownloadData;
            this.innerusertoken = UserToken;
            this.worker.RunWorkerAsync(new requestmeta(myRequest));
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (this.CurrentTask)
            {
                case Task.DownloadFile:
                    var _filerequestmeta = e.Argument as filerequestmeta;
                    WebResponse myRespfile = this.GetWebResponse(_filerequestmeta.Request);
                    using (Stream networkStream = myRespfile.GetResponseStream())
                    using (BufferedStream bufferStream = new BufferedStream(networkStream, 1024))
                    using (FileStream localfile = File.Create(_filerequestmeta.Filename))
                    {
                        long totalread = 0;
                        byte[] arr = new byte[1024];
                        int readbyte = bufferStream.Read(arr, 0, arr.Length);
                        while (readbyte > 0)
                        {
                            if (this.worker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            localfile.Write(arr, 0, readbyte);
                            totalread += readbyte;
                            if (myRespfile.ContentLength > 0)
                                this.worker.ReportProgress(1, new DownloadProgressChangedStruct(null, totalread, myRespfile.ContentLength));
                            readbyte = bufferStream.Read(arr, 0, arr.Length);
                        }
                        localfile.Flush();
                    }
                    break;
                case Task.DownloadData:
                    var _requestmetadata = e.Argument as requestmeta;
                    WebResponse myRespdata = this.GetWebResponse(_requestmetadata.Request);
                    byte[] dataresult = null;
                    using (Stream networkStream = myRespdata.GetResponseStream())
                    using (BufferedStream bufferStream = new BufferedStream(networkStream, 1024))
                    using (MemoryStream localfile = new MemoryStream())
                    {
                        long totalread = 0;
                        byte[] arr = new byte[1024];
                        int readbyte = bufferStream.Read(arr, 0, arr.Length);
                        while (readbyte > 0)
                        {
                            if (this.worker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            localfile.Write(arr, 0, readbyte);
                            totalread += readbyte;
                            if (myRespdata.ContentLength > 0)
                                this.worker.ReportProgress(1, new DownloadProgressChangedStruct(null, totalread, myRespdata.ContentLength));
                            readbyte = bufferStream.Read(arr, 0, arr.Length);
                        }
                        localfile.Flush();
                        dataresult = localfile.ToArray();
                    }
                    e.Result = dataresult;
                    break;
                case Task.DownloadString:
                    var _requestmetastring = e.Argument as requestmeta;
                    WebResponse myRespstr = this.GetWebResponse(_requestmetastring.Request);
                    System.Text.StringBuilder stringresult = new System.Text.StringBuilder();
                    using (Stream networkStream = myRespstr.GetResponseStream())
                    using (BufferedStream bufferStream = new BufferedStream(networkStream, 1024))
                    {
                        long totalread = 0;
                        byte[] arr = new byte[1024];
                        int readbyte = bufferStream.Read(arr, 0, arr.Length);
                        while (readbyte > 0)
                        {
                            if (this.worker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            stringresult.Append(this.Encoding.GetString(arr, 0, readbyte));
                            totalread += readbyte;
                            if (myRespstr.ContentLength > 0)
                                this.worker.ReportProgress(1, new DownloadProgressChangedStruct(null, totalread, myRespstr.ContentLength));
                            readbyte = bufferStream.Read(arr, 0, arr.Length);
                        }
                    }
                    e.Result = stringresult.ToString();
                    break;
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1:
                    DownloadProgressChangedStruct progressmeta = e.UserState as DownloadProgressChangedStruct;
                    if (progressmeta != null)
                        this.OnDownloadProgressChanged(this.GetDownloadProgressChangedEventArgs(progressmeta.UserToken, progressmeta.BytesReceived, progressmeta.TotalBytesToReceive));
                    break;
            }
        }

        private object innerusertoken;

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Do not put the Task=None below this switch, otherwise:
            //The Completed method set the task, then the task again being set to None, which cause DoWork do nothing
            var suchConditionRace = this.CurrentTask;
            this.CurrentTask = Task.None;
            switch (suchConditionRace)
            {
                case Task.DownloadFile:
                    this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, innerusertoken));
                    break;
                case Task.DownloadString:
                    if (e.Error != null || e.Cancelled)
                        this.OnDownloadStringCompleted(GetDownloadStringCompletedEventArgs(null, e.Error, e.Cancelled, innerusertoken));
                    else
                        this.OnDownloadStringCompleted(GetDownloadStringCompletedEventArgs(e.Result as string, e.Error, e.Cancelled, innerusertoken));
                    break;
                case Task.DownloadData:
                    if (e.Error != null || e.Cancelled)
                        this.OnDownloadDataCompleted(GetDownloadDataCompletedEventArgs(null, e.Error, e.Cancelled, innerusertoken));
                    else
                        this.OnDownloadDataCompleted(GetDownloadDataCompletedEventArgs(e.Result as byte[], e.Error, e.Cancelled, innerusertoken));
                    break;
            }
        }

        private Task CurrentTask = Task.None;
        private enum Task : byte
        {
            None,
            DownloadFile,
            DownloadData,
            DownloadString
        }

        private class filerequestmeta : requestmeta
        {
            public string Filename { get; }
            public filerequestmeta(WebRequest _request, string _filename) : base(_request)
            {
                this.Filename = _filename;
            }
        }

        private class dataresultmeta : resultmeta
        {
            public byte[] Result { get; }
            public dataresultmeta(byte[] _data, object _usertoken) : base(_usertoken)
            {
                this.Result = _data;
            }
        }

        private class stringresultmeta : resultmeta
        {
            public string Result { get; }
            public stringresultmeta(string _str, object _usertoken) : base(_usertoken)
            {
                this.Result = _str;
            }
        }

        private class resultmeta
        {
            public object UserToken { get; }
            public resultmeta(object _usertoken)
            {
                this.UserToken = _usertoken;
            }
        }

        private class requestmeta
        {
            public WebRequest Request { get; }
            public requestmeta(WebRequest _request)
            {
                this.Request = _request;
            }
        }

        private bool IsHTTP()
        {
            if (this.CurrentURL == null)
                return false;
            else
            {
                if ((this.CurrentURL.Scheme == Uri.UriSchemeHttp) || (this.CurrentURL.Scheme == Uri.UriSchemeHttps))
                    return true;
                else
                    return false;
            }
        }

        protected DownloadDataCompletedEventArgs GetDownloadDataCompletedEventArgs(byte[] bytes, System.Exception ex, bool cancelled, object usertoken)
        {
            return (DownloadDataCompletedEventArgs)Activator.CreateInstance(typeof(DownloadDataCompletedEventArgs),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null, new object[] { bytes, ex, cancelled, usertoken }, null);
        }

        protected DownloadStringCompletedEventArgs GetDownloadStringCompletedEventArgs(string str, System.Exception ex, bool cancelled, object usertoken)
        {
            return (DownloadStringCompletedEventArgs)Activator.CreateInstance(typeof(DownloadStringCompletedEventArgs),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null, new object[] { str, ex, cancelled, usertoken }, null);
        }

        private class DownloadProgressChangedStruct
        {
            public long BytesReceived { get; }
            public long TotalBytesToReceive { get; }
            public object UserToken { get; }
            public DownloadProgressChangedStruct(object _userToken, long _bytesReceived, long _totalBytesToReceive)
            {
                this.UserToken = _userToken;
                this.TotalBytesToReceive = _totalBytesToReceive;
                this.BytesReceived = _bytesReceived;
            }
        }

        protected DownloadProgressChangedEventArgs GetDownloadProgressChangedEventArgs(object userToken, long bytesReceived, long totalBytesToReceive)
        {
            return this.GetDownloadProgressChangedEventArgs((int)((100F * bytesReceived) / totalBytesToReceive), userToken, bytesReceived, totalBytesToReceive);
        }

        protected DownloadProgressChangedEventArgs GetDownloadProgressChangedEventArgs(int progressPercentage, object userToken, long bytesReceived, long totalBytesToReceive)
        {
            /*return (DownloadStringCompletedEventArgs)typeof(DownloadStringCompletedEventArgs).GetConstructor(
                  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                  null, Type.EmptyTypes, null).Invoke(null);//*/
            return (DownloadProgressChangedEventArgs)Activator.CreateInstance(typeof(DownloadProgressChangedEventArgs),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null, new object[] { progressPercentage, userToken, bytesReceived, totalBytesToReceive }, null);
        }

        public new void Dispose()
        {
            base.Dispose();
            this.worker.Dispose();
        }
    }
}
