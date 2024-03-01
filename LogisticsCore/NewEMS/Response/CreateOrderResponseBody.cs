namespace LogisticsCore.NewEMS.Response
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
    public class CreateOrderResponseBody
    {
        /// <summary>
        /// 物流订单号（客户内部订单号）
        /// </summary>
        public string logisticsOrderNo { get; set; }
        /// <summary>
        /// 物流运单号（一票多件、返单业务单号逗号分隔）
        /// </summary>
        public string waybillNo { get; set; }
        /// <summary>
        /// 四段码（分拣码）
        /// </summary>
        public string routeCode { get; set; }
        /// <summary>
        /// 集包地编码
        /// </summary>
        public string packageCode { get; set; }
        /// <summary>
        /// 集包地名称
        /// </summary>
        public string packageCodeName { get; set; }
        /// <summary>
        /// 大头笔编码
        /// </summary>
        public string markDestinationCode { get; set; }
        /// <summary>
        /// 大头笔名称
        /// </summary>
        public string markDestinationName { get; set; }

    }
}
