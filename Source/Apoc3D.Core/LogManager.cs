using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D
{
    public class LogManager
    {
        static volatile LogManager singleton;
        static volatile object syncHelper = new object();

        public static LogManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncHelper)
                    {
                        if (singleton == null)
                        {
                            singleton = new LogManager();
                        }
                    }
                }
                return singleton;
            }
        }

    }
}
