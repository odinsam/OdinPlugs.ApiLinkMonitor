using System.Runtime.ExceptionServices;
using System.Buffers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OdinPlugs.ApiLinkMonitor.Utils;
using OdinPlugs.OdinInject;
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
                    System.Console.WriteLine("=========OdinExceptionMiddleware response start==========");
                    System.Console.WriteLine("=========OdinExceptionMiddleware response end==========");
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

        private async Task HandlerException(HttpContext context, Exception ex)
        {
            System.Console.WriteLine("=========HandlerException start==========");
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
            System.Console.WriteLine("=========HandlerException end==========");
        }
    }
}