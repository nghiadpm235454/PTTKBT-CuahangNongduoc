using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DataLayer
{
    public class DangNhapFactory
    {
        // Sửa đúng 1 dòng này nếu DataService của bạn đổi tên/quyền truy cập field chuỗi kết nối
        private static string KetNoi() { return DataService.m_ConnectString; }

        public DataTable LayTheoTen(string ten)
        {
            using (SqlConnection cn = new SqlConnection(KetNoi()))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT ID, TEN_DANG_NHAP, MATKHAU, SALT, ROLE, HOTEN, IS_ACTIVE, MUST_CHANGE_PWD " +
                "FROM NGUOI_DUNG WHERE TEN_DANG_NHAP = @ten", cn))
            {
                cmd.Parameters.AddWithValue("@ten", ten);
                DataTable tbl = new DataTable();
                new SqlDataAdapter(cmd).Fill(tbl);
                return tbl;
            }
        }

        // Đọc bảng QUYEN (vai trò -> chức năng). Ném lỗi nếu chưa chạy script CN09_VaiTro_Quyen.sql;
        // PhanQuyen.NapTuCsdl() bắt lỗi đó và dùng bảng mặc định.
        public DataTable LayQuyen()
        {
            using (SqlConnection cn = new SqlConnection(KetNoi()))
            using (SqlCommand cmd = new SqlCommand("SELECT MA_VAI_TRO, MA_CHUC_NANG FROM QUYEN", cn))
            {
                DataTable tbl = new DataTable();
                new SqlDataAdapter(cmd).Fill(tbl);
                return tbl;
            }
        }

        public void CapNhatMatKhau(int id, string hash, string muoi)
        {
            using (SqlConnection cn = new SqlConnection(KetNoi()))
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE NGUOI_DUNG SET MATKHAU = @mk, SALT = @muoi, UPDATED_AT = GETDATE() WHERE ID = @id", cn))
            {
                cmd.Parameters.AddWithValue("@mk", hash);
                cmd.Parameters.AddWithValue("@muoi", muoi);
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
