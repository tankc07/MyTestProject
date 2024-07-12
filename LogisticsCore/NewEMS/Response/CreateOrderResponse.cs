using System;
using Newtonsoft.Json;

namespace LogisticsCore.NewEMS.Response
{
    public class CreateOrderResponse : NewEmsResponseBase
    {
#pragma warning disable IDE1006 // 命名样式
        private string _retBody;
        public string retBody
        {
            get => _retBody;
            set
            {
                _retBody = value;
                _retBodyObj = null;
            }
        }
        private CreateOrderResponseBody _retBodyObj;
        public CreateOrderResponseBody retBodyObj
        {
            get
            {
                if (_retBodyObj == null && _retBody != null)
                {
                    try
                    {
                        _retBodyObj = JsonConvert.DeserializeObject<CreateOrderResponseBody>(retBody);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($@"retBody值, Json转换异常, Exception: {e}");
                        return null;
                    }
                }
                return _retBodyObj;
            }
        }
#pragma warning restore IDE1006 // 命名样式
    }
}
