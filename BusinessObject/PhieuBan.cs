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

        private IList<ChiTietPhieuBan> m_ChiTiet;

        public IList<ChiTietPhieuBan> ChiTiet
        {
            get { return m_ChiTiet; }
            set { m_ChiTiet = value; }
        }


        // ===== CN-06: thông tin hóa đơn mở rộng =====
        private long m_TienHang;
        public long TienHang { get { return m_TienHang; } set { m_TienHang = value; } }

        private long m_TongDichVu;
        public long TongDichVu { get { return m_TongDichVu; } set { m_TongDichVu = value; } }

        private bool m_GiaoHang;
        public bool GiaoHang { get { return m_GiaoHang; } set { m_GiaoHang = value; } }

        private double m_KhoangCachKm;
        public double KhoangCachKm { get { return m_KhoangCachKm; } set { m_KhoangCachKm = value; } }

        private long m_PhiVanChuyen;
        public long PhiVanChuyen { get { return m_PhiVanChuyen; } set { m_PhiVanChuyen = value; } }

        private long m_ChietKhau;
        public long ChietKhau { get { return m_ChietKhau; } set { m_ChietKhau = value; } }

        private long m_TienKhuyenMai;
        public long TienKhuyenMai { get { return m_TienKhuyenMai; } set { m_TienKhuyenMai = value; } }

        private long m_TienGiamGia;      // số tiền giảm đã tính ra
        public long TienGiamGia { get { return m_TienGiamGia; } set { m_TienGiamGia = value; } }

        private int m_IdNguoiDung;       // 0 = chưa có
        public int IdNguoiDung { get { return m_IdNguoiDung; } set { m_IdNguoiDung = value; } }

        private ThongTinGiamGia m_GiamGia = new ThongTinGiamGia();
        public ThongTinGiamGia GiamGia { get { return m_GiamGia; } set { m_GiamGia = value; } }

        private IList<DichVuPhieuBan> m_DichVu = new List<DichVuPhieuBan>();
        public IList<DichVuPhieuBan> DichVu { get { return m_DichVu; } set { m_DichVu = value; } }
	
    }
}
