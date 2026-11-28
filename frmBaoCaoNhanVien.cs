using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Security;

namespace CuahangNongduoc
{
    /// <summary>
    /// UI-07 – Báo cáo hóa đơn giảm giá / khuyến mãi theo nhân viên, có nút In.
    /// REFACTORING STT 56 – Extract Subclass   https://refactoring.guru/extract-subclass
    ///   TRƯỚC: phần "theo nhân viên" nằm lẫn trong frmBaoCaoThongKe (phải có cờ/if bật tắt).
    ///   SAU:   tách thành lớp con frmBaoCaoNhanVien, chỉ dùng cho báo cáo này.
    /// REFACTORING STT 55 – Push Down Field    https://refactoring.guru/push-down-field
    ///   TRƯỚC: combo chọn nhân viên khai báo ở lớp cha frmBaoCaoThongKe dù chỉ báo cáo này dùng.
    ///   SAU:   cmbNhanVien chuyển xuống lớp con.
    /// Quyền: admin chọn bất kỳ nhân viên; nhân viên thường chỉ xem hóa đơn của chính mình (PhienDangNhap).
    /// </summary>
    public class frmBaoCaoNhanVien : frmBaoCaoThongKe
    {
        private ComboBox cmbNhanVien;   // Push Down Field

        public frmBaoCaoNhanVien()
        {
            this.Text = "Báo cáo hóa đơn giảm giá / khuyến mãi theo nhân viên";

            Label lbl = new Label();
            lbl.Text = "Nhân viên:";
            lbl.AutoSize = true;
            lbl.Margin = new Padding(6, 6, 0, 0);

            cmbNhanVien = new ComboBox();
            cmbNhanVien.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNhanVien.Width = 170;

            pnlLoc.Controls.Add(lbl);
            pnlLoc.Controls.Add(cmbNhanVien);
        }

        protected override IList<LoaiThongKe> DanhSachLoai()
        {
            return new LoaiThongKe[] { new ThongKeGiamGia(this, LocGiamGia.Ca) };
        }

        protected override int? IdNguoiDung
        {
            get
            {
                // Nhân viên thường: luôn lọc theo chính mình, không phụ thuộc combo
                if (!PhienDangNhap.LaAdmin && PhienDangNhap.Id > 0)
                    return PhienDangNhap.Id;

                if (cmbNhanVien.SelectedValue == null) return null;
                int id;
                if (!int.TryParse(Convert.ToString(cmbNhanVien.SelectedValue), out id)) return null;
                return id == 0 ? (int?)null : id;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cmbLoai.Enabled = false;   // chỉ có một loại báo cáo

            DataTable tbl = new BaoCaoFactory().DanhSachNguoiDung();
            DataRow tatCa = tbl.NewRow();
            tatCa["ID"] = 0;
            tatCa["TEN"] = "(Tất cả nhân viên)";
            tbl.Rows.InsertAt(tatCa, 0);

            cmbNhanVien.DisplayMember = "TEN";
            cmbNhanVien.ValueMember = "ID";
            cmbNhanVien.DataSource = tbl;

            if (!PhienDangNhap.LaAdmin && PhienDangNhap.Id > 0)
            {
                cmbNhanVien.SelectedValue = PhienDangNhap.Id;
                cmbNhanVien.Enabled = false;   // nhân viên chỉ xem của mình
            }
        }
    }
}
