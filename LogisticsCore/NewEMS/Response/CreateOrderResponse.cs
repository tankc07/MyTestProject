using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsCore.NewEMS.Response
{
    public class CreateOrderResponse:NewEmsResponseBase
    {
        #pragma warning disable IDE1006 // 命名样式
        public CreateOrderResponseBody retBody { get; set; }
        #pragma warning restore IDE1006 // 命名样式
    }
}
