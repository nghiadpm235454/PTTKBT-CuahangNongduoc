using System;

namespace CuahangNongduoc
{
    // ======================================================================================
    //  [REFACTORING STT 58]  Extract Interface   (nhom: Dealing with Generalization)
    //  Huong dan : https://refactoring.guru/extract-interface
    //  Phu trach : P10 - Nguyen
    //  --------------------------------------------------------------------------------------
    //  TRUOC : 8 form bao cao/in phieu (frmSoLuongTon, frmSanphamHethan, frmSoLuongBan,
    //          frmInPhieuBan, frmInPhieuNhap, frmInPhieuChi, frmInPhieuThanhToan,
    //          frmInDunoKhachHang) cung co mot viec "hien thi bao cao" nhung khong co
    //          kieu chung nao de noi ve viec do. Muon goi chung phai biet ro tung lop cu the.
    //  SAU   : tach phan giao dien chung ra thanh interface IReportOutput.
    //          BaseReportForm cai dat interface nay => ben ngoai chi can biet "IReportOutput",
    //          khong can biet form cu the (phu thuoc vao truu tuong, khong vao lop cu the).
    //  Ap dung tai: BaoCao/BaseReportForm.cs  (class BaseReportForm : Form, IReportOutput)
    //  Vi du dung: foreach (Form f in this.MdiChildren)
    //                  { IReportOutput r = f as IReportOutput; if (r != null) r.HienThiBaoCao(); }
    // ======================================================================================
    public interface IReportOutput
    {
        /// <summary>Nap tham so + du lieu roi ve lai bao cao tren man hinh.</summary>
        void HienThiBaoCao();
    }
}
