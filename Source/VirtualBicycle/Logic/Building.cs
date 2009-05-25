using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class Building : StaticObject
    {
        #region 字段

        BuildingType buildingType;

        #endregion

        #region 构造函数

        public Building(BuildingType bldType)
        {
            this.buildingType = bldType; 
            
            this.Model = bldType.Model;
            this.LodModel = bldType.LodModel;
        }

        #endregion

        #region 属性

        public string TypeName
        {
            get { return buildingType.TypeName; }
        }

        #endregion

        #region SceneObject 序列化

        public override bool IsSerializable
        {
            get { return true; }
        }

        public override void Serialize(BinaryDataWriter data)
        {
            BuildingFactory.Serialize(this, data);
        }

        public override string TypeTag
        {
            get { return BuildingFactory.TypeId; }
        }
        #endregion
    }
}
