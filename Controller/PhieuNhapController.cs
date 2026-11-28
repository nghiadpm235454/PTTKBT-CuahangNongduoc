using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;
using System.Windows.Forms;
using System.Data;

namespace CuahangNongduoc.Controller
{
    
    public class PhieuNhapController
    {
        PhieuNhapFactory factory = new PhieuNhapFactory();
        BindingSource bs = new BindingSource();

        public PhieuNhapController()
        {
            bs.DataSource = factory.LayPhieuNhap("-1");
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
        // [R33 Save() trả kết quả chuẩn]
        // Trước: public void Save() { factory.Save(); }   (nuốt kết quả, 6 controller khác đã trả bool)
        // Sau: public bool Save() { return factory.Save(); }  (giống KhachHang/SanPham/DonViTinh... ; nơi gọi cũ bỏ qua giá trị trả về vẫn chạy)
        public bool Save()
        {
            return factory.Save();
        }

    
        public PhieuNhap LayPhieuNhap(String id)
        {
            DataTable tbl = factory.LayPhieuNhap(id);
            PhieuNhap ph = null;
            NhaCungCapController ctrlNCC = new NhaCungCapController();
            if (tbl.Rows.Count > 0)
            {
                DataRow row = tbl.Rows[0];
                ph = new PhieuNhap();
                ph.Id = Convert.ToString(row["ID"]);
                ph.NgayNhap = row["NGAY_NHAP"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NGAY_NHAP"]);
                ph.TongTien = row["TONG_TIEN"] == DBNull.Value ? 0 : Convert.ToInt64(row["TONG_TIEN"]);
                ph.DaTra = row["DA_TRA"] == DBNull.Value ? 0 : Convert.ToInt64(row["DA_TRA"]);
                ph.ConNo = row["CON_NO"] == DBNull.Value ? 0 : Convert.ToInt64(row["CON_NO"]);
                ph.NhaCungCap = ctrlNCC.LayNCC(Convert.ToString(row["ID_NHA_CUNG_CAP"]));
                MaSanPhamController ctrl = new MaSanPhamController();
                ph.ChiTiet = ctrl.ChiTietPhieuNhap(ph.Id);
            }
            return ph;
        }
        public void HienthiPhieuNhap(BindingNavigator bn, DataGridView dg)
        {
            
            bs.DataSource = factory.DanhsachPhieuNhap();
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiPhieuNhap(BindingNavigator bn,TextBox txt,ComboBox cmb, DateTimePicker dt, NumericUpDown numTongTien, NumericUpDown numDaTra, NumericUpDown numConNo)
        {

            bn.BindingSource = bs;

            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs,"ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_NHA_CUNG_CAP");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_NHAP");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            numDaTra.DataBindings.Clear();
            numDaTra.DataBindings.Add("Value", bs, "DA_TRA");

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", bs, "CON_NO");
            
        }

        public void TimPhieuNhap(String maNCC, DateTime dt)
        {
            factory.TimPhieuNhap(maNCC, dt);
        }
   
    }
}
