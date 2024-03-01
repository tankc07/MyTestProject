using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings
{
	public class Setings
	{
		public enum EnumLogicType
		{
			/// <summary>
			/// 默认算法
			/// </summary>
			Default=0,
			/// <summary>
			/// Log_ShunFeng
			/// </summary>
			顺丰=1,
			/// <summary>
			/// Log_YouZhengEms
			/// </summary>
			邮政EMS=2,
			/// <summary>
			/// Log_BaishiJitu
			/// </summary>
			极兔百事=3,
			/// <summary>
			/// Log_JingDong
			/// </summary>
			京东物流=4,
			/// <summary>
			/// Log_ZhongTong
			/// </summary>
			中通快递=5,
			/// <summary>
			/// Log_ShenTong
			/// </summary>
			申通快递=6,
			//Modify: 修改时间: 2024-02-27 By:Ly 修改内容: 新增枚举选项,EnumLogicType.新邮政Ems
			/// <summary>
			/// Log_NewEms
			/// </summary>
			新邮政Ems=7
		}
		public enum EnumMessageType
		{
			输入,
			提示,
			严重,
			异常,
			测试信息,

		}
		public enum EnumOrderStatus
		{
			关机任务,
			关机完成,
			/// <summary>
			/// 初始化
			/// </summary>
			无动作,
			/// <summary>
			/// 异常
			/// </summary>
			异常_曾经下传过,
			/// <summary>
			/// 异常2
			/// </summary>
			异常_写入失败,
			/// <summary>
			/// 异常3
			/// </summary>
			异常_类型转换失败,
			异常_WMS不存在,
			异常_电商信息不存在,
			异常_ERPID不正确,
			异常_物流信息不正确,
			异常_物流下单不成功,
			异常_打印方案不存在,
			异常_物流不支持打印,
			异常_引用ID错误,
			异常_汇达合同信息获取失败,
			/// <summary>
			/// 删除1
			/// </summary>
			准备停止,
			/// <summary>
			/// 删除2
			/// </summary>
			停止完成,
			/// <summary>
			/// 第一步
			/// </summary>
			新增未写缓存,
			/// <summary>
			/// 第二步
			/// </summary>
			已下传,
			/// <summary>
			/// 第三步
			/// </summary>
			已获取WMS信息,
			/// <summary>
			/// 第四步
			/// </summary>
			已获取电商信息,
			/// <summary>
			/// 第五步
			/// </summary>
			已获取物流单号,
			/// <summary>
			/// 第六步
			/// </summary>
			已回写电商平台,
			仅要求打印,
			单据完成,
			/// <summary>
			/// 服务器不处理此类,通过RELBID关联更新
			/// </summary>
			补打任务,
			补打完成,
			追加物流子单,
			追加物流子单完成,
			//此种单据,不需要服务器任务循环处理,是通过组合单总单关联
			ERP组合单子单,
			ERP组合单
		}
		public enum EnumOrderPrintStatus
		{
			/// <summary>
			/// 第六步,物流,和随货都打印
			/// </summary>
			等待打印,
			/// <summary>
			/// 第七步
			/// </summary>
			已打印,
			/// <summary>
			/// 特殊操作
			/// </summary>
			等待补打随货,
			等待补打物流,
			/// <summary>
			/// 单据不完整,不能打印
			/// </summary>
			不可打印,
			补打物流单完成,
			补打随货单完成,
			异常_类型转换失败,
			特殊_不更改此参数
		}
		public enum EnumServerTaskType
		{
			无,
			新单据,
			补打随货,
			补打物流全部,
			补打物流子单,
			终止订单,
			修改物流,
			添加物流子单,
			关机任务,
			异常_类型转换失败,
			ERP组合单,
			ERP组合单子单
		}
		public enum EnumTaskType
		{
			下传数据库,
			更改DataTable状态,
			要求打印,
			要求补打,
			要求停止

		}
		public enum EnumPlatformType
		{
			无=0,
			药师帮=1,
			小药药=2,
			药京采=3
		}
	}
	public class APITokenKey
	{
		/// <summary>
		/// 百度地图
		/// </summary>
		public static string BaiDuMapKey
		{
			get
			{
				return "E7f81f972faf012680af0f5b7de722c8";
				
			}
		}
		public static string ShunfengCustomerId
		{
			get
			{
				//return "704"; //测试
				return "719"; //正式
			}
		}
		public static string ShunfengSecret
		{
			get
			{
				//return "ee6030e3f4f3f6eab5bca5f36a8ccf73";//测试
				return "4a6919c248276e6ae6bea9925ac94525";//正式
			}
		}
		public static bool ShunfengIsTest
		{
			get
			{
				return false;
			}
		}
		public static string EmsYouzhengSenderNo
		{
			get
			{
				return "1100094086346";
			}
		}
		public static string EmsYouzhengSecrect
		{
			get
			{
				return "b565b8f0348c592d7eb766cd40717032";
			}
		}
		public static string EmsYouzhengSecrectTest
		{
			get
			{
				return "485a7335256edfe015deb0eb269f1b51";
			}
		}
		public static bool EmsYouzhengIsTest
		{
			get
			{
				return false;
			}
		}
		public static string JingdongWlAppKey
		{
			get
			{
				return "A69D7A8036F837904F399534B2837F17";
			}
		}
		public static string JingdongWlAppSecret
		{
			get
			{
				return "b5bc8d4c3cdd4c99bfc7ec36a3d1645f";
			}
		}
		public static string JingdongWlAccessUrl
		{
			get
			{
				return @"https://api.jd.com/routerjson";
			}
		}
		public static string JingdongWlToken
		{
			get
			{
				return "e2075d659ec04fd7bcbafed7c1c329c2tiwn";
			}
		}
		public static string JingdongWlDeptNo
		{
			get
			{
				return @"010K680062"; //EBU4418046594811
			}
		}
		public static string ZhongTongWlAppKey
		{
			get
			{
				return @"f8af3a0a7f2bf72c0c0c0";
			}
		}
		public static string ZhongTongWlAppKeyTest
		{
			get
			{
				return @"3daabc717c853dc948c9f";
			}
		}
		public static string ZhongTongappSecret
		{
			get
			{
				return @"83cba11de165e102b1d357fa7de9ca66";
			}
		}
		public static string ZhongTongappSecretTest
		{
			get
			{
				return @"93aa63fb124f5df1252f663492f66722";
			}
		}
		public static string ZhongTongCustomid
		{
			get
			{
				return @"ZTO1421661240847000";
				
			}
		}
		public static string ZhongTongCustomPwd
		{
			get
			{
				return @"9IKBLIK7";
			}
		}
		public static bool ZhongTongIsTest
		{
			get
			{
				return false;
			}
		}
		public static string ShenTongAppKey
		{
			get
			{
				return @"CAKYnBVqIIaiFYf";
			}
		}
		public static string ShenTongSecretKey
		{
			get
			{
				return @"A11TgLxfVkJKIu1XYyuIrdEbC6v8DC6I";
			}
		}
		public static string ShenTongResourceCode
		{
			get
			{
				return @"CAKYnBVqIIaiFYf";
			}
		}
		public static string ShenTongFormOrderCode
		{
			get
			{
				return @"HDDS";
			}
		}
		public static string ShenTongSiteCode
		{
			get
			{
				return @"450288";
			}
		}
		public static string ShenTongCustomerName
		{
			get
			{
				return @"450288000018";
			}
		}
		public static string ShenTongSitePwd
		{
			get
			{
				return @"Sto2022@";
			}
		}
		public static bool ShenTongIsTest
		{
			get
			{
				return false;
			}
		}

		//Modify: 修改时间: 2024-02-27 By:Ly 修改内容: 新增NewEms
		/// <summary>
		/// NewEms Api请求基地址
		/// </summary>
		public static string NewEmsBaseUrl = "https://api.ems.com.cn";
		/// <summary>
		/// 正式协议客户号
		/// </summary>
		public static string NewEmsSenderNo => "1100051370536";
        /// <summary>
        /// 正式环境请求地址
        /// </summary>
        public static string NewEmsUrl => "/amp-prod-api/f/amp/api/open";
        /// <summary>
        /// 正式授权码
        /// </summary>
        public static string NewEmsAuthorization => "SetemYDjgS80gdSa";
		/// <summary>
		/// 正式签名钥匙
		/// </summary>
		public static string NewEmsSignKey => "T1ZPenBHTmIxNlh3dnhRQw==";
		/// <summary>
		/// 测试沙箱环境请求地址
		/// </summary>
		public static string NewEmsTestUrl => "/amp-prod-api/f/amp/api/test";
		/// <summary>
		/// 测试授权码
		/// </summary>
		public static string NewEmsTestAuthorization => "UBP0lGUev6irKPi7";
		/// <summary>
		/// 测试签名钥匙
		/// </summary>
		public static string NewEmsTestSignKey => "NkhUbzFkOTdOQjFNRzNpQw==";
		/// <summary>
		/// 是否启用测试环境及参数
		/// </summary>
		public static bool NewEmsIsTest => false;


	}
	public class Configs
	{
		public static string GetShunfPrinterName
		{
			get
			{
				//return @"\\172.16.2.152\DASCOM DL-100";
				return YJT.ConfigTxt.AppConfigFIle.GetValueEX("GetShunfPrinterName", @"\\172.16.2.150\BTP-L540H(BPLZ)(U)1", true);//顺丰
			}
		}
		public static string GetEmsYouzPrinterName
		{
			get
			{
				return YJT.ConfigTxt.AppConfigFIle.GetValueEX("GetEmsYouzPrinterName", @"\\172.16.2.150\DASCOM DL-100", true);//邮政
			}
		}
		public static String GetSHTXPrinterName
		{
			get
			{
				return YJT.ConfigTxt.AppConfigFIle.GetValueEX("GetSHTXPrinterName", @"\\172.16.2.150\Epson LQ-1600K", true);//随货通行
				//return @"\\172.16.2.201\Epson LQ-1600K (副本 2)";
			}
		}

		public static string GetJingDongPrinterName
		{
			get
			{
				return YJT.ConfigTxt.AppConfigFIle.GetValueEX("GetJingDongPrinterName", @"\\172.16.2.150\Deli DL-888B", true);//京东打印机
			}
		}

		public static string GetZhongTongPrinterName
		{
			get
			{
				return YJT.ConfigTxt.AppConfigFIle.GetValueEX("GetZhongTongPrinterName", @"\\172.16.2.150\HPRT N31C", true);//中通打印机
			}
		}

		public static string GetShenTongPrinterName
		{
			get
			{
				return YJT.ConfigTxt.AppConfigFIle.GetValueEX("GetShenTongPrinterName", @"\\172.16.2.150\HPRT N31C", true);//申通打印机用的是中通打印机
			}
		}

		public static string GetHeTongPrinterName
		{
			get
			{
				return YJT.ConfigTxt.AppConfigFIle.GetValueEX("GetHeTongPrinterName", @"Canon iR-ADV 4225/4235 UFR II", true);//合同打印机
			}
		}
		public static string GetFmPrinterName
		{
			get
			{
				return YJT.ConfigTxt.AppConfigFIle.GetValueEX("GetFmPrinterName", @"Canon iR-ADV 4225/4235 UFR II", true);//药品信息封面打印
				//return @"EPSON0CF480 (M200 Series)";
			}
		}
		/// <summary>
		/// 是否检测电子秤
		/// </summary>
		public static string GetIsNotCheckCom
		{
			get
			{
				if (System.IO.File.Exists(@"D:\YDECAP\notcheckcom"))
				{
					return "是";
				}
				else
				{
					return "否";
				}
			}
		}
	}
}
