using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!DataService.OpenConnection()) return;

            using (frmDangNhap f = new frmDangNhap())
            {
                if (f.ShowDialog() != DialogResult.OK) return;
            }

            Application.Run(new frmMain());
        }
    }
}