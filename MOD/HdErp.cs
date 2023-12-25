using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOD.HdErp
{
	public class HdErpHeTong
	{
		DateTime? _confirmdate = null;
		/// <summary>
		/// 确认日期
		/// </summary>
		public string Confirmdate
		{
			get
			{
				if (_confirmdate == null)
				{
					return "";
				}
				else
				{
					return _confirmdate.Value.ToString("yyyy-MM-dd");
				}
			}
			set
			{
				_confirmdate= YJT.DataBase.Common.ObjectTryToObj<DateTime?>(value, null);
			}
		}
		string _salesid = "";
		/// <summary>
		/// 总单ID
		/// </summary>
		public string salesid
		{
			get
			{
				return _salesid;
			}
			set
			{
				if (YJT.Text.Verification.IsNullOrEmpty(value))
				{
					_salesid = "";
				}
				else
				{
					_salesid = value;
				}
			}
		}
		string _goodsName = "";
		/// <summary>
		/// 药品名称
		/// </summary>
		public string GoodsName
		{
			get
			{
				return _goodsName;
			}
			set
			{
				if (YJT.Text.Verification.IsNullOrEmpty(value))
				{
					_goodsName = "";
				}
				else
				{
					_goodsName = value;
				}
			}
		}

		string _factoryName = "";
		/// <summary>
		/// 生产厂家
		/// </summary>
		public string FactoryName
		{
			get
			{
				return _factoryName;
			}
			set
			{
				if (YJT.Text.Verification.IsNullOrEmpty(value))
				{
					_factoryName = "";
				}
				else
				{
					_factoryName = value;
				}
			}
		}

		string _goodsType = "";
		/// <summary>
		/// 商品规格
		/// </summary>
		public string GoodsType
		{
			get
			{
				return _goodsType;
			}
			set
			{
				if (YJT.Text.Verification.IsNullOrEmpty(value))
				{
					_goodsType = "";
				}
				else
				{
					_goodsType = value;
				}
			}
		}
		string _goodsUnit = "";
		/// <summary>
		/// 货品单位
		/// </summary>
		public string GoodsUnit
		{
			get
			{
				return _goodsUnit;
			}
			set
			{
				if (YJT.Text.Verification.IsNullOrEmpty(value))
				{
					_goodsUnit = "";
				}
				else
				{
					_goodsUnit = value;
				}
			}
		}
		double _goodsQty = 0d;
		/// <summary>
		/// 单品数量
		/// </summary>
		public double? GoodsQty
		{
			get
			{
				return _goodsQty;
			}
			set
			{
				if (value == null)
				{
					_goodsQty = 0d;
				}
				else
				{
					_goodsQty = value.Value;
				}
			}
		}
		double _unitprice = 0d;
		/// <summary>
		/// 单品单价
		/// </summary>
		public double? Unitprice
		{
			get
			{
				return _unitprice;
			}
			set
			{
				if (value == null)
				{
					_unitprice = 0d;
				}
				else
				{
					_unitprice = value.Value;
				}
			}
		}
		double _total_line = 0d;
		/// <summary>
		/// 单品金额
		/// </summary>
		public double? Total_line
		{
			get
			{
				return _total_line;
			}
			set
			{
				if (value == null)
				{
					_total_line = 0d;
				}
				else
				{
					_total_line = value.Value;
				}
			}
		}
		string _dtlMemo = "";
		/// <summary>
		/// 细单备注
		/// </summary>
		public string DtlMemo
		{
			get
			{
				return _dtlMemo;
			}
			set
			{
				if (YJT.Text.Verification.IsNullOrEmpty(value))
				{
					_dtlMemo = "";
				}
				else
				{
					_dtlMemo = value;
				}
			}
		}
		double _totalLine = 0d;
		/// <summary>
		/// 整单金额
		/// </summary>
		public double? TotalLine
		{
			get
			{
				return _totalLine;
			}
			set
			{
				if (value == null)
				{
					_totalLine = 0d;
				}
				else
				{
					_totalLine = value.Value;
				}
			}
		}
		string _customName = "";
		/// <summary>
		/// 购方名称
		/// </summary>
		public string CustomName
		{
			get
			{
				return _customName;
			}
			set
			{
				if (YJT.Text.Verification.IsNullOrEmpty(value))
				{
					_customName = "";
				}
				else
				{
					_customName = value;
				}
			}
		}


	}
}
