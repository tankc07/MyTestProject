using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using MOD;
using Newtonsoft.Json;
using Settings;
using YJT;

namespace CenterServer
{
    class Program
    {
        static BLL.Blll bll = null;
        static System.Threading.Thread th = null;
        static System.Threading.Thread thFileDel = null;
        static bool thExit = false;
        static string _handIp = "";
        static string _handMac = "";
        static string _handMain = "";
        private static string _isDebugMode = null;
        private static string _isDebugPrint = null;
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            _isPause = false;
            //Console.ReadKey();
            _handIp = YJT.Text.ExtName.RemoveDbVarcharInvalid(YJT.ConfigTxt.AppConfigFIle.GetValueEX("handip", "", true), true);
            _handMac = YJT.Text.ExtName.RemoveDbVarcharInvalid(YJT.ConfigTxt.AppConfigFIle.GetValueEX("handmac", "", true), true);
            _handMain = YJT.Text.ExtName.RemoveDbVarcharInvalid(YJT.ConfigTxt.AppConfigFIle.GetValueEX("handMain", "否", true), true);
            _isDebugMode = YJT.Text.ExtName.RemoveDbVarcharInvalid(YJT.ConfigTxt.AppConfigFIle.GetValueEX("IsDebugMode", "false", true), true);
            _isDebugPrint = YJT.Text.ExtName.RemoveDbVarcharInvalid(YJT.ConfigTxt.AppConfigFIle.GetValueEX("IsDebugPrint", "false", true), true);
            ShowDebug($@"IsDebugMode: {_isDebugMode}, IsDebugPrint: {_isDebugPrint}", 2);
            if (YJT.Text.Verification.IsNullOrEmpty(_handIp) == true || YJT.Text.Verification.IsNullOrEmpty(_handMac) == true)
            {
                Console.WriteLine(@"配置文件不正确");
                Console.ReadKey();
                return;
            }
            //string a = "";
            //a=Settings.Configs.GetShunfPrinterName;
            //a=Settings.Configs.GetEmsYouzPrinterName;
            //a = Settings.Configs.GetSHTXPrinterName;
            //a = Settings.Configs.GetJingDongPrinterName;
            //a = Settings.Configs.GetZhongTongPrinterName;
            //a = Settings.Configs.GetShenTongPrinterName;
            //a = Settings.Configs.GetHeTongPrinterName;
            //a = Settings.Configs.GetFmPrinterName;

            //TODO:增加自动升级检查

            uint l = 0;
            if (YJT.MSystem.Common.MutexCheck("CenterServer" + _handIp + _handMac, out l, 1) == false)
            {
                Console.WriteLine(@"不能重复运行,按任意键退出");
                Console.ReadKey(false);
                return;
            }
            MOD.SysMod.ClinetTag ct = Common.PubMethod.GetClientTag();
            ShowDebug(JsonConvert.SerializeObject(ct), 2);
            if(_isDebugMode == "true")
                Console.ReadKey();
            YJT.StaticResources.Add("userObj", ct, true);
            YJT.StaticResources.Add("handObj", _handMac, true);
            Console.WriteLine(@"等待其他软件运行,期间等待60秒");
            //Modify: 修改时间: 2024-02-22 By:Ly 修改内容: 增加 "172.16.7.46" 跳过启动时的60秒倒计时.
            //如果是调试模式或者ip=数组中的ip,则跳过倒计时, 否则等待60秒, 如: ip=150,_isDebugMode=false, 则等待60秒, ip=150,_isDebugMode=true, 则跳过60秒
            if (new[] { "172.16.7.50", "172.16.7.46", "172.16.7.46|" }.Contains(ct.Ip) || _isDebugMode == "true")
            {
            }
            else
            {
                for (int i = 0; i < 60; i++)
                {
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("剩余:" + (60 - i).ToString() + "秒");
                    System.Threading.Thread.Sleep(1000);
                }
            }


            //Console.ReadKey();
            Console.Clear();
            Console.WriteLine(@"服务器启动");
            Console.WriteLine(@"如需退出,按 alt+S 键 不要直接关闭,否则可能会有未执行完成的单据");
            Console.WriteLine(@"-----------------------------------------------------------");
            YJT.BaiduService.BaiduMap.AccessKey = Settings.APITokenKey.BaiDuMapKey;

            if (ct == null)
            {
                Console.WriteLine(@"未能获取服务器信息,按任意键退出");
                Console.ReadKey(false);
                return;
            }

            BLL.Blll._clientInfoObj = ct;

            BLL.Blll.AddMsgOutEve += Blll_AddMsgOutEve;

            bll = BLL.Blll.init();
            if (bll == null)
            {
                Console.WriteLine(@"业务层初始化失败");
                Console.ReadKey(false);
                return;
            }

            //只有240服务器,即文件拉取服务器, 才能删除文件
            if (_handMain == "是" && ct.Ip.Contains("172.16.2.240"))
            {
                YJT.StaticResources.Add("canModifyPic", _handMain, true);
                thFileDel = new System.Threading.Thread(FileDelFun);
                thFileDel.IsBackground = true;
                thFileDel.Start();
            }
            th = new System.Threading.Thread(RunManTh);
            th.SetApartmentState(System.Threading.ApartmentState.STA);
            th.IsBackground = true;

