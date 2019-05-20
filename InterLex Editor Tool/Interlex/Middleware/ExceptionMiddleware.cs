using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Interlex.Middleware
{
    public class ExceptionMiddleware
    {
        internal static readonly String REQUEST_METHOD = "RequestMethod";
        internal static readonly String REQUEST_ACCEPT = "RequestAccept";
        internal static readonly String REQUEST_CONTENT_TYPE = "RequestContentType";
        internal static readonly String REQUEST_QUERY_STRING = "RequestQueryString";
        internal static readonly String REQUEST_BODY = "RequestBody";
        internal static readonly String RESPONSE_CONTENT_TYPE = "ResposeContentType";
        internal static readonly String USERNAME = "Username";

        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.logger = loggerFactory.CreateLogger("Middleware");
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception e)
            {
                var username = context.User?.Identity?.Name ?? "__unknown";

                var logPropertyBodyText = await GetRequestBodyTextAsync(context);

                // register the properties used the serilog message template
                using (AddLogProperty(REQUEST_METHOD, context.Request.Method))
                using (AddLogProperty(REQUEST_ACCEPT, context.Request.Headers["Accept"].FirstOrDefault()))
                using (AddLogProperty(REQUEST_CONTENT_TYPE, context.Request.ContentType))
                using (AddLogProperty(REQUEST_QUERY_STRING, context.Request.QueryString.Value))
                using (AddLogProperty(REQUEST_BODY, logPropertyBodyText))
                using (AddLogProperty(RESPONSE_CONTENT_TYPE, context.Response.ContentType))
                using (AddLogProperty(USERNAME, username))
                {
                    this.logger.Log(
                         logLevel: LogLevel.Error,
                         eventId: 0,
                         state: new { },
                         exception: e,
                         formatter: (state, ex) => "");

                    // note that the message template (formatter) used for the log is from the Serilog config in the appsettings.json
                    // so we are not obligated to explicitly log the excpetion as part of the formatter mesasge or any additional information as the username .. etc
                }

                context.Response.StatusCode = 500;
            }
        }

        private static String GetLogValue(String value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            return value;
        }

        private static IDisposable AddLogProperty(String name, String value)
        {
            var unifiedValue = GetLogValue(value);

            return LogContext.PushProperty(name, unifiedValue);
        }

        private static async Task<String> GetRequestBodyTextAsync(HttpContext context)
        {
            var logText = (String)null;
            try
            {
                var fileContentTypes = new[] { "application/octet-stream", "application/x-binary", "binary/octet-stream" };

                var isPost = context.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase);
                var contentType = context.Request.ContentType ?? String.Empty;
                var isRequestBodyFile = fileContentTypes.Any(ct => contentType.IndexOf(ct, StringComparison.OrdinalIgnoreCase) >= 0);

                if (isPost && (isRequestBodyFile == false))
                {
                    logText = await RequestBodyToTextAsync(context.Request);
                }

            }
            catch { }

            return logText;
        }

        private static async Task<String> RequestBodyToTextAsync(HttpRequest request)
        {
            if (request.HasFormContentType)
            {
                return FormCollectionToText(request.Form);
            }
            else
            {
                request.EnableRewind();
                request.Body.Seek(0, SeekOrigin.Begin);
                // note: not disposing the stream reader because it will dispose and the underlying stream 
                // -> bad idea to dispose the request stream which is heavily used from various middlewares
                var streamReader = new StreamReader(request.Body);
                var content = await streamReader.ReadToEndAsync();

                return $"{Environment.NewLine}{content}{Environment.NewLine}";
            }
        }

        private static String FormCollectionToText(IFormCollection form)
        {
            var builder = new StringBuilder();
            builder.AppendLine();

            foreach (var key in form.Keys)
            {
                var formValue = form[key];
                if (formValue == StringValues.Empty || (formValue.Count == 1 && String.IsNullOrEmpty(formValue[0])))
                {
                    formValue = new StringValues("null");
                }

                builder.AppendLine($"----{key}: {formValue}");
            }

            foreach (var file in form.Files)
            {
                var fileName = GetLogValue(file.FileName) ?? "null";
                builder.AppendLine($"----file_{fileName}: {file.Length} bytes ");
            }

            return builder.ToString();
        }
    }
}
