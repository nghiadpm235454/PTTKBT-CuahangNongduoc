using System;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;

namespace CuahangNongduoc
{
    // ======================================================================================
    //  [REFACTORING STT 56]  Extract Subclass   (nhom: Dealing with Generalization)
    //  Huong dan : https://refactoring.guru/extract-subclass
    //  Phu trach : P10 - Nguyen
    //  --------------------------------------------------------------------------------------
    //  TRUOC : 5 form IN PHIEU (frmInPhieuBan, frmInPhieuNhap, frmInPhieuChi, frmInPhieuThanhToan,
    //          frmInDunoKhachHang) deu viet lai 4 dong giong het nhau:
    //              ten_cua_hang, dia_chi, dien_thoai  (lay tu ThamSo.LayCuaHang())
    //              bang_chu                           (Num2Str.NumberToString(tong tien))
    //          Trong khi 3 form THONG KE (frmSoLuongTon, frmSanphamHethan, frmSoLuongBan)
    //          KHONG can nhung tham so nay.
    //  SAU   : phan "chi dung cho phieu in" duoc tach thanh lop con BasePhieuInForm : BaseReportForm.
    //          Form thong ke van ke thua BaseReportForm (khong bi vuong thong tin cua hang);
    //          form in phieu ke thua BasePhieuInForm va chi phai cho biet "so tien" + "du lieu".
    //  Hook moi: LaySoTien()  - moi phieu tra ve tong tien cua minh (dang chuoi).
    // ======================================================================================
    public class BasePhieuInForm : BaseReportForm
    {
        /// <summary>HOOK cua lop con: tong tien (chuoi) dung de doc thanh chu.</summary>
        protected virtual string LaySoTien()
        {
            return "0";
        }

        // Ghi de HOOK 2 cua BaseReportForm: them 4 tham so chung cua moi phieu in
        protected override void ThemThamSo(IList<ReportParameter> ds)
        {
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            ds.Add(new ReportParameter("ten_cua_hang", ch.TenCuaHang));
            ds.Add(new ReportParameter("dia_chi", ch.DiaChi));
            ds.Add(new ReportParameter("dien_thoai", ch.DienThoai));

            Num2Str num = new Num2Str();
            ds.Add(new ReportParameter("bang_chu", num.NumberToString(LaySoTien())));
        }
    }
}
