using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuChiFactory : BaseFactory
    {
        public DataTable TimPhieuChi(int lydo, DateTime ngay)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_CHI WHERE ID_LY_DO_CHI = @lydo AND NGAY_CHI = @ngay");
            cmd.Parameters.Add("@lydo", SqlDbType.Int).Value = lydo;
            cmd.Parameters.Add("@ngay", SqlDbType.DateTime).Value = ngay;

            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable DanhsachPhieuChi()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_CHI ");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LayPhieuChi(String id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_CHI WHERE ID = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public static long LayTongTien(String lydo, int thang, int nam)
        {
            DataService ds = new DataService();
            // FIX-08 / R29 + R32
            // Trước: SUM(TONG_TIEN) ... MONTH(NGAY_CHI)=@thang AND YEAR(NGAY_CHI)=@nam
            // Sau  : ISNULL(SUM(TONG_TIEN),0) ... NGAY_CHI >= @tu AND NGAY_CHI < @den
            DateTime dauThang = new DateTime(nam, thang, 1);
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(TONG_TIEN), 0) FROM PHIEU_CHI WHERE ID_LY_DO_CHI = @lydo AND NGAY_CHI >= @tu AND NGAY_CHI < @den");
            cmd.Parameters.Add("@lydo", SqlDbType.VarChar, 50).Value = lydo;
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
