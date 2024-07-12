using System;
using System.Collections.Generic;
using System.Text;
using LogisticsCore.JingDong.Request;
using LogisticsCore.JingDong.Response;
using LopOpensdkDotnet;
using LopOpensdkDotnet.Filters;
using LopOpensdkDotnet.Support;
using Newtonsoft.Json;

namespace LogisticsCore.JingDong
{
    public class FreshMedicineDelivery
    {
        /// <summary>
        /// 所有实例
        /// </summary>
        protected static readonly Dictionary<string, FreshMedicineDelivery> AllList = new Dictionary<string, FreshMedicineDelivery>();

        /// <summary>
        /// 0cdc29884bde4f978c48a3be1f473946
        /// </summary>
        public string AppKey { get; }
        /// <summary>
        /// f462cc68dd9d45e6a920bf3a818d9c1b
        /// </summary>
        public string AppSecret { get; }
        /// <summary>
        /// 生产环境授权 
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 京东生鲜医药[快递]请求地址
        /// </summary>
        public string JdFreshMedicineDeliveryBaseUrl { get; }

        /// <summary>
        /// 商家编码 010K2731958
        /// </summary>
        public string CustomerCode { get; }

        private IClient _client;
        private FreshMedicineDelivery(string appKey, string appSecret, string accessToken,
            string jdFreshMedicineDeliveryBaseUrl, string customerCode)
        {
            AppKey = appKey;
            AppSecret = appSecret;
            AccessToken = accessToken;
            JdFreshMedicineDeliveryBaseUrl = jdFreshMedicineDeliveryBaseUrl;
            CustomerCode = customerCode;
            _client = new DefaultClient(jdFreshMedicineDeliveryBaseUrl);
        }

        public static FreshMedicineDelivery Init(string appKey, string appSecret, string accessToken,
            string jdFreshMedicineDeliveryBaseUrl, string customerCode)
        {
            FreshMedicineDelivery t;
            var k = appKey + appSecret + accessToken + customerCode;
            if (AllList.TryGetValue(k, out var value))
            {
                t = value;
            }
            else
            {
                t = new FreshMedicineDelivery(appKey, appSecret, accessToken, jdFreshMedicineDeliveryBaseUrl,
                    customerCode);

                AllList.Add(k, t);
            }
            return t;
        }

        public bool RemoveJdObj(string appKey, string appSecret, string accessToken, string customerCode)
        {
            var k = appKey + appSecret + accessToken + customerCode;
            if (AllList.ContainsKey(k))
            {
                AllList.Remove(k);
                return true;
            }
            return false;
        }

        public NoOrderNumberReceiveResponse CreateOrder(NoOrderNumberReceiveRequest createOrderRequest, 
            out bool isOk, out int errCode, out string errMsg, out string sendData, out string recData)
        {
            isOk = false;
            errCode = -99;
            errMsg = "";
            sendData = "";
            recData = "";
            var createOrderResponse = new NoOrderNumberReceiveResponse();
            try
            {
                var isvFilter = new IsvFilter(AppKey, AppSecret, AccessToken);
                var errorResponseFilter = new ErrorResponseFilter();

                var request = new GenericRequest
                {
                    // 对接方案编码，不同的对接方案取值不同，具体取值可在【控制台-应用管理-对接方案-编码】查看
                    Domain = Settings.APITokenKey.JdFreshMedicineDeliveryDomain,
                    // 接口调用地址，具体取值请看【接口文档-请求地址-调用路径（path）】
                    Path = Settings.APITokenKey.JdFreshMedicineDeliveryCreateOrderUrl,
                    // 固定是POST，并且是大写
                    Method = "POST"
                };

                // 请求报文，根据接口文档组织请求报文

                //报文需要在最外围增加一对中括号, 所以先把对象放到list中,再序列化即可.
                var body = new List<NoOrderNumberReceiveRequest> { createOrderRequest };
                var bodyJson = JsonConvert.SerializeObject(body,
                    new JsonSerializerSettings()
                    { NullValueHandling = NullValueHandling.Ignore });
                //请求参数 格式:NoOrderNumberReceiveRequest序列化
                sendData = bodyJson;
                request.Body = Encoding.UTF8.GetBytes(bodyJson);
                request.AddFilter(isvFilter);
                request.AddFilter(errorResponseFilter);

                //可以指定加密方式和解码方式, 默认即可
                var options = new Options();

                var response = _client.Execute(request, options);
                if (response != null)
                {
                    //返回参数
                    //格式: "GenericResponse{succeed=" + Succeed + ", TraceId=" + TraceId + ", ZhDesc=" + ZhDesc + ", EnDesc=" + EnDesc + ", Code=" + Code + ", Status=" + Status + ", Body=" + Encoding.UTF8.GetString(Body) + ", Entity=" + Entity?.ToString() +"}"
                    recData = response.ToString();
                    if (response.Code == 0 && response.Succeed)
                    {
                        if (response.Body != null)
                        {
                            var decodedString = Encoding.UTF8.GetString(response.Body);
                            createOrderResponse = JsonConvert.DeserializeObject<NoOrderNumberReceiveResponse>(decodedString);
                            if (createOrderResponse.success && createOrderResponse.HasData && YJT.Text.Verification.IsNotNullOrEmpty(createOrderResponse.data.waybillNo))
                            {
                                errMsg = "";
                                errCode = 0;
                                isOk = true;
                            }
                            else
                            {
                                errMsg = string.IsNullOrWhiteSpace(createOrderResponse.message) ? "下单请求错误,响应异常." : createOrderResponse.message;
                                errCode = -5;
                                isOk = false;
                            }
                        }
                        else
                        {
                            errMsg = "response.Body为null";
                            errCode = -4;
                            isOk = false;
                        }
                    }
                    else
                    {
                        errMsg = string.IsNullOrWhiteSpace(response.ZhDesc) ? "返回结果response.Code不为0" : response.ZhDesc;
                        errCode = -3;
                        isOk = false;
                    }
                }
                else
                {
                    errMsg = "返回结果为null";
                    errCode = -2;
                    isOk = false;
                }

            }
            catch (Exception e)
            {
                errMsg = e.ToString();
                errCode = -1;
                isOk = false;
            }
            return createOrderResponse;
        }

