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
    public partial class FrmMauSacSP : DevExpress.XtraEditors.XtraForm
    {
        public FrmMauSacSP()
        {
            InitializeComponent();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        private void MauSac_Load()
        {
            string str = "Select * from MauSac";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdMauSac.DataSource = dt;
        }
        private void FrmMauSacSP_Load(object sender, EventArgs e)
        {
            bdsource.DataSource = data.MauSac_Info();
            grdMauSac.DataSource = bdsource;
        }

        private void grdMauSac_Click(object sender, EventArgs e)
        {
            txtMaMau.DataBindings.Clear();
            txtMaMau.DataBindings.Add("Text", grdMauSac.DataSource, "Mã màu");
            txtMauSac.DataBindings.Clear();
            txtMauSac.DataBindings.Add("Text", grdMauSac.DataSource, "Tên màu");
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
                if (string.IsNullOrEmpty(txtMaMau.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Mã Màu Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"delete from MauSac where MaMau = '" + txtMaMau.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa: " + txtMaMau.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa mã màu " + txtMaMau.Text + "thành công!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    MauSac_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.MauSac_Info();
            grdMauSac.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaMau.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Mã Màu Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update MauSac set MaMau='" + txtMaMau.Text + "',TenMau= N'" + txtMauSac.Text + "'WHERE MaMau = '" + txtMaMau.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaMau.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật màu sắc " + txtMaMau.Text + "thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MauSac_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.MauSac_Info();
            grdMauSac.DataSource = bdsource;
        }
        private void ResetValues_Data() // reset giá trị cho các mục 
        {
            txtMaMau.Text = "";
            txtMauSac.Text = "";
        }

        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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
                if (txtMaMau.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã Màu !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string insert = @"insert into MauSac (MaMau, TenMau) 
                                    values ('" + txtMaMau.Text + "',N'" + txtMauSac.Text + "')";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm mã màu:" + txtMaMau.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MauSac_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.MauSac_Info();
            grdMauSac.DataSource = bdsource;
        }
    }
}
