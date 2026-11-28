using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmInPhieuNhap : Form
    {
        CuahangNongduoc.BusinessObject.PhieuNhap m_PhieuNhap = null;
        public frmInPhieuNhap(CuahangNongduoc.BusinessObject.PhieuNhap ph)
        {
            m_PhieuNhap = ph;
            InitializeComponent();

            //reportViewer.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence);
            this.reportViewer.LocalReport.SubreportProcessing += new Microsoft.Reporting.WinForms.SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
        }

        void LocalReport_SubreportProcessing(object sender, Microsoft.Reporting.WinForms.SubreportProcessingEventArgs e)
        {
            e.DataSources.Clear();
            e.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("CuahangNongduoc_BusinessObject_MaSanPham", m_PhieuNhap.ChiTiet));
        }

        private void frmInPhieuNhap_Load(object sender, EventArgs e)
        {
            if (m_PhieuNhap == null)
            {
                MessageBox.Show("Không có dữ liệu phiếu nhập để in!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            try
            {
                Num2Str num = new Num2Str();
                IList<Microsoft.Reporting.WinForms.ReportParameter> param = new List<Microsoft.Reporting.WinForms.ReportParameter>();
                CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();

                string tenCH = (ch != null && ch.TenCuaHang != null) ? ch.TenCuaHang : "";
                string diaChi = (ch != null && ch.DiaChi != null) ? ch.DiaChi : "";
                string dienThoai = (ch != null && ch.DienThoai != null) ? ch.DienThoai : "";
                string bangChu = num.NumberToString(Convert.ToInt64(m_PhieuNhap.TongTien).ToString()) ?? "";

                param.Add(new Microsoft.Reporting.WinForms.ReportParameter("ten_cua_hang", tenCH));
                param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dia_chi", diaChi));
                param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dien_thoai", dienThoai));
                param.Add(new Microsoft.Reporting.WinForms.ReportParameter("bang_chu", bangChu));

                this.reportViewer.LocalReport.SetParameters(param);
                this.PhieuNhapBindingSource.DataSource = m_PhieuNhap;
                this.reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                Exception cur = ex;
                while (cur != null)
                {
                    sb.AppendLine(cur.Message);
                    cur = cur.InnerException;
                }
                MessageBox.Show(sb.ToString(), "Chi tiết lỗi RDLC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void reportViewer_Load(object sender, EventArgs e)
        {

        }
    }
}