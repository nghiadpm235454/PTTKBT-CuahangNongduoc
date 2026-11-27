using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    // [REFACTORING STT 60 - Replace Inheritance with Delegation]  https://refactoring.guru/replace-inheritance-with-delegation
    // Phu trach: P10 - Nguyen (can thong nhat voi P01 vi DataService dung chung toan du an; P01 lam R04 tren cung file nay)
    //   TRUOC: public class DataService : DataTable   -> DataService "la" DataTable, ke thua ~100 thanh vien
    //          (Select, Compute, Merge, WriteXml...) nhung chi dung 6: Load, Rows, Columns, NewRow, ExecuteNoneQuery, ExecuteScalar.
    //   SAU  : public class DataService  (khong ke thua) + field m_Table (DataTable) ben trong.
    //          - Chi "uy quyen" (delegate) dung cac thanh vien dang can: Rows, Columns, NewRow().
    //          - Noi nao can cai DataTable that (de bind, tra ve cho Controller) thi dung  .Table
    //          - Cac DataLayer/*Factory.cs: doi  "return m_Ds;"  ->  "return m_Ds.Table;"
    public class DataService
    {
        // Chuoi ket noi SQL Server. Sua Server/Database cho phu hop may cua ban.
        //  - SQL Server Express: .\SQLEXPRESS"
        //  - SQL Server mac dinh : "." hoac "localhost"
        //  - Dung tai khoan SQL : "Server=...;Database=...;User Id=sa;Password=...;"
        public static String m_ConnectString =
    @"Server=.;Database=CuahangNongduoc;Integrated Security=True;TrustServerCertificate=True;";

        private static SqlConnection m_Connection;
        private SqlCommand m_Command;
        private SqlDataAdapter m_DataAdapter;

        // [STT 60] field giu DataTable thay cho quan he ke thua
        private readonly DataTable m_Table = new DataTable();

        public DataService() { }

        /// <summary>[STT 60] DataTable ben trong - dung de tra ve cho Controller / BindingSource.</summary>
        public DataTable Table
        {
            get { return m_Table; }
        }

        // [STT 60] Uy quyen (delegate) cho DataTable: chi 3 thanh vien nay duoc dung ben ngoai
        public DataRowCollection Rows
        {
            get { return m_Table.Rows; }
        }

        public DataColumnCollection Columns
        {
            get { return m_Table.Columns; }
        }

        public DataRow NewRow()
        {
            return m_Table.NewRow();
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

                m_Table.Clear();   // [STT 60] truoc: this.Clear()
                m_DataAdapter.Fill(m_Table);   // [STT 60] truoc: Fill(this)
            }
            catch (Exception e)
            {
                MessageBox.Show("Loi doc du lieu: " + e.Message, "SQL Server",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool OpenConnection()
        {
            try
            {
                if (m_Connection == null)
                    m_Connection = new SqlConnection(m_ConnectString);

                if (m_Connection.State == ConnectionState.Closed)
                    m_Connection.Open();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Khong ket noi duoc SQL Server:\n" + e.Message +
                    "\n\nKiem tra lai chuoi ket noi trong DataService.cs",
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

                result = m_DataAdapter.Update(m_Table);   // [STT 60] truoc: Update(this)

                tr.Commit();
            }
            catch (Exception e)
            {
                if (tr != null) tr.Rollback();
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

                m_Table.AcceptChanges();   // [STT 60] truoc: this.AcceptChanges()
                tr.Commit();
            }
            catch
            {
                if (tr != null) tr.Rollback();
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

                m_Table.AcceptChanges();   // [STT 60] truoc: this.AcceptChanges()
                tr.Commit();

                if (result == DBNull.Value)
                    result = null;
            }
            catch
            {
                if (tr != null) tr.Rollback();
                throw;
            }
            return result;
        }
    }
}