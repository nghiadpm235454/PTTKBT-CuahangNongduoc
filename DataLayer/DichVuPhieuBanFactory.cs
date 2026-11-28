using System;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class DichVuPhieuBanFactory
    {
        DataService m_Ds = new DataService();

        public DataTable LayDichVu(String idPhieuBan)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM CHI_TIET_DICH_VU_PHIEU_BAN WHERE ID_PHIEU_BAN = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = idPhieuBan;
            m_Ds.Load(cmd);
            return m_Ds;
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
