using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
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
using DataTable = System.Data.DataTable;

namespace MINHTUANMOBILE
{
    public partial class FrmNhapKho : DevExpress.XtraEditors.XtraForm
    {
        public FrmNhapKho()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        string maPhieu;
        private void Nhap_Load()
        {
            string str = "select * from NhapKho";
            SqlDataAdapter adapter = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grdNhapKho.DataSource = dt;
        }

        private void btnctnhap_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPN.Text))
            {
                MessageBox.Show("Vui Lòng Chọn Phiếu Nhập !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string maPhieuNhap = txtPN.Text;
            using (FrmChiTietNhapKho frmChiTietNhapKho = new FrmChiTietNhapKho())
            {
                frmChiTietNhapKho.MaPhieuNhap = maPhieuNhap;
                frmChiTietNhapKho.ShowDialog();
            }

        }
        private void FrmNhapKho_Load(object sender, EventArgs e)
        {
            ChiNhanh_Load();
            NCC_Load();
            bdsource.DataSource = data.NhapKho_Info();
            grdNhapKho.DataSource = bdsource;
            SetGridViewReadOnly(gridViewNK);
            // Làm mới GridControl
            grdNhapKho.RefreshDataSource();
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValues_Data();
            // xử lí enable các nút

            btnHuy.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = false;
            // Sinh mã phiếu nhập tự động
            txtPN.Text = PNTuDong();
        }

        private string PNTuDong()
        {
            string maPhieu = "PN" + DateTime.Now.ToString("yyyyMMddHH");
            txtPN.Text = maPhieu;
            // Kiểm tra xem mã chi tiết nhập đã tồn tại chưa
            int stt = LaySoThuTuTangDan(maPhieu);

            // Thêm số thứ tự vào mã chi tiết nhập
            maPhieu += stt.ToString("D2");
            return maPhieu;


        }
        private int LaySoThuTuTangDan(string maPhieuNhap)
        {
            // Kiểm tra xem có chi tiết phiếu nhập nào chưa
            string query = "SELECT MAX(CAST(SUBSTRING(PhieuNhap, 13, 2) AS INT)) FROM NhapKho WHERE PhieuNhap LIKE '" + maPhieuNhap + "%'";

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
                maPhieu = PNTuDong();
                // Kiểm tra xem MaCN và MaNCC đã được chọn hay chưa
                if (cboCN.SelectedValue == null || cboNCC.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Chi Nhánh Và Nhà Cung Cấp !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string insert = @"INSERT INTO NhapKho (PhieuNhap, MaCN, MaNCC, NguoiTao, NgayNhap, TongTien, TrangThai) 
                            VALUES (@PhieuNhap, @MaCN, @MaNCC, @NguoiTao, @NgayNhap, @TongTien, @TrangThai)";

                lenh.CommandText = insert;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@PhieuNhap", maPhieu);
                lenh.Parameters.AddWithValue("@MaCN", cboCN.SelectedValue);
                lenh.Parameters.AddWithValue("@MaNCC", cboNCC.SelectedValue);
                lenh.Parameters.AddWithValue("@NguoiTao", txtNguoiTao.Text);
                lenh.Parameters.AddWithValue("@NgayNhap", DateTime.Now);
                lenh.Parameters.AddWithValue("@TongTien", txtTongTien.Text);
                lenh.Parameters.AddWithValue("@TrangThai", false);

                data.ExCuteNonQuery2(lenh);

                MessageBox.Show("Thêm Phiếu Nhập: " + txtPN.Text + " Thành Công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Nhap_Load();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không lưu được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bdsource.DataSource = data.NhapKho_Info();
            grdNhapKho.DataSource = bdsource;
        }
        private void ResetValues_Data() // reset giá trị cho các mục 
        {
            txtPN.Text = "";
            cboCN.Text = "";
            cboNCC.Text = "";
            txtNguoiTao.Text = "";
            datePhieuNhap.Text = "";
            txtTongTien.Text = "";
        }

        private void grdNhapKho_Click(object sender, EventArgs e)
        {
            txtPN.DataBindings.Clear();
            txtPN.DataBindings.Add("Text", grdNhapKho.DataSource, "Phiếu Nhập");
            cboCN.DataBindings.Clear();
            cboCN.DataBindings.Add("Text", grdNhapKho.DataSource, "Mã chi nhánh");
            cboNCC.DataBindings.Clear();
            cboNCC.DataBindings.Add("Text", grdNhapKho.DataSource, "Mã nhà cung cấp");
            txtNguoiTao.DataBindings.Clear();
            txtNguoiTao.DataBindings.Add("Text", grdNhapKho.DataSource, "Người Tạo");
            datePhieuNhap.DataBindings.Clear();
            datePhieuNhap.DataBindings.Add("Text", grdNhapKho.DataSource, "Ngày nhập");
            txtTongTien.DataBindings.Clear();
            txtTongTien.DataBindings.Add("Text", grdNhapKho.DataSource, "Tổng Tiền");

        }
        void ChiNhanh_Load()
        {
            try
            {
                lenh.CommandText = "SELECT MaCN, TenCN FROM ChiNhanh";
                lenh.Parameters.Clear();

                da.SelectCommand = lenh;
                DataTable dt = new DataTable("ChiNhanh");
                da.Fill(dt);

                cboCN.DataSource = dt;
                cboCN.ValueMember = "MaCN";
                cboCN.DisplayMember = "TenCN";
                // gán cả giá trị mã chi nhánh vào ComboBox, bạn có thể sử dụng Tag
                cboCN.Tag = dt;
                cboCN.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void NCC_Load()
        {
            try
            {
                lenh.CommandText = "SELECT MaNCC, TenNCC FROM NhaCungCap";
                lenh.Parameters.Clear();

                da.SelectCommand = lenh;
                DataTable dt = new DataTable("NhaCungCap");
                da.Fill(dt);

                cboNCC.DataSource = dt;
                cboNCC.ValueMember = "MaNCC";
                cboNCC.DisplayMember = "TenNCC";

                // Nếu bạn cần gán cả giá trị mã chi nhánh vào ComboBox, bạn có thể sử dụng Tag
                cboNCC.Tag = dt;
                cboNCC.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // nếu đang ở chế độ thêm
            if (btnThem.Enabled == false)
                ResetValues_Data();
            btnThem.Enabled = true;
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

        private void btnXuatPN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtPN.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Phiếu Nhập Để In !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SqlConnection conn = new SqlConnection(@"Data Source=HAVI\HAVISQLEXPRESS;Initial Catalog=MinhTuanMobile_DB;Integrated Security=True");
                conn.Open();
                // Sử dụng parameterized query để tránh SQL Injection
                SqlCommand command = new SqlCommand("Select * from Report_PN where PhieuNhap = '" + txtPN.Text + "'", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();

                PNReport report = new PNReport();
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
