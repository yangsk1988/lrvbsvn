using System;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class Vehicle : DynamicObject
    {
      
        public override bool IsSerializable
        {
            get { return true; }
        }

        public override void Serialize(BinaryDataWriter data)
        {
            throw new NotImplementedException();
        }
    }
}
