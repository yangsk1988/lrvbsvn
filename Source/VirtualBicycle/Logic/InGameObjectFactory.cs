using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;
using VirtualBicycle.IO;

namespace VirtualBicycle.Logic
{
    public abstract class InGameObjectFactory
    {
        #region 字段

        InGameObjectManager manager;

        #endregion

        #region 构造函数

        protected InGameObjectFactory(InGameObjectManager mgr)
        {
            manager = mgr;
        }

        
        #endregion

        #region 属性

        public InGameObjectManager Manager
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
