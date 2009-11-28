using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Vfs;

namespace Apoc3D.Scene
{
    public abstract class SceneObjectFactory
    {
        #region 字段

        ObjectTypeManager manager;

        #endregion

        #region 构造函数

        protected SceneObjectFactory(ObjectTypeManager mgr)
        {
            manager = mgr;
        }

        
        #endregion

        #region 属性

        public ObjectTypeManager Manager
        {
            get { return manager; }
        }
     
        public abstract string TypeTag
        {
            get;
        }

        #endregion

        #region 方法

        public abstract SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData);

        #endregion
    }
}
