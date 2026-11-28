using System;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Security
{
    // Phiên đăng nhập DUY NHẤT của chương trình (trước đây có 3 bản: PhienLamViec, Security.PhienDangNhap
    // và CuahangNongduoc.PhienDangNhap trong frmDangNhap.cs -> frmDangNhap ghi vào bản không ai đọc).
    // Dùng ở mọi nơi: PhienDangNhap.Hien.Id, PhienDangNhap.HoTen, PhienDangNhap.CoQuyen(ChucNang.BAN_LE) ...
    public static class PhienDangNhap
    {
        // [RF-36 Introduce Null Object]
        // Trước: public static NguoiDung Hien = null;
        //        public static bool CoQuyen(string ma) { return Hien != null && Hien.CoQuyen(ma); }
        //        (mọi nơi dùng đều phải kiểm null: if (Hien != null && Hien.DaDangNhap) ...)
        // Sau: Hien luôn khác null; chưa đăng nhập = NguoiDungKhach (DaDangNhap=false, CoQuyen=false).
        // Lý do: bỏ kiểm null rải rác, frmMain/Proxy/Chain gọi thẳng.
        public static NguoiDung Hien { get; private set; }

        static PhienDangNhap() { Hien = new NguoiDungKhach(); }

        public static int Id { get { return Hien.Id; } }
        public static string TenDangNhap { get { return Hien.TenDangNhap; } }
        public static string HoTen { get { return Hien.HoTen; } }
        public static string Role { get { return Hien.Role; } }
        public static bool DaDangNhap { get { return Hien.DaDangNhap; } }
        public static bool LaAdmin
        {
            get { return Hien.DaDangNhap && string.Equals(Hien.Role, "admin", StringComparison.OrdinalIgnoreCase); }
        }

        public static void DatNguoiDung(NguoiDung nd) { Hien = nd ?? new NguoiDungKhach(); }
        public static bool CoQuyen(string maChucNang) { return Hien.CoQuyen(maChucNang); }
        public static void DangXuat() { Hien = new NguoiDungKhach(); }
    }
}
