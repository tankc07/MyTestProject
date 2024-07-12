namespace LogisticsCore.JingDong.Model
{
    /// <summary>
    /// 退货人信息(暂不用)
    /// </summary>
    public class BackContactModel
    {
        /// <summary>
        /// 退货人手机号（和退货人座机号二选一必填）（指定退货地址退货场景必填）；最大长度100
        /// </summary>
        public string backMobile { get; set; }
        /// <summary>
        /// 退货人村/镇名称（直辖市不填写）（指定退货地址退货场景必填）；最大长度100
        /// </summary>
        public string backTownName { get; set; }
        /// <summary>
        /// 退货人姓名（指定退货地址退货场景必填）；最大长度100
        /// </summary>
        public string backName { get; set; }
        /// <summary>
        /// 退货人市名称（直辖市填写区名称）（指定退货地址退货场景必填）；最大长度100
        /// </summary>
        public string backCityName { get; set; }
        /// <summary>
        /// 退货人座机号（和退货人手机号二选一必填）（指定退货地址退货场景必填）；最大长度100
        /// </summary>
        public string backPhone { get; set; }
        /// <summary>
        /// 退货人省名称（直辖市填写市名称）（指定退货地址退货场景必填）；最大长度100
        /// </summary>
        public string backProvinceName { get; set; }
        /// <summary>
        /// 退货人区/县名称（直辖市填写街道名称）（指定退货地址退货场景必填）；最大长度100
        /// </summary>
        public string backCountyName { get; set; }
        /// <summary>
        /// 退货人详细地址名称（指定退货地址退货场景必填）；最大长度100
        /// </summary>
        public string backDetailAddress { get; set; }
    }
}
