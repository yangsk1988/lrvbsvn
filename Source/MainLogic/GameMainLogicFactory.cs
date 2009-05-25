using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Logic.Mod;

namespace MainLogic
{
    public class GameMainLogicFactory : LogicModFactroy
    {
        public override LogicMod CreateInstance()
        {
            return new GameMainLogic();
        }
    }
}
