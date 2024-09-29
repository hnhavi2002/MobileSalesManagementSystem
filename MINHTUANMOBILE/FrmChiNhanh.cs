using DevExpress.XtraGrid;
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
    public partial class FrmChiNhanh : DevExpress.XtraEditors.XtraForm
    {
        public FrmChiNhanh()
        {
            InitializeComponent();
        }
        ConnectDB data = new ConnectDB();
        private BindingSource bdsoure = new BindingSource();
        private DataTable DT = new DataTable();
        private void ChiNhanh_Load()
        {
            string str = "select * from ChiNhanh";
            SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            grdChiNhanh.DataSource = dt;
        }

        private void grdChiNhanh_Click(object sender, EventArgs e)
        {
            txtMaCN.DataBindings.Clear(); // xóa dữ liệu đang hiển thị trong textbox
            txtMaCN.DataBindings.Add("Text", grdChiNhanh.DataSource, "Mã chi nhánh"); // truyền dữ liệu từ gridcontrol lên textbox
            txtTenCN.DataBindings.Clear();
            txtTenCN.DataBindings.Add("Text", grdChiNhanh.DataSource, "Tên chi nhánh");
            txtDiaChi.DataBindings.Clear();
            txtDiaChi.DataBindings.Add("Text", grdChiNhanh.DataSource, "Địa chỉ");
            txtSDT.DataBindings.Clear();
            txtSDT.DataBindings.Add("Text", grdChiNhanh.DataSource, "Số điện thoại");
        }

        private void FrmChiNhanh_Load(object sender, EventArgs e)
        {
            bdsoure.DataSource = data.ChiNhanh_Info();
            grdChiNhanh.DataSource = bdsoure;    
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
                data.ExCuteNonQuery(@"delete from ChiNhanh where MaCN = '" + txtMaCN.Text + "';");
                DialogResult result = new DialogResult();
                result = MessageBox.Show("Bạn có muốn xóa: " + txtMaCN.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Xóa chi nhánh " + txtMaCN.Text+ "thành công!", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    ChiNhanh_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsoure.DataSource = data.ChiNhanh_Info();
            grdChiNhanh.DataSource = bdsoure;
      
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                data.ExCuteNonQuery(@"update ChiNhanh set MaCN='" + txtMaCN.Text + "',TenCN= N'" + txtTenCN.Text + "',DiaChi= N'" + txtDiaChi.Text + "',SDT= '" + txtSDT.Text + "'WHERE MaCN = '" + txtMaCN.Text + "';");
                DialogResult dg = new DialogResult();
                dg = MessageBox.Show("Bạn có muốn cập nhật " + txtMaCN.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dg == DialogResult.Yes)
                {
                    MessageBox.Show("Cập nhật chi nhánh " + txtMaCN.Text + "thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ChiNhanh_Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsoure.DataSource = data.ChiNhanh_Info();
            grdChiNhanh.DataSource = bdsoure;
          
        }
        private void ResetValues_Data() // reset giá trị cho các mục 
        {
            txtMaCN.Text = "";
            txtDiaChi.Text = "";
            txtTenCN.Text = "";
            txtSDT.Text = "";
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
                string insert_CV = @"insert into ChiNhanh (MaCN, TenCN, DiaChi,SDT) 
                                    values ('" + txtMaCN.Text + "',N'" + txtTenCN.Text + "',N'" + txtDiaChi.Text + "',N'" + txtSDT.Text + "')";
                data.ExCuteNonQuery(insert_CV);
                MessageBox.Show("Thêm mã CV:" + txtMaCN.Text + " thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChiNhanh_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bdsoure.DataSource = data.ChiNhanh_Info();
            grdChiNhanh.DataSource = bdsoure;
        }
    }
}
