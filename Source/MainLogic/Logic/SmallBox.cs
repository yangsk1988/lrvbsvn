using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.CollisionModel.Shapes;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;
using PM = VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.Logic
{
    public class SmallBox : DynamicObject
    {
        public SmallBox(Device device) 
        {
            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(VirtualBicycle.IO.Paths.Models, "box1.mesh"), FileLocateRules.Default);
            Model = ModelManager.Instance.CreateInstance(device, fl);

            BoundingSphere.Radius = MathEx.Root3 * 0.5f;
        }

        public override string TypeTag
        {
            get
            {
                return SmallBoxFactory.TypeId;
            }
        }

        public override bool IsSerializable
        {
            get { return true; }
        }

        public override void Serialize(BinaryDataWriter data)
        {
            SmallBoxFactory.Serialize(this, data);
        }
        public override void BuildPhysicsModel(DynamicsWorld world)
        {
            UpdateTransform();

            BoxShape shape = new BoxShape(new Vector3(.5f));
            MotionState motionState = new DefaultMotionState(Transformation);

            PM.Vector3 inertia;
            shape.CalculateLocalInertia(1, out inertia);

            RigidBody = new RigidBody(1, motionState, shape, inertia, 0, 0, 0.5f, 0.5f);

            if (world != null)
            {
                world.AddRigidBody(RigidBody);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Model != null)
                {
                    ModelManager.Instance.DestoryInstance(Model);
                }
            }
        }
    }
}
