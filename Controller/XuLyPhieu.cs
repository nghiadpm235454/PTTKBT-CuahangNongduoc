using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// Mẫu Template Method: khung xử lý phiếu cố định 5 bước.
    /// Các lớp con (phiếu nhập, phiếu chi...) chỉ viết lại từng bước.
    /// </summary>
    public abstract class XuLyPhieu
    {
        private String m_ThongBaoLoi = "";

        // Nội dung lỗi để form hiển thị khi Thuchien() trả về false
        public String ThongBaoLoi
        {
            get { return m_ThongBaoLoi; }
            protected set { m_ThongBaoLoi = value; }
        }

        // Template Method: thứ tự 5 bước cố định, lớp con KHÔNG được đổi (không virtual)
        public bool Thuchien()
        {
            // Bước 1: kiểm tra dữ liệu hợp lệ
            if (!KiemTraHopLe())
                return false;

            // Bước 2: tạo dòng phiếu mới
            TaoDongPhieu();

            // Bước 3: lưu phiếu và chi tiết (lưu thất bại thì dừng, không tăng bộ đếm)
            if (!LuuPhieu())
                return false;

            // Bước 4: cập nhật bộ đếm mã phiếu sau khi lưu thành công
            CapNhatMaPhieu();

            // Bước 5: việc cần làm sau khi lưu (giá nhập, tồn kho...)
            CapNhatSauKhiLuu();

            return true;
        }

        // Các bước bắt buộc lớp con phải viết
        protected abstract bool KiemTraHopLe();
        protected abstract void TaoDongPhieu();
        protected abstract bool LuuPhieu();

        // Các bước tùy chọn (hook): mặc định không làm gì
        protected virtual void CapNhatMaPhieu() { }
        protected virtual void CapNhatSauKhiLuu() { }
    }
}
