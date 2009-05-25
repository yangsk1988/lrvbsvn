using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Scene
{
    public interface IObjectFilter
    {
        bool Check(SceneObject obj);
    }
}
