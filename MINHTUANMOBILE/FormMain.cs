using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010;

namespace MINHTUANMOBILE
{
    public partial class FormMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Dictionary<Type, XtraForm> openForms = new Dictionary<Type, XtraForm>();

        public static string quyen;
        public bool isExit = true;
        public event EventHandler Logout;

        string UID = FrmDangNhap.ID_User;
        StatusBar stbar = new StatusBar();
        StatusBarPanel datetimePanel = new StatusBarPanel();
        public FormMain()
        {
            InitializeComponent();
            IsMdiContainer = true;
            btnDoiMK.ItemClick += btnDoiMK_ItemClick;
            btnCV.ItemClick += btnCV_ItemClick;
            btnTTKH.ItemClick += btnTTKH_ItemClick;
            btnNV.ItemClick += btnNV_ItemClick;
            btnHangHoa.ItemClick += btnHangHoa_ItemClick;
            btnLoaiHH.ItemClick += btnLoaiHH_ItemClick;
            btnCTKM.ItemClick += btnCTKM_ItemClick;
            btnPTTT.ItemClick += btnPTTT_ItemClick;
            btnTTNCC.ItemClick += btnTTNCC_ItemClick;
            btnDonHang.ItemClick += btnDonHang_ItemClick;
            btnNhapKho.ItemClick += btnNhapKho_ItemClick;
            btnXuatKho.ItemClick += btnXuatKho_ItemClick;
            btnTonKho.ItemClick += btnTonKho_ItemClick;
            btnDL.ItemClick += btnDL_ItemClick;
            btnMauSac.ItemClick += btnMauSac_ItemClick;
            btnHDH.ItemClick += btnHDH_ItemClick;
            btnDoanhThu.ItemClick += btnDoanhThu_ItemClick;
            btnDThuSP.ItemClick += btnDThuSP_ItemClick;
        }

       

        private void OpenForm(Type formType)
        {
            XtraForm formToShow = GetOrCreateForm(formType);

            if (formToShow != null)
            {
                // Nếu form chưa mở, thêm vào MdiChildren và hiển thị
                if (!formToShow.Visible)
                {
                    formToShow.MdiParent = this;
                    formToShow.Show();
                }
                else
                {
                    // Nếu form đã mở, đưa lên trên cùng
                    formToShow.Focus();
                }
            }
        }

        private XtraForm GetOrCreateForm(Type formType)
        {
            if (!openForms.ContainsKey(formType) || openForms[formType].IsDisposed)
            {
                // Nếu form chưa mở hoặc đã bị dispose, tạo mới
                XtraForm newForm = (XtraForm)Activator.CreateInstance(formType);
                openForms[formType] = newForm;
                newForm.FormClosed += (sender, e) => { openForms.Remove(formType); };

                // Đăng ký sự kiện để theo dõi khi form đóng
                return newForm;
            }
            else
            {
                // Nếu form đã mở, trả về form đó
                return openForms[formType];
            }
        }    
    
        private void btnCV_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmChucVu));
        }

        private void btnNV_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmNhanVien));
        }

        private void btnLoaiHH_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmLoaiSP));
        }

        private void btnHangHoa_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmSanPham));
        }

        private void btnDonHang_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmDonHang));
        }
        private void btnNhapKho_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmNhapKho));
        }

        private void btnXuatKho_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmXuatKho));
        }

        private void btnTonKho_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmTonKho));
        }

        private void btnChiNhanh_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmChiNhanh));
        }

        private void btnDoiMK_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmMatKhau));
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //Neu quyen bang voi ma chuc vu nao thi se duoc phan quyen tuong ung voi ma chuc vu do
            if (quyen == "CV1") //chức vụ admin
            {
                // có quyền truy cập vào tất cả chức năng
            }
            if (quyen == "CV2") // chức vụ thu ngân
            {
                btnChiNhanh.Enabled = false;
                btnNV.Enabled = false;
                btnCV.Enabled = false;
                btnHangHoa.Enabled = false;
                btnLoaiHH.Enabled = false;
                btnTTNCC.Enabled = false;
                btnNhapKho.Enabled = false;
                btnXuatKho.Enabled = false;
                btnTonKho.Enabled = false;
                btnHDH.Enabled = false;
                btnMauSac.Enabled = false;
                btnDL.Enabled = false;
                btnDoanhThu.Enabled = false;
                btnDThuSP.Enabled = false;
            }
            if (quyen == "CV3") // chức vụ bán hàng
            {
                btnNV.Enabled = false;
                btnCV.Enabled = false;
                btnNhapKho.Enabled = false;
                btnXuatKho.Enabled = false;
                btnTonKho.Enabled = false;
                btnDonHang.Enabled = false;
                btnThanhToan.Enabled = false;
                btnDoanhThu.Enabled = false;
                btnDThuSP.Enabled = false;
            }
            if (quyen=="CV4") // chức vụ kho
            {
                btnNV.Enabled = false;
                btnCV.Enabled = false;
                btnTTKH.Enabled = false;
                btnTTNCC.Enabled = false;
                btnCTKM.Enabled = false;
                btnThanhToan.Enabled = false;
                btnDonHang.Enabled = false;
                btnDoanhThu.Enabled = false;
                btnDThuSP.Enabled = false;
            }    
            datetimePanel.BorderStyle = StatusBarPanelBorderStyle.Raised;
            datetimePanel.Text = System.DateTime.Today.ToLongDateString();
            datetimePanel.AutoSize = StatusBarPanelAutoSize.Contents;
            stbar.Panels.Add(datetimePanel);
            stbar.ShowPanels = true;
            this.Controls.Add(stbar);
        }

        private void btnDX_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Kiểm tra điều kiện trước khi đóng Form Main
            if (MessageBox.Show("Bạn Có Chắc Chắn Muốn Đăng Xuất?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                FrmDangNhap formDangNhap = new FrmDangNhap();
                formDangNhap.Show();
            }
        }

        private void btnDL_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmDungLuong));
        }

        private void btnMauSac_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmMauSacSP));
        }

        private void btnHDH_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmHeDieuHanhSP));
        }

        private void btnCTKM_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmKhuyenMai));
        }

        private void btnPTTT_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmLoaiThanhToan));
        }

        private void btnTTKH_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmKhachHang));
        }

        private void btnTTNCC_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmNhaCungCap));
        }

        private void btnDoanhThu_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof(FrmDoanhThu));
        }

        private void btnDThuSP_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(typeof (FrmDoanhThuSP));
        }
    }
}