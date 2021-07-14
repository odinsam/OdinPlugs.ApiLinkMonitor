using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OdinPlugs.ApiLinkMonitor.Utils;
using OdinPlugs.OdinMvcCore.OdinMiddleware.Utils;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware
{
    public class OdinApiLinkMonitorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _lstStr;
        /// <summary>
        /// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托，如果有其它参数，也同样通过注入的方式获得
        /// </summary>
        /// <param name="next"></param>
        public OdinApiLinkMonitorMiddleware(RequestDelegate next, List<string> lstStr = null)
        {
            //通过注入方式获得对象
            _next = next;
            _lstStr = lstStr;
        }
        /// <summary>
        /// 自定义中间件要执行的逻辑
        /// </summary>w
        /// <param name="context"></param> 
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (!OdinAopMiddlewarePathRegex.IsIgnorePath(context.Request.Path, _lstStr))
            {
                Exception exception = null;
                try
                {
                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Request  start==========");
                    OdinAopMiddlewareHelper.MiddlewareBefore(context);
                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Request  end==========");
                    await _next(context);
                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Response  start==========");
                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Response  end==========");
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"=========OdinApiLinkMonitorMiddleware  catch  start==========");
                    exception = ex;
                }
                finally
                {
                    System.Console.WriteLine($"=========OdinApiLinkMonitorMiddleware  finally  start==========");
                    OdinAopMiddlewareHelper.MiddlewareAfterAsync(context, exception);
                    System.Console.WriteLine($"=========OdinApiLinkMonitorMiddleware  finally  end==========");
                }
                // 响应完成记录时间和存入日志
                context.Response.OnCompleted(() =>
                {

                    System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware  Response  OnCompleted==========");
                    OdinAopMiddlewareHelper.MiddlewareResponseCompleted();
                    return Task.CompletedTask;
                });
            }
            else
            {
                await _next(context);
            }
        }
    }
}