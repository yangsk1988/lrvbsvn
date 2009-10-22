using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.IO;
using VirtualBicycle.MathLib;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class TerrainObjectFactory : InGameObjectFactory
    {
        #region 常量

        static readonly string TypeNameTag = "TypeName";
        static readonly string HeightScaleTag = "HeightScale";
        static readonly string PositionTag = "Position";
        static readonly string RotationTag = "Rotation";

        static readonly string OffsetTag = "Offset";
        static readonly string typeTag = "TerrainObject";

        #endregion

        #region 构造函数
        
        public TerrainObjectFactory(InGameObjectManager mgr)
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

            TerrainObjectType type = Manager.GetTreeType(typeName);
            if (type == null) 
            {
                type = Manager.GetTOType(typeName);
            }

            TerrainObject obj = type.CreateInstance();


            br = data.GetData(PositionTag);
            Vector3 pos;
            pos.X = br.ReadSingle();
            pos.Y = br.ReadSingle();
            pos.Z = br.ReadSingle();
            br.Close();

            obj.Position = pos;
            obj.Orientation = Quaternion.RotationAxis(Vector3.UnitY, data.GetDataSingle(RotationTag));
            obj.HeightScale = data.GetDataSingle(HeightScaleTag);
            obj.UpdateTransform();

            br = data.GetData(OffsetTag);
            obj.OffsetX = br.ReadSingle();
            obj.OffsetY = br.ReadSingle();
            obj.OffsetZ = br.ReadSingle();
            br.Close();


            return obj;
        }

        public static void Serialize(TerrainObject obj, BinaryDataWriter data)
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

            data.AddEntry(RotationTag, obj.Orientation.Angle);
            data.AddEntry(HeightScaleTag, obj.HeightScale);


            bw = data.AddEntry(OffsetTag);
            bw.Write(obj.OffsetX);
            bw.Write(obj.OffsetY);
            bw.Write(obj.OffsetZ);
            bw.Close();
        }

        #endregion
    }
}
