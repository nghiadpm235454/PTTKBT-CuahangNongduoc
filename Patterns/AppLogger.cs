using System;
using System.IO;

namespace CuahangNongduoc
{
    // MAU: SINGLETON (P01 Chu Ba) - ghi nhat ky loi ra thu muc logs.
    public sealed class AppLogger
    {
        private static readonly AppLogger instance = new AppLogger();
        public static AppLogger Instance { get { return instance; } }

        private readonly object khoa = new object();
        private readonly string thuMuc;

        private AppLogger()
        {
            thuMuc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        }

        public void ThongTin(string noiDung) { Ghi("INFO", noiDung); }
        public void Loi(string nguCanh, Exception ex) { Ghi("ERROR", nguCanh + ": " + ex); }

        private void Ghi(string muc, string noiDung)
        {
            lock (khoa)
            {
                try
                {
                    Directory.CreateDirectory(thuMuc);
                    string tep = Path.Combine(thuMuc, "app-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                    File.AppendAllText(tep, DateTime.Now.ToString("HH:mm:ss") + " [" + muc + "] " + noiDung + Environment.NewLine);
                }
                catch (Exception) { /* khong duoc de loi ghi log lam hong nghiep vu */ }
            }
        }
    }
}
