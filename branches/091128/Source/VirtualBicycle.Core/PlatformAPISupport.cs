using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle
{
    public struct PlatformAPISupport
    {
        int mark;
        string name;

        public PlatformAPISupport(int mark, string name)
        {
            this.mark = mark;
            this.name = name;
        }

        public string PlatformName
        {
            get { return name; }
        }
        public int Mark
        {
            get { return mark; }
        }
    }
}
