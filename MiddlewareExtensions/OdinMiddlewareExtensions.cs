using Microsoft.AspNetCore.Builder;
using OdinPlugs.OdinMiddleware;

namespace OdinPlugs.OdinMvcCore.OdinMiddleware.MiddlewareExtensions
{
    /// <summary>
    /// 扩展中间件
    /// </summary>
    public static class OdinAopMiddlewareExtensions
    {
        public static IApplicationBuilder UseOdinAop(this IApplicationBuilder app)
        {
            return app.UseMiddleware<OdinAopMiddleware>();
        }

        public static IApplicationBuilder UseOdinException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<OdinExceptionMiddleware>();
        }
    }
}