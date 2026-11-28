using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc
{
    public class Num2Str
    {
        public Num2Str()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        // Hàm này có thực hiện cắt các số 0
        // ví dụ 005 sẽ đọc là không trăm linh năm
        private string Group32StrX(string num)
        {
            string[] No = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string kq, tram, chuc, donvi;
            // Trăm
            if (num.Substring(0, 1) == "0")
            { tram = ""; }
            else
            { tram = No[Convert.ToByte(num.Substring(0, 1))] + " trăm "; }
            // Chục
            switch (num.Substring(1, 1))
            {
                case "0":
                    if (num.Substring(2, 1) != "0" && num.Substring(0, 1) != "0")
                    {
                        chuc = "linh ";
                    }
                    else { chuc = ""; }
                    ; break;
                case "1": chuc = "mười "; break;
                default:
                    chuc = No[Convert.ToByte(num.Substring(1, 1))] + " mươi "; break;
            }
            // Đơn vị
            switch (num.Substring(2, 1))
            {
                case "0": donvi = ""; break;
                case "1":
                    if ((num.Substring(1, 1) == "0") || (num.Substring(1, 1) == "1"))
                    {
                        donvi = "một";
                    }
                    else
                    {
                        donvi = "mốt";
                    }
                    ; break;
                case "5":
                    if (num.Substring(1, 1) != "0")
                    {
                        donvi = "lăm";
                    }
                    else
                    {
                        donvi = "năm";
                    }
                    ; break;
                default:
                    donvi = No[Convert.ToByte(num.Substring(2, 1))]; break;
            }
            kq = tram + chuc + donvi;
            return kq;
        }

        private string IntNum2Str(string num)
        {
            string[] Cap = { "", " nghìn ", " triệu ", " tỷ ", " nghìn tỷ ", " triệu tỷ ", " tỷ tỷ ", " nghìn tỷ tỷ " };
            string kq = "", str = num, g3, kqtg;
            int caps = 0;
            while (str.Length > 3)
            {
                g3 = str.Substring(str.Length - 3, 3);
                str = str.Substring(0, str.Length - 3);
                if (g3 != "000")
                { kqtg = Group32StrX(g3) + Cap[Convert.ToByte(caps)]; }
                else { kqtg = ""; }
                kq = kqtg + kq;
                caps++;
            }
            //Chuẩn bị trước khi sử dụng hàm Group32Str1
            while (str.Length < 3)
            { str = "0" + str; }

            if ((str == "000") && (num.Length <= 3))
            { kqtg = "không"; }
            else
            { kqtg = Group32StrX(str) + Cap[Convert.ToByte(caps)]; }
            kq = kqtg + kq;
            return kq;
        }

        private string FracNum2Str(string num)
        {
            string[] No = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string kq = "";
            for (int i = 0; i < num.Length; i++)
            {
                kq += No[Convert.ToByte(num.Substring(i, 1))] + " ";
            }
            return kq;
        }

        private string No2Str(string num)
        {
            string intstr, fracstr, am;
            // Biến tạm giữ số đã bỏ dấu "-", không gán lại tham số num
            string soDuong = num;
            // Xử lý khi là số âm
            if (num.Substring(0, 1) == "-")
            {
                am = "âm ";
                soDuong = num.Replace("-", "");
            }
            else { am = ""; }

            string[] str = soDuong.Split('.');
            // Số quá lớn nhiều hơn 27 chữ số
            if (str[0].Length > 28)
                throw new Exception("Số lớn quá không hiển thị được! ");

            string s;
            // Xử lý phần số, nếu là có số thập phân hoặc không
            if (str.Length == 2)
            {
                intstr = IntNum2Str(str[0]);
                if (str[1].Length <= 2)
                {
                    if (str[1].Substring(0, 1) == "0")
                    {
                        fracstr = FracNum2Str(str[1]);
                    }
                    else
                    {
                        fracstr = IntNum2Str(str[1]);
                    }
                }
                else
                {
                    fracstr = FracNum2Str(str[1]);
                }
                s = (intstr + " phảy " + fracstr);
            }
            else
            {
                intstr = IntNum2Str(str[0]);
                s = intstr;
            }
            s = am + s;
            string chuhoa = s.Substring(0, 1).ToUpper();
            s = s.Substring(1, s.Length - 1);
            return (chuhoa + s);
        }

        public string NumNormalize(string num)
        {
            char ThousandSpace = ',';
            char PointScape = '.';
            string[] s = num.Split('.');
            string g3, str = "";
            while (s[0].Length > 3)
            {
                g3 = s[0].Substring(s[0].Length - 3, 3);
                s[0] = s[0].Substring(0, s[0].Length - 3);
                str = ThousandSpace + g3 + str;
            }
            str = s[0] + str;
            if (s.Length == 2)
            {
                str = (str + PointScape + s[1]);
            }
            return str.Replace("-,", "-");
        }

        public string NumberToString(string no)
        {
            if (string.IsNullOrWhiteSpace(no)) return "không";

            // Biến tạm thay cho việc gán lại tham số no
            string so = no.Trim().Replace(",", ".");
            // Xử lý khi nó là chữ chứ không phải là số
            if (so == "0" || so == "") return "không";
            double val;
            try
            {
                val = Convert.ToDouble(so, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new Exception("Đây không phải là số");
            }
            if (val == 0) return "không";
            // tiêu diệt các số không đứng đầu (giữ lại ít nhất 1 ký tự nếu là "0...")
            while (so.Length > 1 && so.Substring(0, 1) == "0" && so.Substring(1, 1) != ".")
            {
                so = so.Substring(1, so.Length - 1);
            }

            if (so.IndexOf(".") != -1)
            {
                while (so.Length > 1 && so.Substring(so.Length - 1, 1) == "0")
                {
                    so = so.Substring(0, so.Length - 1);
                }
                if (so.EndsWith("."))
                {
                    so = so.Substring(0, so.Length - 1);
                }
            }

            return No2Str(so);
        }
    }
}