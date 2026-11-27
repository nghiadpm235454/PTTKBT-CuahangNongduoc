using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.DataLayer
{
    public class ChiTietPhieuBanFactory
    {
        DataService m_Ds = new DataService();

        public DataTable LayChiTietPhieuBan(String idPhieuBan)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id");
            cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idPhieuBan;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayChiTietPhieuBan(DateTime dtNgayBan)
        {
            SqlCommand cmd = new SqlCommand("SELECT CT.* FROM CHI_TIET_PHIEU_BAN CT INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                    " WHERE PB.NGAY_BAN = @ngayban");
            cmd.Parameters.Add("@ngayban", SqlDbType.Date).Value = dtNgayBan;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayChiTietPhieuBan(int thang, int nam)
        {
            SqlCommand cmd = new SqlCommand("SELECT CT.* FROM CHI_TIET_PHIEU_BAN CT INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                    " WHERE MONTH(PB.NGAY_BAN) = @thang AND YEAR(PB.NGAY_BAN)= @nam");
            cmd.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
            cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;
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
        // ===== FIX-01 (R27 + R31) =====
        // TRUOC (R27): Save() goi CapNhatSoLuong (moi lan 1 transaction rieng) RoI MOI luu chi tiet
        //              bang ExecuteNoneQuery() - ham nay nuot loi -> chi tiet hong ma kho da tru.
        // TRUOC (R31): chi xu ly dong THEM; dong SUA khong chinh kho; SAN_PHAM.SO_LUONG khong bao gio giam.
        // SAU:  chi tiet + ton lo + ton san pham cung 1 transaction, xu ly du 3 trang thai
        //       Added / Modified / Deleted; loi thi rollback toan bo va nem exception ro rang.

        public bool Save()
        {
            List<TonKhoEventArgs> suKien = new List<TonKhoEventArgs>();
            int soDong;
            using (SqlConnection cn = LoHangDao.MoKetNoi())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                soDong = Save(cn, tx, suKien);
                tx.Commit();
            }
            HoanTatSauCommit(suKien);
            return soDong > 0;
        }

        /// Ban dung chung transaction voi ben ngoai (P01: BanHangFacade.LuuHoaDon / UnitOfWork).
        /// Ben goi phai Commit roi goi HoanTatSauCommit(suKien).
        public int Save(SqlConnection cn, SqlTransaction tx, IList<TonKhoEventArgs> suKien)
        {
            int dem = 0;
            foreach (DataRow row in m_Ds.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    string lo = Convert.ToString(row["ID_MA_SAN_PHAM"]);
                    int sl = Convert.ToInt32(row["SO_LUONG"]);
                    ThemDong(cn, tx, row);
                    GhiTon(cn, tx, lo, -sl, suKien);
                    dem++;
                }
                else if (row.RowState == DataRowState.Deleted)
                {
                    string loCu = Convert.ToString(row["ID_MA_SAN_PHAM", DataRowVersion.Original]);
                    int slCu = Convert.ToInt32(row["SO_LUONG", DataRowVersion.Original]);
                    XoaDong(cn, tx, row);
                    GhiTon(cn, tx, loCu, +slCu, suKien);          // hoan kho
                    dem++;
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    string loCu = Convert.ToString(row["ID_MA_SAN_PHAM", DataRowVersion.Original]);
                    int slCu = Convert.ToInt32(row["SO_LUONG", DataRowVersion.Original]);
                    string loMoi = Convert.ToString(row["ID_MA_SAN_PHAM"]);
                    int slMoi = Convert.ToInt32(row["SO_LUONG"]);
                    SuaDong(cn, tx, row, loCu, loMoi);
                    if (loCu == loMoi)
                    {
                        GhiTon(cn, tx, loMoi, slCu - slMoi, suKien);   // tang SL -> tru them, giam -> hoan bot
                    }
                    else
                    {
                        GhiTon(cn, tx, loCu, +slCu, suKien);
                        GhiTon(cn, tx, loMoi, -slMoi, suKien);
                    }
                    dem++;
                }
            }
            return dem;
        }

        public void HoanTatSauCommit(IList<TonKhoEventArgs> suKien)
        {
            m_Ds.AcceptChanges();
            TonKhoSubject.MacDinh.ThongBao(suKien);   // Observer: canh bao ton thap / sap het han
        }

        private static void GhiTon(SqlConnection cn, SqlTransaction tx, string lo, int delta, IList<TonKhoEventArgs> suKien)
        {
            int tonMoi = LoHangDao.CapNhatTon(cn, tx, lo, delta);
            if (delta < 0)
            {
                MaSanPham m = LoHangDao.LayLo(cn, tx, lo);
                suKien.Add(new TonKhoEventArgs(m, tonMoi - delta, tonMoi, DateTime.Now));
            }
        }

        private static void ThemDong(SqlConnection cn, SqlTransaction tx, DataRow row)
        {
            // SO_LO / NGAY_HET_HAN / GIA_VON lay tu chinh lo -> UI khong can tu dien
            object giaXuat = row.Table.Columns.Contains("GIA_XUAT") ? row["GIA_XUAT"] : DBNull.Value;
            using (SqlCommand cmd = new SqlCommand(
                "INSERT INTO dbo.CHI_TIET_PHIEU_BAN (ID_PHIEU_BAN, ID_MA_SAN_PHAM, SO_LUONG, DON_GIA, THANH_TIEN, SO_LO, NGAY_HET_HAN, GIA_VON, GIA_XUAT) " +
                "SELECT @pb, m.ID, @sl, @dg, @tt, m.ID, m.NGAY_HET_HAN, m.DON_GIA_NHAP, @gx FROM dbo.MA_SAN_PHAM m WHERE m.ID = @lo", cn, tx))
            {
                cmd.Parameters.Add("@pb", SqlDbType.NVarChar, 50).Value = Convert.ToString(row["ID_PHIEU_BAN"]);
                cmd.Parameters.Add("@lo", SqlDbType.NVarChar, 50).Value = Convert.ToString(row["ID_MA_SAN_PHAM"]);
                cmd.Parameters.Add("@sl", SqlDbType.Int).Value = Convert.ToInt32(row["SO_LUONG"]);
                cmd.Parameters.Add("@dg", SqlDbType.Int).Value = Convert.ToInt32(row["DON_GIA"]);
                cmd.Parameters.Add("@tt", SqlDbType.Int).Value = Convert.ToInt32(row["THANH_TIEN"]);
                cmd.Parameters.Add("@gx", SqlDbType.Int).Value = giaXuat == null || giaXuat == DBNull.Value ? (object)DBNull.Value : Convert.ToInt32(giaXuat);
                if (cmd.ExecuteNonQuery() != 1)
                    throw new TonKhoKhongDuException(Convert.ToString(row["ID_MA_SAN_PHAM"]), 0);
            }
        }

        private static void XoaDong(SqlConnection cn, SqlTransaction tx, DataRow row)
        {
            using (SqlCommand cmd = new SqlCommand(
                "DELETE FROM dbo.CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @pb AND ID_MA_SAN_PHAM = @lo", cn, tx))
            {
                cmd.Parameters.Add("@pb", SqlDbType.NVarChar, 50).Value = Convert.ToString(row["ID_PHIEU_BAN", DataRowVersion.Original]);
                cmd.Parameters.Add("@lo", SqlDbType.NVarChar, 50).Value = Convert.ToString(row["ID_MA_SAN_PHAM", DataRowVersion.Original]);
                cmd.ExecuteNonQuery();
            }
        }

        private static void SuaDong(SqlConnection cn, SqlTransaction tx, DataRow row, string loCu, string loMoi)
        {
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE dbo.CHI_TIET_PHIEU_BAN SET ID_MA_SAN_PHAM = @loMoi, SO_LO = @loMoi, SO_LUONG = @sl, DON_GIA = @dg, THANH_TIEN = @tt, " +
                "NGAY_HET_HAN = (SELECT NGAY_HET_HAN FROM dbo.MA_SAN_PHAM WHERE ID = @loMoi) " +
                "WHERE ID_PHIEU_BAN = @pb AND ID_MA_SAN_PHAM = @loCu", cn, tx))
            {
                cmd.Parameters.Add("@pb", SqlDbType.NVarChar, 50).Value = Convert.ToString(row["ID_PHIEU_BAN", DataRowVersion.Original]);
                cmd.Parameters.Add("@loCu", SqlDbType.NVarChar, 50).Value = loCu;
                cmd.Parameters.Add("@loMoi", SqlDbType.NVarChar, 50).Value = loMoi;
                cmd.Parameters.Add("@sl", SqlDbType.Int).Value = Convert.ToInt32(row["SO_LUONG"]);
                cmd.Parameters.Add("@dg", SqlDbType.Int).Value = Convert.ToInt32(row["DON_GIA"]);
                cmd.Parameters.Add("@tt", SqlDbType.Int).Value = Convert.ToInt32(row["THANH_TIEN"]);
                cmd.ExecuteNonQuery();
            }
        }

        // ---- ho tro UI phan lo khi lap phieu (chua luu) ----

        /// So luong tung lo dang "giu" boi cac dong CHUA luu tren phieu hien tai
        public IDictionary<string, int> LayDaGiu()
        {
            Dictionary<string, int> kq = new Dictionary<string, int>();
            foreach (DataRow row in m_Ds.Rows)
            {
                if (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Unchanged) continue;
                string lo = Convert.ToString(row["ID_MA_SAN_PHAM"]);
                int them = Convert.ToInt32(row["SO_LUONG"]);
                if (row.RowState == DataRowState.Modified &&
                    Convert.ToString(row["ID_MA_SAN_PHAM", DataRowVersion.Original]) == lo)
                    them -= Convert.ToInt32(row["SO_LUONG", DataRowVersion.Original]);
                int hienCo;
                kq[lo] = (kq.TryGetValue(lo, out hienCo) ? hienCo : 0) + them;
            }
            return kq;
        }

        /// PK (ID_PHIEU_BAN, ID_MA_SAN_PHAM) khong cho 2 dong cung lo -> UI tim dong cu de cong don
        public DataRow TimDongTheoLo(string idLo)
        {
            foreach (DataRow row in m_Ds.Rows)
            {
                if (row.RowState != DataRowState.Deleted && Convert.ToString(row["ID_MA_SAN_PHAM"]) == idLo)
                    return row;
            }
            return null;
        }
    }
}
