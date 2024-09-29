using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using DevExpress.CodeParser;
using DevExpress.XtraEditors.Mask.Design;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using OfficeOpenXml;
namespace MINHTUANMOBILE
{
    public partial class FrmSanPham : DevExpress.XtraEditors.XtraForm
    {
        public FrmSanPham()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter adap = new SqlDataAdapter();
        string maSP;
        private void FrmSanPham_Load(object sender, EventArgs e)
        {
            LoaiSP_Data();
            MauSac_Data();
            DL_data();
            HDH_Data();
            CN_Data();
            SetGridViewReadOnl(gridView1);
            // grdSanPham.DataSource = GetProductData();
            bdsource.DataSource = data.SanPham_Info();
            grdSanPham.DataSource = bdsource;      
        }

        void LoaiSP_Data()
        {
            lenh.CommandText = "select MaLoaiSP, TenLoaiSP from LoaiSanPham";
            lenh.Parameters.Clear();
            adap.SelectCommand = lenh;
            DataTable dt = new DataTable("LoaiSanPham");
            adap.Fill(dt);
            cboMaLoaiSP.DataSource = dt;
            cboMaLoaiSP.ValueMember = "MaLoaiSP";
            ConnectDB.FillCombo("SELECT MaLoaiSP, TenLoaiSP from LoaiSanPham", cboMaLoaiSP, "MaLoaiSP", "MaLoaiSP");
            cboMaLoaiSP.SelectedIndex = -1;

        }
        void MauSac_Data()
        {
            lenh.CommandText = "select MaMau, TenMau from MauSac";
            lenh.Parameters.Clear();
            adap.SelectCommand = lenh;
            DataTable dt = new DataTable("MauSac");
            adap.Fill(dt);
            cboMaMau.DataSource = dt;
            cboMaMau.ValueMember = "MaMau";
            ConnectDB.FillCombo("SELECT MaMau, TenMau from MauSac", cboMaMau, "MaMau", "MaMau");
            cboMaMau.SelectedIndex = -1;

        }
        void DL_data()
        {
            lenh.CommandText = "select MaDL, LoaiDL from DungLuong";
            lenh.Parameters.Clear();
            adap.SelectCommand = lenh;
            DataTable dt = new DataTable("DungLuong");
            adap.Fill(dt);
            cboMaDL.DataSource = dt;
            cboMaDL.ValueMember = "MaDL";
            ConnectDB.FillCombo("SELECT MaDL, LoaiDL from DungLuong", cboMaDL, "MaDL", "MaDL");
            cboMaDL.SelectedIndex = -1;

        }
        void HDH_Data()
        {
            lenh.CommandText = "select MaHDH, TenHDH from HeDieuHanh";
            lenh.Parameters.Clear();
            adap.SelectCommand = lenh;
            DataTable dt = new DataTable("HeDieuHanh");
            adap.Fill(dt);
            cboMaHDH.DataSource = dt;
            cboMaHDH.ValueMember = "MaHDH";
            ConnectDB.FillCombo("SELECT MaHDH, TenHDH from HeDieuHanh", cboMaHDH, "MaHDH", "MaHDH");
            cboMaHDH.SelectedIndex = -1;

        }
      