            th.Start();
            ConsoleKeyInfo t = Console.ReadKey(true);
            while (true)
            {

                if (t.Modifiers == ConsoleModifiers.Alt)
                {
                    if (((int)t.KeyChar) == 115)
                    {

                        _isPause = true;
                        break;
                    }
                }
                Console.WriteLine(@"-----------------------------------------------------------");
                Console.WriteLine(@"如需退出,按 alt+S 键");
                Console.WriteLine(@"-----------------------------------------------------------");
                t = Console.ReadKey(true);
            }
            Console.WriteLine(@"退出...............");
            thExit = true;
            return;

        }
        /// <summary>
        /// 文件删除线程方法
        /// </summary>
        private static void FileDelFun()
        {
            while (true)
            {
                YJT.ValuePara.MulitValue.TwoValueClass<int, int, string> res = BLL.FileHandle.DelOldFile();
                if (res.V1 != res.V2)
                {
                    Blll_AddMsgOutEve(res.V3, Settings.Setings.EnumMessageType.异常, "FileHandl.FileDelFun", -1, res.V3, "", DateTime.Now);
                }
                System.Threading.Thread.Sleep(24 * 60 * 60 * 1000);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">一句话描述</param>
        /// <param name="type"></param>
        /// <param name="NS"></param>
        /// <param name="errCode"></param>
        /// <param name="errMsg">更细节一句话描述</param>
        /// <param name="parasList">详细参数</param>
        /// <param name="dt"></param>
        private static void Blll_AddMsgOutEve(string msg, Settings.Setings.EnumMessageType type, string NS, int errCode, string errMsg, string parasList, DateTime dt)
        {
            string outMsg = "";
            string outMsg2 = "";
            if (YJT.Text.Verification.IsNotNullOrEmpty(msg))
            {
                outMsg = msg + " (" + errCode.ToString() + ")\r\n\t" +
                NS + "\r\n\t" +
                errMsg.Replace("\r\n", "\r\n\t") +
                "\r\n" + parasList.Replace("\r\n", "\r\n\t") +
                "\r\n";
                outMsg2 = dt.ToString("yyyy-MM-dd HH:mm:ss") + "	[" + type.ToString() + "] " + outMsg + "\r\n";
            }
            else
            {
                outMsg2 = ("\r\n-------------------------------------------------------------\r\n" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
            }
            Console.WriteLine(outMsg2);
            System.IO.File.AppendAllText(@"D:\YdecapServerLog\" + DateTime.Now.ToString("yyyyMMdd") + @".txt", outMsg2);
        }
        static bool _isPause = false;
        [STAThreadAttribute]
        public static void RunManTh()
        {
            bool isOk = false;
            int errCode = 0;
            string errMsg = "";
            MOD.BllMod.Order orderThis = null;
            MOD.SysMod.ClinetTag thisServer = YJT.StaticResources.Get<MOD.SysMod.ClinetTag>("userObj");
            while (_isPause == false)
            {
                if (BLL.FileHandle.VerFtp(BLL.FileHandle.EnumFtpCompany.汇达ErpFtp))
                {

                    if (thisServer.Ip.Contains("172.16.7.50") || thisServer.Ip.Contains("172.16.7.46") || BLL.FileHandle.VerSerPicPath() || _isDebugMode == "true")
                    {

                        //修改
                        //Console.ReadKey();

                        System.Threading.Thread.Sleep(1000);
                        if (thExit == true)
                        {
                            break;
                        }

                        //修改
                        orderThis = bll.GetNeedServerHandle(_handIp, _handMac, out isOk, out errCode, out errMsg);

                        //强制false, 只获取当前运行服务端的IP生成的订单.
                        //if (thisServer.Ip.Contains("172.16.7.46") && _isDebugMode == "true")
                        //{
                        //    //测试用, 如果取不到IP是172.16.7.46的订单, 则拿取一个172.16.2.150生成的订单
                        //    //orderThis = bll.GetNeedServerHandle(_handIp, _handMac, out isOk, out errCode, out errMsg)
                        //    //             ?? bll.GetNeedServerHandle("172.16.2.150", "94C691F3D450", out isOk, out errCode, out errMsg);

                        //    orderThis = bll.GetNeedServerHandle(_handIp, _handMac, out isOk, out errCode, out errMsg)
                        //                ?? bll.GetNeedServerHandle2(out isOk, out errCode, out errMsg, "346553", Settings.Setings.EnumOrderStatus.已获取电商信息);
                        //    //if (orderThis != null)
                        //    //    orderThis.PrintStatus = Setings.EnumOrderPrintStatus.等待打印;
                        //}

                        ShowDebug(orderThis != null ? @"获取orderThis成功" : @"获取orderThis为null", orderThis != null ? 1 : 2);
                        //修改
                        //orderThis = bll.GetNeedServerHandle2(out isOk, out errCode, out errMsg, "121161", Settings.Setings.EnumOrderStatus.已回写电商平台);
                        //orderThis.PrintStatus = Settings.Setings.EnumOrderPrintStatus.已获取电商信息;
                        //orderThis.Status = Settings.Setings.EnumOrderStatus.已回写电商平台;
                        //orderThis.PrintStatus = Settings.Setings.EnumOrderPrintStatus.等待打印;



                        //orderThis.IsSyzz = "1";
                        //orderThis.IsHeTong = "333";
                        //orderThis.AGENTNAME = "刘聪聪1";
                        //orderThis.ErpId = "7401146";

                        //orderThis = bll.ServerGetOrder("72", out isOk, out errCode, out errMsg);
                        //orderThis.Status = Settings.Setings.EnumOrderStatus.关机任务;
                        //orderThis.PrintStatus = Settings.Setings.EnumOrderPrintStatus.等待打印;
                        //orderThis.WmsDanjbh = "1788874";
                        if (orderThis != null)
                        {
                            ShowDebug(@"获取到的orderThis不为空, 开始处理...", 1);
                            try
                            {
                                Blll_AddMsgOutEve("获取一张待处理单据", Settings.Setings.EnumMessageType.提示, Common.PubMethod.GetNameSpace(), 0, "", YJT.Json.FunStrSerializeToStr(orderThis), DateTime.Now);
                                switch (orderThis.Status)
                                {
                                    case Settings.Setings.EnumOrderStatus.准备停止:
                                        {

                                            if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.ErpId))
                                            {
                                                List<MOD.BllMod.Order> needStopOrders = bll.ServerGetOrderByErpid(orderThis, out isOk, out errCode, out errMsg);
                                                if (needStopOrders.Count > 0 && isOk == true)
                                                {
                                                    bool flag = false;
                                                    foreach (MOD.BllMod.Order item in needStopOrders)
                                                    {
                                                        if (item.ServerTaskType == Settings.Setings.EnumServerTaskType.新单据)
                                                        {
                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(item.WL_NUMBER))
                                                            {
                                                                if (!item.WL_NUMBER.StartsWith("已删"))
                                                                {
                                                                    bll.ServerStopLogic(item, out isOk, out errCode, out errMsg);
                                                                    if (isOk == true)
                                                                    {
                                                                        bll.ServerClearEcInfo(item, out isOk, out errCode, out errMsg);
                                                                        bll.ServerUpdateOrderAll(item, out isOk, out errCode, out errMsg);
                                                                        flag = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        Blll_AddMsgOutEve(item.Bid.ToString() + errMsg, Settings.Setings.EnumMessageType.异常, "停止订单", -99, "", "bid:" + orderThis.Bid, DateTime.Now);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (item.ServerTaskType == Settings.Setings.EnumServerTaskType.ERP组合单)
                                                        {

                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(item.WL_NUMBER))
                                                            {
                                                                if (!item.WL_NUMBER.StartsWith("已删"))
                                                                {
                                                                    bll.ServerStopLogic(item, out isOk, out errCode, out errMsg);
                                                                    if (isOk == true)
                                                                    {
                                                                        List<MOD.BllMod.Order> subOrders = bll.ServerGetOrderByRelOrderId(item.OrderId, item.ClientId, out isOk, out errCode, out errMsg);
                                                                        bll.ServerUpdateOrderAll(item, out isOk, out errCode, out errMsg);
                                                                        if (subOrders.Count > 0)
                                                                        {
                                                                            foreach (MOD.BllMod.Order item2 in subOrders)
                                                                            {
                                                                                bll.ServerClearEcInfo(item2, out isOk, out errCode, out errMsg);
                                                                                bll.ServerUpdateOrderAll(item2, out isOk, out errCode, out errMsg);
                                                                            }
                                                                        }
                                                                        flag = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        Blll_AddMsgOutEve(item.Bid.ToString() + errMsg, Settings.Setings.EnumMessageType.异常, "停止订单", -99, "", "bid:" + orderThis.Bid, DateTime.Now);
                                                                    }
                                                                }
                                                            }


                                                        }
                                                        else if (item.ServerTaskType == Settings.Setings.EnumServerTaskType.添加物流子单)
                                                        {
                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(item.WL_NUMBER))
                                                            {
                                                                if (!item.WL_NUMBER.StartsWith("已删"))
                                                                {
                                                                    bll.ServerStopLogic(item, out isOk, out errCode, out errMsg);
                                                                    if (isOk == true)
                                                                    {
                                                                        bll.ServerClearEcInfo(item, out isOk, out errCode, out errMsg);
                                                                        bll.ServerUpdateOrderAll(item, out isOk, out errCode, out errMsg);
                                                                        flag = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        Blll_AddMsgOutEve(item.Bid.ToString() + errMsg, Settings.Setings.EnumMessageType.异常, "停止订单", -99, "", "bid:" + orderThis.Bid, DateTime.Now);
                                                                    }
                                                                }
                                                            }
                                                        }


                                                    }
                                                    if (flag == false)
                                                    {
                                                        bll.ServerForceFinesh(orderThis.Bid, "没有成功停止单据:" + errMsg, Settings.Setings.EnumOrderStatus.异常_ERPID不正确, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg, null);
                                                    }
                                                    else
                                                    {
                                                        bll.ServerForceFinesh(orderThis.Bid, "停止完成", Settings.Setings.EnumOrderStatus.停止完成, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg, null);
                                                    }
                                                }
                                                else
                                                {
                                                    bll.ServerForceFinesh(orderThis.Bid, "没有要停止的单据", Settings.Setings.EnumOrderStatus.异常_ERPID不正确, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg, null);
                                                }
                                            }
                                            else
                                            {
                                                bll.ServerForceFinesh(orderThis.Bid, "编号为空", Settings.Setings.EnumOrderStatus.异常_ERPID不正确, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg, null);
                                            }
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.追加物流子单:
                                        {
                                            if (orderThis.RelBId > 0)
                                            {
                                                MOD.BllMod.Order oldOrder = bll.ServerGetOrder(orderThis.RelBId.ToString(), out isOk, out errCode, out errMsg);
                                                if (isOk == true)
                                                {
                                                    if (orderThis != null)
                                                    {
                                                        //获取单据ERP,判断物流信息,判断有多少个子单了;返回最新的ERP编号
                                                        orderThis = bll.ServerAddSubLogic(orderThis, oldOrder, out isOk, out errCode, out errMsg);
                                                        if (isOk == true)
                                                        {
                                                            PrintWL(orderThis, orderThis);
                                                            PrintSH(orderThis, orderThis);
                                                            PrintObj po = new PrintObj(orderThis.CUSTOMNAME, orderThis.CUSTOMID, orderThis.ErpId, orderThis.WmsYewy);
                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsHeTong))
                                                            {
                                                                List<MOD.HdErp.HdErpHeTong> t1 = null;
                                                                PrintHT(orderThis, orderThis, out t1);
                                                                if (t1 != null)
                                                                {
                                                                    po.HeTongs = t1;
                                                                }
                                                            }
                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsSyzz) || YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsPj))
                                                            {
                                                                List<MOD.ModGoodsInfo> t2 = null;
                                                                PrintPJ(orderThis, orderThis, out t2);
                                                                if (t2 != null)
                                                                {
                                                                    po.Pjs = t2;
                                                                }
                                                            }

                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsYjbb) || orderThis.AGENTNAME.Contains("刘聪聪1"))
                                                            {
                                                                List<MOD.ModGoodsInfo> t3 = null;
                                                                PrintLDYJ(orderThis, orderThis, out t3);
                                                                if (t3 != null)
                                                                {
                                                                    po.Ldyjs = t3;
                                                                }
                                                            }
                                                            if (
                                                                (po.HeTongs != null && po.HeTongs.Count > 0) ||
                                                                (po.Pjs != null && po.Pjs.Count > 0) ||
                                                                (po.Ldyjs != null && po.Ldyjs.Count > 0)
                                                            )
                                                            {
                                                                ComplatePrint(po, orderThis, orderThis);
                                                            }
                                                        }
                                                        //生成新的物流订单
                                                        //直接打印吧

                                                    }
                                                    else
                                                    {
                                                        //不可能,如果有这个情况,说明获取订单信息方法有错误
                                                    }
                                                }
                                                else
                                                {
                                                    bll.ServerForceFinesh(orderThis.Bid, "RELID:" + orderThis.RelBId.ToString() + "被关联单据错误:" + errMsg, Settings.Setings.EnumOrderStatus.异常_引用ID错误, Settings.Setings.EnumOrderPrintStatus.不可打印, out isOk, out errCode, out errMsg);
                                                }

                                            }
                                            else
                                            {
                                                bll.ServerForceFinesh(orderThis.Bid, "关联单据错误", Settings.Setings.EnumOrderStatus.异常_引用ID错误, Settings.Setings.EnumOrderPrintStatus.不可打印, out isOk, out errCode, out errMsg);
                                            }
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.ERP组合单:
                                        {
                                            if (orderThis.ErpId.Contains(","))
                                            {
                                                //验证单据是否正确
                                                string[] erpids = orderThis.ErpId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                if (erpids.Length > 0)
                                                {

                                                    List<MOD.BllMod.Order> subOrders = bll.ServerGetOrderByRelOrderId(orderThis.OrderId, orderThis.ClientId, out isOk, out errCode, out errMsg);
                                                    if (isOk == true && subOrders.Count > 0)
                                                    {
                                                        if (erpids.Length == subOrders.Count)
                                                        {
                                                            //回写相关ID
                                                            int count = bll.ServerWriteOrderRelBid(orderThis.Bid, subOrders, out isOk, out errCode, out errMsg);
                                                            if (count == subOrders.Count)
                                                            {
                                                                orderThis.ERPORDER_ID = orderThis.ErpId;
                                                                int i = 0;
                                                                foreach (MOD.BllMod.Order item in subOrders)
                                                                {
                                                                    //验证WMS
                                                                    bll.ServerGetWmsInfo2(item, out isOk, out errCode, out errMsg);
                                                                    if (isOk == true)
                                                                    {
                                                                        if (YJT.Text.Verification.IsNotNullOrEmpty(item.sysFirst))
                                                                        {
                                                                            orderThis.sysFirst = item.sysFirst;
                                                                        }
                                                                        //验证电商信息
                                                                        bll.ServerGetECInfo2(item, true, out isOk, out errCode, out errMsg);
                                                                        if (isOk == true)
                                                                        {
                                                                            bll.ServerCopyLogicInfo(orderThis, item, (i == 0 ? true : false), out isOk, out errCode, out errMsg);
                                                                            i++;
                                                                        }
                                                                        else
                                                                        {
                                                                            break;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        break;
                                                                    }

                                                                }
                                                                if (i == subOrders.Count)
                                                                {
                                                                    bll.ServerCreateLogic2(orderThis, out isOk, out errCode, out errMsg);
                                                                    if (isOk == true)
                                                                    {
                                                                        foreach (MOD.BllMod.Order item in subOrders)
                                                                        {
                                                                            bll.ServerCopyLogicInfo2(item, orderThis, false, out isOk, out errCode, out errMsg);
                                                                            bll.ServerEcBcLogicInfo(item, out isOk, out errCode, out errMsg);
                                                                            if (isOk == true)
                                                                            {
                                                                                item.Status = Settings.Setings.EnumOrderStatus.单据完成;
                                                                                item.ErrMsg = "完成";
                                                                                bll.ServerUpdateOrderAll(item, out isOk, out errCode, out errMsg);
                                                                                PrintSH(item, item);
                                                                                PrintObj po = new PrintObj(item.CUSTOMNAME, item.CUSTOMID, item.ErpId, item.WmsYewy);
                                                                                if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsHeTong))
                                                                                {
                                                                                    List<MOD.HdErp.HdErpHeTong> t1 = null;
                                                                                    PrintHT(item, item, out t1);
                                                                                    if (t1 != null)
                                                                                    {
                                                                                        po.HeTongs = t1;
                                                                                    }
                                                                                }
                                                                                if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsSyzz) || YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsPj))
                                                                                {
                                                                                    List<MOD.ModGoodsInfo> t2 = null;
                                                                                    PrintPJ(item, item, out t2);
                                                                                    if (t2 != null)
                                                                                    {
                                                                                        po.Pjs = t2;
                                                                                    }
                                                                                }

                                                                                if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsYjbb) || orderThis.AGENTNAME.Contains("刘聪聪1"))
                                                                                {
                                                                                    List<MOD.ModGoodsInfo> t3 = null;
                                                                                    PrintLDYJ(item, item, out t3);
                                                                                    if (t3 != null)
                                                                                    {
                                                                                        po.Ldyjs = t3;
                                                                                    }
                                                                                }
                                                                                if (
                                                                                    (po.HeTongs != null && po.HeTongs.Count > 0) ||
                                                                                    (po.Pjs != null && po.Pjs.Count > 0) ||
                                                                                    (po.Ldyjs != null && po.Ldyjs.Count > 0)
                                                                                )
                                                                                {
                                                                                    ComplatePrint(po, item, item);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                item.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
                                                                                item.ErrMsg = "回写电商细单数据失败:" + errMsg;
                                                                                bll.ServerUpdateOrderAll(item, out isOk, out errCode, out errMsg);
                                                                                Blll_AddMsgOutEve(item.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单ServerCreateLogic2", -99, "", "bid:" + orderThis.Bid, DateTime.Now);
                                                                            }
                                                                        }
                                                                        orderThis.Status = Settings.Setings.EnumOrderStatus.单据完成;
                                                                        orderThis.ErrMsg = "完成";
                                                                        bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                                        if (isOk == true)
                                                                        {
                                                                            PrintWL(orderThis, orderThis);
                                                                        }
                                                                        else
                                                                        {
                                                                            orderThis.Status = Settings.Setings.EnumOrderStatus.异常_写入失败;
                                                                            orderThis.ErrMsg = "更新主单数据失败:" + errMsg;
                                                                            bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                                            Blll_AddMsgOutEve(orderThis.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单ServerCreateLogic2", -99, "", "bid:" + orderThis.Bid, DateTime.Now);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        orderThis.Status = Settings.Setings.EnumOrderStatus.异常_物流下单不成功;
                                                                        orderThis.ErrMsg = "组合单物流生成" + errMsg;
                                                                        bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                                        Blll_AddMsgOutEve(orderThis.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单ServerCreateLogic2", -99, "", "bid:" + orderThis.Bid, DateTime.Now);
                                                                    }
                                                                    //获取物流
                                                                    //分组回写电商
                                                                    //分组打印
                                                                }
                                                                else
                                                                {
                                                                    orderThis.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
                                                                    orderThis.ErrMsg = "明细校验失败" + errMsg;
                                                                    bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                                    Blll_AddMsgOutEve(orderThis.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单", -99, "", "bid:" + orderThis.Bid, DateTime.Now);

                                                                }
                                                            }
                                                            else
                                                            {
                                                                orderThis.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
                                                                orderThis.ErrMsg = "回写的子单数量不一致";
                                                                bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                                Blll_AddMsgOutEve(orderThis.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单ServerWriteOrderRelBid", -99, "", "bid:" + orderThis.Bid, DateTime.Now);

                                                            }
                                                        }
                                                        else
                                                        {
                                                            orderThis.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
                                                            orderThis.ErrMsg = "子单数量与总单编号数量不一致";
                                                            bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                            Blll_AddMsgOutEve(orderThis.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单ServerGetOrderByRelOrderId", -99, "", "bid:" + orderThis.Bid, DateTime.Now);

                                                        }

                                                    }
                                                    else
                                                    {
                                                        orderThis.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
                                                        orderThis.ErrMsg = "服务器未能获取到子单','";
                                                        bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                        Blll_AddMsgOutEve(orderThis.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单ServerGetOrderByRelOrderId", -99, "", "bid:" + orderThis.Bid, DateTime.Now);


                                                    }

                                                }
                                                else
                                                {
                                                    orderThis.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
                                                    orderThis.ErrMsg = "单据不包含分隔符','";
                                                    bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                    Blll_AddMsgOutEve(orderThis.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单", -99, "", "bid:" + orderThis.Bid, DateTime.Now);
                                                }


                                            }
                                            else
                                            {
                                                orderThis.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
                                                orderThis.ErrMsg = "单据不包含分隔符','";
                                                bll.ServerUpdateOrderAll(orderThis, out isOk, out errCode, out errMsg);
                                                Blll_AddMsgOutEve(orderThis.Bid.ToString() + orderThis.ErrMsg, Settings.Setings.EnumMessageType.异常, "组合单", -99, "", "bid:" + orderThis.Bid, DateTime.Now);
                                            }
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.关机任务:
                                        {
                                            _isPause = true;
                                            bll.ServerForceFinesh(orderThis.Bid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "关机", Settings.Setings.EnumOrderStatus.关机完成, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg);
                                            System.Diagnostics.Process.Start("c:/windows/system32/shutdown.exe", "-f -s -t " + 0);
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.已下传:
                                        {
                                            //处理刚下传的单据,从WMS中开始获取信息
                                            orderThis = bll.ServerGetWmsInfo(orderThis, out isOk, out errCode, out errMsg);
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.已获取WMS信息:
                                        {
                                            //已经获取了WMS信息,准备从电商接口服务器上获取信息
                                            orderThis = bll.ServerGetECInfo(orderThis, out isOk, out errCode, out errMsg);
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.已获取电商信息:
                                        {
                                            //电商信息获取完毕,准备为单据创建物流订单
                                            orderThis = bll.ServerCreateLogic(orderThis, out isOk, out errCode, out errMsg);
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.已获取物流单号:
                                        {
                                            //物流订单创建完成,准备从物流接口中获取面单信息,目的为了打印
                                            orderThis = bll.ServerLogicInfo(orderThis, out isOk, out errCode, out errMsg);
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.已回写电商平台:
                                        {
                                            //面单信息已经获取完毕,准备进入打印阶段
                                            if (orderThis.PrintStatus == Settings.Setings.EnumOrderPrintStatus.等待打印)
                                            {
                                                //只能有等待打印的情况,因为电商平台写入完成后,必然要打印随货通行以及物流订单


                                                //修改
                                                PrintSH(orderThis, orderThis);
                                                PrintWL(orderThis, orderThis);
                                                PrintObj po = new PrintObj(orderThis.CUSTOMNAME, orderThis.CUSTOMID, orderThis.ErpId, orderThis.WmsYewy);
                                                if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsHeTong))
                                                {
                                                    List<MOD.HdErp.HdErpHeTong> t1 = null;
                                                    PrintHT(orderThis, orderThis, out t1);
                                                    if (t1 != null)
                                                    {
                                                        po.HeTongs = t1;
                                                    }
                                                }
                                                if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsSyzz) || YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsPj))
                                                {
                                                    List<MOD.ModGoodsInfo> t2 = null;
                                                    PrintPJ(orderThis, orderThis, out t2);
                                                    if (t2 != null)
                                                    {
                                                        po.Pjs = t2;
                                                    }
                                                }

                                                if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsYjbb) || orderThis.AGENTNAME.Contains("刘聪聪1"))
                                                {
                                                    List<MOD.ModGoodsInfo> t3 = null;
                                                    PrintLDYJ(orderThis, orderThis, out t3);
                                                    if (t3 != null)
                                                    {
                                                        po.Ldyjs = t3;
                                                    }
                                                }
                                                if (
                                                    (po.HeTongs != null && po.HeTongs.Count > 0) ||
                                                    (po.Pjs != null && po.Pjs.Count > 0) ||
                                                    (po.Ldyjs != null && po.Ldyjs.Count > 0)
                                                )
                                                {
                                                    ComplatePrint(po, orderThis, orderThis);
                                                }
                                                //fr.LoadPrintFrx( @"")
                                                //fr.Design("");
                                            }
                                            else
                                            {
                                                bll.ServerForceFinesh(orderThis.Bid, "此单据无需打印", Settings.Setings.EnumOrderStatus.单据完成, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg);
                                            }
                                            break;
                                        }
                                    case Settings.Setings.EnumOrderStatus.补打任务:
                                        {
                                            MOD.BllMod.Order relOrder = null;
                                            if (orderThis.RelBId > -1)
                                            {
                                                relOrder = bll.ServerGetOrder(orderThis.RelBId.ToString(), out isOk, out errCode, out errMsg);
                                                if (relOrder != null)
                                                {
                                                    if (relOrder.ErpId == orderThis.ErpId)
                                                    {
                                                        if (orderThis.PrintStatus == Settings.Setings.EnumOrderPrintStatus.等待打印)
                                                        {
                                                        }
                                                        else if (orderThis.PrintStatus == Settings.Setings.EnumOrderPrintStatus.等待补打物流)
                                                        {
                                                            PrintWL(relOrder, orderThis);
                                                        }
                                                        else if (orderThis.PrintStatus == Settings.Setings.EnumOrderPrintStatus.等待补打随货)
                                                        {
                                                            orderThis.WmsDanjbh = relOrder.WmsDanjbh;
                                                            PrintSH(relOrder, orderThis);
                                                            PrintObj po = new PrintObj(relOrder.CUSTOMNAME, relOrder.CUSTOMID, relOrder.ErpId, relOrder.WmsYewy);
                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsHeTong))
                                                            {
                                                                List<MOD.HdErp.HdErpHeTong> t1 = null;
                                                                PrintHT(orderThis, orderThis, out t1);
                                                                if (t1 != null)
                                                                {
                                                                    po.HeTongs = t1;
                                                                }
                                                            }
                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsSyzz) || YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsPj))
                                                            {
                                                                List<MOD.ModGoodsInfo> t2 = null;
                                                                PrintPJ(orderThis, orderThis, out t2);
                                                                if (t2 != null)
                                                                {
                                                                    po.Pjs = t2;
                                                                }
                                                            }

                                                            if (YJT.Text.Verification.IsNotNullOrEmpty(orderThis.IsYjbb) || orderThis.AGENTNAME.Contains("刘聪聪1"))
                                                            {
                                                                List<MOD.ModGoodsInfo> t3 = null;
                                                                PrintLDYJ(orderThis, orderThis, out t3);
                                                                if (t3 != null)
                                                                {
                                                                    po.Ldyjs = t3;
                                                                }
                                                            }
                                                            if (
                                                                (po.HeTongs != null && po.HeTongs.Count > 0) ||
                                                                (po.Pjs != null && po.Pjs.Count > 0) ||
                                                                (po.Ldyjs != null && po.Ldyjs.Count > 0)
                                                            )
                                                            {
                                                                ComplatePrint(po, relOrder, orderThis);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        bll.ServerForceFinesh(orderThis.Bid, "要补打的单据编号与下传时不一致", Settings.Setings.EnumOrderStatus.异常_引用ID错误, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg);
                                                    }


                                                }
                                                else
                                                {
                                                    bll.ServerForceFinesh(orderThis.Bid, "引用的对象并非记录集中的元素", Settings.Setings.EnumOrderStatus.异常_引用ID错误, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg);
                                                }

                                            }
                                            else
                                            {
                                                bll.ServerForceFinesh(orderThis.Bid, "引用的对象不存在", Settings.Setings.EnumOrderStatus.异常_引用ID错误, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out isOk, out errCode, out errMsg);
                                            }


                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            catch (Exception ee)
                            {
                                ShowDebug($@"处理orderThis过程出现异常, Exception: {ee}", 3);
                                System.IO.File.AppendAllText(@"D:\YdecapServerLog\" + DateTime.Now.ToString("yyyyMMdd") + "Exception.txt", "-------------------------------------------------------------------------------\r\n");
                                System.IO.File.AppendAllText(@"D:\YdecapServerLog\" + DateTime.Now.ToString("yyyyMMdd") + "Exception.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
                                System.IO.File.AppendAllText(@"D:\YdecapServerLog\" + DateTime.Now.ToString("yyyyMMdd") + "Exception.txt", ee.ToString());
                            }



                        }
                        else
                        {
                            Console.WriteLine(@"未获取到要处理的单据,等待30秒后重新获取");
                            YJT.MSystem.GC.Collect();
                            ShowDebug(@"清理GC完成", 2);
                            ShowDebug(@"未获取到要处理的单据,(orderThis=null),等待30秒", 1);
                            System.Threading.Thread.Sleep(1000 * 30);
                            ShowDebug(@"等待30秒已完成.", 1);
                        }
                    }
                    else
                    {
                        Blll_AddMsgOutEve(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":图片服务器共享未开启", Settings.Setings.EnumMessageType.严重, "检查", -99, "图片服务器共享未开启", "", DateTime.Now);
                        _isPause = true;
                    }

                }
                else
                {
                    Blll_AddMsgOutEve(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":汇达FTP未开启", Settings.Setings.EnumMessageType.严重, "检查", -99, "汇达FTP未开启", "", DateTime.Now);
                    _isPause = true;
                }

            }
        }

        private static void ComplatePrint(PrintObj po, MOD.BllMod.Order printObj, MOD.BllMod.Order updateObj)
        {
            bool isok = false;
            int errCode = 0;
            string errMsg = "";
            string printContentType = "";
            int totalPage = 0;
            int hetongPages = 0;
            string printRiqi = DateTime.Now.ToString("yyyy-MM-dd");
            //获取封面信息
            bll.GetComplatePrintInfo(po.ErpId, ref po, out isok, out errCode, out errMsg);

            //检查合同文件


            if (po.HeTongs != null && po.HeTongs.Count > 0)
            {


                hetongPages = (int)(po.HeTongs.Count / 9);
                hetongPages += (po.HeTongs.Count % 9) > 0 ? 1 : 0;
                printContentType = printContentType + $"购销合同 共计页数:{hetongPages.ToString()} \r\n";
                totalPage += hetongPages;
            }
            //检查批件
            int pjPages = 0;
            if (po.Pjs != null && po.Pjs.Count > 0)
            {
                foreach (var item in po.Pjs)
                {
                    if (item.GoodsPic != null && item.GoodsPic.Count > 0)
                    {
                        foreach (var item2 in item.GoodsPic)
                        {
                            if (System.IO.File.Exists(item2.LocalPath))
                            {
                                pjPages++;
                            }
                        }
                    }
                }
                printContentType = printContentType + $"药品批件 共计页数:{pjPages.ToString()} \r\n";
                totalPage += pjPages;
            }


            int ldyjPages = 0;
            if (po.Ldyjs != null && po.Ldyjs.Count > 0)
            {
                foreach (var item in po.Ldyjs)
                {
                    if (item.GoodsPic != null && item.GoodsPic.Count > 0)
                    {
                        foreach (var item2 in item.GoodsPic)
                        {
                            if (System.IO.File.Exists(item2.LocalPath))
                            {
                                ldyjPages++;
                            }
                        }
                    }
                }
                printContentType = printContentType + $"路单药检 共计页数:{ldyjPages.ToString()} \r\n";
                totalPage += ldyjPages;
            }






            //打印封面
            string fmPath = YJT.Path.FunStrGetLocalPath() + "\\frx\\HdGoodFM.frx";
            if (totalPage > 0)
            {
                if (System.IO.File.Exists(fmPath))
                {
                    frFM.LoadPrintFrx(fmPath);
                    frFM.AddMod<PrintObj>(po);
                    frFM.AddValue("totalPage", totalPage);
                    frFM.AddValue("printContentType", printContentType);
                    frFM.AddValue("hetongPages", hetongPages);
                    frFM.AddValue("pjPages", pjPages);
                    frFM.AddValue("printRiqi", printRiqi);
                    //修改
                    //if (1 == 1 || BLL.Blll._clientInfoObj.Ip == "172.16.7.50")
                    if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                    {
                        //frXsht.Print(false, "Microsoft XPS Document Writer", updateOrder);
                        frFM.Design("asdf");
                    }
                    else
                    {
                        frFM.Print(false, Settings.Configs.GetFmPrinterName, updateObj);
                    }
                }
                else
                {
                    //封面打印方案不存在
                }
            }




            //打印合同
            if (hetongPages > 0)
            {
                fmPath = YJT.Path.FunStrGetLocalPath() + @"\frx\HdXSHT.frx";
                if (System.IO.File.Exists(fmPath))
                {
                    frXsht.LoadPrintFrx(fmPath);
                    frXsht.AddMod(printObj);
                    System.Data.DataTable dt = YJT.DataTableHandle.ListToClass.ToDataTable<MOD.HdErp.HdErpHeTong>(po.HeTongs);
                    frXsht.AddDataTable("DetailData", dt);
                    //修改
                    //if (1==1 || BLL.Blll._clientInfoObj.Ip == "172.16.7.50")
                    if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                    {
                        //frXsht.Print(false, "Microsoft XPS Document Writer", updateOrder);
                        frXsht.Design("asdf");
                    }
                    else
                    {
                        frXsht.Print(false, Settings.Configs.GetHeTongPrinterName, updateObj);
                    }
                    frXsht.RemoveDataTable("DetailData");
                }
            }




            //打印批件
            if (pjPages > 0)
            {
                fmPath = YJT.Path.FunStrGetLocalPath() + @"\frx\HdHPZZ.frx";
                if (System.IO.File.Exists(fmPath))
                {
                    frHdHpzz.LoadPrintFrx(fmPath);
                    System.Data.DataTable tab1 = new System.Data.DataTable();
                    System.Data.DataColumn dc = new System.Data.DataColumn("picpath", typeof(System.Drawing.Image));
                    System.Data.DataColumn w = new System.Data.DataColumn("width", typeof(int));
                    System.Data.DataColumn h = new System.Data.DataColumn("height", typeof(int));
                    System.Data.DataColumn tableTitle = new System.Data.DataColumn("tableTitle", typeof(string));
                    tab1.Columns.Add(dc);
                    tab1.Columns.Add(w);
                    tab1.Columns.Add(h);
                    tab1.Columns.Add(tableTitle);

                    foreach (var item in po.Pjs)
                    {

                        int i = 1;
                        foreach (var item2 in item.GoodsPic)
                        {
                            if (System.IO.File.Exists(item2.LocalPath))
                            {
                                try
                                {
                                    Image im = Image.FromFile(item2.LocalPath);
                                    int ww = im.Width;
                                    int hh = im.Height;
                                    int ww_Expect = 1240;
                                    int hh_Expect = 1754;
                                    Image otim = new Bitmap(ww_Expect, hh_Expect);
                                    Graphics gh = Graphics.FromImage(otim);
                                    gh.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                                    gh.DrawImage(im, 0, 0, ww_Expect, hh_Expect);
                                    gh.Dispose();
                                    gh = null;
                                    //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                                    //otim.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    System.Data.DataRow dr = tab1.NewRow();
                                    dr["picpath"] = otim;
                                    dr["width"] = ww;
                                    dr["height"] = hh;
                                    dr["tableTitle"] = "首营资质 ErpId:" + po.ErpId + " 品名:" + item.GoodsName + " 计数:" + i.ToString();
                                    tab1.Rows.Add(dr);
                                    i++;
                                }
                                catch
                                {
                                    //有个图片不能打印
                                }

                            }
                            if (tab1.Rows.Count >= 5)
                            {
                                frHdHpzz.AddDataTable("lists", tab1);
                                //修改
                                //if (1 == 1 ||  BLL.Blll._clientInfoObj.Ip == "172.16.7.50")
                                if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                                {
                                    //frXsht.Print(false, "Microsoft XPS Document Writer", updateOrder);
                                    frHdHpzz.Design("asdf");
                                }
                                else
                                {
                                    frHdHpzz.Print(false, Settings.Configs.GetHeTongPrinterName, updateObj);
                                }
                                tab1.Rows.Clear();
                            }
                        }


                    }
                    if (tab1.Rows.Count > 0)
                    {
                        frHdHpzz.AddDataTable("lists", tab1);
                        //修改
                        //if (1 == 1 ||  BLL.Blll._clientInfoObj.Ip == "172.16.7.50"  || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                        if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50")
                        {
                            //frXsht.Print(false, "Microsoft XPS Document Writer", updateOrder);
                            frHdHpzz.Design("asdf");
                        }
                        else
                        {
                            frHdHpzz.Print(false, Settings.Configs.GetHeTongPrinterName, updateObj);
                        }
                        tab1.Rows.Clear();
                    }
                    try
                    {
                        tab1.Dispose();
                    }
                    catch { }
                    tab1 = null;
                }

            }




            //打印路单药检
            if (ldyjPages > 0)
            {
                fmPath = YJT.Path.FunStrGetLocalPath() + @"\frx\WmsHPLDYJ.frx";
                if (System.IO.File.Exists(fmPath))
                {
                    frWmsLdyj.LoadPrintFrx(fmPath);
                    System.Data.DataTable tab1 = new System.Data.DataTable();
                    System.Data.DataColumn dc = new System.Data.DataColumn("picpath", typeof(System.Drawing.Image));
                    System.Data.DataColumn w = new System.Data.DataColumn("width", typeof(int));
                    System.Data.DataColumn h = new System.Data.DataColumn("height", typeof(int));
                    System.Data.DataColumn tableTitle = new System.Data.DataColumn("tableTitle", typeof(string));
                    tab1.Columns.Add(dc);
                    tab1.Columns.Add(w);
                    tab1.Columns.Add(h);
                    tab1.Columns.Add(tableTitle);
                    foreach (var item in po.Ldyjs)
                    {
                        int i = 1;
                        foreach (var item2 in item.GoodsPic)
                        {
                            if (System.IO.File.Exists(item2.LocalPath))
                            {
                                try
                                {
                                    Image im = Image.FromFile(item2.LocalPath);
                                    int ww = im.Width;
                                    int hh = im.Height;
                                    int ww_Expect = 1240;
                                    int hh_Expect = 1754;
                                    Image otim = new Bitmap(ww_Expect, hh_Expect);
                                    Graphics gh = Graphics.FromImage(otim);
                                    gh.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                                    gh.DrawImage(im, 0, 0, ww_Expect, hh_Expect);
                                    gh.Dispose();
                                    gh = null;
                                    //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                                    //otim.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    System.Data.DataRow dr = tab1.NewRow();
                                    dr["picpath"] = otim;
                                    dr["width"] = ww;
                                    dr["height"] = hh;
                                    dr["tableTitle"] = "路单药检 ErpId:" + po.ErpId + " 品名:" + item.GoodsName + " 计数:" + i.ToString();
                                    tab1.Rows.Add(dr);
                                    i++;
                                }
                                catch
                                {
                                    //有个图片不能打印
                                }

                            }
                            if (tab1.Rows.Count >= 5)
                            {
                                frWmsLdyj.AddDataTable("lists", tab1);
                                //修改
                                //if (1==1 || BLL.Blll._clientInfoObj.Ip == "172.16.7.50")
                                if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                                {
                                    //frXsht.Print(false, "Microsoft XPS Document Writer", updateOrder);
                                    frWmsLdyj.Design("asdf");
                                }
                                else
                                {
                                    frWmsLdyj.Print(false, Settings.Configs.GetHeTongPrinterName, updateObj);
                                }
                                tab1.Rows.Clear();
                            }
                        }
                    }
                    if (tab1.Rows.Count > 0)
                    {
                        frWmsLdyj.AddDataTable("lists", tab1);
                        //修改
                        //if (1==1 || BLL.Blll._clientInfoObj.Ip == "172.16.7.50")
                        if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                        {
                            //frXsht.Print(false, "Microsoft XPS Document Writer", updateOrder);
                            frWmsLdyj.Design("asdf");
                        }
                        else
                        {
                            frWmsLdyj.Print(false, Settings.Configs.GetHeTongPrinterName, updateObj);
                        }
                        tab1.Rows.Clear();
                    }
                    try
                    {
                        tab1.Dispose();
                    }
                    catch { }
                    tab1 = null;

                }

            }

        }

        static YJT.Print.FastReport _shtxFr = new YJT.Print.FastReport((printObj) =>
        {
            bool isOk2 = false;
            int errCode2 = 0;
            string errMsg2 = "";
            if (printObj != null)
            {
                YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, MOD.SysMod.PrintDataM> keyvp = printObj as YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, MOD.SysMod.PrintDataM>;
                if (keyvp != null)
                {
                    MOD.BllMod.Order order2 = keyvp.V1;
                    MOD.SysMod.PrintDataM wmsInfo = keyvp.V2;
                    if (order2 != null)
                    {
                        if (order2.Status == Settings.Setings.EnumOrderStatus.补打任务)
                        {
                            order2.Status = Settings.Setings.EnumOrderStatus.补打完成;
                            if (order2.PrintStatus == Settings.Setings.EnumOrderPrintStatus.等待补打物流)
                            {
                                order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.补打物流单完成;
                                order2.ErrMsg = order2.PrintStatus.ToString();
                            }
                            else
                            {
                                order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.补打随货单完成;
                                order2.ErrMsg = order2.PrintStatus.ToString();
                            }

                        }
                        else if (order2.ServerTaskType == Settings.Setings.EnumServerTaskType.补打物流子单)
                        {
                            order2.Status = Settings.Setings.EnumOrderStatus.补打完成;
                            order2.ErrMsg = "完成";
                            order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.补打物流单完成;
                        }
                        else if (order2.ServerTaskType == Settings.Setings.EnumServerTaskType.添加物流子单)
                        {
                            order2.Status = Settings.Setings.EnumOrderStatus.追加物流子单完成;
                            order2.ErrMsg = "追加物流子单完成";
                            order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.已打印;
                        }
                        else
                        {
                            order2.Status = Settings.Setings.EnumOrderStatus.单据完成;
                            order2.ErrMsg = "完成";
                            order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.已打印;
                        }



                        if (order2.ServerTaskType == Settings.Setings.EnumServerTaskType.新单据 || order2.ServerTaskType == Settings.Setings.EnumServerTaskType.ERP组合单子单)
                        {
                            bll.ServerWmsBCPrintStatus(wmsInfo, out isOk2, out errCode2, out errMsg2);
                            if (isOk2 != true)
                            {
                                Blll_AddMsgOutEve(order2.ErpId + "回写WMS失败", Settings.Setings.EnumMessageType.提示, "RunManTh.fr.eve", errCode2, errMsg2, "bid:" + order2.Bid, DateTime.Now);
                            }
                        }

                        order2 = bll.ServerPrintFinishOrder(order2, out isOk2, out errCode2, out errMsg2);
                        if (isOk2 == true)
                        {
                            Blll_AddMsgOutEve(order2.ErpId + "更新完成", Settings.Setings.EnumMessageType.提示, "RunManTh.fr.eve", errCode2, errMsg2, "bid:" + order2.Bid, DateTime.Now);
                        }
                        else
                        {
                            Blll_AddMsgOutEve(order2.ErpId + "更新失败", Settings.Setings.EnumMessageType.提示, "RunManTh.fr.eve", errCode2, errMsg2, "bid:" + order2.Bid, DateTime.Now);
                        }
                    }
                }

            }

        });
        public static bool PrintSH(MOD.BllMod.Order printObj, MOD.BllMod.Order updateObj)
        {
            bool res = false;
            string errMsg = "";
            int errCode = 0;
            string pfrPath = YJT.Path.FunStrGetLocalPath();

            MOD.SysMod.PrintDataM danjM = bll.ServerGetPrintData(printObj.WmsDanjbh, out res, out errMsg, out errCode);//printData
            if (res == true)
            {
                pfrPath = pfrPath + @"\frx\shtx_hd.frx";
                if (System.IO.File.Exists(pfrPath))
                {
                    _shtxFr.AddValue("WMS销售单号", danjM.WMS销售单号);
                    _shtxFr.AddValue("YJT_打印抬头", danjM.YJT_打印抬头);
                    _shtxFr.AddValue("业务员", danjM.业务员);
                    _shtxFr.AddValue("发货日期1", danjM.发货日期1);
                    _shtxFr.AddValue("发货日期2", danjM.发货日期2);
                    _shtxFr.AddValue("开票日期", danjM.开票日期);
                    _shtxFr.AddValue("品种总数", danjM.品种总数);
                    _shtxFr.AddValue("备注", danjM.备注);
                    _shtxFr.AddValue("客户ID", danjM.客户ID);
                    _shtxFr.AddValue("开票员", danjM.开票员);
                    _shtxFr.AddValue("总金额", danjM.总金额);
                    _shtxFr.AddValue("打印次数", danjM.打印次数);
                    _shtxFr.AddValue("收货单位ID", danjM.收货单位ID);
                    _shtxFr.AddValue("收货单位名称", danjM.收货单位名称);
                    _shtxFr.AddValue("收货地址", danjM.收货地址);
                    _shtxFr.AddValue("货主ID", danjM.货主ID);
                    _shtxFr.AddValue("货主原单据编号", danjM.货主原单据编号);
                    _shtxFr.AddValue("货主名称", danjM.货主名称);
                    _shtxFr.AddValue("总计件数", danjM.总计件数);
                    _shtxFr.AddValue("总计散件", danjM.总计散件);
                    _shtxFr.AddValue("打印日期最新", danjM.打印日期最新);
                    _shtxFr.AddValue("电商ID", printObj.Bid);
                    System.Data.DataTable dt = YJT.DataTableHandle.ListToClass.ToDataTable(danjM.mx);
                    _shtxFr.AddDataTable("mx", dt);



                    _shtxFr.LoadPrintFrx(pfrPath);
                    //string addModRes = fr.AddMod(printObj).ToString() + "个对象添加";
                    //Blll_AddMsgOutEve(addModRes, Settings.Setings.EnumMessageType.提示, "随货.Print", 1, "", "", DateTime.Now);
                    YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, MOD.SysMod.PrintDataM> keyvp = new YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, MOD.SysMod.PrintDataM>();
                    keyvp.V1 = printObj;
                    keyvp.V2 = danjM;
                    if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                    {
                        //_shtxFr.Design();
                        _shtxFr.Print(false, "Microsoft XPS Document Writer", keyvp);

                    }
                    else
                    {
                        _shtxFr.Print(false, Settings.Configs.GetSHTXPrinterName, keyvp);
                    }
                }
                else
                {
                    printObj.Status = Settings.Setings.EnumOrderStatus.异常_打印方案不存在;
                    printObj.ErrMsg = "汇达随货通行打印方案不存在";
                    Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "汇达随货", -1, pfrPath, printObj.ErpId, DateTime.Now);
                    bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                }

            }
            else
            {
                printObj.Status = Settings.Setings.EnumOrderStatus.异常_WMS不存在;
                printObj.ErrMsg = errMsg;
                Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "汇达随货", -1, pfrPath, "出入库ID" + printObj.WmsDanjbh, DateTime.Now);
                bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
            }

            return res;
        }

        static YJT.Print.FastReport frWmsLdyj = new YJT.Print.FastReport((printObj) => { });
        static YJT.Print.FastReport frHdHpzz = new YJT.Print.FastReport((printObj) => { });
        static YJT.Print.FastReport frXsht = new YJT.Print.FastReport((printObj) => { });
        static YJT.Print.FastReport frFM = new YJT.Print.FastReport((printObj) => { });
        static YJT.Print.FastReport fr = new YJT.Print.FastReport((printObj) =>
        {
            bool isOk2 = false;
            int errCode2 = 0;
            string errMsg2 = "";
            if (printObj != null)
            {
                MOD.BllMod.Order order2 = printObj as MOD.BllMod.Order;
                if (order2 != null)
                {
                    if (order2.Status == Settings.Setings.EnumOrderStatus.补打任务)
                    {
                        order2.Status = Settings.Setings.EnumOrderStatus.补打完成;
                        if (order2.PrintStatus == Settings.Setings.EnumOrderPrintStatus.等待补打物流)
                        {
                            order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.补打物流单完成;
                            order2.ErrMsg = order2.PrintStatus.ToString();
                        }
                        else
                        {
                            order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.补打随货单完成;
                            order2.ErrMsg = order2.PrintStatus.ToString();
                        }

                    }
                    else if (order2.ServerTaskType == Settings.Setings.EnumServerTaskType.补打物流子单)
                    {
                        order2.Status = Settings.Setings.EnumOrderStatus.补打完成;
                        order2.ErrMsg = "完成";
                        order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.补打物流单完成;
                    }
                    else if (order2.ServerTaskType == Settings.Setings.EnumServerTaskType.添加物流子单)
                    {
                        order2.Status = Settings.Setings.EnumOrderStatus.追加物流子单完成;
                        order2.ErrMsg = "子单追加完成";
                        order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.已打印;
                    }
                    else
                    {
                        order2.Status = Settings.Setings.EnumOrderStatus.单据完成;
                        order2.ErrMsg = "完成";
                        order2.PrintStatus = Settings.Setings.EnumOrderPrintStatus.已打印;
                    }



                    order2 = bll.ServerPrintFinishOrder(order2, out isOk2, out errCode2, out errMsg2);
                    if (isOk2 == true)
                    {
                        Blll_AddMsgOutEve(order2.ErpId + "更新完成", Settings.Setings.EnumMessageType.提示, "RunManTh.fr.eve", errCode2, errMsg2, "bid:" + order2.Bid, DateTime.Now);
                    }
                    else
                    {
                        Blll_AddMsgOutEve(order2.ErpId + "更新失败", Settings.Setings.EnumMessageType.提示, "RunManTh.fr.eve", errCode2, errMsg2, "bid:" + order2.Bid, DateTime.Now);
                    }
                }
            }

        });
        /// <summary>
        /// 打印路单药检
        /// </summary>
        /// <param name="orderThis1"></param>
        /// <param name="orderThis2"></param>
        private static bool PrintLDYJ(BllMod.Order printObj, BllMod.Order updateOrder, out List<MOD.ModGoodsInfo> goods)
        {
            bool res = true;
            bool isOk = false;
            int errCode = 0;
            string errMsg = "";
            goods = null;
            string erpid = printObj.ErpId;
            if (YJT.Text.Verification.IsNotNullOrEmpty(erpid))
            {
                goods = bll.GetGoodsLdyjPic(erpid, out isOk, out errCode, out errMsg);
                if (goods != null && goods.Count > 0)
                {
                    res = true;
                }
                else
                {
                    res = false;
                    string t = "";
                    if (YJT.Text.Verification.IsNotNullOrEmpty(errMsg))
                    {
                        t = "提示:" + errMsg;
                    }
                    bll.ServerForceFinesh(printObj.Bid, t + printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "路单药检", -1, errMsg, printObj.ErpId, DateTime.Now);
                }
            }
            return res;
        }
        private static bool PrintPJ(BllMod.Order printObj, BllMod.Order updateOrder, out List<MOD.ModGoodsInfo> goods)
        {
            bool res = true;
            bool isOk = false;
            int errCode = 0;
            string errMsg = "";
            goods = null;
            //获取品种列表
            string erpid = printObj.ErpId;
            //string pfrPath = YJT.Path.FunStrGetLocalPath();
            //pfrPath = pfrPath + @"\frx\HdHPZZ.frx";
            if (YJT.Text.Verification.IsNotNullOrEmpty(erpid))
            {
                goods = bll.GetGoodsPic(erpid, out isOk, out errCode, out errMsg);
                if (goods != null && goods.Count > 0)
                {
                    res = true;
                }
                else
                {
                    res = false;
                    string t = "";
                    if (YJT.Text.Verification.IsNotNullOrEmpty(errMsg))
                    {
                        t = "提示:" + errMsg;
                    }
                    bll.ServerForceFinesh(printObj.Bid, t + printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "商品批件", -1, errMsg, printObj.ErpId, DateTime.Now);
                }
            }
            //按照品种获取资质
            //按照资质,生成打印方案
            //进行对打印方案打印


            return res;

        }



        public static bool PrintHT(MOD.BllMod.Order printObj, MOD.BllMod.Order updateOrder, out List<MOD.HdErp.HdErpHeTong> hderpHetong)
        {
            bool res = true;
            bool isOk = false;
            int errCode = 0;
            string errMsg = "";
            hderpHetong = null;
            //根据需要打印的单据信息,在汇达系统中获取要打印的数据
            string erpid = printObj.ErpId;
            if (YJT.Text.Verification.IsNotNullOrEmpty(erpid))
            {
                hderpHetong = bll.GetHdHeTongOrder(erpid, out isOk, out errCode, out errMsg);
                if (isOk == true)
                {
                    res = true;


                }
                else
                {
                    string t = "";
                    if (YJT.Text.Verification.IsNotNullOrEmpty(errMsg))
                    {
                        t = "提示:" + errMsg;
                    }
                    bll.ServerForceFinesh(printObj.Bid, t + printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "销售合同", -1, errMsg, printObj.ErpId, DateTime.Now);
                    res = false;
                }
            }
            return res;
        }
        public static bool PrintWL(MOD.BllMod.Order printObj, MOD.BllMod.Order updateObj)
        {
            bool res = true;
            string errMsg = "";
            int errCode = 0;
            string pfrPath = YJT.Path.FunStrGetLocalPath();
            switch (printObj.Logic)
            {
                case Settings.Setings.EnumLogicType.Default:
                    printObj.Status = Settings.Setings.EnumOrderStatus.异常_物流不支持打印;
                    printObj.ErrMsg = "Default物流不支持打印";
                    Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, ".Default", -1, pfrPath, printObj.ErpId, DateTime.Now);
                    bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    break;
                case Settings.Setings.EnumLogicType.顺丰:
                    pfrPath = pfrPath + @"\frx\Logic_Shunfeng.frx";
                    if (System.IO.File.Exists(pfrPath))
                    {
                        fr.LoadPrintFrx(pfrPath);
                        string addModRes = fr.AddMod(printObj).ToString() + "个对象添加";
                        Blll_AddMsgOutEve(addModRes, Settings.Setings.EnumMessageType.提示, "顺丰.Print", 1, "", "", DateTime.Now);
                        if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                        {
                            fr.Print(false, "Microsoft XPS Document Writer", updateObj);
                        }
                        else
                        {
                            fr.Print(false, Settings.Configs.GetShunfPrinterName, updateObj);
                        }

                    }
                    else
                    {
                        printObj.Status = Settings.Setings.EnumOrderStatus.异常_打印方案不存在;
                        printObj.ErrMsg = "顺丰打印方案不存在";
                        Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "顺丰", -1, pfrPath, printObj.ErpId, DateTime.Now);
                        bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    }
                    break;
                case Settings.Setings.EnumLogicType.中通快递:
                    {
                        pfrPath = pfrPath + @"\frx\Logic_ZhongTong.frx";
                        //pfrPath = @"C:\Users\Administrator\Desktop\Logic_ZhongTong.frx";
                        if (System.IO.File.Exists(pfrPath))
                        {
                            fr.LoadPrintFrx(pfrPath);
                            string addModRes = fr.AddMod(printObj).ToString() + "个对象添加";
                            Blll_AddMsgOutEve(addModRes, Settings.Setings.EnumMessageType.提示, "中通.Print", 1, "", "", DateTime.Now);
                            if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                            {
                                //fr.Print(false, "Microsoft XPS Document Writer", updateObj);
                                fr.Design();
                            }
                            else
                            {
                                fr.Print(false, Settings.Configs.GetZhongTongPrinterName, updateObj);
                            }

                        }
                        else
                        {
                            printObj.Status = Settings.Setings.EnumOrderStatus.异常_打印方案不存在;
                            printObj.ErrMsg = "中通打印方案不存在";
                            Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "中通", -1, pfrPath, printObj.ErpId, DateTime.Now);
                            bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                        }
                        break;
                    }
                case Settings.Setings.EnumLogicType.申通快递:
                    {
                        pfrPath = pfrPath + @"\frx\Logic_ShenTong.frx";
                        if (System.IO.File.Exists(pfrPath))
                        {
                            fr.LoadPrintFrx(pfrPath);
                            string addModRes = fr.AddMod(printObj).ToString() + "个对象添加";
                            Blll_AddMsgOutEve(addModRes, Settings.Setings.EnumMessageType.提示, "申通.Print", 1, "", "", DateTime.Now);
                            if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                            {
                                //fr.Print(false, "Microsoft XPS Document Writer", updateObj);
                                fr.Design();
                            }
                            else
                            {
                                fr.Print(false, Settings.Configs.GetShenTongPrinterName, updateObj);
                            }

                        }
                        else
                        {
                            printObj.Status = Settings.Setings.EnumOrderStatus.异常_打印方案不存在;
                            printObj.ErrMsg = "中通打印方案不存在";
                            Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "中通", -1, pfrPath, printObj.ErpId, DateTime.Now);
                            bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                        }
                        break;
                    }
                case Settings.Setings.EnumLogicType.邮政EMS:
                    pfrPath = pfrPath + @"\frx\Logic_EmsYouzheng.frx";
                    if (System.IO.File.Exists(pfrPath))
                    {
                        fr.LoadPrintFrx(pfrPath);
                        string addModRes = fr.AddMod(printObj).ToString() + "个对象添加";
                        Blll_AddMsgOutEve(addModRes, Settings.Setings.EnumMessageType.提示, "邮政.Print", 1, "", "", DateTime.Now);
                        if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                        {
                            fr.Print(false, "Microsoft XPS Document Writer", updateObj);
                        }
                        else
                        {
                            fr.Print(false, Settings.Configs.GetEmsYouzPrinterName, updateObj);
                        }

                    }
                    else
                    {
                        printObj.Status = Settings.Setings.EnumOrderStatus.异常_打印方案不存在;
                        printObj.ErrMsg = "顺丰打印方案不存在";
                        Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "顺丰", -1, pfrPath, printObj.ErpId, DateTime.Now);
                        bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    }
                    break;
                //Modify: 修改时间: 2024-02-29 By:Ly 修改内容: PrintWL方法, 新增新邮政Ems分支
                case Settings.Setings.EnumLogicType.新邮政Ems:
                    pfrPath = pfrPath + @"\frx\Logic_NewEms.frx";
                    if (System.IO.File.Exists(pfrPath))
                    {
                        fr.LoadPrintFrx(pfrPath);
                        string addModRes = fr.AddMod(printObj).ToString() + "个对象添加";
                        Blll_AddMsgOutEve(addModRes, Settings.Setings.EnumMessageType.提示, "新邮政接口.Print", 1, "", "", DateTime.Now);
                        if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                        {
                            fr.Print(false, "Microsoft XPS Document Writer", updateObj);
                        }
                        else
                        {
                            fr.Print(false, Settings.Configs.GetEmsYouzPrinterName, updateObj);
                        }

                    }
                    else
                    {
                        printObj.Status = Settings.Setings.EnumOrderStatus.异常_打印方案不存在;
                        printObj.ErrMsg = "新邮政打印方案不存在";
                        Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "新邮政接口", -1, pfrPath, printObj.ErpId, DateTime.Now);
                        bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    }
                    break;
                case Settings.Setings.EnumLogicType.极兔百事:
                    printObj.Status = Settings.Setings.EnumOrderStatus.异常_物流不支持打印;
                    printObj.ErrMsg = "极兔百事物流不支持打印";
                    Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "极兔百事", -1, pfrPath, printObj.ErpId, DateTime.Now);
                    bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    break;
                case Settings.Setings.EnumLogicType.京东物流:
                    pfrPath = pfrPath + @"\frx\Logic_JingdongWl.frx";
                    if (System.IO.File.Exists(pfrPath))
                    {
                        fr.LoadPrintFrx(pfrPath);
                        string[] tarr = printObj.JingdongWl.Split(new string[] { "@<|||>@\n" }, StringSplitOptions.None);
                        if (tarr != null && tarr.Length > 0)
                        {
                            fr.AddValue("JD_aging", tarr[0]);
                            fr.AddValue("JD_agingName", tarr[1]);
                            fr.AddValue("JD_collectionAddress", tarr[2]);
                            fr.AddValue("JD_coverCode", tarr[3]);
                            fr.AddValue("JD_distributeCode", tarr[4]);
                            fr.AddValue("JD_isHideContractNumbers", tarr[5]);
                            fr.AddValue("JD_isHideName", tarr[6]);
                            fr.AddValue("JD_qrcodeUrl", tarr[7]);
                            fr.AddValue("JD_road", tarr[8]);
                            fr.AddValue("JD_siteId", tarr[9]);
                            fr.AddValue("JD_siteName", tarr[10]);
                            fr.AddValue("JD_siteType", tarr[11]);
                            fr.AddValue("JD_slideNo", tarr[12]);
                            fr.AddValue("JD_sourceCrossCode", tarr[13]);
                            fr.AddValue("JD_sourceSortCenterId", tarr[14]);
                            fr.AddValue("JD_sourceSortCenterName", tarr[15]);
                            fr.AddValue("JD_sourceTabletrolleyCode", tarr[16]);
                            fr.AddValue("JD_targetSortCenterId", tarr[17]);
                            fr.AddValue("JD_targetSortCenterName", tarr[18]);
                            fr.AddValue("JD_targetTabletrolleyCode", tarr[19]);
                        }
                        string addModRes = fr.AddMod(printObj).ToString() + "个对象添加";
                        Blll_AddMsgOutEve(addModRes, Settings.Setings.EnumMessageType.提示, "京东.Print", 1, "", "", DateTime.Now);
                        if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.Ip == "172.16.7.46" || _isDebugPrint == "true")
                        {
                            //fr.Print(false, "Microsoft XPS Document Writer", updateObj);
                            fr.Design();
                        }
                        else
                        {
                            fr.Print(false, Settings.Configs.GetJingDongPrinterName, updateObj);
                        }

                    }
                    else
                    {
                        printObj.Status = Settings.Setings.EnumOrderStatus.异常_打印方案不存在;
                        printObj.ErrMsg = "顺丰打印方案不存在";
                        Blll_AddMsgOutEve(printObj.ErrMsg, Settings.Setings.EnumMessageType.异常, "顺丰", -1, pfrPath, printObj.ErpId, DateTime.Now);
                        bll.ServerForceFinesh(updateObj.Bid, printObj.ErrMsg, printObj.Status, Settings.Setings.EnumOrderPrintStatus.特殊_不更改此参数, out res, out errCode, out errMsg);
                    }
                    break;
                default:
                    break;
            }

            return res;
        }

        /// <summary>
        /// 自定义控制台Debug输出
        /// <para>level: 0:Debug, 1:Info, 2: Warn, 3: Error, 4: Fatal</para>
        /// </summary>
        /// <param name="msg">输出内容</param>
        /// <param name="level">0:Debug, 1:Info, 2: Warn, 3: Error, 4: Fatal</param>
        private static void ShowDebug(string msg, int level = 0)
        {

            if (_isDebugMode == "true" && !string.IsNullOrEmpty(msg))
            {
                var originColor = Console.ForegroundColor;
                switch (level)
                {
                    case 0:
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    default:
                        Console.ForegroundColor = originColor;
                        break;
                }
                Console.WriteLine($@"{DateTime.Now:HH:mm:ss:fff}: {msg}");
                Console.ForegroundColor = originColor;
            }
        }
    }
}
