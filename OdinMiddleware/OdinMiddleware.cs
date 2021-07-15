using System.Collections.Generic;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware
{
    public class OdinMiddleware
    {
        public List<string> LstStr { get; set; }
        public OdinMiddleware(List<string> lstStr)
        {
            this.LstStr = lstStr;
        }
    }
}