using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class MaSanPhanFactory
    {
        DataService m_Ds = new DataService();

        public void LoadSchema()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE ID = '-1'");
            m_Ds.Load(cmd);
        }

        public DataTable DanhsachMaSanPham(String sp)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE ID_SAN_PHAM=@id AND SO_LUONG > 0");
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = sp;
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable DanhsachChiTiet(String sp)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE ID_PHIEU_NHAP=@id");
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = sp;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LaySanPham(String idMaSanPham)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT SP.* FROM SAN_PHAM SP INNER JOIN MA_SAN_PHAM MSP ON SP.ID = MSP.ID_SAN_PHAM WHERE MSP.ID = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = idMaSanPham;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayMaSanPham(String idMaSanPham)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM MSP WHERE MSP.ID = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = idMaSanPham;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable DanhsachMaSanPhamHetHan(DateTime dt)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0 AND NGAY_HET_HAN <= @ngay");
            cmd.Parameters.Add("ngay", OleDbType.Date).Value = dt;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable DanhsachMaSanPham()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public static void CapNhatSoLuong(String masp, int so_luong)
        {
            // TRUOC: UPDATE MA_SAN_PHAM bang OleDb, khong dung SAN_PHAM, cho phep ton am
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
