using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Core;
using VirtualBicycle.Vfs;
using VirtualBicycle.Media;

namespace VirtualBicycle.Graphics
{
    public class TextureManager : ResourceManager
    {
        static volatile TextureManager singleton;
        static volatile object syncHelper = new object();

        public static TextureManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncHelper)
                    {
                        if (singleton == null)
                        {
                            singleton = new TextureManager();
                        }
                    }
                }
                return singleton;
            }
        }

        private TextureManager()
        {
            CreationUsage = TextureUsage.Static;
        }

        public ObjectFactory Factory
        {
            get;
            set;
        }
        public TextureUsage CreationUsage
        {
            get;
            set;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        /// <remarks>以这种方式创建的资源不会被管理</remarks>
        public Texture CreateInstance(Image image)
        {
            if (Factory == null)
            {
                throw new InvalidOperationException();
            }

            return Factory.CreateTexture(image, CreationUsage);
        }
        /// <remarks>以这种方式创建的资源不会被管理</remarks>
        public Texture CreateInstance(int width, int height, int surfaceCount, ImagePixelFormat format)
        {
            if (Factory == null)
            {
                throw new InvalidOperationException();
            }

            return Factory.CreateTexture(width, height, surfaceCount, CreationUsage, format);
        }
        /// <remarks>以这种方式创建的资源不会被管理</remarks>
        public Texture CreateInstance(int width, int height, int depth, int surfaceCount, ImagePixelFormat format)
        {
            if (Factory == null)
            {
                throw new InvalidOperationException();
            }

            return Factory.CreateTexture(width, height, depth, surfaceCount, CreationUsage, format);
        }
        /// <remarks>以这种方式创建的资源不会被管理</remarks>
        public Texture CreateInstance(int length, int surfaceCount, PixelFormat format)
        {
            if (Factory == null)
            {
                throw new InvalidOperationException();
            }
            return Factory.CreateTexture(length, surfaceCount, CreationUsage, format);
        }

        public ResourceHandle<Texture> CreateInstance(FileLocation fl)
        {
            if (Factory == null)
            {
                throw new InvalidOperationException();
            }
            Resource retrived = base.Exists(fl.Name);
            if (retrived == null)
            {
                Texture tex = Factory.CreateTexture(fl, CreationUsage);
                retrived = tex;
                base.NotifyResourceNew(tex, CacheType.Static);
            }
            else
            {
                retrived.Use();
            }

            return new ResourceHandle<Texture>((Texture)retrived);
        }

    }
}
