using System.Collections.Generic;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware
{
    /// <summary>
    /// 链路监控忽略uri
    /// </summary>
    public class OdinMiddleware
    {
        public List<string> LstStr { get; set; }
        public OdinMiddleware(List<string> lstStr)
        {
            this.LstStr = lstStr;
        }
    }
}