using System;
using System.Data;
using System.Globalization;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc
{
    /// <summary>Tham số lọc của báo cáo (Introduce Parameter Object).</summary>
    public class ThongKeFilter
    {
        public DateTime Tu;
        public DateTime Den;
        public int? IdNguoiDung;
    }

    /// <summary>Phía Implementor của Bridge: nơi nhận báo cáo đã dựng và xuất ra màn hình / máy in.</summary>
    public interface IThongKeOutput : IReportOutput
    {
        void DatBaoCao(ThongKeReport baoCao);   // nạp báo cáo đã dựng
        void In();                              // HienThiBaoCao() có sẵn từ IReportOutput
    }

    /// <summary>Định dạng giá trị DataRow, an toàn với DBNull (FIX-08).</summary>
    internal static class DinhDang
    {
        private static readonly CultureInfo VN = new CultureInfo("vi-VN");
        public static string Chu(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToString(o); }
        public static decimal So(object o) { return o == null || o == DBNull.Value ? 0m : Convert.ToDecimal(o); }
        public static string Tien(object o) { return So(o).ToString("N0", VN); }
        public static string Ngay(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToDateTime(o).ToString("dd/MM/yyyy"); }
        public static string Km(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToDecimal(o).ToString("0.##", VN); }
    }

    // ======================================================================================
    //  MẪU THIẾT KẾ: BRIDGE   https://refactoring.guru/design-patterns/bridge
    //  Phụ trách : P10 - Nguyên (UI-06)
    //  Abstraction = LoaiThongKe (tồn kho / vận chuyển / dịch vụ / giảm giá / khuyến mãi)
    //  Implementor = IThongKeOutput (xem trên màn hình / in)   -> frmBaoCaoThongKe cài đặt
    //  Hai chiều thay đổi độc lập: thêm loại thống kê mới không đụng đến cách xuất, và ngược lại.
    // ======================================================================================
    public abstract class LoaiThongKe
    {
        protected readonly IThongKeOutput output;
        protected readonly BaoCaoFactory factory = new BaoCaoFactory();

        protected LoaiThongKe(IThongKeOutput output)
        {
            this.output = output;
        }

        public abstract string Ten { get; }
        protected abstract ThongKeReport Tao(ThongKeFilter f);

        public void Xem(ThongKeFilter f)
        {
            output.DatBaoCao(Tao(f));
            output.HienThiBaoCao();
        }

        public void In(ThongKeFilter f)
        {
            output.DatBaoCao(Tao(f));
            output.In();
        }

        public override string ToString() { return Ten; }
    }

    public class ThongKeTonKho : LoaiThongKe
    {
        public ThongKeTonKho(IThongKeOutput o) : base(o) { }
        public override string Ten { get { return "Tồn kho"; } }

        protected override ThongKeReport Tao(ThongKeFilter f)
        {
            DataTable t = factory.TonKho(f.Tu, f.Den);
            ThongKeReportBuilder b = new ThongKeReportBuilder()
                .DatTieuDe("BÁO CÁO TỒN KHO").DatKhoangNgay(f.Tu, f.Den)
                .DatCot("Mã SP", "ĐVT", "Tên sản phẩm", "Bán trong kỳ", "Tồn cuối kỳ");
            decimal tongBan = 0, tongTon = 0;
            foreach (DataRow r in t.Rows)
            {
                tongBan += DinhDang.So(r["BAN_TRONG_KY"]);
                tongTon += DinhDang.So(r["TON_CUOI"]);
                b.ThemDong(DinhDang.Chu(r["ID"]), DinhDang.Chu(r["DVT"]), DinhDang.Chu(r["TEN_SAN_PHAM"]),
                    DinhDang.Tien(r["BAN_TRONG_KY"]), DinhDang.Tien(r["TON_CUOI"]));
            }
            return b.ThemDong("", "", "Tổng cộng", tongBan.ToString("N0", new CultureInfo("vi-VN")),
                tongTon.ToString("N0", new CultureInfo("vi-VN"))).Build();
        }
    }

    public class ThongKeVanChuyen : LoaiThongKe
    {
        public ThongKeVanChuyen(IThongKeOutput o) : base(o) { }
        public override string Ten { get { return "Chi phí vận chuyển"; } }

        protected override ThongKeReport Tao(ThongKeFilter f)
        {
            DataTable t = factory.VanChuyen(f.Tu, f.Den, f.IdNguoiDung);
            ThongKeReportBuilder b = new ThongKeReportBuilder()
                .DatTieuDe("BÁO CÁO CHI PHÍ VẬN CHUYỂN").DatKhoangNgay(f.Tu, f.Den)
                .DatCot("Mã phiếu", "Ngày bán", "Khách hàng", "Km", "Phí vận chuyển", "Nhân viên");
            decimal tong = 0;
            foreach (DataRow r in t.Rows)
            {
                tong += DinhDang.So(r["PHI_VAN_CHUYEN"]);
                b.ThemDong(DinhDang.Chu(r["ID"]), DinhDang.Ngay(r["NGAY_BAN"]), DinhDang.Chu(r["HO_TEN"]),
                    DinhDang.Km(r["KHOANG_CACH_KM"]), DinhDang.Tien(r["PHI_VAN_CHUYEN"]), DinhDang.Chu(r["HOTEN"]));
            }
            return b.ThemDongTong(1, 5, tong).Build();
        }
    }

    public class ThongKeDichVu : LoaiThongKe
    {
        public ThongKeDichVu(IThongKeOutput o) : base(o) { }
        public override string Ten { get { return "Dịch vụ phụ"; } }

        protected override ThongKeReport Tao(ThongKeFilter f)
        {
            DataTable t = factory.DichVu(f.Tu, f.Den, f.IdNguoiDung);
            ThongKeReportBuilder b = new ThongKeReportBuilder()
                .DatTieuDe("BÁO CÁO DỊCH VỤ PHỤ").DatKhoangNgay(f.Tu, f.Den)
                .DatCot("Mã phiếu", "Ngày bán", "Tên dịch vụ", "Số tiền");
            decimal tong = 0;
            foreach (DataRow r in t.Rows)
            {
                tong += DinhDang.So(r["SO_TIEN"]);
                b.ThemDong(DinhDang.Chu(r["ID"]), DinhDang.Ngay(r["NGAY_BAN"]),
                    DinhDang.Chu(r["TEN_DICH_VU"]), DinhDang.Tien(r["SO_TIEN"]));
            }
            return b.ThemDongTong(1, 4, tong).Build();
        }
    }

    /// <summary>Giảm giá / khuyến mãi / cả hai (dùng chung, khác nhau ở LocGiamGia).</summary>
    public class ThongKeGiamGia : LoaiThongKe
    {
        private readonly LocGiamGia m_Loc;
        private readonly string m_Ten;
        private readonly string m_TieuDe;

        public ThongKeGiamGia(IThongKeOutput o, LocGiamGia loc) : base(o)
        {
            m_Loc = loc;
            switch (loc)
            {
                case LocGiamGia.KhuyenMai: m_Ten = "Khuyến mãi"; m_TieuDe = "BÁO CÁO HÓA ĐƠN KHUYẾN MÃI"; break;
                case LocGiamGia.GiamGia: m_Ten = "Giảm giá"; m_TieuDe = "BÁO CÁO HÓA ĐƠN GIẢM GIÁ"; break;
                default: m_Ten = "Giảm giá / khuyến mãi"; m_TieuDe = "BÁO CÁO HÓA ĐƠN GIẢM GIÁ / KHUYẾN MÃI THEO NHÂN VIÊN"; break;
            }
        }

        public override string Ten { get { return m_Ten; } }

        protected override ThongKeReport Tao(ThongKeFilter f)
        {
            DataTable t = factory.GiamGia(f.Tu, f.Den, f.IdNguoiDung, m_Loc);
            ThongKeReportBuilder b = new ThongKeReportBuilder()
                .DatTieuDe(m_TieuDe).DatKhoangNgay(f.Tu, f.Den)
                .DatCot("Mã phiếu", "Ngày bán", "Khách hàng", "Tiền hàng", "Giảm giá", "Nhân viên");
            decimal tong = 0;
            foreach (DataRow r in t.Rows)
            {
                tong += DinhDang.So(r["TIEN_GIAM_GIA"]);
                string khach = DinhDang.Chu(r["HO_TEN"]);
                if (m_Loc == LocGiamGia.Ca) khach += " (" + DinhDang.Chu(r["LOAI"]) + ")";
                b.ThemDong(DinhDang.Chu(r["ID"]), DinhDang.Ngay(r["NGAY_BAN"]), khach,
                    DinhDang.Tien(r["TIEN_HANG"]), DinhDang.Tien(r["TIEN_GIAM_GIA"]), DinhDang.Chu(r["HOTEN"]));
            }
            return b.ThemDongTong(1, 5, tong).Build();
        }
    }
}
