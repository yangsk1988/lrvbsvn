using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Traffic
{
    public class JunctionFactory : InGameObjectFactory
    {
        static readonly string typeId = "RoadCrossing";

        static readonly string TypeNameTag = "TypeName";

        TrafficNet trafficNet;

        #region 属性

        public override string TypeTag
        {
            get { return typeId; }
        }
        public static string TypeId
        {
            get { return typeId; }
        }

        #endregion

        #region 方法
        public JunctionFactory(InGameObjectManager manager, TrafficNet trafficNet)
            : base(manager)
        {
            this.trafficNet = trafficNet;
        }

        public override SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData)
        {
            ContentBinaryReader br = data.GetData(TypeNameTag);
            string typeName = br.ReadStringUnicode();
            br.Close();

            JunctionType type = Manager.GetCrossingType(typeName);
            Junction cross = type.CreateInstance(trafficNet);

            cross.LoadData(data);

            return cross;
        }

        public static void Serialize(Junction crossing, BinaryDataWriter data)
        {
            ContentBinaryWriter bw = data.AddEntry(TypeNameTag);
            bw.WriteStringUnicode(crossing.TypeName);
            bw.Close();

            crossing.SaveData(data);
        }
        #endregion

    }
}
