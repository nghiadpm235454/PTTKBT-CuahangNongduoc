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

        FormEditState status = FormEditState.Normal;
        PhieuNhapCaretaker caretaker = new PhieuNhapCaretaker();
        NhapHangMediator mediator;

        public frmNhapHang()
        {
            InitializeComponent();
            status = FormEditState.AddNew;
        }
        public frmNhapHang(PhieuNhapController ctrlPN)
            : this()
        {
            this.ctrl = ctrlPN;
            status = FormEditState.Normal;
        }

        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == FormEditState.Normal)
                ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            mediator = new NhapHangMediator(numGiaNhap, numSoLuong, numThanhTien, numTongTien, numDaTra, numConNo);

            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlSanPham.HienthiDataGridViewComboBoxColumn(colSanPham);
            ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);

            ctrl.HienthiPhieuNhap(bindingNavigator, txtMaPhieu, cmbNhaCungCap, dtNgayNhap, numTongTien, numDaTra, numConNo);
            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);

            if (status == FormEditState.AddNew)
            {
                txtMaPhieu.Text = ThamSoManager.Instance.GetThamSo(KhoaThamSo.PhieuNhap).ToString();
                ctrlMaSP.HienThiChiTietPhieuNhap("-1", dataGridView);
                BatCheDoNhap();
            }
            else
            {
                ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
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
                d.DonGiaNhap = Convert.ToDecimal(r.Cells["colDonGiaNhap"].Value);
                d.SoLuong = Convert.ToDecimal(r.Cells["colSoLuong"].Value);
                d.NgaySanXuat = Convert.ToDateTime(r.Cells["colNgaySanXuat"].Value);
                d.NgayHetHan = Convert.ToDateTime(r.Cells["colNgayHetHan"].Value);
                m.Dong.Add(d);
            }
            return m;
        }

        void PhucHoiMemento(PhieuNhapMemento m)
        {
            txtMaPhieu.Text = m.MaPhieu;
            cmbNhaCungCap.SelectedValue = m.IdNhaCungCap;
            dtNgayNhap.Value = m.NgayNhap;
            numTongTien.Value = 0;
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
                numTongTien.Value += d.DonGiaNhap * d.SoLuong;
            }
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
                if (status == FormEditState.Normal) return;
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
            row["ID_PHIEU_NHAP"] = txtMaPhieu.Text.Trim();
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
            string maSo = txtMaSo.Text.Trim();
            if (cmbSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Sản phẩm !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (maSo.Length <= 0)
            {
                MessageBox.Show("Vui lòng nhập Mã sản phẩm !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MaSanPhamController ctrlKiemTra = new MaSanPhamController();
            MaSanPham masp = ctrlKiemTra.LayMaSanPham(maSo);
            if (masp != null)
            {
                MessageBox.Show("Mã sản phẩm này đã tồn tại! Vui lòng nhập lại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow) continue;
                string maTrongLuoi = Convert.ToString(view.Cells["colMaSanPham"].Value).Trim();
                if (string.Equals(maSo, maTrongLuoi, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Mã sản phẩm này đã tồn tại trong danh sách! Vui lòng nhập lại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (numGiaNhap.Value <= 0)
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
                info.MaLo = maSo;
                info.IdSanPham = cmbSanPham.SelectedValue;
                info.Gia = numGiaNhap.Value;
                info.SoLuong = numSoLuong.Value;
                info.NSX = dtNgaySanXuat.Value;
                info.HSD = dtNgayHetHan.Value;
                ctrlMaSP.Add(TaoDong(info));
            }
        }

        private void toolLuuThoat_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            if (this.Luu())
            {
                status = FormEditState.Normal;
                this.TatCheDoNhap();
            }
        }

        private bool Luu()
        {
            if (status == FormEditState.AddNew)
            {
                return ThemMoi();
            }
            CapNhat();
            return true;
        }

        private void CapNhat()
        {
            ctrlMaSP.Save();
            ctrl.Update();
        }

        private bool ThemMoi()
        {
            // Factory Method: Creator tạo ra bộ xử lý phiếu nhập
            PhieuCreator creator = new PhieuNhapCreator(ctrl, ctrlMaSP);
            XuLyPhieuNhap xuLy = (XuLyPhieuNhap)creator.TaoXuLy();

            // Đưa dữ liệu trên form vào bộ xử lý
            string maPhieu = txtMaPhieu.Text.Trim();
            xuLy.Id = maPhieu;
            xuLy.NgayNhap = dtNgayNhap.Value.Date;
            xuLy.TongTien = numTongTien.Value;
            xuLy.IdNhaCungCap = cmbNhaCungCap.SelectedValue;
            xuLy.DaTra = numDaTra.Value;
            xuLy.ConNo = numConNo.Value;

            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow) continue;
                DataRowView drv = view.DataBoundItem as DataRowView;
                if (drv != null && drv.Row.RowState == DataRowState.Added)
                {
                    drv.Row["ID_PHIEU_NHAP"] = maPhieu;
                    drv.Row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
                }

                DongGiaNhap dong = new DongGiaNhap();
                dong.IdSanPham = Convert.ToString(view.Cells["colSanPham"].Value);
                dong.DonGiaNhap = Convert.ToInt64(view.Cells["colDonGiaNhap"].Value);
                dong.SoLuong = Convert.ToInt64(view.Cells["colSoLuong"].Value);
                xuLy.DanhSachGiaNhap.Add(dong);
            }

            // Template Method: chạy 5 bước, nếu sai thì báo lỗi và giữ nguyên trạng thái nhập để người dùng sửa
            if (!xuLy.Thuchien())
            {
                MessageBox.Show(xuLy.ThongBaoLoi, "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void toolLuuThem_Click(object sender, EventArgs e)
        {
            ctrl = new PhieuNhapController();
            ctrl.HienthiPhieuNhap(bindingNavigator, txtMaPhieu, cmbNhaCungCap, dtNgayNhap, numTongTien, numDaTra, numConNo);
            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);

            status = FormEditState.AddNew;

            txtMaPhieu.Text = ThamSoManager.Instance.GetThamSo(KhoaThamSo.PhieuNhap).ToString();
            numTongTien.Value = 0;
            numDaTra.Value = 0;
            numConNo.Value = 0;
            ctrlMaSP.HienThiChiTietPhieuNhap("-1", dataGridView);
            this.BatCheDoNhap();
        }

        private void toolSavePrint_Click(object sender, EventArgs e)
        {
            if (status != FormEditState.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu nhập hiện tại!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                String ma_phieu = txtMaPhieu.Text.Trim();
                PhieuNhapController ctrlPN = new PhieuNhapController();
                CuahangNongduoc.BusinessObject.PhieuNhap ph = ctrlPN.LayPhieuNhap(ma_phieu);
                if (ph == null)
                {
                    MessageBox.Show("Không tìm thấy phiếu nhập để in!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                frmInPhieuNhap PhieuNhap = new frmInPhieuNhap(ph);
                PhieuNhap.Show();
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != FormEditState.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu nhập này không?", "Phieu Nhap Hang", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!this.Luu())
                        return;
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
            status = FormEditState.Edit;
            this.BatCheDoNhap();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null || dataGridView.CurrentRow.IsNewRow)
                return;

            if (MessageBox.Show("Bạn chắc chắn xóa dòng sản phẩm này không?", "Phieu Nhap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                long tru = Convert.ToInt64(dataGridView.CurrentRow.Cells["colDonGiaNhap"].Value)
                         * Convert.ToInt64(dataGridView.CurrentRow.Cells["colSoLuong"].Value);
                numTongTien.Value = Math.Max(0, numTongTien.Value - tru);
                dataGridView.Rows.Remove(dataGridView.CurrentRow);
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn xóa dòng sản phẩm này không?", "Phieu Nhap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                long tru = Convert.ToInt64(e.Row.Cells["colDonGiaNhap"].Value) * Convert.ToInt64(e.Row.Cells["colSoLuong"].Value);
                numTongTien.Value = Math.Max(0, numTongTien.Value - tru);
            }
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource.Current == null) return;

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
