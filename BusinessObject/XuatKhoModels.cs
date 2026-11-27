using System;
using System.Collections.Generic;

namespace CuahangNongduoc.BusinessObject
{
    // Neu Nghia (P04) da co enum cung ten thi xoa enum nay va dung cua Nghia
    public enum PhuongPhapXuatKho { FIFO = 0, ChiDinh = 1 }

    /// Mot dong xuat: lay bao nhieu tu lo nao
    public class DongXuat
    {
        private readonly MaSanPham m_Lo;
        private readonly int m_SoLuong;

        public DongXuat(MaSanPham lo, int soLuong) { m_Lo = lo; m_SoLuong = soLuong; }

        public MaSanPham Lo { get { return m_Lo; } }
        public int SoLuong { get { return m_SoLuong; } }
        public string SoLo { get { return m_Lo.Id; } }
        public DateTime NgayHetHan { get { return m_Lo.NgayHetHan; } }
    }

    /// Ket qua phan lo: khong dung exception cho loi nghiep vu (thieu hang, lo het han...)
    public class KetQuaPhanLo
    {
        private readonly List<DongXuat> m_Dong = new List<DongXuat>();
        private int m_ConThieu;
        private string m_ThongBaoLoi;

        public IList<DongXuat> Dong { get { return m_Dong; } }
        public int ConThieu { get { return m_ConThieu; } set { m_ConThieu = value; } }
        public string ThongBaoLoi { get { return m_ThongBaoLoi; } }
        public bool ThanhCong { get { return m_ThongBaoLoi == null; } }

        public void Them(DongXuat d) { m_Dong.Add(d); }
        public void DatLoi(string s) { m_ThongBaoLoi = s; }
    }

    public class TonKhoKhongDuException : Exception
    {
        public TonKhoKhongDuException(string idLo, int delta)
            : base(string.Format("Lô {0}: không đủ tồn để xuất {1} (hoặc lô không tồn tại).", idLo, -delta)) { }
    }
}
