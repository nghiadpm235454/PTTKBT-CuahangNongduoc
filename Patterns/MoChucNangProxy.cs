using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace CuahangNongduoc.Patterns
{
    public interface IMoChucNang { void Mo(Form cha); }

    public class MoForm<T> : IMoChucNang where T : Form, new()
    {
        private T _form;

        // [RF-31 Consolidate Conditional Expression]
        // Trước (frmMain.cs, lặp 19 lần cho 19 form):
        //   if (DonViTinh == null || DonViTinh.IsDisposed) { DonViTinh = new frmDonViTinh(); DonViTinh.MdiParent = this; DonViTinh.Show(); }
        //   else DonViTinh.Activate();
        // Sau: gom điều kiện vào CanMoMoi, dùng chung cho mọi form.
        // Lý do: bỏ 19 bản sao điều kiện, sửa 1 chỗ.
        private bool CanMoMoi { get { return _form == null || _form.IsDisposed; } }

        public void Mo(Form cha)
        {
            if (CanMoMoi)
            {
                _form = new T();
                _form.MdiParent = cha;
                _form.Show();
            }
            else
                _form.Activate();
        }
    }

    public class MoChucNangProxy : IMoChucNang
    {
        private readonly IMoChucNang _that;
        private readonly string _ma;
        public MoChucNangProxy(IMoChucNang that, string maChucNang) { _that = that; _ma = maChucNang; }

        public void Mo(Form cha)
        {
            KetQuaKiemTra kq = ChuoiKiemTra.Tao().Xu(new YeuCau { MaChucNang = _ma });
            if (!kq.ThanhCong)
            {
                MessageBox.Show(kq.ThongBao, "Phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _that.Mo(cha);
        }
    }
}
