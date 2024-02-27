using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsCore.NewEMS
{
    /// <summary>
    /// 下单取号接口参数类
    /// </summary>
    public class CreateOrderInterface : LogisticsInterfaceBase
    {
        /// <summary>
        /// * 创建时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// * 电商客户标识: 这个字段可以填一个小于50位随机数
        /// </summary>
        public string EcommerceUserId { get; set; }
        /// <summary>
        /// 协议客户号: 由邮政分配的客户号
        /// </summary>
        public string SenderNo { get; set; }
        /// <summary>
        /// * 物流订单号: 客户内部订单号(ERPID?)
        /// </summary>
        public string LogisticsOrderNo { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string WaybillNo { get; set; }
        /// <summary>
        /// 一票多件标识: 0正常, 1一票多件
        /// </summary>
        public int OneBillFlag { get; set; }
        /// <summary>
        /// 一票多件数量: 最大"一主九子"，即submailNo 小于等于 9
        /// </summary>
        public int OneBillNum { get; set; }
        /// <summary>
        /// 子单号: 一票多单子单号，以 “,” (半角逗号)分隔， 非一票多单不填
        /// </summary>
        public string SubMailNo { get; set; }
        /// <summary>
        /// 一票多件计费方式: 1主单统一计费, 2分单免首重计费, 3平均重量计费, 4主分单单独计费
        /// </summary>
        public int OneBillFeeType { get; set; }
        /// <summary>
        /// * 内件性质(文档:1,3): 1文件, 3物品, (2信函, 4包裹)
        /// </summary>
        public int ContentsAttribute { get; set; }
        /// <summary>
        /// * 业务产品分类: 1特快专递, 2快递包裹, 3特快到付, 9国内即日, 10电商标快, 11国内标快
        /// </summary>
        public string BizProductNo { get; set; }
        /// <summary>
        /// 订单重量: 单位: 克
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// 订单体积: 单位: 立方厘米(Double (8,3))
        /// </summary>
        public double Volume { get; set; }
        /// <summary>
        /// 订单长度: 单位: 厘米(Double (8,3))
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// 订单宽度: 单位: 厘米(Double (8,3))
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 订单高度: 单位: 厘米(Double (8,3))
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 邮费: 单位: 元(Double (12,3))
        /// </summary>
        public double PostageTotal { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string PickupNotes { get; set; }
        /// <summary>
        /// 保价标志(文档1,2): 1基本, 2保价, 3保险
        /// </summary>
        public int InsuranceFlag { get; set; }
        /// <summary>
        /// 保价金额: 单位: 元(Double (12,3))
        /// </summary>
        public double InsuranceAmount { get; set; }
        /// <summary>
        /// 保险金额: 单位: 元(Double (12,3))
        /// </summary>
        public double InsurancePremiumAmount { get; set; }
        /// <summary>
        /// 投递方式: 1客户自提, 2上门投递, 3智能包裹柜, 4网点代投
        /// </summary>
        public int DeliverType { get; set; } = 2;
        /// <summary>
        /// 投递预约开始时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string DeliverPreDate { get; set; }
        /// <summary>
        /// 投递预约结束时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string DeliverPreEndDate { get; set; }
        /// <summary>
        /// 揽收方式：0客户送货上门，1机构上门揽收
        /// </summary>
        public int PickupType { get; set; }
        /// <summary>
        /// 揽收预约起始时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string PickupPreBeginTime { get; set; }
        /// <summary>
        /// 揽收预约截止时间: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string PickupPreEndTime { get; set; }
        /// <summary>
        /// 付款方式(文档:1,2,4): 1寄件人, 2收件人, 4收件人集中付费, (3第三方, 5免费, 6寄/收件人, 7预付卡)
        /// </summary>
        public int PaymentMode { get; set; }
        /// <summary>
        /// 代收款标志: 1代收货款, 2投递代, (9:无)
        /// </summary>
        public int CodFlag { get; set; }
        /// <summary>
        /// 代收款金额: 单位: 元(Double (8,3))
        /// </summary>
        public string CodAmount { get; set; }
        /// <summary>
        /// 回单标识(文档:6:实物返单): 1基本, 2回执, 3短信, 5电子返单, 6格式返单, 7自备返单, 8反向返单
        /// </summary>
        public string ReceiptFlag { get; set; }
        /// <summary>
        /// 回单运单号
        /// </summary>
        public string ReceiptWaybillNo { get; set; }
        /// <summary>
        /// 电子优惠券号
        /// </summary>
        public string ElectronicPreferentialNo { get; set; }
        /// <summary>
        /// 优惠券金额: 单位: 元
        /// </summary>
        public string ElectronicPreferentialAmount { get; set; }
        /// <summary>
        /// 贵品标识: 0无, 1有
        /// </summary>
        public int ValuableFlag { get; set; }
        /// <summary>
        /// 寄件人安全码
        /// </summary>
        public string SenderSafetyCode { get; set; }
        /// <summary>
        /// 收件人安全码
        /// </summary>
        public string ReceiverSafetyCode { get; set; }
        /// <summary>
        /// 备注(针对公安交管邮件)
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 项目标识: 山西公安户籍（ SXGAHJ）, 公安网上车管（GAWSCG）, 苹果（APPLE）
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 特安标识: 特殊安全保障性产品需单独签署协议确认, 0否, 1是
        /// </summary>
        public int TeanIncrementFlag { get; set; }
        /// <summary>
        /// 虚拟号标识: 0不使用虚拟号, 1使用虚拟号, 默认: 不使用虚拟号
        /// </summary>
        public string VirtualNumFlag { get; set; }
        /// <summary>
        /// * 寄件人信息（节点名称 sender）
        /// </summary>
        public AddressModel Sender { get; set; }
        /// <summary>
        /// * 收件人信息（节点名称 receiver）
        /// </summary>
        public AddressModel Receiver { get; set; }
        public List<Cargo> Cargos { get; set; }
    }

    /// <summary>
    /// 地址对象Model
    /// </summary>
    public class AddressModel
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户邮编
        /// </summary>
        public string PostCode { get; set; }
        /// <summary>
        /// 用户电话，包括区号、电话号码及分机号，中间用“-”分隔；
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 用户移动电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 用户所在省，使用国标全称
        /// </summary>
        public string Prov { get; set; }
        /// <summary>
        /// 用户所在市，使用国标全称
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 用户所在县（区），使用国标全称
        /// </summary>
        public string County { get; set; }
        /// <summary>
        /// 用户详细地址
        /// </summary>
        public string Address { get; set; }
    }

    /// <summary>
    /// Cargo对象Model
    /// </summary>
    public class Cargo
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string CargoName { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public string CargoCategory { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public string CargoQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CargoValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CargoWeight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CargoMailNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Length { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string High { get; set; }
    }
}
