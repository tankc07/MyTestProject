namespace LogisticsCore.JingDong.Model
{
    /// <summary>
    /// 收件人信息
    /// </summary>
    public class ReceiverContactModel
    {
        /// <summary>
        /// * 收件人姓名，说明：不能为生僻字，暂不支持emoji；最大长度50
        /// </summary>
        public string receiverName { get; set; }
        /// <summary>
        /// 收件人手机号(收件人电话、手机至少有一个不为空)；最大长度50
        /// </summary>
        public string receiverMobile { get; set; }
        /// <summary>
        /// 收件人省编码；最大长度100
        /// </summary>
        public int? receiverProvince { get; set; }
        /// <summary>
        /// 收件人邮编，长度：6
        /// </summary>
        public string receiverPostcode { get; set; }
        /// <summary>
        /// 收件人公司，长度：50，说明：不能为生僻字
        /// </summary>
        public string receiverCompany { get; set; }
        /// <summary>
        /// 收件人县编码；最大长度100
        /// </summary>
        public int? receiverCounty { get; set; }
        /// <summary>
        /// 收件人市编码；最大长度100
        /// </summary>
        public int? receiverCity { get; set; }
        /// <summary>
        /// * 收件人地址，说明：不能为生僻字，请填写省市区县详细地址；最大长度350
        /// </summary>
        public string receiverAddress { get; set; }
        /// <summary>
        /// 收件人电话；最大长度50
        /// </summary>
        public string receiverPhone { get; set; }
        /// <summary>
        /// 收件人市；最大长度100
        /// </summary>
        public string receiverCityName { get; set; }

        /// <summary>
        /// 收件人镇；最大长度100
        /// </summary>
        public string receiverTownName { get; set; }
        /// <summary>
        /// 收件人省；最大长度100
        /// </summary>
        public string receiverProvinceName { get; set; }
        /// <summary>
        /// 收件人镇编码；最大长度100
        /// </summary>
        public int? receiverTown { get; set; }
        /// <summary>
        /// 收件人县；最大长度100
        /// </summary>
        public string receiverCountyName { get; set; }
        /// <summary>
        /// 当销售平台为（0010001）京东商城，收件人信息可以通过OAID进行解密
        /// </summary>
        public string receiverOAID { get; set; }
    }
}
