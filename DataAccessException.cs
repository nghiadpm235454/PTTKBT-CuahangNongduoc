using System;

namespace CuahangNongduoc
{
    public class DataAccessException : Exception
    {
        public DataAccessException(string message, Exception inner) : base(message, inner) { }
    }
}
