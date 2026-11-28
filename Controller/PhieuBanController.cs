using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;
using System.Windows.Forms;
using System.Data;

namespace CuahangNongduoc.Controller
{
    
    public class PhieuBanController
    {
        PhieuBanFactory factory = new PhieuBanFactory();

        BindingSource bs = new BindingSource();


        public PhieuBanController()
        {
            bs.DataSource = factory.LayPhieuBan("-1");
        }
        public DataRow NewRow()
        {
            return factory.NewRow();
        }
        public void Add(DataRow row)
        {
            factory.Add(row);
        }
        public void Update()
        {
            bs.MoveNext();
            factory.Save();
        }
        // FIX-04 (R14) PhieuBanController.Save bỏ MessageBox -> trả SaveResult
        // Trước:
        //   public void Save() {
        //       int n = PhieuBanFactory.LaySoPhieu();
        //       if (n >= 50) MessageBox.Show("Đây là bản dùng thử! Chỉ lưu được 50 phiếu bán!", ...);
        //       else { MessageBox.Show("... còn " + (50-n) + " phiếu bán!", ...); factory.Save(); }
        //   }
        //   -> Controller tự bật MessageBox, form không biết lưu thành công hay không.
        // Sau: Controller chỉ trả kết quả, form quyết định hiển thị.
        public SaveResult Save()
        {
            int n = PhieuBanFactory.LaySoPhieu();
            if (n >= 50)
            {
                return SaveResult.Fail("Đây là bản dùng thử! Chỉ lưu được 50 phiếu bán!");
            }
            if (!factory.Save())
            {
                return SaveResult.Fail("Không lưu được phiếu bán!");
            }
            return SaveResult.Ok("Đây là bản dùng thử! Chỉ lưu được thêm " + Convert.ToString(50 - n - 1) + " phiếu bán!");
        }
        public void HienthiPhieuBanLe(BindingNavigator bn, DataGridView dg)
        {

            bs.DataSource = factory.DanhsachPhieuBanLe();
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiPhieuBanSi(BindingNavigator bn, DataGridView dg)
        {

            bs.DataSource = factory.DanhsachPhieuBanSi();
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiPhieuBan(BindingNavigator bn,ComboBox cmb, TextBox txt, DateTimePicker dt, NumericUpDown numTongTien, NumericUpDown numDatra, NumericUpDown numConNo)
        {

            bn.BindingSource = bs;

            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_KHACH_HANG");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_BAN");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            numDatra.DataBindings.Clear();
            numDatra.DataBindings.Add("Value", bs, "DA_TRA");

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", bs, "CON_NO");


        }

        public PhieuBan LayPhieuBan(String id)
        {
            DataTable tbl = factory.LayPhieuBan(id);
            PhieuBan ph = null;
            if (tbl.Rows.Count > 0)
            {

                ph = new PhieuBan();
                ph.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                
                ph.NgayBan = Convert.ToDateTime(tbl.Rows[0]["NGAY_BAN"]);
                ph.TongTien = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
                ph.DaTra = Convert.ToInt64(tbl.Rows[0]["DA_TRA"]);
                ph.ConNo = Convert.ToInt64(tbl.Rows[0]["CON_NO"]);
                KhachHangController ctrlKH = new KhachHangController();
                ph.KhachHang = ctrlKH.LayKhachHang(Convert.ToString(tbl.Rows[0]["ID_KHACH_HANG"]));
                ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();
                ph.ChiTiet = ctrl.ChiTietPhieuBan(ph.Id);
                DataRow r = tbl.Rows[0];
                ph.TienHang = DocLong(r, "TIEN_HANG");
                ph.TongDichVu = DocLong(r, "TONG_DICH_VU");
                ph.GiaoHang = DocLong(r, "GIAO_HANG") != 0;
                ph.KhoangCachKm = DocDouble(r, "KHOANG_CACH_KM");
                ph.PhiVanChuyen = DocLong(r, "PHI_VAN_CHUYEN");
                ph.ChietKhau = DocLong(r, "CHIET_KHAU");
                ph.TienKhuyenMai = DocLong(r, "TIEN_KHUYEN_MAI");
                ph.TienGiamGia = DocLong(r, "TIEN_GIAM_GIA");
                ph.IdNguoiDung = (int)DocLong(r, "ID_NGUOI_DUNG");

                ThongTinGiamGia gg = new ThongTinGiamGia();
                gg.Loai = LoaiGiamGia.TuMa(DocChuoi(r, "LOAI_GIAM_GIA"));
                gg.TyLe = DocDouble(r, "TY_LE_GIAM_GIA");
                gg.SoTien = (gg.Loai == LoaiGiamGia.TienMat) ? DocLong(r, "TIEN_GIAM_GIA") : 0;
                gg.QuyTac = DocChuoi(r, "QUY_TAC_GIAM_GIA");
                ph.GiamGia = gg;

                ph.DichVu = new DichVuPhieuBanController().LayDichVu(ph.Id);
            }
            return ph;
        }

        static long DocLong(DataRow r, String col)
        {
            if (!r.Table.Columns.Contains(col) || Convert.IsDBNull(r[col])) return 0;
            return Convert.ToInt64(r[col]);
        }
        static double DocDouble(DataRow r, String col)
        {
            if (!r.Table.Columns.Contains(col) || Convert.IsDBNull(r[col])) return 0;
            return Convert.ToDouble(r[col]);
        }
        static String DocChuoi(DataRow r, String col)
        {
            if (!r.Table.Columns.Contains(col) || Convert.IsDBNull(r[col])) return "";
            return Convert.ToString(r[col]);
        }

        // Form bán hàng gọi trong ThemMoi(): tính tổng rồi ghi các cột mới vào DataRow
        public void GhiThongTinHoaDon(DataRow row, PhieuBan pb)
        {
            CuahangNongduoc.Patterns.TinhHoaDonService.CapNhatTong(pb);

            row["TONG_TIEN"] = pb.TongTien;
            row["CON_NO"] = pb.ConNo;
            row["TIEN_HANG"] = pb.TienHang;
            row["TONG_DICH_VU"] = pb.TongDichVu;
            row["GIAO_HANG"] = pb.GiaoHang;
            row["KHOANG_CACH_KM"] = pb.KhoangCachKm;
            row["PHI_VAN_CHUYEN"] = pb.PhiVanChuyen;
            row["CHIET_KHAU"] = pb.ChietKhau;
            row["TIEN_KHUYEN_MAI"] = pb.TienKhuyenMai;
            row["LOAI_GIAM_GIA"] = pb.GiamGia.Loai.Ma;
            row["TY_LE_GIAM_GIA"] = pb.GiamGia.TyLe;
            row["QUY_TAC_GIAM_GIA"] = pb.GiamGia.QuyTac ?? "";
            row["TIEN_GIAM_GIA"] = pb.TienGiamGia;
            if (pb.IdNguoiDung > 0) row["ID_NGUOI_DUNG"] = pb.IdNguoiDung;
        }

        public void TimPhieuBan(String maKH, DateTime dt)
        {
            factory.TimPhieuBan(maKH, dt);

        }

    }
}
