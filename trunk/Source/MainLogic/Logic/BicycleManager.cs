using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Logic
{
    public class BicycleManager
    {
        #region Fields
        public List<Bicycle> BicycleList
        {
            get;
            private set;
        }

        public static int BicycleCount
        {
            get;
            private set;
        }
        #endregion

        #region Constructor
        public BicycleManager()
        {
            BicycleList = new List<Bicycle>();
        }
        #endregion

        #region Methods
        public bool RegisterBicycle(string name,Bicycle bicycle)
        {
            for (int i = 0; i < BicycleList.Count; i++)
            {
                if (BicycleList[i] == bicycle)
                {
                    return false;
                }
            }

            bicycle.OwnerName = name;
            bicycle.BicycleMgr = this;
            BicycleList.Add(bicycle);

            BicycleCount = BicycleList.Count;
            return true;
        }

        public bool RegisterBicycle(Bicycle bicycle)
        {
            for (int i = 0; i < BicycleList.Count; i++)
            {
                if (BicycleList[i] == bicycle)
                {
                    return false;
                }
            }
            bicycle.BicycleMgr = this;
            BicycleList.Add(bicycle);

            BicycleCount = BicycleList.Count;
            return true;
        }
        #endregion
    }
}
