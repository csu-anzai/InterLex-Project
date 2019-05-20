namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using HtmlAgilityPack;
    using Interlex.Crawler.Common;

    /// <summary>
    /// Represents http response
    /// </summary>
    [DebuggerDisplay("{DebugInfo()}")]
    public class HttpModel
    {
        private Lazy<String> strLazy;
        private Lazy<HtmlDocument> htmlLazy;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="url"></param>
        /// <param name="raw"></param>
        /// <param name="encoding"></param>
        /// <param name="mimeType"></param>
        /// <param name="contentDisposition"></param>
        /// <param name="cookies"></param>
        /// <param name="statusCode"></param>
        public HttpModel(String url, Byte[] raw, Encoding encoding, String mimeType, String contentDisposition, CookieCollection cookies, HttpStatusCode statusCode)
        {
            this.Url = url;
            this.Raw = raw;
            this.Encoding = encoding;
            this.MimeType = mimeType;
            this.ContentDisposition = contentDisposition;
            this.Cookies = cookies;
            this.StatusCode = statusCode;
            this.IsHtml = this.MimeType.IsIn("text/html", "text/htm");

            this.strLazy = new Lazy<string>(() => this.Raw == null ? null : this.Encoding.GetString(this.Raw), true);
            this.htmlLazy = new Lazy<HtmlDocument>(() =>
            {
                var html = new HtmlDocument { OptionWriteEmptyNodes = true, OptionEmptyCollection = true };
                if(String.IsNullOrEmpty(this.strLazy.Value) == false)
                {
                    html.LoadHtml(this.strLazy.Value);
                }

                return html;
            });
        }


        /// <summary>
        /// Url of the response
        /// </summary>
        public String Url { get; }

        /// <summary>
        /// Content of the response
        /// </summary>
        public Byte[] Raw { get; }

        /// <summary>
        /// Mime type (content-type) of the response
        /// </summary>
        public String MimeType { get; }

        /// <summary>
        /// Content disposition of the response
        /// </summary>
        public String ContentDisposition { get; }

        /// <summary>
        /// Encoding of the response
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Cookies of the response
        /// </summary>
        public CookieCollection Cookies { get; }

        /// <summary>
        /// Status code of the response
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Indicates if the response is Html (MimeType = text/html)
        /// </summary>
        public bool IsHtml { get; }


        /// <summary>
        /// Returns the reponse as string
        /// </summary>
        /// <returns></returns>
        public String GetAsString() => this.strLazy.Value;

        /// <summary>
        /// Returns the response as html document
        /// </summary>
        /// <returns></returns>
        public HtmlDocument GetAsHtml() => this.htmlLazy.Value;

#if DEBUG
        public String DebugInfo()
        {
            return $"{this.Url}";
        }
#endif
    }
}
