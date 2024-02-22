using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestControl
{
	class Program
	{
		static void Main(string[] args)
		{
			//清理未完成单据
			YJT.Logistics.ShenTongLogistic st= YJT.Logistics.ShenTongLogistic.Init(Settings.APITokenKey.ShenTongAppKey, Settings.APITokenKey.ShenTongSecretKey, Settings.APITokenKey.ShenTongResourceCode, Settings.APITokenKey.ShenTongFormOrderCode, Settings.APITokenKey.ShenTongSiteCode, Settings.APITokenKey.ShenTongCustomerName, Settings.APITokenKey.ShenTongSitePwd, Settings.APITokenKey.ShenTongIsTest);
			
			YJT.DataBase.DbHelper dbh= new YJT.DataBase.DbHelperSqlServer("172.16.1.15", "YanduECommerceAutomaticPrinting", "dsby", "dsby", "3341");
			string sqlCmd = "SELECT TOP 1 ydanjbh FROM tmp01 WHERE iscancel='否'";
			string sqlCmd2 = "update tmp01 set iscancel='是' where ydanjbh='{0}'";
			string sqlCmd4 = "update tmp01 set iscancel='错' where ydanjbh='{0}'";
			string sqlCmd3 = "";
			string ydanjbh = "";
			do
			{
				bool isok = false;
				int errCode = -1;
				string errMsg="";
				YJT.Logistics.ShenTongLogistic.ClassPostData postData = null;
				string sendData = "";
				string recData = "";
				object dbres = dbh.ExecuteScalar(sqlCmd, null, true);
				if (YJT.DataBase.Common.IsNotDbNull(dbres))
				{
					ydanjbh = dbres.ToString();
					YJT.Logistics.ShenTongLogistic.ClassCancelOrder cco= new YJT.Logistics.ShenTongLogistic.ClassCancelOrder();
					cco.billCode = ydanjbh;
					cco.creater = "管理员";
					cco.orderSource = "KB";
					cco.orderType = "01";
					cco.remark = "重复下单";
					cco.sourceOrderId = null;
					YJT.Logistics.ShenTongLogistic.ClassCancelOrderRes ccor= st.CancelOrder(cco, out isok, out errCode,out errMsg, out postData, out sendData, out recData);
					if (isok == true && errCode == 0 && ccor != null && ccor.data == "成功" && ccor.success == "true")
					{
						sqlCmd3 = string.Format(sqlCmd2, ydanjbh);
						int dbres2 = dbh.ExecuteNonQuery(sqlCmd3, null, CommandType.Text, false, true, "");
						if (dbres2 > 0)
						{
							Console.WriteLine(ydanjbh+"停止完成");
						}
						else
						{
							Console.WriteLine("更新失败:\r\n" + sqlCmd2);
							break;
						}
					}
					else
					{
						
						Console.WriteLine("停止失败:\r\n" + cco.billCode+"\r\n"+ "订单已完结,无法取消");
						sqlCmd3 = string.Format(sqlCmd4, ydanjbh);
						int dbres2 = dbh.ExecuteNonQuery(sqlCmd3, null, CommandType.Text, false, true, "");
						if (dbres2 > 0)
						{

						}
						else
						{
							Console.WriteLine("错误更新失败:\r\n" + sqlCmd2);
							break;
						}
						break;
					}

				}
				else
				{
					break;
				}
			} while (1 == 1);
			





			//测试
			//Random rd = new Random();
			//string orderby = "";
			//string whereString = "";
			//restart:
			//System.Data.DataColumn[] dcs = new System.Data.DataColumn[5];
			//dcs[0] = new System.Data.DataColumn("riqi");
			//dcs[1] = new System.Data.DataColumn("danjbh");
			//dcs[2] = new System.Data.DataColumn("jine");
			//dcs[3] = new System.Data.DataColumn("yewy");
			//System.Data.DataTable dt = new System.Data.DataTable();
			//dt.Columns.AddRange(dcs);
			//System.Data.DataRow dr = dt.NewRow();
			//dr["riqi"] = "1983-11-25";
			//dr["danjbh"] = YJT.Text.ClassCreateText.FunStrCombinationDanjbh("RY", (dt.Rows.Count + 1).ToString(),5);
			//dr["jine"] = ((double)(rd.Next(100, 1000))) + rd.NextDouble();
			//dr["yewy"] = "张三";
			//dt.Rows.Add(dr);

			//dr = dt.NewRow();
			//dr["riqi"] = "1983-11-26";
			//dr["danjbh"] = YJT.Text.ClassCreateText.FunStrCombinationDanjbh("RY", (dt.Rows.Count + 1).ToString(), 5);
			//dr["jine"] = ((double)(rd.Next(100, 1000))) + rd.NextDouble();
			//dr["yewy"] = "李四";
			//dt.Rows.Add(dr);

			//dr = dt.NewRow();
			//dr["riqi"] = "1984-11-26";
			//dr["danjbh"] = YJT.Text.ClassCreateText.FunStrCombinationDanjbh("RY", (dt.Rows.Count + 1).ToString(), 5);
			//dr["jine"] = ((double)(rd.Next(100, 1000))) + rd.NextDouble();
			//dr["yewy"] = "王五";
			//dt.Rows.Add(dr);

			//dr = dt.NewRow();
			//dr["riqi"] = "2022-11-26";
			//dr["danjbh"] = YJT.Text.ClassCreateText.FunStrCombinationDanjbh("RY", (dt.Rows.Count + 1).ToString(), 5);
			//dr["jine"] = ((double)(rd.Next(100, 1000))) + rd.NextDouble();
			//dr["yewy"] = "赵六";
			//dt.Rows.Add(dr);

			//dr = dt.NewRow();
			//dr["riqi"] = "2021-11-26";
			//dr["danjbh"] = YJT.Text.ClassCreateText.FunStrCombinationDanjbh("RY", (dt.Rows.Count + 1).ToString(), 5);
			//dr["jine"] = ((double)(rd.Next(100, 1000))) + rd.NextDouble();
			//dr["yewy"] = "陈七";
			//dt.Rows.Add(dr);

			//dr = dt.NewRow();
			//dr["riqi"] = "2018-11-26";
			//dr["danjbh"] = YJT.Text.ClassCreateText.FunStrCombinationDanjbh("RY", (dt.Rows.Count + 1).ToString(), 5);
			//dr["jine"] = ((double)(rd.Next(100, 1000))) + rd.NextDouble();
			//dr["yewy"] = "张三";
			//dt.Rows.Add(dr);

			//dr = dt.NewRow();
			//dr["riqi"] = "1983-11-26";
			//dr["danjbh"] = YJT.Text.ClassCreateText.FunStrCombinationDanjbh("RY", (dt.Rows.Count + 1).ToString(), 5);
			//dr["jine"] = ((double)(rd.Next(100, 1000))) + rd.NextDouble();
			//dr["yewy"] = "张三";
			//dt.Rows.Add(dr);

			//System.Data.DataTable dt2;
			//dt.DefaultView.Sort = orderby;
			//dt.DefaultView.RowFilter = whereString;
			//dt2 = dt.DefaultView.ToTable();


			//dt.Dispose();
			//dt = null;
			//Console.WriteLine("日期	单据编号	金额	人员");
			//foreach (System.Data.DataRow item in dt2.Rows)
			//{
			//	Console.WriteLine(item["riqi"].ToString()+"	"+item["danjbh"].ToString()+"	"+double.Parse(item["jine"].ToString()).ToString("#0.000") +"	"+item["yewy"].ToString());
			//}
			////Console.ReadKey(true);
			//orderby = Console.ReadLine();
			//whereString = Console.ReadLine();
			//Console.Clear();
			//goto restart;

			//return;

			//BLL.FileHandle.GetHdGoodsQualificationFile("7444", "211682","");
			//string aa = "aaa||@||||@||ccc";
			//string[] bb = aa.Split(new string[] { "||@||" }, StringSplitOptions.None);
			//int i = 1;
			//foreach (string item in bb)
			//{
			//	Console.WriteLine(i.ToString()+"	"+item);
			//	i++;
			//}

			//YJT.Logistics.JingDongLogistics jd = YJT.Logistics.JingDongLogistics.Init(Settings.APITokenKey.JingdongWlAppKey, Settings.APITokenKey.JingdongWlAppSecret, Settings.APITokenKey.JingdongWlToken, Settings.APITokenKey.JingdongWlAccessUrl, Settings.APITokenKey.JingdongWlDeptNo);

			//YJT.Logistics.JingDongLogistics.ClassCreateOrder createClass = new YJT.Logistics.JingDongLogistics.ClassCreateOrder()
			//{
			//	OrderBaojia = 0,
			//	OrderColdChainOn = 3,
			//	OrderCreateMan = "电商部",
			//	OrderCreateTime = DateTime.Now,
			//	OrderDanjbh = "CS0001",
			//	OrderDeliverType = 17,
			//	OrderGoodsName = "药品",
			//	OrderGoodsQty = 1,
			//	OrderPayType = 0,
			//	OrderProjectName = "测试",
			//	OrderPs = "测试",
			//	OrderRecMoney = 0,
			//	OrderTemptureNum = 10,
			//	OrderWeight =1,
			//	OrderVolume = 1,


			//	RecAddress = "广东省汕尾市附城镇南湖社区（祥德路明云居101号吴滨校卫生室）",
			//	RecCompany = "附城镇南湖吴滨校卫生室",
			//	RecMan = "庞英梅",
			//	RecPhone = "18647425177",
			//	RecSheng = "广东省",
			//	RecShi = "汕尾市",
			//	RecTel = "18647425177",
			//	RecXian = "附城镇",

			//	SendAddress = "河北省保定市莲池区北三环801号",
			//	senderCompany = "河北燕都医药物流有限公司",
			//	SendMan = "刘聪聪",
			//	SendPhone = "13931252550",
			//	SendSheng = "河北省",
			//	SendShi = "保定市",
			//	SendTel = "13931252550",
			//	SendXian = "莲池区"
			//};
			//int errCode = 0;
			//bool isOk = false;
			//string errMsg = "";
			//string sendDate = "";
			//string recData = "";
			//jd.GetDept(null,out isOk,out errCode,out errMsg,out sendDate,out recData);
			//YJT.Logistics.JingDongLogistics.ClassCreateOrderRes res= jd.CreateOrder(createClass,out isOk,out errCode,out errMsg,out sendDate,out recData);
			//string json = YJT.Json.FunStrSerializeToStr(res);
			//System.IO.File.WriteAllText(@"d:\1.txt", sendDate);
			//System.IO.File.WriteAllText(@"d:\2.txt", recData);
			//System.IO.File.WriteAllText(@"d:\3.txt", json);





			//Console.WriteLine(a);

			////YJT.Sound.TTSMicrosoft.Init().Speak("声音检测完成", isSyn: true);
			//MOD.SysMod.ClinetTag ct = Common.PubMethod.GetClientTag();
			//if (ct == null)
			//{
			//	Console.WriteLine("未能获取服务器信息,按任意键退出");
			//	Console.ReadKey(false);
			//	return;
			//}

			//BLL.Blll._clientInfoObj = ct;

			//bll = BLL.Blll.init();
			//Console.WriteLine(YJT.Text.ClassCreateText.FunStrCreateNumberStr(6, null));
			//string aa = Common.Logistics.GetFP("","专用发票", Settings.Setings.EnumPlatformType.小药药);
			//Console.WriteLine(aa);

			Console.ReadKey();

		}
	}
}
