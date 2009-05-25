using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle
{
    public struct Pair<A, B>
    {

        public Pair(A first, B second)
        {
            this.first = first;
            this.second = second;
        }
        /// <summary></summary>
        public A first;
        /// <summary></summary>
        public B second;
    }
}
