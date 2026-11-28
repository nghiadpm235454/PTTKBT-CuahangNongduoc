using System;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Security
{
    public enum LoaiKetQua { ThanhCong, ThieuThongTin, SaiThongTin, LoiHeThong }

    public class KetQuaDangNhap
    {
        public LoaiKetQua Loai;
        public string ThongBao;
        public bool ThanhCong { get { return Loai == LoaiKetQua.ThanhCong; } }

        public static KetQuaDangNhap Ok() { return new KetQuaDangNhap { Loai = LoaiKetQua.ThanhCong, ThongBao = "" }; }
        public static KetQuaDangNhap Loi(LoaiKetQua loai, string tb) { return new KetQuaDangNhap { Loai = loai, ThongBao = tb }; }
    }

    // [RF-35 Replace Conditional with Polymorphism]
    // Trước (frmDangNhap.HopLe):
    //   if (salt.Length == 0) return luu == matKhauNhap;   // mật khẩu lưu văn bản thường
    //   else                  return false;                // "sau này có SALT thì thêm cách so sánh ở đây"
    // Sau: mỗi cách lưu mật khẩu là 1 lớp cài IKiemTraMatKhau; ChonKiemTra() chọn đúng lớp theo SALT,
    //      thêm cách lưu mới = thêm 1 lớp, không sửa if/else.
    public interface IKiemTraMatKhau
    {
        bool Khop(string matKhauNhap, string matKhauLuu, string muoi);
    }

    public class MatKhauVanBan : IKiemTraMatKhau
    {
        public bool Khop(string matKhauNhap, string matKhauLuu, string muoi)
        {
            return MatKhauHelper.SoSanh(matKhauNhap, matKhauLuu);
        }
    }

    public class MatKhauBam : IKiemTraMatKhau
    {
        public bool Khop(string matKhauNhap, string matKhauLuu, string muoi)
        {
            return MatKhauHelper.SoSanh(MatKhauHelper.Bam(matKhauNhap, muoi), matKhauLuu);
        }
    }

    public class DangNhapService
    {
        public const int SO_LAN_SAI_TOI_DA = 5;

        private const string TB_THIEU = "Vui lòng nhập tên đăng nhập và mật khẩu.";
        private const string TB_SAI = "Sai tên đăng nhập hoặc mật khẩu, hoặc tài khoản đã bị khóa.";

        private readonly DangNhapFactory _factory = new DangNhapFactory();

        public static IKiemTraMatKhau ChonKiemTra(string muoi)
        {
            if (string.IsNullOrEmpty(muoi)) return new MatKhauVanBan(); // tài khoản cũ trong script SQL (SALT = NULL)
            return new MatKhauBam();
        }

        // [RF-34 Replace Nested Conditional with Guard Clauses]
        // Trước (frmDangNhap.btnDangNhap_Click + HopLe): if (...) { if (...) { if (...) { ...đăng nhập OK... } } }
        //   lồng 3 tầng: nhập đủ -> có tài khoản -> đang hoạt động -> đúng mật khẩu.
        // Sau: mỗi điều kiện sai thoát ngay bằng return; đường thành công nằm thẳng ở cuối hàm.
        public KetQuaDangNhap DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
                return KetQuaDangNhap.Loi(LoaiKetQua.ThieuThongTin, TB_THIEU);

            DataTable tbl;
            try { tbl = _factory.LayTheoTen(tenDangNhap.Trim()); }
            catch (Exception ex)
            {
                return KetQuaDangNhap.Loi(LoaiKetQua.LoiHeThong, "Không đọc được dữ liệu người dùng:\n" + ex.Message);
            }

            if (tbl.Rows.Count == 0)
                return KetQuaDangNhap.Loi(LoaiKetQua.SaiThongTin, TB_SAI);

            DataRow row = tbl.Rows[0];
            if (row["IS_ACTIVE"] != DBNull.Value && !Convert.ToBoolean(row["IS_ACTIVE"]))
                return KetQuaDangNhap.Loi(LoaiKetQua.SaiThongTin, TB_SAI);

            string luu = Convert.ToString(row["MATKHAU"]);
            string muoi = row["SALT"] == DBNull.Value ? "" : Convert.ToString(row["SALT"]);
            if (!ChonKiemTra(muoi).Khop(matKhau, luu, muoi))
                return KetQuaDangNhap.Loi(LoaiKetQua.SaiThongTin, TB_SAI);

            int id = Convert.ToInt32(row["ID"]);
            if (string.IsNullOrEmpty(muoi)) NangCapSangBam(id, matKhau);

            PhienDangNhap.DatNguoiDung(TaoNguoiDung(row, id));
            PhanQuyen.NapTuCsdl();
            return KetQuaDangNhap.Ok();
        }

        // Mật khẩu cũ đang lưu văn bản thường -> băm PBKDF2 + SALT ngay lần đăng nhập đúng đầu tiên.
        // Lỗi ghi CSDL không được chặn việc đăng nhập (lần sau sẽ nâng cấp tiếp).
        private void NangCapSangBam(int id, string matKhau)
        {
            try
            {
                string muoi = MatKhauHelper.TaoMuoi();
                _factory.CapNhatMatKhau(id, MatKhauHelper.Bam(matKhau, muoi), muoi);
            }
            catch (Exception) { }
        }

        private static NguoiDung TaoNguoiDung(DataRow row, int id)
        {
            return new NguoiDung
            {
                Id = id,
                TenDangNhap = Convert.ToString(row["TEN_DANG_NHAP"]),
                HoTen = Convert.ToString(row["HOTEN"]),
                Role = Convert.ToString(row["ROLE"]).Trim(),
                MustChangePwd = row["MUST_CHANGE_PWD"] != DBNull.Value && Convert.ToBoolean(row["MUST_CHANGE_PWD"])
            };
        }
    }
}
