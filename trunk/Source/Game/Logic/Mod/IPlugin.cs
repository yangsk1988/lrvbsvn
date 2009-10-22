using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;

namespace VirtualBicycle.Logic.Mod
{
    public interface IPlugin
    {
        /// <summary>
        ///  获取所有场景物体的工厂
        /// </summary>
        /// <returns></returns>
        InGameObjectFactory[] GetObjectTypes(Device device, InGameObjectManager mgr);        
    }
}
