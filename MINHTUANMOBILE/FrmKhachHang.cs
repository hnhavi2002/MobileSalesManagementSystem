using DevExpress.XtraBars.Commands.Internal;
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
    public partial class FrmKhachHang : DevExpress.XtraEditors.XtraForm
    {
        public FrmKhachHang()
        {
            InitializeComponent();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();

        private void KhachHang_Load()
        {
            string str = "select * from KhachHang";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdKhachHang.DataSource = dt;
        }
        private void FrmKhachHang_Load(object sender, EventArgs e)
        {
            bdsource.DataSource = data.KhachHang_Info();
            grdKhachHang.DataSource = bdsource;
        }

        private void grdKhachHang_Click(object sender, EventArgs e)
        {
            txtMaKH.DataBindings.Clear();
            txtMaKH.DataBindings.Add("Text", grdKhachHang.DataSource, "Mã khách hàng");
            txtTenKH.DataBindings.Clear();
            txtTenKH.DataBindings.Add("Text", grdKhachHang.DataSource, "Tên khách hàng");
            txtDiaChi.DataBindings.Clear();
            txtDiaChi.DataBindings.Add("Text", grdKhachHang.DataSource, "Địa chỉ");
            txtSDT.DataBindings.Clear();
            txtSDT.DataBindings.Add("Text", grdKhachHang.DataSource, "Số điện thoại");
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
                if (string.IsNullOrEmpty(txtMaKH.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Khách Hàng Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"delete from KhachHang where MaKH ='" + txtMaKH.Text + "';' ");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa? " + txtMaKH.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa thông tin khách hàng " + txtMaKH.Text + " thành công!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    KhachHang_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.KhachHang_Info();
            grdKhachHang.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaKH.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Khách Hàng Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update KhachHang set MaKH='" + txtMaKH.Text + "',TenKH= N'" + txtTenKH.Text + "',DiaChi= N'" + txtDiaChi.Text + "',SDT= '" + txtSDT.Text + "'WHERE MaKH = '" + txtMaKH.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaKH.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật khách hàng " + txtMaKH.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    KhachHang_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.KhachHang_Info();
            grdKhachHang.DataSource = bdsource;
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
            txtMaKH.Clear();
            txtTenKH.Clear();
            txtDiaChi.Clear();
            txtSDT.Clear();
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtMaKH.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã KH !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string insert = @"insert into KhachHang (MaKH, TenKH, DiaChi,SDT) 
                                    values ('" + txtMaKH.Text + "',N'" + txtTenKH.Text + "',N'" + txtDiaChi.Text + "',N'" + txtSDT.Text + "')";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm mã khách hàng:" + txtMaKH.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                KhachHang_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.KhachHang_Info();
            grdKhachHang.DataSource = bdsource;
        }
    }
}