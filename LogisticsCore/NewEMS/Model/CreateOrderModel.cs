using System;
using System.Collections.Generic;

namespace LogisticsCore.NewEMS.Model
{
    /// <summary>
    /// 下单取号接口参数类
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
    public class CreateOrderModel : LogisticsInterfaceBase
    {
        /// <summary>
        /// * 创建时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string createdTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// * 电商客户标识: 这个字段可以填一个小于50位随机数
        /// </summary>
        public string ecommerceUserId { get; set; }
        /// <summary>
        /// 协议客户号: 由邮政分配的客户号
        /// </summary>
        public string senderNo { get; set; }
        /// <summary>
        /// * 物流订单号: 客户内部订单号(ERPID?)
        /// </summary>
        public string logisticsOrderNo { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string batchNo { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string waybillNo { get; set; }
        /// <summary>
        /// 一票多件标识: 0正常, 1一票多件
        /// </summary>
        public string oneBillFlag { get; set; }
        /// <summary>
        /// 一票多件数量: 最大"一主九子"，即submailNo 小于等于 9
        /// </summary>
        public string oneBillNum { get; set; }
        /// <summary>
        /// 子单号: 一票多单子单号，以 “,” (半角逗号)分隔， 非一票多单不填
        /// </summary>
        public string subMailNo { get; set; }
        /// <summary>
        /// 一票多件计费方式: 1主单统一计费, 2分单免首重计费, 3平均重量计费, 4主分单单独计费
        /// <para>参照旧接口未使用一票多件可填 0</para>
        /// </summary>
        public string oneBillFeeType { get; set; }
        /// <summary>
        /// * 内件性质(文档:1,3): 1文件, 3物品, (2信函, 4包裹)
        /// </summary>
        public string contentsAttribute { get; set; }
        /// <summary>
        /// * 业务产品分类: 1特快专递, 2快递包裹, 3特快到付, 9国内即日, 10电商标快, 11国内标快
        /// </summary>
        public string bizProductNo { get; set; }
        /// <summary>
        /// 订单重量: 单位: 克
        /// </summary>
        public string weight { get; set; }
        /// <summary>
        /// 订单体积: 单位: 立方厘米(double (8,3))
        /// 此接口所有值类型都可以传递字符串,但是字符串的值必须是数字
        /// </summary>
        public string volume { get; set; }
        /// <summary>
        /// 订单长度: 单位: 厘米(double (8,3))
        /// </summary>
        public string length { get; set; }
        /// <summary>
        /// 订单宽度: 单位: 厘米(double (8,3))
        /// </summary>
        public string width { get; set; }
        /// <summary>
        /// 订单高度: 单位: 厘米(double (8,3))
        /// </summary>
        public string height { get; set; }
        /// <summary>
        /// 邮费: 单位: 元(string (12,3))
        /// </summary>
        public string postageTotal { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string pickupNotes { get; set; }
        /// <summary>
        /// 保价标志(文档1,2): 1基本, 2保价, 3保险
        /// </summary>
        public string insuranceFlag { get; set; }
        /// <summary>
        /// 保价金额: 单位: 元(string (12,3))
        /// </summary>
        public string insuranceAmount { get; set; }
        /// <summary>
        /// 保险金额: 单位: 元(string (12,3))
        /// </summary>
        public string insurancePremiumAmount { get; set; }
        /// <summary>
        /// 投递方式: 1客户自提, 2上门投递, 3智能包裹柜, 4网点代投
        /// </summary>
        public string deliverType { get; set; }
        /// <summary>
        /// 投递预约开始时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string deliverPreDate { get; set; }
        /// <summary>
        /// 投递预约结束时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string deliverPreEndDate { get; set; }
        /// <summary>
        /// 揽收方式：0客户送货上门，1机构上门揽收
        /// </summary>
        public string pickupType { get; set; }
        /// <summary>
        /// 揽收预约起始时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string pickupPreBeginTime { get; set; }
        /// <summary>
        /// 揽收预约截止时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string pickupPreEndTime { get; set; }
        /// <summary>
        /// 付款方式(文档:1,2,4): 1寄件人, 2收件人, 4收件人集中付费, (3第三方, 5免费, 6寄/收件人, 7预付卡)
        /// </summary>
        public string paymentMode { get; set; }
        /// <summary>
        /// 代收款标志: 1代收货款, 2投递代, 9:无
        /// </summary>
        public string codFlag { get; set; }
        /// <summary>
        /// 代收款金额: 单位: 元(double (8,3))
        /// </summary>
        public string codAmount { get; set; }
        /// <summary>
        /// 回单标识(文档:6:实物返单): 1基本, 2回执, 3短信, 5电子返单, 6格式返单, 7自备返单, 8反向返单
        /// </summary>
        public string receiptFlag { get; set; }
        /// <summary>
        /// 回单运单号
        /// </summary>
        public string receiptWaybillNo { get; set; }
        /// <summary>
        /// 电子优惠券号
        /// </summary>
        public string electronicPreferentialNo { get; set; }
        /// <summary>
        /// 优惠券金额: 单位: 元
        /// </summary>
        public string electronicPreferentialAmount { get; set; }
        /// <summary>
        /// 贵品标识: 0无, 1有
        /// </summary>
        public string valuableFlag { get; set; }
        /// <summary>
        /// 寄件人安全码
        /// </summary>
        public string senderSafetyCode { get; set; }
        /// <summary>
        /// 收件人安全码
        /// </summary>
        public string receiverSafetyCode { get; set; }
        /// <summary>
        /// 备注(针对公安交管邮件)
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 项目标识: 山西公安户籍（ SXGAHJ）, 公安网上车管（GAWSCG）, 苹果（APPLE）
        /// </summary>
        public string projectId { get; set; }
        /// <summary>
        /// 特安标识: 特殊安全保障性产品需单独签署协议确认, 0否, 1是
        /// </summary>
        public string teanIncrementFlag { get; set; }
        /// <summary>
        /// 虚拟号标识: 0不使用虚拟号, 1使用虚拟号, 默认: 不使用虚拟号
        /// </summary>
        public string virtualNumFlag { get; set; }
        /// <summary>
        /// * 寄件人信息（节点名称 sender）
        /// </summary>
        public AddressModel sender { get; set; }
        /// <summary>
        /// * 收件人信息（节点名称 receiver）
        /// </summary>
        public AddressModel receiver { get; set; }
        /// <summary>
        /// * 商品信息/内件集合（节点名称 cargos)
        /// </summary>
        public List<CargoModel> cargos { get; set; }
    }




}
