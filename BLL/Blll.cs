using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOD;
using MOD.HdErp;
using Settings;

namespace BLL
{
	public class Blll
	{
		#region 初始化
		static Blll _hand = null;
		YJT.DataBase.DbHelper _dbhWms = null;
		YJT.DataBase.DbHelper _dbhInterface = null;
		YJT.DataBase.DbHelper _dbhLocal = null;
		YJT.DataBase.DbHelper _dbhHdErp = null;
		DateTime _lastVerDb = DateTime.Now.AddMinutes(-5);
		public static MOD.SysMod.ClinetTag _clientInfoObj = null;

		/// <summary>
		/// msg,errType,nameSpace,errCode,errMsg,paras
		/// </summary>
		public static event Action<string, Settings.Setings.EnumMessageType, string, int, string, string, DateTime> AddMsgOutEve = null;

		/// <summary>
		/// BLL日志输出
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="type"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <param name="paras"></param>
		public static void AddMsgOut(string msg, Settings.Setings.EnumMessageType type, int errCode, string errMsg, params object[] paras)
		{
			System.Reflection.MethodBase callObj = new System.Diagnostics.StackTrace(true).GetFrame(1).GetMethod();
			string callNameSpace = "[" + callObj.DeclaringType.Namespace + "].[" + callObj.DeclaringType.FullName + "].[" + callObj.Name + "]";
			string parasList = "";
			if (paras != null && paras.Length > 0)
			{
				foreach (object item in paras)
				{
					parasList = parasList + "," + item.ToString();
				}
			}
			AddMsgOutEve?.Invoke(msg, type, "消息调用者:" + callNameSpace, errCode, errMsg, parasList, DateTime.Now);
		}
		YJT.Logistics.ShunFengLogistic _sf = YJT.Logistics.ShunFengLogistic.Init(Settings.APITokenKey.ShunfengCustomerId, Settings.APITokenKey.ShunfengSecret);
		YJT.Logistics.YouZhengEms _emsYz = YJT.Logistics.YouZhengEms.Init(Settings.APITokenKey.EmsYouzhengSenderNo, Settings.APITokenKey.EmsYouzhengSecrect, Settings.APITokenKey.EmsYouzhengSecrectTest);
		YJT.Logistics.JingDongChunPeiLogistics _jdWl = YJT.Logistics.JingDongChunPeiLogistics.Init(Settings.APITokenKey.JingdongWlAppKey, Settings.APITokenKey.JingdongWlAppSecret, Settings.APITokenKey.JingdongWlToken, Settings.APITokenKey.JingdongWlAccessUrl, Settings.APITokenKey.JingdongWlDeptNo);
		YJT.Logistics.ZhongTongLogistics _ztkdWl = YJT.Logistics.ZhongTongLogistics.Init(Settings.APITokenKey.ZhongTongWlAppKey, Settings.APITokenKey.ZhongTongappSecret, Settings.APITokenKey.ZhongTongCustomid, Settings.APITokenKey.ZhongTongCustomPwd, Settings.APITokenKey.ZhongTongWlAppKeyTest, Settings.APITokenKey.ZhongTongappSecretTest, Settings.APITokenKey.ZhongTongIsTest);
		YJT.Logistics.ShenTongLogistic _shenTongWl = YJT.Logistics.ShenTongLogistic.Init(Settings.APITokenKey.ShenTongAppKey, Settings.APITokenKey.ShenTongSecretKey, Settings.APITokenKey.ShenTongResourceCode, Settings.APITokenKey.ShenTongFormOrderCode, Settings.APITokenKey.ShenTongSiteCode, Settings.APITokenKey.ShenTongCustomerName, Settings.APITokenKey.ShenTongSitePwd, Settings.APITokenKey.ShenTongIsTest);
		//
		/// <summary>
		/// 私有构造函数
		/// </summary>
		private Blll()
		{
			_dbhWms = new YJT.DataBase.DbHelperOracle("172.16.1.242", "orcl", "wms_prod", "ydwmsprod", "1521");//正式环境
                                                                                                               //_dbhWms = new YJT.DataBase.DbHelperOracle("172.16.1.216", "ydorcl", "ydwms_test", "ydwms_test", "1521");//测试环境
            //Modify: 修改时间: 2024-02-22 By:Ly 修改内容:修改数据库地址和账号
			//_dbhLocal = new YJT.DataBase.DbHelperSqlServer("172.16.1.15", "YanduECommerceAutomaticPrinting", "dsby", "dsby", "3341");
            _dbhLocal = new YJT.DataBase.DbHelperSqlServer("172.16.7.46", "YanduECommerceAutomaticPrinting", "sa", "SqlA123b456c789.", "1433");
			//_dbhInterface = new YJT.DataBase.DbHelperOracle("172.16.1.216", "ydorcl", "neterp_to_wms", "neterp_to_wms", "1521");
            _dbhInterface = new YJT.DataBase.DbHelperOracle("172.16.1.245", "orcl", "neterp_to_wms", "neterp_to_wms", "1521");
			_dbhHdErp = new YJT.DataBase.DbHelperOracle("172.16.1.245", "orcl", "bdhdyy", "bdhdyy2014", "1521");
			_sf.IsTest = Settings.APITokenKey.ShunfengIsTest;
			_emsYz.IsTest = Settings.APITokenKey.EmsYouzhengIsTest;
			_ztkdWl.IsTest = Settings.APITokenKey.ZhongTongIsTest;
			_shenTongWl.IsTest = Settings.APITokenKey.ShenTongIsTest;
		}

		public System.Data.DataTable ClientGetDetailList(DateTime sriqi, DateTime eriqi, out bool isOK, out int errCode, out string errMsg, string erpDanjbh = "", string danjStatus = "", string mac = "",
			string serverTaskType = "", string wlLogic = "", string ecpl = "", string ddwid = "", string ddwname = "", string wlNumber = "", string addr = "", bool isYunfei = false, bool isYcz = false,bool isPj=false)
		{
			isOK = false;
			errCode = -99;
			errMsg = "初始化";
			System.Data.DataTable res = null;
			if (_clientInfoObj != null)
			{
				string whereString = "";
				if (YJT.Text.Verification.IsNotNullOrEmpty(erpDanjbh))
				{
					whereString = whereString + $"and isnull(aa.ErpId,'')='{erpDanjbh.Replace("'", "")}' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(danjStatus))
				{
					whereString = whereString + $"and isnull(aa.Status,'')='{danjStatus.Replace("'", "")}' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(mac))
				{
					whereString = whereString + $"and isnull(aa.Mac,'')='{mac.Replace("'", "")}' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(serverTaskType))
				{
					whereString = whereString + $"and isnull(aa.ServerTaskType,'')='{serverTaskType.Replace("'", "")}' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(wlLogic))
				{
					whereString = whereString + $"and isnull(aa.WL_COMPANYNAME,'')='{wlLogic.Replace("'", "")}' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(ecpl))
				{
					whereString = whereString + $"and isnull(aa.NETORDER_FROM,'')='{ecpl.Replace("'", "")}' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(ddwid))
				{
					whereString = whereString + $"and isnull(aa.CUSTOMID,'')='{ddwid.Replace("'", "")}' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(ddwname))
				{
					whereString = whereString + $"and isnull(aa.CUSTOMNAME,'') like '%{ddwname.Replace("'", "%")}%' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(wlNumber))
				{
					whereString = whereString + $"and isnull(aa.WL_NUMBER,'') like '%{wlNumber.Replace("'", "")}' \r\n";
				}
				if (YJT.Text.Verification.IsNotNullOrEmpty(addr))
				{
					whereString = whereString + $"and isnull(aa.ADDRESS,'') like '%{addr.Replace("'", "%")}%' \r\n";
				}
				if (isYunfei == true)
				{
					whereString = whereString + $"and isnull(aa.PAIDCOSTS,0)>0 \r\n";
				}
				if (isYcz == true)
				{
					whereString = whereString + $" and ISNULL(aa.Weight,4)<>4 \r\n";
				}
				if (isPj == true)
				{
					whereString = whereString + $" and(ISNULL(aa.IsHeTong,'')<>'' OR  ISNULL(aa.IsSyzz,'')<>'' OR ISNULL(aa.IsPj,'')<>'' OR ISNULL(aa.IsYjbb,'')<>'' OR ISNULL(aa.AGENTNAME,'')='刘聪聪1') \r\n";
				}
				//定义语句
				string sqlCmd = $@"
SELECT
	aa.Bid ,
	aa.OrderId ,
	aa.AddDate ,
	aa.ErpId ,
	aa.Logic ,
	aa.Weight ,
	aa.Length ,
	aa.Width ,
	aa.Height ,
	aa.Status ,
	bb.clientIP Ip ,
	bb.clientMac Mac ,
	bb.clientName ComputerName ,
	aa.ErrMsg ,
	aa.PrintStatus ,
	aa.WmsDanjbh ,
	aa.netDanjbh ,
	bb.id ClientId,
	CONVERT(VARCHAR(23),ISNULL(aa.ServerHandleDate,CAST('1970-01-01' AS datetime)),21) as ServerHandleDate,
	aa.ClientHandleDate,
	aa.WmsYewy,
	aa.WmsDdwname,
	aa.NETORDER_FROMID,
	aa.NETORDER_FROM,
	aa.ERPORDER_ID,
	aa.CUSTOMID,
	aa.CUSTOMNAME,
	aa.AGENTNAME,
	aa.ADDRESS,
	aa.WL_COMPANYID,
	aa.WL_NUMBER,
	aa.WL_COMPANYNAME,
	aa.RECVNAME,
	aa.RECVPHONE,
	aa.PROVINCENAME,
	aa.CITYNAME,
	aa.DISTRICTNAME,
	aa.STREETNAME,
	aa.ORIGINALREMARK,
	aa.IsFp,
	aa.IsPj,
	aa.IsHeTong,
	aa.IsQysy,
	aa.IsSyzz,
	aa.IsYjbb,
	aa.PlatformType,
	aa.logi_dstRoute,
	aa.logi_PayType,
	aa.logi_monAccNum,
	aa.logi_baojiaJine,
	aa.logi_dsJine,
	aa.logi_logcNum,
	aa.logi_ysJine,
	aa.logi_ysJineTotal,
	aa.logi_shouhuory,
	aa.logi_jinjianRiqi,
	aa.logi_shoufqianshu,
	aa.logi_shoufRiqi,
	aa.logi_sendSheng,
	aa.logi_sendShi,
	aa.logi_sendXian,
	aa.logi_sendAddress,
	aa.logi_sendMan,
	aa.logi_sendPhone,
	aa.logi_feiyongTotal,
	aa.logi_goodQty,
	aa.logi_goodName,
	aa.needBaojia,
	aa.logi_OrderId,
	aa.logi_CreateDate,
	aa.logi_ChanpinTypeStr,
	aa.PrintDatetime,
	aa.sysFirst,
	aa.total_amt,
	aa.RelBId,
	aa.fplx,
	aa.ServerTaskType,
	aa.PAIDCOSTS,
	aa.logi_ReceivePwd,
	aa.logi_SubOrderSn,
	aa.logi_PhonNum,
	aa.logi_TelNum,
	aa.RelOrderId,
	aa.JingdongWl
 FROM
	BllMod_Order aa
	LEFT JOIN info_Client bb ON aa.ClientId=bb.id
 WHERE
	AddDate between convert(datetime,'{sriqi.ToString("yyyy-MM-dd HH:mm:ss")}') and convert(datetime,'{eriqi.ToString("yyyy-MM-dd HH:mm:ss")}')
	{whereString}

";
				res = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
				if (res.Rows.Count > 0)
				{
					isOK = true;
					errMsg = "";
					errCode = 0;

				}
				else
				{
					isOK = false;
					errMsg = sqlCmd;
					errCode = -1;
				}
			}
			return res;
		}



		public void ServerStopLogic(BllMod.Order item, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			string sJson = "";
			string rJson = "";
			if (item.Logic == Setings.EnumLogicType.顺丰)
			{
				YJT.Logistics.ShunFengLogistic.ClassCancelOrderRes res = _sf.CancelOrder(item.ErpId + (item.logi_SubOrderSn > 0 ? "_" + item.logi_SubOrderSn.ToString() : ""), out isOk, out errCode, out errMsg, out sJson, out rJson);
				if (isOk == true)
				{
					if (res != null)
					{
						if (res.data == "取消成功")
						{
							item.WL_NUMBER = "已删" + item.WL_NUMBER;
							item.Status = Setings.EnumOrderStatus.停止完成;
						}
					}
				}
			}
			else if (item.Logic == Setings.EnumLogicType.邮政EMS)
			{
				DateTime jijianriqi;
				if (!DateTime.TryParse(item.logi_jinjianRiqi, out jijianriqi))
				{
					jijianriqi = DateTime.Now;
				}
				YJT.Logistics.YouZhengEms.ClassYouZhengEmsMod mod = _emsYz.GetModCancelOrder(item.ErpId + (item.logi_SubOrderSn > 0 ? "_" + item.logi_SubOrderSn.ToString() : ""), item.WL_NUMBER, jijianriqi, out isOk, out errCode, out errMsg);
				YJT.Logistics.YouZhengEms.ClassResBase res = _emsYz.SendGetEmsOrder(mod, out isOk, out errCode, out errMsg, out sJson, out rJson);
				if (isOk == true)
				{
					YJT.Logistics.YouZhengEms.Classoms_cancelorderRes res2 = res as YJT.Logistics.YouZhengEms.Classoms_cancelorderRes;
					if (res2 != null)
					{
						item.WL_NUMBER = "已删" + item.WL_NUMBER;
						item.Status = Setings.EnumOrderStatus.停止完成;
					}
				}

			}
			else if (item.Logic == Setings.EnumLogicType.京东物流)
			{
				isOk = true;
				item.WL_NUMBER = "已删" + item.WL_NUMBER;
				item.Status = Setings.EnumOrderStatus.停止完成;
				errMsg = "没有取消";
				//YJT.Logistics.JingDongChunPeiLogistics.ClassCancelOrder cancel = new YJT.Logistics.JingDongChunPeiLogistics.ClassCancelOrder()
				//{
				//	Danjbh = item.ErpId + (item.logi_SubOrderSn > 0 ? "_" + item.logi_SubOrderSn.ToString() : ""),
				//	Ps = "取消",
				//	WlNumber = item.WL_NUMBER
				//};
				//string sendData = "";
				//string recData = "";
				//YJT.Logistics.JingDongLogistics.ClassCancelOrderRes res = _jdWl.CancelOrder(cancel, out isOk, out errCode, out errMsg, out sendData, out recData);
				//try
				//{
				//	if (!System.IO.Directory.Exists(@"D:\WLLOG"))
				//	{
				//		System.IO.Directory.CreateDirectory(@"D:\WLLOG");
				//	}
				//	System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
				//	System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + sendData + "\r\n");
				//	System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
				//	System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + recData + "\r\n");

				//}
				//catch (Exception ee)
				//{
				//	AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
				//}

			}
			else if (item.Logic == Setings.EnumLogicType.中通快递)
			{
				isOk = true;
				item.WL_NUMBER = "已删" + item.WL_NUMBER;
				item.Status = Setings.EnumOrderStatus.停止完成;
				errMsg = "没有取消";
				//YJT.Logistics.JingDongChunPeiLogistics.ClassCancelOrder cancel = new YJT.Logistics.JingDongChunPeiLogistics.ClassCancelOrder()
				//{
				//	Danjbh = item.ErpId + (item.logi_SubOrderSn > 0 ? "_" + item.logi_SubOrderSn.ToString() : ""),
				//	Ps = "取消",
				//	WlNumber = item.WL_NUMBER
				//};
				//string sendData = "";
				//string recData = "";
				//YJT.Logistics.JingDongLogistics.ClassCancelOrderRes res = _jdWl.CancelOrder(cancel, out isOk, out errCode, out errMsg, out sendData, out recData);
				//try
				//{
				//	if (!System.IO.Directory.Exists(@"D:\WLLOG"))
				//	{
				//		System.IO.Directory.CreateDirectory(@"D:\WLLOG");
				//	}
				//	System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
				//	System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + sendData + "\r\n");
				//	System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
				//	System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + recData + "\r\n");

				//}
				//catch (Exception ee)
				//{
				//	AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
				//}

			}
			else if (item.Logic == Setings.EnumLogicType.申通快递)
			{
				YJT.Logistics.ShenTongLogistic.ClassCancelOrder cco = _shenTongWl.IntorCancelOrder(item.ErpId + (item.logi_SubOrderSn > 0 ? "_" + item.logi_SubOrderSn.ToString() : ""), item.WL_NUMBER, "自动打印", YJT.Logistics.ShenTongLogistic.ClassCreateOrder.EnumOrderType.普通订单01, "取消");
				string sendData = "";
				string recData = "";
				YJT.Logistics.ShenTongLogistic.ClassPostData cpd = null;
				YJT.Logistics.ShenTongLogistic.ClassCancelOrderRes ccor = _shenTongWl.CancelOrder(cco, out isOk, out errCode, out errMsg, out cpd, out sendData, out recData);
				try
				{
					if (!System.IO.Directory.Exists(@"D:\WLLOG"))
					{
						System.IO.Directory.CreateDirectory(@"D:\WLLOG");
					}
					System.IO.File.AppendAllText(@"D:\WLLOG\ShenTongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ShenTongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + sendData + "\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ShenTongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ShenTongCancel_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + recData + "\r\n");
					if (isOk == true)
					{
						item.WL_NUMBER = "已删" + item.WL_NUMBER;
						item.Status = Setings.EnumOrderStatus.停止完成;
					}

				}
				catch (Exception ee)
				{
					AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
				}

			}
		}

		public List<BllMod.Order> ServerGetOrderByErpid(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			bool isOk2 = false;
			int errCode2 = 0;
			string errMsg2 = "";
			List<MOD.BllMod.Order> res = new List<BllMod.Order>();
			if (VerDb_Local)
			{
				string sqlCmd = $@"SELECT Bid FROM BllMod_Order WHERE erpid='{order.ErpId}' AND Bid<>{order.Bid.ToString()} AND Status<>'停止完成' AND ISNULL(ServerTaskType,'')<>'终止订单'";
				System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
				if (dt.Rows.Count > 0)
				{
					foreach (System.Data.DataRow item in dt.Rows)
					{
						long t = YJT.DataBase.Common.ObjectTryToObj(item["Bid"], -1L);
						if (t != -1)
						{
							MOD.BllMod.Order t2 = ServerGetOrder(t.ToString(), out isOk2, out errCode2, out errMsg2);
							if (t2 != null && isOk2 == true)
							{
								res.Add(t2);
							}
						}

					}
					if (res.Count == 0)
					{
						errMsg = "单据不能获取到实力";
						errCode = -3;
					}
					else
					{
						isOk = true;
						errCode = 0;
						errMsg = "";
					}

				}
				else
				{
					errCode = -2;
					errMsg = "没有相关单据";
				}
				try
				{
					dt.Dispose();
					dt = null;
				}
				catch { }
			}
			else
			{
				errCode = -1;
				errMsg = "数据库连接失败";
			}



			return res;
		}

		public void ServerCopyLogicInfo(BllMod.Order orderTo, BllMod.Order orderForm, bool isNew, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (orderTo != null)
			{
				if (orderForm != null)
				{
					if (isNew)
					{
						orderTo.ADDRESS = orderForm.ADDRESS;
						orderTo.RECVNAME = orderForm.RECVNAME;
						orderTo.RECVPHONE = orderForm.RECVPHONE;
						orderTo.PROVINCENAME = orderForm.PROVINCENAME;
						orderTo.CITYNAME = orderForm.CITYNAME;
						orderTo.DISTRICTNAME = orderForm.DISTRICTNAME;
						orderTo.STREETNAME = orderForm.STREETNAME;
						orderTo.CUSTOMNAME = orderForm.CUSTOMNAME;
						orderTo.IsFp = orderForm.IsFp;
						orderTo.IsPj = orderForm.IsPj;
						orderTo.IsHeTong = orderForm.IsHeTong;
						orderTo.IsQysy = orderForm.IsQysy;
						orderTo.IsSyzz = orderForm.IsSyzz;
						orderTo.IsYjbb = orderForm.IsYjbb;
						orderTo.ORIGINALREMARK = orderForm.ORIGINALREMARK;
						orderTo.total_amt = orderForm.total_amt;
						orderTo.needBaojia = 0d;
						orderTo.PlatformType = orderForm.PlatformType;
					}
					else
					{
						if (YJT.Text.Verification.IsNotNullOrEmpty(orderForm.IsFp))
						{
							orderTo.IsFp = orderForm.IsFp;
						}
						if (YJT.Text.Verification.IsNotNullOrEmpty(orderForm.IsPj))
						{
							orderTo.IsPj = orderForm.IsPj;
						}
						if (YJT.Text.Verification.IsNotNullOrEmpty(orderForm.IsHeTong))
						{
							orderTo.IsHeTong = orderForm.IsHeTong;
						}
						if (YJT.Text.Verification.IsNotNullOrEmpty(orderForm.IsQysy))
						{
							orderTo.IsQysy = orderForm.IsQysy;
						}
						if (YJT.Text.Verification.IsNotNullOrEmpty(orderForm.IsSyzz))
						{
							orderTo.IsSyzz = orderForm.IsSyzz;
						}
						if (YJT.Text.Verification.IsNotNullOrEmpty(orderForm.IsYjbb))
						{
							orderTo.IsYjbb = orderForm.IsYjbb;
						}

						orderTo.ORIGINALREMARK = orderTo.ORIGINALREMARK + orderForm.ORIGINALREMARK;
						double t1Total_amt = 0d;
						if (double.TryParse(orderForm.total_amt, out t1Total_amt))
						{
							double t2Total_amt = 0d;
							if (!double.TryParse(orderTo.total_amt, out t2Total_amt))
							{
								t2Total_amt = 0d;
							}
							orderTo.total_amt = (t2Total_amt + t1Total_amt).ToString("#0.00");
						}
						orderTo.needBaojia = 0d;
					}
				}
				else
				{
					errMsg = "来源对象为空";
				}


			}
			else
			{
				errMsg = "目的对象为空";
			}
		}
		public void ServerClearEcInfo(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (order != null)
			{
				if (VerDb_Interface)
				{
					string sqlCmd = "";
					switch (order.PlatformType)
					{
						case Setings.EnumPlatformType.无:
							sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_companyid='',
	a.wl_number='',
	a.WL_COMPANYNAME='',
	a.WL_CREDATE=null
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={order.NETORDER_FROMID.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={order.ErpId}
	and a.customid={order.CUSTOMID}
	and a.agentname='{order.AGENTNAME}'
";
							break;
						case Setings.EnumPlatformType.药师帮:
							string[] wlNumbers = order.WL_NUMBER.Replace("已删", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
							string wlNumber = "";
							if (wlNumbers.Length > 0)
							{
								wlNumber = wlNumbers[0];
							}
							if (order.ServerTaskType == Settings.Setings.EnumServerTaskType.新单据)
							{
								sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_companyid='',
	a.wl_number='',
	a.WL_COMPANYNAME='',
	a.WL_CREDATE=null
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={order.NETORDER_FROMID.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={order.ErpId}
	and a.customid={order.CUSTOMID}
	and a.agentname='{order.AGENTNAME}'
	and a.WL_NUMBER='{wlNumber}'
";
							}
							else if (order.ServerTaskType == Settings.Setings.EnumServerTaskType.添加物流子单)
							{
								sqlCmd = $@"
delete
	NETORDER_DOC a
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={order.NETORDER_FROMID.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={order.ErpId}
	and a.customid={order.CUSTOMID}
	and a.agentname='{order.AGENTNAME}'
	and a.WL_NUMBER='{wlNumber}'
";
							}
							else
							{
								sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_companyid='',
	a.wl_number='',
	a.WL_COMPANYNAME='',
	a.WL_CREDATE=null
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={order.NETORDER_FROMID.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={order.ErpId}
	and a.customid={order.CUSTOMID}
	and a.agentname='{order.AGENTNAME}'
";
							}

								break;
						case Setings.EnumPlatformType.小药药:
							sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_companyid='',
	a.wl_number='',
	a.WL_COMPANYNAME='',
	a.WL_CREDATE=null
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={order.NETORDER_FROMID.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={order.ErpId}
	and a.customid={order.CUSTOMID}
	and a.agentname='{order.AGENTNAME}'
";
							break;
						case Setings.EnumPlatformType.药京采:
							sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_companyid='',
	a.wl_number='',
	a.WL_COMPANYNAME='',
	a.WL_CREDATE=null
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={order.NETORDER_FROMID.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={order.ErpId}
	and a.customid={order.CUSTOMID}
	and a.agentname='{order.AGENTNAME}'
";
							break;
						default:
							sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_companyid='',
	a.wl_number='',
	a.WL_COMPANYNAME='',
	a.WL_CREDATE=null
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={order.NETORDER_FROMID.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={order.ErpId}
	and a.customid={order.CUSTOMID}
	and a.agentname='{order.AGENTNAME}'
";
							break;
					}
					int dbres = _dbhInterface.ExecuteNonQuery(sqlCmd, null, System.Data.CommandType.Text, false, true, "");
					if (dbres > 0)
					{
						errCode = 0;
						errMsg = "";
						isOk = true;
					}
					else
					{
						errCode = -3;
						errMsg = "电商编号回写失败";
						isOk = false;
					}
				}
				else
				{
					errCode = -2;
					errMsg = "电商数据库连接失败";
					isOk = false;
				}

			}
			else
			{
				errCode = -1;
				errMsg = "回写的对象为null";
				isOk = false;
			}
		}
		public void ServerEcBcLogicInfo(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (order != null)
			{
				if (VerDb_Interface)
				{
					string sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_companyid={order.WL_COMPANYID.ToString()},
	a.wl_number='{order.WL_NUMBER}',
	a.WL_COMPANYNAME='{order.WL_COMPANYNAME}',
	a.WL_CREDATE=to_date('{order.logi_jinjianRiqi}','yyyy-mm-dd hh24:mi:ss')
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={order.NETORDER_FROMID.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={order.ERPORDER_ID}
	and a.customid={order.CUSTOMID}
	and a.agentname='{order.AGENTNAME}'
";
					int dbres = _dbhInterface.ExecuteNonQuery(sqlCmd, null, System.Data.CommandType.Text, false, true, "");
					if (dbres > 0)
					{
						errCode = 0;
						errMsg = "";
						isOk = true;
					}
					else
					{
						errCode = -3;
						errMsg = "电商编号回写失败";
						isOk = false;
					}
				}
				else
				{
					errCode = -2;
					errMsg = "电商数据库连接失败";
					isOk = false;
				}

			}
			else
			{
				errCode = -1;
				errMsg = "回写的对象为null";
				isOk = false;
			}

		}

		public void ServerCopyLogicInfo2(BllMod.Order orderTo, BllMod.Order orderForm, bool v, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = true;
			errCode = 0;
			errMsg = "";
			if (orderTo != null)
			{
				if (orderForm != null)
				{
					orderTo.Logic = orderForm.Logic;
					//orderNew.Weight = orderOld.Weight;
					//orderNew.Length = orderOld.Length;
					//orderNew.Width = orderOld.Width;
					//orderNew.Height = orderOld.Height;
					//orderTo.WmsDanjbh = orderForm.WmsDanjbh;
					//orderTo.NetDanjbh = orderForm.NetDanjbh;
					//orderTo.WmsYewy = orderForm.WmsYewy;
					//orderTo.WmsDdwname = orderForm.WmsDdwname;
					//orderTo.NETORDER_FROMID = orderForm.NETORDER_FROMID;
					//orderTo.NETORDER_FROM = orderForm.NETORDER_FROM;
					//orderTo.ERPORDER_ID = orderForm.ERPORDER_ID;
					//orderTo.CUSTOMID = orderForm.CUSTOMID;
					//orderTo.CUSTOMNAME = orderForm.CUSTOMNAME;
					//orderTo.AGENTNAME = orderForm.AGENTNAME;
					//orderTo.ADDRESS = orderForm.ADDRESS;
					orderTo.WL_COMPANYID = orderForm.WL_COMPANYID;
					orderTo.WL_COMPANYNAME = orderForm.WL_COMPANYNAME;
					orderTo.WL_NUMBER = orderForm.WL_NUMBER;
					//orderTo.RECVNAME = orderForm.RECVNAME;
					//orderTo.RECVPHONE = orderForm.RECVPHONE;
					//orderTo.PROVINCENAME = orderForm.PROVINCENAME;
					//orderTo.CITYNAME = orderForm.CITYNAME;
					//orderTo.DISTRICTNAME = orderForm.DISTRICTNAME;
					//orderTo.STREETNAME = orderForm.STREETNAME;
					//orderTo.ORIGINALREMARK = orderForm.ORIGINALREMARK;
					//orderTo.IsFp = orderForm.IsFp;
					//orderTo.IsPj = orderForm.IsPj;
					//orderTo.IsQysy = orderForm.IsQysy;
					//orderTo.IsSyzz = orderForm.IsSyzz;
					//orderTo.IsYjbb = orderForm.IsYjbb;
					//orderTo.PlatformType = orderForm.PlatformType;
					orderTo.logi_dstRoute = orderForm.logi_dstRoute;
					orderTo.logi_PayType = orderForm.logi_PayType;
					orderTo.logi_monAccNum = orderForm.logi_monAccNum;
					orderTo.logi_baojiaJine = orderForm.logi_baojiaJine;
					orderTo.logi_dsJine = orderForm.logi_dsJine;
					orderTo.logi_logcNum = orderForm.logi_logcNum;
					orderTo.logi_ysJine = orderForm.logi_ysJine;
					orderTo.logi_ysJineTotal = orderForm.logi_ysJineTotal;
					orderTo.logi_shouhuory = orderForm.logi_shouhuory;
					orderTo.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					orderTo.logi_shoufqianshu = orderForm.logi_shoufqianshu;
					orderTo.logi_shoufRiqi = orderForm.logi_shoufRiqi;
					orderTo.logi_sendSheng = orderForm.logi_sendSheng;
					orderTo.logi_sendShi = orderForm.logi_sendShi;
					orderTo.logi_sendXian = orderForm.logi_sendXian;
					orderTo.logi_sendAddress = orderForm.logi_sendAddress;
					orderTo.logi_sendMan = orderForm.logi_sendMan;
					orderTo.logi_sendPhone = orderForm.logi_sendPhone;
					orderTo.logi_feiyongTotal = orderForm.logi_feiyongTotal;
					orderTo.logi_goodQty = orderForm.logi_goodQty;
					orderTo.logi_goodName = orderForm.logi_goodName;
					orderTo.needBaojia = orderForm.needBaojia;
					orderTo.logi_OrderId = orderForm.logi_OrderId;
					orderTo.logi_CreateDate = orderForm.logi_CreateDate;
					orderTo.logi_ChanpinTypeStr = orderForm.logi_ChanpinTypeStr;
					//orderTo.sysFirst = orderForm.sysFirst;
					//orderTo.total_amt = orderForm.total_amt;
					//orderTo.fplx = orderForm.fplx;
					//orderTo.PAIDCOSTS = orderForm.PAIDCOSTS;
					orderTo.logi_ReceivePwd = orderForm.logi_ReceivePwd;
					//orderTo.logi_SubOrderSn = orderForm.logi_SubOrderSn;
					orderTo.logi_PhonNum = orderForm.logi_PhonNum;
					orderTo.logi_TelNum = orderForm.logi_TelNum;
					//orderTo.RelOrderId = orderForm.RelOrderId;
					orderTo.JingdongWl = orderForm.JingdongWl;
				}
			}

		}

		public void ServerCreateLogic2(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			bool isok2 = false;
			int errCode2 = -99;
			string errMsg2 = "";
			if (order != null)
			{
				if (YJT.Text.Verification.IsNotNullOrEmpty(order.ADDRESS))
				{
					//检查物流信息是否正确
					bool logicStatus = false;
					YJT.BaiduService.BaiduMap.GetAddressDetalRes info = PubGetAddressInfo(order.ADDRESS, out isok2, out errCode2, out errMsg2);
					if (isok2 != true)
					{
						AddMsgOut("通过百度API没有正确获得地址信息", Settings.Setings.EnumMessageType.提示, errCode2, errMsg2, order.ADDRESS);
					}
					if (YJT.Text.Verification.IsNullOrEmpty(order.RECVNAME))
					{
						errCode = -3;
						isOk = false;
						order.ErrMsg = "订单没有收货人";
						errMsg = order.ErrMsg;
						order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
					}
					else
					{

						if (YJT.Text.Verification.IsNullOrEmpty(order.RECVPHONE))
						{
							errCode = -4;
							isOk = false;
							order.ErrMsg = "订单没有收货人电话";
							errMsg = order.ErrMsg;
							order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
						}
						else
						{
							string input = order.RECVPHONE.Trim();
							string phoneNum = "";
							string telNum = "";
							int fillNum = 0;
							System.Text.RegularExpressions.MatchCollection phones = _regPhoneFor.Matches(input);
							if (phones.Count > 0)
							{
								for (int i = 0; i < ((phones.Count > 2) ? 2 : phones.Count); i++)
								{
									if (fillNum > 1)
									{
										break;
									}
									else if (fillNum == 0)
									{
										phoneNum = phones[i].Groups[1].Value;
									}
									else
									{
										telNum = phones[i].Groups[1].Value;
									}
									fillNum++;

								}
							}
							if (fillNum < 2)
							{
								System.Text.RegularExpressions.MatchCollection tels = _regTelFor.Matches(input);
								if (tels.Count > 0)
								{
									for (int i = 0; i < ((tels.Count > 2) ? 2 : tels.Count); i++)
									{
										if (fillNum > 1)
										{
											break;
										}
										else if (fillNum == 0)
										{
											phoneNum = tels[i].Groups[1].Value;
										}
										else
										{
											telNum = tels[i].Groups[1].Value;
										}
										fillNum++;

									}
								}
							}
							if (YJT.Text.Verification.IsNullOrEmpty(telNum))
							{
								telNum = phoneNum;
							}
							if (YJT.Text.Verification.IsNullOrEmpty(phoneNum))
							{
								errCode = -5;
								isOk = false;
								order.ErrMsg = "没有分析到正确的电话号码";
								errMsg = order.ErrMsg;
								order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
							}
							else
							{
								order.logi_PhonNum = phoneNum;
								order.logi_TelNum = telNum;
								if (YJT.Text.Verification.IsNullOrEmpty(order.PROVINCENAME))
								{
									order.PROVINCENAME = info.result.addressComponent.province;
								}
								if (YJT.Text.Verification.IsNullOrEmpty(order.CITYNAME))
								{
									order.CITYNAME = info.result.addressComponent.city;
								}
								if (YJT.Text.Verification.IsNullOrEmpty(order.DISTRICTNAME))
								{
									order.DISTRICTNAME = info.result.addressComponent.district;
								}
								if (YJT.Text.Verification.IsNullOrEmpty(order.STREETNAME))
								{
									order.STREETNAME = info.result.addressComponent.street;
								}
								logicStatus = true;
							}

						}
					}
					if (logicStatus == true)
					{
						if (order.Logic == Settings.Setings.EnumLogicType.Default)
						{
							//开始计算
							if (order.Weight <= 3.0)
							{
								order.Logic = Settings.Setings.EnumLogicType.申通快递;
							}
							else
							{
								//if (order.PlatformType == Setings.EnumPlatformType.小药药 || order.PlatformType == Setings.EnumPlatformType.药京采)
								if (YJT.Text.Verification.IsLikeIn(order.PROVINCENAME, new List<string>() { "北京", "天津", "河北" }, true))
								{
									order.Logic = Settings.Setings.EnumLogicType.邮政EMS;
								}
								else
								{
									order.Logic = Setings.EnumLogicType.顺丰;
								}
							}



						}
						///设置发货人
						order.logi_sendAddress = @"河北省保定市莲池区北三环801号";
						order.logi_sendSheng = @"河北省";
						order.logi_sendShi = @"保定市";
						order.logi_sendXian = @"莲池区";
						if (order.PlatformType == Setings.EnumPlatformType.药师帮)
						{
							order.logi_sendMan = "刘聪聪";
							order.logi_sendPhone = "13931252550";
						}
						else if (order.PlatformType == Setings.EnumPlatformType.小药药)
						{
							order.logi_sendMan = "杨雪丽";
							order.logi_sendPhone = "13930270307";
						}
						else if (order.PlatformType == Setings.EnumPlatformType.药京采)
						{
							order.logi_sendMan = "赵楠";
							order.logi_sendPhone = "15097755954";
						}
						else
						{
							order.Logic = Setings.EnumLogicType.Default;
						}
						switch (order.Logic)
						{
							case Settings.Setings.EnumLogicType.顺丰:
								{
									YJT.Logistics.ShunFengLogistic.ClassCreateOrder.CargoDetailsClass 货品 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.CargoDetailsClass()
									{
										count = 1,
										name = "药品",
										specifications = "药品",
										weight = order.Weight
									};
									YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass 发件方 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass()
									{
										address = order.logi_sendAddress,
										city = order.logi_sendShi,
										company = "河北燕都医药物流有限公司",
										contact = order.logi_sendMan,
										contactType = 1,
										county = order.logi_sendXian,
										mobile = order.logi_sendPhone,
										province = order.logi_sendSheng,
										tel = order.logi_sendPhone
									};
									YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass 收件方 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass()
									{
										address = order.ADDRESS,
										city = order.CITYNAME,
										company = order.CUSTOMNAME,
										contact = order.RECVNAME,
										contactType = 2,
										county = order.DISTRICTNAME,
										mobile = order.logi_PhonNum,
										province = order.PROVINCENAME,
										tel = order.logi_TelNum

									};
									string 备注 = "";
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
									{
										备注 = 备注 + "发票";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
									{
										备注 = 备注 + " 货品批件";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
									{
										备注 = 备注 + " 企业首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
									{
										备注 = 备注 + " 货品首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
									{
										备注 = 备注 + " 药检报告";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
									{
										备注 = 备注 + " 购销合同";
									}
									YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass 增值服务 = YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.CtorService(YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.enum增值服务类别.配送服务, null);
									//增值服务 = null;
									YJT.Logistics.ShunFengLogistic.ClassCreateOrder a = _sf.Ctor_CreateOrderObj(order.ERPORDER_ID, 发件方, 收件方, 增值服务, 货品, 备注, "13", YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型.顺丰标快, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式.寄付月结);
									double tTotal_amt = 0d;
									if (!double.TryParse(order.total_amt, out tTotal_amt))
									{
										tTotal_amt = 0d;
									}
									if (tTotal_amt != 0d)
									{
										if (tTotal_amt > 0)
										{
											//int b = (int)(tTotal_amt / 500);
											//double c = tTotal_amt % 500;
											//if (c > 0)
											//{
											//	b = b + 1;
											//}
											//order.needBaojia = b * 500;
											//order.total_amt = tTotal_amt.ToString("#0.00");
											if (tTotal_amt < 500)
											{
												order.needBaojia = 500;
											}
											else
											{
												order.needBaojia = 1000;
											}
											order.total_amt = tTotal_amt.ToString("#0.00");
										}
									}
									if (order.needBaojia > 0)
									{
										YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass 保价服务 = YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.CtorService(YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.enum增值服务类别.保价, order.needBaojia.ToString());
										a.AddserviceList(保价服务);
									}
									else
									{
										order.needBaojia = 0;
									}
									string stxt = "";
									string otxt = "";
									YJT.Logistics.ShunFengLogistic.ClassCreateOrderRes res = _sf.CreateOrder(a, out isOk, out errCode, out errMsg, out stxt, out otxt);
									try
									{
										if (!System.IO.Directory.Exists(@"D:\WLLOG"))
										{
											System.IO.Directory.CreateDirectory(@"D:\WLLOG");
										}
										System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

									}
									catch (Exception ee)
									{
										AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
									}

									if (isOk == true && YJT.Text.Verification.IsNotNullOrEmpty(res.data))
									{
										order.WL_COMPANYID = ((int)order.Logic).ToString();
										order.WL_COMPANYNAME = order.Logic.ToString();
										order.WL_NUMBER = res.data;
										order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
										order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
										order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
										YJT.Logistics.ShunFengLogistic.ClassGetOrderInfoRes res2 = _sf.GetOrderInfo(order.ERPORDER_ID, out isOk, out errCode, out errMsg);
										order.ErrMsg = "物流信息获取完成";
										order.logi_baojiaJine = res2.data2.insureFee;
										order.logi_dsJine = res2.data2.codValue;
										order.logi_dstRoute = res2.data2.destRouteLabel;
										order.logi_feiyongTotal = res2.data2.totalFee;
										order.logi_goodName = res2.data2.cargo;
										order.logi_goodQty = res2.data2.cargoCount;
										order.logi_logcNum = res2.data2.codMonthAccount;
										order.logi_monAccNum = res2.data2.monthAccount;
										order.logi_sendAddress = res2.data2.deliverAddress;
										order.logi_sendMan = res2.data2.deliverName;
										order.logi_sendPhone = res2.data2.deliverTel;
										order.logi_sendSheng = res2.data2.deliverProvince;
										order.logi_sendShi = res2.data2.deliverCity;
										order.logi_sendXian = res2.data2.deliverCounty;
										order.logi_shoufqianshu = res2.data2.returnTrackingNo;
										order.logi_shoufRiqi = "";
										order.logi_shouhuory = "";
										order.logi_ChanpinTypeStr = res2.data2.expressType;
										YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型 t = YJT.DataBase.Common.ObjectTryToObj(order.logi_ChanpinTypeStr, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型.错误);
										order.logi_ChanpinTypeStr = t.ToString();


										order.logi_PayType = res2.data2.payMethod;//坑
										YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式 t2 = YJT.DataBase.Common.ObjectTryToObj(order.logi_PayType, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式.错误);
										order.logi_PayType = t2.ToString();

										order.logi_OrderId = res2.data2.orderNo;
										double total = 0;
										double baojia = 0;
										double yunfei = 0;
										if (!double.TryParse(order.logi_feiyongTotal, out total))
										{
											total = 0;
										}
										if (!double.TryParse(order.logi_baojiaJine, out baojia))
										{
											baojia = 0;
										}
										yunfei = total;
										if (yunfei <= 0)
										{
											order.logi_ysJine = yunfei.ToString("#0.000");
											order.logi_ysJineTotal = order.logi_ysJine;
										}
										else
										{
											order.logi_ysJine = "";
											order.logi_ysJineTotal = "";
										}
										isOk = true;
										errCode = 0;
										errMsg = "";



									}
									else
									{
										isOk = false;
										errCode = -6;
										order.ErrMsg = "顺丰物流下单不成功:" + res.data;
										errMsg = order.ErrMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
									}
									break;
								}
							case Setings.EnumLogicType.邮政EMS:
								{
									string 备注 = "";
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
									{
										备注 = 备注 + "发票";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
									{
										备注 = 备注 + " 货品批件";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
									{
										备注 = 备注 + " 企业首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
									{
										备注 = 备注 + " 货品首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
									{
										备注 = 备注 + " 药检报告";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
									{
										备注 = 备注 + " 购销合同";
									}
									order.needBaojia = 0d;
									YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.Cargo 药品info = _emsYz.GetSubModCargo(
										cargo_category: "",
										cargo_name: "药品",
										cargo_quantity: 0,
										cargo_value: 1,
										cargo_length: 0, cargo_width: 0, cargo_high: 0,
										cargo_weight: 0,
										cargo_order_no: "");
									YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.ClassAddress 发件方 = _emsYz.GetSubModAddress
										(
										 address: order.logi_sendAddress,
										 prov: order.logi_sendSheng,
										 city: order.logi_sendShi,
										 county: order.logi_sendXian,
										 name: order.logi_sendMan,
										 mobile: order.logi_sendPhone,
										 phone: order.logi_sendPhone,
										 post_code: "");
									YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.ClassAddress 收件方 = _emsYz.GetSubModAddress(
										address: order.ADDRESS,
										 prov: order.PROVINCENAME,
										 city: order.CITYNAME,
										 county: order.DISTRICTNAME,
										 name: order.RECVNAME,
										 mobile: order.logi_PhonNum,
										 phone: order.logi_TelNum,
										 post_code: ""
										);
									order.logi_ReceivePwd = YJT.Text.ClassCreateText.FunStrCreateNumberStr(6, null);
									YJT.Logistics.YouZhengEms.ClassYouZhengEmsMod 取号 = _emsYz.GetModGetEmsOrder(
										ecommerce_user_id: order.PlatformType.ToString(),
										erpid: order.ERPORDER_ID,
										length: order.Length, width: order.Width, height: order.Height, weight: order.Weight,
										sender: 发件方,
										receiver: 收件方,
										cargos: new YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.Cargo[] { 药品info },
										batch_no: "",
										one_bill_fee_type: 0,
										contents_attribute: 3,
										base_product_no: "1",
										biz_product_no: "1",
										cod_amount: 0,
										cod_flag: 9,
										deliver_type: 2,
										electronic_preferential_amount: 0,
										electronic_preferential_no: "",
										insurance_amount: 0,//order.needBaojia,
										insurance_flag: 2,
										insurance_premium_amount: 0,
										note: "",
										payment_mode: 1,
										pickup_notes: 备注,
										pickup_type: 1,
										postage_total: 0,
										receipt_flag: 1,
										receiver_safety_code: order.logi_ReceivePwd,
										sender_safety_code: "",
										valuable_flag: 0,
										waybill_no: "",
										submail_no: ""
									);
									string stxt = "";
									string otxt = "";
									YJT.Logistics.YouZhengEms.ClassResBase resobj = null;
									int tryCountEms = 5;
									while (tryCountEms > 0)
									{
										tryCountEms--;
										resobj = _emsYz.SendGetEmsOrder(取号, out isOk, out errCode, out errMsg, out stxt, out otxt);
										try
										{
											if (!System.IO.Directory.Exists(@"D:\WLLOG"))
											{
												System.IO.Directory.CreateDirectory(@"D:\WLLOG");
											}
											System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");
										}
										catch (Exception ee)
										{
											AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
										}
										if (isOk == true)
										{
											break;
										}
										else
										{
											System.Threading.Thread.Sleep(1000);
										}

									}
									
									if (resobj != null)
									{
										if (isOk == true)
										{
											YJT.Logistics.YouZhengEms.Classoms_ordercreate_waybillnoRes 下单结果 = resobj as YJT.Logistics.YouZhengEms.Classoms_ordercreate_waybillnoRes;
											if (下单结果 != null)
											{
												order.WL_COMPANYID = ((int)order.Logic).ToString();
												order.WL_COMPANYNAME = order.Logic.ToString();
												order.WL_NUMBER = 下单结果.body.waybill_no;
												order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
												order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
												order.ErrMsg = "创建物流订单完成";
												order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
												order.logi_dstRoute = 下单结果.body.routeCode;
												order.logi_PayType = "寄件人";//payment_mode
												order.logi_monAccNum = "";
												order.logi_baojiaJine = order.needBaojia.ToString("#0.00");
												order.logi_dsJine = "";
												order.logi_logcNum = "";
												order.logi_ysJine = "0.00";
												order.logi_ysJineTotal = "0.00";
												order.logi_shouhuory = "";
												order.logi_shoufqianshu = "";
												order.logi_shoufRiqi = "";
												order.logi_sendSheng = 发件方.prov;
												order.logi_sendShi = 发件方.city;
												order.logi_sendXian = 发件方.county;
												order.logi_sendAddress = 发件方.address;
												order.logi_sendMan = 发件方.name;
												order.logi_sendPhone = 发件方.mobile;
												order.logi_feiyongTotal = "0";
												order.logi_goodQty = "0";
												order.logi_goodName = "药品";
												order.logi_ChanpinTypeStr = "特快专递";//biz_product_no
												isOk = true;
												errCode = 0;
												errMsg = "";
											}
											else
											{
												//返回的内容不是下单取号的结果类
												isOk = false;
												errCode = -9;
												order.ErrMsg = "邮政物流下单不成功:返回的对象不是Classoms_ordercreate_waybillnoRes";
												errMsg = order.ErrMsg;
												order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											}
										}
										else
										{
											//下单不成功
											isOk = false;
											errCode = -8;
											order.ErrMsg = "邮政物流下单不成功:" + errMsg;
											errMsg = order.ErrMsg;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										}
									}
									else
									{
										isOk = false;
										errCode = -7;
										order.ErrMsg = "邮政物流下单不成功:提交过来的对象为NULL";
										errMsg = order.ErrMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
									}
									break;
								}
							case Settings.Setings.EnumLogicType.极兔百事:
								{
									order.ErrMsg = "物流公司极兔百事不支持";
									order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
									break;
								}
							case Settings.Setings.EnumLogicType.京东物流:
								{
									string 备注2 = "";
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
									{
										备注2 = 备注2 + "发票";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
									{
										备注2 = 备注2 + " 货品批件";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
									{
										备注2 = 备注2 + " 企业首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
									{
										备注2 = 备注2 + " 货品首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
									{
										备注2 = 备注2 + " 药检报告";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
									{
										备注2 = 备注2 + " 购销合同";
									}
									YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrder createClass = new YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrder("0030001", order.ErpId, order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.RECVNAME, order.ADDRESS, order.logi_TelNum, 1, order.Weight, 1)
									{
										province = order.PROVINCENAME,
										city = order.CITYNAME,
										county = order.DISTRICTNAME,
										town = order.STREETNAME,
										description = "药品",
										senderMobile = order.logi_sendPhone,
										senderTel = order.logi_sendPhone,

										receiveMobile = order.logi_PhonNum,
										receiveTel = order.logi_TelNum,

										remark = 备注2
									};
									string stxt2 = "";
									string otxt2 = "";
									YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrderRes res3 = _jdWl.CreateOrder(createClass, out isOk, out errCode, out errMsg, out stxt2, out otxt2);
									try
									{
										if (!System.IO.Directory.Exists(@"D:\WLLOG"))
										{
											System.IO.Directory.CreateDirectory(@"D:\WLLOG");
										}
										System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt2 + "\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\JingdongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt2 + "\r\n");

									}
									catch (Exception ee)
									{
										AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
									}
									if (isOk == true && YJT.Text.Verification.IsNotNullOrEmpty(res3.deliveryId))
									{
										order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
										order.WL_COMPANYID = ((int)order.Logic).ToString();
										order.WL_COMPANYNAME = order.Logic.ToString();
										order.WL_NUMBER = res3.deliveryId;
										order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
										order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
										order.ErrMsg = "创建物流订单完成";
										order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

										order.logi_baojiaJine = "0.00";
										order.logi_dsJine = "0.00";
										order.logi_feiyongTotal = 0.ToString("#0.00");
										order.logi_goodName = createClass.description;
										order.logi_goodQty = createClass.packageCount.ToString();
										order.logi_logcNum = "";
										order.logi_monAccNum = "";
										order.logi_sendAddress = order.logi_sendAddress;
										order.logi_sendMan = order.logi_sendMan;
										order.logi_sendSheng = order.logi_sendSheng;
										order.logi_sendShi = order.logi_sendShi;
										order.logi_sendXian = order.logi_sendXian;
										order.logi_shoufqianshu = "";
										order.logi_shoufRiqi = "";
										order.logi_shouhuory = "";
										order.logi_ChanpinTypeStr = "纯配";
										order.logi_PayType = "月结";//坑
										order.logi_OrderId = res3.deliveryId;

										order.logi_dstRoute = "";
										if (res3.preSortResult != null)
										{

											order.logi_dstRoute = res3.preSortResult.sourceSortCenterId.Value.ToString() + "-" + res3.preSortResult.sourceSortCenterName + "-|-" + res3.preSortResult.targetSortCenterId.Value.ToString() + "-" + res3.preSortResult.targetSortCenterName;
											order.JingdongWl = res3.preSortResult.aging.Value.ToString() + "@<|||>@\n" +
												res3.preSortResult.agingName.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.collectionAddress.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.coverCode.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.distributeCode.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.isHideContractNumbers.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.isHideName.ToString().Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.qrcodeUrl.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.road.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.siteId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.siteName.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.siteType.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.slideNo.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.sourceCrossCode.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.sourceSortCenterId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.sourceSortCenterName.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.sourceTabletrolleyCode.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.targetSortCenterId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.targetSortCenterName.Replace("'", "") + "@<|||>@\n" +
												res3.preSortResult.targetTabletrolleyCode.Replace("'", "") + "@<|||>@\n";
										}
										double total = 0;
										double baojia = 0;
										double yunfei = 0;
										if (!double.TryParse(order.logi_feiyongTotal, out total))
										{
											total = 0;
										}
										if (!double.TryParse(order.logi_baojiaJine, out baojia))
										{
											baojia = 0;
										}
										yunfei = total;
										if (yunfei <= 0)
										{
											order.logi_ysJine = yunfei.ToString("#0.000");
											order.logi_ysJineTotal = order.logi_ysJine;
										}
										else
										{
											order.logi_ysJine = "";
											order.logi_ysJineTotal = "";
										}




									}
									else
									{
										order.ErrMsg = "京东物流下单不成功:" + errMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										isOk = false;
										errMsg = order.ErrMsg;
										errCode = -9;
									}
									break;
								}
							case Settings.Setings.EnumLogicType.Default:
								{
									order.ErrMsg = "物流公司选择不确认或者平台信息错误";
									order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
									break;
								}
							case Setings.EnumLogicType.中通快递:
								{

									bool isErrorFlag = false;
									string 备注 = "";
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
									{
										备注 = 备注 + "发票";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
									{
										备注 = 备注 + " 货品批件";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
									{
										备注 = 备注 + " 企业首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
									{
										备注 = 备注 + " 货品首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
									{
										备注 = 备注 + " 药检报告";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
									{
										备注 = 备注 + " 购销合同";
									}
									YJT.Logistics.ZhongTongLogistics.ClassCreateOrder cco = YJT.Logistics.ZhongTongLogistics.ClassCreateOrder.Intor(order.ERPORDER_ID, "2", "1", _ztkdWl, out isOk, out errMsg, 备注);
									if (isOk == false)
									{
										isOk = false;
										errCode = -6;
										order.ErrMsg = "创建下单对象失败:" + errMsg;
										errMsg = order.ErrMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										isErrorFlag = true;
									}
									if (!cco.CreateAccountInfo(out errMsg, 1))
									{
										isOk = false;
										errCode = -6;
										order.ErrMsg = "账户信息创建错误:" + errMsg;
										errMsg = order.ErrMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										isErrorFlag = true;
									}
									if (!cco.CreateSenderInfo(order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.logi_sendSheng, order.logi_sendShi, order.logi_sendXian, out errMsg))
									{
										isOk = false;
										errCode = -6;
										order.ErrMsg = "账户发件信息错误:" + errMsg;
										errMsg = order.ErrMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										isErrorFlag = true;

									}
									if (!cco.CreateReceiveInfo(order.RECVNAME, order.ADDRESS, order.logi_PhonNum, order.PROVINCENAME, order.CITYNAME, order.DISTRICTNAME, out errMsg, order.logi_TelNum))
									{
										isOk = false;
										errCode = -6;
										order.ErrMsg = "账户收件信息错误:" + errMsg;
										errMsg = order.ErrMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										isErrorFlag = true;

									}
									cco.CreateSummaryInfo(quantity: 1);
									if (!cco.CreateOrderItems(errMsg: out errMsg, name: "药品", weight: (long)order.Weight, quantity: 1))
									{
										isOk = false;
										errCode = -6;
										order.ErrMsg = "药品信息错误:" + errMsg;
										errMsg = order.ErrMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										isErrorFlag = true;
									}
									double tTotal_amt = 0d;
									if (!double.TryParse(order.total_amt, out tTotal_amt))
									{
										tTotal_amt = 0d;
									}
									order.needBaojia = 0;
									order.total_amt = tTotal_amt.ToString("#0.00");
									string stxt = "";
									string otxt = "";
									if (isErrorFlag == false)
									{
										YJT.Logistics.ZhongTongLogistics.ClassCreateOrderRes r1 = _ztkdWl.CreateOrder(cco, out isOk, out errMsg, out errCode, out stxt, out otxt);
										try
										{
											if (!System.IO.Directory.Exists(@"D:\WLLOG"))
											{
												System.IO.Directory.CreateDirectory(@"D:\WLLOG");
											}
											System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

										}
										catch (Exception ee)
										{
											AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
										}
										if (r1 != null && isOk == true)
										{
											order.WL_COMPANYID = ((int)order.Logic).ToString();
											order.WL_COMPANYNAME = order.Logic.ToString();
											order.WL_NUMBER = r1.result.billCode;
											order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
											order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
											order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
											order.ErrMsg = "物流信息获取完成";
											order.logi_baojiaJine = "0.00";
											order.logi_dsJine = "0.00";
											order.logi_dstRoute = r1.result.bigMarkInfo.mark;
											order.logi_feiyongTotal = "0.00";
											order.logi_goodName = "药品";
											order.logi_goodQty = "1";
											order.logi_logcNum = "";
											order.logi_monAccNum = "";
											order.logi_shoufqianshu = "";
											order.logi_shoufRiqi = "";
											order.logi_shouhuory = "";
											order.logi_ChanpinTypeStr = "非集团客户-全网件";
											order.logi_PayType = "";
											order.logi_OrderId = r1.result.orderCode;
											order.JingdongWl = r1.result.siteName + "转" + r1.result.bigMarkInfo.bagAddr;
											double total = 0;
											double baojia = 0;
											double yunfei = 0;
											if (!double.TryParse(order.logi_feiyongTotal, out total))
											{
												total = 0;
											}
											if (!double.TryParse(order.logi_baojiaJine, out baojia))
											{
												baojia = 0;
											}
											yunfei = total;
											if (yunfei <= 0)
											{
												order.logi_ysJine = yunfei.ToString("#0.000");
												order.logi_ysJineTotal = order.logi_ysJine;
											}
											else
											{
												order.logi_ysJine = "";
												order.logi_ysJineTotal = "";
											}
											isOk = true;
											errCode = 0;
											errMsg = "";
										}
										else
										{
											isOk = false;
											errCode = -6;
											order.ErrMsg = "中通物流下单不成功:" + errMsg;
											errMsg = order.ErrMsg;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										}
									}


									//YJT.Logistics.ZhongTongLogistics.ClassGetOrderInfo cgoi = new YJT.Logistics.ZhongTongLogistics.ClassGetOrderInfo(1, null, r1.result.billCode);
									//YJT.Logistics.ZhongTongLogistics.ClassGetOrderInfoRes r2 = _ztkdWl.GetOrderInfo(cgoi, out isOk, out errMsg, out errCode, out sd, out rd);
									//YJT.Logistics.ZhongTongLogistics.ClassCancelPreOrder ccpo = new YJT.Logistics.ZhongTongLogistics.ClassCancelPreOrder(1, null, r1.result.billCode);
									//_ztkdWl.CancelPreOrder(ccpo, out isOk, out errMsg, out errCode, out sd, out rd);
									break;
								}
							case Setings.EnumLogicType.申通快递:
								{
									string 备注 = "";
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
									{
										备注 = 备注 + "发票";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
									{
										备注 = 备注 + " 货品批件";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
									{
										备注 = 备注 + " 企业首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
									{
										备注 = 备注 + " 货品首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
									{
										备注 = 备注 + " 药检报告";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
									{
										备注 = 备注 + " 购销合同";
									}





									double tTotal_amt = 0d;
									if (!double.TryParse(order.total_amt, out tTotal_amt))
									{
										tTotal_amt = 0d;
									}
									order.needBaojia = 0;
									order.total_amt = tTotal_amt.ToString("#0.00");
									string stxt = "";
									string otxt = "";
									order.logi_sendMan = "赵志强-" + order.logi_sendMan;
									YJT.Logistics.ShenTongLogistic.ClassCreateOrder cco = _shenTongWl.IntorCreateOrder(
										order.ERPORDER_ID, YJT.Logistics.ShenTongLogistic.ClassCreateOrder.EnumBillType.普通00,
										order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.logi_sendSheng, order.logi_sendShi, order.logi_sendXian,
										order.RECVNAME, order.ADDRESS, order.logi_PhonNum, order.PROVINCENAME, order.CITYNAME, order.DISTRICTNAME,
										"药品", recTel: order.logi_TelNum, weight: order.Weight, battery: YJT.Logistics.ShenTongLogistic.ClassCreateOrder.Cargo.EnumBattery.不带电30, ps: 备注
									);
									YJT.Logistics.ShenTongLogistic.ClassPostData cpd = null;
									YJT.Logistics.ShenTongLogistic.ClassCreateOrderRes r1 = null;
									int tryCount = 5;
									while (tryCount > 0)
									{
										tryCount--;
										r1 = _shenTongWl.CreateOrder(cco, out isOk, out errCode, out errMsg, out cpd, out stxt, out otxt);
										try
										{
											if (!System.IO.Directory.Exists(@"D:\WLLOG"))
											{
												System.IO.Directory.CreateDirectory(@"D:\WLLOG");
											}
											System.IO.File.AppendAllText(@"D:\WLLOG\ShenTongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\ShenTongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\ShenTongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\ShenTongCombo_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

										}
										catch (Exception ee)
										{
											AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
										}
										if (isOk ==true)
										{
											break;
										}
										else
										{
											System.Threading.Thread.Sleep(1000);
										}
									}
									
									if (r1 != null && isOk == true)
									{
										order.WL_COMPANYID = ((int)order.Logic).ToString();
										order.WL_COMPANYNAME = order.Logic.ToString();
										order.WL_NUMBER = r1.data.waybillNo;
										order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
										order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
										order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
										order.ErrMsg = "物流信息获取完成";
										order.logi_baojiaJine = "0.00";
										order.logi_dsJine = "0.00";
										order.logi_dstRoute = r1.data.bigWord;
										order.logi_feiyongTotal = "0.00";
										order.logi_goodName = "药品";
										order.logi_goodQty = "1";
										order.logi_logcNum = "";
										order.logi_monAccNum = "";
										order.logi_shoufqianshu = "";
										order.logi_shoufRiqi = "";
										order.logi_shouhuory = "";
										order.logi_ChanpinTypeStr = YJT.Logistics.ShenTongLogistic.ClassCreateOrder.EnumBillType.普通00.ToString();
										order.logi_PayType = "";
										order.logi_OrderId = r1.data.orderNo;
										order.JingdongWl = r1.data.packagePlace;
										double total = 0;
										double baojia = 0;
										double yunfei = 0;
										if (!double.TryParse(order.logi_feiyongTotal, out total))
										{
											total = 0;
										}
										if (!double.TryParse(order.logi_baojiaJine, out baojia))
										{
											baojia = 0;
										}
										yunfei = total;
										if (yunfei <= 0)
										{
											order.logi_ysJine = yunfei.ToString("#0.000");
											order.logi_ysJineTotal = order.logi_ysJine;
										}
										else
										{
											order.logi_ysJine = "";
											order.logi_ysJineTotal = "";
										}
										isOk = true;
										errCode = 0;
										errMsg = "";
									}
									else
									{
										isOk = false;
										errCode = -6;
										order.ErrMsg = "申通物流下单不成功:" + errMsg;
										errMsg = order.ErrMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
									}
									break;
								}
							default:
								{
									order.ErrMsg = "暂不支持其他物流";
									order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
									break;
								}

						}
					}

				}
				else
				{
					//没有地址信息
					errCode = -2;
					order.ErrMsg = "平台地址信息为空";
					errMsg = order.ErrMsg;
					isOk = false;
					order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
				}

			}
			else
			{
				errCode = -1;
				errMsg = "创建物流时,订单为NULL";
				isOk = false;
				AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
			}
		}



		public bool GetComplatePrintInfo(string erpId, ref PrintObj po, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(erpId))
			{
				if (po != null)
				{
					if (po.ErpId == erpId)
					{
						string sqlCmd = $@"
select
	aaa.BussinesDep,
	aaa.LineRoute,
	aaa.WmsWareBH,
	aaa.WmsWareClass,
	aaa.PihaoS,
	wmsys.wm_concat('['||aaa.GoodsClass ||']有'|| aaa.gcCount||'个') GoodsClass
from
(
	select
		cc.deptname BussinesDep,
		cc.routename||'—'||cc.WAVEID LineRoute, 
		80000000+cc.wavedtlid WmsWareBH,
		WMS_HYD.getyjdprtclass(cc.prtclass) WmsWareClass,
		case
			when nvl(bb.jkhzflag,-99)=1 then '国产'
			when nvl(bb.jkhzflag,-99)=2 then '合资'
			when nvl(bb.jkhzflag,-99)=3 then '进口'
		end as GoodsClass,
		count(1) gcCount,
		wmsys.wm_concat(distinct '品名:'||bb.goodsname ||' 批号:'||bb.lotno) PihaoS

	from
		wms_out_order_v aa
		inner join wms_wave_goods_dtl_v bb on aa.wavedtlid=bb.wavedtlid
		inner join wms_wave_dtl_v cc on cc.wavedtlid=aa.wavedtlid
	where
		aa.srcexpno ='{po.ErpId}'
	group by
		cc.deptname,
		cc.routename||'—'||cc.WAVEID, 
		80000000+cc.wavedtlid,
		WMS_HYD.getyjdprtclass(cc.prtclass),
		case
			when nvl(bb.jkhzflag,-99)=1 then '国产'
			when nvl(bb.jkhzflag,-99)=2 then '合资'
			when nvl(bb.jkhzflag,-99)=3 then '进口'
		end
) aaa
group by
	aaa.BussinesDep,
	aaa.LineRoute,
	aaa.WmsWareBH,
	aaa.WmsWareClass,
	aaa.PihaoS
";
						System.Data.DataTable dt = _dbhWms.ExecuteToDataTable(sqlCmd, null, true);
						if (dt.Rows.Count > 0)
						{
							po.BussinesDep = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["BussinesDep"], "");
							po.LineRoute = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["LineRoute"], "");
							po.WmsWareBH = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WmsWareBH"], "");
							po.WmsWareClass= YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WmsWareClass"], "");
							po.GoodsClass = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["GoodsClass"], "");
							po.PihaoS= YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["PihaoS"], "");
							isOk = true;
						}
						
						try
						{
							dt.Dispose();
						}
						catch { }
						dt = null;
					}
				}
			}

			return isOk;
		}


		public List<ModGoodsInfo> GetGoodsLdyjPic(string erpid, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			List<ModGoodsInfo> res = new List<ModGoodsInfo>();
			if (VerDb_Wms == true)
			{
				string sqlCmd = $@"
select
	bb.goodsname,
	bb.lotno,
	bb.bmslotid folderName,
	bb.goodsid
from
	wms_out_order_v aa
	inner join wms_wave_goods_dtl_v bb on aa.wavedtlid=bb.wavedtlid
	inner join wms_wave_dtl_v cc on cc.wavedtlid=aa.wavedtlid
where
	aa.srcexpno='{erpid}'
";
				System.Data.DataTable dt = _dbhWms.ExecuteToDataTable(sqlCmd, null, true);
				if (dt.Rows.Count > 0)
				{
					foreach (System.Data.DataRow item in dt.Rows)
					{
						ModGoodsInfo t = new ModGoodsInfo()
						{
							FileGroupId = YJT.DataBase.Common.ObjectTryToObj(item["folderName"], ""),
							GoodsId = YJT.DataBase.Common.ObjectTryToObj(item["goodsid"], ""),
							GoodsName = YJT.DataBase.Common.ObjectTryToObj(item["goodsname"], ""),
							Xiaoqizhi = null,
							FileSopcode = null
							 
						};
						if (YJT.Text.Verification.IsNotNullOrEmpty(t.FileGroupId))
						{
							List<MOD.Mod_bllFileHandle> p = FileHandle.GetWmsGoodsLdyjFile(t);
							if (p != null && p.Count > 0)
								{
									t.GoodsPic.AddRange(p.ToArray());
								}
						}
						res.Add(t);
					}

				}
				else
				{
					errMsg = errMsg + "此单据无品种明细";
				}
				try
				{
					dt.Dispose();
				}
				catch
				{
				}
				dt = null;
			}
			else
			{
				//WMS数据库连接失败
				errMsg = errMsg + "WMS数据库连接失败";
			}
			return res;
		}
		public List<ModGoodsInfo> GetGoodsPic(string erpid, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			List<ModGoodsInfo> res = new List<ModGoodsInfo>();
			if (VerDb_hderp == true)
			{
				//获取这个单子上有哪些品种,
				string sqlCmd1 = $@"
select
	aa.goodsid,aa.goodsname,nvl(bb.vd,'') as vd,nvl(bb.fg,'') as fg,nvl(bb.fs,'') as fs
from
	bms_sa_dtl_v aa
	left join (
		select
			goodsid,min(validenddate) vd,wmsys.wm_concat(distinct nvl(filegroupid,'')) fg,wmsys.wm_concat(distinct replace(nvl(filesopcode,''),',','')) as fs
		from
			gsp_goods_license_view
		group by
			goodsid
	) bb on aa.goodsid=bb.goodsid
where
	--aa.salesid=7423900
	aa.salesid in({erpid})
";
				string sqlCmd2 = @"
select
	goodsid,goodsname,wmsys.wm_concat(distinct nvl(filegroupid,'')) fg,min(validenddate) vd
from
	gsp_goods_license_view aa
where
	aa.goodsname='{0}'
	and goodsid in({1})
group by
	goodsid,goodsname
having
	min(validenddate)>=trunc(sysdate)
	and wmsys.wm_concat(distinct nvl(filegroupid,'')) is not null
";
				System.Data.DataTable dt = _dbhHdErp.ExecuteToDataTable(sqlCmd1, null, true);
				if (dt.Rows.Count > 0)
				{
					foreach (System.Data.DataRow item in dt.Rows)
					{
						string ftpFolderNames = "";
						ModGoodsInfo t = new ModGoodsInfo()
						{
							FileGroupId = YJT.DataBase.Common.ObjectTryToObj(item["fg"], ""),
							GoodsId = YJT.DataBase.Common.ObjectTryToObj(item["goodsid"], ""),
							GoodsName = YJT.DataBase.Common.ObjectTryToObj(item["goodsname"], ""),
							Xiaoqizhi = YJT.DataBase.Common.ObjectTryToObj<DateTime?>(item["vd"], null),
							FileSopcode = YJT.DataBase.Common.ObjectTryToObj(item["fs"], ""),
						};
						t.GoodsName = YJT.Text.Verification.FunJustGetStr(t.GoodsName, 8);
						//if (t.Xiaoqizhi != null)
						if(1==1)
						{
							//if (t.Xiaoqizhi.Value.Date >= DateTime.Now.Date)
							if(1==1)
							{
								if (YJT.Text.Verification.IsNotNullOrEmpty(t.FileGroupId))
								{
									ftpFolderNames = ftpFolderNames + t.FileGroupId + ",";
								}
								else
								{
									//没有上传文件
									//需要判断是否有纸质存放编码
									if (YJT.Text.Verification.IsNotNullOrEmpty(t.FileSopcode))
									{
										List<string> goodsid2 = YJT.Text.Verification.FunSplitStrByType(t.FileSopcode, 0);
										if (goodsid2 != null && goodsid2.Count > 0)
										{
											string whereGoodsIds = "";
											foreach (string item2 in goodsid2)
											{
												whereGoodsIds = whereGoodsIds + item2 + ",";
											}
											whereGoodsIds = YJT.Text.Format.RemoveChars(whereGoodsIds, YJT.Text.Format.EnumCharDirection.Right, ",");
											string debugtxt1 = string.Format(sqlCmd2, t.GoodsName.Trim(), whereGoodsIds);
											System.Data.DataTable dt2 = new System.Data.DataTable();
											try
											{
												dt2 = _dbhHdErp.ExecuteToDataTable(debugtxt1, null, true);
											}
											catch (Exception ee2)
											{
												string debugFileName1 = @"D:\YdecapServerLogExc\blllcs" + YJT.DateTimeHandle.DateConvert.GetDatetimeString(1, DateTime.Now) + ".txt";
												if (!System.IO.Directory.Exists(@"D:\YdecapServerLogExc"))
												{
													System.IO.Directory.CreateDirectory(@"D:\YdecapServerLogExc");
												}
												System.IO.File.AppendAllText(debugFileName1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
												System.IO.File.AppendAllText(debugFileName1, debugtxt1 + "\r\n");
												System.IO.File.AppendAllText(debugFileName1, ee2.ToString() + "\r\n");
												System.IO.File.AppendAllText(debugFileName1, "---------------------------------------------------------" + "\r\n");
												throw new Exception("已在" + debugFileName1 + "存档本次错误");
											}

											if (dt2.Rows.Count > 0)
											{
												foreach (System.Data.DataRow item3 in dt2.Rows)
												{
													string fgNew = YJT.DataBase.Common.ObjectTryToObj<string>(item3["fg"], "");
													if (YJT.Text.Verification.IsNotNullOrEmpty(fgNew))
													{
														ftpFolderNames = ftpFolderNames + fgNew + ",";
													}
													
												}
											}
											else
											{
												//就算根据纸质编码,也没有找到合适的资质文件
												errMsg = errMsg + t.GoodsName + "在相关的纸质文件中,没有找到合适的资质图片";
											}
											try
											{
												dt2.Dispose();
											}
											catch { }
											dt2 = null;
										}
										else
										{
											errMsg = errMsg + t.GoodsName + "此商品设置纸质存放文件没有找到对应的商品";
										}
										
									}
									else
									{
										//也没有纸质存放编码
										errMsg = errMsg + t.GoodsName + "此商品没有资质图片,且没有设置纸质存放文件";
									}

								}
							}
							else
							{
								//品种资质过期
								errMsg = errMsg + t.GoodsName + "此商品资质过期,均不打印";
							}
						}
						else
						{
							//品种资没有填写效期
							errMsg = errMsg + t.GoodsName + "此商品资没有填写效期,均不打印";
						}
						if (YJT.Text.Verification.IsNotNullOrEmpty(ftpFolderNames))
						{
							foreach (string item2 in ftpFolderNames.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
							{
								List<MOD.Mod_bllFileHandle> p = FileHandle.GetHdGoodsQualificationFile(t.GoodsId, item2, "");
								if (p != null && p.Count > 0)
								{
									t.GoodsPic.AddRange(p.ToArray());
								}
							}
						}
						res.Add(t);
					}
				}
				else
				{
					//没有对应的品种明细
					errMsg = errMsg + "此单据无品种明细";
				}
				try
				{
					dt.Dispose();
				}
				catch
				{
				}
				dt = null;
			}
			else
			{
				errMsg = errMsg + "汇达系统数据库连接失败";
			}


			return res;
		}

		public List<HdErpHeTong> GetHdHeTongOrder(string erpid, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			List<HdErpHeTong> res = new List<HdErpHeTong>();
			if (VerDb_hderp == true)
			{
				string sqlCmd = $@"
select a.salesid,
	a.customname,
	to_char(a.confirmdate,'yyyy-mm-dd') confirmdate,
	b.goodsname,
	b.factoryname,
	b.goodstype,
	b.goodsunit,
	nvl(b.goodsqty,0) goodsqty,
	nvl(b.unitprice,0) unitprice,
	nvl(b.total_line,0) total_line,
	b.dtlmemo
  from bms_sa_doc_v a, bms_sa_dtl_v b
 where a.salesid = b.salesid
 and a.salesid ={erpid}
";
				System.Data.DataTable dt = _dbhHdErp.ExecuteToDataTable(sqlCmd, null, true);
				if (dt.Rows.Count > 0)
				{
					double totalLine = 0d;
					foreach (System.Data.DataRow item in dt.Rows)
					{
						totalLine += (YJT.DataBase.Common.ObjectTryToObj(item["total_line"], 0d));
						MOD.HdErp.HdErpHeTong t = new HdErpHeTong()
						{

							Confirmdate = YJT.DataBase.Common.ObjectTryToObj(item["confirmdate"], ""),
							CustomName = YJT.DataBase.Common.ObjectTryToObj(item["customname"], ""),
							DtlMemo = YJT.DataBase.Common.ObjectTryToObj(item["dtlmemo"], ""),
							FactoryName = YJT.DataBase.Common.ObjectTryToObj(item["factoryname"], ""),
							GoodsName = YJT.DataBase.Common.ObjectTryToObj(item["goodsname"], ""),
							GoodsQty = YJT.DataBase.Common.ObjectTryToObj(item["goodsqty"], 0d),
							GoodsType = YJT.DataBase.Common.ObjectTryToObj(item["goodstype"], ""),
							GoodsUnit = YJT.DataBase.Common.ObjectTryToObj(item["goodsunit"], ""),
							salesid = YJT.DataBase.Common.ObjectTryToObj(item["salesid"], ""),
							Total_line = YJT.DataBase.Common.ObjectTryToObj(item["total_line"], 0d),
							Unitprice = YJT.DataBase.Common.ObjectTryToObj(item["unitprice"], 0d),

						};
						res.Add(t);
					}
					foreach (MOD.HdErp.HdErpHeTong item in res)
					{
						item.TotalLine = totalLine;
					}
					isOk = true;
					errMsg = "";
					errCode = 0;
				}


				try
				{
					dt.Dispose();
					dt = null;
				}
				catch
				{
				}

			}
			else
			{
				errCode = -1;
				errMsg = "汇达ERP数据库连接失败";
			}
			return res;
		}

		public int ServerWriteOrderRelBid(long bid, List<BllMod.Order> subOrders, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			int res = 0;
			if (subOrders != null && subOrders.Count > 0)
			{
				if (VerDb_Local)
				{
					string bids = "";
					foreach (MOD.BllMod.Order item in subOrders)
					{
						bids = bids + item.Bid.ToString() + ",";
					}
					bids = YJT.Text.Format.RemoveChars(bids, YJT.Text.Format.EnumCharDirection.Right);
					string sqlCmd = $@"UPDATE BllMod_Order SET RelBId={bid.ToString()} WHERE Bid IN({bids}) ";
					res = _dbhLocal.ExecuteNonQuery(sqlCmd, null, System.Data.CommandType.Text, false, true, "");
					if (res > 0)
					{
						isOk = true;
					}
					else
					{
						errCode = -3;
						errMsg = "没有更新的数据";
					}
				}
				else
				{
					errCode = -2;
					errMsg = "本地数据库连接失败";
				}
			}
			else
			{
				errCode = -1;
				errMsg = "子单对象为空";
			}
			return res;
		}

		public BllMod.Order ServerAddSubLogic(BllMod.Order orderNew, BllMod.Order orderOld, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			bool isOk2 = false;
			int errCode2 = 0;
			string errMsg2 = "";
			if (orderNew.ErpId == orderOld.ErpId)
			{
				if (orderOld.Status == Setings.EnumOrderStatus.单据完成 && orderOld.PrintStatus == Setings.EnumOrderPrintStatus.已打印 && orderOld.ServerTaskType == Setings.EnumServerTaskType.新单据)
				{
					if (YJT.Text.Verification.IsNotNullOrEmpty(orderOld.WL_NUMBER))
					{
						//获取单据ERP,判断物流信息,判断有多少个子单了;返回最新的ERP编号
						//生成新的物流订单
						//直接打印吧
						int newSubOrderId = ServerGetMaxSubOrderId(orderNew.ErpId, orderNew.Bid, out isOk2, out errCode2, out errMsg2);
						if (isOk2 == true)
						{
							orderNew.Logic = orderOld.Logic;
							//orderNew.Weight = orderOld.Weight;
							//orderNew.Length = orderOld.Length;
							//orderNew.Width = orderOld.Width;
							//orderNew.Height = orderOld.Height;
							orderNew.WmsDanjbh = orderOld.WmsDanjbh;
							orderNew.NetDanjbh = orderOld.NetDanjbh;
							orderNew.WmsYewy = orderOld.WmsYewy;
							orderNew.WmsDdwname = orderOld.WmsDdwname;
							orderNew.NETORDER_FROMID = orderOld.NETORDER_FROMID;
							orderNew.NETORDER_FROM = orderOld.NETORDER_FROM;
							orderNew.ERPORDER_ID = orderOld.ERPORDER_ID;
							orderNew.CUSTOMID = orderOld.CUSTOMID;
							orderNew.CUSTOMNAME = orderOld.CUSTOMNAME;
							orderNew.AGENTNAME = orderOld.AGENTNAME;
							orderNew.ADDRESS = orderOld.ADDRESS;
							orderNew.WL_COMPANYID = orderOld.WL_COMPANYID;
							orderNew.WL_COMPANYNAME = orderOld.WL_COMPANYNAME;
							orderNew.RECVNAME = orderOld.RECVNAME;
							orderNew.RECVPHONE = orderOld.RECVPHONE;
							orderNew.PROVINCENAME = orderOld.PROVINCENAME;
							orderNew.CITYNAME = orderOld.CITYNAME;
							orderNew.DISTRICTNAME = orderOld.DISTRICTNAME;
							orderNew.STREETNAME = orderOld.STREETNAME;
							orderNew.ORIGINALREMARK = orderOld.ORIGINALREMARK;
							orderNew.IsFp = orderOld.IsFp;
							orderNew.IsPj = orderOld.IsPj;
							orderNew.IsHeTong = orderOld.IsHeTong;
							orderNew.IsQysy = orderOld.IsQysy;
							orderNew.IsSyzz = orderOld.IsSyzz;
							orderNew.IsYjbb = orderOld.IsYjbb;
							orderNew.PlatformType = orderOld.PlatformType;
							orderNew.logi_dstRoute = orderOld.logi_dstRoute;
							orderNew.logi_PayType = orderOld.logi_PayType;
							orderNew.logi_monAccNum = orderOld.logi_monAccNum;
							orderNew.logi_baojiaJine = orderOld.logi_baojiaJine;
							orderNew.logi_dsJine = orderOld.logi_dsJine;
							orderNew.logi_logcNum = orderOld.logi_logcNum;
							orderNew.logi_ysJine = orderOld.logi_ysJine;
							orderNew.logi_ysJineTotal = orderOld.logi_ysJineTotal;
							orderNew.logi_shouhuory = orderOld.logi_shouhuory;
							orderNew.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
							orderNew.logi_shoufqianshu = orderOld.logi_shoufqianshu;
							orderNew.logi_shoufRiqi = orderOld.logi_shoufRiqi;
							orderNew.logi_sendSheng = orderOld.logi_sendSheng;
							orderNew.logi_sendShi = orderOld.logi_sendShi;
							orderNew.logi_sendXian = orderOld.logi_sendXian;
							orderNew.logi_sendAddress = orderOld.logi_sendAddress;
							orderNew.logi_sendMan = orderOld.logi_sendMan;
							orderNew.logi_sendPhone = orderOld.logi_sendPhone;
							orderNew.logi_feiyongTotal = orderOld.logi_feiyongTotal;
							orderNew.logi_goodQty = orderOld.logi_goodQty;
							orderNew.logi_goodName = orderOld.logi_goodName;
							orderNew.needBaojia = orderOld.needBaojia;
							orderNew.logi_OrderId = orderOld.logi_OrderId;
							orderNew.logi_CreateDate = orderOld.logi_CreateDate;
							orderNew.logi_ChanpinTypeStr = orderOld.logi_ChanpinTypeStr;
							orderNew.sysFirst = orderOld.sysFirst;
							orderNew.total_amt = orderOld.total_amt;
							orderNew.fplx = orderOld.fplx;
							orderNew.PAIDCOSTS = orderOld.PAIDCOSTS;
							orderNew.logi_ReceivePwd = orderOld.logi_ReceivePwd;
							orderNew.logi_SubOrderSn = newSubOrderId + 1;
							orderNew.logi_PhonNum = orderOld.logi_PhonNum;
							orderNew.logi_TelNum = orderOld.logi_TelNum;
							orderNew.RelOrderId = orderOld.RelOrderId;
							orderNew = ServerCreateLogicSub(orderNew, out isOk2, out errCode2, out errMsg2);

							if (isOk2 == true)
							{
								switch (orderNew.PlatformType)
								{
									case Setings.EnumPlatformType.无:
										orderOld.WL_NUMBER = orderOld.WL_NUMBER + "," + orderNew.WL_NUMBER;
										break;
									case Setings.EnumPlatformType.药师帮:
										orderOld.WL_NUMBER = orderOld.WL_NUMBER + "," + orderNew.WL_NUMBER;
										break;
									case Setings.EnumPlatformType.小药药:
										orderOld.WL_NUMBER = orderOld.WL_NUMBER + "," + orderNew.WL_NUMBER;
										break;
									case Setings.EnumPlatformType.药京采:
										orderOld.WL_NUMBER = orderOld.WL_NUMBER + "," + orderNew.WL_NUMBER;
										break;
									default:
										orderOld.WL_NUMBER = orderOld.WL_NUMBER + "," + orderNew.WL_NUMBER;
										break;
								}
								
								ServerUpdateECInfo(orderOld,orderNew, out isOk2, out errCode2, out errMsg2);
								if (isOk2 == false)
								{
									orderNew.Status = Setings.EnumOrderStatus.异常_电商信息不存在;
									orderNew.ErrMsg = errMsg2;
									orderNew = ServerUpdateOrderAll(orderNew, out isOk2, out errCode2, out errMsg2);
									errCode = -6;
									errMsg = "回写电商记录失败:" + errMsg2;
								}
								else
								{
									orderNew = ServerUpdateOrderAll(orderNew, out isOk2, out errCode2, out errMsg2);
									orderOld = ServerUpdateOrderAll(orderOld, out isOk2, out errCode2, out errMsg2);
									isOk = true;
									errMsg = "";
									errCode = 0;
								}


							}
							else
							{
								errCode = -5;
								errMsg = "创建子单物流错误:" + errMsg2;
								orderNew.ErrMsg = errMsg;
								orderNew.Status = Setings.EnumOrderStatus.异常_物流下单不成功;
								orderNew = ServerUpdateOrderAll(orderNew, out isOk2, out errCode2, out errMsg2);
							}

						}
						else
						{
							errCode = -4;
							errMsg = "获取子单总数时错误:" + errMsg2;
							ServerForceFinesh(orderNew.Bid, errMsg, Setings.EnumOrderStatus.异常_引用ID错误, Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk2, out errCode2, out errMsg2);
						}

					}
					else
					{
						errCode = -3;
						errMsg = "原始单据物流编号不存在 OldBid:" + orderOld.Bid.ToString();
						ServerForceFinesh(orderNew.Bid, errMsg, Setings.EnumOrderStatus.异常_引用ID错误, Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk2, out errCode2, out errMsg2);
					}


				}
				else
				{
					errCode = -2;
					errMsg = "原始单据并非可以添加子单的状态";
					ServerForceFinesh(orderNew.Bid, errMsg, Setings.EnumOrderStatus.异常_引用ID错误, Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk2, out errCode2, out errMsg2);
				}

			}
			else
			{
				errCode = -1;
				errMsg = "创建订单时,ERP编号不一致";
				ServerForceFinesh(orderNew.Bid, errMsg, Setings.EnumOrderStatus.异常_引用ID错误, Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk2, out errCode2, out errMsg2);

			}




			return orderNew;
		}

		private bool ServerUpdateECInfo(BllMod.Order orderOld, BllMod.Order orderNew, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (VerDb_Interface)
			{
				long wl_companyid = 0;
				if (!long.TryParse(orderOld.WL_COMPANYID, out wl_companyid))
				{
					wl_companyid = 0;
				}
				long netorder_fromid = 0;
				if (!long.TryParse(orderOld.NETORDER_FROMID, out netorder_fromid))
				{
					netorder_fromid = 0;
				}
				long erporder_id = 0;
				if (!long.TryParse(orderOld.ERPORDER_ID, out erporder_id))
				{
					erporder_id = 0;
				}
				long customid = 0;
				if (!long.TryParse(orderOld.CUSTOMID, out customid))
				{
					customid = 0;
				}
				string sqlCmd = "";
				switch (orderOld.PlatformType)
				{
					case Setings.EnumPlatformType.无:
						sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_number='{orderOld.WL_NUMBER}'
where
	a.netorder_id='{orderOld.NetDanjbh}'
	and a.netorder_fromid={netorder_fromid.ToString()}
	and a.NETORDER_FROM='{orderOld.NETORDER_FROM}'
	and a.erporder_id={erporder_id.ToString()}
	and a.customid={customid.ToString()}
	and a.agentname='{orderOld.AGENTNAME}'
";
						break;
					case Setings.EnumPlatformType.药师帮:
						sqlCmd = $@"
insert INTO
	NETORDER_DOC
	(
		NETORDER_ID, NETORDER_FROMID, NETORDER_FROM, ERPORDER_ID, CUSTOMID, CUSTOMNAME, AGENTNAME, ADDRESS, WL_COMPANYID, WL_NUMBER, WL_COMPANYNAME,
		RECVNAME, RECVPHONE, PROVINCENAME, CITYNAME, DISTRICTNAME, STREETNAME, ORIGINALREMARK, WL_CREDATE, FPLX, PAIDCOSTS, PROVIDERDOC
	)
	SELECT
		NETORDER_ID, NETORDER_FROMID, NETORDER_FROM, ERPORDER_ID, CUSTOMID, CUSTOMNAME, AGENTNAME, ADDRESS, {wl_companyid.ToString()}, '{orderNew.WL_NUMBER}', '{orderNew.WL_COMPANYNAME}',
		RECVNAME, RECVPHONE, PROVINCENAME, CITYNAME, DISTRICTNAME, STREETNAME, ORIGINALREMARK, to_date('{orderNew.logi_jinjianRiqi}','yyyy-mm-dd hh24:mi:ss'), FPLX, PAIDCOSTS, PROVIDERDOC
	FROM
		NETORDER_DOC a
	WHERE
		a.netorder_id='{orderOld.NetDanjbh}'
		and a.netorder_fromid={netorder_fromid.ToString()}
		and a.NETORDER_FROM='{orderOld.NETORDER_FROM}'
		and a.erporder_id={erporder_id.ToString()}
		and a.customid={customid.ToString()}
		and a.agentname='{orderOld.AGENTNAME}'
		AND ROWNUM=1
";
						break;
					case Setings.EnumPlatformType.小药药:
						sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_number='{orderOld.WL_NUMBER}'
where
	a.netorder_id='{orderOld.NetDanjbh}'
	and a.netorder_fromid={netorder_fromid.ToString()}
	and a.NETORDER_FROM='{orderOld.NETORDER_FROM}'
	and a.erporder_id={erporder_id.ToString()}
	and a.customid={customid.ToString()}
	and a.agentname='{orderOld.AGENTNAME}'
";
						break;
					case Setings.EnumPlatformType.药京采:
						sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_number='{orderOld.WL_NUMBER}'
where
	a.netorder_id='{orderOld.NetDanjbh}'
	and a.netorder_fromid={netorder_fromid.ToString()}
	and a.NETORDER_FROM='{orderOld.NETORDER_FROM}'
	and a.erporder_id={erporder_id.ToString()}
	and a.customid={customid.ToString()}
	and a.agentname='{orderOld.AGENTNAME}'
";
						break;
					default:
						break;
				}
				
				int dbres = _dbhInterface.ExecuteNonQuery(sqlCmd, null, System.Data.CommandType.Text, false, true, "");
				if (dbres > 0)
				{
					isOk = true;
					errCode = 0;
					errMsg = "";
				}
				else
				{
					isOk = false;
					errCode = -2;
					errMsg = "电商数据更新失败";
				}
			}
			else
			{
				isOk = false;
				errCode = -1;
				errMsg = "电商服务器连接失败";
			}
			return isOk;

		}

		/// <summary>
		/// 创建子单物流总
		/// </summary>
		/// <param name="orderNew"></param>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		private BllMod.Order ServerCreateLogicSub(BllMod.Order orderNew, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (orderNew != null)
			{
				if (orderNew.Logic == Setings.EnumLogicType.顺丰)
				{
					orderNew = ServerCreateLogicSubShunfeng(orderNew, out isOk, out errCode, out errMsg);
				}
				else if (orderNew.Logic == Setings.EnumLogicType.邮政EMS)
				{
					orderNew = ServerCreateLogicSubEmsYouzheng(orderNew, out isOk, out errCode, out errMsg);
				}
				else if (orderNew.Logic == Setings.EnumLogicType.京东物流)
				{
					orderNew = ServerCreateLogicSubJingDongWl(orderNew, out isOk, out errCode, out errMsg);
				}
				else if (orderNew.Logic == Setings.EnumLogicType.中通快递)
				{
					orderNew = ServerCreateLogicSubZhongTongWl(orderNew, out isOk, out errCode, out errMsg);
				}
				else if (orderNew.Logic == Setings.EnumLogicType.申通快递)
				{
					orderNew = ServerCreateLogicSubShenTongWl(orderNew, out isOk, out errCode, out errMsg);
				}
				else
				{
					isOk = false;
					errCode = -1;
					errMsg = "此物流不支持增加子单";
				}
			}
			else
			{
				isOk = false;
				errCode = -1;
				errMsg = "对象为null";
			}
			return orderNew;
		}
		private BllMod.Order ServerCreateLogicSubShenTongWl(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			bool isErrorFlag = false;
			string 备注 = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
			{
				备注 = 备注 + "发票";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
			{
				备注 = 备注 + " 货品批件";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
			{
				备注 = 备注 + " 企业首营";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
			{
				备注 = 备注 + " 货品首营";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
			{
				备注 = 备注 + " 药检报告";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
			{
				备注 = 备注 + " 购销合同";
			}





			double tTotal_amt = 0d;
			if (!double.TryParse(order.total_amt, out tTotal_amt))
			{
				tTotal_amt = 0d;
			}
			order.needBaojia = 0;
			order.total_amt = tTotal_amt.ToString("#0.00");
			string stxt = "";
			string otxt = "";
			order.logi_sendMan = "赵志强-" + order.logi_sendMan;
			YJT.Logistics.ShenTongLogistic.ClassCreateOrder cco = _shenTongWl.IntorCreateOrder(
				order.ErpId + "_" + order.logi_SubOrderSn.ToString(), YJT.Logistics.ShenTongLogistic.ClassCreateOrder.EnumBillType.普通00,
				order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.logi_sendSheng, order.logi_sendShi, order.logi_sendXian,
				order.RECVNAME, order.ADDRESS, order.logi_PhonNum, order.PROVINCENAME, order.CITYNAME, order.DISTRICTNAME,
				"药品", recTel: order.logi_TelNum, weight: order.Weight, battery: YJT.Logistics.ShenTongLogistic.ClassCreateOrder.Cargo.EnumBattery.不带电30, ps: 备注
			);
			YJT.Logistics.ShenTongLogistic.ClassPostData cpd = null;
			YJT.Logistics.ShenTongLogistic.ClassCreateOrderRes r1 = null;
			int tryCount = 5;
			while (tryCount > 0)
			{
				tryCount--;
				r1 = _shenTongWl.CreateOrder(cco, out isOk, out errCode, out errMsg, out cpd, out stxt, out otxt);
				try
				{
					if (!System.IO.Directory.Exists(@"D:\WLLOG"))
					{
						System.IO.Directory.CreateDirectory(@"D:\WLLOG");
					}
					System.IO.File.AppendAllText(@"D:\WLLOG\ShenTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ShenTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ShenTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ShenTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

				}
				catch (Exception ee)
				{
					AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
				}
				if (isOk == true)
				{
					break;
				}
				else
				{
					System.Threading.Thread.Sleep(1000);
				}
			}
			
			if (r1 != null && isOk == true)
			{
				isOk = true;
				errCode = 0;
				order.WL_COMPANYID = ((int)order.Logic).ToString();
				order.WL_COMPANYNAME = order.Logic.ToString();
				order.WL_NUMBER = r1.data.waybillNo;
				order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
				order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
				order.ErrMsg = "创建物流订单完成";

				order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				order.logi_baojiaJine = "0.00";
				order.logi_dsJine = "0.00";
				order.logi_feiyongTotal = "0.00";
				order.logi_goodName = "药品";
				order.logi_goodQty = "1";
				order.logi_logcNum = "";
				order.logi_monAccNum = "";
				order.logi_shoufqianshu = "";
				order.logi_shoufRiqi = "";
				order.logi_shouhuory = "";
				order.logi_ChanpinTypeStr = YJT.Logistics.ShenTongLogistic.ClassCreateOrder.EnumBillType.普通00.ToString();
				order.logi_PayType = "";
				order.logi_OrderId = r1.data.orderNo;
				order.logi_dstRoute = r1.data.bigWord;
				order.JingdongWl = r1.data.packagePlace;


				double total = 0;
				double baojia = 0;
				double yunfei = 0;
				if (!double.TryParse(order.logi_feiyongTotal, out total))
				{
					total = 0;
				}
				if (!double.TryParse(order.logi_baojiaJine, out baojia))
				{
					baojia = 0;
				}
				yunfei = total;
				if (yunfei <= 0)
				{
					order.logi_ysJine = yunfei.ToString("#0.000");
					order.logi_ysJineTotal = order.logi_ysJine;
				}
				else
				{
					order.logi_ysJine = "";
					order.logi_ysJineTotal = "";
				}
				isOk = true;
				errCode = 0;
				errMsg = "";
			}
			else
			{

				order.ErrMsg = "申通物流下单不成功:" + errMsg;
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isOk = false;
				errCode = -1;
				errMsg = order.ErrMsg;
			}

			return order;
		}
		private BllMod.Order ServerCreateLogicSubZhongTongWl(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			bool isErrorFlag = false;
			string 备注 = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
			{
				备注 = 备注 + "发票";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
			{
				备注 = 备注 + " 货品批件";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
			{
				备注 = 备注 + " 企业首营";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
			{
				备注 = 备注 + " 货品首营";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
			{
				备注 = 备注 + " 药检报告";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
			{
				备注 = 备注 + " 购销合同";
			}
			YJT.Logistics.ZhongTongLogistics.ClassCreateOrder cco = YJT.Logistics.ZhongTongLogistics.ClassCreateOrder.Intor(order.ErpId + "_" + order.logi_SubOrderSn.ToString(), "2", "1", _ztkdWl, out isOk, out errMsg, 备注);
			if (isOk == false)
			{
				order.ErrMsg = "中通物流下单不成功:" + errMsg;
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isErrorFlag = true;
			}
			if (!cco.CreateAccountInfo(out errMsg, 1))
			{
				order.ErrMsg = "账户信息创建错误:" + errMsg;
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isErrorFlag = true;
			}
			if (!cco.CreateSenderInfo(order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.logi_sendSheng, order.logi_sendShi, order.logi_sendXian, out errMsg))
			{
				order.ErrMsg = "账户发件信息错误:" + errMsg;
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isErrorFlag = true;
			}
			if (!cco.CreateReceiveInfo(order.RECVNAME, order.ADDRESS, order.logi_PhonNum, order.PROVINCENAME, order.CITYNAME, order.DISTRICTNAME, out errMsg, order.logi_TelNum))
			{
				order.ErrMsg = "账户收件信息错误:" + errMsg;
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isErrorFlag = true;
			}
			cco.CreateSummaryInfo(quantity: 1);
			if (!cco.CreateOrderItems(errMsg: out errMsg, name: "药品", weight: (long)order.Weight, quantity: 1))
			{
				order.ErrMsg = "药品信息错误:" + errMsg;
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isErrorFlag = true;
			}
			double tTotal_amt = 0d;
			if (!double.TryParse(order.total_amt, out tTotal_amt))
			{
				tTotal_amt = 0d;
			}
			order.needBaojia = 0;
			order.total_amt = tTotal_amt.ToString("#0.00");
			string stxt = "";
			string otxt = "";
			if (isErrorFlag == false)
			{
				YJT.Logistics.ZhongTongLogistics.ClassCreateOrderRes r1 = _ztkdWl.CreateOrder(cco, out isOk, out errMsg, out errCode, out stxt, out otxt);
				try
				{
					if (!System.IO.Directory.Exists(@"D:\WLLOG"))
					{
						System.IO.Directory.CreateDirectory(@"D:\WLLOG");
					}
					System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

				}
				catch (Exception ee)
				{
					AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
				}
				if (r1 != null && isOk == true)
				{
					isOk = true;
					errCode = 0;
					order.WL_COMPANYID = ((int)order.Logic).ToString();
					order.WL_COMPANYNAME = order.Logic.ToString();
					order.WL_NUMBER = r1.result.billCode;
					order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
					order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
					order.ErrMsg = "创建物流订单完成";

					order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					order.logi_baojiaJine = "0.00";
					order.logi_dsJine = "0.00";
					order.logi_feiyongTotal = "0.00";
					order.logi_goodName = "药品";
					order.logi_goodQty = "1";
					order.logi_logcNum = "";
					order.logi_monAccNum = "";
					order.logi_shoufqianshu = "";
					order.logi_shoufRiqi = "";
					order.logi_shouhuory = "";
					order.logi_ChanpinTypeStr = "非集团客户-全网件";
					order.logi_PayType = "";
					order.logi_OrderId = r1.result.orderCode;
					order.logi_dstRoute = r1.result.bigMarkInfo.mark;
					order.JingdongWl = r1.result.siteName + "转" + r1.result.bigMarkInfo.bagAddr;


					double total = 0;
					double baojia = 0;
					double yunfei = 0;
					if (!double.TryParse(order.logi_feiyongTotal, out total))
					{
						total = 0;
					}
					if (!double.TryParse(order.logi_baojiaJine, out baojia))
					{
						baojia = 0;
					}
					yunfei = total;
					if (yunfei <= 0)
					{
						order.logi_ysJine = yunfei.ToString("#0.000");
						order.logi_ysJineTotal = order.logi_ysJine;
					}
					else
					{
						order.logi_ysJine = "";
						order.logi_ysJineTotal = "";
					}
					isOk = true;
					errCode = 0;
					errMsg = "";
				}
				else
				{

					order.ErrMsg = "中通物流下单不成功:" + errMsg;
					order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
					isOk = false;
					errCode = -1;
					errMsg = order.ErrMsg;
				}
			}
			else
			{
				order.ErrMsg = "中通物流下单不成功:" + errMsg;
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isOk = false;
				errCode = -1;
				errMsg = order.ErrMsg;
			}
			return order;
		}
		private BllMod.Order ServerCreateLogicSubJingDongWl(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			string 备注2 = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
			{
				备注2 = 备注2 + "发票";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
			{
				备注2 = 备注2 + " 货品批件";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
			{
				备注2 = 备注2 + " 企业首营";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
			{
				备注2 = 备注2 + " 货品首营";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
			{
				备注2 = 备注2 + " 药检报告";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
			{
				备注2 = 备注2 + " 购销合同";
			}
			YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrder createClass = new YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrder("0030001", order.ErpId + "_" + order.logi_SubOrderSn.ToString(), order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.RECVNAME, order.ADDRESS, order.logi_TelNum, 1, order.Weight, 1)
			{
				province = order.PROVINCENAME,
				city = order.CITYNAME,
				county = order.DISTRICTNAME,
				town = order.STREETNAME,
				description = "药品",
				senderMobile = order.logi_sendPhone,
				senderTel = order.logi_sendPhone,

				receiveMobile = order.logi_PhonNum,
				receiveTel = order.logi_TelNum,

				remark = 备注2
			};
			string stxt2 = "";
			string otxt2 = "";
			YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrderRes res2 = _jdWl.CreateOrder(createClass, out isOk, out errCode, out errMsg, out stxt2, out otxt2);
			try
			{
				if (!System.IO.Directory.Exists(@"D:\WLLOG"))
				{
					System.IO.Directory.CreateDirectory(@"D:\WLLOG");
				}
				System.IO.File.AppendAllText(@"D:\WLLOG\Jingdong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
				System.IO.File.AppendAllText(@"D:\WLLOG\Jingdong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt2 + "\r\n");
				System.IO.File.AppendAllText(@"D:\WLLOG\Jingdong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
				System.IO.File.AppendAllText(@"D:\WLLOG\Jingdong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt2 + "\r\n");

			}
			catch (Exception ee)
			{
				AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
			}
			if (isOk == true && YJT.Text.Verification.IsNotNullOrEmpty(res2.deliveryId))
			{
				isOk = true;
				errCode = 0;
				errMsg = order.ErrMsg;
				order.WL_COMPANYID = ((int)order.Logic).ToString();
				order.WL_COMPANYNAME = order.Logic.ToString();
				order.WL_NUMBER = res2.deliveryId;
				order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
				order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
				order.ErrMsg = "创建物流订单完成";

				order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				order.logi_baojiaJine = "0.00";
				order.logi_dsJine = "0.00";
				order.logi_feiyongTotal = 0.ToString("#0.00");
				order.logi_goodName = createClass.description;
				order.logi_goodQty = createClass.packageCount.ToString();
				order.logi_logcNum = "";
				order.logi_monAccNum = "";
				order.logi_sendAddress = order.logi_sendAddress;
				order.logi_sendMan = order.logi_sendMan;
				order.logi_sendSheng = order.logi_sendSheng;
				order.logi_sendShi = order.logi_sendShi;
				order.logi_sendXian = order.logi_sendXian;
				order.logi_shoufqianshu = "";
				order.logi_shoufRiqi = "";
				order.logi_shouhuory = "";
				order.logi_ChanpinTypeStr = "纯配";
				order.logi_PayType = "月结";//坑
				order.logi_OrderId = res2.deliveryId;

				order.logi_dstRoute = "";
				if (res2.preSortResult != null)
				{

					order.logi_dstRoute = res2.preSortResult.sourceSortCenterId.Value.ToString() + "-" + res2.preSortResult.sourceSortCenterName + "-|-" + res2.preSortResult.targetSortCenterId.Value.ToString() + "-" + res2.preSortResult.targetSortCenterName;
					order.JingdongWl = res2.preSortResult.aging.Value.ToString() + "@<|||>@\n" +
						res2.preSortResult.agingName.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.collectionAddress.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.coverCode.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.distributeCode.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.isHideContractNumbers.Value.ToString().Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.isHideName.ToString().Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.qrcodeUrl.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.road.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.siteId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.siteName.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.siteType.Value.ToString().Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.slideNo.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.sourceCrossCode.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.sourceSortCenterId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.sourceSortCenterName.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.sourceTabletrolleyCode.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.targetSortCenterId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.targetSortCenterName.Replace("'", "") + "@<|||>@\n" +
						res2.preSortResult.targetTabletrolleyCode.Replace("'", "") + "@<|||>@\n";
				}
				double total = 0;
				double baojia = 0;
				double yunfei = 0;
				if (!double.TryParse(order.logi_feiyongTotal, out total))
				{
					total = 0;
				}
				if (!double.TryParse(order.logi_baojiaJine, out baojia))
				{
					baojia = 0;
				}
				yunfei = total;
				if (yunfei <= 0)
				{
					order.logi_ysJine = yunfei.ToString("#0.000");
					order.logi_ysJineTotal = order.logi_ysJine;
				}
				else
				{
					order.logi_ysJine = "";
					order.logi_ysJineTotal = "";
				}

			}
			else
			{
				order.ErrMsg = "京东物流下单不成功:" + errMsg;
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isOk = false;
				errCode = -1;
				errMsg = order.ErrMsg;
			}








			return order;
		}

		private BllMod.Order ServerCreateLogicSubEmsYouzheng(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			string 备注 = "";
			isOk = false;
			errCode = -1;
			errMsg = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
			{
				备注 = 备注 + "发票";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
			{
				备注 = 备注 + " 货品批件";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
			{
				备注 = 备注 + " 企业首营";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
			{
				备注 = 备注 + " 货品首营";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
			{
				备注 = 备注 + " 药检报告";
			}
			if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
			{
				备注 = 备注 + " 购销合同";
			}
			//if (order.total_amt != "0")
			//{
			//	double tTotalamt = 0;
			//	if (!double.TryParse(order.total_amt, out tTotalamt))
			//	{
			//		tTotalamt = 0;
			//	}
			//	if (tTotalamt > 0)
			//	{
			//		int b = (int)(tTotalamt / 500);
			//		double c = tTotalamt % 500;
			//		if (c > 0)
			//		{
			//			b = b + 1;
			//		}
			//		order.needBaojia = b * 500;
			//		order.total_amt = tTotalamt.ToString();
			//	}
			//}
			order.needBaojia = 0d;
			YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.Cargo 药品info = _emsYz.GetSubModCargo(
				cargo_category: "",
				cargo_name: "药品",
				cargo_quantity: 0,
				cargo_value: 1,
				cargo_length: 0, cargo_width: 0, cargo_high: 0,
				cargo_weight: 0,
				cargo_order_no: "");
			YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.ClassAddress 发件方 = _emsYz.GetSubModAddress
				(
				 address: order.logi_sendAddress,
				 prov: order.logi_sendSheng,
				 city: order.logi_sendShi,
				 county: order.logi_sendXian,
				 name: order.logi_sendMan,
				 mobile: order.logi_sendPhone,
				 phone: order.logi_sendPhone,
				 post_code: "");
			YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.ClassAddress 收件方 = _emsYz.GetSubModAddress(
				address: order.ADDRESS,
				 prov: order.PROVINCENAME,
				 city: order.CITYNAME,
				 county: order.DISTRICTNAME,
				 name: order.RECVNAME,
				 mobile: order.logi_PhonNum,
				 phone: order.logi_TelNum,
				 post_code: ""
				);
			order.logi_ReceivePwd = YJT.Text.ClassCreateText.FunStrCreateNumberStr(6, null);
			YJT.Logistics.YouZhengEms.ClassYouZhengEmsMod 取号 = _emsYz.GetModGetEmsOrder(
				ecommerce_user_id: order.PlatformType.ToString(),
				erpid: order.ERPORDER_ID + "_" + order.logi_SubOrderSn.ToString(),
				length: order.Length, width: order.Width, height: order.Height, weight: order.Weight,
				sender: 发件方,
				receiver: 收件方,
				cargos: new YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.Cargo[] { 药品info },
				batch_no: "",
				one_bill_fee_type: 0,
				contents_attribute: 3,
				base_product_no: "1",
				biz_product_no: "1",
				cod_amount: 0,
				cod_flag: 9,
				deliver_type: 2,
				electronic_preferential_amount: 0,
				electronic_preferential_no: "",
				insurance_amount: 0,//order.needBaojia,
				insurance_flag: 2,
				insurance_premium_amount: 0,
				note: "",
				payment_mode: 1,
				pickup_notes: 备注,
				pickup_type: 1,
				postage_total: 0,
				receipt_flag: 1,
				receiver_safety_code: order.logi_ReceivePwd,
				sender_safety_code: "",
				valuable_flag: 0,
				waybill_no: "",
				submail_no: ""
			);
			string stxt = "";
			string otxt = "";
			YJT.Logistics.YouZhengEms.ClassResBase resobj = null;
			int tryCountEms = 5;
			while (tryCountEms > 0)
			{
				tryCountEms--;
				resobj = _emsYz.SendGetEmsOrder(取号, out isOk, out errCode, out errMsg, out stxt, out otxt);
				try
				{
					if (!System.IO.Directory.Exists(@"D:\WLLOG"))
					{
						System.IO.Directory.CreateDirectory(@"D:\WLLOG");
					}
					System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZ_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZ_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZ_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
					System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZ_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");
				}
				catch (Exception ee)
				{
					AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
				}
				if (isOk == true)
				{
					break;
				}
				else
				{
					System.Threading.Thread.Sleep(1000);
				}
			}
			
			
			if (resobj != null)
			{
				if (isOk == true)
				{
					YJT.Logistics.YouZhengEms.Classoms_ordercreate_waybillnoRes 下单结果 = resobj as YJT.Logistics.YouZhengEms.Classoms_ordercreate_waybillnoRes;
					if (下单结果 != null)
					{
						order.WL_COMPANYID = ((int)order.Logic).ToString();
						order.WL_COMPANYNAME = order.Logic.ToString();
						order.WL_NUMBER = 下单结果.body.waybill_no;
						order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
						order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
						order.ErrMsg = "子单物流订单完成";
						order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
						order.logi_dstRoute = 下单结果.body.routeCode;
						order.logi_PayType = "寄件人";//payment_mode
						order.logi_monAccNum = "";
						order.logi_baojiaJine = order.needBaojia.ToString("#0.00");
						order.logi_dsJine = "";
						order.logi_logcNum = "";
						order.logi_ysJine = "0.00";
						order.logi_ysJineTotal = "0.00";
						order.logi_shouhuory = "";
						order.logi_shoufqianshu = "";
						order.logi_shoufRiqi = "";
						order.logi_sendSheng = 发件方.prov;
						order.logi_sendShi = 发件方.city;
						order.logi_sendXian = 发件方.county;
						order.logi_sendAddress = 发件方.address;
						order.logi_sendMan = 发件方.name;
						order.logi_sendPhone = 发件方.mobile;
						order.logi_feiyongTotal = "0";
						order.logi_goodQty = "0";
						order.logi_goodName = "药品";
						order.logi_ChanpinTypeStr = "特快专递";//biz_product_no
					}
					else
					{
						//返回的内容不是下单取号的结果类
						order.ErrMsg = "邮政物流下单不成功:返回的对象不是Classoms_ordercreate_waybillnoRes";
						order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
						isOk = false;
					}
				}
				else
				{
					//下单不成功
					order.ErrMsg = "邮政物流下单不成功:" + errMsg;
					order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
					isOk = false;
				}
			}
			else
			{
				order.ErrMsg = "邮政物流下单不成功:提交过来的对象为NULL";
				order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
				isOk = false;
			}
			return order;
		}

		private BllMod.Order ServerCreateLogicSubShunfeng(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (order != null)
			{
				if (order.Logic == Setings.EnumLogicType.顺丰)
				{
					YJT.Logistics.ShunFengLogistic.ClassCreateOrder.CargoDetailsClass 货品 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.CargoDetailsClass()
					{
						count = 1,
						name = "药品",
						specifications = "药品",
						weight = order.Weight
					};
					YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass 发件方 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass()
					{
						address = order.logi_sendAddress,
						city = order.logi_sendShi,
						company = "河北燕都医药物流有限公司",
						contact = order.logi_sendMan,
						contactType = 1,
						county = order.logi_sendXian,
						mobile = order.logi_sendPhone,
						province = order.logi_sendSheng,
						tel = order.logi_sendPhone

					};


					YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass 收件方 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass()
					{
						address = order.ADDRESS,
						city = order.CITYNAME,
						company = order.CUSTOMNAME,
						contact = order.RECVNAME,
						contactType = 2,
						county = order.DISTRICTNAME,
						mobile = order.logi_PhonNum,
						province = order.PROVINCENAME,
						tel = order.logi_TelNum

					};
					string 备注 = "";
					if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
					{
						备注 = 备注 + "发票";
					}
					if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
					{
						备注 = 备注 + " 货品批件";
					}
					if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
					{
						备注 = 备注 + " 企业首营";
					}
					if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
					{
						备注 = 备注 + " 货品首营";
					}
					if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
					{
						备注 = 备注 + " 药检报告";
					}
					if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
					{
						备注 = 备注 + " 购销合同";
					}
					YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass 增值服务 = YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.CtorService(YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.enum增值服务类别.配送服务, null);
					//增值服务 = null;
					//order.ERPORDER_ID+"_"+order.logi_SubOrderSn.ToString()
					YJT.Logistics.ShunFengLogistic.ClassCreateOrder a = _sf.Ctor_CreateOrderObj(order.ERPORDER_ID + "_" + order.logi_SubOrderSn.ToString(), 发件方, 收件方, 增值服务, 货品, 备注, "13", YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型.顺丰标快, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式.寄付月结);
					double tTotal_amt = 0d;
					if (!double.TryParse(order.total_amt, out tTotal_amt))
					{
						tTotal_amt = 0d;
					}
					if (tTotal_amt != 0d)
					{
						if (tTotal_amt > 0)
						{
							//int b = (int)(tTotal_amt / 500);
							//double c = tTotal_amt % 500;
							//if (c > 0)
							//{
							//	b = b + 1;
							//}
							//order.needBaojia = b * 500;
							//order.total_amt = tTotal_amt.ToString("#0.00");
							if (tTotal_amt < 500)
							{
								order.needBaojia = 500;
							}
							else
							{
								order.needBaojia = 1000;
							}
							order.total_amt = tTotal_amt.ToString("#0.00");
						}
					}
					if (order.needBaojia > 0)
					{
						YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass 保价服务 = YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.CtorService(YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.enum增值服务类别.保价, order.needBaojia.ToString());
						a.AddserviceList(保价服务);
					}
					else
					{
						order.needBaojia = 0;
					}
					string stxt = "";
					string otxt = "";
					YJT.Logistics.ShunFengLogistic.ClassCreateOrderRes res = _sf.CreateOrder(a, out isOk, out errCode, out errMsg, out stxt, out otxt);
					try
					{

						if (!System.IO.Directory.Exists(@"D:\WLLOG"))
						{
							System.IO.Directory.CreateDirectory(@"D:\WLLOG");
						}
						System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengSub_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
						System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengSub_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
						System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengSub_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
						System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengSub_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

					}
					catch (Exception ee)
					{
						AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志子单错误");
					}

					if (isOk == true && YJT.Text.Verification.IsNotNullOrEmpty(res.data))
					{

						order.WL_COMPANYID = ((int)order.Logic).ToString();
						order.WL_COMPANYNAME = order.Logic.ToString();
						order.WL_NUMBER = res.data;
						order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
						order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
						order.ErrMsg = "子单创建物流订单完成";
						order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
						////////////////////////////////////////////////////////////////////
						YJT.Logistics.ShunFengLogistic.ClassGetOrderInfoRes res2 = _sf.GetOrderInfo(order.ERPORDER_ID + "_" + order.logi_SubOrderSn.ToString(), out isOk, out errCode, out errMsg);
						if (isOk == true && res2.data2 != null)
						{
							order.ErrMsg = "物流子单信息获取完成";
							order.logi_baojiaJine = res2.data2.insureFee;
							order.logi_dsJine = res2.data2.codValue;
							order.logi_dstRoute = res2.data2.destRouteLabel;
							order.logi_feiyongTotal = res2.data2.totalFee;
							order.logi_goodName = res2.data2.cargo;
							order.logi_goodQty = res2.data2.cargoCount;
							order.logi_logcNum = res2.data2.codMonthAccount;
							order.logi_monAccNum = res2.data2.monthAccount;
							order.logi_sendAddress = res2.data2.deliverAddress;
							order.logi_sendMan = res2.data2.deliverName;
							order.logi_sendPhone = res2.data2.deliverTel;
							order.logi_sendSheng = res2.data2.deliverProvince;
							order.logi_sendShi = res2.data2.deliverCity;
							order.logi_sendXian = res2.data2.deliverCounty;
							order.logi_shoufqianshu = res2.data2.returnTrackingNo;
							order.logi_shoufRiqi = "";
							order.logi_shouhuory = "";
							order.logi_ChanpinTypeStr = res2.data2.expressType;
							YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型 t = YJT.DataBase.Common.ObjectTryToObj(order.logi_ChanpinTypeStr, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型.错误);
							order.logi_ChanpinTypeStr = t.ToString();
							order.logi_PayType = res2.data2.payMethod;//坑
							YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式 t2 = YJT.DataBase.Common.ObjectTryToObj(order.logi_PayType, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式.错误);
							order.logi_PayType = t2.ToString();
							order.logi_OrderId = res2.data2.orderNo;
							double total = 0;
							double baojia = 0;
							double yunfei = 0;
							if (!double.TryParse(order.logi_feiyongTotal, out total))
							{
								total = 0;
							}
							if (!double.TryParse(order.logi_baojiaJine, out baojia))
							{
								baojia = 0;
							}
							yunfei = total;
							if (yunfei <= 0)
							{
								order.logi_ysJine = yunfei.ToString("#0.000");
								order.logi_ysJineTotal = order.logi_ysJine;
							}
							else
							{
								order.logi_ysJine = "";
								order.logi_ysJineTotal = "";
							}
						}
						else
						{
							order.ErrMsg = "子单物流获取的运单号失败";
							order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
						}




						//YJT.Logistics.ShunFengLogistic.ClassGetOrderInfoRes res2 = sf.GetOrderInfo(order.ERPORDER_ID, out isOk, out errCode, out errMsg);
						//if (isOk == true && res2.data2 != null)
						//{
						//	//res2.data2.destRouteLabel //371DN-072
						//	//monthAccount
						//}
					}
					else
					{
						order.ErrMsg = "顺丰物流子单下单不成功:" + res.data;
						order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
					}
					//YJT.Logistics.ShunFengLogistic.ClassCreateOrder.CargoDetailsClass 货品 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.CargoDetailsClass()
					//{
					//	count = 1,
					//	name = "药品",
					//	specifications = "药品",
					//	weight = order.Weight
					//};
					//List<YJT.Logistics.ShunFengLogistic.ClassAddSubOrder> sub = _sf.Ctor_AddSubOrderObj(order.ErpId, 货品);
					//string stxt = "";
					//string otxt = "";
					//YJT.Logistics.ShunFengLogistic.ClassAddSubOrderRes webres = _sf.AddSubOrder(sub, out isOk, out errCode, out errMsg, out stxt, out otxt);
					//try
					//{
					//	if (!System.IO.Directory.Exists(@"D:\WLLOG"))
					//	{
					//		System.IO.Directory.CreateDirectory(@"D:\WLLOG");
					//	}
					//	System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengSub_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
					//	System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengSub_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
					//	System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengSub_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
					//	System.IO.File.AppendAllText(@"D:\WLLOG\ShunfengSub_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

					//}
					//catch (Exception ee)
					//{
					//	AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
					//}
					//if (isOk == true)
					//{
					//	errMsg = "顺丰子单编号已经获取";
					//	order.WL_NUMBER = webres.data[0].mailNo;
					//	order.ErrMsg = errMsg;
					//	isOk = true;
					//	errCode = 0;

					//}
					//else
					//{
					//	order.ErrMsg = errMsg;
					//	order.Status = Setings.EnumOrderStatus.异常_物流下单不成功;
					//}
				}
				else
				{
					errMsg = "此单据并非顺丰物流";
					isOk = false;
					errCode = -1;
					order.Status = Setings.EnumOrderStatus.异常_物流下单不成功;
					order.ErrMsg = errMsg;
				}
			}
			else
			{
				errMsg = "此单据并非顺丰物流";
				isOk = false;
				errCode = -1;
			}
			return order;
		}

		private int ServerGetMaxSubOrderId(string erpId, long bid, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			int res = -1;
			if (VerDb_Local)
			{
				if (YJT.Text.Verification.IsNotNullOrEmpty(erpId))
				{
					string sqlCmd = $@"SELECT COUNT(1) AS cu FROM BllMod_Order WHERE ErpId='{erpId}' AND Bid<{bid.ToString()} AND ServerTaskType='添加物流子单'";
					object dbres = _dbhLocal.ExecuteScalar(sqlCmd, null, true);
					res = YJT.DataBase.Common.ObjectTryToObj(dbres, -1);
					if (res >= 0)
					{
						isOk = true;
						errCode = 0;
						errMsg = "";
					}
					else
					{
						errCode = -3;
						errMsg = "数据库查询阶段发生错误";
						AddMsgOut(errMsg, Setings.EnumMessageType.严重, -3, sqlCmd);
					}
				}
				else
				{
					errCode = -2;
					errMsg = "ERPID为空";
				}
			}
			else
			{
				errCode = -1;
				errMsg = "本地数据库连接失败";
			}
			return res;
		}

		public BllMod.Order ServerUpdateOrderAll(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (order != null)
			{
				if (VerDb_Local)
				{
					string sqlCmd = $@"
UPDATE
		BllMod_Order
	SET	
		OrderId = {order.OrderId.ToString()},
		AddDate = convert(datetime,'{order.AddDate.ToString("yyyy-MM-dd HH:mm:ss")}') ,
		ErpId = '{order.ErpId}' ,
		Logic = '{order.Logic.ToString()}' ,
		Weight = {order.Weight.ToString()} ,
		Length = {order.Length.ToString()} ,
		Width = {order.Width.ToString()} ,
		Height = {order.Height.ToString()} ,
		Status = '{order.Status.ToString()}' ,
		Ip = '{order.Ip}' ,
		Mac = '{order.Mac}' ,
		ComputerName = '{order.ComputerName}' ,
		ErrMsg = '{order.ErrMsg}' ,
		ClientId = {order.ClientId.ToString()} ,
		PrintStatus = '{order.PrintStatus.ToString()}' ,
		WmsDanjbh = '{order.WmsDanjbh}' ,
		netDanjbh = '{order.NetDanjbh}' ,
		ServerHandleDate=GETDATE(),
		ClientHandleDate=GETDATE()-1,
		WmsYewy = '{order.WmsYewy}' ,
		WmsDdwname = '{order.WmsDdwname}' ,
		NETORDER_FROMID = '{order.NETORDER_FROMID}' ,
		NETORDER_FROM = '{order.NETORDER_FROM}' ,
		ERPORDER_ID = '{order.ERPORDER_ID}' ,
		CUSTOMID = '{order.CUSTOMID}' ,
		CUSTOMNAME = '{order.CUSTOMNAME}' ,
		AGENTNAME = '{order.AGENTNAME}' ,
		ADDRESS = '{order.ADDRESS}' ,
		WL_COMPANYID = '{order.WL_COMPANYID}' ,
		WL_NUMBER = '{order.WL_NUMBER}' ,
		WL_COMPANYNAME = '{order.WL_COMPANYNAME}' ,
		RECVNAME = '{order.RECVNAME}' ,
		RECVPHONE = '{order.RECVPHONE}' ,
		PROVINCENAME = '{order.PROVINCENAME}' ,
		CITYNAME = '{order.CITYNAME}' ,
		DISTRICTNAME = '{order.DISTRICTNAME}' ,
		STREETNAME = '{order.STREETNAME}' ,
		ORIGINALREMARK = '{order.ORIGINALREMARK}' ,
		IsFp = '{order.IsFp}' ,
		IsPj = '{order.IsPj}' ,
		IsHeTong='{order.IsHeTong}',
		IsQysy = '{order.IsQysy}' ,
		IsSyzz = '{order.IsSyzz}' ,
		IsYjbb = '{order.IsYjbb}' ,
		PlatformType = '{order.PlatformType}' ,
		logi_dstRoute = '{order.logi_dstRoute}' ,
		logi_PayType = '{order.logi_PayType}' ,
		logi_monAccNum = '{order.logi_monAccNum}' ,
		logi_baojiaJine = '{order.logi_baojiaJine}' ,
		logi_dsJine = '{order.logi_dsJine}' ,
		logi_logcNum = '{order.logi_logcNum}' ,
		logi_ysJine = '{order.logi_ysJine}' ,
		logi_ysJineTotal = '{order.logi_ysJineTotal}' ,
		logi_shouhuory = '{order.logi_shouhuory}' ,
		logi_jinjianRiqi = '{order.logi_jinjianRiqi}' ,
		logi_shoufqianshu = '{order.logi_shoufqianshu}' ,
		logi_shoufRiqi = '{order.logi_shoufRiqi}' ,
		logi_sendSheng = '{order.logi_sendSheng}' ,
		logi_sendShi = '{order.logi_sendShi}' ,
		logi_sendXian = '{order.logi_sendXian}' ,
		logi_sendAddress = '{order.logi_sendAddress}' ,
		logi_sendMan = '{order.logi_sendMan}' ,
		logi_sendPhone = '{order.logi_sendPhone}' ,
		logi_feiyongTotal = '{order.logi_feiyongTotal}' ,
		logi_goodQty = '{order.logi_goodQty}' ,
		logi_goodName = '{order.logi_goodName}' ,
		needBaojia = {order.needBaojia.ToString("#0.000")} ,
		logi_OrderId = '{order.logi_OrderId}' ,
		logi_CreateDate = '{order.logi_CreateDate}' ,
		logi_ChanpinTypeStr = '{order.logi_ChanpinTypeStr}' ,
		PrintDatetime = '{order.PrintDatetime}' ,
		sysFirst = '{order.sysFirst}' ,
		total_amt = '{order.total_amt}' ,
		RelBId = {order.RelBId.ToString()} ,
		fplx = '{order.fplx}' ,
		ServerTaskType = '{order.ServerTaskType.ToString()}' ,
		PAIDCOSTS = '{order.PAIDCOSTS.ToString()}' ,
		logi_ReceivePwd = '{order.logi_ReceivePwd}' ,
		logi_SubOrderSn = {order.logi_SubOrderSn.ToString()},
		logi_PhonNum='{order.logi_PhonNum}',
		logi_TelNum='{order.logi_TelNum}',
		RelOrderId={order.RelOrderId.ToString()},
		JingdongWl='{order.JingdongWl}'
	OUTPUT
		Inserted.ServerHandleDate,
		Inserted.ClientHandleDate
	WHERE
		Bid ={order.Bid.ToString()};
";
					System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
					if (dt.Rows.Count > 0)
					{
						order.ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ClientHandleDate"], DateTime.MinValue);
						order.ServerHandleDate = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ServerHandleDate"], DateTime.MinValue);
						errMsg = "";
						errCode = 0;
						isOk = true;
					}
					else
					{
						errMsg = "未找到可更新的记录";
						errCode = -2;
					}
					try
					{
						dt.Dispose();
						dt = null;
					}
					catch { }
				}
				else
				{
					errMsg = "数据库连接失败";
					errCode = -2;
				}
			}
			else
			{
				errMsg = "单据为NULL";
				errCode = -1;
			}


			return order;
		}

		/// <summary>
		/// 业务层初始化构造器
		/// </summary>
		/// <returns></returns>
		public static Blll init()
		{
			string errMsg = "";
			bool isOk = false;
			int errCode = 0;
			if (_hand == null)
			{
				_hand = new Blll();
			}
			if (_hand.VerDb(out isOk, out errCode, out errMsg))
			{
				if (_clientInfoObj != null)
				{
					if (_hand.VerDbClient(_clientInfoObj, out isOk, out errCode, out errMsg))
					{
						return _hand;
					}
					else
					{
						AddMsgOut("初始化比对客户信息失败", Settings.Setings.EnumMessageType.严重, errCode, errMsg);
						return null;
					}
				}
				else
				{
					return _hand;
				}

			}
			else
			{
				AddMsgOut("初始化Init时候数据库验证出错", Settings.Setings.EnumMessageType.严重, errCode, errMsg);
				return null;
			}

		}
		/// <summary>
		/// 验证本客户端与SQL获取客户端的信息是否一致
		/// </summary>
		private bool VerDbClient(MOD.SysMod.ClinetTag tag, out bool isOk, out int errCode, out string errMsg)
		{

			isOk = false;
			errCode = -1;
			errMsg = "";
			string callNameSpace = "";
			try
			{
				System.Reflection.MethodBase callObj = new System.Diagnostics.StackTrace(true).GetFrame(1).GetMethod();
				callNameSpace = " 方法调用者:" + "[" + callObj.DeclaringType.Namespace + "].[" + callObj.DeclaringType.FullName + "].[" + callObj.Name + "]";
			}
			catch
			{
				callNameSpace = "";
			}
			string sqlCmd = @"SELECT  CONNECTIONPROPERTY('PROTOCOL_TYPE') AS proType,CONNECTIONPROPERTY('CLIENT_NET_ADDRESS') AS ip,connectionproperty('net_transport') transPort";
			System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
			if (dt.Rows.Count > 0)
			{
				//MOD.SysMod.ClinetTag ct = Common.PubMethod.GetClientTag();
				//YJT.StaticResources.Add("userObj", ct, true);
				string qz = "";
				object handObj = YJT.StaticResources.GetObject("handObj");
				if (handObj != null)
				{
					qz = handObj.ToString();
				}
				
				string tip = YJT.DataBase.Common.ObjectTryToObj<string>(dt.Rows[0]["ip"], "无");
				System.IO.File.AppendAllText($@"d:\YDECAP\debug{qz}.txt", tip + "\r\n");
				System.IO.File.AppendAllText($@"d:\YDECAP\debug{qz}.txt", tag.ComputerName + "\r\n");
				System.IO.File.AppendAllText($@"d:\YDECAP\debug{qz}.txt", tag.Mac + "\r\n");
				//if (tip == "172.16.7.50")
				//{
				//	tip = "172.16.2.150";
				//}

				if (YJT.Text.Verification.IsContain(tag.Ip, tip))
				{
					tag.Ip = tip;
					sqlCmd = $@"SELECT id ,clientIP ,clientName ,clientMac ,enableInput,isnull(isServer,0) as isServer FROM info_Client where clientIP='{tip}' and clientName='{tag.ComputerName}' and clientMac='{tag.Mac}'";
					System.Data.DataTable dt2 = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
					if (dt2.Rows.Count == 1)
					{
						tag.EnableInput = YJT.DataBase.Common.ObjectTryToObj<bool>(dt2.Rows[0]["enableInput"], false);
						tag.ClientId = YJT.DataBase.Common.ObjectTryToObj<int>(dt2.Rows[0]["id"], -1);
						tag.isServer = YJT.DataBase.Common.ObjectTryToObj<bool>(dt2.Rows[0]["isServer"], false);
						isOk = true;
						errCode = 0;
						errMsg = "";
					}
					else
					{
						isOk = false;
						errCode = -3;
						errMsg = "info_Client中未获取到已注册的信息 IP:" + tag.Ip + " " + callNameSpace;
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "dt2.Rows.Count!=1");
					}
					try
					{
						dt2.Dispose();
						dt2 = null;
					}
					catch
					{
					}

				}
				else
				{
					isOk = false;
					errCode = -2;
					errMsg = "获取的IP与注册的IP不一致 获取的IP:" + tag.Ip + " 注册的IP:" + tip + callNameSpace;
					AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
				}
			}
			else
			{
				isOk = false;
				errCode = -2;
				errMsg = "SQL获取IP未返回数据" + callNameSpace;
				AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
			}
			try
			{
				dt.Dispose();
				dt = null;
			}
			catch
			{
			}
			return isOk;

		}



		public bool VerDb_All
		{
			get
			{
				bool isok = false;
				int errCode = 0;
				string errMsg = "";
				VerDb(out isok, out errCode, out errMsg);
				return _verDb_All;
			}
		}
		bool _verDb_All = false;
		public bool VerDb_Interface
		{
			get
			{
				bool isok = false;
				int errCode = 0;
				string errMsg = "";
				VerDb(out isok, out errCode, out errMsg);
				return _verDb_Interface;
			}
		}
		bool _verDb_Interface = false;

		public bool VerDb_hderp
		{
			get
			{
				bool isok = false;
				int errCode = 0;
				string errMsg = "";
				VerDb(out isok, out errCode, out errMsg);
				return _verDb_hderp;

			}
		}
		bool _verDb_hderp = false;

		public bool VerDb_Local
		{
			get
			{
				bool isok = false;
				int errCode = 0;
				string errMsg = "";
				VerDb(out isok, out errCode, out errMsg);
				return _verDb_local;
			}
		}
		bool _verDb_local = false;
		public bool VerDb_Wms
		{
			get
			{
				bool isok = false;
				int errCode = 0;
				string errMsg = "";
				VerDb(out isok, out errCode, out errMsg);
				return _verDb_wms;
			}
		}
		bool _verDb_wms = false;
		/// <summary>
		/// 验证数据库 ,0正常,-1WMS错误,-2本地错误,-3接口服务器错误
		/// </summary>
		/// <returns>0正常,-1WMS错误,-2本地错误,-3接口服务器错误</returns>
		public bool VerDb(out bool isOk, out int errCode, out string errMsg)
		{
			isOk = true;
			errCode = -1;
			errMsg = "";
			string callNameSpace = "";
			try
			{
				System.Reflection.MethodBase callObj = new System.Diagnostics.StackTrace(true).GetFrame(1).GetMethod();
				callNameSpace = " 方法调用者:" + "[" + callObj.DeclaringType.Namespace + "].[" + callObj.DeclaringType.FullName + "].[" + callObj.Name + "]";
			}
			catch
			{
				callNameSpace = "";
			}

			if ((DateTime.Now - _lastVerDb).TotalMinutes > 5)
			{

				try
				{
					if (_dbhLocal.VerificationConn("select top 1 1 from sysobjects"))
					{
						errMsg = "";
						errCode = 0;
						_verDb_local = true;
					}
					else
					{
						errMsg = "本地数据库连接验证失败" + callNameSpace;
						errCode = 1;
						_verDb_local = false;
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
					}
				}
				catch (Exception ee)
				{
					_verDb_local = false;
					errMsg = "本地数据库连接验证失败" + ee.ToString() + callNameSpace;
					errCode = 1;
					AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");

				}


				try
				{
					if (_dbhWms.VerificationConn("select 1 from dual"))
					{
						errMsg = "";
						errCode = 0;
						_verDb_wms = true;
					}
					else
					{
						errMsg = "WMS数据库连接验证失败" + callNameSpace;
						errCode = 2;
						_verDb_wms = false;
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
					}
				}
				catch (Exception ee)
				{
					errMsg = "WMS数据库连接验证失败:" + ee.ToString() + callNameSpace;
					errCode = 2;
					_verDb_wms = false;
					AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
				}



				try
				{
					if (_dbhInterface.VerificationConn("select 1 from dual"))
					{
						errMsg = "";
						errCode = 0;
						_verDb_Interface = true;

					}
					else
					{
						errMsg = "平台接口数据库连接验证失败" + callNameSpace;
						errCode = 2;
						_verDb_Interface = false;
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
					}
				}
				catch (Exception ee)
				{
					errMsg = "平台接口数据库连接验证失败" + callNameSpace;
					errCode = 2;
					_verDb_Interface = false;
					AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
				}

				try
				{
					if (_dbhHdErp.VerificationConn("select 1 from dual"))
					{
						errMsg = "";
						errCode = 0;
						_verDb_hderp = true;
					}
					else
					{
						errMsg = "汇达ERP数据库连接验证失败" + callNameSpace;
						errCode = 2;
						_verDb_hderp = false;
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
					}
				}
				catch (Exception ee)
				{
					errMsg = "汇达数据库连接验证失败" + callNameSpace;
					errCode = 2;
					_verDb_hderp = false;
					AddMsgOut(errMsg, Settings.Setings.EnumMessageType.严重, errCode, "");
				}


			}
			if (_verDb_wms == true && _verDb_local == true && _verDb_Interface == true)
			{
				_lastVerDb = DateTime.Now;
				_verDb_All = true;
				isOk = true;
			}
			else
			{
				_verDb_All = false;
				isOk = false;
			}
			return _verDb_All;

		}

		public bool ServerWmsBCPrintStatus(MOD.SysMod.PrintDataM order2, out bool isOk2, out int errCode2, out string errMsg2)
		{
			isOk2 = false;
			errCode2 = -99;
			errMsg2 = "";
			if (order2 != null)
			{
				if (YJT.Text.Verification.IsNotNullOrEmpty(order2.WMS销售单号))
				{
					long wmsDanjbh = 0;
					int printCount = 0;
					DateTime printDate = YJT.DataBase.Common.ObjectTryToObj(order2.打印日期最新, DateTime.Now);
					if (long.TryParse(order2.WMS销售单号, out wmsDanjbh))
					{

						if (!int.TryParse(order2.打印次数, out printCount))
						{
							printCount = 1;
						}
						string sqlCmd = $@"update wms_out_order aa set aa.printmanid=2446,aa.printflag={printCount.ToString()},printdate={
							YJT.DataBase.Common.ConvertStringValue(
								printDate.ToString("yyyy-MM-dd HH:mm:ss"),
								YJT.DataBase.Common.ConvertType.日期,
								0,
								DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
								false,
								YJT.DataBase.Common.DbType.Oracle
							)} where outid={wmsDanjbh.ToString()} ";
						if (VerDb_Wms == true)
						{
							int dbres = _dbhWms.ExecuteNonQuery(sqlCmd, null, System.Data.CommandType.Text, false, true, "");
							if (dbres > 0)
							{
								isOk2 = true;
								errCode2 = 0;
								errMsg2 = "";
							}
							else
							{
								isOk2 = false;
								errCode2 = -1;
								errMsg2 = "WMS回写更新失败";
							}
						}
						else
						{
							isOk2 = false;
							errCode2 = -2;
							errMsg2 = "WMS数据库连接失败";
						}
					}
					else
					{
						isOk2 = false;
						errCode2 = -3;
						errMsg2 = "出入库ID不为数字";
					}

				}
				else
				{
					isOk2 = false;
					errCode2 = -4;
					errMsg2 = "wms出入库ID为空";
				}
			}
			else
			{
				isOk2 = false;
				errCode2 = -5;
				errMsg2 = "打印对象为null";
			}
			return isOk2;
		}

		public SysMod.PrintDataM ServerGetPrintData(string wmsDanjbh, out bool isOk, out string errMsg, out int errCode)
		{
			isOk = false;
			errMsg = "";
			errCode = 0;
			SysMod.PrintDataM res = null;
			if (VerDb_Wms == true)
			{
				if (YJT.Text.Verification.IsNotNullOrEmpty(wmsDanjbh))
				{
					//reccompanyid WMS 收货单位ID  我这里是  客户ID
					//gcompanyid WMS 原货主单位ID
					string sqlCmd = $@"
select 
	aa.goodsownerid as 货主ID,
	aa.goodsownername as 货主名称,
	aa.printflag as 打印次数,
	'xxxxxxx' as yjt_打印抬头,
	aa.srcexpno as 货主原单据编号, --汇达,其他使用
	aa.outid as wms销售单号, --燕都销售单号

	aa.gcompanyid as 收货单位id,
	aa.reccompanyname as 收货单位名称,
	aa.salername as 业务员,
	aa.total_amt as 总金额,
	aa.receiveaddr as 收货地址,
	aa.reccompanyid as 客户ID,
	aa.exportmemo as 备注,
	sysdate as 发货日期1, --第三方取得是打印日期
	aa.detaillines as 品种总数,
	aa.credate as 开票日期, --打印日期为开票日期+当前时间
	aa.time4 as 发货日期2,
	aa.ZX_SAINPUTMAN as 开票员
from
	wms_out_order_v aa
where
	aa.outid={wmsDanjbh}
";
					string sqlCmd2 = $@"
select 
	Row_Number() Over(order by bb.outdtlid asc) as 序号 ,
	bb.outid wms单据编号,
	bb.outdtlid wms细表序号,
	bb.goodsid as 商品IDWMS,
	dd.goodsownid as 商品IDERP,
	cc.goodsname as 商品名称,
	bb.factname as 生产厂家,
	cc.zx_treatmentscope as 上市持有人,
	bb.goodstype as 规格,
	bb.lotno as 批号,
	ff.drugform as 剂型,
	bb.GOODSQTY as 数量,
	bb.tradegoodspack as 商品单位,
	bb.price as 单价,
	bb.price*bb.GOODSQTY as 总金额,
    round(bb.goodsqty / nvl(bb.packsize, 999999),3) as 件数,
	bb.packsize 包装单位,
	bb.proddate 生产日期,
	bb.VALIDDATE 有效期,
	bb.approvedocno as 批准文号,
	bb.prodarea as 产地,
	gg.ddlname as 运输条件,
	
	ee.scopename as 经营范围名,
	dd.prodlicenseno as 生产许可证,
	dd.registdocno as 注册证,
	case
		when ee.scopename like '%二类精神%' then 1
		when ee.scopename like '%流产%' then 2
		when (ee.scopename like '%冷藏%' and ee.scopename not like '%器械%') then 3
		when ee.scopename like '%含麻%' then 4
		else 0
	end as 经营类别
from
	wms_out_order_dtl_v bb
	left join tpl_pub_goods cc on bb.goodsid=cc.goodsid
	left join tpl_goods dd on bb.ownergoodsid=dd.ownergoodsid
	left join wms_busiscope_def ee on dd.scopedefid=ee.scopedefid
	left join tpl_drugform ff on dd.drugform=ff.drugformno
	left join (select * from sys_ddl_all_v where keyword = 'WMS_GOODS_STORECONDITION') gg on gg.ddlid= dd.storagecondition
where
	bb.outid={wmsDanjbh}
order by
	bb.outdtlid asc
";
					System.Data.DataTable dt = _dbhWms.ExecuteToDataTable(sqlCmd, null, true);
					if (dt.Rows.Count > 0)
					{
						SysMod.PrintDataM t = new SysMod.PrintDataM()
						{
							iShmhz = "0",
							iSjshz = "0",
							iSlchz = "0",
							iSlljj = "0",
							iSshtx = "1",
							iSyshz = "1",
							WMS销售单号 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WMS销售单号"], ""),
							YJT_打印抬头 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["YJT_打印抬头"], ""),
							业务员 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["业务员"], ""),
							发货日期1 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["发货日期1"], ""),
							发货日期2 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["发货日期2"], ""),
							品种总数 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["品种总数"], ""),
							备注 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["备注"], ""),
							客户ID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["客户ID"], ""),
							开票员 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["开票员"], ""),
							开票日期 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["开票日期"], ""),
							总金额 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["总金额"], ""),
							打印次数 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["打印次数"], ""),
							收货单位ID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["收货单位ID"], ""),
							收货单位名称 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["收货单位名称"], ""),
							收货地址 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["收货地址"], ""),
							货主ID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["货主ID"], ""),
							货主原单据编号 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["货主原单据编号"], ""),
							货主名称 = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["货主名称"], ""),
						};
						try
						{
							dt.Dispose();
							dt = null;
						}
						catch { }
						dt = _dbhWms.ExecuteToDataTable(sqlCmd2, null, true);
						double zjjs = 0;
						double zjsj = 0;
						if (dt.Rows.Count > 0)
						{
							try
							{
								foreach (System.Data.DataRow item in dt.Rows)
								{
									SysMod.PrintDataM.Mod_PrintDataD t2 = new SysMod.PrintDataM.Mod_PrintDataD()
									{
										WMS单据编号 = YJT.DataBase.Common.ObjectTryToObj(item["WMS单据编号"], ""),
										WMS细表序号 = YJT.DataBase.Common.ObjectTryToObj(item["WMS细表序号"], ""),
										上市持有人 = YJT.DataBase.Common.ObjectTryToObj(item["上市持有人"], ""),
										产地 = YJT.DataBase.Common.ObjectTryToObj(item["产地"], ""),
										件数 = YJT.DataBase.Common.ObjectTryToObj(item["件数"], ""),
										剂型 = YJT.DataBase.Common.ObjectTryToObj(item["剂型"], ""),
										包装单位 = YJT.DataBase.Common.ObjectTryToObj(item["包装单位"], ""),
										单价 = YJT.DataBase.Common.ObjectTryToObj(item["单价"], ""),
										商品IDERP = YJT.DataBase.Common.ObjectTryToObj(item["商品IDERP"], ""),
										商品IDWMS = YJT.DataBase.Common.ObjectTryToObj(item["商品IDWMS"], ""),
										商品单位 = YJT.DataBase.Common.ObjectTryToObj(item["商品单位"], ""),
										商品名称 = YJT.DataBase.Common.ObjectTryToObj(item["商品名称"], ""),
										序号 = YJT.DataBase.Common.ObjectTryToObj(item["序号"], ""),
										总金额 = YJT.DataBase.Common.ObjectTryToObj(item["总金额"], ""),
										批准文号 = YJT.DataBase.Common.ObjectTryToObj(item["批准文号"], ""),
										批号 = YJT.DataBase.Common.ObjectTryToObj(item["批号"], ""),
										数量 = YJT.DataBase.Common.ObjectTryToObj(item["数量"], ""),
										有效期 = YJT.DataBase.Common.ObjectTryToObj(item["有效期"], ""),
										注册证 = YJT.DataBase.Common.ObjectTryToObj(item["注册证"], ""),
										生产厂家 = YJT.DataBase.Common.ObjectTryToObj(item["生产厂家"], ""),
										生产日期 = YJT.DataBase.Common.ObjectTryToObj(item["生产日期"], ""),
										生产许可证 = YJT.DataBase.Common.ObjectTryToObj(item["生产许可证"], ""),
										经营类别 = YJT.DataBase.Common.ObjectTryToObj(item["经营类别"], ""),
										经营范围名 = YJT.DataBase.Common.ObjectTryToObj(item["经营范围名"], ""),
										规格 = YJT.DataBase.Common.ObjectTryToObj(item["规格"], ""),
										运输条件 = YJT.DataBase.Common.ObjectTryToObj(item["运输条件"], ""),
									};
									zjjs = zjjs + double.Parse(t2.件数);
									zjsj = zjsj + (double.Parse(t2.数量) % double.Parse(t2.包装单位));
									int t_jylb = int.Parse(t2.经营类别);
									switch (t_jylb)
									{
										case 1:
											t.iSjshz = "1";
											//二类
											break;
										case 2:
											t.iSlchz = "1";
											//流产
											break;
										case 3:
											t.iSlljj = "1";
											//冷藏
											break;
										case 4:
											t.iShmhz = "1";
											//含麻
											break;
										default:
											//普通
											break;
									}
									t.mx.Add(t2);
								}
							}
							catch (Exception ee)
							{
								isOk = false;
								errMsg = "细单数据转换失败" + ee.ToString();
								errCode = -4;
							}

						}
						if (t.mx.Count > 0)
						{
							t.总计件数 = zjjs.ToString();
							t.总计散件 = zjsj.ToString();
							t.打印日期最新 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
							int dycs = 0;
							if (int.TryParse(t.打印次数, out dycs))
							{
								dycs++;
							}
							else
							{
								dycs = 1;
							}
							t.打印次数 = dycs.ToString();
							res = t;
							isOk = true;
							errMsg = "";
							errCode = 0;
						}
						else
						{
							isOk = false;
							errMsg = "WMS没有细单数据";
							errCode = -4;
						}
					}
					else
					{
						isOk = false;
						errMsg = "WMS没有总单数据";
						errCode = -3;
					}
				}
				else
				{
					isOk = false;
					errMsg = "WMS单据编号为空";
					errCode = -1;
				}
			}
			else
			{
				isOk = false;
				errMsg = "无法访问WMS数据库";
				errCode = -2;
			}


			return res;



		}

		public long GetMaxClientOrderId(out bool isOk, out int errCode, out string errMsg)
		{
			errMsg = "";
			errCode = -99;
			isOk = false;
			long res = 0;
			if (_clientInfoObj != null)
			{
				string sqlCmd = $@"SELECT ISNULL(MAX(OrderId),0) AS OrderId FROM BllMod_Order WHERE ip='{_clientInfoObj.Ip}' AND Mac='{_clientInfoObj.Mac}' AND ComputerName='{_clientInfoObj.ComputerName}'";
				if (VerDb_Local)
				{
					object dbres = _dbhLocal.ExecuteScalar(sqlCmd, null, true);
					res = YJT.DataBase.Common.ObjectTryToObj(dbres, 0L);
					errMsg = "";
					isOk = true;
					errCode = 0;
				}
				else
				{
					errCode = -2;
					errMsg = "本地数据库连接失败";
				}
			}
			else
			{
				errCode = -1;
				errMsg = "客户端信息为空";
			}
			return res;
		}

		public bool VerVersion(out string errMsg)
		{
			bool res = false;
			errMsg = "";
			if (VerDb_Local)
			{

				string sqlCmd = $@"select top 1 value from serverSet where name='version'";
				System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
				if (dt.Rows.Count > 0)
				{
					string val = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["value"], "");
					if (val == "1.1.7")
					{
						res = true;
					}
					else
					{
						res = false;
						errMsg = "客户机版本不正确";
					}
				}
				else
				{
					res = false;
					errMsg = "验证版本时,无记录";
				}
			}
			else
			{
				res = false;
				errMsg = "数据库连接失败";
			}
			return res;


		}


		#endregion




		#region bll公共部分
		/// <summary>
		/// 检查地址如果地址曾经发过,就直接用之前的信息,没有发过,从百度上获取信息
		/// </summary>
		/// <param name="addressStr"></param>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		private YJT.BaiduService.BaiduMap.GetAddressDetalRes PubGetAddressInfo(string addressStr, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			YJT.BaiduService.BaiduMap.GetAddressDetalRes res = null;
			if (YJT.Text.Verification.IsNotNullOrEmpty(addressStr))
			{
				string sqlCmd = $@"select isnull(sheng,'') as sheng ,isnull(shi,'') as shi,isnull(xian,'') as xian ,isnull(jiedao,'') as jiedao from AddressInfo where isnull(addressStr,'')='{addressStr}'";
				if (VerDb_Local)
				{
					System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, false);
					if (dt.Rows.Count > 0)
					{
						res = new YJT.BaiduService.BaiduMap.GetAddressDetalRes();
						res.result = new YJT.BaiduService.BaiduMap.GetAddressDetalRes.Result();
						res.result.addressComponent = new YJT.BaiduService.BaiduMap.GetAddressDetalRes.Addresscomponent();
						res.result.addressComponent.province = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["sheng"], "");
						res.result.addressComponent.city = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["shi"], "");
						res.result.addressComponent.district = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["xian"], "");
						res.result.addressComponent.street = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["jiedao"], "");
						isOk = true;
						errCode = 1;
						errMsg = "";

					}
					else
					{
						YJT.BaiduService.BaiduMap.GetAddressDetalRes res1 = YJT.BaiduService.BaiduMap.GetAddressDetalByAddr(addressStr, out errMsg, out errCode, out isOk);
						if (res1 != null && res1.result != null && res1.result.location != null)
						{
							res = YJT.BaiduService.BaiduMap.GetAddressDetal(res1.result.location.lat, res1.result.location.lng, out errMsg, out errCode, out isOk);
							if (res != null && res.result != null && res.result.addressComponent != null)
							{
								//吉林省 吉林市 蛟河市 迎宾大道东民俗村A区3号楼7、8、9、10、11号门市
								//res.result.addressComponent.province "吉林省"
								//res.result.addressComponent.city "吉林市"
								//res.result.addressComponent.district "蛟河市"

								//res.result.addressComponent.street "迎宾大道"
								if (YJT.Text.Verification.IsNullOrEmpty(res.result.addressComponent.province))
								{
									res.result.addressComponent.province = "";
								}
								if (YJT.Text.Verification.IsNullOrEmpty(res.result.addressComponent.city))
								{
									res.result.addressComponent.city = "";
								}
								if (YJT.Text.Verification.IsNullOrEmpty(res.result.addressComponent.district))
								{
									res.result.addressComponent.district = "";
								}
								if (YJT.Text.Verification.IsNullOrEmpty(res.result.addressComponent.street))
								{
									res.result.addressComponent.street = "";
								}
								sqlCmd = $@"INSERT INTO AddressInfo
		( AddressStr , sheng , shi , xian , jiedao )
	VALUES
		( '{addressStr}' , -- AddressStr - varchar(8000)
			'{res.result.addressComponent.province}' , -- sheng - varchar(1000)
			'{res.result.addressComponent.city}' , -- shi - varchar(1000)
			'{res.result.addressComponent.district}' , -- xian - varchar(1000)
			'{res.result.addressComponent.street}'  -- jiedao - varchar(1000)
			)";
								int dbres = _dbhLocal.ExecuteNonQuery(sqlCmd, null, System.Data.CommandType.Text, false, true, "");
								if (dbres > 0)
								{
									isOk = true;
									errMsg = "";
									errCode = 0;
								}
								else
								{
									isOk = true;
									errMsg = "本地数据库写入失败";
									errCode = 1;
								}

							}
							else
							{
								isOk = false;
								errCode = -4;
								errMsg = "经纬度转换地址信息失败";
							}
						}
						else
						{
							isOk = false;
							errCode = -3;
							errMsg = "地址经纬度转换失败";
						}

					}
					try
					{
						dt.Dispose();
						dt = null;
					}
					catch { }
				}
				else
				{
					isOk = false;
					errCode = -2;
					errMsg = "本地数据库连接失败";
				}
			}
			else
			{
				isOk = false;
				errCode = -1;
				errMsg = "地址字符串为空";
			}
			return res;

		}
		#endregion






































































		#region 客户端部分
		/// <summary>
		/// 下传新扫码单据
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="isOK"></param>
		/// <param name="errCode">1 新增子单任务,0 下传完成,-1单据存在,-2写入失败</param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public bool SendNewOrder(BllMod.Order order, out bool isOK, out int errCode, out string errMsg)
		{
			isOK = false;
			errCode = -99;
			errMsg = "";
			//定义语句1
			string sqlCmd = $@"
SELECT
	aa.Bid ,aa.OrderId ,aa.AddDate ,aa.ErpId ,aa.Logic ,aa.Weight ,aa.Length ,aa.Width ,aa.Height ,aa.Status ,aa.Ip ,aa.Mac ,aa.ComputerName ,aa.ErrMsg ,aa.ClientId
	,aa.PrintStatus ,aa.WmsDanjbh ,aa.netDanjbh,CONVERT(VARCHAR(23),ISNULL(aa.ServerHandleDate,CAST('1970-01-01' AS datetime)),21) as ServerHandleDate,aa.ClientHandleDate,
	aa.WmsYewy,aa.WmsDdwname,aa.NETORDER_FROMID,aa.NETORDER_FROM,aa.ERPORDER_ID,aa.CUSTOMID,aa.CUSTOMNAME,aa.AGENTNAME,aa.ADDRESS,aa.WL_COMPANYID,aa.WL_NUMBER
	,aa.WL_COMPANYNAME,aa.RECVNAME,aa.RECVPHONE,aa.PROVINCENAME,aa.CITYNAME,aa.DISTRICTNAME,aa.STREETNAME,aa.ORIGINALREMARK,aa.IsFp,aa.IsPj,aa.IsHeTong,aa.IsQysy,aa.IsSyzz
	,aa.IsYjbb,aa.PlatformType,aa.logi_dstRoute,aa.logi_PayType,aa.logi_monAccNum,aa.logi_baojiaJine,aa.logi_dsJine,aa.logi_logcNum,aa.logi_ysJine,aa.logi_ysJineTotal
	,aa.logi_shouhuory,aa.logi_jinjianRiqi,aa.logi_shoufqianshu,aa.logi_shoufRiqi,aa.logi_sendSheng,aa.logi_sendShi,aa.logi_sendXian,aa.logi_sendAddress,aa.logi_sendMan
	,aa.logi_sendPhone,aa.logi_feiyongTotal,aa.logi_goodQty,aa.logi_goodName,aa.needBaojia,aa.logi_OrderId,aa.logi_CreateDate,aa.logi_ChanpinTypeStr
	,aa.PrintDatetime,aa.sysFirst,aa.total_amt,aa.RelBId,aa.fplx,aa.ServerTaskType,aa.PAIDCOSTS,aa.logi_ReceivePwd,aa.logi_SubOrderSn,aa.logi_PhonNum,aa.logi_TelNum,aa.RelOrderId
	,aa.JingdongWl
	
FROM
	BllMod_Order aa
WHERE
	aa.ErpId='{order.ErpId}'
	and isnull(aa.WL_NUMBER,'') not like '已删%'
	and isnull(ServerTaskType,'无') in(
	'{Settings.Setings.EnumServerTaskType.新单据}','{Settings.Setings.EnumServerTaskType.ERP组合单}')
order by
	aa.Bid desc

";

			System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
			if (order.ServerTaskType == Setings.EnumServerTaskType.ERP组合单 || order.ServerTaskType == Setings.EnumServerTaskType.ERP组合单子单)
			{
				//erp组合单
				if (dt.Rows.Count > 0)
				{
					Settings.Setings.EnumOrderStatus status = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["Status"], Settings.Setings.EnumOrderStatus.异常_类型转换失败);
					Settings.Setings.EnumOrderPrintStatus printStatus = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["PrintStatus"], Settings.Setings.EnumOrderPrintStatus.异常_类型转换失败);
					if (status == Settings.Setings.EnumOrderStatus.单据完成 && printStatus == Settings.Setings.EnumOrderPrintStatus.已打印)
					{
						//新增子单物流任务
						order.ServerTaskType = Setings.EnumServerTaskType.添加物流子单;
						order.Status = Setings.EnumOrderStatus.追加物流子单;
						order.RelBId = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["Bid"], 0L);
						ClientAddNewOrder(order, out isOK, out errMsg, out errCode, out sqlCmd);
					}
					else
					{
						isOK = false;
						errCode = -1;
						errMsg = "单据:" + order.ErpId + "已经在" +
							YJT.DataBase.Common.ObjectTryToObj<DateTime>(dt.Rows[0]["AddDate"], DateTime.Parse("1900-01-01")).ToString("yyyy-MM-dd HH:mm:ss") +
							"\r\nIP:" + YJT.DataBase.Common.ObjectTryToObj<string>(dt.Rows[0]["Ip"], "无") + "下传过";
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, -1, "");
						order.Status = Settings.Setings.EnumOrderStatus.异常_曾经下传过;
						order.ErrMsg = errMsg;
					}
				}
				else
				{
					if (order.ServerTaskType == Setings.EnumServerTaskType.ERP组合单子单)
					{
						order.Status = Settings.Setings.EnumOrderStatus.ERP组合单子单;
					}
					else
					{
						order.Status = Settings.Setings.EnumOrderStatus.ERP组合单;
					}

					ClientAddNewOrder(order, out isOK, out errMsg, out errCode, out sqlCmd);
					if (isOK == true)
					{
						errMsg = "";
						errCode = 0;
						isOK = true;
						order.Status = Settings.Setings.EnumOrderStatus.已下传;
						order.ErrMsg = "已下传,等待服务端处理";

					}
					else
					{
						errMsg = "写入失败:\r\n" + sqlCmd;
						errCode = -2;
						isOK = false;
						order.Status = Settings.Setings.EnumOrderStatus.异常_写入失败;
						order.ErrMsg = errMsg;
					}
				}
			}
			else if (order.ServerTaskType == Setings.EnumServerTaskType.终止订单)
			{
				if (dt.Rows.Count > 0)
				{
					//新增子单物流任务
					order.RelBId = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["Bid"], 0L);
					order.Status = Setings.EnumOrderStatus.准备停止;
					ClientAddNewOrder(order, out isOK, out errMsg, out errCode, out sqlCmd);
				}
				else
				{
					order.Status = Setings.EnumOrderStatus.异常_ERPID不正确;
					order.ErrMsg = "要删除的单据不存在";
					order.ServerTaskType = Setings.EnumServerTaskType.无;
					isOK = false;
					errCode = 0;
					errMsg = order.ErrMsg;
				}
			}
			else
			{
				//普通单据
				if (dt.Rows.Count > 0)
				{
					//存在单据编号
					Settings.Setings.EnumOrderStatus status = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["Status"], Settings.Setings.EnumOrderStatus.异常_类型转换失败);
					Settings.Setings.EnumOrderPrintStatus printStatus = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["PrintStatus"], Settings.Setings.EnumOrderPrintStatus.异常_类型转换失败);
					if (status == Settings.Setings.EnumOrderStatus.单据完成 && printStatus == Settings.Setings.EnumOrderPrintStatus.已打印)
					{
						//新增子单物流任务
						order.ServerTaskType = Setings.EnumServerTaskType.添加物流子单;
						order.Status = Setings.EnumOrderStatus.追加物流子单;
						order.RelBId = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["Bid"], 0L);
						ClientAddNewOrder(order, out isOK, out errMsg, out errCode, out sqlCmd);
					}
					else
					{
						isOK = false;
						errCode = -1;
						errMsg = "单据:" + order.ErpId + "已经在" +
							YJT.DataBase.Common.ObjectTryToObj<DateTime>(dt.Rows[0]["AddDate"], DateTime.Parse("1900-01-01")).ToString("yyyy-MM-dd HH:mm:ss") +
							"\r\nIP:" + YJT.DataBase.Common.ObjectTryToObj<string>(dt.Rows[0]["Ip"], "无") + "下传过";
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, -1, "");
						order.Status = Settings.Setings.EnumOrderStatus.异常_曾经下传过;
						order.ErrMsg = errMsg;
					}

				}
				else
				{
					order.ServerTaskType = Setings.EnumServerTaskType.新单据;
					order.Status = Settings.Setings.EnumOrderStatus.已下传;
					ClientAddNewOrder(order, out isOK, out errMsg, out errCode, out sqlCmd);
					if (isOK == true)
					{
						errMsg = "";
						errCode = 0;
						isOK = true;
						order.Status = Settings.Setings.EnumOrderStatus.已下传;
						order.ErrMsg = "已下传,等待服务端处理";

					}
					else
					{
						errMsg = "写入失败:\r\n" + sqlCmd;
						errCode = -2;
						isOK = false;
						order.Status = Settings.Setings.EnumOrderStatus.异常_写入失败;
						order.ErrMsg = errMsg;
					}
				}
			}
			try
			{
				dt.Dispose();
				dt = null;
			}
			catch { }

			return isOK;
		}

		public List<string> ClientGetAllYewys()
		{
			List<string> res = new List<string>();
			if (VerDb_Local == true)
			{
				string sqlCmd = "SELECT DISTINCT AGENTNAME FROM BllMod_Order WHERE ISNULL(AGENTNAME,'')<>''";
				System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
				if (dt.Rows.Count > 0)
				{
					foreach (System.Data.DataRow item in dt.Rows)
					{
						res.Add(YJT.DataBase.Common.ObjectTryToObj(item["AGENTNAME"], ""));
					}
				}
			}
			return res;
		}

		public List<SysMod.ClinetTag> ClientGetAllClients()
		{
			List<SysMod.ClinetTag> res = new List<SysMod.ClinetTag>();
			if (VerDb_Local == true)
			{
				string sqlCmd = "SELECT DISTINCT Ip,Mac,ComputerName FROM BllMod_Order";
				System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
				if (dt.Rows.Count > 0)
				{
					foreach (System.Data.DataRow item in dt.Rows)
					{
						SysMod.ClinetTag t = new SysMod.ClinetTag();
						t.Ip = YJT.DataBase.Common.ObjectTryToObj(item["Ip"], "");
						t.Mac = YJT.DataBase.Common.ObjectTryToObj(item["Mac"], "");
						t.ComputerName = YJT.DataBase.Common.ObjectTryToObj(item["ComputerName"], "");
						res.Add(t);
					}
				}
			}
			return res;
		}

		/// <summary>
		/// 获取未完成的单据
		/// </summary>
		/// <returns></returns>
		public List<BllMod.Order> GetNotComplateOrder(out bool isOK, out int errCode, out string errMsg)
		{
			isOK = false;
			errCode = -99;
			errMsg = "测试";
			List<BllMod.Order> res = new List<BllMod.Order>();
			if (_clientInfoObj != null)
			{
				//定义语句
				string sqlCmd = $@"
SELECT
	aa.Bid ,
	aa.OrderId ,
	aa.AddDate ,
	aa.ErpId ,
	aa.Logic ,
	aa.Weight ,
	aa.Length ,
	aa.Width ,
	aa.Height ,
	aa.Status ,
	bb.clientIP Ip ,
	bb.clientMac Mac ,
	bb.clientName ComputerName ,
	aa.ErrMsg ,
	aa.PrintStatus ,
	aa.WmsDanjbh ,
	aa.netDanjbh ,
	bb.id ClientId,
	CONVERT(VARCHAR(23),ISNULL(aa.ServerHandleDate,CAST('1970-01-01' AS datetime)),21) as ServerHandleDate,
	aa.ClientHandleDate,
	aa.WmsYewy,
	aa.WmsDdwname,
	aa.NETORDER_FROMID,
	aa.NETORDER_FROM,
	aa.ERPORDER_ID,
	aa.CUSTOMID,
	aa.CUSTOMNAME,
	aa.AGENTNAME,
	aa.ADDRESS,
	aa.WL_COMPANYID,
	aa.WL_NUMBER,
	aa.WL_COMPANYNAME,
	aa.RECVNAME,
	aa.RECVPHONE,
	aa.PROVINCENAME,
	aa.CITYNAME,
	aa.DISTRICTNAME,
	aa.STREETNAME,
	aa.ORIGINALREMARK,
	aa.IsFp,
	aa.IsPj,
	aa.IsHeTong,
	aa.IsQysy,
	aa.IsSyzz,
	aa.IsYjbb,
	aa.PlatformType,
	aa.logi_dstRoute,
	aa.logi_PayType,
	aa.logi_monAccNum,
	aa.logi_baojiaJine,
	aa.logi_dsJine,
	aa.logi_logcNum,
	aa.logi_ysJine,
	aa.logi_ysJineTotal,
	aa.logi_shouhuory,
	aa.logi_jinjianRiqi,
	aa.logi_shoufqianshu,
	aa.logi_shoufRiqi,
	aa.logi_sendSheng,
	aa.logi_sendShi,
	aa.logi_sendXian,
	aa.logi_sendAddress,
	aa.logi_sendMan,
	aa.logi_sendPhone,
	aa.logi_feiyongTotal,
	aa.logi_goodQty,
	aa.logi_goodName,
	aa.needBaojia,
	aa.logi_OrderId,
	aa.logi_CreateDate,
	aa.logi_ChanpinTypeStr,
	aa.PrintDatetime,
	aa.sysFirst,
	aa.total_amt,
	aa.RelBId,
	aa.fplx,
	aa.ServerTaskType,
	aa.PAIDCOSTS,
	aa.logi_ReceivePwd,
	aa.logi_SubOrderSn,
	aa.logi_PhonNum,
	aa.logi_TelNum,
	aa.RelOrderId,
	aa.JingdongWl
 FROM
	BllMod_Order aa
	LEFT JOIN info_Client bb ON aa.ClientId=bb.id
 WHERE
	ISNULL(bb.id,-1)={_clientInfoObj.ClientId}
	/*and isnull(aa.Status,'') not in('单据完成','补打完成')*/
	and convert(varchar(10),AddDate,21)='{DateTime.Now.ToString("yyyy-MM-dd")}'
order by
	aa.OrderId asc

";
				System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
				if (dt.Rows.Count > 0)
				{
					foreach (System.Data.DataRow item in dt.Rows)
					{
						try
						{
							//构建对象
							BllMod.Order t = new BllMod.Order()
							{
								AddDate = YJT.DataBase.Common.ObjectTryToObj(item["AddDate"], DateTime.MinValue),
								ClientId = YJT.DataBase.Common.ObjectTryToObj(item["ClientId"], -1),
								ComputerName = YJT.DataBase.Common.ObjectTryToObj(item["ComputerName"], ""),
								ErpId = YJT.DataBase.Common.ObjectTryToObj(item["ErpId"], ""),
								ErrMsg = YJT.DataBase.Common.ObjectTryToObj(item["ErrMsg"], ""),
								Height = YJT.DataBase.Common.ObjectTryToObj(item["Height"], 1d),
								Bid = YJT.DataBase.Common.ObjectTryToObj(item["Bid"], -1),
								Ip = YJT.DataBase.Common.ObjectTryToObj(item["Ip"], ""),
								Length = YJT.DataBase.Common.ObjectTryToObj(item["Length"], 1d),
								Logic = YJT.DataBase.Common.ObjectTryToObj(item["Logic"], Settings.Setings.EnumLogicType.Default),
								Mac = YJT.DataBase.Common.ObjectTryToObj(item["Mac"], ""),
								NetDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["NetDanjbh"], ""),
								OrderId = YJT.DataBase.Common.ObjectTryToObj(item["OrderId"], -1),
								PrintStatus = YJT.DataBase.Common.ObjectTryToObj(item["PrintStatus"], Settings.Setings.EnumOrderPrintStatus.不可打印),
								Status = YJT.DataBase.Common.ObjectTryToObj(item["Status"], Settings.Setings.EnumOrderStatus.异常_类型转换失败),
								Weight = YJT.DataBase.Common.ObjectTryToObj(item["Weight"], 1d),
								Width = YJT.DataBase.Common.ObjectTryToObj(item["Width"], 1d),
								WmsDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["WmsDanjbh"], ""),
								ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ClientHandleDate"], DateTime.Parse("1900-01-01")),
								ServerHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ServerHandleDate"], DateTime.Parse("1900-01-01")),
								WmsYewy = YJT.DataBase.Common.ObjectTryToObj(item["WmsYewy"], ""),
								WmsDdwname = YJT.DataBase.Common.ObjectTryToObj(item["WmsDdwname"], ""),
								NETORDER_FROMID = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROMID"], ""),

								NETORDER_FROM = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROM"], ""),
								ERPORDER_ID = YJT.DataBase.Common.ObjectTryToObj(item["ERPORDER_ID"], ""),
								CUSTOMID = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMID"], ""),
								CUSTOMNAME = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMNAME"], ""),
								AGENTNAME = YJT.DataBase.Common.ObjectTryToObj(item["AGENTNAME"], ""),
								ADDRESS = YJT.DataBase.Common.ObjectTryToObj(item["ADDRESS"], ""),
								WL_COMPANYID = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYID"], ""),
								WL_NUMBER = YJT.DataBase.Common.ObjectTryToObj(item["WL_NUMBER"], ""),
								WL_COMPANYNAME = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYNAME"], ""),
								RECVNAME = YJT.DataBase.Common.ObjectTryToObj(item["RECVNAME"], ""),
								RECVPHONE = YJT.DataBase.Common.ObjectTryToObj(item["RECVPHONE"], ""),
								PROVINCENAME = YJT.DataBase.Common.ObjectTryToObj(item["PROVINCENAME"], ""),
								CITYNAME = YJT.DataBase.Common.ObjectTryToObj(item["CITYNAME"], ""),
								DISTRICTNAME = YJT.DataBase.Common.ObjectTryToObj(item["DISTRICTNAME"], ""),
								STREETNAME = YJT.DataBase.Common.ObjectTryToObj(item["STREETNAME"], ""),
								ORIGINALREMARK = YJT.DataBase.Common.ObjectTryToObj(item["ORIGINALREMARK"], ""),
								IsFp = YJT.DataBase.Common.ObjectTryToObj(item["IsFp"], ""),
								IsPj = YJT.DataBase.Common.ObjectTryToObj(item["IsPj"], ""),
								IsHeTong = YJT.DataBase.Common.ObjectTryToObj(item["IsHeTong"], ""),
								IsQysy = YJT.DataBase.Common.ObjectTryToObj(item["IsQysy"], ""),
								IsSyzz = YJT.DataBase.Common.ObjectTryToObj(item["IsSyzz"], ""),
								IsYjbb = YJT.DataBase.Common.ObjectTryToObj(item["IsYjbb"], ""),
								PlatformType = YJT.DataBase.Common.ObjectTryToObj(item["PlatformType"], Settings.Setings.EnumPlatformType.无),
								logi_dstRoute = YJT.DataBase.Common.ObjectTryToObj(item["logi_dstRoute"], ""),
								logi_PayType = YJT.DataBase.Common.ObjectTryToObj(item["logi_PayType"], ""),
								logi_monAccNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_monAccNum"], ""),
								logi_baojiaJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_baojiaJine"], ""),
								logi_dsJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_dsJine"], ""),
								logi_logcNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_logcNum"], ""),
								logi_ysJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJine"], ""),
								logi_ysJineTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJineTotal"], ""),
								logi_shouhuory = YJT.DataBase.Common.ObjectTryToObj(item["logi_shouhuory"], ""),
								logi_jinjianRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_jinjianRiqi"], ""),
								logi_shoufqianshu = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufqianshu"], ""),
								logi_shoufRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufRiqi"], ""),
								logi_sendSheng = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendSheng"], ""),
								logi_sendShi = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendShi"], ""),
								logi_sendXian = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendXian"], ""),
								logi_sendAddress = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendAddress"], ""),
								logi_sendMan = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendMan"], ""),
								logi_sendPhone = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendPhone"], ""),
								logi_feiyongTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_feiyongTotal"], ""),
								logi_goodQty = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodQty"], ""),
								logi_goodName = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodName"], ""),
								needBaojia = YJT.DataBase.Common.ObjectTryToObj(item["needBaojia"], 0d),
								logi_OrderId = YJT.DataBase.Common.ObjectTryToObj(item["logi_OrderId"], ""),
								logi_CreateDate = YJT.DataBase.Common.ObjectTryToObj(item["logi_CreateDate"], ""),
								logi_ChanpinTypeStr = YJT.DataBase.Common.ObjectTryToObj(item["logi_ChanpinTypeStr"], ""),
								PrintDatetime = YJT.DataBase.Common.ObjectTryToObj(item["PrintDatetime"], ""),
								sysFirst = YJT.DataBase.Common.ObjectTryToObj(item["sysFirst"], ""),
								total_amt = YJT.DataBase.Common.ObjectTryToObj(item["total_amt"], "0"),
								RelBId = YJT.DataBase.Common.ObjectTryToObj(item["RelBId"], -1L),
								fplx = YJT.DataBase.Common.ObjectTryToObj(item["fplx"], ""),
								ServerTaskType = YJT.DataBase.Common.ObjectTryToObj(item["ServerTaskType"], Settings.Setings.EnumServerTaskType.异常_类型转换失败),
								PAIDCOSTS = YJT.DataBase.Common.ObjectTryToObj(item["PAIDCOSTS"], 0d),
								logi_ReceivePwd = YJT.DataBase.Common.ObjectTryToObj(item["logi_ReceivePwd"], ""),
								logi_SubOrderSn = YJT.DataBase.Common.ObjectTryToObj(item["logi_SubOrderSn"], 0),
								logi_PhonNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_PhonNum"], ""),
								logi_TelNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_TelNum"], ""),
								RelOrderId = YJT.DataBase.Common.ObjectTryToObj(item["RelOrderId"], -1L),
								JingdongWl = YJT.DataBase.Common.ObjectTryToObj(item["JingdongWl"], ""),

							};
							res.Add(t);
						}
						catch (Exception ee)
						{
							AddMsgOut("历史记录添加实例化对象时候出错", Settings.Setings.EnumMessageType.提示, -1, ee.ToString());
						}

					}
				}
				try
				{
					dt.Dispose();
					dt = null;
				}
				catch { }
			}

			errCode = 0;
			isOK = true;
			errMsg = "";





			return res;
		}
		/// <summary>
		/// 客户端获取服务端已经处理完成的队列
		/// </summary>
		/// <param name="isOk"></param>
		/// <param name="errMsg"></param>
		/// <param name="errCode"></param>
		/// <returns></returns>
		public List<BllMod.Order> GetServerHandledList(out bool isOk, out string errMsg, out int errCode)
		{
			isOk = false;
			errMsg = "";
			errCode = 0;
			List<BllMod.Order> res = new List<BllMod.Order>();
			//定义语句
			string sqlCmd = $@"
select
	isnull(Bid,-1) as Bid ,
	isnull(OrderId,'') as OrderId ,
	isnull(AddDate,convert(datetime,'1900-01-01')) as AddDate ,
	isnull(ErpId,'') as ErpId ,
	isnull(Logic,'Default') as Logic ,
	isnull(Weight,1) as Weight ,
	isnull(Length,1) as Length ,
	isnull(Width,1) as Width ,
	isnull(Height,1) as Height ,
	isnull(Status,'无动作') as Status ,
	isnull(Ip,'') as Ip ,
	isnull(Mac,'') as  Mac,
	isnull(ComputerName,'') as ComputerName ,
	isnull(ErrMsg,'') as ErrMsg ,
	isnull(ClientId,-1) as ClientId ,
	isnull(PrintStatus,'不可打印') as PrintStatus ,
	isnull(WmsDanjbh,'') as WmsDanjbh ,
	isnull(netDanjbh,'') as netDanjbh ,
	CONVERT(VARCHAR(23),ISNULL(ServerHandleDate,CAST('1970-01-01' AS datetime)),21) as ServerHandleDate ,
	isnull(ClientHandleDate,convert(datetime,'1900-01-01')) as ClientHandleDate,
	isnull(WmsYewy,'') as WmsYewy,
	isnull(WmsDdwname,'') as WmsDdwname,
	isnull(NETORDER_FROMID,'') as NETORDER_FROMID,
	isnull(NETORDER_FROM,'') as NETORDER_FROM,
	isnull(ERPORDER_ID,'') as ERPORDER_ID,
	isnull(CUSTOMID,'') as CUSTOMID,
	isnull(CUSTOMNAME,'') as CUSTOMNAME,
	isnull(AGENTNAME,'') as AGENTNAME,
	isnull(ADDRESS,'') as ADDRESS,
	isnull(WL_COMPANYID,'') as WL_COMPANYID,
	isnull(WL_NUMBER,'') as WL_NUMBER,
	isnull(WL_COMPANYNAME,'') as WL_COMPANYNAME,
	isnull(RECVNAME,'') as RECVNAME,
	isnull(RECVPHONE,'') as RECVPHONE,
	isnull(PROVINCENAME,'') as PROVINCENAME,
	isnull(CITYNAME,'') as CITYNAME,
	isnull(DISTRICTNAME,'') as DISTRICTNAME,
	isnull(STREETNAME,'') as STREETNAME,
	isnull(ORIGINALREMARK,'') as ORIGINALREMARK,
	isnull(IsFp,'') as IsFp,
	isnull(IsPj,'') as IsPj,
	isnull(IsHeTong,'') as IsHeTong,
	isnull(IsQysy,'') as IsQysy,
	isnull(IsSyzz,'') as IsSyzz,
	isnull(IsYjbb,'') as IsYjbb,
	isnull(PlatformType,'无') as PlatformType,
	isnull(logi_dstRoute,'') as logi_dstRoute,
	isnull(logi_PayType,'') as logi_PayType,
	isnull(logi_monAccNum,'') as logi_monAccNum,
	isnull(logi_baojiaJine,'') as logi_baojiaJine,
	isnull(logi_dsJine,'') as logi_dsJine,
	isnull(logi_logcNum,'') as logi_logcNum,
	isnull(logi_ysJine,'') as logi_ysJine,
	isnull(logi_ysJineTotal,'') as logi_ysJineTotal,
	isnull(logi_shouhuory,'') as logi_shouhuory,
	isnull(logi_jinjianRiqi,'') as logi_jinjianRiqi,
	isnull(logi_shoufqianshu,'') as logi_shoufqianshu,
	isnull(logi_shoufRiqi,'') as logi_shoufRiqi,
	isnull(logi_sendSheng,'') as logi_sendSheng,
	isnull(logi_sendShi,'') as logi_sendShi,
	isnull(logi_sendXian,'') as logi_sendXian,
	isnull(logi_sendAddress,'') as logi_sendAddress,
	isnull(logi_sendMan,'') as logi_sendMan,
	isnull(logi_sendPhone,'') as logi_sendPhone,
	isnull(logi_feiyongTotal,'') as logi_feiyongTotal,
	isnull(logi_goodQty,'') as logi_goodQty,
	isnull(logi_goodName,'') as logi_goodName,
	isnull(needBaojia,0) as needBaojia,
	isnull(logi_OrderId,'') as logi_OrderId,
	isnull(logi_CreateDate,'') as logi_CreateDate,
	isnull(logi_ChanpinTypeStr,'') as logi_ChanpinTypeStr,
	isnull(PrintDatetime,'') as PrintDatetime,
	isnull(sysFirst,'') as sysFirst,
	isnull(total_amt,'0') as total_amt,
	isnull(RelBId,-1) as RelBId,
	isnull(fplx,'') as fplx,
	isnull(ServerTaskType,'异常_类型转换失败') as ServerTaskType,
	isnull(PAIDCOSTS,0) as PAIDCOSTS,
	isnull(logi_ReceivePwd,'') as logi_ReceivePwd,
	isnull(logi_SubOrderSn,0) as logi_SubOrderSn,
	isnull(logi_PhonNum,'') as logi_PhonNum,
	isnull(logi_TelNum,'') as logi_TelNum,
	isnull(RelOrderId,-1) as RelOrderId,
	isnull(JingdongWl,'') as JingdongWl

from
	BllMod_Order
where
	isnull(ServerHandleDate,convert(datetime,'1900-01-01'))>isnull(ClientHandleDate,convert(datetime,'1900-01-01'))
	AND isnull(Mac,'')='{_clientInfoObj.Mac}' AND isnull(ComputerName,'')='{_clientInfoObj.ComputerName}' AND isnull(Ip,'')='{_clientInfoObj.Ip}'
	and isnull(Status,'无动作') not in
	(
		'{Settings.Setings.EnumOrderStatus.无动作.ToString()}'
	)
";
			System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
			if (dt.Rows.Count > 0)
			{
				foreach (System.Data.DataRow item in dt.Rows)
				{
					try
					{
						//构建对象
						BllMod.Order t = new BllMod.Order()
						{
							AddDate = YJT.DataBase.Common.ObjectTryToObj(item["AddDate"], DateTime.MinValue),
							ClientId = YJT.DataBase.Common.ObjectTryToObj(item["ClientId"], -1),
							ComputerName = YJT.DataBase.Common.ObjectTryToObj(item["ComputerName"], ""),
							ErpId = YJT.DataBase.Common.ObjectTryToObj(item["ErpId"], ""),
							ErrMsg = YJT.DataBase.Common.ObjectTryToObj(item["ErrMsg"], ""),
							Height = YJT.DataBase.Common.ObjectTryToObj(item["Height"], 1d),
							Bid = YJT.DataBase.Common.ObjectTryToObj(item["Bid"], -1),
							Ip = YJT.DataBase.Common.ObjectTryToObj(item["Ip"], ""),
							Length = YJT.DataBase.Common.ObjectTryToObj(item["Length"], 1d),
							Logic = YJT.DataBase.Common.ObjectTryToObj(item["Logic"], Settings.Setings.EnumLogicType.Default),
							Mac = YJT.DataBase.Common.ObjectTryToObj(item["Mac"], ""),
							NetDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["NetDanjbh"], ""),
							OrderId = YJT.DataBase.Common.ObjectTryToObj(item["OrderId"], -1),
							PrintStatus = YJT.DataBase.Common.ObjectTryToObj(item["PrintStatus"], Settings.Setings.EnumOrderPrintStatus.不可打印),
							Status = YJT.DataBase.Common.ObjectTryToObj(item["Status"], Settings.Setings.EnumOrderStatus.异常_类型转换失败),
							Weight = YJT.DataBase.Common.ObjectTryToObj(item["Weight"], 1d),
							Width = YJT.DataBase.Common.ObjectTryToObj(item["Width"], 1d),
							WmsDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["WmsDanjbh"], ""),
							ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ClientHandleDate"], DateTime.Parse("1900-01-01")),
							ServerHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ServerHandleDate"], DateTime.Parse("1900-01-01")),
							WmsYewy = YJT.DataBase.Common.ObjectTryToObj(item["WmsYewy"], ""),
							WmsDdwname = YJT.DataBase.Common.ObjectTryToObj(item["WmsDdwname"], ""),
							NETORDER_FROMID = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROMID"], ""),
							NETORDER_FROM = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROM"], ""),
							ERPORDER_ID = YJT.DataBase.Common.ObjectTryToObj(item["ERPORDER_ID"], ""),
							CUSTOMID = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMID"], ""),
							CUSTOMNAME = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMNAME"], ""),
							AGENTNAME = YJT.DataBase.Common.ObjectTryToObj(item["AGENTNAME"], ""),
							ADDRESS = YJT.DataBase.Common.ObjectTryToObj(item["ADDRESS"], ""),
							WL_COMPANYID = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYID"], ""),
							WL_NUMBER = YJT.DataBase.Common.ObjectTryToObj(item["WL_NUMBER"], ""),
							WL_COMPANYNAME = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYNAME"], ""),
							RECVNAME = YJT.DataBase.Common.ObjectTryToObj(item["RECVNAME"], ""),
							RECVPHONE = YJT.DataBase.Common.ObjectTryToObj(item["RECVPHONE"], ""),
							PROVINCENAME = YJT.DataBase.Common.ObjectTryToObj(item["PROVINCENAME"], ""),
							CITYNAME = YJT.DataBase.Common.ObjectTryToObj(item["CITYNAME"], ""),
							DISTRICTNAME = YJT.DataBase.Common.ObjectTryToObj(item["DISTRICTNAME"], ""),
							STREETNAME = YJT.DataBase.Common.ObjectTryToObj(item["STREETNAME"], ""),
							ORIGINALREMARK = YJT.DataBase.Common.ObjectTryToObj(item["ORIGINALREMARK"], ""),
							IsFp = YJT.DataBase.Common.ObjectTryToObj(item["IsFp"], ""),
							IsPj = YJT.DataBase.Common.ObjectTryToObj(item["IsPj"], ""),
							IsHeTong = YJT.DataBase.Common.ObjectTryToObj(item["IsHeTong"], ""),
							IsQysy = YJT.DataBase.Common.ObjectTryToObj(item["IsQysy"], ""),
							IsSyzz = YJT.DataBase.Common.ObjectTryToObj(item["IsSyzz"], ""),
							IsYjbb = YJT.DataBase.Common.ObjectTryToObj(item["IsYjbb"], ""),
							PlatformType = YJT.DataBase.Common.ObjectTryToObj(item["PlatformType"], Settings.Setings.EnumPlatformType.无),
							logi_dstRoute = YJT.DataBase.Common.ObjectTryToObj(item["logi_dstRoute"], ""),
							logi_PayType = YJT.DataBase.Common.ObjectTryToObj(item["logi_PayType"], ""),
							logi_monAccNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_monAccNum"], ""),
							logi_baojiaJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_baojiaJine"], ""),
							logi_dsJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_dsJine"], ""),
							logi_logcNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_logcNum"], ""),
							logi_ysJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJine"], ""),
							logi_ysJineTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJineTotal"], ""),
							logi_shouhuory = YJT.DataBase.Common.ObjectTryToObj(item["logi_shouhuory"], ""),
							logi_jinjianRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_jinjianRiqi"], ""),
							logi_shoufqianshu = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufqianshu"], ""),
							logi_shoufRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufRiqi"], ""),
							logi_sendSheng = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendSheng"], ""),
							logi_sendShi = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendShi"], ""),
							logi_sendXian = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendXian"], ""),
							logi_sendAddress = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendAddress"], ""),
							logi_sendMan = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendMan"], ""),
							logi_sendPhone = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendPhone"], ""),
							logi_feiyongTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_feiyongTotal"], ""),
							logi_goodQty = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodQty"], ""),
							logi_goodName = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodName"], ""),
							needBaojia = YJT.DataBase.Common.ObjectTryToObj(item["needBaojia"], 0d),
							logi_OrderId = YJT.DataBase.Common.ObjectTryToObj(item["logi_OrderId"], ""),
							logi_CreateDate = YJT.DataBase.Common.ObjectTryToObj(item["logi_CreateDate"], ""),
							logi_ChanpinTypeStr = YJT.DataBase.Common.ObjectTryToObj(item["logi_ChanpinTypeStr"], ""),
							PrintDatetime = YJT.DataBase.Common.ObjectTryToObj(item["PrintDatetime"], ""),
							sysFirst = YJT.DataBase.Common.ObjectTryToObj(item["sysFirst"], ""),
							total_amt = YJT.DataBase.Common.ObjectTryToObj(item["total_amt"], "0"),
							RelBId = YJT.DataBase.Common.ObjectTryToObj(item["RelBId"], -1),
							fplx = YJT.DataBase.Common.ObjectTryToObj(item["fplx"], ""),
							ServerTaskType = YJT.DataBase.Common.ObjectTryToObj(item["ServerTaskType"], Settings.Setings.EnumServerTaskType.异常_类型转换失败),
							PAIDCOSTS = YJT.DataBase.Common.ObjectTryToObj(item["PAIDCOSTS"], 0d),
							logi_ReceivePwd = YJT.DataBase.Common.ObjectTryToObj(item["logi_ReceivePwd"], ""),
							logi_SubOrderSn = YJT.DataBase.Common.ObjectTryToObj(item["logi_SubOrderSn"], 0),
							logi_PhonNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_PhonNum"], ""),
							logi_TelNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_TelNum"], ""),
							RelOrderId = YJT.DataBase.Common.ObjectTryToObj(item["RelOrderId"], -1L),
							JingdongWl = YJT.DataBase.Common.ObjectTryToObj(item["JingdongWl"], ""),
						};
						res.Add(t);
					}
					catch (Exception ee)
					{
						AddMsgOut("获取服务器处理完毕的单据转换出错" + YJT.Json.FunStrSerializeToStr(item), Settings.Setings.EnumMessageType.提示, -1, ee.ToString());
					}

				}
			}
			try
			{
				dt.Dispose();
				dt = null;
			}
			catch { }
			return res;
		}

		public bool ClientAddShutdownServer(BllMod.Order order)
		{
			string errMsg = "";
			bool isOk = false;
			int errCode = 0;
			string sql = "";
			order.Status = Setings.EnumOrderStatus.关机任务;
			order.ServerTaskType = Setings.EnumServerTaskType.关机任务;
			ClientAddNewOrder(order, out isOk, out errMsg, out errCode, out sql);
			return isOk;
		}

		private bool ClientAddNewOrder(BllMod.Order order, out bool isOk, out string errMsg, out int errCode, out string sqlCmd)
		{
			isOk = false;
			errMsg = "";
			errCode = -99;
			sqlCmd = $@"
INSERT INTO BllMod_Order
		(
			OrderId ,AddDate ,ErpId ,Logic ,Weight ,
			Length ,Width ,Height ,Status ,Ip ,
			Mac ,ComputerName ,ErrMsg ,ClientId ,PrintStatus ,
			WmsDanjbh ,netDanjbh,ServerHandleDate,ClientHandleDate,WmsYewy,
			WmsDdwname,NETORDER_FROMID,NETORDER_FROM,ERPORDER_ID,CUSTOMID
			,CUSTOMNAME,AGENTNAME,ADDRESS,WL_COMPANYID,WL_NUMBER
			,WL_COMPANYNAME,RECVNAME,RECVPHONE,PROVINCENAME,CITYNAME
			,DISTRICTNAME,STREETNAME,ORIGINALREMARK,IsFp,IsPj,IsHeTong
			,IsQysy,IsSyzz,IsYjbb,PlatformType,logi_dstRoute
			,logi_PayType,logi_monAccNum,logi_baojiaJine,logi_dsJine,logi_logcNum
			,logi_ysJine,logi_ysJineTotal,logi_shouhuory,logi_jinjianRiqi
			,logi_shoufqianshu,logi_shoufRiqi,logi_sendSheng,logi_sendShi,logi_sendXian
			,logi_sendAddress,logi_sendMan,logi_sendPhone,logi_feiyongTotal,logi_goodQty
			,logi_goodName,needBaojia,logi_OrderId,logi_CreateDate,logi_ChanpinTypeStr
			,PrintDatetime,sysFirst,total_amt,RelBId,fplx,ServerTaskType,PAIDCOSTS
			,logi_ReceivePwd,logi_SubOrderSn,logi_PhonNum,logi_TelNum,RelOrderId
			,JingdongWl
		)
	OUTPUT inserted.Bid,inserted.AddDate,inserted.ServerHandleDate,inserted.ClientHandleDate
	VALUES
		(
			{order.OrderId.ToString()} , -- OrderId - bigint
			GETDATE() , -- AddDate - datetime
			'{order.ErpId}' , -- ErpId - varchar(100)
			'{order.Logic.ToString()}' , -- Logic - varchar(200)
			{order.Weight} , -- Weight - decimal
			{order.Length} , -- Length - decimal
			{order.Width} , -- Width - decimal
			{order.Height} , -- Height - decimal
			'{order.Status.ToString()}' , -- Status - varchar(200)
			'{order.Ip}' , -- Ip - varchar(100)
			'{order.Mac}' , -- Mac - varchar(100)
			'{order.ComputerName}' , -- ComputerName - varchar(300)
			'{"已下传,等待服务端处理"}' , -- ErrMsg - varchar(8000)
			{order.ClientId} , -- ClientId - int
			'{order.PrintStatus.ToString()}' , -- PrintStatus - varchar(200)
			'{order.WmsDanjbh}' , -- WmsDanjbh - varchar(100)
			'{order.NetDanjbh}',  -- netDanjbh - varchar(100)
			GETDATE(),  -- ServerHandleDate - datetime
			GETDATE(),  -- ClientHandleDate - datetime
			'{order.WmsYewy}', --WmsYewy --varchar(100)
			'{order.WmsDdwname}', --WmsDdwname --varchar(1000)
			'{order.NETORDER_FROMID}', --NETORDER_FROMID --varchar(100)
			'{order.NETORDER_FROM}', --NETORDER_FROM --varchar(1000)
			'{order.ERPORDER_ID}', --ERPORDER_ID --varchar(1000)
			'{order.CUSTOMID}', --CUSTOMID --varchar(1000)
			'{order.CUSTOMNAME}', --CUSTOMNAME --varchar(1000)
			'{order.AGENTNAME}', --AGENTNAME --varchar(1000)
			'{order.ADDRESS}', --ADDRESS --varchar(1000)
			'{order.WL_COMPANYID}', --WL_COMPANYID --varchar(1000)
			'{order.WL_NUMBER}', --WL_NUMBER --varchar(1000)
			'{order.WL_COMPANYNAME}', --WL_COMPANYNAME --varchar(1000)
			'{order.RECVNAME}', --RECVNAME --varchar(1000)
			'{order.RECVPHONE}', --RECVPHONE --varchar(1000)
			'{order.PROVINCENAME}', --PROVINCENAME --varchar(1000)
			'{order.CITYNAME}', --CITYNAME --varchar(1000)
			'{order.DISTRICTNAME}', --DISTRICTNAME --varchar(1000)
			'{order.STREETNAME}', --STREETNAME --varchar(1000)
			'{order.ORIGINALREMARK}', --ORIGINALREMARK --varchar(8000)
			'{order.IsFp}', --IsFp --varchar(2000)
			'{order.IsPj}', --IsPj --varchar(2000)
			'{order.IsHeTong}',--IsHeTong --varchar(2000)
			'{order.IsQysy}', --IsQysy --varchar(2000)
			'{order.IsSyzz}', --IsSyzz --varchar(2000)
			'{order.IsYjbb}', --IsYjbb --varchar(2000)
			'{order.PlatformType.ToString()}', --PlatformType --varchar(2000)
			'{order.logi_dstRoute}', --logi_dstRoute --varchar(2000)
			'{order.logi_PayType}', --logi_PayType --varchar(2000)
			'{order.logi_monAccNum}', --logi_monAccNum --varchar(2000)
			'{order.logi_baojiaJine}', --logi_baojiaJine --varchar(2000)
			'{order.logi_dsJine}', --logi_dsJine --varchar(2000)
			'{order.logi_logcNum}', --logi_logcNum --varchar(2000)
			'{order.logi_ysJine}', --logi_ysJine --varchar(2000)
			'{order.logi_ysJineTotal}', --logi_ysJineTotal --varchar(2000)
			'{order.logi_shouhuory}', --logi_shouhuory --varchar(2000)
			'{order.logi_jinjianRiqi}', --logi_jinjianRiqi --varchar(2000)
			'{order.logi_shoufqianshu}', --logi_shoufqianshu --varchar(2000)
			'{order.logi_shoufRiqi}', --logi_shoufRiqi --varchar(2000)
			'{order.logi_sendSheng}', --logi_sendSheng --varchar(2000)
			'{order.logi_sendShi}', --logi_sendShi --varchar(2000)
			'{order.logi_sendXian}', --logi_sendXian --varchar(2000)
			'{order.logi_sendAddress}', --logi_sendAddress --varchar(2000)
			'{order.logi_sendMan}', --logi_sendMan --varchar(2000)
			'{order.logi_sendPhone}', --logi_sendPhone --varchar(2000)
			'{order.logi_feiyongTotal}', --logi_feiyongTotal --varchar(2000)
			'{order.logi_goodQty}', --logi_goodQty --varchar(2000)
			'{order.logi_goodName}', --logi_goodName --varchar(2000)	
			{order.needBaojia.ToString("#0.000")}, --needBaojia --decimal(18,3)	
			'{order.logi_OrderId}', --logi_goodName --varchar(2000)	
			'{order.logi_CreateDate}', --logi_goodName --varchar(2000)	
			'{order.logi_ChanpinTypeStr}', --logi_ChanpinTypeStr --varchar(2000)	
			'{order.PrintDatetime}', --PrintDatetime --varchar(2000)	
			'{order.sysFirst}', --sysFirst --varchar(2000)	
			'{order.total_amt}', --total_amt --varchar(2000)	
			{order.RelBId.ToString()}, --RelBId --bigint
			'{order.fplx}', --fplx --varchar(2000)	
			'{order.ServerTaskType.ToString()}', --ServerTaskType --varchar(2000)
			{order.PAIDCOSTS.ToString()}, --PAIDCOSTS --decimal(18,3)
			'{order.logi_ReceivePwd}', --logi_ReceivePwd --varchar(2000)
			{order.logi_SubOrderSn.ToString()}, --logi_SubOrderSn --int
			'{order.logi_PhonNum}', --logi_PhonNum --varchar(2000)
			'{order.logi_TelNum}', --logi_TelNum --varchar(2000)
			{order.RelOrderId.ToString()}, --RelOrderId --bigint
			'{order.JingdongWl.ToString()}' --JingdongWl --varchar(8000)
		)
";
			System.Data.DataTable dt2 = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
			if (dt2.Rows.Count > 0)
			{
				errMsg = "";
				errCode = 0;
				isOk = true;
				order.AddDate = YJT.DataBase.Common.ObjectTryToObj(dt2.Rows[0]["AddDate"], DateTime.MinValue);
				order.ServerHandleDate = YJT.DataBase.Common.ObjectTryToObj(dt2.Rows[0]["ServerHandleDate"], DateTime.MinValue);
				order.Bid = YJT.DataBase.Common.ObjectTryToObj(dt2.Rows[0]["Bid"], 0L);
				order.ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(dt2.Rows[0]["ClientHandleDate"], DateTime.MinValue);

			}
			else
			{
				errMsg = "写入失败";
				errCode = -2;
				isOk = false;
				order.Status = Settings.Setings.EnumOrderStatus.异常_写入失败;
				order.ErrMsg = errMsg;
			}
			try
			{
				dt2.Dispose();
				dt2 = null;
			}
			catch { }
			return isOk;


		}

		/// <summary>
		/// 客户端要求删除
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="isOK"></param>
		/// <param name="errCode">0,等待服务端删除,1未下传,直接删除</param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public bool ClientRequestDel(BllMod.Order v1, out bool isOK, out int errCode, out string errMsg)
		{
			isOK = true;
			errMsg = "";
			errCode = 0;
			//v1.ErrMsg = "等待服务器删除";
			//string sqlCmd1 = $@"update BllMod_Order set Status='{v1.Status}',ErrMsg='{v1.ErrMsg}',ClientHandleDate=GETDATE(),ServerHandleDate=GETDATE() OUTPUT inserted.ClientHandleDate,inserted.ServerHandleDate where ErpId='{v1.ErpId}'";
			//System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd1, null, true);
			//if (dt.Rows.Count > 0)
			//{
			//	v1.ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ClientHandleDate"], DateTime.MinValue);
			//	v1.ServerHandleDate = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ServerHandleDate"], DateTime.MaxValue);
			//	errCode = 0;
			//}
			//else
			//{
			//	errCode = 1;
			//}
			return isOK;
		}
		/// <summary>
		/// 客户端已经刷新显示
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="isOK"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		public bool ClientViewed(BllMod.Order v1, out bool isOK, out int errCode, out string errMsg)
		{
			isOK = true;
			errMsg = "";
			errCode = 0;
			string sqlCmd1 = $@"update BllMod_Order set ClientHandleDate=GETDATE() OUTPUT inserted.ClientHandleDate where Bid='{v1.Bid}' and CONVERT(VARCHAR(23),ISNULL(ServerHandleDate,CAST('1970-01-01' AS datetime)),21)='{v1.ServerHandleDate.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
			System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd1, null, true);
			if (dt.Rows.Count > 0)
			{
				v1.ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ClientHandleDate"], DateTime.MinValue);
				errCode = 0;
			}
			else
			{
				errCode = 1;
			}
			return isOK;
		}
		#endregion

























































































		#region 服务端部分
		/// <summary>
		/// 强制设置一个单据的状态
		/// </summary>
		/// <param name="bid"></param>
		/// <param name="dberrMsg"></param>
		/// <param name="status"></param>
		/// <param name="printStatus"></param>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg">执行成功后返回ServerHandleDate</param>
		/// <param name="setOtherPara">xxx='xxx' 不要有后面的逗号</param>

		public void ServerForceFinesh(long bid, string dberrMsg, Setings.EnumOrderStatus status, Setings.EnumOrderPrintStatus printStatus, out bool isOk, out int errCode, out string errMsg, params string[] setOtherPara)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			//重点考虑    特殊_不更改此参数
			string prints = "";
			if (printStatus != Setings.EnumOrderPrintStatus.特殊_不更改此参数)
			{
				prints = $@" PrintStatus='{printStatus.ToString()}', ";
			}
			if (setOtherPara != null && setOtherPara.Length > 0)
			{
				foreach (string item in setOtherPara)
				{
					prints = prints + item + " ,";
				}
			}
			string sqlCmd = $@"
UPDATE
	BllMod_Order
SET
	{prints}
	ServerHandleDate=GETDATE(),
	ClientHandleDate=GETDATE()-1,
	ErrMsg='{dberrMsg.Replace("'", "")}',
	Status='{status.ToString()}'
OUTPUT
	Inserted.ServerHandleDate
WHERE Bid={bid.ToString()}
";
			try
			{
				DateTime dbres = YJT.DataBase.Common.ObjectTryToObj(_dbhLocal.ExecuteScalar(sqlCmd, null, true), DateTime.MinValue);
				if (dbres != DateTime.MinValue)
				{
					errMsg = dbres.ToString("yyyy-MM-dd HH:mm:ss");
					isOk = true;
					errCode = 0;
					errMsg = "完成";
				}
				else
				{
					isOk = false;
					errCode = -1;
					errMsg = "没有更新到指定行";
				}
			}
			catch (Exception ee)
			{
				isOk = false;
				errCode = -2;
				errMsg = ee.ToString();
			}

		}
		public List<BllMod.Order> ServerGetOrderByRelOrderId(long RelOrderId, int clientId, out bool isok, out int errCode, out string errMsg)
		{
			isok = false;
			errCode = -99;
			errMsg = "";
			bool isok2 = false;
			int errCode2 = -99;
			string errMsg2 = "";
			List<MOD.BllMod.Order> res = new List<BllMod.Order>();
			if (RelOrderId >= 0)
			{
				if (clientId > 0)
				{
					if (VerDb_Local == true)
					{
						string sqlCmd = $@"select bid from BllMod_Order where ClientId={clientId.ToString()} and RelOrderId={RelOrderId.ToString()}";
						System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
						if (dt.Rows.Count > 0)
						{
							foreach (System.Data.DataRow item in dt.Rows)
							{
								long bid = YJT.DataBase.Common.ObjectTryToObj(item["bid"], -1L);
								if (bid != -1)
								{
									MOD.BllMod.Order t = ServerGetOrder(bid.ToString(), out isok2, out errCode2, out errMsg2);
									if (isok2 == true && t != null)
									{
										res.Add(t);
									}
								}
							}
						}
						try
						{
							dt.Dispose();
							dt = null;
						}
						catch { }
						isok = true;
						errCode = 0;
						errMsg = "";
					}
					else
					{
						errCode = -3;
						errMsg = "本地服务器连接失败";
					}
				}
				else
				{
					errCode = -2;
					errMsg = "客户ID,不能小于0";
				}
			}
			else
			{
				errCode = -1;
				errMsg = "相关ORDERid不能小于0";
			}
			return res;

		}
		/// <summary>
		/// 根据数据库ID,获取一个订单,用于检测当前状态
		/// </summary>
		/// <param name="bid"></param>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public MOD.BllMod.Order ServerGetOrder(string bid, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			bool isOk2 = false;
			int errCode2 = -99;
			string errMsg2 = "";
			MOD.BllMod.Order res = null;
			if (VerDb_Local)
			{
				if (YJT.Text.Verification.IsNotNullOrEmpty(bid))
				{
					//定义语句
					string sqlCmd = $@"
SELECT
	aa.Bid ,
	aa.OrderId ,
	aa.AddDate ,
	aa.ErpId ,
	aa.Logic ,
	aa.Weight ,
	aa.Length ,
	aa.Width ,
	aa.Height ,
	aa.Status ,
	bb.clientIP Ip ,
	bb.clientMac Mac ,
	bb.clientName ComputerName ,
	aa.ErrMsg ,
	aa.PrintStatus ,
	aa.WmsDanjbh ,
	aa.netDanjbh ,
	bb.id ClientId,
	CONVERT(VARCHAR(23),ISNULL(aa.ServerHandleDate,CAST('1970-01-01' AS datetime)),21) as ServerHandleDate,
	aa.ClientHandleDate,
	aa.WmsYewy,
	aa.WmsDdwname,
	aa.NETORDER_FROMID,
	aa.NETORDER_FROM,
	aa.ERPORDER_ID,
	aa.CUSTOMID,
	aa.CUSTOMNAME,
	aa.AGENTNAME,
	aa.ADDRESS,
	aa.WL_COMPANYID,
	aa.WL_NUMBER,
	aa.WL_COMPANYNAME,
	aa.RECVNAME,
	aa.RECVPHONE,
	aa.PROVINCENAME,
	aa.CITYNAME,
	aa.DISTRICTNAME,
	aa.STREETNAME,
	aa.ORIGINALREMARK,
	aa.IsFp,
	aa.IsPj,
	aa.IsHeTong,
	aa.IsQysy,
	aa.IsSyzz,
	aa.IsYjbb,
	aa.PlatformType,
	aa.logi_dstRoute,
	aa.logi_PayType,
	aa.logi_monAccNum,
	aa.logi_baojiaJine,
	aa.logi_dsJine,
	aa.logi_logcNum,
	aa.logi_ysJine,
	aa.logi_ysJineTotal,
	aa.logi_shouhuory,
	aa.logi_jinjianRiqi,
	aa.logi_shoufqianshu,
	aa.logi_shoufRiqi,
	aa.logi_sendSheng,
	aa.logi_sendShi,
	aa.logi_sendXian,
	aa.logi_sendAddress,
	aa.logi_sendMan,
	aa.logi_sendPhone,
	aa.logi_feiyongTotal,
	aa.logi_goodQty,
	aa.logi_goodName,
	aa.needBaojia,
	aa.logi_OrderId,
	aa.logi_CreateDate,
	aa.logi_ChanpinTypeStr,
	aa.PrintDatetime,
	aa.sysFirst,
	aa.total_amt,
	aa.RelBId,
	aa.fplx,
	aa.ServerTaskType,
	aa.PAIDCOSTS,
	aa.logi_ReceivePwd,
	aa.logi_SubOrderSn,
	aa.logi_PhonNum,
	aa.logi_TelNum,
	aa.RelOrderId,
	aa.JingdongWl
 FROM
	BllMod_Order aa
	LEFT JOIN info_Client bb ON aa.ClientId=bb.id
 WHERE
	aa.Bid={bid}
";
					System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
					if (dt.Rows.Count > 0)
					{

						System.Data.DataRow item = dt.Rows[0];
						//构建对象
						res = new BllMod.Order()
						{
							AddDate = YJT.DataBase.Common.ObjectTryToObj(item["AddDate"], DateTime.MinValue),
							ClientId = YJT.DataBase.Common.ObjectTryToObj(item["ClientId"], -1),
							ComputerName = YJT.DataBase.Common.ObjectTryToObj(item["ComputerName"], ""),
							ErpId = YJT.DataBase.Common.ObjectTryToObj(item["ErpId"], ""),
							ErrMsg = YJT.DataBase.Common.ObjectTryToObj(item["ErrMsg"], ""),
							Height = YJT.DataBase.Common.ObjectTryToObj(item["Height"], 1d),
							Bid = YJT.DataBase.Common.ObjectTryToObj(item["Bid"], -1),
							Ip = YJT.DataBase.Common.ObjectTryToObj(item["Ip"], ""),
							Length = YJT.DataBase.Common.ObjectTryToObj(item["Length"], 1d),
							Logic = YJT.DataBase.Common.ObjectTryToObj(item["Logic"], Settings.Setings.EnumLogicType.Default),
							Mac = YJT.DataBase.Common.ObjectTryToObj(item["Mac"], ""),
							NetDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["NetDanjbh"], ""),
							OrderId = YJT.DataBase.Common.ObjectTryToObj(item["OrderId"], -1),
							PrintStatus = YJT.DataBase.Common.ObjectTryToObj(item["PrintStatus"], Settings.Setings.EnumOrderPrintStatus.不可打印),
							Status = YJT.DataBase.Common.ObjectTryToObj(item["Status"], Settings.Setings.EnumOrderStatus.异常_类型转换失败),
							Weight = YJT.DataBase.Common.ObjectTryToObj(item["Weight"], 1d),
							Width = YJT.DataBase.Common.ObjectTryToObj(item["Width"], 1d),
							WmsDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["WmsDanjbh"], ""),
							ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ClientHandleDate"], DateTime.Parse("1900-01-01")),
							ServerHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ServerHandleDate"], DateTime.Parse("1900-01-01")),
							WmsYewy = YJT.DataBase.Common.ObjectTryToObj(item["WmsYewy"], ""),
							WmsDdwname = YJT.DataBase.Common.ObjectTryToObj(item["WmsDdwname"], ""),
							NETORDER_FROMID = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROMID"], ""),
							NETORDER_FROM = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROM"], ""),
							ERPORDER_ID = YJT.DataBase.Common.ObjectTryToObj(item["ERPORDER_ID"], ""),
							CUSTOMID = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMID"], ""),
							CUSTOMNAME = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMNAME"], ""),
							AGENTNAME = YJT.DataBase.Common.ObjectTryToObj(item["AGENTNAME"], ""),
							ADDRESS = YJT.DataBase.Common.ObjectTryToObj(item["ADDRESS"], ""),
							WL_COMPANYID = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYID"], ""),
							WL_NUMBER = YJT.DataBase.Common.ObjectTryToObj(item["WL_NUMBER"], ""),
							WL_COMPANYNAME = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYNAME"], ""),
							RECVNAME = YJT.DataBase.Common.ObjectTryToObj(item["RECVNAME"], ""),
							RECVPHONE = YJT.DataBase.Common.ObjectTryToObj(item["RECVPHONE"], ""),
							PROVINCENAME = YJT.DataBase.Common.ObjectTryToObj(item["PROVINCENAME"], ""),
							CITYNAME = YJT.DataBase.Common.ObjectTryToObj(item["CITYNAME"], ""),
							DISTRICTNAME = YJT.DataBase.Common.ObjectTryToObj(item["DISTRICTNAME"], ""),
							STREETNAME = YJT.DataBase.Common.ObjectTryToObj(item["STREETNAME"], ""),
							ORIGINALREMARK = YJT.DataBase.Common.ObjectTryToObj(item["ORIGINALREMARK"], ""),
							IsFp = YJT.DataBase.Common.ObjectTryToObj(item["IsFp"], ""),
							IsPj = YJT.DataBase.Common.ObjectTryToObj(item["IsPj"], ""),
							IsHeTong = YJT.DataBase.Common.ObjectTryToObj(item["IsHeTong"], ""),
							IsQysy = YJT.DataBase.Common.ObjectTryToObj(item["IsQysy"], ""),
							IsSyzz = YJT.DataBase.Common.ObjectTryToObj(item["IsSyzz"], ""),
							IsYjbb = YJT.DataBase.Common.ObjectTryToObj(item["IsYjbb"], ""),
							PlatformType = YJT.DataBase.Common.ObjectTryToObj(item["PlatformType"], Settings.Setings.EnumPlatformType.无),
							logi_dstRoute = YJT.DataBase.Common.ObjectTryToObj(item["logi_dstRoute"], ""),
							logi_PayType = YJT.DataBase.Common.ObjectTryToObj(item["logi_PayType"], ""),
							logi_monAccNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_monAccNum"], ""),
							logi_baojiaJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_baojiaJine"], ""),
							logi_dsJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_dsJine"], ""),
							logi_logcNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_logcNum"], ""),
							logi_ysJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJine"], ""),
							logi_ysJineTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJineTotal"], ""),
							logi_shouhuory = YJT.DataBase.Common.ObjectTryToObj(item["logi_shouhuory"], ""),
							logi_jinjianRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_jinjianRiqi"], ""),
							logi_shoufqianshu = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufqianshu"], ""),
							logi_shoufRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufRiqi"], ""),
							logi_sendSheng = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendSheng"], ""),
							logi_sendShi = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendShi"], ""),
							logi_sendXian = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendXian"], ""),
							logi_sendAddress = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendAddress"], ""),
							logi_sendMan = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendMan"], ""),
							logi_sendPhone = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendPhone"], ""),
							logi_feiyongTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_feiyongTotal"], ""),
							logi_goodQty = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodQty"], ""),
							logi_goodName = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodName"], ""),
							needBaojia = YJT.DataBase.Common.ObjectTryToObj(item["needBaojia"], 0d),
							logi_OrderId = YJT.DataBase.Common.ObjectTryToObj(item["logi_OrderId"], ""),
							logi_CreateDate = YJT.DataBase.Common.ObjectTryToObj(item["logi_CreateDate"], ""),
							logi_ChanpinTypeStr = YJT.DataBase.Common.ObjectTryToObj(item["logi_ChanpinTypeStr"], ""),
							PrintDatetime = YJT.DataBase.Common.ObjectTryToObj(item["PrintDatetime"], ""),
							sysFirst = YJT.DataBase.Common.ObjectTryToObj(item["sysFirst"], ""),
							total_amt = YJT.DataBase.Common.ObjectTryToObj(item["sysFirst"], "total_amt"),
							RelBId = YJT.DataBase.Common.ObjectTryToObj(item["RelBId"], -1),
							fplx = YJT.DataBase.Common.ObjectTryToObj(item["fplx"], ""),
							ServerTaskType = YJT.DataBase.Common.ObjectTryToObj(item["ServerTaskType"], Settings.Setings.EnumServerTaskType.异常_类型转换失败),
							PAIDCOSTS = YJT.DataBase.Common.ObjectTryToObj(item["PAIDCOSTS"], 0d),
							logi_ReceivePwd = YJT.DataBase.Common.ObjectTryToObj(item["logi_ReceivePwd"], ""),
							logi_SubOrderSn = YJT.DataBase.Common.ObjectTryToObj(item["logi_SubOrderSn"], 0),
							logi_PhonNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_PhonNum"], ""),
							logi_TelNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_TelNum"], ""),
							RelOrderId = YJT.DataBase.Common.ObjectTryToObj(item["RelOrderId"], -1L),
							JingdongWl = YJT.DataBase.Common.ObjectTryToObj(item["JingdongWl"], ""),
						};
						errCode = 0;
						errMsg = "";
						isOk = true;

					}
					else
					{
						errCode = -3;
						errMsg = "BID单据不存在";
						isOk = false;
					}
					try
					{
						dt.Dispose();
						dt = null;
					}
					catch { }
				}
				else
				{
					errCode = -1;
					errMsg = "ID为空";
					isOk = false;
				}
			}
			else
			{
				errCode = -2;
				errMsg = "数据库验证失败";
				isOk = false;
			}



			return res;
		}
		public BllMod.Order ServerPrintFinishOrder(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			bool isok2 = false;
			int errCode2 = -99;
			string errMsg2 = "";
			string sqlCmd = "";
			if (order != null)
			{
				MOD.BllMod.Order t = ServerGetOrder(order.Bid.ToString(), out isok2, out errCode2, out errMsg2);
				if (t != null)
				{
					if (t.Status != Settings.Setings.EnumOrderStatus.准备停止)
					{
						order.PrintDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
						sqlCmd = $@"
UPDATE
	BllMod_Order
SET
	ServerHandleDate=GETDATE(),
	ClientHandleDate=GETDATE()-1,
	ErrMsg='{order.ErrMsg}',
	Status='{order.Status.ToString()}',
	PrintStatus='{order.PrintStatus.ToString()}',
	PrintDatetime='{order.PrintDatetime}'
OUTPUT
	Inserted.ServerHandleDate
WHERE Bid={order.Bid}
";
						DateTime dbres = YJT.DataBase.Common.ObjectTryToObj(_dbhLocal.ExecuteScalar(sqlCmd, null, true), DateTime.MinValue);
						if (dbres != DateTime.MinValue)
						{
							order.ServerHandleDate = dbres;
							isOk = true;
							errCode = 0;
							errMsg = "完成";
						}
						else
						{
							isOk = false;
							errCode = -5;
							errMsg = "更新打印完成时候失败";
							AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, sqlCmd);
						}
					}
					else
					{
						errCode = -4;
						isOk = false;
						errMsg = "完成单据时,被要求删除";
						AddMsgOut("完成单据时,被要求删除", Settings.Setings.EnumMessageType.提示, errCode, errMsg2);
					}

				}
			}

			return order;
		}
		public void ServerGetWmsInfo2(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";
			if (order != null)
			{
				if (YJT.Text.Verification.IsNullOrEmpty(order.WmsDanjbh))
				{
					if (VerDb_Wms)
					{
						string sqlCmd = $@"select
	aa.outid as WmsDanjbh,aa.SALERNAME as WmsYewy,aa.RECCOMPANYNAME as WmsDdwname,nvl(aa.total_amt,0) as total_amt,case when bb.reccompanyid is null then '首次客户' else '' end as sysFirst
from
	wms_out_order_v aa
	left join wms_out_order_v bb on aa.reccompanyid=bb.reccompanyid and bb.GOODSOWNERID=2 and bb.credate<aa.credate and bb.credate>=date'2020-01-01'
where
	aa.srcexpno='{order.ErpId}' and aa.GOODSOWNERID=2
	and rownum<=1";
						System.Data.DataTable dt = _dbhWms.ExecuteToDataTable(sqlCmd, null, true);
						if (dt.Rows.Count > 0)
						{
							order.WmsDanjbh = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WmsDanjbh"], "");
							order.WmsYewy = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WmsYewy"], "");
							order.WmsDdwname = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WmsDdwname"], "");
							order.Status = Settings.Setings.EnumOrderStatus.已获取WMS信息;
							order.total_amt = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["total_amt"], "0");
							order.sysFirst = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["sysFirst"], "");
							errCode = 0;
							isOk = true;
							errMsg = "";
						}
						else
						{
							errCode = -3;
							isOk = false;
							errMsg = "WMS中未找到对应的ERP单据";
							AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, sqlCmd, YJT.Json.FunStrSerializeToStr(order, enumConverStr: true));
						}
						try
						{
							dt.Dispose();
							dt = null;
						}
						catch { }
					}
					else
					{
						errCode = -2;
						isOk = false;
						errMsg = "获取WMS数据时验证数据库错误";
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
					}
				}
				else
				{
					errCode = 1;
					errMsg = "此单据曾经获取过wms单据";
					isOk = true;
				}
			}
			else
			{
				errCode = -1;
				errMsg = "订单为NULL";
				isOk = false;
				AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
			}
		}
		/// <summary>
		/// 服务端获取WMS中的出入库ID
		/// </summary>
		/// <param name="order"></param>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public MOD.BllMod.Order ServerGetWmsInfo(MOD.BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			bool isok2 = false;
			int errCode2 = -99;
			string errMsg2 = "";
			if (order != null)
			{
				bool flag = false;
				if (YJT.Text.Verification.IsNullOrEmpty(order.WmsDanjbh))
				{
					if (VerDb(out isok2, out errCode2, out errMsg2))
					{
						string sqlCmd = $@"select
	aa.outid as WmsDanjbh,aa.SALERNAME as WmsYewy,aa.RECCOMPANYNAME as WmsDdwname,nvl(aa.total_amt,0) as total_amt,case when bb.reccompanyid is null then '首次客户' else '' end as sysFirst
from
	wms_out_order_v aa
	left join wms_out_order_v bb on aa.reccompanyid=bb.reccompanyid and bb.GOODSOWNERID=2 and bb.credate<aa.credate and bb.credate>=date'2020-01-01'
where
	aa.srcexpno='{order.ErpId}' and aa.GOODSOWNERID=2
	and rownum<=1";
						System.Data.DataTable dt = _dbhWms.ExecuteToDataTable(sqlCmd, null, true);
						if (dt.Rows.Count > 0)
						{
							order.WmsDanjbh = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WmsDanjbh"], "");
							order.WmsYewy = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WmsYewy"], "");
							order.WmsDdwname = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WmsDdwname"], "");
							order.total_amt = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["total_amt"], "0");
							order.ErrMsg = "获取WMS单据编号完成";
							order.sysFirst = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["sysFirst"], "");
							flag = true;
						}
						else
						{
							errCode = -3;
							isOk = false;
							errMsg = "WMS中未找到对应的ERP单据";
							order.WmsDanjbh = "";
							order.WmsYewy = "";
							order.WmsDdwname = "";
							order.total_amt = "0";
							order.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
							order.ErrMsg = "获取WMS单据编号完成";
							order.sysFirst = "";
							flag = true;
							AddMsgOut("WMS中未找到对应的ERP单据", Settings.Setings.EnumMessageType.异常, errCode, sqlCmd, YJT.Json.FunStrSerializeToStr(order, enumConverStr: true));
						}
						try
						{
							dt.Dispose();
							dt = null;
						}
						catch { }
					}
					else
					{
						errCode = -2;
						isOk = false;
						errMsg = "获取WMS数据时验证数据库错误," + errMsg2;
						AddMsgOut("获取WMS数据时验证数据库错误", Settings.Setings.EnumMessageType.异常, errCode, errMsg2);
					}
				}
				else
				{
					errCode = 1;
					errMsg = "此单据曾经获取过wms单据";
					order.Status = Settings.Setings.EnumOrderStatus.已获取WMS信息;
					flag = true;
					isOk = true;
					AddMsgOut("此单据曾经获取过wms单据", Settings.Setings.EnumMessageType.提示, errCode, errMsg2);
				}
				if (flag == true)
				{

					MOD.BllMod.Order t = ServerGetOrder(order.Bid.ToString(), out isok2, out errCode2, out errMsg2);
					if (t != null)
					{
						if (t.Status != Settings.Setings.EnumOrderStatus.准备停止)
						{
							string sqlCmd = $@"UPDATE BllMod_Order SET sysFirst='{order.sysFirst}',WmsDanjbh='{order.WmsDanjbh}',WmsYewy='{order.WmsYewy}',WmsDdwname='{order.WmsDdwname}',ServerHandleDate=GETDATE(),ClientHandleDate=GETDATE()-1,ErrMsg='{order.ErrMsg}',Status='{order.Status.ToString()}',total_amt='{order.total_amt}' OUTPUT Inserted.ServerHandleDate WHERE Bid={order.Bid}";
							DateTime dbres = YJT.DataBase.Common.ObjectTryToObj(_dbhLocal.ExecuteScalar(sqlCmd, null, true), DateTime.MinValue);
							if (dbres != DateTime.MinValue)
							{
								order.ServerHandleDate = dbres;
								isOk = true;
								errCode = 0;
								errMsg = "";
							}
							else
							{
								AddMsgOut("更新WMS单据时候失败", Settings.Setings.EnumMessageType.异常, errCode, sqlCmd);
							}
						}
						else
						{
							errCode = -4;
							isOk = false;
							errMsg = "单据在处理时,被要求删除";
							AddMsgOut("单据在处理时,被要求删除", Settings.Setings.EnumMessageType.提示, errCode, errMsg2);

						}
					}
				}


			}
			else
			{
				errCode = -1;
				errMsg = "订单为NULL";
				isOk = false;
				AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
			}

			return order;
		}
		public void ServerGetECInfo2(BllMod.Order order, bool isVerWmsInfo, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			if (order != null)
			{
				if (YJT.Text.Verification.IsNotNullOrEmpty(order.WmsDanjbh))
				{
					if (VerDb_Interface == true)
					{
						if (YJT.Text.Verification.IsNotNullOrEmpty(order.ErpId))
						{
							if (YJT.Text.Verification.IsLong(order.ErpId))
							{
								string sqlCmd = $@"
select
	NETORDER_ID, NETORDER_FROMID, NETORDER_FROM, ERPORDER_ID, CUSTOMID, CUSTOMNAME, AGENTNAME, ADDRESS, WL_COMPANYID, WL_NUMBER, WL_COMPANYNAME, RECVNAME, RECVPHONE,
	PROVINCENAME, CITYNAME, DISTRICTNAME, STREETNAME, ORIGINALREMARK,FPLX,nvl(PAIDCOSTS,0) as PAIDCOSTS
from
	NETORDER_DOC
where
	ERPORDER_ID={order.ErpId.ToString()}
";
								System.Data.DataTable dt = _dbhInterface.ExecuteToDataTable(sqlCmd, null, true);
								if (dt.Rows.Count > 0)
								{
									order.NetDanjbh = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["NETORDER_ID"], "");
									order.NETORDER_FROMID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["NETORDER_FROMID"], "");
									order.NETORDER_FROM = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["NETORDER_FROM"], "");
									order.ERPORDER_ID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ERPORDER_ID"], "");
									order.CUSTOMID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["CUSTOMID"], "");
									order.CUSTOMNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["CUSTOMNAME"], "");
									order.AGENTNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["AGENTNAME"], "");
									order.ADDRESS = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ADDRESS"], "");
									order.WL_COMPANYID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WL_COMPANYID"], "");
									order.WL_NUMBER = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WL_NUMBER"], "");
									order.WL_COMPANYNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WL_COMPANYNAME"], "");
									order.RECVNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["RECVNAME"], "");
									order.RECVPHONE = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["RECVPHONE"], "");
									order.PROVINCENAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["PROVINCENAME"], "");
									order.CITYNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["CITYNAME"], "");
									order.DISTRICTNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["DISTRICTNAME"], "");
									order.STREETNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["STREETNAME"], "");
									order.ORIGINALREMARK = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ORIGINALREMARK"], "");
									order.PAIDCOSTS = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["PAIDCOSTS"], 0d);
									order.PlatformType = YJT.DataBase.Common.ObjectTryToObj(order.NETORDER_FROM, Settings.Setings.EnumPlatformType.无);
									order.fplx = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["FPLX"], "");
									order.IsPj = Common.Logistics.GetPJ(order.ORIGINALREMARK, order.PlatformType);
									order.IsHeTong = Common.Logistics.GetHeTong(order.ORIGINALREMARK, order.PlatformType);
									order.IsQysy = Common.Logistics.GetQYSY(order.ORIGINALREMARK, order.PlatformType);
									order.IsSyzz = Common.Logistics.GetSYZZ(order.ORIGINALREMARK, order.PlatformType);
									order.IsYjbb = Common.Logistics.GetYJBB(order.ORIGINALREMARK, order.PlatformType);
									order.IsFp = Common.Logistics.GetFP(order.ORIGINALREMARK, order.fplx, order.PlatformType);
									if (isVerWmsInfo == true)
									{
										if (order.WmsDdwname == order.CUSTOMNAME && order.WmsYewy == order.AGENTNAME)
										{
											errCode = 0;
											isOk = true;
											errMsg = "";
										}
										else
										{
											errCode = -6;
											isOk = false;
											errMsg = "WMS信息与电商平台不一致";
										}
									}
									else
									{
										errCode = 0;
										isOk = true;
										errMsg = "";
									}

								}
								else
								{
									errCode = -3;
									isOk = false;
									order.Status = Setings.EnumOrderStatus.异常_电商信息不存在;
									errMsg = "电商接口中未找到对应的ERP单据";
									AddMsgOut("电商接口中未找到对应的ERP单据", Settings.Setings.EnumMessageType.异常, errCode, sqlCmd, YJT.Json.FunStrSerializeToStr(order, enumConverStr: true));
								}
								try
								{
									dt.Dispose();
									dt = null;
								}
								catch { }
							}
							else
							{
								errCode = -4;
								isOk = false;
								errMsg = "ERP编号并非数字型";
								order.Status = Settings.Setings.EnumOrderStatus.异常_ERPID不正确;
								order.ErrMsg = "ERP编号并非数字型";
								AddMsgOut("ERP编号并非数字型单据", Settings.Setings.EnumMessageType.异常, errCode, "", YJT.Json.FunStrSerializeToStr(order, enumConverStr: true));
							}
						}
						else
						{
							errCode = -5;
							isOk = false;
							errMsg = "ERP编号为空";
							order.Status = Settings.Setings.EnumOrderStatus.异常_ERPID不正确;
							order.ErrMsg = "ERP编号为空";
							AddMsgOut("ERP编号为空", Settings.Setings.EnumMessageType.异常, errCode, "", YJT.Json.FunStrSerializeToStr(order, enumConverStr: true));
						}



					}
					else
					{
						errCode = -2;
						isOk = false;
						errMsg = "获取电商数据时验证数据库错误," + "";
						AddMsgOut("获取电商数据时验证数据库错误", Settings.Setings.EnumMessageType.异常, errCode, "");
					}
				}
				else
				{
					errCode = 1;
					errMsg = "此单据未获得WMS单据编号";
					isOk = true;
					AddMsgOut("此单据未获得WMS单据编号", Settings.Setings.EnumMessageType.提示, errCode, YJT.Json.FunStrSerializeToStr(order));
				}
			}
			else
			{
				errCode = -1;
				errMsg = "获取电商信息时,订单为NULL";
				isOk = false;
				AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
			}
		}
		/// <summary>
		/// 服务端获取电商订单信息
		/// </summary>
		/// <param name="order"></param>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public MOD.BllMod.Order ServerGetECInfo(MOD.BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			bool isok2 = false;
			int errCode2 = -99;
			string errMsg2 = "";
			if (order != null)
			{
				bool flag = false;
				if (YJT.Text.Verification.IsNotNullOrEmpty(order.WmsDanjbh))
				{
					if (VerDb_Interface == true)
					{
						if (YJT.Text.Verification.IsNotNullOrEmpty(order.ErpId))
						{
							if (YJT.Text.Verification.IsLong(order.ErpId))
							{
								string sqlCmd = $@"
select
	NETORDER_ID, NETORDER_FROMID, NETORDER_FROM, ERPORDER_ID, CUSTOMID, CUSTOMNAME, AGENTNAME, ADDRESS, WL_COMPANYID, WL_NUMBER, WL_COMPANYNAME, RECVNAME, RECVPHONE,
	PROVINCENAME, CITYNAME, DISTRICTNAME, STREETNAME, ORIGINALREMARK,FPLX,nvl(PAIDCOSTS,0) as PAIDCOSTS
from
	NETORDER_DOC
where
	ERPORDER_ID={order.ErpId.ToString()}
";
								System.Data.DataTable dt = _dbhInterface.ExecuteToDataTable(sqlCmd, null, true);
								if (dt.Rows.Count > 0)
								{
									order.NetDanjbh = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["NETORDER_ID"], "");

									order.NETORDER_FROMID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["NETORDER_FROMID"], "");
									order.NETORDER_FROM = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["NETORDER_FROM"], "");
									order.ERPORDER_ID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ERPORDER_ID"], "");
									order.CUSTOMID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["CUSTOMID"], "");
									order.CUSTOMNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["CUSTOMNAME"], "");
									order.AGENTNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["AGENTNAME"], "");
									order.ADDRESS = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ADDRESS"], "");
									order.WL_COMPANYID = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WL_COMPANYID"], "");
									order.WL_NUMBER = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WL_NUMBER"], "");
									order.WL_COMPANYNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["WL_COMPANYNAME"], "");
									order.RECVNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["RECVNAME"], "");
									order.RECVPHONE = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["RECVPHONE"], "");
									order.PROVINCENAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["PROVINCENAME"], "");
									order.CITYNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["CITYNAME"], "");
									order.DISTRICTNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["DISTRICTNAME"], "");
									order.STREETNAME = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["STREETNAME"], "");
									order.ORIGINALREMARK = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["ORIGINALREMARK"], "");
									order.PAIDCOSTS = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["PAIDCOSTS"], 0d);
									order.PlatformType = YJT.DataBase.Common.ObjectTryToObj(order.NETORDER_FROM, Settings.Setings.EnumPlatformType.无);
									order.fplx = YJT.DataBase.Common.ObjectTryToObj(dt.Rows[0]["FPLX"], "");
									order.IsPj = Common.Logistics.GetPJ(order.ORIGINALREMARK, order.PlatformType);
									order.IsHeTong = Common.Logistics.GetHeTong(order.ORIGINALREMARK, order.PlatformType);
									order.IsQysy = Common.Logistics.GetQYSY(order.ORIGINALREMARK, order.PlatformType);
									order.IsSyzz = Common.Logistics.GetSYZZ(order.ORIGINALREMARK, order.PlatformType);
									order.IsYjbb = Common.Logistics.GetYJBB(order.ORIGINALREMARK, order.PlatformType);
									order.IsFp = Common.Logistics.GetFP(order.ORIGINALREMARK, order.fplx, order.PlatformType);

									if (YJT.Text.Verification.IsNullOrEmpty(order.ADDRESS)|| YJT.Text.Verification.IsNullOrEmpty(order.RECVNAME) || YJT.Text.Verification.IsNullOrEmpty(order.RECVPHONE))
									{
										sqlCmd = $@"SELECT  aa.companyid,aa.address,aa.telephone,aa.CONTRACTMAN,row_number() OVER(PARTITION BY aa.companyid ORDER BY LENGTH(NVL(aa.address,'')) DESC) pm FROM bms_tr_pos_def_v aa WHERE aa.COMPANYID='{order.CUSTOMID}'";
										System.Data.DataTable dt2 = _dbhHdErp.ExecuteToDataTable(sqlCmd, null, true);
										if (dt2 != null && dt2.Rows.Count > 0)
										{
											if (YJT.Text.Verification.IsNullOrEmpty(order.ADDRESS))
											{
												order.ADDRESS = YJT.DataBase.Common.ObjectTryToObj<string>(dt2.Rows[0]["address"], "");
											}
											if (YJT.Text.Verification.IsNullOrEmpty(order.RECVNAME))
											{
												order.RECVNAME = YJT.DataBase.Common.ObjectTryToObj<string>(dt2.Rows[0]["CONTRACTMAN"], "");
											}
											if (YJT.Text.Verification.IsNullOrEmpty(order.RECVPHONE))
											{
												order.RECVPHONE = YJT.DataBase.Common.ObjectTryToObj<string>(dt2.Rows[0]["telephone"], "");
											}
										}
										try
										{
											dt2.Dispose();
										}
										catch { }
										dt2 = null;
									}


									order.Status = Settings.Setings.EnumOrderStatus.已获取电商信息;
									order.ErrMsg = "获取WMS单据编号完成";
									flag = true;
								}
								else
								{
									errCode = -3;
									isOk = false;
									errMsg = "电商接口中未找到对应的ERP单据";
									order.Status = Settings.Setings.EnumOrderStatus.异常_电商信息不存在;
									order.ErrMsg = "电商接口中未找到对应的ERP单据";
									flag = true;
									AddMsgOut("电商接口中未找到对应的ERP单据", Settings.Setings.EnumMessageType.异常, errCode, sqlCmd, YJT.Json.FunStrSerializeToStr(order, enumConverStr: true));
								}
								try
								{
									dt.Dispose();
									dt = null;
								}
								catch { }
							}
							else
							{
								errCode = -4;
								isOk = false;
								errMsg = "ERP编号并非数字型";
								order.Status = Settings.Setings.EnumOrderStatus.异常_ERPID不正确;
								order.ErrMsg = "ERP编号并非数字型";
								flag = true;
								AddMsgOut("ERP编号并非数字型单据", Settings.Setings.EnumMessageType.异常, errCode, "", YJT.Json.FunStrSerializeToStr(order, enumConverStr: true));
							}
						}
						else
						{
							errCode = -4;
							isOk = false;
							errMsg = "ERP编号为空";
							order.Status = Settings.Setings.EnumOrderStatus.异常_ERPID不正确;
							order.ErrMsg = "ERP编号为空";
							flag = true;
							AddMsgOut("ERP编号为空", Settings.Setings.EnumMessageType.异常, errCode, "", YJT.Json.FunStrSerializeToStr(order, enumConverStr: true));
						}



					}
					else
					{
						errCode = -2;
						isOk = false;
						errMsg = "获取电商数据时验证数据库错误," + errMsg2;
						AddMsgOut("获取电商数据时验证数据库错误", Settings.Setings.EnumMessageType.异常, errCode, errMsg2);
					}
				}
				else
				{
					errCode = 1;
					errMsg = "此单据未获得WMS单据编号";
					isOk = true;
					AddMsgOut("此单据未获得WMS单据编号", Settings.Setings.EnumMessageType.提示, errCode, YJT.Json.FunStrSerializeToStr(order));
				}
				if (flag == true)
				{
					MOD.BllMod.Order t = ServerGetOrder(order.Bid.ToString(), out isok2, out errCode2, out errMsg2);
					if (t != null)
					{
						if (t.Status != Settings.Setings.EnumOrderStatus.准备停止)
						{
							string sqlCmd = $@"
UPDATE
	BllMod_Order
SET
	WmsDanjbh='{order.WmsDanjbh}',
	WmsYewy='{order.WmsYewy}',
	WmsDdwname='{order.WmsDdwname}',
	ServerHandleDate=GETDATE(),
	ClientHandleDate=GETDATE()-1,
	ErrMsg='{order.ErrMsg}',
	Status='{order.Status.ToString()}',
	NETORDER_FROMID='{order.NETORDER_FROMID}',
	NETORDER_FROM='{order.NETORDER_FROM}',
	ERPORDER_ID='{order.ERPORDER_ID}',
	netDanjbh='{order.NetDanjbh}',
	CUSTOMID='{order.CUSTOMID}',
	CUSTOMNAME='{order.CUSTOMNAME}',
	AGENTNAME='{order.AGENTNAME}',
	ADDRESS='{order.ADDRESS}',
	WL_COMPANYID='{order.WL_COMPANYID}',
	WL_NUMBER='{order.WL_NUMBER}',
	WL_COMPANYNAME='{order.WL_COMPANYNAME}',
	RECVNAME='{order.RECVNAME}',
	RECVPHONE='{order.RECVPHONE}',
	PROVINCENAME='{order.PROVINCENAME}',
	CITYNAME='{order.CITYNAME}',
	DISTRICTNAME='{order.DISTRICTNAME}',
	STREETNAME='{order.STREETNAME}',
	ORIGINALREMARK='{order.ORIGINALREMARK}',
	PlatformType='{order.PlatformType.ToString()}',
	IsFp='{order.IsFp}',
	IsPj='{order.IsPj}',
	IsHeTong='{order.IsHeTong}',
	IsQysy='{order.IsQysy}',
	IsSyzz='{order.IsSyzz}',
	IsYjbb='{order.IsYjbb}',
	fplx='{order.fplx}',
	PAIDCOSTS={order.PAIDCOSTS.ToString()}
OUTPUT
	Inserted.ServerHandleDate
WHERE Bid={order.Bid}
";
							DateTime dbres = YJT.DataBase.Common.ObjectTryToObj(_dbhLocal.ExecuteScalar(sqlCmd, null, true), DateTime.MinValue);
							if (dbres != DateTime.MinValue)
							{
								order.ServerHandleDate = dbres;
								isOk = true;
								errCode = 0;
								errMsg = "";
							}
							else
							{
								AddMsgOut("更新电商数据单据时候失败", Settings.Setings.EnumMessageType.异常, errCode, sqlCmd);
							}
						}
						else
						{
							errCode = -4;
							isOk = false;
							errMsg = "单据在处理时,被要求删除";
							AddMsgOut("单据在处理时,被要求删除", Settings.Setings.EnumMessageType.提示, errCode, errMsg2);
						}
					}
				}


			}
			else
			{
				errCode = -1;
				errMsg = "获取电商信息时,订单为NULL";
				isOk = false;
				AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
			}

			return order;
		}
		System.Text.RegularExpressions.Regex _regPhoneFor = new System.Text.RegularExpressions.Regex(@"(1([358][0-9]|4[579]|6[5678]|7[0135678]|9[189])[0-9]{8})");
		System.Text.RegularExpressions.Regex _regTelFor = new System.Text.RegularExpressions.Regex(@"((\+86)*([\(]*(0\d{2}[\)]*-\d{8}(-\d{1,4})?)|[\(]*(0\d{3}[\)]*-*\d{7,8}(-\d{1,4})?)))");
		/// <summary>
		/// 创建物流订单,如果为指定物流,则按照规则生成物流
		/// </summary>
		/// <param name="orderThis"></param>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public BllMod.Order ServerCreateLogic(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			bool isok2 = false;
			int errCode2 = -99;
			string errMsg2 = "";
			if (order != null)
			{
				bool flag = false;
				if (order.Status == Settings.Setings.EnumOrderStatus.已获取电商信息)
				{
					if (YJT.Text.Verification.IsNotNullOrEmpty(order.ADDRESS))
					{
						//检查物流信息是否正确
						bool logicStatus = false;
						YJT.BaiduService.BaiduMap.GetAddressDetalRes info = PubGetAddressInfo(order.ADDRESS, out isok2, out errCode2, out errMsg2);
						if (isok2 != true)
						{
							AddMsgOut("通过百度API没有正确获得地址信息", Settings.Setings.EnumMessageType.提示, errCode2, errMsg2, order.ADDRESS);
						}
						if (YJT.Text.Verification.IsNullOrEmpty(order.RECVNAME))
						{
							order.ErrMsg = "订单没有收货人";
							order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
							flag = true;
						}
						else
						{

							if (YJT.Text.Verification.IsNullOrEmpty(order.RECVPHONE))
							{
								order.ErrMsg = "订单没有收货人电话";
								order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
								flag = true;
							}
							else
							{
								string input = order.RECVPHONE.Trim();
								string phoneNum = "";
								string telNum = "";
								int fillNum = 0;
								System.Text.RegularExpressions.MatchCollection phones = _regPhoneFor.Matches(input);
								if (phones.Count > 0)
								{
									for (int i = 0; i < ((phones.Count > 2) ? 2 : phones.Count); i++)
									{
										if (fillNum > 1)
										{
											break;
										}
										else if (fillNum == 0)
										{
											phoneNum = phones[i].Groups[1].Value;
										}
										else
										{
											telNum = phones[i].Groups[1].Value;
										}
										fillNum++;

									}
								}
								if (fillNum < 2)
								{
									System.Text.RegularExpressions.MatchCollection tels = _regTelFor.Matches(input);
									if (tels.Count > 0)
									{
										for (int i = 0; i < ((tels.Count > 2) ? 2 : tels.Count); i++)
										{
											if (fillNum > 1)
											{
												break;
											}
											else if (fillNum == 0)
											{
												phoneNum = tels[i].Groups[1].Value;
											}
											else
											{
												telNum = tels[i].Groups[1].Value;
											}
											fillNum++;

										}
									}
								}
								if (YJT.Text.Verification.IsNullOrEmpty(telNum))
								{
									telNum = phoneNum;
								}
								if (YJT.Text.Verification.IsNullOrEmpty(phoneNum))
								{
									order.ErrMsg = "没有分析到正确的电话号码";
									order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
									flag = true;
								}
								else
								{
									order.logi_PhonNum = phoneNum;
									order.logi_TelNum = telNum;
									if (isok2 == true)
									{
										if (YJT.Text.Verification.IsNullOrEmpty(order.PROVINCENAME))
										{
											order.PROVINCENAME = info.result.addressComponent.province;
										}
										if (YJT.Text.Verification.IsNullOrEmpty(order.CITYNAME))
										{
											order.CITYNAME = info.result.addressComponent.city;
										}
										if (YJT.Text.Verification.IsNullOrEmpty(order.DISTRICTNAME))
										{
											order.DISTRICTNAME = info.result.addressComponent.district;
										}
										if (YJT.Text.Verification.IsNullOrEmpty(order.STREETNAME))
										{
											order.STREETNAME = info.result.addressComponent.street;
										}
									}
									else
									{
										if (YJT.Text.Verification.IsNullOrEmpty(order.PROVINCENAME))
										{
											order.PROVINCENAME = "";
										}
										if (YJT.Text.Verification.IsNullOrEmpty(order.CITYNAME))
										{
											order.CITYNAME = "";
										}
										if (YJT.Text.Verification.IsNullOrEmpty(order.DISTRICTNAME))
										{
											order.DISTRICTNAME = "";
										}
										if (YJT.Text.Verification.IsNullOrEmpty(order.STREETNAME))
										{
											order.STREETNAME = "";
										}
									}
									
									flag = true;
									logicStatus = true;
								}

							}
						}
						flag = true;
						if (logicStatus == true)
						{
							if (order.Logic == Settings.Setings.EnumLogicType.Default)
							{
								//开始计算
								if (order.Weight <= 3.0)
								{
									order.Logic = Settings.Setings.EnumLogicType.申通快递;
								}
								else
								{
									//if (order.PlatformType == Setings.EnumPlatformType.小药药 || order.PlatformType == Setings.EnumPlatformType.药京采)
									if (YJT.Text.Verification.IsLikeIn(order.PROVINCENAME, new List<string>() { "北京", "天津", "河北" }, true))
									{
										order.Logic = Settings.Setings.EnumLogicType.邮政EMS;
									}
									else
									{
										order.Logic = Setings.EnumLogicType.顺丰;
									}
								}



							}
							///设置发货人
							order.logi_sendAddress = @"河北省保定市莲池区北三环801号";
							order.logi_sendSheng = @"河北省";
							order.logi_sendShi = @"保定市";
							order.logi_sendXian = @"莲池区";
							if (order.PlatformType == Setings.EnumPlatformType.药师帮)
							{
								order.logi_sendMan = "刘聪聪";
								order.logi_sendPhone = "13931252550";
							}
							else if (order.PlatformType == Setings.EnumPlatformType.小药药)
							{
								order.logi_sendMan = "杨雪丽";
								order.logi_sendPhone = "13930270307";
							}
							else if (order.PlatformType == Setings.EnumPlatformType.药京采)
							{
								order.logi_sendMan = "赵楠";
								order.logi_sendPhone = "15097755954";
							}
							
							else
							{
								order.Logic = Setings.EnumLogicType.Default;
							}
							switch (order.Logic)
							{
								case Setings.EnumLogicType.京东物流:
									string 备注2 = "";
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
									{
										备注2 = 备注2 + "发票";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
									{
										备注2 = 备注2 + " 货品批件";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
									{
										备注2 = 备注2 + " 企业首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
									{
										备注2 = 备注2 + " 货品首营";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
									{
										备注2 = 备注2 + " 药检报告";
									}
									if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
									{
										备注2 = 备注2 + " 购销合同";
									}
									YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrder createClass = new YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrder("0030001", order.ErpId, order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.RECVNAME, order.ADDRESS, order.logi_TelNum, 1, order.Weight, 1)
									{
										province = order.PROVINCENAME,
										city = order.CITYNAME,
										county = order.DISTRICTNAME,
										town = order.STREETNAME,
										description = "药品",
										senderMobile = order.logi_sendPhone,
										senderTel = order.logi_sendPhone,

										receiveMobile = order.logi_PhonNum,
										receiveTel = order.logi_TelNum,

										remark = 备注2
									};
									string stxt2 = "";
									string otxt2 = "";
									YJT.Logistics.JingDongChunPeiLogistics.ClassCreateOrderRes res2 = _jdWl.CreateOrder(createClass, out isOk, out errCode, out errMsg, out stxt2, out otxt2);
									try
									{
										if (!System.IO.Directory.Exists(@"D:\WLLOG"))
										{
											System.IO.Directory.CreateDirectory(@"D:\WLLOG");
										}
										System.IO.File.AppendAllText(@"D:\WLLOG\Jingdong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\Jingdong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt2 + "\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\Jingdong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
										System.IO.File.AppendAllText(@"D:\WLLOG\Jingdong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt2 + "\r\n");

									}
									catch (Exception ee)
									{
										AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
									}
									if (isOk == true && YJT.Text.Verification.IsNotNullOrEmpty(res2.deliveryId))
									{
										order.WL_COMPANYID = ((int)order.Logic).ToString();
										order.WL_COMPANYNAME = order.Logic.ToString();
										order.WL_NUMBER = res2.deliveryId;
										order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
										order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
										order.ErrMsg = "创建物流订单完成";
										order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

										order.logi_baojiaJine = "0.00";
										order.logi_dsJine = "0.00";
										order.logi_feiyongTotal = 0.ToString("#0.00");
										order.logi_goodName = createClass.description;
										order.logi_goodQty = createClass.packageCount.ToString();
										order.logi_logcNum = "";
										order.logi_monAccNum = "";
										order.logi_sendAddress = order.logi_sendAddress;
										order.logi_sendMan = order.logi_sendMan;
										order.logi_sendSheng = order.logi_sendSheng;
										order.logi_sendShi = order.logi_sendShi;
										order.logi_sendXian = order.logi_sendXian;
										order.logi_shoufqianshu = "";
										order.logi_shoufRiqi = "";
										order.logi_shouhuory = "";
										order.logi_ChanpinTypeStr = "纯配";
										order.logi_PayType = "月结";//坑
										order.logi_OrderId = res2.deliveryId;

										order.logi_dstRoute = "";
										if (res2.preSortResult != null)
										{

											order.logi_dstRoute = res2.preSortResult.sourceSortCenterId.Value.ToString() + "-" + res2.preSortResult.sourceSortCenterName + "-|-" + res2.preSortResult.targetSortCenterId.Value.ToString() + "-" + res2.preSortResult.targetSortCenterName;
											order.JingdongWl = res2.preSortResult.aging.Value.ToString() + "@<|||>@\n" +
												res2.preSortResult.agingName.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.collectionAddress.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.coverCode.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.distributeCode.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.isHideContractNumbers.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.isHideName.ToString().Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.qrcodeUrl.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.road.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.siteId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.siteName.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.siteType.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.slideNo.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.sourceCrossCode.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.sourceSortCenterId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.sourceSortCenterName.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.sourceTabletrolleyCode.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.targetSortCenterId.Value.ToString().Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.targetSortCenterName.Replace("'", "") + "@<|||>@\n" +
												res2.preSortResult.targetTabletrolleyCode.Replace("'", "") + "@<|||>@\n";
										}
										double total = 0;
										double baojia = 0;
										double yunfei = 0;
										if (!double.TryParse(order.logi_feiyongTotal, out total))
										{
											total = 0;
										}
										if (!double.TryParse(order.logi_baojiaJine, out baojia))
										{
											baojia = 0;
										}
										yunfei = total;
										if (yunfei <= 0)
										{
											order.logi_ysJine = yunfei.ToString("#0.000");
											order.logi_ysJineTotal = order.logi_ysJine;
										}
										else
										{
											order.logi_ysJine = "";
											order.logi_ysJineTotal = "";
										}

									}
									else
									{
										order.ErrMsg = "京东物流下单不成功:" + errMsg;
										order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
										flag = true;
									}
									break;
								case Settings.Setings.EnumLogicType.顺丰:
									{
										YJT.Logistics.ShunFengLogistic.ClassCreateOrder.CargoDetailsClass 货品 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.CargoDetailsClass()
										{
											count = 1,
											name = "药品",
											specifications = "药品",
											weight = order.Weight
										};
										YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass 发件方 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass()
										{
											address = order.logi_sendAddress,
											city = order.logi_sendShi,
											company = "河北燕都医药物流有限公司",
											contact = order.logi_sendMan,
											contactType = 1,
											county = order.logi_sendXian,
											mobile = order.logi_sendPhone,
											province = order.logi_sendSheng,
											tel = order.logi_sendPhone

										};


										YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass 收件方 = new YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ContactinfoClass()
										{
											address = order.ADDRESS,
											city = order.CITYNAME,
											company = order.CUSTOMNAME,
											contact = order.RECVNAME,
											contactType = 2,
											county = order.DISTRICTNAME,
											mobile = order.logi_PhonNum,
											province = order.PROVINCENAME,
											tel = order.logi_TelNum

										};
										string 备注 = "";
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
										{
											备注 = 备注 + "发票";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
										{
											备注 = 备注 + " 货品批件";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
										{
											备注 = 备注 + " 企业首营";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
										{
											备注 = 备注 + " 货品首营";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
										{
											备注 = 备注 + " 药检报告";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
										{
											备注 = 备注 + " 购销合同";
										}
										YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass 增值服务 = YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.CtorService(YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.enum增值服务类别.配送服务, null);
										//增值服务 = null;
										YJT.Logistics.ShunFengLogistic.ClassCreateOrder a = _sf.Ctor_CreateOrderObj(order.ERPORDER_ID, 发件方, 收件方, 增值服务, 货品, 备注, "13", YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型.顺丰标快, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式.寄付月结);
										double tTotal_amt = 0d;
										if (!double.TryParse(order.total_amt, out tTotal_amt))
										{
											tTotal_amt = 0d;
										}
										if (tTotal_amt != 0d)
										{
											if (tTotal_amt > 0)
											{
												//int b = (int)(tTotal_amt / 500);
												//double c = tTotal_amt % 500;
												//if (c > 0)
												//{
												//	b = b + 1;
												//}
												//order.needBaojia = b * 500;
												//order.total_amt = tTotal_amt.ToString("#0.00");
												if (tTotal_amt < 500)
												{
													order.needBaojia = 500;
												}
												else
												{
													order.needBaojia = 1000;
												}
												order.total_amt = tTotal_amt.ToString("#0.00");
											}
										}
										if (order.needBaojia > 0)
										{
											YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass 保价服务 = YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.CtorService(YJT.Logistics.ShunFengLogistic.ClassCreateOrder.ServiceListClass.enum增值服务类别.保价, order.needBaojia.ToString());
											a.AddserviceList(保价服务);
										}
										else
										{
											order.needBaojia = 0;
										}
										string stxt = "";
										string otxt = "";
										YJT.Logistics.ShunFengLogistic.ClassCreateOrderRes res = _sf.CreateOrder(a, out isOk, out errCode, out errMsg, out stxt, out otxt);
										try
										{
											if (!System.IO.Directory.Exists(@"D:\WLLOG"))
											{
												System.IO.Directory.CreateDirectory(@"D:\WLLOG");
											}
											System.IO.File.AppendAllText(@"D:\WLLOG\Shunfeng_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\Shunfeng_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\Shunfeng_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
											System.IO.File.AppendAllText(@"D:\WLLOG\Shunfeng_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

										}
										catch (Exception ee)
										{
											AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
										}

										if (isOk == true && YJT.Text.Verification.IsNotNullOrEmpty(res.data))
										{
											order.WL_COMPANYID = ((int)order.Logic).ToString();
											order.WL_COMPANYNAME = order.Logic.ToString();
											order.WL_NUMBER = res.data;
											order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
											order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
											order.ErrMsg = "创建物流订单完成";
											order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
											//YJT.Logistics.ShunFengLogistic.ClassGetOrderInfoRes res2 = sf.GetOrderInfo(order.ERPORDER_ID, out isOk, out errCode, out errMsg);
											//if (isOk == true && res2.data2 != null)
											//{
											//	//res2.data2.destRouteLabel //371DN-072
											//	//monthAccount
											//}
										}
										else
										{
											order.ErrMsg = "顺丰物流下单不成功:" + res.data;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											flag = true;
										}
									}
									break;
								case Setings.EnumLogicType.邮政EMS:
									{
										string 备注 = "";
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
										{
											备注 = 备注 + "发票";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
										{
											备注 = 备注 + " 货品批件";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
										{
											备注 = 备注 + " 企业首营";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
										{
											备注 = 备注 + " 货品首营";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
										{
											备注 = 备注 + " 药检报告";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
										{
											备注 = 备注 + " 购销合同";
										}
										//if (order.total_amt != "0")
										//{
										//	double tTotalamt = 0;
										//	if (!double.TryParse(order.total_amt, out tTotalamt))
										//	{
										//		tTotalamt = 0;
										//	}
										//	if (tTotalamt > 0)
										//	{
										//		int b = (int)(tTotalamt / 500);
										//		double c = tTotalamt % 500;
										//		if (c > 0)
										//		{
										//			b = b + 1;
										//		}
										//		order.needBaojia = b * 500;
										//		order.total_amt = tTotalamt.ToString();
										//	}
										//}
										order.needBaojia = 0d;
										YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.Cargo 药品info = _emsYz.GetSubModCargo(
											cargo_category: "",
											cargo_name: "药品",
											cargo_quantity: 0,
											cargo_value: 1,
											cargo_length: 0, cargo_width: 0, cargo_high: 0,
											cargo_weight: 0,
											cargo_order_no: "");
										YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.ClassAddress 发件方 = _emsYz.GetSubModAddress
											(
											 address: order.logi_sendAddress,
											 prov: order.logi_sendSheng,
											 city: order.logi_sendShi,
											 county: order.logi_sendXian,
											 name: order.logi_sendMan,
											 mobile: order.logi_sendPhone,
											 phone: order.logi_sendPhone,
											 post_code: "");
										YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.ClassAddress 收件方 = _emsYz.GetSubModAddress(
											address: order.ADDRESS,
											 prov: order.PROVINCENAME,
											 city: order.CITYNAME,
											 county: order.DISTRICTNAME,
											 name: order.RECVNAME,
											 mobile: order.logi_PhonNum,
											 phone: order.logi_TelNum,
											 post_code: ""
											);
										order.logi_ReceivePwd = YJT.Text.ClassCreateText.FunStrCreateNumberStr(6, null);
										YJT.Logistics.YouZhengEms.ClassYouZhengEmsMod 取号 = _emsYz.GetModGetEmsOrder(
											ecommerce_user_id: order.PlatformType.ToString(),
											erpid: order.ERPORDER_ID,
											length: order.Length, width: order.Width, height: order.Height, weight: order.Weight,
											sender: 发件方,
											receiver: 收件方,
											cargos: new YJT.Logistics.YouZhengEms.ClassInterface下单取号.ClassMain.Cargo[] { 药品info },
											batch_no: "",
											one_bill_fee_type: 0,
											contents_attribute: 3,
											base_product_no: "1",
											biz_product_no: "1",
											cod_amount: 0,
											cod_flag: 9,
											deliver_type: 2,
											electronic_preferential_amount: 0,
											electronic_preferential_no: "",
											insurance_amount: 0,//order.needBaojia,
											insurance_flag: 2,
											insurance_premium_amount: 0,
											note: "",
											payment_mode: 1,
											pickup_notes: 备注,
											pickup_type: 1,
											postage_total: 0,
											receipt_flag: 1,
											receiver_safety_code: order.logi_ReceivePwd,
											sender_safety_code: "",
											valuable_flag: 0,
											waybill_no: "",
											submail_no: ""
										);
										string stxt = "";
										string otxt = "";
										YJT.Logistics.YouZhengEms.ClassResBase resobj = null;
										int tryCountEms = 5;
										while (tryCountEms > 0)
										{
											tryCountEms--;
											resobj = _emsYz.SendGetEmsOrder(取号, out isOk, out errCode, out errMsg, out stxt, out otxt);
											try
											{
												if (!System.IO.Directory.Exists(@"D:\WLLOG"))
												{
													System.IO.Directory.CreateDirectory(@"D:\WLLOG");
												}
												System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZ_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZ_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZ_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\EmsYZ_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");
											}
											catch (Exception ee)
											{
												AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
											}
											if (isOk == true)
											{
												break;
											}
											else
											{
												System.Threading.Thread.Sleep(1000);
											}
										}
										
										if (resobj != null)
										{
											if (isOk == true)
											{
												YJT.Logistics.YouZhengEms.Classoms_ordercreate_waybillnoRes 下单结果 = resobj as YJT.Logistics.YouZhengEms.Classoms_ordercreate_waybillnoRes;
												if (下单结果 != null)
												{
													order.WL_COMPANYID = ((int)order.Logic).ToString();
													order.WL_COMPANYNAME = order.Logic.ToString();
													order.WL_NUMBER = 下单结果.body.waybill_no;
													order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
													order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
													order.ErrMsg = "创建物流订单完成";
													order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
													order.logi_dstRoute = 下单结果.body.routeCode;
													order.logi_PayType = "寄件人";//payment_mode
													order.logi_monAccNum = "";
													order.logi_baojiaJine = order.needBaojia.ToString("#0.00");
													order.logi_dsJine = "";
													order.logi_logcNum = "";
													order.logi_ysJine = "0.00";
													order.logi_ysJineTotal = "0.00";
													order.logi_shouhuory = "";
													order.logi_shoufqianshu = "";
													order.logi_shoufRiqi = "";
													order.logi_sendSheng = 发件方.prov;
													order.logi_sendShi = 发件方.city;
													order.logi_sendXian = 发件方.county;
													order.logi_sendAddress = 发件方.address;
													order.logi_sendMan = 发件方.name;
													order.logi_sendPhone = 发件方.mobile;
													order.logi_feiyongTotal = "0";
													order.logi_goodQty = "0";
													order.logi_goodName = "药品";
													order.logi_ChanpinTypeStr = "特快专递";//biz_product_no
												}
												else
												{
													//返回的内容不是下单取号的结果类
													order.ErrMsg = "邮政物流下单不成功:返回的对象不是Classoms_ordercreate_waybillnoRes";
													order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
													flag = true;
												}
											}
											else
											{
												//下单不成功
												order.ErrMsg = "邮政物流下单不成功:" + errMsg;
												order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
												flag = true;
											}
										}
										else
										{
											order.ErrMsg = "邮政物流下单不成功:提交过来的对象为NULL";
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											flag = true;
										}
									}
									break;
								case Setings.EnumLogicType.中通快递:
									{
										bool isErrorFlag = false;
										string 备注 = "";
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
										{
											备注 = 备注 + "发票";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
										{
											备注 = 备注 + " 货品批件";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
										{
											备注 = 备注 + " 企业首营";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
										{
											备注 = 备注 + " 货品首营";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
										{
											备注 = 备注 + " 药检报告";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
										{
											备注 = 备注 + " 购销合同";
										}
										YJT.Logistics.ZhongTongLogistics.ClassCreateOrder cco = YJT.Logistics.ZhongTongLogistics.ClassCreateOrder.Intor(order.ERPORDER_ID, "2", "1", _ztkdWl, out isOk, out errMsg, 备注);
										if (isOk == false)
										{
											order.ErrMsg = "中通物流下单不成功:" + errMsg;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											flag = true;
											isErrorFlag = true;
										}
										if (!cco.CreateAccountInfo(out errMsg, 1))
										{
											order.ErrMsg = "账户信息创建错误:" + errMsg;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											flag = true;
											isErrorFlag = true;
										}
										if (!cco.CreateSenderInfo(order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.logi_sendSheng, order.logi_sendShi, order.logi_sendXian, out errMsg))
										{
											order.ErrMsg = "账户发件信息错误:" + errMsg;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											flag = true;
											isErrorFlag = true;
										}
										if (!cco.CreateReceiveInfo(order.RECVNAME, order.ADDRESS, order.logi_PhonNum, order.PROVINCENAME, order.CITYNAME, order.DISTRICTNAME, out errMsg, order.logi_TelNum))
										{
											order.ErrMsg = "账户收件信息错误:" + errMsg;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											flag = true;
											isErrorFlag = true;
										}
										cco.CreateSummaryInfo(quantity: 1);
										if (!cco.CreateOrderItems(errMsg: out errMsg, name: "药品", weight: (long)order.Weight, quantity: 1))
										{
											order.ErrMsg = "药品信息错误:" + errMsg;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											flag = true;
											isErrorFlag = true;
										}
										double tTotal_amt = 0d;
										if (!double.TryParse(order.total_amt, out tTotal_amt))
										{
											tTotal_amt = 0d;
										}
										order.needBaojia = 0;
										order.total_amt = tTotal_amt.ToString("#0.00");
										string stxt = "";
										string otxt = "";
										if (isErrorFlag == false)
										{
											YJT.Logistics.ZhongTongLogistics.ClassCreateOrderRes r1 = _ztkdWl.CreateOrder(cco, out isOk, out errMsg, out errCode, out stxt, out otxt);
											try
											{
												if (!System.IO.Directory.Exists(@"D:\WLLOG"))
												{
													System.IO.Directory.CreateDirectory(@"D:\WLLOG");
												}
												System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\ZhongTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

											}
											catch (Exception ee)
											{
												AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
											}
											if (r1 != null && isOk == true)
											{
												order.WL_COMPANYID = ((int)order.Logic).ToString();
												order.WL_COMPANYNAME = order.Logic.ToString();
												order.WL_NUMBER = r1.result.billCode;
												order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
												order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
												order.ErrMsg = "创建物流订单完成";
												order.logi_baojiaJine = "0.00";
												order.logi_dsJine = "0.00";
												order.logi_feiyongTotal = "0.00";
												order.logi_goodName = "药品";
												order.logi_goodQty = "1";
												order.logi_logcNum = "";
												order.logi_monAccNum = "";
												order.logi_shoufqianshu = "";
												order.logi_shoufRiqi = "";
												order.logi_shouhuory = "";
												order.logi_ChanpinTypeStr = "非集团客户-全网件";
												order.logi_PayType = "";
												order.logi_OrderId = r1.result.orderCode;
												order.JingdongWl = r1.result.siteName + "转" + r1.result.bigMarkInfo.bagAddr;
												order.logi_dstRoute = r1.result.bigMarkInfo.mark;
												order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
												double total = 0;
												double baojia = 0;
												double yunfei = 0;
												if (!double.TryParse(order.logi_feiyongTotal, out total))
												{
													total = 0;
												}
												if (!double.TryParse(order.logi_baojiaJine, out baojia))
												{
													baojia = 0;
												}
												yunfei = total;
												if (yunfei <= 0)
												{
													order.logi_ysJine = yunfei.ToString("#0.000");
													order.logi_ysJineTotal = order.logi_ysJine;
												}
												else
												{
													order.logi_ysJine = "";
													order.logi_ysJineTotal = "";
												}
												isOk = true;
												errCode = 0;
												errMsg = "";
											}
											else
											{
												order.ErrMsg = "中通物流下单不成功:" + errMsg;
												order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
												flag = true;
											}
										}
										break;
									}
								case Setings.EnumLogicType.申通快递:
									{
										string 备注 = "";
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsFp))
										{
											备注 = 备注 + "发票";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsPj))
										{
											备注 = 备注 + " 货品批件";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsQysy))
										{
											备注 = 备注 + " 企业首营";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsSyzz))
										{
											备注 = 备注 + " 货品首营";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsYjbb))
										{
											备注 = 备注 + " 药检报告";
										}
										if (YJT.Text.Verification.IsNotNullOrEmpty(order.IsHeTong))
										{
											备注 = 备注 + " 购销合同";
										}
										double tTotal_amt = 0d;
										if (!double.TryParse(order.total_amt, out tTotal_amt))
										{
											tTotal_amt = 0d;
										}
										order.needBaojia = 0;
										order.total_amt = tTotal_amt.ToString("#0.00");
										order.logi_sendMan = "赵志强-" + order.logi_sendMan;
										string stxt = "";
										string otxt = "";
										YJT.Logistics.ShenTongLogistic.ClassCreateOrder cco = _shenTongWl.IntorCreateOrder(
											order.ERPORDER_ID, YJT.Logistics.ShenTongLogistic.ClassCreateOrder.EnumBillType.普通00,
											order.logi_sendMan, order.logi_sendAddress, order.logi_sendPhone, order.logi_sendSheng, order.logi_sendShi, order.logi_sendXian,
											order.RECVNAME, order.ADDRESS, order.logi_PhonNum, order.PROVINCENAME, order.CITYNAME, order.DISTRICTNAME,
											"药品", recTel: order.logi_TelNum, weight: order.Weight, battery: YJT.Logistics.ShenTongLogistic.ClassCreateOrder.Cargo.EnumBattery.不带电30, ps: 备注
										);
										YJT.Logistics.ShenTongLogistic.ClassPostData cpd = null;
										YJT.Logistics.ShenTongLogistic.ClassCreateOrderRes r1 = null;
										int tryCount = 5;
										while (tryCount > 0)
										{
											tryCount--;
											r1 = _shenTongWl.CreateOrder(cco, out isOk, out errCode, out errMsg, out cpd, out stxt, out otxt);
											try
											{
												if (!System.IO.Directory.Exists(@"D:\WLLOG"))
												{
													System.IO.Directory.CreateDirectory(@"D:\WLLOG");
												}
												System.IO.File.AppendAllText(@"D:\WLLOG\ShenTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\ShenTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + stxt + "\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\ShenTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "返回-----------------------------------------------------------------\r\n");
												System.IO.File.AppendAllText(@"D:\WLLOG\ShenTong_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "\t" + otxt + "\r\n");

											}
											catch (Exception ee)
											{
												AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
											}
											if (isOk == true)
											{
												break;
											}
											else
											{
												System.Threading.Thread.Sleep(1000);
											}
										}
										
										if (r1 != null && isOk == true)
										{
											order.WL_COMPANYID = ((int)order.Logic).ToString();
											order.WL_COMPANYNAME = order.Logic.ToString();
											order.WL_NUMBER = r1.data.waybillNo;
											order.Status = Settings.Setings.EnumOrderStatus.已获取物流单号;
											order.logi_CreateDate = DateTime.Now.ToString("yyyyMMdd");
											order.ErrMsg = "创建物流订单完成";
											order.logi_baojiaJine = "0.00";
											order.logi_dsJine = "0.00";
											order.logi_feiyongTotal = "0.00";
											order.logi_goodName = "药品";
											order.logi_goodQty = "1";
											order.logi_logcNum = "";
											order.logi_monAccNum = Settings.APITokenKey.ShenTongCustomerName;
											order.logi_shoufqianshu = "";
											order.logi_shoufRiqi = "";
											order.logi_shouhuory = "";
											order.logi_ChanpinTypeStr = YJT.Logistics.ShenTongLogistic.ClassCreateOrder.EnumBillType.普通00.ToString();
											order.logi_PayType = "";
											order.logi_OrderId = r1.data.orderNo;
											order.JingdongWl = r1.data.packagePlace;
											order.logi_dstRoute = r1.data.bigWord;
											order.logi_jinjianRiqi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
											double total = 0;
											double baojia = 0;
											double yunfei = 0;
											if (!double.TryParse(order.logi_feiyongTotal, out total))
											{
												total = 0;
											}
											if (!double.TryParse(order.logi_baojiaJine, out baojia))
											{
												baojia = 0;
											}
											yunfei = total;
											if (yunfei <= 0)
											{
												order.logi_ysJine = yunfei.ToString("#0.000");
												order.logi_ysJineTotal = order.logi_ysJine;
											}
											else
											{
												order.logi_ysJine = "";
												order.logi_ysJineTotal = "";
											}
											isOk = true;
											errCode = 0;
											errMsg = "";
										}
										else
										{
											order.ErrMsg = "申通物流下单不成功:" + errMsg;
											order.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
											flag = true;
										}

										break;
									}
								case Settings.Setings.EnumLogicType.极兔百事:
									order.ErrMsg = "物流公司极兔百事不支持";
									order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
									flag = true;
									break;
								case Settings.Setings.EnumLogicType.Default:
									order.ErrMsg = "物流公司选择不确认或者平台信息错误";
									order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
									flag = true;
									break;
								default:
									order.ErrMsg = "暂不支持其他物流";
									order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
									flag = true;
									break;
							}
						}

					}
					else
					{
						//没有地址信息
						order.ErrMsg = "平台地址信息为空";
						order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
						flag = true;
					}

				}
				else
				{
					//order为null
					order.ErrMsg = "并非已获取电商信息的单据";
					order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
					flag = true;
				}
				//---------------
				if (flag == true)
				{
					if (VerDb_Local)
					{
						MOD.BllMod.Order t = ServerGetOrder(order.Bid.ToString(), out isok2, out errCode2, out errMsg2);
						if (t != null)
						{
							if (t.Status != Settings.Setings.EnumOrderStatus.准备停止)
							{
								string sqlCmd = $@"
UPDATE
	BllMod_Order
SET
	WmsDanjbh='{order.WmsDanjbh}',
	WmsYewy='{order.WmsYewy}',
	WmsDdwname='{order.WmsDdwname}',
	ServerHandleDate=GETDATE(),
	ClientHandleDate=GETDATE()-1,
	ErrMsg='{order.ErrMsg}',
	Status='{order.Status.ToString()}',
	NETORDER_FROMID='{order.NETORDER_FROMID}',
	NETORDER_FROM='{order.NETORDER_FROM}',
	ERPORDER_ID='{order.ERPORDER_ID}',
	CUSTOMID='{order.CUSTOMID}',
	CUSTOMNAME='{order.CUSTOMNAME}',
	AGENTNAME='{order.AGENTNAME}',
	ADDRESS='{order.ADDRESS}',
	WL_COMPANYID='{order.WL_COMPANYID}',
	WL_NUMBER='{order.WL_NUMBER}',
	WL_COMPANYNAME='{order.WL_COMPANYNAME}',
	RECVNAME='{order.RECVNAME}',
	RECVPHONE='{order.RECVPHONE}',
	PROVINCENAME='{order.PROVINCENAME}',
	CITYNAME='{order.CITYNAME}',
	DISTRICTNAME='{order.DISTRICTNAME}',
	STREETNAME='{order.STREETNAME}',
	ORIGINALREMARK='{order.ORIGINALREMARK}',
	PlatformType='{order.PlatformType.ToString()}',
	Logic='{order.Logic.ToString()}',
	PrintStatus='{order.PrintStatus}',
	IsFp='{order.IsFp}',
	IsPj='{order.IsPj}',
	IsHeTong='{order.IsHeTong}',
	IsQysy='{order.IsQysy}',
	IsSyzz='{order.IsSyzz}',
	IsYjbb='{order.IsYjbb}',
	logi_jinjianRiqi='{order.logi_jinjianRiqi}',
	logi_CreateDate='{order.logi_CreateDate}',
	logi_OrderId='{order.logi_OrderId}',
	needBaojia={order.needBaojia.ToString("#0.000")},
	total_amt='{order.total_amt}',
	logi_dstRoute='{order.logi_dstRoute}',
	logi_PayType='{order.logi_PayType}',
	logi_monAccNum='{order.logi_monAccNum}',
	logi_baojiaJine='{order.logi_baojiaJine}',
	logi_dsJine='{order.logi_dsJine}',
	logi_logcNum='{order.logi_logcNum}',
	logi_ysJine='{order.logi_ysJine}',
	logi_ysJineTotal='{order.logi_ysJineTotal}',
	logi_shouhuory='{order.logi_shouhuory}',
	logi_shoufqianshu='{order.logi_shoufqianshu}',
	logi_shoufRiqi='{order.logi_shoufRiqi}',
	logi_sendSheng='{order.logi_sendSheng}',
	logi_sendShi='{order.logi_sendShi}',
	logi_sendXian='{order.logi_sendXian}',
	logi_sendAddress='{order.logi_sendAddress}',
	logi_sendMan='{order.logi_sendMan}',
	logi_sendPhone='{order.logi_sendPhone}',
	logi_feiyongTotal='{order.logi_feiyongTotal}',
	logi_goodQty='{order.logi_goodQty}',
	logi_goodName='{order.logi_goodName}',
	logi_ChanpinTypeStr='{order.logi_ChanpinTypeStr}',
	logi_ReceivePwd='{order.logi_ReceivePwd}',
	logi_PhonNum='{order.logi_PhonNum}',
	logi_TelNum='{order.logi_TelNum}',
	JingdongWl='{order.JingdongWl}'
OUTPUT
	Inserted.ServerHandleDate
WHERE Bid={order.Bid}
";
								DateTime dbres = YJT.DataBase.Common.ObjectTryToObj(_dbhLocal.ExecuteScalar(sqlCmd, null, true), DateTime.MinValue);
								if (dbres != DateTime.MinValue)
								{
									order.ServerHandleDate = dbres;
									isOk = true;
									errCode = 0;
									errMsg = "";
								}
								else
								{
									AddMsgOut("更新电商数据单据时候失败", Settings.Setings.EnumMessageType.异常, errCode, sqlCmd);
								}
							}
							else
							{
								errCode = -4;
								isOk = false;
								errMsg = "单据在处理时,被要求删除";
								AddMsgOut("单据在处理时,被要求删除", Settings.Setings.EnumMessageType.提示, errCode, errMsg2);
							}
						}
						else
						{
							errCode = -1;
							errMsg = "本地数据库中不存在此单据";
							isOk = false;
							AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
						}
					}
					else
					{
						errCode = -1;
						errMsg = "本地数据库连接错误";
						isOk = false;
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
					}
				}
			}
			else
			{
				errCode = -1;
				errMsg = "创建物流时,订单为NULL";
				isOk = false;
				AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
			}

			return order;
		}
		/// <summary>
		/// 获取物流订单信息
		/// </summary>
		/// <param name="orderThis"></param>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public BllMod.Order ServerLogicInfo(BllMod.Order order, out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errCode = -99;
			errMsg = "";

			bool isok2 = false;
			int errCode2 = -99;
			string errMsg2 = "";
			if (order != null)
			{
				bool flag = false;
				if (order.Status == Settings.Setings.EnumOrderStatus.已获取物流单号)
				{
					switch (order.Logic)
					{
						case Setings.EnumLogicType.申通快递:
							order.ErrMsg = "物流信息获取完成";
							flag = true;
							break;
						case Setings.EnumLogicType.中通快递:
							order.ErrMsg = "物流信息获取完成";
							flag = true;
							break;
						case Settings.Setings.EnumLogicType.京东物流:
							order.ErrMsg = "物流信息获取完成";
							flag = true;
							break;
						case Settings.Setings.EnumLogicType.极兔百事:
							order.ErrMsg = "暂不支持极兔百事物流获取物流信息";
							order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
							break;
						case Settings.Setings.EnumLogicType.邮政EMS:
							order.ErrMsg = "物流信息获取完成";
							flag = true;
							break;
						case Settings.Setings.EnumLogicType.顺丰:
							//YJT.Logistics.ShunFengLogistic sf = YJT.Logistics.ShunFengLogistic.Init(Settings.APITokenKey.ShunfengCustomerId, Settings.APITokenKey.ShunfengSecret);
							//sf.IsTest = Settings.APITokenKey.ShunfengIsTest;
							YJT.Logistics.ShunFengLogistic.ClassGetOrderInfoRes res2 = null;
							int tryCountSf = 5;
							while (tryCountSf > 0)
							{
								tryCountSf--;
								res2 = _sf.GetOrderInfo(order.ERPORDER_ID, out isOk, out errCode, out errMsg);
								if (isOk == true)
								{
									break;
								}
								else
								{
									System.Threading.Thread.Sleep(1000);
								}


							}
							
							if (res2!=null && isOk == true && res2.data2 != null)
							{
								if (order.WL_NUMBER != res2.data2.mailNo)
								{
									order.ErrMsg = "物流获取的运单号与下单运单号不一致";
									order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
								}
								else
								{
									order.ErrMsg = "物流信息获取完成";
									order.logi_baojiaJine = res2.data2.insureFee;
									order.logi_dsJine = res2.data2.codValue;
									order.logi_dstRoute = res2.data2.destRouteLabel;
									order.logi_feiyongTotal = res2.data2.totalFee;
									order.logi_goodName = res2.data2.cargo;
									order.logi_goodQty = res2.data2.cargoCount;
									order.logi_logcNum = res2.data2.codMonthAccount;
									order.logi_monAccNum = res2.data2.monthAccount;

									order.logi_sendAddress = res2.data2.deliverAddress;
									order.logi_sendMan = res2.data2.deliverName;
									order.logi_sendPhone = res2.data2.deliverTel;
									order.logi_sendSheng = res2.data2.deliverProvince;
									order.logi_sendShi = res2.data2.deliverCity;
									order.logi_sendXian = res2.data2.deliverCounty;
									order.logi_shoufqianshu = res2.data2.returnTrackingNo;
									order.logi_shoufRiqi = "";
									order.logi_shouhuory = "";
									order.logi_ChanpinTypeStr = res2.data2.expressType;
									YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型 t = YJT.DataBase.Common.ObjectTryToObj(order.logi_ChanpinTypeStr, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum产品类型.错误);
									order.logi_ChanpinTypeStr = t.ToString();


									order.logi_PayType = res2.data2.payMethod;//坑
									YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式 t2 = YJT.DataBase.Common.ObjectTryToObj(order.logi_PayType, YJT.Logistics.ShunFengLogistic.ClassCreateOrder.enum付款方式.错误);
									order.logi_PayType = t2.ToString();

									order.logi_OrderId = res2.data2.orderNo;
									double total = 0;
									double baojia = 0;
									double yunfei = 0;
									if (!double.TryParse(order.logi_feiyongTotal, out total))
									{
										total = 0;
									}
									if (!double.TryParse(order.logi_baojiaJine, out baojia))
									{
										baojia = 0;
									}
									yunfei = total;
									if (yunfei <= 0)
									{
										order.logi_ysJine = yunfei.ToString("#0.000");
										order.logi_ysJineTotal = order.logi_ysJine;
									}
									else
									{
										order.logi_ysJine = "";
										order.logi_ysJineTotal = "";
									}

								}

							}
							else
							{
								order.ErrMsg = "物流获取的运单号失败";
								order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
							}
							break;
						case Settings.Setings.EnumLogicType.Default:
							order.ErrMsg = "物流公司选择不确认无法获取物流信息";
							order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
							break;
						default:
							order.ErrMsg = "暂不支持其他物流获取物流信息";
							order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
							break;
					}

					flag = true;
				}
				else
				{
					order.ErrMsg = "并非已生成物流信息的单据";
					order.Status = Settings.Setings.EnumOrderStatus.异常_物流信息不正确;
					flag = true;
				}
				//---------------
				if (flag == true)
				{
					string sqlCmd = "";
					if (order.Status == Settings.Setings.EnumOrderStatus.已获取物流单号)
					{
						//如果不是这个状态,就说明单号不一致,所以不要回写接口表
						if (VerDb_Interface)
						{
							long wl_companyid = 0;
							if (!long.TryParse(order.WL_COMPANYID, out wl_companyid))
							{
								wl_companyid = 0;
							}
							long netorder_fromid = 0;
							if (!long.TryParse(order.NETORDER_FROMID, out netorder_fromid))
							{
								netorder_fromid = 0;
							}
							long erporder_id = 0;
							if (!long.TryParse(order.ERPORDER_ID, out erporder_id))
							{
								erporder_id = 0;
							}
							long customid = 0;
							if (!long.TryParse(order.CUSTOMID, out customid))
							{
								customid = 0;
							}
							sqlCmd = $@"
update
	NETORDER_DOC a
set
	a.wl_companyid={wl_companyid.ToString()},
	a.wl_number='{order.WL_NUMBER}',
	a.WL_COMPANYNAME='{order.WL_COMPANYNAME}',
	a.WL_CREDATE=to_date('{order.logi_jinjianRiqi}','yyyy-mm-dd hh24:mi:ss')
where
	a.netorder_id='{order.NetDanjbh}'
	and a.netorder_fromid={netorder_fromid.ToString()}
	and a.NETORDER_FROM='{order.NETORDER_FROM}'
	and a.erporder_id={erporder_id.ToString()}
	and a.customid={customid.ToString()}
	and a.agentname='{order.AGENTNAME}'
";
							int dbres = _dbhInterface.ExecuteNonQuery(sqlCmd, null, System.Data.CommandType.Text, false, true, "");
							if (dbres > 0)
							{
								order.Status = Settings.Setings.EnumOrderStatus.已回写电商平台;
								order.PrintStatus = Settings.Setings.EnumOrderPrintStatus.等待打印;
								order.ErrMsg = "回写电商接口完成";
							}
							else
							{
								order.ErrMsg = "电商接口回写失败";
								AddMsgOut("电商接口回写失败", Settings.Setings.EnumMessageType.异常, -1, sqlCmd);
							}
						}
					}

					if (VerDb_Local)
					{
						MOD.BllMod.Order t = ServerGetOrder(order.Bid.ToString(), out isok2, out errCode2, out errMsg2);
						if (t != null)
						{
							if (t.Status != Settings.Setings.EnumOrderStatus.准备停止)
							{
								sqlCmd = $@"
UPDATE
	BllMod_Order
SET
	WmsDanjbh='{order.WmsDanjbh}',
	WmsYewy='{order.WmsYewy}',
	WmsDdwname='{order.WmsDdwname}',
	ServerHandleDate=GETDATE(),
	ClientHandleDate=GETDATE()-1,
	ErrMsg='{order.ErrMsg}',
	Status='{order.Status.ToString()}',
	NETORDER_FROMID='{order.NETORDER_FROMID}',
	NETORDER_FROM='{order.NETORDER_FROM}',
	ERPORDER_ID='{order.ERPORDER_ID}',
	CUSTOMID='{order.CUSTOMID}',
	CUSTOMNAME='{order.CUSTOMNAME}',
	AGENTNAME='{order.AGENTNAME}',
	ADDRESS='{order.ADDRESS}',
	WL_COMPANYID='{order.WL_COMPANYID}',
	WL_NUMBER='{order.WL_NUMBER}',
	WL_COMPANYNAME='{order.WL_COMPANYNAME}',
	RECVNAME='{order.RECVNAME}',
	RECVPHONE='{order.RECVPHONE}',
	PROVINCENAME='{order.PROVINCENAME}',
	CITYNAME='{order.CITYNAME}',
	DISTRICTNAME='{order.DISTRICTNAME}',
	STREETNAME='{order.STREETNAME}',
	ORIGINALREMARK='{order.ORIGINALREMARK}',
	PlatformType='{order.PlatformType.ToString()}',
	PrintStatus='{order.PrintStatus}',
	IsFp='{order.IsFp}',
	IsPj='{order.IsPj}',
	IsHeTong='{order.IsHeTong}',
	IsQysy='{order.IsQysy}',
	IsSyzz='{order.IsSyzz}',
	IsYjbb='{order.IsYjbb}',
	logi_dstRoute='{order.logi_dstRoute}',
	logi_PayType='{order.logi_PayType}',
	logi_monAccNum='{order.logi_monAccNum}',
	logi_baojiaJine='{order.logi_baojiaJine}',
	logi_dsJine='{order.logi_dsJine}',
	logi_logcNum='{order.logi_logcNum}',
	logi_ysJine='{order.logi_ysJine}',
	logi_ysJineTotal='{order.logi_ysJineTotal}',
	logi_shouhuory='{order.logi_shouhuory}',
	logi_jinjianRiqi='{order.logi_jinjianRiqi}',
	logi_shoufqianshu='{order.logi_shoufqianshu}',
	logi_shoufRiqi='{order.logi_shoufRiqi}',
	logi_sendSheng='{order.logi_sendSheng}',
	logi_sendShi='{order.logi_sendShi}',
	logi_sendXian='{order.logi_sendXian}',
	logi_sendAddress='{order.logi_sendAddress}',
	logi_sendMan='{order.logi_sendMan}',
	logi_sendPhone='{order.logi_sendPhone}',
	logi_feiyongTotal='{order.logi_feiyongTotal}',
	logi_goodQty='{order.logi_goodQty}',
	logi_goodName='{order.logi_goodName}',
	logi_OrderId='{order.logi_OrderId}',
	logi_CreateDate='{order.logi_CreateDate}',
	logi_ChanpinTypeStr='{order.logi_ChanpinTypeStr}',
	JingdongWl='{order.JingdongWl}'
OUTPUT
	Inserted.ServerHandleDate
WHERE Bid={order.Bid}
";
								DateTime dbres = YJT.DataBase.Common.ObjectTryToObj(_dbhLocal.ExecuteScalar(sqlCmd, null, true), DateTime.MinValue);
								if (dbres != DateTime.MinValue)
								{
									order.ServerHandleDate = dbres;
									isOk = true;
									errCode = 0;
									errMsg = "";
								}
								else
								{
									AddMsgOut("更新电商数据单据时候失败", Settings.Setings.EnumMessageType.异常, errCode, sqlCmd);
								}
							}
							else
							{
								errCode = -4;
								isOk = false;
								errMsg = "单据在处理时,被要求删除";
								AddMsgOut("单据在处理时,被要求删除", Settings.Setings.EnumMessageType.提示, errCode, errMsg2);
							}
						}
						else
						{
							errCode = -1;
							errMsg = "本地数据库中不存在此单据";
							isOk = false;
							AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
						}
					}
					else
					{
						errCode = -1;
						errMsg = "本地数据库连接错误";
						isOk = false;
						AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
					}
				}
			}
			else
			{
				errCode = -1;
				errMsg = "获取物流订单时,订单为NULL";
				isOk = false;
				AddMsgOut(errMsg, Settings.Setings.EnumMessageType.异常, errCode, "");
			}

			return order;
		}
		/// <summary>
		/// 服务器部分,获取要处理的一张单据
		/// </summary>
		/// <param name="isOk"></param>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public BllMod.Order GetNeedServerHandle(string ip,string mac,out bool isOk, out int errCode, out string errMsg)
		{
			isOk = false;
			errMsg = "";
			errCode = -99;
			BllMod.Order order = null;
			//定义语句
			string sqlCmd = $@"
select top 1
	isnull(Bid,-1) as Bid ,
	isnull(OrderId,'') as OrderId ,
	isnull(AddDate,convert(datetime,'1900-01-01')) as AddDate ,
	isnull(ErpId,'') as ErpId ,
	isnull(Logic,'Default') as Logic ,
	isnull(Weight,1) as Weight ,
	isnull(Length,1) as Length ,
	isnull(Width,1) as Width ,
	isnull(Height,1) as Height ,
	isnull(Status,'无动作') as Status ,
	isnull(Ip,'') as Ip ,
	isnull(Mac,'') as  Mac,
	isnull(ComputerName,'') as ComputerName ,
	isnull(ErrMsg,'') as ErrMsg ,
	isnull(ClientId,-1) as ClientId ,
	isnull(PrintStatus,'不可打印') as PrintStatus ,
	isnull(WmsDanjbh,'') as WmsDanjbh ,
	isnull(netDanjbh,'') as netDanjbh ,
	CONVERT(VARCHAR(23),ISNULL(ServerHandleDate,CAST('1970-01-01' AS datetime)),21) as ServerHandleDate ,
	isnull(ClientHandleDate,convert(datetime,'1900-01-01')) as ClientHandleDate,
	isnull(WmsYewy,'') as WmsYewy,
	isnull(WmsDdwname,'') as WmsDdwname,
	isnull(NETORDER_FROMID,'') as NETORDER_FROMID,

	isnull(NETORDER_FROM,'') as NETORDER_FROM,
	isnull(ERPORDER_ID,'') as ERPORDER_ID,
	isnull(CUSTOMID,'') as CUSTOMID,
	isnull(CUSTOMNAME,'') as CUSTOMNAME,
	isnull(AGENTNAME,'') as AGENTNAME,
	isnull(ADDRESS,'') as ADDRESS,
	isnull(WL_COMPANYID,'') as WL_COMPANYID,
	isnull(WL_NUMBER,'') as WL_NUMBER,
	isnull(WL_COMPANYNAME,'') as WL_COMPANYNAME,
	isnull(RECVNAME,'') as RECVNAME,
	isnull(RECVPHONE,'') as RECVPHONE,
	isnull(PROVINCENAME,'') as PROVINCENAME,
	isnull(CITYNAME,'') as CITYNAME,
	isnull(DISTRICTNAME,'') as DISTRICTNAME,
	isnull(STREETNAME,'') as STREETNAME,
	isnull(ORIGINALREMARK,'') as ORIGINALREMARK,
	isnull(IsFp,'') as IsFp,
	isnull(IsPj,'') as IsPj,
	isnull(IsHeTong,'') as IsHeTong,
	isnull(IsQysy,'') as IsQysy,
	isnull(IsSyzz,'') as IsSyzz,
	isnull(IsYjbb,'') as IsYjbb,
	isnull(PlatformType,'无') as PlatformType,
	isnull(logi_dstRoute,'') as logi_dstRoute,
	isnull(logi_PayType,'') as logi_PayType,
	isnull(logi_monAccNum,'') as logi_monAccNum,
	isnull(logi_baojiaJine,'') as logi_baojiaJine,
	isnull(logi_dsJine,'') as logi_dsJine,
	isnull(logi_logcNum,'') as logi_logcNum,
	isnull(logi_ysJine,'') as logi_ysJine,
	isnull(logi_ysJineTotal,'') as logi_ysJineTotal,
	isnull(logi_shouhuory,'') as logi_shouhuory,
	isnull(logi_jinjianRiqi,'') as logi_jinjianRiqi,
	isnull(logi_shoufqianshu,'') as logi_shoufqianshu,
	isnull(logi_shoufRiqi,'') as logi_shoufRiqi,
	isnull(logi_sendSheng,'') as logi_sendSheng,
	isnull(logi_sendShi,'') as logi_sendShi,
	isnull(logi_sendXian,'') as logi_sendXian,
	isnull(logi_sendAddress,'') as logi_sendAddress,
	isnull(logi_sendMan,'') as logi_sendMan,
	isnull(logi_sendPhone,'') as logi_sendPhone,
	isnull(logi_feiyongTotal,'') as logi_feiyongTotal,
	isnull(logi_goodQty,'') as logi_goodQty,
	isnull(logi_goodName,'') as logi_goodName,
	isnull(needBaojia,0) as needBaojia,
	isnull(logi_OrderId,'') as logi_OrderId,
	isnull(logi_CreateDate,'') as logi_CreateDate,
	isnull(logi_ChanpinTypeStr,'') as logi_ChanpinTypeStr,
	isnull(PrintDatetime,'') as PrintDatetime,
	isnull(sysFirst,'') as sysFirst,
	isnull(total_amt,'0') as total_amt,
	isnull(RelBId,-1) as RelBId,
	isnull(fplx,'') as fplx,
	isnull(ServerTaskType,'异常_类型转换失败') as ServerTaskType,
	isnull(PAIDCOSTS,0) as PAIDCOSTS,
	isnull(logi_ReceivePwd,'') as logi_ReceivePwd,
	isnull(logi_SubOrderSn,0) as logi_SubOrderSn,
	isnull(logi_PhonNum,'') as logi_PhonNum,
	isnull(logi_TelNum,'') as logi_TelNum,
	isnull(RelOrderId,-1) as RelOrderId,
	isnull(JingdongWl,'') as JingdongWl
from
	BllMod_Order
where
	isnull(Ip,'')='{ip}'
	and isnull(Mac,'')='{mac}'
	and isnull(Status,'无动作') in
	(
		'{Settings.Setings.EnumOrderStatus.准备停止.ToString()}',
		'{Settings.Setings.EnumOrderStatus.已下传.ToString()}',
		'{Settings.Setings.EnumOrderStatus.已获取WMS信息.ToString()}',
		'{Settings.Setings.EnumOrderStatus.已获取电商信息.ToString()}',
		'{Settings.Setings.EnumOrderStatus.已获取物流单号.ToString()}',
		'{Settings.Setings.EnumOrderStatus.已回写电商平台.ToString()}',
		'{Settings.Setings.EnumOrderStatus.补打任务.ToString()}',
		'{Settings.Setings.EnumOrderStatus.追加物流子单.ToString()}',
		'{Settings.Setings.EnumOrderStatus.ERP组合单.ToString()}',
		'{Settings.Setings.EnumOrderStatus.关机任务.ToString()}'
	)
	/* or isnull(ClientHandleDate,convert(datetime,'1900-01-01'))>=isnull(ServerHandleDate,convert(datetime,'1900-01-01'))*/
order by
	case
		when isnull(Status,'无动作')='{Settings.Setings.EnumOrderStatus.准备停止.ToString()}' then 0
		when isnull(Status,'无动作')='{Settings.Setings.EnumOrderStatus.关机任务.ToString()}' then 10000
		else 10
	end asc,
	Bid asc

";
			System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
			if (dt.Rows.Count > 0)
			{
				System.Data.DataRow item = dt.Rows[0];
				//构建对象
				order = new BllMod.Order()
				{
					AddDate = YJT.DataBase.Common.ObjectTryToObj(item["AddDate"], DateTime.MinValue),
					ClientId = YJT.DataBase.Common.ObjectTryToObj(item["ClientId"], -1),
					ComputerName = YJT.DataBase.Common.ObjectTryToObj(item["ComputerName"], ""),
					ErpId = YJT.DataBase.Common.ObjectTryToObj(item["ErpId"], ""),
					ErrMsg = YJT.DataBase.Common.ObjectTryToObj(item["ErrMsg"], ""),
					Height = YJT.DataBase.Common.ObjectTryToObj(item["Height"], 1d),
					Bid = YJT.DataBase.Common.ObjectTryToObj(item["Bid"], -1),
					Ip = YJT.DataBase.Common.ObjectTryToObj(item["Ip"], ""),
					Length = YJT.DataBase.Common.ObjectTryToObj(item["Length"], 1d),
					Logic = YJT.DataBase.Common.ObjectTryToObj(item["Logic"], Settings.Setings.EnumLogicType.Default),
					Mac = YJT.DataBase.Common.ObjectTryToObj(item["Mac"], ""),
					NetDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["NetDanjbh"], ""),
					OrderId = YJT.DataBase.Common.ObjectTryToObj(item["OrderId"], -1),
					PrintStatus = YJT.DataBase.Common.ObjectTryToObj(item["PrintStatus"], Settings.Setings.EnumOrderPrintStatus.不可打印),
					Status = YJT.DataBase.Common.ObjectTryToObj(item["Status"], Settings.Setings.EnumOrderStatus.异常_类型转换失败),
					Weight = YJT.DataBase.Common.ObjectTryToObj(item["Weight"], 1d),
					Width = YJT.DataBase.Common.ObjectTryToObj(item["Width"], 1d),
					WmsDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["WmsDanjbh"], ""),
					ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ClientHandleDate"], DateTime.Parse("1900-01-01")),
					ServerHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ServerHandleDate"], DateTime.Parse("1900-01-01")),
					WmsYewy = YJT.DataBase.Common.ObjectTryToObj(item["WmsYewy"], ""),
					WmsDdwname = YJT.DataBase.Common.ObjectTryToObj(item["WmsDdwname"], ""),
					NETORDER_FROMID = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROMID"], ""),

					NETORDER_FROM = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROM"], ""),
					ERPORDER_ID = YJT.DataBase.Common.ObjectTryToObj(item["ERPORDER_ID"], ""),
					CUSTOMID = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMID"], ""),
					CUSTOMNAME = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMNAME"], ""),
					AGENTNAME = YJT.DataBase.Common.ObjectTryToObj(item["AGENTNAME"], ""),
					ADDRESS = YJT.DataBase.Common.ObjectTryToObj(item["ADDRESS"], ""),
					WL_COMPANYID = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYID"], ""),
					WL_NUMBER = YJT.DataBase.Common.ObjectTryToObj(item["WL_NUMBER"], ""),
					WL_COMPANYNAME = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYNAME"], ""),
					RECVNAME = YJT.DataBase.Common.ObjectTryToObj(item["RECVNAME"], ""),
					RECVPHONE = YJT.DataBase.Common.ObjectTryToObj(item["RECVPHONE"], ""),
					PROVINCENAME = YJT.DataBase.Common.ObjectTryToObj(item["PROVINCENAME"], ""),
					CITYNAME = YJT.DataBase.Common.ObjectTryToObj(item["CITYNAME"], ""),
					DISTRICTNAME = YJT.DataBase.Common.ObjectTryToObj(item["DISTRICTNAME"], ""),
					STREETNAME = YJT.DataBase.Common.ObjectTryToObj(item["STREETNAME"], ""),
					ORIGINALREMARK = YJT.DataBase.Common.ObjectTryToObj(item["ORIGINALREMARK"], ""),
					IsFp = YJT.DataBase.Common.ObjectTryToObj(item["IsFp"], ""),
					IsPj = YJT.DataBase.Common.ObjectTryToObj(item["IsPj"], ""),
					IsHeTong = YJT.DataBase.Common.ObjectTryToObj(item["IsHeTong"], ""),
					IsQysy = YJT.DataBase.Common.ObjectTryToObj(item["IsQysy"], ""),
					IsSyzz = YJT.DataBase.Common.ObjectTryToObj(item["IsSyzz"], ""),
					IsYjbb = YJT.DataBase.Common.ObjectTryToObj(item["IsYjbb"], ""),
					PlatformType = YJT.DataBase.Common.ObjectTryToObj(item["PlatformType"], Settings.Setings.EnumPlatformType.无),
					logi_dstRoute = YJT.DataBase.Common.ObjectTryToObj(item["logi_dstRoute"], ""),
					logi_PayType = YJT.DataBase.Common.ObjectTryToObj(item["logi_PayType"], ""),
					logi_monAccNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_monAccNum"], ""),
					logi_baojiaJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_baojiaJine"], ""),
					logi_dsJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_dsJine"], ""),
					logi_logcNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_logcNum"], ""),
					logi_ysJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJine"], ""),
					logi_ysJineTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJineTotal"], ""),
					logi_shouhuory = YJT.DataBase.Common.ObjectTryToObj(item["logi_shouhuory"], ""),
					logi_jinjianRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_jinjianRiqi"], ""),
					logi_shoufqianshu = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufqianshu"], ""),
					logi_shoufRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufRiqi"], ""),
					logi_sendSheng = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendSheng"], ""),
					logi_sendShi = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendShi"], ""),
					logi_sendXian = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendXian"], ""),
					logi_sendAddress = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendAddress"], ""),
					logi_sendMan = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendMan"], ""),
					logi_sendPhone = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendPhone"], ""),
					logi_feiyongTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_feiyongTotal"], ""),
					logi_goodQty = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodQty"], ""),
					logi_goodName = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodName"], ""),
					needBaojia = YJT.DataBase.Common.ObjectTryToObj(item["needBaojia"], 0d),
					logi_OrderId = YJT.DataBase.Common.ObjectTryToObj(item["logi_OrderId"], ""),
					logi_CreateDate = YJT.DataBase.Common.ObjectTryToObj(item["logi_CreateDate"], ""),
					logi_ChanpinTypeStr = YJT.DataBase.Common.ObjectTryToObj(item["logi_ChanpinTypeStr"], ""),
					PrintDatetime = YJT.DataBase.Common.ObjectTryToObj(item["PrintDatetime"], ""),
					sysFirst = YJT.DataBase.Common.ObjectTryToObj(item["sysFirst"], ""),
					total_amt = YJT.DataBase.Common.ObjectTryToObj(item["total_amt"], "0"),
					RelBId = YJT.DataBase.Common.ObjectTryToObj(item["RelBId"], -1),
					fplx = YJT.DataBase.Common.ObjectTryToObj(item["fplx"], ""),
					ServerTaskType = YJT.DataBase.Common.ObjectTryToObj(item["ServerTaskType"], Settings.Setings.EnumServerTaskType.异常_类型转换失败),
					PAIDCOSTS = YJT.DataBase.Common.ObjectTryToObj(item["PAIDCOSTS"], 0d),
					logi_ReceivePwd = YJT.DataBase.Common.ObjectTryToObj(item["logi_ReceivePwd"], ""),
					logi_SubOrderSn = YJT.DataBase.Common.ObjectTryToObj(item["logi_SubOrderSn"], 0),
					logi_PhonNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_PhonNum"], ""),
					logi_TelNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_TelNum"], ""),
					RelOrderId = YJT.DataBase.Common.ObjectTryToObj(item["RelOrderId"], -1L),
					JingdongWl = YJT.DataBase.Common.ObjectTryToObj(item["JingdongWl"], ""),
				};

			}
			try
			{
				dt.Dispose();
				dt = null;
			}
			catch { }


			return order;
		}
		public BllMod.Order GetNeedServerHandle2(out bool isOk, out int errCode, out string errMsg,string bid, Settings.Setings.EnumOrderStatus orderstatus)
		{
			isOk = false;
			errMsg = "";
			errCode = -99;
			BllMod.Order order = null;
			//定义语句
			string sqlCmd = $@"

select top 1
	isnull(Bid,-1) as Bid ,
	isnull(OrderId,'') as OrderId ,
	isnull(AddDate,convert(datetime,'1900-01-01')) as AddDate ,
	isnull(ErpId,'') as ErpId ,
	isnull(Logic,'Default') as Logic ,
	isnull(Weight,1) as Weight ,
	isnull(Length,1) as Length ,
	isnull(Width,1) as Width ,
	isnull(Height,1) as Height ,
	isnull(Status,'无动作') as Status ,
	isnull(Ip,'') as Ip ,
	isnull(Mac,'') as  Mac,
	isnull(ComputerName,'') as ComputerName ,
	isnull(ErrMsg,'') as ErrMsg ,
	isnull(ClientId,-1) as ClientId ,
	isnull(PrintStatus,'不可打印') as PrintStatus ,
	isnull(WmsDanjbh,'') as WmsDanjbh ,
	isnull(netDanjbh,'') as netDanjbh ,
	CONVERT(VARCHAR(23),ISNULL(ServerHandleDate,CAST('1970-01-01' AS datetime)),21) as ServerHandleDate ,
	isnull(ClientHandleDate,convert(datetime,'1900-01-01')) as ClientHandleDate,
	isnull(WmsYewy,'') as WmsYewy,
	isnull(WmsDdwname,'') as WmsDdwname,
	isnull(NETORDER_FROMID,'') as NETORDER_FROMID,

	isnull(NETORDER_FROM,'') as NETORDER_FROM,
	isnull(ERPORDER_ID,'') as ERPORDER_ID,
	isnull(CUSTOMID,'') as CUSTOMID,
	isnull(CUSTOMNAME,'') as CUSTOMNAME,
	isnull(AGENTNAME,'') as AGENTNAME,
	isnull(ADDRESS,'') as ADDRESS,
	isnull(WL_COMPANYID,'') as WL_COMPANYID,
	isnull(WL_NUMBER,'') as WL_NUMBER,
	isnull(WL_COMPANYNAME,'') as WL_COMPANYNAME,
	isnull(RECVNAME,'') as RECVNAME,
	isnull(RECVPHONE,'') as RECVPHONE,
	isnull(PROVINCENAME,'') as PROVINCENAME,
	isnull(CITYNAME,'') as CITYNAME,
	isnull(DISTRICTNAME,'') as DISTRICTNAME,
	isnull(STREETNAME,'') as STREETNAME,
	isnull(ORIGINALREMARK,'') as ORIGINALREMARK,
	isnull(IsFp,'') as IsFp,
	isnull(IsPj,'') as IsPj,
	isnull(IsHeTong,'') as IsHeTong,
	isnull(IsQysy,'') as IsQysy,
	isnull(IsSyzz,'') as IsSyzz,
	isnull(IsYjbb,'') as IsYjbb,
	isnull(PlatformType,'无') as PlatformType,
	isnull(logi_dstRoute,'') as logi_dstRoute,
	isnull(logi_PayType,'') as logi_PayType,
	isnull(logi_monAccNum,'') as logi_monAccNum,
	isnull(logi_baojiaJine,'') as logi_baojiaJine,
	isnull(logi_dsJine,'') as logi_dsJine,
	isnull(logi_logcNum,'') as logi_logcNum,
	isnull(logi_ysJine,'') as logi_ysJine,
	isnull(logi_ysJineTotal,'') as logi_ysJineTotal,
	isnull(logi_shouhuory,'') as logi_shouhuory,
	isnull(logi_jinjianRiqi,'') as logi_jinjianRiqi,
	isnull(logi_shoufqianshu,'') as logi_shoufqianshu,
	isnull(logi_shoufRiqi,'') as logi_shoufRiqi,
	isnull(logi_sendSheng,'') as logi_sendSheng,
	isnull(logi_sendShi,'') as logi_sendShi,
	isnull(logi_sendXian,'') as logi_sendXian,
	isnull(logi_sendAddress,'') as logi_sendAddress,
	isnull(logi_sendMan,'') as logi_sendMan,
	isnull(logi_sendPhone,'') as logi_sendPhone,
	isnull(logi_feiyongTotal,'') as logi_feiyongTotal,
	isnull(logi_goodQty,'') as logi_goodQty,
	isnull(logi_goodName,'') as logi_goodName,
	isnull(needBaojia,0) as needBaojia,
	isnull(logi_OrderId,'') as logi_OrderId,
	isnull(logi_CreateDate,'') as logi_CreateDate,
	isnull(logi_ChanpinTypeStr,'') as logi_ChanpinTypeStr,
	isnull(PrintDatetime,'') as PrintDatetime,
	isnull(sysFirst,'') as sysFirst,
	isnull(total_amt,'0') as total_amt,
	isnull(RelBId,-1) as RelBId,
	isnull(fplx,'') as fplx,
	isnull(ServerTaskType,'异常_类型转换失败') as ServerTaskType,
	isnull(PAIDCOSTS,0) as PAIDCOSTS,
	isnull(logi_ReceivePwd,'') as logi_ReceivePwd,
	isnull(logi_SubOrderSn,0) as logi_SubOrderSn,
	isnull(logi_PhonNum,'') as logi_PhonNum,
	isnull(logi_TelNum,'') as logi_TelNum,
	isnull(RelOrderId,-1) as RelOrderId,
	isnull(JingdongWl,'') as JingdongWl
from
	BllMod_Order
where
		Bid={bid}

order by
	case
		when isnull(Status,'无动作')='{Settings.Setings.EnumOrderStatus.准备停止.ToString()}' then 0
		when isnull(Status,'无动作')='{Settings.Setings.EnumOrderStatus.关机任务.ToString()}' then 10000
		else 10
	end asc,
	Bid asc

";
			System.Data.DataTable dt = _dbhLocal.ExecuteToDataTable(sqlCmd, null, true);
			if (dt.Rows.Count > 0)
			{
				System.Data.DataRow item = dt.Rows[0];
				//构建对象
				order = new BllMod.Order()
				{
					AddDate = YJT.DataBase.Common.ObjectTryToObj(item["AddDate"], DateTime.MinValue),
					ClientId = YJT.DataBase.Common.ObjectTryToObj(item["ClientId"], -1),
					ComputerName = YJT.DataBase.Common.ObjectTryToObj(item["ComputerName"], ""),
					ErpId = YJT.DataBase.Common.ObjectTryToObj(item["ErpId"], ""),
					ErrMsg = YJT.DataBase.Common.ObjectTryToObj(item["ErrMsg"], ""),
					Height = YJT.DataBase.Common.ObjectTryToObj(item["Height"], 1d),
					Bid = YJT.DataBase.Common.ObjectTryToObj(item["Bid"], -1),
					Ip = YJT.DataBase.Common.ObjectTryToObj(item["Ip"], ""),
					Length = YJT.DataBase.Common.ObjectTryToObj(item["Length"], 1d),
					Logic = YJT.DataBase.Common.ObjectTryToObj(item["Logic"], Settings.Setings.EnumLogicType.Default),
					Mac = YJT.DataBase.Common.ObjectTryToObj(item["Mac"], ""),
					NetDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["NetDanjbh"], ""),
					OrderId = YJT.DataBase.Common.ObjectTryToObj(item["OrderId"], -1),
					PrintStatus = YJT.DataBase.Common.ObjectTryToObj(item["PrintStatus"], Settings.Setings.EnumOrderPrintStatus.不可打印),
					Status = YJT.DataBase.Common.ObjectTryToObj(item["Status"], Settings.Setings.EnumOrderStatus.异常_类型转换失败),
					Weight = YJT.DataBase.Common.ObjectTryToObj(item["Weight"], 1d),
					Width = YJT.DataBase.Common.ObjectTryToObj(item["Width"], 1d),
					WmsDanjbh = YJT.DataBase.Common.ObjectTryToObj(item["WmsDanjbh"], ""),
					ClientHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ClientHandleDate"], DateTime.Parse("1900-01-01")),
					ServerHandleDate = YJT.DataBase.Common.ObjectTryToObj(item["ServerHandleDate"], DateTime.Parse("1900-01-01")),
					WmsYewy = YJT.DataBase.Common.ObjectTryToObj(item["WmsYewy"], ""),
					WmsDdwname = YJT.DataBase.Common.ObjectTryToObj(item["WmsDdwname"], ""),
					NETORDER_FROMID = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROMID"], ""),

					NETORDER_FROM = YJT.DataBase.Common.ObjectTryToObj(item["NETORDER_FROM"], ""),
					ERPORDER_ID = YJT.DataBase.Common.ObjectTryToObj(item["ERPORDER_ID"], ""),
					CUSTOMID = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMID"], ""),
					CUSTOMNAME = YJT.DataBase.Common.ObjectTryToObj(item["CUSTOMNAME"], ""),
					AGENTNAME = YJT.DataBase.Common.ObjectTryToObj(item["AGENTNAME"], ""),
					ADDRESS = YJT.DataBase.Common.ObjectTryToObj(item["ADDRESS"], ""),
					WL_COMPANYID = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYID"], ""),
					WL_NUMBER = YJT.DataBase.Common.ObjectTryToObj(item["WL_NUMBER"], ""),
					WL_COMPANYNAME = YJT.DataBase.Common.ObjectTryToObj(item["WL_COMPANYNAME"], ""),
					RECVNAME = YJT.DataBase.Common.ObjectTryToObj(item["RECVNAME"], ""),
					RECVPHONE = YJT.DataBase.Common.ObjectTryToObj(item["RECVPHONE"], ""),
					PROVINCENAME = YJT.DataBase.Common.ObjectTryToObj(item["PROVINCENAME"], ""),
					CITYNAME = YJT.DataBase.Common.ObjectTryToObj(item["CITYNAME"], ""),
					DISTRICTNAME = YJT.DataBase.Common.ObjectTryToObj(item["DISTRICTNAME"], ""),
					STREETNAME = YJT.DataBase.Common.ObjectTryToObj(item["STREETNAME"], ""),
					ORIGINALREMARK = YJT.DataBase.Common.ObjectTryToObj(item["ORIGINALREMARK"], ""),
					IsFp = YJT.DataBase.Common.ObjectTryToObj(item["IsFp"], ""),
					IsPj = YJT.DataBase.Common.ObjectTryToObj(item["IsPj"], ""),
					IsHeTong = YJT.DataBase.Common.ObjectTryToObj(item["IsHeTong"], ""),
					IsQysy = YJT.DataBase.Common.ObjectTryToObj(item["IsQysy"], ""),
					IsSyzz = YJT.DataBase.Common.ObjectTryToObj(item["IsSyzz"], ""),
					IsYjbb = YJT.DataBase.Common.ObjectTryToObj(item["IsYjbb"], ""),
					PlatformType = YJT.DataBase.Common.ObjectTryToObj(item["PlatformType"], Settings.Setings.EnumPlatformType.无),
					logi_dstRoute = YJT.DataBase.Common.ObjectTryToObj(item["logi_dstRoute"], ""),
					logi_PayType = YJT.DataBase.Common.ObjectTryToObj(item["logi_PayType"], ""),
					logi_monAccNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_monAccNum"], ""),
					logi_baojiaJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_baojiaJine"], ""),
					logi_dsJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_dsJine"], ""),
					logi_logcNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_logcNum"], ""),
					logi_ysJine = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJine"], ""),
					logi_ysJineTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_ysJineTotal"], ""),
					logi_shouhuory = YJT.DataBase.Common.ObjectTryToObj(item["logi_shouhuory"], ""),
					logi_jinjianRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_jinjianRiqi"], ""),
					logi_shoufqianshu = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufqianshu"], ""),
					logi_shoufRiqi = YJT.DataBase.Common.ObjectTryToObj(item["logi_shoufRiqi"], ""),
					logi_sendSheng = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendSheng"], ""),
					logi_sendShi = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendShi"], ""),
					logi_sendXian = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendXian"], ""),
					logi_sendAddress = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendAddress"], ""),
					logi_sendMan = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendMan"], ""),
					logi_sendPhone = YJT.DataBase.Common.ObjectTryToObj(item["logi_sendPhone"], ""),
					logi_feiyongTotal = YJT.DataBase.Common.ObjectTryToObj(item["logi_feiyongTotal"], ""),
					logi_goodQty = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodQty"], ""),
					logi_goodName = YJT.DataBase.Common.ObjectTryToObj(item["logi_goodName"], ""),
					needBaojia = YJT.DataBase.Common.ObjectTryToObj(item["needBaojia"], 0d),
					logi_OrderId = YJT.DataBase.Common.ObjectTryToObj(item["logi_OrderId"], ""),
					logi_CreateDate = YJT.DataBase.Common.ObjectTryToObj(item["logi_CreateDate"], ""),
					logi_ChanpinTypeStr = YJT.DataBase.Common.ObjectTryToObj(item["logi_ChanpinTypeStr"], ""),
					PrintDatetime = YJT.DataBase.Common.ObjectTryToObj(item["PrintDatetime"], ""),
					sysFirst = YJT.DataBase.Common.ObjectTryToObj(item["sysFirst"], ""),
					total_amt = YJT.DataBase.Common.ObjectTryToObj(item["total_amt"], "0"),
					RelBId = YJT.DataBase.Common.ObjectTryToObj(item["RelBId"], -1),
					fplx = YJT.DataBase.Common.ObjectTryToObj(item["fplx"], ""),
					ServerTaskType = YJT.DataBase.Common.ObjectTryToObj(item["ServerTaskType"], Settings.Setings.EnumServerTaskType.异常_类型转换失败),
					PAIDCOSTS = YJT.DataBase.Common.ObjectTryToObj(item["PAIDCOSTS"], 0d),
					logi_ReceivePwd = YJT.DataBase.Common.ObjectTryToObj(item["logi_ReceivePwd"], ""),
					logi_SubOrderSn = YJT.DataBase.Common.ObjectTryToObj(item["logi_SubOrderSn"], 0),
					logi_PhonNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_PhonNum"], ""),
					logi_TelNum = YJT.DataBase.Common.ObjectTryToObj(item["logi_TelNum"], ""),
					RelOrderId = YJT.DataBase.Common.ObjectTryToObj(item["RelOrderId"], -1L),
					JingdongWl = YJT.DataBase.Common.ObjectTryToObj(item["JingdongWl"], ""),
				};
				order.Status = orderstatus;

			}
			try
			{
				dt.Dispose();
				dt = null;
			}
			catch { }


			return order;
		}
		#endregion
	}
}
