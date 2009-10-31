using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Goal
{
    public class BicycleToSinglePoint : BaseGoal
    {
        #region Fields
        //自行车到达终点的判断距离
        const float maxDisFinish = 5f;

        private Bicycle bicycle;

        /// <summary>
        /// 得到目标区域
        /// </summary>
        public Vector3 DesiredPos
        {
            get { return desiredPos; }
        }
        private Vector3 desiredPos;

        Vector3 initialPosition;

        //初始化的时候,车身指向目标的向量
        private Vector2 startingVector;
        #endregion

        #region Constructor
        public BicycleToSinglePoint(DynamicObject obj, GoalManager mgr, Vector3 point, BaseGoal fatherGoal) :
            base(obj, mgr, fatherGoal)
        {
            bicycle = (Bicycle)obj;
            desiredPos = point;
            initialPosition = bicycle.Position;
        }

        public BicycleToSinglePoint(DynamicObject obj, GoalManager mgr, BaseGoal fatherGoal) :
            base(obj, mgr, fatherGoal)
        {
            bicycle = (Bicycle)obj;
            initialPosition = bicycle.Position;
        }
        #endregion

        #region Methods
        public override void Activate()
        {
            Vector3 pos = bicycle.Position;// + bicycle.Front * Bicycle.Wheelbase / 2;
            isActived = true;
            startingVector = new Vector2(desiredPos.X, desiredPos.Z) - new Vector2(pos.X,pos.Z);
            startingVector.Normalize();
            //add code
        }

        private float Vector2Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        private bool CheckIsArrived()
        {
            Vector3 pos = bicycle.Position;//+ bicycle.Front * Bicycle.Wheelbase;
            Vector2 nowVector = new Vector2(desiredPos.X, desiredPos.Z) - new Vector2(pos.X, pos.Z);
            nowVector.Normalize();
            float dotValue = Vector2.Dot(nowVector, startingVector);
            if /*(dotValue < Math.Cos(MathEx.PiOver2 * 0.8f)) ||*/ (Vector3.Distance(pos, desiredPos) < maxDisFinish)
            {
                return true;
            }
            return false;
        }

        public override void Process(float dt)
        {
            RigidBody bicycleBody = bicycle.RigidBody;

            //计算自行车是否已经到达了目标点了
            if (CheckIsArrived())
            {
                state = GoalState.Finished;
                Terminate();
                return;
            }

            if (bicycle.RequiresAutoReset) 
            {
                Vector3 newPos = desiredPos;
                newPos.Y = bicycle.Position.Y + 2;
                bicycle.Position = newPos;

                Vector3 dir3 = desiredPos - initialPosition;
                Vector2 dir = new Vector2(dir3.X, dir3.Z);
                dir.Normalize();
                Quaternion ori = Quaternion.RotationAxis(Vector3.UnitY, MathEx.Vector2DirAngle(dir));
                bicycle.Orientation = ori;

                bicycle.RigidBody.LinearVelocity = Vector3.Zero;
                bicycle.RigidBody.AngularVelocity = Vector3.Zero;

            }

            //首先把自行车和目标点投影到二维平面上
            Vector2 bicycleToPos = new Vector2(desiredPos.X, desiredPos.Z) -
                                   new Vector2(bicycle.Position.X, bicycle.Position.Z);
            Vector2 bicycleDir = new Vector2(bicycle.Front.X, bicycle.Front.Z);

            bicycleDir = MathEx.GetRotateVector2(bicycleDir, bicycle.SteeringAngle);
            bicycleToPos.Normalize();
            bicycleDir.Normalize();

            //求出自行车把手的旋转角度
            float angleNeed;
            float sitaToPos;
            float sitaDir;


            sitaToPos = MathEx.Vector2DirAngle(bicycleToPos);
            sitaDir = MathEx.Vector2DirAngle(bicycleDir);
            angleNeed = sitaDir - sitaToPos;

            if (Math.Abs(angleNeed) > Math.PI)

            {
                if (angleNeed > 0)
                {
                    angleNeed -= (float)Math.PI * 2;
                }
                else
                {
                    angleNeed += (float)Math.PI * 2;
                }
            }
            angleNeed = MathEx.Clamp(-MathEx.PiOver2, MathEx.PiOver2, angleNeed);
            if (Math.Abs(bicycle.SteeringAngle) > MathEx.PiOver2 - 0.2f)
            {
                int i = 1;
            }
            //float dis = Vector3.Distance(bicycle.Position, desiredPos);
            //float angleFuzzy = (float)((Math.PI / 10) * dis / startingDis + 0.05f);
            
            //if (angleNeed > angleFuzzy)
            //{
            //    angleNeed -= angleFuzzy;
            //}
            //else if (angleNeed < -angleFuzzy)
            //{
            //    angleNeed += angleFuzzy;
            //}
            //else
            //{
            //    angleNeed -= angleNeed * Vector3.Distance(bicycle.Position, desiredPos) / 10;
            //}

            //angleNeed *= .5f;

            //尽可能多的旋转把手
            //不过把手有一个旋转的速度限制
            float angleRotated = Bicycle.maxForkAngularV * dt;
            if (Math.Abs(angleRotated) > Math.Abs(angleNeed))
            {
                angleRotated = angleNeed;
            }
            else
            {
                angleRotated *= (float)Math.Sign(angleNeed);
            }

            float steeringAngle;
            //更新steeringAngle到自行车去
            if (Vector3.Distance(bicycle.Position, desiredPos) > 1f)
            {
                steeringAngle = bicycle.SteeringAngle + angleRotated;
            }
            else
            {
                steeringAngle = bicycle.SteeringAngle;
            }

            steeringAngle = MathEx.Clamp(-MathEx.PiOver2, MathEx.PiOver2, steeringAngle);
            bicycle.SteeringAngle = steeringAngle;

            //求解计算需要的速度
            float wheelbase = Bicycle.Wheelbase;
            float leanAngle = bicycle.LeanAngle;
            float casterAngle = Bicycle.CasterAngle;
            float steeringRadius = (float)Math.Abs((wheelbase * Math.Cos(leanAngle) / (steeringAngle * Math.Cos(casterAngle))));

            if (!MathEx.IsFloatNormal(steeringRadius))
                steeringRadius = 0;

            //算出最大的速度
            float maxV = BicycleToPointPart.ComputeMaxV(steeringRadius);
            float frontV = bicycle.FrontVelocity.Length();

            //如果支持的速度小于自行车的当前速度
            if (maxV < frontV)
            {
                if (frontV - maxV > Bicycle.maxBicycleBrake * 0.3f)
                {
                    frontV -= Bicycle.maxBicycleBrake * 0.3f * dt;
                }
                else
                {
                    frontV = maxV;
                }
            }
            //如果自行车还有加速的空间
            else
            {
                if (frontV < Bicycle.maxBicycleSpeed)
                {
                    frontV += Bicycle.maxBicycleAcc * dt;
                }
            }

            if (!MathEx.IsFloatNormal(frontV) || frontV < 0)
                frontV = 0;

            //最后得到最大的自行车速度
            Vector3 deltaVelocity = bicycle.Front * frontV -
                Vector3.Dot(bicycleBody.LinearVelocity, bicycle.Front) * bicycle.Front;
            bicycleBody.ApplyCentralImpulse(deltaVelocity * bicycle.Mass);

        }

        public override void Terminate()
        {

        }
        #endregion
    }
}
