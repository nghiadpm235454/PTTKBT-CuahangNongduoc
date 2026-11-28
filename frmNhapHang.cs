using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    public partial class frmNhapHang : Form
    {
        SanPhamController ctrlSanPham = new SanPhamController();
        PhieuNhapController ctrl = new PhieuNhapController();
        MaSanPhamController ctrlMaSP = new MaSanPhamController();
        NhaCungCapController ctrlNCC = new NhaCungCapController();
        PhieuNhap m_PhieuNhap = null;


        Controll status = Controll.Normal;
        PhieuNhapCaretaker caretaker = new PhieuNhapCaretaker();
        NhapHangMediator mediator;
        public frmNhapHang()
        {
            InitializeComponent();
            status = Controll.AddNew;
        }
        public frmNhapHang(PhieuNhapController ctrlPN)
            : this()
        {
            this.ctrl = ctrlPN;
            status = Controll.Normal;
        }


        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
                ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
        }
      
        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            mediator = new NhapHangMediator(numGiaNhap, numSoLuong, numThanhTien, numTongTien, numDaTra, numConNo);

            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlSanPham.HienthiDataGridViewComboBoxColumn(colSanPham);
            ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);


            
            ctrl.HienthiPhieuNhap(bindingNavigator, txtMaPhieu,cmbNhaCungCap, dtNgayNhap, numTongTien, numDaTra, numConNo);
            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);
            
            ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);

            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuNhap().ToString();
                BatCheDoNhap();
            }
            else
            {
                TatCheDoNhap();
            }
            ThemNutNhap();

        }

        PhieuNhapMemento TaoMemento()
        {
            PhieuNhapMemento m = new PhieuNhapMemento();
            m.MaPhieu = txtMaPhieu.Text;
            m.IdNhaCungCap = cmbNhaCungCap.SelectedValue;
            m.NgayNhap = dtNgayNhap.Value;
            m.DaTra = numDaTra.Value;
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (r.IsNewRow) continue;
                DongNhapMemento d = new DongNhapMemento();
                d.IdSanPham = Convert.ToString(r.Cells["colSanPham"].Value);
                d.MaLo = Convert.ToString(r.Cells["colMaSanPham"].Value);
                d.DonGiaNhap = (r.Cells["colDonGiaNhap"].Value == null || r.Cells["colDonGiaNhap"].Value == DBNull.Value)
                    ? 0 : Convert.ToDecimal(r.Cells["colDonGiaNhap"].Value);
                d.SoLuong = (r.Cells["colSoLuong"].Value == null || r.Cells["colSoLuong"].Value == DBNull.Value)
                    ? 0 : Convert.ToDecimal(r.Cells["colSoLuong"].Value);
                d.NgaySanXuat = (r.Cells["colNgaySanXuat"].Value == null || r.Cells["colNgaySanXuat"].Value == DBNull.Value)
                    ? dtNgaySanXuat.Value.Date : Convert.ToDateTime(r.Cells["colNgaySanXuat"].Value);
                d.NgayHetHan = (r.Cells["colNgayHetHan"].Value == null || r.Cells["colNgayHetHan"].Value == DBNull.Value)
                    ? dtNgayHetHan.Value.Date : Convert.ToDateTime(r.Cells["colNgayHetHan"].Value);
                m.Dong.Add(d);
            }
            return m;
        }

        void PhucHoiMemento(PhieuNhapMemento m)
        {
            txtMaPhieu.Text = m.MaPhieu;
            if (m.IdNhaCungCap != null)
                cmbNhaCungCap.SelectedValue = m.IdNhaCungCap;
            dtNgayNhap.Value = m.NgayNhap;

            // Xóa các dòng hiện có trên lưới trước khi khôi phục nháp để tránh trùng mã lô / nhân đôi dòng
            for (int i = dataGridView.Rows.Count - 1; i >= 0; i--)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                    dataGridView.Rows.RemoveAt(i);
            }

            decimal tong = 0;
            foreach (DongNhapMemento d in m.Dong)
            {
                DataRow row = ctrlMaSP.NewRow();
                row["ID_SAN_PHAM"] = d.IdSanPham;
                row["ID_PHIEU_NHAP"] = m.MaPhieu;
                row["ID"] = d.MaLo;
                row["DON_GIA_NHAP"] = d.DonGiaNhap;
                row["SO_LUONG"] = d.SoLuong;
                row["NGAY_NHAP"] = m.NgayNhap.Date;
                row["NGAY_SAN_XUAT"] = d.NgaySanXuat.Date;
                row["NGAY_HET_HAN"] = d.NgayHetHan.Date;
                ctrlMaSP.Add(row);
                tong += d.DonGiaNhap * d.SoLuong;
            }
            numTongTien.Value = tong;
            numDaTra.Value = m.DaTra;
        }

        void ThemNutNhap()
        {
            ToolStripButton btnLuuNhap = new ToolStripButton("Lưu nháp");
            btnLuuNhap.Click += delegate
            {
                caretaker.Luu(TaoMemento());
                MessageBox.Show("Đã lưu nháp!", "Phieu Nhap");
            };

            ToolStripButton btnKhoiPhuc = new ToolStripButton("Khôi phục nháp");
            btnKhoiPhuc.Click += delegate
            {
                if (!caretaker.CoNhap) { MessageBox.Show("Chưa có bản nháp!", "Phieu Nhap"); return; }
                if (status == Controll.Normal) return;
                PhucHoiMemento(caretaker.Lay());
            };

            bindingNavigator.Items.Add(btnLuuNhap);
            bindingNavigator.Items.Add(btnKhoiPhuc);
        }
        void CongTongTien()
        {
            numTongTien.Value += numGiaNhap.Value * numSoLuong.Value;
        }
        class DongNhapInfo
        {
            public string MaLo;
            public object IdSanPham;
            public decimal Gia;
            public decimal SoLuong;
            public DateTime NSX;
            public DateTime HSD;
        }

        DataRow TaoDong(DongNhapInfo i)
        {
            DataRow row = ctrlMaSP.NewRow();
            row["ID_SAN_PHAM"] = i.IdSanPham;
            row["ID_PHIEU_NHAP"] = txtMaPhieu.Text;
            row["ID"] = i.MaLo;
            row["DON_GIA_NHAP"] = i.Gia;
            row["SO_LUONG"] = i.SoLuong;
            row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
            row["NGAY_SAN_XUAT"] = i.NSX.Date;
            row["NGAY_HET_HAN"] = i.HSD.Date;
            return row;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {


            string maLo = txtMaSo.Text.Trim();
            MaSanPhamController ctrlCheck = new MaSanPhamController();
            MaSanPham masp = ctrlCheck.LayMaSanPham(maLo);
            if (masp == null)
            {
                foreach (DataGridViewRow view in dataGridView.Rows)
                {
                    if (view.IsNewRow) continue;
                    string maHienTai = Convert.ToString(view.Cells["colMaSanPham"].Value).Trim();
                    if (string.Equals(maLo, maHienTai, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Mã sản phẩm này đã tồn tại trong danh sách! Vui lòng nhập lại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (cmbSanPham.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Sản phẩm !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (maLo.Length <= 0)
                {
                    MessageBox.Show("Vui lòng nhập Mã sản phẩm !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (numGiaNhap.Value <= 0)
                {
                    MessageBox.Show("Vui lòng nhập Đơn giá !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (numSoLuong.Value <= 0)
                {
                    MessageBox.Show("Vui lòng nhập Số lượng !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (dtNgaySanXuat.Value.Date >= dtNgayHetHan.Value.Date)
                {
                    MessageBox.Show("Ngày hết hạn phải lớn hơn ngày sản xuất!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    CongTongTien();
                    DongNhapInfo info = new DongNhapInfo();
                    info.MaLo = maLo;
                    info.IdSanPham = cmbSanPham.SelectedValue;
                    info.Gia = numGiaNhap.Value;
                    info.SoLuong = numSoLuong.Value;
                    info.NSX = dtNgaySanXuat.Value;
                    info.HSD = dtNgayHetHan.Value;
                    ctrlMaSP.Add(TaoDong(info));
                }
            }
            else
            {
                MessageBox.Show("Mã sản phẩm này đã tồn tại! Vui lòng nhập lại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void toolLuuThoat_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            if (this.Luu())
            {
                status = Controll.Normal;
                this.TatCheDoNhap();
            }
        }

        private bool Luu()
        {
            if (status == Controll.AddNew)
            {
                return ThemMoi();
            }
            else
            {
                CapNhat();
                return true;
            }
        }
        private void CapNhat()
        {
            ctrlMaSP.Save();

            ctrl.Update();
        }
        private bool ThemMoi()
        {
            if (cmbNhaCungCap.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Nhà cung cấp!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            int soDongHopLe = 0;
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (!r.IsNewRow) soDongHopLe++;
            }
            if (soDongHopLe == 0)
            {
                MessageBox.Show("Phiếu nhập chưa có chi tiết sản phẩm!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            PhieuNhapController ctrlPN = new PhieuNhapController();
            if (ctrlPN.LayPhieuNhap(txtMaPhieu.Text.Trim()) != null)
            {
                MessageBox.Show("Mã Phiếu nhập này đã tồn tại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            DataRow row = ctrl.NewRow();
            row["ID"] = txtMaPhieu.Text.Trim();
            row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
            row["TONG_TIEN"] = numTongTien.Value;
            row["ID_NHA_CUNG_CAP"] = cmbNhaCungCap.SelectedValue;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;
            ctrl.Add(row);

            if (ThamSo.LaSoNguyen(txtMaPhieu.Text.Trim()))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text.Trim());
                if (so >= ThamSo.LayMaPhieuNhap())
                {
                    ThamSo.GanMaPhieuNhap(so + 1);
                }
            }

            ctrl.Save();
            ctrlMaSP.Save();

            SanPhamController ctrlSP = new SanPhamController();
            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow) continue;
                ctrlSP.CapNhatGiaNhap(
                    Convert.ToString(view.Cells["colSanPham"].Value),
                    Convert.ToInt64(view.Cells["colDonGiaNhap"].Value),
                    Convert.ToInt64(view.Cells["colSoLuong"].Value));
            }

            return true;
        }

        private void toolLuuThem_Click(object sender, EventArgs e)
        {
            ctrl = new PhieuNhapController();

            status = Controll.AddNew;

            txtMaPhieu.Text = ThamSo.LayMaPhieuNhap().ToString();
            numTongTien.Value = 0;
            numDaTra.Value = 0;
            numConNo.Value = 0;
            ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
            this.BatCheDoNhap();
        }

        private void toolSavePrint_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu nhập hiện tại!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

                String ma_phieu = txtMaPhieu.Text;

                PhieuNhapController ctrlPN = new PhieuNhapController();

                CuahangNongduoc.BusinessObject.PhieuNhap ph = ctrlPN.LayPhieuNhap(ma_phieu);

                frmInPhieuNhap PhieuNhap = new frmInPhieuNhap(ph);

                PhieuNhap.Show();
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu nhập này không?", "Phieu Nhap Hang", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Luu();
                }
            }
           
            this.Close();
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            GridErrorHelper.Xuly(sender, e);
        }
        void BatCheDoNhap()
        {
            SetAllow(true);
        }

        void TatCheDoNhap()
        {
            SetAllow(false);
        }
        void SetAllow(bool val)
        {
            txtMaPhieu.Enabled = val;
            cmbNhaCungCap.Enabled = val;
            dtNgayNhap.Enabled = val;
            numTongTien.Enabled = val;
            numDaTra.Enabled = val;
            numConNo.Enabled = val;
            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dataGridView.Enabled = val;
        }

        private void toolChinhsua_Click(object sender, EventArgs e)
        {
            status = Controll.Edit;
            this.BatCheDoNhap();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null || dataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Vui lòng chọn dòng chi tiết cần xóa!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Bạn chắc chắn xóa dòng hàng này không?", "Phieu Nhap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                decimal donGia = (dataGridView.CurrentRow.Cells["colDonGiaNhap"].Value == null || dataGridView.CurrentRow.Cells["colDonGiaNhap"].Value == DBNull.Value)
                    ? 0 : Convert.ToDecimal(dataGridView.CurrentRow.Cells["colDonGiaNhap"].Value);
                decimal soLuong = (dataGridView.CurrentRow.Cells["colSoLuong"].Value == null || dataGridView.CurrentRow.Cells["colSoLuong"].Value == DBNull.Value)
                    ? 0 : Convert.ToDecimal(dataGridView.CurrentRow.Cells["colSoLuong"].Value);

                decimal tongMoi = numTongTien.Value - (donGia * soLuong);
                numTongTien.Value = Math.Max(numTongTien.Minimum, tongMoi);

                dataGridView.Rows.Remove(dataGridView.CurrentRow);
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow) return;

            if (MessageBox.Show("Bạn chắc chắn xóa dòng hàng này không?", "Phieu Nhap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                decimal donGia = (e.Row.Cells["colDonGiaNhap"].Value == null || e.Row.Cells["colDonGiaNhap"].Value == DBNull.Value)
                    ? 0 : Convert.ToDecimal(e.Row.Cells["colDonGiaNhap"].Value);
                decimal soLuong = (e.Row.Cells["colSoLuong"].Value == null || e.Row.Cells["colSoLuong"].Value == DBNull.Value)
                    ? 0 : Convert.ToDecimal(e.Row.Cells["colSoLuong"].Value);

                decimal tongMoi = numTongTien.Value - (donGia * soLuong);
                numTongTien.Value = Math.Max(numTongTien.Minimum, tongMoi);
            }
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Nhap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bindingNavigator.BindingSource.RemoveCurrent();
                ctrl.Save();
            }
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            frmNhaCungCap NCC = new frmNhaCungCap();
            NCC.ShowDialog();
            ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);
        }
    
     

    }
}