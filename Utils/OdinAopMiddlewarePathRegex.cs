using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OdinPlugs.ApiLinkMonitor.Utils
{
    /// <summary>
    /// 链路忽略监控正则方法
    /// </summary>
    public class OdinAopMiddlewarePathRegex
    {
        /// <summary>
        /// ~ 当前路径是否 是被忽略的路径
        /// </summary>
        /// <param name="requestPath">当前路径</param>
        /// <returns>true 应该被忽略，不需要链路监控监控 otherwise false</returns>
        public static bool IsIgnorePath(string requestPath, List<string> lstStr)
        {
            List<string> paths = new List<string> { @"\/swagger", @"\/index.html", @"\/v[0-9]((.[0-9])?)\/((\w)+)-((\w)+)" };
            paths.AddRange(lstStr);
            System.Console.WriteLine(requestPath);

            foreach (var item in paths)
            {
                System.Console.WriteLine($"item:{item},regex:{Regex.IsMatch(requestPath, item, RegexOptions.IgnoreCase)}");
                if (Regex.IsMatch(requestPath, item, RegexOptions.IgnoreCase))
                    return true;
            }
            return false;
        }
    }
}