namespace LogisticsCore.JingDong.Request
{
    public class CancelOrderByVendorCodeAndDeliveryIdRequest
    {
        /// <summary>
        /// 取消操作人；最大长度50
        /// </summary>
        public string cancelOperator { get; set; }
        /// <summary>
        /// 发起取消时间；最大长度20（格式：yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public string cancelTime { get; set; }
        /// <summary>
        /// 取消操作人编码类型:1-京东pin,2-erp账号；最大长度4
        /// </summary>
        public int? cancelOperatorCodeType { get; set; }
        /// <summary>
        /// * 商家编码；最大长度50
        /// </summary>
        public string customerCode { get; set; }
        /// <summary>
        /// * 拦截原因；最大长度200
        /// </summary>
        public string interceptReason { get; set; }
        /// <summary>
        /// * 运单号；最大长度50
        /// </summary>
        public string waybillNo { get; set; }

    }
}
