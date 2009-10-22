using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle
{
    public class LogManager
    {
        static LogManager singleton;

        public static LogManager Instance 
        {
            get 
            {
                return singleton;
            }
        }
    }
}
