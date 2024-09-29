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
    public partial class FrmLoaiThanhToan : DevExpress.XtraEditors.XtraForm
    {
        public FrmLoaiThanhToan()
        {
            InitializeComponent();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        private void TT_Load()
        {
            string str = "select * from ThanhToan";
            SqlDataAdapter adap = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            adap.Fill(dt);
            grdTT.DataSource = dt;
        }
        private void grdLoaiTT_Click(object sender, EventArgs e)
        {
            txtMaTT.DataBindings.Clear();
            txtMaTT.DataBindings.Add("Text", grdTT.DataSource, "Mã thanh toán");
            txtTenTT.DataBindings.Clear();
            txtTenTT.DataBindings.Add("Text", grdTT.DataSource, "phương thức thanh toán");
        }

        private void FrmLoaiThanhToan_Load(object sender, EventArgs e)
        {
            bdsource.DataSource = data.ThanhToan_Info();
            grdTT.DataSource = bdsource;
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
                if (string.IsNullOrEmpty(txtMaTT.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Mã Thanh Toán Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"delete from ThanhToan where MaTT = '" + txtMaTT.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa: " + txtMaTT.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa mã thanh toán " + txtMaTT.Text + "thành công!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    TT_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.ThanhToan_Info();
            grdTT.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaTT.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Mã Thanh Toán Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update ThanhToan set MaTT='" + txtMaTT.Text + "',PhuongThucTT= N'" + txtTenTT.Text + "'WHERE MaTT = '" + txtMaTT.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaTT.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật mã thanh toán " + txtMaTT.Text + "thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TT_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.ThanhToan_Info();
            grdTT.DataSource = bdsource;
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
            txtMaTT.Text = "";
            txtTenTT.Text = "";
        }
        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtMaTT.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã TT !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string insert = @"insert into ThanhToan (MaTT, PhuongThucTT) 
                                    values ('" + txtMaTT.Text + "',N'" + txtTenTT.Text + "')";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm mã thanh toán:" + txtTenTT.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TT_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.ThanhToan_Info();
            grdTT.DataSource = bdsource;
        }
    }
}
