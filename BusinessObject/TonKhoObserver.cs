using System;
using System.Collections.Generic;

namespace CuahangNongduoc.BusinessObject
{
    public class TonKhoEventArgs
    {
        private readonly MaSanPham m_Lo;
        private readonly int m_TonCu;
        private readonly int m_TonMoi;
        private readonly DateTime m_Ngay;

        public TonKhoEventArgs(MaSanPham lo, int tonCu, int tonMoi, DateTime ngay)
        {
            m_Lo = lo; m_TonCu = tonCu; m_TonMoi = tonMoi; m_Ngay = ngay;
        }
        public MaSanPham Lo { get { return m_Lo; } }
        public int TonCu { get { return m_TonCu; } }
        public int TonMoi { get { return m_TonMoi; } }
        public DateTime Ngay { get { return m_Ngay; } }
    }

    // [GoF Observer] Observer
    public interface ITonKhoObserver
    {
        void CapNhat(TonKhoEventArgs e);
    }

    // Noi gom canh bao de UI doc va hien thi sau khi luu (BusinessObject khong dinh MessageBox)
    public class HopThuCanhBao
    {
        private readonly List<string> m_DS = new List<string>();
        public void Them(string s) { if (!m_DS.Contains(s)) m_DS.Add(s); }
        public string[] LayVaXoa() { string[] kq = m_DS.ToArray(); m_DS.Clear(); return kq; }
    }

    public class CanhBaoTonThapObserver : ITonKhoObserver
    {
        private readonly int m_Nguong;
        private readonly HopThuCanhBao m_HopThu;

        public CanhBaoTonThapObserver(int nguong, HopThuCanhBao hopThu) { m_Nguong = nguong; m_HopThu = hopThu; }

        public void CapNhat(TonKhoEventArgs e)
        {
            if (e.TonMoi <= 0)
                m_HopThu.Them(string.Format("Lô {0} đã HẾT HÀNG.", e.Lo.Id));
            else if (e.TonMoi <= m_Nguong && e.TonCu > m_Nguong)
                m_HopThu.Them(string.Format("Lô {0} sắp hết hàng: còn {1}.", e.Lo.Id, e.TonMoi));
        }
    }

    public class CanhBaoHetHanObserver : ITonKhoObserver
    {
        private readonly int m_SoNgay;
        private readonly HopThuCanhBao m_HopThu;

        public CanhBaoHetHanObserver(int soNgay, HopThuCanhBao hopThu) { m_SoNgay = soNgay; m_HopThu = hopThu; }

        public void CapNhat(TonKhoEventArgs e)
        {
            if (e.TonMoi > 0 && e.Lo.HanDung.SapHetHan(e.Ngay, m_SoNgay))
                m_HopThu.Them(string.Format("Lô {0} còn {1} ngày là hết hạn ({2}), tồn {3}.",
                    e.Lo.Id, e.Lo.HanDung.SoNgayConLai(e.Ngay), ThoiGian.DinhDang(e.Lo.NgayHetHan), e.TonMoi));
        }
    }

    // Subject
    public class TonKhoSubject
    {
        public const int NGUONG_TON_THAP = 10;       // sau nay lay tu THAM_SO (Nghia)
        public const int SO_NGAY_CANH_BAO_HSD = 30;

        public static readonly HopThuCanhBao HopThu = new HopThuCanhBao();
        private static readonly TonKhoSubject s_MacDinh = TaoMacDinh();

        public static TonKhoSubject MacDinh { get { return s_MacDinh; } }

        private static TonKhoSubject TaoMacDinh()
        {
            TonKhoSubject s = new TonKhoSubject();
            s.DangKy(new CanhBaoTonThapObserver(NGUONG_TON_THAP, HopThu));
            s.DangKy(new CanhBaoHetHanObserver(SO_NGAY_CANH_BAO_HSD, HopThu));
            return s;
        }

        private readonly List<ITonKhoObserver> m_Observers = new List<ITonKhoObserver>();

        public void DangKy(ITonKhoObserver o) { if (!m_Observers.Contains(o)) m_Observers.Add(o); }
        public void HuyDangKy(ITonKhoObserver o) { m_Observers.Remove(o); }

        public void ThongBao(TonKhoEventArgs e)
        {
            foreach (ITonKhoObserver o in m_Observers.ToArray()) o.CapNhat(e);
        }

        public void ThongBao(IList<TonKhoEventArgs> ds)
        {
            foreach (TonKhoEventArgs e in ds) ThongBao(e);
        }
    }
}
