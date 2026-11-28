using System;

namespace CuahangNongduoc.BusinessObject
{
    public class DichVuPhieuBan
    {
        public DichVuPhieuBan() { }
        public DichVuPhieuBan(String ten, long soTien)
        {
            m_TenDichVu = ten;
            m_SoTien = soTien;
        }

        private String m_TenDichVu;
        public String TenDichVu { get { return m_TenDichVu; } set { m_TenDichVu = value; } }

        private long m_SoTien;
        public long SoTien { get { return m_SoTien; } set { m_SoTien = value; } }
    }
}
