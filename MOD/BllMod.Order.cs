using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOD
{
	public class BllMod
	{
		public class Order
		{
			long _bid = 0;
			/// <summary>
			/// 数据库唯一ID
			/// </summary>
			public long Bid
			{
				get
				{
					return _bid;
				}
				set
				{
					_bid = value;
				}
			}
			long _orderId = 0;
			/// <summary>
			/// 本次扫码程序ID,用于对应.,一台机器一组序号
			/// </summary>
			public long OrderId
			{
				get
				{
					return _orderId;
				}
				set
				{
					_orderId = value;
				}
			}
			
			DateTime _addDate = DateTime.Parse("1900-01-01");
			/// <summary>
			/// 添加日期
			/// </summary>
			public DateTime AddDate
			{
				get
				{
					return _addDate;
				}
				set
				{
					_addDate = value;
				}
			}
			string _erpId = "";
			/// <summary>
			/// ERPid
			/// </summary>
			public string ErpId
			{
				get
				{
					return _erpId;
				}
				set
				{
					if (YJT.Text.Verification.IsNullOrEmpty(value))
					{
						_erpId = "";
					}
					else
					{
						_erpId = value.Replace("'","");
					}
				}
			}
			Settings.Setings.EnumLogicType _logic = Settings.Setings.EnumLogicType.Default;
			/// <summary>
			/// 物流
			/// </summary>
			public Settings.Setings.EnumLogicType Logic
			{
				get
				{
					return _logic;
				}
				set
				{
					_logic = value;
				}
			}
			double _weight = 0;
			/// <summary>
			/// 重量
			/// </summary>
			public double Weight
			{
				get
				{
					return _weight;
				}
				set
				{
					_weight=value;
				}
			}
			double _length = 0;
			/// <summary>
			/// 长
			/// </summary>
			public double Length
			{
				get
				{
					return _length;
				}
				set
				{
					_length = value;
				}
			}
			double _width = 0;
			/// <summary>
			/// 宽
			/// </summary>
			public double Width
			{
				get
				{
					return _width;
				}
				set
				{
					_width = value;
				}
			}
			double _height = 0;
			/// <summary>
			/// 高
			/// </summary>
			public double Height
			{
				get
				{
					return _height;
				}
				set
				{
					_height = value;
				}
			}
			Settings.Setings.EnumOrderStatus _status = Settings.Setings.EnumOrderStatus.无动作;
			/// <summary>
			/// 本次动作
			/// </summary>
			public Settings.Setings.EnumOrderStatus Status
			{
				get
				{
					return _status;
				}
				set
				{
					_status = value;
				}
			}
			string _ip = "";
			/// <summary>
			/// 当前客户IP
			/// </summary>
			public string Ip
			{
				get
				{
					return _ip;
				}
				set
				{
					_ip = (value==null)?"": value.Replace("'", "");
				}
			}
			string _mac = "";
			/// <summary>
			/// 当前客户MAC
			/// </summary>
			public string Mac
			{
				get
				{
					return _mac;
				}
				set
				{
					_mac = (value==null)?"": value.Replace("'", "");
				}
			}
			string _computerName = "";
			/// <summary>
			/// 当前客户计算机名
			/// </summary>
			public string ComputerName
			{
				get
				{
					return _computerName;
				}
				set
				{
					_computerName = (value==null)?"": value.Replace("'", "");
				}
			}
			string _errMsg = "";
			/// <summary>
			/// 提示或处理错误后的原因
			/// </summary>
			public string ErrMsg
			{
				get
				{
					return _errMsg;
				}
				set
				{
					_errMsg = (value==null)?"": value.Replace("'", "");
				}
			}
			int _clientId = -1;
			/// <summary>
			/// 本机ID,对应IP,MAC,计算机名
			/// </summary>
			public int ClientId
			{
				get
				{
					return _clientId;
				}
				set
				{
					_clientId = value;
				}
			}
			Settings.Setings.EnumOrderPrintStatus _printStatus = Settings.Setings.EnumOrderPrintStatus.不可打印;
			/// <summary>
			/// 物流单打印状态
			/// </summary>
			public Settings.Setings.EnumOrderPrintStatus PrintStatus
			{
				get
				{
					return _printStatus;
				}
				set
				{
					_printStatus = value;
				}
			}
			string _wmsDanjbh = "";
			/// <summary>
			/// WMS出入库ID
			/// </summary>
			public string WmsDanjbh
			{
				get
				{
					return _wmsDanjbh;
				}
				set
				{
					_wmsDanjbh = (value==null)?"": value.Replace("'", "");
				}
			}

			string _netDanjbh = "";
			/// <summary>
			/// 电商平台ID
			/// </summary>
			public string NetDanjbh
			{
				get
				{
					return _netDanjbh;
				}
				set
				{
					_netDanjbh = (value==null)?"": value.Replace("'", "");
				}
			}
			DateTime _clientHandleDate = default(DateTime);
			/// <summary>
			/// 客户端处理时间
			/// </summary>
			public DateTime ClientHandleDate
			{
				get
				{
					return _clientHandleDate;
				}
				set
				{
					_clientHandleDate = value;
				}
			}
			DateTime _serverHandleDate = default(DateTime);
			/// <summary>
			/// 服务端处理的时间
			/// </summary>
			public DateTime ServerHandleDate
			{
				get
				{
					return _serverHandleDate;
				}
				set
				{
					_serverHandleDate = value;
				}
			}
			string _wmsYewy = "";
			public string WmsYewy
			{
				get
				{
					return _wmsYewy;
				}
				set
				{
					_wmsYewy = (value==null)?"": value.Replace("'", "");
				}
			}
			string _wmsDdwname = "";
			public string WmsDdwname
			{
				get
				{
					return _wmsDdwname;
				}
				set
				{
					_wmsDdwname = (value==null)?"": value.Replace("'", "");
				}
			}
			string _NETORDER_FROMID = "";
			/// <summary>
			/// 电商平台编号 1 药师帮,空是异常
			/// </summary>
			public string NETORDER_FROMID
			{
				get
				{
					return _NETORDER_FROMID;
				}
				set
				{
					_NETORDER_FROMID = (value==null)?"": value.Replace("'", "");
				}
			}
			string _NETORDER_FROM = "";
			/// <summary>
			/// 电商平台名称 如 药师帮
			/// </summary>
			public string NETORDER_FROM
			{
				get
				{
					return _NETORDER_FROM;
				}
				set
				{
					_NETORDER_FROM = (value==null)?"": value.Replace("'", "");
				}
			}
			string _ERPORDER_ID = "";
			/// <summary>
			/// 接口数据库中的ERPid,与erpID一致才对
			/// </summary>
			public string ERPORDER_ID
			{
				get
				{
					return _ERPORDER_ID;
				}
				set
				{
					_ERPORDER_ID = (value==null)?"": value.Replace("'", "");
				}
			}
			string _CUSTOMID = "";
			/// <summary>
			/// ERP中客户ID
			/// </summary>
			public string CUSTOMID
			{
				get
				{
					return _CUSTOMID;
				}
				set
				{
					_CUSTOMID = (value==null)?"": value.Replace("'", "");

				}
			}
			string _CUSTOMNAME = "";
			/// <summary>
			/// ERP中对应的客户名称
			/// </summary>
			public string CUSTOMNAME
			{
				get
				{
					return _CUSTOMNAME;
				}
				set
				{
					_CUSTOMNAME = (value==null)?"": value.Replace("'", "");
				}
			}
			string _AGENTNAME = "";
			/// <summary>
			/// 接口中显示业务员,对应WMSyewy才对
			/// </summary>
			public string AGENTNAME
			{
				get
				{
					return _AGENTNAME;
				}
				set
				{
					_AGENTNAME = (value==null)?"": value.Replace("'", "");
				}
			}
			string _ADDRESS = "";
			/// <summary>
			/// 收货地址
			/// </summary>
			public string ADDRESS
			{
				get
				{
					return _ADDRESS;
				}
				set
				{
					_ADDRESS = (value==null)?"": value.Replace("'", "");
				}
			}

			string _WL_COMPANYID = "";
			/// <summary>
			/// 回写用的物流ID
			/// </summary>
			public string WL_COMPANYID
			{
				get
				{
					return _WL_COMPANYID;
				}
				set
				{
					_WL_COMPANYID = (value==null)?"": value.Replace("'", "");
				}
			}
			string _WL_NUMBER = "";
			/// <summary>
			/// 回写用的物流单号
			/// </summary>
			public string WL_NUMBER
			{
				get
				{
					return _WL_NUMBER;
				}
				set
				{
					_WL_NUMBER = (value==null)?"": value.Replace("'", "");
				}
			}
			string _WL_COMPANYNAME = "";
			/// <summary>
			/// 回写用的物流公司中文名
			/// </summary>
			public string WL_COMPANYNAME
			{
				get
				{
					return _WL_COMPANYNAME;
				}
				set
				{
					_WL_COMPANYNAME = (value==null)?"": value.Replace("'", "");
				}
			}
			string _RECVNAME = "";
			/// <summary>
			/// 收货人
			/// </summary>
			public string RECVNAME
			{
				get
				{
					return _RECVNAME;
				}
				set
				{
					_RECVNAME = (value==null)?"": value.Replace("'", "");
				}
			}
			string _RECVPHONE = "";
			/// <summary>
			/// 收货人
			/// </summary>
			public string RECVPHONE
			{
				get
				{
					return _RECVPHONE;
				}
				set
				{
					_RECVPHONE = (value==null)?"": value.Replace("'", "");
				}
			}
			string _PROVINCENAME = "";
			/// <summary>
			/// 收货省
			/// </summary>
			public string PROVINCENAME
			{
				get
				{
					return _PROVINCENAME;
				}
				set
				{
					_PROVINCENAME = (value==null)?"": value.Replace("'", "");
				}
			}
			string _CITYNAME = "";
			/// <summary>
			/// 收货市
			/// </summary>
			public string CITYNAME
			{
				get
				{
					return _CITYNAME;
				}
				set
				{
					_CITYNAME = (value==null)?"": value.Replace("'", "");
				}
			}
			string _DISTRICTNAME = "";
			/// <summary>
			/// 收货 区
			/// </summary>
			public string DISTRICTNAME
			{
				get
				{
					return _DISTRICTNAME;
				}
				set
				{
					_DISTRICTNAME = (value==null)?"": value.Replace("'", "");
				}
			}

			string _STREETNAME = "";
			/// <summary>
			/// 收货 街道
			/// </summary>
			public string STREETNAME
			{
				get
				{
					return _STREETNAME;
				}
				set
				{
					_STREETNAME = (value==null)?"": value.Replace("'", "");
				}
			}
			string _ORIGINALREMARK = "";
			/// <summary>
			/// 备注整体
			/// </summary>
			public string ORIGINALREMARK
			{
				get
				{
					return _ORIGINALREMARK;
				}
				set
				{
					_ORIGINALREMARK = (value==null)?"": value.Replace("'", "");
				}
			}
			string _IsFp = "";
			/// <summary>
			/// 是否需要发票备注
			/// </summary>
			public string IsFp
			{
				get
				{
					return _IsFp;
				}
				set
				{
					_IsFp = (value==null)?"": value.Replace("'", "");
				}
			}
			string _IsPj = "";
			/// <summary>
			/// 是否需要货品批件
			/// </summary>
			public string IsPj
			{
				get
				{
					return _IsPj;
				}
				set
				{
					_IsPj = (value==null)?"": value.Replace("'", "");
				}

			}
			string _IsHeTong = "";
			/// <summary>
			/// 购销合同
			/// </summary>
			public string IsHeTong
			{
				get
				{
					return _IsHeTong;
				}
				set
				{
					_IsHeTong = (value == null) ? "" : value.Replace("'", "");
				}
			}
			string _IsQysy = "";
			/// <summary>
			/// 企业首营
			/// </summary>
			public string IsQysy
			{
				get
				{
					return _IsQysy;
				}
				set
				{
					_IsQysy = (value==null)?"": value.Replace("'", "");
				}
			}
			string _IsSyzz = "";
			/// <summary>
			/// 商品首营资质
			/// </summary>
			public string IsSyzz
			{
				get
				{
					return _IsSyzz;
				}
				set
				{
					_IsSyzz = (value==null)?"": value.Replace("'", "");
				}
			}
			string _IsYjbb = "";
			/// <summary>
			/// 药检报告
			/// </summary>
			public string IsYjbb
			{
				get
				{
					return _IsYjbb;
				}
				set
				{
					_IsYjbb = (value==null)?"": value.Replace("'", "");
				}
			}
			Settings.Setings.EnumPlatformType _platformType = Settings.Setings.EnumPlatformType.无;
			public Settings.Setings.EnumPlatformType PlatformType
			{
				get
				{
					return _platformType;
				}
				set
				{
					_platformType = value;
				}
			}
			string _logi_dstRoute = "";
			/// <summary>
			/// 目的路由
			/// </summary>
			public string logi_dstRoute
			{
				get
				{
					return _logi_dstRoute;
				}
				set
				{
					_logi_dstRoute = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_PayType = "";
			/// <summary>
			/// 付款方式
			/// </summary>
			public string logi_PayType
			{
				get
				{
					return _logi_PayType;
				}
				set
				{
					_logi_PayType = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_monAccNum = "";
			/// <summary>
			/// 月结号
			/// </summary>
			public string logi_monAccNum
			{
				get
				{
					return _logi_monAccNum;
				}
				set
				{
					_logi_monAccNum = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_baojiaJine = "";
			/// <summary>
			/// 保价金额
			/// </summary>
			public string logi_baojiaJine
			{
				get
				{
					return _logi_baojiaJine;
				}
				set
				{
					_logi_baojiaJine = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_dsJine = "";
			/// <summary>
			/// 代收货款
			/// </summary>
			public string logi_dsJine
			{
				get
				{
					return _logi_dsJine;
				}
				set
				{
					_logi_dsJine = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_logcNum = "";
			/// <summary>
			/// 卡号
			/// </summary>
			public string logi_logcNum
			{
				get
				{
					return _logi_logcNum;
				}
				set
				{
					_logi_logcNum = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_ysJine = "";
			/// <summary>
			/// 运费
			/// </summary>
			public string logi_ysJine
			{
				get
				{
					return _logi_ysJine;
				}
				set
				{
					_logi_ysJine = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_ysJineTotal = "";
			/// <summary>
			/// 运费合计
			/// </summary>
			public string logi_ysJineTotal
			{
				get
				{
					return _logi_ysJineTotal;
				}
				set
				{
					_logi_ysJineTotal = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_shouhuory = "";
			/// <summary>
			/// 收件员
			/// </summary>
			public string logi_shouhuory
			{
				get
				{
					return _logi_shouhuory;
				}
				set
				{
					_logi_shouhuory = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_jinjianRiqi = "";
			/// <summary>
			/// 寄件日期
			/// </summary>
			public string logi_jinjianRiqi
			{
				get
				{
					return _logi_jinjianRiqi;
				}
				set
				{
					_logi_jinjianRiqi = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_shoufqianshu = "";
			/// <summary>
			/// 收方签署
			/// </summary>
			public string logi_shoufqianshu
			{
				get
				{
					return _logi_shoufqianshu;
				}
				set
				{
					_logi_shoufqianshu = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_shoufRiqi = "";
			/// <summary>
			/// 收方日期
			/// </summary>
			public string logi_shoufRiqi
			{
				get
				{
					return _logi_shoufRiqi;
				}
				set
				{
					_logi_shoufRiqi = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_sendSheng = "";
			/// <summary>
			/// 寄件 省	
			/// </summary>
			public string logi_sendSheng
			{
				get
				{
					return _logi_sendSheng;
				}
				set
				{
					_logi_sendSheng = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_sendShi = "";
			/// <summary>
			/// 寄件 市	
			/// </summary>
			public string logi_sendShi
			{
				get
				{
					return _logi_sendShi;
				}
				set
				{
					_logi_sendShi = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_sendXian = "";
			/// <summary>
			/// 寄件 县	
			/// </summary>
			public string logi_sendXian
			{
				get
				{
					return _logi_sendXian;
				}
				set
				{
					_logi_sendXian = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_sendAddress = "";
			/// <summary>
			/// 寄件 地址
			/// </summary>
			public string logi_sendAddress
			{
				get
				{
					return _logi_sendAddress;
				}
				set
				{
					_logi_sendAddress = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_sendMan = "";
			/// <summary>
			/// 寄件 人	
			/// </summary>
			public string logi_sendMan
			{
				get
				{
					return _logi_sendMan;
				}
				set
				{
					_logi_sendMan = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_sendPhone = "";
			/// <summary>
			/// 寄件 电话	
			/// </summary>
			public string logi_sendPhone
			{
				get
				{
					return _logi_sendPhone;
				}
				set
				{
					_logi_sendPhone = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_feiyongTotal = "";
			/// <summary>
			/// 费用合计
			/// </summary>
			public string logi_feiyongTotal
			{
				get
				{
					return _logi_feiyongTotal;
				}
				set
				{
					_logi_feiyongTotal = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_goodQty = "";
			/// <summary>
			/// 货品数量
			/// </summary>
			public string logi_goodQty
			{
				get
				{
					return _logi_goodQty;
				}
				set
				{
					_logi_goodQty = (value==null)?"": value.Replace("'", "");
				}
			}

			string _logi_goodName = "";
			/// <summary>
			/// 货品名称
			/// </summary>
			public string logi_goodName
			{
				get
				{
					return _logi_goodName;
				}
				set
				{
					_logi_goodName = (value==null)?"": value.Replace("'", "");
				}
			}
			string _logi_OrderId = "";
			/// <summary>
			/// 物流ID
			/// </summary>
			public string logi_OrderId
			{
				get
				{
					return _logi_OrderId;
				}
				set
				{
					_logi_OrderId = (value == null) ? "" : value.Replace("'", "");
				}
			}
			string _logi_CreateDate = "";
			/// <summary>
			/// 创建物流日期
			/// </summary>
			public string logi_CreateDate
			{
				get
				{
					return _logi_CreateDate;
				}
				set
				{
					_logi_CreateDate = (value == null) ? "" : value.Replace("'", "");
				}
			}
			double _needBaoJia = 0;
			/// <summary>
			/// 要求报价金额 decimal(18,3)
			/// </summary>
			public double needBaojia
			{
				get
				{
					return _needBaoJia;
				}
				set
				{
					_needBaoJia = value;
				}
			}
			string _logi_ChanpinTypeStr="";
			/// <summary>
			/// 使用的物流产品类型
			/// </summary>
			public string logi_ChanpinTypeStr
			{
				get
				{
					return _logi_ChanpinTypeStr;
				}
				set
				{
					_logi_ChanpinTypeStr = (value == null) ? "" : value.Replace("'", "");
				}
			}
			string _PrintDatetime = "";
			/// <summary>
			/// 最后打印日期
			/// </summary>
			public string PrintDatetime
			{
				get
				{
					return _PrintDatetime;
				}
				set
				{
					_PrintDatetime = (value == null) ? "" : value.Replace("'", "");
				}
			}
			string _sysFirst = "";
			/// <summary>
			/// 系统检测是否首次业务
			/// </summary>
			public string sysFirst
			{
				get
				{
					return _sysFirst;
				}
				set
				{
					_sysFirst = (value == null) ? "" : value.Replace("'", "");
				}
			}
			string _total_amt = "0";
			/// <summary>
			/// 单据总金额
			/// </summary>
			public string total_amt
			{
				get
				{
					if (YJT.Text.Verification.IsDouble(_total_amt))
					{
						return _total_amt.ToString();
					}
					else
					{
						return "0";
					}
				}
				set
				{
					if (YJT.Text.Verification.IsNullOrEmpty(value))
					{
						_total_amt = "0";
					}
					else
					{
						if (YJT.Text.Verification.IsDouble(value))
						{
							_total_amt = value;
						}
						else
						{
							_total_amt = "0";
						}
					}
					
				}
			}
			long _relBId = -1;
			/// <summary>
			/// 相关新单据(一般对应补打的无实体任务类型)单据,仅存在于客户端
			/// </summary>
			public long RelBId
			{
				get
				{
					return _relBId;
				}
				set
				{
					_relBId = value;
				}
			}
			string _fplx = "";
			/// <summary>
			/// 发票类型,1普通发票2专用发票,其他为空
			/// </summary>
			public string fplx
			{
				get
				{
					return _fplx;
				}
				set
				{
					if (_platformType == Settings.Setings.EnumPlatformType.药师帮)
					{
						if (YJT.Text.Verification.IsIn(value, false, "1", "普通发票", "普票"))
						{
							_fplx = "普通发票";
						}
						else if (YJT.Text.Verification.IsIn(value, false, "2", "专用发票", "专票"))
						{
							_fplx = "专用发票";
						}
						else
						{
							_fplx = "";
						}
					}
					else if (_platformType == Settings.Setings.EnumPlatformType.小药药)
					{
						//1、电子普通发票；2、增值税专用发票；3、纸质普通发票；4、增值税电子专用发票
						if (YJT.Text.Verification.IsIn(value, false, "1", "3", "普通发票", "普票", "电子普通发票", "纸质普通发票"))
						{
							_fplx = "普通发票";
						}
						else if (YJT.Text.Verification.IsIn(value, false, "2", "4", "专用发票", "专票", "增值税专用发票", "增值税电子专用发票"))
						{
							_fplx = "专用发票";
						}
						else
						{
							_fplx = "";
						}
					}
					else if (_platformType == Settings.Setings.EnumPlatformType.药京采)
					{
						//1 普票,2 专票
						if (YJT.Text.Verification.IsIn(value, false, "1", "普通发票", "普票", "电子普通发票", "纸质普通发票"))
						{
							_fplx = "普通发票";
						}
						else if (YJT.Text.Verification.IsIn(value, false, "2","专用发票", "专票", "增值税专用发票", "增值税电子专用发票"))
						{
							_fplx = "专用发票";
						}
						else
						{
							_fplx = "";
						}
					}
					else if (_platformType == Settings.Setings.EnumPlatformType.无)
					{
						_fplx = "";
					}
					else
					{
						_fplx = "";
					}
					
				}
			}
			Settings.Setings.EnumServerTaskType _serverTaskType = Settings.Setings.EnumServerTaskType.无;
			public Settings.Setings.EnumServerTaskType ServerTaskType
			{
				get
				{
					return _serverTaskType;
				}
				set
				{
					_serverTaskType = value;
				}
			}
			double _PAIDCOSTS = 0;
			/// <summary>
			/// 客户已付运费金额,药师帮
			/// </summary>
			public double PAIDCOSTS
			{
				get
				{
					return _PAIDCOSTS;
				}
				set
				{
					_PAIDCOSTS = value;
				}
			}
			string _logi_ReceivePwd = "";
			/// <summary>
			/// 快递收货密码
			/// </summary>
			public string logi_ReceivePwd
			{
				get
				{
					return _logi_ReceivePwd;
				}
				set
				{
					_logi_ReceivePwd= (value == null) ? "" : value.Replace("'", "");
				}
			}
			int _logi_SubOrderSn = 0;
			/// <summary>
			/// 第几个子单
			/// </summary>
			public int logi_SubOrderSn
			{
				get
				{
					return _logi_SubOrderSn;
				}
				set
				{
					_logi_SubOrderSn = value;
				}
			}
			string _logi_PhonNum = "";
			/// <summary>
			/// 分析后的手机号码
			/// </summary>
			public string logi_PhonNum
			{
				get
				{
					return _logi_PhonNum;
				}
				set
				{
					_logi_PhonNum = (value == null) ? "" : value.Replace("'", "");
				}
			}
			string _logi_TelNum = "";
			/// <summary>
			/// 分析后的固定电话
			/// </summary>
			public string logi_TelNum
			{
				get
				{
					return _logi_TelNum;
				}
				set
				{
					_logi_TelNum = (value == null) ? "" : value.Replace("'", "");
				}
			}
			long _relOrderid = -1;
			/// <summary>
			/// 关联orderId,用于组合单
			/// </summary>
			public long RelOrderId
			{
				get
				{
					return _relOrderid;
				}
				set
				{
					_relOrderid = value;
				}
			}
			string _jingdongWl = "";
			public string JingdongWl
			{
				get
				{
					return _jingdongWl;
				}
				set
				{
					_jingdongWl= (value == null) ? "" : value.Replace("'", "''");
				}
			}

		}
	}
}
