namespace LogisticsCore.NewEMS.Model
{
    /// <summary>
    /// 取消下单接口参数类
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
    public class CancelOrderModel : LogisticsInterfaceBase
    {
        /// <summary>
        /// * 物流订单号（ 客户内部订单号） 
        /// </summary>
        public string logisticsOrderNo { get; set; }
        /// <summary>
        /// * Ems运单号
        /// </summary>
        public string waybillNo { get; set; }
        /// <summary>
        /// * 订单取消原因
        /// </summary>
        public string cancelReason { get; set; }
    }
}
