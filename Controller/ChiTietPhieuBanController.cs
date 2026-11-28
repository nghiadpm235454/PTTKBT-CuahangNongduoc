using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;
using System.Windows.Forms;
using System.Data;

namespace CuahangNongduoc.Controller
{
    public class ChiTietPhieuBanController
    {
        ChiTietPhieuBanFactory factory = new ChiTietPhieuBanFactory();

        public void HienThiChiTiet(DataGridView dgv, String idPhieuBan)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = factory.LayChiTietPhieuBan(idPhieuBan);
            dgv.DataSource = bs;
        }
        public DataRow NewRow()
        {
            return factory.NewRow();
        }
        public void Add(DataRow row)
        {
            factory.Add(row);
        }

        // [R33 Save() trả kết quả chuẩn]
        public bool Save()
        {
            return factory.Save();
        }

        // TRUOC: public void Save() { factory.Save(); }   (loi bi nuot)
        // SAU:   tra ket qua + thong bao loi de form hien thi
        public bool Save(out string loi)
        {
            loi = null;
            try
            {
                factory.Save();
                return true;
            }
            catch (TonKhoKhongDuException ex)
            {
                loi = ex.Message;
                return false;
            }
        }

        public IDictionary<string, int> LayDaGiu()
        {
            return factory.LayDaGiu();
        }

        public DataRow TimDongTheoLo(string idLo)
        {
            return factory.TimDongTheoLo(idLo);
        }

        private ChiTietPhieuBan DongSangChiTiet(DataRow row)
        {
            MaSanPhamController ctrl = new MaSanPhamController();
            ChiTietPhieuBan ct = new ChiTietPhieuBan();
            ct.DonGia = Convert.ToInt64(row["DON_GIA"]);
            ct.GiaXuat = row["GIA_XUAT"] == DBNull.Value ? 0 : Convert.ToInt64(row["GIA_XUAT"]);
            ct.SoLuong = Convert.ToInt32(row["SO_LUONG"]);
            ct.ThanhTien = Convert.ToInt64(row["THANH_TIEN"]);
            ct.MaSanPham = ctrl.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]));
            return ct;
        }

        private IList<ChiTietPhieuBan> DongSangDanhSach(DataTable tbl)
        {
            IList<ChiTietPhieuBan> ds = new List<ChiTietPhieuBan>();
            foreach (DataRow row in tbl.Rows)
            {
                ds.Add(DongSangChiTiet(row));
            }
            return ds;
        }

        public IList<ChiTietPhieuBan> ChiTietPhieuBan(String idPhieuBan)
        {
            return DongSangDanhSach(factory.LayChiTietPhieuBan(idPhieuBan));
        }

        public IList<ChiTietPhieuBan> ChiTietPhieuBan(DateTime dtNgayBan)
        {
            return DongSangDanhSach(factory.LayChiTietPhieuBan(dtNgayBan));
        }

        public IList<ChiTietPhieuBan> ChiTietPhieuBan(int thang, int nam)
        {
            return DongSangDanhSach(factory.LayChiTietPhieuBan(thang, nam));
        }
    }
}
