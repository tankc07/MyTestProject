using System;
using System.Collections.Generic;
using MOD;

namespace BLL
{
	public class FileHandle
	{
		static YJT.DataBase.DbHelper _ecSqlServer = new YJT.DataBase.DbHelperSqlServer("172.16.1.15", "YanduECommerceAutomaticPrinting", "dsby", "dsby", "3341");
		public enum EnumFtpCompany
		{
			汇达ErpFtp,
			none
		}
		//燕都
		//YJT.Ftp _ftpYd = new YJT.Ftp(@"172.16.1.243:9988", "a", "b"); 
		//汇达
		static YJT.Ftp _ftpHd = new YJT.Ftp(@"172.16.1.244:9988", "a", "b");
		/// <summary>
		/// 检查FTP
		/// </summary>
		/// <param name="ftpType"></param>
		/// <returns></returns>
		public static bool VerFtp(EnumFtpCompany ftpType)
		{
			bool res = false;
			switch (ftpType)
			{
				case EnumFtpCompany.汇达ErpFtp:
					for (int i = 0; i < 10; i++)
					{
						if (_ftpHd.ExistDirectory("100014"))
						{
							res = true;
							break;
						}
					}


					break;
				case EnumFtpCompany.none:
					throw new Exception("要检测的FTP参数不正确");
				default:
					break;
			}

			return res;
		}

