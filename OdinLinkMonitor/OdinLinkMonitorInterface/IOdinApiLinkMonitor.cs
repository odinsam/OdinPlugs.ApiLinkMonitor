using System;
using System.Collections.Generic;
using System.Diagnostics;
using AspectCore.DynamicProxy;
using Microsoft.AspNetCore.Http;
using OdinPlugs.ApiLinkMonitor.Models.ApiLinkModels;
using OdinPlugs.OdinInject.InjectInterface;

namespace OdinPlugs.ApiLinkMonitor.OdinLinkMonitor.OdinLinkMonitorInterface
{
    public interface IOdinApiLinkMonitor : IAutoInject
    {
        Dictionary<long, Stack<OdinApiLinkModel>> CreateOdinLinkMonitor(long linkSnowFlakeId);
        Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerLinkMonitor(AspectContext context);
        Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerToEndLinkMonitor(AspectContext context, bool isSuccess, Stopwatch stopwatch);
        Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerOverLinkMonitor(HttpContext context, long elapsedTime, bool isSuccess = true);
    }
}