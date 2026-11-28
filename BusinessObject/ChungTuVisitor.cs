using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    /// <summary>
    /// P09 - Design pattern VISITOR: tính tổng thống kê trên nhiều loại chứng từ
    /// mà không phải sửa logic bên trong từng loại phiếu.
    /// Mỗi phiếu (PhieuBan, PhieuNhap, PhieuChi, PhieuThanhToan) cài IChungTu.Accept(visitor).
    /// </summary>
    public interface IChungTu
    {
        void Accept(IChungTuVisitor visitor);
    }

    public interface IChungTuVisitor
    {
        void Visit(PhieuBan phieu);
        void Visit(PhieuThanhToan phieu);
        void Visit(PhieuNhap phieu);
        void Visit(PhieuChi phieu);
    }

    /// <summary>Cộng tổng THU (bán + thanh toán) và tổng CHI (nhập + chi).</summary>
    public class TongTienVisitor : IChungTuVisitor
    {
        private long m_TongThu;
        private long m_TongChi;
        private int m_SoChungTu;

        public long TongThu { get { return m_TongThu; } }
        public long TongChi { get { return m_TongChi; } }
        public long ChenhLech { get { return m_TongThu - m_TongChi; } }
        public int SoChungTu { get { return m_SoChungTu; } }

        public void Visit(PhieuBan phieu) { m_TongThu += phieu.TongTien; m_SoChungTu++; }
        public void Visit(PhieuThanhToan phieu) { m_TongThu += phieu.TongTien; m_SoChungTu++; }
        public void Visit(PhieuNhap phieu) { m_TongChi += phieu.TongTien; m_SoChungTu++; }
        public void Visit(PhieuChi phieu) { m_TongChi += phieu.TongTien; m_SoChungTu++; }
    }

    /// <summary>Dựng đối tượng chứng từ từ một dòng thống kê (Id, ngày, tổng tiền).</summary>
    public static class ChungTuFactory
    {
        public static IChungTu Tao(string loai, string id, DateTime ngay, long tongTien)
        {
            switch (loai)
            {
                case "PHIEU_BAN":
                    PhieuBan pb = new PhieuBan();
                    pb.Id = id; pb.NgayBan = ngay; pb.TongTien = tongTien;
                    return pb;
                case "PHIEU_THANH_TOAN":
                    PhieuThanhToan tt = new PhieuThanhToan(id);
                    tt.NgayThanhToan = ngay; tt.TongTien = tongTien;
                    return tt;
                case "PHIEU_NHAP":
                    PhieuNhap pn = new PhieuNhap(id);
                    pn.NgayNhap = ngay; pn.TongTien = tongTien;
                    return pn;
                case "PHIEU_CHI":
                    PhieuChi pc = new PhieuChi(id);
                    pc.NgayChi = ngay; pc.TongTien = tongTien;
                    return pc;
                default:
                    throw new ArgumentException("Loại chứng từ không hợp lệ: " + loai);
            }
        }
    }
}
