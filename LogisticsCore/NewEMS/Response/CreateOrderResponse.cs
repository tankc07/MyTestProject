using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LogisticsCore.NewEMS.Response
{
    public class CreateOrderResponse:NewEmsResponseBase
    {
        #pragma warning disable IDE1006 // 命名样式
        public CreateOrderResponseBody retBodyObj
        {
            get
            {
                if (retBody == null) return null;
                try
                {
                    return JsonConvert.DeserializeObject<CreateOrderResponseBody>(retBody);
                }
                catch
                {
                    return null;
                }
            }
        }
        #pragma warning restore IDE1006 // 命名样式
    }
}
