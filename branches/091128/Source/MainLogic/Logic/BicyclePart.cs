using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.CollisionModel.Shapes;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;
using PM = VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.Logic
{
    [Obsolete()]
    public class BicyclePart : DynamicObject
    {
        Bicycle bike;

        public BicyclePart(Bicycle bike, Device device)
            : base(false)
        {
            this.bike = bike;
            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(VirtualBicycle.IO.Paths.Models, "sphere0.25.mesh"), FileLocateRules.Default);
            ModelL0 = ModelManager.Instance.CreateInstance(device, fl);
        }

        public override bool IsSerializable
        {
            get { return false; }
        }

        public override bool HasPhysicsModel
        {
            get { return false; }
        }

        internal void BuildPhysicsModelImpl()
        {
            SphereShape shape = new SphereShape(0.25f);
            MotionState motionState = new DefaultMotionState(Matrix.Translation(position));

            PM.Vector3 inertia;
            shape.CalculateLocalInertia(1, out inertia);

            RigidBody = new RigidBody(1, motionState, shape, inertia, 0, 0, 0.5f, 0.1f);
        }

        //public override void BuildPhysicsModel()
        //{
        //    if (!isPhyBuilt) 
        //    {               
        //        bike.BuildPhysicsModel();
        //        isPhyBuilt = true;
        //    }
        //}
    }
}
