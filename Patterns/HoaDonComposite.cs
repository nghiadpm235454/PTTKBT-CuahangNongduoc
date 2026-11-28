using System;
using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Patterns
{
    // [Composite] Thành phần của hóa đơn: lá hoặc nhóm
    public interface IThanhPhanHoaDon
    {
        String Ten { get; }
        long TinhTien();
    }

    public class DongHang : IThanhPhanHoaDon
    {
        private ChiTietPhieuBan m_CT;
        public DongHang(ChiTietPhieuBan ct) { m_CT = ct; }
        public String Ten { get { return "Hàng hóa"; } }
        public long TinhTien() { return m_CT.ThanhTien; }
    }

    public class DongDichVu : IThanhPhanHoaDon
    {
        private DichVuPhieuBan m_DV;
        public DongDichVu(DichVuPhieuBan dv) { m_DV = dv; }
        public String Ten { get { return m_DV.TenDichVu; } }
        public long TinhTien() { return m_DV.SoTien; }
    }

    public class DongVanChuyen : IThanhPhanHoaDon
    {
        private long m_Phi;
        public DongVanChuyen(long phi) { m_Phi = phi; }
        public String Ten { get { return "Phí vận chuyển"; } }
        public long TinhTien() { return m_Phi; }
    }

    public class NhomHoaDon : IThanhPhanHoaDon
    {
        private String m_Ten;
        private IList<IThanhPhanHoaDon> m_Con = new List<IThanhPhanHoaDon>();

        public NhomHoaDon(String ten) { m_Ten = ten; }
        public String Ten { get { return m_Ten; } }

        public void Them(IThanhPhanHoaDon p) { m_Con.Add(p); }

        public long TinhTien()
        {
            long tong = 0;
            foreach (IThanhPhanHoaDon p in m_Con)
            {
                tong += p.TinhTien();
            }
            return tong;
        }
    }
}
