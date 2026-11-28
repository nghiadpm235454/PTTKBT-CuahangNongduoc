using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmPhieuChi : Form
    {
        LyDoChiController ctrlLyDo = new LyDoChiController();
        PhieuChiController ctrl = new PhieuChiController();
        FormEditState status = FormEditState.Normal;

        public frmPhieuChi()
        {
            InitializeComponent();
        }

        private void frmThanhToan_Load(object sender, EventArgs e)
        {
            ctrlLyDo.HienthiAutoComboBox(cmbLyDoChi);
            ctrlLyDo.HienthiDataGridviewComboBox(colLyDoChi);
            ctrl.HienthiPhieuChi(bindingNavigator, dataGridView, cmbLyDoChi, txtMaPhieu, dtNgayChi, numTongTien, txtGhiChu);
        }

        private void XoaLienKetDieuKhien()
        {
            txtMaPhieu.DataBindings.Clear();
            cmbLyDoChi.DataBindings.Clear();
            dtNgayChi.DataBindings.Clear();
            numTongTien.DataBindings.Clear();
            txtGhiChu.DataBindings.Clear();
        }

        private void ThoatCheDoThemMoi()
        {
            status = FormEditState.Normal;
            txtMaPhieu.ReadOnly = false;
            dataGridView.Enabled = true;
            BindingSource bs = bindingNavigator.BindingSource;
            if (bs != null)
            {
                ctrl.GanLaiDieuKhien(bs, bindingNavigator, dataGridView, cmbLyDoChi, txtMaPhieu, dtNgayChi, numTongTien, txtGhiChu);
            }
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            status = FormEditState.AddNew;
            XoaLienKetDieuKhien();
            txtMaPhieu.Text = ThamSo.PhieuChi.ToString();
            txtMaPhieu.ReadOnly = true;
            dtNgayChi.Value = DateTime.Now.Date;
            numTongTien.Value = 0;
            txtGhiChu.Text = "";
            if (cmbLyDoChi.Items.Count > 0 && cmbLyDoChi.SelectedIndex < 0)
            {
                cmbLyDoChi.SelectedIndex = 0;
            }
            dataGridView.Enabled = false;
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn xóa phiếu chi này không?", "Phieu Chi",   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            if (status == FormEditState.AddNew)
            {
                ThoatCheDoThemMoi();
                return;
            }

            if (bindingNavigator.BindingSource.Current == null) return;

            if (MessageBox.Show("Bạn chắc chắn xóa phiếu chi này không?", "Phieu Chi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bindingNavigator.BindingSource.RemoveCurrent();
                ctrl.Save();
            }
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            txtMaPhieu.Focus();

            if (status == FormEditState.AddNew)
            {
                // Factory Method + Template Method: kiểm tra, tạo phiếu, lưu, rồi mới tăng bộ đếm
                PhieuCreator creator = new PhieuChiCreator(ctrl);
                XuLyPhieuChi xuLy = (XuLyPhieuChi)creator.TaoXuLy();
                xuLy.NgayChi = dtNgayChi.Value.Date;
                xuLy.TongTien = numTongTien.Value;
                xuLy.IdLyDoChi = cmbLyDoChi.SelectedValue;
                xuLy.GhiChu = txtGhiChu.Text;

                if (!xuLy.Thuchien())
                {
                    MessageBox.Show(xuLy.ThongBaoLoi, "Phieu Chi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ThoatCheDoThemMoi();
                if (bindingNavigator.BindingSource != null)
                {
                    bindingNavigator.BindingSource.MoveLast();
                }
                return;
            }

            if (bindingNavigator.BindingSource != null)
            {
                bindingNavigator.BindingSource.EndEdit();
            }
            ctrl.Save();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void toolIn_Click(object sender, EventArgs e)
        {
            if (status == FormEditState.AddNew)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu chi hiện tại!", "Phieu Chi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataRowView row = (DataRowView)bindingNavigator.BindingSource.Current;
            if (row != null)
            {
                PhieuChiController ctrlChi = new PhieuChiController();
                String ma_phieu = row["ID"].ToString();
                CuahangNongduoc.BusinessObject.PhieuChi ph = ctrlChi.LayPhieuChi(ma_phieu);
                if (ph == null) return;
                frmInPhieuChi InPhieu = new frmInPhieuChi(ph);
                InPhieu.Show();
            }
        }

        private void btnThemLyDoChi_Click(object sender, EventArgs e)
        {
            frmLyDoChi Chi = new frmLyDoChi();
            Chi.ShowDialog();
            ctrlLyDo.HienthiAutoComboBox(cmbLyDoChi);
        }

        private void toolTimKiem_Click(object sender, EventArgs e)
        {
            frmTimPhieuChi Tim = new frmTimPhieuChi();
            Point p = PointToScreen(toolTimKiem.Bounds.Location);
            p.X += toolTimKiem.Width;
            p.Y += toolTimKiem.Height;
            Tim.Location = p;
            Tim.ShowDialog();
            if (Tim.DialogResult == DialogResult.OK && Tim.cmbLyDo.SelectedValue != null)
            {
                status = FormEditState.Normal;
                txtMaPhieu.ReadOnly = false;
                dataGridView.Enabled = true;
                ctrl.TimPhieuChi(bindingNavigator, dataGridView, cmbLyDoChi, txtMaPhieu, dtNgayChi, numTongTien, txtGhiChu,
                    Convert.ToInt32(Tim.cmbLyDo.SelectedValue), Tim.dtNgayChi.Value.Date);
            }
        }
    }
}