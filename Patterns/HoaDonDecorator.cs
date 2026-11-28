using System;

namespace CuahangNongduoc.Patterns
{
    // [Decorator] Chồng chiết khấu / khuyến mãi / giảm giá lên tổng hóa đơn
    public abstract class HoaDonDecorator : IThanhPhanHoaDon
    {
        protected IThanhPhanHoaDon Inner;
        protected HoaDonDecorator(IThanhPhanHoaDon inner) { Inner = inner; }
        public abstract String Ten { get; }
        public abstract long TinhTien();
    }

    public class ChietKhauDecorator : HoaDonDecorator
    {
        private long m_SoTien;
        public ChietKhauDecorator(IThanhPhanHoaDon inner, long soTien) : base(inner) { m_SoTien = soTien; }
        public override String Ten { get { return Inner.Ten + " - chiết khấu"; } }
        public override long TinhTien() { return Math.Max(0, Inner.TinhTien() - m_SoTien); }
    }

    public class KhuyenMaiDecorator : HoaDonDecorator
    {
        private long m_SoTien;
        public KhuyenMaiDecorator(IThanhPhanHoaDon inner, long soTien) : base(inner) { m_SoTien = soTien; }
        public override String Ten { get { return Inner.Ten + " - khuyến mãi"; } }
        public override long TinhTien() { return Math.Max(0, Inner.TinhTien() - m_SoTien); }
    }

    public class GiamGiaDecorator : HoaDonDecorator
    {
        private GiamGia m_GiamGia;
        public GiamGiaDecorator(IThanhPhanHoaDon inner, GiamGia gg) : base(inner) { m_GiamGia = gg; }
        public override String Ten { get { return Inner.Ten + " - giảm giá"; } }
        public override long TinhTien()
        {
            long t = Inner.TinhTien();
            return Math.Max(0, t - m_GiamGia.TinhTienGiam(t));
        }
    }
}
