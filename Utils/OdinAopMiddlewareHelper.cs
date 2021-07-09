using System.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OdinPlugs.OdinMvcCore.OdinMiddleware.Utils
{
    public class OdinAopMiddlewareHelper
    {
        private static Aop_ApiInvokerRecord_Model apiInvokerRecordModel = null;
        private static Aop_ApiInvokerCatch_Model apiInvokerCatchModel = null;
        private static Aop_ApiInvokerThrow_Model apiInvokerThrow_Model = null;

        /// <summary>
        /// 中间件请求前，尚未进入action方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="apiInvokerModel"></param> 
        public static void MiddlewareBefore(HttpContext context, Aop_Invoker_Model apiInvokerModel)
        {

        }

        /// <summary>
        /// 中间件请求成功后  action方法已经执行完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="apiInvokerModel"></param>
        /// <param name="originalBodyStream"></param>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        public void MiddlewareAfter(HttpContext context, Aop_Invoker_Model apiInvokerModel, Stream originalBodyStream, MemoryStream responseBody)
        {


        }

        public void MiddlewareResponseCompleted(HttpContext context, Aop_Invoker_Model apiInvokerModel, Stopwatch stopWatch)
        {

        }

        public async Task MiddlewareException(HttpContext context, Aop_Invoker_Model apiInvokerModel, Exception ex)
        {

        }
    }
}