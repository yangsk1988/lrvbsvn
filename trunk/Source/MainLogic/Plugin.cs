using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Logic;
using VirtualBicycle.Logic.Mod;

namespace MainLogic
{
    class Plugin : IPlugin
    {

        public static InGameObjectFactory[] GetAllObjectTypes(Device device, InGameObjectManager mgr, GameMainLogic logic)
        {
            BicycleFactory fac1 = new BicycleFactory(mgr, device);
            fac1.Logic = logic;

            LogicalAreaFactory fac2 = new LogicalAreaFactory(mgr);
            SmallBoxFactory fac3 = new SmallBoxFactory(mgr, device);

            return new InGameObjectFactory[] { fac1, fac2, fac3 };
        }

        #region IPlugin 成员

        public InGameObjectFactory[] GetObjectTypes(Device device, InGameObjectManager mgr)
        {
            return GetAllObjectTypes(device, mgr, null);
        }

        #endregion
    }
}
