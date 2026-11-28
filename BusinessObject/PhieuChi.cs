using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class PhieuChi : IChungTu
    {
        // P09 - Visitor: tính tổng thống kê trên nhiều loại chứng từ
        public void Accept(IChungTuVisitor visitor)
        {
            visitor.Visit(this);
        }

        public PhieuChi() { }
        public PhieuChi(String id)
        {
            m_Id = id;
        }
        private String m_Id;

        public String Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        private DateTime m_NgayChi;

        public DateTime NgayChi
        {
            get { return m_NgayChi; }
            set { m_NgayChi = value; }
        }
        private long m_TongTien;

        public long TongTien
        {
            get { return m_TongTien; }
            set { m_TongTien = value; }
        }

        private LyDoChi m_LyDo;

        public LyDoChi LyDoChi
        {
            get { return m_LyDo; }
            set { m_LyDo = value; }
        }

        private String m_GhiChu;

        public String GhiChu
        {
            get { return m_GhiChu; }
            set { m_GhiChu = value; }
        }
	

	
    }
}
