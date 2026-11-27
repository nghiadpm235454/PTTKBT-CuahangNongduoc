<<<<<<< HEAD
using System;
using System.Data;
using System.Data.SqlClient;
using BO = CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    public enum Controll
    {
        Normal,
        AddNew,
        Edit
    }

    // FIX-03 (R07) - Truoc: ThamSo co 7 cap Lay/Gan lap lai (LayMaPhieuNhap/GanMaPhieuNhap,
    // LayMaPhieuBan/GanMaPhieuBan, LayMaPhieuThanhToan/GanMaPhieuThanhToan, va cac property
    // SanPham, NhaCungCap, KhachHang, PhieuChi).
    // Sau: tat ca chuyen vao ThamSoManager.GetThamSo(KhoaThamSo.x) / SetThamSo(KhoaThamSo.x, gia_tri).
    // RF-02 #21/#20: PreMonth(ref, ref, ...) chuyen thanh BusinessObject.KyThang.ThangTruoc().
    public class ThamSo
    {
        // RF-02 #22: ma ky thuat thay cho chuoi "so ma thuat" lap lai
        private const string XUAT_KHO_FIFO = "FIFO";
        private const string XUAT_KHO_CHI_DINH = "CHI_DINH";
        private const string GIA_XUAT_FIFO = "FIFO";
        private const string GIA_XUAT_BQGQ = "BQGQ";

        public static bool LaSoNguyen(String so)
        {
            try
            {
                Convert.ToInt64(so);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static BO.CuaHang LayCuaHang()
        {
            BO.CuaHang ch = new BO.CuaHang();
            DataService ds = new DataService();
            ds.Load(new SqlCommand("SELECT TEN_CUA_HANG, DIA_CHI, DIEN_THOAI, PP_XUAT_KHO, PP_GIA_XUAT FROM THAM_SO"));
            if (ds.Rows.Count > 0)
            {
                ch.TenCuaHang = ds.Rows[0]["TEN_CUA_HANG"].ToString();
                ch.DiaChi = ds.Rows[0]["DIA_CHI"].ToString();
                ch.DienThoai = ds.Rows[0]["DIEN_THOAI"].ToString();
                ch.PhuongPhapXuatKho = ds.Rows[0]["PP_XUAT_KHO"].ToString() == XUAT_KHO_CHI_DINH
                    ? BO.PhuongPhapXuatKho.ChiDinh : BO.PhuongPhapXuatKho.FIFO;
                ch.PhuongPhapGiaXuat = ds.Rows[0]["PP_GIA_XUAT"].ToString() == GIA_XUAT_FIFO
                    ? BO.PhuongPhapGiaXuat.FIFO : BO.PhuongPhapGiaXuat.BQGQ;
            }
            return ch;
        }

        // Giu nguyen chu ky cu de form cu van chay (chi cap nhat 3 cot thong tin).
        public static void GanCuaHang(String ten_cua_hang, String dia_chi, String dien_thoai)
        {
            DataService ds = new DataService();
            SqlCommand cmd = new SqlCommand("UPDATE THAM_SO SET TEN_CUA_HANG = @ten_cua_hang, DIA_CHI = @dia_chi, DIEN_THOAI = @dien_thoai");
            cmd.Parameters.Add("@ten_cua_hang", SqlDbType.NVarChar).Value = ten_cua_hang;
            cmd.Parameters.Add("@dia_chi", SqlDbType.NVarChar).Value = dia_chi;
            cmd.Parameters.Add("@dien_thoai", SqlDbType.NVarChar).Value = dien_thoai;

            ds.ExecuteNoneQuery(cmd);
            ThamSoManager.Instance.LamMoiCuaHang();
        }

        // CN-01: luu them phuong phap xuat kho + phuong phap tinh gia xuat.
        public static void GanCuaHang(String ten_cua_hang, String dia_chi, String dien_thoai,
            BO.PhuongPhapXuatKho xuatKho, BO.PhuongPhapGiaXuat giaXuat)
        {
            DataService ds = new DataService();
            SqlCommand cmd = new SqlCommand("UPDATE THAM_SO SET TEN_CUA_HANG = @ten_cua_hang, DIA_CHI = @dia_chi, " +
                "DIEN_THOAI = @dien_thoai, PP_XUAT_KHO = @pp_xuat_kho, PP_GIA_XUAT = @pp_gia_xuat");
            cmd.Parameters.Add("@ten_cua_hang", SqlDbType.NVarChar).Value = ten_cua_hang;
            cmd.Parameters.Add("@dia_chi", SqlDbType.NVarChar).Value = dia_chi;
            cmd.Parameters.Add("@dien_thoai", SqlDbType.NVarChar).Value = dien_thoai;
            cmd.Parameters.Add("@pp_xuat_kho", SqlDbType.NVarChar, 20).Value =
                xuatKho == BO.PhuongPhapXuatKho.ChiDinh ? XUAT_KHO_CHI_DINH : XUAT_KHO_FIFO;
            cmd.Parameters.Add("@pp_gia_xuat", SqlDbType.NVarChar, 20).Value =
                giaXuat == BO.PhuongPhapGiaXuat.FIFO ? GIA_XUAT_FIFO : GIA_XUAT_BQGQ;

            ds.ExecuteNoneQuery(cmd);
            ThamSoManager.Instance.LamMoiCuaHang();
        }
    }
}
=======
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc
{
    public enum Controll
    {
        Normal,
        AddNew,
        Edit
    }
    public class ThamSo
    {
        public static void PreMonth(ref int thangtruoc, ref int namtruoc, int thang, int nam)
        {
            thangtruoc = thang - 1;
            namtruoc = nam;
            if (thangtruoc == 0)
            {
                thangtruoc = 12;
                namtruoc = nam - 1;
            }
        }

        public static bool LaSoNguyen(String so)
        {
            try
            {
                Convert.ToInt64(so);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static long LayMaPhieuNhap()
        {
            DataService ds = new DataService();
            object obj = ds.ExecuteScalar(new SqlCommand("SELECT PHIEU_NHAP FROM THAM_SO"));
            return Convert.ToInt64(obj);
        }
        public static void GanMaPhieuNhap(long id)
        {
            DataService ds = new DataService();
            ds.ExecuteNoneQuery(new SqlCommand("UPDATE THAM_SO SET PHIEU_NHAP = " + id));

        }

        public static long LayMaPhieuBan()
        {
            DataService ds = new DataService();
            object obj = ds.ExecuteScalar(new SqlCommand("SELECT PHIEU_BAN FROM THAM_SO"));
            return Convert.ToInt64(obj);
        }
        public static void GanMaPhieuBan(long id)
        {
            DataService ds = new DataService();
            ds.ExecuteNoneQuery(new SqlCommand("UPDATE THAM_SO SET PHIEU_BAN = " + id));

        }

        public static long LayMaPhieuThanhToan()
        {
            DataService ds = new DataService();
            object obj = ds.ExecuteScalar(new SqlCommand("SELECT PHIEU_THANH_TOAN FROM THAM_SO"));
            return Convert.ToInt64(obj);
        }
        public static void GanMaPhieuThanhToan(long id)
        {
            DataService ds = new DataService();
            ds.ExecuteNoneQuery(new SqlCommand("UPDATE THAM_SO SET PHIEU_THANH_TOAN = " + id));

        }



        public static long SanPham
        {
            get
            {
                DataService ds = new DataService();
                object obj = ds.ExecuteScalar(new SqlCommand("SELECT SAN_PHAM FROM THAM_SO"));
                return Convert.ToInt64(obj);
            }
            set
            {
                DataService ds = new DataService();
                ds.ExecuteNoneQuery(new SqlCommand("UPDATE THAM_SO SET SAN_PHAM = " + value));
            }
        }


        public static CuahangNongduoc.BusinessObject.CuaHang LayCuaHang()
        {
            CuahangNongduoc.BusinessObject.CuaHang ch = new CuahangNongduoc.BusinessObject.CuaHang();
            DataService ds = new DataService();
            ds.Load(new SqlCommand("SELECT TEN_CUA_HANG, DIA_CHI, DIEN_THOAI FROM THAM_SO"));
            if (ds.Rows.Count > 0)
            {
                ch.TenCuaHang = ds.Rows[0]["TEN_CUA_HANG"].ToString();
                ch.DiaChi = ds.Rows[0]["DIA_CHI"].ToString();
                ch.DienThoai = ds.Rows[0]["DIEN_THOAI"].ToString();
            }
            return ch;
        }
        public static void GanCuaHang(String ten_cua_hang, String dia_chi, String dien_thoai)
        {
            DataService ds = new DataService();
            SqlCommand cmd = new SqlCommand("UPDATE THAM_SO SET TEN_CUA_HANG = @ten_cua_hang, DIA_CHI = @dia_chi, DIEN_THOAI = @dien_thoai ");
            cmd.Parameters.Add("@ten_cua_hang", SqlDbType.NVarChar).Value = ten_cua_hang;
            cmd.Parameters.Add("@dia_chi", SqlDbType.NVarChar).Value = dia_chi;
            cmd.Parameters.Add("@dien_thoai", SqlDbType.NVarChar).Value = dien_thoai;

            ds.ExecuteNoneQuery(cmd);
        }



        public static long NhaCungCap
        {
            get
            {
                DataService ds = new DataService();
                object obj = ds.ExecuteScalar(new SqlCommand("SELECT NHA_CUNG_CAP FROM THAM_SO"));
                return Convert.ToInt64(obj);
            }
            set
            {
                DataService ds = new DataService();
                ds.ExecuteNoneQuery(new SqlCommand("UPDATE THAM_SO SET NHA_CUNG_CAP = " + value));
            }
        }

        public static long KhachHang
        {
            get
            {
                DataService ds = new DataService();
                object obj = ds.ExecuteScalar(new SqlCommand("SELECT KHACH_HANG FROM THAM_SO"));
                return Convert.ToInt64(obj);
            }
            set
            {
                DataService ds = new DataService();
                ds.ExecuteNoneQuery(new SqlCommand("UPDATE THAM_SO SET KHACH_HANG = " + value));
            }
        }

        public static long PhieuChi
        {
            get
            {
                DataService ds = new DataService();
                object obj = ds.ExecuteScalar(new SqlCommand("SELECT PHIEU_CHI FROM THAM_SO"));
                return Convert.ToInt64(obj);
            }
            set
            {
                DataService ds = new DataService();
                ds.ExecuteNoneQuery(new SqlCommand("UPDATE THAM_SO SET PHIEU_CHI = " + value));
            }
        }


    }
}
>>>>>>> master
