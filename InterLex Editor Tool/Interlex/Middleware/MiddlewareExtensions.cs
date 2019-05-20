using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Interlex.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder appBuilder) => appBuilder.UseMiddleware<ExceptionMiddleware>();
        public static IApplicationBuilder UseNoCacheMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<NoCacheMiddleware>();
    }
}
