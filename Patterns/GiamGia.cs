using System;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Patterns
{
    // RF-27 Replace Type Code with Subclasses
    // Trước: if (loai == "PHAN_TRAM") {...} else if (loai == "TIEN_MAT") {...} else if (loai == "QUY_TAC") {...}
    // Sau  : mỗi loại giảm giá là 1 lớp con
    public abstract class GiamGia
    {
        public abstract long TinhTienGiam(long tongTien);
    }

    public class GiamGiaPhanTram : GiamGia
    {
        private double m_TyLe;
        public GiamGiaPhanTram(double tyLe) { m_TyLe = tyLe; }
        public override long TinhTienGiam(long tongTien)
        {
            return (long)Math.Round(tongTien * m_TyLe / 100.0);
        }
    }

    public class GiamGiaTienMat : GiamGia
    {
        private long m_SoTien;
        public GiamGiaTienMat(long soTien) { m_SoTien = soTien; }
        public override long TinhTienGiam(long tongTien)
        {
            return Math.Min(m_SoTien, tongTien);
        }
    }

    public class GiamGiaTheoQuyTac : GiamGia
    {
        private IBieuThuc m_BieuThuc;
        public GiamGiaTheoQuyTac(String quyTac) { m_BieuThuc = TrinhPhanTichGiamGia.Parse(quyTac); }
        public override long TinhTienGiam(long tongTien)
        {
            return Math.Min(m_BieuThuc.TinhGiam(tongTien), tongTien);
        }
    }

    public static class GiamGiaFactory
    {
        public static GiamGia Tao(ThongTinGiamGia t)
        {
            if (t.Loai == LoaiGiamGia.PhanTram) return new GiamGiaPhanTram(t.TyLe);
            if (t.Loai == LoaiGiamGia.QuyTac) return new GiamGiaTheoQuyTac(t.QuyTac);

            // RF-29 Replace Subclass with Fields
            // Trước: return new GiamGiaKhong();   // lớp con chỉ khác nhau ở hằng số 0
            // Sau  : bỏ lớp GiamGiaKhong, dùng GiamGiaTienMat với số tiền = 0
            return new GiamGiaTienMat(t.Loai == LoaiGiamGia.TienMat ? t.SoTien : 0);
        }
    }
}
