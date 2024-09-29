using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MINHTUANMOBILE
{
    public partial class FrmMatKhau : DevExpress.XtraEditors.XtraForm
    { 
        public FrmMatKhau()
        {
            InitializeComponent();
        }
        SqlConnection connect = new SqlConnection (@"Data Source=HAVI\HAVISQLEXPRESS;Initial Catalog=MinhTuanMobile_DB;Integrated Security=True");

        private void FrmMatKhau_Load(object sender, EventArgs e)
        {

        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT COUNT(*) FROM NhanVien WHERE MaNV='" + txtTenDN.Text + "' AND MatKhau=N'" + txtMKCu.Text + "'", connect);
            DataTable dt = new DataTable();
            da.Fill(dt);
            errorProvider1.Clear();

            if (dt.Rows[0][0].ToString() == "1")
            {
                if (txtMKMoi.Text.Length > 5 && txtMKMoi.Text.Length <= 8 && !ContainsSpecialCharacters(txtMKMoi.Text))
                {
                    if (txtMKMoi.Text == txtMKMoi2.Text)
                    {
                        try
                        {
                            SqlDataAdapter da1 = new SqlDataAdapter("UPDATE NhanVien SET MatKhau=N'" + txtMKMoi.Text + "' WHERE MaNV=N'" + txtTenDN.Text + "' AND MatKhau=N'" + txtMKCu.Text + "'", connect);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);
                            MessageBox.Show("Đổi Mật Khẩu Thành Công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        errorProvider1.SetError(txtMKMoi2, "Mật Khẩu Nhập Lại Không Đúng");
                    }
                }
                else
                {
                    errorProvider1.SetError(txtMKMoi, "Độ Dài Mật Khẩu Bắt Buộc 6 Ký Tự Và Không Chứa Ký Tự Đặt Biệt !!!");
                }
            }
            else
            {
                errorProvider1.SetError(txtTenDN, "Tên Người Dùng Không Đúng !!!");
                errorProvider1.SetError(txtMKCu, "Mật Khẩu Cũ Không Đúng !!!");
            }
        }

        private bool ContainsSpecialCharacters(string input)
        {
            // Kiểm tra xem chuỗi có chứa ký tự đặc biệt hay không
            string pattern = @"[!@#$%^&*(),.?""':{}|<>]";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
    }
}
