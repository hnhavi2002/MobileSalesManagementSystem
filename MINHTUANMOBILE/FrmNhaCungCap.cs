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
    public partial class FrmNhaCungCap : DevExpress.XtraEditors.XtraForm
    {
        public FrmNhaCungCap()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter adap = new SqlDataAdapter();

        private void NhaCungCap_Load()
        {
            string str = "select * from NhaCungCap";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdNhaCungCap.DataSource = dt;
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
        private void FrmNhaCungCap_Load(object sender, EventArgs e)
        {
            LoaiSP_Data();
            bdsource.DataSource = data.NhaCungCap_Info();
            grdNhaCungCap.DataSource = bdsource;
        }

    

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValues_Data();
            // xử lí enable các nút

            btnHuy.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = false;
            btnXoa.Enabled = false;
            btnSua.Enabled = false;
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaNCC.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Nhà Cung Cấp Để Xóa !!!.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"delete from NhaCungCap where MaNCC ='" + txtMaNCC.Text + "';' ");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn Có Muốn Xóa? " + txtMaNCC.Text, "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa Thông Tin Nhà Cung Cấp " +  txtMaNCC.Text + "thành công!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    NhaCungCap_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.NhaCungCap_Info();
            grdNhaCungCap.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaNCC.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Nhà Cung Cấp Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update NhaCungCap set MaNCC = '" + txtMaNCC.Text + "',TenNCC= N'" + txtTenNCC.Text + "',DiaChi = N'" + txtDiachi.Text
                                  + "',SDT ='" + txtSDT.Text + "',Email ='" + txtEmail.Text + "',MaLoaiSP = N'" + cboMaLoaiSP.Text + "',  ThuongHieu = N'" + txtThuongHieu.Text + "'WHERE MaNCC = '" + txtMaNCC.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn Muốn Cập Nhập " + txtMaNCC.Text, "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Cập Nhật Nhà Cung Cấp: " + txtMaNCC.Text + "Thành Công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.NhaCungCap_Info();
            grdNhaCungCap.DataSource = bdsource;
        }

        private void grdNhaCungCap_Click(object sender, EventArgs e)
        {
            txtMaNCC.DataBindings.Clear();
            txtMaNCC.DataBindings.Add("Text", grdNhaCungCap.DataSource, "Mã nhà cung cấp");
            txtTenNCC.DataBindings.Clear();
            txtTenNCC.DataBindings.Add("Text", grdNhaCungCap.DataSource, "Tên nhà cung cấp");
            txtDiachi.DataBindings.Clear();
            txtDiachi.DataBindings.Add("Text", grdNhaCungCap.DataSource, "Địa chỉ");
            txtSDT.DataBindings.Clear();
            txtSDT.DataBindings.Add("Text", grdNhaCungCap.DataSource, "Số điện thoại");
            txtEmail.DataBindings.Clear();
            txtEmail.DataBindings.Add("Text", grdNhaCungCap.DataSource, "Email");
            cboMaLoaiSP.DataBindings.Clear();
            cboMaLoaiSP.DataBindings.Add("Text", grdNhaCungCap.DataSource, "Mã loại sp");
        
            txtThuongHieu.DataBindings.Clear();
            txtThuongHieu.DataBindings.Add("Text", grdNhaCungCap.DataSource, "Thương hiệu");


        }

        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnThem.Enabled == false)
                ResetValues_Data();
            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
        }

        private void ResetValues_Data()
        {
            txtMaNCC.Text = "";
            txtTenNCC.Text = "";
            txtDiachi.Text = "";
            txtSDT.Text = "";
            txtThuongHieu.Text = "";
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtMaNCC.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã Nhà Cung Cấp !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (cboMaLoaiSP.SelectedValue == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Loại Sản Phẩm!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string insert = @"insert into NhaCungCap (MaNCC, TenNCC, DiaChi,SDT, Email, MaLoaiSP, ThuongHieu) 
                                    values ('" + txtMaNCC.Text + "',N'" + txtTenNCC.Text + "',N'" + txtDiachi.Text + "',N'" + txtSDT.Text + "',N'" + txtEmail.Text + "',N'" + cboMaLoaiSP.Text + "',N'"  + txtThuongHieu.Text + "')";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm mã Nhà Cung Cấp:" + txtMaNCC.Text + " Thành Công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NhaCungCap_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.NhaCungCap_Info();
            grdNhaCungCap.DataSource = bdsource;
        }
    }
}
