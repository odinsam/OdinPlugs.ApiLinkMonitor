using System;
using System.Collections.Generic;
using System.Diagnostics;
using AspectCore.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OdinPlugs.ApiLinkMonitor.Models.ApiLinkModels;
using OdinPlugs.ApiLinkMonitor.Models.EnumLink;
using OdinPlugs.ApiLinkMonitor.OdinLinkMonitor.OdinLinkMonitorInterface;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinInject.InjectPlugs.OdinMongoDbInject;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;
using OdinPlugs.SnowFlake.SnowFlakePlugs.ISnowFlake;

namespace OdinPlugs.ApiLinkMonitor.OdinLinkMonitor
{
    public class OdinApiLinkMonitor : IOdinApiLinkMonitor
    {
        Dictionary<long, Stack<OdinApiLinkModel>> linksDic;
        static IOdinSnowFlake snowFlake = OdinInjectCore.GetService<IOdinSnowFlake>();
        IOdinMongo mongoHelper = OdinInjectCore.GetService<IOdinMongo>();

        /// <summary>
        /// 创建链路监控对象
        /// </summary>
        /// <param name="linkSnowFlakeId">当次链路的唯一标识，雪花ID</param>
        /// <returns>可以存储链路记录的容器对象</returns>
        public Dictionary<long, Stack<OdinApiLinkModel>> CreateOdinLinkMonitor(long linkSnowFlakeId)
        {
            /*
            1. 创建 dic类型 链路保存容器 key: 当次链路唯一标识，雪花ID  value: 栈类型数据，内置链路模型对象
            2. 创建 栈空间 内置链路模型对象
            3. 创建 当次链路的雪花ID
            4. 创建当次链路对象 - start链路
            4. 将链路对象push到栈当中
            5. 栈空间存储到 链路保存容器dic当中
            6. 将链路保存容器dic存储在当前httpcontext中方便后续传递和使用
            */
            linksDic = new Dictionary<long, Stack<OdinApiLinkModel>>();
            var stackApiLink = new Stack<OdinApiLinkModel>();
            var linkModel = new OdinApiLinkModel
            {
                Id = linkSnowFlakeId,
                LinkStatusEnum = EnumLinkStatus.Start,
                LinkStatusStr = EnumLinkStatus.Start.ToString(),
                LinkNext = snowFlake.CreateSnowFlakeId(),
                LinkSort = 0,
            };
            stackApiLink.Push(linkModel);
            linksDic.Add(linkSnowFlakeId, stackApiLink);
            #region 保存link记录到mongodb--链路Start
            mongoHelper.AddModel<OdinApiLinkModel>("ApiLink_Monitor", linkModel);
            #endregion
            return linksDic;
        }

        /// <summary>
        /// 生成方法调用的链路节点,并将数据添加到当前链路当中
        /// </summary>
        /// <param name="linkSnowFlakeId">当次链路的唯一标识，雪花ID</param>
        /// <returns>可以存储链路记录的容器对象</returns>
        public Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerLinkMonitor(AspectContext context)
        {
            try
            {
                var odinlinkId = Convert.ToInt64(context.GetHttpContext().Items["odinlinkId"]);
                var linksDic = context.GetHttpContext().Items["odinlink"] as Dictionary<long, Stack<OdinApiLinkModel>>;
                if (linksDic != null && linksDic.ContainsKey(odinlinkId) && linksDic[odinlinkId].Count > 0)
                {
                    Dictionary<string, Object> dicParams = new Dictionary<string, object>();
                    var stackLink = linksDic[odinlinkId];
                    // 使用peek方法获取栈顶元素但不弹出
                    var stackTopele = stackLink.Peek();
                    //创建当前链路节点
                    var linkModel = new OdinApiLinkModel
                    {
                        Id = odinlinkId,
                        LinkStatusEnum = EnumLinkStatus.Invoker,
                        LinkStatusStr = EnumLinkStatus.Invoker.ToString(),
                        // 在栈中获取上次的链路节点的next值，作为当前的节点的前一次的值 
                        LinkPrevious = stackTopele.LinkNext,
                        LinkNext = snowFlake.CreateSnowFlakeId(),
                        InvokerClassFullName = context.ServiceMethod.DeclaringType.FullName,
                        InvokerClassName = context.ServiceMethod.DeclaringType.Name,
                        InvokerMethodName = context.ServiceMethod.Name,
                        // InvokerMethodParams = context.ServiceMethod.GetParameters() != null && context.ServiceMethod.GetParameters().Length > 0 ?
                        // JsonConvert.SerializeObject(context.ServiceMethod.GetParameters().Select(p => p.Name).ToList()) :
                        // null,
                        LinkSort = stackTopele.LinkSort + 1,
                    };
                    stackLink.Push(linkModel);
                    #region 保存link记录到mongodb--链路Invoker
                    mongoHelper.AddModel<OdinApiLinkModel>("ApiLink_Monitor", linkModel);
                    #endregion
                    return linksDic;
                }
                else
                {
                    throw new System.Exception("当前链路的雪花ID有误,无法记录调用链路");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(ex).ToJsonFormatString());
                return null;
            }
        }


