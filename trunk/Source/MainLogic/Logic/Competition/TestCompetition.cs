using System;
using System.Collections.Generic;
using System.Text;
using MainLogic;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Input;
using VirtualBicycle.Logic.Goal;
using VirtualBicycle.Logic.PathWay;
using VirtualBicycle.Logic.Traffic;
using VirtualBicycle.MathLib;
using VirtualBicycle.ObjectMgr;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;
using PM = VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.Logic.Competition
{
    /// <summary>
    /// 测试代码放在这儿
    /// </summary>
    public class TestCompetition : BaseCompetition
    {
        BicycleManager bicycleManager;

        Bicycle playerBicycle;
        Bicycle comBicycle;

        ObjectManager objectManager;
        ObjectCreator objectCreator;

        AIPathManager aiPathManager;
        GameMainLogic logic;
        Building destination;

        Vector3[] tempTest;

        public TestCompetition(Game game, GameMainLogic logic, World world, List<LogicalArea> areas, SceneObject[] sceneObjects)
            : base(game, world, areas, sceneObjects)
        {
            this.logic = logic;
        }

        public override void Initialize()
        {
            #region 建立Bicycle和道路信息
            bicycleManager = new BicycleManager();
            objectManager = new ObjectManager();
            objectCreator = new ObjectCreator(device, game.CurrentWorld, objectManager);

            playerBicycle = objectCreator.CreateBicycle(new Vector3(2, 2, 2));
            playerBicycle.OwnerType = Bicycle.BicycleOwner.Player;
            playerBicycle.BuildPhysicsModel(world.PhysicsWorld);
            bicycleManager.RegisterBicycle(playerBicycle);

            //把当前的目标设置到CurrentBicycle上
            CurrentBicycle = playerBicycle;

            comBicycle = objectCreator.CreateBicycle(new Vector3(10, 2, 10));
            comBicycle.BuildPhysicsModel(world.PhysicsWorld);
            comBicycle.BicycleGoalMgr = new GoalManager(comBicycle);
            comBicycle.OwnerType = Bicycle.BicycleOwner.Computer;
            bicycleManager.RegisterBicycle(comBicycle);

            GoalManager goalMgr = comBicycle.BicycleGoalMgr;
            BicycleThinkGoal thinkGoal = new BicycleThinkGoal(comBicycle, goalMgr, null);

            //建立道路信息
            aiPathManager = new AIPathManager(SceneObjects);
            Queue<Vector3> points = new Queue<Vector3>();

            if (aiPathManager.AITrafficComponets.Count > 0)
            {
                AIPort comBicycleStartPort = aiPathManager.GetPortOfUID(0);
                comBicycle.Position = comBicycleStartPort.Position + new Vector3(0f, 2f, 0f);
                comBicycle.Orientation = Quaternion.RotationAxis(Vector3.UnitY, MathEx.PIf);

                thinkGoal.AddSubGoal(new BicycleToPorts(comBicycle, goalMgr, thinkGoal, aiPathManager, 0, aiPathManager.AIPorts.Count - 1));
                goalMgr.GoalSeq.Enqueue(thinkGoal);

                playerBicycle.Position = comBicycle.Position + new Vector3(2f, 2f, 2f);
                playerBicycle.Orientation = Quaternion.RotationAxis(Vector3.UnitY, MathEx.PIf);
            }
            #endregion

            #region 初始化摄像机类
            playerView = new ViewManager((float)game.Window.ClientSize.Width / (float)game.Window.ClientSize.Height);

            playerView.SetNearPlane(0.1f);
            playerView.SetFarPlane(750);
            playerView.SetFieldOfView(45);
            playerView.SetRenderTarget(device.GetRenderTarget(0));
            playerView.Mode = ViewMode.ThirdPersion;
            playerView.CurrentBicycle = playerBicycle;

            world.Scene.RegisterCamera(playerView);

            InputManager.Instance.ViewChanged += this.ViewChanged;
            InputManager.Instance.Reset += this.ResetPlayerBicycle;
            
            #endregion

            
            for (int i = 0; i < base.SceneObjects.Length; i++) 
            {
                if (destination == null)
                {
                    Building bld = SceneObjects[i] as Building;
                    if (bld != null)
                    {
                        if (CaseInsensitiveStringComparer.Compare(bld.TypeName, "finalflag"))
                        {
                            destination = bld;
                        }
                    }
                }
                if (tempTest == null)
                {
                    Road road = SceneObjects[i] as Road;

                    if (road != null) 
                    {
                        List<RoadNode> nodes = road.TrackLine.InterposedPoints;

                        tempTest = new Vector3[nodes.Count];

                        for (int j = 0; j < nodes.Count; j++)
                        {
                            tempTest[j] = nodes[j].Position;
                        }

                        Vector3 dir3 = tempTest[1] - tempTest[0];
                        Vector2 dir = new Vector2(dir3.X, dir3.Z);
                        dir.Normalize();

                        Quaternion ori = Quaternion.RotationAxis(Vector3.UnitY, MathEx.Vector2DirAngle(dir));
                        comBicycle.Orientation = ori;
                        playerBicycle.Orientation = ori;

                        ResetPlayerBicycle(this, EventArgs.Empty);
                    }
                }
                if (destination != null && tempTest != null) 
                {
                    break;
                }
            }

            base.Initialize();
        }

        protected void ResetPlayerBicycle(object sender, EventArgs e)
        {
            if (tempTest != null)
            {
                float distance = float.MaxValue;
                int nearestIndex = -1;

                Vector3 pos = playerBicycle.Position;
                for (int i = 0; i < tempTest.Length; i++)
                {
                    float d = Vector3.Distance(pos, tempTest[i]);

                    if (d < distance)
                    {
                        distance = d;
                        nearestIndex = i;
                    }
                }

                if (nearestIndex != -1)
                {
                    playerBicycle.Position = tempTest[nearestIndex] + Vector3.UnitY * 2;

                    if (nearestIndex < tempTest.Length - 1)
                    {
                        Vector3 dir3 = tempTest[nearestIndex + 1] - tempTest[nearestIndex];
                        Vector2 dir = new Vector2(dir3.X, dir3.Z);
                        dir.Normalize();

                        Quaternion ori = Quaternion.RotationAxis(Vector3.UnitY, MathEx.Vector2DirAngle(dir));
                        playerBicycle.Orientation = ori;

                        playerBicycle.RigidBody.LinearVelocity = Vector3.Zero;
                        playerBicycle.RigidBody.AngularVelocity = Vector3.Zero;

                    }
                }
            }
        }

        void Win()
        {
            game.GameUI.Pop();
            base.game.CurrentWorld = null;
            world.Paused = true;
            world.Dispose();

            game.GameUI.CurrentComponent = logic.UILogic.GetReportScreen();
        }

        public override void Update(float dt)
        {
            if (world != null && world.IsValid)
            {
                RigidBody body = comBicycle.RigidBody;
                DefaultMotionState motionState = (DefaultMotionState)body.MotionState;
                PM.Matrix worldTransform = motionState.GraphicsWorldTransform;

                comBicycle.UpdateLogic(dt);

                if (!body.IsActive)
                    body.Activate();

                playerBicycle.UpdateLogic(dt);

                body = playerBicycle.RigidBody;

                if (!body.IsActive)
                    body.Activate();

                if (destination != null)
                {
                    float dist = Vector3.Distance(playerBicycle.BoundingSphere.Center, destination.BoundingSphere.Center);

                    if (dist < 6)                     
                    {
                        Win();
                    }
                }
            }

            base.Update(dt);
        }

        protected override void Dispose(bool disposing)
        {
            Unload();
            playerBicycle.Dispose();
            comBicycle.Dispose();
        }

        public override void Unload()
        {
            InputManager.Instance.ViewChanged -= this.ViewChanged;
            InputManager.Instance.Reset -= this.ResetPlayerBicycle;
            
            base.Unload();
        }

        protected void ViewChanged(object sender, EventArgs e)
        {
            if (world != null && world.IsValid)
            {
                if (playerView.Mode == ViewMode.FirstPerson)
                {
                    playerView.Mode = ViewMode.ThirdPersion;
                }
                else
                {
                    playerView.Mode = ViewMode.FirstPerson;
                }
            }
        }
    }
}
