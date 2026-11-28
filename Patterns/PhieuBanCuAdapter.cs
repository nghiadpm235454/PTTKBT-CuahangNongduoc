using System;
using System.Data;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc
{
    // =====================================================================
    // MAU: ADAPTER (P01 Chu Ba)
    // Giao dien moi ma phan con lai mong doi: IDocPhieuBan (tra PhieuBanDto).
    // DAO cu (PhieuBanFactory + ChiTietPhieuBanFactory) tra DataTable, ten cot rieng.
    // PhieuBanCuAdapter "dich" giua hai ben, khong phai sua DAO cu.
    // =====================================================================
    public interface IDocPhieuBan
    {
        PhieuBanDto Doc(string id);
    }

    public class PhieuBanCuAdapter : IDocPhieuBan
    {
        private readonly PhieuBanFactory phieu;
        private readonly ChiTietPhieuBanFactory chiTiet;

        public PhieuBanCuAdapter() : this(new PhieuBanFactory(), new ChiTietPhieuBanFactory()) { }

        public PhieuBanCuAdapter(PhieuBanFactory phieu, ChiTietPhieuBanFactory chiTiet)
        {
            this.phieu = phieu;
            this.chiTiet = chiTiet;
        }

        public PhieuBanDto Doc(string id)
        {
            DataTable pb = phieu.LayPhieuBan(id);
            if (pb.Rows.Count == 0) return null;
            DataRow r = pb.Rows[0];

            PhieuBanDto p = new PhieuBanDto();
            p.Id = Convert.ToString(r["ID"]);
            p.IdKhachHang = Convert.ToString(r["ID_KHACH_HANG"]);
            p.NgayBan = Convert.ToDateTime(r["NGAY_BAN"]);
            p.TongTien = Convert.ToDecimal(r["TONG_TIEN"]);
            p.DaTra = Convert.ToDecimal(r["DA_TRA"]);
            p.ConNo = Convert.ToDecimal(r["CON_NO"]);

            foreach (DataRow ct in chiTiet.LayChiTietPhieuBan(id).Rows)
            {
                ChiTietBanDto d = new ChiTietBanDto();
                d.IdMaSanPham = Convert.ToString(ct["ID_MA_SAN_PHAM"]);
                d.DonGia = Convert.ToDecimal(ct["DON_GIA"]);
                d.SoLuong = Convert.ToInt32(ct["SO_LUONG"]);
                d.ThanhTien = Convert.ToDecimal(ct["THANH_TIEN"]);
                p.ChiTiet.Add(d);
            }
            return p;
        }
    }
}
