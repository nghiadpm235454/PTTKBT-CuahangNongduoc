using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class MaSanPham
    {
        public MaSanPham() { }
        public MaSanPham(String id, int so_luong)
        {
            // [REFACTOR #17 - Self Encapsulate Field]
            // TRUOC: m_Id = id;  m_SoLuong = so_luong;
            // SAU:   dung property de moi cho ghi/doc di qua mot cua
            Id = id;
            SoLuong = so_luong;
        }

        private String m_Id;

        public String Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        private SanPham m_SanPham;

        public SanPham SanPham
        {
            get { return m_SanPham; }
            set { m_SanPham = value; }
        }
        private long m_GiaNhap;

        public long GiaNhap
        {
            get { return m_GiaNhap; }
            set { m_GiaNhap = value; }
        }
        private int m_SoLuong;

        public int SoLuong
        {
            get { return m_SoLuong; }
            set { m_SoLuong = value; }
        }

        private long m_ThanhTien;

        public long ThanhTien
        {
            get { return m_ThanhTien; }
            set { m_ThanhTien = value; }
        }

        private DateTime m_NgayNhap;

        public DateTime NgayNhap
        {
            get { return m_NgayNhap; }
            set { m_NgayNhap = value; }
        }

        // [REFACTOR #18 - Replace Data Value with Object]
        // TRUOC: private DateTime m_NgaySanXuat; private DateTime m_NgayHetHan;
        // SAU:   mot doi tuong HanSuDung; hai property cu giu lai de rdlc/controller khong vo.
        private HanSuDung m_HanDung = new HanSuDung(DateTime.MinValue, DateTime.MinValue);

        public HanSuDung HanDung
        {
            get { return m_HanDung; }
            set { m_HanDung = value; }
        }

        public DateTime NgaySanXuat
        {
            get { return HanDung.NgaySanXuat; }
            set { HanDung = new HanSuDung(value, HanDung.NgayHetHan); }
        }

        public DateTime NgayHetHan
        {
            get { return HanDung.NgayHetHan; }
            set { HanDung = new HanSuDung(HanDung.NgaySanXuat, value); }
        }

        private PhieuNhap m_PhieuNhap;

        public PhieuNhap PhieuNhap
        {
            get { return m_PhieuNhap; }
            set { m_PhieuNhap = value; }
        }

        // [REFACTOR #14 - Hide Delegate]
        // TRUOC: form goi masp.SanPham.GiaBanLe (lo cau truc MaSanPham -> SanPham)
        // SAU:   masp.GiaBanLe
        public String TenSanPham { get { return SanPham.TenSanPham; } }
        public long GiaBanLe { get { return SanPham.GiaBanLe; } }
        public long GiaBanSi { get { return SanPham.GiaBanSi; } }
        public long GiaBQGQ { get { return SanPham.DonGiaNhap; } }
    }
}
