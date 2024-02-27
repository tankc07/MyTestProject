using System.Collections.Generic;

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
        public string Secret { get; set; }
        /// <summary>
        /// 测试签名密钥
        /// </summary>
        public string TestSecret { get; set; }
        /// <summary>
        /// 授权码（ 区分测试和生产 )
        /// </summary>
        public string Authorization { get; set; }
        /// <summary>
        /// 测试授权码（ 区分测试和生产 )
        /// </summary>
        public string TestAuthorization { get; set; }

        /// <summary>
        /// 是否启用测试环境与参数 <b>（ 默认:true )</b>
        /// </summary>
        public bool IsTest { get; set; } = true;


        public static NewEms Init(string senderNo, string secret, string authorization, string testSecret = "", string testAuthorization = "", bool isTest = true)
        {
            NewEms t = null;
            if (AllList.TryGetValue(senderNo, out var value))
            {
                t = value;
            }
            else
            {
                t = new NewEms(senderNo)
                {
                    Secret = secret,
                    Authorization = authorization,
                    TestSecret = testSecret,
                    TestAuthorization = testAuthorization,
                    IsTest = isTest
                };
                AllList.Add(senderNo, t);
            }
            return t;
        }

        public NewEms(string senderNo)
        {
            SenderNo = senderNo;
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


    }
}
