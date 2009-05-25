using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Input;
using VirtualBicycle.Logic.Goal;
using VirtualBicycle.Logic.PathWay;
using VirtualBicycle.MathLib;
using VirtualBicycle.ObjectMgr;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;
using PM = VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.Logic.Competition
{
    /// <summary>
    /// 表示一辆自行车开始的信息
    /// </summary>
    public struct StartData
    {
        public int PlayerID;
        public Vector3 Position;
        public int Stat;

        public StartData(int pID, Vector3 pos, int stat)
        {
            PlayerID = pID;
            Position = pos;
            Stat = stat;
        }
    }

    /// <summary>
    /// 表示一辆自行车的结束的信息
    /// </summary>
    public struct EndData
    {
        public Vector3 Position;
        public float Radius;

        public EndData(Vector3 pos, float r)
        {
            Position = pos;
            Radius = r;
        }
    }

    /// <summary>
    /// 普通竞速赛
    /// </summary>
    public class RacingCompetition : BaseCompetition
    {
        #region Fields

        List<StartData> BicycleStartInfo;
        List<EndData>  BicycleEndInfo;

        ObjectManager objectManager;
        ObjectCreator objectCreator;
        BicycleManager bicycleManager;

        AIPathManager aiPathManager;

        #endregion

        public RacingCompetition(Game game, World world, List<LogicalArea> areas, SceneObject[] sceneObjects)
            : base(game, world, areas, sceneObjects)
        {

        }

        private void PlaceRegisterBicycle(Vector3 startPos,Vector3 endPos,int PlayerID)
        {
            //建立Bicycle
            Bicycle bicycle = objectCreator.CreateBicycle(Vector3.Zero);

            //设置Bicycle的位置和方向
            Vector2 dir = new Vector2(endPos.X - startPos.X, endPos.Z - startPos.Z);
            float theta = MathEx.VectorToPolar(dir).sita;

            Quaternion ori;
            ori = Quaternion.Identity * Quaternion.RotationAxis(Vector3.UnitY, theta);

            bicycle.Position = startPos + Vector3.UnitY * 1.5f;
            bicycle.Orientation = ori;

            //建立Bicycle的碰撞模型
            bicycle.BuildPhysicsModel(world.PhysicsWorld);

            //注册bicycle
            if (PlayerID == 0)
            {
                bicycle.OwnerType = Bicycle.BicycleOwner.Player;
                bicycleManager.RegisterBicycle("LiveForRide", bicycle);
            }
            else
            {
                bicycle.OwnerType = Bicycle.BicycleOwner.Computer;
                bicycleManager.RegisterBicycle("Player" + PlayerID.ToString(), bicycle);
            }
        }

        static readonly char[] sepratorChars = new char[] { '_'};

        private bool CheckIsStartPos(LogicalArea area)
        {
            string[] strSplited = area.TypeName.Split(sepratorChars, StringSplitOptions.RemoveEmptyEntries);
            if (strSplited[2] == "START")
            {
                return true;
            }

            return false;
        }

        private StartData GetStartData(LogicalArea area)
        {
            StartData returnValue;

            if (CheckIsStartPos(area))
            {
                string[] splitedStr = area.TypeName.Split(sepratorChars, StringSplitOptions.RemoveEmptyEntries);

                returnValue.Position = area.Position;
                returnValue.PlayerID = int.Parse(splitedStr[3]);
                returnValue.Stat = int.Parse(splitedStr[4]);
            }
            else
            {
                returnValue = new StartData(0, Vector3.Zero, 0);
            }

            return returnValue;
        }

        private bool CheckIsEndPos(LogicalArea area)
        {
            string[] strSplited = area.TypeName.Split(sepratorChars, StringSplitOptions.RemoveEmptyEntries);
            if (strSplited[2] == "END")
            {
                return true;
            }

            return false;
        }

        private void PlaceBicyclesToStartPosition()
        {

        }

        private void ProcessLogicAreas()
        {
            #region 处理自行车的开始信息
            BicycleStartInfo = new List<StartData>();

            //首先把可利用的信息撞到StartInfo里面去
            for (int i = 0; i < LogicalAreas.Count; i++)
            {
                if (CheckIsStartPos(LogicalAreas[i]))
                {
                    StartData data = GetStartData(LogicalAreas[i]);
                    BicycleStartInfo.Add(data);
                }
            }

            //已经建立了的playerID
            bool[] playerIDCreated = new bool[64];

            //然后遍历StartInfo,建立自行车
            for (int i = 0; i < BicycleStartInfo.Count; i++)
            {
                if ((!playerIDCreated[BicycleStartInfo[i].PlayerID]) && (BicycleStartInfo[i].Stat == 0))
                {
                    playerIDCreated[BicycleStartInfo[i].PlayerID] = true;

                    //找到了起点
                    Vector3 startPos = BicycleStartInfo[i].Position;
                    Vector3 endPos = new Vector3();
                    int curID = BicycleStartInfo[i].PlayerID;
                    //在List中寻找指向的点
                    for (int j = 0; j < BicycleStartInfo.Count; i++)
                    {
                        if ((curID == BicycleStartInfo[j].PlayerID) && (BicycleStartInfo[j].Stat == 1))
                        {
                            endPos = BicycleStartInfo[j].Position;
                        }
                    }

                    //根据startPos,endPos和curID建立自行车,并注册
                    PlaceRegisterBicycle(startPos, endPos, curID);
                }
            }
            #endregion

            #region 处理自行车的结束信息
            BicycleEndInfo = new List<EndData>();
            for (int i = 0; i < LogicalAreas.Count; i ++)
            {
                if (CheckIsEndPos(LogicalAreas[i]))
                {
                    EndData endData;
                    endData.Position = LogicalAreas[i].Position;
                    endData.Radius = LogicalAreas[i].Radius;
                    BicycleEndInfo.Add(endData);
                }
            }
            #endregion

            #region 处理自行车的行走路线
            //第一步,得出自行车的起点和终点
            Vector3 sPos = BicycleStartInfo[0].Position;
            Vector3 ePos = BicycleEndInfo[0].Position;

            //第二步,得出自行车的经过的路径
            List<Vector3> pointPassed = aiPathManager.PathFinder.GetWay(sPos, ePos);
            #endregion
        }

        private void InitCreator()
        {
            bicycleManager = new BicycleManager();
            objectManager = new ObjectManager();
            objectCreator = new ObjectCreator(device, game.CurrentWorld, objectManager);
            aiPathManager = new AIPathManager(SceneObjects);
        }

        public override void Initialize()
        {
            //第一步,生成BicycleManager,ObjectCreator
            InitCreator();

            ///***********************************TEST*/
            //Vector3 startPos = aiPathManager.AIPorts[0].Position;
            //Vector3 endPos = aiPathManager.AIPorts[aiPathManager.AIPorts.Count - 1].Position;
            //List<Vector3> pointPassed = aiPathManager.PathFinder.GetWay(startPos, endPos);
            //PassedPointsMgr mgr = new PassedPointsMgr(pointPassed, null,BicycleEndInfo);
            ///***********************************TEST*/

            //第二步,处理LogicArea
            ProcessLogicAreas();

            base.Initialize();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void Unload()
        {
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
