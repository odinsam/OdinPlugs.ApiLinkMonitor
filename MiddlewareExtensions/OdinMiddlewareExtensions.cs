using Microsoft.AspNetCore.Builder;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware.MiddlewareExtensions
{
    /// <summary>
    /// 扩展中间件
    /// </summary>
    public static class OdinAopMiddlewareExtensions
    {
        public static IApplicationBuilder UseOdinApiLinkMonitor(this IApplicationBuilder app)
        {
            return app.UseMiddleware<OdinApiLinkMonitorMiddleware>();
        }

        public static IApplicationBuilder UseOdinException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<OdinExceptionMiddleware>();
        }
    }
}