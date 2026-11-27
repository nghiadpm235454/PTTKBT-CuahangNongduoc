using System;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public static class GridErrorHelper
    {
        public static void Xuly(object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            string tenCot = (e.ColumnIndex >= 0) ? dgv.Columns[e.ColumnIndex].HeaderText : "";
            MessageBox.Show("Dữ liệu cột \"" + tenCot + "\" (dòng " + (e.RowIndex + 1) + ") không hợp lệ!",
                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.ThrowException = false;
            e.Cancel = true;
        }
    }
}