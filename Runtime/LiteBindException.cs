using System;

namespace LiteBindDI
{
    public class LiteBindException : Exception
    {
        public LiteBindException(string message) : base(message) { }
    }
}
