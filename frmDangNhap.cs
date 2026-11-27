using CuahangNongduoc.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    // Thong tin nguoi dung dang dang nhap, dung o bat ky form nao: PhienDangNhap.HoTen ...
    public static class PhienDangNhap
    {
        public static int Id;
        public static string TenDangNhap = "";
        public static string HoTen = "";
        public static string Role = "";
        public static bool LaAdmin
        {
            get { return Role.ToLower() == "admin"; }
        }
    }

    public partial class frmDangNhap : Form
    {
        private int m_SoLanSai = 0;

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
            string ten = txtTen.Text.Trim();
            string mk = txtMatKhau.Text;

            if (ten.Length == 0 || mk.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Đăng nhập",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NguoiDungFactory factory = new NguoiDungFactory();
            DataTable tbl = factory.LayNguoiDung(ten);

            if (tbl.Rows.Count == 0 || !HopLe(tbl.Rows[0], mk))
            {
                m_SoLanSai++;
                if (m_SoLanSai >= 5)
                {
                    MessageBox.Show("Sai quá 5 lần. Chương trình sẽ thoát.", "Đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu, hoặc tài khoản đã bị khóa.", "Đăng nhập",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMatKhau.Clear();
                txtMatKhau.Focus();
                return;
            }

            DataRow row = tbl.Rows[0];
            PhienDangNhap.Id = Convert.ToInt32(row["ID"]);
            PhienDangNhap.TenDangNhap = Convert.ToString(row["TEN_DANG_NHAP"]);
            PhienDangNhap.HoTen = Convert.ToString(row["HOTEN"]);
            PhienDangNhap.Role = Convert.ToString(row["ROLE"]);

            this.DialogResult = DialogResult.OK;
        }

        // Kiem tra tai khoan dang hoat dong va mat khau dung
        private bool HopLe(DataRow row, string matKhauNhap)
        {
            if (row["IS_ACTIVE"] != DBNull.Value && !Convert.ToBoolean(row["IS_ACTIVE"]))
                return false;

            string luu = Convert.ToString(row["MATKHAU"]);
            string salt = row["SALT"] == DBNull.Value ? "" : Convert.ToString(row["SALT"]);

            // Hien tai mat khau luu dang van ban thuong (SALT = NULL) nhu trong script.
            if (salt.Length == 0)
                return luu == matKhauNhap;

            // Neu sau nay ban ma hoa mat khau (co SALT), them cach so sanh o day.
            return false;
        }
    }
}