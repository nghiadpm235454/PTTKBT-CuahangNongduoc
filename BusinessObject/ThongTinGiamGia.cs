using System;

namespace CuahangNongduoc.BusinessObject
{
    // RF-25 Replace Record with Data Class
    // Trước: 3 cột rời LOAI_GIAM_GIA / TY_LE_GIAM_GIA / TIEN_GIAM_GIA đọc thẳng từ DataRow
    // Sau  : gom thành 1 lớp dữ liệu
    public class ThongTinGiamGia
    {
        private LoaiGiamGia m_Loai = LoaiGiamGia.Khong;
        public LoaiGiamGia Loai { get { return m_Loai; } set { m_Loai = value; } }

        private double m_TyLe;
        public double TyLe { get { return m_TyLe; } set { m_TyLe = value; } }

        private long m_SoTien;
        public long SoTien { get { return m_SoTien; } set { m_SoTien = value; } }

        private String m_QuyTac;
        public String QuyTac { get { return m_QuyTac; } set { m_QuyTac = value; } }
    }
}
