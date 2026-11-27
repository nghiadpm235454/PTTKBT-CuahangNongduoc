using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    public partial class frmThongtinCuahang : Form
    {
        ComboBox cmbXuatKho = new ComboBox();
        ComboBox cmbGiaXuat = new ComboBox();

        public frmThongtinCuahang()
        {
            InitializeComponent();
            KhoiTaoCauHinhKho();
        }

        private void KhoiTaoCauHinhKho()
        {
            Label lblXuatKho = new Label();
            lblXuatKho.AutoSize = true;
            lblXuatKho.Location = new Point(26, 150);
            lblXuatKho.Text = "Xuất kho";

            cmbXuatKho.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbXuatKho.Location = new Point(106, 147);
            cmbXuatKho.Size = new Size(184, 21);
            cmbXuatKho.Items.AddRange(new object[] { "FIFO (theo HSD)", "Chỉ định lô" });

            Label lblGiaXuat = new Label();
            lblGiaXuat.AutoSize = true;
            lblGiaXuat.Location = new Point(26, 178);
            lblGiaXuat.Text = "Giá xuất";

            cmbGiaXuat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGiaXuat.Location = new Point(106, 175);
            cmbGiaXuat.Size = new Size(184, 21);
            cmbGiaXuat.Items.AddRange(new object[] { "Bình quân gia quyền (BQGQ)", "FIFO" });

            this.Controls.Add(lblXuatKho);
            this.Controls.Add(cmbXuatKho);
            this.Controls.Add(lblGiaXuat);
            this.Controls.Add(cmbGiaXuat);

            this.btnOK.Location = new Point(this.btnOK.Location.X, 208);
            this.ClientSize = new Size(this.ClientSize.Width, 248);
        }

        private void frmThongtinCuahang_Load(object sender, EventArgs e)
        {
            CuaHang ch = ThamSo.LayCuaHang();
            txtTenCuaHang.Text = ch.TenCuaHang;
            txtDienThoai.Text = ch.DienThoai;
            txtDiaChi.Text = ch.DiaChi;

            cmbXuatKho.SelectedIndex = (CauHinhKho.LayPpXuatKho() == "CHI_DINH") ? 1 : 0;
            cmbGiaXuat.SelectedIndex = (CauHinhKho.LayPpGiaXuat() == "FIFO") ? 1 : 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PhuongPhapXuatKho xuatKho = cmbXuatKho.SelectedIndex == 1 ? PhuongPhapXuatKho.ChiDinh : PhuongPhapXuatKho.FIFO;
            PhuongPhapGiaXuat giaXuat = cmbGiaXuat.SelectedIndex == 1 ? PhuongPhapGiaXuat.FIFO : PhuongPhapGiaXuat.BQGQ;
            ThamSo.GanCuaHang(txtTenCuaHang.Text, txtDiaChi.Text, txtDienThoai.Text, xuatKho, giaXuat);
            this.Close();
        }
    }
}