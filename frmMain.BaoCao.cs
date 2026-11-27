using System;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    // Thêm 2 mục menu báo cáo của P10 (UI-06, UI-07) mà không sửa frmMain.Designer.cs.
    // Được gọi 1 lần trong constructor frmMain, ngay sau InitializeComponent().
    public partial class frmMain
    {
        private frmBaoCaoThongKe m_BaoCaoThongKe = null;
        private frmBaoCaoNhanVien m_BaoCaoNhanVien = null;

        private void ThemMenuBaoCaoP10()
        {
            ToolStripMenuItem mnuThongKe = new ToolStripMenuItem("Báo cáo thống kê");
            mnuThongKe.Name = "mnuBaoCaoThongKe";
            mnuThongKe.Click += new EventHandler(mnuBaoCaoThongKe_Click);

            ToolStripMenuItem mnuNhanVien = new ToolStripMenuItem("Giảm giá / khuyến mãi theo nhân viên");
            mnuNhanVien.Name = "mnuBaoCaoNhanVien";
            mnuNhanVien.Click += new EventHandler(mnuBaoCaoNhanVien_Click);

            mnuBaocao.DropDownItems.Add(new ToolStripSeparator());
            mnuBaocao.DropDownItems.Add(mnuThongKe);
            mnuBaocao.DropDownItems.Add(mnuNhanVien);
        }

        private void mnuBaoCaoThongKe_Click(object sender, EventArgs e)
        {
            if (m_BaoCaoThongKe == null || m_BaoCaoThongKe.IsDisposed)
            {
                m_BaoCaoThongKe = new frmBaoCaoThongKe();
                m_BaoCaoThongKe.MdiParent = this;
                m_BaoCaoThongKe.Show();
            }
            else
                m_BaoCaoThongKe.Activate();
        }

        private void mnuBaoCaoNhanVien_Click(object sender, EventArgs e)
        {
            if (m_BaoCaoNhanVien == null || m_BaoCaoNhanVien.IsDisposed)
            {
                m_BaoCaoNhanVien = new frmBaoCaoNhanVien();
                m_BaoCaoNhanVien.MdiParent = this;
                m_BaoCaoNhanVien.Show();
            }
            else
                m_BaoCaoNhanVien.Activate();
        }
    }
}
