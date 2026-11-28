using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Security
{
    public static class ChucNang
    {
        public const string QL_LYDOCHI = "QL_LYDOCHI";
        public const string QL_DONVITINH = "QL_DONVITINH";
        public const string QL_SANPHAM = "QL_SANPHAM";
        public const string QL_KHACHHANG = "QL_KHACHHANG";
        public const string QL_DAILY = "QL_DAILY";
        public const string QL_NHACUNGCAP = "QL_NHACUNGCAP";
        public const string NHAP_HANG = "NHAP_HANG";
        public const string BAN_LE = "BAN_LE";
        public const string BAN_SI = "BAN_SI";
        public const string PHIEU_CHI = "PHIEU_CHI";
        public const string PHIEU_THU = "PHIEU_THU";
        public const string TONGHOP_DUNO = "TONGHOP_DUNO";
        public const string BC_TON = "BC_TON";
        public const string BC_BAN = "BC_BAN";
        public const string BC_HETHAN = "BC_HETHAN";
        public const string CAU_HINH = "CAU_HINH";
        public const string TRO_GIUP = "TRO_GIUP";
    }

    // Vai trò -> tập chức năng được dùng.
    // Mặc định nằm sẵn trong code; nếu CSDL có bảng QUYEN (script CN09_VaiTro_Quyen.sql) thì NapTuCsdl() ghi đè,
    // chưa chạy script thì chương trình vẫn chạy bằng bảng mặc định.
    public static class PhanQuyen
    {
        private static Dictionary<string, HashSet<string>> _quyen = MacDinh();

        private static Dictionary<string, HashSet<string>> MacDinh()
        {
            Dictionary<string, HashSet<string>> d = new Dictionary<string, HashSet<string>>();
            d["admin"] = new HashSet<string>
            {
                ChucNang.QL_LYDOCHI, ChucNang.QL_DONVITINH, ChucNang.QL_SANPHAM,
                ChucNang.QL_KHACHHANG, ChucNang.QL_DAILY, ChucNang.QL_NHACUNGCAP,
                ChucNang.NHAP_HANG, ChucNang.BAN_LE, ChucNang.BAN_SI,
                ChucNang.PHIEU_CHI, ChucNang.PHIEU_THU, ChucNang.TONGHOP_DUNO,
                ChucNang.BC_TON, ChucNang.BC_BAN, ChucNang.BC_HETHAN,
                ChucNang.CAU_HINH, ChucNang.TRO_GIUP
            };
            d["banhang"] = new HashSet<string>
            {
                ChucNang.BAN_LE, ChucNang.BAN_SI, ChucNang.QL_KHACHHANG, ChucNang.QL_DAILY,
                ChucNang.PHIEU_THU, ChucNang.TONGHOP_DUNO, ChucNang.BC_BAN, ChucNang.BC_TON,
                ChucNang.TRO_GIUP
            };
            d["thukho"] = new HashSet<string>
            {
                ChucNang.NHAP_HANG, ChucNang.QL_SANPHAM, ChucNang.QL_DONVITINH,
                ChucNang.QL_NHACUNGCAP, ChucNang.BC_TON, ChucNang.BC_HETHAN,
                ChucNang.TRO_GIUP
            };
            return d;
        }

        public static void NapTuCsdl()
        {
            try
            {
                DataTable tbl = new DangNhapFactory().LayQuyen();
                if (tbl.Rows.Count == 0) return;

                Dictionary<string, HashSet<string>> moi = new Dictionary<string, HashSet<string>>();
                foreach (DataRow r in tbl.Rows)
                {
                    string vaiTro = Convert.ToString(r["MA_VAI_TRO"]).Trim().ToLowerInvariant();
                    string maCN = Convert.ToString(r["MA_CHUC_NANG"]).Trim();
                    HashSet<string> ds;
                    if (!moi.TryGetValue(vaiTro, out ds)) { ds = new HashSet<string>(); moi[vaiTro] = ds; }
                    ds.Add(maCN);
                }
                _quyen = moi;
            }
            catch (Exception)
            {
                // chưa có bảng QUYEN -> giữ bảng mặc định
            }
        }

        public static bool CoQuyen(string role, string maChucNang)
        {
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(maChucNang)) return false;
            HashSet<string> ds;
            return _quyen.TryGetValue(role.Trim().ToLowerInvariant(), out ds) && ds.Contains(maChucNang);
        }
    }
}
