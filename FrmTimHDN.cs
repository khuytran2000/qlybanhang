using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Quản_lý_bán_hàng
{
    public partial class FrmTimHDN : Form
    {
        DataTable tblHDN;
        public FrmTimHDN()
        {
            InitializeComponent();
        }
        private void FrmTimHDN_Load(object sender, EventArgs e)
        {
            ResetValues();
            GridViewTimHDN.DataSource = null;
        }
        private void ResetValues()
        {
            foreach (Control Ctl in this.Controls)
                if (Ctl is TextBox)
                    Ctl.Text = "";
            txtMaHDN.Focus();
        }
        private void btnTimkiem_Click(object sender, EventArgs e)
        {
            DAO.OpenConnection();
            string sql;
            if ((txtMaHDN.Text == "") && (txtThang.Text == "") && (txtNam.Text == "") &&
               (txtMaNV.Text == "") && (txtMaNCC.Text == "") && (txtNgay.Text == "") &&
               (txtTongtien.Text == ""))
            {
                MessageBox.Show("Hãy nhập một điều kiện tìm kiếm!!!", "Yêu cầu ...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            sql = "SELECT * FROM HDN WHERE 1=1";
            if (txtMaHDN.Text != "")
                sql = sql + " AND MaHDN Like N'%" + txtMaHDN.Text + "%'";
            if (txtNgay.Text != "")
                sql = sql + " AND DAY(Ngaynhap) =" + txtNgay.Text;
            if (txtThang.Text != "")
                sql = sql + " AND MONTH(Ngaynhap) =" + txtThang.Text;
            if (txtNam.Text != "")
                sql = sql + " AND YEAR(Ngaynhap) =" + txtNam.Text;
            if (txtMaNV.Text != "")
                sql = sql + " AND MaNV Like N'%" + txtMaNV.Text + "%'";
            if (txtMaNCC.Text != "")
                sql = sql + " AND MaNCC Like N'%" + txtMaNCC.Text + "%'";
            if (txtTongtien.Text != "")
                sql = sql + " AND Tongtien <=" + txtTongtien.Text;
            tblHDN = DAO.GetDataToTable(sql);
            if (tblHDN.Rows.Count == 0)
            {
                MessageBox.Show("Không có bản ghi thỏa mãn điều kiện!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Có " + tblHDN.Rows.Count + " bản ghi thỏa mãn điều kiện!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GridViewTimHDN.DataSource = tblHDN;
            LoadDataGridView();

        }

        private void LoadDataGridView()
        {
            GridViewTimHDN.Columns[0].HeaderText = "Mã HĐN";
            GridViewTimHDN.Columns[1].HeaderText = "Mã nhân viên";
            GridViewTimHDN.Columns[2].HeaderText = "Ngày bán";
            GridViewTimHDN.Columns[3].HeaderText = "Mã NCC";
            GridViewTimHDN.Columns[4].HeaderText = "Tổng tiền";
            GridViewTimHDN.Columns[0].Width = 150;
            GridViewTimHDN.Columns[1].Width = 100;
            GridViewTimHDN.Columns[2].Width = 80;
            GridViewTimHDN.Columns[3].Width = 80;
            GridViewTimHDN.Columns[4].Width = 80;
            GridViewTimHDN.AllowUserToAddRows = false;
            GridViewTimHDN.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void btnTimlai_Click(object sender, EventArgs e)
        {
            ResetValues();
            GridViewTimHDN.DataSource = null;
        }

        private void txtTongtien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar >= '0') && (e.KeyChar <= '9')) || (Convert.ToInt32(e.KeyChar) == 8))
                e.Handled = false;
            else
                e.Handled = true;

        }
        private void FrmTimHDN_DoubleClick(object sender, EventArgs e)
        {
            string mahd;
            if (MessageBox.Show("Bạn có muốn hiển thị thông tin chi tiết?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                mahd = GridViewTimHDN.CurrentRow.Cells["MaHDN"].Value.ToString();
                FrmHDN frm = new FrmHDN();
                frm.txtMaHDN.Text = mahd;
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();
            }

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
