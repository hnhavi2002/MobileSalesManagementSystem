using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid;

namespace MINHTUANMOBILE
{
    class ConnectDB
    {
        static string connectString = @"Data Source=HAVI\HAVISQLEXPRESS;Initial Catalog=MinhTuanMobile_DB;Integrated Security=True";

        public static void RunSQl(string sql)
        {
            using (SqlConnection connect = new SqlConnection(connectString))
            {
                connect.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql; // gán lệnh SQL
                cmd.Connection = connect; //gán lệnh kết nối
                try
                {
                    cmd.ExecuteNonQuery(); //thực hiện câu lệnh sql
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                cmd.Dispose(); //giải phóng bộ nhớ
                connect.Close();
            }
        }
        public SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connectString);
            return conn;
        }

        public int ExCuteNonQuery(string ex)
        {
            int data = 0;
            using (SqlConnection connect = new SqlConnection(connectString))
            {
                connect.Open();
                SqlCommand thucthi = new SqlCommand(ex, connect);
                data = thucthi.ExecuteNonQuery();
                connect.Close();
            }
            return data;
        }
        public DataTable excuteQuery(string ex)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connect = new SqlConnection(connectString))
            {
                connect.Open();
                SqlCommand thucthi = new SqlCommand(ex, connect);
                SqlDataAdapter laydulieu = new SqlDataAdapter(thucthi);
                laydulieu.Fill(dt);
                connect.Close();
            }
            return dt;
        }
        public static string GetFieldValues(string sql)
        {

            SqlConnection conn = new SqlConnection(connectString);
            conn.Open();
            string ma = "";
            SqlCommand thucthi = new SqlCommand(sql, conn);
            SqlDataReader reader;
            reader = thucthi.ExecuteReader();
            while (reader.Read())
                ma = reader.GetValue(0).ToString();
            reader.Close();
            return ma;
        }

        internal static void FillCombo(string sql, ComboBox cbo, string ma, string ten)
        {
            SqlDataAdapter dap = new SqlDataAdapter(sql, connectString);
            DataTable table = new DataTable();
            dap.Fill(table);
            cbo.DataSource = table;
            cbo.ValueMember = ma; //Trường giá trị
            cbo.DisplayMember = ten; //Trường hiển thị
        }
        public DataTable ChucVu_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaCV as [Mã chức vụ], TenCV as [Tên chức vụ] from ChucVu", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable ChiNhanh_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaCN as [Mã chi nhánh], TenCN as [Tên chi nhánh], DiaChi as [Địa chỉ], SDT as [Số điện thoại] from ChiNhanh", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable KhachHang_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaKH as [Mã khách hàng], TenKH as [Tên khách hàng], DiaChi as [Địa chỉ], SDT as [Số điện thoại] from KhachHang", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable NhanVien_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaNV as [Mã nhân viên], TenNV as [Tên nhân viên], DiaChi as [Địa chỉ], SDT as [Số điện thoại], MaCN as [Mã chi nhánh], MaCV as [Mã chức vụ] from NhanVien", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable LoaiSanPham_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(" Select MaLoaiSP as [Mã loại sản phẩm], TenLoaiSP as [Tên loại sản phẩm] from LoaiSanPham", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable DungLuong_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaDL as [Mã dung lượng], LoaiDL as [Loại dung lượng] from DungLuong", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable MauSac_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaMau as [Mã màu], TenMau as [Tên màu] from MauSac", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable HeDieuHanh_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaHDH as [Mã hệ điều hành], TenHDH as [Tên hệ điều hành] from HeDieuHanh", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable NhaCungCap_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaNCC as [Mã nhà cung cấp], TenNCC as [Tên nhà cung cấp], DiaChi as [Địa chỉ], SDT as [Số điện thoại], Email, MaLoaiSP as [Mã loại sp], ThuongHieu as [Thương hiệu] from NhaCungCap", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable SanPham_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaSP as [Mã SP], TenSP as [Tên SP], Hinh as [Hình], MaMau as [Mã màu], MaHDH,  MaDL, MaLoaiSP as[Mã loại sp], " +
                                                             " MaCN, BaoHanh as [Bảo hành], GiaBan as [Giá bán], SLTon as [Số lượng tồn] from SanPham", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable KhuyenMai_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaKM as [Mã khuyến mãi], TenKM as [Tên khuyến mãi], NgayBD as [Ngày bắt đầu], NgayKT as [Ngày kết thúc], Discount from KhuyenMai", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable ThanhToan_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaTT as [Mã thanh toán], PhuongThucTT as [Phương thức thanh toán] from ThanhToan", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable NhapKho_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select PhieuNhap as [Phiếu nhập], MaCN as [Mã chi nhánh], MaNCC as [Mã nhà cung cấp], NguoiTao as [Người Tạo], NgayNhap as [Ngày nhập], TongTien as [Tổng tiền] from NhapKho", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable ChiTietNhap_Info(string phieuNhap)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                // Sử dụng thực hiện truy vấn với tham số @PhieuNhap
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT MaCTPN, MaSP as [Mã sản phẩm]," +
                                                                   "GiaNhap as [Giá nhập], SoLuongNhap as [Số lượng nhập], ChietKhau as [Chiết khấu], ThanhTien as [Thành tiền] FROM ChiTietNhapKho WHERE PhieuNhap = @PhieuNhap", connection))
                {
                    // Thêm tham số @PhieuNhap vào adapter
                    adapter.SelectCommand.Parameters.AddWithValue("@PhieuNhap", phieuNhap);
                    adapter.Fill(data);
                }
            }
            return data;
        }
        public DataTable XuatKho_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select PhieuXuat as [Phiếu xuất], MaCN as [Mã chi nhánh], MaNCC as [Nhà cung cấp],  NgayXuat as [Ngày xuất], NguoiTao as [Người Tạo] from XuatKho", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable ChiTietXuat_Info(string phieuXuat)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                // Sử dụng thực hiện truy vấn với tham số @PhieuNhap
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT PhieuXuat as [Phiếu Xuất], MaSP as [Mã sản phẩm],SoLuongXuat as [Số lượng xuất] FROM ChiTietXuatKho WHERE PhieuXuat = @PhieuXuat", connection))
                {
                    // Thêm tham số @PhieuNhap vào adapter
                    adapter.SelectCommand.Parameters.AddWithValue("@PhieuXuat", phieuXuat);
                    adapter.Fill(data);
                }
            }
            return data;
        }
        public DataTable DonHang_Info()
        {
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter("Select MaDH as [Mã đơn hàng], MaKH as [Khách hàng], TenNV as [Nhân viên], MaCN as [Chi Nhánh]" +
                                                            ",MaTT as [Thanh toán], TongTien as [Tổng tiền], NgayTao as [Ngày tạo] from DonHang", GetConnection());
            adapter.Fill(data);
            return data;
        }
        public DataTable ChiTietDonHang_Info(String donHang)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                // Sử dụng thực hiện truy vấn với tham số @PhieuNhap
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT MaCTDH as [Mã CTDH], MaSP as [Sản phẩm], GiaBan as [Giá Bán], SoLuongMua as [Số lượng] , Discount as [Giảm Giá], ThanhTien as [Thành Tiền] FROM ChiTietDonHang WHERE MaDH = @MaDH", connection))
                {
                    // Thêm tham số @PhieuNhap vào adapter
                    adapter.SelectCommand.Parameters.AddWithValue("@MaDH", donHang);
                    adapter.Fill(data);
                }
            }
            return data;  
        }

        internal static void FillCombo(string v1, DevExpress.XtraEditors.LookUpEdit lookupedit, string v2, string v3)
        {
            SqlDataAdapter dap = new SqlDataAdapter(v1, connectString);
            DataTable table = new DataTable();
            dap.Fill(table);
            lookupedit.Properties.DataSource = table;
            lookupedit.Properties.ValueMember = v2; //Trường giá trị
            lookupedit.Properties.DisplayMember = v3; //Trường hiển thị
        }

        internal object ExecuteScalar(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Thực hiện truy vấn và trả về giá trị của cột đầu tiên của dòng đầu tiên
                    object result = command.ExecuteScalar();

                    return result;
                }
            }
        }

        public int ExCuteNonQuery2(SqlCommand command)
        {
            using (SqlConnection connection = new SqlConnection(connectString))
            {
                connection.Open();
                command.Connection = connection;

                int affectedRows = command.ExecuteNonQuery();

                return affectedRows;
            }
        }

        internal object ExecuteScalar(SqlCommand lenh)
        {

                using (SqlConnection connection = new SqlConnection(connectString))
                {
                    connection.Open();
                    lenh.Connection = connection;
                    // Thực hiện truy vấn và trả về giá trị duy nhất
                    return lenh.ExecuteScalar();
                }
            }

        internal DataTable ExCuteDataTable(SqlCommand lenh)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectString))
            {
                lenh.Connection = connection;

                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(lenh))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý exception (log, hiển thị thông báo, v.v.)
                    Console.WriteLine("Lỗi khi thực hiện truy vấn: " + ex.Message);
                }
            }

            return dataTable;
        }
    }
}
     


