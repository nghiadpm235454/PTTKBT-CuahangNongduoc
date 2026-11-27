using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Strategy
{
    /// <summary>CN-05: chien luoc tinh gia xuat (don gia von) cua mot san pham.</summary>
    public interface IGiaXuatStrategy
    {
        /// <param name="cacLoConHang">Cac lo con hang (SoLuong &gt; 0) cua san pham dang ban.</param>
        long TinhGiaXuat(IList<MaSanPham> cacLoConHang);
    }
}
