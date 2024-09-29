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
    public partial class FrmChiTietNhapKho : DevExpress.XtraEditors.XtraForm
    {
        public bool was_modify_CTDH = false;
        public bool is_them_DH = false;
        string maCTPN;
        private string maPhieuNhap;
        public string MaPhieuNhap
        {
            get { return maPhieuNhap; }
            set { maPhieuNhap = value; }
        }
        public FrmChiTietNhapKho()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
            this.FormClosing += FormCTNhapKho_FormClosing;
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        private void CTNhap_Load()
        {
            string str = "select * from ChiTietNhapKho";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdCTNhap.DataSource = dt;
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
        private void FrmChiTietNhapKho_Load(object sender, EventArgs e)
        {
            SetGridViewReadOnly(gridView1);
            ResetValues_CTDH();
            LoadData_CTDH();
            SP_Load();
            bdsource.DataSource = data.ChiTietNhap_Info(maPhieuNhap);
            // Gán dữ liệu vào gridControl
            grdCTNhap.DataSource = bdsource;
        }

        private void TinhThanhTien()
        {
            double tt, sl, gianhap, gg;

            if (txtSL.Text == "")
                sl = 0;
            else
                sl = Convert.ToDouble(txtSL.Text);

            if (txtCK.Text == "")
                gg = 0;
            else
                gg = Convert.ToDouble(txtCK.Text);

            if (txtGiaNhap.Text == "")
                gianhap = 0;
            else
                gianhap = Convert.ToDouble(txtGiaNhap.Text);

            tt = sl * gianhap - sl * gianhap * gg / 100;
            txtThanhTien.Text = tt.ToString();
        }
        private void txtSL_EditValueChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }

        private void txtCK_EditValueChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }

        private void txtGiaNhap_EditValueChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }
        // tạo mã chi tiết phiếu nhập tự động
        private string TaoMaCTNKTuDong()
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


        private void cboMaSP_SelectedIndexChanged(object sender, EventArgs e)
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
        // ngăn ng dùng nhập vào gridview
        private void SetGridViewReadOnly(GridView gridView)
        {
            gridView.OptionsBehavior.Editable = false;
            // Nếu bạn muốn chặn chỉnh sửa từng dòng cụ thể, bạn có thể sử dụng sự kiện CustomRowCellEdit
            gridView.CustomRowCellEdit += (sender, e) =>
            {
                e.RepositoryItem.ReadOnly = true; // Chặn chỉnh sửa cột này
            };
        }
        private void ResetValues_CTDH() // reset giá trị cho các mục 
        {
            cboMaSP.Text = "";
            txtGiaNhap.Text = "";
            txtSL.Text = "";
            txtCK.Text = "";
            txtThanhTien.Text = "";
            btnCapNhat.Enabled = true;
            btnHuy.Enabled = true;
            txtMau.Text = "";
            txtDL.Text = "";
            txtBH.Text = "";
        }

        // tải dữ liệu chi nhánh, nhà cung cấp cho các TextBox
        private void LoadData_CTDH()
        {
            DataTable tblCTDH; // table để chứa dữ liệu truy vấn
            // set text cho mã đơn hàng
            txtSoPN.Text = MaPhieuNhap;

            // set text cho manv, makh, madt, soluong, giamgia, tongtien
            string sql = "SELECT MaCN, MaNCC " +
                "FROM NhapKho " +
                "WHERE PhieuNhap = '" + MaPhieuNhap + "' ";
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

        //xử lý nút cập nhật chi tiết nhập kho
        private void btnCapNhat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                maCTPN = TaoMaCTNKTuDong();
                if (cboMaSP.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Sản Phẩm !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lấy số lượng tồn hiện tại của sản phẩm
                int soLuongTonHienTai = LaySoLuongTonHienTai(cboMaSP.SelectedValue.ToString());
                // lấy số lượng nhập
                int soLuongNhap = Convert.ToInt32(txtSL.Text);

                string insert = @"INSERT INTO ChiTietNhapKho (MaCTPN, PhieuNhap, MaSP, GiaNhap, SoLuongNhap, ChietKhau, ThanhTien, TrangThai, NgayNhap) 
                            VALUES (@MaCTPN, @PhieuNhap, @MaSP, @GiaNhap, @SoLuongNhap, @ChietKhau, @ThanhTien, @TrangThai, @NgayNhap)";

                lenh.CommandText = insert;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaCTPN", maCTPN);
                lenh.Parameters.AddWithValue("@PhieuNhap", txtSoPN.Text);
                lenh.Parameters.AddWithValue("@MaSP", cboMaSP.SelectedValue);
                lenh.Parameters.AddWithValue("@GiaNhap", txtGiaNhap.Text);
                lenh.Parameters.AddWithValue("@SoLuongNhap", txtSL.Text);
                lenh.Parameters.AddWithValue("@ChietKhau", txtCK.Text);
                lenh.Parameters.AddWithValue("@ThanhTien", txtThanhTien.Text);
                lenh.Parameters.AddWithValue("@TrangThai", true);
                lenh.Parameters.AddWithValue("@NgayNhap", DateTime.Now);
                data.ExCuteNonQuery2(lenh);

                // cập nhật tổng các thành tiền vào tổng tiền trong bảng nhập kho
                decimal tongTienNhapKho = TinhTongTienNK(txtSoPN.Text);

                string updateTongTienQuery = "UPDATE NhapKho SET TongTien = @TongTien WHERE PhieuNhap = @PhieuNhap";
                lenh.CommandText = updateTongTienQuery;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@TongTien", tongTienNhapKho);
                lenh.Parameters.AddWithValue("@PhieuNhap", txtSoPN.Text);

                data.ExCuteNonQuery2(lenh);

                // Cập nhật số lượng tồn mới cho sản phẩm
                CapNhatSoLuongTon(cboMaSP.SelectedValue.ToString(), soLuongTonHienTai + soLuongNhap);

                MessageBox.Show("Thêm Chi Tiết Phiếu Nhập: " + txtSoPN.Text + " Thành Công !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CTNhap_Load();
                bdsource.DataSource = data.ChiTietNhap_Info(maPhieuNhap);
                // Gán dữ liệu vào gridControl
                grdCTNhap.DataSource = bdsource;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không lưu được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnHuy.Enabled = true;
        }
        // lấy số lượng tồn hiện tại trong bảng sản phẩm
        private int LaySoLuongTonHienTai(string maSP)
        {
            string query = "SELECT SLTon FROM SanPham WHERE MaSP = @MaSP";
            lenh.CommandText = query;
            lenh.Parameters.Clear();
            lenh.Parameters.AddWithValue("@MaSP", maSP);
            object result = data.ExecuteScalar(lenh);
            return result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }
        // cập nhật số lượng tồn vào bảng sản phẩm
        private void CapNhatSoLuongTon(string maSP, int soLuongMoi)
        {
            string query = "UPDATE SanPham SET SLTon = @SoLuongMoi WHERE MaSP = @MaSP";
            lenh.CommandText = query;
            lenh.Parameters.Clear();
            lenh.Parameters.AddWithValue("@SoLuongMoi", soLuongMoi);
            lenh.Parameters.AddWithValue("@MaSP", maSP);
            data.ExCuteNonQuery2(lenh);
        }

        // xử lý nút hủy
        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValues_CTDH();
        }

        // tính tổng các thành tiền
        private decimal TinhTongTienNK(string maPhieuNhap)
        {
            decimal tongTien = 0;

            try
            {
                string query = "SELECT SUM(ThanhTien) FROM ChiTietNhapKho WHERE PhieuNhap = @PhieuNhap AND TrangThai = 1";
                lenh.CommandText = query;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@PhieuNhap", maPhieuNhap);

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

        private void grdCTNhap_Click(object sender, EventArgs e)
        {
            //txtSoPN.DataBindings.Clear();
            //txtSoPN.DataBindings.Add("Text", grdCTNhap.DataSource, "Phiếu Nhập");
            cboMaSP.DataBindings.Clear();
            cboMaSP.DataBindings.Add("Text", grdCTNhap.DataSource, "Mã sản phẩm");
            txtGiaNhap.DataBindings.Clear();
            txtGiaNhap.DataBindings.Add("Text", grdCTNhap.DataSource, "Giá nhập");
            txtSL.DataBindings.Clear();
            txtSL.DataBindings.Add("Text", grdCTNhap.DataSource, "Số lượng nhập");
            txtCK.DataBindings.Clear();
            txtCK.DataBindings.Add("Text", grdCTNhap.DataSource, "chiết khấu");
            txtThanhTien.DataBindings.Clear();
            txtThanhTien.DataBindings.Add("Text", grdCTNhap.DataSource, "thành tiền");
        }
        // đóng form ctnk và trả về form nhập kho
        private void FormCTNhapKho_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Kiểm tra điều kiện trước khi đóng Form Chi tiết nhập kho
            if (MessageBox.Show("Bạn Có Chắc Chắn Muốn Thoát Khỏi Giao Diện Chi Tiết Nhập Kho?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                FrmNhapKho formNhapKho = new FrmNhapKho();
                formNhapKho.Show();
            }
            else
            {
                // Ngăn chặn việc đóng Form Chi tiết nhập kho nếu người dùng chọn "No"
                e.Cancel = true;
            }
        }
    }
}
