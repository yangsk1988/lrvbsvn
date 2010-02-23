using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Graphics
{
    [Flags()]
    public enum WeatherType
    {
        None = 0,
        Cloudy = 1 << 0,
        Rainy = 1 << 1,
        Windy = 1 << 2
    }
}
