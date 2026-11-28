using System;

namespace CuahangNongduoc.Controller
{
    // FIX-04 (R14): Controller trả kết quả thay vì tự bật MessageBox
    public class SaveResult
    {
        private bool m_Success;
        public bool Success { get { return m_Success; } }

        private String m_Message;
        public String Message { get { return m_Message; } }

        private SaveResult(bool ok, String msg) { m_Success = ok; m_Message = msg; }

        public static SaveResult Ok(String msg) { return new SaveResult(true, msg); }
        public static SaveResult Fail(String msg) { return new SaveResult(false, msg); }
    }
}
