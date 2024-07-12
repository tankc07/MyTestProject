using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using LogisticsCore.NewEMS.Model;
using LogisticsCore.NewEMS.Request;
using LogisticsCore.NewEMS.Response;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;
using RestSharp;
using YJT.Text;
using Formatting = Newtonsoft.Json.Formatting;

namespace LogisticsCore.NewEMS
{
    public class NewEms
    {
        /// <summary>
        /// 所有实例,以协议编码区分
        /// </summary>
        protected static readonly Dictionary<string, NewEms> AllList = new Dictionary<string, NewEms>();

        /// <summary>
        /// 协议客户号
        /// </summary>
        public string SenderNo { get; }
        /// <summary>
        /// 签名密钥
        /// </summary>
        public string SignKey { get; set; }
        /// <summary>
        /// 测试签名密钥
        /// </summary>
        public string TestSignKey { get; set; }
        /// <summary>
        /// 授权码（ 区分测试和生产 )
        /// </summary>
        public string Authorization { get; set; }
        /// <summary>
        /// 测试授权码（ 区分测试和生产 )
        /// </summary>
        public string TestAuthorization { get; set; }
        public string BaseUrl { get; set; }
        public string NewEmsUrl { get; set; }
        public string NewEmsTestUrl { get; set; }
        /// <summary>
        /// 是否启用测试环境与参数 <b>（ 默认:true )</b>
        /// </summary>
        public bool IsTest { get; set; } = true;

        private readonly RestClient _client;

        public static NewEms Init(string senderNo, string signKey, string authorization,
            string testSignKey = "", string testAuthorization = "",
            string baseUrl = "https://api.ems.com.cn", string newEmsUrl = "/amp-prod-api/f/amp/api/open", string newEmsTestUrl = "/amp-prod-api/f/amp/api/test", bool isTest = true)
        {
            NewEms t;
            if (AllList.TryGetValue(senderNo, out var value))
            {
                t = value;
            }
            else
            {
                t = new NewEms(senderNo)
                {
                    SignKey = signKey,
                    Authorization = authorization,
                    TestSignKey = testSignKey,
                    TestAuthorization = testAuthorization,
                    BaseUrl = baseUrl,
                    NewEmsUrl = newEmsUrl,
                    NewEmsTestUrl = newEmsTestUrl,
                    IsTest = isTest,
                };
                AllList.Add(senderNo, t);
            }
            return t;
        }

        private NewEms(string senderNo)
        {
            SenderNo = senderNo;
            _client = new RestClient(new RestClientOptions(Settings.APITokenKey.NewEmsBaseUrl)
            {
                ConfigureMessageHandler = handler => new HttpClientHandler() { ServerCertificateCustomValidationCallback = delegate { return true; } },
                FailOnDeserializationError = true,
                ThrowOnAnyError = true,
                MaxTimeout = -1,
            });
        }

        public bool RemoveEmsObj(string senderNo)
        {
            if (AllList.ContainsKey(senderNo))
            {
                AllList.Remove(senderNo);
                return true;
            }
            return false;
        }


        public NewEmsRequestBase GetCreateOrderModel(AddressModel sender, AddressModel receiver, CargoModel[] cargoes,
            string ecommerceUserId, string erpId, int weight, double length, double width, double height, string receiverSafetyCode,
            string pickupNotes = null,
            string batchNo = null,
            int insuranceFlag = 1,
            double insuranceAmount = 0.0,
            double insurancePremiumAmount = 0.0,
            double postageTotal = 0.0,
            double electronicPreferentialAmount = 0.0)
        {
            if (cargoes == null || cargoes.Length < 1)
            {
                throw new Exception("至少指定一个货品cargo");
            }

            var model = new NewEmsRequestBase
            {
                apiCode = "020003",
                senderNo = SenderNo,
                authorization = IsTest ? TestAuthorization : Authorization,
                timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                msgType = "0"
            };
            //以下两个参数不重要, 非必填, 默认null即可.
            //model.Version = "V1.0.0";
            //model.UserCode = null;
            var t = new CreateOrderModel
            {
                createdTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ecommerceUserId = ecommerceUserId,
                senderNo = SenderNo,
                logisticsOrderNo = erpId,
                batchNo = batchNo,
                waybillNo = null,
                oneBillFlag = "0",
                oneBillNum = null,
                subMailNo = null,
                oneBillFeeType = null,
                contentsAttribute = "3",
                bizProductNo = "10",
                weight = weight.ToString(),
                volume = (length * width * height).ToString("#0.000"),
                length = length.ToString("#0.00"),
                width = width.ToString("#0.00"),
                height = height.ToString("#0.00"),
                postageTotal = postageTotal > 0 ? postageTotal.ToString("#0.00") : null,
                pickupNotes = pickupNotes,
                insuranceFlag = insuranceFlag.ToString(),
                insuranceAmount = insuranceAmount > 0 ? insuranceAmount.ToString("#0.00") : null,
                insurancePremiumAmount = insurancePremiumAmount > 0 ? insurancePremiumAmount.ToString("#0.00") : null,
                deliverType = "2",
                deliverPreDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                deliverPreEndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                pickupType = "1",
                pickupPreBeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                pickupPreEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                paymentMode = "1",
                codFlag = "9",
                codAmount = "0",
                receiptFlag = "1",
                receiptWaybillNo = null,
                electronicPreferentialNo = null,
                electronicPreferentialAmount = electronicPreferentialAmount > 0 ? electronicPreferentialAmount.ToString("#0.00") : null,
                valuableFlag = "0",
                senderSafetyCode = null,
                receiverSafetyCode = receiverSafetyCode,
                note = null,
                projectId = null,
                teanIncrementFlag = "0",
                virtualNumFlag = "0",
                sender = sender,
                receiver = receiver,
                cargos = cargoes.ToList()
            };
            model.logitcsInterface = t;
            return model;
        }

