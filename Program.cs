using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Quản_lý_bán_hàng
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Data Source=DESKTOP-KLPOHE4\\SQLEXPRESS;Initial Catalog=QLbanhang;Integrated Security=True";
            try
            {
                conn.Open();
                MessageBox.Show("Mo ket noi thanh cong");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            Application.Run(new FrmHDN());
        }
    }
}
