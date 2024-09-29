using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MINHTUANMOBILE
{
    public partial class FrmKhuyenMai : DevExpress.XtraEditors.XtraForm
    {
        public FrmKhuyenMai()
        {
            InitializeComponent();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        private void KM_Load()
        {
            string str = "select * from KhuyenMai";
            SqlDataAdapter adap = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            adap.Fill(dt);
           grdKhuyenMai.DataSource= dt;
        }
        private void FrmKhuyenMai_Load(object sender, EventArgs e)
        {
            bdsource.DataSource = data.KhuyenMai_Info();
            grdKhuyenMai.DataSource = bdsource;
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
                if (string.IsNullOrEmpty(txtMaKM.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Khuyến Mãi Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"delete from KhuyenMai where MaKM = '" + txtMaKM.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa: " + txtMaKM.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa mã khuyến mãi " + txtMaKM.Text + "thành công!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    KM_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.KhuyenMai_Info();
            grdKhuyenMai.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaKM.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Khuyến Mãi Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"update KhuyenMai set MaKM='" + txtMaKM.Text + "',TenKM= N'" + txtTenKM.Text + "',NgayBD= N'" + dateBD.Text + "',NgayKT= '" + dateKT.Text + "',Discount= '" + txtDiscount.Text + "'WHERE MaKM = '" + txtMaKM.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaKM.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật mã khuyến mãi " + txtMaKM.Text + "thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    KM_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.KhuyenMai_Info();
            grdKhuyenMai.DataSource = bdsource;
        }

        private void grdKhuyenMai_Click(object sender, EventArgs e)
        {
            txtMaKM.DataBindings.Clear();
            txtMaKM.DataBindings.Add("Text", grdKhuyenMai.DataSource, "Mã khuyến mãi");
            txtTenKM.DataBindings.Clear();
            txtTenKM.DataBindings.Add("Text", grdKhuyenMai.DataSource, "Tên khuyến mãi");
            dateBD.DataBindings.Clear();
            dateBD.DataBindings.Add("DateTime", grdKhuyenMai.DataSource, "Ngày bắt đầu");
            dateKT.DataBindings.Clear();
            dateKT.DataBindings.Add("DateTime", grdKhuyenMai.DataSource, "Ngày kết thúc");
            txtDiscount.DataBindings.Clear();
            txtDiscount.DataBindings.Add("Text", grdKhuyenMai.DataSource, "Discount");
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
            txtMaKM.Text = "";
            txtTenKM.Text = "";
            txtDiscount.Text = "";
            dateBD.Text = "";
            dateKT.Text = "";
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtMaKM.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã KM !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string insert = @"insert into KhuyenMai (MaKM, TenKM, NgayBD, NgayKT, Discount) 
                                    values ('" + txtMaKM.Text + "',N'" + txtTenKM.Text + "',N'" + dateBD.Text + "',N'" + dateKT.Text + "',N'" + txtDiscount.Text + "')";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm mã khuyến mãi:" + txtMaKM.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                KM_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.KhuyenMai_Info();
            grdKhuyenMai.DataSource = bdsource;
        }
    }
}
