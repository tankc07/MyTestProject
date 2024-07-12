namespace LogisticsCore.JingDong.Model
{
    /// <summary>
    /// 客户箱(暂不用)
    /// </summary>
    public class CustomerBoxListModel
    {
        /// <summary>
        /// 客户箱型编号；最大长度50
        /// </summary>
        public string customerBoxNo { get; set; }
        /// <summary>
        /// 客户箱型箱数；最大长度5，最大值99999
        /// </summary>
        public int? customerBoxQty { get; set; }
    }
}
