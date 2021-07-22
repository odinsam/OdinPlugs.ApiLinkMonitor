using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OdinPlugs.ApiLinkMonitor.Utils;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinObject;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware
{
    public class OdinExceptionMiddleware : OdinMiddleware
    {
        private readonly RequestDelegate _next;
        public OdinExceptionMiddleware(RequestDelegate next, List<string> lstStr = null) : base(lstStr)
        {
            //通过注入方式获得对象
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!OdinAopMiddlewarePathRegex.IsIgnorePath(context.Request.Path, LstStr))
            {
                try
                {
#if DEBUG
                    System.Console.WriteLine("=========OdinExceptionMiddleware request start==========");
#endif
#if DEBUG
                    System.Console.WriteLine("=========OdinExceptionMiddleware request end==========");
#endif
                    await _next(context);
#if DEBUG
                    System.Console.WriteLine("=========OdinExceptionMiddleware response start==========");
#endif
#if DEBUG
                    System.Console.WriteLine("=========OdinExceptionMiddleware response end==========");
#endif
                }
                catch (System.Exception ex)
                {
#if DEBUG
                    System.Console.WriteLine("=========OdinExceptionMiddleware catch Exception start==========");
#endif
                    if (context.Response.HasStarted)
                    {
                        System.Console.WriteLine("触发了异常, 但是Response HasStarted!");
                        throw;
                    }
                    await HandlerException(context, ex);
#if DEBUG
                    System.Console.WriteLine("=========OdinExceptionMiddleware catch Exception end==========");
#endif
                }
                finally
                {
#if DEBUG
                    System.Console.WriteLine("=========OdinExceptionMiddleware finally Exception start==========");
#endif
#if DEBUG
                    System.Console.WriteLine("=========OdinExceptionMiddleware finally Exception end==========");
#endif
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task HandlerException(HttpContext context, Exception ex)
        {
#if DEBUG
            System.Console.WriteLine("=========HandlerException start==========");
#endif
            var linkMonitorId = Convert.ToInt64(context.Items["odinlinkId"]);
            var exceptionResult = new
            {
                SnowFlakeId = linkMonitorId.ToString(),
                Data = ex,
                StatusCode = "sys-error",
                ErrorMessage = "系统异常，请联系管理员",
                Message = "系统异常:[sys-error]"
            };
            // var errorResponse = new { Data = JsonConvert.SerializeObject(ex), Message = "OdinExceptionMiddleware HandlerException" };
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(exceptionResult.ToJson());
#if DEBUG
            System.Console.WriteLine("=========HandlerException end==========");
#endif
        }
    }
}