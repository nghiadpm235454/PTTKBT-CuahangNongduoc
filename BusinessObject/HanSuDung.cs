using System;

namespace CuahangNongduoc.BusinessObject
{
    // [REFACTOR #18 - Replace Data Value with Object]
    // TRUOC: MaSanPham giu 2 DateTime roi (NgaySanXuat, NgayHetHan), logic han dung rai rac.
    // SAU:   gom thanh doi tuong HanSuDung co hanh vi (DaHetHan, SoNgayConLai, SapHetHan).
    public class HanSuDung
    {
        private readonly DateTime m_NgaySanXuat;
        private readonly DateTime m_NgayHetHan;

        public HanSuDung(DateTime ngaySanXuat, DateTime ngayHetHan)
        {
            m_NgaySanXuat = ngaySanXuat;
            m_NgayHetHan = ngayHetHan;
        }

        public DateTime NgaySanXuat { get { return m_NgaySanXuat; } }
        public DateTime NgayHetHan { get { return m_NgayHetHan; } }

        public bool DaHetHan(DateTime ngayXet) { return ThoiGian.DaHetHan(NgayHetHan, ngayXet); }
        public int SoNgayConLai(DateTime ngayXet) { return ThoiGian.SoNgayConLai(NgayHetHan, ngayXet); }
        public bool SapHetHan(DateTime ngayXet, int soNgayCanhBao)
        {
            return !DaHetHan(ngayXet) && SoNgayConLai(ngayXet) <= soNgayCanhBao;
        }
    }
}
