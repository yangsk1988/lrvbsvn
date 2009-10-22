using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace VirtualBicycle
{
    [AttributeUsage(AttributeTargets.All ^ AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class TestOnlyAttribute : Attribute
    {
        public TestOnlyAttribute()
        {
        }
    }
}
