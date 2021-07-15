using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OdinPlugs.ApiLinkMonitor.Utils;
using OdinPlugs.OdinMvcCore.OdinMiddleware.Utils;
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
                    System.Console.WriteLine("=========OdinExceptionMiddleware request start==========");
                    System.Console.WriteLine("=========OdinExceptionMiddleware request end==========");
                    await _next(context);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine("=========OdinExceptionMiddleware catch Exception start==========");
                    if (context.Response.HasStarted)
                    {
                        System.Console.WriteLine("触发了异常, 但是Response HasStarted!");
                        throw;
                    }
                    await HandlerException(context, ex);
                    System.Console.WriteLine("=========OdinExceptionMiddleware catch Exception end==========");
                }
                finally
                {
                    System.Console.WriteLine("=========OdinExceptionMiddleware finally Exception start==========");

                    System.Console.WriteLine("=========OdinExceptionMiddleware finally Exception end==========");
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task HandlerException(HttpContext httpContext, Exception ex)
        {
            var errorResponse = new { Data = JsonConvert.SerializeObject(ex), Message = "OdinExceptionMiddleware HandlerException" };
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(errorResponse.ToJson());
        }
    }
}