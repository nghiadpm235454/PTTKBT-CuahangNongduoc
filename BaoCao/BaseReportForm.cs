using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace CuahangNongduoc
{
    // ======================================================================================
    //  [REFACTORING STT 57]  Extract Superclass   (nhom: Dealing with Generalization)
    //  Huong dan : https://refactoring.guru/extract-superclass
    //  [REFACTORING STT 59]  Form Template Method (nhom: Dealing with Generalization)
    //  Huong dan : https://refactoring.guru/form-template-method
    //  Phu trach : P10 - Nguyen
    //  --------------------------------------------------------------------------------------
    //  TRUOC : 8 form tu viet lai cung mot day lenh (copy-paste) trong Load / nut Xem:
    //              1) tao danh sach ReportParameter
    //              2) reportViewer.LocalReport.SetParameters(param)
    //              3) gan  xxxBindingSource.DataSource = du lieu
    //              4) reportViewer.RefreshReport()
    //          Cac form: frmSoLuongTon, frmSanphamHethan, frmSoLuongBan (x2 nut),
    //                    frmInPhieuBan, frmInPhieuNhap, frmInPhieuChi,
    //                    frmInPhieuThanhToan, frmInDunoKhachHang. Cung ke thua truc tiep Form.
    //  SAU   : - Extract Superclass : tao lop cha BaseReportForm gom phan chung (thu tu 4 buoc).
    //          - Form Template Method: HienThiBaoCao() la "khung thuat toan" co dinh 4 buoc;
    //            cac buoc khac nhau giua cac form duoc day xuong lop con qua 3 "hook":
    //                BoXemBaoCao  (lay reportViewer cua form)
    //                ThemThamSo   (them tham so rieng)
    //                NapDuLieu    (gan nguon du lieu rieng)
    //  Bo sung: HienThiBaoCao() bat loi ReportViewer va hien Inner Exception (de biet nguyen nhan that).
    //  Luu y : khong dung "abstract" vi Visual Studio Designer khong mo duoc form con ke thua
    //          tu lop abstract; vi vay cac hook la "protected virtual" co thuc thi mac dinh.
    // ======================================================================================
    public class BaseReportForm : Form, IReportOutput
    {
        private bool m_DaChayTrongAppDomain = false;

        /// <summary>HOOK 1 - lop con tra ve control ReportViewer (khai bao trong file Designer cua no).</summary>
        protected virtual ReportViewer BoXemBaoCao
        {
            get { return null; }
        }

        /// <summary>HOOK 2 - lop con them cac ReportParameter rieng (mac dinh: khong co).</summary>
        protected virtual void ThemThamSo(IList<ReportParameter> ds)
        {
        }

        /// <summary>HOOK 3 - lop con gan nguon du lieu cho BindingSource (mac dinh: khong lam gi).</summary>
        protected virtual void NapDuLieu()
        {
        }

        /// <summary>
        /// TEMPLATE METHOD (STT 59): khung 4 buoc co dinh, cac form con khong duoc viet lai thu tu nay.
        /// </summary>
        public void HienThiBaoCao()
        {
            ReportViewer viewer = BoXemBaoCao;
            if (viewer == null) return;

            try
            {
                // Bao cao dung doi tuong long nhau (vd Fields!SanPham.Value.TenSanPham) nen phai chay trong AppDomain hien tai,
                // neu khong cot se hien #Error / loi group expression. Chi can goi 1 lan, truoc khi xu ly bao cao dau tien.
                // (Can <NetFx40_LegacySecurityPolicy enabled="true"/> trong app.config.)
                if (!m_DaChayTrongAppDomain)
                {
                    // [FIX-09] Bao cao cua P10 da dung thuoc tinh PHANG nen KHONG phu thuoc buoc nay;
                    // neu may khong ho tro (thieu LegacySecurityPolicy) thi bo qua thay vi lam hong ca bao cao.
                    try { viewer.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence); }
                    catch (Exception) { }
                    m_DaChayTrongAppDomain = true;
                }

                // Buoc 1 + 2: tham so (neu form co tham so)
                IList<ReportParameter> ds = new List<ReportParameter>();
                ThemThamSo(ds);
                if (ds.Count > 0)
                    viewer.LocalReport.SetParameters(ds);

                // Buoc 3: du lieu
                NapDuLieu();

                // Buoc 4: ve lai bao cao
                viewer.RefreshReport();
            }
            catch (Exception ex)
            {
                // Gom TAT CA ReportViewer loi ve 1 cho: hien nguyen nhan that (Inner Exception)
                // thay vi de chuong trinh sap voi thong bao chung chung "An error occurred during local report processing".
                MessageBox.Show(MoTaLoi(ex), "Loi hien thi bao cao", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Ghep thong diep cua exception va moi Inner Exception ben trong no.</summary>
        private static string MoTaLoi(Exception ex)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (Exception e = ex; e != null; e = e.InnerException)
            {
                if (sb.Length > 0) sb.AppendLine().AppendLine("--> Inner: ");
                sb.AppendLine(e.GetType().Name + ": " + e.Message);
            }
            string all = sb.ToString();
            if (all.IndexOf("ProcessingObjectModel", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sb.AppendLine().AppendLine("=> THIEU THU VIEN Microsoft.ReportViewer.ProcessingObjectModel (10.0.0.0).");
                sb.AppendLine("   Cach sua: Visual Studio > Tools > NuGet Package Manager > Package Manager Console, chay:");
                sb.AppendLine("   Install-Package Microsoft.ReportViewer.WinForms -Version 10.0.40219.1");
                sb.AppendLine("   (hoac cai 'Microsoft Report Viewer 2010 SP1 Redistributable'), roi Rebuild.");
            }
            return sb.ToString();
        }
    }
}
