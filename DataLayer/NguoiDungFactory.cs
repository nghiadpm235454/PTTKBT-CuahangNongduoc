using System;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class NguoiDungFactory
    {
        DataService m_Ds = new DataService();

        public DataTable LayNguoiDung(String tenDangNhap)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT ID, TEN_DANG_NHAP, MATKHAU, SALT, [ROLE], HOTEN, IS_ACTIVE, MUST_CHANGE_PWD " +
                "FROM NGUOI_DUNG WHERE TEN_DANG_NHAP = @ten");
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 100).Value = tenDangNhap;
            m_Ds.Load(cmd);
            return m_Ds;
        }

    }
}