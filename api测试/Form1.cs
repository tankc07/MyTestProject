using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace api测试
{
	
	public partial class Form1 : Form
	{
		struct LOGFONT
		{
			public int lfHeight;
			// Other fields omitted for brevity
		}
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(System.Drawing.Point point);
		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
		[DllImport("gdi32.dll")]
		static extern IntPtr CreateFontIndirect(LOGFONT lplf);
		[DllImport("gdi32.dll")]
		static extern bool DeleteObject(IntPtr hObject);
		const uint WM_SETFONT = 0x30;
		const int GWL_WNDPROC = -4;
		public Form1()
		{
			InitializeComponent();
		}
		IntPtr hwnd;
		private void timer1_Tick(object sender, EventArgs e)
		{
			try
			{
				System.Drawing.Point mousePos = new System.Drawing.Point();
				GetCursorPos(ref mousePos);
				hwnd = WindowFromPoint(mousePos);
				textBox1.Text = hwnd.ToString();
			}
			catch (Exception ee){
				textBox1.Text = ee.ToString();
			}
			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			timer1.Enabled = !timer1.Enabled;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			
		}

		private void button2_Click(object sender, EventArgs e)
		{
			System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(hwnd);
			System.Drawing.Font font = new System.Drawing.Font("Arial", 100);
			graphics.DrawString("Hello, World!", font, System.Drawing.Brushes.Black, new System.Drawing.PointF(0, 0));
		}

		private void button3_Click(object sender, EventArgs e)
		{
			//var a = Control.FromHandle(hwnd);
			//if (a != null)
			//{
			//	textBox1.Text = a.ToString();
			//	Form form = (Form)Control.FromHandle(hwnd);
			//	if (form == null)
			//	{
			//		form = Control.FromHandle(hwnd).FindForm();
			//	}
			//	if (form != null)
			//	{
			//		System.Drawing.Font font = new System.Drawing.Font("Arial", 100);
			//		form.Font = font;
			//	}
			//}

			FontDialog fd = new FontDialog();
			if (fd.ShowDialog() == DialogResult.OK)
			{
				Font font = fd.Font;

				//System.Drawing.Font font = new System.Drawing.Font("Arial",1,FontStyle.Bold);
				IntPtr hFont = font.ToHfont();
				SendMessage(hwnd, WM_SETFONT, hFont, hFont);
			}


			//{
			//	LOGFONT lo = new LOGFONT();
			//	lo.lfHeight = 100;
			//	int t1 = Marshal.SizeOf(lo);
			//	IntPtr hFon2 = Marshal.AllocHGlobal(t1);
			//	Marshal.StructureToPtr(lo, hFon2, true);
			//	SendMessage(hwnd, WM_SETFONT, hFon2, (IntPtr)(-1));
			//	//SetWindowLong(hwnd, GWL_WNDPROC, hFon2);
			//	//SetWindowLong(hwnd, GWL_WNDPROC, hFont);
			//}

			//{
			//	LOGFONT lf = new LOGFONT();
			//	lf.lfHeight = 100;
			//	IntPtr hFont = CreateFontIndirect(lf);

			//	SendMessage(hwnd, WM_SETFONT, hFont, (IntPtr)(-1));

			//	// Don't forget to delete the font handle when you're done
			//	DeleteObject(hFont);
			//}



		}
	}
}
