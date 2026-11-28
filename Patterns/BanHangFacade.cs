using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc
{
    public class LoiNghiepVu : Exception
    {
        public LoiNghiepVu(string thongBao) : base(thongBao) { }
    }

    public class ChiTietBanDto
    {
        public string IdMaSanPham;
        public decimal DonGia;
        public int SoLuong;
        public decimal ThanhTien;
    }

    public class PhieuBanDto
    {
        public string Id;
        public string IdKhachHang;
        public DateTime NgayBan;
        public decimal TongTien;
        public decimal DaTra;
        public decimal ConNo;
        public readonly List<ChiTietBanDto> ChiTiet = new List<ChiTietBanDto>();
    }

    // =====================================================================
    // MAU: FACADE (P01 Chu Ba)  - CN-10
    // LuuPhieuBan gom MOT giao dich: kiem tra ma -> ghi phieu -> tru ton tung lo
    // (UPDATE co dieu kien SO_LUONG >= @sl) -> ghi chi tiet -> nang bo dem THAM_SO
    // -> Commit. Loi o buoc nao cung Rollback toan bo.
    // Sua loi cu: ChiTietPhieuBanFactory.Save tru kho truoc, roi moi luu phieu o giao dich khac.
    // =====================================================================
    public class BanHangFacade
    {
        public string LuuPhieuBan(PhieuBanDto p)
        {
            KiemTra(p);

            UnitOfWork uow = new UnitOfWork();
            try
            {
                SqlCommand kt = uow.TaoLenh("SELECT COUNT(*) FROM PHIEU_BAN WHERE ID = @id");
                kt.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = p.Id;
                if (Convert.ToInt32(kt.ExecuteScalar()) > 0)
                    throw new LoiNghiepVu("Ma phieu ban " + p.Id + " da ton tai.");

                SqlCommand pb = uow.TaoLenh(
                    "INSERT INTO PHIEU_BAN (ID, ID_KHACH_HANG, NGAY_BAN, TONG_TIEN, DA_TRA, CON_NO, GIAO_HANG) " +
                    "VALUES (@id, @kh, @ngay, @tong, @datra, @conno, 0)");
                pb.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = p.Id;
                pb.Parameters.Add("@kh", SqlDbType.NVarChar, 50).Value = p.IdKhachHang;
                pb.Parameters.Add("@ngay", SqlDbType.Date).Value = p.NgayBan.Date;
                pb.Parameters.Add("@tong", SqlDbType.Decimal).Value = p.TongTien;
                pb.Parameters.Add("@datra", SqlDbType.Decimal).Value = p.DaTra;
                pb.Parameters.Add("@conno", SqlDbType.Decimal).Value = p.ConNo;
                pb.ExecuteNonQuery();

                foreach (ChiTietBanDto d in p.ChiTiet)
                {
                    SqlCommand tru = uow.TaoLenh(
                        "UPDATE MA_SAN_PHAM SET SO_LUONG = SO_LUONG - @sl WHERE ID = @lo AND SO_LUONG >= @sl");
                    tru.Parameters.Add("@sl", SqlDbType.Int).Value = d.SoLuong;
                    tru.Parameters.Add("@lo", SqlDbType.NVarChar, 50).Value = d.IdMaSanPham;
                    if (tru.ExecuteNonQuery() != 1)
                        throw new LoiNghiepVu("Lo " + d.IdMaSanPham + " khong ton tai hoac khong du hang (can " + d.SoLuong + ").");

                    SqlCommand ct = uow.TaoLenh(
                        "INSERT INTO CHI_TIET_PHIEU_BAN (ID_MA_SAN_PHAM, ID_PHIEU_BAN, DON_GIA, SO_LUONG, THANH_TIEN) " +
                        "VALUES (@lo, @pb, @dg, @sl, @tt)");
                    ct.Parameters.Add("@lo", SqlDbType.NVarChar, 50).Value = d.IdMaSanPham;
                    ct.Parameters.Add("@pb", SqlDbType.NVarChar, 50).Value = p.Id;
                    ct.Parameters.Add("@dg", SqlDbType.Decimal).Value = d.DonGia;
                    ct.Parameters.Add("@sl", SqlDbType.Int).Value = d.SoLuong;
                    ct.Parameters.Add("@tt", SqlDbType.Decimal).Value = d.ThanhTien;
                    ct.ExecuteNonQuery();
                }

                long so;
                if (long.TryParse(p.Id, out so))
                {
                    // dem ma phieu chi tang, khong bao gio giam
                    SqlCommand dem = uow.TaoLenh("UPDATE THAM_SO SET PHIEU_BAN = @moi WHERE PHIEU_BAN <= @cu");
                    dem.Parameters.Add("@moi", SqlDbType.BigInt).Value = so + 1;
                    dem.Parameters.Add("@cu", SqlDbType.BigInt).Value = so;
                    dem.ExecuteNonQuery();
                }

                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                if (!(ex is LoiNghiepVu)) AppLogger.Instance.Loi("LuuPhieuBan", ex);
                throw;
            }
            finally
            {
                uow.Dispose();
            }

            AppLogger.Instance.ThongTin("Da luu phieu ban " + p.Id + ", tong " + p.TongTien.ToString("#,##0"));
            return p.Id;
        }

        private static void KiemTra(PhieuBanDto p)
        {
            if (p == null) throw new LoiNghiepVu("Phieu ban rong.");
            if (string.IsNullOrEmpty(p.Id)) throw new LoiNghiepVu("Chua co ma phieu ban.");
            if (string.IsNullOrEmpty(p.IdKhachHang)) throw new LoiNghiepVu("Chua chon khach hang.");
            if (p.ChiTiet.Count == 0) throw new LoiNghiepVu("Phieu ban chua co mat hang.");
            foreach (ChiTietBanDto d in p.ChiTiet)
                if (d.SoLuong <= 0) throw new LoiNghiepVu("So luong lo " + d.IdMaSanPham + " phai lon hon 0.");
        }
    }
}
