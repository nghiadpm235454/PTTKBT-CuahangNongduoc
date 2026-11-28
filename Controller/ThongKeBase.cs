using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    /// <summary>Lớp cha của mọi báo cáo thống kê (P09).</summary>
    public abstract class ThongKeBase
    {
        protected DateTime m_TuNgay;
        protected DateTime m_DenNgay;
        protected ThongKeFactory m_Factory = new ThongKeFactory();

        protected ThongKeBase(DateTime tuNgay, DateTime denNgay)
        {
            m_TuNgay = tuNgay.Date;
            m_DenNgay = denNgay.Date;
            KiemTraKhoangNgay();
        }

        protected void KiemTraKhoangNgay()
        {
            if (m_TuNgay > m_DenNgay)
                throw new ThongKeException("'Từ ngày' phải nhỏ hơn hoặc bằng 'Đến ngày'.");
        }

        public DateTime TuNgay { get { return m_TuNgay; } }
        public DateTime DenNgay { get { return m_DenNgay; } }

        /// <summary>Tiêu đề hiển thị trên báo cáo.</summary>
        public abstract string TieuDe { get; }

        /// <summary>Lấy dữ liệu thống kê từ SQL Server.</summary>
        public abstract DataTable Lay();
    }

    // ------------------------------------------------------------------ tồn kho
    public class ThongKeTonKho : ThongKeBase
    {
        public ThongKeTonKho() : base(DateTime.Today, DateTime.Today) { }
        public override string TieuDe { get { return "BÁO CÁO TỒN KHO"; } }
        public override DataTable Lay() { return m_Factory.TonKho(); }
    }

    // -------------------------------------------------------------- vận chuyển
    public class ThongKeVanChuyen : ThongKeBase
    {
        public ThongKeVanChuyen(DateTime tu, DateTime den) : base(tu, den) { }
        public override string TieuDe { get { return "THỐNG KÊ CHI PHÍ VẬN CHUYỂN"; } }
        public override DataTable Lay() { return m_Factory.ChiPhiVanChuyen(m_TuNgay, m_DenNgay); }
    }

    // ------------------------------------------------------------- dịch vụ phụ
    public class ThongKeDichVu : ThongKeBase
    {
        public ThongKeDichVu(DateTime tu, DateTime den) : base(tu, den) { }
        public override string TieuDe { get { return "THỐNG KÊ DỊCH VỤ PHỤ"; } }
        public override DataTable Lay() { return m_Factory.DichVuPhu(m_TuNgay, m_DenNgay); }
    }

    // ------------------------------------------------- giảm giá / khuyến mãi
    public class ThongKeGiamGia : ThongKeBase
    {
        private string m_Loai;   // null | "GIAM_GIA" | "KHUYEN_MAI"

        public ThongKeGiamGia(DateTime tu, DateTime den, string loai) : base(tu, den)
        {
            m_Loai = loai;
        }

        public override string TieuDe
        {
            get
            {
                if (m_Loai == ThongKeController.LOAI_KHUYEN_MAI) return "THỐNG KÊ KHUYẾN MÃI";
                if (m_Loai == ThongKeController.LOAI_GIAM_GIA) return "THỐNG KÊ GIẢM GIÁ";
                return "THỐNG KÊ GIẢM GIÁ VÀ KHUYẾN MÃI";
            }
        }
        public override DataTable Lay() { return m_Factory.GiamGia(m_TuNgay, m_DenNgay, m_Loai); }
    }

    // ----------------------------------------- giảm giá / khuyến mãi theo nhân viên
    public class ThongKeGiamGiaTheoNhanVien : ThongKeBase
    {
        private int? m_IdNguoiDung;   // null = mọi nhân viên

        public ThongKeGiamGiaTheoNhanVien(DateTime tu, DateTime den, int? idNguoiDung) : base(tu, den)
        {
            m_IdNguoiDung = idNguoiDung;
        }

        public override string TieuDe
        {
            get { return "THỐNG KÊ GIẢM GIÁ / KHUYẾN MÃI THEO NHÂN VIÊN"; }
        }

        public override DataTable Lay()
        {
            return m_Factory.GiamGiaTheoNhanVien(m_TuNgay, m_DenNgay, m_IdNguoiDung);
        }

        /// <summary>Gộp tổng giảm giá theo từng nhân viên: NHAN_VIEN | SO_PHIEU | TONG_GIAM</summary>
        public DataTable TongTheoNhanVien()
        {
            DataTable chiTiet = Lay();

            DataTable kq = new DataTable("TongTheoNhanVien");
            kq.Columns.Add("NHAN_VIEN", typeof(string));
            kq.Columns.Add("SO_PHIEU", typeof(int));
            kq.Columns.Add("TONG_GIAM", typeof(decimal));

            Dictionary<string, DataRow> theoNv = new Dictionary<string, DataRow>();
            foreach (DataRow r in chiTiet.Rows)
            {
                string nv = Convert.ToString(r["NHAN_VIEN"]);
                DataRow dong;
                if (!theoNv.TryGetValue(nv, out dong))
                {
                    dong = kq.NewRow();
                    dong["NHAN_VIEN"] = nv;
                    dong["SO_PHIEU"] = 0;
                    dong["TONG_GIAM"] = 0m;
                    kq.Rows.Add(dong);
                    theoNv.Add(nv, dong);
                }
                dong["SO_PHIEU"] = (int)dong["SO_PHIEU"] + 1;
                dong["TONG_GIAM"] = (decimal)dong["TONG_GIAM"] + Convert.ToDecimal(r["TIEN_GIAM_GIA"]);
            }
            return kq;
        }
    }
}
