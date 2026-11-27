using System;
using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class XuatKhoController
    {
        /// UI goi khi bam "Them": tra ve danh sach dong (lo, so luong) can them vao phieu.
        public KetQuaPhanLo TaoDongXuat(string idSanPham, int soLuong, PhuongPhapXuatKho phuongPhap,
                                        string idLoChiDinh, DateTime ngayXuat, IDictionary<string, int> daGiu)
        {
            SanPham sp = new SanPhamController().LaySanPham(idSanPham);   // 1 lan cho tat ca lo -> co gia cho UI
            KetQuaPhanLo kq;

            if (phuongPhap == PhuongPhapXuatKho.ChiDinh)
            {
                MaSanPham lo = LoHangDao.LayLo(idLoChiDinh);
                if (lo != null && lo.SanPham.Id != idSanPham)
                {
                    kq = new KetQuaPhanLo();
                    kq.DatLoi(string.Format("Lô {0} không thuộc sản phẩm đã chọn.", idLoChiDinh));
                    return kq;
                }
                if (lo != null) lo.SanPham = sp;
                return XuatKhoService.XuatLoChiDinh(lo, soLuong, ngayXuat, daGiu);
            }

            IList<MaSanPham> cacLo = LoHangDao.LayCacLoConHang(idSanPham);
            foreach (MaSanPham lo in cacLo) lo.SanPham = sp;
            return XuatKhoService.PhanLoFIFO(cacLo, soLuong, ngayXuat, daGiu);
        }
    }
}
