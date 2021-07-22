using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using OdinPlugs.ApiLinkMonitor.OdinMiddleware;

namespace OdinPlugs.ApiLinkMonitor.MiddlewareExtensions
{
    public static class OdinExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseOdinException(this IApplicationBuilder app, Action<List<string>> options = null)
        {
            var lstStr = new List<string>();
            if (options != null)
                options(lstStr);
            return app.UseMiddleware<OdinExceptionMiddleware>(lstStr);
        }
    }
}