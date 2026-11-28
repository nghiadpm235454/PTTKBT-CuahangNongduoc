using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    /// <summary>Hàm tải một danh mục từ CSDL (C# 2.0 không có Func).</summary>
    public delegate DataTable TaiDanhMuc();

    /// <summary>
    /// P09 - Design pattern FLYWEIGHT: cache danh mục dùng chung (đơn vị tính, lý do chi...).
    /// Mỗi danh mục chỉ tải từ CSDL 1 lần, mọi form dùng chung cùng một đối tượng.
    /// Gọi XoaCache() sau khi người dùng thêm/sửa danh mục.
    ///
    /// Cách dùng:
    ///     DataTable dvt = DanhMucCache.DonViTinh();
    ///     DataTable ld  = DanhMucCache.LyDoChi();
    /// </summary>
    public static class DanhMucCache
    {
        private static readonly Dictionary<string, DataTable> m_Cache = new Dictionary<string, DataTable>();
        private static readonly object m_Khoa = new object();

        public static DataTable Lay(string ten, TaiDanhMuc taiTuDb)
        {
            lock (m_Khoa)
            {
                DataTable tbl;
                if (!m_Cache.TryGetValue(ten, out tbl))
                {
                    tbl = taiTuDb();            // chỉ chạm CSDL lần đầu
                    m_Cache.Add(ten, tbl);
                }
                return tbl;
            }
        }

        public static DataTable DonViTinh()
        {
            return Lay("DON_VI_TINH", delegate() { return new DonViTinhFactory().DanhsachDVT(); });
        }

        public static DataTable LyDoChi()
        {
            return Lay("LY_DO_CHI", delegate() { return new LyDoChiFactory().DanhsachLyDo(); });
        }

        public static void XoaCache()
        {
            lock (m_Khoa) { m_Cache.Clear(); }
        }

        public static void XoaCache(string ten)
        {
            lock (m_Khoa) { m_Cache.Remove(ten); }
        }
    }
}
