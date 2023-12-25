using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOD;
using Microsoft.Win32;
using System.Reflection;

namespace YanduECommerceAutomaticPrinting
{

	public partial class Form1 : Form
	{
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetForegroundWindow(); //获得本窗体的句柄
		[System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体

		//定义变量,句柄类型
		public IntPtr _handle1;
		#region 全局变量
		/// <summary>
		/// 用于显示队列的表格
		/// </summary>
		System.Data.DataTable _dataTableQueueList = new DataTable();
		#endregion















































































































		#region 界面
		bool _enableForceTop = true;
		//是否允许退出程序
		bool _canCancal = false;
		//是否暂停
		bool _isPause = false;
		/// <summary>
		/// 查询物流状态
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button13_Click(object sender, EventArgs e)
		{
			ShowLogicInfo sli = new ShowLogicInfo();
			sli.ShowDialog();
		}
		/// <summary>
		/// 检索
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button4_Click(object sender, EventArgs e)
		{
			dataGridView2.DataSource = null;
			if (bll == null)
			{
				MessageBox.Show("bll未null");
				return;
			}
			string errMsg = "";
			int errCode = 0;
			bool isok = false;
			DateTime sriqi = DateTime.Parse(sSriqi.Value.ToString("yyyy-MM-dd"));
			DateTime eriqi = DateTime.Parse(sEriqi.Value.ToString("yyyy-MM-dd") + " 23:59:59");
			string erpDanjbh = stbDanjbh.Text;
			string danjStatus = scbDanjStatus.Text;
			if (danjStatus == "所有")
			{
				danjStatus = "";
			}
			string mac = "";
			if (scbClientName.Text != "所有")
			{
				object sclient = scbClientName.SelectedItem;
				if (sclient != null)
				{
					YJT.ValuePara.ComplexValue.ComplexValue cv = sclient as YJT.ValuePara.ComplexValue.ComplexValue;
					if (cv != null)
					{
						if (cv.ObjectValue != null && cv.ObjectValue.Length > 0)
						{
							MOD.SysMod.ClinetTag ct = cv.ObjectValue[0] as MOD.SysMod.ClinetTag;
							if (ct != null)
							{
								mac = ct.Mac;
							}
						}
					}
				}
			}
			string serverTaskType = scbTaskType.Text;
			if (serverTaskType == "所有")
			{
				serverTaskType = "";
			}

			string wlLogic = scbLogicpl.Text;
			if (wlLogic == "所有")
			{
				wlLogic = "";
			}

			string ecpl = scbEcpl.Text;
			if (ecpl == "所有")
			{
				ecpl = "";
			}
			string ddwid = stbCustomid.Text;
			string ddwName = stbCustomName.Text;
			string wlNumber = stbLogicNo.Text;
			string addr = stbCustomAddr.Text;




			System.Data.DataTable list = bll.ClientGetDetailList(sriqi, eriqi, out isok, out errCode, out errMsg, erpDanjbh, danjStatus, mac, serverTaskType, wlLogic, ecpl, ddwid, ddwName, wlNumber, addr, sisYunfei.Checked, sisYCZ.Checked, bcISPJ.Checked);
			if (list != null && list.Rows.Count > 0)
			{
				//list.Rows.Cast<DataRow>().OrderBy(r=>)
				dataGridView2.DataSource = list;
				dataGridView2.Refresh();
				AddMsgLog("记录数:" + list.Rows.Count.ToString(), Settings.Setings.EnumMessageType.提示, DateTime.Now, "", false, true);
			}
			else
			{
				if (list != null && list.Rows.Count == 0)
				{
					AddMsgLog("无记录", Settings.Setings.EnumMessageType.提示, DateTime.Now, "主界面.查询", false, true);
					MessageBox.Show("无记录");
				}
				else
				{
					AddMsgLog("查询错误:" + errCode, Settings.Setings.EnumMessageType.异常, DateTime.Now, "主界面.查询", true, true);
				}

			}


		}
		bool expOver = true;
		/// <summary>
		/// 导出
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button11_Click(object sender, EventArgs e)
		{
			if (expOver == false)
			{
				MessageBox.Show("未导出完成,不能再次导出,请稍后");
				return;
			}
			if (dataGridView2.DataSource != null)
			{
				System.Data.DataTable dt = dataGridView2.DataSource as System.Data.DataTable;


				if (dt != null && dt.Rows.Count > 0)
				{
					Dictionary<string, string> keys = new Dictionary<string, string>();
					foreach (DataGridViewColumn item in dataGridView2.Columns)
					{
						keys.Add(item.DataPropertyName, item.HeaderText);
					}
					System.Data.DataTable dt2 = dt.Copy();
					for (int i = 0; i < dt2.Rows.Count; i++)
					{
						dt2.Rows[i]["WL_NUMBER"] = "'" + YJT.DataBase.Common.ObjectTryToObj(dt2.Rows[i]["WL_NUMBER"], "");
					}
					YJT.DataTableHandle.DataTableTool.ReplaceFieldName(dt2, keys);
					SaveFileDialog sfd = new SaveFileDialog();
					sfd.Filter = "Excel文件|*.xls";
					sfd.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "电商记录.xls";
					sfd.RestoreDirectory = true;
					sfd.Title = "导出excel";
					sfd.ValidateNames = true;
					sfd.OverwritePrompt = true;
					sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
					ReOpen:
					if (sfd.ShowDialog() == DialogResult.OK)
					{
						string filePath = sfd.FileName;
						if (!System.IO.File.Exists(filePath))
						{
							expOver = false;
							YJT.Excel ex = new YJT.Excel();
							bool outfile = ex.ExportExcelForDataTable(dt2, filePath);
							System.Threading.Thread th = new System.Threading.Thread(() =>
							{
								while (ex.IsComplate == false)
								{
									System.Threading.Thread.Sleep(1000);
									AddMsgLog("共:" + ex.CountLines.ToString() + "行 已导出:" + ex.CurrentWriteLines.ToString() + "行", Settings.Setings.EnumMessageType.提示, DateTime.Now, "", false, true);
								}
								AddMsgLog("导出完毕", Settings.Setings.EnumMessageType.提示, DateTime.Now, "", false, true);
								expOver = true;
							});
							th.IsBackground = true;
							th.Start();
						}
						else
						{
							MessageBox.Show("文件存在,重新选择文件");
							goto ReOpen;
						}


					}
					else
					{
						MessageBox.Show("取消导出");
					}




				}
			}
		}
		/// <summary>
		/// 补打物流
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button8_Click(object sender, EventArgs e)
		{

		}
		/// <summary>
		/// 补打随货
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button9_Click(object sender, EventArgs e)
		{

		}
		/// <summary>
		/// 橙3 按钮被点击
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button5_Click(object sender, EventArgs e)
		{
		}
		/// <summary>
		/// 青2 按钮被点击
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button6_Click(object sender, EventArgs e)
		{
			//res.Rows.Remove(res.Rows[dataGridView1.SelectedRows[0].Index]);
		}
		/// <summary>
		/// 蓝1 被点击 最小化
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button7_Click(object sender, EventArgs e)
		{
			if (_enableForceTop == false)
			{
				this.WindowState = FormWindowState.Minimized;
			}
			else
			{
				if (_isPause == true)
				{
					this.WindowState = FormWindowState.Minimized;
				}
			}

			//res.Rows[dataGridView1.SelectedRows[0].Index]["CStatus"] = "已修改";
			//////
			//_orderList[0].V2["ErrMsg"] = "asdf阿斯顿发送到发送到发送地方";
		}
		/// <summary>
		/// 点击暂停按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, EventArgs e)
		{
			if (_isPause)
			{
				//进行启动
				if (_forceLock == true)
				{
					AddMsgLog("遇到严重问题,不能启动", Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
					return;

				}
				else
				{
					_isPause = false;
					tabControl1.SelectedIndex = 0;
					button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
					button1.Enabled = true;
					Tb_intupBox.Enabled = true;
					Tb_intupBox.ReadOnly = false;
					Tb_intupBox.Focus();
					tabControl1.TabsVisible = false;
					Btn_RemoveQueue.Enabled = false;
					button3.Text = "暂停";
					_areGetServerHandledList.Set();
					_are.Set();
					panel1.Visible = false;
					AddMsgLog("程序启动", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
				}

			}
			else
			{
				//进行暂停
				_isPause = true;
				button3.BackColor = System.Drawing.Color.Lime;
				Tb_intupBox.Enabled = false;
				button1.Enabled = false;
				Tb_intupBox.ReadOnly = true;
				tabControl1.TabsVisible = true;
				this.TopMost = false;
				Btn_RemoveQueue.Enabled = true;
				button3.Text = "启动";
				panel1.Visible = true;
				AddMsgLog("程序暂停", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
			}
		}
		void StartServer()
		{
			if (_isPause)
			{
				//进行启动
				if (_forceLock == true)
				{
					AddMsgLog("遇到严重问题,不能启动", Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
					return;

				}
				else
				{
					_isPause = false;
					tabControl1.SelectedIndex = 0;
					button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
					button1.Enabled = true;
					Tb_intupBox.Enabled = true;
					Tb_intupBox.ReadOnly = false;
					Tb_intupBox.Focus();
					tabControl1.TabsVisible = false;
					Btn_RemoveQueue.Enabled = false;
					button3.Text = "暂停";
					this.TopMost = true;
					_areGetServerHandledList.Set();
					_are.Set();
					panel1.Visible = true;
					AddMsgLog("程序启动", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
				}

			}
			else
			{
				//进行暂停
				AddMsgLog("服务未暂停", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
			}
		}
		void StopServer(bool isForceStop = false)
		{
			if (_isPause && isForceStop == false)
			{
				//进行启动
				AddMsgLog("程序未启动", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());

			}
			else
			{
				//进行暂停
				_isPause = true;
				button3.BackColor = System.Drawing.Color.Lime;
				button1.Enabled = false;
				Tb_intupBox.Enabled = false;
				Tb_intupBox.ReadOnly = true;
				tabControl1.TabsVisible = true;
				this.TopMost = false;
				Btn_RemoveQueue.Enabled = true;
				button3.Text = isForceStop ? "不能启动" : "启动";
				panel1.Visible = true;
				AddMsgLog("程序" + (isForceStop ? "强制关闭" : "暂停"), Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
			}
		}
		/// <summary>
		/// 点击退出按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void button2_Click(object sender, EventArgs e)
		{
			if (_isPause)
			{
				AddMsgLog("程序退出", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
				await Task.Delay(1000);
				_canCancal = true;
				//YJT.MSystem.SystemShutdown.Init().ShutdownOff(true);
				this.Close();
			}
			else
			{
				AddMsgLog("先暂停", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
			}

		}
		
		/// <summary>
		/// 在界面中,当输入框焦点丢失后,回到输入框
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tb_intupBox_LostFocus(object sender, EventArgs e)
		{
			Tb_intupBox.Focus();
		}
		/// <summary>
		/// 当 当前程序不是被激活状态执行
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Deactivate(object sender, EventArgs e)
		{
			if (_enableForceTop == true)
			{
				AddMsgLog("窗口丢失", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
				timer_focusWin.Enabled = true;
			}

		}
		/// <summary>
		/// 设置当前程序 转换到 激活状态
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer_focusWin_Tick(object sender, EventArgs e)
		{
			if (_enableForceTop == true)
			{
				if (_isPause == false)
				{
					if (GetForegroundWindow() != _handle1)
					{
						SetForegroundWindow(_handle1);
						this.Focus();
						this.Activate();
						this.Show();
						this.TopMost = true;
						this.WindowState = FormWindowState.Maximized;
					}
					else
					{
						timer_focusWin.Enabled = false;
					}
				}
				else
				{
					timer_focusWin.Enabled = false;
				}
			}

		}
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			string tmpPath = System.IO.Path.GetTempPath();
			deleteFolder(tmpPath);
			base.OnFormClosed(e);
		}
		protected override void OnClosed(EventArgs e)
		{
			string tmpPath = System.IO.Path.GetTempPath();
			deleteFolder(tmpPath);
			base.OnClosed(e);
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
		}
		/// <summary>
		/// 当程序推出时候,检测是否允许退出
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!_canCancal)
			{
				e.Cancel = true;
			}
			else
			{
				string tmpPath = System.IO.Path.GetTempPath();
				deleteFolder(tmpPath);
				base.OnFormClosing(e);
			}

		}
		/// <summary>
		/// 退出应用
		/// </summary>
		private void ExitApp()
		{
			AddMsgLog("退出程序", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
			button2.PerformClick();
		}
		/// <summary>
		/// 关机
		/// </summary>
		private void ShutdownSys()
		{
			AddMsgLog("电脑关机", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());

		}
		/// <summary>
		/// 重启
		/// </summary>

		private void ResetSystem()
		{
			AddMsgLog("电脑重启", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());

		}
		/// <summary>
		/// 点击队列中的列表时处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (_isPause == true)
			{
				if (dataGridView1.SelectedRows.Count > 0)
				{
					textBox1.Text = dataGridView1.SelectedRows[0].Cells["CErpId"].Value.ToString();
					textBox2.Text = dataGridView1.SelectedRows[0].Cells["OrderId"].Value.ToString();
				}
			}

		}
		/// <summary>
		/// 点击删除所选
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Btn_RemoveQueue_Click(object sender, EventArgs e)
		{
			string erpId = textBox1.Text;
			string orderId = textBox2.Text;
			MOD.BllMod.Order order = null;
			if (YJT.Text.Verification.IsNotNullOrEmpty(erpId) && YJT.Text.Verification.IsNotNullOrEmpty(orderId))
			{
				bool flag = false;
				//如果有任务在队列中,先删除队列
				if (_handleQueue.Count > 0)
				{

					foreach (YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType> item in _handleQueue)
					{
						if (item.V1.OrderId.ToString() == orderId && item.V1.ErpId == erpId)
						{
							item.V2 = Settings.Setings.EnumTaskType.要求停止;
							item.V1.Status = Settings.Setings.EnumOrderStatus.准备停止;
							AddMsgLog("修改删除单号任务:" + erpId + "顺序ID" + orderId, Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace(), true, true);
							flag = true;
							order = item.V1;
							break;
						}
					}
				}
				//如果没有队列,说明单据是上一次软件运行添加的,从表中查找
				if (flag == false)
				{
					if (_orderList.Count > 0)
					{
						foreach (KeyValuePair<long, YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, System.Data.DataRow>> item in _orderList)
						{
							if (item.Value.V1.OrderId.ToString() == orderId && item.Value.V1.ErpId == erpId)
							{
								item.Value.V1.Status = Settings.Setings.EnumOrderStatus.准备停止;
								_handleQueue.Enqueue(new YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType>() { V1 = item.Value.V1, V2 = Settings.Setings.EnumTaskType.要求停止 });//HandleQueue 进行处理
								AddMsgLog("新增删除单号任务:" + erpId + "顺序ID" + orderId, Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace(), true, true);
								flag = true;
								order = item.Value.V1;
								break;
							}
						}
					}
				}
				if (flag == false)
				{
					AddMsgLog("没有找到要删除的单号:" + erpId + "顺序ID" + orderId + " 可能已经完成或已经删除等待刷新列表", Settings.Setings.EnumMessageType.异常, DateTime.Now, Common.PubMethod.GetNameSpace(), true, true);
				}
				else
				{
					if (order != null)
					{
						//更改表格显示内容
						ResetDataTableRow(order);
					}
				}
			}
		}
		/// <summary>
		/// 当任务队列表格中添加一行,处理行号
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			for (int i = 0; i < dataGridView1.RowCount; i++)
				dataGridView1.Rows[i].HeaderCell.Value = i.ToString();
		}
		#endregion

















































































































		#region 初始化
		BLL.Blll bll = null;
		/// <summary>
		/// 强制禁止本服务器东,为true此系统不能运行
		/// </summary>
		bool _forceLock = false;
		MOD.SysMod.ClinetTag _clientTag = null;

		public Form1()
		{
			InitializeComponent();
		}
		YJT.HardControl.ComPort _comPort = null;
		private void SetLable8Value(string str)
		{
			if (label8.InvokeRequired)
			{
				label8.Invoke(new Action<string, Label>((str1, lab) =>
				{
					lab.Text = str1;
				}), str, label8);
			}
			else
			{
				label8.Text = str;
			}



		}
		public void deleteFolder(string folder)
		{
			if (System.IO.Directory.Exists(folder))
			{
				string[] files = System.IO.Directory.GetFiles(folder);
				if (files != null && files.Length > 0)
				{
					foreach (string item in files)
					{
						try
						{
							System.IO.File.Delete(item);
						}
						catch { }
					}
				}
				string[] dirs = System.IO.Directory.GetDirectories(folder);
				if (dirs != null && dirs.Length > 0)
				{
					foreach (string item in dirs)
					{
						deleteFolder(item);
						try
						{
							System.IO.Directory.Delete(item, true);
						}
						catch { }
					}
				}
			}
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			//for (int i = 0; i < 30; i++)
			//{
			//	System.Threading.Thread.Sleep(1000);
			//}

			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			Type dgvType = dataGridView1.GetType();
			PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			pi.SetValue(dataGridView1, true, null);


			scbDanjStatus.Items.Add("所有");
			scbDanjStatus.Items.AddRange(Enum.GetNames(typeof(Settings.Setings.EnumOrderStatus)));
			scbDanjStatus.SelectedIndex = 0;


			scbTaskType.Items.Add("所有");
			scbTaskType.Items.AddRange(Enum.GetNames(typeof(Settings.Setings.EnumServerTaskType)));
			scbTaskType.SelectedIndex = 0;

			scbLogicpl.Items.Add("所有");
			scbLogicpl.Items.AddRange(Enum.GetNames(typeof(Settings.Setings.EnumLogicType)));
			scbLogicpl.SelectedIndex = 0;

			scbEcpl.Items.Add("所有");
			scbEcpl.Items.AddRange(Enum.GetNames(typeof(Settings.Setings.EnumPlatformType)));
			scbEcpl.SelectedIndex = 0;

			dataGridView1.AutoGenerateColumns = false;
			dataGridView2.AutoGenerateColumns = false;
			//进程唯一处理
			uint runCount = 0;
			_handle1 = this.Handle;
			if (YJT.MSystem.Common.MutexCheck("YanduECommerceAutomaticPrinting", out runCount, 1) == false)
			{
				_isPause = true;
				_forceLock = true;
				_canCancal = true;
				MessageBox.Show("不允许重复运行");
				this.Close();
			}
			else
			{
				try
				{
					RegistryKey key = Registry.LocalMachine;
					RegistryKey subKey1 = key.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\COM Name Arbiter", true);
					RegistryKey subKey2 = key.OpenSubKey(@"SYSTEM\ControlSet002\Control\COM Name Arbiter", true);

					try
					{
						subKey1.DeleteValue(@"ComDB");

					}
					catch { }
					try
					{
						subKey2.DeleteValue(@"ComDB");

					}
					catch { }
					subKey1.Close();
					subKey1.Dispose();
					subKey1 = null;
					subKey2.Close();
					subKey2.Dispose();
					subKey2 = null;
					key.Close();
					key.Dispose();
					key = null;
				}
				catch { }

				//检测客户端标志
				_clientTag = Common.PubMethod.GetClientTag();
				if (_clientTag == null)
				{
					_isPause = true;
					_forceLock = true;
					MessageBox.Show("未能获取到客户标志");
					this.Close();
				}
				else
				{
					BLL.Blll._clientInfoObj = _clientTag;
					//注册BLL中统一输出错误的方法
					BLL.Blll.AddMsgOutEve += Blll_AddMsgOutEve;
					bll = BLL.Blll.init();
					if (bll == null)
					{
						AddMsgLog("业务层初始化失败", Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
						_forceLock = true;
						_isPause = true;
						StopServer(true);
						return;
					}
					else
					{
						AddMsgLog("", Settings.Setings.EnumMessageType.提示, DateTime.Now, "");
						AddMsgLog("初始化完成", Settings.Setings.EnumMessageType.提示, DateTime.Now, "");
					}
					if (BLL.Blll._clientInfoObj.Ip == "172.16.7.50" || BLL.Blll._clientInfoObj.isForge == true)
					{
						_enableForceTop = false;
					}
					else
					{
						_enableForceTop = true;
					}
					//提取本机最大ID
					bool isOk = false;
					int errCode = 0;
					string errMsg = "";
					try
					{
						List<string> coms = YJT.HardControl.ComPort.GetConNameList("USB-SERIAL CH340");
						if (coms.Count > 0)
						{
							_comPort = YJT.HardControl.ComPort.Init(coms[0], 9600);
							if (_comPort.Open(out errMsg) == false)
							{
								MessageBox.Show("电子秤连接失败:" + errMsg);
								_comPort = null;
							}

						}
					}
					catch (Exception ee)
					{
						MessageBox.Show(ee.Message);
						_comPort = null;
					}
					errMsg = "";
					if (GetMaxClientOrderId(out isOk, out errCode, out errMsg) == false)
					{
						AddMsgLog(errMsg, Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
						_forceLock = true;
						_isPause = true;
						StopServer(true);
						MessageBox.Show(errMsg);
						return;
					}
					//删除队列按钮
					Btn_RemoveQueue.Enabled = false;
					//TAB顶端按钮
					tabControl1.TabsVisible = false;
					tabControl1.SelectedIndex = 0;
					//输入框默认焦点
					Tb_intupBox.Focus();
					Tb_intupBox.LostFocus += Tb_intupBox_LostFocus;
					timer_focusWin.Enabled = true;
					//文本输出框最大行数
					Tb_logBox.MaxLength = 0;
					if (bll.VerVersion(out errMsg) == false)
					{
						AddMsgLog(errMsg + ",请退出后更新", Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
						_forceLock = true;
						_isPause = true;
						StopServer(true);
						MessageBox.Show(errMsg + ",请退出后更新");
						return;
					}
					//队列表格结构
					System.Data.DataColumn[] dc = new DataColumn[82];
					dc[0] = new DataColumn("CAddDate");
					dc[0].Caption = "CAddDate";
					dc[0].DataType = typeof(string);

					dc[1] = new DataColumn("CErpId");
					dc[1].Caption = "CErpId";
					dc[1].DataType = typeof(string);

					dc[2] = new DataColumn("CLogic");
					dc[2].Caption = "CLogic";
					dc[2].DataType = typeof(string);

					dc[3] = new DataColumn("CWeight");
					dc[3].Caption = "CWeight";
					dc[3].DataType = typeof(string);

					dc[4] = new DataColumn("CStatus");
					dc[4].Caption = "CStatus";
					dc[4].DataType = typeof(string);


					dc[5] = new DataColumn("CLength");
					dc[5].Caption = "CLength";
					dc[5].DataType = typeof(string);

					dc[6] = new DataColumn("CWidth");
					dc[6].Caption = "CWidth";
					dc[6].DataType = typeof(string);

					dc[7] = new DataColumn("CHeight");
					dc[7].Caption = "CHeight";
					dc[7].DataType = typeof(string);

					dc[8] = new DataColumn("CTagObj");
					dc[8].Caption = "CTagObj";
					dc[8].DataType = typeof(object);

					dc[9] = new DataColumn("ErrMsg");
					dc[9].Caption = "ErrMsg";
					dc[9].DataType = typeof(string);

					dc[10] = new DataColumn("ClientId");
					dc[10].Caption = "ClientId";
					dc[10].DataType = typeof(int);


					dc[11] = new DataColumn("PrintStatus");
					dc[11].Caption = "PrintStatus";
					dc[11].DataType = typeof(string);

					dc[12] = new DataColumn("WmsDanjbh");
					dc[12].Caption = "WmsDanjbh";
					dc[12].DataType = typeof(string);

					dc[13] = new DataColumn("NetDanjbh");
					dc[13].Caption = "NetDanjbh";
					dc[13].DataType = typeof(string);

					dc[14] = new DataColumn("OrderId");
					dc[14].Caption = "OrderId";
					dc[14].DataType = typeof(long);

					dc[15] = new DataColumn("ClientHandleDate");
					dc[15].Caption = "ClientHandleDate";
					dc[15].DataType = typeof(DateTime);

					dc[16] = new DataColumn("ServerHandleDate");
					dc[16].Caption = "ServerHandleDate";
					dc[16].DataType = typeof(DateTime);

					dc[17] = new DataColumn("Bid");
					dc[17].Caption = "Bid";
					dc[17].DataType = typeof(long);

					dc[18] = new DataColumn("WmsYewy");
					dc[18].Caption = "WmsYewy";
					dc[18].DataType = typeof(string);

					dc[19] = new DataColumn("WmsDdwname");
					dc[19].Caption = "WmsDdwname";
					dc[19].DataType = typeof(string);

					dc[20] = new DataColumn("NETORDER_FROMID");
					dc[20].Caption = "NETORDER_FROMID";
					dc[20].DataType = typeof(string);

					dc[21] = new DataColumn("NETORDER_FROM");
					dc[21].Caption = "NETORDER_FROM";
					dc[21].DataType = typeof(string);

					dc[22] = new DataColumn("ERPORDER_ID");
					dc[22].Caption = "ERPORDER_ID";
					dc[22].DataType = typeof(string);

					dc[23] = new DataColumn("CUSTOMID");
					dc[23].Caption = "CUSTOMID";
					dc[23].DataType = typeof(string);

					dc[24] = new DataColumn("CUSTOMNAME");
					dc[24].Caption = "CUSTOMNAME";
					dc[24].DataType = typeof(string);

					dc[25] = new DataColumn("AGENTNAME");
					dc[25].Caption = "AGENTNAME";
					dc[25].DataType = typeof(string);

					dc[26] = new DataColumn("ADDRESS");
					dc[26].Caption = "ADDRESS";
					dc[26].DataType = typeof(string);

					dc[27] = new DataColumn("WL_COMPANYID");
					dc[27].Caption = "WL_COMPANYID";
					dc[27].DataType = typeof(string);

					dc[28] = new DataColumn("WL_NUMBER");
					dc[28].Caption = "WL_NUMBER";
					dc[28].DataType = typeof(string);

					dc[29] = new DataColumn("WL_COMPANYNAME");
					dc[29].Caption = "WL_COMPANYNAME";
					dc[29].DataType = typeof(string);

					dc[30] = new DataColumn("RECVNAME");
					dc[30].Caption = "RECVNAME";
					dc[30].DataType = typeof(string);

					dc[31] = new DataColumn("RECVPHONE");
					dc[31].Caption = "RECVPHONE";
					dc[31].DataType = typeof(string);

					dc[32] = new DataColumn("PROVINCENAME");
					dc[32].Caption = "PROVINCENAME";
					dc[32].DataType = typeof(string);

					dc[33] = new DataColumn("CITYNAME");
					dc[33].Caption = "CITYNAME";
					dc[33].DataType = typeof(string);

					dc[34] = new DataColumn("DISTRICTNAME");
					dc[34].Caption = "DISTRICTNAME";
					dc[34].DataType = typeof(string);

					dc[35] = new DataColumn("STREETNAME");
					dc[35].Caption = "STREETNAME";
					dc[35].DataType = typeof(string);

					dc[36] = new DataColumn("ORIGINALREMARK");
					dc[36].Caption = "ORIGINALREMARK";
					dc[36].DataType = typeof(string);

					dc[37] = new DataColumn("IsFp");
					dc[37].Caption = "IsFp";
					dc[37].DataType = typeof(string);

					dc[38] = new DataColumn("IsPj");
					dc[38].Caption = "IsPj";
					dc[38].DataType = typeof(string);

					dc[39] = new DataColumn("IsQysy");
					dc[39].Caption = "IsQysy";
					dc[39].DataType = typeof(string);

					dc[40] = new DataColumn("IsSyzz");
					dc[40].Caption = "IsSyzz";
					dc[40].DataType = typeof(string);

					dc[41] = new DataColumn("IsYjbb");
					dc[41].Caption = "IsYjbb";
					dc[41].DataType = typeof(string);

					dc[42] = new DataColumn("PlatformType");
					dc[42].Caption = "PlatformType";
					dc[42].DataType = typeof(string);

					dc[43] = new DataColumn("logi_dstRoute");
					dc[43].Caption = "logi_dstRoute";
					dc[43].DataType = typeof(string);

					dc[44] = new DataColumn("logi_PayType");
					dc[44].Caption = "logi_PayType";
					dc[44].DataType = typeof(string);

					dc[45] = new DataColumn("logi_monAccNum");
					dc[45].Caption = "logi_monAccNum";
					dc[45].DataType = typeof(string);

					dc[46] = new DataColumn("logi_baojiaJine");
					dc[46].Caption = "logi_baojiaJine";
					dc[46].DataType = typeof(string);

					dc[47] = new DataColumn("logi_dsJine");
					dc[47].Caption = "logi_dsJine";
					dc[47].DataType = typeof(string);

					dc[48] = new DataColumn("logi_logcNum");
					dc[48].Caption = "logi_logcNum";
					dc[48].DataType = typeof(string);

					dc[49] = new DataColumn("logi_ysJine");
					dc[49].Caption = "logi_ysJine";
					dc[49].DataType = typeof(string);

					dc[50] = new DataColumn("logi_ysJineTotal");
					dc[50].Caption = "logi_ysJineTotal";
					dc[50].DataType = typeof(string);

					dc[51] = new DataColumn("logi_shouhuory");
					dc[51].Caption = "logi_shouhuory";
					dc[51].DataType = typeof(string);

					dc[52] = new DataColumn("logi_jinjianRiqi");
					dc[52].Caption = "logi_jinjianRiqi";
					dc[52].DataType = typeof(string);

					dc[53] = new DataColumn("logi_shoufqianshu");
					dc[53].Caption = "logi_shoufqianshu";
					dc[53].DataType = typeof(string);

					dc[54] = new DataColumn("logi_shoufRiqi");
					dc[54].Caption = "logi_shoufRiqi";
					dc[54].DataType = typeof(string);

					dc[55] = new DataColumn("logi_sendSheng");
					dc[55].Caption = "logi_sendSheng";
					dc[55].DataType = typeof(string);

					dc[56] = new DataColumn("logi_sendShi");
					dc[56].Caption = "logi_sendShi";
					dc[56].DataType = typeof(string);

					dc[57] = new DataColumn("logi_sendXian");
					dc[57].Caption = "logi_sendXian";
					dc[57].DataType = typeof(string);

					dc[58] = new DataColumn("logi_sendAddress");
					dc[58].Caption = "logi_sendAddress";
					dc[58].DataType = typeof(string);

					dc[59] = new DataColumn("logi_sendMan");
					dc[59].Caption = "logi_sendMan";
					dc[59].DataType = typeof(string);

					dc[60] = new DataColumn("logi_sendPhone");
					dc[60].Caption = "logi_sendPhone";
					dc[60].DataType = typeof(string);

					dc[61] = new DataColumn("logi_feiyongTotal");
					dc[61].Caption = "logi_feiyongTotal";
					dc[61].DataType = typeof(string);

					dc[62] = new DataColumn("logi_goodQty");
					dc[62].Caption = "logi_goodQty";
					dc[62].DataType = typeof(string);

					dc[63] = new DataColumn("logi_goodName");
					dc[63].Caption = "logi_goodName";
					dc[63].DataType = typeof(string);

					dc[64] = new DataColumn("needBaojia");
					dc[64].Caption = "needBaojia";
					dc[64].DataType = typeof(string);

					dc[65] = new DataColumn("logi_OrderId");
					dc[65].Caption = "logi_OrderId";
					dc[65].DataType = typeof(string);

					dc[66] = new DataColumn("logi_CreateDate");
					dc[66].Caption = "logi_CreateDate";
					dc[66].DataType = typeof(string);

					dc[67] = new DataColumn("logi_ChanpinTypeStr");
					dc[67].Caption = "logi_ChanpinTypeStr";
					dc[67].DataType = typeof(string);

					dc[68] = new DataColumn("PrintDatetime");
					dc[68].Caption = "PrintDatetime";
					dc[68].DataType = typeof(string);

					dc[69] = new DataColumn("sysFirst");
					dc[69].Caption = "sysFirst";
					dc[69].DataType = typeof(string);

					dc[70] = new DataColumn("total_amt");
					dc[70].Caption = "total_amt";
					dc[70].DataType = typeof(string);

					dc[71] = new DataColumn("RelBId");
					dc[71].Caption = "RelBId";
					dc[71].DataType = typeof(string);

					dc[72] = new DataColumn("fplx");
					dc[72].Caption = "fplx";
					dc[72].DataType = typeof(string);

					dc[73] = new DataColumn("ServerTaskType");
					dc[73].Caption = "ServerTaskType";
					dc[73].DataType = typeof(string);

					dc[74] = new DataColumn("PAIDCOSTS");
					dc[74].Caption = "PAIDCOSTS";
					dc[74].DataType = typeof(string);

					dc[75] = new DataColumn("logi_ReceivePwd");
					dc[75].Caption = "logi_ReceivePwd";
					dc[75].DataType = typeof(string);

					dc[76] = new DataColumn("logi_SubOrderSn");
					dc[76].Caption = "logi_SubOrderSn";
					dc[76].DataType = typeof(string);

					dc[77] = new DataColumn("logi_PhonNum");
					dc[77].Caption = "logi_PhonNum";
					dc[77].DataType = typeof(string);

					dc[78] = new DataColumn("logi_TelNum");
					dc[78].Caption = "logi_TelNum";
					dc[78].DataType = typeof(string);

					dc[79] = new DataColumn("RelOrderId");
					dc[79].Caption = "RelOrderId";
					dc[79].DataType = typeof(string);

					dc[80] = new DataColumn("JingdongWl");
					dc[80].Caption = "JingdongWl";
					dc[80].DataType = typeof(string);

					dc[81] = new DataColumn("IsHeTong");
					dc[81].Caption = "IsHeTong";
					dc[81].DataType = typeof(string);


					_dataTableQueueList.Columns.AddRange(dc);
					dataGridView1.DataSource = _dataTableQueueList;
					//队列处理
					_backWorkHandleQueueTh = new System.Threading.Thread(HandleQueue);
					_backWorkHandleQueueTh.IsBackground = true;
					_backWorkHandleQueueTh.Start();
					//日志文本框
					_TextBoxOutHandleQueueTh = new System.Threading.Thread(TextBoxOut);
					_TextBoxOutHandleQueueTh.IsBackground = true;
					_TextBoxOutHandleQueueTh.Start();
					//文件写入队列线程
					_AddMsgLogToFileTh = new System.Threading.Thread(AddMsgLogToFile);
					_AddMsgLogToFileTh.IsBackground = true;
					_AddMsgLogToFileTh.Start();
					YJT.MWinForm.TextboxControl.TextBoxAddCtrlAFullSelect(Tb_logBox);
					//获取服务器处理结果线程
					_getServerHandledListTh = new System.Threading.Thread(GetServerHandledList);
					_getServerHandledListTh.IsBackground = true;
					_getServerHandledListTh.Start();
					//界面置顶
					if (_enableForceTop == true)
					{
						this.TopMost = true;
					}
					YJT.Sound.TTSMicrosoft.Init().Speak("声音检测完成", isSyn: true, nuberMode: 1);
					this.WindowState = FormWindowState.Maximized;
					if (!System.IO.File.Exists(_fileLogPath + @"\enableInput"))
					{
						AddMsgLog("此客户端只能查询", Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
						_forceLock = true;
						_isPause = true;
						StopServer(true);
					}
					if (_clientTag.EnableInput == false)
					{
						AddMsgLog("系统要求,此客户端只能查询", Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
						_forceLock = true;
						_isPause = true;
						StopServer(true);
					}
					if (_forceLock != true && _comPort == null && Settings.Configs.GetIsNotCheckCom == "否")
					{
						AddMsgLog("系统无法连接电子秤", Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
						_forceLock = true;
						_isPause = true;
						StopServer(true);
					}
					else if (_comPort != null)
					{
						_comPort._readLine = (comMsg) =>
						{
							try
							{
								string strT = comMsg.Replace(".00kg", "");
								double vT = 0d;
								if (double.TryParse(strT, out vT))
								{
									SetLable8Value(vT.ToString("#0.00"));
								}
							}
							catch (Exception ee)
							{
								//string strT = com
								SetLable8Value(0.ToString("#0.00"));
							}


						};
					}
					else
					{
						AddMsgLog("跳过电子秤检测", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
					}
					//提取未完成的记录
					GetNotComplateOrder();
					//设置删除菜单隐藏
					panel1.Visible = false;


					scbClientName.Items.Add("所有");
					List<MOD.SysMod.ClinetTag> clients = bll.ClientGetAllClients();
					if (clients.Count > 0)
					{
						foreach (MOD.SysMod.ClinetTag item in clients)
						{
							YJT.ValuePara.ComplexValue.ComplexValue t = new YJT.ValuePara.ComplexValue.ComplexValue();
							t.Text = "[" + item.ComputerName + "]" + item.Ip.Replace("172.16.", "") + "(" + item.Mac + ")";
							t.ObjectValue = new object[] { item };
							scbClientName.Items.Add(t);
						}
					}
					scbClientName.SelectedIndex = 0;

					cbYewy.Items.Add("所有");
					List<string> yewys = bll.ClientGetAllYewys();
					if (yewys.Count > 0)
					{
						foreach (string item in yewys)
						{
							cbYewy.Items.Add(item);
						}
					}
					cbYewy.SelectedIndex = 0;


				}

			}

		}














		private bool GetMaxClientOrderId(out bool isOk, out int errCode, out string errMsg)
		{
			errMsg = "";
			errCode = 0;
			isOk = false;
			_listCount = 0;
			long res = bll.GetMaxClientOrderId(out isOk, out errCode, out errMsg);
			if (isOk == true)
			{
				_listCount = res + 1;
			}
			return isOk;

		}

		/// <summary>
		/// 只在初始化调用,其他时候调用会导致重复
		/// </summary>
		private void GetNotComplateOrder()
		{
			bool isOk = false;
			int errCode = 0;
			string errMsg = "";
			List<MOD.BllMod.Order> list = bll.GetNotComplateOrder(out isOk, out errCode, out errMsg);
			if (isOk == true)
			{
				if (list.Count > 0)
				{
					int i = 0;
					foreach (MOD.BllMod.Order order in list)
					{
						if (!_orderList.ContainsKey(order.OrderId))
						{
							//生成记录
							System.Data.DataRow dr = _dataTableQueueList.NewRow();
							dr["CAddDate"] = order.AddDate.ToString();
							dr["CErpId"] = order.ErpId;
							dr["CLogic"] = order.Logic.ToString();
							dr["CWeight"] = order.Weight.ToString("#0.00");
							dr["CLength"] = order.Length.ToString("#0.00");
							dr["CWidth"] = order.Width.ToString("#0.00");
							dr["CHeight"] = order.Height.ToString("#0.00");
							dr["CStatus"] = order.Status.ToString();
							dr["ErrMsg"] = order.ErrMsg;
							dr["ClientId"] = order.ClientId.ToString();
							dr["PrintStatus"] = order.PrintStatus.ToString();
							dr["WmsDanjbh"] = order.WmsDanjbh;
							dr["NetDanjbh"] = order.NetDanjbh;
							dr["OrderId"] = order.OrderId;
							dr["ClientHandleDate"] = order.ClientHandleDate;
							dr["ServerHandleDate"] = order.ServerHandleDate;
							dr["Bid"] = order.Bid;
							dr["WmsYewy"] = order.WmsYewy;
							dr["WmsDdwname"] = order.WmsDdwname;
							dr["NETORDER_FROMID"] = order.NETORDER_FROMID;
							dr["NETORDER_FROM"] = order.NETORDER_FROM;
							dr["ERPORDER_ID"] = order.ERPORDER_ID;
							dr["CUSTOMID"] = order.CUSTOMID;
							dr["CUSTOMNAME"] = order.CUSTOMNAME;
							dr["AGENTNAME"] = order.AGENTNAME;
							dr["ADDRESS"] = order.ADDRESS;
							dr["WL_COMPANYID"] = order.WL_COMPANYID;
							dr["WL_NUMBER"] = order.WL_NUMBER;
							dr["WL_COMPANYNAME"] = order.WL_COMPANYNAME;
							dr["RECVNAME"] = order.RECVNAME;
							dr["RECVPHONE"] = order.RECVPHONE;
							dr["PROVINCENAME"] = order.PROVINCENAME;
							dr["CITYNAME"] = order.CITYNAME;
							dr["DISTRICTNAME"] = order.DISTRICTNAME;
							dr["STREETNAME"] = order.STREETNAME;
							dr["ORIGINALREMARK"] = order.ORIGINALREMARK;
							dr["IsFp"] = order.IsFp;
							dr["IsPj"] = order.IsPj;
							dr["IsQysy"] = order.IsQysy;
							dr["IsSyzz"] = order.IsSyzz;
							dr["IsYjbb"] = order.IsYjbb;
							dr["PlatformType"] = order.PlatformType.ToString();
							dr["logi_dstRoute"] = order.logi_dstRoute;
							dr["logi_PayType"] = order.logi_PayType;
							dr["logi_monAccNum"] = order.logi_monAccNum;
							dr["logi_baojiaJine"] = order.logi_baojiaJine;
							dr["logi_dsJine"] = order.logi_dsJine;
							dr["logi_logcNum"] = order.logi_logcNum;
							dr["logi_ysJine"] = order.logi_ysJine;
							dr["logi_ysJineTotal"] = order.logi_ysJineTotal;
							dr["logi_shouhuory"] = order.logi_shouhuory;
							dr["logi_jinjianRiqi"] = order.logi_jinjianRiqi;
							dr["logi_shoufqianshu"] = order.logi_shoufqianshu;
							dr["logi_shoufRiqi"] = order.logi_shoufRiqi;
							dr["logi_sendSheng"] = order.logi_sendSheng;
							dr["logi_sendShi"] = order.logi_sendShi;
							dr["logi_sendXian"] = order.logi_sendXian;
							dr["logi_sendAddress"] = order.logi_sendAddress;
							dr["logi_sendMan"] = order.logi_sendMan;
							dr["logi_sendPhone"] = order.logi_sendPhone;
							dr["logi_feiyongTotal"] = order.logi_feiyongTotal;
							dr["logi_goodQty"] = order.logi_goodQty;
							dr["logi_goodName"] = order.logi_goodName;
							dr["needBaojia"] = order.needBaojia.ToString("#0.000");
							dr["logi_OrderId"] = order.logi_OrderId;
							dr["logi_CreateDate"] = order.logi_CreateDate;
							dr["logi_ChanpinTypeStr"] = order.logi_ChanpinTypeStr;
							dr["PrintDatetime"] = order.PrintDatetime;
							dr["sysFirst"] = order.sysFirst;
							dr["total_amt"] = order.total_amt;
							dr["RelBId"] = order.RelBId.ToString();
							dr["fplx"] = order.fplx;
							dr["ServerTaskType"] = order.ServerTaskType.ToString();
							dr["PAIDCOSTS"] = order.PAIDCOSTS.ToString();
							dr["logi_ReceivePwd"] = order.logi_ReceivePwd;
							dr["logi_SubOrderSn"] = order.logi_SubOrderSn.ToString();
							dr["logi_PhonNum"] = order.logi_PhonNum;
							dr["logi_TelNum"] = order.logi_TelNum;
							dr["RelOrderId"] = order.RelOrderId.ToString();
							dr["JingdongWl"] = order.JingdongWl;
							dr["IsHeTong"] = order.IsHeTong;
							_dataTableQueueList.Rows.Add(dr);
							_orderList.Add(order.OrderId, new YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, DataRow>() { V1 = order, V2 = dr });
							i++;
						}
					}
					AddMsgLog("添加历史记录共计:" + i.ToString(), Settings.Setings.EnumMessageType.提示, DateTime.Now, "");

				}
			}

		}
		#endregion
































































































		#region 日志处理
		Queue<string> _outMsg = new Queue<string>();
		string _fileLogPath = @"d:\YDECAP";
		Queue<string> _fileStringlist = new Queue<string>();
		System.Threading.AutoResetEvent _areWriteFile = new System.Threading.AutoResetEvent(false);
		System.Threading.AutoResetEvent _areTbOut = new System.Threading.AutoResetEvent(false);
		/// <summary>
		/// 文件日志写入队列
		/// </summary>
		void AddMsgLogToFile()
		{
			while (true)
			{
				if (_fileStringlist.Count > 0)
				{
					string msg = _fileStringlist.Dequeue();
					if (!System.IO.Directory.Exists(_fileLogPath))
					{
						System.IO.Directory.CreateDirectory(_fileLogPath);
					}
					if (!System.IO.Directory.Exists(_fileLogPath))
					{
						_forceLock = true;
						_isPause = true;
						_outMsg.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "	[" + Settings.Setings.EnumMessageType.严重.ToString() + "] " + "日志目录无法创建,无法保存日志");
						StopServer(true);
					}
					else
					{
						string fileLogName = _fileLogPath + @"\" + DateTime.Now.ToString("yyyyMMdd").ToString() + ".txt";
						if (!System.IO.File.Exists(fileLogName))
						{
							System.IO.File.AppendAllText(fileLogName, DateTime.Now.ToString("yyyy-MM-dd") + "日志创建\r\n");
						}
						if (System.IO.File.Exists(fileLogName))
						{
							System.IO.File.AppendAllText(fileLogName, msg);
						}
						else
						{
							_forceLock = true;
							_isPause = true;
							_outMsg.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "	[" + Settings.Setings.EnumMessageType.严重.ToString() + "] " + "日志文件无法创建,无法保存日志");
							StopServer(true);
						}
					}
				}
				else
				{
					_areWriteFile.WaitOne();
				}
			}

		}
		/// <summary>
		/// 添加日志
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="msType"></param>
		void AddMsgLog(string msg, Settings.Setings.EnumMessageType msType, DateTime dt, string nameSpace = "", bool isWriteFile = true, bool isOutputWindow = true)
		{

			if (isWriteFile == true)
			{
				if (msg == "")
				{
					_fileStringlist.Enqueue("\r\n-------------------------------------------------------------\r\n" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
				}
				else
				{
					_fileStringlist.Enqueue(dt.ToString("yyyy-MM-dd HH:mm:ss") + "	[" + msType.ToString() + "] " + nameSpace + msg + "\r\n");
				}

				_areWriteFile.Set();
			}
			if (isOutputWindow)
			{
				if (msg == "")
				{
					_outMsg.Enqueue("\r\n-------------------------------------------------------------\r\n" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
				}
				else
				{
					_outMsg.Enqueue(dt.ToString("yyyy-MM-dd HH:mm:ss") + "	[" + msType.ToString() + "] " + nameSpace + msg);
				}
			}
			_areTbOut.Set();
		}
		/// <summary>
		/// 用于BLL绑定的日志输出
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="type"></param>
		/// <param name="callNameSpace"></param>
		/// <param name="errCode"></param>
		/// <param name=""></param>
		/// <param name="arg4"></param>
		/// <param name="arg5"></param>
		private void Blll_AddMsgOutEve(string msg, Settings.Setings.EnumMessageType type, string callNameSpace, int errCode, string errMsg, string paras, DateTime dt)
		{
			AddMsgLog(msg + " (" + errCode.ToString() + ")\r\n\t" + callNameSpace + "\r\n\t" + errMsg.Replace("\r\n", "\r\n\t") + "\r\n" + paras + "\r\n", type, dt, "");
		}
		/// <summary>
		/// 文本框日志输出队列
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxOut()
		{
			while (true)
			{
				if (_outMsg.Count > 0)
				{
					do
					{
						if (Tb_logBox.InvokeRequired)
						{
							Tb_logBox.Invoke(new Action<TextBox, string>((tb, msg) =>
							{
								tb.AppendText(msg);
								tb.Select(tb.Text.Length, 0);
								tb.ScrollToCaret();
							}), Tb_logBox, _outMsg.Dequeue() + "\r\n");
						}
						else
						{
							Tb_logBox.AppendText(_outMsg.Dequeue() + "\r\n");
							Tb_logBox.Select(Tb_logBox.Text.Length, 0);
							Tb_logBox.ScrollToCaret();

						}
					} while (_outMsg.Count > 0);
				}
				else
				{
					_areTbOut.WaitOne();
				}
			}

		}
		#endregion

































		#region 特殊直插功能
		public bool AddStopOrder(string erpId)
		{
			bool res = false;
			DateTime nowDateTime = DateTime.Now;

			MOD.BllMod.Order order = new MOD.BllMod.Order()
			{
				//初始化对象
				AddDate = DateTime.Now,
				ErpId = erpId,
				Logic = Settings.Setings.EnumLogicType.Default,
				Weight = 1,
				Length = 1,
				Width = 1,
				Height = 1,
				Status = Settings.Setings.EnumOrderStatus.新增未写缓存,
				ComputerName = _clientTag.ComputerName,
				Mac = _clientTag.Mac,
				Ip = _clientTag.Ip,
				ClientId = _clientTag.ClientId,
				OrderId = _listCount,
				ErrMsg = "新增",
				PrintStatus = Settings.Setings.EnumOrderPrintStatus.不可打印,
				Bid = -1,
				WmsDanjbh = "",
				NetDanjbh = "",
				ClientHandleDate = nowDateTime,
				ServerHandleDate = nowDateTime,
				WmsYewy = "",
				WmsDdwname = "",
				NETORDER_FROMID = "",
				NETORDER_FROM = "",
				ERPORDER_ID = "",
				CUSTOMID = "",
				CUSTOMNAME = "",
				AGENTNAME = "",
				ADDRESS = "",
				WL_COMPANYID = "",
				WL_NUMBER = "",
				WL_COMPANYNAME = "",
				RECVNAME = "",
				RECVPHONE = "",
				PROVINCENAME = "",
				CITYNAME = "",
				DISTRICTNAME = "",
				STREETNAME = "",
				ORIGINALREMARK = "",
				IsFp = "",
				IsPj = "",
				IsQysy = "",
				IsSyzz = "",
				IsYjbb = "",
				PlatformType = Settings.Setings.EnumPlatformType.无,
				logi_dstRoute = "",
				logi_PayType = "",
				logi_monAccNum = "",
				logi_baojiaJine = "",
				logi_dsJine = "",
				logi_logcNum = "",
				logi_ysJine = "",
				logi_ysJineTotal = "",
				logi_shouhuory = "",
				logi_jinjianRiqi = "",
				logi_shoufqianshu = "",
				logi_shoufRiqi = "",
				logi_sendSheng = "",
				logi_sendShi = "",
				logi_sendXian = "",
				logi_sendAddress = "",
				logi_sendMan = "",
				logi_sendPhone = "",
				logi_feiyongTotal = "",
				logi_goodQty = "",
				logi_goodName = "",
				needBaojia = 0,
				logi_OrderId = "",
				logi_CreateDate = "",
				logi_ChanpinTypeStr = "",
				PrintDatetime = "",
				sysFirst = "",
				total_amt = "0",
				RelBId = -1,
				fplx = "",
				ServerTaskType = Settings.Setings.EnumServerTaskType.终止订单,
				PAIDCOSTS = 0,
				logi_ReceivePwd = "",
				logi_SubOrderSn = 0,
				logi_TelNum = "",
				logi_PhonNum = "",
				RelOrderId = -1,
				JingdongWl = "",
				IsHeTong = "",
			};
			_listCount++;
			MakeOrderAndDataTable(order);
			_handleQueue.Enqueue(new YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType>() { V1 = order, V2 = Settings.Setings.EnumTaskType.下传数据库 });//HandleQueue 进行处理
			_are.Set();
			AddMsgLog("新增队列" + erpId, Settings.Setings.EnumMessageType.提示, DateTime.Now, "");







			return res;
		}
		#endregion



























































































































































































		#region 扫码操作
		/// <summary>
		/// 录入的组 1 ERP编号,2 指定物流,3 重量kg,4长cm,5宽cm,6高cm
		/// </summary>
		YJT.ValuePara.MulitValue.TwoValueClass<string, Settings.Setings.EnumLogicType, double, double, double, double> _inputGroup = new YJT.ValuePara.MulitValue.TwoValueClass<string, Settings.Setings.EnumLogicType, double, double, double, double>();
		/// <summary>
		/// 默认物流,不指定自动计算,指定后,不更改指定物流的情况下,使用此物流
		/// </summary>
		Settings.Setings.EnumLogicType _defLogic = Settings.Setings.EnumLogicType.Default;
		/// <summary>
		/// 货品物流平台
		/// </summary>
		bool _isLogic;
		/// <summary>
		/// 货品重量
		/// </summary>
		bool _isWeight;
		/// <summary>
		/// 扫描的ERP编号
		/// </summary>
		bool _isId;
		/// <summary>
		/// 货品长
		/// </summary>
		bool _isLength;
		/// <summary>
		/// 货品宽
		/// </summary>
		bool _isWidth;
		/// <summary>
		/// 货品高
		/// </summary>
		bool _isHeight;
		/// <summary>
		/// 扫码的内容
		/// </summary>
		string _tempInputStr = "";
		/// <summary>
		/// 当输入框按下回车后,确定扫码结束
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tb_intupBox_KeyPress(object sender, KeyPressEventArgs e)
		{

			if (((int)e.KeyChar) == 13)
			{
				ListenHandle();
			}

		}
		/// <summary>
		/// 当按下执行按钮后,确认扫码结束
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, EventArgs e)
		{

			ListenHandle();

		}
		/// <summary>
		/// 重置一次当前录入的状态
		/// </summary>
		/// <param name="isDisplay"></param>
		void ResetInputGroup(bool isDisplay = true)
		{
			_isId = _isLogic = _isWeight = _isLength = _isWidth = _isHeight = false;
			_inputGroup.V1 = "";
			_inputGroup.V2 = Settings.Setings.EnumLogicType.Default;
			_inputGroup.V3 = 0;
			_inputGroup.V4 = 0;
			_inputGroup.V5 = 0;
			_inputGroup.V6 = 0;
			ShowInputGroup();
			AddMsgLog("重置操作", Settings.Setings.EnumMessageType.提示, DateTime.Now, "");
			splitContainer1.BackColor = System.Drawing.SystemColors.Control;
		}
		/// <summary>
		/// 重置当前显示
		/// </summary>
		void ShowInputGroup()
		{
			Lb_ErpId.Text = _inputGroup.V1;
			if (_comPort == null)
			{
				LB_Logic.Text = _defLogic.ToString();
			}
			else
			{
				LB_Logic.Text = _inputGroup.V2.ToString();
			}
			
			//LB_Logic.Text = "强制顺丰";
			Lb_Length.Text = _inputGroup.V4.ToString("#0.00");
			Lb_width.Text = _inputGroup.V5.ToString("#0.00");
			Lb_height.Text = _inputGroup.V6.ToString("#0.00");

		}
		/// <summary>
		/// 处理一次扫码动作
		/// </summary>
		void ListenHandle()
		{
			string errMsg = "";
			if (bll.VerVersion(out errMsg) == false)
			{
				AddMsgLog(errMsg + ",请退出后更新", Settings.Setings.EnumMessageType.严重, DateTime.Now, Common.PubMethod.GetNameSpace());
				_forceLock = true;
				_isPause = true;
				StopServer(true);
				MessageBox.Show(errMsg + ",请退出后更新");
				return;
			}
			if (!_isPause)
			{
				splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
				//空,表示重置
				//快递
				_tempInputStr = Tb_intupBox.Text.Trim();
				if (YJT.Text.Verification.IsNotNullOrEmpty(_tempInputStr))
				{
					if (_tempInputStr == "Reset..")
					{
						//重置
						ResetInputGroup();
					}
					else if (_tempInputStr == "StopOrder..")
					{
						_enableForceTop = false;
						this.TopMost = false;

						FrmStopOrder fso = new FrmStopOrder();
						fso.ShowDialog();

						_enableForceTop = true;
						this.TopMost = true;
						if (YJT.Text.Verification.IsNotNullOrEmpty(fso.InputErpDanjbh))
						{
							AddStopOrder(fso.InputErpDanjbh);
							ResetInputGroup();
							Tb_intupBox.Clear();
						}
						else
						{
							ResetInputGroup();
							Tb_intupBox.Clear();
						}

						return;
					}
					else if (_tempInputStr == "ReSys..")
					{
						//重启电脑
						ResetSystem();
						ResetInputGroup();
					}
					else if (_tempInputStr == "ShutdownServer..")
					{
						//服务器关机
						if (关闭服务器任务() == true)
						{
							_isPause = true;
							ExitApp();
						}
						ResetInputGroup();

					}
					else if (_tempInputStr == "ShutdownSys..")
					{
						//关机
						ShutdownSys();
						ResetInputGroup();
					}
					else if (_tempInputStr == "ExitApp..")
					{
						//退出程序
						ExitApp();
						ResetInputGroup();
					}
					else if (_tempInputStr == "StartServer..")
					{
						//启动服务
						StartServer();
						ResetInputGroup();
					}
					else if (_tempInputStr == "StopServer..")
					{
						//暂停服务
						StopServer();
						ResetInputGroup();
					}
					else if (_tempInputStr.ToLower() == "Log_ShunFeng".ToLower())
					{
						//指定物流顺丰
						_inputGroup.V2 = Settings.Setings.EnumLogicType.顺丰;
						_defLogic = Settings.Setings.EnumLogicType.顺丰;
						_isLogic = true;
					}
					else if (_tempInputStr.ToLower() == "Log_YouZhengEms".ToLower())
					{
						//指定物流邮政
						_inputGroup.V2 = Settings.Setings.EnumLogicType.邮政EMS;
						_defLogic = Settings.Setings.EnumLogicType.邮政EMS;
						_isLogic = true;
					}
					else if (_tempInputStr.ToLower() == "Log_JiTuBaiShi".ToLower())
					{
						//指定物流邮政
						_inputGroup.V2 = Settings.Setings.EnumLogicType.极兔百事;
						_defLogic = Settings.Setings.EnumLogicType.极兔百事;
						_isLogic = true;
					}
					else if (_tempInputStr.ToLower() == "Log_JingDongWuliu".ToLower())
					{
						//指定物流邮政
						_inputGroup.V2 = Settings.Setings.EnumLogicType.京东物流;
						_defLogic = Settings.Setings.EnumLogicType.京东物流;
						_isLogic = true;
					}
					else if (_tempInputStr.ToLower() == "Log_ZhongTong".ToLower())
					{
						//指定物流中通
						_inputGroup.V2 = Settings.Setings.EnumLogicType.中通快递;
						_defLogic = Settings.Setings.EnumLogicType.中通快递;
						_isLogic = true;
					}
					else if (_tempInputStr.ToLower() == "Log_ShenTong".ToLower())
					{
						//指定物流申通
						_inputGroup.V2 = Settings.Setings.EnumLogicType.申通快递;
						_defLogic = Settings.Setings.EnumLogicType.申通快递;
						_isLogic = true;
					}

					else if (YJT.Text.Verification.IsLeftLike(_tempInputStr, "WEH_KG_") == true)
					{
						//扫码重量
						string t = _tempInputStr.Replace("WEH_KG_", "");
						double t2 = 0;
						if (double.TryParse(t, out t2))
						{
							_inputGroup.V3 = t2;
							_isWeight = true;
						}
						else
						{
							AddMsgLog("未能识别重量值:" + _tempInputStr, Settings.Setings.EnumMessageType.提示, DateTime.Now);
						}

					}
					else if (YJT.Text.Verification.IsLeftLike(_tempInputStr, "VLE_") == true)
					{
						string t = "";
						double t2 = 0;
						//扫码体积
						if (YJT.Text.Verification.IsLeftLike(_tempInputStr, "VLE_L_CM:") == true)
						{
							//长
							t = _tempInputStr.Replace("VLE_L_CM:", "");
							if (double.TryParse(t, out t2))
							{
								_inputGroup.V4 = t2;
								_isLength = true;
							}
							else
							{
								AddMsgLog("未能识别长度值:" + _tempInputStr, Settings.Setings.EnumMessageType.提示, DateTime.Now);
							}
						}
						else if (YJT.Text.Verification.IsLeftLike(_tempInputStr, "VLE_W_CM:") == true)
						{
							//宽
							t = _tempInputStr.Replace("VLE_W_CM:", "");
							if (double.TryParse(t, out t2))
							{
								_inputGroup.V5 = t2;
								_isWidth = true;
							}
							else
							{
								AddMsgLog("未能识别宽度值:" + _tempInputStr, Settings.Setings.EnumMessageType.提示, DateTime.Now);
							}
						}
						else if (YJT.Text.Verification.IsLeftLike(_tempInputStr, "VLE_H_CM:") == true)
						{
							//高
							t = _tempInputStr.Replace("VLE_H_CM:", "");
							if (double.TryParse(t, out t2))
							{
								_inputGroup.V6 = t2;
								_isHeight = true;
							}
							else
							{
								AddMsgLog("未能识别高度值:" + _tempInputStr, Settings.Setings.EnumMessageType.提示, DateTime.Now);
							}
						}
						else
						{
							AddMsgLog("未能识别体积单位:" + _tempInputStr, Settings.Setings.EnumMessageType.提示, DateTime.Now);
						}
					}
					else
					{
						//扫描ERP编号
						_inputGroup.V1 = _tempInputStr;
						_isId = true;
					}
					AddMsgLog(_tempInputStr, Settings.Setings.EnumMessageType.输入, DateTime.Now, "");
					if (_isId)
					{
						double weight = 0d;
						if (_inputGroup.V3 > 0)
						{
						}
						else if (double.TryParse(label8.Text, out weight))
						{
							_inputGroup.V3 = weight;
							if (_inputGroup.V3 < 0)
							{
								_inputGroup.V3 = 0;
							}

						}
						else
						{
							_inputGroup.V3 = 0;
						}
						_isWeight = true;
						if (_inputGroup.V3 == 0)
						{
							_inputGroup.V3 = 4;
						}
						//if (_inputGroup.V3 <= 3.2)
						//{

						//	AddMsgLog("由于重量小于4公斤,当前重量"+_inputGroup.V3.ToString()+",不能生成物流因此退出:" + _tempInputStr, Settings.Setings.EnumMessageType.提示, DateTime.Now);
						//	YJT.Sound.TTSMicrosoft.Init().Speak("编号:" + _inputGroup.V1 + "<spause>重量过小", isSyn: true, pb: System.Speech.Synthesis.PromptBreak.ExtraSmall, filePath: YJT.Path.FunStrGetLocalPath() + "\\files\\yz.wav",nuberMode:1);
						//}
						//else
						{
							if (_comPort == null)
							{
								_inputGroup.V2 = _defLogic;
								_isLogic = true;
							}

							_inputGroup.V4 = 1;
							_isLength = true;
							_inputGroup.V5 = 1;
							_isWidth = true;
							_inputGroup.V6 = 1;
							_isHeight = true;

							//添加一组到队列 v1业务系统编号  v2物流 v3重量 v4长 v5宽 v6高
							//强制顺丰
							//_inputGroup.V2 = Settings.Setings.EnumLogicType.顺丰;
							//_isLogic = true;
							//结束强制
							AddHandleQueue(_inputGroup.V1, _inputGroup.V2, _inputGroup.V3, _inputGroup.V4, _inputGroup.V5, _inputGroup.V6);

						}
						ResetInputGroup(false);
					}
					Lb_lastInput.Text = _tempInputStr;
					ShowInputGroup();

					Tb_intupBox.Clear();
				}
				else
				{
					AddMsgLog("未输入内容", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
				}

			}
		}

		private bool 关闭服务器任务()
		{
			DateTime nowDateTime = DateTime.Now;
			bool res = false;
			MOD.BllMod.Order order = new MOD.BllMod.Order()
			{
				//初始化对象
				AddDate = nowDateTime,
				ErpId = "",
				Logic = Settings.Setings.EnumLogicType.Default,
				Weight = 0,
				Length = 0,
				Width = 0,
				Height = 0,
				Status = Settings.Setings.EnumOrderStatus.关机任务,
				ComputerName = _clientTag.ComputerName,
				Mac = _clientTag.Mac,
				Ip = _clientTag.Ip,
				ClientId = _clientTag.ClientId,
				OrderId = _listCount,
				ErrMsg = "新增关机任务",
				PrintStatus = Settings.Setings.EnumOrderPrintStatus.不可打印,
				Bid = -1,
				WmsDanjbh = "",
				NetDanjbh = "",
				ClientHandleDate = nowDateTime,
				ServerHandleDate = nowDateTime,
				WmsYewy = "",
				WmsDdwname = "",
				NETORDER_FROMID = "",
				NETORDER_FROM = "",
				ERPORDER_ID = "",
				CUSTOMID = "",
				CUSTOMNAME = "",
				AGENTNAME = "",
				ADDRESS = "",
				WL_COMPANYID = "",
				WL_NUMBER = "",
				WL_COMPANYNAME = "",
				RECVNAME = "",
				RECVPHONE = "",
				PROVINCENAME = "",
				CITYNAME = "",
				DISTRICTNAME = "",
				STREETNAME = "",
				ORIGINALREMARK = "",
				IsFp = "",
				IsPj = "",
				IsQysy = "",
				IsSyzz = "",
				IsYjbb = "",
				PlatformType = Settings.Setings.EnumPlatformType.无,
				logi_dstRoute = "",
				logi_PayType = "",
				logi_monAccNum = "",
				logi_baojiaJine = "",
				logi_dsJine = "",
				logi_logcNum = "",
				logi_ysJine = "",
				logi_ysJineTotal = "",
				logi_shouhuory = "",
				logi_jinjianRiqi = "",
				logi_shoufqianshu = "",
				logi_shoufRiqi = "",
				logi_sendSheng = "",
				logi_sendShi = "",
				logi_sendXian = "",
				logi_sendAddress = "",
				logi_sendMan = "",
				logi_sendPhone = "",
				logi_feiyongTotal = "",
				logi_goodQty = "",
				logi_goodName = "",
				needBaojia = 0,
				logi_OrderId = "",
				logi_CreateDate = "",
				logi_ChanpinTypeStr = "",
				PrintDatetime = "",
				sysFirst = "",
				total_amt = "0",
				RelBId = -1,
				fplx = "",
				ServerTaskType = Settings.Setings.EnumServerTaskType.关机任务,
				PAIDCOSTS = 0,
				logi_ReceivePwd = "",
				logi_SubOrderSn = 0,
				logi_PhonNum = "",
				logi_TelNum = "",
				RelOrderId = -1,
				JingdongWl = "",
				IsHeTong = "",
			};
			_listCount++;
			MakeOrderAndDataTable(order);
			bll.ClientAddShutdownServer(order);
			AddMsgLog("服务器关机", Settings.Setings.EnumMessageType.提示, DateTime.Now, "");
			res = true;
			return res;
		}

		/// <summary>
		/// 本机起始
		/// </summary>
		long _listCount = 0;
		/// <summary>
		/// 本机处理的ID 对应  一个order订单实体 和 一个 _dataTableQueueList中的行
		/// </summary>
		Dictionary<long, YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, System.Data.DataRow>> _orderList = new Dictionary<long, YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, System.Data.DataRow>>();
		void MakeOrderAndDataTable(MOD.BllMod.Order order)
		{
			System.Data.DataRow dr = _dataTableQueueList.NewRow();
			//对datatable赋值
			dr["CAddDate"] = order.AddDate.ToString();
			dr["CErpId"] = order.ErpId;
			dr["CLogic"] = order.Logic.ToString();
			dr["CWeight"] = order.Weight.ToString("#0.00");
			dr["CLength"] = order.Length.ToString("#0.00");
			dr["CWidth"] = order.Width.ToString("#0.00");
			dr["CHeight"] = order.Height.ToString("#0.00");
			dr["CStatus"] = order.Status.ToString();
			dr["ErrMsg"] = order.ErrMsg;
			dr["ClientId"] = order.ClientId.ToString();
			dr["PrintStatus"] = order.PrintStatus.ToString();
			dr["CTagObj"] = new
			{
				aaa = 1,
				bbb = 2
			};
			dr["WmsDanjbh"] = order.WmsDanjbh;
			dr["NetDanjbh"] = order.NetDanjbh;
			dr["OrderId"] = order.OrderId;
			dr["Bid"] = order.Bid;
			dr["ClientHandleDate"] = order.ClientHandleDate;
			dr["ServerHandleDate"] = order.ServerHandleDate;
			dr["WmsYewy"] = order.WmsYewy;
			dr["WmsDdwname"] = order.WmsDdwname;
			dr["NETORDER_FROMID"] = order.NETORDER_FROMID;
			dr["NETORDER_FROM"] = order.NETORDER_FROM;
			dr["ERPORDER_ID"] = order.ERPORDER_ID;
			dr["CUSTOMID"] = order.CUSTOMID;
			dr["CUSTOMNAME"] = order.CUSTOMNAME;
			dr["AGENTNAME"] = order.AGENTNAME;
			dr["ADDRESS"] = order.ADDRESS;
			dr["WL_COMPANYID"] = order.WL_COMPANYID;
			dr["WL_NUMBER"] = order.WL_NUMBER;
			dr["WL_COMPANYNAME"] = order.WL_COMPANYNAME;
			dr["RECVNAME"] = order.RECVNAME;
			dr["RECVPHONE"] = order.RECVPHONE;
			dr["PROVINCENAME"] = order.PROVINCENAME;
			dr["CITYNAME"] = order.CITYNAME;
			dr["DISTRICTNAME"] = order.DISTRICTNAME;
			dr["STREETNAME"] = order.STREETNAME;
			dr["ORIGINALREMARK"] = order.ORIGINALREMARK;
			dr["IsFp"] = order.IsFp;
			dr["IsPj"] = order.IsPj;
			dr["IsQysy"] = order.IsQysy;
			dr["IsSyzz"] = order.IsSyzz;
			dr["IsYjbb"] = order.IsYjbb;
			dr["PlatformType"] = order.PlatformType.ToString();
			dr["logi_dstRoute"] = order.logi_dstRoute;
			dr["logi_PayType"] = order.logi_PayType;
			dr["logi_monAccNum"] = order.logi_monAccNum;
			dr["logi_baojiaJine"] = order.logi_baojiaJine;
			dr["logi_dsJine"] = order.logi_dsJine;
			dr["logi_logcNum"] = order.logi_logcNum;
			dr["logi_ysJine"] = order.logi_ysJine;
			dr["logi_ysJineTotal"] = order.logi_ysJineTotal;
			dr["logi_shouhuory"] = order.logi_shouhuory;
			dr["logi_jinjianRiqi"] = order.logi_jinjianRiqi;
			dr["logi_shoufqianshu"] = order.logi_shoufqianshu;
			dr["logi_shoufRiqi"] = order.logi_shoufRiqi;
			dr["logi_sendSheng"] = order.logi_sendSheng;
			dr["logi_sendShi"] = order.logi_sendShi;
			dr["logi_sendXian"] = order.logi_sendXian;
			dr["logi_sendAddress"] = order.logi_sendAddress;
			dr["logi_sendMan"] = order.logi_sendMan;
			dr["logi_sendPhone"] = order.logi_sendPhone;
			dr["logi_feiyongTotal"] = order.logi_feiyongTotal;
			dr["logi_goodQty"] = order.logi_goodQty;
			dr["logi_goodName"] = order.logi_goodName;
			dr["needBaojia"] = order.needBaojia.ToString("#0.000");
			dr["logi_OrderId"] = order.logi_OrderId;
			dr["logi_CreateDate"] = order.logi_CreateDate;
			dr["logi_ChanpinTypeStr"] = order.logi_ChanpinTypeStr;
			dr["PrintDatetime"] = order.PrintDatetime;
			dr["sysFirst"] = order.sysFirst;
			dr["total_amt"] = order.total_amt;
			dr["RelBId"] = order.RelBId.ToString();
			dr["fplx"] = order.fplx;
			dr["ServerTaskType"] = order.ServerTaskType.ToString();
			dr["PAIDCOSTS"] = order.PAIDCOSTS.ToString();
			dr["logi_ReceivePwd"] = order.logi_ReceivePwd;
			dr["logi_SubOrderSn"] = order.logi_SubOrderSn.ToString();
			dr["logi_PhonNum"] = order.logi_PhonNum;
			dr["logi_TelNum"] = order.logi_TelNum;
			dr["RelOrderId"] = order.RelOrderId.ToString();
			dr["JingdongWl"] = order.JingdongWl;
			dr["IsHeTong"] = order.IsHeTong;
			//用于显示队列的表格
			_dataTableQueueList.Rows.Add(dr);
			//本机处理的ID 对应  一个order订单实体 和 一个 datatable中的行
			_orderList.Add(order.OrderId, new YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, DataRow>() { V1 = order, V2 = dr });

		}
		/// <summary>
		/// 添加一个任务到队列
		/// </summary>
		/// <param name="erpId"></param>
		/// <param name="logicType"></param>
		/// <param name="weight"></param>
		public void AddHandleQueue(string erpId, Settings.Setings.EnumLogicType logicType, double weight, double length, double width, double height)
		{
			DateTime nowDateTime = DateTime.Now;
			if (erpId.Contains(","))
			{
				string[] erpids = erpId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				MOD.BllMod.Order order = new MOD.BllMod.Order()
				{
					//初始化对象
					AddDate = DateTime.Now,
					ErpId = erpId,
					Logic = logicType,
					Weight = weight,
					Length = length,
					Width = width,
					Height = height,
					Status = Settings.Setings.EnumOrderStatus.新增未写缓存,
					ComputerName = _clientTag.ComputerName,
					Mac = _clientTag.Mac,
					Ip = _clientTag.Ip,
					ClientId = _clientTag.ClientId,
					OrderId = _listCount++,
					ErrMsg = "新增",
					PrintStatus = Settings.Setings.EnumOrderPrintStatus.不可打印,
					Bid = -1,
					WmsDanjbh = "",
					NetDanjbh = "",
					ClientHandleDate = nowDateTime,
					ServerHandleDate = nowDateTime,
					WmsYewy = "",
					WmsDdwname = "",
					NETORDER_FROMID = "",
					NETORDER_FROM = "",
					ERPORDER_ID = "",
					CUSTOMID = "",
					CUSTOMNAME = "",
					AGENTNAME = "",
					ADDRESS = "",
					WL_COMPANYID = "",
					WL_NUMBER = "",
					WL_COMPANYNAME = "",
					RECVNAME = "",
					RECVPHONE = "",
					PROVINCENAME = "",
					CITYNAME = "",
					DISTRICTNAME = "",
					STREETNAME = "",
					ORIGINALREMARK = "",
					IsFp = "",
					IsPj = "",
					IsQysy = "",
					IsSyzz = "",
					IsYjbb = "",
					PlatformType = Settings.Setings.EnumPlatformType.无,
					logi_dstRoute = "",
					logi_PayType = "",
					logi_monAccNum = "",
					logi_baojiaJine = "",
					logi_dsJine = "",
					logi_logcNum = "",
					logi_ysJine = "",
					logi_ysJineTotal = "",
					logi_shouhuory = "",
					logi_jinjianRiqi = "",
					logi_shoufqianshu = "",
					logi_shoufRiqi = "",
					logi_sendSheng = "",
					logi_sendShi = "",
					logi_sendXian = "",
					logi_sendAddress = "",
					logi_sendMan = "",
					logi_sendPhone = "",
					logi_feiyongTotal = "",
					logi_goodQty = "",
					logi_goodName = "",
					needBaojia = 0,
					logi_OrderId = "",
					logi_CreateDate = "",
					logi_ChanpinTypeStr = "",
					PrintDatetime = "",
					sysFirst = "",
					total_amt = "0",
					RelBId = -1,
					fplx = "",
					ServerTaskType = Settings.Setings.EnumServerTaskType.ERP组合单,
					PAIDCOSTS = 0,
					logi_ReceivePwd = "",
					logi_SubOrderSn = 0,
					logi_PhonNum = "",
					logi_TelNum = "",
					RelOrderId = -1,
					JingdongWl = "",
					IsHeTong = "",
				};
				List<MOD.BllMod.Order> subOrders = new List<BllMod.Order>();
				foreach (string item in erpids)
				{
					MOD.BllMod.Order t = new BllMod.Order()
					{
						AddDate = DateTime.Now,
						ErpId = item,
						Logic = logicType,
						Weight = weight,
						Length = length,
						Width = width,
						Height = height,
						Status = Settings.Setings.EnumOrderStatus.新增未写缓存,
						ComputerName = _clientTag.ComputerName,
						Mac = _clientTag.Mac,
						Ip = _clientTag.Ip,
						ClientId = _clientTag.ClientId,
						OrderId = _listCount++,
						ErrMsg = "新增",
						PrintStatus = Settings.Setings.EnumOrderPrintStatus.不可打印,
						Bid = -1,
						WmsDanjbh = "",
						NetDanjbh = "",
						ClientHandleDate = nowDateTime,
						ServerHandleDate = nowDateTime,
						WmsYewy = "",
						WmsDdwname = "",
						NETORDER_FROMID = "",
						NETORDER_FROM = "",
						ERPORDER_ID = "",
						CUSTOMID = "",
						CUSTOMNAME = "",
						AGENTNAME = "",
						ADDRESS = "",
						WL_COMPANYID = "",
						WL_NUMBER = "",
						WL_COMPANYNAME = "",
						RECVNAME = "",
						RECVPHONE = "",
						PROVINCENAME = "",
						CITYNAME = "",
						DISTRICTNAME = "",
						STREETNAME = "",
						ORIGINALREMARK = "",
						IsFp = "",
						IsPj = "",
						IsQysy = "",
						IsSyzz = "",
						IsYjbb = "",
						PlatformType = Settings.Setings.EnumPlatformType.无,
						logi_dstRoute = "",
						logi_PayType = "",
						logi_monAccNum = "",
						logi_baojiaJine = "",
						logi_dsJine = "",
						logi_logcNum = "",
						logi_ysJine = "",
						logi_ysJineTotal = "",
						logi_shouhuory = "",
						logi_jinjianRiqi = "",
						logi_shoufqianshu = "",
						logi_shoufRiqi = "",
						logi_sendSheng = "",
						logi_sendShi = "",
						logi_sendXian = "",
						logi_sendAddress = "",
						logi_sendMan = "",
						logi_sendPhone = "",
						logi_feiyongTotal = "",
						logi_goodQty = "",
						logi_goodName = "",
						needBaojia = 0,
						logi_OrderId = "",
						logi_CreateDate = "",
						logi_ChanpinTypeStr = "",
						PrintDatetime = "",
						sysFirst = "",
						total_amt = "0",
						RelBId = -1,
						fplx = "",
						ServerTaskType = Settings.Setings.EnumServerTaskType.ERP组合单子单,
						PAIDCOSTS = 0,
						logi_ReceivePwd = "",
						logi_SubOrderSn = 0,
						logi_PhonNum = "",
						logi_TelNum = "",
						RelOrderId = order.OrderId,
						JingdongWl = "",
						IsHeTong = "",
					};
					subOrders.Add(t);
				}
				if (subOrders.Count > 0)
				{

					foreach (MOD.BllMod.Order item in subOrders)
					{
						MakeOrderAndDataTable(item);
						_handleQueue.Enqueue(new YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType>() { V1 = item, V2 = Settings.Setings.EnumTaskType.下传数据库 });//HandleQueue 进行处理
					}
					MakeOrderAndDataTable(order);
					_handleQueue.Enqueue(new YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType>() { V1 = order, V2 = Settings.Setings.EnumTaskType.下传数据库 });//HandleQueue 进行处理
					_are.Set();
					AddMsgLog("新增ERP组合单队列" + erpId, Settings.Setings.EnumMessageType.提示, DateTime.Now, "");
				}
			}
			else
			{

				MOD.BllMod.Order order = new MOD.BllMod.Order()
				{
					//初始化对象
					AddDate = DateTime.Now,
					ErpId = erpId,
					Logic = logicType,
					Weight = weight,
					Length = length,
					Width = width,
					Height = height,
					Status = Settings.Setings.EnumOrderStatus.新增未写缓存,
					ComputerName = _clientTag.ComputerName,
					Mac = _clientTag.Mac,
					Ip = _clientTag.Ip,
					ClientId = _clientTag.ClientId,
					OrderId = _listCount,
					ErrMsg = "新增",
					PrintStatus = Settings.Setings.EnumOrderPrintStatus.不可打印,
					Bid = -1,
					WmsDanjbh = "",
					NetDanjbh = "",
					ClientHandleDate = nowDateTime,
					ServerHandleDate = nowDateTime,
					WmsYewy = "",
					WmsDdwname = "",
					NETORDER_FROMID = "",
					NETORDER_FROM = "",
					ERPORDER_ID = "",
					CUSTOMID = "",
					CUSTOMNAME = "",
					AGENTNAME = "",
					ADDRESS = "",
					WL_COMPANYID = "",
					WL_NUMBER = "",
					WL_COMPANYNAME = "",
					RECVNAME = "",
					RECVPHONE = "",
					PROVINCENAME = "",
					CITYNAME = "",
					DISTRICTNAME = "",
					STREETNAME = "",
					ORIGINALREMARK = "",
					IsFp = "",
					IsPj = "",
					IsQysy = "",
					IsSyzz = "",
					IsYjbb = "",
					PlatformType = Settings.Setings.EnumPlatformType.无,
					logi_dstRoute = "",
					logi_PayType = "",
					logi_monAccNum = "",
					logi_baojiaJine = "",
					logi_dsJine = "",
					logi_logcNum = "",
					logi_ysJine = "",
					logi_ysJineTotal = "",
					logi_shouhuory = "",
					logi_jinjianRiqi = "",
					logi_shoufqianshu = "",
					logi_shoufRiqi = "",
					logi_sendSheng = "",
					logi_sendShi = "",
					logi_sendXian = "",
					logi_sendAddress = "",
					logi_sendMan = "",
					logi_sendPhone = "",
					logi_feiyongTotal = "",
					logi_goodQty = "",
					logi_goodName = "",
					needBaojia = 0,
					logi_OrderId = "",
					logi_CreateDate = "",
					logi_ChanpinTypeStr = "",
					PrintDatetime = "",
					sysFirst = "",
					total_amt = "0",
					RelBId = -1,
					fplx = "",
					ServerTaskType = Settings.Setings.EnumServerTaskType.无,
					PAIDCOSTS = 0,
					logi_ReceivePwd = "",
					logi_SubOrderSn = 0,
					logi_PhonNum = "",
					logi_TelNum = "",
					RelOrderId = -1,
					JingdongWl = "",
					IsHeTong = "",
				};
				_listCount++;
				MakeOrderAndDataTable(order);
				_handleQueue.Enqueue(new YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType>() { V1 = order, V2 = Settings.Setings.EnumTaskType.下传数据库 });//HandleQueue 进行处理
				_are.Set();
				AddMsgLog("新增队列" + erpId, Settings.Setings.EnumMessageType.提示, DateTime.Now, "");
			}
		}

		/// <summary>
		/// 等待暂停线程循环
		/// </summary>
		System.Threading.AutoResetEvent _are = new System.Threading.AutoResetEvent(false);
		/// <summary>
		/// 处理队列的线程
		/// </summary>
		System.Threading.Thread _backWorkHandleQueueTh;
		System.Threading.Thread _TextBoxOutHandleQueueTh;
		System.Threading.Thread _AddMsgLogToFileTh;
		/// <summary>
		/// 待线程处理下传数据库或更新DATATABLE的列表,在HandleQueue方法中处理
		/// </summary>
		Queue<YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType>> _handleQueue = new Queue<YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType>>();




		System.Threading.AutoResetEvent _areGetServerHandledList = new System.Threading.AutoResetEvent(false);


		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}


		System.Threading.Thread _getServerHandledListTh = null;

		/// <summary>
		/// 重置行内容
		/// </summary>
		/// <param name="v1"></param>
		private void ResetDataTableRow(BllMod.Order r)
		{
			try
			{
				if (_orderList.ContainsKey(r.OrderId))
				{
					//更新datatable
					_orderList[r.OrderId].V1 = r;
					System.Data.DataRow dr = _orderList[r.OrderId].V2;
					if (YJT.Text.Verification.IsLeftLike(r.Status.ToString(), "异常"))
					{
						YJT.Sound.TTSMicrosoft.Init().Speak("编号:" + r.ErpId + "<spause>" + r.Status.ToString(), isSyn: true, pb: System.Speech.Synthesis.PromptBreak.ExtraSmall, filePath: YJT.Path.FunStrGetLocalPath() + "\\files\\bj.wav", nuberMode: 1);
					}
					else if (r.Status.ToString() == "单据完成" && r.Logic.ToString() == "邮政EMS" && r.ServerTaskType.ToString() == "新单据")
					{
						YJT.Sound.TTSMicrosoft.Init().Speak("编号:" + r.ErpId + "<spause>使用邮政快递", isSyn: true, pb: System.Speech.Synthesis.PromptBreak.ExtraSmall, filePath: YJT.Path.FunStrGetLocalPath() + "\\files\\yz.wav", nuberMode: 1);
					}
					if (YJT.Text.Verification.IsLeftLike(r.ErrMsg.ToString(), "提示:"))
					{
						YJT.Sound.TTSMicrosoft.Init().Speak("编号:" + r.ErpId + "<spause>" + r.ErrMsg.ToString(), isSyn: true, pb: System.Speech.Synthesis.PromptBreak.ExtraSmall, filePath: YJT.Path.FunStrGetLocalPath() + "\\files\\yz.wav", nuberMode: 1);
					}
					dr["CAddDate"] = r.AddDate.ToString();
					dr["CErpId"] = r.ErpId;
					dr["CLogic"] = r.Logic.ToString();
					dr["CWeight"] = r.Weight.ToString("#0.00");
					dr["CLength"] = r.Length.ToString("#0.00");
					dr["CWidth"] = r.Width.ToString("#0.00");
					dr["CHeight"] = r.Height.ToString("#0.00");
					dr["CStatus"] = r.Status.ToString();
					dr["ErrMsg"] = r.ErrMsg;
					dr["ClientId"] = r.ClientId.ToString();
					dr["PrintStatus"] = r.PrintStatus.ToString();
					//dr["CTagObj"] = r.ct
					dr["WmsDanjbh"] = r.WmsDanjbh;
					dr["NetDanjbh"] = r.NetDanjbh;
					dr["OrderId"] = r.OrderId;
					dr["Bid"] = r.Bid;
					dr["ClientHandleDate"] = r.ClientHandleDate;
					dr["ServerHandleDate"] = r.ServerHandleDate;
					dr["WmsYewy"] = r.WmsYewy;
					dr["WmsDdwname"] = r.WmsDdwname;
					dr["NETORDER_FROMID"] = r.NETORDER_FROMID;

					dr["NETORDER_FROM"] = r.NETORDER_FROM;
					dr["ERPORDER_ID"] = r.ERPORDER_ID;
					dr["CUSTOMID"] = r.CUSTOMID;
					dr["CUSTOMNAME"] = r.CUSTOMNAME;
					dr["AGENTNAME"] = r.AGENTNAME;
					dr["ADDRESS"] = r.ADDRESS;
					dr["WL_COMPANYID"] = r.WL_COMPANYID;
					dr["WL_NUMBER"] = r.WL_NUMBER;
					dr["WL_COMPANYNAME"] = r.WL_COMPANYNAME;
					dr["RECVNAME"] = r.RECVNAME;
					dr["RECVPHONE"] = r.RECVPHONE;
					dr["PROVINCENAME"] = r.PROVINCENAME;
					dr["CITYNAME"] = r.CITYNAME;
					dr["DISTRICTNAME"] = r.DISTRICTNAME;
					dr["STREETNAME"] = r.STREETNAME;
					dr["ORIGINALREMARK"] = r.ORIGINALREMARK;
					dr["IsFp"] = r.IsFp;
					dr["IsPj"] = r.IsPj;
					dr["IsQysy"] = r.IsQysy;
					dr["IsSyzz"] = r.IsSyzz;
					dr["PlatformType"] = r.PlatformType.ToString();
					dr["logi_dstRoute"] = r.logi_dstRoute;
					dr["logi_PayType"] = r.logi_PayType;
					dr["logi_monAccNum"] = r.logi_monAccNum;
					dr["logi_baojiaJine"] = r.logi_baojiaJine;
					dr["logi_dsJine"] = r.logi_dsJine;
					dr["logi_logcNum"] = r.logi_logcNum;
					dr["logi_ysJine"] = r.logi_ysJine;
					dr["logi_ysJineTotal"] = r.logi_ysJineTotal;
					dr["logi_shouhuory"] = r.logi_shouhuory;
					dr["logi_jinjianRiqi"] = r.logi_jinjianRiqi;
					dr["logi_shoufqianshu"] = r.logi_shoufqianshu;
					dr["logi_shoufRiqi"] = r.logi_shoufRiqi;
					dr["logi_sendSheng"] = r.logi_sendSheng;
					dr["logi_sendShi"] = r.logi_sendShi;
					dr["logi_sendXian"] = r.logi_sendXian;
					dr["logi_sendAddress"] = r.logi_sendAddress;
					dr["logi_sendMan"] = r.logi_sendMan;
					dr["logi_sendPhone"] = r.logi_sendPhone;
					dr["logi_feiyongTotal"] = r.logi_feiyongTotal;
					dr["logi_goodQty"] = r.logi_goodQty;
					dr["logi_goodName"] = r.logi_goodName;
					dr["needBaojia"] = r.needBaojia.ToString("#0.00");
					dr["logi_OrderId"] = r.logi_OrderId;
					dr["logi_CreateDate"] = r.logi_CreateDate;
					dr["logi_ChanpinTypeStr"] = r.logi_ChanpinTypeStr;
					dr["PrintDatetime"] = r.PrintDatetime;
					dr["sysFirst"] = r.sysFirst;
					dr["total_amt"] = r.total_amt;
					dr["RelBId"] = r.RelBId.ToString();
					dr["fplx"] = r.fplx;
					dr["ServerTaskType"] = r.ServerTaskType.ToString();
					dr["PAIDCOSTS"] = r.PAIDCOSTS.ToString();
					dr["logi_ReceivePwd"] = r.logi_ReceivePwd;
					dr["logi_SubOrderSn"] = r.logi_SubOrderSn.ToString();
					dr["logi_PhonNum"] = r.logi_PhonNum;
					dr["logi_TelNum"] = r.logi_TelNum;
					dr["RelOrderId"] = r.RelOrderId.ToString();
					dr["JingdongWl"] = r.JingdongWl;
					dr["IsHeTong"] = r.IsHeTong;
					if (dataGridView1.InvokeRequired)
					{
						dataGridView1.Invoke(new Action<DataGridView>((dgv) =>
						{
							dgv.Refresh();
						}), dataGridView1);
					}
					else
					{
						dataGridView1.Refresh();
					}

				}
				else
				{
					AddMsgLog("列表中不存在此ID:" + r.OrderId.ToString() + "无法刷新行", Settings.Setings.EnumMessageType.异常, DateTime.Now, "DelDataTableRow", true, true);
				}
			}
			catch (Exception ee)
			{
				AddMsgLog(ee.ToString() + r.OrderId.ToString(), Settings.Setings.EnumMessageType.异常, DateTime.Now, "DelDataTableRow", true, true);
			}
		}
		/// <summary>
		/// 删除一行
		/// </summary>
		/// <param name="r"></param>
		public void DelDataTableRow(MOD.BllMod.Order r)
		{
			try
			{
				if (_orderList.ContainsKey(r.OrderId))
				{
					_dataTableQueueList.Rows.Remove(_orderList[r.OrderId].V2);
					_orderList.Remove(r.OrderId);
					if (dataGridView1.InvokeRequired)
					{
						dataGridView1.Invoke(new Action<DataGridView>((dgv) =>
						{
							dgv.Refresh();
						}), dataGridView1);
					}
					else
					{
						dataGridView1.Refresh();
					}

				}
				else
				{
					AddMsgLog("列表中不存在此ID:" + r.OrderId.ToString() + "无法移除行", Settings.Setings.EnumMessageType.异常, DateTime.Now, "DelDataTableRow", true, true);
				}
			}
			catch (Exception ee)
			{
				AddMsgLog(ee.ToString() + r.OrderId.ToString(), Settings.Setings.EnumMessageType.异常, DateTime.Now, "DelDataTableRow", true, true);
			}

		}

		#endregion























































		int _handCount = 0;
		#region 线程循环处理
		/// <summary>
		/// 队列处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleQueue()
		{
			bool isOK = false;
			int errCode = -1;
			string errMsg = "";

			string nameSpace = "";
			Settings.Setings.EnumMessageType zt = Settings.Setings.EnumMessageType.测试信息;
			string errMsg2 = "";
			while (true)
			{
				if (_isPause == true)
				{
					AddMsgLog("下传队列暂停", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
					_are.WaitOne();
					AddMsgLog("下传队列启动", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
				}
				else
				{
					if (_handleQueue.Count > 0)
					{
						YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType> t = _handleQueue.Dequeue();
						switch (t.V2)
						{
							case Settings.Setings.EnumTaskType.下传数据库:
								bll.SendNewOrder(t.V1, out isOK, out errCode, out errMsg);
								switch (errCode)
								{
									case -1:
										//单据已经下传过了
										errMsg2 = t.V1.ErpId + "曾经下传过\r\n\t" + errMsg;
										zt = Settings.Setings.EnumMessageType.异常;
										nameSpace = "[" + t.V2.ToString() + "].[" + errCode.ToString() + "].[SendNewOrder]";
										DelDataTableRow(t.V1);
										break;
									case 0:
										if (t.V1.ServerTaskType == Settings.Setings.EnumServerTaskType.添加物流子单)
										{
											YJT.Sound.TTSMicrosoft.Init().Speak(t.V1.ErpId + "子单添加", isSyn: true, pb: System.Speech.Synthesis.PromptBreak.ExtraSmall, filePath: YJT.Path.FunStrGetLocalPath() + "\\files\\yz.wav", nuberMode: 1);
										}
										else if (t.V1.ServerTaskType == Settings.Setings.EnumServerTaskType.ERP组合单)
										{
											YJT.Sound.TTSMicrosoft.Init().Speak(t.V1.ErpId + "组合单添加", isSyn: true, pb: System.Speech.Synthesis.PromptBreak.ExtraSmall, filePath: YJT.Path.FunStrGetLocalPath() + "\\files\\yz.wav", nuberMode: 1);
										}

										//正常回写
										errMsg2 = t.V1.ErpId + "下传完成,等待服务端处理";
										zt = Settings.Setings.EnumMessageType.提示;
										nameSpace = "[" + t.V2.ToString() + "].[" + errCode.ToString() + "].[SendNewOrder]";
										ResetDataTableRow(t.V1);
										break;
									case 1:
									//新增子单任务

									default:
										break;
								}
								AddMsgLog(errMsg2, zt, DateTime.Now, nameSpace, true, true);
								break;
							//case Settings.Setings.EnumTaskType.要求停止:
							//	bll.ClientRequestDel(t.V1, out isOK, out errCode, out errMsg);
							//	if (errCode == 0)
							//	{
							//		errMsg2 = "下传删除成功,等待服务端执行" + t.V1.ErpId;
							//		zt = Settings.Setings.EnumMessageType.提示;
							//		nameSpace = "[" + t.V2.ToString() + "].[" + errCode.ToString() + "].[RequestDel]";
							//		ResetDataTableRow(t.V1);
							//	}
							//	else if (errCode == 1)
							//	{
							//		errMsg2 = "直接删除成功" + t.V1.ErpId;
							//		zt = Settings.Setings.EnumMessageType.提示;
							//		nameSpace = "[" + t.V2.ToString() + "].[" + errCode.ToString() + "].[RequestDel]";
							//		DelDataTableRow(t.V1);
							//	}
							//	else
							//	{
							//		ResetDataTableRow(t.V1);
							//		errMsg2 = "删除失败" + t.V1.ErpId + " 原因:" + errMsg;
							//		zt = Settings.Setings.EnumMessageType.提示;
							//		nameSpace = "[" + t.V2.ToString() + "].[" + errCode.ToString() + "].[RequestDel]";
							//	}
							//	AddMsgLog(errMsg2, zt, DateTime.Now, nameSpace, true, true);
							//	break;
							case Settings.Setings.EnumTaskType.更改DataTable状态:
								ResetDataTableRow(t.V1);
								bll.ClientViewed(t.V1, out isOK, out errCode, out errMsg);
								break;
							default:
								break;
						}
						try
						{
							_handCount++;
							if (_handCount > 10)
							{
								YJT.MSystem.GC.Collect();
								_handCount = 0;
							}
						}
						catch { }

					}
					else
					{
						try
						{
							YJT.MSystem.GC.Collect();
						}
						catch { }
						_are.WaitOne();
					}
				}

			}
		}
		/// <summary>
		/// 获取服务端处理完成的单据,用于刷新datagridview的内容变动,以及,打印
		/// </summary>
		public void GetServerHandledList()
		{
			bool isOk = false;
			string errMsg = "";
			int errCode = 0;
			while (true)
			{
				if (_isPause == false)
				{
					//AddMsgLog("刷新一次", Settings.Setings.EnumMessageType.测试信息, DateTime.Now, "", false, true);
					List<MOD.BllMod.Order> orders = bll.GetServerHandledList(out isOk, out errMsg, out errCode);
					if (orders.Count > 0)
					{
						foreach (MOD.BllMod.Order order in orders)
						{
							YJT.ValuePara.MulitValue.TwoValueClass<MOD.BllMod.Order, Settings.Setings.EnumTaskType> t = new YJT.ValuePara.MulitValue.TwoValueClass<BllMod.Order, Settings.Setings.EnumTaskType>();
							t.V1 = order;
							t.V2 = Settings.Setings.EnumTaskType.更改DataTable状态;

							_handleQueue.Enqueue(t);//HandleQueue 进行处理
						}
						_are.Set();
					}
					System.Threading.Thread.Sleep(1000 * 10);
				}
				else
				{
					AddMsgLog("同步队列暂停", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
					_areGetServerHandledList.WaitOne();
					AddMsgLog("同步队列启动", Settings.Setings.EnumMessageType.提示, DateTime.Now, Common.PubMethod.GetNameSpace());
				}
			}

		}
		#endregion

		private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
		{

			if (dataGridView2.SelectedRows.Count > 0)
			{
				stbSelectPS.Text = dataGridView2.SelectedRows[0].Cells["SORIGINALREMARK"].Value.ToString();
				stbSelectBID.Text = dataGridView2.SelectedRows[0].Cells["SBid"].Value.ToString();
				stbSelectDanjbh.Text = dataGridView2.SelectedRows[0].Cells["SErpId"].Value.ToString();
			}

		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				_comPort.Close();
				_comPort = null;
				try
				{
					RegistryKey key = Registry.LocalMachine;
					RegistryKey subKey1 = key.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\COM Name Arbiter", true);
					RegistryKey subKey2 = key.OpenSubKey(@"SYSTEM\ControlSet002\Control\COM Name Arbiter", true);

					try
					{
						subKey1.DeleteValue(@"ComDB");

					}
					catch { }
					try
					{
						subKey2.DeleteValue(@"ComDB");

					}
					catch { }
					subKey1.Close();
					subKey1.Dispose();
					subKey1 = null;
					subKey2.Close();
					subKey2.Dispose();
					subKey2 = null;
					key.Close();
					key.Dispose();
					key = null;
				}
				catch { }
			}
			catch (Exception ee)
			{
			}
		}

		private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
