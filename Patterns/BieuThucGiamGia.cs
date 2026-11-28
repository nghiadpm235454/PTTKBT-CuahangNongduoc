using System;
using System.Globalization;
using System.Text;

namespace CuahangNongduoc.Patterns
{
    // [Interpreter] Quy tắc: "10% nếu > 1.000.000", "500000 nếu >= 5000000", "5%"
    public interface IBieuThuc
    {
        long TinhGiam(long tongTien);
    }

    public class MucGiamExpr : IBieuThuc
    {
        private bool m_PhanTram;
        private double m_GiaTri;
        public MucGiamExpr(bool phanTram, double giaTri) { m_PhanTram = phanTram; m_GiaTri = giaTri; }
        public long TinhGiam(long tongTien)
        {
            if (m_PhanTram) return (long)Math.Round(tongTien * m_GiaTri / 100.0);
            return (long)m_GiaTri;
        }
    }

    public class DieuKienExpr
    {
        private String m_Op;
        private long m_Nguong;
        public DieuKienExpr(String op, long nguong) { m_Op = op; m_Nguong = nguong; }
        public bool Dung(long tong)
        {
            switch (m_Op)
            {
                case ">=": return tong >= m_Nguong;
                case "<=": return tong <= m_Nguong;
                case ">": return tong > m_Nguong;
                case "<": return tong < m_Nguong;
                default: return tong == m_Nguong;
            }
        }
    }

    public class NeuExpr : IBieuThuc
    {
        private DieuKienExpr m_DieuKien;
        private IBieuThuc m_Muc;
        public NeuExpr(DieuKienExpr dk, IBieuThuc muc) { m_DieuKien = dk; m_Muc = muc; }
        public long TinhGiam(long tongTien)
        {
            return m_DieuKien.Dung(tongTien) ? m_Muc.TinhGiam(tongTien) : 0;
        }
    }

    public static class TrinhPhanTichGiamGia
    {
        public static IBieuThuc Parse(String quyTac)
        {
            if (quyTac == null || quyTac.Trim().Length == 0) return new MucGiamExpr(false, 0);

            String s = quyTac.Trim().ToLower();
            String[] phan = s.Split(new String[] { "nếu", "neu", " if " }, StringSplitOptions.None);
            IBieuThuc muc = ParseMuc(phan[0]);
            if (phan.Length == 1) return muc;
            return new NeuExpr(ParseDieuKien(phan[1]), muc);
        }

        static IBieuThuc ParseMuc(String t)
        {
            t = t.Trim();
            if (t.EndsWith("%"))
            {
                double v = 0;
                double.TryParse(t.TrimEnd('%').Trim().Replace(',', '.'),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out v);
                return new MucGiamExpr(true, v);
            }
            return new MucGiamExpr(false, ParseTien(t));
        }

        static DieuKienExpr ParseDieuKien(String t)
        {
            t = t.Trim();
            String[] ops = new String[] { ">=", "<=", ">", "<", "=" };
            foreach (String op in ops)
            {
                if (t.StartsWith(op))
                {
                    return new DieuKienExpr(op, ParseTien(t.Substring(op.Length)));
                }
            }
            return new DieuKienExpr("=", ParseTien(t));
        }

        static long ParseTien(String t)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in t)
            {
                if (Char.IsDigit(c)) sb.Append(c);
            }
            return sb.Length == 0 ? 0 : long.Parse(sb.ToString());
        }
    }
}
