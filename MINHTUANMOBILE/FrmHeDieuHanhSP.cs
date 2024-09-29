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
    public partial class FrmHeDieuHanhSP : DevExpress.XtraEditors.XtraForm
    {
        public FrmHeDieuHanhSP()
        {
            InitializeComponent();
        }
        ConnectDB data= new ConnectDB();
        private BindingSource bdsource = new BindingSource();

        private void HDH_Load()
        {
            string str = "select * from HeDieuHanh";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdHDH.DataSource = dt;
        }
        private void FrmHeDieuHanhSP_Load(object sender, EventArgs e)
        {
            bdsource.DataSource = data.HeDieuHanh_Info();
            grdHDH.DataSource = bdsource;
        }

        private void grdHDH_Click(object sender, EventArgs e)
        {
            txtMaHDH.DataBindings.Clear();
            txtMaHDH.DataBindings.Add("Text", grdHDH.DataSource, "Mã hệ điều hành");
            txtTenHDH.DataBindings.Clear();
            txtTenHDH.DataBindings.Add("Text", grdHDH.DataSource, "tên hệ điều hành");
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

        private void ResetValues_Data()
        {
            txtMaHDH.Text = "";
            txtTenHDH.Text = "";
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaHDH.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Hệ Điều Hành Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                data.ExCuteNonQuery(@"delete from HeDieuHanh where MaHDH = '" + txtMaHDH.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa: " + txtMaHDH.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa mã hệ điều hành " + txtMaHDH.Text + "thành công!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    HDH_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.HeDieuHanh_Info();
            grdHDH.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaHDH.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Hệ Điều Hành Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update HeDieuHanh set MaHDH='" + txtMaHDH.Text + "',TenHDH= N'" + txtTenHDH.Text + "'WHERE MaHDH = '" + txtMaHDH.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaHDH.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật hệ điều hành " + txtMaHDH.Text + "thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    HDH_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.HeDieuHanh_Info();
            grdHDH.DataSource = bdsource;
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
                if (txtMaHDH.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã HĐH !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string insert = @"insert into HeDieuHanh (MaHDH, TenHDH) 
                                    values ('" + txtMaHDH.Text + "',N'" + txtTenHDH.Text + "')";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm mã hệ điều hành:" + txtMaHDH.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HDH_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.HeDieuHanh_Info();
            grdHDH.DataSource = bdsource;
        }
    }
}
