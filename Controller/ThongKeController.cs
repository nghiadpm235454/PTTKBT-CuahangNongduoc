using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// P09 - Cổng vào của thống kê. Form của Nguyên (P10) chỉ cần gọi:
    ///     ThongKeBase tk = ThongKeController.Tao(ThongKeController.LOAI_VAN_CHUYEN, tu, den);
    ///     dgv.DataSource = tk.Lay();
    /// </summary>
    public class ThongKeController
    {
        public const string LOAI_TON_KHO = "TONKHO";
        public const string LOAI_VAN_CHUYEN = "VANCHUYEN";
        public const string LOAI_DICH_VU = "DICHVU";
        public const string LOAI_GIAM_GIA_VA_KM = "GIAMGIA_KM";       // cả giảm giá lẫn khuyến mãi
        public const string LOAI_GIAM_GIA = "GIAM_GIA";               // = giá trị cột PHIEU_BAN.LOAI_GIAM_GIA
        public const string LOAI_KHUYEN_MAI = "KHUYEN_MAI";           // = giá trị cột PHIEU_BAN.LOAI_GIAM_GIA

        /// <summary>Tạo đối tượng thống kê theo loại (Form chỉ cần gọi hàm này).</summary>
        public static ThongKeBase Tao(string loai, DateTime tuNgay, DateTime denNgay)
        {
            switch (loai)
            {
                case LOAI_TON_KHO: return new ThongKeTonKho();
                case LOAI_VAN_CHUYEN: return new ThongKeVanChuyen(tuNgay, denNgay);
                case LOAI_DICH_VU: return new ThongKeDichVu(tuNgay, denNgay);
                case LOAI_GIAM_GIA_VA_KM: return new ThongKeGiamGia(tuNgay, denNgay, null);
                case LOAI_GIAM_GIA: return new ThongKeGiamGia(tuNgay, denNgay, LOAI_GIAM_GIA);
                case LOAI_KHUYEN_MAI: return new ThongKeGiamGia(tuNgay, denNgay, LOAI_KHUYEN_MAI);
                default:
                    throw new ThongKeException("Loại thống kê không hợp lệ: " + loai);
            }
        }

        /// <summary>
        /// CN-08: giảm giá/khuyến mãi theo nhân viên.
        /// idNguoiDung = null: mọi nhân viên; nếu truyền id (nhân viên đang đăng nhập) thì chỉ của người đó.
        /// </summary>
        public static ThongKeGiamGiaTheoNhanVien TaoTheoNhanVien(DateTime tuNgay, DateTime denNgay, int? idNguoiDung)
        {
            return new ThongKeGiamGiaTheoNhanVien(tuNgay, denNgay, idNguoiDung);
        }

        /// <summary>
        /// CN-08 + Visitor: tính tổng THU / CHI trên nhiều loại chứng từ
        /// (phiếu bán, thanh toán, nhập, chi) trong khoảng ngày.
        /// </summary>
        public static TongTienVisitor TongHopChungTu(DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay.Date > denNgay.Date)
                throw new ThongKeException("'Từ ngày' phải nhỏ hơn hoặc bằng 'Đến ngày'.");

            DataTable tbl = new ThongKeFactory().TongHopChungTu(tuNgay, denNgay);
            TongTienVisitor visitor = new TongTienVisitor();

            foreach (DataRow r in tbl.Rows)
            {
                IChungTu ct = ChungTuFactory.Tao(Convert.ToString(r["LOAI"]),
                                                 Convert.ToString(r["ID"]),
                                                 Convert.ToDateTime(r["NGAY"]),
                                                 Convert.ToInt64(r["TONG_TIEN"]));
                ct.Accept(visitor);
            }
            return visitor;
        }
    }
}
