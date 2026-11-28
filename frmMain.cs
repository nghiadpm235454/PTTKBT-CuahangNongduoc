using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Patterns;
using CuahangNongduoc.Security;

namespace CuahangNongduoc
{
    public partial class frmMain : Form
    {
        // [RF-31 Consolidate Conditional Expression] + [Proxy]
        // Trước: 19 field form + 19 handler, mỗi handler lặp
        //          if (X == null || X.IsDisposed) { X = new frmX(); X.MdiParent = this; X.Show(); } else X.Activate();
        //        và không kiểm quyền.
        // Sau: mỗi handler chỉ gọi Mo<frmX>(ChucNang.XXX); điều kiện mở/kích hoạt nằm ở MoForm<T>,
        //      kiểm đăng nhập + quyền nằm ở MoChucNangProxy (Chain of Responsibility).
        private readonly Dictionary<Type, IMoChucNang> m_ChucNang = new Dictionary<Type, IMoChucNang>();

        public frmMain()
        {
            InitializeComponent();
            ThemMenuBaoCaoP10();
        }

        private void Mo<T>(string maChucNang) where T : Form, new()
        {
            IMoChucNang chucNang;
            if (!m_ChucNang.TryGetValue(typeof(T), out chucNang))
            {
                chucNang = new MoChucNangProxy(new MoForm<T>(), maChucNang);
                m_ChucNang[typeof(T)] = chucNang;
            }
            chucNang.Mo(this);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            DataService.OpenConnection();

            this.Text = this.Text + " - " + PhienDangNhap.HoTen + " (" + PhienDangNhap.Role + ")";
            ApDungPhanQuyen();
        }

        // ---------- Ẩn/hiện menu, thanh công cụ, task pane theo quyền (UI-05) ----------
        private void ApDungPhanQuyen()
        {
            // Menu
            Dat(mnuLyDoChi, ChucNang.QL_LYDOCHI);
            Dat(mnuDonViTinh, ChucNang.QL_DONVITINH);
            Dat(mnuSanPham, ChucNang.QL_SANPHAM);
            Dat(mnuKhachHang, ChucNang.QL_KHACHHANG);
            Dat(mnuDaiLy, ChucNang.QL_DAILY);
            Dat(mnuNhaCungCap, ChucNang.QL_NHACUNGCAP);
            Dat(mnuNhapHang, ChucNang.NHAP_HANG);
            Dat(mnuBanHangKH, ChucNang.BAN_LE);
            Dat(mnuBanHangDL, ChucNang.BAN_SI);
            Dat(mnuPhieuChi, ChucNang.PHIEU_CHI);
            Dat(mnuThanhtoan, ChucNang.PHIEU_THU);
            Dat(mnuTonghopDuno, ChucNang.TONGHOP_DUNO);
            Dat(mnuBaocaoSoluongton, ChucNang.BC_TON);
            Dat(mnuSoLuongBan, ChucNang.BC_BAN);
            Dat(mnuSanphamHethan, ChucNang.BC_HETHAN);
            Dat(mnuTuychinhThongtin, ChucNang.CAU_HINH);
            Dat(mnuTrogiupHuongdan, ChucNang.TRO_GIUP);
            Dat(mnuTrogiupLienhe, ChucNang.TRO_GIUP);

            // Thanh công cụ
            Dat(toolSanPham, ChucNang.QL_SANPHAM);
            Dat(toolKhachHang, ChucNang.QL_KHACHHANG);
            Dat(toolDaiLy, ChucNang.QL_DAILY);
            Dat(toolNhaCungCap, ChucNang.QL_NHACUNGCAP);
            Dat(toolNhapHang, ChucNang.NHAP_HANG);
            Dat(toolBanSi, ChucNang.BAN_SI);
            Dat(toolBanLe, ChucNang.BAN_LE);
            Dat(toolTonKho, ChucNang.BC_TON);
            Dat(toolPhieuChi, ChucNang.PHIEU_CHI);
            Dat(toolThanhtoan, ChucNang.PHIEU_THU);

            // Thanh chức năng (task pane)
            Dat(itemDaiLy, ChucNang.QL_DAILY);
            Dat(itemKhachHang, ChucNang.QL_KHACHHANG);
            Dat(itemSanPham, ChucNang.QL_SANPHAM);
            Dat(itemNhaCungCap, ChucNang.QL_NHACUNGCAP);
            Dat(itemNhapHang, ChucNang.NHAP_HANG);
            Dat(itemBanSi, ChucNang.BAN_SI);
            Dat(itemBanLe, ChucNang.BAN_LE);
            Dat(itemThanhToan, ChucNang.PHIEU_THU);
            Dat(itemPhieuChi, ChucNang.PHIEU_CHI);
            Dat(itemTonghopDoanhthu, ChucNang.BC_BAN);
            Dat(itemTonKho, ChucNang.BC_TON);
            Dat(taskItem1, ChucNang.BC_HETHAN);

            // Menu cha: ẩn nếu không còn mục con nào hiện
            foreach (ToolStripItem it in menuStrip.Items)
            {
                ToolStripMenuItem cha = it as ToolStripMenuItem;
                if (cha != null) CapNhatMenuCha(cha);
            }
        }

