using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;

namespace CuahangNongduoc.DataLayer
{
    public enum LocGiamGia { GiamGia, KhuyenMai, Ca }

    /// <summary>
    /// Truy vấn thống kê cho UI-06 / UI-07 trên SQL Server. Dùng DataService của nhóm (chung chuỗi kết nối).
    /// Mọi truy vấn đều tham số hóa; lọc ngày theo khoảng  NGAY >= @tu AND NGAY &lt; @den  (den = ngày cuối + 1)
    /// để không sót phiếu có giờ (FIX-08).
    /// </summary>
    public class BaoCaoFactory
    {
        /// <summary>
        /// Phân loại khuyến mãi theo PHIEU_BAN.LOAI_GIAM_GIA (do form bán hàng của P07 ghi): giá trị bắt đầu bằng
        /// "KHUYEN" hoặc "Khuyến" là khuyến mãi; còn lại (có TIEN_GIAM_GIA &gt; 0) là giảm giá/chiết khấu.
        /// Nếu P07 dùng giá trị khác, chỉ cần sửa điều kiện này.
        /// </summary>
        private const string DK_KHUYEN_MAI = "(ISNULL(pb.LOAI_GIAM_GIA, N'') LIKE N'KHUYEN%' OR ISNULL(pb.LOAI_GIAM_GIA, N'') LIKE N'Khuyến%')";

        private static DataTable Lay(SqlCommand cmd)
        {
            DataService ds = new DataService();
            ds.Load(cmd);
            return ds.Table;
        }

        private static SqlCommand TaoLenh(string sql, DateTime tu, DateTime den)
        {
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.Add("@tu", SqlDbType.DateTime).Value = tu.Date;
            cmd.Parameters.Add("@den", SqlDbType.DateTime).Value = den.Date.AddDays(1);
            return cmd;
        }

        private static void ThemNhanVien(StringBuilder sql, SqlCommand cmd, int? idNguoiDung)
        {
            if (idNguoiDung.HasValue)
            {
                sql.Append(" AND pb.ID_NGUOI_DUNG = @nd");
                cmd.Parameters.Add("@nd", SqlDbType.Int).Value = idNguoiDung.Value;
            }
        }

        public DataTable DanhSachNguoiDung()
        {
            return Lay(new SqlCommand("SELECT ID, ISNULL(HOTEN, TEN_DANG_NHAP) AS TEN FROM NGUOI_DUNG WHERE IS_ACTIVE = 1 ORDER BY TEN"));
        }

        /// <summary>
        /// Tồn kho theo khoảng ngày: "Bán trong kỳ" = số lượng bán từ tu..den; "Tồn cuối kỳ" = tồn tại hết ngày den
        /// = tồn hiện tại của các lô nhập đến ngày den + số lượng đã bán sau ngày den.
        /// </summary>
        public DataTable TonKho(DateTime tu, DateTime den)
        {
            string sql =
                "SELECT SP.ID, SP.TEN_SAN_PHAM, ISNULL(DVT.TEN_DON_VI, N'') AS DVT, " +
                "       ISNULL(BAN.SL, 0) AS BAN_TRONG_KY, ISNULL(TON.SL, 0) AS TON_CUOI " +
                "FROM SAN_PHAM SP " +
                "LEFT JOIN DON_VI_TINH DVT ON DVT.ID = SP.ID_DON_VI_TINH " +
                "LEFT JOIN ( " +
                "    SELECT MA.ID_SAN_PHAM, SUM(CT.SO_LUONG) AS SL " +
                "    FROM CHI_TIET_PHIEU_BAN CT " +
                "    JOIN PHIEU_BAN PB ON PB.ID = CT.ID_PHIEU_BAN " +
                "    JOIN MA_SAN_PHAM MA ON MA.ID = CT.ID_MA_SAN_PHAM " +
                "    WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den " +
                "    GROUP BY MA.ID_SAN_PHAM) BAN ON BAN.ID_SAN_PHAM = SP.ID " +
                "LEFT JOIN ( " +
                "    SELECT MA.ID_SAN_PHAM, SUM(ISNULL(MA.SO_LUONG, 0) + ISNULL(SAU.SL, 0)) AS SL " +
                "    FROM MA_SAN_PHAM MA " +
                "    LEFT JOIN ( " +
                "        SELECT CT.ID_MA_SAN_PHAM, SUM(CT.SO_LUONG) AS SL " +
                "        FROM CHI_TIET_PHIEU_BAN CT JOIN PHIEU_BAN PB ON PB.ID = CT.ID_PHIEU_BAN " +
                "        WHERE PB.NGAY_BAN >= @den GROUP BY CT.ID_MA_SAN_PHAM) SAU ON SAU.ID_MA_SAN_PHAM = MA.ID " +
                "    WHERE ISNULL(MA.NGAY_NHAP, '19000101') < @den " +
                "    GROUP BY MA.ID_SAN_PHAM) TON ON TON.ID_SAN_PHAM = SP.ID " +
                "ORDER BY SP.ID";
            return Lay(TaoLenh(sql, tu, den));
        }

