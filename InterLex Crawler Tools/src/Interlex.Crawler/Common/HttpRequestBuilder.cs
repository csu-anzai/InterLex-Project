namespace Interlex.Crawler.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents class with capabilities to execute http requests
    /// </summary>
    public class HttpRequestBuilder
    {
        private String url;
        private String host;
        private String refer;
        private String postData;
        private String method;
        private String cookieValue;
        private String contentType;
        private WebProxy proxy;
        private String charset;
        private String accept;
        private TimeSpan timeout = TimeSpan.FromSeconds(60);
        private readonly NameValueCollection headers = new NameValueCollection();
        private CancellationToken cancellationToken = CancellationToken.None;
        private CookieCollection cookies = new CookieCollection();
        private bool useGzip;
        private bool allowRedirect = true;

        /// <summary>
        /// Executes the request and returns the string representation of the respose uisng the charset of the content type header (if no charset provided uf8 is used)
        /// </summary>
        /// <returns></returns>
        public String Execute()
        {
            return this.ExecuteRich().ResponseContent;
        }

        /// <summary>
        /// Executes the request and returns wrapped object for the http response with headers, content ... etc
        /// </summary>
        /// <returns></returns>
        public ResponseRichResult ExecuteRich()
        {
            using (var response = this.ExecuteWebRespose())
            {
                using (var stream = response.GetResponseStream())
                {
                    var buffer = new byte[16 * 1024];
                    using (var ms = new MemoryStream())
                    {
                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }

                        var headers = (response.Headers?.AllKeys ?? new String[0])
                            .Select(x => new { key = x, value = response.Headers[x] })
                            .ToDictionary(x => x.key, x => x.value);

                        var cookies = response.Cookies.OfType<Cookie>().ToList();

                        return ResponseRichResult.Create(statusCode: response.StatusCode, bytes: ms.ToArray(), headers: headers, cookies: cookies, charset: response.CharacterSet);
                    }
                }
            }
        }

        /// <summary>
        /// Executes asynchronous request and returns wrapped object for the http response with headers, content ... etc
        /// </summary>
        /// <returns></returns>
        public Task<ResponseRichResult> ExecuteRichAsync()
        {
            var task = this.ExecuteWebResposeAsync()
                .ContinueWith(t =>
                {
                    using (var response = t.Result)
                    using (var stream = response.GetResponseStream())
                    using (var ms = new MemoryStream())
                    {
                        var buffer = new byte[16 * 1024];

                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }

                        var headers = (response.Headers?.AllKeys ?? new String[0])
                            .Select(x => new { key = x, value = response.Headers[x] })
                            .ToDictionary(x => x.key, x => x.value);

                        var cookies = response.Cookies.OfType<Cookie>().ToList();

                        return ResponseRichResult.Create(statusCode: response.StatusCode, bytes: ms.ToArray(), headers: headers, cookies: cookies, charset: response.CharacterSet);
                    }
                });

            return task;
        }

        /// <summary>
        /// Executes the request and returns the dotnet response object
        /// </summary>
        /// <returns></returns>
        public HttpWebResponse ExecuteWebRespose()
        {
            var request = this.CreateRequestInternal();

            var response = (HttpWebResponse)request.GetResponse();

            return response;
        }

        /// <summary>
        /// Executes asynchronous request and returns the dotnet response object
        /// </summary>
        /// <returns></returns>
        public Task<HttpWebResponse> ExecuteWebResposeAsync()
        {
            var request = this.CreateRequestInternal();

            var task = Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)
                .ContinueWith(t => (HttpWebResponse)t.Result, TaskContinuationOptions.OnlyOnRanToCompletion);

            return task;
        }

        /// <summary>
        /// Sets the url of the request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithUrl(String url)
        {
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) == false)
            {
                url = $"http://{url}";
            }

            this.url = url;
            this.host = new Uri(url).Host;

            return this;
        }

        /// <summary>
        /// Sets the host of the url. Usually this value will be set when using <see cref="WithUrl(string)"/> method
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithHost(String host)
        {
            this.host = host;
            return this;
        }

        /// <summary>
        /// Sets the refer of the request
        /// </summary>
        /// <param name="refer"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithRefer(String refer)
        {
            this.refer = refer;
            return this;
        }

        /// <summary>
        /// Sets the post data of the ruquest. Used when the method is POST.
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithPostData(String postData)
        {
            this.Post();

            this.postData = postData;

            return this;
        }

        /// <summary>
        /// Sets the method of the request. Example: post, get, put, patch, head .. etc
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithMethod(String method)
        {
            this.method = method;
            return this;
        }

        /// <summary>
        /// Sets cookie for the request
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithCookie(String cookie)
        {
            this.cookieValue = cookie;
            return this;
        }


        /// <summary>
        /// Adds cookie for the request
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithCookie(Cookie cookie)
        {
            this.cookies.Add(cookie);

            return this;
        }

        /// <summary>
        /// Adds cookies for the request
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithCookie(IEnumerable<Cookie> cookies)
        {
            foreach (var cookie in cookies)
            {
                this.cookies.Add(cookie);
            }

            return this;
        }

        /// <summary>
        /// Sets the content type of the request
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithContentType(String type)
        {
            this.contentType = type;

            return this;
        }

        /// <summary>
        /// The request will notify the server that a gzip response is requested if the server support it
        /// </summary>
        /// <returns></returns>
        public HttpRequestBuilder UseGZip()
        {
            this.useGzip = true;

            return this;
        }

        /// <summary>
        /// Sets the method of the request to post
        /// </summary>
        /// <returns></returns>
        public HttpRequestBuilder Post()
        {
            this.method = "POST";
            return this;
        }

        /// <summary>
        /// Sets the method of the request to get
        /// </summary>
        /// <returns></returns>
        public HttpRequestBuilder Get()
        {
            this.method = "GET";
            return this;
        }

        /// <summary>
        /// Sets the method of the request to head
        /// </summary>
        /// <returns></returns>
        public HttpRequestBuilder Head()
        {
            this.method = "HEAD";
            return this;
        }

        /// <summary>
        /// Sets the content type to application/json
        /// </summary>
        /// <returns></returns>
        public HttpRequestBuilder WithApplicationJsonType()
        {
            this.contentType = "application/json";
            return this;
        }

        /// <summary>
        /// Sets proxy for the request
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithProxy(String proxy)
        {
            this.proxy = new WebProxy(proxy);
            return this;
        }

        /// <summary>
        /// Sets proxy for the request
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithProxy(WebProxy proxy)
        {
            this.proxy = proxy;
            return this;
        }

        /// <summary>
        /// Sets specific header for the request
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">header value</param>
        /// <returns></returns>
        public HttpRequestBuilder WithHeader(String name, String value)
        {
            this.headers.Add(name, value);

            return this;
        }

        /// <summary>
        /// Sets after what time a timeout exception will be rised
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;

            return this;
        }

        /// <summary>
        /// Sets the charset of the content type
        /// </summary>
        /// <param name="charset"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithCharset(String charset)
        {
            this.charset = charset;

            return this;
        }


        /// <summary>
        /// Sets cancellation token for the request
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithCancellationToken(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            return this;
        }


        /// <summary>
        /// Sets the accept header for the request
        /// </summary>
        /// <param name="accept"></param>
        /// <returns></returns>
        public HttpRequestBuilder WithAccept(String accept)
        {
            this.accept = accept;

            return this;
        }

        /// <summary>
        /// Enable / disable auto redirect for direct responses
        /// </summary>
        /// <param name="allowRedirect">True to enable</param>
        /// <returns></returns>
        public HttpRequestBuilder WithAllowRedirect(bool allowRedirect)
        {
            this.allowRedirect = allowRedirect;

            return this;
        }

        private void ResetState()
        {
            this.url = null;
            this.host = null;
            this.refer = null;
            this.method = null;
            this.cookieValue = null;
            this.contentType = null;
            this.proxy = null;
            this.postData = null;
            this.charset = null;
            this.headers.Clear();
            this.timeout = TimeSpan.FromSeconds(60);
            this.cancellationToken = CancellationToken.None;
            this.useGzip = false;
            this.accept = null;
            this.allowRedirect = true;
            this.cookies = new CookieCollection();
        }


        private HttpWebRequest CreateRequestInternal()
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(this.url);
            if (this.cancellationToken != CancellationToken.None)
            {
                // abort the request on cancellation requested
                this.cancellationToken.Register(state => (state as HttpWebRequest).Abort(), request);
            }

            request.Timeout = (int)this.timeout.TotalMilliseconds;

            if (!String.IsNullOrEmpty(this.cookieValue))
            {
                request.Headers["Cookie"] = this.cookieValue;
            }
            else //  use of the two approaches to configure cookies
            {
                request.CookieContainer = new CookieContainer();
            }

            foreach (var cookie in this.cookies.OfType<Cookie>())
            {
                request.CookieContainer.Add(cookie);
            }

            request.Date = DateTime.Now;

            request.Method = this.method ?? "GET";
            request.Accept = this.accept ?? "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8,application/json";
            request.ContentType = $"{this.contentType}";
            request.Host = this.host;
            request.Referer = this.refer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.154 Safari/537.36";
            request.AllowAutoRedirect = this.allowRedirect;

            if (!String.IsNullOrEmpty(this.contentType) && !String.IsNullOrEmpty(this.charset))
            {
                request.ContentType += $" ;charset={this.charset}";
            }

            if (this.useGzip)
            {
                request.AutomaticDecompression = DecompressionMethods.GZip;
            }

            request.Headers.Add(this.headers);

            if (this.proxy != null)
            {
                request.Proxy = this.proxy;
            }
            else
            {
                request.Proxy = null;
            }

            if (request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    if (!String.IsNullOrEmpty(this.postData))
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(this.postData);
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();
                    }
                }
            }

            this.ResetState();

            return request;
        }
    }
}
