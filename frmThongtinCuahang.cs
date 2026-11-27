using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmThongtinCuahang : Form
    {
        public frmThongtinCuahang()
        {
            InitializeComponent();
        }

        private void frmThongtinCuahang_Load(object sender, EventArgs e)
        {
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            txtTenCuaHang.Text = ch.TenCuaHang;
            txtDienThoai.Text = ch.DienThoai;
            txtDiaChi.Text = ch.DiaChi;

            cmbXuatKho.SelectedIndex = (CauHinhKho.LayPpXuatKho() == "CHI_DINH") ? 1 : 0;
            cmbGiaXuat.SelectedIndex = (CauHinhKho.LayPpGiaXuat() == "FIFO") ? 1 : 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            CuahangNongduoc.BusinessObject.CuaHang ch = new CuahangNongduoc.BusinessObject.CuaHang();
            ch.TenCuaHang = txtTenCuaHang.Text;
            ch.DiaChi = txtDiaChi.Text;
            ch.DienThoai = txtDienThoai.Text;
            ThamSo.GanCuaHang(ch);
            CauHinhKho.GanPpXuatKho(cmbXuatKho.SelectedIndex == 1 ? "CHI_DINH" : "FIFO");
            CauHinhKho.GanPpGiaXuat(cmbGiaXuat.SelectedIndex == 1 ? "FIFO" : "BQGQ");
            this.Close();
        }
    }
}