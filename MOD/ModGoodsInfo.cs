using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOD
{
	public class ModGoodsInfo
	{
		string _goodsid = "";
		public string GoodsId
		{
			get
			{
				return _goodsid;
			}
			set
			{
				_goodsid = (value != null) ? value : "";
			}
		}
		string _goodsName = "";
		public string GoodsName
		{
			get
			{
				return _goodsName;
			}
			set
			{
				_goodsName = (value != null) ? value : "";
			}
		}
		public DateTime? Xiaoqizhi
		{
			get; set;
		} = null;
		public string FileGroupId
		{
			get; set;
		} = "";
		public string FileSopcode
		{
			get; set;
		} = "";
		public List<MOD.Mod_bllFileHandle> GoodsPic
		{
			get;
			set;
		} = new List<Mod_bllFileHandle>();
		public bool IsExistsFile
		{
			get;
			set;
		} = false;

	}

}
