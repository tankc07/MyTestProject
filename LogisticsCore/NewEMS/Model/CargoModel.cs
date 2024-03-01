namespace LogisticsCore.NewEMS.Model
{
    /// <summary>
    /// Cargo对象Model
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
    public class CargoModel
    {
        /// <summary>
        /// * 商品名称
        /// </summary>
        public string cargoName { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public string cargoCategory { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public string cargoQuantity { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public string cargoValue { get; set; }
        /// <summary>
        /// 商品重量
        /// </summary>
        public string cargoWeight { get; set; }
        /// <summary>
        /// 一票多件的子单号?????
        /// </summary>
        public string cargoMailNo { get; set; }
        /// <summary>
        /// 订单号: 用于一票多件
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 长: 单位：厘米（用于一票多件）
        /// </summary>
        public string length { get; set; }
        /// <summary>
        /// 宽: 单位：厘米（用于一票多件）
        /// </summary>
        public string width { get; set; }
        /// <summary>
        /// 高: 单位：厘米（用于一票多件）
        /// </summary>
        public string high { get; set; }
    }
}
