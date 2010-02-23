using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class BuildingFactory : InGameObjectFactory
    {
        #region 常量

        static readonly string TypeNameTag = "TypeName";
        static readonly string OrientationTag = "Orientation";
        static readonly string PositionTag = "Position";

        static readonly string typeTag = "Building";
        static readonly string OffsetTag = "Offset";

        #endregion

        #region 构造函数

        public BuildingFactory(InGameObjectManager mgr)
            : base(mgr)
        {
        }

        #endregion

        #region 属性

        public static string TypeId
        {
            get { return typeTag; }
        }

        public override string TypeTag
        {
            get { return typeTag; }
        }

        #endregion

        #region 方法

        public override SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData)
        {
            ContentBinaryReader br = data.GetData(TypeNameTag);
            string typeName = br.ReadStringUnicode();
            br.Close();


            BuildingType type = Manager.GetBuildingType(typeName);
            Building bld = type.CreateInstance();

            br = data.GetData(PositionTag);
            Vector3 pos;
            pos.X = br.ReadSingle();
            pos.Y = br.ReadSingle();
            pos.Z = br.ReadSingle();
            br.Close();

            br = data.GetData(OrientationTag);
            Quaternion quat;
            quat.W = br.ReadSingle();
            quat.X = br.ReadSingle();
            quat.Y = br.ReadSingle();
            quat.Z = br.ReadSingle();
            br.Close();

            bld.Position = pos;
            bld.Orientation = quat;


            br = data.GetData(OffsetTag);
            bld.OffsetX = br.ReadSingle();
            bld.OffsetY = br.ReadSingle();
            bld.OffsetZ = br.ReadSingle();
            br.Close();


            return bld;
        }

        public static void Serialize(Building obj, BinaryDataWriter data)
        {
            ContentBinaryWriter bw = data.AddEntry(TypeNameTag);
            bw.WriteStringUnicode(obj.TypeName);
            bw.Close();


            bw = data.AddEntry(PositionTag);
            Vector3 pos = obj.Position;
            bw.Write(pos.X);
            bw.Write(pos.Y);
            bw.Write(pos.Z);
            bw.Close();


            bw = data.AddEntry(OrientationTag);
            Quaternion quat = obj.Orientation;
            bw.Write(quat.W);
            bw.Write(quat.X);
            bw.Write(quat.Y);
            bw.Write(quat.Z);
            bw.Close();


            bw = data.AddEntry(OffsetTag);
            bw.Write(obj.OffsetX);
            bw.Write(obj.OffsetY);
            bw.Write(obj.OffsetZ);
            bw.Close();
        }

        #endregion
    }
}
