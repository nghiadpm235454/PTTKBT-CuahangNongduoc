using System;
using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc.Test
{
    // Goi tam tu Program.Main trong ban Debug: CuahangNongduoc.Test.XuatKhoServiceTest.ChayThu();
    public static class XuatKhoServiceTest
    {
        static MaSanPham Lo(string id, int sl, DateTime hsd)
        {
            MaSanPham m = new MaSanPham(id, sl);
            m.NgayHetHan = hsd;
            m.NgayNhap = DateTime.Today;
            return m;
        }

        public static void ChayThu()
        {
            DateTime hom = new DateTime(2026, 11, 24);
            IList<MaSanPham> ds = new List<MaSanPham>();
            ds.Add(Lo("L2", 20, hom.AddDays(60)));
            ds.Add(Lo("L1", 10, hom.AddDays(15)));
            ds.Add(Lo("L3", 15, hom.AddDays(-3)));      // het han -> phai bi bo qua

            KetQuaPhanLo kq = XuatKhoService.PhanLoFIFO(ds, 25, hom, null);
            Kiem(kq.ThanhCong && kq.Dong.Count == 2, "FIFO 25: 2 dong");
            Kiem(kq.Dong[0].SoLo == "L1" && kq.Dong[0].SoLuong == 10, "FIFO 25: L1 = 10");
            Kiem(kq.Dong[1].SoLo == "L2" && kq.Dong[1].SoLuong == 15, "FIFO 25: L2 = 15");

            kq = XuatKhoService.PhanLoFIFO(ds, 40, hom, null);   // chi co 30 con han
            Kiem(!kq.ThanhCong && kq.ConThieu == 10, "FIFO 40: thieu 10");

            kq = XuatKhoService.XuatLoChiDinh(ds[2], 1, hom, null);   // L3 het han
            Kiem(!kq.ThanhCong, "Chi dinh lo het han bi tu choi");

            Dictionary<string, int> daGiu = new Dictionary<string, int>();
            daGiu["L1"] = 10;                                    // L1 da bi giu het tren phieu
            kq = XuatKhoService.PhanLoFIFO(ds, 5, hom, daGiu);
            Kiem(kq.ThanhCong && kq.Dong[0].SoLo == "L2", "FIFO bo qua phan da giu");

            System.Windows.Forms.MessageBox.Show("XuatKhoServiceTest: TAT CA DAT");
        }

        static void Kiem(bool dieuKien, string ten)
        {
            if (!dieuKien) throw new Exception("TEST FAIL: " + ten);
        }
    }
}
