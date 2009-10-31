using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Graphics
{
    public struct TapeHelper
    {
        public Vector3 Source;
        public Vector3 Target;

        public TapeHelper(Vector3 src, Vector3 target)
        {
            this.Source = src;
            this.Target = target;
        }
    }
}
