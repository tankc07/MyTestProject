namespace LogisticsCore.NewEMS.Model
{
    /// <summary>
    /// 地址对象Model
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
    public class AddressModel
    {
        /// <summary>
        /// 用户姓名
        /// </summary>

        public string name { get; set; }
        /// <summary>
        /// 用户邮编
        /// </summary>
        public string postCode { get; set; }
        /// <summary>
        /// 用户电话，包括区号、电话号码及分机号，中间用“-”分隔；
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 用户移动电话
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 用户所在省，使用国标全称
        /// </summary>
        public string prov { get; set; }
        /// <summary>
        /// 用户所在市，使用国标全称
        /// </summary>

        public string city { get; set; }
        /// <summary>
        /// 用户所在县（区），使用国标全称
        /// </summary>
        public string county { get; set; }
        /// <summary>
        /// 用户详细地址
        /// </summary>
        public string address { get; set; }
    }
}