        void CN_Data()
        {
            lenh.CommandText = "select MaCN, TenCN from ChiNhanh";
            lenh.Parameters.Clear();
            adap.SelectCommand = lenh;
            DataTable dt = new DataTable("ChiNhanh");
            adap.Fill(dt);
            cboMaCN.DataSource = dt;
            cboMaCN.ValueMember = "MaCN";
            ConnectDB.FillCombo("SELECT  MaCN, TenCN from ChiNhanh", cboMaCN, "MaCN", "MaCN");
            cboMaCN.SelectedIndex = -1;

        }
        //import file excel hàng hóa vào bảng sản phẩm
        private void btnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImportDataFromExcel(openFileDialog.FileName);
                // Sau khi import, làm mới danh sách sản phẩm trong GridControl
                RefreshProductGrid();
            }
        }

        private void RefreshProductGrid()
        {
            // Viết logic để làm mới GridControl
            // Ví dụ: Gán lại nguồn dữ liệu cho GridControl sau khi import
            grdSanPham.DataSource = GetProductData();
        }

        //lấy danh sách sản phẩm
        private object GetProductData()
        {
            string str = "select * from SanPham";
            SqlDataAdapter adapter = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grdSanPham.DataSource = dt;
            return dt;
        }

        private void ImportDataFromExcel(string filePath)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings[@"Data Source=HAVI\HAVISQLEXPRESS;Initial Catalog=MinhTuanMobile_DB;Integrated Security=True"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                                int rowCount = worksheet.Dimension.Rows;
                                int colCount = worksheet.Dimension.Columns;

                                // Đặt tên cột tương ứng với cột trong tệp Excel
                                string MaSPcolumn = "Mã sản phẩm";
                                string TenSPcolumn = "Tên sản phẩm";
                                string Hinhcolumn = "Hình";
                                string MaMaucolumn = "Mã màu";
                                string Maucolumn = "Tên màu";
                                string MaHDHcolumn = "Mã hệ điều hành";
                                string TenHDHcolumn = "Tên hệ điều hành";
                                string MaDLcolumn = "Mã dung lượng";
                                string LoaiDLcolumn = "Loại dung lượng";
                                string MHcolumn = "Màn hình";
                                string MaLSPcolumn = "Mã loại sản phẩm";
                                string TenLSPcolumn = "Tên loại sản phẩm";
                                string MaNCCcolumn = "Mã nhà cung cấp";
                                string THcolumn = "Thương hiệu";
                                string MaCNcolumn = "Mã chi nhánh";
                                string TenCNcolumn = "Tên chi nhánh";
                                string BHanhcolumn = "Bảo hành";
                                string Giacolumn = "Giá bán";
                                string SLcolumn = "Số lượng";


                                // Lấy danh sách sản phẩm từ cột Excel
                               // List<Product> products = new List<Product>();
                                DataTable dt = new DataTable();
                                dt.Columns.Add(MaSPcolumn, typeof(string));
                                dt.Columns.Add(TenSPcolumn, typeof(string));
                                dt.Columns.Add(Hinhcolumn, typeof(string));
                                dt.Columns.Add(MaMaucolumn, typeof(string));
                                dt.Columns.Add(Maucolumn, typeof(string));
                                dt.Columns.Add(MaHDHcolumn, typeof(string));
                                dt.Columns.Add(TenHDHcolumn, typeof(string));
                                dt.Columns.Add(MaDLcolumn, typeof(string));
                                dt.Columns.Add(LoaiDLcolumn, typeof(string));
                                dt.Columns.Add(MHcolumn, typeof(string));
                                dt.Columns.Add(MaLSPcolumn, typeof(string));
                                dt.Columns.Add(TenLSPcolumn, typeof(string));
                                dt.Columns.Add(MaNCCcolumn, typeof(string));
                                dt.Columns.Add(THcolumn, typeof(string));
                                dt.Columns.Add(MaCNcolumn, typeof(string));
                                dt.Columns.Add(TenCNcolumn, typeof(string));
                                dt.Columns.Add(BHanhcolumn, typeof(string));
                                dt.Columns.Add(Giacolumn, typeof(float));
                                dt.Columns.Add(SLcolumn, typeof(string));

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    Product product = new Product();
                                    product.MaSP = worksheet.Cells[row, 1].Value?.ToString();
                                    product.TenSP = worksheet.Cells[row, 2].Value?.ToString();
                                    product.Hinh = worksheet.Cells[row, 3].Value?.ToString();
                                    product.MaMau = worksheet.Cells[row, 4].Value?.ToString();
                                    product.TenMau = worksheet.Cells[row, 5].Value?.ToString();
                                    product.MaHDH = worksheet.Cells[row, 6].Value?.ToString();
                                    product.TenHDH = worksheet.Cells[row, 7].Value?.ToString();
                                    product.MaDL = worksheet.Cells[row, 8].Value?.ToString();
                                    product.LoaiDL = worksheet.Cells[row, 9].Value?.ToString();
                                    product.ManHinh = worksheet.Cells[row, 10].Value?.ToString();
                                    product.MaLoaiSP = worksheet.Cells[row, 11].Value?.ToString();
                                    product.TenLoaiSP = worksheet.Cells[row, 12].Value?.ToString();
                                    product.MaNCC = worksheet.Cells[row, 13].Value?.ToString();
                                    product.ThuongHieu = worksheet.Cells[row, 14].Value?.ToString();
                                    product.MaCN = worksheet.Cells[row, 15].Value?.ToString();
                                    product.TenCN = worksheet.Cells[row, 16].Value?.ToString();
                                    product.BaoHanh = worksheet.Cells[row, 17].Value?.ToString();
                                    product.GiaBan = Convert.ToSingle(worksheet.Cells[row, 18].Value);
                                    product.SoLuong = Convert.ToInt32(worksheet.Cells[row, 19].Value);
                                    dt.Rows.Add(product);
                                }
                                // Thực hiện thêm sản phẩm vào cơ sở dữ liệu
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                                {
                                    bulkCopy.DestinationTableName = "Product";
                                    bulkCopy.WriteToServer(dt);
                                }

                                // Commit transaction khi mọi thứ thành công
                                transaction.Commit();
                                transaction.Commit();
                            }

                            // Hiển thị thông báo import thành công
                            MessageBox.Show("Import dữ liệu thành công!");
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction nếu có lỗi
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi khi có lỗi xảy ra
                MessageBox.Show($"Lỗi import dữ liệu: {ex.Message}");
            }
            //// Thực hiện thêm sản phẩm vào cơ sở dữ liệu
            //foreach (var product in products)
            //                    {
            //                        using (SqlCommand cmd = new SqlCommand("INSERT INTO SanPham (MaSP, TenSP, Hinh, MaMau, TenMau, MaHDH, TenHDH, MaDL, LoaiDL, ManHinh, MaLoaiSP, TenLoaiSP, MaNCC, ThuongHieu, MaCN, TenCN, BaoHanh, GiaBan, SoLuong) " +
            //                            "VALUES (@MaSP, @TenSP, @Hinh, @MaMau, @TenMau, @MaHDH, @TenHDH, @MaDL, @LoaiDL, @ManHinh, @MaLoaiSP, @TenLoaiSP, @MaNCC, @ThuongHieu, @MaCN, @TenCN, @BaoHanh, @GiaBan, @SoLuong)", connection, transaction))
            //                        {
            //                            cmd.Parameters.AddWithValue("@MaSP", product.MaSP);
            //                            cmd.Parameters.AddWithValue("@TenSP", product.TenSP);
            //                            cmd.Parameters.AddWithValue("@Hinh", product.Hinh);
            //                            cmd.Parameters.AddWithValue("@MaMau", product.MaMau);
            //                            cmd.Parameters.AddWithValue("@TenMau", product.TenMau);
            //                            cmd.Parameters.AddWithValue("@MaHDH", product.MaHDH);
            //                            cmd.Parameters.AddWithValue("@TenHDH", product.TenHDH);
            //                            cmd.Parameters.AddWithValue("@MaDL", product.MaDL);
            //                            cmd.Parameters.AddWithValue("@LoaiDL", product.LoaiDL);
            //                            cmd.Parameters.AddWithValue("@ManHinh", product.ManHinh);
            //                            cmd.Parameters.AddWithValue("@MaLoaiSP", product.MaLoaiSP);
            //                            cmd.Parameters.AddWithValue("@TenLoaiSP", product.TenLoaiSP);
            //                            cmd.Parameters.AddWithValue("@MaNCC", product.MaNCC);
            //                            cmd.Parameters.AddWithValue("@ThuongHieu", product.ThuongHieu);
            //                            cmd.Parameters.AddWithValue("@MaCN", product.MaCN);
            //                            cmd.Parameters.AddWithValue("@TenCN", product.TenCN);
            //                            cmd.Parameters.AddWithValue("@BaoHanh", product.BaoHanh);
            //                            cmd.Parameters.AddWithValue("@GiaBan", product.GiaBan);
            //                            cmd.Parameters.AddWithValue("@SoLuong", product.SoLuong);
            //                            cmd.ExecuteNonQuery();
            //                        }
            //                    }

            //                    // Commit transaction khi mọi thứ thành công
            //                    transaction.Commit();
            //                }

            //                // Hiển thị thông báo import thành công
            //                MessageBox.Show("Import dữ liệu thành công!");
            //            }
            //            catch (Exception ex)
            //            {
            //                // Rollback transaction nếu có lỗi
            //                transaction.Rollback();
            //                throw;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Hiển thị thông báo lỗi khi có lỗi xảy ra
            //    MessageBox.Show($"Lỗi import dữ liệu: {ex.Message}");
            //}
        }

        private void grdSanPham_Click(object sender, EventArgs e)
        {
            txtMaSP.DataBindings.Clear();
            txtMaSP.DataBindings.Add("Text", grdSanPham.DataSource, "Mã SP");
            txtTenSP.DataBindings.Clear();
            txtTenSP.DataBindings.Add("Text", grdSanPham.DataSource, "Tên SP");
            picBoxDT.DataBindings.Clear();
            txtAnh.DataBindings.Clear();
            txtAnh.DataBindings.Add("Text", grdSanPham.DataSource, "Hình");
            cboMaMau.DataBindings.Clear();
            cboMaMau.DataBindings.Add("Text", grdSanPham.DataSource, "Mã màu");
            cboMaHDH.DataBindings.Clear();
            cboMaHDH.DataBindings.Add("Text", grdSanPham.DataSource, "MaHDH");
            cboMaDL.DataBindings.Clear();
            cboMaDL.DataBindings.Add("Text", grdSanPham.DataSource, "MaDL");
            cboMaLoaiSP.DataBindings.Clear();
            cboMaLoaiSP.DataBindings.Add("Text", grdSanPham.DataSource, "Mã loại sp");    
            cboMaCN.DataBindings.Clear();
            cboMaCN.DataBindings.Add("Text", grdSanPham.DataSource, "MaCN");
            txtBaoHanh.DataBindings.Clear();
            txtBaoHanh.DataBindings.Add("Text", grdSanPham.DataSource, "bảo hành");
            txtGiaBan.DataBindings.Clear();
            txtGiaBan.DataBindings.Add("Text", grdSanPham.DataSource, "giá bán");
            txtSL.DataBindings.Clear();
            txtSL.DataBindings.Add("Text", grdSanPham.DataSource, "số lượng tồn");
            try
            {
                picBoxDT.DataBindings.Clear();

                if (!string.IsNullOrEmpty(txtAnh.Text) && File.Exists(txtAnh.Text))
                {
                    picBoxDT.Image = Image.FromFile(txtAnh.Text);
                }
                else
                {
                    // Nếu đường dẫn ảnh trống hoặc ảnh không tồn tại, làm trống PictureBox
                    picBoxDT.Image = null;
                    MessageBox.Show("Ảnh không tồn tại, vui lòng kiểm tra lại đường dẫn tới ảnh !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // hàm lấy file ảnh từ folder
        private void btnOpenFileImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg)|*.jpg|GIF(*.gif)|*.gif|All files(*.*)|*.*";
            dlgOpen.FilterIndex = 2;
            dlgOpen.Title = "Chọn hình sản phẩm";
            if (picBoxDT.Image != null)
            {
                picBoxDT.Image.Dispose();
            }
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                picBoxDT.Image = Image.FromFile(dlgOpen.FileName);
                txtAnh.Text = dlgOpen.FileName;
            }
        }
       
        //tự động sinh mã sản phẩm
        private string MaSPTuDong()
        {
            string maSP = "SP" + DateTime.Now.ToString("yyyyMMddHH");
            txtMaSP.Text = maSP;
            // Kiểm tra xem mã chi tiết nhập đã tồn tại chưa
            int stt = LaySoThuTuTangDan(maSP);

            // Thêm số thứ tự vào mã chi tiết nhập
            maSP += stt.ToString("D2");
            return maSP;
        }

        private int LaySoThuTuTangDan(string maSP)
        {
            // Kiểm tra xem có chi tiết phiếu nhập nào chưa
            string query = "SELECT MAX(CAST(SUBSTRING(MaSP, 13, 2) AS INT)) FROM SanPham WHERE MaSP LIKE '" + maSP + "%'";

            object result = data.ExecuteScalar(query);

            // Nếu chưa có chi tiết phiếu nhập nào, trả về 1
            if (result == DBNull.Value)
            {
                return 1;
            }

            // Nếu có chi tiết phiếu nhập rồi, trả về số thứ tự tăng dần tiếp theo
            return Convert.ToInt32(result) + 1;
        }

        private void ResetValues_Data() // reset giá trị cho các mục 
        {
            txtMaSP.Text = "";
            txtTenSP.Text = "";
            txtAnh.Text = "";
            cboMaMau.Text = "";
            cboMaDL.Text = "";
            cboMaHDH.Text = "";
            cboMaLoaiSP.Text = "";
            cboMaCN.Text = "";
            txtSL.Text = "";
            txtBaoHanh.Text = "";
            txtGiaBan.Text = "";
            txtLoaiSP.Text = "";
            txtDL.Text = "";
            txtTenMau.Text = "";
            txtHDH.Text = "";
            txtCN.Text = "";
            picBoxDT.Image = null;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }
        private void cboMaHDH_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaHDH.SelectedIndex != -1)
            {
                string str;
                if (cboMaHDH.Text == "")
                {
                    txtHDH.Text = "";
                }
                str = "Select TenHDH from HeDieuHanh where MaHDH = N'" + cboMaHDH.SelectedValue + "'";
                txtHDH.Text = ConnectDB.GetFieldValues(str);
            }
        }
        private void cboMaMau_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str;
            if (cboMaMau.Text == "")
            {
                txtTenMau.Text = "";
            }
            str = "Select TenMau from MauSac where MaMau = N'" + cboMaMau.SelectedValue + "'";
            txtTenMau.Text = ConnectDB.GetFieldValues(str);
        }

        private void cboMaDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str;
            if (cboMaDL.Text == "")
            {
                txtDL.Text = "";
            }
            str = "Select LoaiDL from DungLuong where MaDL = N'" + cboMaDL.SelectedValue + "'";
            txtDL.Text = ConnectDB.GetFieldValues(str);
        }

        private void cboMaLoaiSP_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str;
            if (cboMaLoaiSP.Text == "")
            {
                txtLoaiSP.Text = "";
            }
            str = "Select TenLoaiSP from LoaiSanPham where MaLoaiSP = N'" + cboMaLoaiSP.SelectedValue + "'";
            txtLoaiSP.Text = ConnectDB.GetFieldValues(str);
        }
        private void cboMaCN_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str;
            if (cboMaCN.Text == "")
            {
                txtCN.Text = "";
            }
            str = "Select TenCN from ChiNhanh where MaCN = N'" + cboMaCN.SelectedValue + "'";
            txtCN.Text = ConnectDB.GetFieldValues(str);
        }
        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValues_Data();
            // xử lí enable các nút

            btnHuy.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = false;
            txtMaSP.Text = MaSPTuDong();
        }
        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                maSP = MaSPTuDong();
                if (txtMaSP.Text=="")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã Sản Phẩm !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (cboMaDL.SelectedValue == null || cboMaHDH.SelectedValue == null || cboMaMau.SelectedValue ==null)
                {
                    MessageBox.Show("Vui Lòng Chọn Dung Lượng, Hệ Điều Hành, Màu Sắc !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (cboMaCN.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Chi Nhánh !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }  
                string insert = @"insert into SanPham (MaSP, TenSP, Hinh, MaMau, MaHDH, MaDL, MaLoaiSP, MaCN, BaoHanh, GiaBan, SLTon, NgayTao) 
                                    values  (@MaSP, @TenSP, @Hinh, @MaMau, @MaHDH, @MaDL, @MaLoaiSP, @MaCN, @BaoHanh, @GiaBan, @SLTon, @NgayTao)";
                lenh.CommandText = insert;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaSP", maSP);
                lenh.Parameters.AddWithValue("@TenSP", txtTenSP.Text);
                lenh.Parameters.AddWithValue("@Hinh", txtAnh.Text);
                lenh.Parameters.AddWithValue("@MaMau", cboMaMau.SelectedValue);
                lenh.Parameters.AddWithValue("@MaHDH", cboMaHDH.SelectedValue);
                lenh.Parameters.AddWithValue("@MaDL", cboMaDL.SelectedValue);
                lenh.Parameters.AddWithValue("@MaLoaiSP", cboMaLoaiSP.SelectedValue);
                lenh.Parameters.AddWithValue("@MaCN", cboMaCN.SelectedValue);
                lenh.Parameters.AddWithValue("@BaoHanh", txtBaoHanh.Text);
                lenh.Parameters.AddWithValue("@GiaBan", txtGiaBan.Text);
                lenh.Parameters.AddWithValue("@SLTon", txtSL.Text);
                lenh.Parameters.AddWithValue("@NgayTao", DateTime.Now);

                data.ExCuteNonQuery2(lenh);
                MessageBox.Show("Thêm sản phẩm: " + txtMaSP.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetProductData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không lưu được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bdsource.DataSource = data.SanPham_Info();
            grdSanPham.DataSource = bdsource;
        }

        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // nếu đang ở chế độ thêm
            if (btnThem.Enabled == false)
            ResetValues_Data();
            btnThem.Enabled = true;
            btnXoa.Enabled= true;
            btnSua.Enabled= true;
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaSP.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Mã Sản Phẩm Để Xóa !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string delete = @"DELETE FROM SanPham WHERE MaSP = @MaSP";

                lenh.CommandText = delete;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaSP", txtMaSP.Text);

                data.ExCuteNonQuery2(lenh);
                GetProductData();
                MessageBox.Show("Xóa Sản Phẩm Có Mã: " + txtMaSP.Text + " Thành Công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Không Xóa Được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            bdsource.DataSource = data.SanPham_Info();
            grdSanPham.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaSP.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Mã Sản Phẩm Để Cập Nhật !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string update = @"UPDATE SanPham SET TenSP = @TenSP, Hinh = @Hinh, MaMau = @MaMau, MaHDH = @MaHDH, MaDL = @MaDL, MaLoaiSP = @MaLoaiSP, MaCN =@MaCN, BaoHanh =@BaoHanh, GiaBan=@GiaBan, SLTon=@SLTon, NgayTao=@NgayTao
                          WHERE MaSP = @MaSP";

                lenh.CommandText = update;
                lenh.Parameters.Clear();
                lenh.Parameters.AddWithValue("@MaSP", txtMaSP.Text);
                lenh.Parameters.AddWithValue("@TenSP", txtTenSP.Text);
                lenh.Parameters.AddWithValue("@Hinh", txtAnh.Text);
                lenh.Parameters.AddWithValue("@MaMau", cboMaMau.SelectedValue);
                lenh.Parameters.AddWithValue("@MaHDH", cboMaHDH.SelectedValue);
                lenh.Parameters.AddWithValue("@MaDL", cboMaDL.SelectedValue);
                lenh.Parameters.AddWithValue("@MaLoaiSP", cboMaLoaiSP.SelectedValue);
                lenh.Parameters.AddWithValue("@MaCN", cboMaCN.SelectedValue);
                lenh.Parameters.AddWithValue("@BaoHanh", txtBaoHanh.Text);
                lenh.Parameters.AddWithValue("@GiaBan", txtGiaBan.Text);
                lenh.Parameters.AddWithValue("@SLTon", txtSL.Text);
                lenh.Parameters.AddWithValue("@NgayTao", DateTime.Now);

                data.ExCuteNonQuery2(lenh);
                GetProductData();
                MessageBox.Show("Sửa Sản Phẩm Có Mã: " + txtMaSP.Text + " Thành công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Không Sửa Được " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bdsource.DataSource = data.SanPham_Info();
            grdSanPham.DataSource = bdsource;
        }

        private void SetGridViewReadOnl(GridView gridView1)
        {

            gridView1.OptionsBehavior.Editable = false;
            // Nếu bạn muốn chặn chỉnh sửa từng dòng cụ thể, bạn có thể sử dụng sự kiện CustomRowCellEdit
            gridView1.CustomRowCellEdit += (sender, e) =>
            {
                e.RepositoryItem.ReadOnly = true; // Chặn chỉnh sửa cột này
            };
        }
    }

}
