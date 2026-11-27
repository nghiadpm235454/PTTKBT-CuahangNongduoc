using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class ChiTietPhieuBan
    {
        private PhieuBan m_PhieuBan;

        public PhieuBan PhieuBan
        {
            get { return m_PhieuBan; }
            set { m_PhieuBan = value; }
        }
        private MaSanPham m_MaSP;

        public MaSanPham MaSanPham
        {
            get { return m_MaSP; }
            set { m_MaSP = value; }
        }
        private int m_SoLuong;

        public int SoLuong
        {
            get { return m_SoLuong; }
            set { m_SoLuong = value; }
        }
        private long m_DonGia;

        public long DonGia
        {
            get { return m_DonGia; }
            set { m_DonGia = value; }
        }
        private long m_ThanhTien;

        public long ThanhTien
        {
            get { return m_ThanhTien; }
            set { m_ThanhTien = value; }
        }

    
        // [FIX-09] Thuoc tinh PHANG cho ReportViewer: rdlc doc truc tiep, khong can truy cap doi tuong long nhau
        // (Fields!SanPham.Value.TenSanPham gay #Error khi bao cao chay trong sandbox / thieu CAS).
        public string MaLo { get { return MaSanPham == null ? "" : MaSanPham.Id; } }
        public string TenSanPham { get { return (MaSanPham == null || MaSanPham.SanPham == null) ? "" : MaSanPham.SanPham.TenSanPham; } }
}
}
