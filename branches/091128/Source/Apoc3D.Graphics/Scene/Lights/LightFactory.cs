using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Vfs;

namespace Apoc3D.Scene
{
    class LightFactory : SceneObjectFactory
    { 
        #region 常量

        static readonly string typeTag = "Light";
        
        #endregion


        public LightFactory(ObjectTypeManager mgr)
            : base(mgr)
        {
        }

        public static string TypeId
        {
            get { return typeTag; }
        }

        public override string TypeTag
        {
            get { return typeTag; }
        }

        public override SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData)
        {
            throw new NotImplementedException();
        }
    }
}
