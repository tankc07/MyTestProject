using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace 测试
{
	public partial class Form1 : Form
	{
		[DllImport("user32.dll")]
		private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
		public Form1()
		{
			InitializeComponent();
		}
		// 定义窗体透明毛玻璃效果的结构体
		private struct WindowCompositionAttributeData
		{
			public WindowCompositionAttribute Attribute;
			public IntPtr Data;
			public int SizeOfData;
		}

		private enum WindowCompositionAttribute
		{
			// 启用透明毛玻璃效果
			WCA_ACCENT_POLICY = 19
		}

		private struct AccentPolicy
		{
			public AccentState AccentState;
			public int AccentFlags;
			public int GradientColor;
			public int AnimationId;
		}

		private enum AccentState
		{
			// 使用透明毛玻璃效果
			ACCENT_ENABLE_BLURBEHIND = 0
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//// 设置透明毛玻璃效果
			////var windowHelper = new WindowInteropHelper(this.Handle);
			//var accent = new AccentPolicy();
			//accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
			//accent.AccentFlags = 50;
			//accent.GradientColor = 0x0000987;

			//var accentStructSize = Marshal.SizeOf(accent);

			//var accentPtr = Marshal.AllocHGlobal(accentStructSize);
			//Marshal.StructureToPtr(accent, accentPtr, false);

			//var data = new WindowCompositionAttributeData();
			//data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
			//data.SizeOfData = accentStructSize;
			//data.Data = accentPtr;

			//SetWindowCompositionAttribute(this.Handle, ref data);

			//Marshal.FreeHGlobal(accentPtr);



			
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddArc(0, 0, 20, 20, 180, 90);
			path.AddArc(Width - 21, 0, 20, 20, 270, 90);
			path.AddArc(Width - 21, Height - 21, 20, 20, 0, 90);
			path.AddArc(0, Height - 21, 20, 20, 90, 90);
			path.CloseFigure();

			// 设置窗体的 Region 属性
			Region = new Region(path);
			base.OnSizeChanged(e);
		}
	}
}
