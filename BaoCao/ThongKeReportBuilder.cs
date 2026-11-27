using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Reporting.WinForms;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    /// <summary>Sản phẩm của Builder: báo cáo thống kê hoàn chỉnh (tiêu đề, khoảng ngày, tiêu đề cột, các dòng).</summary>
    public class ThongKeReport
    {
        public const int SoCot = 6;
        public string TieuDe = "";
        public string KhoangNgay = "";
        public string[] TieuDeCot = new string[SoCot] { "", "", "", "", "", "" };
        public List<ThongKeRow> Dong = new List<ThongKeRow>();

        /// <summary>Tham số cho rptThongKe.rdlc. Cột nào để trống tiêu đề thì rdlc tự ẩn.</summary>
        public IList<ReportParameter> TaoThamSo()
        {
            IList<ReportParameter> ps = new List<ReportParameter>();
            ps.Add(new ReportParameter("tieu_de", TieuDe));
            ps.Add(new ReportParameter("khoang_ngay", KhoangNgay));
            for (int i = 0; i < SoCot; i++)
                ps.Add(new ReportParameter("h" + (i + 1), TieuDeCot[i] ?? ""));
            return ps;
        }
    }

    // ======================================================================================
    //  MẪU THIẾT KẾ: BUILDER   https://refactoring.guru/design-patterns/builder
    //  Phụ trách : P10 - Nguyên (UI-06)
    //  Dựng báo cáo thống kê từng bước: DatTieuDe -> DatKhoangNgay -> DatCot -> ThemDong... -> ThemDongTong -> Build.
    //  Mỗi LoaiThongKe (LoaiThongKe.cs) đóng vai Director: biết thứ tự gọi các bước cho loại báo cáo của mình.
    //  Nhờ vậy 5 báo cáo khác nhau dùng chung MỘT form và MỘT rdlc mà không phải copy code.
    // ======================================================================================
    public class ThongKeReportBuilder
    {
        private static readonly CultureInfo VN = new CultureInfo("vi-VN");
        private ThongKeReport m_Report = new ThongKeReport();

        public ThongKeReportBuilder DatTieuDe(string tieuDe)
        {
            m_Report.TieuDe = tieuDe;
            return this;
        }

        public ThongKeReportBuilder DatKhoangNgay(DateTime tu, DateTime den)
        {
            m_Report.KhoangNgay = "Từ ngày " + tu.ToString("dd/MM/yyyy") + " đến ngày " + den.ToString("dd/MM/yyyy");
            return this;
        }

        public ThongKeReportBuilder DatCot(params string[] tieuDeCot)
        {
            for (int i = 0; i < ThongKeReport.SoCot; i++)
                m_Report.TieuDeCot[i] = i < tieuDeCot.Length ? tieuDeCot[i] : "";
            return this;
        }

        public ThongKeReportBuilder ThemDong(params string[] giaTri)
        {
            ThongKeRow r = new ThongKeRow();
            if (giaTri.Length > 0) r.C1 = giaTri[0];
            if (giaTri.Length > 1) r.C2 = giaTri[1];
            if (giaTri.Length > 2) r.C3 = giaTri[2];
            if (giaTri.Length > 3) r.C4 = giaTri[3];
            if (giaTri.Length > 4) r.C5 = giaTri[4];
            if (giaTri.Length > 5) r.C6 = giaTri[5];
            m_Report.Dong.Add(r);
            return this;
        }

        /// <summary>Thêm dòng "Tổng cộng". cotNhan, cotTong tính từ 1.</summary>
        public ThongKeReportBuilder ThemDongTong(int cotNhan, int cotTong, decimal tong)
        {
            string[] v = new string[ThongKeReport.SoCot];
            for (int i = 0; i < v.Length; i++) v[i] = "";
            v[cotNhan - 1] = "Tổng cộng";
            v[cotTong - 1] = tong.ToString("N0", VN);
            return ThemDong(v);
        }

        public ThongKeReport Build()
        {
            ThongKeReport kq = m_Report;
            m_Report = new ThongKeReport();
            return kq;
        }
    }
}
