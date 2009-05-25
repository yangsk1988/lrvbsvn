using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Config;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;

namespace VirtualBicycle.Logic
{
    public class BuildingType : EntityType
    {
        #region 属性

        public float Radius
        {
            get;
            protected set;
        }

        #endregion

        #region IConfigurable 成员

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            Radius = sect.GetSingle("Radius", 5f);

        }

        #endregion

        #region 方法

        public Building CreateInstance()
        {
            Building result = new Building(this);

            result.BoundingSphere.Radius = Radius;

            return result;
        }

        #endregion
    }
}
