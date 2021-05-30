using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;


namespace Quản_lý_bán_hàng
{
    public partial class FrmHanggiay : Form
    {
        DataTable tblHanggiay;
        public FrmHanggiay()
        {
            InitializeComponent();
        }

        private void FrmHanggiay_Load(object sender, EventArgs e)
        {
            LoadDataGridView();
        }
        private void LoadDataGridView()
        {
                DAO.OpenConnection();
                string sql = "select * from Hanggiay";
                tblHanggiay = DAO.GetDataToTable(sql);
                GridViewHanggiay.DataSource = tblHanggiay;
        }
        private void GridViewHanggiay_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMahang.Text = GridViewHanggiay.CurrentRow.Cells["Mahang"].Value.ToString();
            txtTenhang.Text = GridViewHanggiay.CurrentRow.Cells["Tenhang"].Value.ToString();
            txtMahang.Enabled = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            btnBoqua.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = true;
            btnThem.Enabled = false;
            ResetValue(); 
            txtMahang.Enabled = true;
            txtMahang.Focus();

        }
        private void ResetValue()
        {
            txtMahang.Text = "";
            txtTenhang.Text = "";
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (txtMahang.Text == "")
            {
                MessageBox.Show("bạn chưa nhập mã hãng giày");
                txtMahang.Focus();
                return;
            }
            if (txtTenhang.Text == "")
            {
                MessageBox.Show("bạn chưa nhập tên hãng giày");
                txtTenhang.Focus();
                return;
            }
            string sqlcheckkey = "select * from Hanggiay where Mahang = '" + txtMahang.Text.Trim() + "'";
            if (DAO.CheckKeyExit(sqlcheckkey))
            {
                MessageBox.Show("Mã hãng giày đã tồn tại, bạn phải nhập mã khác");
                DAO.CloseConnection();
                txtMahang.Focus();
                return;
            }
            else
            {
                DAO.OpenConnection();
                string sql = " insert into Hanggiay(Mahang,Tenhang)  values('" + txtMahang.Text.Trim() + "',N'" + txtTenhang.Text.Trim() + "')";
                DAO.RunSQL(sql);
                LoadDataGridView();
                ResetValue();
                btnXoa.Enabled = true;
                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnBoqua.Enabled = false;
                btnLuu.Enabled = false;
                txtMahang.Enabled = false;

            }

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            txtMahang.Enabled = false;
            string sql = "update Hanggiay set Tenhang = N'" + txtTenhang.Text.Trim() 
                 + "'where Mahang = '" + txtMahang.Text + "'";
            DAO.RunSQL(sql);
            LoadDataGridView();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string sql;
            if (txtMahang.Text == "") //nếu chưa chọn bản ghi nào
            {
                MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có muốn xoá không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                sql = "delete from Hanggiay where Mahang = '" + txtMahang.Text + "'";
                DAO.RunSQL(sql);
                LoadDataGridView();
                ResetValue();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBoqua_Click(object sender, EventArgs e)
        {
            ResetValue();
            btnBoqua.Enabled = false;
            btnThem.Enabled = true;
            btnXoa.Enabled = true;
            btnSua.Enabled = true;
            btnLuu.Enabled = false;
            txtMahang.Enabled = false;

        }
    }
}
