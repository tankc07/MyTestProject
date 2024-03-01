namespace LogisticsCore.NewEMS.Response
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
    public class NewEmsResponseBase
    {
        /// <summary>
        /// 每次请求的唯一编码，用于查询日志使用
        /// </summary>
        public string serialNo { get; set; }
        /// <summary>
        /// 响应代码
        /// <para>00000- 表示成功 S00001- 表示系统级错误 B00001- 表示接口级错误</para>
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string retMsg { get; set; }
        ///// <summary>
        ///// 响应消息体报文
        ///// </summary>
        //public string retBody { get; set; }
        /// <summary>
        /// 各接口返回的时间
        /// </summary>
        public string retDate { get; set; }
        /// <summary>
        /// 是否成功 true:成功 false:失败
        /// </summary>
        public bool success { get; set; }
    }
}
