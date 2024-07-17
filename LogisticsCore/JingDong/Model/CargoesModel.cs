namespace LogisticsCore.JingDong.Model
{
    /// <summary>
    /// 物品信息
    /// </summary>
    public class CargoesModel
    {
        /// <summary>
        /// * 体积(单位：cm3，保留小数点后两位)；最大长度28
        /// </summary>
        public double volume { get; set; }
        /// <summary>
        /// 寄托物数量；最大长度5，最大值99999
        /// </summary>
        public int? goodsCount { get; set; }
        /// <summary>
        /// 包裹长(单位：cm,保留小数点后两位)；最大长度10
        /// </summary>
        public double? length { get; set; }
        /// <summary>
        /// 包裹宽(单位：cm，保留小数点后两位)；最大长度10
        /// </summary>
        public double? width { get; set; }
        /// <summary>
        /// 包裹高(单位：cm，保留小数点后两位)；最大长度10
        /// </summary>
        public double? height { get; set; }
        /// <summary>
        /// * 订单总重量(单位：kg，保留小数点后两位)；最大长度18
        /// </summary>
        public double weight { get; set; }
        /// <summary>
        /// 商品描述；最大长度255
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 托寄物名称，长度：200，说明：为避免托运物品在铁路、航空安检中被扣押，请务必下传商品类目或名称，例如：服装、3C等；最大长度200
        /// </summary>
        public string goodsName { get; set; }
        /// <summary>
        /// 包裹数(大于0，小于99999)；最大长度5
        /// </summary>
        public int packageQty { get; set; }

    }
}
