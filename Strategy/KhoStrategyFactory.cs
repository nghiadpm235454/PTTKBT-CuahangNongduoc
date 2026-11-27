using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Strategy
{
    /// <summary>
    /// CN-01: Abstract Factory - tao cac chien luoc kho theo cau hinh cua hang.
    /// Khi Mai xong CN-03/04: them IXuatKhoStrategy TaoXuatKho() vao interface va class ben duoi.
    /// </summary>
    public interface IKhoStrategyFactory
    {
        IGiaXuatStrategy TaoGiaXuat();
        // IXuatKhoStrategy TaoXuatKho();
    }

    public class KhoStrategyFactory : IKhoStrategyFactory
    {
        private readonly CuaHang m_CuaHang;

        public KhoStrategyFactory(CuaHang cuaHang)
        {
            m_CuaHang = cuaHang;
        }

        /// <summary>Doc cau hinh hien tai cua cua hang (THAM_SO).</summary>
        public static KhoStrategyFactory TuCauHinh()
        {
            return new KhoStrategyFactory(ThamSoManager.Instance.LayCuaHang());
        }

        public IGiaXuatStrategy TaoGiaXuat()
        {
            if (m_CuaHang.PhuongPhapGiaXuat == PhuongPhapGiaXuat.FIFO)
                return new GiaXuatFIFO();
            return new GiaXuatBQGQ();
        }
    }
}
