namespace LogisticsCore.JingDong.Model
{
    /// <summary>
    /// 寄件人信息
    /// </summary>
    public class SenderContactModel
    {
        /// <summary>
        /// 寄件人手机；最大长度50；与寄件人电话必填其一
        /// </summary>
        public string senderPhone { get; set; }
        /// <summary>
        /// * 寄件人姓名，说明：不能为生僻字，暂不支持emoji；最大长度50
        /// </summary>
        public string senderName { get; set; }
        /// <summary>
        /// * 寄件人地址，说明：不能为生僻字，请填写省市区县详细地址；最大长度350
        /// </summary>
        public string senderAddress { get; set; }
        /// <summary>
        /// 寄件人邮编，最大长度6
        /// </summary>
        public string senderPostcode { get; set; }
        /// <summary>
        /// 寄件人公司，长度：50，说明：不能为生僻字
        /// </summary>
        public string senderCompany { get;set; }
        /// <summary>
        /// 寄件人电话；最大长度50；与寄件人手机必填其一
        /// </summary>
        public string senderMobile { get; set; }

    }
}
