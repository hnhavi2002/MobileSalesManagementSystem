//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MINHTUANMOBILE
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChiTietNhapKho
    {
        public string MaCTPN { get; set; }
        public string PhieuNhap { get; set; }
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string DungLuong { get; set; }
        public string MauSac { get; set; }
        public string HeDieuHanh { get; set; }
        public Nullable<double> GiaNhap { get; set; }
        public Nullable<int> SoLuongNhap { get; set; }
        public Nullable<double> ChietKhau { get; set; }
        public Nullable<double> ThanhTien { get; set; }
    
        public virtual SanPham SanPham { get; set; }
        public virtual NhapKho NhapKho { get; set; }
    }
}
