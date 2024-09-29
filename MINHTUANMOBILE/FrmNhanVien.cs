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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Customization;
using DevExpress.CodeParser;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraExport.Helpers;
using System.Windows.Media.Animation;

namespace MINHTUANMOBILE
{
    public partial class FrmNhanVien : DevExpress.XtraEditors.XtraForm
    {
        public FrmNhanVien()
        {
            InitializeComponent();
            lenh.Connection = data.GetConnection();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        SqlCommand lenh = new SqlCommand();
        SqlDataAdapter da1 = new SqlDataAdapter();
        private void NhanVien_Load()
        {
            string str = "select MaNV, TenNV, DiaChi, SDT, MaCN, MaCV from NhanVien";
            SqlDataAdapter adapter= new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grdNhanVien.DataSource = dt;
        }
   
        //nạp dữ liệu chi nhánh vào lookupedit
        void ChiNhanh_load()
        {
            lenh.CommandText = "select MaCN,TenCN from ChiNhanh";
            lenh.Parameters.Clear();
            da1.SelectCommand = lenh;
            DataTable dt = new DataTable("ChiNhanh");
            da1.Fill(dt);
            lkedMaCN.Properties.DataSource = dt;
            lkedMaCN.Properties.ValueMember = "MaCN";
            ConnectDB.FillCombo("SELECT MaCN,TenCN FROM ChiNhanh", lkedMaCN, "MaCN", "MaCN");
        }

        //nạp dữ liệu chức vụ vào lookupedit
        void ChucVu_Load()
        {
            lenh.CommandText = "Select MaCV, TenCV from ChucVu";
            lenh.Parameters.Clear();
            da1.SelectCommand= lenh;
            DataTable dt = new DataTable("ChucVu");
            da1.Fill(dt);
            lkedMaCV.Properties.DataSource = dt;
            lkedMaCV.Properties.ValueMember = "MaCV";
            ConnectDB.FillCombo("Select MaCV, TenCV from ChucVu", lkedMaCV, "MaCV", "MaCV");
        }

        private void FrmNhanVien_Load(object sender, EventArgs e)
        {
            ChucVu_Load();
            ChiNhanh_load();
            bdsource.DataSource = data.NhanVien_Info();
            grdNhanVien.DataSource = bdsource; 
        }

        private void lkedMaCN_EditValueChanged(object sender, EventArgs e)
        {
            string id = (string)lkedMaCN.EditValue;
            GetListCN(id);
        }
        private void GetListCN(string id)
        {
            lenh.CommandText = "select MaCN, TenCN from ChiNhanh";
            lenh.Parameters.Clear();
            da1.SelectCommand = lenh;
            DataTable dt = new DataTable("ChiNhanh");
            da1.Fill(dt);
            lkedMaCN.Properties.DataSource = dt;
            lkedMaCN.Properties.ValueMember = "MaCN";
            lkedMaCN.Properties.DisplayMember = "MaCN";
        }

        private void lkedMaCV_EditValueChanged(object sender, EventArgs e)
        {
            string id = (string)lkedMaCV.EditValue;
            GetListCV(id);
        }
        
        private void GetListCV(string id)
        {
            lenh.CommandText = "Select MaCV, TenCV from ChucVu";
            lenh.Parameters.Clear();
            da1.SelectCommand = lenh;
            DataTable dt = new DataTable("ChucVu");
            da1.Fill(dt);
            lkedMaCV.Properties.DataSource = dt;
            lkedMaCV.Properties.ValueMember = "MaCV";
            lkedMaCV.Properties.DisplayMember = "MaCV";
        }
        private void btnthem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValues_Data();
            // xử lí enable các nút

            btnHuy.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = false;
            btnXoa.Enabled = false;
            btnSua.Enabled = false;
        }

        private void btnxoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaNV.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Nhân Viên Để Xóa!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                data.ExCuteNonQuery(@"delete from NhanVien where MaNV ='" + txtMaNV.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa? " + txtMaNV.Text, "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa Nhân Viên " + txtTenNV.Text + " Thành Công!", "Thông Báo",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    NhanVien_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.NhanVien_Info();
            grdNhanVien.DataSource = bdsource;
        }

        private void btnsua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaNV.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Nhân Viên Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update NhanVien set MaNV = '" + txtMaNV.Text + "',TenNV = N'" +txtTenNV.Text + "',DiaChi = N'" +txtDiaChi.Text 
                                  + "',SDT ='" + txtSDT.Text + "',MaCN = N'" + lkedMaCN.Text + "', MaCV = N'" + lkedMaCV.Text + "'WHERE MaNV = '" + txtMaNV.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn cập nhật " + txtMaNV.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật nhân viên: " + txtMaNV.Text + "thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.NhanVien_Info();
            grdNhanVien.DataSource = bdsource;
        }
       
        private void grdNhanVien_Click(object sender, EventArgs e)
        {
            txtMaNV.DataBindings.Clear();
            txtMaNV.DataBindings.Add("Text", grdNhanVien.DataSource, "Mã nhân viên");
            txtTenNV.DataBindings.Clear();
            txtTenNV.DataBindings.Add("Text", grdNhanVien.DataSource, "Tên nhân viên");
            txtDiaChi.DataBindings.Clear();
            txtDiaChi.DataBindings.Add("Text", grdNhanVien.DataSource, "địa chỉ");
            txtSDT.DataBindings.Clear();
            txtSDT.DataBindings.Add("Text", grdNhanVien.DataSource, "số điện thoại");
            lkedMaCN.DataBindings.Clear();
            lkedMaCN.DataBindings.Add("Text", grdNhanVien.DataSource, "mã chi nhánh");
            lkedMaCV.DataBindings.Clear();
            lkedMaCV.DataBindings.Add("Text", grdNhanVien.DataSource, "mã chức vụ");
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
            txtMaNV.Text = "";
            txtTenNV.Text = "";
            txtDiaChi.Text = "";
            txtSDT.Text = "";

        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtMaNV.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã NV !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (lkedMaCV.Text == "" || lkedMaCN.Text == "")
                {
                    MessageBox.Show("Vui Lòng Chọn Chi Nhánh Và Chức Vụ !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string insert = @"insert into NhanVien (MaNV, TenNV, DiaChi, SDT, MaCN, MaCV)
                                values('" + txtMaNV.Text + "',N'" + txtTenNV.Text + "',N'" + txtDiaChi.Text + "',N'"
                                + txtSDT.Text + "',N'" + lkedMaCN.Text + "',N'" + lkedMaCV.Text + "');";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm Nhân Viên: " + txtTenNV.Text + " Thành Công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NhanVien_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.NhanVien_Info();
            grdNhanVien.DataSource = bdsource;
        }
    }
}
