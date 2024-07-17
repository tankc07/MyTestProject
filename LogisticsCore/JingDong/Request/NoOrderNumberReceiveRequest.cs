using System.Collections.Generic;
using LogisticsCore.JingDong.Model;

namespace LogisticsCore.JingDong.Request
{
    /// <summary>
    /// 无单号下单请求实体
    /// </summary>
    public class NoOrderNumberReceiveRequest
    {
        /// <summary>
        /// 开箱验货标识（1：随心验(收费)，2：开商品包装验货，3：开物流包装验货，4：不支持开箱验货）；最大长度4
        /// </summary>
        public string unpackingInspection { get; set; }

        /// <summary>
        /// 运单类型(普通外单：0，O2O外单：1) 默认为0；最大长度4
        /// </summary>
        public int? orderType { get; set; }

        /// <summary>
        /// * 商家订单号，请保证商家编码下唯一，最大长度50 
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 运费；最大长度28，单位元，保留两位小数
        /// </summary>
        public double? freight { get; set; }

        /// <summary>
        /// * 寄件人信息
        /// </summary>
        public SenderContactModel senderContactRequest { get; set; }

        /// <summary>
        /// * 商家编码
        /// </summary>
        public string customerCode { get; set; }

        /// <summary>
        /// 站点名称；最大长度200
        /// </summary>
        public string siteName { get; set; }

        /// <summary>
        /// 运单备注，长度：20,说明：打印面单时备注内容也会显示在快递面单上；最大长度20
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 预约配送时间（格式：yyyy-MM-dd HH:mm:ss）；最大长度20
        /// </summary>
        public string orderSendTime { get; set; }

        /// <summary>
        /// 身份证号；最大长度50
        /// </summary>
        public string idNumber { get; set; }

        /// <summary>
        /// 销售渠道；最大长度50； 274：天猫，278：拼多多
        /// </summary>
        public int? salesChannel { get; set; }

        /// <summary>
        /// 发货仓编码；最大长度50
        /// </summary>
        public string warehouseCode { get; set; }

        /// <summary>
        /// 取件方式（填写中文：【上门收货】或【自送】，不填写：商家无诉求，此字段无意义）；最大长度50
        /// </summary>
        public string pickMethod { get; set; }

        /// <summary>
        /// * 销售平台（0010001京东商城；0010002天猫、淘宝订单；0030001其他平台）；最大长度50
        /// </summary>
        public string salePlatform { get; set; }

        /// <summary>
        /// 是否代收货款(是：1，否：0，不填或者超出范围，默认是0)；最大长度4
        /// </summary>
        public int? isCod { get; set; }

        /// <summary>
        /// * 物品信息
        /// </summary>
        public CargoesModel cargoesRequest { get; set; }

        /// <summary>
        /// 退货人信息
        /// </summary>
        public BackContactModel backContactRequest { get; set; }

        /// <summary>
        /// 配送结束时间（格式：yyyy-MM-dd HH:mm:ss）；最大长度20
        /// </summary>
        public string shipmentEndTime { get; set; }

        /// <summary>
        /// 函速达的文件地址，如果pickMethod是上门收货，则此字段可以不填；最大长度300
        /// </summary>
        public string fileUrl { get; set; }

        /// <summary>
        /// * 收件人信息
        /// </summary>
        public ReceiverContactModel receiverContactRequest { get; set; }

        /// <summary>
        /// 配送起始时间（格式：yyyy-MM-dd HH:mm:ss）；最大长度20
        /// </summary>
        public string shipmentStartTime { get; set; }

        /// <summary>
        /// 站点类型；最大长度20
        /// </summary>
        public int? siteType { get; set; }

        /// <summary>
        /// 销售平台订单号；如果有多个单号，用英文,间隔。例如：7898675,7898676)，注：逗号前需要填写订单号；最大长度50
        /// </summary>
        public string channelOrderId { get; set; }

        /// <summary>
        /// 门店编码(只O2O运单需要传，普通运单不需要传)；最大长度50
        /// </summary>
        public string shopCode { get; set; }

        /// <summary>
        /// 接货省ID；最大长度50
        /// </summary>
        public int? areaProvinceId { get; set; }

        /// <summary>
        /// 接货市ID；最大长度50
        /// </summary>
        public int? areaCityId { get; set; }

        /// <summary>
        /// 保价金额(单位元，保留小数点后两位)；最大长度20
        /// </summary>
        public double? guaranteeValueAmount { get; set; }

        /// <summary>
        /// 是否保价(是：1，否：0，不填或者超出范围，默认是0)；最大长度4
        /// </summary>
        public int? guaranteeValue { get; set; }

        /// <summary>
        /// 预约取件开始时间（格式：yyyy-MM-dd HH:mm:ss）；最大长度20
        /// </summary>
        public string pickUpStartTime { get; set; }

        /// <summary>
        /// * 产品类型（22：医药冷链 26：冷链专送,29医药专送）；最大长度10
        /// </summary>
        public int promiseTimeType { get; set; }

        /// <summary>
        /// 代收货款金额(单位元，保留小数点后两位)；最大长度20
        /// </summary>
        public double? receivable { get; set; }

        /// <summary>
        /// 配送业务类型（ 1:普通，2:生鲜常温，5:鲜活，6:控温，7:冷藏，8:冷冻，9:深冷；21:医药冷藏，23:医药控温，24:医药常温，25:医药冷冻，27:医药深冷）
        /// 默认是1；若是生鲜相关产品， 则填写枚举2、5、6、7、8、9，若是医药相关产品，则填写21、23、24、25、27，否则不填或填1；最大长度4
        /// </summary>
        public int goodsType { get; set; }
        /// <summary>
        /// 签单返还类型：0-不返单，1-普通返单，2-校验身份返单，3-电子签返单，4-电子返单+普通返单；最大长度4
        /// </summary>
        public int? receiptFlag { get; set; }
        /// <summary>
        /// 运输类型(陆运：1，航空：2，高铁：3， 不填默认是1)；最大长度4
        /// </summary>
        public int? transType { get; set; }
        /// <summary>
        /// 预约取件结束时间；当pickUpStartTime不为空时，该值必传，且必须晚于pickUpStartTime（格式：yyyy-MM-dd HH:mm:ss）；最大长度20
        /// </summary>
        public string pickUpEndTime { get; set; }
        /// <summary>
        /// 站点编码；最大长度20
        /// </summary>
        public int? siteId { get; set; }
        /// <summary>
        /// 时效(普通：1，工作日：2，非工作日：3，晚间：4。O2O一小时达：5。O2O定时达：6。不填或者超出范围，默认是1)；最大长度4
        /// </summary>
        public int? aging { get; set; }
        /// <summary>
        /// 增值服务(未使用)
        /// </summary>
        public AddedServiceModel addedService { get; set; }
        /// <summary>
        /// 商家箱号集合(未使用)
        /// </summary>
        public List<string> boxNoList { get; set; }
        /// <summary>
        /// 客户箱集合(未使用)
        /// </summary>
        public List<CustomerBoxListModel> customerBoxList { get; set; }

    }
}
