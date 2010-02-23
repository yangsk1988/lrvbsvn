using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle
{
    [Serializable]
    public class InvalidFormatException : Exception
    {
        public InvalidFormatException() { }
        public InvalidFormatException(string message) : base(message) { }
        public InvalidFormatException(string message, Exception inner) : base(message, inner) { }
    }

    [Serializable]
    public class DataFormatException : Exception
    {
        public DataFormatException() { }
        public DataFormatException(string message) : base(message) { }
        public DataFormatException(string message, Exception inner) : base(message, inner) { }
    }
}
