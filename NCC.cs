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
    public partial class FrmNCC : Form
    {
        DataTable tblNCC;
        public FrmNCC()
        {
            InitializeComponent();
        }

        private void FrmNCC_Load(object sender, EventArgs e)
        {
            LoadDataToGridview();
        }
        private void LoadDataToGridview()
        {
                DAO.OpenConnection();
                string sql = "select * from NhaCC";
                tblNCC = DAO.GetDataToTable(sql);
                GridViewNCC.DataSource = tblNCC;
                //GridViewNCC.Columns[0].HeaderText = "Mã NCC";
                //GridViewNCC.Columns[1].HeaderText = "Tên NCC";
        }

        private void GridViewNCC_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMaNCC.Text = GridViewNCC.CurrentRow.Cells["MaNCC"].Value.ToString();
            txtTenNCC.Text = GridViewNCC.CurrentRow.Cells["TenNCC"].Value.ToString();
            txtSDT.Text = GridViewNCC.CurrentRow.Cells["SDT"].Value.ToString();
            txtDiachi.Text = GridViewNCC.CurrentRow.Cells["Diachi"].Value.ToString();
            txtMaNCC.Enabled = false;
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
            txtMaNCC.Enabled = true; 
            txtMaNCC.Focus();
        }
        private void ResetValue()
        {
            txtMaNCC.Text = "";
            txtTenNCC.Text = "";
            txtSDT.Text = "";
            txtDiachi.Text = "";
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (txtMaNCC.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập mã nhà cung cấp");
                txtMaNCC.Focus();
                return;
            }
            if (txtTenNCC.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập tên nhà cung cấp");
                txtTenNCC.Focus();
                return;
            }
            if (txtSDT.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập số điện thoại");
                txtSDT.Focus();
                return;
            }
            if (txtDiachi.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập địa chỉ");
                txtDiachi.Focus();
                return;
            }
            string sqlcheckkey = "select * from NhaCC where MaNCC = '" + txtMaNCC.Text.Trim() + "'";
            if (DAO.CheckKeyExit(sqlcheckkey))
            {
                MessageBox.Show("Mã nhà cung cấp đã tồn tại, bạn phải nhập mã khác");
                DAO.CloseConnection();
                txtMaNCC.Focus();
                return;
            }
            else
            {
                DAO.OpenConnection();
                string sql = " insert into NhaCC(MaNCC,TenNCC,Diachi,SDT)  values('" + txtMaNCC.Text.Trim() + "',N'" + txtTenNCC.Text.Trim() + "',N'" + txtDiachi.Text.Trim() + "',N'" + txtSDT.Text.Trim() + "')";
                DAO.RunSQL(sql);
                LoadDataToGridview();
                ResetValue();
                btnXoa.Enabled = true;
                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnBoqua.Enabled = false;
                btnLuu.Enabled = false;
                txtMaNCC.Enabled = false;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string sql = "update NhaCC set TenNCC = N'" + txtTenNCC.Text.Trim() + 
                "', Diachi = N'" + txtDiachi.Text.Trim() + "', SDT = N'" + txtSDT.Text.Trim() + "'where MaNCC = '" + txtMaNCC.Text + "'";
            DAO.RunSQL(sql);
            LoadDataToGridview();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string sql;
            if (txtMaNCC.Text == "") 
            {
                MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có muốn xoá không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                sql = "delete from NhaCC where MaNCC = '" + txtMaNCC.Text + "'";
                DAO.RunSQL(sql);
                LoadDataToGridview();
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
            txtMaNCC.Enabled = false;
        }
    }
}
