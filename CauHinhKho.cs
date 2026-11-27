using System;
using System.Data.SqlClient;

namespace CuahangNongduoc
{
    public static class CauHinhKho
    {
        public static string LayPpXuatKho() { return Lay("PP_XUAT_KHO", "FIFO"); }
        public static string LayPpGiaXuat() { return Lay("PP_GIA_XUAT", "BQGQ"); }
        public static void GanPpXuatKho(string giaTri) { Gan("PP_XUAT_KHO", giaTri); }
        public static void GanPpGiaXuat(string giaTri) { Gan("PP_GIA_XUAT", giaTri); }

        private static string Lay(string cot, string macDinh)
        {
            DataService ds = new DataService();
            ds.Load(new SqlCommand("SELECT " + cot + " FROM THAM_SO"));
            if (ds.Rows.Count > 0 && ds.Rows[0][0] != DBNull.Value)
                return ds.Rows[0][0].ToString();
            return macDinh;
        }

        private static void Gan(string cot, string giaTri)
        {
            DataService ds = new DataService();
            SqlCommand cmd = new SqlCommand("UPDATE THAM_SO SET " + cot + " = @v");
            cmd.Parameters.Add("@v", System.Data.SqlDbType.NVarChar).Value = giaTri;
            ds.ExecuteNoneQuery(cmd);
        }
    }
}