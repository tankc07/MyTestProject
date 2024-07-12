using System;
using System.Windows.Forms;

namespace YanduECommerceAutomaticPrinting
{
	public class MyTabControl : TabControl
	{
		bool tabsVisible = true;
		public bool TabsVisible
		{
			get
			{
				return tabsVisible;
			}
			set
			{
				if (tabsVisible == value)
				{
					return;
				}
				tabsVisible = value;
				RecreateHandle();
			}
		}
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x1328)
			{
				if (!tabsVisible && !DesignMode)
				{
					m.Result = (IntPtr)1;
					return;
				}
			}
			base.WndProc(ref m);
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.Tab) || keyData == (Keys.Control | Keys.Shift | Keys.Tab) || keyData == (Keys.Left) || keyData == (Keys.Right))
			{
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