		/// <summary>
		/// v1需要删除的文件个数,v成功个数,v3错误消息
		/// </summary>
		/// <returns></returns>
		public static YJT.ValuePara.MulitValue.TwoValueClass<int, int, string> DelOldFile()
		{
			YJT.ValuePara.MulitValue.TwoValueClass<int, int, string> res = new YJT.ValuePara.MulitValue.TwoValueClass<int, int, string>();
			res.V1 = 0;
			res.V2 = 0;
			res.V3 = "";
			string sqlSelectCmd = @"
SELECT Id ,
		NetPath ,
		LocalPath ,
		goodsId ,
		batchId ,
		addDate ,
		lastReadTime ,
		usedCount ,
		fileName ,
		erpFileGroupId,
		fileSize FROM bllFileHandle WHERE CONVERT(DATETIME,CONVERT(VARCHAR(10),DATEADD(dd,-180,GETDATE()),21))>addDate
";
			string sqlDelCmd = @"
DELETE bllFileHandle WHERE Id={0}
";
			System.Data.DataTable dt = _ecSqlServer.ExecuteToDataTable(sqlSelectCmd, null, true);
			if (dt.Rows.Count > 0)
			{
				res.V1 = dt.Rows.Count;
				foreach (System.Data.DataRow item in dt.Rows)
				{
					string localPath = YJT.DataBase.Common.ObjectTryToObj(item["LocalPath"], "");
					string id = YJT.DataBase.Common.ObjectTryToObj(item["Id"], "");
					if (YJT.Text.Verification.IsNotNullOrEmpty(localPath))
					{
						if (System.IO.File.Exists(localPath))
						{
							try
							{
								System.IO.File.Delete(localPath);
							}
							catch (Exception ee)
							{
								res.V3 = res.V3 + "文件删除异常" + ee.ToString();
							}
							if (!System.IO.File.Exists(localPath))
							{
								int dbres = _ecSqlServer.ExecuteNonQuery(string.Format(sqlDelCmd, id), null, System.Data.CommandType.Text, false, true, "");
								if (dbres > 0)
								{
									res.V2++;
								}
								else
								{
									//数据库删除失败
									res.V3 = res.V3 + "数据库删除失败:id=" + id;
								}
							}
							else
							{
								//文件删除失败
								res.V3 = res.V3 + "文件删除失败:路径=" + localPath;
							}

						}
					}
					else
					{
						//文件不存在
						res.V3 = res.V3 + "文件不存在:路径=" + localPath;
					}

				}
			}
			else
			{
				//没有要删除的文件
				res.V3 = res.V3 + "没有要删除的文件";
			}
			try
			{
				dt.Dispose();
			}
			catch
			{
			}
			dt = null;
			return res;

		}
		public static YJT.ValuePara.MulitValue.TwoValueClass<int, int, string> DelOldFile(long fileId)
		{
			//按ID删除文件
			YJT.ValuePara.MulitValue.TwoValueClass<int, int, string> res = new YJT.ValuePara.MulitValue.TwoValueClass<int, int, string>();
			res.V1 = 1;
			res.V2 = 0;
			res.V3 = "";
			string sqlCmd = $@"DELETE bllFileHandle OUTPUT Deleted.LocalPath WHERE Id={fileId.ToString()}";
			object dbres = _ecSqlServer.ExecuteScalar(sqlCmd, null, true);
			string dbresStr = YJT.DataBase.Common.ObjectTryToObj(dbres, "");
			if (YJT.Text.Verification.IsNotNullOrEmpty(dbresStr))
			{
				try
				{
					System.IO.File.Delete(dbresStr);
					res.V2 = 1;
					res.V3 = "";
				}
				catch {
					res.V2 = 1;
					res.V3 = "磁盘文件删除失败";
				}
			}
			else
			{
				res.V2 = 0;
				res.V3 = "没有删除数据";
			}


			

			return res;
		}
		public static List<Mod_bllFileHandle> GetWmsGoodsLdyjFile(ModGoodsInfo t)
		{
			List<MOD.Mod_bllFileHandle> res = new List<Mod_bllFileHandle>();
			if (YJT.Text.Verification.IsNotNullOrEmpty(t.FileGroupId))
			{
				MOD.SysMod.ClinetTag u = YJT.StaticResources.Get<MOD.SysMod.ClinetTag>("userObj");
				string localPath = @"D:\ECDownloadFileLDYJ";
				if (!u.Ip.Contains(@"172.16.2.240"))
				{
					localPath = @"\\172.16.2.240\debugec\ECDownloadFileLDYJ";
				}

				if (!System.IO.Directory.Exists(localPath))
				{
					System.IO.Directory.CreateDirectory(localPath);
				}
				string serverPath = $@"\\172.16.1.238\e\yjd\电子档案根目录\功能电子档案\1912\出入库明细\{t.FileGroupId}\药检单";
				if (System.IO.Directory.Exists(serverPath))
				{
					string[] filesA= System.IO.Directory.GetFiles(serverPath);
					List<string> filesB = new List<string>();
					if (filesA != null && filesA.Length > 0)
					{
						foreach (string item in filesA)
						{
							if (YJT.Text.Verification.IsRightLike(item.ToLower(), new List<string> { ".jpg", ".png", ".bmp" }))
							{
								filesB.Add(item);
							}
						}
					}
					if (filesB.Count > 0)
					{
						
						string sqlCmd = $@"
SELECT Id ,
		NetPath ,
		LocalPath ,
		goodsId ,
		batchId ,
		addDate ,
		lastReadTime ,
		usedCount ,
		fileName ,
		fileSize ,
		erpFileGroupId ,
		FileType ,
		AccountInfo FROM bllFileHandle where goodsId='{t.GoodsId}' AND erpFileGroupId='{t.FileGroupId}' and FileType='路单药检' AND AccountInfo='WMS'
";
						System.Data.DataTable dt = _ecSqlServer.ExecuteToDataTable(sqlCmd, null, true);
						if (dt.Rows.Count > 0 && dt.Rows.Count == filesB.Count)
						{
							//本地与服务器图片数量一致
							foreach (System.Data.DataRow item in dt.Rows)
							{
								MOD.Mod_bllFileHandle tMod = new Mod_bllFileHandle()
								{
									AccountInfo = YJT.DataBase.Common.ObjectTryToObj(item["AccountInfo"], ""),
									addDate = YJT.DataBase.Common.ObjectTryToObj(item["addDate"], DateTime.MinValue),
									batchId = YJT.DataBase.Common.ObjectTryToObj(item["batchId"], ""),
									erpFileGroupId = YJT.DataBase.Common.ObjectTryToObj(item["erpFileGroupId"], ""),
									fileName = YJT.DataBase.Common.ObjectTryToObj(item["fileName"], ""),
									fileSize = YJT.DataBase.Common.ObjectTryToObj(item["fileSize"], 0),
									FileType = YJT.DataBase.Common.ObjectTryToObj(item["FileType"], ""),
									goodsId = YJT.DataBase.Common.ObjectTryToObj(item["goodsId"], ""),
									Id = YJT.DataBase.Common.ObjectTryToObj(item["Id"], -1L),
									lastReadTime = YJT.DataBase.Common.ObjectTryToObj(item["lastReadTime"], DateTime.MinValue),
									LocalPath = YJT.DataBase.Common.ObjectTryToObj(item["LocalPath"], ""),
									NetPath = YJT.DataBase.Common.ObjectTryToObj(item["NetPath"], ""),
									usedCount = YJT.DataBase.Common.ObjectTryToObj(item["usedCount"], 0)
								};
								if (!u.Ip.Contains(@"172.16.2.240"))
								{
									tMod.LocalPath = tMod.LocalPath.Replace(@"D:\", @"\\172.16.2.240\debugec\");
								}
								if (YJT.Text.Verification.IsRightLike(tMod.LocalPath.ToLower(), new List<string> { ".jpg", ".png", ".bmp" }))
								{
									res.Add(tMod);
								}

							}
						}
						else
						{
							//与服务器图片数量不一致
							//曾经下载过,需要先删一边
							if (dt.Rows.Count != filesB.Count && dt.Rows.Count > 0)
							{
								long fid = 0L;
								foreach (System.Data.DataRow item in dt.Rows)
								{
									fid = YJT.DataBase.Common.ObjectTryToObj<long>(item["Id"], 0L);
									DelOldFile(fid);
								}
							}
							//开始下载
							DateTime addDateNow = DateTime.Now;
							foreach (string item in filesB)
							{
								System.IO.FileInfo fi = new System.IO.FileInfo(item);
								MOD.Mod_bllFileHandle tMod = new Mod_bllFileHandle();
								tMod.addDate = addDateNow;
								tMod.batchId = "";
								tMod.erpFileGroupId = t.FileGroupId;
								tMod.fileName = fi.Name;
								tMod.fileSize = (int)fi.Length;
								tMod.goodsId = t.GoodsId;
								tMod.Id = -1;
								tMod.lastReadTime = fi.LastWriteTime;
								tMod.NetPath =item;
								tMod.LocalPath = "";
								tMod.usedCount = 1;
								tMod.FileType = "路单药检";
								tMod.AccountInfo = "WMS";
								string fileExtName = "";
								fileExtName = fi.Extension;
								if (YJT.Text.Verification.IsNullOrEmpty(fileExtName))
								{
									fileExtName = ".tmp";
								}
								fileExtName = YJT.Text.Format.RemoveChars(fileExtName, YJT.Text.Format.EnumCharDirection.Left, ".");
								do
								{
									tMod.LocalPath = YJT.Path.GetTempFilePath(fileExtName, "yj", localPath);
								} while (System.IO.File.Exists(tMod.LocalPath));

								for (int i = 0; i < 10; i++)
								{
									try
									{
										fi.CopyTo(tMod.LocalPath, false);
										string sqlCmd2 = "";
										sqlCmd2 = $@"
INSERT INTO bllFileHandle
		(
			NetPath ,
			LocalPath ,
			goodsId ,
			batchId ,
			addDate ,
			lastReadTime ,
			usedCount ,
			fileName ,
			fileSize ,
			erpFileGroupId ,
			FileType ,
			AccountInfo
		)
OUTPUT inserted.Id
	VALUES
		(
			'{tMod.NetPath}' , -- NetPath - varchar(4000)
			'{tMod.LocalPath.Replace(@"\\172.16.2.240\debugec\", @"D:\")}' , -- LocalPath - varchar(4000)
			'{tMod.goodsId}' , -- goodsId - varchar(1000)
			'{tMod.batchId}' , -- batchId - varchar(1000)
			convert(datetime,'{tMod.addDate.ToString("yyyy-MM-dd HH:mm:ss")}') , -- addDate - datetime
			convert(datetime,'{tMod.lastReadTime.ToString("yyyy-MM-dd HH:mm:ss")}') , -- lastReadTime - datetime
			{tMod.usedCount.ToString()} , -- usedCount - int
			'{tMod.fileName}' , -- fileName - varchar(1000)
			{tMod.fileSize.ToString()} , -- fileSize - int
			'{tMod.erpFileGroupId}' , -- erpFileGroupId - varchar(1000)
			'{tMod.FileType}' , -- FileType - varchar(100)
			'{tMod.AccountInfo}'  -- AccountInfo - varchar(100)
		)
";
										System.Data.DataTable dt2 = _ecSqlServer.ExecuteToDataTable(sqlCmd2, null, true);
										if (dt2.Rows.Count > 0)
										{
											tMod.Id = YJT.DataBase.Common.ObjectTryToObj(dt2.Rows[0]["Id"], -1L);
											res.Add(tMod);
										}
										else
										{
											//插入记录失败
											//此文件无法写入到数据库
											try
											{
												System.IO.File.Delete(tMod.LocalPath);
											}
											catch
											{
											}
										}
										break;
									}
									catch
									{

									}
								}
							}
						}
					}
					else
					{
						//没有图片存在
					}
				}
				else
				{
					//服务器端不存在指定目录
				}
			}
			else
			{
				//子目录文件夹不存在
			}
			return res;

		}

		public static bool VerSerPicPath()
		{
			bool res = false;
			try
			{
				if (System.IO.Directory.Exists(@"\\172.16.2.240\debugec\ECDownloadFile"))
				{
					if (System.IO.Directory.Exists(@"\\172.16.2.240\debugec\ECDownloadFileLDYJ"))
					{
						res = true;
					}
				}
			}
			catch
			{
				res = false;
			}
			
			return res;
		}

		/// <summary>
		/// 新增一个商品资质
		/// </summary>
		/// <param name="goodsId"></param>
		/// <param name="folderName"></param>
		/// <param name="batchId"></param>
		/// <returns></returns>

		public static List<Mod_bllFileHandle> GetHdGoodsQualificationFile(string goodsId, string folderName, string batchId)
		{
			batchId = batchId.Replace("'","");
			goodsId = goodsId.Replace("'", "");
			List<MOD.Mod_bllFileHandle> res = new List<Mod_bllFileHandle>();
			//Mod_bllFileHandle t = new Mod_bllFileHandle()
			//{
			//	addDate = null,
			//	batchId = "",
			//	erpFileGroupId = "",
			//	fileName = "",
			//	fileSize = "",
			//	goodsId = "",
			//	Id = "",
			//	lastReadTime = "",
			//	LocalPath = "",
			//	NetPath = "",
			//	usedCount = 0,
			//};
			string localPath = @"D:\ECDownloadFile";
			MOD.SysMod.ClinetTag u = YJT.StaticResources.Get<MOD.SysMod.ClinetTag>("userObj");
			if (u == null)
			{
				localPath = @"D:\ECDownloadFile";
			}
			else
			{
				if (u.Ip.Contains(@"172.16.2.240"))
				{
					localPath = @"D:\ECDownloadFile";
				}
				else
				{
					localPath = @"\\172.16.2.240\debugec\ECDownloadFile";
				}
			}

			
			if (!System.IO.Directory.Exists(localPath))
			{
				System.IO.Directory.CreateDirectory(localPath);
			}
			//获取FTP上文件个数
			List<string> t1 = _ftpHd.GetFileList(folderName);

			string sqlCmd = $@"
SELECT Id ,
		NetPath ,
		LocalPath ,
		goodsId ,
		batchId ,
		addDate ,
		lastReadTime ,
		usedCount ,
		fileName ,
		fileSize ,
		erpFileGroupId ,
		FileType ,
		AccountInfo FROM bllFileHandle where goodsId='{goodsId}' AND batchId='{batchId}' AND erpFileGroupId='{folderName}' and FileType='药品批件' AND AccountInfo='汇达'
";
			System.Data.DataTable dt = _ecSqlServer.ExecuteToDataTable(sqlCmd, null, true);
			if (dt.Rows.Count > 0 && dt.Rows.Count == t1.Count)
			{
				//找到图片直接打印
				foreach (System.Data.DataRow item in dt.Rows)
				{
					MOD.Mod_bllFileHandle t = new Mod_bllFileHandle()
					{
						 AccountInfo = YJT.DataBase.Common.ObjectTryToObj(item["AccountInfo"], ""),
						 addDate = YJT.DataBase.Common.ObjectTryToObj(item["addDate"], DateTime.MinValue),
						 batchId = YJT.DataBase.Common.ObjectTryToObj(item["batchId"], ""),
						 erpFileGroupId = YJT.DataBase.Common.ObjectTryToObj(item["erpFileGroupId"], ""),
						 fileName = YJT.DataBase.Common.ObjectTryToObj(item["fileName"], ""),
						 fileSize = YJT.DataBase.Common.ObjectTryToObj(item["fileSize"], 0),
						 FileType = YJT.DataBase.Common.ObjectTryToObj(item["FileType"], ""),
						 goodsId = YJT.DataBase.Common.ObjectTryToObj(item["goodsId"], ""),
						 Id = YJT.DataBase.Common.ObjectTryToObj(item["Id"], -1L),
						 lastReadTime = YJT.DataBase.Common.ObjectTryToObj(item["lastReadTime"], DateTime.MinValue),
						 LocalPath = YJT.DataBase.Common.ObjectTryToObj(item["LocalPath"], ""),
						 NetPath = YJT.DataBase.Common.ObjectTryToObj(item["NetPath"], ""),
						 usedCount = YJT.DataBase.Common.ObjectTryToObj(item["usedCount"], 0)
					};
					if (!u.Ip.Contains(@"172.16.2.240"))
					{
						t.LocalPath = t.LocalPath.Replace(@"D:\", @"\\172.16.2.240\debugec\");
					}

					if (YJT.Text.Verification.IsRightLike(t.LocalPath.ToLower(), new List<string> { ".jpg", ".png", ".bmp" }))
					{
						res.Add(t);
					}
						
				}

			}
			else
			{
				//曾经下载过,需要先删一边
				if (dt.Rows.Count != t1.Count && dt.Rows.Count>0)
				{
					long fid = 0L;
					foreach (System.Data.DataRow item in dt.Rows)
					{
						fid = YJT.DataBase.Common.ObjectTryToObj<long>(item["Id"], 0L);
						DelOldFile(fid);
					}
				}
				//开始下载新文件
				if (t1 != null && t1.Count > 0)
				{
					DateTime addDateNow = DateTime.Now;
					int 的几个文件 = 0;
					foreach (string item in t1)
					{
						if (item.StartsWith("-rwx"))
						{
							string[] part = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
							if (part.Length > 8)
							{
								DateTime fileTime;
								if (YJT.Text.Verification.IsRightLike(part[part.Length - 1].ToLower(), new List<string> { ".jpg", ".png", ".bmp" }))
								{
									string p7 = part[5] + " " + part[6] + " " + part[7] + " ";
									//分析日期
									switch (part[5])
									{
										case "Jan":
											part[5] = "01";
											break;
										case "Feb":
											part[5] = "02";
											break;
										case "Mar":
											part[5] = "03";
											break;
										case "Apr":
											part[5] = "04";
											break;
										case "May":
											part[5] = "05";
											break;
										case "Jun":
											part[5] = "06";
											break;
										case "Jul":
											part[5] = "07";
											break;
										case "Aug":
											part[5] = "08";
											break;
										case "Sep":
											part[5] = "09";
											break;
										case "Oct":
											part[5] = "10";
											break;
										case "Nov":
											part[5] = "11";
											break;
										case "Dec":
											part[5] = "12";
											break;
										default:
											part[5] = "12";
											part[6] = "01";
											break;
									}
									DateTime a;

									string time = "";
									if (part[7].Contains(":"))
									{
										time = part[7];
										part[7] = DateTime.Now.Year.ToString();
									}
									if (!DateTime.TryParse(part[7] + "-" + part[5] + "-" + part[6] + " " + time, out a))
									{
										a = DateTime.Parse("1900-01-01");
									}
									fileTime = a;
									//获取网络文件名
									string netFileName = YJT.Text.Format.Substring(item, item.IndexOf(p7) + p7.Length, item.Length);
									//获取网络文件大小
									int fileSize = 0;
									if (!int.TryParse(part[4], out fileSize))
									{
										fileSize = 0;
									}

									MOD.Mod_bllFileHandle t = new Mod_bllFileHandle();
									t.addDate = addDateNow;
									t.batchId = batchId;
									t.erpFileGroupId = folderName.Replace("'","");
									t.fileName = netFileName.Replace("'","");
									t.fileSize = fileSize;
									t.goodsId = goodsId;
									t.Id = -1;
									t.lastReadTime = fileTime;
									t.NetPath = t.erpFileGroupId + "/" + t.fileName;
									t.LocalPath = "";
									t.usedCount = 1;
									t.FileType = "药品批件";
									t.AccountInfo = "汇达";

									string fileExtName = "";
									fileExtName = System.IO.Path.GetExtension(t.fileName);
									if (YJT.Text.Verification.IsNullOrEmpty(fileExtName))
									{
										fileExtName = ".tmp";
									}
									fileExtName = YJT.Text.Format.RemoveChars(fileExtName, YJT.Text.Format.EnumCharDirection.Left, ".");
									do
									{
										t.LocalPath = YJT.Path.GetTempFilePath(fileExtName, "yj", localPath);
									} while (System.IO.File.Exists(t.LocalPath));
									 
									//int j = 1;
									//string newFileName = selectObj.companyid.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + j.ToString() + System.IO.Path.GetExtension(netFileName);
									//while (System.IO.File.Exists(path + "\\" + newFileName))
									//{
									//	j++;
									//	newFileName = selectObj.companyid.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + j.ToString() + System.IO.Path.GetExtension(netFileName);
									//}
									int k = 0;
									bool flagK = false;
									string sqlCmd2 = "";
									do
									{
										if (_ftpHd.DownloadFile(folderName + "/" + netFileName, t.LocalPath))
										{
											sqlCmd2 = $@"
INSERT INTO bllFileHandle
		(
			NetPath ,
			LocalPath ,
			goodsId ,
			batchId ,
			addDate ,
			lastReadTime ,
			usedCount ,
			fileName ,
			fileSize ,
			erpFileGroupId ,
			FileType ,
			AccountInfo
		)
OUTPUT inserted.Id
	VALUES
		(
			'{t.NetPath}' , -- NetPath - varchar(4000)
			'{t.LocalPath.Replace(@"\\172.16.2.240\debugec\", @"D:\")}' , -- LocalPath - varchar(4000)
			'{t.goodsId}' , -- goodsId - varchar(1000)
			'{t.batchId}' , -- batchId - varchar(1000)
			convert(datetime,'{t.addDate.ToString("yyyy-MM-dd HH:mm:ss")}') , -- addDate - datetime
			convert(datetime,'{t.lastReadTime.ToString("yyyy-MM-dd HH:mm:ss")}') , -- lastReadTime - datetime
			{t.usedCount.ToString()} , -- usedCount - int
			'{t.fileName}' , -- fileName - varchar(1000)
			{t.fileSize.ToString()} , -- fileSize - int
			'{t.erpFileGroupId}' , -- erpFileGroupId - varchar(1000)
			'{t.FileType}' , -- FileType - varchar(100)
			'{t.AccountInfo}'  -- AccountInfo - varchar(100)
		)
";
											System.Data.DataTable dt2 = _ecSqlServer.ExecuteToDataTable(sqlCmd2, null, true);
											if (dt2.Rows.Count > 0)
											{
												t.Id = YJT.DataBase.Common.ObjectTryToObj(dt2.Rows[0]["Id"], -1L);
												flagK = true;
												k = 10;
												res.Add(t);
											}
											else
											{
												//插入记录失败
												//此文件无法写入到数据库
												try
												{
													System.IO.File.Delete(t.LocalPath);
												}
												catch
												{
												}
												k++;
											}
											
											
											
											
											try
											{
												dt2.Dispose();
											}
											catch { }
											dt2 = null;
											的几个文件++;
										}
										else
										{
											k++;
										}
									}
									while (k < 10);
									if (flagK == false)
									{
										//此文件无法下载,尝试过10次
									}

								}
								else
								{
									//此文件并非图片格式,也要写入数据库,但是不打印
								}

							}
							else
							{
								//获取的文件列表并非可以识别的ftp返回结果 开始位置并非 字段数量并非超过8个列
							}
						}
						else
						{
							//获取的文件列表并非可以识别的ftp返回结果 开始位置并非 -rwx
						}
					}
				}
				else
				{
					//服务器上没有对应货品资质文件,
				}
			}

			try
			{
				dt.Dispose();
			}
			catch { }
			dt = null;


			
			
			//判断FTP是否正常
			//获取文件夹中的文件
			//开始循环下载,并写入本地,并写入数据库,并标记,
			//返回当前获取的文件信息


			return res;
		}

	
	}
}
