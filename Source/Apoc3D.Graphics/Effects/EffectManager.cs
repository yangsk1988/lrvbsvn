using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D.Graphics.Effects
{
    public class EffectManager : Singleton
    {
        static EffectManager singleton;

        public static void Initialize(RenderSystem device)
        {
            singleton = new EffectManager();

            EngineConsole.Instance.Write("特效系统启动。", ConsoleMessageType.Information);

            //singleton.RegisterModelEffectType(StandardEffectFactory.Name, new StandardEffectFactory(device));
            //singleton.RegisterModelEffectType(TerrainRenderingEffectFactory.Name, new TerrainRenderingEffectFactory(device));

            //singleton.RegisterModelEffectType(RoadEffectFactory.Name, new RoadEffectFactory(device));
        }


        /// <summary>
        ///   Gets the singleton instance of this class.
        /// </summary>
        public static EffectManager Instance
        {
            get
            {
                return singleton;
            }
        }

        private EffectManager()
        {
            Enabled = true;
        }

        Dictionary<string, Effect> modelFX = new Dictionary<string, Effect>(CaseInsensitiveStringComparer.Instance);
        


        Dictionary<string, EffectFactory> modelFXFac = new Dictionary<string, EffectFactory>(CaseInsensitiveStringComparer.Instance);



        public void RegisterModelEffectType(string name, EffectFactory fac)
        {
            modelFXFac.Add(name, fac);
            EngineConsole.Instance.Write("找到模型特效 " + name, ConsoleMessageType.Information);
        }


        public void UnregisterModelEffectType(string name)
        {
            modelFXFac.Remove(name);
        }


        public Effect GetModelEffect(string name)
        {
            return Enabled ? modelFX[name] : null;
        }

        public int ModelEffectCount
        {
            get { return modelFX.Count; }
        }

        public bool HasModelEffect(string name)
        {
            return modelFX.ContainsKey(name);
        }

        public void LoadEffects()
        {
            EffectsLoaded = true;
            foreach (KeyValuePair<string, EffectFactory> e in modelFXFac)
            {
                modelFX.Add(e.Key, e.Value.CreateInstance());
            }

            string msg = "{0} 种特效已经加载。";
            EngineConsole.Instance.Write(
                string.Format(msg, modelFXFac.Count.ToString()), ConsoleMessageType.Information);
        }

        public void UnloadEffects()
        {
            EffectsLoaded = false;
            foreach (KeyValuePair<string, Effect> e in modelFX)
            {
                modelFXFac[e.Key].DestroyInstance(e.Value);
            }
            modelFX.Clear();
        }

        public bool Enabled
        {
            get;
            set;
        }

        public bool EffectsLoaded
        {
            get;
            private set;
        }
        protected override void dispose()
        {
            if (EffectsLoaded)
            {
                UnloadEffects();
            }
            singleton = null;
        }
    }
}
