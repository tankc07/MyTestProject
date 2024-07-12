using System.Collections.Generic;
using Settings;

namespace Common
{
	public class Logistics
	{
		/// <summary>
		/// 判断是否需要发票
		/// </summary>
		/// <param name="ps"></param>
		/// <param name="platformType"></param>
		/// <returns></returns>
		public static string GetFP(string ps, string fplx, Settings.Setings.EnumPlatformType platformType)
		{
			string res = "";
			if (!YJT.Text.Verification.IsNotNullOrEmpty(ps))
			{
				ps = "";
			}
			switch (platformType)
			{
				case Settings.Setings.EnumPlatformType.药师帮:
					//普通发票,专用发票
					if (fplx == "专用发票"|| YJT.Text.Verification.IsContain(ps, "专用发票"))
					{
						res = "专票 ";
					}
					else
					{
						if (
								//YJT.Text.Verification.IsContain(ps, "电子发票") || 
								//YJT.Text.Verification.IsContain(ps, "电子税票")||
								//YJT.Text.Verification.IsContain(ps, "纸质发票") ||
								//YJT.Text.Verification.IsContain(ps, "纸质税票") ||
								YJT.Text.Verification.IsContain(ps, "发票") ||
								YJT.Text.Verification.IsContain(ps, "税票")||
								YJT.Text.Verification.IsContain(ps, "票据请随货同行")
							)
						{
							res = "普票 ";
						}
						else if (fplx == "普通发票")
						{
							res = "电子发票 ";
						}
						else
						{
							res = "";
						}
					}
					break;
				case Setings.EnumPlatformType.小药药:
					if (fplx == "专用发票" || YJT.Text.Verification.IsContain(ps, "专用发票"))
					{
						res = "专票 ";
					}
					else
					{
						if (
								//YJT.Text.Verification.IsContain(ps, "电子发票") || 
								//YJT.Text.Verification.IsContain(ps, "电子税票")||
								//YJT.Text.Verification.IsContain(ps, "纸质发票") ||
								//YJT.Text.Verification.IsContain(ps, "纸质税票") ||
								YJT.Text.Verification.IsContain(ps, "发票") ||
								YJT.Text.Verification.IsContain(ps, "税票")||
								YJT.Text.Verification.IsContain(ps, "票据请随货同行")
							)
						{
							res = "普票 ";
						}
						else if (fplx == "普通发票")
						{
							res = "电子发票 ";
						}
						else
						{
							res = "";
						}
					}
					break;
				case Setings.EnumPlatformType.药京采:
					if (fplx == "专用发票")
					{
						res = "专票 ";
					}
					else
					{
						if (YJT.Text.Verification.IsContain(ps, "专票") || YJT.Text.Verification.IsContain(ps, "专用发票"))
						{
							res = "专票 ";
						}
						else if (YJT.Text.Verification.IsContain(ps, "发票"))
						{
							res = "普票 ";
						}
						else if (fplx == "普通发票")
						{
							res = "电子发票 ";
						}
						else
						{
							res = "";
						}
					}
					break;
				default:
					break;
			}
			return res;
		}
		/// <summary>
		/// 判断是否需要药品批件
		/// </summary>
		/// <param name="oRIGINALREMARK"></param>
		/// <param name="platformType"></param>
		/// <returns></returns>
		public static string GetPJ(string ps, Setings.EnumPlatformType platformType)
		{
			string res = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(ps))
			{
				switch (platformType)
				{
					case Settings.Setings.EnumPlatformType.药师帮:
						if (YJT.Text.Verification.IsContain(ps, "批件"))
						{
							res = "批件 ";
						}
						else
						{
							res = "";
						}
						break;
					default:
						break;
				}
			}

			return res;
		}
		/// <summary>
		/// 判断是否需要购销合同
		/// </summary>
		/// <param name="oRIGINALREMARK"></param>
		/// <param name="platformType"></param>
		/// <returns></returns>
		public static string GetHeTong(string ps, Setings.EnumPlatformType platformType)
		{
			string res = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(ps))
			{
				if (YJT.Text.Verification.IsContain(ps, "合同"))
				{
					res = "合同 ";
				}
				else
				{
					res = "";
				}
			}

			return res;
		}
		/// <summary>
		/// 判断是否需要企业首营
		/// </summary>
		/// <param name="oRIGINALREMARK"></param>
		/// <param name="platformType"></param>
		/// <returns></returns>
		public static string GetQYSY(string ps, Setings.EnumPlatformType platformType)
		{
			string res = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(ps))
			{
				switch (platformType)
				{
					case Settings.Setings.EnumPlatformType.药师帮:
						if (YJT.Text.Verification.IsContain(ps, "企业首营"))
						{
							res = "企业首营 ";
						}
						else
						{
							res = "";
						}
						break;
					case Setings.EnumPlatformType.小药药:
						if (YJT.Text.Verification.IsContains(ps, new List<string>() { "首营", "资质", "企业首营", "三证", "委托书", "年度报告", "首营资质", "首营", "首营全套资质", "首营资质", "全套资质" }))
						{
							res = "企业首营";
						}
						else
						{
							res = "";
						}
						break;
					default:
						break;
				}
			}

			return res;
		}
		/// <summary>
		/// 判断是否需要商品首营 商品首营资质
		/// </summary>
		/// <param name="oRIGINALREMARK"></param>
		/// <param name="platformType"></param>
		/// <returns></returns>
		public static string GetSYZZ(string ps, Setings.EnumPlatformType platformType)
		{
			string res = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(ps))
			{
				switch (platformType)
				{
					case Settings.Setings.EnumPlatformType.药师帮:
						if (YJT.Text.Verification.IsContain(ps, "首营资质"))
						{
							res = "首营资质 ";
						}
						else
						{
							res = "";
						}
						if (YJT.Text.Verification.IsContain(ps, "鲜章"))
						{
							res = "鲜章 " + res;
						}
						break;
					case Setings.EnumPlatformType.小药药:
						if (YJT.Text.Verification.IsContains(ps, new List<string>() { "资质","质检" }))
						{
							res = "首营资质";
						}
						else
						{
							res = "";
						}
						break;
					default:
						break;
				}
			}

			return res;
		}

		public static string GetYJBB(string ps, Setings.EnumPlatformType platformType)
		{
			string res = "";
			if (YJT.Text.Verification.IsNotNullOrEmpty(ps))
			{
				switch (platformType)
				{
					case Settings.Setings.EnumPlatformType.药师帮:
						List<string> t = YJT.Text.Verification.FunSplitStrByType(ps, 2);
						if (t != null && t.Count > 0)
						{
							if (t.Contains("质检"))
							{
								res = "药检 ";
							}

						}
						if (YJT.Text.Verification.IsContain(ps, "药检"))
						{
							res = "药检 ";
						}
						break;
					default:
						break;
				}
			}

			return res;
		}


	}
}
