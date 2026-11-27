using System;

namespace CuahangNongduoc.BusinessObject
{
    /// <summary>Một dòng của báo cáo thống kê dùng chung (tối đa 6 cột chữ), khớp Report\rptThongKe.rdlc.</summary>
    public class ThongKeRow
    {
        private string m_C1 = "", m_C2 = "", m_C3 = "", m_C4 = "", m_C5 = "", m_C6 = "";

        public string C1 { get { return m_C1; } set { m_C1 = value; } }
        public string C2 { get { return m_C2; } set { m_C2 = value; } }
        public string C3 { get { return m_C3; } set { m_C3 = value; } }
        public string C4 { get { return m_C4; } set { m_C4 = value; } }
        public string C5 { get { return m_C5; } set { m_C5 = value; } }
        public string C6 { get { return m_C6; } set { m_C6 = value; } }
    }
}
