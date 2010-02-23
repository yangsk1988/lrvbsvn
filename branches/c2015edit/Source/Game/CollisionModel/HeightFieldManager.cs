using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Core;
using VirtualBicycle.Graphics;
using VBC = VirtualBicycle.Core;

namespace VirtualBicycle.CollisionModel
{
    public class HeightFieldManager : VBC.ResourceManager
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

            VBC.Resource retrived = base.Exists(hashString);
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
