using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsCore.NewEMS
{
    public class NewEmsLog
    {
        public string datetime { get; set; }
        public string responseContent { get; set; }
        public string contentToDynamicObj { get; set; }
        public string dynamicObjToJsonText { get; set; }

        public string convertDynamicJsonToCreateOrderResponse { get; set; }
        public string responseError { get; set; }
        public string contentNull { get; set; }
        public string catchErr { get; set; }
    }
}
