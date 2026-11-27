using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Strategy;

namespace CuahangNongduoc
{
    public partial class frmBanSi: Form
    {
        SanPhamController ctrlSanPham = new SanPhamController();
        KhachHangController ctrlKhachHang = new KhachHangController();
        MaSanPhamController ctrlMaSanPham = new MaSanPhamController();
        PhieuBanController ctrlPhieuBan = new PhieuBanController();
        ChiTietPhieuBanController ctrlChiTiet = new ChiTietPhieuBanController();
        XuatKhoController ctrlXuatKho = new XuatKhoController();
        CheckBox chkChiDinhLo = new CheckBox();   // tao bang code, khong dung Designer


        Controll status = Controll.Normal;

        public frmBanSi()
        {
            InitializeComponent();
            
            status = Controll.AddNew;
        }


        public frmBanSi(PhieuBanController ctrlPB)
            : this()
        {
            this.ctrlPhieuBan = ctrlPB;
            status = Controll.Normal;
        }

        private void frmBanSi_Load(object sender, EventArgs e)
        {
            // [CN-04] mac dinh theo cau hinh cua hang (FIFO theo HSD hoac chi dinh lo)
            chkChiDinhLo.Text = "Chỉ định lô";
            chkChiDinhLo.AutoSize = true;
            chkChiDinhLo.Checked = (ThamSoManager.Instance.LayCuaHang().PhuongPhapXuatKho == PhuongPhapXuatKho.ChiDinh);
            chkChiDinhLo.Location = new Point(cmbMaSanPham.Right + 6, cmbMaSanPham.Top + 2);
            cmbMaSanPham.Parent.Controls.Add(chkChiDinhLo);
            chkChiDinhLo.BringToFront();

            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            cmbSanPham.SelectedIndexChanged += new EventHandler(cmbSanPham_SelectedIndexChanged);

            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);

            
            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator,cmbKhachHang, txtMaPhieu, dtNgayLapPhieu, numTongTien, numDaTra, numConNo);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);
            
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);


            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSoManager.Instance.GetThamSo(KhoaThamSo.PhieuBan).ToString();
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
                ctrlMSP.HienThiAutoComboBox(cmbSanPham.SelectedValue.ToString(), cmbMaSanPham, true);
                cmbMaSanPham.SelectedIndexChanged += new EventHandler(cmbMaSanPham_SelectedIndexChanged);
            }
        }

        void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            MaSanPhamController ctrl = new MaSanPhamController();
            MaSanPham masp = ctrl.LayMaSanPham(cmbMaSanPham.SelectedValue.ToString());
            numDonGia.Value = masp.GiaBanSi;
            txtGiaNhap.Text = masp.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = masp.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = masp.GiaBanLe.ToString("#,###0");
            txtGiaBQGQ.Text = masp.GiaBQGQ.ToString("#,###0");


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // [CN-03 / CN-04] XuatKhoController phan lo (FIFO theo HSD hoac lo chi dinh) + [CN-05] tinh GIA_XUAT (Strategy)
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
                return;
            }

            IList<MaSanPham> cacLo = new MaSanPhamController().LayLoConHang(Convert.ToString(cmbSanPham.SelectedValue));
            long giaXuat = KhoStrategyFactory.TuCauHinh().TaoGiaXuat().TinhGiaXuat(cacLo);
            long donGia = (long)numDonGia.Value;
            foreach (DongXuat d in kq.Dong)
            {
                DataRow dong = ctrlChiTiet.TimDongTheoLo(d.SoLo);
                if (dong != null)
                {
                    dong["SO_LUONG"] = Convert.ToInt32(dong["SO_LUONG"]) + d.SoLuong;
                    dong["THANH_TIEN"] = Convert.ToInt64(dong["SO_LUONG"]) * donGia;
                    dong["GIA_XUAT"] = giaXuat;
                }
                else
                {
                    dong = ctrlChiTiet.NewRow();
                    dong["ID_MA_SAN_PHAM"] = d.SoLo;
                    dong["ID_PHIEU_BAN"] = txtMaPhieu.Text;
                    dong["DON_GIA"] = donGia;
                    dong["SO_LUONG"] = d.SoLuong;
                    dong["THANH_TIEN"] = d.SoLuong * donGia;
                    dong["GIA_XUAT"] = giaXuat;
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
                if (so >= ThamSoManager.Instance.GetThamSo(KhoaThamSo.PhieuBan))
                {
                    ThamSoManager.Instance.SetThamSo(KhoaThamSo.PhieuBan, so + 1);
                }
            }

            ctrlPhieuBan.Save();

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
            txtMaPhieu.Text = ThamSoManager.Instance.GetThamSo(KhoaThamSo.PhieuBan).ToString();
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
                    this.Luu();
                }

            }
            this.Close();
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
             DataRowView view =  (DataRowView)bindingNavigator.BindingSource.Current;
             if (view != null)
             {

                 if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Si", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

        private void toolXemLai_Click(object sender, EventArgs e)
        {
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            frmDaiLy DaiLy = new frmDaiLy();
            DaiLy.ShowDialog();
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
            
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }

     }
}
