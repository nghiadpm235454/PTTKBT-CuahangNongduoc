using System;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc
{
    // UNIT OF WORK (P01 Chu Ba): gom nhieu lenh vao MOT giao dich.
    // Dispose ma chua Commit thi tu Rollback.
    public class UnitOfWork : IDisposable
    {
        private readonly SqlConnection cn;
        private readonly SqlTransaction tx;
        private bool xong;

        public UnitOfWork()
        {
            cn = DatabaseManager.Instance.MoKetNoi();
            tx = cn.BeginTransaction();
        }

        public SqlCommand TaoLenh(string sql)
        {
            SqlCommand c = cn.CreateCommand();
            c.Transaction = tx;
            c.CommandText = sql;
            return c;
        }

        public void Commit() { tx.Commit(); xong = true; }

        public void Rollback()
        {
            if (xong) return;
            try { tx.Rollback(); }
            catch (Exception ex) { AppLogger.Instance.Loi("UnitOfWork.Rollback", ex); }
            xong = true;
        }

        public void Dispose()
        {
            Rollback();
            tx.Dispose();
            cn.Dispose();
        }
    }
}
