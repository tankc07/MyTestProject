namespace LogisticsCore.JingDong.Response
{
    /// <summary>
    /// 京东生鲜医药快递接口响应基类
    /// </summary>
    public class FreshMedicineDeliveryResponseBase
    {
        /// <summary>
        /// 返回码；最大长度10；1000成功，其他数字失败
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回信息；最大长度200
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 表示操作或请求花费的毫秒数???
        /// </summary>
        public int mills { get; set; }
        /// <summary>
        /// 请求的唯一标识符，用于跟踪或日志记录
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 是否成功标志
        /// </summary>
        public bool success { get; set; }
    }
}
