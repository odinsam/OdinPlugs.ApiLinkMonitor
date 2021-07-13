namespace OdinPlugs.ApiLinkMonitor.Models.EnumLink
{
    public enum EnumLinkStatus
    {
        /// <summary>
        /// 链路开始
        /// </summary>
        Start,
        /// <summary>
        /// 链路调用
        /// </summary>
        Invoker,
        /// <summary>
        /// 链路结束
        /// </summary>
        Over,
        /// <summary>
        /// 链路到底,开始返回
        /// </summary>
        ToEndReturn,
    }
}