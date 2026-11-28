using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// Mẫu Factory Method: lớp tạo (Creator).
    /// Mỗi lớp con quyết định tạo bộ xử lý phiếu nào (XuLyPhieu cụ thể).
    /// </summary>
    public abstract class PhieuCreator
    {
        // Factory Method
        public abstract XuLyPhieu TaoXuLy();
    }

    /// <summary>
    /// Creator cụ thể: tạo bộ xử lý PHIẾU NHẬP.
    /// </summary>
    public class PhieuNhapCreator : PhieuCreator
    {
        private PhieuNhapController m_Ctrl;
        private MaSanPhamController m_CtrlMaSP;

        public PhieuNhapCreator(PhieuNhapController ctrl, MaSanPhamController ctrlMaSP)
        {
            m_Ctrl = ctrl;
            m_CtrlMaSP = ctrlMaSP;
        }

        public override XuLyPhieu TaoXuLy()
        {
            return new XuLyPhieuNhap(m_Ctrl, m_CtrlMaSP);
        }
    }

    /// <summary>
    /// Creator cụ thể: tạo bộ xử lý PHIẾU CHI.
    /// </summary>
    public class PhieuChiCreator : PhieuCreator
    {
        private PhieuChiController m_Ctrl;

        public PhieuChiCreator(PhieuChiController ctrl)
        {
            m_Ctrl = ctrl;
        }

        public override XuLyPhieu TaoXuLy()
        {
            return new XuLyPhieuChi(m_Ctrl);
        }
    }
}
