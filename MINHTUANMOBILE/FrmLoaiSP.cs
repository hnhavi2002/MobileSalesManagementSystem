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
    public partial class FrmLoaiSP : DevExpress.XtraEditors.XtraForm
    {
        public FrmLoaiSP()
        {
            InitializeComponent();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        private void LoaiSP_Load()
        {
            string str = "select * from LoaiSanPham";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdLoaiSP.DataSource = dt;
        }
        private void FrmLoaiSP_Load(object sender, EventArgs e)
        {
            bdsource.DataSource = data.LoaiSanPham_Info();
            grdLoaiSP.DataSource = bdsource;
        }

        private void grdLoaiSP_Click(object sender, EventArgs e)
        {
            txtMaLoaiSP.DataBindings.Clear();
            txtMaLoaiSP.DataBindings.Add("Text", grdLoaiSP.DataSource, "Mã loại sản phẩm");
            txtTenLoaiSP.DataBindings.Clear();
            txtTenLoaiSP.DataBindings.Add("Text", grdLoaiSP.DataSource, "Tên loại sản phẩm");
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
                if (string.IsNullOrEmpty(txtMaLoaiSP.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Loại Sản Phẩm Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"delete from LoaiSanPham where MaLoaiSP = '" + txtMaLoaiSP.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa: " + txtMaLoaiSP.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa loại sản phẩm " + txtMaLoaiSP.Text + "thành công!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    LoaiSP_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.LoaiSanPham_Info();
            grdLoaiSP.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaLoaiSP.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Loại Sản Phẩm Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update LoaiSanPham set MaLoaiSP='" + txtMaLoaiSP.Text + "',TenLoaiSP= N'" + txtTenLoaiSP.Text + "'WHERE MaLoaiSP = '" + txtMaLoaiSP.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaLoaiSP.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật loại sản phẩm " + txtMaLoaiSP.Text + "thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoaiSP_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.LoaiSanPham_Info();
            grdLoaiSP.DataSource = bdsource;
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

        private void ResetValues_Data()
        {
            txtMaLoaiSP.Text = "";
            txtTenLoaiSP.Text = "";
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtMaLoaiSP.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã Loại SP !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string insert_CV = @"insert into LoaiSanPham (MaLoaiSP, TenLoaiSP) 
                                    values ('" + txtMaLoaiSP.Text + "',N'" + txtTenLoaiSP.Text + "')";
                data.ExCuteNonQuery(insert_CV);
                MessageBox.Show("Thêm mã loại sp: " + txtMaLoaiSP.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoaiSP_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.LoaiSanPham_Info();
            grdLoaiSP.DataSource = bdsource;
        }
    }
}
