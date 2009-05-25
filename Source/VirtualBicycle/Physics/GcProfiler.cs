using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace VirtualBicycle.Physics
{
    public class GcProfiler
    {
        static readonly int maxGens = GC.MaxGeneration;
        static readonly StringBuilder sb = new StringBuilder("GCs:");

        public static string GenerationsText
        {
            get 
            {
                int max = GC.MaxGeneration;
                sb.Remove(4, sb.Length-4);

                for (int i = 0; i <= max; i++)
                {
                    sb.Append(" Gen");
                    sb.Append(i);
                    sb.Append(":");
                    sb.Append(GC.CollectionCount(i));
                }

                return sb.ToString();
            }
        }

    }
}
