using System;
using System.Collections.Generic;

namespace CuahangNongduoc.BusinessObject
{
    // [GoF Iterator] Duyet cac lo theo thu tu HSD gan nhat ma khong lo cach luu/sap xep.
    public interface ILoIterator
    {
        bool CoTiep();
        MaSanPham Tiep();
        void VeDau();
    }

    // Aggregate: tap cac lo cua mot san pham
    public class DanhSachLo
    {
        private readonly List<MaSanPham> m_Lo;

        public DanhSachLo(IList<MaSanPham> cacLo) { m_Lo = new List<MaSanPham>(cacLo); }

        public ILoIterator TaoIteratorTheoHSD(DateTime ngayXuat)
        {
            return new LoTheoHSDIterator(m_Lo, ngayXuat);
        }
    }

    // Concrete Iterator: bo lo het hang/het han, sap HSD tang dan (dong HSD thi lo nhap truoc)
    public class LoTheoHSDIterator : ILoIterator
    {
        private readonly List<MaSanPham> m_DaLoc = new List<MaSanPham>();
        private int m_ViTri;

        public LoTheoHSDIterator(IList<MaSanPham> cacLo, DateTime ngayXuat)
        {
            foreach (MaSanPham lo in cacLo)
            {
                if (lo.SoLuong > 0 && !lo.HanDung.DaHetHan(ngayXuat))
                    m_DaLoc.Add(lo);
            }
            m_DaLoc.Sort(delegate(MaSanPham a, MaSanPham b)
            {
                int c = a.NgayHetHan.CompareTo(b.NgayHetHan);
                if (c != 0) return c;
                c = a.NgayNhap.CompareTo(b.NgayNhap);
                if (c != 0) return c;
                return string.Compare(a.Id, b.Id, StringComparison.Ordinal);
            });
            m_ViTri = 0;
        }

        public bool CoTiep() { return m_ViTri < m_DaLoc.Count; }
        public MaSanPham Tiep() { return m_DaLoc[m_ViTri++]; }
        public void VeDau() { m_ViTri = 0; }
    }
}
