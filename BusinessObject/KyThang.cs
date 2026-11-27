using System;

namespace CuahangNongduoc.BusinessObject
{
    /// <summary>
    /// RF-02 #21 (Replace Array with Object): thay cap tham so ref int thang, ref int nam cua ThamSo.PreMonth.
    /// RF-02 #20 (Change Reference to Value): doi tuong bat bien, so sanh theo gia tri.
    /// </summary>
    public sealed class KyThang
    {
        private readonly int m_Thang;
        private readonly int m_Nam;

        public KyThang(int thang, int nam)
        {
            m_Thang = thang;
            m_Nam = nam;
        }

        public int Thang { get { return m_Thang; } }
        public int Nam { get { return m_Nam; } }

        /// <summary>Ky lien truoc (thang 1 -> thang 12 nam truoc).</summary>
        public KyThang ThangTruoc()
        {
            if (m_Thang <= 1) return new KyThang(12, m_Nam - 1);
            return new KyThang(m_Thang - 1, m_Nam);
        }

        public override bool Equals(object obj)
        {
            KyThang k = obj as KyThang;
            return k != null && k.m_Thang == m_Thang && k.m_Nam == m_Nam;
        }

        public override int GetHashCode()
        {
            return m_Nam * 100 + m_Thang;
        }

        public override string ToString()
        {
            return m_Thang + "/" + m_Nam;
        }
    }
}
