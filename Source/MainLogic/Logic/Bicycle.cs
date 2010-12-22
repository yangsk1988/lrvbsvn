using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.CollisionModel.Shapes;
using VirtualBicycle.Graphics;
using VirtualBicycle.Input;
using VirtualBicycle.IO;
using VirtualBicycle.Logic.Goal;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;
using PM = VirtualBicycle.Physics.MathLib;
using VirtualBicycle.Collections;


namespace VirtualBicycle.Logic
{
    public enum BicycleColor 
    {
        Red,
        Yellow,
        Blue,
        Purple,
        Green
    }

    public class Bicycle : DynamicObject
    {
        #region Fields

        Vector3 force;
        PM.Vector3 lastLinearVel;


        public enum BicycleOwner
        {
            Computer,
            Player
        }

        float mass;

        public float Mass
        {
            get { return mass; }
        }

        //最大的前叉旋转角速度
        public const float maxForkAngularV = MathEx.PiOver2 * 4;

        //最大的自行车速度
        public const float maxBicycleSpeed = 16.67f; //60km/h

        //自行车的最大加速度
        public const float maxBicycleAcc = 5f;

        //自行车的最大刹车速度
        public const float maxBicycleBrake = 10f;

        float preSteeringAngle = float.MaxValue;

        float deltaSteeringAngle;

        Queue<float> qLeanAngle = new Queue<float>();
        float drawLeanAngle;

        public float DrawLeanAngle
        {
            get { return drawLeanAngle; }
        }

        /// <summary>
        /// 自行车的所有者类型
        /// 电脑或者玩家控制
        /// </summary>
        private BicycleOwner ownerType;
        public BicycleOwner OwnerType
        {
            get { return ownerType; }
            set
            {
                ownerType = value;

                if (value == BicycleOwner.Computer)
                {
                    if (isHumanInputBound)
                    {
                        UnbindInput();
                        isHumanInputBound = false;
                    }
                }
            }
        }

        /// <summary>
        /// 前轮与车身之间的夹角
        /// </summary>
        private float steeringAngle;
        public float SteeringAngle
        {
            get { return steeringAngle; }
            set { steeringAngle = value; }
        }

        private bool isMovingFront;

        /// <summary>
        /// 前后轮轴心之间的距离
        /// </summary>
        public const float Wheelbase = 1.1f;

        /// <summary>
        /// 转弯的时候的半径
        /// </summary>
        private float steeringRadius;
        public float SteeringRadius
        {
            get { return steeringRadius; }
            set { steeringRadius = value; }
        }

        /// <summary>
        /// 前轮的轴与水平面的夹角
        /// (参考:http://en.wikipedia.org/wiki/Bicycle_and_motorcycle_geometry#Steering_axis_angle)
        /// </summary>
        public const float CasterAngle = (float)((74.0f / 360.0f) * Math.PI * 2);
        public static readonly Vector3 CasterAxis;

        /// <summary>
        /// 计算静摩擦力的时候的因子
        /// </summary>
        public const float StaticFrictionFactor = 2.0f;

        /// <summary>
        /// 车身的倾斜角
        /// </summary>
        private float leanAngle;
        public float LeanAngle
        {
            get { return leanAngle; }
            set
            {
                leanAngle = value;
            }
        }

        private GoalManager bicycleGoalMgr;
        public GoalManager BicycleGoalMgr
        {
            get { return bicycleGoalMgr; }
            set { bicycleGoalMgr = value; }
        }

        private Vector3 front;
        public Vector3 Front
        {
            get { return front; }
        }

        private Vector3 up;
        public Vector3 Up
        {
            get { return up; }
        }

        private Vector3 right;
        public Vector3 Right
        {
            get { return right; }
        }

        private Vector3 frontVelocity;
        public Vector3 FrontVelocity
        {
            get { return frontVelocity; }
        }

        private Vector3 upVelocity;
        public Vector3 UpVelocity
        {
            get { return upVelocity; }
        }

