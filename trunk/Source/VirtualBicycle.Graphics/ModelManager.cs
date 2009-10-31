using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Core;
using VirtualBicycle.Vfs;

namespace VirtualBicycle.Graphics
{
    public class ModelManager : ResourceManager
    {
        static ModelManager singleton;

        public static ModelManager Instance
        {
            get
            {
                return singleton;
            }
        }

        public static void Initialize()
        {
            if (singleton == null)
            {
                singleton = new ModelManager(1048576 * 92);
            }
            EngineConsole.Instance.Write("模型管理器初始化完毕。内存使用上限" + Math.Round(singleton.TotalCacheSize / 1048576.0, 2).ToString() + "MB。", ConsoleMessageType.Information);
        }

        public ModelManager() { }
        public ModelManager(int cacheSize)
            : base(cacheSize)
        {
        }
        public ResourceHandle<Model> CreateInstance(RenderSystem device, ResourceLocation rl)
        {
            Resource retrived = base.Exists(rl.Name);
            if (retrived == null)
            {
                Model mdl = new Model(device, rl);
                retrived = mdl;
                base.NotifyResourceNew(mdl, CacheType.Static);
            }
            else
            {
                retrived.Use();
            }
            return new ResourceHandle<Model>((Model)retrived);
        }

    }
}
