using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.CollisionModel.Shapes;
using VirtualBicycle.Graphics;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;
using PM = VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.Logic.Traffic
{
    public class RoadSegment : StaticObject
    {

        public Road Track
        {
            get;
            private set;
        }

        public RoadSegment(Road parent, Model model)
            : base(false)
        {
            base.Model = model;
            this.Track = parent;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
        public override bool EditorMovable
        {
            get { return false; }
        }

        public override unsafe void BuildPhysicsModel(DynamicsWorld world)
        {
            base.BuildPhysicsModel(world);

            if (RigidBody != null) 
            {
                RigidBody.Restitution = 0;
            }
        }


    }
}
