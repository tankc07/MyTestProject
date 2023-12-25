using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YanduECommerceAutomaticPrinting
{
	public partial class FrmStopOrder : Form
	{
		string _inputErpDanjbh = "";
		public string InputErpDanjbh
		{
			get
			{
				return _inputErpDanjbh;
			}
			set
			{
				_inputErpDanjbh = value;
			}

		}
		public FrmStopOrder()
		{
			InitializeComponent();
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.WindowState = FormWindowState.Normal;
		}

		private void FrmStopOrder_Load(object sender, EventArgs e)
		{
			this.Focus();
			textBox1.Focus();
			InputErpDanjbh = "";
		}
		

		private void button1_Click(object sender, EventArgs e)
		{
			InputErpDanjbh = textBox1.Text;
			this.Close();
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (((int)e.KeyChar) == 13)
			{
				button1.PerformClick();
			}
		}
	}
}
