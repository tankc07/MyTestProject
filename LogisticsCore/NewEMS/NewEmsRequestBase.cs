using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsCore.NewEMS
{
    /// <summary>
    /// 公共参数请求基类
    /// </summary>
    public class NewEmsRequestBase
    {
        /// <summary>
        /// * 接口代码: 020003-订单接入(下单取号), 020006-订单取消(取消下单)
        /// </summary>
        public string ApiCode { get; set; }
        /// <summary>
        /// * 客户代码(即协议客户号)
        /// </summary>
        public string SenderNo { get; set; }
        /// <summary>
        /// * 授权码（ 区分测试和生产 ）
        /// </summary>
        public string Authorization { get; set; }

        /// <summary>
        /// 0-json 1-xml, 默认: 0-json
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 请求时间: yyyy-MM-dd HH:mm:ss </summary>
        public string TimeStamp { get; set; }
        /// <summary>
        /// 版本号: 默认V1.0.0
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// * 请求消息体(用于请求时加密接口参数)
        /// </summary>
        public LogisticsInterfaceBase LogisticsInterface { get; set; }
        /// <summary>
        /// 用户编码 (API控制台测试时,传的是测试协议用户编码:CJOASWRKT)
        /// </summary>
        public string UserCode { get; set; }
    }
}
