using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Logic;

namespace VirtualBicycle.ObjectMgr
{
    /// <summary>
    /// 保存场景中需要物体的引用
    /// </summary>
    public class ObjectManager
    {
        public List<Bicycle> bicycles;

        public ObjectManager()
        {
            bicycles = new List<Bicycle>();
        }

        public void AddBicycle(Bicycle bicycle)
        {
            bicycles.Add(bicycle);
        }
    }
}
