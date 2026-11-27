using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Strategy
{
    /// <summary>Nhap truoc xuat truoc: lay gia nhap cua lo nhap som nhat con hang.</summary>
    public class GiaXuatFIFO : IGiaXuatStrategy
    {
        public long TinhGiaXuat(IList<MaSanPham> cacLoConHang)
        {
            MaSanPham somNhat = null;
            foreach (MaSanPham lo in cacLoConHang)
            {
                if (lo.SoLuong <= 0) continue;
                if (somNhat == null || lo.NgayNhap < somNhat.NgayNhap) somNhat = lo;
            }
            return somNhat == null ? 0 : somNhat.GiaNhap;
        }
    }
}
