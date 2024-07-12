using System;
using System.Collections.Generic;

namespace PrintFSet
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			YJT.Print.FastReport fr = new YJT.Print.FastReport();
			List<string> prints = fr.GetPrints();
			if (prints == null || prints.Count <= 0)
			{
				Console.WriteLine(@"此电脑没有打印机");
				return;
			}
			foreach (string item in prints)
			{
				Console.WriteLine(item);
				System.IO.File.AppendAllText(@"d:\printsName.txt", item+"\r\n");
			}

			string errmsg = "";
			int errcode = 0;
			bool isok = false;
			string orderType = "";
			if (args == null || args.Length == 0)
			{
				Console.WriteLine(@"顺丰票据:sf");
				Console.WriteLine(@"邮政票据:yz");
				Console.WriteLine(@"百世票据:bs");
				Console.WriteLine(@"京东快递:jd");
				Console.WriteLine(@"随货票据:sh");
				Console.WriteLine(@"申通快递:st");
				Console.WriteLine(@"新邮政票据:nyz");
				Console.WriteLine(@"京东生鲜医药快递:jdfmd");

                var frxPath = AppDomain.CurrentDomain.BaseDirectory + @"frx\";
				if (BLL.Blll._clientInfoObj.Ip.Contains(@"192.168.50."))
				{
					frxPath = @"E:\WorkSpace\Source\Work_Project\面单打印模板\frx\";
                }

				orderType =Console.ReadLine();
				if (orderType == "sf")
				{
					args = new string[] { frxPath + @"Logic_Shunfeng.frx" };
				}
				else if (orderType == "yz")
				{
					args = new string[] { frxPath + @"Logic_EmsYouzheng.frx" };
				}
				else if (orderType == "jd")
				{
					args = new string[] { frxPath + @"Logic_JingdongWl.frx" };
					
				}
				else if (orderType == "st")
				{
					args = new string[] { frxPath + @"Logic_ShenTong.frx" };

				}
				else if (orderType == "nyz")
				{
					args = new string[] { frxPath + @"Logic_NewEms.frx" };
				}
				else if (orderType == "jdfmd")
                {
                    args = new string[] { frxPath + @"Logic_JingdongWl.frx" };
                }
				else if (orderType == "bs")
				{
					Console.WriteLine(@"打印方案不存在");
					Console.ReadKey(false);
					return;

				}
				else if (orderType == "sh")
				{
					args = new string[] { frxPath + @"shtx_hd.frx" };
				}
				else
				{
					Console.WriteLine(@"打印方案不存在");
					Console.ReadKey(false);
					return;
				}
			}
			string fullFileName = args[0];
			if (System.IO.File.Exists(fullFileName))
			{
			}
			else
			{
				Console.WriteLine(@"打印方案不存在");
				Console.ReadKey(false);
				return;
			}
			string filename = System.IO.Path.GetFileName(fullFileName);
			if (filename == "Logic_Shunfeng.frx")
			{
				orderType = "sf";
			}
			else if (filename == "shtx_hd.frx")
			{
				orderType = "sh";
			}
			else if (filename == "Logic_EmsYouzheng.frx")
			{
				orderType = "yz";
			}
			else if (filename == "Logic_JingdongWl.frx")
			{
				orderType = "jd";
			}
			else if (filename == "Logic_ShenTong.frx")
			{
				orderType = "st";
			}
			else if (filename == "Logic_NewEms.frx")
			{
				orderType = "nyz";
			}
			else if (filename == "Logic_JingdongWl.frx")
            {
                orderType = "jdfmd";
            }
            else
			{
				Console.WriteLine(@"打印方案不存在");
				Console.ReadKey(false);
				return;
			}

			Console.WriteLine(@"默认34,是否需要改变,不改变,直接回车");
			string bid = Console.ReadLine();
			if (YJT.Text.Verification.IsNullOrEmpty(bid))
			{
				bid = "34";
			}
			BLL.Blll bll = BLL.Blll.init();
			MOD.BllMod.Order order = bll.ServerGetOrder(bid, out isok, out errcode, out errmsg);
			if (order != null)
			{
				if (orderType == "sf")
				{
					fr.LoadPrintFrx(args[0]);
					fr.AddMod(order);
					fr.Design();
				}
				else if (orderType == "yz")
				{
					fr.LoadPrintFrx(args[0]);
					fr.AddMod(order);
					fr.Design();
				}
				else if (orderType == "jd")
				{
					fr.LoadPrintFrx(args[0]);
					string[] tarr = order.JingdongWl.Split(new string[] { "@<|||>@\n" }, StringSplitOptions.None);
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
					fr.AddMod(order);
					fr.Design();
				}
				else if (orderType == "sh")
				{
					MOD.SysMod.PrintDataM danjM = bll.ServerGetPrintData(order.WmsDanjbh, out isok, out errmsg, out errcode);
					if (isok == true)
					{
						
						
						if (System.IO.File.Exists(fullFileName))
						{
							fr.AddValue("WMS销售单号", danjM.WMS销售单号);
							fr.AddValue("YJT_打印抬头", danjM.YJT_打印抬头);
							fr.AddValue("业务员", danjM.业务员);
							fr.AddValue("发货日期1", danjM.发货日期1);
							fr.AddValue("发货日期2", danjM.发货日期2);
							fr.AddValue("开票日期", danjM.开票日期);
							fr.AddValue("品种总数", danjM.品种总数);
							fr.AddValue("备注", danjM.备注);
							fr.AddValue("客户ID", danjM.客户ID);
							fr.AddValue("开票员", danjM.开票员);
							fr.AddValue("总金额", danjM.总金额);
							fr.AddValue("打印次数", danjM.打印次数);
							fr.AddValue("收货单位ID", danjM.收货单位ID);
							fr.AddValue("收货单位名称", danjM.收货单位名称);
							fr.AddValue("收货地址", danjM.收货地址);
							fr.AddValue("货主ID", danjM.货主ID);
							fr.AddValue("货主原单据编号", danjM.货主原单据编号);
							fr.AddValue("货主名称", danjM.货主名称);
							fr.AddValue("总计件数", danjM.总计件数);
							fr.AddValue("总计散件", danjM.总计散件);
							fr.AddValue("打印日期最新", danjM.打印日期最新);
							fr.AddValue("电商ID", order.Bid);
							fr.AddValue("收货单位联系方式", danjM.收货单位联系方式);
							System.Data.DataTable dt = YJT.DataTableHandle.ListToClass.ToDataTable(danjM.mx);
							fr.AddDataTable("mx", dt);
							fr.LoadPrintFrx(fullFileName);
							fr.Design();
						}
					}
				}
				else if (orderType == "st")
				{
					fr.LoadPrintFrx(args[0]);
					fr.AddMod(order);
					fr.Design();
				}
				else if(orderType == "nyz")
				{
                    fr.LoadPrintFrx(args[0]);
                    fr.AddMod(order);
                    fr.Design();
                }
				else if (orderType == "jdfmd")
                {
                    fr.LoadPrintFrx(args[0]);
                    string[] tarr = order.JingdongWl.Split(new string[] { "@<|||>@\n" }, StringSplitOptions.None);
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
                    fr.AddMod(order);
                    fr.Design();
                }
			}
			
			Console.ReadKey();
			return;
		}
	}
}
