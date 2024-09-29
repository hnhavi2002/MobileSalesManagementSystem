using DevExpress.XtraEditors;
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
    public partial class FrmDangNhap : DevExpress.XtraEditors.XtraForm
    {
        public FrmDangNhap()
        {
            InitializeComponent();
        }
        ConnectDB data = new ConnectDB();
        private string getID()
        {
            string id = "";
            try
            {
                string str = "select * from NhanVien where MaNV ='" + txtTenDN.Text + "'and MatKhau='" + txtMK.Text + "'";
                data.ExCuteNonQuery(str);
                SqlDataAdapter da = new SqlDataAdapter(str, data.GetConnection());
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        id = dr["MatKhau"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return id;
        }
        public static string ID_User = "";
        private void F_Logout(object sender, EventArgs e)
        {
            (sender as FormMain).isExit = false;
            (sender as FormMain).Close();
            this.Show();
        }

        private void FrmDangNhap_Load(object sender, EventArgs e)
        {
            txtMK.UseSystemPasswordChar = true;
        }

        private void chkMK_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMK.Checked)
            {
                txtMK.UseSystemPasswordChar = false;
            }
            if (!chkMK.Checked)
            {
                txtMK.UseSystemPasswordChar = true;
            }
        }

        private void btnDN_Click(object sender, EventArgs e)
        {
            ID_User = getID();
            if (ID_User != "")
            {
                MessageBox.Show("Đăng Nhập Thành Công!");
                openSplashForm();
                //MessageBox.Show("Đăng Nhập Thành Công!");
                //FormMain frm = new FormMain();
                FormMain.quyen = data.excuteQuery("select MaCV from NhanVien where MaNV ='" + txtTenDN.Text + "'and MatKhau='" + txtMK.Text + "'").Rows[0][0].ToString().Trim();
                //frm.Show();
                //frm.Logout += F_Logout;
                this.Hide();
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng");
            }
        }
        public void openSplashForm()
        {
            this.Hide();
            SplashForm splashForm = new SplashForm();

            splashForm.Show();
           
        }

        private void btnThoat_Click_1(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Bạn Có Chắc Chắn Muốn Thoát Khỏi Chương Trình?", "Xác Nhận Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Thoát khỏi ứng dụng
                Application.Exit();
            }
        }
    }
}