using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Vfs;
using Apoc3D.Media;

namespace Apoc3D.Graphics
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
        public Texture CreateInstance(int length, int surfaceCount, ImagePixelFormat format)
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
                base.NotifyResourceNew(tex);
            }
            else
            {
                retrived.Use();
            }

            return new ResourceHandle<Texture>((Texture)retrived);
        }

    }
}
