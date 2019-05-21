namespace Interlex.Crawler.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Represents class for http response
    /// </summary>
    public class ResponseRichResult
    {
        private readonly Lazy<String> responseContentLazy;
        private readonly Dictionary<String, String> headers;

        internal static ResponseRichResult Create(Byte[] bytes, HttpStatusCode statusCode, Dictionary<String, String> headers, IEnumerable<Cookie> cookies, String charset)
        {
            return new ResponseRichResult(bytes, statusCode, headers, cookies, charset);
        }

        /// <summary>
        /// Raw bytes of the response
        /// </summary>
        public Byte[] ResponseBytes { get; private set; }

        /// <summary>
        /// Response content represent as string using the encoding of the response
        /// </summary>
        public String ResponseContent => this.responseContentLazy.Value;

        /// <summary>
        /// Http status code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        private ResponseRichResult(
            Byte[] responseByte,
            HttpStatusCode statusCode,
            Dictionary<String, String> headers,
            IEnumerable<Cookie> cookies,
            String charset)
        {
            this.ResponseBytes = responseByte;
            this.StatusCode = statusCode;
            this.Charset = charset;
            this.Encoding = GetEncoding(this.Charset);

            this.responseContentLazy = new Lazy<string>(() => this.Encoding.GetString(this.ResponseBytes), true);

            this.headers = headers.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase); // copy dictionary
            this.Cookies = new CookieCollection();

            foreach (var cookie in cookies)
            {
                this.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Cookies
        /// </summary>
        public CookieCollection Cookies { get; }

        /// <summary>
        /// Charset
        /// </summary>
        public String Charset { get; }

        /// <summary>
        /// Encoding. Default is utf8
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Returns the value of a header. Null is returned if no header with the specified valu
        /// </summary>
        /// <param name="headerName">header value</param>
        /// <returns></returns>
        public String GetHeaderValue(String headerName)
        {
            if (this.headers.ContainsKey(headerName))
            {
                return this.headers[headerName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// True if the header exists
        /// </summary>
        /// <param name="headerName">header name</param>
        /// <returns></returns>
        public bool HasHeader(String headerName) => this.headers.ContainsKey(headerName);

        private static Encoding GetEncoding(String charset)
        {
            var encoding = Encoding.UTF8;
            if (String.IsNullOrEmpty(charset) == false)
            {
                try
                {
                    encoding = Encoding.GetEncoding(charset);
                }
                catch { }
            }

            return encoding;
        }
    }
}
