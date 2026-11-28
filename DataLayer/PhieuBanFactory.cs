using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuBanFactory : BaseFactory
    {
        public DataTable TimPhieuBan(String idKh, DateTime dt)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_BAN WHERE NGAY_BAN = @ngay AND ID_KHACH_HANG=@kh");
            cmd.Parameters.Add("@ngay", SqlDbType.DateTime).Value = dt;
            cmd.Parameters.Add("@kh", SqlDbType.VarChar, 50).Value = idKh;

            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable DanhsachPhieuBanLe()
        {
            SqlCommand cmd = new SqlCommand("SELECT PB.* FROM PHIEU_BAN PB INNER JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG=KH.ID WHERE KH.LOAI_KH = 0");
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable DanhsachPhieuBanSi()
        {
            SqlCommand cmd = new SqlCommand("SELECT PB.* FROM PHIEU_BAN PB INNER JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG=KH.ID WHERE KH.LOAI_KH = 1");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LayPhieuBan(String id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_BAN WHERE ID = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayChiTietPhieuBan(String idPhieuBan)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idPhieuBan;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public static long LayConNo(String kh, int thang, int nam)
        {
            DataService ds = new DataService();
            // FIX-08 / R29 + R32
            // Trước: SELECT SUM(CON_NO) ... MONTH(NGAY_BAN)=@thang AND YEAR(NGAY_BAN)=@nam   (SUM có thể NULL; hàm trên cột)
            // Sau  : ISNULL(SUM(CON_NO),0) ... NGAY_BAN >= @tu AND NGAY_BAN < @den
            DateTime dauThang = new DateTime(nam, thang, 1);
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(CON_NO), 0) FROM PHIEU_BAN WHERE ID_KHACH_HANG = @kh AND NGAY_BAN >= @tu AND NGAY_BAN < @den");
            cmd.Parameters.Add("@kh", SqlDbType.VarChar, 50).Value = kh;
            cmd.Parameters.Add("@tu", SqlDbType.DateTime).Value = dauThang;
            cmd.Parameters.Add("@den", SqlDbType.DateTime).Value = dauThang.AddMonths(1);

            object obj = ds.ExecuteScalar(cmd);
            if (obj == null)
                return 0;
            else
                return Convert.ToInt64(obj);
        }

        public static int LaySoPhieu()
        {
            DataService ds = new DataService();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM PHIEU_BAN");

            object obj = ds.ExecuteScalar(cmd);
            if (obj == null)
                return 0;
            else
                return Convert.ToInt32(obj);
        }
    }
}