        public NewEmsRequestBase GetCancelOrderModel(string erpId, string waybillNo, DateTime createTime, out bool isOk, out int errCode, out string errMsg, string cancelReason = "用户放弃")
        {
            isOk = false;
            errCode = -99;
            errMsg = "";
            if (Verification.IsNullOrEmpty(erpId, false, true))
            {
                isOk = false;
                errCode = -1;
                errMsg = "单据编号为空";
                return new NewEmsRequestBase();
            }
            if (Verification.IsNullOrEmpty(waybillNo, false, true))
            {
                isOk = false;
                errCode = -1;
                errMsg = "运单编号为空";
                return new NewEmsRequestBase();
            }
            if (createTime == default(DateTime))
            {
                isOk = false;
                errCode = -1;
                errMsg = "订单接入时间为空";
                return new NewEmsRequestBase();
            }
            if (Verification.IsNullOrEmpty(cancelReason, false, true))
            {
                isOk = false;
                errCode = -1;
                errMsg = "取消原因为空";
                return new NewEmsRequestBase();
            }

            var model = new NewEmsRequestBase
            {
                apiCode = "020006",
                senderNo = SenderNo,
                authorization = IsTest ? TestAuthorization : Authorization,
                timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                msgType = "0"
            };
            var t = new CancelOrderModel()
            {
                logisticsOrderNo = erpId,
                waybillNo = waybillNo,
                cancelReason = cancelReason
            };
            model.logitcsInterface = t;
            return model;
        }

        public AddressModel GetAddressModel(string address, string prov, string city, string county, string name, string mobile, string phone, string postCode = "")
        {
            var model = new AddressModel
            {
                address = address,
                prov = prov,
                city = city,
                county = county,
                name = name,
                mobile = mobile,
                phone = phone,
                postCode = postCode
            };
            return model;
        }

        public CargoModel GetCargoModel(string cargoName = "药品", string cargoCategory = "", int cargoQuantity = 0, double cargoValue = 1.0, double cargoWeight = 1.0, double cargoHigh = 0.0, double cargoLength = 0.0, double cargoWidth = 0.0, string orderNo = "", string cargoMailNo = "")
        {
            var model = new CargoModel
            {
                cargoName = cargoName,
                cargoCategory = cargoCategory,
                cargoQuantity = cargoQuantity > 0 ? cargoQuantity.ToString() : "",
                cargoValue = cargoValue > 0 ? cargoValue.ToString("#0.00") : "",
                cargoWeight = cargoWeight > 0 ? ((int)cargoWeight).ToString() : "",
                high = cargoHigh > 0 ? cargoHigh.ToString("#0.000") : "",
                length = cargoLength > 0 ? cargoLength.ToString("#0.000") : "",
                width = cargoWidth > 0 ? cargoWidth.ToString("#0.000") : "",
                orderNo = orderNo,
                cargoMailNo = cargoMailNo,
            };
            return model;
        }

