using LogisticsCore.NewEMS.Model;

namespace LogisticsCore.NewEMS.Request
{
    /// <summary>
    /// 公共参数请求基类
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
    public class NewEmsRequestBase
    {
        
        /// <summary>
        /// * 接口代码: 020003-订单接入(下单取号), 020006-订单取消(取消下单)
        /// </summary>
        public string apiCode { get; set; }
        /// <summary>
        /// * 客户代码(即协议客户号)
        /// </summary>
        public string senderNo { get; set; }
        /// <summary>
        /// * 授权码（ 区分测试和生产 ）
        /// </summary>
        public string authorization { get; set; }

        /// <summary>
        /// 0-json 1-xml, 默认: 0-json
        /// </summary>
        public string msgType { get; set; }
        /// <summary>
        /// * 请求时间: yyyy-MM-dd HH:mm:ss </summary>
        public string timeStamp { get; set; }
        /// <summary>
        /// 版本号: 默认V1.0.0
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// * 请求消息体(用于请求时加密接口参数)
        /// <para>可接受<see cref="CreateOrderModel"/>和<see cref="CancelOrderModel"/>两种类型Model</para>
        /// </summary>
        public LogisticsInterfaceBase logitcsInterface { get; set; }
        /// <summary>
        /// 用户编码 (API控制台测试时,传的是测试协议用户编码:CJOASWRKT)
        /// </summary>
        public string userCode { get; set; }
        
    }
}
