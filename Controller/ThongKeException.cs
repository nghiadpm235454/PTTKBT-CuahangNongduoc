using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.Controller
{
    /// <summary>Lỗi nghiệp vụ của thống kê (ví dụ: từ ngày > đến ngày).</summary>
    public class ThongKeException : Exception
    {
        public ThongKeException(string message) : base(message) { }
        public ThongKeException(string message, Exception inner) : base(message, inner) { }
    }
}
