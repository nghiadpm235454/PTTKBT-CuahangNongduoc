using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    // [REFACTORING STT 57 - Extract Superclass] TRUOC: ke thua Form, tu viet 2 dong nap du lieu + RefreshReport.
    //                                           SAU  : ke thua BaseReportForm (BaoCao/BaseReportForm.cs).
    public partial class frmSoLuongTon : BaseReportForm
    {
        public frmSoLuongTon()
        {
            InitializeComponent();
        }

        // [STT 59 - Form Template Method] hook 1: cho lop cha biet reportViewer nao can ve
        protected override Microsoft.Reporting.WinForms.ReportViewer BoXemBaoCao
        {
            get { return this.reportViewer; }
        }

        // [STT 59 - Form Template Method] hook 3: buoc rieng cua form nay = lay so luong ton
        //   TRUOC: data = LaySoLuongTon(); BindingSource.DataSource = data; reportViewer.RefreshReport();
        //   SAU  : chi con dong gan du lieu; SetParameters/Refresh nam o BaseReportForm.HienThiBaoCao()
        protected override void NapDuLieu()
        {
            IList<CuahangNongduoc.BusinessObject.SoLuongTon> data = CuahangNongduoc.Controller.SanPhamController.LaySoLuongTon();
            this.SoLuongTonBindingSource.DataSource = data;
        }

        private void frmSoLuongTon_Load(object sender, EventArgs e)
        {
            HienThiBaoCao();   // goi Template Method cua lop cha
        }
    }
}
