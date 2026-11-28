using System;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public abstract class BaseFactory
    {
        protected DataService m_Ds = new DataService();

        public DataRow NewRow()
        {
            return m_Ds.NewRow();
        }
        public void Add(DataRow row)
        {
            m_Ds.Rows.Add(row);
        }
        public virtual bool Save()
        {
            return m_Ds.ExecuteNoneQuery() > 0;
        }
    }
}
