<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmInPhieuChi : Form
    {
        CuahangNongduoc.BusinessObject.PhieuChi m_PhieuChi;
        public frmInPhieuChi(CuahangNongduoc.BusinessObject.PhieuChi ph)
        {
            InitializeComponent();
            m_PhieuChi = ph;
        }

        private void frmInPhieuChi_Load(object sender, EventArgs e)
        {
            Num2Str num = new Num2Str();
            IList<Microsoft.Reporting.WinForms.ReportParameter> param = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSoManager.Instance.LayCuaHang();
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("ten_cua_hang", ch.TenCuaHang));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dia_chi", ch.DiaChi));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dien_thoai", ch.DienThoai));

            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("bang_chu", num.NumberToString(m_PhieuChi.TongTien.ToString())));

            this.reportViewer.LocalReport.SetParameters(param);
            this.PhieuChiBindingSource.DataSource = m_PhieuChi;
            this.reportViewer.RefreshReport();
        }
    }
}
=======
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
    public partial class frmInPhieuChi : BasePhieuInForm
    {
        CuahangNongduoc.BusinessObject.PhieuChi m_PhieuChi;
        public frmInPhieuChi(CuahangNongduoc.BusinessObject.PhieuChi ph)
        {
            InitializeComponent();
            m_PhieuChi = ph;
        }

        // [STT 59 - Form Template Method] hook 1: reportViewer cua form nay
        protected override Microsoft.Reporting.WinForms.ReportViewer BoXemBaoCao
        {
            get { return this.reportViewer; }
        }

        // [STT 56 - Extract Subclass] hook rieng cua phieu in: tong tien de doc thanh chu
        protected override string LaySoTien()
        {
            return m_PhieuChi.TongTien.ToString();
        }

        // [STT 59 - Form Template Method] hook 3: gan du lieu
        protected override void NapDuLieu()
        {
            this.PhieuChiBindingSource.DataSource = m_PhieuChi;
        }

        private void frmInPhieuChi_Load(object sender, EventArgs e)
        {
            HienThiBaoCao();
        }
    }
}
>>>>>>> master
