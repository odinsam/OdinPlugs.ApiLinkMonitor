using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OdinPlugs.ApiLinkMonitor.Utils;
using OdinPlugs.OdinMvcCore.OdinMiddleware.Utils;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware
{
    public class OdinApiLinkMonitorMiddleware : OdinMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托，如果有其它参数，也同样通过注入的方式获得
        /// </summary>
        /// <param name="next"></param>
        public OdinApiLinkMonitorMiddleware(RequestDelegate next, List<string> lstStr = null) : base(lstStr)
        {
            //通过注入方式获得对象
            _next = next;
        }
        /// <summary>
        /// 自定义中间件要执行的逻辑
        /// </summary>w
        /// <param name="context"></param> 
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (!OdinAopMiddlewarePathRegex.IsIgnorePath(context.Request.Path, LstStr))
            {
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
#if DEBUG
                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Request  start==========");
#endif
                    OdinAopMiddlewareHelper.MiddlewareBefore(context);
#if DEBUG
                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Request  end==========");
#endif
                    await _next(context);
#if DEBUG
                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Response  start==========");
#endif
                    OdinAopMiddlewareHelper.MiddlewareAfterAsync(context);
#if DEBUG
                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Response  end==========");
#endif
                }
                // context.Response.OnCompleted(() =>
                // {

                //     System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Response  OnCompleted==========");
                //     OdinAopMiddlewareHelper.MiddlewareResponseCompleted();
                //     return Task.CompletedTask;
                // });
            }
            else
            {
                await _next(context);
            }
        }
    }
}