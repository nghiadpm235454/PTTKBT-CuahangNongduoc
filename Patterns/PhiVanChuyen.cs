using System;

namespace CuahangNongduoc.Patterns
{
    // RF-28 Replace Type Code with State/Strategy
    // Trước: if (pb.GiaoHang) phi = km * 5000; else phi = 0;   // cờ bool GiaoHang quyết định cách tính
    // Sau  : mỗi cách tính phí là 1 strategy
    public interface IPhiVanChuyen
    {
        long Tinh(double km);
    }

    public class KhongGiaoHang : IPhiVanChuyen
    {
        public long Tinh(double km) { return 0; }
    }

    public class GiaoTheoKm : IPhiVanChuyen
    {
        private long m_DonGiaKm;
        public GiaoTheoKm(long donGiaKm) { m_DonGiaKm = donGiaKm; }
        public long Tinh(double km)
        {
            return km <= 0 ? 0 : (long)Math.Round(km * m_DonGiaKm);
        }
    }
}
