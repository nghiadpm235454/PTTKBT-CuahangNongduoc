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
        private long m_GiaXuat;

        public long GiaXuat
        {
            get { return m_GiaXuat; }
            set { m_GiaXuat = value; }
        }

        // [REFACTOR #14 - Hide Delegate] hoa don/bao cao goi ct.SoLo thay vi ct.MaSanPham.Id
        public String SoLo { get { return MaSanPham.Id; } }
        public DateTime NgayHetHan { get { return MaSanPham.NgayHetHan; } }
        public String TenSanPham { get { return MaSanPham.TenSanPham; } }

    }
}
