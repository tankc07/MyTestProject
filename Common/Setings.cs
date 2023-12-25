using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public class PubMethod
	{
		public static string GetNameSpace()
		{
			string callNameSpace = "";
			try
			{
				System.Reflection.MethodBase callObj = new System.Diagnostics.StackTrace(true).GetFrame(1).GetMethod();
				callNameSpace = "[" + callObj.DeclaringType.Namespace + "].[" + callObj.DeclaringType.FullName + "].[" + callObj.Name + "]\r\n\t";
			}
			catch
			{
				callNameSpace = "获取命名空间异常";
			}
			return callNameSpace;
			
		}
		public static MOD.SysMod.ClinetTag GetClientTag()
		{
			MOD.SysMod.ClinetTag res = null;
			List<Dictionary<string,string>> list = YJT.NetWork.NetTools.FunListGetNetInterfaces();
			if (list.Count > 0)
			{
				foreach (Dictionary<string, string> item in list)
				{
					if (item.ContainsKey("Status"))
					{
						if (YJT.Text.Verification.IsEqualsEx(item["Status"], "up", true))
						{
							MOD.SysMod.ClinetTag t = new MOD.SysMod.ClinetTag();
							t.Mac = item["MAC"];
							t.Ip = item["IPV4S"];
							t.ComputerName = YJT.MSystem.SysInfo.GetComputerName();
							res = t;
							//if (res.Ip == "172.16.7.50|")
							//{
							//	res.Ip = "172.16.2.150";
							//	res.Mac = "94C691F3D450";
							//	res.ComputerName = "DESKTOP-AU62CUP";
							//	res.isForge = true;
							//}
							break;
						}
					}


				}
			}
			return res;

		}
	}

}
