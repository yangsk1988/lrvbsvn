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
                if (singleton == null)
                    singleton = new LogManager();
                return singleton;
            }
        }

    }
}