        public CancelOrderByVendorCodeAndDeliveryIdResponse CancelOrder(
            CancelOrderByVendorCodeAndDeliveryIdRequest cancelOrderRequest,
            out bool isOk, out int errCode, out string errMsg, out string sendData, out string recData)
        {
            isOk = false;
            errCode = -99;
            errMsg = "";
            sendData = "";
            recData = "";

            var cancelOrderResponse = new CancelOrderByVendorCodeAndDeliveryIdResponse();
            try
            {
                var isvFilter = new IsvFilter(AppKey, AppSecret, AccessToken);
                var errorResponseFilter = new ErrorResponseFilter();

                var request = new GenericRequest
                {
                    // 对接方案编码，不同的对接方案取值不同，具体取值可在【控制台-应用管理-对接方案-编码】查看
                    Domain = Settings.APITokenKey.JdFreshMedicineDeliveryDomain,
                    // 接口调用地址，具体取值请看【接口文档-请求地址-调用路径（path）】
                    Path = Settings.APITokenKey.JdFreshMedicineDeliveryCancelOrderUrl,
                    // 固定是POST，并且是大写
                    Method = "POST"
                };

                // 请求报文，根据接口文档组织请求报文

                //报文需要在最外围增加一对中括号, 所以先把对象放到list中,再序列化即可.
                var body = new List<CancelOrderByVendorCodeAndDeliveryIdRequest> { cancelOrderRequest };
                var bodyJson = JsonConvert.SerializeObject(body,
                    new JsonSerializerSettings()
                    { NullValueHandling = NullValueHandling.Ignore });
                //请求参数 格式:NoOrderNumberReceiveRequest序列化
                sendData = bodyJson;
                request.Body = Encoding.UTF8.GetBytes(bodyJson);
                request.AddFilter(isvFilter);
                request.AddFilter(errorResponseFilter);

                //可以指定加密方式和解码方式, 默认即可
                var options = new Options();

                var response = _client.Execute(request, options);
                if (response != null)
                {
                    //返回参数
                    //格式: "GenericResponse{succeed=" + Succeed + ", TraceId=" + TraceId + ", ZhDesc=" + ZhDesc + ", EnDesc=" + EnDesc + ", Code=" + Code + ", Status=" + Status + ", Body=" + Encoding.UTF8.GetString(Body) + ", Entity=" + Entity?.ToString() +"}"
                    recData = response.ToString();
                    if (response.Code == 0 && response.Succeed)
                    {
                        if (response.Body != null)
                        {
                            var decodedString = Encoding.UTF8.GetString(response.Body);
                            cancelOrderResponse = JsonConvert.DeserializeObject<CancelOrderByVendorCodeAndDeliveryIdResponse>(decodedString);
                            if (cancelOrderResponse.success && cancelOrderResponse.HasData && YJT.Text.Verification.IsNotNullOrEmpty(cancelOrderResponse.data))
                            {
                                errMsg = "";
                                errCode = 0;
                                isOk = true;
                            }
                            else
                            {
                                errMsg = string.IsNullOrWhiteSpace(cancelOrderResponse.message) ? "取消/拦截请求失败,响应异常." : cancelOrderResponse.message;
                                errCode = -5;
                                isOk = false;
                            }
                        }
                        else
                        {
                            errMsg = "response.Body为null";
                            errCode = -4;
                            isOk = false;
                        }
                    }
                    else
                    {
                        errMsg = string.IsNullOrWhiteSpace(response.ZhDesc) ? "返回结果response.Code不为0" : response.ZhDesc;
                        errCode = -3;
                        isOk = false;
                    }
                }
                else
                {
                    errMsg = "返回结果为null";
                    errCode = -2;
                    isOk = false;
                }

            }
            catch (Exception e)
            {
                errMsg = e.ToString();
                errCode = -1;
                isOk = false;
            }
            return cancelOrderResponse;
        }

