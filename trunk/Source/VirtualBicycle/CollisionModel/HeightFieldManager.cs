using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;

namespace VirtualBicycle.CollisionModel
{
    public class HeightFieldManager : ResourceManager
    {
        static HeightFieldManager singleton;

        public static HeightFieldManager Instance
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
                singleton = new HeightFieldManager();
            }
        }
        private HeightFieldManager()
        {
        }

        public HeightField CreateInstance(GameTexture dispTex)
        {
            string hashString = dispTex.HashString;

            Resource retrived = base.Exists(hashString);
            if (retrived == null)
            {
                HeightField mdl = new HeightField(dispTex);

                retrived = mdl;
                base.NewResource(mdl, CacheType.Static);
            }
            else
            {
                retrived.Use();
            }
            return new HeightField((HeightField)retrived);
        }
        public void DestoryInstance(HeightField hf)
        {
            base.DestoryResource(hf);
        }
    }
}
