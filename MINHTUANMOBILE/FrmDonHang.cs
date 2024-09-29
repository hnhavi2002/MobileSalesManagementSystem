
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using iTextSharp.text.pdf;
using iTextSharp.text;
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
using System.Windows.Media.Animation;
using Document = iTextSharp.text.Document;
using DevExpress.XtraReports.UI;

namespace MINHTUANMOBILE
{
    public partial class FrmDonHang : DevExpress.XtraEditors.XtraForm
    {
        public FrmDonHang()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        string maDH;
        private void DH_Load()
        {
            string str = "select * from DonHang";
            SqlDataAdapter adapter = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grdDonHang.DataSource = dt;
        }

        private void FrmDonHang_Load(object sender, EventArgs e)
        {
            ChiNhanh_Load();
            KhachHang_Load();
            ThanhToan_Load();
            bdsource.DataSource = data.DonHang_Info();
            grdDonHang.DataSource = bdsource;
            SetGridViewReadOnly(gridViewDH);

        }
        // load chi nhánh
        void ChiNhanh_Load()
        {
            try
            {
                lenh.CommandText = "SELECT MaCN, TenCN FROM ChiNhanh";
                lenh.Parameters.Clear();

                da.SelectCommand = lenh;
                DataTable dt = new DataTable("ChiNhanh");
                da.Fill(dt);

                cboMaCN.DataSource = dt;
                cboMaCN.ValueMember = "MaCN";
                cboMaCN.DisplayMember = "TenCN";

                // Nếu bạn cần gán cả giá trị mã chi nhánh vào ComboBox, bạn có thể sử dụng Tag
                cboMaCN.Tag = dt;
                cboMaCN.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // load khách hàng
        void KhachHang_Load()
        {
            try
            {
                lenh.CommandText = "SELECT MaKH, TenKH FROM KhachHang";
                lenh.Parameters.Clear();

                da.SelectCommand = lenh;
                DataTable dt = new DataTable("KhachHang");
                da.Fill(dt);

                cboMaKH.DataSource = dt;
                cboMaKH.ValueMember = "MaKH";
                cboMaKH.DisplayMember = "TenKH";

                // Nếu bạn cần gán cả giá trị mã chi nhánh vào ComboBox, bạn có thể sử dụng Tag
                cboMaKH.Tag = dt;
                cboMaKH.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // load thanh toán
        void ThanhToan_Load()
        {
            try
            {
                lenh.CommandText = "SELECT MaTT, PhuongThucTT FROM ThanhToan";
                lenh.Parameters.Clear();

                da.SelectCommand = lenh;
                DataTable dt = new DataTable("ThanhToan");
                da.Fill(dt);

                cboMaTT.DataSource = dt;
                cboMaTT.ValueMember = "MaTT";
                cboMaTT.DisplayMember = "PhuongThucTT";

                // Nếu bạn cần gán cả giá trị mã chi nhánh vào ComboBox, bạn có thể sử dụng Tag
                cboMaTT.Tag = dt;
                cboMaTT.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ngăn ngdung chỉnh sửa trên gridview
        private void SetGridViewReadOnly(GridView gridView)
        {
            gridView.OptionsBehavior.Editable = false;
            // Nếu bạn muốn chặn chỉnh sửa từng dòng cụ thể, bạn có thể sử dụng sự kiện CustomRowCellEdit
            gridView.CustomRowCellEdit += (sender, e) =>
            {
                e.RepositoryItem.ReadOnly = true; // Chặn chỉnh sửa cột này
            };
        }

        // làm mới giá trị
        private void ResetValues_Data() // reset giá trị cho các mục 
        {
            txtMaDH.Text = "";
            cboMaCN.Text = "";
            cboMaKH.Text = "";
            cboMaTT.Text = "";
            dateNgayTao.Text = "";
            txtTenNV.Text = "";
            txtTongTien.Text = "";
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private void grdDonHang_Click(object sender, EventArgs e)
        {
            txtMaDH.DataBindings.Clear();
            txtMaDH.DataBindings.Add("Text", grdDonHang.DataSource, "Mã đơn hàng");
            txtTenNV.DataBindings.Clear();
            txtTenNV.DataBindings.Add("Text", grdDonHang.DataSource, "nhân viên");
            dateNgayTao.DataBindings.Clear();
            dateNgayTao.DataBindings.Add("Text", grdDonHang.DataSource, "Ngày tạo");
            txtTongTien.DataBindings.Clear();
            txtTongTien.DataBindings.Add("Text", grdDonHang.DataSource, "Tổng Tiền");
            cboMaTT.DataBindings.Clear();
            cboMaTT.DataBindings.Add("Text", grdDonHang.DataSource, "thanh toán");
            cboMaCN.DataBindings.Clear();
            cboMaCN.DataBindings.Add("Text", grdDonHang.DataSource, "Chi Nhánh");
            cboMaKH.DataBindings.Clear();
            cboMaKH.DataBindings.Add("Text", grdDonHang.DataSource, "khách hàng");
        }

        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // nếu đang ở chế độ thêm
            if (btnThem.Enabled == false)
                ResetValues_Data();
            btnThem.Enabled = true;
            btnXoa.Enabled = true;
            btnSua.Enabled = true;
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValues_Data();
            // xử lí enable các nút
            btnHuy.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = false;
            // Sinh mã phiếu nhập tự động
            txtMaDH.Text = MaDHTuDong();
        }

        private string MaDHTuDong()
        {
            string maDH = "DH" + DateTime.Now.ToString("yyyyMMddHH");
            txtMaDH.Text = maDH;
            // Kiểm tra xem mã chi tiết nhập đã tồn tại chưa
            int stt = LaySoThuTuTangDan(maDH);

            // Thêm số thứ tự vào mã chi tiết nhập
            maDH += stt.ToString("D2");
            return maDH;


        }
        private int LaySoThuTuTangDan(string maDHang)
        {
            // Kiểm tra xem có chi tiết phiếu nhập nào chưa
            string query = "SELECT MAX(CAST(SUBSTRING(MaDH, 13, 2) AS INT)) FROM DonHang WHERE MaDH LIKE '" + maDHang + "%'";

            object result = data.ExecuteScalar(query);

            // Nếu chưa có chi tiết phiếu nhập nào, trả về 1
            if (result == DBNull.Value)
            {
                return 1;
            }

            // Nếu có chi tiết phiếu nhập rồi, trả về số thứ tự tăng dần tiếp theo
            return Convert.ToInt32(result) + 1;
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                maDH = MaDHTuDong();
                // Kiểm tra xem MaCN và MaNCC đã được chọn hay chưa
                if (cboMaKH.SelectedValue == null || cboMaCN.SelectedValue == null || cboMaTT.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Khách Hàng, Chi nhánh, Phương Thức Thanh Toán !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string insert = @"INSERT INTO DonHang (MaDH, MaKH, TenNV, MaCN, MaTT, TongTien, NgayTao) 
                            VALUES (@MaDH, @MaKH, @TenNV, @MaCN, @MaTT, @TongTien, @NgayTao)";

                lenh.CommandText = insert;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaDH", maDH);
                lenh.Parameters.AddWithValue("@MaKH", cboMaKH.SelectedValue);
                lenh.Parameters.AddWithValue("@TenNV", txtTenNV.Text);
                lenh.Parameters.AddWithValue("@MaCN", cboMaCN.SelectedValue);
                lenh.Parameters.AddWithValue("@MaTT", cboMaTT.SelectedValue);
                lenh.Parameters.AddWithValue("@TongTien", txtTongTien.Text);
                lenh.Parameters.AddWithValue("@NgayTao", DateTime.Now);

                data.ExCuteNonQuery2(lenh);
                DH_Load();
                MessageBox.Show("Thêm phiếu nhập: " + txtMaDH.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không lưu được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bdsource.DataSource = data.DonHang_Info();
            grdDonHang.DataSource = bdsource;
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageBox.Show("Hiện không xóa được !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageBox.Show("Hiện không sửa được !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCTDH_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaDH.Text))
            {
                MessageBox.Show("Vui Lòng Chọn Mã Đơn Hàng !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string maDonHang = txtMaDH.Text;
            using (FrmChiTietDonHang frmChiTietDonHang = new FrmChiTietDonHang())
            {
                frmChiTietDonHang.MaDonHang = maDonHang;
                frmChiTietDonHang.ShowDialog();
            }
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
                MessageBox.Show("Lỗi không in được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
