using System.Collections.Generic;

namespace MOD
{
	public class SysMod
	{
		public class ClinetTag
		{
			public string Ip = "";
			public string Mac = "";
			public string ComputerName = "";
			public int ClientId = -1;
			public bool EnableInput = false;
			public bool isServer = false;
			public bool isForge = false;
		}
		public class PrintDataM
		{
			public class Mod_PrintDataD
			{
				public string 经营类别
				{
					get; set;
				}
				public string 序号
				{
					get; set;
				}
				public string WMS单据编号
				{
					get; set;
				}
				public string WMS细表序号
				{
					get; set;
				}
				public string 商品IDWMS
				{
					get; set;
				}
				public string 商品IDERP
				{
					get; set;
				}
				public string 商品名称
				{
					get; set;
				}
				public string 生产厂家
				{
					get; set;
				}
				public string 上市持有人
				{
					get; set;
				}
				public string 规格
				{
					get; set;
				}
				public string 批号
				{
					get; set;
				}
				public string 剂型
				{
					get; set;
				}
				public string 数量
				{
					get; set;
				}
				public string 商品单位
				{
					get; set;
				}
				public string 单价
				{
					get; set;
				}
				public string 总金额
				{
					get; set;
				}
				public string 件数
				{
					get; set;
				}
				public string 包装单位
				{
					get; set;
				}
				public string 生产日期
				{
					get; set;
				}
				public string 有效期
				{
					get; set;
				}
				public string 批准文号
				{
					get; set;
				}
				public string 产地
				{
					get; set;
				}
				public string 运输条件
				{
					get; set;
				}
				public string 经营范围名
				{
					get; set;
				}
				public string 生产许可证
				{
					get; set;
				}
				public string 注册证
				{
					get; set;
				}
			}
			public string iSjshz
			{
				get;
				set;
			}
			public string iSlchz
			{
				get;
				set;
			}
			public string iSyshz
			{
				get;
				set;
			}
			public string iSshtx
			{
				get;
				set;
			}
			public string iSlljj
			{
				get;
				set;
			}
			public string iShmhz
			{
				get;
				set;
			}
			public string 总计散件
			{
				get; set;
			}
			public string 总计件数
			{
				get; set;
			}
			public string 货主ID
			{
				get; set;
			}
			public string 货主名称
			{
				get; set;
			}
			public string 打印日期最新
			{
				get; set;
			}
			public string 打印次数
			{
				get; set;
			}
			public string YJT_打印抬头
			{
				get; set;
			}
			public string 发货日期1
			{
				get; set;
			}
			public string 货主原单据编号
			{
				get; set;
			}
			public string WMS销售单号
			{
				get; set;
			}
			public string 收货单位ID
			{
				get; set;
			}
			public string 收货单位名称
			{
				get; set;
			}
			public string 业务员
			{
				get; set;
			}
			public string 总金额
			{
				get; set;
			}
			public string 收货地址
			{
				get; set;
			}
			public string 客户ID
			{
				get; set;
			}
			public string 备注
			{
				get; set;
			}
			public string 品种总数
			{
				get; set;
			}
			public string 开票日期
			{
				get; set;
			}
			public string 发货日期2
			{
				get; set;
			}
			public string 开票员
			{
				get; set;
			}

			public string 收货单位联系方式
			{
				get; set;
			}
			public List<Mod_PrintDataD> mx = new List<Mod_PrintDataD>();
		}

	}
}
