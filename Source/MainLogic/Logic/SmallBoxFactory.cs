using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class SmallBoxFactory : InGameObjectFactory
    { 
        static readonly string typeTag = "SmallBox";
        
        static readonly string OrientationTag = "Orientation";
        static readonly string PositionTag = "Position";
        static readonly string OffsetTag = "Offset";


        Device device;

        public SmallBoxFactory(InGameObjectManager mgr, Device dev)
            : base(mgr)
        {
            device = dev;
        }

        public static string TypeId 
        {
            get { return typeTag; }
        }
        public override string TypeTag
        {
            get { return typeTag; }
        }

        public override SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData)
        {
            SmallBox obj = new SmallBox(device);

            ContentBinaryReader br = data.GetData(PositionTag);
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

            obj.Position = pos;
            obj.Orientation = quat;

            br = data.GetData(OffsetTag);
            obj.OffsetX = br.ReadSingle();
            obj.OffsetY = br.ReadSingle();
            obj.OffsetZ = br.ReadSingle();
            br.Close();


            return obj;
        }

        public static void Serialize(SmallBox obj, BinaryDataWriter data)
        {
            ContentBinaryWriter bw = data.AddEntry(PositionTag);

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
    }
}
