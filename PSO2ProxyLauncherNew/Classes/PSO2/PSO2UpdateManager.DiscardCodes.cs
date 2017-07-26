using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2ProxyLauncherNew.Classes.PSO2
{
    class PSO2UpdateManager
    {
        public bool RedownloadFiles(Dictionary<string, string> fileList, EventHandler<StringEventArgs> stepReport, Func<int, int, bool> downloadprogressReport, RunWorkerCompletedEventHandler downloadFinished_CallBack)
        {
            return RedownloadFiles(this.myWebClient, fileList, stepReport, downloadprogressReport, downloadFinished_CallBack);
        }

        /// <summary>
        /// Redownload files with given relative filenames.
        /// </summary>
        /// <returns>RunWorkerCompletedEventArgs. True if the download is succeeded, otherwise false.</returns>
        public static RunWorkerCompletedEventArgs RedownloadFile(ExtendedWebClient _webClient, string relativeFilename, string destinationFullfilename, Func<int, bool> progress_callback)
        {
            bool continueDownload = true;
            Exception Myex = null;
            Uri currenturl = null;
            DownloadProgressChangedEventHandler ooooo = null;
            if (progress_callback != null)
                ooooo = new DownloadProgressChangedEventHandler(delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    if (progress_callback.Invoke(e.ProgressPercentage))
                    {
                        continueDownload = false;
                        _webClient.CancelAsync();
                    }
                });
            if (ooooo != null)
                _webClient.DownloadProgressChanged += ooooo;
            try
            {
                HttpStatusCode lastCode;
                var _pso22fileurl = new PSO2FileUrl(Leayal.UriHelper.URLConcat(DefaultValues.Web.MainDownloadLink, relativeFilename), Leayal.UriHelper.URLConcat(DefaultValues.Web.OldDownloadLink, relativeFilename));
                currenturl = _pso22fileurl.MainUrl;
                lastCode = HttpStatusCode.ServiceUnavailable;
                try
                {
                    _webClient.AutoUserAgent = true;
                    _webClient.DownloadFile(currenturl, destinationFullfilename);
                    _webClient.AutoUserAgent = false;
                }
                catch (WebException webEx)
                {
                    if (webEx.Response != null)
                    {
                        HttpWebResponse rep = webEx.Response as HttpWebResponse;
                        lastCode = rep.StatusCode;
                    }
                    else
                        throw webEx;
                }
                if (lastCode == HttpStatusCode.NotFound)
                {
                    currenturl = _pso22fileurl.GetTheOtherOne(currenturl.OriginalString);
                    try
                    {
                        _webClient.AutoUserAgent = true;
                        _webClient.DownloadFile(currenturl, destinationFullfilename);
                        _webClient.AutoUserAgent = false;
                    }
                    catch (WebException webEx)
                    {
                        if (webEx.Response != null)
                        {
                            HttpWebResponse rep = webEx.Response as HttpWebResponse;
                            if (rep.StatusCode != HttpStatusCode.NotFound)
                                throw webEx;
                        }
                        else
                            throw webEx;
                    }
                }
            }
            catch (Exception ex) { Myex = ex; }
            if (ooooo != null)
                _webClient.DownloadProgressChanged -= ooooo;
            return new RunWorkerCompletedEventArgs(null, Myex, !continueDownload);
        }

        /// <summary>
        /// Redownload files with given relative filenames.
        /// </summary>
        /// <param name="stepReport">This method will be invoked everytime the download proceed to tell the filename. This is thread-safe invoke.</param>
        /// <param name="downloadprogressReport">This method will be invoked everytime the download proceed. This is thread-safe invoke.</param>
        /// <param name="downloadFinished_CallBack">This method will be invoked when the download is finished. This is thread-safe invoke.</param>
        /// <returns>Bool. True if the download is succeeded, otherwise false.</returns>
        public static bool RedownloadFiles(ExtendedWebClient _webClient, Dictionary<string, string> fileList, EventHandler<StringEventArgs> stepReport, Func<int, int, bool> downloadprogressReport, RunWorkerCompletedEventHandler downloadFinished_CallBack)
        {
            bool continueDownload = true;
            Exception Myex = null;
            int filecount = 0;
            Uri currenturl = null;
            var asdasdads = _webClient.CacheStorage;
            _webClient.CacheStorage = null;
            List<string> failedfiles = new List<string>();
            try
            {
                HttpStatusCode lastCode;
                byte[] buffer = new byte[1024];
                //long byteprocessed, filelength;
                foreach (var _keypair in fileList)
                {
                    if (stepReport != null)
                        WebClientPool.SynchronizationContext.Send(new System.Threading.SendOrPostCallback(delegate { stepReport.Invoke(_webClient, new StringEventArgs(_keypair.Key)); }), null);
                    using (FileStream local = File.Create(_keypair.Value, 1024))
                    {
                        var _pso22fileurl = new PSO2FileUrl(Leayal.UriHelper.URLConcat(DefaultValues.Web.MainDownloadLink, _keypair.Key), Leayal.UriHelper.URLConcat(DefaultValues.Web.OldDownloadLink, _keypair.Key));
                        currenturl = _pso22fileurl.MainUrl;
                        lastCode = HttpStatusCode.ServiceUnavailable;
                        //byteprocessed = 0;
                        //filelength = 0;
                        try
                        {
                            using (HttpWebResponse theRep = _webClient.Open(currenturl) as HttpWebResponse)
                            {
                                if (theRep.StatusCode == HttpStatusCode.NotFound)
                                    throw new WebException("File not found", null, WebExceptionStatus.ReceiveFailure, theRep);
                                else if (theRep.StatusCode == HttpStatusCode.Forbidden)
                                    throw new WebException("Access denied", null, WebExceptionStatus.ReceiveFailure, theRep);
                                /*if (theRep.ContentLength > 0)
                                    filelength = theRep.ContentLength;
                                else
                                {
                                    HttpWebRequest headReq = _webClient.CreateRequest(currenturl, "HEAD") as HttpWebRequest;
                                    headReq.AutomaticDecompression = DecompressionMethods.None;
                                    HttpWebResponse headRep = headReq.GetResponse() as HttpWebResponse;
                                    if (headRep != null)
                                    {
                                        if (headRep.ContentLength > 0)
                                            filelength = headRep.ContentLength;
                                        headRep.Close();
                                    }
                                }*/
                                using (var theRepStream = theRep.GetResponseStream())
                                {
                                    int count = theRepStream.Read(buffer, 0, buffer.Length);
                                    while (count > 0)
                                    {
                                        local.Write(buffer, 0, count);
                                        //byteprocessed += count;
                                        count = theRepStream.Read(buffer, 0, buffer.Length);
                                    }
                                }
                            }
                        }
                        catch (WebException webEx)
                        {
                            if (webEx.Response != null)
                            {
                                HttpWebResponse rep = webEx.Response as HttpWebResponse;
                                lastCode = rep.StatusCode;
                            }
                        }
                        if (lastCode == HttpStatusCode.NotFound)
                        {
                            currenturl = _pso22fileurl.GetTheOtherOne(currenturl.OriginalString);
                            try
                            {
                                using (HttpWebResponse theRep = _webClient.Open(currenturl) as HttpWebResponse)
                                {
                                    if (theRep.StatusCode == HttpStatusCode.NotFound)
                                        throw new WebException("File not found", null, WebExceptionStatus.ReceiveFailure, theRep);
                                    else if (theRep.StatusCode == HttpStatusCode.Forbidden)
                                        throw new WebException("Access denied", null, WebExceptionStatus.ReceiveFailure, theRep);
                                    /*if (theRep.ContentLength > 0)
                                        filelength = theRep.ContentLength;
                                    else
                                    {
                                        HttpWebRequest headReq = _webClient.CreateRequest(currenturl, "HEAD") as HttpWebRequest;
                                        headReq.AutomaticDecompression = DecompressionMethods.None;
                                        HttpWebResponse headRep = headReq.GetResponse() as HttpWebResponse;
                                        if (headRep != null)
                                        {
                                            if (headRep.ContentLength > 0)
                                                filelength = headRep.ContentLength;
                                            headRep.Close();
                                        }
                                    }*/
                                    using (var theRepStream = theRep.GetResponseStream())
                                    {
                                        int count = theRepStream.Read(buffer, 0, buffer.Length);
                                        while (count > 0)
                                        {
                                            local.Write(buffer, 0, count);
                                            //byteprocessed += count;
                                            count = theRepStream.Read(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }
                            catch (WebException webEx)
                            {
                                if (webEx.Response != null)
                                {
                                    HttpWebResponse rep = webEx.Response as HttpWebResponse;
                                    if (rep.StatusCode != HttpStatusCode.NotFound)
                                        failedfiles.Add(_keypair.Key);
                                }
                            }
                        }
                        else
                            failedfiles.Add(_keypair.Key);
                    }
                    //fileList[filecount].IndexOfAny(' ');
                    if (downloadprogressReport != null)
                        WebClientPool.SynchronizationContext.Send(new System.Threading.SendOrPostCallback(delegate { continueDownload = downloadprogressReport.Invoke(filecount, fileList.Count); }), null);
                    filecount++;
                }
            }
            catch (Exception ex)
            { Myex = ex; }
            _webClient.CacheStorage = asdasdads;
            var myevent = new RunWorkerCompletedEventArgs(failedfiles, Myex, !continueDownload);
            if (downloadFinished_CallBack != null)
                WebClientPool.SynchronizationContext.Post(new System.Threading.SendOrPostCallback(delegate { downloadFinished_CallBack.Invoke(_webClient, myevent); }), null);

            if (myevent.Error != null && !myevent.Cancelled)
                if (failedfiles.Count == 0)
                    return true;
            return false;
        }
    }
}
