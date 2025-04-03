using MOD;
using Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Windows.Forms;
using FastReport;
using LogisticsCore.JingDong;
using LogisticsCore.JingDong.Model;
using LogisticsCore.JingDong.Request;
using LogisticsCore.JingDong.Response;
using LogisticsCore.NewEMS;
using LogisticsCore.NewEMS.Model;
using LopOpensdkDotnet.Support;
using Newtonsoft.Json;
using SqlSugar;
using DbType = SqlSugar.DbType;
using LopOpensdkDotnet.Filters;
using LopOpensdkDotnet;
using LogisticsCore.NewEMS.Response;
using System.Text.RegularExpressions;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using YJT;
using static MOD.BllMod;
using RestSharp;
using static YJT.Logistics.ShenTongLogistic.ClassCreateOrder;
using System.Net.Http;
using System.Net.Sockets;
using static YJT.Logistics.ShunFengLogistic;

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
            catch (Exception ee)
            {
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

        private void button5_Click(object sender, EventArgs e)
        {
            //bool isOk;
            //string errMsg;
            //int errCode;
            //var db = new SqlSugarScope(GetConnectionConfig());
            //var db2 = BLL.Blll.init();
            //var order = db2.ServerGetOrder("54788", out isOk, out errCode, out errMsg);
            //order.Status = Setings.EnumOrderStatus.已获取电商信息;
            //order.Logic = Setings.EnumLogicType.Default;
            //var res = db2.ServerCreateLogic(order, out isOk, out errCode, out errMsg);
            var json = txtJson.Text;
            var ems = NewEms.Init(Settings.APITokenKey.NewEmsSenderNo, Settings.APITokenKey.NewEmsSignKey, APITokenKey.NewEmsAuthorization,
                APITokenKey.NewEmsTestSignKey, APITokenKey.NewEmsTestAuthorization, APITokenKey.NewEmsBaseUrl, APITokenKey.NewEmsUrl, APITokenKey.NewEmsTestUrl, true);

            ShowLog(JsonConvert.SerializeObject(ems, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }));

            var senderModel = ems.GetAddressModel("稠义路1号金汇化纤8楼左边大门", "江西省", "南昌市", "昌北区", "张三", "18178977225", "18178977225", "322000");
            var receiverModel = ems.GetAddressModel("裕华西路北国商城1楼", "河北省", "保定市", "莲池区", "李四", "13912345678", "13912345678", "071000");
            var cargo = ems.GetCargoModel();

            var emsOrder = ems.GetCreateOrderModel(senderModel, receiverModel, new[] { cargo }, Setings.EnumPlatformType.无.ToString(), "9999999", 4, 1.0, 1.0, 1.0,
                YJT.Text.ClassCreateText.FunStrCreateNumberStr(6), "无备注");

            ShowLog(JsonConvert.SerializeObject(emsOrder, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }));

            bool isOk;
            int errCode;
            string errMsg;
            string sendText;
            string resText;
            var res = ems.SendNewEmsOrder(emsOrder, out isOk, out errCode, out errMsg, out sendText, out resText);
            if (res is CreateOrderResponse)
            {
                var createOrderResponse = res as CreateOrderResponse;

                ShowLog(JsonConvert.SerializeObject(createOrderResponse, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }));

                txtWaybillNo.Text = createOrderResponse.retBodyObj.waybillNo;
                txtSendDate.Text = createOrderResponse.retDate;
                txtLogisticsOrderNo.Text = createOrderResponse.retBodyObj.logisticsOrderNo;
            }
            else
            {
                txtWaybillNo.Text = "CreateOrderResponse 解析错误";
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            bool isOk;
            int errCode;
            string errMsg;
            string sJson;
            string rJson;

            var ems = NewEms.Init(Settings.APITokenKey.NewEmsSenderNo, Settings.APITokenKey.NewEmsSignKey, APITokenKey.NewEmsAuthorization,
                APITokenKey.NewEmsTestSignKey, APITokenKey.NewEmsTestAuthorization, APITokenKey.NewEmsBaseUrl, APITokenKey.NewEmsUrl, APITokenKey.NewEmsTestUrl, true);
            ShowLog(JsonConvert.SerializeObject(ems, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }));

            var waybillNo = txtWaybillNo.Text;
            if (string.IsNullOrEmpty(waybillNo) || !Regex.IsMatch(waybillNo, @"^\d+$"))
            {
                MessageBox.Show("请输入正确的运单号");
                return;
            }
            if (string.IsNullOrEmpty(txtSendDate.Text) || !DateTime.TryParseExact(txtSendDate.Text, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime sendtime))
            {
                MessageBox.Show("请输入正确的日期");
                return;
            }

            var mod = ems.GetCancelOrderModel(txtLogisticsOrderNo.Text, waybillNo, DateTime.Now, out isOk, out errCode, out errMsg);

            ShowLog(JsonConvert.SerializeObject(mod, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }));

            NewEmsResponseBase res = ems.SendNewEmsOrder(mod, out isOk, out errCode, out errMsg, out sJson, out rJson);

            ShowLog(JsonConvert.SerializeObject(res, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }));
        }
        private void button14_Click(object sender, EventArgs e)
        {
            ShowLog(@"示例:|$4|efB6PnjDpgHG4xfrvYlXonyBuMJoGTynkfasopHvbl2u3nmNeP+rznA3DyRwb/2GeZL7I3rL6HKD5+Tv3Uy6x8jIDISbYG8Bg14caH2flYE=");
            var ems = NewEms.Init(Settings.APITokenKey.NewEmsSenderNo, Settings.APITokenKey.NewEmsSignKey, APITokenKey.NewEmsAuthorization,
                APITokenKey.NewEmsTestSignKey, APITokenKey.NewEmsTestAuthorization, APITokenKey.NewEmsBaseUrl, APITokenKey.NewEmsUrl, APITokenKey.NewEmsTestUrl, true);
            var res = ems.GetSignBySm4EncryptEcb(new LogisticsInterfaceBase());
            ShowLog(@"结果:" + res);
        }
        public class JsonClass
        {
            public string language { get; set; }
            public string orderId { get; set; }
            public DateTime createTime { get; set; }
        }
        private static ConnectionConfig GetConnectionConfig()
        {
            return new ConnectionConfig()
            {
                ConnectionString = @"Data Source=.;Initial Catalog=YanduECommerceAutomaticPrinting;Integrated Security=true",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                LanguageType = LanguageType.Chinese,
                MoreSettings = new ConnMoreSettings()
                {
                    DisableNvarchar = true,
                },
            };
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var report = new Report();
            report.Load(@"E:\WorkSpace\Source\Work_Project\面单打印模板\frx\Logic_ShenTong - 副本.frx");
            //re.Design();
            report.Show();
            report.Print();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var order = new CreateOrderModel();
            order.sender = new AddressModel();
            order.receiver = new AddressModel();
            var cargo = new CargoModel();
            order.cargos = new List<CargoModel>();
            order.cargos.Add(cargo);
            var json = JsonConvert.SerializeObject(order, new JsonSerializerSettings() { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            txtLog.Text = json;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var client = new DefaultClient("https://uat-api.jdl.com");
            // 系统参数，应用的app_ley和app_secret，可从【控制台-应用管理-概览】中查看；access_token是用户授权时获取的令牌，用户授权相关说明请查看https://cloud.jdl.com/#/devSupport/53392
            var isvFilter = new IsvFilter("0cdc29884bde4f978c48a3be1f473946", "f462cc68dd9d45e6a920bf3a818d9c1b", "d9b8c0c8beef451289cdda371e55b1ab");
            var errorResponseFilter = new ErrorResponseFilter();

            var request = new GenericRequest();
            // 对接方案编码，不同的对接方案取值不同，具体取值可在【控制台-应用管理-对接方案-编码】查看
            request.Domain = "FreshMedicineDelivery";
            // 接口调用地址，具体取值请看【接口文档-请求地址-调用路径（path）】
            request.Path = @"/freshmedicinedelivery/delivery/create/order/v1";
            // 固定是POST，并且是大写
            request.Method = "POST";

            // 请求报文，根据接口文档组织请求报文
            var body = new NoOrderNumberReceiveRequest
            {
                //orderId的值在下单成功后, 如果orderId不变, 地址即使改变不支持下单的地方, 也可以下单成功, 因为成功创建了京东订单, 只有取消后, 才能返回地址错误.
                orderId = "测试订单SSSS1234567890",
                senderContactRequest = new SenderContactModel
                {
                    senderName = "张三",
                    senderAddress = "河北省保定市中诚汇达",
                    senderCompany = null,
                    senderMobile = "13333333333"
                },
                customerCode = "010K2731958",
                remark = "测试备注",
                salePlatform = "0030001",
                cargoesRequest = new CargoesModel
                {
                    volume = 2.0,
                    goodsCount = 1,
                    weight = 1.0,
                    goodsName = "药品",
                    packageQty = 1
                },
                backContactRequest = null,
                shipmentEndTime = null,
                fileUrl = null,
                receiverContactRequest = new ReceiverContactModel
                {
                    receiverName = "李四",
                    receiverMobile = "19999999999",
                    receiverProvince = null,
                    receiverPostcode = null,
                    receiverCompany = null,
                    receiverCounty = null,
                    receiverCity = null,
                    receiverAddress = "北京市朝阳区11111号",
                    receiverCityName = null,
                    receiverTownName = null,
                    receiverProvinceName = null,
                    receiverTown = null,
                    receiverCountyName = null,
                    receiverOAID = null
                },
                shipmentStartTime = null,
                siteType = null,
                channelOrderId = null,
                shopCode = null,
                areaProvinceId = null,
                areaCityId = null,
                guaranteeValueAmount = null,
                guaranteeValue = null,
                pickUpStartTime = null,
                promiseTimeType = 29,
                receivable = null,
                goodsType = 24,
                receiptFlag = null,
                transType = null,
                pickUpEndTime = null,
                siteId = null,
                aging = null,
                addedService = null,
                boxNoList = null,
                customerBoxList = null
            };
            var bodys = new List<NoOrderNumberReceiveRequest> { body };
            //var bodyJson = JsonConvert.SerializeObject(wrapper,
            //    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented});

            var bodyJson = System.Text.Json.JsonSerializer.Serialize(bodys,
                new JsonSerializerOptions()
                { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
            Console.WriteLine(bodyJson);
            ShowLog(bodyJson);
            request.Body = Encoding.UTF8.GetBytes(bodyJson);

            request.AddFilter(isvFilter);
            request.AddFilter(errorResponseFilter);

            var options = new Options();

            var response = client.Execute(request, options);
            if (response != null)
            {
                var responseJson = JsonConvert.SerializeObject(response, Formatting.Indented);
                ShowLog(responseJson);
                if (response.Body != null)
                {
                    var decodedString = Encoding.UTF8.GetString(response.Body);
                    ShowLog(decodedString);
                }
            }

            ////response = (GenericResponse)response;
            //var responseJson = JsonConvert.SerializeObject(response, Formatting.Indented);
            //dynamic res = JsonConvert.DeserializeObject(responseJson);
            //ShowLog(responseJson);

            //if (res == null)
            //{
            //    ShowLog("Res 为空");
            //    return;
            //}
            //byte[] decodedBytes = Convert.FromBase64String(res.Body.ToString());
            //string decodedString = Encoding.UTF8.GetString(decodedBytes);
            //var jsonObject = JsonConvert.DeserializeObject<NoOrderNumberReceiveResponse>(decodedString);
            //var json = JsonConvert.SerializeObject(jsonObject);
            //ShowLog(json);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            var client = new DefaultClient("https://uat-api.jdl.com");
            // 系统参数，应用的app_ley和app_secret，可从【控制台-应用管理-概览】中查看；access_token是用户授权时获取的令牌，用户授权相关说明请查看https://cloud.jdl.com/#/devSupport/53392
            var isvFilter = new IsvFilter("0cdc29884bde4f978c48a3be1f473946", "f462cc68dd9d45e6a920bf3a818d9c1b", "d9b8c0c8beef451289cdda371e55b1ab");
            var errorResponseFilter = new ErrorResponseFilter();

            var request = new GenericRequest();
            // 对接方案编码，不同的对接方案取值不同，具体取值可在【控制台-应用管理-对接方案-编码】查看
            request.Domain = "FreshMedicineDelivery";
            // 接口调用地址，具体取值请看【接口文档-请求地址-调用路径（path）】
            request.Path = @"/freshmedicinedelivery/delivery/cancel/waybill/v1";
            // 固定是POST，并且是大写
            request.Method = "POST";

            // 请求报文，根据接口文档组织请求报文
            var body = new CancelOrderByVendorCodeAndDeliveryIdRequest
            {
                cancelOperator = null,
                cancelTime = null,
                cancelOperatorCodeType = null,
                customerCode = "010K2731958",
                interceptReason = "放弃订单",
                waybillNo = txtJdwlNumber.Text
            };
            var bodys = new List<CancelOrderByVendorCodeAndDeliveryIdRequest> { body };
            //var bodyJson = JsonConvert.SerializeObject(wrapper,
            //    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented});

            var bodyJson = System.Text.Json.JsonSerializer.Serialize(bodys,
                new JsonSerializerOptions()
                { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
            Console.WriteLine(bodyJson);
            ShowLog(bodyJson);
            request.Body = Encoding.UTF8.GetBytes(bodyJson);

            request.AddFilter(isvFilter);
            request.AddFilter(errorResponseFilter);

            var options = new Options();

            var response = client.Execute(request, options);
            if (response != null)
            {
                var responseJson = JsonConvert.SerializeObject(response, Formatting.Indented);
                ShowLog(responseJson);
                if (response.Body != null)
                {
                    var decodedString = Encoding.UTF8.GetString(response.Body);
                    ShowLog(decodedString);
                }
            }
        }
        public void ShowLog(string msg)
        {
            if (txtLog.Lines.Length > 1000)
            {
                txtLog.Clear();
            }

            if (txtLog.Lines.Length == 0)
            {
                txtLog.AppendText(msg);
            }
            else
            {
                txtLog.AppendText(Environment.NewLine + "=============================" + Environment.NewLine);
                txtLog.AppendText(msg);
            }

            txtLog.ScrollToCaret();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var client = new DefaultClient("https://uat-api.jdl.com");
            // 系统参数，应用的app_ley和app_secret，可从【控制台-应用管理-概览】中查看；access_token是用户授权时获取的令牌，用户授权相关说明请查看https://cloud.jdl.com/#/devSupport/53392
            var isvFilter = new IsvFilter("0cdc29884bde4f978c48a3be1f473946", "f462cc68dd9d45e6a920bf3a818d9c1b", "d9b8c0c8beef451289cdda371e55b1ab");
            var errorResponseFilter = new ErrorResponseFilter();

            var request = new GenericRequest();
            // 对接方案编码，不同的对接方案取值不同，具体取值可在【控制台-应用管理-对接方案-编码】查看
            request.Domain = "FreshMedicineDelivery";
            // 接口调用地址，具体取值请看【接口文档-请求地址-调用路径（path）】
            request.Path = APITokenKey.JdFreshMedicineDeliveryCheckOrderUrl;
            // 固定是POST，并且是大写
            request.Method = "POST";

            // 请求报文，根据接口文档组织请求报文
            var body = new RangeCheckDeliveryQueryApiRequest
            {
                orderId = "测试订单111111",
                customerCode = APITokenKey.JdFreshMdicineDeliveryCustomerCode,
                goodsType = 1,
                receiverContactRequest = new ReceiverContactRequest
                {
                    receiverAddress = "河北省保定市中诚汇达",
                    receiverCityName = null,
                    receiverProvince = null,
                    receiverCounty = null,
                    receiverTownName = null,
                    receiverCity = null,
                    receiverTown = null,
                    receiverProvinceName = null,
                    receiverCountyName = null
                },
                senderContactRequest = new SenderContactRequest
                {
                    senderProvince = null,
                    senderCity = null,
                    senderCounty = null,
                    senderTown = null,
                    senderAddress = "北京市朝阳区111111号",
                    senderProvinceName = null,
                    senderCityName = null,
                    senderCountyName = null,
                    senderTownName = null
                }
            };
            var bodys = new List<RangeCheckDeliveryQueryApiRequest> { body };
            //var bodyJson = JsonConvert.SerializeObject(wrapper,
            //    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented});

            var bodyJson = System.Text.Json.JsonSerializer.Serialize(bodys,
                new JsonSerializerOptions()
                { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
            Console.WriteLine(bodyJson);
            ShowLog(bodyJson);
            request.Body = Encoding.UTF8.GetBytes(bodyJson);

            request.AddFilter(isvFilter);
            request.AddFilter(errorResponseFilter);

            var options = new Options();

            var response = client.Execute(request, options);
            if (response != null)
            {
                var responseJson = JsonConvert.SerializeObject(response, Formatting.Indented);
                ShowLog(responseJson);
                if (response.Body != null)
                {
                    var decodedString = Encoding.UTF8.GetString(response.Body);
                    ShowLog(decodedString);
                    var res = JsonConvert.DeserializeObject<RangeCheckDeliveryQueryApiResponse>(decodedString);
                    if (res.HasData)
                    {
                        var logi_dstRoute = res.data.originalSortCenterId + "_" +
                                            res.data.originalSortCenterName + "-|-" +
                                            res.data.targetSortCenterId + "-" +
                                            res.data.targetSortCenterName;

                        var JingdongWl = res.data.aging + "@<|||>@\n" +
                                         res.data.agingName + "@<|||>@\n" +
                                         res.data.collectionAddress + "@<|||>@\n" +
                                         res.data.coverCode + "@<|||>@\n" +
                                         res.data.distributeCode + "@<|||>@\n" +
                                         res.data.isHideContractNumbers + "@<|||>@\n" +
                                         res.data.isHideName + "@<|||>@\n" +
                                         res.data.qrcodeUrl + "@<|||>@\n" +
                                         res.data.road + "@<|||>@\n" +
                                         res.data.siteId + "@<|||>@\n" +
                                         res.data.siteName + "@<|||>@\n" +
                                         res.data.siteType + "@<|||>@\n" +
                                         res.data.targetCrossCode + "@<|||>@\n" +
                                         res.data.originalCrossCode + "@<|||>@\n" +
                                         res.data.originalSortCenterId + "@<|||>@\n" +
                                         res.data.originalSortCenterName + "@<|||>@\n" +
                                         res.data.originalTabletrolleyCode + "@<|||>@\n" +
                                         res.data.targetSortCenterId + "@<|||>@\n" +
                                         res.data.targetSortCenterName + "@<|||>@\n" +
                                         res.data.targetTabletrolleyCode + "@<|||>@\n";
                        ShowLog(logi_dstRoute);
                        ShowLog(JingdongWl);
                    }
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            var isOk = false;
            var errCode = -99;
            var errMsg = "";

            var jdwl = FreshMedicineDelivery.Init("0cdc29884bde4f978c48a3be1f473946",
                "f462cc68dd9d45e6a920bf3a818d9c1b",
                "d9b8c0c8beef451289cdda371e55b1ab",
                "https://uat-api.jdl.com",
                "010K2731958");

            var request = new RangeCheckDeliveryQueryApiRequest
            {
                orderId = "测试订单111111",
                customerCode = APITokenKey.JdFreshMdicineDeliveryCustomerCode,
                goodsType = 1,
                receiverContactRequest = new ReceiverContactRequest
                {
                    receiverAddress = "北京市朝阳区111111号",
                    receiverCityName = null,
                    receiverProvince = null,
                    receiverCounty = null,
                    receiverTownName = null,
                    receiverCity = null,
                    receiverTown = null,
                    receiverProvinceName = null,
                    receiverCountyName = null
                },
                senderContactRequest = new SenderContactRequest
                {
                    senderProvince = null,
                    senderCity = null,
                    senderCounty = null,
                    senderTown = null,
                    senderAddress = "河北省保定市中诚汇达",
                    senderProvinceName = null,
                    senderCityName = null,
                    senderCountyName = null,
                    senderTownName = null
                }
            };
            var sendData = "";
            var recData = "";
            var res = jdwl.CheckOrder(request, out isOk, out errCode, out errMsg, out sendData, out recData);
            if (isOk == true && res.HasData)
            {
                var logi_dstRoute = res.data.originalSortCenterId + "_" +
                                    res.data.originalSortCenterName + "-|-" +
                                    res.data.targetSortCenterId + "-" +
                                    res.data.targetSortCenterName;

                var JingdongWl = res.data.aging + "@<|||>@\n" +
                                 res.data.agingName + "@<|||>@\n" +
                                 res.data.collectionAddress + "@<|||>@\n" +
                                 res.data.coverCode + "@<|||>@\n" +
                                 res.data.distributeCode + "@<|||>@\n" +
                                 res.data.isHideContractNumbers + "@<|||>@\n" +
                                 res.data.isHideName + "@<|||>@\n" +
                                 res.data.qrcodeUrl + "@<|||>@\n" +
                                 res.data.road + "@<|||>@\n" +
                                 res.data.siteId + "@<|||>@\n" +
                                 res.data.siteName + "@<|||>@\n" +
                                 res.data.siteType + "@<|||>@\n" +
                                 res.data.targetCrossCode + "@<|||>@\n" +
                                 res.data.originalCrossCode + "@<|||>@\n" +
                                 res.data.originalSortCenterId + "@<|||>@\n" +
                                 res.data.originalSortCenterName + "@<|||>@\n" +
                                 res.data.originalTabletrolleyCode + "@<|||>@\n" +
                                 res.data.targetSortCenterId + "@<|||>@\n" +
                                 res.data.targetSortCenterName + "@<|||>@\n" +
                                 res.data.targetTabletrolleyCode + "@<|||>@\n";
                ShowLog(logi_dstRoute);
                ShowLog(JingdongWl);
            }
            else
            {
                ShowLog(errMsg);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                return;
            try
            {
                ShowLog(ToCHN(double.Parse(textBox2.Text)));
            }
            catch (Exception ex)
            {
                ShowLog(ex.Message);
                textBox2.Text = "";
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                return;
            try
            {
                ShowLog(MoneyConverter.ToCHN(double.Parse(textBox2.Text)));
            }
            catch (Exception ex)
            {
                ShowLog(ex.Message);
                textBox2.Text = "";
            }
        }
        private string ToCHN(double money)
        {
            string s = money.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", delegate (Match m) { return "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万億兆京垓秭穰"[m.Value[0] - '-'].ToString(); });
        }

        private void button17_Click(object sender, EventArgs e)
        {
            var money = double.Parse(textBox2.Text);
            //money = 123.0051;
            decimal decimalMoney = (decimal)money;
            ShowLog("decimalMoney " + decimalMoney);

            long integerPart = (long)decimalMoney;
            var xiaoshu = (decimalMoney - integerPart);
            ShowLog("xiaoshu " + xiaoshu);
            int decimalPart = (int)Math.Round(xiaoshu * 100, MidpointRounding.AwayFromZero);
            ShowLog("xiaoshu * 100 " + xiaoshu * 100);
            ShowLog("整数部分" + integerPart.ToString() + "小数部分" + decimalPart.ToString());
            var intnum = 0.49999999999954525;
            var intnum2 = 0.509999999999877;
            ShowLog("double");
            ShowLog("0.49999999999954525 四舍五入 " + (int)Math.Round((double)intnum));
            ShowLog("0.509999999999877 四舍五入 " + (int)Math.Round((double)intnum2));
            ShowLog("decimal");
            ShowLog("0.49999999999954525 四舍五入 " + (int)Math.Round((decimal)intnum));
            ShowLog("0.509999999999877 四舍五入 " + (int)Math.Round((decimal)intnum2));
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //指定测试环境
            YJT.Logistics.ShunFengLogistic _sf = YJT.Logistics.ShunFengLogistic.Init("704", "ee6030e3f4f3f6eab5bca5f36a8ccf73");
            _sf.IsTest = true;
            var json = @"{
	  ""Bid"": 458415,
	  ""OrderId"": 275919,
	  ""AddDate"": ""2024-07-19 15:31:52"",
	  ""ErpId"": ""9103766"",
	  ""Logic"": 0,
	  ""Weight"": 1.23,
	  ""Length"": 1.0,
	  ""Width"": 1.0,
	  ""Height"": 1.0,
	  ""Status"": 20,
	  ""Ip"": ""172.16.2.150"",
	  ""Mac"": ""94C691F3D450"",
	  ""ComputerName"": ""DESKTOP-AU62CUP"",
	  ""ErrMsg"": ""获取WMS单据编号完成"",
	  ""ClientId"": 4,
	  ""PrintStatus"": 4,
	  ""WmsDanjbh"": ""4054150"",
	  ""NetDanjbh"": ""426129998"",
	  ""ClientHandleDate"": ""2024-07-18 15:34:28"",
	  ""ServerHandleDate"": ""2024-07-19 15:34:28"",
	  ""WmsYewy"": ""刘聪聪Y"",
	  ""WmsDdwname"": ""三河市新宝德堂药店"",
	  ""NETORDER_FROMID"": ""1"",
	  ""NETORDER_FROM"": ""药师帮"",
	  ""ERPORDER_ID"": ""9103766"",
	  ""CUSTOMID"": ""401205"",
	  ""CUSTOMNAME"": ""三河市新宝德堂药店"",
	  ""AGENTNAME"": ""刘聪聪Y"",
	  ""ADDRESS"": ""廊坊三河市燕郊开发区北欧小镇南商住楼C12号"",
	  ""WL_COMPANYID"": """",
	  ""WL_NUMBER"": """",
	  ""WL_COMPANYNAME"": """",
	  ""RECVNAME"": ""燕郊开发区学院街宝德堂药店王晓丽"",
	  ""RECVPHONE"": ""13785475853"",
	  ""PROVINCENAME"": ""河北省"",
	  ""CITYNAME"": ""廊坊市"",
	  ""DISTRICTNAME"": ""三河市"",
	  ""STREETNAME"": ""燕郊镇"",
	  ""ORIGINALREMARK"": ""配送到店加急资质带全。企业首营资料、23年度报告、1盒包邮 蒙奇 清血八味片0.5g*120片首营资质和药检报告"",
	  ""IsFp"": ""电子发票 "",
	  ""IsPj"": """",
	  ""IsHeTong"": """",
	  ""IsQysy"": ""企业首营 "",
	  ""IsSyzz"": ""首营资质 "",
	  ""IsYjbb"": ""药检 "",
	  ""PlatformType"": 1,
	  ""logi_dstRoute"": """",
	  ""logi_PayType"": """",
	  ""logi_monAccNum"": """",
	  ""logi_baojiaJine"": """",
	  ""logi_dsJine"": """",
	  ""logi_logcNum"": """",
	  ""logi_ysJine"": """",
	  ""logi_ysJineTotal"": """",
	  ""logi_shouhuory"": """",
	  ""logi_jinjianRiqi"": """",
	  ""logi_shoufqianshu"": """",
	  ""logi_shoufRiqi"": """",
	  ""logi_sendSheng"": """",
	  ""logi_sendShi"": """",
	  ""logi_sendXian"": """",
	  ""logi_sendAddress"": """",
	  ""logi_sendMan"": """",
	  ""logi_sendPhone"": """",
	  ""logi_feiyongTotal"": """",
	  ""logi_goodQty"": """",
	  ""logi_goodName"": """",
	  ""logi_OrderId"": """",
	  ""logi_CreateDate"": """",
	  ""needBaojia"": 0.0,
	  ""logi_ChanpinTypeStr"": """",
	  ""PrintDatetime"": """",
	  ""sysFirst"": """",
	  ""total_amt"": ""339.50"",
	  ""RelBId"": -1,
	  ""fplx"": ""普通发票"",
	  ""ServerTaskType"": 1,
	  ""PAIDCOSTS"": 0.0,
	  ""logi_ReceivePwd"": """",
	  ""logi_SubOrderSn"": 0,
	  ""logi_PhonNum"": """",
	  ""logi_TelNum"": """",
	  ""RelOrderId"": -1,
	  ""JingdongWl"": """"
	}";
            
            var order = JsonConvert.DeserializeObject<BllMod.Order>(json);

            var flag = false;
            var isOk = false;
            var errCode = -99;
            var errMsg = "";

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
                ShowLog("异常" + @"写入物流日志错误: " + ee.ToString());
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

        private void button19_Click(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {

            bool isOk;
            int errCode;
            string errMsg;
            YJT.Logistics.ShunFengLogistic _sf = YJT.Logistics.ShunFengLogistic.Init("704", "ee6030e3f4f3f6eab5bca5f36a8ccf73");

            YJT.Logistics.ShunFengLogistic.ClassGetOrderInfoRes res2 = null;
            
            res2 = _sf.GetOrderInfo(textBox3.Text, out isOk, out errCode, out errMsg);
            if (res2 != null && isOk == true && res2.data2 != null)
            {
                Console.WriteLine(JsonConvert.SerializeObject(res2));
            }
            else
            {
                Console.WriteLine(errMsg);
            }

        }

        private void button21_Click(object sender, EventArgs e)
        {
            var customerId = "704";
            var secret = "ee6030e3f4f3f6eab5bca5f36a8ccf73";
            //var customerId = "719";
            //var secret = "4a6919c248276e6ae6bea9925ac94525";
            var baseUrl = @"https://yqt-ms.sf-express.com";
            var testBaseUrl = @"https://yqt-ms.sit.sf-express.com";

            var getOrderInfoUrl = @"/hb-pcc-core/deliveryOrder/getSurfaceOrder";

            var _client = new RestClient(new RestClientOptions(testBaseUrl)
            {
                ConfigureMessageHandler = handler => new HttpClientHandler() { ServerCertificateCustomValidationCallback = delegate { return true; } },
                FailOnDeserializationError = true,
                ThrowOnAnyError = true,

            });
            ClassGetOrderInfo cgoi = new ClassGetOrderInfo();
            cgoi.customerId = customerId;
            cgoi.deliveryOrder = textBox3.Text;

            cgoi.timestamp = YJT.DateTimeHandle.DateConvert.GetUtcTimeStampNow("msec").ToString();
            cgoi.sign = CreateSign(customerId, secret, cgoi.timestamp);

            var json = JsonConvert.SerializeObject(cgoi, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
            });
            var restRequest = new RestRequest(getOrderInfoUrl, Method.Post);
            restRequest.AddHeader("Content-Type", "application/json");
            //restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //restRequest.AddParameter("customerId", cgoi.customerId);
            //restRequest.AddParameter("timestamp", cgoi.timestamp);
            //restRequest.AddParameter("sign", cgoi.sign);
            restRequest.AddJsonBody(json);


            try
            {
                var response = _client.Execute(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    ShowLog($"{JsonConvert.SerializeObject(response.Content, Formatting.Indented)}");
                }
                else
                {
                    ShowLog($"{response.StatusCode}");
                }
            }
            catch (HttpRequestException hre)
            {
                ShowLog($"{hre.ToString()}");
            }
        }
        private string CreateSign(string customerId, string secret, string timestamp)
        {
            string res = "";
            string key = customerId + "&sk=" + secret + "&timestamp=" + timestamp;
            byte[] sha512 = YJT.Encrypt.SHAEncrypt.EncryptSHA512Str.Init().FunByteGetSHA512StringAdv(key, "UTF-8");
            res = YJT.Encrypt.Base64.EncryptBase64UrlSafe.Init().EncryStrGetEnBase64UrlSafeByByte(sha512);
            return res;
        }
    }
}
