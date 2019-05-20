namespace Interlex.Crawler.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using HeyRed.Mime;
    using Interlex.Crawler.Model;

    /// <summary>
    /// Represents http based requester
    /// </summary>
    public class Http
    {
        internal static readonly int DefaultRetryCount = 5;
        internal static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
        internal static readonly bool DefaultReUseCookies = false;
        internal static readonly bool DefaultAllowRedirect = false;

        private readonly bool useProxy;
        private readonly int retryCount;
        private readonly bool reUseCookies;
        private readonly bool allowRedirect;
        private readonly TimeSpan timeout;

        private IReadOnlyCollection<Cookie> cookies;

        public Http() : this(DefaultRetryCount, DefaultReUseCookies, DefaultAllowRedirect, DefaultTimeout)
        {

        }

        /// <summary>
        /// Creates new instace of <see cref="Http"/>
        /// </summary>
        /// <param name="retryCount">Retry count if request fails</param>
        /// <param name="reUseCookies">True to reuse cookies between requests</param>
        /// <param name="allowRedirect">True to allow redirects</param>
        /// <param name="timeout">Timeout for the request</param>
        public Http(int retryCount, bool reUseCookies, bool allowRedirect, TimeSpan timeout)
        {
            this.retryCount = retryCount;
            this.reUseCookies = reUseCookies;
            this.allowRedirect = allowRedirect;
            this.timeout = timeout;
        }

        /// <summary>
        /// Predicate to check if the response result was not found (404). 
        /// </summary>
        public Predicate<HttpModel> NotFoundPredicate { get; internal set; }

        /// <summary>
        /// Executes http get
        /// </summary>
        /// <param name="uri">Request url</param>
        /// <returns></returns>
        public Task<HttpModel> GetAsync(Uri uri) => this.GetAsync(uri.AbsoluteUri);

        /// <summary>
        /// Executes http get 
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns></returns>
        public Task<HttpModel> GetAsync(String url)
        {
            return this.GetAsync(url, null);
        }

        /// <summary>
        /// Executes http get
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="opt">Action for access to the underling http builder. Provides capabilities for additional configuration.</param>
        /// <returns></returns>
        public async Task<HttpModel> GetAsync(String url, Action<HttpRequestBuilder> opt)
        {
            Exception lastException = null;

            var maxRequests = this.retryCount + 1;  // + 1 indicates the initial request

            while (maxRequests > 0)
            {
                try
                {
                    var builder = new HttpRequestBuilder()
                        .Get()
                        .WithUrl(url)
                        .WithTimeout(this.timeout)
                        .WithAllowRedirect(this.allowRedirect)
                        .UseGZip();

                    if (this.reUseCookies && this.cookies != null)
                    {
                        builder.WithCookie(this.cookies);
                    }

                    var httpResult = await builder.ExecuteRichAsync();

                    if (this.reUseCookies)
                    {
                        this.cookies = httpResult.Cookies.OfType<Cookie>().ToList();
                    }

                    opt?.Invoke(builder);

                    var encoding = httpResult.Encoding;
                    var mimeType = GetMimeType(httpResult);
                    var contentDisposition = httpResult.GetHeaderValue("Content-Disposition");

                    var httpGet = new HttpModel(url, httpResult.ResponseBytes, encoding, mimeType, contentDisposition, httpResult.Cookies, httpResult.StatusCode);
                    if (this.NotFoundPredicate?.Invoke(httpGet) == true)
                    {
                        throw new WebException($"{url} not found");
                    }

                    return httpGet;
                }
                catch (Exception e)
                {
                    lastException = e;

                    maxRequests--;
                }
            }

            throw lastException;
        }


        /// <summary>
        /// Executes http post
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="json">Json to post</param>
        /// <returns></returns>
        public async Task<HttpModel> PostAsync(String url, String json)
        {
            return await this.PostAsync(url, json, "application/json");
        }

        /// <summary>
        /// Executes http post
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="postData">Data to post</param>
        /// <param name="contentType">Content type of <paramref name="postData"/></param>
        /// <returns></returns>
        public async Task<HttpModel> PostAsync(String url, String postData, String contentType)
        {
            Exception lastException = null;

            var maxRequests = this.retryCount + 1;  // + 1 indicates the initial request

            while (maxRequests > 0)
            {
                try
                {
                    var builder = new HttpRequestBuilder()
                        .Post()
                        .WithUrl(url)
                        .WithPostData(postData)
                        .WithTimeout(this.timeout)
                        .WithAllowRedirect(false)
                        .UseGZip();

                    if (this.reUseCookies && this.cookies != null)
                    {
                        builder.WithCookie(this.cookies);
                    }

                    var httpResult = await builder.ExecuteRichAsync();

                    if (this.reUseCookies)
                    {
                        this.cookies = httpResult.Cookies.OfType<Cookie>().ToList();
                    }

                    var encoding = httpResult.Encoding;
                    var mimeType = GetMimeType(httpResult);
                    var contentDisposition = httpResult.GetHeaderValue("Content-Disposition");

                    var httpGet = new HttpModel(url, httpResult.ResponseBytes, encoding, mimeType, contentDisposition, httpResult.Cookies, httpResult.StatusCode);
                    if (this.NotFoundPredicate?.Invoke(httpGet) == true)
                    {
                        throw new WebException($"{url} not found");
                    }

                    return httpGet;
                }
                catch (Exception e)
                {
                    lastException = e;

                    maxRequests--;
                }
            }

            throw lastException;
        }

        private static String GetMimeType(ResponseRichResult httpResult)
        {
            var format = httpResult.GetHeaderValue("Content-Type").Split(';')[0];
            if (httpResult.HasHeader("Content-Disposition"))
            {
                var contentDispositionStr = httpResult.GetHeaderValue("Content-Disposition");

                // for cases when dublicate header is send (example: https://uodo.gov.pl/pl/file/701) which is interpreted by the .net as comma separated header values (https://tools.ietf.org/html/rfc7230#section-3.2.2)
                // we ca try to get single value from the aggregated header values, otherwise content disposition is throwing exception during creation
                contentDispositionStr = contentDispositionStr.Split(',').Distinct().Single();
                var fileNameIndex = contentDispositionStr.IndexOf("filename=");
                if (fileNameIndex >= 0)
                {
                    var before = contentDispositionStr.Substring(0, fileNameIndex);
                    var fileNameContent = contentDispositionStr.Substring(fileNameIndex + "filename=".Length).Replace(" ", "_");
                    contentDispositionStr = $"{before}filename={fileNameContent}";
                }
                
                contentDispositionStr = contentDispositionStr.EncodeNonAsciiCharacters();
                var contentDisposition = new ContentDisposition(contentDispositionStr);
                var fileName = contentDisposition.FileName;

                format = MimeTypesMap.GetMimeType(fileName);
            }

            return format;
        }
    }
}
