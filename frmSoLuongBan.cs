using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    // [REFACTORING STT 57 - Extract Superclass] TRUOC: ke thua Form. SAU: ke thua BaseReportForm.
    public partial class frmSoLuongBan : BaseReportForm
    {
        public frmSoLuongBan()
        {
            InitializeComponent();
        }

        private void frmSoLuongBan_Load(object sender, EventArgs e)
        {
            cmbThang.SelectedIndex = DateTime.Now.Month - 1;
            numNam.Value = DateTime.Now.Year;
        }

        // [REFACTORING STT 55 - Push Down Field]  https://refactoring.guru/push-down-field
        //   Cac field duoi day CHI frmSoLuongBan dung (7 form con lai trong nhom bao cao khong can),
        //   nen de o lop con nay, KHONG dua len BaseReportForm.
        //   TRUOC: (ban nhap lop cha) BaseReportForm co  protected ChiTietPhieuBanController ctrl  -> 7 form con lai mang field thua.
        //   SAU  : field nam dung o lop con duy nhat can no, doi thanh private.
        private ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();
        private string m_NhanKy;                   // "Ngay dd/MM/yyyy" hoac "Thang m/yyyy" -> tham so "ngay" cua bao cao
        private Func<object> m_LayDuLieu;          // cach lay du lieu: theo ngay hoac theo thang

        // [STT 59 - Form Template Method] hook 1
        protected override Microsoft.Reporting.WinForms.ReportViewer BoXemBaoCao
        {
            get { return this.reportViewer; }
        }

        // [STT 59 - Form Template Method] hook 2: tham so "ngay" (khac nhau giua xem ngay / xem thang)
        protected override void ThemThamSo(IList<Microsoft.Reporting.WinForms.ReportParameter> ds)
        {
            ds.Add(new Microsoft.Reporting.WinForms.ReportParameter("ngay", m_NhanKy));
        }

        // [STT 59 - Form Template Method] hook 3: gan du lieu (khac nhau giua xem ngay / xem thang)
        protected override void NapDuLieu()
        {
            this.ChiTietPhieuBanBindingSource.DataSource = m_LayDuLieu();
        }

        // TRUOC: btnXemNgay_Click va btnXemThang_Click moi ham tu viet lai: tao param -> SetParameters -> gan du lieu -> Refresh
        // SAU  : moi nut chi khai bao "phan khac nhau" (nhan ky + cach lay du lieu) roi goi HienThiBaoCao()
        private void btnXemNgay_Click(object sender, EventArgs e)
        {
            m_NhanKy = "Ngày " + dtNgay.Value.Date.ToString("dd/MM/yyyy");
            m_LayDuLieu = () => ctrl.ChiTietPhieuBan(dtNgay.Value.Date);
            HienThiBaoCao();
        }

        private void btnXemThang_Click(object sender, EventArgs e)
        {
            m_NhanKy = "Tháng " + Convert.ToString(cmbThang.SelectedIndex + 1) + "/" + numNam.Value.ToString();
            m_LayDuLieu = () => ctrl.ChiTietPhieuBan(cmbThang.SelectedIndex + 1, Convert.ToInt32(numNam.Value));
            HienThiBaoCao();
        }
    }
}