        /// <summary>
        /// 生成方法调用返回的的链路节点,并将数据添加到当前链路当中
        /// </summary>
        /// <param name="context">AspectContext上下文</param>
        /// <param name="isSuccess">true 当前方法调用成功 false 当前方法调用catch</param>
        /// <param name="elapsedTime">当前方法调用耗时</param>
        /// <returns>可以存储链路记录的容器对象</returns>
        /// <exception cref="Exception"></exception>
        public Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerToEndLinkMonitor(AspectContext context,
                                                        bool isSuccess, Stopwatch stopwatch)
        {
            try
            {
                var result = context.ReturnValue != null ? JsonConvert.SerializeObject((context.ReturnValue)) : "";
                var odinlinkId = Convert.ToInt64(context.GetHttpContext().Items["odinlinkId"]);
                var linksDic = context.GetHttpContext().Items["odinlink"] as Dictionary<long, Stack<OdinApiLinkModel>>;
                if (linksDic != null && linksDic.ContainsKey(odinlinkId) && linksDic[odinlinkId].Count > 0)
                {
                    var stackLink = linksDic[odinlinkId];
                    // 使用peek方法获取栈顶元素但不弹出
                    var stackTopele = stackLink.Peek();
                    var invokerReturnStatusenum =
                        isSuccess ? EnumInvokerReturnStatus.Success : EnumInvokerReturnStatus.CatchReturn;
                    stopwatch.Stop();
                    //创建当前链路节点
                    var linkModel = new OdinApiLinkModel
                    {
                        Id = odinlinkId,
                        LinkStatusEnum = EnumLinkStatus.ToEndReturn,
                        LinkStatusStr = EnumLinkStatus.ToEndReturn.ToString(),
                        // 在栈中获取上次的链路节点的next值，作为当前的节点的前一次的值 
                        LinkPrevious = stackTopele.LinkNext,
                        InvokerReturnStatusEnum = invokerReturnStatusenum,
                        InvokerReturnStatusStr = invokerReturnStatusenum.ToString(),
                        // InvokerResult = result,
                        LinkNext = snowFlake.CreateSnowFlakeId(),
                        ElapsedTime = stopwatch.ElapsedMilliseconds,
                        InvokerClassFullName = context.ServiceMethod.DeclaringType?.FullName,
                        InvokerClassName = context.ServiceMethod.DeclaringType?.Name,
                        InvokerMethodName = context.ServiceMethod.Name,
                        // InvokerMethodParams = context.ServiceMethod.GetParameters().Length > 0
                        //             ?
                        //             JsonConvert.SerializeObject(context.ServiceMethod.GetParameters().Select(p => p.Name).ToList())
                        //             :
                        //             null,
                        LinkSort = stackTopele.LinkSort + 1,
                    };
                    stackLink.Push(linkModel);
                    #region 保存link记录到mongodb--链路ToEndReturn
                    mongoHelper.AddModel<OdinApiLinkModel>("ApiLink_Monitor", linkModel);
                    #endregion
                    return linksDic;
                }
                else
                {
                    throw new System.Exception("当前链路的雪花ID有误,无法记录调用链路");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(ex).ToJsonFormatString());
                throw ex;
            }
        }



        /// <summary>
        /// 创建链路监控结束对象
        /// </summary>
        /// <param name="context">HttpContext 上下文</param>
        /// <param name="elapsedTime">链路耗时</param>
        /// <param name="isSuccess">链路中的方法是否成功</param>
        /// <returns>key: 雪花Id value: 当前链路的栈</returns>
        /// <exception cref="Exception"></exception>
        public Dictionary<long, Stack<OdinApiLinkModel>> ApiInvokerOverLinkMonitor(HttpContext context, long elapsedTime, bool isSuccess = true)
        {
            try
            {
                var odinlinkId = Convert.ToInt64(context.Items["odinlinkId"]);
                var linksDic = context.Items["odinlink"] as Dictionary<long, Stack<OdinApiLinkModel>>;
                if (linksDic != null && linksDic.ContainsKey(odinlinkId) && linksDic[odinlinkId].Count > 0)
                {
                    var stackLink = linksDic[odinlinkId];
                    // 使用peek方法获取栈顶元素但不弹出
                    var stackTopele = stackLink.Peek();
                    var invokerReturnStatusenum = isSuccess ? EnumInvokerReturnStatus.Success : EnumInvokerReturnStatus.CatchReturn;
                    //创建当前链路节点
                    var linkModel = new OdinApiLinkModel
                    {
                        Id = odinlinkId,
                        LinkStatusEnum = EnumLinkStatus.Over,
                        LinkStatusStr = EnumLinkStatus.Over.ToString(),
                        // 在栈中获取上次的链路节点的next值，作为当前的节点的前一次的值 
                        LinkPrevious = stackTopele.LinkNext,
                        InvokerReturnStatusEnum = invokerReturnStatusenum,
                        InvokerReturnStatusStr = invokerReturnStatusenum.ToString(),
                        // InvokerResult = "result",
                        ElapsedTime = elapsedTime,
                        // InvokerMethodParams = JsonConvert.SerializeObject(OdinRequestParamasHelper.GetRequestParams(context)),
                        LinkSort = stackTopele.LinkSort + 1,
                    };
                    stackLink.Push(linkModel);
                    #region 保存link记录到mongodb--链路Over
                    mongoHelper.AddModel<OdinApiLinkModel>("ApiLink_Monitor", linkModel);
                    #endregion
                    return linksDic;
                }
                else
                {
                    throw new System.Exception("当前链路的雪花ID有误,无法记录调用链路");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(e).ToJsonFormatString());
                throw e;
            }
        }
    }
}