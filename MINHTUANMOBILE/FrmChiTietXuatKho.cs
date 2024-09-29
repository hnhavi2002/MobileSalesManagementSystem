using DevExpress.CodeParser;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask.Design;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Animation;

namespace MINHTUANMOBILE
{
    public partial class FrmChiTietXuatKho : DevExpress.XtraEditors.XtraForm
    {
        string maCTPX;
        private string maPhieuXuat;
        public string MaPhieuXuat
        {
            get { return maPhieuXuat; }
            set { maPhieuXuat = value; }
        }
        public FrmChiTietXuatKho()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
            this.FormClosing += FormCTNhapKho_FormClosing;
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        private void CTXuat_Load()
        {
            string str = "select * from ChiTietXuatKho";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdCTXK.DataSource = dt;
        }
        void SP_Load()
        {
            try
            {
                lenh.CommandText = "SELECT MaSP, TenSP FROM SanPham";
                lenh.Parameters.Clear();

                da.SelectCommand = lenh;
                DataTable dt = new DataTable("SanPham");
                da.Fill(dt);

                cboMaSP.DataSource = dt;
                cboMaSP.ValueMember = "MaSP";
                cboMaSP.DisplayMember = "TenSP";

                // Nếu bạn cần gán cả giá trị mã chi nhánh vào ComboBox, bạn có thể sử dụng Tag
                cboMaSP.Tag = dt;
                cboMaSP.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FrmChiTietXuatKho_Load(object sender, EventArgs e)
        {

            SetGridViewReadOnly(gridView1);
            ResetValues_CTDH();
            LoadData_CTDH();
            SP_Load();
            bdsource.DataSource = data.ChiTietXuat_Info(maPhieuXuat);
            // Gán dữ liệu vào gridControl
            grdCTXK.DataSource = bdsource;
        }

        private void LoadData_CTDH()
        {
            DataTable tblCTDH; // table để chứa dữ liệu truy vấn
            // set text cho mã đơn hàng
            txtPX.Text = MaPhieuXuat;

            // set text cho manv, makh, madt, soluong, giamgia, tongtien
            string sql = "SELECT MaCN, MaNCC " +
                "FROM XuatKho " +
                "WHERE PhieuXuat = '" + MaPhieuXuat + "' ";
            tblCTDH = data.excuteQuery(sql);

            if (tblCTDH.Rows.Count > 0)
            {
                // Lấy thông tin chi nhánh từ bảng ChiNhanh
                string maCN = tblCTDH.Rows[0].Field<string>("MaCN");
                string queryChiNhanh = "SELECT TenCN FROM ChiNhanh WHERE MaCN = '" + maCN + "'";
                DataTable tblChiNhanh = data.excuteQuery(queryChiNhanh);

                if (tblChiNhanh.Rows.Count > 0)
                {
                    // Hiển thị tên chi nhánh
                    txtCN.Text = tblChiNhanh.Rows[0].Field<string>("TenCN");
                }

                // Lấy thông tin nhà cung cấp từ bảng NhaCungCap
                string maNCC = tblCTDH.Rows[0].Field<string>("MaNCC");
                string queryNhaCungCap = "SELECT TenNCC FROM NhaCungCap WHERE MaNCC = '" + maNCC + "'";
                DataTable tblNhaCungCap = data.excuteQuery(queryNhaCungCap);

                if (tblNhaCungCap.Rows.Count > 0)
                {
                    // Hiển thị tên nhà cung cấp
                    txtNCC.Text = tblNhaCungCap.Rows[0].Field<string>("TenNCC");
                }
            }
        }

        private void ResetValues_CTDH()
        {
            cboMaSP.Text = "";
            txtSL.Text = "";
            btnCapNhat.Enabled = true;
            btnHuy.Enabled = true;
            txtMau.Text = "";
            txtDL.Text = "";
            txtBH.Text = "";
        }

        private void SetGridViewReadOnly(GridView gridView1)
        {
            gridView1.OptionsBehavior.Editable = false;
            // Nếu bạn muốn chặn chỉnh sửa từng dòng cụ thể, bạn có thể sử dụng sự kiện CustomRowCellEdit
            gridView1.CustomRowCellEdit += (sender, e) =>
            {
                e.RepositoryItem.ReadOnly = true; // Chặn chỉnh sửa cột này
            };
        }
        private string TaoMaCTXKTuDong()
        {
            // Tạo mã chi tiết nhập theo định dạng "CTPNyyyyMMdd"
            string maCTPN = "CTPN" + DateTime.Now.ToString("yyyyMMdd");

            // Kiểm tra xem mã chi tiết nhập đã tồn tại chưa
            int stt = LaySoThuTuTangDan(maCTPN);

            // Thêm số thứ tự vào mã chi tiết nhập
            maCTPN += stt.ToString("D2");
            return maCTPN;
        }
        private int LaySoThuTuTangDan(string maCTPN)
        {
            // Kiểm tra xem có chi tiết phiếu nhập nào chưa
            string query = "SELECT MAX(CAST(SUBSTRING(MaCTPN, 13, 2) AS INT)) FROM ChiTietNhapKho WHERE MaCTPN LIKE '" + maCTPN + "%'";

            object result = data.ExecuteScalar(query);

            // Nếu chưa có chi tiết phiếu nhập nào, trả về 1
            if (result == DBNull.Value)
            {
                return 1;
            }

            // Nếu có chi tiết phiếu nhập rồi, trả về số thứ tự tăng dần tiếp theo
            return Convert.ToInt32(result) + 1;
        }

        private void cboSP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaSP.SelectedIndex != -1)
            {
                // Lấy giá trị đã chọn từ ComboBox
                string selectedMaSP = cboMaSP.SelectedValue.ToString();

                // Lấy thông tin màu từ bảng MauSac
                string queryMau = "SELECT TenMau FROM MauSac WHERE MaMau = (SELECT MaMau FROM SanPham WHERE MaSP = '" + selectedMaSP + "')";
                string tenMau = ConnectDB.GetFieldValues(queryMau);
                txtMau.Text = tenMau;

                // Lấy thông tin dung lượng từ bảng DL
                string queryDL = "SELECT LoaiDL FROM DungLuong WHERE MaDL = (SELECT MaDL FROM SanPham WHERE MaSP = '" + selectedMaSP + "')";
                string tenDL = ConnectDB.GetFieldValues(queryDL);
                txtDL.Text = tenDL;

                // Lấy thông tin bảo hành từ bảng SanPham
                string queryBH = "SELECT BaoHanh FROM SanPham WHERE MaSP = '" + selectedMaSP + "'";
                string BH = ConnectDB.GetFieldValues(queryBH);
                txtBH.Text = BH;
            }


        }
        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValues_CTDH();
        }

