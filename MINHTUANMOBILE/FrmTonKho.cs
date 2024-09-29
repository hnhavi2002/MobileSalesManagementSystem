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
    public partial class FrmTonKho : DevExpress.XtraEditors.XtraForm
    {
        public FrmTonKho()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
        }
        ConnectDB data = new ConnectDB();
        SqlCommand lenh = new SqlCommand();

        void CN_Load()
        {
            ConnectDB.FillCombo("SELECT MaCN, TenCN FROM ChiNhanh", cboMaCN, "MaCN", "TenCN");
            cboMaCN.SelectedIndex = -1;
        }
        private void FrmTonKho_Load(object sender, EventArgs e)
        {
            CN_Load();
        }

        private void btnTim_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (cboMaCN.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Chi Nhánh !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                lenh.Connection = data.GetConnection();
                lenh.Connection.Open();

                string query = "SELECT SanPham.TenSP as [Tên Sản Phẩm], MauSac.TenMau as [ Màu Sắc], DungLuong.LoaiDL as [Loại dung lượng], BaoHanh as [Bảo Hành], GiaBan as [Giá Bán], SLTon as [Số lượng tồn]" +
               " FROM SanPham" +
               " INNER JOIN Dungluong ON SanPham.MaDL = DungLuong.MaDL" +
               " INNER JOIN MauSac ON SanPham.MaMau = MauSac.MaMau" +
               " WHERE SanPham.MaCN = @MaCN AND SanPham.NgayTao BETWEEN @TuNgay AND @DenNgay";

                lenh.CommandText = query;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaCN", cboMaCN.SelectedValue);
                lenh.Parameters.AddWithValue("@TuNgay", dtBD.DateTime);
                lenh.Parameters.AddWithValue("@DenNgay", dtKT.DateTime);
                SqlDataAdapter da = new SqlDataAdapter(lenh);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Tính doanh thu và hiển thị vào txtDoanhThu
                decimal tonkho = TinhTonKho(cboMaCN.SelectedValue.ToString());

                // Hiển thị doanh thu lên txtDoanhThu
                txtTonCuoi.Text = tonkho.ToString();

                //load thông tin lên gridcontrol
                grdTonKho.DataSource = dt;

                // ngăn không cho chỉnh sửa trong gridview
                gridView1.OptionsBehavior.Editable = false;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                lenh.Connection.Close();
            }
        }

        private decimal TinhTonKho(string maChiNhanh)
        {
            decimal tongTien = 0;

            try
            {
                // Thực hiện truy vấn SQL để tính tổng doanh thu từ đơn hàng của chi nhánh
                string query = "SELECT SUM(SLTon) FROM SanPham WHERE MaCN = @MaCN";
                lenh.CommandText = query;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaCN", maChiNhanh);

                // Sử dụng ExecuteScalar để lấy giá trị tổng tiền
                object result = lenh.ExecuteScalar();

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
    }
}
