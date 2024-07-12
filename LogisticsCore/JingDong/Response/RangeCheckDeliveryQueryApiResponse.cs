using System.Collections.Generic;

namespace LogisticsCore.JingDong.Response
{
    /// <summary>
    /// 下单前置校验 响应结果
    /// </summary>
    public class RangeCheckDeliveryQueryApiResponse : FreshMedicineDeliveryResponseBase
    {
        public RangeCheckDeliveryQueryApiResponseBody data { get; set; }
        public bool HasData => data != null;
    }

    /// <summary>
    /// 下单前置校验请求返回参数中的Data实体
    /// </summary>
    public class RangeCheckDeliveryQueryApiResponseBody
    {
        /// <summary>
        /// 滑道号
        /// </summary>
        public string targetCrossCode { get; set; }
        /// <summary>
        /// 面单集货地名称（面单集包地显示值）
        /// </summary>
        public string collectionAddress { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string siteName { get; set; }
        /// <summary>
        /// 校验错误码集合
        /// </summary>
        public List<ValidationResult> validationResultList { get; set; }
        /// <summary>
        /// 预分拣状态值
        /// </summary>
        public int? preSortCode { get; set; }
        /// <summary>
        /// 分拣中心代码
        /// </summary>
        public string distributeCode { get; set; }
        /// <summary>
        /// 是否隐藏联系方式
        /// </summary>
        public int? isHideContractNumbers { get; set; }
        /// <summary>
        /// 是否隐藏姓名
        /// </summary>
        public int? isHideName { get; set; }
        /// <summary>
        /// 路区
        /// </summary>
        public string road { get; set; }
        /// <summary>
        /// 始发分拣中心ID
        /// </summary>
        public int? originalSortCenterId { get; set; }
        /// <summary>
        /// 始发分拣中心名称
        /// </summary>
        public string originalSortCenterName { get; set; }
        /// <summary>
        /// 目的分拣中心
        /// </summary>
        public string targetSortCenterName { get; set; }
        /// <summary>
        /// 时效名称
        /// </summary>
        public string agingName { get; set; }
        /// <summary>
        /// 站点类型
        /// </summary>
        public int? siteType { get; set; }
        /// <summary>
        /// 特快送运营模式 
        /// </summary>
        public int? expressOperationMode { get; set; }
        /// <summary>
        /// 二维码转换链接
        /// </summary>
        public string qrcodeUrl { get; set; }
        /// <summary>
        /// 始发笼车号
        /// </summary>
        public string originalTabletrolleyCode { get; set; }
        /// <summary>
        /// 目的分拣中心
        /// </summary>
        public int? targetSortCenterId { get; set; }
        /// <summary>
        /// 物流模式为空（0 - 快递承载 1 - 快运B承载）
        /// </summary>
        public int? deliveryType { get; set; }
        /// <summary>
        /// 时效产品
        /// </summary>
        public int? promiseTimeType { get; set; }
        /// <summary>
        /// 目的笼车号
        /// </summary>
        public string targetTabletrolleyCode { get; set; }
        /// <summary>
        /// 时效产品是否降级
        /// </summary>
        public bool? promiseTimeTypeDownGrade { get; set; }
        /// <summary>
        /// 温层
        /// </summary>
        public int? goodsType { get; set; }
        /// <summary>
        /// 运输类型
        /// </summary>
        public int? transType { get; set; }
        /// <summary>
        /// 配送机构
        /// </summary>
        public int? siteId { get; set; }
        /// <summary>
        /// 时效
        /// </summary>
        public int? aging { get; set; }
        /// <summary>
        /// 代表时效产品简码
        /// </summary>
        public string coverCode { get; set; }
        /// <summary>
        /// 始发道口号
        /// </summary>
        public string originalCrossCode { get; set; }
        /// <summary>
        /// 卡位号
        /// </summary>
        public string truckSpot { get; set; }
    }

    /// <summary>
    /// 校验错误码实体
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string message { get; set; }
    }
}
