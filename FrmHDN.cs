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
using COMExcel = Microsoft.Office.Interop.Excel;

namespace Quản_lý_bán_hàng
{
    public partial class FrmHDN : Form
    {
        DataTable tblHDN;
        public FrmHDN()
        {
            InitializeComponent();
        }

        private void FrmHDN_Load(object sender, EventArgs e)
        {
            DAO.OpenConnection();
            btnThem.Enabled = true;
            btnLuu.Enabled = false;
            btnHuy.Enabled = false;
            btnIn.Enabled = false;
            txtMaHDN.ReadOnly = true;
            txtTenNV.ReadOnly = true;
            txtTenNCC.ReadOnly = true;
            txtDiachi.ReadOnly = true;
            txtSDT.ReadOnly = true;
            txtTenSP.ReadOnly = true;
            txtDongia.ReadOnly = true;
            txtThanhtien.ReadOnly = true;
            txtTongtien.ReadOnly = true;
            txtChietkhau.Text = "0";
            txtTongtien.Text = "0";
            DAO.FillDataToCombo("Select MaNV,TenNV from Nhanvien", cmbMaNV, "MaNV", "TenNV");
            cmbMaNV.SelectedIndex = -1;
            DAO.FillDataToCombo("Select MaNCC,TenNCC from NhaCC", cmbMaNCC, "MaNCC", "TenNCC");
            cmbMaNCC.SelectedIndex = -1;
            DAO.FillDataToCombo("Select a.MaSP_Size,b.TenSP from SanPham_Size as a join Sanpham as b on a.MaSP=b.MaSP ", cmbMaSP_Size, "MaSP_Size","TenSP");
            cmbMaSP_Size.SelectedIndex = -1;
            DAO.FillDataToCombo("Select MaHDN from HDN", cmbMaNCC, "MaHDN", "MaHDN");
            cmbMaHDN.SelectedIndex = -1;
            if (txtMaHDN.Text != "")
            {
                LoadInfoHoadon();
                btnHuy.Enabled = true;
                btnIn.Enabled = true;
            }
            LoadDataGridView();
        }
        private void LoadDataGridView()
        {
                string sql = "select a.MaSP_Size,c.TenSP,a.Soluong,a.Dongia,a.Thanhtien,a.Chietkhau,b.MaSize from ChitietHDN as a " +
                    "inner join Sanpham_Size as b on a.MaSP_Size = b.MaSP_Size " +
                    "inner join Sanpham as c on b.MaSP = c.MaSP where a.MaHDN = N'" + txtMaHDN.Text + "'";
                tblHDN = DAO.GetDataToTable(sql);
                GridViewHDN.DataSource = tblHDN;
        }
        private void LoadInfoHoadon()
        {
            DAO.OpenConnection();
            string str;
            str = "SELECT Ngaynhap FROM HDN WHERE MaHDN = N'" + txtMaHDN.Text + "'";
            txtNgaynhap.Text = DAO.ConvertDateTime(DAO.GetFieldValues(str));
            str = "SELECT MaNV FROM HDN WHERE MaHDN = N'" + txtMaHDN.Text + "'";
            cmbMaNV.Text = DAO.GetFieldValues(str);
            str = "SELECT MaNCC FROM NhaCC WHERE MaHDN = N'" + txtMaHDN.Text + "'";
            cmbMaNCC.Text = DAO.GetFieldValues(str);
            str = "SELECT Tongtien FROM HDN WHERE MaHDN = N'" + txtMaHDN.Text + "'";
            txtTongtien.Text = DAO.GetFieldValues(str);
            lblBangchu.Text = "Bằng chữ: " + DAO.ChuyenSoSangChu(txtTongtien.Text);
        }