        /// <summary>
        /// 获取加密后的签名
        /// 加密方式为: SM4,ECB模式, PKCS5Padding填充方式
        /// </summary>
        /// <param name="logisticsInterfaceBase">待加密实体类对象</param>
        /// <param name="algorithm">解密算法/解密算法模式/填充方式</param>
        /// <param name="prefix">加密后字符串的前缀</param>
        /// <returns></returns>
        public string GetSignBySm4EncryptEcb(LogisticsInterfaceBase logisticsInterfaceBase, string algorithm = "SM4/ECB/PKCS5Padding", string prefix = "|$4|")
        {
            if (logisticsInterfaceBase == null)
                throw new ArgumentNullException(nameof(logisticsInterfaceBase));
            var logisticsInterfaceBaseJson = JsonConvert.SerializeObject(new[] { logisticsInterfaceBase }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
            var secret = IsTest ? TestSignKey : SignKey;
            //secret = "TvaBgrhE46sft3nZlfe7xw==";
            var keyBytes = Convert.FromBase64String(secret);
            var plaintext = logisticsInterfaceBaseJson + secret;

            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            try
            {
                var key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);

                //ECB模式不需要IV, 只有CBC模式需要IV
                //byte[] iv = Encoding.UTF8.GetBytes("0000000000000000");
                //ParametersWithIV keyParamWithIv = new ParametersWithIV(key, iv);

                //获取SM4加密算法实例 SM4/ECB/PKCS5Padding: SM4算法,ECB模式,PKCS5Padding填充方式
                var inCipher = CipherUtilities.GetCipher(algorithm);
                //初始化加密算法, true表示加密, false表示解密, ECB模式不需要IV, 传递KeyParameter类型即可, CBC模式需要IV, 需传递ParametersWithIV类型
                inCipher.Init(true, key);
                //SM4加密
                var cipher = inCipher.DoFinal(plaintextBytes);
                //将密文转换为Base64字符串,并拼接前缀|$4|
                var result = prefix + Convert.ToBase64String(cipher);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public NewEmsResponseBase SendNewEmsOrder(NewEmsRequestBase request, out bool isOk, out int errCode, out string errMsg, out string sendString, out string resString)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //调试查看用
            var logisticsInterfaceJson = JsonConvert.SerializeObject(request.logitcsInterface, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });

            var restRequest = new RestRequest(IsTest ? NewEmsTestUrl : NewEmsUrl, Method.Post);

            restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            restRequest.AddParameter("senderNo", request.senderNo, ParameterType.GetOrPost);
            restRequest.AddParameter("apiCode", request.apiCode, ParameterType.GetOrPost);
            restRequest.AddParameter("authorization", request.authorization, ParameterType.GetOrPost);
            restRequest.AddParameter("msgType", request.msgType, ParameterType.GetOrPost);
            restRequest.AddParameter("timeStamp", request.timeStamp, ParameterType.GetOrPost);
            var logitcsInterface = GetSignBySm4EncryptEcb(request.logitcsInterface);
            restRequest.AddParameter("logitcsInterface", logitcsInterface, ParameterType.GetOrPost);
            //restRequest.AddParameter("userCode", request.SenderNo);
            sendString = $"senderNo={request.senderNo}&apiCode={request.apiCode}&authorization={request.authorization}&msgType={request.msgType}&timeStamp={request.timeStamp}&logitcsInterface={logitcsInterface}";

            try
            {
                var response = _client.Execute(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    NewEmsResponseBase responseObj = null;
                    if (response.Content != null)
                    {
                        resString = response.Content;
                        switch (request.apiCode)
                        {
                            case "020003":
                                responseObj = JsonConvert.DeserializeObject<CreateOrderResponse>(response.Content);
                                isOk = responseObj.success;
                                errCode = responseObj.success ? 0 : 1;
                                errMsg = responseObj.retMsg;
                                break;
                            case "020006":
                                responseObj = JsonConvert.DeserializeObject<CancelOrderResponse>(response.Content);
                                isOk = responseObj.success;
                                errCode = responseObj.success ? 0 : 1;
                                errMsg = responseObj.retMsg;
                                break;
                            default:
                                isOk = false;
                                errCode = -2;
                                errMsg = $@"NewEms接口响应错误, 接口代码(apiCode): {request.apiCode}, 不受支持.";
                                resString = errMsg;
                                break;
                        }
                    }
                    else
                    {
                        isOk = false;
                        errCode = -99;
                        errMsg = $@"NewEms接口响应内容(response.Content)为空";
                        resString = errMsg;
                    }
                    return responseObj;
                }
                else
                {
                    isOk = false;
                    errCode = -1;
                    errMsg =
                        $"NewEms下单取号请求失败: StatusCode: {response.StatusCode.ToString()}, ErrorMessage: {response.ErrorMessage ?? "无ErrorMessage消息."}";
                    resString = errMsg;
                    return null;
                }
            }
            catch (Exception e)
            {
                isOk = false;
                errCode = -99;
                errMsg = $"请求或响应处理过程中发生异常, Exception : {e.Message}";
                resString = errMsg;
                if (ConfigurationManager.AppSettings["IsDebugMode"] == "true")
                {
                    var basePath = AppDomain.CurrentDomain.BaseDirectory + @"\DebugLogs\";
                    try
                    {
                        if (!Directory.Exists(basePath))
                        {
                            Directory.CreateDirectory(basePath);
                        }
                        var msg = $@"{DateTime.Now:yyyy-MM-dd HH:mm:ss}-----------------------------------{Environment.NewLine}{errMsg}{Environment.NewLine}";
                        File.AppendAllText($"{basePath}_SendNewEmsOrder_Exception.txt", msg);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                return null;
            }
        }
    }
}
