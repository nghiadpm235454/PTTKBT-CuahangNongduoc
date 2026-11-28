using System;
using System.Data.SqlClient;

namespace CuahangNongduoc
{
    // MAU: SINGLETON (P01 Chu Ba) - chuoi ket noi duy nhat cua ung dung.
    // static readonly: CLR bao dam khoi tao an toan da luong.
    public sealed class DatabaseManager
    {
        private static readonly DatabaseManager instance = new DatabaseManager();
        public static DatabaseManager Instance { get { return instance; } }

        // Chuoi mac dinh lay tu DataService cu. Sua Server/Database cho phu hop may cua ban.
        private string chuoiKetNoi =
            @"Server=.\SQLEXPRESS01;Database=CuahangNongduoc;Integrated Security=True;TrustServerCertificate=True;";

        private DatabaseManager() { }

        public string ChuoiKetNoi { get { return chuoiKetNoi; } }

        public void DatChuoiKetNoi(string chuoi)
        {
            if (string.IsNullOrEmpty(chuoi)) throw new ArgumentException("Chuoi ket noi rong");
            chuoiKetNoi = chuoi;
        }

        /// <summary>Mo mot ket noi moi (nguoi goi phai Dispose).</summary>
        public SqlConnection MoKetNoi()
        {
            SqlConnection c = new SqlConnection(chuoiKetNoi);
            c.Open();
            return c;
        }
    }
}
