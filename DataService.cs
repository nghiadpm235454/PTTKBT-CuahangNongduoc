using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public class DataService : DataTable
    {
        // Chuoi ket noi SQL Server. Sua Server/Database cho phu hop may cua ban.
        //  - SQL Server Express: .\SQLEXPRESS"
        //  - SQL Server mac dinh : "." hoac "localhost"
        //  - Dung tai khoan SQL : "Server=...;Database=...;User Id=sa;Password=...;"
        // Singleton: chuoi ket noi nam o DatabaseManager (Patterns/DatabaseManager.cs).
        // Giu lai ca m_ConnectString va ChuoiKetNoi de cac cho goi cu va moi deu dung duoc.
        public static String m_ConnectString
        {
            get { return DatabaseManager.Instance.ChuoiKetNoi; }
            set { DatabaseManager.Instance.DatChuoiKetNoi(value); }
        }

        public static String ChuoiKetNoi
        {
            get { return DatabaseManager.Instance.ChuoiKetNoi; }
            set { DatabaseManager.Instance.DatChuoiKetNoi(value); }
        }

        private static SqlConnection m_Connection;
        private SqlCommand m_Command;
        private SqlDataAdapter m_DataAdapter;

        public DataService() { }

        public DataTable Table
        {
            get { return this; }
        }

        public SqlCommand Command
        {
            get { return m_Command; }
            set { m_Command = value; }
        }

        public void Load(SqlCommand command)
        {
            if (!OpenConnection()) return;
            m_Command = command;
            try
            {
                m_Command.Connection = m_Connection;

                m_DataAdapter = new SqlDataAdapter();
                m_DataAdapter.SelectCommand = m_Command;

                this.Clear();
                m_DataAdapter.Fill(this);
            }
            catch (Exception e)
            {
                AppLogger.Instance.Loi("DataService.Load", e);
                MessageBox.Show("Loi doc du lieu: " + e.Message, "SQL Server",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool OpenConnection()
        {
            try
            {
                if (m_Connection == null)
                    m_Connection = new SqlConnection(DatabaseManager.Instance.ChuoiKetNoi);

                if (m_Connection.State == ConnectionState.Closed)
                    m_Connection.Open();
                return true;
            }
            catch (Exception e)
            {
                AppLogger.Instance.Loi("DataService.OpenConnection", e);
                MessageBox.Show("Khong ket noi duoc SQL Server:\n" + e.Message +
                    "\n\nKiem tra lai chuoi ket noi trong DatabaseManager.cs",
                    "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (m_Connection != null) m_Connection.Close();
                return false;
            }
        }

        public void CloseConnection()
        {
            if (m_Connection != null) m_Connection.Close();
        }

        /// <summary>
        /// Cap nhat DataTable xuong CSDL
        /// </summary>
        public int ExecuteNoneQuery()
        {
            int result = 0;
            if (!OpenConnection()) return 0;
            SqlTransaction tr = null;
            try
            {
                tr = m_Connection.BeginTransaction();

                m_Command.Connection = m_Connection;
                m_Command.Transaction = tr;

                m_DataAdapter = new SqlDataAdapter();
                m_DataAdapter.SelectCommand = m_Command;

                SqlCommandBuilder builder = new SqlCommandBuilder(m_DataAdapter);

                result = m_DataAdapter.Update(this);

                tr.Commit();
            }
            catch (Exception e)
            {
                if (tr != null) tr.Rollback();
                AppLogger.Instance.Loi("DataService.ExecuteNoneQuery", e);
                MessageBox.Show("Loi luu du lieu: " + e.Message, "SQL Server",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        /// <summary>
        /// Thuc thi mot command (INSERT/UPDATE/DELETE)
        /// </summary>
        public int ExecuteNoneQuery(SqlCommand cmd)
        {
            int result = 0;
            OpenConnection();
            SqlTransaction tr = null;
            try
            {
                tr = m_Connection.BeginTransaction();
                cmd.Connection = m_Connection;
                cmd.Transaction = tr;

                result = cmd.ExecuteNonQuery();

                this.AcceptChanges();
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) tr.Rollback();
                AppLogger.Instance.Loi("DataService (giao dich)", ex);
                throw;
            }
            return result;
        }

        public object ExecuteScalar(SqlCommand cmd)
        {
            object result = null;
            OpenConnection();
            SqlTransaction tr = null;
            try
            {
                tr = m_Connection.BeginTransaction();
                cmd.Connection = m_Connection;
                cmd.Transaction = tr;

                result = cmd.ExecuteScalar();

                this.AcceptChanges();
                tr.Commit();

                if (result == DBNull.Value)
                    result = null;
            }
            catch (Exception ex)
            {
                if (tr != null) tr.Rollback();
                AppLogger.Instance.Loi("DataService (giao dich)", ex);
                throw;
            }
            return result;
        }
    }
}
