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
using System.Windows.Media.Animation;

namespace MINHTUANMOBILE
{
    public partial class FrmChucVu : DevExpress.XtraEditors.XtraForm
    { 
        public FrmChucVu()
        {
            InitializeComponent();
        }
        ConnectDB data= new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        private void ChucVu_Load()
        {
            string str = "select * from ChucVu";
            SqlDataAdapter adapter = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grdChucVu.DataSource = dt;
        }
        private void FrmChucVu_Load(object sender, EventArgs e)
        {
            bdsource.DataSource = data.ChucVu_Info();
            grdChucVu.DataSource = bdsource;
        }

        private void grdChucVu_Click(object sender, EventArgs e)
        {
            txtMaCV.DataBindings.Clear();
            txtMaCV.DataBindings.Add("Text", grdChucVu.DataSource, "Mã chức vụ"); 
            txtTenCV.DataBindings.Clear();
            txtTenCV.DataBindings.Add("Text", grdChucVu.DataSource, "Tên chức vụ");
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
                if (string.IsNullOrEmpty(txtMaCV.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Chức Vụ Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                data.ExCuteNonQuery(@"delete from ChucVu where MaCV = '" + txtMaCV.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa: " + txtMaCV.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa chức vụ " + txtMaCV.Text + "thành công!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    ChucVu_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.ChucVu_Info();
            grdChucVu.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaCV.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Chức Vụ Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update ChucVu set MaCV='" + txtMaCV.Text + "',TenCV= N'" + txtTenCV.Text + "'WHERE MaCV = '" + txtMaCV.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaCV.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật chức vụ " + txtMaCV.Text + "thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ChucVu_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.ChucVu_Info();
            grdChucVu.DataSource = bdsource;
        }
        private void ResetValues_Data() // reset giá trị cho các mục 
        {
            txtMaCV.Text = "";
            txtTenCV.Text = "";
        }

        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // nếu đang ở chế độ thêm
            if (btnThem.Enabled == false)
                ResetValues_Data();
            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtMaCV.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã CV !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string insert = @"insert into ChucVu (MaCV,TenCV)
                                    values ('" + txtMaCV.Text + "',N'" + txtTenCV.Text + "')";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm mã chức vụ: " + txtMaCV.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChucVu_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.ChucVu_Info();
            grdChucVu.DataSource = bdsource;

            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
        }
    }
}
