using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogisticsRePrint
{
    public partial class Form1 : Form
    {
        YJT.DataBase.DbHelper _dbhLocal = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _dbhLocal = new YJT.DataBase.DbHelperSqlServer("127.0.0.1", "YanduECommerceAutomaticPrinting", "sa", "qzmpchen", "1433");
            bool flag = false;
            Exception lastException = null;
            DbParameter[] paras = null;
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataSet ds = new System.Data.DataSet();

            var sqlcmd = @"select top 100 * from BllMod_Order ";
            for (int i = 0; i < 4; i++)
            {
                using (System.Data.SqlClient.SqlConnection sqlconn = new System.Data.SqlClient.SqlConnection(_dbhLocal.SqlConnStr))
                {
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sqlcmd, sqlconn))
                    {
                        da.SelectCommand.CommandTimeout = 0;
                        if (paras != null)
                        {
                            if (paras.Length > 0)
                            {
                                da.SelectCommand.Parameters.AddRange(paras);
                            }
                        }
                        try
                        {
                            da.Fill(ds);
                            flag = true;
                            break;
                        }
                        catch (Exception ee)
                        {
                            lastException = ee;
                            System.Threading.Thread.Sleep(5000);
                        }
                    }
                    try
                    {
                        sqlconn.Close();
                    }
                    catch { }
                }
            }
            if(dt!=null && dt.Rows.Count > 0)
            {
                
            }
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Refresh();
        }

    }
}
