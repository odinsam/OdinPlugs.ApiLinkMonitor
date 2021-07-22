using System.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OdinPlugs.OdinInject;
using OdinPlugs.ApiLinkMonitor.OdinLinkMonitor.OdinLinkMonitorInterface;
using OdinPlugs.SnowFlake.SnowFlakePlugs.ISnowFlake;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;
using System.Collections.Generic;
using OdinPlugs.ApiLinkMonitor.Models.ApiLinkModels;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinObject;

namespace OdinPlugs.OdinMvcCore.OdinMiddleware.Utils
{
    public class OdinAopMiddlewareHelper
    {
        private static Stopwatch stopWatch;

        /// <summary>
        /// 中间件请求前，尚未进入action方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="apiInvokerModel"></param> 
        public static void MiddlewareBefore(HttpContext context)
        {
#if DEBUG
            System.Console.WriteLine($"=============MiddlewareBefore  OnActionExecuting  start=============");
#endif
            stopWatch = Stopwatch.StartNew();
            stopWatch.Restart();
            var odinLinkMonitor = OdinInjectCore.GetService<IOdinApiLinkMonitor>();
            var snowFlakeId = OdinInjectCore.GetService<IOdinSnowFlake>().CreateSnowFlakeId();
            // 创建链路，并生成起始节点
            var linkMonitor = odinLinkMonitor.CreateOdinLinkMonitor(snowFlakeId);
            context.Items.Add("odinlinkId", snowFlakeId);
            context.Items.Add("odinlink", linkMonitor);
            System.Console.WriteLine(JsonConvert.SerializeObject(linkMonitor[snowFlakeId].Peek()).ToJsonFormatString());

            #region 保存link记录到mongodb--链路Start

            #endregion
#if DEBUG
            System.Console.WriteLine($"=============MiddlewareBefore  OnActionExecuting  end=============");
#endif
        }

        /// <summary>
        /// 中间件请求成功后  action方法已经执行完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="apiInvokerModel"></param>
        /// <param name="originalBodyStream"></param>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        public static async void MiddlewareAfterAsync(HttpContext context)
        {
#if DEBUG
            System.Console.WriteLine($"=============MiddlewareAfterAsync  OnActionExecuted  Start=============");
#endif
            var odinLinkMonitor = OdinInjectCore.GetService<IOdinApiLinkMonitor>();
            stopWatch.Stop();
            var elapseTime = stopWatch.ElapsedMilliseconds;
            Dictionary<long, Stack<OdinApiLinkModel>> linkMonitor = null;
            // 生成结束链路
            linkMonitor = odinLinkMonitor.ApiInvokerOverLinkMonitor(context, elapseTime);
            var linkMonitorId = Convert.ToInt64(context.Items["odinlinkId"]);
#if DEBUG
            System.Console.WriteLine(JsonConvert.SerializeObject(linkMonitor[linkMonitorId].Peek()).ToJsonFormatString());
            System.Console.WriteLine($"=============MiddlewareAfterAsync  OnActionExecuted  end=============");
#endif
            await Task.CompletedTask;
        }

        public static void MiddlewareResponseCompleted()
        {
#if DEBUG
            System.Console.WriteLine($"=============MiddlewareResponseCompleted    start=============");
#endif
#if DEBUG
            System.Console.WriteLine($"=============MiddlewareResponseCompleted    end=============");
#endif
        }

        public static async Task MiddlewareExceptionAsync(HttpContext context, Exception ex)
        {
#if DEBUG
            System.Console.WriteLine($"=============MiddlewareExceptionAsync    start=============");
#endif
            System.Console.WriteLine(JsonConvert.SerializeObject(ex).ToJson());
#if DEBUG
            System.Console.WriteLine($"=============MiddlewareExceptionAsync    end=============");
#endif
            await Task.CompletedTask;
        }
    }
}