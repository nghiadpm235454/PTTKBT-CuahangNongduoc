<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class SoLuongTon
    {
        private SanPham  m_SanPham;

        public SanPham  SanPham
        {
            get { return m_SanPham; }
            set { m_SanPham = value; }
        }
        private int m_SoLuongTon;

        public int SoLuong
        {
            get { return m_SoLuongTon; }
            set { m_SoLuongTon = value; }
        }

	
    }
}
=======
using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class SoLuongTon
    {
        private SanPham  m_SanPham;

        public SanPham  SanPham
        {
            get { return m_SanPham; }
            set { m_SanPham = value; }
        }
        private int m_SoLuongTon;

        public int SoLuong
        {
            get { return m_SoLuongTon; }
            set { m_SoLuongTon = value; }
        }

	
    
        // [FIX-09] Thuoc tinh PHANG cho ReportViewer: rdlc doc truc tiep, khong can truy cap doi tuong long nhau
        // (Fields!SanPham.Value.TenSanPham gay #Error khi bao cao chay trong sandbox / thieu CAS).
        public string MaSP { get { return m_SanPham == null ? "" : m_SanPham.Id; } }
        public string TenSanPham { get { return m_SanPham == null ? "" : m_SanPham.TenSanPham; } }
        public long DonGiaNhap { get { return m_SanPham == null ? 0 : m_SanPham.DonGiaNhap; } }
        public long GiaBanLe { get { return m_SanPham == null ? 0 : m_SanPham.GiaBanLe; } }
        public long GiaBanSi { get { return m_SanPham == null ? 0 : m_SanPham.GiaBanSi; } }
        public string TenDonVi { get { return (m_SanPham == null || m_SanPham.DonViTinh == null) ? "" : m_SanPham.DonViTinh.Ten; } }
}
}
>>>>>>> master
