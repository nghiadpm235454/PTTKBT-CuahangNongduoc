using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class PhieuBan
    {
        private String m_Id;

        public String Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        private KhachHang m_KH;

        public KhachHang KhachHang
        {
            get { return m_KH; }
            set { m_KH = value; }
        }

        private DateTime m_NgayBan;

        public DateTime NgayBan
        {
            get { return m_NgayBan; }
            set { m_NgayBan = value; }
        }
        private long m_TongTien;

        public long TongTien
        {
            get { return m_TongTien; }
            set { m_TongTien = value; }
        }
        private long m_DaTra;

        public long DaTra
        {
            get { return m_DaTra; }
            set { m_DaTra = value; }
        }
        private long m_ConNo;

        public long ConNo
        {
            get { return m_ConNo; }
            set { m_ConNo = value; }
        }

        // RF-02 #24 - Truoc: public IList<ChiTietPhieuBan> ChiTiet { get; set; }
        private readonly List<ChiTietPhieuBan> m_ChiTiet = new List<ChiTietPhieuBan>();

        public IEnumerable<ChiTietPhieuBan> ChiTiet
        {
            get { return m_ChiTiet.AsReadOnly(); }
        }

        public void ThemChiTiet(ChiTietPhieuBan ct)
        {
            m_ChiTiet.Add(ct);
        }

        public void DatChiTiet(IEnumerable<ChiTietPhieuBan> ds)
        {
            m_ChiTiet.Clear();
            m_ChiTiet.AddRange(ds);
        }

	
    }
}