        private void ResetValues()
        {
            txtMaHDN.Text = "";
            txtNgaynhap.Text = DateTime.Now.ToShortDateString();
            cmbMaNV.Text = "";
            cmbMaNCC.Text = "";
            txtTongtien.Text = "0";
            lblBangchu.Text = "Bằng chữ: ";
            cmbMaSP_Size.Text = "";
            txtSoluong.Text = "";
            txtChietkhau.Text = "0";
            txtThanhtien.Text = "0";
            cmbMaSize.Text = "";
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            btnIn.Enabled = false;
            btnHuy.Enabled = false;
            btnBoqua.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = false;
            ResetValues();
            txtMaHDN.Text = DAO.CreateKey("HDN");
            LoadDataGridView();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string sql;
            double sl, SLcon, tong, Tongmoi;
            sql = "SELECT MaHDN FROM HDN WHERE MaHDN=N'" + txtMaHDN.Text + "'";
            if (!DAO.CheckKeyExit(sql))
            {
                // Mã hóa đơn chưa có, tiến hành lưu các thông tin chung
                // Mã HDBan được sinh tự động do đó không có trường hợp trùng khóa
                if (txtNgaynhap.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập ngày nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNgaynhap.Focus();
                    return;
                }
                if (cmbMaNV.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmbMaNV.Focus();
                    return;
                }
                if (cmbMaNCC.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmbMaNCC.Focus();
                    return;
                }
                sql = "INSERT INTO HDN(MaHDN, Ngaynhap, MaNV, MaNCC, Tongtien) VALUES (N'" + txtMaHDN.Text.Trim() + "','" +
                        DAO.ConvertDateTime(txtNgaynhap.Text.Trim()) + "',N'" + cmbMaNV.SelectedValue + "',N'" +
                        cmbMaNCC.SelectedValue + "'," + txtTongtien.Text + ")";
                DAO.RunSQL(sql);
            }
            // Lưu thông tin của các mặt hàng
            if (cmbMaSP_Size.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập sản phẩm & size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbMaSP_Size.Focus();
                return;
            }
            if ((txtSoluong.Text.Trim().Length == 0) || (txtSoluong.Text == "0"))
            {
                MessageBox.Show("Bạn phải nhập số lượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoluong.Text = "";
                txtSoluong.Focus();
                return;
            }
            if (txtChietkhau.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập chiết khấu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtChietkhau.Focus();
                return;
            }
            if (cmbMaSize.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbMaSize.Focus();
                return;
            }
            sql = "SELECT MaSP_Size FROM ChitietHDN WHERE MaSP_Size=N'" + cmbMaSP_Size.SelectedValue + "' AND MaHDN = N'" + txtMaHDN.Text.Trim() + "'";
            if (DAO.CheckKeyExit(sql))
            {
                MessageBox.Show("Mã hàng này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetValuesHang();
                cmbMaSP_Size.Focus();
                return;
            }
            // Kiểm tra xem số lượng hàng trong kho còn đủ để cung cấp không?
            sl = Convert.ToDouble(DAO.GetFieldValues("SELECT Soluong FROM Sanpham_Size WHERE MaSP_Size = N'" + cmbMaSP_Size.SelectedValue + "'"));
            if (Convert.ToDouble(txtSoluong.Text) > sl)
            {
                MessageBox.Show("Số lượng mặt hàng này chỉ còn " + sl, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoluong.Text = "";
                txtSoluong.Focus();
                return;
            }
            sql = "INSERT INTO ChitietHDBN(MaHDN,MaSP_Size,Soluong,Dongia,Chietkhau,Thanhtien) VALUES(N'" + txtMaHDN.Text.Trim() + "',N'" + cmbMaSP_Size.SelectedValue + "'," + txtSoluong.Text + "," + txtDongia.Text + "," + txtChietkhau.Text + "," + txtThanhtien.Text + ")";
            DAO.RunSQL(sql);
            LoadDataGridView();
            // Cập nhật lại số lượng của mặt hàng vào bảng tblHang
            SLcon = sl - Convert.ToDouble(txtSoluong.Text);
            sql = "UPDATE Sanpham_Size SET Soluong =" + SLcon + " WHERE MaSP_Size= N'" + cmbMaSP_Size.SelectedValue + "'";
            DAO.RunSQL(sql);
            // Cập nhật lại tổng tiền cho hóa đơn bán
            tong = Convert.ToDouble(DAO.GetFieldValues("SELECT Tongtien FROM HDN WHERE MaHDN = N'" + txtMaHDN.Text + "'"));
            Tongmoi = tong + Convert.ToDouble(txtThanhtien.Text);
            sql = "UPDATE HDN SET Tongtien =" + Tongmoi + " WHERE MaHDN = N'" + txtMaHDN.Text + "'";
            DAO.RunSQL(sql);
            txtTongtien.Text = Tongmoi.ToString();
            lblBangchu.Text = "Bằng chữ: " + DAO.ChuyenSoSangChu(Tongmoi.ToString());
            ResetValuesHang();
            btnHuy.Enabled = true;
            btnThem.Enabled = true;
            btnIn.Enabled = true;
        }
        private void ResetValuesHang()
        {
            cmbMaSP_Size.Text = "";
            txtSoluong.Text = "";
            txtChietkhau.Text = "0";
            txtThanhtien.Text = "0";
            cmbMaSize.Text = "";
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            double sl, slcon, slhuy;
            if (MessageBox.Show("Bạn có chắc chắn muốn hủy không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sql = "SELECT MaS_Size,Soluong FROM ChitietHDN WHERE MaHDN = N'" + txtMaHDN.Text + "'";
                DataTable tblHang = DAO.GetDataToTable(sql);
                for (int hang = 0; hang <= tblHang.Rows.Count - 1; hang++)
                {
                    // Cập nhật lại số lượng cho các mặt hàng
                    sl = Convert.ToDouble(DAO.GetFieldValues("SELECT Soluong FROM Sanpham_Size WHERE MaSP_Size = N'" + tblHang.Rows[hang][0].ToString() + "'"));
                    slhuy = Convert.ToDouble(tblHang.Rows[hang][1].ToString());
                    slcon = sl + slhuy;
                    sql = "UPDATE Sanpham_Size SET Soluong =" + slcon + " WHERE MaSP_Size= N'" + tblHang.Rows[hang][0].ToString() + "'";
                    DAO.RunSQL(sql);
                }

                //Xóa chi tiết hóa đơn
                sql = "DELETE ChitietHDN WHERE MaHDN=N'" + txtMaHDN.Text + "'";
                DAO.RunSqlDel(sql);

                //Xóa hóa đơn
                sql = "DELETE HDN WHERE MaHDN=N'" + txtMaHDN.Text + "'";
                DAO.RunSqlDel(sql);
                ResetValues();
                LoadDataGridView();
                btnHuy.Enabled = false;
                btnIn.Enabled = false;
            }
        }


        private void cmbMaNV_TextChanged(object sender, EventArgs e)
        {
            string str;
            if (cmbMaNV.Text == "")
                txtTenNV.Text = "";
            // Khi chọn Mã nhân viên thì tên nhân viên tự động hiện ra
            str = "Select TenNV from Nhanvien where MaNV =N'" + cmbMaNV.SelectedValue + "'";
            txtTenNV.Text = DAO.GetFieldValues(str);
        }
        private void cmbMaSP_Size_TextChanged(object sender, EventArgs e)
        {
            string str;
            if (cmbMaSP_Size.Text == "")
            {
                txtTenSP.Text = "";
                txtDongia.Text = "";
            }
            // Khi chọn mã hàng thì các thông tin về hàng hiện ra
            str = "SELECT TenSP FROM Sanpham as a inner join Sanpham_Size as b on a.MaSP = b.MaSP WHERE MaSP_Size =N'" + cmbMaSP_Size.SelectedValue + "'";
            txtTenSP.Text = DAO.GetFieldValues(str);
            str = "SELECT Dongia FROM ChitietHDN WHERE MaSP_Size =N'" + cmbMaSP_Size.SelectedValue + "'";
            txtDongia.Text = DAO.GetFieldValues(str);
            str = "SELECT MaSize FROM Sanpham_Size WHERE MaSP_Size =N'" + cmbMaSP_Size.SelectedValue + "'";
            cmbMaSize.Text = DAO.GetFieldValues(str);
        }
        private void cmbMaNCC_TextChanged(object sender, EventArgs e)
        {
            string str;
            if (cmbMaNCC.Text == "")
            {
                txtTenNCC.Text = "";
                txtDiachi.Text = "";
                txtSDT.Text = "";
            }
            str = "Select TenNCC from NhaCC where MaNCC = N'" + cmbMaNCC.SelectedValue + "'";
            txtTenNCC.Text = DAO.GetFieldValues(str);
            str = "Select Diachi from NhaCC where MaNCC = N'" + cmbMaNCC.SelectedValue + "'";
            txtDiachi.Text = DAO.GetFieldValues(str);
            str = "Select SDT from NhaCC where MaNCC= N'" + cmbMaNCC.SelectedValue + "'";
            txtSDT.Text = DAO.GetFieldValues(str);
        }

        private void txtSoluong_TextChanged(object sender, EventArgs e)
        {
            //Khi thay đổi số lượng thì thực hiện tính lại thành tiền
            double tt, sl, dg, ck;
            if (txtSoluong.Text == "")
                sl = 0;
            else
                sl = Convert.ToDouble(txtSoluong.Text);
            if (txtChietkhau.Text == "")
                ck = 0;
            else
                ck = Convert.ToDouble(txtChietkhau.Text);
            if (txtDongia.Text == "")
                dg = 0;
            else
                dg = Convert.ToDouble(txtDongia.Text);
            tt = sl * dg - sl * dg * ck / 100;
            txtThanhtien.Text = tt.ToString();
        }

        private void txtChietkhau_TextChanged(object sender, EventArgs e)
        {
            //Khi thay đổi chiết khấu thì tính lại thành tiền
            double tt, sl, dg, ck;
            if (txtSoluong.Text == "")
                sl = 0;
            else
                sl = Convert.ToDouble(txtSoluong.Text);
            if (txtChietkhau.Text == "")
                ck = 0;
            else
                ck = Convert.ToDouble(txtChietkhau.Text);
            if (txtDongia.Text == "")
                dg = 0;
            else
                dg = Convert.ToDouble(txtDongia.Text);
            tt = sl * dg - sl * dg* ck / 100;
            txtThanhtien.Text = tt.ToString();
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            // Khởi động chương trình Excel
            COMExcel.Application exApp = new COMExcel.Application();
            COMExcel.Workbook exBook; //Trong 1 chương trình Excel có nhiều Workbook
            COMExcel.Worksheet exSheet; //Trong 1 Workbook có nhiều Worksheet
            COMExcel.Range exRange;
            string sql;
            int hang = 0, cot = 0;
            DataTable tblThongtinHD, tblThongtinSP;
            exBook = exApp.Workbooks.Add(COMExcel.XlWBATemplate.xlWBATWorksheet);
            exSheet = exBook.Worksheets[1];
            // Định dạng chung
            exRange = exSheet.Cells[1, 1];
            exRange.Range["A1:Z300"].Font.Name = "Times new roman"; //Font chữ
            exRange.Range["A1:B3"].Font.Size = 10;
            exRange.Range["A1:B3"].Font.Bold = true;
            exRange.Range["A1:B3"].Font.ColorIndex = 5; //Màu xanh da trời
            exRange.Range["A1:A1"].ColumnWidth = 7;
            exRange.Range["B1:B1"].ColumnWidth = 15;
            exRange.Range["A1:B1"].MergeCells = true;
            exRange.Range["A1:B1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A1:B1"].Value = "Shop Polite Art";
            exRange.Range["A2:B2"].MergeCells = true;
            exRange.Range["A2:B2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A2:B2"].Value = "Thanh Xuân - Hà Nội";
            exRange.Range["A3:B3"].MergeCells = true;
            exRange.Range["A3:B3"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A3:B3"].Value = "Điện thoại: 0868101222";
            exRange.Range["C2:E2"].Font.Size = 16;
            exRange.Range["C2:E2"].Font.Bold = true;
            exRange.Range["C2:E2"].Font.ColorIndex = 3; //Màu đỏ
            exRange.Range["C2:E2"].MergeCells = true;
            exRange.Range["C2:E2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["C2:E2"].Value = "HÓA ĐƠN NHẬP";
            // Biểu diễn thông tin chung của hóa đơn bán
            sql = "SELECT a.MaHDN, a.Ngaynhap, a.Tongtien, b.TenNCC, b.Diachi, b.SDT, c.TenNV FROM HDN AS a, NhaCC AS b, Nhanvien AS c WHERE a.MaHDN = N'" + txtMaHDN.Text + "' AND a.MaNCC = b.MaNCC AND a.MaNV = c.MaNV";
            tblThongtinHD = DAO.GetDataToTable(sql);
            exRange.Range["B6:C9"].Font.Size = 12;
            exRange.Range["B6:B6"].Value = "Mã hóa đơn:";
            exRange.Range["C6:E6"].MergeCells = true;
            exRange.Range["C6:E6"].Value = tblThongtinHD.Rows[0][0].ToString();
            exRange.Range["B7:B7"].Value = "Nhà cung cấp:";
            exRange.Range["C7:E7"].MergeCells = true;
            exRange.Range["C7:E7"].Value = tblThongtinHD.Rows[0][3].ToString();
            exRange.Range["B8:B8"].Value = "Địa chỉ:";
            exRange.Range["C8:E8"].MergeCells = true;
            exRange.Range["C8:E8"].Value = tblThongtinHD.Rows[0][4].ToString();
            exRange.Range["B9:B9"].Value = "Điện thoại:";
            exRange.Range["C9:E9"].MergeCells = true;
            exRange.Range["C9:E9"].Value = tblThongtinHD.Rows[0][5].ToString();
            //Lấy thông tin các mặt hàng
            sql = "SELECT b.TenSP, a.Soluong, b.Dongia, a.Chietkhau, a.Thanhtien " +
                  "FROM ChitietHDN AS a , Sanpham AS b, Sanpham_Size as c WHERE a.MaHDN = N'" +
                  txtMaHDN.Text + "' AND a.MaSP_Size = b.MaSP_Size AND b.MaSP = c.MaSP";
            SqlDataAdapter Myadapter = new SqlDataAdapter(sql, DAO.conn);
            tblThongtinSP = DAO.GetDataToTable(sql);
            //Tạo dòng tiêu đề bảng
            exRange.Range["A11:F11"].Font.Bold = true;
            exRange.Range["A11:F11"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["C11:F11"].ColumnWidth = 12;
            exRange.Range["A11:A11"].Value = "STT";
            exRange.Range["B11:B11"].Value = "Tên hàng";
            exRange.Range["C11:C11"].Value = "Số lượng";
            exRange.Range["D11:D11"].Value = "Đơn giá";
            exRange.Range["E11:E11"].Value = "Size";
            exRange.Range["F11:F11"].Value = "Giảm giá";
            exRange.Range["G11:G11"].Value = "Thành tiền";
            for (hang = 0; hang < tblThongtinSP.Rows.Count; hang++)
            {
                //Điền số thứ tự vào cột 1 từ dòng 12
                exSheet.Cells[1][hang + 12] = hang + 1;
                for (cot = 0; cot < tblThongtinSP.Columns.Count; cot++)
                //Điền thông tin hàng từ cột thứ 2, dòng 12
                {
                    exSheet.Cells[cot + 2][hang + 12] = tblThongtinSP.Rows[hang][cot].ToString();
                    if (cot == 3) exSheet.Cells[cot + 2][hang + 12] = tblThongtinSP.Rows[hang][cot].ToString() + "%";
                }
            }
            exRange = exSheet.Cells[cot][hang + 14];
            exRange.Font.Bold = true;
            exRange.Value2 = "Tổng tiền:";
            exRange = exSheet.Cells[cot + 1][hang + 14];
            exRange.Font.Bold = true;
            exRange.Value2 = tblThongtinHD.Rows[0][2].ToString();
            exRange = exSheet.Cells[1][hang + 15]; //Ô A1 
            exRange.Range["A1:F1"].MergeCells = true;
            exRange.Range["A1:F1"].Font.Bold = true;
            exRange.Range["A1:F1"].Font.Italic = true;
            exRange.Range["A1:F1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignRight;
            exRange.Range["A1:F1"].Value = "Bằng chữ: " + DAO.ChuyenSoSangChu(tblThongtinHD.Rows[0][2].ToString());
            exRange = exSheet.Cells[4][hang + 17]; //Ô A1 
            exRange.Range["A1:C1"].MergeCells = true;
            exRange.Range["A1:C1"].Font.Italic = true;
            exRange.Range["A1:C1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            DateTime d = Convert.ToDateTime(tblThongtinHD.Rows[0][1]);
            exRange.Range["A1:C1"].Value = "Hà Nội, ngày " + d.Day + " tháng " + d.Month + " năm " + d.Year;
            exRange.Range["A2:C2"].MergeCells = true;
            exRange.Range["A2:C2"].Font.Italic = true;
            exRange.Range["A2:C2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A2:C2"].Value = "Nhân viên bán hàng";
            exRange.Range["A6:C6"].MergeCells = true;
            exRange.Range["A6:C6"].Font.Italic = true;
            exRange.Range["A6:C6"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A6:C6"].Value = tblThongtinHD.Rows[0][6];
            exSheet.Name = "Hóa đơn nhập";
            exApp.Visible = true;
        }

        private void btnTimkiem_Click(object sender, EventArgs e)
        {
            if (cmbMaHDN.Text == "")
            {
                MessageBox.Show("Bạn phải chọn một mã hóa đơn để tìm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbMaHDN.Focus();
                return;
            }
            txtMaHDN.Text = cmbMaHDN.Text;
            LoadInfoHoadon();
            LoadDataGridView();
            btnHuy.Enabled = true;
            btnLuu.Enabled = true;
            btnIn.Enabled = true;
            cmbMaHDN.SelectedIndex = -1;
        }

        private void txtSoluong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar >= '0') && (e.KeyChar <= '9')) || (Convert.ToInt32(e.KeyChar) == 8))
                e.Handled = false;
            else e.Handled = true;
        }

        private void txtChietkhau_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar >= '0') && (e.KeyChar <= '9')) || (Convert.ToInt32(e.KeyChar) == 8))
                e.Handled = false;
            else e.Handled = true;
        }

        private void cmbMaHDN_DropDown(object sender, EventArgs e)
        {
            DAO.FillDataToCombo("SELECT MaHDN FROM HDN", cmbMaHDN, "MaHDN", "MaHDN");
            cmbMaHDN.SelectedIndex = -1;
        }
        private void FrmHDN_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResetValues();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBoqua_Click(object sender, EventArgs e)
        {
            ResetValues();
            ResetValuesHang();
            btnHuy.Enabled = true;
            btnIn.Enabled = false;
            btnThem.Enabled = true;
            btnBoqua.Enabled = false;
            btnLuu.Enabled = false;
            txtMaHDN.Enabled = false;
        }

        private void GridViewHDN_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            cmbMaSP_Size.Text = GridViewHDN.CurrentRow.Cells["MaSP_Size"].Value.ToString();
            txtTenSP.Text = GridViewHDN.CurrentRow.Cells["TenSP"].Value.ToString();
            txtSoluong.Text = GridViewHDN.CurrentRow.Cells["Soluong"].Value.ToString();
            txtChietkhau.Text = GridViewHDN.CurrentRow.Cells["Chietkhau"].Value.ToString();
            txtDongia.Text = GridViewHDN.CurrentRow.Cells["Dongia"].Value.ToString();
            txtThanhtien.Text = GridViewHDN.CurrentRow.Cells["Thanhtien"].Value.ToString();
            cmbMaSize.Text = GridViewHDN.CurrentRow.Cells["MaSize"].Value.ToString();
            btnThoat.Enabled = true;
            btnHuy.Enabled = true;
            btnBoqua.Enabled = true;
          
        }
    }
}
