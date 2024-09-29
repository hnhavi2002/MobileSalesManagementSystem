using DevExpress.XtraEditors.Mask.Design;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace MINHTUANMOBILE
{
    public partial class FrmChiTietDonHang : DevExpress.XtraEditors.XtraForm
    {
        string maCTDH;
        private string maDonHang;
        public string MaDonHang
        {
            get { return maDonHang; }
            set { maDonHang = value; }
        }
        public FrmChiTietDonHang()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
            this.FormClosing += FormCTDH_FormClosing;
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        private void CTDH_Load()
        {
            string str = "select * from ChiTietDonHang";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdCTDH.DataSource = dt;
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

                // Nếu cần gán cả giá trị mã sản phẩm vào ComboBox, có thể sử dụng Tag
                cboMaSP.Tag = dt;
                cboMaSP.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void KM_Load()
        {
            try
            {
                lenh.CommandText = "SELECT MaKM, TenKM FROM KhuyenMai";
                lenh.Parameters.Clear();

                da.SelectCommand = lenh;
                DataTable dt = new DataTable("KhuyenMai");
                da.Fill(dt);

                cboMaKM.DataSource = dt;
                cboMaKM.ValueMember = "MaKM";
                cboMaKM.DisplayMember = "TenKM";

                cboMaKM.Tag = dt;
                cboMaKM.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FrmChiTietDonHang_Load(object sender, EventArgs e)
        {
            SP_Load();
            KM_Load();
            SetGridViewReadOnly(gridView1);
            ResetValues_CTDH();
            LoadData_CTDH();
            bdsource.DataSource = data.ChiTietDonHang_Info(maDonHang);
            // Gán dữ liệu vào gridControl
            grdCTDH.DataSource = bdsource;
        }

        private void LoadData_CTDH()
        {
            DataTable tblCTDH; // table để chứa dữ liệu truy vấn
            // set text cho mã đơn hàng
            txtMaDH.Text = MaDonHang;

            // set text maCN, MaKH
            string sql = "SELECT MaKH, MaCN " +
                "FROM DonHang " +
                "WHERE MaDH = '" + MaDonHang + "' ";
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
                string maKH = tblCTDH.Rows[0].Field<string>("MaKH");
                string queryKhachHang = "SELECT TenKH FROM KhachHang WHERE MaKH = '" + maKH + "'";
                DataTable tblKhachHang = data.excuteQuery(queryKhachHang);

                if (tblKhachHang.Rows.Count > 0)
                {
                    // Hiển thị tên nhà cung cấp
                    txtKH.Text = tblKhachHang.Rows[0].Field<string>("TenKH");
                }
            }
        }

        //Tạo mã tự động
        private string MaCTDHTuDong()
        {
            // Tạo mã chi tiết nhập theo định dạng "CTPNyyyyMMdd"
            string maCTDH = "CTDH" + DateTime.Now.ToString("yyyyMMdd");

            // Kiểm tra xem mã chi tiết HĐ đã tồn tại chưa
            int stt = LaySoThuTuTangDan(maCTDH);

            // Thêm số thứ tự vào mã chi tiết nhập
            maCTDH += stt.ToString("D2");
            return maCTDH;
        }

        private int LaySoThuTuTangDan(string maCTDHang)
        {
            // Kiểm tra xem có chi tiết phiếu nhập nào chưa
            string query = "SELECT MAX(CAST(SUBSTRING(MaCTDH, 13, 2) AS INT)) FROM ChiTietDonHang WHERE MaCTDH LIKE '" + maCTDHang + "%'";

            object result = data.ExecuteScalar(query);

            // Nếu chưa có chi tiết phiếu nhập nào, trả về 1
            if (result == DBNull.Value)
            {
                return 1;
            }

            // Nếu có chi tiết phiếu nhập rồi, trả về số thứ tự tăng dần tiếp theo
            return Convert.ToInt32(result) + 1;
        }
        private void ResetValues_CTDH()
        {
            cboMaSP.SelectedValue = "";
            txtMau.Text = "";
            txtHDH.Text = "";
            txtDL.Text = "";
            cboMaKM.SelectedValue = "";
            txtDiscount.Text = "";
            txtSoLuong.Text = "";
            txtThanhTien.Text = "";
            txtGiaBan.Text = "";

        }

        private void SetGridViewReadOnly(GridView gridView1)
        {
            gridView1.OptionsBehavior.Editable = false;
         
            gridView1.CustomRowCellEdit += (sender, e) =>
            {
                e.RepositoryItem.ReadOnly = true; // Chặn chỉnh sửa cột này
            };
        }
        private void FormCTDH_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Kiểm tra điều kiện trước khi đóng Form Chi tiết nhập kho
            if (MessageBox.Show("Bạn Có Chắc Chắn Muốn Thoát?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                FrmDonHang formDonHang = new FrmDonHang();
                formDonHang.Show();
            }
            else
            {
                // Ngăn chặn việc đóng Form Chi tiết nhập kho nếu người dùng chọn "No"
                e.Cancel = true;
            }
        }

        private void cboMaSP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaSP.SelectedIndex != -1)
            {
                string maSP = cboMaSP.SelectedValue.ToString();
                // Lấy thông tin màu từ bảng MauSac
                string queryMau = "SELECT TenMau FROM MauSac WHERE MaMau = (SELECT MaMau FROM SanPham WHERE MaSP = '" + maSP + "')";
                string tenMau = ConnectDB.GetFieldValues(queryMau);
                txtMau.Text = tenMau;

                // Lấy thông tin dung lượng từ bảng DL
                string queryDL = "SELECT LoaiDL FROM DungLuong WHERE MaDL = (SELECT MaDL FROM SanPham WHERE MaSP = '" + maSP + "')";
                string tenDL = ConnectDB.GetFieldValues(queryDL);
                txtDL.Text = tenDL;

                // Lấy thông tin hệ điều hành từ bảng HĐH
                string queryHDH = "SELECT TenHDH FROM HeDieuHanh WHERE MaHDH = (SELECT MaHDH FROM SanPham WHERE MaSP = '" + maSP + "')";
                string tenHDH = ConnectDB.GetFieldValues(queryHDH);
                txtHDH.Text = tenHDH;

                // Lấy thông tin bảo hành từ bảng SanPham
                string queryBH = "SELECT BaoHanh FROM SanPham WHERE MaSP = '" + maSP + "'";
                string BH= ConnectDB.GetFieldValues(queryBH);
                txtBH.Text = BH;

                // Lấy thông tin giá bán từ bảng SanPham
                string queryGia = "SELECT GiaBan FROM SanPham WHERE MaSP = '" + maSP + "'";
                string Gia = ConnectDB.GetFieldValues(queryGia);
                txtGiaBan.Text = Gia;       
            }
        }
        private void cboMaKM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cboMaKM.SelectedIndex != -1)
            {
                string maKM = cboMaKM.SelectedValue.ToString();
                // Lấy thông tin giảm giá từ bảng KM
                string queryDiscount = "SELECT Discount FROM KhuyenMai WHERE MaKM = '" + maKM + "'";
                string KM = ConnectDB.GetFieldValues(queryDiscount);
                txtDiscount.Text = KM;
            }    
        }
        //Tính thành tiền
        private void TinhThanhTien()
        {
            double tt, sl, gianhap, gg;

            if (txtSoLuong.Text == "")
                sl = 0;
            else
                sl = Convert.ToDouble(txtSoLuong.Text);

            if (txtDiscount.Text == "")
                gg = 0;
            else
                gg = Convert.ToDouble(txtDiscount.Text);

            if (txtGiaBan.Text == "")
                gianhap = 0;
            else
                gianhap = Convert.ToDouble(txtGiaBan.Text);

            tt = sl * gianhap - sl * gianhap * gg / 100;
            txtThanhTien.Text = tt.ToString();
        }
        private void txtDiscount_EditValueChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }

        private void txtGiaBan_EditValueChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }

        private void txtSoLuong_EditValueChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }

        private void grdCTDH_Click(object sender, EventArgs e)
        {

        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                maCTDH = MaCTDHTuDong();
                if (cboMaSP.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Sản Phẩm !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // Lấy số lượng tồn hiện tại của sản phẩm
                int soLuongTonHienTai = LaySoLuongTonHienTai(cboMaSP.SelectedValue.ToString());
                // lấy số lượng nhập
                int soLuongBan = Convert.ToInt32(txtSoLuong.Text);

                string insert = @"INSERT INTO ChiTietDonHang (MaCTDH, MaDH, MaSP, GiaBan, SoLuongMua, MaKM, Discount, ThanhTien, NgayTao) 
                            VALUES (@MaCTDH, @MaDH, @MaSP, @GiaBan, @SoLuongMua, @MaKM, @Discount, @ThanhTien, @NgayTao)";

                lenh.CommandText = insert;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaCTDH", maCTDH);
                lenh.Parameters.AddWithValue("@MaDH", txtMaDH.Text);
                lenh.Parameters.AddWithValue("@MaSP", cboMaSP.SelectedValue);
                lenh.Parameters.AddWithValue("@GiaBan", txtGiaBan.Text);
                lenh.Parameters.AddWithValue("@SoLuongMua", txtSoLuong.Text);
                lenh.Parameters.AddWithValue("@MaKM", cboMaKM.SelectedValue);
                lenh.Parameters.AddWithValue("@Discount", txtDiscount.Text);
                lenh.Parameters.AddWithValue("@ThanhTien", txtThanhTien.Text);
                lenh.Parameters.AddWithValue("@NgayTao", DateTime.Now);
                data.ExCuteNonQuery2(lenh);

                // cập nhật tổng các thành tiền vào tổng tiền trong bảng nhập kho
                decimal tongtienDH = TinhTongTienDH(txtMaDH.Text);

                string updateTongTienQuery = "UPDATE DonHang SET TongTien = @TongTien WHERE MaDH = @MaDH";
                lenh.CommandText = updateTongTienQuery;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@TongTien", tongtienDH);
                lenh.Parameters.AddWithValue("@MaDH", txtMaDH.Text);

                data.ExCuteNonQuery2(lenh);

                // Cập nhật số lượng tồn mới cho sản phẩm
                CapNhatSoLuongTon(cboMaSP.SelectedValue.ToString(), soLuongTonHienTai - soLuongBan);

                MessageBox.Show("Thêm Chi Tiết Đơn Hàng Có Mã: " + txtMaDH.Text + " Thành Công !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CTDH_Load();
                bdsource.DataSource = data.ChiTietDonHang_Info(maDonHang);
                // Gán dữ liệu vào gridControl
                grdCTDH.DataSource = bdsource;
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

        private decimal TinhTongTienDH(string maDonHang)
        {
            decimal tongTien = 0;

            try
            {
                string query = "SELECT SUM(ThanhTien) FROM ChiTietDonHang WHERE MaDH = @MaDH";
                lenh.CommandText = query;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaDH", maDonHang);

                object result = data.ExecuteScalar(lenh);

                if (result != DBNull.Value)
                {
                    tongTien = Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }

            return tongTien;
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

        private void btnIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaDH.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Đơn Hàng Để In !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SqlConnection conn = new SqlConnection(@"Data Source=HAVI\HAVISQLEXPRESS;Initial Catalog=MinhTuanMobile_DB;Integrated Security=True");
                conn.Open();
                // Sử dụng parameterized query để tránh SQL Injection
                SqlCommand command = new SqlCommand("Select * from Report_DH where MaDH = '" + txtMaDH.Text + "'", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();

                DHReport report = new DHReport();
                report.DataSource = dt;
                report.ShowPreviewDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không sửa được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
    }
}
