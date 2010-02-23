using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Core;
using VirtualBicycle.Graphics;
using VirtualBicycle.Vfs;

namespace VirtualBicycle.Scene
{
    /// <summary>
    ///  负责管理DisplacementMap，ColorMap，NormalMap以及IndexMap
    /// </summary>
    public class TerrainTextureManager : ResourceManager
    {
        static int defaultCacheSize = 128 * 1048576;

        public static int DefaultCacheSize
        {
            get { return defaultCacheSize; }
            private set { defaultCacheSize = value; }
        }

        #region 单例相关

        static TerrainTextureManager singleton;

        /// <summary>
        ///  获取TextureManager的实例
        /// </summary>
        public static TerrainTextureManager Instance
        {
            get
            {
                return singleton;
            }
        }

        public static void Initialize(int cacheSize)
        {
            if (singleton == null)
            {
                singleton = new TerrainTextureManager(cacheSize);
            }
            EngineConsole.Instance.Write("地形纹理管理器初始化完毕。内存使用上限" + Math.Round(cacheSize / 1048576.0, 2).ToString() + "MB。", ConsoleMessageType.Information);
        }

        #endregion

        private TerrainTextureManager(int cacheSize)
            : base(cacheSize)
        {
            CreationUsage = TextureUsage.Static;
        }

        #region 属性

        /// <summary>
        ///  获取或设置新创建纹理的用法(Usage)
        /// </summary>
        public TextureUsage CreationUsage
        {
            get;
            set;
        }

        #endregion

        #region 方法

        /// <summary>
        ///  创建一个新的纹理对象，并对其进行管理
        /// </summary>
        /// <param name="device"></param>
        /// <param name="rl"></param>
        /// <returns></returns>
        public TerrainTexture CreateInstance(RenderSystem device, ResourceLocation rl, bool isDisp)
        {
            Resource retrived = base.Exists(rl.Name);
            if (retrived == null)
            {
                TerrainTexture tex = new TerrainTexture(this, device, rl, CreationUsage, isDisp);
                retrived = tex;
                base.NotifyResourceNew(tex, CacheType.Static);

                return new TerrainTexture(this, tex);
            }
            else
            {
                retrived.Use();

                if (retrived.IsResourceEntity)
                {
                    return new TerrainTexture(this, (TerrainTexture)retrived);
                }
                return (TerrainTexture)retrived;
            }
        }

        public TerrainTexture CreateInstance(Texture texture, bool isDisp)
        {
            TerrainTexture tex = new TerrainTexture(this, texture, isDisp);

            base.NotifyNewCached(tex, CacheType.Static);

            return new TerrainTexture(this, tex);
        }

        /// <summary>
        ///  销毁一个纹理对象
        ///  即使该纹理不受管理也照常销毁
        /// </summary>
        /// <param name="texture"></param>
        public void DestroyInstance(TerrainTexture texture)
        {
            base.DestoryResource(texture);
        }

        #endregion
    }
}
