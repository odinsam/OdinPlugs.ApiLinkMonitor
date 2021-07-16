using System;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Newtonsoft.Json;
using OdinPlugs.ApiLinkMonitor.OdinLinkMonitor.OdinLinkMonitorInterface;
using OdinPlugs.OdinInject;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;

namespace OdinPlugs.ApiLinkMonitor.OdinAspectCore.IOdinAspectCoreInterface
{
    public class OdinAspectCoreInterceptorAttribute : AbstractInterceptorAttribute, IOdinAspectCoreInterceptorAttribute
    {
        Stopwatch stopWatch;
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            stopWatch = Stopwatch.StartNew();
            stopWatch.Restart();
            bool isSuccess = true;
            try
            {
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  request  start=============");
                var odinLinkMonitor = OdinInjectCore.GetService<IOdinApiLinkMonitor>();
                var linkMonitorId = Convert.ToInt64(context.GetHttpContext().Items["odinlinkId"]);
                var linkMonitor = odinLinkMonitor.ApiInvokerLinkMonitor(context);
                System.Console.WriteLine(JsonConvert.SerializeObject(linkMonitor[linkMonitorId].Peek()).ToJsonFormatString());
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  request  end=============");



                await next(context);



                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  response  start=============");
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  response  end=============");

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("执行出错");
                isSuccess = false;
                throw ex;
            }
            finally
            {

                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  return  start=============");
                // stopWatch.Stop();
                var odinLinkMonitor = OdinInjectCore.GetService<IOdinApiLinkMonitor>();
                var linkMonitorId = Convert.ToInt64(context.GetHttpContext().Items["odinlinkId"]);
                Console.WriteLine($"isSuccess:{isSuccess}");
                var linkMonitor = odinLinkMonitor.ApiInvokerToEndLinkMonitor(context, isSuccess, stopWatch);
                System.Console.WriteLine(JsonConvert.SerializeObject(linkMonitor[linkMonitorId].Peek()).ToJsonFormatString());
                System.Console.WriteLine("拦截后执行");
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  return  end=============");
            }
        }
    }
}