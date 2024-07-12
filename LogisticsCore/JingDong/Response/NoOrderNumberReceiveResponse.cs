namespace LogisticsCore.JingDong.Response
{
    /// <summary>
    /// 无单号下单响应
    /// </summary>
    public class NoOrderNumberReceiveResponse : FreshMedicineDeliveryResponseBase
    {
        public NoOrderNumberReceiveResponseBody data { get; set; }
        public bool HasData => data != null;
    }

    /// <summary>
    /// 下单请求返回参数中的Data实体
    /// </summary>
    public class NoOrderNumberReceiveResponseBody
    {
        /// <summary>
        /// 销售平台订单号；如果有多个单号，用英文,间隔。例如：7898675,7898676)，注：逗号前需要填写订单号；最大长度50
        /// </summary>
        public string channelOrderId { get; set; }
        /// <summary>
        /// 运输类型（1：陆运 2：航空）；最大长度4
        /// </summary>
        public int? transType { get; set; }
        /// <summary>
        /// 订单号；最大长度50
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 是否需要重试，人工预分拣时，值为true；最大长度1
        /// </summary>
        public bool needRetry { get; set; }
        /// <summary>
        /// 产品类型（1：特惠送 2：特快送 4：城际闪送 5：同城当日达 6：次晨达 7：微小件 8: 生鲜专送 16：生鲜特快 17:生鲜特惠）；最大长度4
        /// </summary>
        public int? promiseTimeType { get; set; }
        /// <summary>
        /// 运单号；最大长度50
        /// </summary>
        public string waybillNo { get; set; }
        /// <summary>
        /// 运营模式 (1,P2,航),(2, P4, 同城),(3, P7, 次晨达)，此字段仅在产品类型是特快送时返回；最大长度4
        /// </summary>
        public int? expressOperationMode { get; set; }
    }
}
