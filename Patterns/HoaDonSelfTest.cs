using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Patterns
{
    // QA-01: kiểm tra nhanh logic hóa đơn (Composite + Decorator + Interpreter)
    public static class HoaDonSelfTest
    {
        static int m_Dung;
        static int m_Sai;
        static StringBuilder m_Log;

        public static String ChayTatCa()
        {
            m_Dung = 0;
            m_Sai = 0;
            m_Log = new StringBuilder();

            PhieuBan pb = TaoPhieu(100000, 200000);
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC01 chỉ hàng hóa", 300000, pb.TongTien);

            pb = TaoPhieu(100000, 200000);
            pb.DichVu.Add(new DichVuPhieuBan("Bốc xếp", 20000));
            pb.GiaoHang = true;
            pb.KhoangCachKm = 10;
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC02 phí vận chuyển", 50000, pb.PhiVanChuyen);
            Kiem("TC02 tổng dịch vụ", 20000, pb.TongDichVu);
            Kiem("TC02 tổng tiền", 370000, pb.TongTien);

            pb.ChietKhau = 10000;
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC03 chiết khấu", 360000, pb.TongTien);

            pb.GiamGia.Loai = LoaiGiamGia.PhanTram;
            pb.GiamGia.TyLe = 10;
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC04 giảm 10%", 324000, pb.TongTien);
            Kiem("TC04 tiền giảm", 36000, pb.TienGiamGia);

            pb.DaTra = 100000;
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC05 còn nợ", 224000, pb.ConNo);

            pb.GiamGia = new ThongTinGiamGia();
            pb.TienKhuyenMai = 5000;
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC06 khuyến mãi", 355000, pb.TongTien);

            pb = TaoPhieu(100000);
            pb.GiamGia.Loai = LoaiGiamGia.TienMat;
            pb.GiamGia.SoTien = 999999999;
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC07 giảm quá tổng", 0, pb.TongTien);

            pb = TaoPhieu(1500000);
            pb.GiamGia.Loai = LoaiGiamGia.QuyTac;
            pb.GiamGia.QuyTac = "10% nếu > 1.000.000";
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC08 quy tắc đủ điều kiện", 1350000, pb.TongTien);

            pb = TaoPhieu(300000);
            pb.GiamGia.Loai = LoaiGiamGia.QuyTac;
            pb.GiamGia.QuyTac = "10% nếu > 1.000.000";
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC09 quy tắc không đủ", 300000, pb.TongTien);

            pb = TaoPhieu(6000000);
            pb.GiamGia.Loai = LoaiGiamGia.QuyTac;
            pb.GiamGia.QuyTac = "500000 nếu >= 5000000";
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC10 quy tắc tiền cố định", 5500000, pb.TongTien);

            pb = TaoPhieu(100000);
            pb.GiaoHang = true;
            pb.KhoangCachKm = 0;
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC11 giao hàng 0km", 0, pb.PhiVanChuyen);

            pb = TaoPhieu(100000);
            pb.GiaoHang = false;
            pb.KhoangCachKm = 30;
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC12 không giao hàng", 0, pb.PhiVanChuyen);

            pb = TaoPhieu(100000);
            pb.GiamGia.Loai = LoaiGiamGia.QuyTac;
            pb.GiamGia.QuyTac = "";
            TinhHoaDonService.CapNhatTong(pb);
            Kiem("TC13 quy tắc rỗng", 100000, pb.TongTien);

            m_Log.AppendLine();
            m_Log.AppendLine("ĐÚNG: " + m_Dung + "   SAI: " + m_Sai);
            return m_Log.ToString();
        }

        static PhieuBan TaoPhieu(params long[] thanhTien)
        {
            PhieuBan pb = new PhieuBan();
            pb.ChiTiet = new List<ChiTietPhieuBan>();
            foreach (long t in thanhTien)
            {
                ChiTietPhieuBan ct = new ChiTietPhieuBan();
                ct.SoLuong = 1;
                ct.DonGia = t;
                ct.ThanhTien = t;
                pb.ChiTiet.Add(ct);
            }
            return pb;
        }

        static void Kiem(String ten, long ky_vong, long thuc_te)
        {
            if (ky_vong == thuc_te)
            {
                m_Dung++;
                m_Log.AppendLine("[OK]  " + ten);
            }
            else
            {
                m_Sai++;
                m_Log.AppendLine("[SAI] " + ten + " | kỳ vọng " + ky_vong + " - thực tế " + thuc_te);
            }
        }
    }
}
