using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle;
using VirtualBicycle.Input;
using VirtualBicycle.Logic;
using VirtualBicycle.Logic.Competition;
using VirtualBicycle.Logic.Goal;
using VirtualBicycle.Logic.Mod;
using VirtualBicycle.Logic.PathWay;
using VirtualBicycle.Logic.Traffic;
using VirtualBicycle.MathLib;
using VirtualBicycle.ObjectMgr;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;
using VirtualBicycle.UI;
using PM = VirtualBicycle.Physics.MathLib;

namespace MainLogic
{
    public class GameMainLogic : LogicMod
    {
        Device device;

        UILogic uiLogic;


        public BaseCompetition CurrentCompetition
        {
            get;
            private set;
        }

        public static int CurrentCompetitionID
        {
            get;
            set;
        }

        public UILogic UILogic
        {
            get { return uiLogic; }
        }


        protected override void GameInitialize()
        {
            base.GameInitialize();

            uiLogic = new UILogic(this);
            uiLogic.Initialize();

            device = Game.GameUI.Device;
        }

        static readonly char[] sepChars = new char[] { '_' };

        void LoadCompetitions(World world, SceneObject[] sceObjs)
        {
            Dictionary<int, List<LogicalArea>> table
                = new Dictionary<int, List<LogicalArea>>();

            for (int i = 0; i < sceObjs.Length; i++)
            {
                LogicalArea area = sceObjs[i] as LogicalArea;

                if (area != null)
                {
                    string[] v = area.TypeName.Split(sepChars, StringSplitOptions.RemoveEmptyEntries);

                    int comId = int.Parse(v[0]);

                    if (v.Length >= 1)
                    {
                        List<LogicalArea> list;
                        if (!table.TryGetValue(comId, out list)) 
                        {
                            list = new List<LogicalArea>();
                            table.Add(comId, list);
                        }

                        list.Add(area);
                    }
                }
            }

            foreach (KeyValuePair<int, List<LogicalArea>> e in table) 
            {
                List<LogicalArea> list = e.Value;
                string[] v = list[0].TypeName.Split(sepChars, StringSplitOptions.RemoveEmptyEntries);

                BaseCompetition com = null;
                switch (v[1])
                {
                    case "R":
                        com = new RacingCompetition(Game, world, list, sceObjs);
                        break;
                    case "T":
                        break;
                    default:
                        throw new NotSupportedException(v[1]);
                }
            }
        }

        protected override void WorldLoaded(World world)
        {
            base.WorldLoaded(world);
            uiLogic.WorldLoaded(world);

            #region 查找到SceneObject[]
            GameScene gameScene = world.Scene;
            SceneData data = gameScene.Data;
            SceneObject[] objects = data.SceneObjects;

            #endregion

            LoadCompetitions(world, objects);

            #region 构建当前的比赛
            CurrentCompetition = new TestCompetition(Game, this, world, null, objects);
            //CurrentCompetition = new RacingCompetition(Game, world, null, objects);
            #endregion
        }

        protected override void WorldFinalize()
        {
            base.WorldFinalize();

            CurrentCompetition.Dispose();
        }

        protected override void WorldInitialize(InGameObjectManager manager)
        {
            base.WorldInitialize(manager);
            uiLogic.WorldInitialize(manager);

            InGameObjectFactory[] fac = Plugin.GetAllObjectTypes(device, manager, this);
            for (int i = 0; i < fac.Length; i++)
            {
                manager.RegisterObjectType(fac[i]);
            }
        }

        protected override void Update(float dt)
        {
            if (CurrentCompetition != null)
            {
                CurrentCompetition.Update(dt);
            }
            uiLogic.Update(dt);
        }

        #region Test
        private void TestFindWay()
        {

        }
        #endregion
    }
}
