using CuahangNongduoc.Security;
using System;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    // Thông tin người đang đăng nhập: dùng CuahangNongduoc.Security.PhienDangNhap (ở bất kỳ form nào),
    // ví dụ PhienDangNhap.HoTen, PhienDangNhap.Hien.Id (ghi vào PHIEU_BAN.ID_NGUOI_DUNG).
    public partial class frmDangNhap : Form
    {
        private int m_SoLanSai = 0;
        private readonly DangNhapService m_Service = new DangNhapService();

        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void chkHienMk_CheckedChanged(object sender, EventArgs e)
        {
            txtMatKhau.UseSystemPasswordChar = !chkHienMk.Checked;
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            KetQuaDangNhap kq = m_Service.DangNhap(txtTen.Text.Trim(), txtMatKhau.Text);

            if (kq.ThanhCong)
            {
                this.DialogResult = DialogResult.OK;
                return;
            }

            // Thiếu thông tin / lỗi hệ thống: báo lỗi, KHÔNG tính vào số lần nhập sai.
            if (kq.Loai != LoaiKetQua.SaiThongTin)
            {
                MessageBox.Show(kq.ThongBao, "Đăng nhập", MessageBoxButtons.OK,
                    kq.Loai == LoaiKetQua.ThieuThongTin ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
                return;
            }

            m_SoLanSai++;
            bool quaSoLan = m_SoLanSai >= DangNhapService.SO_LAN_SAI_TOI_DA;

            // [RF-32 Consolidate Duplicate Conditional Fragments]
            // Trước: if (m_SoLanSai >= 5) { MessageBox.Show("Sai quá 5 lần...", "Đăng nhập", OK, Error); DialogResult = Cancel; return; }
            //        else                 { MessageBox.Show("Sai tên đăng nhập...", "Đăng nhập", OK, Error); txtMatKhau.Clear(); txtMatKhau.Focus(); }
            //        (lệnh MessageBox.Show giống hệt nhau ở cả 2 nhánh, chỉ khác nội dung)
            // Sau: nhánh chỉ chọn nội dung; MessageBox.Show nằm ngoài if, viết 1 lần.
            string thongBao = quaSoLan
                ? string.Format("Sai quá {0} lần. Chương trình sẽ thoát.", DangNhapService.SO_LAN_SAI_TOI_DA)
                : kq.ThongBao;
            MessageBox.Show(thongBao, "Đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (quaSoLan)
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            txtMatKhau.Clear();
            txtMatKhau.Focus();
        }
    }
}
