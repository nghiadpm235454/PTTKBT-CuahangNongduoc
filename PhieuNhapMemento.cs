using System;
using System.Collections.Generic;

namespace CuahangNongduoc
{
    public class DongNhapMemento
    {
        public string IdSanPham;
        public string MaLo;
        public decimal DonGiaNhap;
        public decimal SoLuong;
        public DateTime NgaySanXuat;
        public DateTime NgayHetHan;
    }

    public class PhieuNhapMemento
    {
        public string MaPhieu;
        public object IdNhaCungCap;
        public DateTime NgayNhap;
        public decimal DaTra;
        public List<DongNhapMemento> Dong = new List<DongNhapMemento>();
    }

    public class PhieuNhapCaretaker
    {
        private PhieuNhapMemento m_Memento;
        public bool CoNhap { get { return m_Memento != null; } }
        public void Luu(PhieuNhapMemento m) { m_Memento = m; }
        public PhieuNhapMemento Lay() { return m_Memento; }
    }
}