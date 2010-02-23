using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MainLogic;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class BicycleFactory : InGameObjectFactory
    {
        static readonly string typeTag = "Bicycle";
        static readonly string OrientationTag = "Orientation";
        static readonly string PositionTag = "Position";
        static readonly string OffsetTag = "Offset";

        Device device;

        public BicycleFactory(InGameObjectManager mgr, Device device)
            : base(mgr)
        {
            this.device = device;
        }

        public GameMainLogic Logic
        {
            get;
            set;
        }

        public override string TypeTag
        {
            get { return typeTag; }
        }

        public override SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData)
        {
            Bicycle bike = new Bicycle(device);

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

            bike.Position = pos;
            bike.Orientation = quat;

            br = data.GetData(OffsetTag);
            bike.OffsetX = br.ReadSingle();
            bike.OffsetY = br.ReadSingle();
            bike.OffsetZ = br.ReadSingle();
            br.Close();


            //Logic.TestBike = bike;
            return bike;
        }

        public static void Serialize(Bicycle obj, BinaryDataWriter data)
        {
            ContentBinaryWriter bw = data.AddEntry(PositionTag);

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
    }
}
