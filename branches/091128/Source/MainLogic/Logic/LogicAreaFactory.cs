using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;
using VirtualBicycle.IO;
using SlimDX;

namespace VirtualBicycle.Logic
{
    public class LogicalAreaFactory : InGameObjectFactory
    {
        #region Fields
        static readonly string TypeNameTag = "TypeName";
        static readonly string PositionTag = "Position";
        static readonly string RadiusTag = "Radius";
        #endregion

        public LogicalAreaFactory(InGameObjectManager mgr)
            : base(mgr)
        {
        }

        public static string TypeId 
        {
            get { return "LogicArea"; }
        }
        public override string TypeTag
        {
            get { return TypeId; }
        }

        public override SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData)
        {
            ContentBinaryReader br;

            Vector3 position;
            br = data.GetData(PositionTag);
            position.X = br.ReadSingle();
            position.Y = br.ReadSingle();
            position.Z = br.ReadSingle();
            br.Close();

            float radius;
            br = data.GetData(RadiusTag);
            radius = br.ReadSingle();
            br.Close();

            string name;
            br = data.GetData(TypeNameTag);
            name = br.ReadStringUnicode();
            br.Close();

            LogicalArea obj = new LogicalArea(radius, position,name);
            return obj;
        }

        public static void Serialize(LogicalArea obj, BinaryDataWriter data)
        {
            ContentBinaryWriter bw = data.AddEntry(PositionTag);
            Vector3 pos = obj.Position;
            bw.Write(pos.X);
            bw.Write(pos.Y);
            bw.Write(pos.Z);
            bw.Close();

            data.AddEntry(RadiusTag, obj.Radius);
            bw.Close();

            bw = data.AddEntry(TypeNameTag);
            bw.WriteStringUnicode(obj.TypeName);
            bw.Close();
        }
    }
}
