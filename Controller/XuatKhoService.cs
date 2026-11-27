using System;
using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Controller
{
    /// Logic thuan (khong dung DB) -> test duoc ma khong can SQL Server.
    public static class XuatKhoService
    {
        // daGiu: so luong moi lo da nam tren phieu dang lap nhung CHUA luu (lo -> SL). Co the null.
        private static int ConLai(MaSanPham lo, IDictionary<string, int> daGiu)
        {
            int giu;
            if (daGiu == null || !daGiu.TryGetValue(lo.Id, out giu)) giu = 0;
            return lo.SoLuong - giu;
        }

        /// CN-03: tu dong lay tu lo HSD gan nhat, thieu thi lay tiep lo ke
        public static KetQuaPhanLo PhanLoFIFO(IList<MaSanPham> cacLo, int soLuongCan,
                                              DateTime ngayXuat, IDictionary<string, int> daGiu)
        {
            KetQuaPhanLo kq = new KetQuaPhanLo();
            if (soLuongCan <= 0) { kq.DatLoi("Số lượng xuất phải lớn hơn 0."); return kq; }

            int conCan = soLuongCan;
            ILoIterator it = new DanhSachLo(cacLo).TaoIteratorTheoHSD(ngayXuat);
            while (conCan > 0 && it.CoTiep())
            {
                MaSanPham lo = it.Tiep();
                int lay = Math.Min(ConLai(lo, daGiu), conCan);
                if (lay <= 0) continue;
                kq.Them(new DongXuat(lo, lay));
                conCan -= lay;
            }
            kq.ConThieu = conCan;
            if (conCan > 0)
                kq.DatLoi(string.Format("Không đủ hàng còn hạn: còn thiếu {0}.", conCan));
            return kq;
        }

        /// CN-04: xuat dung lo nguoi dung chon; tu choi neu lo het han hoac khong du
        public static KetQuaPhanLo XuatLoChiDinh(MaSanPham lo, int soLuongCan,
                                                 DateTime ngayXuat, IDictionary<string, int> daGiu)
        {
            KetQuaPhanLo kq = new KetQuaPhanLo();
            if (soLuongCan <= 0)
                kq.DatLoi("Số lượng xuất phải lớn hơn 0.");
            else if (lo == null)
                kq.DatLoi("Không tìm thấy lô đã chọn.");
            else if (lo.HanDung.DaHetHan(ngayXuat))
                kq.DatLoi(string.Format("Lô {0} đã hết hạn ngày {1}, không được xuất.", lo.Id, ThoiGian.DinhDang(lo.NgayHetHan)));
            else if (ConLai(lo, daGiu) < soLuongCan)
                kq.DatLoi(string.Format("Lô {0} chỉ còn {1} (đã trừ phần đang có trên phiếu).", lo.Id, ConLai(lo, daGiu)));
            else
                kq.Them(new DongXuat(lo, soLuongCan));
            return kq;
        }
    }
}
