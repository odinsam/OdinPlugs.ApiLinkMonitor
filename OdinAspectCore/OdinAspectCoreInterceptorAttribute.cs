using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Newtonsoft.Json;
using OdinPlugs.ApiLinkMonitor.OdinLinkMonitor.OdinLinkMonitorInterface;
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
#if DEBUG
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  request  start=============");
#endif
                var odinLinkMonitor = OdinInjectCore.GetService<IOdinApiLinkMonitor>();
                var linkMonitorId = Convert.ToInt64(context.GetHttpContext().Items["odinlinkId"]);
                var linkMonitor = odinLinkMonitor.ApiInvokerLinkMonitor(context);
#if DEBUG
                System.Console.WriteLine(JsonConvert.SerializeObject(linkMonitor[linkMonitorId].Peek()).ToJsonFormatString());
#endif
#if DEBUG
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  request  end=============");
#endif



                await next(context);



#if DEBUG
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  response  start=============");
#endif
#if DEBUG
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  response  end=============");
#endif

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("执行出错");
                isSuccess = false;
                throw ex;
            }
            finally
            {
#if DEBUG
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  return  start=============");
#endif
                // stopWatch.Stop();
                var odinLinkMonitor = OdinInjectCore.GetService<IOdinApiLinkMonitor>();
                var linkMonitorId = Convert.ToInt64(context.GetHttpContext().Items["odinlinkId"]);
                Console.WriteLine($"isSuccess:{isSuccess}");
                var linkMonitor = odinLinkMonitor.ApiInvokerToEndLinkMonitor(context, isSuccess, stopWatch);
                System.Console.WriteLine(JsonConvert.SerializeObject(linkMonitor[linkMonitorId].Peek()).ToJsonFormatString());
#if DEBUG
                System.Console.WriteLine($"=============OdinAspectCoreInterceptorAttribute  return  end=============");
#endif
            }
        }
    }
}