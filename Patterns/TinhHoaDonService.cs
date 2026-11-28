using System;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Patterns
{
    public static class TinhHoaDonService
    {
        // Giả định 5.000đ/km (chỉnh theo yêu cầu thầy)
        public const long DON_GIA_KM = 5000;

        public static long TinhTienHang(PhieuBan pb)
        {
            long tong = 0;
            if (pb.ChiTiet != null)
            {
                foreach (ChiTietPhieuBan ct in pb.ChiTiet) tong += ct.ThanhTien;
            }
            return tong;
        }

        public static long TinhTongDichVu(PhieuBan pb)
        {
            long tong = 0;
            foreach (DichVuPhieuBan dv in pb.DichVu) tong += dv.SoTien;
            return tong;
        }

        public static long TinhPhiVanChuyen(PhieuBan pb)
        {
            IPhiVanChuyen cachTinh = pb.GiaoHang ? (IPhiVanChuyen)new GiaoTheoKm(DON_GIA_KM)
                                                 : new KhongGiaoHang();
            return cachTinh.Tinh(pb.KhoangCachKm);
        }

        // Composite: hàng + dịch vụ + vận chuyển; Decorator: chiết khấu, khuyến mãi
        static IThanhPhanHoaDon DungHoaDonTruocGiam(PhieuBan pb)
        {
            NhomHoaDon nhom = new NhomHoaDon("Hóa đơn");
            if (pb.ChiTiet != null)
            {
                foreach (ChiTietPhieuBan ct in pb.ChiTiet) nhom.Them(new DongHang(ct));
            }
            foreach (DichVuPhieuBan dv in pb.DichVu) nhom.Them(new DongDichVu(dv));
            if (CoVanChuyen(pb)) nhom.Them(new DongVanChuyen(pb.PhiVanChuyen));

            IThanhPhanHoaDon hd = nhom;
            if (CoChietKhau(pb)) hd = new ChietKhauDecorator(hd, pb.ChietKhau);
            if (CoKhuyenMai(pb)) hd = new KhuyenMaiDecorator(hd, pb.TienKhuyenMai);
            return hd;
        }

        // RF-30 Decompose Conditional
        // Trước: if (pb.GiaoHang && pb.PhiVanChuyen > 0) {...}
        //        if (pb.ChietKhau > 0) {...}
        //        if (pb.TienKhuyenMai > 0) {...}   // điều kiện viết thẳng trong hàm dựng hóa đơn
        // Sau  : mỗi điều kiện tách thành hàm có tên rõ nghĩa
        static bool CoVanChuyen(PhieuBan pb) { return pb.GiaoHang && pb.PhiVanChuyen > 0; }
        static bool CoChietKhau(PhieuBan pb) { return pb.ChietKhau > 0; }
        static bool CoKhuyenMai(PhieuBan pb) { return pb.TienKhuyenMai > 0; }

        // Tính lại toàn bộ số liệu của phiếu
        public static void CapNhatTong(PhieuBan pb)
        {
            pb.TienHang = TinhTienHang(pb);
            pb.TongDichVu = TinhTongDichVu(pb);
            pb.PhiVanChuyen = TinhPhiVanChuyen(pb);

            IThanhPhanHoaDon truocGiam = DungHoaDonTruocGiam(pb);
            IThanhPhanHoaDon sauGiam = new GiamGiaDecorator(truocGiam, GiamGiaFactory.Tao(pb.GiamGia));

            pb.TongTien = sauGiam.TinhTien();
            pb.TienGiamGia = truocGiam.TinhTien() - pb.TongTien;
            pb.ConNo = pb.TongTien - pb.DaTra;
        }

        public static long TinhTongTien(PhieuBan pb)
        {
            CapNhatTong(pb);
            return pb.TongTien;
        }
    }
}
