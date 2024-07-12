using System.Collections.Generic;

namespace MOD
{
	public class PrintObj
	{
		public string DdwName
		{
			get; set;
		} = "";
		public string DdwId
		{
			get; set;
		} = "";
		public string ErpId
		{
			get; set;
		} = "";
		public string BussinesDep
		{
			get; set;
		} = "";
		public string LineRoute
		{
			get; set;
		} = "";
		public string WmsWareBH
		{
			get; set;
		} = "";
		public string WmsWareClass
		{
			get; set;
		} = "";
		public string GoodsClass
		{
			get; set;
		} = "";
		public string PihaoS
		{
			get; set;
		} = "";
		public string Yewy
		{
			get; set;
		} = "";
		public PrintObj(string ddwname, string ddwId, string erpId, string yewy)
		{
			this.DdwName = ddwname;
			this.DdwId = ddwId;
			this.Yewy = yewy;
			this.ErpId = erpId;
		}
		public List<MOD.HdErp.HdErpHeTong> HeTongs
		{
			get;
			set;
		} = new List<HdErp.HdErpHeTong>();
		public List<MOD.ModGoodsInfo> Pjs
		{
			get;
			set;
		} = new List<ModGoodsInfo>();
		public List<MOD.ModGoodsInfo> Ldyjs
		{
			get;
			set;
		} = new List<ModGoodsInfo>();
	}
}