        private Vector3 rightVelocity;
        public Vector3 RightVelocity
        {
            get { return rightVelocity; }
        }


        public bool IsOutOfControl
        {
            get;
            private set;
        }
        public bool IsFreeFalling
        {
            get;
            private set;
        }

        /// <summary>
        /// 倒下的时间
        /// </summary>
        private float fallTime;

        public string OwnerName
        {
            get;
            set;
        }

        public BicycleManager BicycleMgr
        {
            get;
            set;
        }

        public int Rank
        {
            get;
            set;
        }

        bool isHumanInputBound;

        //bool lastCtrlState;

        #endregion

        #region Constructor


        static Bicycle()
        {
            Quaternion q = Quaternion.RotationAxis(Vector3.UnitX, MathEx.PiOver2 - CasterAngle);
            CasterAxis = MathEx.QuaternionRotate(q, Vector3.UnitY);
        }

        public Bicycle(Device device)
            : this(device, BicycleOwner.Player)
        { 
        }

        public Bicycle(Device device, BicycleColor color) 
        {
            string fileName = "bicycle.mesh";
            switch (color) 
            {
                case BicycleColor.Red:
                    fileName = "bicycle.mesh";
                    break;
                case BicycleColor.Green:
                    fileName = "bicycle_green.mesh";
                    break;
                case BicycleColor.Blue:
                    fileName = "bicycle_blue.mesh";
                    break;
                case BicycleColor.Purple:
                    fileName = "bicycle_purple.mesh";
                    break;
                case BicycleColor.Yellow:
                    fileName = "bicycle_yellow.mesh";
                    break;
            }
            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(VirtualBicycle.IO.Paths.Models, fileName), FileLocateRules.Default);
            ModelL0 = ModelManager.Instance.CreateInstance(device, fl);
            mass = 70;//kilogram
           
        }

