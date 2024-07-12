namespace LogisticsCore.JingDong.Request
{
    /// <summary>
    /// 下单前校验 可以请求到站点 路由号等相关信息
    /// </summary>
    public class RangeCheckDeliveryQueryApiRequest
    {
        /// <summary>
        /// 发货仓编码（发货仓编码、寄件地址、寄件人省市id三者必须有一个）；最大长度20
        /// </summary>
        public string warehouseCode { get; set; }
        /// <summary>
        /// 派送站点编码；最大长度20
        /// </summary>
        public string siteCode { get; set; }
        /// <summary>
        /// * 商家订单号；最大长度50
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// * 商家编号；最大长度50
        /// </summary>
        public string customerCode { get; set; }
        /// <summary>
        /// 站点名称；最大长度200
        /// </summary>
        public string siteName { get; set; }
        /// <summary>
        /// 产品类型 1-特惠送，2-特快送，4-特瞬送城际，5-同城当日达，6-特快次晨，7-微小件，8-生鲜专送，16-生鲜速达，17-生鲜惠达，21-特惠小包；最大长度10
        /// </summary>
        public int? promiseTimeType { get; set; }
        /// <summary>
        /// * 配送业务类型；1:普通，3:填仓，4:特配，5:鲜活，6:控温，7:冷藏，8:冷冻，9:深冷，默认是1；最大长度4
        /// </summary>
        public int goodsType { get; set; }
        /// <summary>
        /// 发货时间（ YYYY-MM-DD HH:MM:SS） ；最大长度20
        /// </summary>
        public string sendTime { get; set; }
        /// <summary>
        /// 销售平台；最大长度50
        /// </summary>
        public string salePlatform { get; set; }
        /// <summary>
        /// 是否货到付款；1：货到付款，0：在线支付，默认为1；最大长度4
        /// </summary>
        public int? isCode { get; set; }
        /// <summary>
        /// 增值服务信息
        /// </summary>
        public AddedServiceRequest addedService { get; set; }
        /// <summary>
        /// 揽收站点编码；最大长度20
        /// </summary>
        public string pickupSiteCode { get; set; }
        /// <summary>
        /// 派件预分拣是否优先匹配国际疫情自提点；1是 0否 ；最大长度4
        /// </summary>
        public int? requireDeliveryPresortMode { get; set; }
        /// <summary>
        /// 揽收站点id；最大长度20
        /// </summary>
        public int? pickupSiteId { get; set; }
        /// <summary>
        /// 站点ID；最大长度20
        /// </summary>
        public int? siteId { get; set; }
        /// <summary>
        /// 运输类型（1：陆运,2：航空,3:高铁）；最大长度4
        /// </summary>
        public int? transportType { get; set; }
        /// <summary>
        /// * 收件人信息
        /// </summary>
        public ReceiverContactRequest receiverContactRequest { get; set; }
        /// <summary>
        /// 结算方式 0月结 1到付 2寄付；最大长度4
        /// </summary>
        public int? settleType { get; set; }
        /// <summary>
        /// 寄件人信息
        /// </summary>
        public SenderContactRequest senderContactRequest { get; set; }
    }

    /// <summary>
    /// 增值服务 下单前置校验 请求中的类型
    /// </summary>
    public class AddedServiceRequest
    {
        /// <summary>
        /// 特殊签收方式；最大长度4; 0、不需要此服务（默认） 2、短信验证3、身份证验证4、合并签收6、短信验证两条短信（联通华盛）7、司法专邮签收（签收\拒收前必须拍照）8、签收前激活（电信卡）
        /// 9、物业代收10、门卫代收11、小区门口自取12、指定地点存放13、采集身份信息14、签收验证码验证（验证码下单时通过指定签收增值服务产品下传）
        /// </summary>
        public int? signType { get; set; }
        /// <summary>
        /// 包装服务；最大长度4；0、不需要1、需要
        /// </summary>
        public int? packageService { get; set; }
        /// <summary>
        /// 派送运输方式；最大长度4;0、不需要此服务（默认 C网） 1、城市配送（B网）2、众包配送3、使用TMS运输揽收派送9、可B可C
        /// </summary>
        public int? deliveryType { get; set; }
        /// <summary>
        /// 揽收运输方式；最大长度4;0、 C网1、城市配送（B网）9、可B可C
        /// </summary>
        public int? pupDeliveryType { get; set; }
        /// <summary>
        /// 京尊达服务；最大长度4;0、不需要此服务（默认） 1、启用京尊达服务
        /// </summary>
        public int? qualityService { get; set; }
        /// <summary>
        /// 生鲜专送；最大长度4;0、不需要1、生鲜专送
        /// </summary>
        public int? freshDelivery { get; set; }
        /// <summary>
        /// 送取同步服务；最大长度4;0、不需要此服务（默认）1、派送同时取件场景（送取同步，上门换新）先妥投后取件对应的派送运单2、派送同时取件场景（送取同步，上门换新）先取件后妥投对应的派送运单
        /// 3、派送同时取件场景（送取同步，上门换新）先妥投后取件对应的取件运单4、派送同时取件场景（送取同步，上门换新）先取件后妥投对应的取件运单
        /// </summary>
        public int? sendAndPickupType { get; set; }
        /// <summary>
        /// 正逆向服务；最大长度4;0、正向下单（默认）1、逆向下单
        /// </summary>
        public int? reverseType { get; set; }
        /// <summary>
        /// 揽件防撕码；最大长度4;0、不需要此服务（默认）1、需要此服务
        /// </summary>
        public int? tearProofCodeType { get; set; }
        /// <summary>
        /// 结算方式；最大长度4;0、默认，商家月结 1、到付现结 2、寄付现结 3、寄付临欠 5、向多方收费
        /// </summary>
        public int? settleType { get; set; }
        /// <summary>
        /// 抢单服务；最大长度4;0、正常单子（默认）1、抢单
        /// </summary>
        public int? preemptionType { get; set; }
        /// <summary>
        /// 送货入仓；最大长度4;0：不需要1：需要
        /// </summary>
        public int? sendToWarehouse { get; set; }
        /// <summary>
        /// 运输产品类型；最大长度4;1：整车业务，2：纯配快运零担，3：仓配零担4：冷链整车5：冷链纯配快运零担6：TC零担7：TC整车
        /// </summary>
        public int? transProductType { get; set; }
        /// <summary>
        /// 装车卸车；最大长度4;1.仅装车2.仅卸车3.装+卸仅限整车
        /// </summary>
        public int? loadingType { get; set; }
        /// <summary>
        /// 是否要隔离运输；最大长度4;1-清真2-易污染3-清真+易污染
        /// </summary>
        public int? goodsIsolateType { get; set; }
        /// <summary>
        /// 是否需要给商家重货上楼；最大长度4;0-不需要1-需要
        /// </summary>
        public int? deliveryUpstairs { get; set; }
        /// <summary>
        /// 取消前确认；最大长度4;0-不需要1-需要
        /// </summary>
        public int? cancelConfirmType { get; set; }
        /// <summary>
        /// 快运产品类型；最大长度4;0、默认1、经济快运2、精准快运3、标准达4、即时配5、同城直发6、城配7、卡班8、专车
        /// 9、特快重货10、大票直达11、(已占用)12、航空重货（占用但未使用此字段进行判断，通过productType进行判断的为了兼容使用）
        /// </summary>
        public int? expressProductType { get; set; }
        /// <summary>
        /// 派送端产品类型；最大长度4;1、需要从分拣中心发送给终端或者用户2、用户到分拣中心提货，不需要送货到门3、需要从空港或火车站发送给终端或者用户
        /// 4、用户到空港或者火车站提货，不需要送货到门5、仓配一体业务，用户到京东仓自提
        /// </summary>
        public int? deliverProductType { get; set; }
        /// <summary>
        /// 危险品标识；最大长度4;0-否1-普通危险品其他枚举具体值待定
        /// </summary>
        public int? hazardSymbols { get; set; }
        /// <summary>
        /// 收件人微笑面单；最大长度4;0、不隐藏1、隐藏姓名 2、隐藏电话3、隐藏姓名 + 隐藏电话 4、隐藏地址 5、隐藏姓名 + 隐藏地址6、隐藏电话 + 隐藏地址 7、隐藏全部8、商家无诉求
        /// </summary>
        public int? receiveSmileType { get; set; }
        /// <summary>
        /// 下单方式；最大长度4;0:单个下单 1:好友寄2:扫码下单3:语音寄4:批量寄5:无忧寄6:寄物资绿色通道7:云柜8:小V寄9:预制条码下单10:扫码（该码绑定了收件人信息）寄--未上线
        /// </summary>
        public int? inputOrderType { get; set; }
        /// <summary>
        /// 揽收(时)采集商品明细；最大长度40：不采集(默认)1：采集（橙联专用）2：采集
        /// </summary>
        public int? collectGoodsInfoPickup { get; set; }
        /// <summary>
        /// 寄件人微笑面单；最大长度4;0、不隐藏1、隐藏姓名2、隐藏电话3、隐藏姓名 + 隐藏电话4、隐藏地址5、隐藏姓名 + 隐藏地址6、隐藏电话 + 隐藏地址7、隐藏全部8、商家无诉求
        /// </summary>
        public int? senderSmileType { get; set; }
        /// <summary>
        /// 京尊取；最大长度4；1：使用京尊取；不使用不要传
        /// </summary>
        public int? onTimeService { get; set; }
        /// <summary>
        /// 京准取；最大长度4；1：使用京准取；不使用不要传
        /// </summary>
        public int? pickupInTime { get; set; }
    }

    /// <summary>
    /// 寄件人信息 下单前置校验 请求中的类型
    /// </summary>
    public class SenderContactRequest
    {
        /// <summary>
        /// 寄件人省编码；最大长度100
        /// </summary>
        public int? senderProvince { get; set; }
        /// <summary>
        /// 寄件人市编码；最大长度100
        /// </summary>
        public int? senderCity { get; set; }
        /// <summary>
        /// 寄件人县编码；最大长度100
        /// </summary>
        public int? senderCounty { get; set; }
        /// <summary>
        /// 寄件人镇编码；最大长度100
        /// </summary>
        public int? senderTown { get; set; }
        /// <summary>
        /// 寄件地址；最大长度350
        /// </summary>
        public string senderAddress { get; set; }
        /// <summary>
        /// 寄件人省；最大长度100
        /// </summary>
        public string senderProvinceName { get; set; }
        /// <summary>
        /// 寄件人市；最大长度100
        /// </summary>
        public string senderCityName { get; set; }
        /// <summary>
        /// 寄件人县；最大长度100
        /// </summary>
        public string senderCountyName { get; set; }
        /// <summary>
        /// 寄件人镇；最大长度100
        /// </summary>
        public string senderTownName { get; set; }
    }

    /// <summary>
    /// * 收件人信息 下单前置校验 请求中的类型
    /// </summary>
    public class ReceiverContactRequest
    {
        /// <summary>
        /// * 收件人地址，说明：不能为生僻字，请填写省市区县详细地址；最大长度350
        /// </summary>
        public string receiverAddress { get; set; }
        /// <summary>
        /// 收件人市；最大长度100
        /// </summary>
        public string receiverCityName { get; set; }
        /// <summary>
        /// 收件人省编码；最大长度100
        /// </summary>
        public int? receiverProvince { get; set; }
        /// <summary>
        /// 收件人县编码；最大长度100
        /// </summary>
        public int? receiverCounty { get; set; }
        /// <summary>
        /// 收件人镇；最大长度100
        /// </summary>
        public string receiverTownName { get; set; }
        /// <summary>
        /// 收件人市编码；最大长度100
        /// </summary>
        public int? receiverCity { get; set; }
        /// <summary>
        /// 收件人镇编码；最大长度100
        /// </summary>
        public int? receiverTown { get; set; }
        /// <summary>
        /// 收件人省；最大长度100
        /// </summary>
        public string receiverProvinceName { get; set; }
        /// <summary>
        /// 收件人县；最大长度100
        /// </summary>
        public string receiverCountyName { get; set; }
    }
}
