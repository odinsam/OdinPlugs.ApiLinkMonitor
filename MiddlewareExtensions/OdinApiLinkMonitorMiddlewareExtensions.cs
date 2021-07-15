using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware.MiddlewareExtensions
{
    /// <summary>
    /// 扩展中间件
    /// </summary>
    public static class OdinApiLinkMonitorMiddlewareExtensions
    {
        public static IApplicationBuilder UseOdinApiLinkMonitor(this IApplicationBuilder app, Action<List<string>> options = null)
        {
            var lstStr = new List<string>();
            if (options != null)
                options(lstStr);
            return app.UseMiddleware<OdinApiLinkMonitorMiddleware>(lstStr);
        }
    }
}