        private static void Dat(ToolStripItem muc, string maChucNang)
        {
            muc.Visible = PhienDangNhap.CoQuyen(maChucNang);
        }

        private static void Dat(Control muc, string maChucNang)
        {
            muc.Visible = PhienDangNhap.CoQuyen(maChucNang);
        }

        private static bool CapNhatMenuCha(ToolStripMenuItem cha)
        {
            // Doc Available thay vi Visible vi trong frmMain_Load form chua hien nen Visible luon tra ve false.
            if (cha.DropDownItems.Count == 0) return cha.Available;

            bool coMucCon = false;
            foreach (ToolStripItem it in cha.DropDownItems)
            {
                ToolStripMenuItem con = it as ToolStripMenuItem;
                if (con != null && CapNhatMenuCha(con)) coMucCon = true;
            }
            cha.Visible = coMucCon;
            return coMucCon;
        }

        // ---------- Handler (tên giữ nguyên vì Designer đã nối sự kiện) ----------
        private void mnuDonViTinh_Click(object sender, EventArgs e) { Mo<frmDonViTinh>(ChucNang.QL_DONVITINH); }
        private void mnuSanPham_Click(object sender, EventArgs e) { Mo<frmSanPham>(ChucNang.QL_SANPHAM); }
        private void mnuKhachHang_Click(object sender, EventArgs e) { Mo<frmKhachHang>(ChucNang.QL_KHACHHANG); }
        private void mnuDaiLy_Click(object sender, EventArgs e) { Mo<frmDaiLy>(ChucNang.QL_DAILY); }
        private void mnuNhapHang_Click(object sender, EventArgs e) { Mo<frmDanhsachPhieuNhap>(ChucNang.NHAP_HANG); }
        private void mnuBanHangKH_Click(object sender, EventArgs e) { Mo<frmDanhsachPhieuBanLe>(ChucNang.BAN_LE); }
        private void mnuBanHangDL_Click(object sender, EventArgs e) { Mo<frmDanhsachPhieuBanSi>(ChucNang.BAN_SI); }
        private void mnuThanhtoan_Click(object sender, EventArgs e) { Mo<frmThanhToan>(ChucNang.PHIEU_THU); }
        private void mnuTonghopDuno_Click(object sender, EventArgs e) { Mo<frmDunoKhachhang>(ChucNang.TONGHOP_DUNO); }
        private void mnuBaocaoDoanhThu_Click(object sender, EventArgs e) { Mo<frmDoanhThu>(ChucNang.BC_BAN); }
        private void mnuBaocaoSoluongton_Click(object sender, EventArgs e) { Mo<frmSoLuongTon>(ChucNang.BC_TON); }
        private void mnuSoLuongBan_Click(object sender, EventArgs e) { Mo<frmSoLuongBan>(ChucNang.BC_BAN); }
        private void mnuSanphamHethan_Click(object sender, EventArgs e) { Mo<frmSanphamHethan>(ChucNang.BC_HETHAN); }
        private void mnuTuychinhThongtin_Click(object sender, EventArgs e) { Mo<frmThongtinCuahang>(ChucNang.CAU_HINH); }
        private void mnuTrogiupLienhe_Click(object sender, EventArgs e) { Mo<frmThongtinLienhe>(ChucNang.TRO_GIUP); }
        private void mnuNhaCungCap_Click(object sender, EventArgs e) { Mo<frmNhaCungCap>(ChucNang.QL_NHACUNGCAP); }
        private void mnuLyDoChi_Click(object sender, EventArgs e) { Mo<frmLyDoChi>(ChucNang.QL_LYDOCHI); }
        private void mnuPhieuChi_Click(object sender, EventArgs e) { Mo<frmPhieuChi>(ChucNang.PHIEU_CHI); }

        private void mnuThanhCongCu_Click(object sender, EventArgs e)
        {
            mnuThanhCongCu.Checked = !mnuThanhCongCu.Checked;
            toolStrip.Visible = mnuThanhCongCu.Checked;
        }

        private void mnuThanhChucNang_Click(object sender, EventArgs e)
        {
            mnuThanhChucNang.Checked = !mnuThanhChucNang.Checked;
            taskPane.Visible = mnuThanhChucNang.Checked;
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuTrogiupHuongdan_Click(object sender, EventArgs e)
        {
            // Help.ShowHelp(this, "CPP.CHM");
        }
    }
}
