using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class ChiTietPhieuBanFactory : BaseFactory
    {
        public DataTable LayChiTietPhieuBan(String idPhieuBan)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idPhieuBan;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayChiTietPhieuBan(DateTime dtNgayBan)
        {
            // FIX-08 / R32
            // Trước: WHERE PB.NGAY_BAN = @ngayban            (so sánh bằng -> sai khi NGAY_BAN có giờ)
            // Sau  : WHERE PB.NGAY_BAN >= @tu AND < @den     (khoảng [ngày, ngày+1))
            SqlCommand cmd = new SqlCommand("SELECT CT.* FROM CHI_TIET_PHIEU_BAN CT INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                    " WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den");
            cmd.Parameters.Add("@tu", SqlDbType.DateTime).Value = dtNgayBan.Date;
            cmd.Parameters.Add("@den", SqlDbType.DateTime).Value = dtNgayBan.Date.AddDays(1);
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayChiTietPhieuBan(int thang, int nam)
        {
            // FIX-08 / R32
            // Trước: WHERE MONTH(PB.NGAY_BAN) = @thang AND YEAR(PB.NGAY_BAN) = @nam   (hàm trên cột -> không dùng được index)
            // Sau  : WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den                  (khoảng ngày đầu tháng -> đầu tháng sau)
            DateTime dauThang = new DateTime(nam, thang, 1);
            SqlCommand cmd = new SqlCommand("SELECT CT.* FROM CHI_TIET_PHIEU_BAN CT INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                    " WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den");
            cmd.Parameters.Add("@tu", SqlDbType.DateTime).Value = dauThang;
            cmd.Parameters.Add("@den", SqlDbType.DateTime).Value = dauThang.AddMonths(1);
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public override bool Save()
        {
            foreach (DataRow row in m_Ds.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(Convert.ToString(row["ID_MA_SAN_PHAM"]), -Convert.ToInt32(row["SO_LUONG"]));
                }
            }
            return m_Ds.ExecuteNoneQuery() > 0;
        }
    }
}
