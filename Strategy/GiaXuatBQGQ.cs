using System;
using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Strategy
{
    /// <summary>Binh quan gia quyen: tong(SoLuong x GiaNhap) / tong SoLuong cac lo con hang.</summary>
    public class GiaXuatBQGQ : IGiaXuatStrategy
    {
        public long TinhGiaXuat(IList<MaSanPham> cacLoConHang)
        {
            long tongTien = 0;
            long tongSoLuong = 0;
            foreach (MaSanPham lo in cacLoConHang)
            {
                if (lo.SoLuong <= 0) continue;
                tongTien += (long)lo.SoLuong * lo.GiaNhap;
                tongSoLuong += lo.SoLuong;
            }
            if (tongSoLuong == 0) return 0;
            return (long)Math.Round((double)tongTien / tongSoLuong);
        }
    }
}
