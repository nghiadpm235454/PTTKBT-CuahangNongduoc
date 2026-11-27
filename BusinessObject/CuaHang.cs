using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public enum PhuongPhapXuatKho { FIFO, ChiDinh }
    public enum PhuongPhapGiaXuat { BQGQ, FIFO }

    public class CuaHang
    {
        private String m_TenCuaHang;

        public String TenCuaHang
        {
            get { return m_TenCuaHang; }
            set { m_TenCuaHang = value; }
        }

        private String m_DiaChi;

        public String DiaChi
        {
            get { return m_DiaChi; }
            set { m_DiaChi = value; }
        }

        private String m_DienThoai;

        public String DienThoai
        {
            get { return m_DienThoai; }
            set { m_DienThoai = value; }
        }

        private PhuongPhapXuatKho m_PhuongPhapXuatKho = PhuongPhapXuatKho.FIFO;

        public PhuongPhapXuatKho PhuongPhapXuatKho
        {
            get { return m_PhuongPhapXuatKho; }
            set { m_PhuongPhapXuatKho = value; }
        }

        private PhuongPhapGiaXuat m_PhuongPhapGiaXuat = PhuongPhapGiaXuat.BQGQ;

        public PhuongPhapGiaXuat PhuongPhapGiaXuat
        {
            get { return m_PhuongPhapGiaXuat; }
            set { m_PhuongPhapGiaXuat = value; }
        }

    }
}
