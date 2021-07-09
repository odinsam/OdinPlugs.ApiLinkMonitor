using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OdinPlugs.OdinMvcCore;
using OdinPlugs.OdinMvcCore.OdinMiddleware.Utils;

namespace OdinPlugs.OdinMiddleware
{
    public class OdinAopMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托，如果有其它参数，也同样通过注入的方式获得
        /// </summary>
        /// <param name="next"></param>
        public OdinAopMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            //通过注入方式获得对象
            _next = next;
        }
        /// <summary>
        /// 自定义中间件要执行的逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            System.Console.WriteLine("=========OdinAopMiddleware Request  start==========");

            System.Console.WriteLine("=========OdinAopMiddleware Request  end==========");


            await _next(context);


            System.Console.WriteLine($"=========OdinAopMiddleware Response    start==========");
            System.Console.WriteLine($"=========OdinAopMiddleware Response    end==========");
            // 响应完成记录时间和存入日志
            context.Response.OnCompleted(() =>
            {
                System.Console.WriteLine("=========OdinAopMiddleware Response OnCompleted==========");
                return Task.CompletedTask;
            });
        }
    }
}