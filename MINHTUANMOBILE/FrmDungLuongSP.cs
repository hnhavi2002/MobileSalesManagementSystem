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
    public partial class FrmDungLuong : DevExpress.XtraEditors.XtraForm
    {
        public FrmDungLuong()
        {
            InitializeComponent();
        }

       
        ConnectDB data = new ConnectDB();
        private BindingSource bdsource = new BindingSource();
        //private BindingSource bdsource2 = new BindingSource();
        
        private void DungLuong_Load()
        {
            string str = "Select * from DungLuong";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdDungLuong.DataSource = dt;
        }
        private void FrmDungLuong_Load(object sender, EventArgs e)
        {
            bdsource.DataSource = data.DungLuong_Info();
            grdDungLuong.DataSource = bdsource;

        }

        private void grdDungLuong_Click(object sender, EventArgs e)
        {
            txtMaDL.DataBindings.Clear();
            txtMaDL.DataBindings.Add("Text", grdDungLuong.DataSource, "Mã dung lượng");
            txtDL.DataBindings.Clear();
            txtDL.DataBindings.Add("Text", grdDungLuong.DataSource, "Loại dung lượng");
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
                if (string.IsNullOrEmpty(txtDL.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Mã Dung Lượng Để Xóa !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                data.ExCuteNonQuery(@"delete from DungLuong where MaDL = '" + txtMaDL.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa: " + txtMaDL.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa Mã Dung Lượng " + txtMaDL.Text + "Thành Công !", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    DungLuong_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.DungLuong_Info();
            grdDungLuong.DataSource = bdsource;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDL.Text))
                {
                    MessageBox.Show("Vui Lòng Chọn Mã Dung Lượng Để Cập Nhật !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                data.ExCuteNonQuery(@"update DungLuong set MaDL='" + txtMaDL.Text + "',LoaiDL= N'" + txtDL.Text + "'WHERE MaDL = '" + txtMaDL.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaDL.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập Nhật Dung Lượng" + txtMaDL.Text + "Thành Công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DungLuong_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.DungLuong_Info();
            grdDungLuong.DataSource = bdsource;
        }
        private void ResetValues_Data() // reset giá trị cho các mục 
        {
            txtDL.Text = "";
            txtMaDL.Text = "";
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
                if (txtDL.Text == "")
                {
                    MessageBox.Show("Vui Lòng Nhập Mã DL !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string insert = @"insert into DungLuong (MaDL, LoaiDL) 
                                    values ('" + txtMaDL.Text + "',N'" + txtDL.Text + "')";
                data.ExCuteNonQuery(insert);
                MessageBox.Show("Thêm Dung Lượng:" + txtMaDL.Text + " Thành Công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DungLuong_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsource.DataSource = data.DungLuong_Info();
            grdDungLuong.DataSource = bdsource;
        }
    }
}