        public RangeCheckDeliveryQueryApiResponse CheckOrder(RangeCheckDeliveryQueryApiRequest checkOrderRequest, 
            out bool isOk, out int errCode, out string errMsg, out string sendData, out string recData)
        {
            isOk = false;
            errCode = -99;
            errMsg = "";
            sendData = "";
            recData = "";

            var checkOrderResponse = new RangeCheckDeliveryQueryApiResponse();
            try
            {
                var isvFilter = new IsvFilter(AppKey, AppSecret, AccessToken);
                var errorResponseFilter = new ErrorResponseFilter();

                var request = new GenericRequest
                {
                    // 对接方案编码，不同的对接方案取值不同，具体取值可在【控制台-应用管理-对接方案-编码】查看
                    Domain = Settings.APITokenKey.JdFreshMedicineDeliveryDomain,
                    // 接口调用地址，具体取值请看【接口文档-请求地址-调用路径（path）】
                    Path = Settings.APITokenKey.JdFreshMedicineDeliveryCheckOrderUrl,
                    // 固定是POST，并且是大写
                    Method = "POST"
                };

                // 请求报文，根据接口文档组织请求报文

                //报文需要在最外围增加一对中括号, 所以先把对象放到list中,再序列化即可.
                var body = new List<RangeCheckDeliveryQueryApiRequest> { checkOrderRequest };
                var bodyJson = JsonConvert.SerializeObject(body,
                    new JsonSerializerSettings()
                    { NullValueHandling = NullValueHandling.Ignore });
                //请求参数 格式:NoOrderNumberReceiveRequest序列化
                sendData = bodyJson;
                request.Body = Encoding.UTF8.GetBytes(bodyJson);
                request.AddFilter(isvFilter);
                request.AddFilter(errorResponseFilter);

                //可以指定加密方式和解码方式, 默认即可
                var options = new Options();

                var response = _client.Execute(request, options);
                if (response != null)
                {
                    //返回参数
                    //格式: "GenericResponse{succeed=" + Succeed + ", TraceId=" + TraceId + ", ZhDesc=" + ZhDesc + ", EnDesc=" + EnDesc + ", Code=" + Code + ", Status=" + Status + ", Body=" + Encoding.UTF8.GetString(Body) + ", Entity=" + Entity?.ToString() +"}"
                    recData = response.ToString();
                    if (response.Code == 0 && response.Succeed)
                    {
                        if (response.Body != null)
                        {
                            var decodedString = Encoding.UTF8.GetString(response.Body);
                            checkOrderResponse = JsonConvert.DeserializeObject<RangeCheckDeliveryQueryApiResponse>(decodedString);
                            if (checkOrderResponse.success && checkOrderResponse.HasData)
                            {
                                errMsg = "";
                                errCode = 0;
                                isOk = true;
                            }
                            else
                            {
                                errMsg = string.IsNullOrWhiteSpace(checkOrderResponse.message) ? "下单前置校验请求失败,响应异常." : checkOrderResponse.message;
                                errCode = -5;
                                isOk = false;
                            }
                        }
                        else
                        {
                            errMsg = "response.Body为null";
                            errCode = -4;
                            isOk = false;
                        }
                    }
                    else
                    {
                        errMsg = string.IsNullOrWhiteSpace(response.ZhDesc) ? "返回结果response.Code不为0" : response.ZhDesc;
                        errCode = -3;
                        isOk = false;
                    }
                }
                else
                {
                    errMsg = "返回结果为null";
                    errCode = -2;
                    isOk = false;
                }

            }
            catch (Exception e)
            {
                errMsg = e.ToString();
                errCode = -1;
                isOk = false;
            }
            return checkOrderResponse;

        }
    }
}
