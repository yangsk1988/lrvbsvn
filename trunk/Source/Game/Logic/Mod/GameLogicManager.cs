using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Collections;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;

namespace VirtualBicycle.Logic.Mod
{
    public class GameLogicManager : Singleton
    {
        static GameLogicManager singleton;

        public static GameLogicManager Instance
        {
            get { return singleton; }
        }

        public static void Initialize()
        {
            singleton = new GameLogicManager();
        }

        List<LogicMod> plugins;
        FastList<RenderOperation> opBuffer;

        private GameLogicManager()
        {
            opBuffer = new FastList<RenderOperation>();


            string[] dlls = FileSystem.Instance.SearchFile("*.lgc");

            plugins = new List<LogicMod>(dlls.Length);

            for (int i = 0; i < dlls.Length; i++)
            {
                Assembly am = Assembly.LoadFile(dlls[i]);

                EngineConsole.Instance.Write(string.Format("发现逻辑模块 {0}", dlls[i]), ConsoleMessageType.Information);

                LoadModule(am);
            }
            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].GameInitialize();
            }
        }

        void LoadModule(Assembly m)
        {
            Type baseType = typeof(LogicModFactroy);

            Type[] types = m.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsSubclassOf(baseType))
                {
                    LogicModFactroy fac = (LogicModFactroy)Activator.CreateInstance(types[i]);

                    LogicMod plug = fac.CreateInstance();

                    plugins.Add(plug);

                    EngineConsole.Instance.Write(string.Format("创建逻辑处理对象 {0}", plug.GetType().Name), ConsoleMessageType.Information);
                }
            }
        }

        //public void Render(Sprite spr)
        //{
        //    for (int i = 0; i < plugins.Count; i++)
        //    {
        //        plugins[i].Render(spr);
        //    }
        //}

        public void Update(float dt) 
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].Update(dt);
            }
        }

        //public RenderOperation[] GetRenderOperation()
        //{
        //    opBuffer.Clear();


        //    for (int i = 0; i < plugins.Count; i++)
        //    {
        //        RenderOperation[] op = plugins[i].Render();
        //        if (op != null)
        //        {
        //            opBuffer.Add(op);
        //        }
        //    }

        //    return opBuffer.Elements;
        //}

        public void GameFinalize() 
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].GameFinalize();
            }
        }

        public void WorldFinalize()
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].WorldFinalize();
            }
        }



        public void WorldLoaded(World world)
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].WorldLoaded(world);
            }
        }

        public void WorldInitialize(InGameObjectManager manager)
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].WorldInitialize(manager);
            }

        }
        protected override void dispose()
        {

        }
    }
}
