using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuThanhToanFactory : BaseFactory
    {
        public DataTable DanhsachPhieuThanhToan()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_THANH_TOAN ");
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable TimPhieuThanhToan(String kh, DateTime ngay)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_THANH_TOAN WHERE ID_KHACH_HANG=@kh AND NGAY_THANH_TOAN = @ngay");
            cmd.Parameters.Add("@kh", SqlDbType.VarChar, 50).Value = kh;
            cmd.Parameters.Add("@ngay", SqlDbType.DateTime).Value = ngay;

            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LayPhieuThanhToan(String id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_THANH_TOAN WHERE ID = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public static long LayTongTien(String kh, int thang, int nam)
        {
            DataService ds = new DataService();
            // FIX-08 / R29 + R32
            // Trước: SUM(TONG_TIEN) ... MONTH(NGAY_THANH_TOAN)=@thang AND YEAR(NGAY_THANH_TOAN)=@nam
            // Sau  : ISNULL(SUM(TONG_TIEN),0) ... NGAY_THANH_TOAN >= @tu AND NGAY_THANH_TOAN < @den
            DateTime dauThang = new DateTime(nam, thang, 1);
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(TONG_TIEN), 0) FROM PHIEU_THANH_TOAN WHERE ID_KHACH_HANG = @kh AND NGAY_THANH_TOAN >= @tu AND NGAY_THANH_TOAN < @den");
            cmd.Parameters.Add("@kh", SqlDbType.VarChar, 50).Value = kh;
            cmd.Parameters.Add("@tu", SqlDbType.DateTime).Value = dauThang;
            cmd.Parameters.Add("@den", SqlDbType.DateTime).Value = dauThang.AddMonths(1);

            object obj = ds.ExecuteScalar(cmd);

            if (obj == null)
                return 0;
            else
                return Convert.ToInt64(obj);
        }
    }
}
