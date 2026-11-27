using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc
{
    /// <summary>
    /// UI-06 – Báo cáo thống kê: tồn kho, vận chuyển, dịch vụ phụ, giảm giá, khuyến mãi;
    /// lọc từ ngày – đến ngày, có nút Xem và In (rdlc: Report\rptThongKe.rdlc).
    /// Kế thừa BaseReportForm (STT 57 Extract Superclass + STT 59 Form Template Method): chỉ ghi đè 3 hook.
    /// Mẫu thiết kế: Builder (ThongKeReportBuilder) + Bridge (LoaiThongKe x IThongKeOutput).
    /// Form dựng giao diện bằng code nên không có file Designer.
    /// </summary>
    public class frmBaoCaoThongKe : BaseReportForm, IThongKeOutput
    {
        protected FlowLayoutPanel pnlLoc;
        protected ComboBox cmbLoai;
        private DateTimePicker dtTu, dtDen;
        private Button btnXem, btnIn;
        private ReportViewer reportViewer;
        private BindingSource thongKeRowBindingSource;
        private ThongKeReport m_BaoCao = new ThongKeReport();

        public frmBaoCaoThongKe()
        {
            KhoiTaoGiaoDien();
        }

        private void KhoiTaoGiaoDien()
        {
            this.Text = "Báo cáo thống kê";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            pnlLoc = new FlowLayoutPanel();
            pnlLoc.Dock = DockStyle.Top;
            pnlLoc.AutoSize = true;
            pnlLoc.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlLoc.Padding = new Padding(6, 6, 6, 6);

            cmbLoai = new ComboBox();
            cmbLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLoai.Width = 190;

            dtTu = new DateTimePicker();
            dtTu.Format = DateTimePickerFormat.Short;
            dtTu.Width = 100;
            dtTu.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            dtDen = new DateTimePicker();
            dtDen.Format = DateTimePickerFormat.Short;
            dtDen.Width = 100;
            dtDen.Value = DateTime.Today;

            btnXem = new Button();
            btnXem.Text = "Xem";
            btnXem.Click += new EventHandler(btnXem_Click);

            btnIn = new Button();
            btnIn.Text = "In";
            btnIn.Click += new EventHandler(btnIn_Click);

            pnlLoc.Controls.Add(TaoNhan("Loại:"));
            pnlLoc.Controls.Add(cmbLoai);
            pnlLoc.Controls.Add(TaoNhan("Từ ngày:"));
            pnlLoc.Controls.Add(dtTu);
            pnlLoc.Controls.Add(TaoNhan("Đến ngày:"));
            pnlLoc.Controls.Add(dtDen);
            pnlLoc.Controls.Add(btnXem);
            pnlLoc.Controls.Add(btnIn);

            thongKeRowBindingSource = new BindingSource();
            thongKeRowBindingSource.DataSource = typeof(ThongKeRow);

            reportViewer = new ReportViewer();
            reportViewer.Dock = DockStyle.Fill;
            reportViewer.LocalReport.ReportEmbeddedResource = "CuahangNongduoc.Report.rptThongKe.rdlc";
            reportViewer.LocalReport.DataSources.Add(
                new ReportDataSource("CuahangNongduoc_BusinessObject_ThongKeRow", thongKeRowBindingSource));

            this.Controls.Add(reportViewer);
            this.Controls.Add(pnlLoc);   // thêm sau để Dock = Top nằm trên ReportViewer
        }

        private static Label TaoNhan(string text)
        {
            Label l = new Label();
            l.Text = text;
            l.AutoSize = true;
            l.Margin = new Padding(6, 6, 0, 0);
            return l;
        }

        /// <summary>Các loại thống kê hiển thị trong combo. Lớp con ghi đè để đổi danh sách.</summary>
        protected virtual IList<LoaiThongKe> DanhSachLoai()
        {
            return new LoaiThongKe[]
            {
                new ThongKeTonKho(this),
                new ThongKeVanChuyen(this),
                new ThongKeDichVu(this),
                new ThongKeGiamGia(this, LocGiamGia.GiamGia),
                new ThongKeGiamGia(this, LocGiamGia.KhuyenMai)
            };
        }

        /// <summary>Nhân viên cần lọc; null = tất cả. frmBaoCaoNhanVien ghi đè.</summary>
        protected virtual int? IdNguoiDung { get { return null; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cmbLoai.Items.Clear();
            foreach (LoaiThongKe l in DanhSachLoai())
                cmbLoai.Items.Add(l);
            if (cmbLoai.Items.Count > 0)
                cmbLoai.SelectedIndex = 0;
        }

        private ThongKeFilter TaoBoLoc()
        {
            ThongKeFilter f = new ThongKeFilter();
            f.Tu = dtTu.Value.Date;
            f.Den = dtDen.Value.Date;
            f.IdNguoiDung = IdNguoiDung;
            return f;
        }

        private void ThucHien(bool in_)
        {
            LoaiThongKe loai = cmbLoai.SelectedItem as LoaiThongKe;
            if (loai == null) return;
            if (dtTu.Value.Date > dtDen.Value.Date)
            {
                MessageBox.Show("Từ ngày phải nhỏ hơn hoặc bằng đến ngày.", "Báo cáo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (in_) loai.In(TaoBoLoc()); else loai.Xem(TaoBoLoc());
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không đọc được dữ liệu từ SQL Server:\n" + ex.Message, "Báo cáo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXem_Click(object sender, EventArgs e) { ThucHien(false); }
        private void btnIn_Click(object sender, EventArgs e) { ThucHien(true); }

        // ---- IThongKeOutput (Implementor của Bridge) ----
        public void DatBaoCao(ThongKeReport baoCao)
        {
            m_BaoCao = baoCao;
        }

        public void In()
        {
            HienThiBaoCao();
            reportViewer.PrintDialog();
        }

        // ---- Các hook của Template Method (BaseReportForm) ----
        protected override ReportViewer BoXemBaoCao { get { return reportViewer; } }

        protected override void ThemThamSo(IList<ReportParameter> ds)
        {
            foreach (ReportParameter p in m_BaoCao.TaoThamSo())
                ds.Add(p);
        }

        protected override void NapDuLieu()
        {
            thongKeRowBindingSource.DataSource = m_BaoCao.Dong;
        }
    }
}
