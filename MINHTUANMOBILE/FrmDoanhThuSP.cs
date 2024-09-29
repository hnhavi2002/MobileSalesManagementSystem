using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraEditors;
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
    public partial class FrmDoanhThuSP : DevExpress.XtraEditors.XtraForm
    {
        public FrmDoanhThuSP()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
        }
        ConnectDB data = new ConnectDB();
        SqlCommand lenh = new SqlCommand();
        void SP_Load()
        {
            ConnectDB.FillCombo("SELECT MaSP, TenSP FROM SanPham", cboMaSP, "MaSP", "TenSP");
            cboMaSP.SelectedIndex = -1;
        }
        private void FrmDoanhThuSP_Load(object sender, EventArgs e)
        {
            SP_Load();
        }

        private void btnTim_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                if (cboMaSP.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Sản Phẩm !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                lenh.Connection = data.GetConnection();
                lenh.Connection.Open();

                string query = "Select ChiTietDonHang.MaCTDH as [Mã chi tiết đơn], GiaBan as [Giá Bán], SoLuongMua as [Số Lượng Bán], Discount as [Giảm Giá], ThanhTien as [Thành Tiền] from ChiTietDonHang" +
                         " WHERE ChiTietDonHang.MaSP = @MaSP AND ChiTietDonHang.NgayTao BETWEEN @TuNgay AND @DenNgay";

                lenh.CommandText = query;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaSP", cboMaSP.SelectedValue);
                lenh.Parameters.AddWithValue("@TuNgay", dtStart.DateTime);
                lenh.Parameters.AddWithValue("@DenNgay", dtEnd.DateTime);
                SqlDataAdapter da = new SqlDataAdapter(lenh);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Tính doanh thu và hiển thị vào txtDoanhThu
                decimal doanhThu = TinhDoanhThu(cboMaSP.SelectedValue.ToString());
                // Hiển thị doanh thu lên txtDoanhThu
                txtDT.Text = doanhThu.ToString();

                //load thông tin lên gridcontrol
                grdDTSP.DataSource = dt;

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

        private decimal TinhDoanhThu(string maSP)
        {
            decimal tongTien = 0;

            try
            {
                string query = "SELECT SUM(ThanhTien) " +
                        "FROM ChiTietDonHang " +
                        "WHERE MaSP = @MaSP";

                lenh.CommandText = query;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaSP", maSP);

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
    }
}