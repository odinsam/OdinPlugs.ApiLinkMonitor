namespace OdinPlugs.ApiLinkMonitor.Models.EnumLink
{
    /// <summary>
    /// 链路返回状态
    /// </summary>
    public enum EnumInvokerReturnStatus
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// catch异常 - 被捕获
        /// </summary>
        CatchReturn,
        /// <summary>
        /// throw 异常
        /// </summary>
        ThrowException
    }
}