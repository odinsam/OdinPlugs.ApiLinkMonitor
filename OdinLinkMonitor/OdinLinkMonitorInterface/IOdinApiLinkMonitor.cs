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
        /// <summary>
        /// 创建链路起始节点
        /// </summary>
        /// <param name="linkSnowFlakeId">当前链路的雪花Id</param>
        /// <returns>key: 雪花Id value: 当前链路的栈</returns>
        Dictionary<long, Stack<OdinApiLinkModel>> CreateOdinLinkMonitor(long linkSnowFlakeId);
        /// <summary>
        /// 创建链路调用节点
        /// </summary>
        /// <param name="context">AspectContext上下文</param>
        /// <returns>key: 雪花Id value: 当前链路的栈</returns>
        Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerLinkMonitor(AspectContext context);
        /// <summary>
        /// 生成方法调用返回的的链路节点,并将数据添加到当前链路当中
        /// </summary>
        /// <param name="context">AspectContext上下文</param>
        /// <param name="isSuccess">true 当前方法调用成功 false 当前方法调用catch</param>
        /// <param name="stopwatch">当前方法调用耗时</param>
        /// <returns>key: 雪花Id value: 当前链路的栈</returns>
        Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerToEndLinkMonitor(AspectContext context, bool isSuccess, Stopwatch stopwatch);
        /// <summary>
        /// 创建链路监控结束对象
        /// </summary>
        /// <param name="context">HttpContext 上下文</param>
        /// <param name="elapsedTime">链路耗时</param>
        /// <param name="isSuccess">链路中的方法是否成功</param>
        /// <returns>key: 雪花Id value: 当前链路的栈</returns>
        Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerOverLinkMonitor(HttpContext context, long elapsedTime, bool isSuccess = true);
    }
}