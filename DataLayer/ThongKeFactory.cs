using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    /// <summary>
    /// P09 (Thư) - CN-07/CN-08: gọi view / stored procedure thống kê trên SQL Server.
    /// Dùng SqlClient (không dùng OleDb). Lỗi SQL được ném lên cho Controller xử lý
    /// (không nuốt lỗi như DataService.Load cũ).
    /// </summary>
    public class ThongKeFactory
    {
        // TẠM THỜI: khi Chú Ba (P01) xong DatabaseManager thì đổi dòng này
        // thành:  return DatabaseManager.Instance.ConnectionString;
        // Có thể gán lại từ Program.cs:  ThongKeFactory.ConnectionString = "...";
        private static string m_ConnectionString =
            "Data Source=.;Initial Catalog=CuahangNongduoc;Integrated Security=True;";

        public static string ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        // ---- hàm dùng chung -------------------------------------------------
        private DataTable ChayProc(string tenProc, params SqlParameter[] thamSo)
        {
            using (SqlConnection cn = new SqlConnection(m_ConnectionString))
            using (SqlCommand cmd = new SqlCommand(tenProc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (thamSo != null)
                {
                    foreach (SqlParameter p in thamSo)
                        cmd.Parameters.Add(p);
                }
                DataTable tbl = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tbl);
                return tbl;
            }
        }

        private static SqlParameter ThamSoNgay(string ten, DateTime giaTri)
        {
            SqlParameter p = new SqlParameter(ten, SqlDbType.DateTime);
            p.Value = giaTri.Date;
            return p;
        }

        // ---- các truy vấn thống kê ------------------------------------------
        public DataTable TonKho()
        {
            using (SqlConnection cn = new SqlConnection(m_ConnectionString))
            {
                DataTable tbl = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM dbo.vw_TonKho ORDER BY TEN_SAN_PHAM", cn);
                da.Fill(tbl);
                return tbl;
            }
        }

        public DataTable ChiPhiVanChuyen(DateTime tuNgay, DateTime denNgay)
        {
            return ChayProc("dbo.sp_ChiPhiVanChuyen",
                ThamSoNgay("@tu", tuNgay), ThamSoNgay("@den", denNgay));
        }

        public DataTable DichVuPhu(DateTime tuNgay, DateTime denNgay)
        {
            return ChayProc("dbo.sp_DichVuPhu",
                ThamSoNgay("@tu", tuNgay), ThamSoNgay("@den", denNgay));
        }

        /// <param name="loai">null = tất cả; "GIAM_GIA" hoặc "KHUYEN_MAI"</param>
        public DataTable GiamGia(DateTime tuNgay, DateTime denNgay, string loai)
        {
            SqlParameter pLoai = new SqlParameter("@loai", SqlDbType.NVarChar, 20);
            pLoai.Value = (loai == null) ? (object)DBNull.Value : (object)loai;
            return ChayProc("dbo.sp_GiamGia",
                ThamSoNgay("@tu", tuNgay), ThamSoNgay("@den", denNgay), pLoai);
        }

        /// <param name="idNguoiDung">null = mọi nhân viên; số = nhân viên đang đăng nhập</param>
        public DataTable GiamGiaTheoNhanVien(DateTime tuNgay, DateTime denNgay, int? idNguoiDung)
        {
            SqlParameter pNv = new SqlParameter("@idNguoiDung", SqlDbType.Int);
            if (idNguoiDung.HasValue) pNv.Value = idNguoiDung.Value;
            else pNv.Value = DBNull.Value;
            return ChayProc("dbo.sp_GiamGiaTheoNhanVien",
                ThamSoNgay("@tu", tuNgay), ThamSoNgay("@den", denNgay), pNv);
        }

        public DataTable TongHopChungTu(DateTime tuNgay, DateTime denNgay)
        {
            return ChayProc("dbo.sp_TongHopChungTu",
                ThamSoNgay("@tu", tuNgay), ThamSoNgay("@den", denNgay));
        }
    }
}
