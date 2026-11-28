using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class DichVuPhieuBanController
    {
        DichVuPhieuBanFactory factory = new DichVuPhieuBanFactory();

        public IList<DichVuPhieuBan> LayDichVu(String idPhieuBan)
        {
            IList<DichVuPhieuBan> ds = new List<DichVuPhieuBan>();
            DataTable tbl = factory.LayDichVu(idPhieuBan);
            foreach (DataRow row in tbl.Rows)
            {
                ds.Add(new DichVuPhieuBan(Convert.ToString(row["TEN_DICH_VU"]),
                                          Convert.ToInt64(row["SO_TIEN"])));
            }
            return ds;
        }

        // Ghi đè toàn bộ dịch vụ của 1 phiếu (xóa cũ, thêm mới)
        public bool Luu(String idPhieuBan, IList<DichVuPhieuBan> dsMoi)
        {
            DataTable tbl = factory.LayDichVu(idPhieuBan);
            int n = tbl.Rows.Count;
            for (int i = 0; i < n; i++)
            {
                tbl.Rows[i].Delete();
            }
            foreach (DichVuPhieuBan dv in dsMoi)
            {
                DataRow row = factory.NewRow();
                row["ID_PHIEU_BAN"] = idPhieuBan;
                row["TEN_DICH_VU"] = dv.TenDichVu;
                row["SO_TIEN"] = dv.SoTien;
                factory.Add(row);
            }
            return factory.Save();
        }
    }
}
