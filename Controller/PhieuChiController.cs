using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;
using System.Windows.Forms;
using System.Data;

namespace CuahangNongduoc.Controller
{
    
    public class PhieuChiController
    {
        PhieuChiFactory factory = new PhieuChiFactory();


        public DataRow NewRow()
        {
            return factory.NewRow();
        }
        public void Add(DataRow row)
        {
            factory.Add(row);
        }
        // [R33 Save() trả kết quả chuẩn]
        // Trước: public void Save() { factory.Save(); }   (nuốt kết quả, 6 controller khác đã trả bool)
        // Sau: public bool Save() { return factory.Save(); }  (giống KhachHang/SanPham/DonViTinh... ; nơi gọi cũ bỏ qua giá trị trả về vẫn chạy)
        public bool Save()
        {
            return factory.Save();
        }

        public PhieuChi LayPhieuChi(String id)
        {
            PhieuChi ph = null;
            DataTable tbl = factory.LayPhieuChi(id);
            if (tbl.Rows.Count > 0)
            {
                DataRow row = tbl.Rows[0];
                ph = new PhieuChi();
                ph.Id = Convert.ToString(row["ID"]);
                LyDoChiController ctrlLyDo = new LyDoChiController();
                long idLyDo = row["ID_LY_DO_CHI"] == DBNull.Value ? 0 : Convert.ToInt64(row["ID_LY_DO_CHI"]);
                ph.LyDoChi = ctrlLyDo.LayLyDoChi(idLyDo);
                ph.NgayChi = row["NGAY_CHI"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NGAY_CHI"]);
                ph.TongTien = row["TONG_TIEN"] == DBNull.Value ? 0 : Convert.ToInt64(row["TONG_TIEN"]);
                ph.GhiChu = Convert.ToString(row["GHI_CHU"]);
            }
            return ph;
        }

        public void GanLaiDieuKhien(BindingSource bs, BindingNavigator bn, DataGridView dg,
            ComboBox cmb, TextBox txt, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu)
        {
            bn.BindingSource = bs;
            dg.DataSource = bs;

            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_LY_DO_CHI");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_CHI");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            txtGhichu.DataBindings.Clear();
            txtGhichu.DataBindings.Add("Text", bs, "GHI_CHU");
        }

        public void HienthiPhieuChi(BindingNavigator bn, DataGridView dg, ComboBox cmb, TextBox txt, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = factory.DanhsachPhieuChi();
            GanLaiDieuKhien(bs, bn, dg, cmb, txt, dt, numTongTien, txtGhichu);
        }

        public void TimPhieuChi(BindingNavigator bn, DataGridView dg, ComboBox cmb, TextBox txt, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu,
            int lydo, DateTime ngay)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = factory.TimPhieuChi(lydo, ngay);
            GanLaiDieuKhien(bs, bn, dg, cmb, txt, dt, numTongTien, txtGhichu);
        }
    }
}
