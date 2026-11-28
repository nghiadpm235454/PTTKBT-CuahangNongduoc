using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// Phiếu chi: viết lại từng bước của Template Method XuLyPhieu.
    /// Các bước lấy từ luồng toolAdd_Click / toolSave_Click của frmPhieuChi.
    /// </summary>
    public class XuLyPhieuChi : XuLyPhieu
    {
        private PhieuChiController m_Ctrl;
        private long m_MaPhieu;
        private DataRow m_DongMoi;

        // Dữ liệu phiếu (form gán trước khi gọi Thuchien)
        public DateTime NgayChi { get; set; }
        public decimal TongTien { get; set; }
        public object IdLyDoChi { get; set; }
        public String GhiChu { get; set; }

        // ctrl là controller form đang dùng (đang gắn với BindingSource của lưới)
        public XuLyPhieuChi(PhieuChiController ctrl)
        {
            m_Ctrl = ctrl;
        }

        // Bước 1: phải chọn lý do chi và số tiền > 0
        // (LayPhieuChi() đọc ID_LY_DO_CHI bằng Convert.ToInt64 nên để trống sẽ gây lỗi)
        protected override bool KiemTraHopLe()
        {
            if (IdLyDoChi == null || IdLyDoChi == DBNull.Value)
            {
                ThongBaoLoi = "Chưa chọn lý do chi !";
                return false;
            }
            if (TongTien <= 0)
            {
                ThongBaoLoi = "Số tiền chi phải lớn hơn 0 !";
                return false;
            }
            return true;
        }

        // Bước 2: lấy mã phiếu kế tiếp từ bộ đếm (chỉ đọc, chưa tăng) và tạo dòng phiếu
        protected override void TaoDongPhieu()
        {
            m_MaPhieu = ThamSo.PhieuChi;

            m_DongMoi = m_Ctrl.NewRow();
            m_DongMoi["ID"] = m_MaPhieu.ToString();
            m_DongMoi["NGAY_CHI"] = NgayChi.Date;
            m_DongMoi["TONG_TIEN"] = TongTien;
            m_DongMoi["ID_LY_DO_CHI"] = IdLyDoChi;
            m_DongMoi["GHI_CHU"] = GhiChu ?? (object)DBNull.Value;
            m_Ctrl.Add(m_DongMoi);
        }

        // Bước 3: lưu phiếu chi
        protected override bool LuuPhieu()
        {
            if (!m_Ctrl.Save())
            {
                if (m_DongMoi != null && m_DongMoi.RowState == DataRowState.Added)
                {
                    m_DongMoi.RejectChanges();
                }
                ThongBaoLoi = "Không thể lưu phiếu chi vào cơ sở dữ liệu !";
                return false;
            }
            return true;
        }

        // Bước 4: tăng bộ đếm mã phiếu chi sau khi lưu thành công
        protected override void CapNhatMaPhieu()
        {
            ThamSo.PhieuChi = m_MaPhieu + 1;
        }

        // Bước 5: phiếu chi không có việc gì sau khi lưu nên không viết lại CapNhatSauKhiLuu()
    }
}
