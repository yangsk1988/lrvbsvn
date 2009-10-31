using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Goal
{
    public class BicycleToPointPart : BaseGoal
    {
        #region Fields

        private Bicycle bicycle;

        //记录自行车所要移动到的线段的左右的限制
        private Vector3 leftSide;
        private Vector3 rightSide;

        private float startingCross;
        #endregion

        #region Constructor
        public BicycleToPointPart(DynamicObject obj, GoalManager mgr,
            BaseGoal fatherGoal, Vector3 leftPoint, Vector3 rightPoint)
            : base(obj, mgr, fatherGoal)
        {
            bicycle = (Bicycle)obj;
            leftSide = leftPoint;
            rightSide = rightPoint;
        }
        #endregion

        #region Methods
        public override void Activate()
        {
            isActived = true;
            //初始的外积,非Vector3自带的Cross
            startingCross = MathEx.Vec3Cross((rightSide - leftSide), bicycle.Position - leftSide);
        }

        private bool CheckIsArrived()
        {
            float curCross = MathEx.Vec3Cross((rightSide - leftSide), bicycle.Position - leftSide);
            float curAngle = MathEx.Vec3AngleAbs(rightSide - bicycle.Position, leftSide - bicycle.Position);
            if ((startingCross * curCross <= 0f) || (curAngle > MathEx.PiOver2 * 2 - 0.2f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        enum EnumDangerType
        {
            //不会碰撞
            Cannot,
            //追击碰撞
            Chase,
            //相对碰撞
            FaceToFaceCollide,
            //其他碰撞
            Others
        }

        /// <summary>
        /// 根据目标的位置和速度向量得出是否危险
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="targetV"></param>
        /// <returns>返回值表示危险的可能时间值(越小越危险,极小的数表示安全)</returns>
        private EnumDangerType GetDangerType(Vector3 cV,float steeringAngle,Bicycle target,out float cTime)
        {
            float zero = 1e-2f;
            float nearZero = 0.2f;
            float nearPI = (float)Math.PI - nearZero;
            float time = 0f;
            float tCur = 0f;
            float tTar = 0f;

            cTime = float.MaxValue;

            //首先把这几个向量得出来
            Vector3 targetV_3 = target.RigidBody.LinearVelocity;
            Vector3 targetPos_3 = target.Position;
            Vector3 curV_3 = cV;
            Vector3 curPos_3 = bicycle.Position;

            //然后转化到xz平面上
            Vector2 targetV = new Vector2(targetV_3.X, targetV_3.Z);
            Vector2 targetPos = new Vector2(targetPos_3.X, targetPos_3.Z);
            Vector2 curV = new Vector2(curV_3.X, curV_3.Z);
            curV = MathEx.GetRotateVector2(curV, steeringAngle);
            Vector2 curPos = new Vector2(curPos_3.X, curPos_3.Z);

            //然后处理一下一些特殊的情况
            //1.两者的速度都很小
            if ((targetV.Length() < zero) && (curV.Length() < zero))
            {
                //不考虑
                return EnumDangerType.Cannot;
            }

            //2.目标在当前自行车的后方
            Vector2 curToTar = new Vector2(targetPos.X - curPos.X,targetPos.Y - curPos.Y);
            if (MathEx.Vec2AngleAbs(curToTar, new Vector2(bicycle.Front.X,bicycle.Front.Z)) > MathEx.PiOver2)
            {
                //不考虑
                return EnumDangerType.Cannot;
            }
            
            //下面两个决策的考虑首先是速度都不能太小
            if ((targetV.Length() > zero) && (curV.Length() > zero))
            {
                //3.考虑两者的速度方向几乎一样
                if (MathEx.Vec2AngleAbs(targetV, curV) <= nearZero)
                {
                    //如果目标的速度更快(追不上)
                    if (targetV.Length() - zero > curV.Length())
                    {
                        return EnumDangerType.Cannot;
                    }
                    else
                    {
                        time = Vector2.Distance(curPos, targetPos) / (targetV.Length() - curV.Length());
                        cTime = time;
                        return EnumDangerType.Chase;
                    }
                }

                //4.考虑两者的速度方向几乎反向
                if (MathEx.Vec2AngleAbs(targetV, curV) >= nearPI)
                {
                    time = Vector2.Distance(curPos, targetPos) / (targetV.Length() + curV.Length());
                    cTime = time;
                    return EnumDangerType.FaceToFaceCollide;
                }
            }

            //下面就是考虑直线求交点推出的碰撞公式了
            //考虑分子为0的情况
            if (Math.Abs(targetV.X * curV.Y - targetV.Y * curV.X) < zero)
            {
                return EnumDangerType.Cannot;
            }
            else
            {
                tTar = (curV.Y * targetPos.X - curV.X * targetPos.Y + curPos.Y * curV.X - curPos.X * curV.Y)
                    / (targetV.X * curV.Y - targetV.Y * curV.X);
                tCur = (targetPos.X * targetV.Y - targetV.X * targetPos.Y + targetV.X * curPos.Y - targetV.Y * curPos.X)
                    / (targetV.Y * curV.X - targetV.X * curV.Y);
                //如果碰撞的时间有一个小于0,则返回不能碰撞
                if ((tTar < -zero) || (tCur < -zero))
                {
                    return EnumDangerType.Cannot;
                }
                else
                {
                    cTime = tTar - tCur;
                    if (tTar - tCur < 1.5f)
                    {
                        return EnumDangerType.Others;
                    }
                }
            }

            return EnumDangerType.Cannot;
        }

        public static float ComputeMaxV(float steeringRadius)
        {
            return (float)Math.Sqrt(MathEx.GravityAcceleration * Bicycle.StaticFrictionFactor * steeringRadius * 0.1f);
        }

        private Vector3 GetMaxFrontVelocity(float steeringAngle)
        {
            float wheelbase = Bicycle.Wheelbase;
            float leanAngle = bicycle.LeanAngle;
            float casterAngle = Bicycle.CasterAngle;
            float steeringRadius = (float)Math.Abs((wheelbase * Math.Cos(leanAngle) / (steeringAngle * Math.Cos(casterAngle))));
            float maxV = ComputeMaxV(steeringRadius);

            if (!MathEx.IsFloatNormal(steeringRadius))
                steeringRadius = 0;

            return maxV * bicycle.Front;
        }

        private Vector3 GetDeltaVelocity(float steeringAngle,float dt)
        {
            Vector3 deltaVelocity;

            //求解计算需要的速度
            float wheelbase = Bicycle.Wheelbase;
            float leanAngle = bicycle.LeanAngle;
            float casterAngle = Bicycle.CasterAngle;
            float steeringRadius = (float)Math.Abs((wheelbase * Math.Cos(leanAngle) / (steeringAngle * Math.Cos(casterAngle))));

            if (!MathEx.IsFloatNormal(steeringRadius))
                steeringRadius = 0;

            //算出最大的速度
            float maxV = ComputeMaxV(steeringRadius);
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
            deltaVelocity = bicycle.Front * frontV -
                Vector3.Dot(bicycle.RigidBody.LinearVelocity, bicycle.Front) * bicycle.Front;
            return deltaVelocity;
        }

        private void CalculateNormalMotion(out float steeringAngle, out Vector3 deltaVelocity,float dt)
        {
            RigidBody bicycleBody = bicycle.RigidBody;
            Vector3 desiredPos = (leftSide + rightSide) * 0.5f;
            //赋初值
            steeringAngle = 0f;
            deltaVelocity = new Vector3();

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

            //更新steeringAngle到自行车去
            if (Vector3.Distance(bicycle.Position, desiredPos) > 1f)
            {
                steeringAngle = bicycle.SteeringAngle + angleNeed;
            }
            else
            {
                steeringAngle = bicycle.SteeringAngle;
            }

            steeringAngle = MathEx.Clamp(-MathEx.PiOver2, MathEx.PiOver2, steeringAngle);

            deltaVelocity = GetDeltaVelocity(steeringAngle,dt);
        }

        private void ApplyPhysicMethod(Vector3 deltaVelocity, float steeringAngle)
        {
            bicycle.SteeringAngle = steeringAngle;
            bicycle.RigidBody.ApplyCentralImpulse(deltaVelocity * bicycle.Mass);
        }

        public override void Process(float dt)
        {
            BicycleManager bicycleMgr = bicycle.BicycleMgr;

            //如果到达目标了,则设置当前状态
            if (CheckIsArrived())
            {
                state = GoalState.Finished;
                return;
            }

            Vector3 normalDeltaV;
            float normalSteeringAngle;
            EnumDangerType dangerType = EnumDangerType.Cannot;
            Vector3 normalNewV;
            float collideTime = float.MaxValue;
            Bicycle dangerBicycle = null;
            float steeringAngle;

            //计算出正常移动情况下的状态
            CalculateNormalMotion(out normalSteeringAngle, out normalDeltaV, dt);
            normalNewV = (Vector3)bicycle.RigidBody.LinearVelocity + normalDeltaV;

            for (int i = 0; i < bicycleMgr.BicycleList.Count; i++)
            {
                if (bicycleMgr.BicycleList[i] != bicycle)
                {
                    EnumDangerType curDangerType;
                    float cTime = 0f;
                    curDangerType = GetDangerType(normalNewV, normalSteeringAngle, bicycleMgr.BicycleList[i], out cTime);
                    if (dangerType != EnumDangerType.Cannot)
                    {
                        if (cTime < collideTime)
                        {
                            dangerType = curDangerType;
                            collideTime = cTime;
                            dangerBicycle = bicycleMgr.BicycleList[i];
                        }
                    }
                }
            }

            //如果正常移动情况下会不安全
            if (dangerType != EnumDangerType.Cannot)
            {
                bool isGetSolution = false;
                float newSteeringAngle = normalSteeringAngle;

                //尝试旋转角度,看看能否使得更加安全
                float dAngle = 0f;
                float leftAngle = normalSteeringAngle - dAngle;
                float rightAngle = normalSteeringAngle + dAngle;

                //每次增加一个小的旋转角度,进行计算
                while ((leftAngle > -MathEx.PiOver2) && (rightAngle < MathEx.PiOver2))
                {
                    //每次优先取出最接近中心的旋转角度,得出速度和角度
                    if (Math.Abs(leftAngle) > Math.Abs(rightAngle))
                    {
                        Vector3 curV = GetMaxFrontVelocity(rightAngle);
                        float cTime = 0f;
                        if (GetDangerType(curV, rightAngle, dangerBicycle, out cTime) == EnumDangerType.Cannot)
                        {
                            isGetSolution = true;
                            newSteeringAngle = rightAngle;
                            break;
                        }

                        if (GetDangerType(curV, leftAngle, dangerBicycle, out cTime) == EnumDangerType.Cannot)
                        {
                            isGetSolution = true;
                            newSteeringAngle = leftAngle;
                            break;
                        }
                    }
                    else
                    {
                        Vector3 curV = GetMaxFrontVelocity(rightAngle);
                        float cTime = 0f;
                        if (GetDangerType(curV, leftAngle, dangerBicycle, out cTime) == EnumDangerType.Cannot)
                        {
                            isGetSolution = true;
                            newSteeringAngle = leftAngle;
                            break;
                        }

                        if (GetDangerType(curV, rightAngle, dangerBicycle, out cTime) == EnumDangerType.Cannot)
                        {
                            isGetSolution = true;
                            newSteeringAngle = rightAngle;
                            break;
                        }
                    }
                    dAngle += MathEx.PiOver2 / 9;
                    leftAngle = normalSteeringAngle - dAngle;
                    rightAngle = normalSteeringAngle + dAngle;
                }

                //如果有某种旋转方式可以使得自行车相对安全
                if (isGetSolution)
                {
                    steeringAngle = newSteeringAngle;
                }
                else
                {
                    steeringAngle = normalSteeringAngle;
                }
            }
            else
            {
                steeringAngle = normalSteeringAngle;
            }

            Vector3 deltaV = GetDeltaVelocity(steeringAngle, dt);
            ApplyPhysicMethod(deltaV, steeringAngle);
        }

        public override void Terminate()
        {

        }
        #endregion
    }
}
