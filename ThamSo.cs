using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc
{
    // [R21 Rename Type] Trước: enum Controll { Normal, AddNew, Edit }  (tên "Controll" sai chính tả, không nói lên ý nghĩa)
    // Sau: FormEditState = trạng thái chỉnh sửa của form (Normal/AddNew/Edit). Đã đổi ở 3 form dùng enum: frmBanLe, frmBanSi, frmNhapHang.
    public enum FormEditState
    {
        Normal,
        AddNew,
        Edit
    }
    public class ThamSo
    {
        public static void PreMonth(ref int thangtruoc, ref int namtruoc, int thang, int nam)
        {
            // Lùi 1 tháng bằng DateTime, tự xử lý chuyện tháng 1 lùi về tháng 12 năm trước
            DateTime ngayTruoc = new DateTime(nam, thang, 1).AddMonths(-1);
            thangtruoc = ngayTruoc.Month;
            namtruoc = ngayTruoc.Year;
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

        // R08: tham so hoa cau lenh UPDATE (ten cot la hang chu co dinh, gia tri truyen bang tham so)
        private static SqlCommand TaoLenhGan(string cot, long giaTri)
        {
            SqlCommand cmd = new SqlCommand("UPDATE THAM_SO SET " + cot + " = @gt");
            cmd.Parameters.Add("@gt", SqlDbType.BigInt).Value = giaTri;
            return cmd;
        }

        public static long LayMaPhieuNhap()
        {
            DataService ds = new DataService();
            return Convert.ToInt64(ds.ExecuteScalar(new SqlCommand("SELECT PHIEU_NHAP FROM THAM_SO")));
        }
        public static void GanMaPhieuNhap(long id)
        {
            DataService ds = new DataService();
            ds.ExecuteNoneQuery(TaoLenhGan("PHIEU_NHAP", id));

        }

        public static long LayMaPhieuBan()
        {
            DataService ds = new DataService();
            return Convert.ToInt64(ds.ExecuteScalar(new SqlCommand("SELECT PHIEU_BAN FROM THAM_SO")));
        }
        public static void GanMaPhieuBan(long id)
        {
            DataService ds = new DataService();
            ds.ExecuteNoneQuery(TaoLenhGan("PHIEU_BAN", id));

        }

        public static long LayMaPhieuThanhToan()
        {
            DataService ds = new DataService();
            return Convert.ToInt64(ds.ExecuteScalar(new SqlCommand("SELECT PHIEU_THANH_TOAN FROM THAM_SO")));
        }
        public static void GanMaPhieuThanhToan(long id)
        {
            DataService ds = new DataService();
            ds.ExecuteNoneQuery(TaoLenhGan("PHIEU_THANH_TOAN", id));

        }



        public static long SanPham
        {
            get
            {
                DataService ds = new DataService();
                return Convert.ToInt64(ds.ExecuteScalar(new SqlCommand("SELECT SAN_PHAM FROM THAM_SO")));
            }
            set
            {
                DataService ds = new DataService();
                ds.ExecuteNoneQuery(TaoLenhGan("SAN_PHAM", value));
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
                return Convert.ToInt64(ds.ExecuteScalar(new SqlCommand("SELECT NHA_CUNG_CAP FROM THAM_SO")));
            }
            set
            {
                DataService ds = new DataService();
                ds.ExecuteNoneQuery(TaoLenhGan("NHA_CUNG_CAP", value));
            }
        }

        public static long KhachHang
        {
            get
            {
                DataService ds = new DataService();
                return Convert.ToInt64(ds.ExecuteScalar(new SqlCommand("SELECT KHACH_HANG FROM THAM_SO")));
            }
            set
            {
                DataService ds = new DataService();
                ds.ExecuteNoneQuery(TaoLenhGan("KHACH_HANG", value));
            }
        }

        public static long PhieuChi
        {
            get
            {
                DataService ds = new DataService();
                return Convert.ToInt64(ds.ExecuteScalar(new SqlCommand("SELECT PHIEU_CHI FROM THAM_SO")));
            }
            set
            {
                DataService ds = new DataService();
                ds.ExecuteNoneQuery(TaoLenhGan("PHIEU_CHI", value));
            }
        }


    }
}
