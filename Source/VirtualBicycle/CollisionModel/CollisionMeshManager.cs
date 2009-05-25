using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Graphics;
using SlimDX;

namespace VirtualBicycle.CollisionModel
{
    public class CollisionMeshManager : ResourceManager
    {
        static CollisionMeshManager singleton;

        public static CollisionMeshManager Instance
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
                singleton = new CollisionMeshManager();
            }
        }

        private CollisionMeshManager()
        {
        }

        public CollisionMesh CreateInstance(Model model, Matrix trans)
        {
            string hashString = CollisionMesh.GetHashString(model, trans);

            Resource retrived = base.Exists(hashString);
            if (retrived == null)
            {
                CollisionMesh mdl = new CollisionMesh(model, trans);

                retrived = mdl;
                base.NewResource(mdl, CacheType.Dynamic);
            }
            else
            {
                retrived.Use();
            }
            return new CollisionMesh((CollisionMesh)retrived);
        }
        //public CollisionMesh CreateInstance(GameMesh mesh, Matrix trans)
        //{
        //    string hashString = CollisionMesh.GetHashString(mesh, trans);

        //    Resource retrived = base.Exists(hashString);
        //    if (retrived == null)
        //    {
        //        CollisionMesh mdl = new CollisionMesh(mesh, trans);

        //        retrived = mdl;
        //        base.NewResource(mdl, CacheType.Dynamic);
        //    }
        //    else
        //    {
        //        retrived.Use();
        //    }
        //    return new CollisionMesh((CollisionMesh)retrived);
        //}
        public void DestoryInstance(CollisionMesh model)
        {
            base.DestoryResource(model);
        }
    }
}
