using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Traffic
{
    public class RoadFactory : InGameObjectFactory
    {
        #region 常量

        static readonly string typeTag = "Path";

        #endregion

        #region 字段

        Device device;
        TrafficNet trafficNet;

        #endregion

        #region 构造函数

        public RoadFactory(Device device, InGameObjectManager mgr, TrafficNet trafficNet)
            : base(mgr)
        {
            this.device = device;
            this.trafficNet = trafficNet;
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
            Road track = new Road(device, trafficNet);
            track.Deserialize(data);
            return track;
        }

        #endregion
    }
}
