using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OdinPlugs.ApiLinkMonitor.Utils
{
    public class OdinAopMiddlewarePathRegex
    {
        /// <summary>
        /// ~ 当前路径是否 是被忽略的路径
        /// </summary>
        /// <param name="requestPath">当前路径</param>
        /// <returns>true 应该被忽略，不需要链路监控监控 otherwise false</returns>
        public static bool IsIgnorePath(string requestPath, List<string> lstStr)
        {
            List<string> paths = new List<string> { "/swagger", "/v2.0/api-docs" };
            paths.AddRange(lstStr);
            return paths.Where(p => requestPath.StartsWith(p)).SingleOrDefault() != null;
        }
    }
}