using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OdinPlugs.OdinMvcCore.OdinMiddleware.Utils;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware
{
    public class OdinApiLinkMonitorMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托，如果有其它参数，也同样通过注入的方式获得
        /// </summary>
        /// <param name="next"></param>
        public OdinApiLinkMonitorMiddleware(RequestDelegate next, IWebHostEnvironment environment)
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
            Exception exception = null;
            try
            {
                // System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware Request  start==========");
                // context.Request.EnableBuffering();
                // var requestReader = new StreamReader(context.Request.Body);
                // var requestContent = requestReader.ReadToEnd();
                // Console.WriteLine($"Request Body: {requestContent}");
                // context.Request.Body.Position = 0;
                // using (var ms = new MemoryStream())
                // {
                //     context.Response.Body = ms;
                //     OdinAopMiddlewareHelper.MiddlewareBefore(context);
                //     System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware Request  end==========");
                //     await _next(context);
                //     ms.Position = 0;
                //     var responseReader = new StreamReader(ms);
                //     var responseContent = responseReader.ReadToEnd();
                //     Console.WriteLine($"Response Body: {responseContent}");
                //     ms.Position = 0;
                //     context.Request.Body.Position = 0;
                // }
                System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware Request  start==========");
                OdinAopMiddlewareHelper.MiddlewareBefore(context);
                System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware Request  end==========");
                await _next(context);
                System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware Response  start==========");
                OdinAopMiddlewareHelper.MiddlewareBefore(context);
                System.Console.WriteLine("=========OdinApiLinkMonitorMiddleware Response  end==========");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"=========OdinApiLinkMonitorMiddleware exception    start==========");
                exception = ex;
            }
            finally
            {
                System.Console.WriteLine($"=========OdinAopMiddleware Response    start==========");
                OdinAopMiddlewareHelper.MiddlewareAfterAsync(context, exception);
                System.Console.WriteLine($"=========OdinAopMiddleware Response    end==========");
            }
            // 响应完成记录时间和存入日志
            context.Response.OnCompleted(() =>
            {
                System.Console.WriteLine("=========OdinAopMiddleware Response OnCompleted==========");
                OdinAopMiddlewareHelper.MiddlewareResponseCompleted();
                return Task.CompletedTask;
            });
        }
    }
}