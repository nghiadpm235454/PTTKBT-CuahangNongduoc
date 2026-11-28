using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Security
{
    public static class MatKhauHelper
    {
        public static string TaoMuoi()
        {
            byte[] b = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) rng.GetBytes(b);
            return Convert.ToBase64String(b);
        }

        public static string Bam(string matKhau, string muoiBase64)
        {
            using (Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(
                matKhau, Convert.FromBase64String(muoiBase64), 100000, HashAlgorithmName.SHA256))
                return Convert.ToBase64String(kdf.GetBytes(32));
        }

        public static bool SoSanh(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int d = 0;
            for (int i = 0; i < a.Length; i++) d |= a[i] ^ b[i];
            return d == 0;
        }
    }
}
