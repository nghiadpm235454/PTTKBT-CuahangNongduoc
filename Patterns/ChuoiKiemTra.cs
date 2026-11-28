using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuahangNongduoc.Security;

namespace CuahangNongduoc.Patterns
{
    public class YeuCau { public string MaChucNang; public object DuLieu; }

    public class KetQuaKiemTra
    {
        public bool ThanhCong;
        public string ThongBao;
        public static KetQuaKiemTra Ok() { return new KetQuaKiemTra { ThanhCong = true, ThongBao = "" }; }
        public static KetQuaKiemTra Loi(string tb) { return new KetQuaKiemTra { ThanhCong = false, ThongBao = tb }; }
    }

    public abstract class BuocKiemTra
    {
        private BuocKiemTra _ke;
        public BuocKiemTra Noi(BuocKiemTra ke) { _ke = ke; return ke; }

        public KetQuaKiemTra Xu(YeuCau y)
        {
            KetQuaKiemTra kq = Kiem(y);
            if (!kq.ThanhCong || _ke == null) return kq;
            return _ke.Xu(y);
        }
        protected abstract KetQuaKiemTra Kiem(YeuCau y);
    }

    public class KiemTraDaDangNhap : BuocKiemTra
    {
        protected override KetQuaKiemTra Kiem(YeuCau y)
        {
            return PhienDangNhap.Hien.DaDangNhap ? KetQuaKiemTra.Ok() : KetQuaKiemTra.Loi("Bạn chưa đăng nhập.");
        }
    }

    public class KiemTraQuyen : BuocKiemTra
    {
        protected override KetQuaKiemTra Kiem(YeuCau y)
        {
            return PhienDangNhap.CoQuyen(y.MaChucNang)
                ? KetQuaKiemTra.Ok()
                : KetQuaKiemTra.Loi("Bạn không có quyền sử dụng chức năng này.");
        }
    }

    public class KiemTraNghiepVu : BuocKiemTra
    {
        private readonly Func<YeuCau, KetQuaKiemTra> _f;
        public KiemTraNghiepVu(Func<YeuCau, KetQuaKiemTra> f) { _f = f; }
        protected override KetQuaKiemTra Kiem(YeuCau y) { return _f(y); }
    }

    public static class ChuoiKiemTra
    {
        public static BuocKiemTra Tao(Func<YeuCau, KetQuaKiemTra> nghiepVu = null)
        {
            BuocKiemTra dau = new KiemTraDaDangNhap();
            BuocKiemTra cuoi = dau.Noi(new KiemTraQuyen());
            if (nghiepVu != null) cuoi.Noi(new KiemTraNghiepVu(nghiepVu));
            return dau;
        }
    }
}