        public DataTable VanChuyen(DateTime tu, DateTime den, int? idNguoiDung)
        {
            StringBuilder sql = new StringBuilder(
                "SELECT pb.ID, pb.NGAY_BAN, kh.HO_TEN, pb.KHOANG_CACH_KM, ISNULL(pb.PHI_VAN_CHUYEN, 0) AS PHI_VAN_CHUYEN, nd.HOTEN " +
                "FROM PHIEU_BAN pb " +
                "LEFT JOIN KHACH_HANG kh ON kh.ID = pb.ID_KHACH_HANG " +
                "LEFT JOIN NGUOI_DUNG nd ON nd.ID = pb.ID_NGUOI_DUNG " +
                "WHERE pb.NGAY_BAN >= @tu AND pb.NGAY_BAN < @den AND pb.GIAO_HANG = 1");
            SqlCommand cmd = TaoLenh("", tu, den);
            ThemNhanVien(sql, cmd, idNguoiDung);
            sql.Append(" ORDER BY pb.NGAY_BAN, pb.ID");
            cmd.CommandText = sql.ToString();
            return Lay(cmd);
        }

        public DataTable DichVu(DateTime tu, DateTime den, int? idNguoiDung)
        {
            StringBuilder sql = new StringBuilder(
                "SELECT pb.ID, pb.NGAY_BAN, dv.TEN_DICH_VU, dv.SO_TIEN " +
                "FROM PHIEU_BAN pb " +
                "JOIN CHI_TIET_DICH_VU_PHIEU_BAN dv ON dv.ID_PHIEU_BAN = pb.ID " +
                "WHERE pb.NGAY_BAN >= @tu AND pb.NGAY_BAN < @den");
            SqlCommand cmd = TaoLenh("", tu, den);
            ThemNhanVien(sql, cmd, idNguoiDung);
            sql.Append(" ORDER BY pb.NGAY_BAN, pb.ID, dv.ID");
            cmd.CommandText = sql.ToString();
            return Lay(cmd);
        }

        public DataTable GiamGia(DateTime tu, DateTime den, int? idNguoiDung, LocGiamGia loc)
        {
            StringBuilder sql = new StringBuilder(
                "SELECT pb.ID, pb.NGAY_BAN, kh.HO_TEN, ISNULL(pb.TIEN_HANG, pb.TONG_TIEN) AS TIEN_HANG, " +
                "ISNULL(pb.TIEN_GIAM_GIA, 0) AS TIEN_GIAM_GIA, " +
                "CASE WHEN " + DK_KHUYEN_MAI + " THEN N'Khuyến mãi' ELSE N'Giảm giá' END AS LOAI, nd.HOTEN " +
                "FROM PHIEU_BAN pb " +
                "LEFT JOIN KHACH_HANG kh ON kh.ID = pb.ID_KHACH_HANG " +
                "LEFT JOIN NGUOI_DUNG nd ON nd.ID = pb.ID_NGUOI_DUNG " +
                "WHERE pb.NGAY_BAN >= @tu AND pb.NGAY_BAN < @den AND ISNULL(pb.TIEN_GIAM_GIA, 0) > 0");
            SqlCommand cmd = TaoLenh("", tu, den);
            if (loc == LocGiamGia.KhuyenMai)
                sql.Append(" AND " + DK_KHUYEN_MAI);
            else if (loc == LocGiamGia.GiamGia)
                sql.Append(" AND NOT " + DK_KHUYEN_MAI);
            ThemNhanVien(sql, cmd, idNguoiDung);
            sql.Append(" ORDER BY pb.NGAY_BAN, pb.ID");
            cmd.CommandText = sql.ToString();
            return Lay(cmd);
        }
    }
}
