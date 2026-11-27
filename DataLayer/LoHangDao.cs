using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.DataLayer
{
    public static class LoHangDao
    {
        // Dung chung chuoi ket noi voi DataService cua nhom
        public static string ChuoiKetNoi
        {
            get { return DataService.ChuoiKetNoi; }
            set { DataService.ChuoiKetNoi = value; }
        }

        public static SqlConnection MoKetNoi()
        {
            SqlConnection cn = new SqlConnection(DataService.ChuoiKetNoi);
            cn.Open();
            return cn;
        }

        private const string CHON_LO =
            "SELECT ID, ID_SAN_PHAM, ID_PHIEU_NHAP, DON_GIA_NHAP, SO_LUONG, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN FROM dbo.MA_SAN_PHAM ";

        public static IList<MaSanPham> LayCacLoConHang(string idSanPham)
        {
            using (SqlConnection cn = MoKetNoi())
            using (SqlCommand cmd = new SqlCommand(CHON_LO +
                "WHERE ID_SAN_PHAM = @sp AND SO_LUONG > 0 ORDER BY NGAY_HET_HAN, NGAY_NHAP, ID", cn))
            {
                cmd.Parameters.Add("@sp", SqlDbType.NVarChar, 50).Value = idSanPham;
                IList<MaSanPham> ds = new List<MaSanPham>();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) ds.Add(DocMotLo(rd));
                }
                return ds;
            }
        }

        public static MaSanPham LayLo(string idLo)
        {
            using (SqlConnection cn = MoKetNoi())
            {
                return LayLo(cn, null, idLo);
            }
        }

        public static MaSanPham LayLo(SqlConnection cn, SqlTransaction tx, string idLo)
        {
            using (SqlCommand cmd = new SqlCommand(CHON_LO + "WHERE ID = @id", cn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idLo;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    return rd.Read() ? DocMotLo(rd) : null;
                }
            }
        }

        /// Cong (delta > 0) hoac tru (delta < 0) ton cua LO va cua SAN PHAM trong CUNG transaction.
        /// Tru qua ton -> nem TonKhoKhongDuException. Tra ve ton moi cua lo.
        public static int CapNhatTon(SqlConnection cn, SqlTransaction tx, string idLo, int delta)
        {
            object tonMoi;
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE dbo.MA_SAN_PHAM SET SO_LUONG = ISNULL(SO_LUONG,0) + @d " +
                "OUTPUT inserted.SO_LUONG " +
                "WHERE ID = @id AND ISNULL(SO_LUONG,0) + @d >= 0", cn, tx))
            {
                cmd.Parameters.Add("@d", SqlDbType.Int).Value = delta;
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idLo;
                tonMoi = cmd.ExecuteScalar();
            }
            if (tonMoi == null) throw new TonKhoKhongDuException(idLo, delta);

            using (SqlCommand cmd2 = new SqlCommand(
                "UPDATE dbo.SAN_PHAM SET SO_LUONG = ISNULL(SO_LUONG,0) + @d " +
                "WHERE ID = (SELECT ID_SAN_PHAM FROM dbo.MA_SAN_PHAM WHERE ID = @id)", cn, tx))
            {
                cmd2.Parameters.Add("@d", SqlDbType.Int).Value = delta;
                cmd2.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idLo;
                cmd2.ExecuteNonQuery();
            }
            return Convert.ToInt32(tonMoi);
        }

        /// Ban tu mo transaction (cho cac cho cu nhu hoan kho khi xoa ca phieu)
        public static void CapNhatTon(string idLo, int delta)
        {
            using (SqlConnection cn = MoKetNoi())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                CapNhatTon(cn, tx, idLo, delta);
                tx.Commit();   // neu loi truoc do -> Dispose tu rollback
            }
        }

        // ---- doc du lieu (cot nao cung co the NULL) ----
        private static MaSanPham DocMotLo(SqlDataReader rd)
        {
            MaSanPham lo = new MaSanPham();
            lo.Id = Chuoi(rd, "ID");
            SanPham sp = new SanPham();
            sp.Id = Chuoi(rd, "ID_SAN_PHAM");
            lo.SanPham = sp;
            lo.GiaNhap = So(rd, "DON_GIA_NHAP");
            lo.SoLuong = (int)So(rd, "SO_LUONG");
            lo.NgayNhap = Ngay(rd, "NGAY_NHAP", DateTime.MinValue);
            lo.NgaySanXuat = Ngay(rd, "NGAY_SAN_XUAT", DateTime.MinValue);
            lo.NgayHetHan = Ngay(rd, "NGAY_HET_HAN", DateTime.MaxValue);   // NULL = khong gioi han han
            return lo;
        }

        private static string Chuoi(SqlDataReader rd, string c)
        {
            int i = rd.GetOrdinal(c);
            return rd.IsDBNull(i) ? "" : rd.GetString(i);
        }

        private static long So(SqlDataReader rd, string c)
        {
            int i = rd.GetOrdinal(c);
            return rd.IsDBNull(i) ? 0 : Convert.ToInt64(rd.GetValue(i));
        }

        private static DateTime Ngay(SqlDataReader rd, string c, DateTime macDinh)
        {
            int i = rd.GetOrdinal(c);
            return rd.IsDBNull(i) ? macDinh : rd.GetDateTime(i);
        }
    }
}
