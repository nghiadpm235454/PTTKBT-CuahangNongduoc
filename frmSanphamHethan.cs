using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    // [REFACTORING STT 57 - Extract Superclass] TRUOC: ke thua Form. SAU: ke thua BaseReportForm.
    public partial class frmSanphamHethan : BaseReportForm
    {
        public frmSanphamHethan()
        {
            InitializeComponent();
        }

        // [STT 59 - Form Template Method] hook 1
        protected override Microsoft.Reporting.WinForms.ReportViewer BoXemBaoCao
        {
            get { return this.reportViewer; }
        }

        // [STT 59 - Form Template Method] hook 2: tham so rieng = ngay tinh het han
        protected override void ThemThamSo(IList<Microsoft.Reporting.WinForms.ReportParameter> ds)
        {
            ds.Add(new Microsoft.Reporting.WinForms.ReportParameter("ngay_tinh", dt.Value.Date.ToString("dd/MM/yyyy")));
        }

        // [STT 59 - Form Template Method] hook 3: du lieu rieng = ma san pham het han den ngay chon
        protected override void NapDuLieu()
        {
            IList<CuahangNongduoc.BusinessObject.MaSanPham> data = CuahangNongduoc.Controller.MaSanPhamController.LayMaSanPhamHetHan(dt.Value.Date);
            this.MaSanPhamBindingSource.DataSource = data;
        }

        // TRUOC: nut Xem tu lam ca 4 buoc (data -> param -> SetParameters -> Refresh)
        // SAU  : nut Xem chi goi khung HienThiBaoCao() cua lop cha
        private void btnXem_Click(object sender, EventArgs e)
        {
            HienThiBaoCao();
        }

        private void frmSanphamHethan_Load(object sender, EventArgs e)
        {

        }
    }
}
