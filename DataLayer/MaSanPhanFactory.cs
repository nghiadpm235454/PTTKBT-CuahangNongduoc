using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class MaSanPhanFactory
    {
        DataService m_Ds = new DataService();

        public void LoadSchema()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE ID = '-1'");
            m_Ds.Load(cmd);
        }

        public DataTable DanhsachMaSanPham(String sp)
        {
            return DanhsachMaSanPham(sp, false);
        }

        public DataTable DanhsachMaSanPham(String sp, bool chiConHan)
        {
            String sql = "SELECT * FROM MA_SAN_PHAM WHERE ID_SAN_PHAM=@id AND SO_LUONG > 0";
            if (chiConHan)
                sql += " AND NGAY_HET_HAN >= @ngay";
            sql += " ORDER BY NGAY_HET_HAN ASC";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = sp;
            if (chiConHan)
                cmd.Parameters.Add("@ngay", SqlDbType.DateTime).Value = DateTime.Today;
            m_Ds.Load(cmd);

            return m_Ds;
        }
        // CN-05: cac lo con hang cua 1 san pham, lo nhap som nhat truoc.
        public DataTable DanhsachLoConHang(String idSanPham)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE ID_SAN_PHAM=@id AND SO_LUONG > 0 ORDER BY NGAY_NHAP, ID");
            cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idSanPham;
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable DanhsachChiTiet(String sp)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE ID_PHIEU_NHAP=@id");
            cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = sp;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LaySanPham(String idMaSanPham)
        {
            SqlCommand cmd = new SqlCommand("SELECT SP.* FROM SAN_PHAM SP INNER JOIN MA_SAN_PHAM MSP ON SP.ID = MSP.ID_SAN_PHAM WHERE MSP.ID = @id");
            cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idMaSanPham;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayMaSanPham(String idMaSanPham)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM MSP WHERE MSP.ID = @id");
            cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idMaSanPham;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable DanhsachMaSanPhamHetHan(DateTime dt)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0 AND NGAY_HET_HAN <= @ngay");
            cmd.Parameters.Add("@ngay", SqlDbType.Date).Value = dt;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable DanhsachMaSanPham()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public static void CapNhatSoLuong(String masp, int so_luong)
        {
            // TRUOC: UPDATE MA_SAN_PHAM, khong dong bo SAN_PHAM, cho phep ton am
            // SAU:   dung LoHangDao: cap nhat ca MA_SAN_PHAM + SAN_PHAM, chan ton am
            LoHangDao.CapNhatTon(masp, so_luong);
        }

        public DataRow NewRow()
        {
            return m_Ds.NewRow();
        }
        public void Add(DataRow row)
        {
            m_Ds.Rows.Add(row);
        }
        public bool Save()
        {
            return m_Ds.ExecuteNoneQuery() > 0;
        }
    }
}