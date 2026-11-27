using System;
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
    public partial class frmBanLe: Form
    {
        SanPhamController ctrlSanPham = new SanPhamController();
        KhachHangController ctrlKhachHang = new KhachHangController();
        MaSanPhamController ctrlMaSanPham = new MaSanPhamController();
        PhieuBanController ctrlPhieuBan = new PhieuBanController();
        ChiTietPhieuBanController ctrlChiTiet = new ChiTietPhieuBanController();
        XuatKhoController ctrlXuatKho = new XuatKhoController();
        CheckBox chkChiDinhLo = new CheckBox();   // tao bang code, khong dung Designer
        Controll status = Controll.Normal;

        public frmBanLe()
        {
            InitializeComponent();
            status = Controll.AddNew;
        }

     
        public frmBanLe(PhieuBanController ctrlPB)
            : this()
        {
            this.ctrlPhieuBan = ctrlPB;
            status = Controll.Normal;
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            // [CN-04] mac dinh FIFO theo HSD; tick de chi dinh lo
            chkChiDinhLo.Text = "Chỉ định lô";
            chkChiDinhLo.AutoSize = true;
            chkChiDinhLo.Location = new Point(cmbMaSanPham.Right + 6, cmbMaSanPham.Top + 2);
            cmbMaSanPham.Parent.Controls.Add(chkChiDinhLo);
            chkChiDinhLo.BringToFront();


            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            cmbSanPham.SelectedIndexChanged += new EventHandler(cmbSanPham_SelectedIndexChanged);

            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);

            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator,cmbKhachHang, txtMaPhieu, dtNgayLapPhieu, numTongTien, numDaTra, numConNo);

            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);
            
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);


            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
            }
            else
            {
                this.Allow(false);
            }


        }

        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
            {
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            }
        }


        void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSanPham.SelectedValue != null)
            {
                MaSanPhamController ctrlMSP = new MaSanPhamController();

                cmbMaSanPham.SelectedIndexChanged -= new EventHandler(cmbMaSanPham_SelectedIndexChanged);
                ctrlMSP.HienThiAutoComboBox(cmbSanPham.SelectedValue.ToString(), cmbMaSanPham);
                cmbMaSanPham.SelectedIndexChanged += new EventHandler(cmbMaSanPham_SelectedIndexChanged);
            }
        }

        void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            MaSanPhamController ctrl = new MaSanPhamController();
            MaSanPham masp = ctrl.LayMaSanPham(cmbMaSanPham.SelectedValue.ToString());
            numDonGia.Value = masp.GiaBanLe;
            txtGiaNhap.Text = masp.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = masp.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = masp.GiaBanLe.ToString("#,###0");
            txtGiaBQGQ.Text = masp.GiaBQGQ.ToString("#,###0");

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // [CN-03 / CN-04] TRUOC: moi lan Them = 1 dong cua dung ma san pham dang chon, khong kiem HSD/ton.
            // SAU:   XuatKhoController phan lo: FIFO theo HSD (tu tach nhieu lo) hoac lo chi dinh.
            if (cmbSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Sản phẩm !", "Phieu Ban", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Vui lòng nhập Số lượng !", "Phieu Ban", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PhuongPhapXuatKho pp = chkChiDinhLo.Checked ? PhuongPhapXuatKho.ChiDinh : PhuongPhapXuatKho.FIFO;
            string idLo = cmbMaSanPham.SelectedValue == null ? null : Convert.ToString(cmbMaSanPham.SelectedValue);
            if (pp == PhuongPhapXuatKho.ChiDinh && idLo == null)
            {
                MessageBox.Show("Vui lòng chọn Mã sản phẩm (lô) !", "Phieu Ban", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            KetQuaPhanLo kq = ctrlXuatKho.TaoDongXuat(Convert.ToString(cmbSanPham.SelectedValue),
                (int)numSoLuong.Value, pp, idLo, dtNgayLapPhieu.Value.Date, ctrlChiTiet.LayDaGiu());
            if (!kq.ThanhCong)
            {
                MessageBox.Show(kq.ThongBaoLoi, "Xuất kho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;   // loi thi KHONG them dong nao
            }

            long donGia = (long)numDonGia.Value;
            foreach (DongXuat d in kq.Dong)
            {
                DataRow dong = ctrlChiTiet.TimDongTheoLo(d.SoLo);
                if (dong != null)   // cung lo da co tren phieu -> cong don (PK: ID_PHIEU_BAN + ID_MA_SAN_PHAM)
                {
                    dong["SO_LUONG"] = Convert.ToInt32(dong["SO_LUONG"]) + d.SoLuong;
                    dong["THANH_TIEN"] = Convert.ToInt64(dong["SO_LUONG"]) * donGia;
                }
                else
                {
                    dong = ctrlChiTiet.NewRow();
                    dong["ID_MA_SAN_PHAM"] = d.SoLo;
                    dong["ID_PHIEU_BAN"] = txtMaPhieu.Text;
                    dong["DON_GIA"] = donGia;
                    dong["SO_LUONG"] = d.SoLuong;
                    dong["THANH_TIEN"] = d.SoLuong * donGia;
                    ctrlChiTiet.Add(dong);
                }
                numTongTien.Value += d.SoLuong * donGia;
            }
        }

        private void numDonGia_ValueChanged(object sender, EventArgs e)
        {
            numThanhTien.Value = numDonGia.Value * numSoLuong.Value;
        }

        private void numTongTien_ValueChanged(object sender, EventArgs e)
        {
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            if (this.Luu())
            {
                status = Controll.Normal;
                this.Allow(false);
            }
        }

        bool Luu()
        {
            if (status == Controll.AddNew)
            {
                return ThemMoi();
            }
            else
            {
                return CapNhat();
            }
        }

        bool CapNhat()
        {
            // [FIX-01] TRUOC: vong foreach (deleted) hoan kho rieng + ctrlChiTiet.Save() nuot loi
            // SAU:     Save() tu xu ly Added/Modified/Deleted trong 1 transaction -> bo danh sach 'deleted'
            string loi;
            if (!ctrlChiTiet.Save(out loi))
            {
                MessageBox.Show(loi, "Phieu Ban", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            ctrlPhieuBan.Update();
            HienCanhBaoKho();
            return true;
        }

        // [CN-04 Observer] hien cac canh bao ton thap / sap het han do Observer gom lai
        void HienCanhBaoKho()
        {
            string[] canhBao = TonKhoSubject.HopThu.LayVaXoa();
            if (canhBao.Length > 0)
                MessageBox.Show(string.Join(Environment.NewLine, canhBao), "Cảnh báo kho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        bool ThemMoi()
        {

            DataRow row = ctrlPhieuBan.NewRow();
            row["ID"] = txtMaPhieu.Text;
            row["ID_KHACH_HANG"] = cmbKhachHang.SelectedValue;
            row["NGAY_BAN"] = dtNgayLapPhieu.Value.Date;
            row["TONG_TIEN"] = numTongTien.Value;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;
            ctrlPhieuBan.Add(row);

            PhieuBanController ctrl = new PhieuBanController();

            if (ctrl.LayPhieuBan(txtMaPhieu.Text) != null)
            {
                MessageBox.Show("Mã Phiếu bán này đã tồn tại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (ThamSo.LaSoNguyen(txtMaPhieu.Text))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text);
                if (so >= ThamSo.LayMaPhieuBan())
                {
                    ThamSo.GanMaPhieuBan(so + 1);
                }
            }

            ctrlPhieuBan.Save();

            // [FIX-01] TRUOC: ctrlChiTiet.Save();  (loi tru kho bi nuot)
            string loi;
            if (!ctrlChiTiet.Save(out loi))
            {
                MessageBox.Show(loi, "Phieu Ban", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            HienCanhBaoKho();
            return true;

        }

        private void toolLuu_Them_Click(object sender, EventArgs e)
        {
            ctrlPhieuBan = new PhieuBanController();
            status = Controll.AddNew;
            txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
            numTongTien.Value = 0;
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            this.Allow(true);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                BindingSource bs = ((BindingSource)dgvDanhsachSP.DataSource);
                DataRowView row = (DataRowView)bs.Current;
                numTongTien.Value -= Convert.ToInt64(row["THANH_TIEN"]);
                bs.RemoveCurrent();
            }
           
        }

        private void dgvDanhsachSP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                BindingSource bs = ((BindingSource)dgvDanhsachSP.DataSource);
                DataRowView row = (DataRowView)bs.Current;
                numTongTien.Value -= Convert.ToInt64(row["THANH_TIEN"]);

            }
        }

        private void toolLuuIn_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu bán hiện tại!", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                String ma_phieu = txtMaPhieu.Text;

                PhieuBanController ctrlPB = new PhieuBanController();

                CuahangNongduoc.BusinessObject.PhieuBan ph = ctrlPB.LayPhieuBan(ma_phieu);

                frmInPhieuBan InPhieuBan = new frmInPhieuBan(ph);

                InPhieuBan.Show();

            }
        }

        private void dgvDanhsachSP_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;

        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {

        }

        private void toolChinhSua_Click(object sender, EventArgs e)
        {
            status = Controll.Edit;
            this.Allow(true);
        }

        void Allow(bool val)
        {
            txtMaPhieu.Enabled = val;
            dtNgayLapPhieu.Enabled = val;
            numTongTien.Enabled = val;
            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dgvDanhsachSP.Enabled = val;
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu bán này không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!this.Luu()) return;
                }
            }
            this.Close();
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            DataRowView view =  (DataRowView)bindingNavigator.BindingSource.Current;
            if (view != null)
            {
                if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();
                    IList<ChiTietPhieuBan> ds = ctrl.ChiTietPhieuBan(view["ID"].ToString());
                    foreach (ChiTietPhieuBan ct in ds)
                    {
                        CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
                    }
                    bindingNavigator.BindingSource.RemoveCurrent();
                    ctrlPhieuBan.Save();
                }
            }
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            frmKhachHang KhachHang = new frmKhachHang();
            KhachHang.ShowDialog();
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
        }


     }
}
