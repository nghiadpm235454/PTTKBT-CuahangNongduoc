using System;

namespace CuahangNongduoc.BusinessObject
{
    // [REFACTOR #16 - Introduce Foreign Method]
    // DateTime la lop cua .NET nen khong sua duoc -> dat cac ham "dang le thuoc DateTime" vao day.
    public static class ThoiGian
    {
        public static string DinhDang(DateTime d) { return d.ToString("dd/MM/yyyy"); }

        // HSD la ngay cuoi cung con dung duoc -> het han khi HSD < ngay xet
        public static bool DaHetHan(DateTime hanDung, DateTime ngayXet) { return hanDung.Date < ngayXet.Date; }

        public static int SoNgayConLai(DateTime hanDung, DateTime ngayXet) { return (hanDung.Date - ngayXet.Date).Days; }
    }
}
