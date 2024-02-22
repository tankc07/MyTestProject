using api测试.Properties;
using MOD;
using Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace api测试
{
	
	public partial class Form1 : Form
	{
		struct LOGFONT
		{
			public int lfHeight;
			// Other fields omitted for brevity
		}
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(System.Drawing.Point point);
		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
		[DllImport("gdi32.dll")]
		static extern IntPtr CreateFontIndirect(LOGFONT lplf);
		[DllImport("gdi32.dll")]
		static extern bool DeleteObject(IntPtr hObject);
		const uint WM_SETFONT = 0x30;
		const int GWL_WNDPROC = -4;
		public Form1()
		{
			InitializeComponent();
		}
		IntPtr hwnd;
		private void timer1_Tick(object sender, EventArgs e)
		{
			try
			{
				System.Drawing.Point mousePos = new System.Drawing.Point();
				GetCursorPos(ref mousePos);
				hwnd = WindowFromPoint(mousePos);
				textBox1.Text = hwnd.ToString();
			}
			catch (Exception ee){
				textBox1.Text = ee.ToString();
			}
			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			timer1.Enabled = !timer1.Enabled;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			
		}

		private void button2_Click(object sender, EventArgs e)
		{
			System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(hwnd);
			System.Drawing.Font font = new System.Drawing.Font("Arial", 100);
			graphics.DrawString("Hello, World!", font, System.Drawing.Brushes.Black, new System.Drawing.PointF(0, 0));
		}

		private void button3_Click(object sender, EventArgs e)
		{
			//var a = Control.FromHandle(hwnd);
			//if (a != null)
			//{
			//	textBox1.Text = a.ToString();
			//	Form form = (Form)Control.FromHandle(hwnd);
			//	if (form == null)
			//	{
			//		form = Control.FromHandle(hwnd).FindForm();
			//	}
			//	if (form != null)
			//	{
			//		System.Drawing.Font font = new System.Drawing.Font("Arial", 100);
			//		form.Font = font;
			//	}
			//}

			FontDialog fd = new FontDialog();
			if (fd.ShowDialog() == DialogResult.OK)
			{
				Font font = fd.Font;

				//System.Drawing.Font font = new System.Drawing.Font("Arial",1,FontStyle.Bold);
				IntPtr hFont = font.ToHfont();
				SendMessage(hwnd, WM_SETFONT, hFont, hFont);
			}


			//{
			//	LOGFONT lo = new LOGFONT();
			//	lo.lfHeight = 100;
			//	int t1 = Marshal.SizeOf(lo);
			//	IntPtr hFon2 = Marshal.AllocHGlobal(t1);
			//	Marshal.StructureToPtr(lo, hFon2, true);
			//	SendMessage(hwnd, WM_SETFONT, hFon2, (IntPtr)(-1));
			//	//SetWindowLong(hwnd, GWL_WNDPROC, hFon2);
			//	//SetWindowLong(hwnd, GWL_WNDPROC, hFont);
			//}

			//{
			//	LOGFONT lf = new LOGFONT();
			//	lf.lfHeight = 100;
			//	IntPtr hFont = CreateFontIndirect(lf);

			//	SendMessage(hwnd, WM_SETFONT, hFont, (IntPtr)(-1));

			//	// Don't forget to delete the font handle when you're done
			//	DeleteObject(hFont);
			//}



		}
        static BLL.Blll bll = null;
        YJT.DataBase.DbHelper _dbhLocal = null;

        YJT.Logistics.ShunFengLogistic _sf = YJT.Logistics.ShunFengLogistic.Init(Settings.APITokenKey.ShunfengCustomerId, Settings.APITokenKey.ShunfengSecret);
        YJT.Logistics.YouZhengEms _emsYz = YJT.Logistics.YouZhengEms.Init(Settings.APITokenKey.EmsYouzhengSenderNo, Settings.APITokenKey.EmsYouzhengSecrect, Settings.APITokenKey.EmsYouzhengSecrectTest);
        YJT.Logistics.JingDongChunPeiLogistics _jdWl = YJT.Logistics.JingDongChunPeiLogistics.Init(Settings.APITokenKey.JingdongWlAppKey, Settings.APITokenKey.JingdongWlAppSecret, Settings.APITokenKey.JingdongWlToken, Settings.APITokenKey.JingdongWlAccessUrl, Settings.APITokenKey.JingdongWlDeptNo);
        YJT.Logistics.ZhongTongLogistics _ztkdWl = YJT.Logistics.ZhongTongLogistics.Init(Settings.APITokenKey.ZhongTongWlAppKey, Settings.APITokenKey.ZhongTongappSecret, Settings.APITokenKey.ZhongTongCustomid, Settings.APITokenKey.ZhongTongCustomPwd, Settings.APITokenKey.ZhongTongWlAppKeyTest, Settings.APITokenKey.ZhongTongappSecretTest, Settings.APITokenKey.ZhongTongIsTest);
        YJT.Logistics.ShenTongLogistic _shenTongWl = YJT.Logistics.ShenTongLogistic.Init(Settings.APITokenKey.ShenTongAppKey, Settings.APITokenKey.ShenTongSecretKey, Settings.APITokenKey.ShenTongResourceCode, Settings.APITokenKey.ShenTongFormOrderCode, Settings.APITokenKey.ShenTongSiteCode, Settings.APITokenKey.ShenTongCustomerName, Settings.APITokenKey.ShenTongSitePwd, Settings.APITokenKey.ShenTongIsTest);
        private void button4_Click(object sender, EventArgs e)
        {
            //bll = BLL.Blll.init();
            _dbhLocal = new YJT.DataBase.DbHelperSqlServer("172.16.1.15", "YanduECommerceAutomaticPrinting", "dsby", "dsby.", "3341");
            bool isOk;
            int errCode;
            string errMsg;
            var order = GetNeedServerHandle("172.16.2.150", "94C691F3D450", out isOk, out errCode, out errMsg);
            ServerStopLogic(order, out isOk, out errCode, out errMsg);
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
                    if (!System.IO.Directory.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}\WLLOG"))
                    {
                        System.IO.Directory.CreateDirectory($@"{AppDomain.CurrentDomain.BaseDirectory}\WLLOG");
                    }
                    System.IO.File.AppendAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\WLLOG\ShenTongCancel_{DateTime.Now.ToString("yyyyMMdd")}.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---------------------------------------------\r\n");
                    System.IO.File.AppendAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\WLLOG\ShenTongCancel_{DateTime.Now.ToString("yyyyMMdd")}.txt", "\t" + sendData + "\r\n");
                    System.IO.File.AppendAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\WLLOG\ShenTongCancel_{DateTime.Now.ToString("yyyyMMdd")}.txt", "返回-----------------------------------------------------------------\r\n");
                    System.IO.File.AppendAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\WLLOG\ShenTongCancel_{DateTime.Now.ToString("yyyyMMdd")}.txt", "\t" + recData + "\r\n");
                    if (isOk == true)
                    {
                        item.WL_NUMBER = "已删" + item.WL_NUMBER;
                        item.Status = Setings.EnumOrderStatus.停止完成;
                    }

                }
                catch (Exception ee)
                {
                    //AddMsgOut(ee.ToString(), Settings.Setings.EnumMessageType.异常, 0, "写入物流日志错误");
                    MessageBox.Show(ee.ToString());
                }

            }
        }

        public BllMod.Order GetNeedServerHandle(string ip, string mac, out bool isOk, out int errCode, out string errMsg)
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
    }
}
