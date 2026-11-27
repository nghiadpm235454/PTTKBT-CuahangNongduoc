using System;
using System.Data;
using System.Data.SqlClient;
using BO = CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    /// <summary>Ten cot trong bang THAM_SO (RF-02 #22: thay chuoi lap lai bang hang so).</summary>
    public static class KhoaThamSo
    {
        public const string PhieuNhap = "PHIEU_NHAP";
        public const string PhieuBan = "PHIEU_BAN";
        public const string PhieuThanhToan = "PHIEU_THANH_TOAN";
        public const string SanPham = "SAN_PHAM";
        public const string NhaCungCap = "NHA_CUNG_CAP";
        public const string KhachHang = "KHACH_HANG";
        public const string PhieuChi = "PHIEU_CHI";
    }

    /// <summary>
    /// FIX-03 (R07): thay cac cap Lay/Gan lap lai trong ThamSo bang GetThamSo/SetThamSo.
    /// Dong thoi giu 1 doi tuong CuaHang dung chung (RF-02 #19 Change Value to Reference).
    /// </summary>
    public class ThamSoManager
    {
        private static readonly ThamSoManager m_Instance = new ThamSoManager();
        public static ThamSoManager Instance { get { return m_Instance; } }
        private ThamSoManager() { }

        private static readonly string[] CAC_KHOA = {
            KhoaThamSo.PhieuNhap, KhoaThamSo.PhieuBan, KhoaThamSo.PhieuThanhToan,
            KhoaThamSo.SanPham, KhoaThamSo.NhaCungCap, KhoaThamSo.KhachHang, KhoaThamSo.PhieuChi
        };

        // Ten cot duoc noi vao cau SQL nen chi cho phep khoa nam trong danh sach.
        private static void KiemTraKhoa(string khoa)
        {
            if (Array.IndexOf(CAC_KHOA, khoa) < 0)
                throw new ArgumentException("Khoa tham so khong hop le: " + khoa);
        }

        public long GetThamSo(string khoa)
        {
            KiemTraKhoa(khoa);
            DataService ds = new DataService();
            object obj = ds.ExecuteScalar(new SqlCommand("SELECT " + khoa + " FROM THAM_SO"));
            return Convert.ToInt64(obj);
        }

        public void SetThamSo(string khoa, long giaTri)
        {
            KiemTraKhoa(khoa);
            DataService ds = new DataService();
            SqlCommand cmd = new SqlCommand("UPDATE THAM_SO SET " + khoa + " = @gia_tri");
            cmd.Parameters.Add("@gia_tri", SqlDbType.BigInt).Value = giaTri;
            ds.ExecuteNoneQuery(cmd);
        }

        // ---- CuaHang dung chung (RF-02 #19) ----
        private BO.CuaHang m_CuaHang;

        public BO.CuaHang LayCuaHang()
        {
            if (m_CuaHang == null) m_CuaHang = ThamSo.LayCuaHang();
            return m_CuaHang;
        }

        /// <summary>Goi sau khi cap nhat THAM_SO de lan doc sau lay du lieu moi.</summary>
        public void LamMoiCuaHang()
        {
            m_CuaHang = null;
        }
    }
}