        public Bicycle(Device device, BicycleOwner owner)
        {
            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(VirtualBicycle.IO.Paths.Models, "bicycle.mesh"), FileLocateRules.Default);
            ModelL0 = ModelManager.Instance.CreateInstance(device, fl);
            mass = 70;
            ownerType = owner;
            
        }
        #endregion


        float pwrUpdateDuration;
        private void Bicycle_WheelSpeedChanged(WheelSpeedChangedEventArgs e)
        {
            Power = 0;
            if (!IsOutOfControl && !IsFreeFalling)
            {
                Vector3 impluse;
                if (e.Reference)
                {
                    impluse = front * 10 * Math.Sign(e.DeltaSpeed);
                }
                else
                {
                    float frontV = Vector3.Dot(front, RigidBody.LinearVelocity);
                    float diff = e.Speed - frontV;

                    impluse = Mass * diff * front;
                }

                if (pwrUpdateDuration > float.Epsilon)
                {
                    Vector3 v1 = RigidBody.LinearVelocity;
                    Vector3 v2 = v1 + impluse / Mass;

                    Power = Math.Max(0, (0.5f * Mass * (v2.LengthSquared() - v1.LengthSquared())) / pwrUpdateDuration);

                    pwrUpdateDuration = 0;
                }

                RigidBody.ApplyCentralImpulse(impluse);
            }
        }

        private void Bicycle_HandlerbarRotated(HandlebarRotatedEventArgs e)
        {
            if (!IsOutOfControl && !IsFreeFalling)
            {
                steeringAngle = e.Angle;
            }
        }

        private void BindInput()
        {
            InputManager.Instance.WheelSpeedChanged += this.Bicycle_WheelSpeedChanged;
            InputManager.Instance.HandlebarRotated += this.Bicycle_HandlerbarRotated;
        }

        private void UnbindInput()
        {
            InputManager.Instance.WheelSpeedChanged -= this.Bicycle_WheelSpeedChanged;
            InputManager.Instance.HandlebarRotated -= this.Bicycle_HandlerbarRotated;
        }


        #region 处理输入相关

        /// <summary>
        /// 处理输入
        /// 包括:键盘,串口,目标状态机
        /// </summary>
        /// <param name="dt"></param>
        private void ProcessInput(float dt)
        {
            //如果是人类玩家
            if (ownerType == BicycleOwner.Player)
            {
                if (!IsOutOfControl && !IsFreeFalling)
                {
                    if (!isHumanInputBound)
                    {
                        BindInput();

                        isHumanInputBound = true;
                    }
                }
                //ProcessKeyboardInput(dt);
            }

            //如果是电脑玩家
            if (ownerType == BicycleOwner.Computer)
            {
                bicycleGoalMgr.Update(dt);
            }

        }
        #endregion

        #region 更新


        int forceFeedbackCD = 10;
        FastList<float> dotSmoother = new FastList<float>();

        /// <summary>
        /// 处理运动状态
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateMotion(float dt)
        {
            if (!isMovingFront)
            {
                steeringAngle = -steeringAngle;
            }

            if (!IsOutOfControl && !IsFreeFalling)
            {
                if (preSteeringAngle < float.MaxValue)
                {
                    deltaSteeringAngle = steeringAngle - preSteeringAngle;
                }
                else
                {
                    deltaSteeringAngle = 0f;
                }

                float frontVLen = frontVelocity.Length();
                steeringRadius = (float)Math.Abs((Wheelbase * Math.Cos(LeanAngle) / (steeringAngle * Math.Cos(CasterAngle))));

                if (!MathEx.IsFloatNormal(steeringRadius))
                {
                    steeringRadius = float.MaxValue;
                }

                LeanAngle = Math.Sign(steeringAngle) * (float)(Math.Atan(frontVLen * frontVLen / (MathEx.GravityAcceleration * steeringRadius)));

                if (!MathEx.IsFloatNormal(LeanAngle))
                {
                    LeanAngle = 0;
                }

                if (!isMovingFront)
                {
                    LeanAngle = -LeanAngle;
                }

                RigidBody.AngularVelocity = -((frontVLen / steeringRadius * Math.Sign(steeringAngle)) * up);
                Vector3 linearV = CalcLinearVelocity();
                RigidBody.LinearVelocity = linearV * LinearVelRatio;

                frontWheelAngV = Vector3.Dot(front, linearV);
                backWheelAngV = frontWheelAngV;

                fallTime = 0;

                preSteeringAngle = steeringAngle;

                if (ownerType == BicycleOwner.Player)
                {
                    Vector3 hoz = Front;
                    hoz.Y = 0;
                    Vector3.Normalize(ref hoz, out hoz);

                    float f = -Vector3.Dot(hoz, Up);

                    f *= 5;
                    if (f > 1) f = 1;
                    if (f < -1) f = -1;

                    dotSmoother.Add(0.5f * (f + 1));


                    while (dotSmoother.Count > 10) 
                    {
                        dotSmoother.RemoveAt(0);
                    }

                    f = 0;
                    for (int i = 0; i < dotSmoother.Count; i++) 
                    {
                        f += dotSmoother[i];
                    }
                    f /= (float)dotSmoother.Count;

                    forceFeedbackCD--;
                    if (forceFeedbackCD < 0)
                    {  
                        InputManager.Instance.ForceFeedback(f);
                        forceFeedbackCD = 10;
                    }
                }
            }
            else if (IsOutOfControl && !IsFreeFalling)
            {
                fallTime += dt;
                if (OwnerType == BicycleOwner.Player)
                {
                    if (fallTime > 2)
                    {
                        Vector2 frontVector2 = new Vector2(front.X, front.Z);
                        frontVector2.Normalize();

                        float theta = MathEx.Vector2DirAngle(frontVector2);

                        Orientation = Quaternion.RotationAxis(Vector3.UnitY, theta);
                        Position = Position + Vector3.UnitY;
                        RigidBody.LinearVelocity = Vector3.Zero;
                        RigidBody.AngularVelocity = Vector3.Zero;

                        fallTime = 0;
                    }
                }
                preSteeringAngle = float.MaxValue;
            }

            frontWheelAngV -= frontWheelAngV * 0.05f * dt;
            backWheelAngV -= backWheelAngV * 0.05f * dt;

            frontWheelRot += frontWheelAngV * dt;
            backWheelRot += backWheelAngV * dt;

            frontWheelRot %= 2 * MathEx.PIf;
            backWheelRot %= 2 * MathEx.PIf;

            if (!isMovingFront)
            {
                steeringAngle = -steeringAngle;
            }

            //计算LeanAngle相关
            //更改的仅仅是drawLeanAngle
            qLeanAngle.Enqueue(leanAngle);
            while (qLeanAngle.Count > 10)
            {
                qLeanAngle.Dequeue();
            }
            if (qLeanAngle.Count > 0)
            {
                drawLeanAngle = 0;
                foreach (float lean in qLeanAngle)
                {
                    drawLeanAngle += lean;
                }
                drawLeanAngle /= qLeanAngle.Count;
            }
            else
            {
                drawLeanAngle = leanAngle;
            }
        }


        private bool GetIsMovingFront()
        {
            if (MathEx.Vec3AngleAbs(front, frontVelocity) > MathEx.PiOver2 + 0.1f)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 预计算一些内容
        /// </summary>
        /// 
        float sllipingFriction = 1.5f, rollingFriction = 0.05f;
        public void PreCaculate(float dt)
        {
            //MotionState    motionState = (DefaultMotionState)RigidBody.MotionState;
            //PM.Matrix worldTransform = Transformation;

            front = MathEx.GetMatrixFront(ref Transformation);
            up =  MathEx.GetMatrixUp(ref Transformation);
            right = MathEx.GetMatrixRight(ref Transformation);

            front.Normalize();
            up.Normalize();
            right.Normalize();

            Vector3 linV = RigidBody.LinearVelocity / LinearVelRatio;
            frontVelocity = Vector3.Dot(linV, front) * front;
            upVelocity = Vector3.Dot(linV, up) * up;
            rightVelocity = Vector3.Dot(linV, right) * right;

            if (dt > float.Epsilon)
            {
                PM.Vector3 linV2 = linV;
                force = (Mass / dt) * (linV2 - lastLinearVel);
            }

            bool flag = CheckIsFall() | CheckIsSliped();

            if (flag && !IsOutOfControl)
            {
                Matrix lean = Matrix.Translation(0, 1, 0) * Matrix.RotationAxis(Vector3.UnitZ, drawLeanAngle) * Matrix.Translation(0, -1, 0) * Transformation;

                //Matrix.Transpose(ref lean, out lean);

                Orientation = Quaternion.RotationMatrix(lean);
                Position = Position + Vector3.UnitY * 0.2f;//new Vector3(lean.M14, lean.M24 + 2f, lean.M34);

                //Orientation = Quaternion.RotationAxis(Vector3.UnitZ, -LeanAngle) * Orientation;

                //Quaternion rot = Quaternion.RotationAxis(front, -LeanAngle);
                //Vector4 tmp = Vector3.Transform(up, rot);

                //Vector3 up2 = new Vector3(tmp.X, tmp.Y, tmp.Z);

                //Position -= up2 * (0.02f);

                //Matrix lean = Matrix.Translation(0, 1, 0) * Matrix.RotationAxis(Vector3.UnitZ, LeanAngle) * Matrix.Translation(0, -1, 0) * Transformation;
                //Position = new Vector3(lean.M41, lean.M42, lean.M43);

                LeanAngle = 0;
            }

            IsFreeFalling = (force.Y <= 0.2f * RigidBody.Gravity.Y);
            IsOutOfControl = flag;

            this.Friction = IsOutOfControl ? sllipingFriction : rollingFriction;

            pwrUpdateDuration += dt;
            //lastCtrlState = flag1;
            lastLinearVel = linV;

            isMovingFront = GetIsMovingFront();
        }

        /// <summary>
        /// 更新逻辑
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateLogic(float dt)
        {
            PreCaculate(dt);

            ProcessInput(dt);


            UpdateMotion(dt);
        }



        #endregion

        #region 物理的一些判读
        /// <summary>
        /// 检测是否轮胎与地面打滑了
        /// </summary>
        /// <returns></returns>
        private bool CheckIsSliped()
        {
            frontVelocity = Vector3.Dot(RigidBody.LinearVelocity, front) * front;
            float frontVLen = frontVelocity.Length();
            float CentripetalAcc = frontVLen * frontVLen / steeringRadius;
            if (CentripetalAcc > MathEx.GravityAcceleration * StaticFrictionFactor)
            {
                return true;
            }

            return false;
        }


        public const float LinearVelRatio = 3.5f;
        /// <summary>
        /// 处理线速度
        /// </summary>
        /// <returns></returns>
        private Vector3 CalcLinearVelocity()
        {
            //const float maxUpVelocity = 18.2f;

            Vector3 linV = RigidBody.LinearVelocity / LinearVelRatio;

            frontVelocity = Vector3.Dot(linV, front) * front;
            upVelocity = Vector3.Dot(linV, up) * up;
            rightVelocity = Vector3.Dot(linV, right) * right;

            Vector4 vec4 = Vector3.Transform(frontVelocity, Quaternion.RotationAxis(up, -deltaSteeringAngle));
            frontVelocity = new Vector3(vec4.X, vec4.Y, vec4.Z);

            //if (!isFreeVelocity)
            //{
            //    if (Vector3.Dot(RigidBody.LinearVelocity,-Vector3.UnitY) > maxUpVelocity) //如果向下的速度太大(类似于掉下去了)
            //    {
            //            isFreeVelocity = true;
            //    }
            //}

            //if (isFreeVelocity)
            //{
            //    return frontVelocity + upVelocity + rightVelocity;
            //}
            //else
            //{
            return frontVelocity + upVelocity;
            //}
        }

        /// <summary>
        /// 检查是否摔倒
        /// </summary>
        /// <returns></returns>
        private bool CheckIsFall()
        {
            const float maxDot = 0.25f; //(float)Math.Cos(75.0 / 180.0 * Math.PI);
            //如果身体和地面的夹角小于15度了,则认为摔倒了
            if (Vector3.Dot(up, -Vector3.Normalize(RigidBody.Gravity)) < maxDot)
            {
                //修改摩擦系数
                return true;
            }
            return false;
        }

        public BoundingBox GetAABB2D()
        {
            Vector3 Pointa = position + front * Wheelbase / 2;
            Vector3 Pointb = position - front * Wheelbase / 2;
            Vector3 minPoint = Vector3.Zero;
            Vector3 maxPoint = Vector3.Zero;

            minPoint.X = Pointa.X > Pointb.X ? Pointb.X : Pointa.X;
            minPoint.Y = Pointa.Y > Pointb.Y ? Pointb.Y : Pointa.Y;
            maxPoint.X = Pointa.X < Pointb.X ? Pointb.X : Pointa.X;
            maxPoint.Y = Pointa.Y < Pointb.Y ? Pointb.Y : Pointa.Y;
            return new BoundingBox(minPoint, maxPoint);
        }
        #endregion

        #region 初始化
        public override bool IsSerializable
        {
            get { return false; }
        }

        public override void BuildPhysicsModel(DynamicsWorld world)
        {
            if (RequiresUpdate)
                UpdateTransform();

            BoxShape shape = new BoxShape(new Vector3(0.35f, 1f, 1f));

            MotionState motionState = new DefaultMotionState(Transformation);

            PM.Vector3 inertia;
            shape.CalculateLocalInertia(Mass, out inertia);

            RigidBody = new RigidBody(Mass, motionState, shape, inertia, 0, 0, 0.05f, 0);

            this.Restitution = 0.1f;

            if (world != null)
            {
                world.AddRigidBody(RigidBody);
            }
        }
        #endregion

        public bool RequiresAutoReset
        {
            get { return OwnerType == BicycleOwner.Computer && fallTime > 2 && IsOutOfControl && !IsFreeFalling; }
        }

        public float Power
        {
            get;// { return !IsOutOfControl ? Math.Max(0, -Vector3.Dot(front, force) * Vector3.Dot(front, frontVelocity)) : 0; }
            private set;
        }

        int handleBarIndex = -1;
        int frontWheelIndex = -1;
        int backWheelIndex = -1;

        float frontWheelRot;
        float backWheelRot;

        float frontWheelAngV;
        float backWheelAngV;



        public override RenderOperation[] GetRenderOperation()
        {
            if (HasLodModel)
            {
                if (ModelL1 != null)
                {
                    return ModelL1.GetRenderOperation();
                }
            }
            if (ModelL0 != null)
            {
                RenderOperation[] ops = ModelL0.GetRenderOperation();

                if (handleBarIndex == -1 || frontWheelIndex == -1 || backWheelIndex == -1)
                {
                    GameMesh[] ents = ModelL0.Entities;
                    for (int i = 0; i < ents.Length; i++)
                    {
                        if (CaseInsensitiveStringComparer.Compare(ents[i].Name, "handlebar"))
                        {
                            for (int j = 0; j < ops.Length; j++)
                            {
                                if (object.ReferenceEquals(ops[j].Geomentry.VertexBuffer, ents[i].VertexBuffer))
                                {
                                    handleBarIndex = j;
                                    break;
                                }
                            }
                        }
                        else if (CaseInsensitiveStringComparer.Compare(ents[i].Name, "frontwheel"))
                        {
                            for (int j = 0; j < ops.Length; j++)
                            {
                                if (object.ReferenceEquals(ops[j].Geomentry.VertexBuffer, ents[i].VertexBuffer))
                                {
                                    frontWheelIndex = j;
                                    break;
                                }
                            }
                        }
                        else if (CaseInsensitiveStringComparer.Compare(ents[i].Name, "backwheel"))
                        {
                            for (int j = 0; j < ops.Length; j++)
                            {
                                if (object.ReferenceEquals(ops[j].Geomentry.VertexBuffer, ents[i].VertexBuffer))
                                {
                                    backWheelIndex = j;
                                    break;
                                }
                            }
                        }

                    }
                }

                if (handleBarIndex != -1)
                {
                    Matrix hbRot = Matrix.RotationAxis(Vector3.UnitY, -steeringAngle);

                    ops[handleBarIndex].Transformation = hbRot * ops[handleBarIndex].Transformation;
                }
                if (frontWheelIndex != -1)
                {
                    Matrix hbRot = Matrix.RotationAxis(Vector3.UnitZ, steeringAngle);

                    const float WheelAxisDist = 1.55f;

                    ops[frontWheelIndex].Transformation = Matrix.RotationY(frontWheelRot) *
                        Matrix.Translation(-WheelAxisDist, 0, 0) * hbRot * Matrix.Translation(WheelAxisDist, 0, 0) *
                        ops[frontWheelIndex].Transformation;
                }
                if (backWheelIndex != -1)
                {
                    ops[backWheelIndex].Transformation = Matrix.RotationY(backWheelRot) * ops[backWheelIndex].Transformation;
                }

                if (!IsOutOfControl)
                {
                    Matrix lean = Matrix.Translation(0, 1, 0) * Matrix.RotationAxis(Vector3.UnitZ, drawLeanAngle) * Matrix.Translation(0, -1, 0);

                    for (int i = 0; i < ops.Length; i++)
                    {
                        ops[i].Transformation *= lean;
                    }
                }
                return ops;
            }
            return null;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (isHumanInputBound)
                {
                    UnbindInput();
                    isHumanInputBound = false;
                }


                if (ModelL0 != null)
                {
                    ModelManager.Instance.DestoryInstance(ModelL0);
                }
                if (ModelL1 != null)
                {
                    ModelManager.Instance.DestoryInstance(ModelL1);
                }
            }
            base.Dispose(disposing);
        }
    }
}
