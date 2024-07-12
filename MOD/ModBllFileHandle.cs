using System;

namespace MOD
{
	public class Mod_bllFileHandle
	{
		private long _id = 0;
		/// <summary>
		/// 文件ID  bigint(8,0)
		/// </summary>
		public long Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		private string _netPath = "";
		/// <summary>
		/// 网络路径 varchar(4000,0)
		/// </summary>
		public string NetPath
		{
			get
			{
				if (YJT.Text.Verification.IsNullOrEmpty(_netPath))
				{
					return "";
				}
				else
				{
					return _netPath;
				}
			}
			set
			{
				_netPath = value;
			}
		}
		private string _localPath = "";
		/// <summary>
		/// 本地路径 varchar(4000,0)
		/// </summary>
		public string LocalPath
		{
			get
			{
				if (YJT.Text.Verification.IsNullOrEmpty(_localPath))
				{
					return "";
				}
				else
				{
					return _localPath;
				}
			}
			set
			{
				_localPath = value;
			}
		}
		private string _goodsId = "";
		/// <summary>
		/// 货品ID varchar(1000,0)
		/// </summary>
		public string goodsId
		{
			get
			{
				if (YJT.Text.Verification.IsNullOrEmpty(_goodsId))
				{
					return "";
				}
				else
				{
					return _goodsId;
				}
			}
			set
			{
				_goodsId = value;
			}
		}
		private string _batchId = "";
		/// <summary>
		/// 货品批次ID varchar(1000,0)
		/// </summary>
		public string batchId
		{
			get
			{
				if (YJT.Text.Verification.IsNullOrEmpty(_batchId))
				{
					return "";
				}
				else
				{
					return _batchId;
				}
			}
			set
			{
				_batchId = value;
			}
		}
		private DateTime _addDate = new DateTime(0);
		/// <summary>
		/// 最后文件使用日期 datetime(8,3)
		/// </summary>
		public DateTime addDate
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
		private DateTime _lastReadTime = new DateTime(0);
		/// <summary>
		/// 服务器文件上传日期 datetime(8,3)
		/// </summary>
		public DateTime lastReadTime
		{
			get
			{
				return _lastReadTime;
			}
			set
			{
				_lastReadTime = value;
			}
		}
		private int _usedCount = 0;
		/// <summary>
		/// 文件被使用次数 int(4,0)
		/// </summary>
		public int usedCount
		{
			get
			{
				return _usedCount;
			}
			set
			{
				_usedCount = value;
			}
		}
		private string _fileName = "";
		/// <summary>
		/// 文件名,不包含路径,包含后缀名 varchar(1000,0)
		/// </summary>
		public string fileName
		{
			get
			{
				if (YJT.Text.Verification.IsNullOrEmpty(_fileName))
				{
					return "";
				}
				else
				{
					return _fileName;
				}
			}
			set
			{
				_fileName = value;
			}
		}
		private int _fileSize = 0;
		/// <summary>
		/// 文件大小 int(4,0)
		/// </summary>
		public int fileSize
		{
			get
			{
				return _fileSize;
			}
			set
			{
				_fileSize = value;
			}
		}
		private string _erpFileGroupId = "";
		public string erpFileGroupId
		{
			get
			{
				return _erpFileGroupId;
			}
			set
			{
				_erpFileGroupId = (value == null) ? "" : value;
			}
		}

		private string _fileType = "";
		/// <summary>
		/// 文件类型,暂定 路单药检,药品批件
		/// </summary>

		public string FileType
		{
			get
			{
				return _fileType;
			}
			set
			{
				_fileType = (value == null) ? "" : value;
			}
		}
		private string _accountInfo = "";
		/// <summary>
		/// 帐套信息,比如 汇达,燕都
		/// </summary>
		public string AccountInfo
		{
			get
			{
				return _accountInfo;
			}
			set
			{
				_accountInfo = (value == null) ? "" : value;
			}
		}
		public override string ToString()
		{
			return _id.ToString();
		}
	}

}
