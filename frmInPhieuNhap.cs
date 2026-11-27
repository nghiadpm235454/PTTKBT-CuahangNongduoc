using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    // [REFACTORING STT 56 - Extract Subclass] + [STT 57 - Extract Superclass]
    //   TRUOC: ke thua Form; Load tu viet ~10 dong: 4 tham so (ten_cua_hang, dia_chi, dien_thoai, bang_chu)
    //          + SetParameters + gan DataSource + RefreshReport.
    //   SAU  : ke thua BasePhieuInForm (BaoCao/BasePhieuInForm.cs). Form chi khai bao phan RIENG: so tien + du lieu.
    public partial class frmInPhieuNhap : BasePhieuInForm
    {
        CuahangNongduoc.BusinessObject.PhieuNhap m_PhieuNhap = null;
        public frmInPhieuNhap(CuahangNongduoc.BusinessObject.PhieuNhap ph)
        {
            m_PhieuNhap = ph;
            InitializeComponent();

            this.reportViewer.LocalReport.SubreportProcessing += new Microsoft.Reporting.WinForms.SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
        }

        void LocalReport_SubreportProcessing(object sender, Microsoft.Reporting.WinForms.SubreportProcessingEventArgs e)
        {
            e.DataSources.Clear();
            e.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("CuahangNongduoc_BusinessObject_MaSanPham", m_PhieuNhap.ChiTiet));
        }

        // [STT 59 - Form Template Method] hook 1: reportViewer cua form nay
        protected override Microsoft.Reporting.WinForms.ReportViewer BoXemBaoCao
        {
            get { return this.reportViewer; }
        }

        // [STT 56 - Extract Subclass] hook rieng cua phieu in: tong tien de doc thanh chu
        protected override string LaySoTien()
        {
            return m_PhieuNhap.TongTien.ToString();
        }

        // [STT 59 - Form Template Method] hook 3: gan du lieu
        protected override void NapDuLieu()
        {
            this.PhieuNhapBindingSource.DataSource = m_PhieuNhap;
        }

        private void frmInPhieuNhap_Load(object sender, EventArgs e)
        {
            HienThiBaoCao();
        }
    }
}
