using System;
using System.IO;
using System.Net;

namespace Leayal.Net
{
    public class CacheResponse : WebResponse
    {
        internal static CacheResponse From(CacheInfo info, WebResponse rep)
        {
            return new CacheResponse(info, rep.Headers);
        }

        internal static CacheResponse From(CacheStorage storage, WebResponse rep)
        {
            return From(storage.GetCacheFromURL(rep.ResponseUri), rep);
        }

        private CacheInfo innerInfo;
        private WebHeaderCollection _header;
        internal CacheResponse(CacheInfo info, WebHeaderCollection head) : base()
        {
            this.innerInfo = info;
            this._header = head;
            if (this._header == null)
                this._header = new WebHeaderCollection();
            this._header.Remove(HttpResponseHeader.ContentType);
            this._header.Remove(HttpResponseHeader.ContentEncoding);
            this._header.Set(HttpResponseHeader.ContentLength, info.FileSize.ToString());
        }

        public override Stream GetResponseStream()
        {
            return this.innerInfo.OpenRead();
        }

        public DateTime LastModified => innerInfo.LastModifiedDate;

        public override long ContentLength => this.innerInfo.FileSize;
        public override string ContentType => System.Net.Mime.MediaTypeNames.Application.Octet;

        public override void Close()
        {
            base.Close();
        }

        public override WebHeaderCollection Headers { get { return this._header; } }

        public override bool IsFromCache => true;
        public override bool IsMutuallyAuthenticated => false;
        public override Uri ResponseUri => null;
    }
}
