using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.Scene;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics.Dynamics;

namespace VirtualBicycle.Logic.Goal
{
    public class BicycleBrake:BaseGoal
    {
        #region Fields
        private Bicycle bicycle;
        #endregion

        #region Constructor
        public BicycleBrake(DynamicObject obj, GoalManager mgr,BaseGoal fatherGoal) :
            base(obj, mgr,fatherGoal)
        {
            bicycle = (Bicycle)obj;
        }
        #endregion

        #region Methods
        public override void Activate()
        {
            isActived = true;
        }

        public override void Process(float dt)
        {
            float frontVLen = bicycle.FrontVelocity.Length();
            float breakLen = Bicycle.maxBicycleBrake * dt;
            if (frontVLen > 0)
            {
                if (breakLen > frontVLen)
                {
                    breakLen = frontVLen;
                }
                frontVLen -= breakLen;
            }
            RigidBody body = bicycle.RigidBody;
            body.LinearVelocity = (Vector3)body.LinearVelocity - bicycle.FrontVelocity + bicycle.Front * frontVLen;
        }

        public override void Terminate()
        {
        }
        #endregion
    }
}
