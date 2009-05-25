using System;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class VehicleFactory : InGameObjectFactory
    {
        static readonly string typeTag = "Vehicle";

        public VehicleFactory(InGameObjectManager mgr)
            : base(mgr)
        {
        }

        public override string TypeTag
        {
            get { return typeTag; }
        }

        public override SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData)
        {
            throw new NotImplementedException();
        }

        public static void Serialize(Vehicle obj, BinaryDataWriter data)
        {

        }
    }
}
