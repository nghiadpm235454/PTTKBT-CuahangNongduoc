using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// Một dòng của lưới nhập hàng, dùng để cập nhật lại giá nhập sản phẩm (bước 5).
    /// </summary>
    public class DongGiaNhap
    {
        public String IdSanPham { get; set; }
        public long DonGiaNhap { get; set; }
        public long SoLuong { get; set; }
    }

    /// <summary>
    /// Phiếu nhập: viết lại từng bước của Template Method XuLyPhieu.
    /// Các bước lấy đúng từ luồng ThemMoi() của frmNhapHang.
    /// </summary>
    public class XuLyPhieuNhap : XuLyPhieu
    {
        private PhieuNhapController m_Ctrl;
        private MaSanPhamController m_CtrlMaSP;
        private DataRow m_DongMoi;

        // Dữ liệu phiếu (form gán trước khi gọi Thuchien)
        public String Id { get; set; }
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }
        public object IdNhaCungCap { get; set; }
        public decimal DaTra { get; set; }
        public decimal ConNo { get; set; }
        public IList<DongGiaNhap> DanhSachGiaNhap { get; set; }

        // ctrl và ctrlMaSP là 2 controller form đang dùng (đã chứa các dòng chi tiết thêm từ lưới)
        public XuLyPhieuNhap(PhieuNhapController ctrl, MaSanPhamController ctrlMaSP)
        {
            m_Ctrl = ctrl;
            m_CtrlMaSP = ctrlMaSP;
            DanhSachGiaNhap = new List<DongGiaNhap>();
        }

        // Bước 1: mã phiếu không rỗng, nhà cung cấp hợp lệ và mã phiếu chưa tồn tại
        protected override bool KiemTraHopLe()
        {
            if (String.IsNullOrWhiteSpace(Id))
            {
                ThongBaoLoi = "Chưa nhập mã phiếu nhập !";
                return false;
            }
            Id = Id.Trim();

            if (IdNhaCungCap == null || IdNhaCungCap == DBNull.Value || String.IsNullOrWhiteSpace(Convert.ToString(IdNhaCungCap)))
            {
                ThongBaoLoi = "Chưa chọn nhà cung cấp !";
                return false;
            }
            if (TongTien < 0 || DaTra < 0)
            {
                ThongBaoLoi = "Số tiền phiếu nhập không hợp lệ !";
                return false;
            }

            // Dùng controller RIÊNG để kiểm tra, vì LayPhieuNhap() nạp lại bảng của controller gọi nó
            PhieuNhapController ctrlKiemTra = new PhieuNhapController();
            if (ctrlKiemTra.LayPhieuNhap(Id) != null)
            {
                ThongBaoLoi = "Mã Phiếu nhập này đã tồn tại !";
                return false;
            }
            return true;
        }

        // Bước 2: tạo dòng phiếu nhập
        protected override void TaoDongPhieu()
        {
            m_DongMoi = m_Ctrl.NewRow();
            m_DongMoi["ID"] = Id;
            m_DongMoi["NGAY_NHAP"] = NgayNhap.Date;
            m_DongMoi["TONG_TIEN"] = TongTien;
            m_DongMoi["ID_NHA_CUNG_CAP"] = IdNhaCungCap;
            m_DongMoi["DA_TRA"] = DaTra;
            m_DongMoi["CON_NO"] = ConNo;
            m_Ctrl.Add(m_DongMoi);
        }

        // Bước 3: lưu phiếu rồi lưu các dòng chi tiết (lô)
        protected override bool LuuPhieu()
        {
            if (!m_Ctrl.Save())
            {
                if (m_DongMoi != null && m_DongMoi.RowState == DataRowState.Added)
                {
                    m_DongMoi.RejectChanges();
                }
                ThongBaoLoi = "Không thể lưu phiếu nhập vào cơ sở dữ liệu !";
                return false;
            }

            if (DanhSachGiaNhap.Count > 0 && !m_CtrlMaSP.Save())
            {
                ThongBaoLoi = "Không thể lưu chi tiết lô sản phẩm của phiếu nhập !";
                return false;
            }
            return true;
        }

        // Bước 4: nếu mã là số thì đẩy bộ đếm mã phiếu nhập lên
        protected override void CapNhatMaPhieu()
        {
            if (ThamSo.LaSoNguyen(Id))
            {
                long so = Convert.ToInt64(Id);
                if (so >= ThamSo.LayMaPhieuNhap())
                {
                    ThamSo.GanMaPhieuNhap(so + 1);
                }
            }
        }

        // Bước 5: cập nhật giá nhập cho từng sản phẩm trong phiếu
        protected override void CapNhatSauKhiLuu()
        {
            SanPhamController ctrlSP = new SanPhamController();
            foreach (DongGiaNhap dong in DanhSachGiaNhap)
            {
                if (!String.IsNullOrWhiteSpace(dong.IdSanPham))
                {
                    ctrlSP.CapNhatGiaNhap(dong.IdSanPham, dong.DonGiaNhap, dong.SoLuong);
                }
            }
        }
    }
}
