namespace LogisticsCore.JingDong.Model
{
    /// <summary>
    /// 增值服务(暂不用)
    /// </summary>
    public class AddedServiceModel
    {
        /// <summary>
        /// 验证签收需求身份证号key（身份证号后6位）
        /// </summary>
        public string idNumber { get; set; }
        /// <summary>
        /// 验证签收需求签收码key，填写1-8位字母数字
        /// </summary>
        public string verificationCode { get;set; }
        /// <summary>
        /// 三方运单号
        /// </summary>
        public string salesThrOrderId { get; set; }
        /// <summary>
        /// 特殊签收方式
        /// 0、不需要此服务（默认） 2、短信验证 3、身份证验证 4、合并签收
        /// 6、短信验证两条短信（联通华盛）7、司法专邮签收（签收\拒收前必须拍照）8、签收前激活（电信卡） 9、物业代收10、门卫代收
        /// 11、小区门口自取12、指定地点存放13、采集身份信息14、签收验证码验证（验证码下单时通过指定签收增值服务产品下传）
        /// </summary>
        public string signType { get; set; }
        /// <summary>
        /// 签收类型为指定签收码
        /// </summary>
        public string signCode { get; set; }
        /// <summary>
        /// "揽收运输方式 0、 C网 1、城市配送（B网）9、可B可C"
        /// </summary>
        public int? pupDeliveryType { get; set; }
        /// <summary>
        /// 派送运输方式 0、不需要此服务（默认 C网） 1、城市配送（B网）2、众包配送3、使用TMS运输揽收派送 9、可B可C
        /// </summary>
        public int? deliveryType { get; set; }
        /// <summary>
        /// 装车卸车 1、仅装车 2、仅卸车 3、装+卸仅限整车
        /// </summary>
        public int? loadingType { get; set; }
        /// <summary>
        /// 送货入仓； 0、不需要；1、需要
        /// </summary>
        public int? sendToWarehouse { get; set; }
        /// <summary>
        /// 运输产品类型 1、整车业务 2、纯配快运零担 3、仓配零担 4、冷链整车 5、冷链纯配快运零担 6、TC零担 7、TC整车
        /// </summary>
        public int? transProductType { get; set; }
        /// <summary>
        /// 结算方式； 0、月结 1、到付 2、现付 3、寄付临欠 5、向多方收费
        /// </summary>
        public int? settleType { get; set; }
        /// <summary>
        /// 条码扫描； 0、无意义 1、取件校验商品条码 2、验证部分机身码（自营瑞表
        /// </summary>
        public int? barCodeScanner { get; set; }
        /// <summary>
        /// 到付现结支付方式 0、无意义 2、仅支持二维码支付
        /// </summary>
        public int? paymentType { get; set; }
        /// <summary>
        /// 派送运输方式-用户原始诉求 0、不需要此服务（默认 C网） 1、城市配送（B网）2、众包配送 3、使用TMS运输揽收派送 9、可B可C
        /// </summary>
        public int? deliveryTypeSource { get; set; }
        /// <summary>
        /// 隐私通话（不传值时，若商家开通此服务，则补齐此服务） 1- 需要
        /// </summary>
        public int? hidePrivacyType { get; set; }
        /// <summary>
        /// 寄件地区
        /// </summary>
        public string senderRegion { get; set; }
        /// <summary>
        /// 接收地区
        /// </summary>
        public string receiveRegion { get; set; }
        /// <summary>
        /// 商家简码
        /// </summary>
        public string shortCode { get; set; }
        /// <summary>
        /// 送货入仓增值服务 0-不需要 1-需要
        /// </summary>
        public int? warehouseService { get; set; }
        /// <summary>
        /// 进仓备注
        /// </summary>
        public string enterHouseAppointmentRemark { get; set; }
        /// <summary>
        /// 进仓开始时间；YYYY-MM-DD HH:MM:SS
        /// </summary>
        public string enterHouseAppointmentStartTime { get; set; }
        /// <summary>
        /// 进仓结束时间；YYYY-MM-DD HH:MM:SS
        /// </summary>
        public string enterHouseAppointmentEndTime { get; set; }
        /// <summary>
        /// 进仓预约号
        /// </summary>
        public string enterHouseAppointmentNo { get; set; }
        /// <summary>
        /// 包装服务 0、不需要，1、需要,医药专送使用
        /// </summary>
        public int? packagingService { get; set; }
    }
}
