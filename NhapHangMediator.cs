using System;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public class NhapHangMediator
    {
        NumericUpDown numGiaNhap, numSoLuong, numThanhTien, numTongTien, numDaTra, numConNo;

        public NhapHangMediator(NumericUpDown giaNhap, NumericUpDown soLuong, NumericUpDown thanhTien,
                                NumericUpDown tongTien, NumericUpDown daTra, NumericUpDown conNo)
        {
            numGiaNhap = giaNhap; numSoLuong = soLuong; numThanhTien = thanhTien;
            numTongTien = tongTien; numDaTra = daTra; numConNo = conNo;

            numGiaNhap.ValueChanged += delegate { TinhThanhTien(); };
            numSoLuong.ValueChanged += delegate { TinhThanhTien(); };
            numDaTra.ValueChanged += delegate { TinhConNo(); };
            numTongTien.ValueChanged += delegate { TinhConNo(); };
        }

        void TinhThanhTien() { numThanhTien.Value = numGiaNhap.Value * numSoLuong.Value; }
        void TinhConNo() { numConNo.Value = numTongTien.Value - numDaTra.Value; }
    }
}