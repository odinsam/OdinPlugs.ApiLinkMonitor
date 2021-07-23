**服务调用链路监控框架**

1. 简介

项目基于 [OdinInjectCore](https://github.com/odinsam/OdinPlugs.OdinInject) 和 [SnowFlake](https://github.com/odinsam/OdinPlugs.SnowFlake) 框架开发，使用 .net core 中间件实现，主要用到了 AspectCore.Core 第三方框架为底层服务类方法调用的链路监控。监控的项目调用的链路信息入库 mongo，所以需要搭配 mongo 一起使用.
并且

2. 使用

```csharp
// 注入链路监控
services
    .AddSingletonSnowFlake(dataCenterId, workerId)
    .AddOdinTransientMongoDb(
        opt => { opt.ConnectionString = mongoConnection; opt.DbName = databaseName; })
    .AddOdinTransientInject<IOdinApiLinkMonitor>();

services.ConfigureDynamicProxy(config =>
    {
        // ~ 使用通配符的特定全局拦截器
        config.Interceptors.AddTyped<OdinAspectCoreInterceptorAttribute>(Predicates.ForService("*Service"));
    });


```

3. 链路数据

controller 调用的 service 类需要以 Service 结尾,例如 接口 ITestService 和 实现类 TestService,controller 的 api 方法被调用后生成链路数据.
所有数据均进入 mongo。

```json
{
    // 链路的 雪花Id，本次链路唯一
    "Id": 206324387228553216,
    // 链路 当前的雪花Id
    "CurrentId": 206324387245330432,
    // 链路状态
    "LinkStatusEnum": 0,
    // 链路状态描述 start 表示本次链路开始
    "LinkStatusStr": "Start",
    // 上层链路雪花Id 0代表没有上层链路
    "LinkPrevious": 0,
    // 链路调用返回的状态
    "InvokerReturnStatusEnum": 0,
    // 链路调用返回的状态描述
    "InvokerReturnStatusStr": null,
    // 下级链路雪花Id
    "LinkNext": 206324387245330433,
    // 链路耗时
    "ElapsedTime": null,
    // 链路调用的完整类名
    "InvokerClassFullName": null,
    // 链路调用的类名
    "InvokerClassName": null,
    // 调用的方法名
    "InvokerMethodName": null,
    // 本次链路的需要 从小到大排序
    "LinkSort": 0
}


{
    "Id": 206324387228553216,
    "CurrentId": 206324394790883328,
    "LinkStatusEnum": 1,
    // 链路状态描述  Invoker 表示链路调用中
    "LinkStatusStr": "Invoker",
    // 与上一层链路的 LinkNext 对应
    "LinkPrevious": 206324387245330433,
    "InvokerReturnStatusEnum": 0,
    "InvokerReturnStatusStr": null,
    "LinkNext": 206324394790883329,
    "ElapsedTime": null,
    "InvokerClassFullName": "OdinCore.Services.InterfaceServices.ITestService",
    "InvokerClassName": "ITestService",
    "InvokerMethodName": "show",
    "LinkSort": 1
}

{
    "Id": 206324387228553216,
    "CurrentId": 206324394962849792,
    "LinkStatusEnum": 1,
    "LinkStatusStr": "Invoker",
    "LinkPrevious": 206324394790883329,
    "InvokerReturnStatusEnum": 0,
    "InvokerReturnStatusStr": null,
    "LinkNext": 206324394962849793,
    "ElapsedTime": null,
    "InvokerClassFullName": "OdinCore.Services.InterfaceServices.IInerService",
    "InvokerClassName": "IInerService",
    "InvokerMethodName": "show",
    "LinkSort": 2
}

{
    "Id": 206324387228553216,
    "CurrentId": 206324395122233344,
    "LinkStatusEnum": 1,
    "LinkStatusStr": "Invoker",
    "LinkPrevious": 206324394962849793,
    "InvokerReturnStatusEnum": 0,
    "InvokerReturnStatusStr": null,
    "LinkNext": 206324395122233345,
    "ElapsedTime": null,
    "InvokerClassFullName": "OdinCore.Services.InterfaceServices.ITTService",
    "InvokerClassName": "ITTService",
    "InvokerMethodName": "show",
    "LinkSort": 3
}

{
    "Id": 206324387228553216,
    "CurrentId": 206324395277422592,
    "LinkStatusEnum": 3,
    //  链路状态描述  ToEndReturn 表示链路调用返回
    "LinkStatusStr": "ToEndReturn",
    "LinkPrevious": 206324395122233345,
    "InvokerReturnStatusEnum": 2,
    // 链路调用返回的状态描述 CatchReturn 表示调用出现异常 但是被catch捕获
    "InvokerReturnStatusStr": "CatchReturn",
    "LinkNext": 206324395277422593,
    "ElapsedTime": 37,
    "InvokerClassFullName": "OdinCore.Services.InterfaceServices.ITTService",
    "InvokerClassName": "ITTService",
    "InvokerMethodName": "show",
    "LinkSort": 4
}

{
    "Id": 206324387228553216,
    "CurrentId": 206324395520692224,
    "LinkStatusEnum": 3,
    "LinkStatusStr": "ToEndReturn",
    "LinkPrevious": 206324395277422593,
    "InvokerReturnStatusEnum": 2,
    "InvokerReturnStatusStr": "CatchReturn",
    "LinkNext": 206324395520692225,
    "ElapsedTime": 133,
    "InvokerClassFullName": "OdinCore.Services.InterfaceServices.IInerService",
    "InvokerClassName": "IInerService",
    "InvokerMethodName": "show",
    "LinkSort": 5
}

{
    "Id": 206324387228553216,
    "CurrentId": 206324395667492864,
    "LinkStatusEnum": 3,
    "LinkStatusStr": "ToEndReturn",
    "LinkPrevious": 206324395520692225,
    "InvokerReturnStatusEnum": 2,
    "InvokerReturnStatusStr": "CatchReturn",
    "LinkNext": 206324395667492865,
    "ElapsedTime": 210,
    "InvokerClassFullName": "OdinCore.Services.InterfaceServices.ITestService",
    "InvokerClassName": "ITestService",
    "InvokerMethodName": "show",
    "LinkSort": 6
}

{
    "Id": 206324387228553216,
    "CurrentId": 206324396925784064,
    "LinkStatusEnum": 2,
    //  链路状态描述  Over 表示本次链路调用结束
    "LinkStatusStr": "Over",
    "LinkPrevious": 206324395667492865,
    "InvokerReturnStatusEnum": 1,
    // 调用成功结束
    "InvokerReturnStatusStr": "Success",
    "LinkNext": 0,
    // 本次调用 2558 ms
    "ElapsedTime": 2558,
    "InvokerClassFullName": null,
    "InvokerClassName": null,
    "InvokerMethodName": null,
    "LinkSort": 7
}

```

4. 解析

通过链路数据得出结论：
本次 api 调用：
首先 调用 OdinCore.Services.InterfaceServices.ITestService 类的 show 方法,方法内部调用 OdinCore.Services.InterfaceServices.IInerService 类的 show 方法，
然后又调用 OdinCore.Services.InterfaceServices.ITTService 类的 show 方法，结果调用出错，但是被 catch 捕获最后返回。

swagger 调用返回信息如下:

```json
{
    "SnowFlakeId": "206324387228553216",
    "Data": {
        "ClassName": "System.Exception",
        "Message": "ttservice throw",
        "Data": null,
        "InnerException": null,
        "HelpURL": null,
        "StackTraceString": "   at OdinPlugs.OdinMvcCore.OdinFilter.ApiInvokerFilterAttribute.OnActionExecuted(ActionExecutedContext context) in /Users/odin/workSpace/github/odinmaf/OdinPlugs/OdinMvcCore/OdinFilter/ApiInvokFilterAttribute.cs:line 109\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeNextActionFilterAsync()\n--- End of stack trace from previous location ---\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeInnerFilterAsync()\n--- End of stack trace from previous location ---\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextExceptionFilterAsync>g__Awaited|25_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ExceptionContextSealed context)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeNextResourceFilter()\n--- End of stack trace from previous location ---\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResourceExecutedContextSealed context)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeFilterPipelineAsync()\n--- End of stack trace from previous location ---\n   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)\n   at Microsoft.AspNetCore.MiddlewareAnalysis.AnalysisMiddleware.Invoke(HttpContext httpContext)\n   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)\n   at Microsoft.AspNetCore.MiddlewareAnalysis.AnalysisMiddleware.Invoke(HttpContext httpContext)\n   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)\n   at Microsoft.AspNetCore.MiddlewareAnalysis.AnalysisMiddleware.Invoke(HttpContext httpContext)\n   at Microsoft.AspNetCore.MiddlewareAnalysis.AnalysisMiddleware.Invoke(HttpContext httpContext)\n   at Microsoft.AspNetCore.MiddlewareAnalysis.AnalysisMiddleware.Invoke(HttpContext httpContext)\n   at Microsoft.AspNetCore.MiddlewareAnalysis.AnalysisMiddleware.Invoke(HttpContext httpContext)\n   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)\n   at Microsoft.AspNetCore.MiddlewareAnalysis.AnalysisMiddleware.Invoke(HttpContext httpContext)\n   at OdinPlugs.ApiLinkMonitor.OdinMiddleware.OdinExceptionMiddleware.Invoke(HttpContext context) in /Users/odin/workSpace/github/odinmaf/OdinPlugs.ApiLinkMonitor/OdinMiddleware/OdinExceptionMiddleware.cs:line 34",
        "RemoteStackTraceString": null,
        "RemoteStackIndex": 0,
        "ExceptionMethod": null,
        "HResult": -2146233088,
        "Source": "AspectCore.Core",
        "WatsonBuckets": null
    },
    "StatusCode": "sys-error",
    "ErrorMessage": "系统异常，请联系管理员",
    "Message": "系统异常:[sys-error]"
}
```

通过相同的雪花 Id 最后会清晰的分析出整个调用过程中的过程，并且如果能够结合 [OdinPlugs](https://github.com/odinsam/OdinPlugs.OdinPlugs) 框架,可以监控到 controller 中 api 的调用记录，包括入参、时间、返回信息等会更加详细。
