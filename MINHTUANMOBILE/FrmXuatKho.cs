using DevExpress.DataProcessing.InMemoryDataProcessor;
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

namespace MINHTUANMOBILE
{
    public partial class FrmXuatKho : DevExpress.XtraEditors.XtraForm
    {
        public FrmXuatKho()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        private DataTable dt = new DataTable();
        string maPX;
        private void Xuat_Load()
        {
            string str = "select * from XuatKho";
            SqlDataAdapter adapter = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grdXuatKho.DataSource = dt;
        }

        private void btnctxuat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPX.Text))
            {
                MessageBox.Show("Vui Lòng Chọn Phiếu Xuất !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string maPhieuXuat = txtPX.Text;
            using (FrmChiTietXuatKho frmChiTietXuatKho = new FrmChiTietXuatKho())
            {
                frmChiTietXuatKho.MaPhieuXuat = maPhieuXuat;
                frmChiTietXuatKho.ShowDialog();
            }
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
                // gán cả giá trị mã chi nhánh vào ComboBox, bạn có thể sử dụng Tag
                cboNCC.Tag = dt;
                cboNCC.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmXuatKho_Load(object sender, EventArgs e)
        {
            ChiNhanh_Load();
            NCC_Load();
            bdsource.DataSource = data.XuatKho_Info();
            grdXuatKho.DataSource = bdsource;
            SetGridViewReadOnly(gridView1);
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

   
        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // nếu đang ở chế độ thêm
            if (btnThem.Enabled == false)
                ResetValues_Data();
            btnThem.Enabled = true;
        }

        private void ResetValues_Data()
        {
            txtPX.Text = "";
            cboCN.Text = "";
            cboNCC.Text = "";
            txtNguoiTao.Text = "";
            dateXuatKho.Text = ""; 
        }

        private void grdXuatKho_Click(object sender, EventArgs e)
        {
            txtPX.DataBindings.Clear();
            txtPX.DataBindings.Add("Text", grdXuatKho.DataSource, "Phiếu Xuất");
            cboCN.DataBindings.Clear();
            cboCN.DataBindings.Add("Text", grdXuatKho.DataSource, "Mã chi nhánh");
            cboNCC.DataBindings.Clear();
            cboNCC.DataBindings.Add("Text", grdXuatKho.DataSource, "nhà cung cấp");
            txtNguoiTao.DataBindings.Clear();
            txtNguoiTao.DataBindings.Add("Text", grdXuatKho.DataSource, "Người Tạo");
            dateXuatKho.DataBindings.Clear();
            dateXuatKho.DataBindings.Add("Text", grdXuatKho.DataSource, "Ngày xuất");
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValues_Data();
            // xử lí enable các nút

            btnHuy.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = false;
            // Sinh mã phiếu nhập tự động
            txtPX.Text=PXTuDong();
        }
        private string PXTuDong()
        {
            string maPX = "PX" + DateTime.Now.ToString("yyyyMMddHH");
            txtPX.Text = maPX;
            // Kiểm tra xem mã chi tiết nhập đã tồn tại chưa
            int stt = LaySoThuTuTangDan(maPX);

            // Thêm số thứ tự vào mã chi tiết nhập
            maPX += stt.ToString("D2");
            return maPX;
        }
        private int LaySoThuTuTangDan(string maPX)
        {
            // Kiểm tra xem có chi tiết phiếu nhập nào chưa
            string query = "SELECT MAX(CAST(SUBSTRING(PhieuXuat, 13, 2) AS INT)) FROM XuatKho WHERE PhieuXuat LIKE '" + maPX + "%'";

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
            {// Kiểm tra xem MaCN và MaNCC đã được chọn hay chưa
                if (cboCN.SelectedValue == null || cboNCC.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Chi Nhánh Và Nhà Cung Cấp !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string insert = @"INSERT INTO XuatKho (PhieuXuat, MaCN, MaNCC, NgayXuat, NguoiTao, TrangThai) 
                            VALUES (@PhieuXuat, @MaCN, @MaNCC, @NgayXuat, @NguoiTao, @TrangThai)";

                lenh.CommandText = insert;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@PhieuXuat", txtPX.Text);
                lenh.Parameters.AddWithValue("@MaCN", cboCN.SelectedValue);
                lenh.Parameters.AddWithValue("@MaNCC", cboNCC.SelectedValue);
                lenh.Parameters.AddWithValue("@NgayXuat", DateTime.Now);
                lenh.Parameters.AddWithValue("@NguoiTao", txtNguoiTao.Text);
                lenh.Parameters.AddWithValue("@TrangThai", true);

                data.ExCuteNonQuery2(lenh);

                MessageBox.Show("Thêm Phiếu Xuất: " + txtPX.Text + " Thành Công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Xuat_Load();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không lưu được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bdsource.DataSource = data.XuatKho_Info();
            grdXuatKho.DataSource = bdsource;
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtPX.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Phiếu Xuất Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string delete = @"DELETE FROM XuatKho WHERE PhieuXuat = @PhieuXuat";

                lenh.CommandText = delete;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@PhieuXuat", txtPX.Text);

                data.ExCuteNonQuery2(lenh);
                Xuat_Load();
                MessageBox.Show("Xóa phiếu nhập có mã: " + txtPX.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xóa được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Load lại dữ liệu sau khi xóa
            bdsource.DataSource = data.XuatKho_Info();
            grdXuatKho.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtPX.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Phiếu Xuất Để Sửa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string update = @"UPDATE XuatKho SET MaCN = @MaCN, MaNCC = @MaNCC, NgayXuat = @NgayXuat, NguoiTao = @NguoiTao, TrangThai = @TrangThai
                          WHERE PhieuXuat = @PhieuXuat";

                lenh.CommandText = update;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@PhieuXuat", txtPX.Text);
                lenh.Parameters.AddWithValue("@MaCN", cboCN.SelectedValue);
                lenh.Parameters.AddWithValue("@MaNCC", cboNCC.SelectedValue);
                lenh.Parameters.AddWithValue("@NgayXuat", DateTime.Now);
                lenh.Parameters.AddWithValue("@NguoiTao", txtNguoiTao.Text);
                lenh.Parameters.AddWithValue("@TrangThai", true);

                data.ExCuteNonQuery2(lenh);
                Xuat_Load();
                MessageBox.Show("Sửa Phiếu Xuất Có Mã: " + txtPX.Text + " Thành Công!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Không Sửa Được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Load lại dữ liệu sau khi sửa
            bdsource.DataSource = data.XuatKho_Info();
            grdXuatKho.DataSource = bdsource;
        }

        private void btnIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtPX.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Phiếu Xuất Để In !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SqlConnection conn = new SqlConnection(@"Data Source=HAVI\HAVISQLEXPRESS;Initial Catalog=MinhTuanMobile_DB;Integrated Security=True");
                conn.Open();
                // Sử dụng parameterized query để tránh SQL Injection
                SqlCommand command = new SqlCommand("Select * from Report_PX where PhieuXuat = '" + txtPX.Text + "'", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();

                PXReport report = new PXReport();
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