        private void btnCapNhat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                maCTPX = TaoMaCTXKTuDong();
                if (cboMaSP.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Sản Phẩm !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (txtSL.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Số Lượng Xuất !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // Lấy số lượng tồn hiện tại của sản phẩm
                int soLuongTonHienTai = LaySoLuongTonHienTai(cboMaSP.SelectedValue.ToString());

                // Kiểm tra xem có đủ số lượng tồn không
                int soLuongXuat = Convert.ToInt32(txtSL.Text);
                if (soLuongXuat > soLuongTonHienTai)
                {
                    MessageBox.Show($"Số lượng tồn trong kho không đủ !!! Số lượng tồn hiện tại là {soLuongTonHienTai}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string insert = @"INSERT INTO ChiTietXuatKho (MaCTPX, PhieuXuat, MaSP, SoLuongXuat, NgayXuat, TrangThai) 
                            VALUES (@MaCTPX, @PhieuXuat, @MaSP, @SoLuongXuat,@NgayXuat, @TrangThai)";

                lenh.CommandText = insert;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaCTPX", maCTPX);
                lenh.Parameters.AddWithValue("@PhieuXuat", txtPX.Text);
                lenh.Parameters.AddWithValue("@MaSP", cboMaSP.SelectedValue);
                lenh.Parameters.AddWithValue("@SoLuongXuat", txtSL.Text);
                lenh.Parameters.AddWithValue("@NgayXuat", DateTime.Now);
                lenh.Parameters.AddWithValue("@TrangThai", true);
                data.ExCuteNonQuery2(lenh);

                // Cập nhật số lượng tồn mới cho sản phẩm
                CapNhatSoLuongTon(cboMaSP.SelectedValue.ToString(), soLuongTonHienTai - soLuongXuat);

                MessageBox.Show("Thêm Chi Tiết Phiếu Xuất: " + txtPX.Text + " Thành Công !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CTXuat_Load();
                bdsource.DataSource = data.ChiTietXuat_Info(maPhieuXuat);
                // Gán dữ liệu vào gridControl
                grdCTXK.DataSource = bdsource;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không lưu được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnHuy.Enabled = true;
        }

        private void CapNhatSoLuongTon(string maSP, int soLuongMoi)
        {
            string query = "UPDATE SanPham SET SLTon = @SoLuongMoi WHERE MaSP = @MaSP";
            lenh.CommandText = query;
            lenh.Parameters.Clear();
            lenh.Parameters.AddWithValue("@SoLuongMoi", soLuongMoi);
            lenh.Parameters.AddWithValue("@MaSP", maSP);
            data.ExCuteNonQuery2(lenh);
        }

        private int LaySoLuongTonHienTai(string maSP)
        {
            string query = "SELECT SLTon FROM SanPham WHERE MaSP = @MaSP";
            lenh.CommandText = query;
            lenh.Parameters.Clear();
            lenh.Parameters.AddWithValue("@MaSP", maSP);
            object result = data.ExecuteScalar(lenh);
            return result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }
        // đóng form ctnk và trả về form xuất kho
        private void FormCTNhapKho_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Kiểm tra điều kiện trước khi đóng Form Chi tiết xuất kho
            if (MessageBox.Show("Bạn Có Chắc Chắn Muốn Thoát Khỏi Giao Diện Chi Tiết Xuất Kho?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                FrmXuatKho formXuatKho = new FrmXuatKho();
                formXuatKho.Show();
            }
            else
            {
                // Ngăn chặn việc đóng Form Chi tiết nhập kho nếu người dùng chọn "No"
                e.Cancel = true;
            }
        }            
    }
}
