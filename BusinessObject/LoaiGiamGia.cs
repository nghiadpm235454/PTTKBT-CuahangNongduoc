using System;

namespace CuahangNongduoc.BusinessObject
{
    // RF-26 Replace Type Code with Class
    // Trước: string LOAI_GIAM_GIA = "PHAN_TRAM" / "TIEN_MAT" ... rải khắp nơi, dễ gõ sai
    // Sau  : lớp LoaiGiamGia với các giá trị cố định
    public sealed class LoaiGiamGia
    {
        public static readonly LoaiGiamGia Khong = new LoaiGiamGia("KHONG");
        public static readonly LoaiGiamGia PhanTram = new LoaiGiamGia("PHAN_TRAM");
        public static readonly LoaiGiamGia TienMat = new LoaiGiamGia("TIEN_MAT");
        public static readonly LoaiGiamGia QuyTac = new LoaiGiamGia("QUY_TAC");

        private readonly string m_Ma;
        private LoaiGiamGia(string ma) { m_Ma = ma; }

        public string Ma { get { return m_Ma; } }

        public static LoaiGiamGia TuMa(string ma)
        {
            switch ((ma ?? "").Trim().ToUpper())
            {
                case "PHAN_TRAM": return PhanTram;
                case "TIEN_MAT": return TienMat;
                case "QUY_TAC": return QuyTac;
                default: return Khong;
            }
        }
    }
}
