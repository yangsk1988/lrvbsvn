﻿using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Vfs;

namespace VirtualBicycle.Scene
{
    public abstract class ObjectFactory
    {
        #region 字段

        ObjectTypeManager manager;

        #endregion

        #region 构造函数

        protected ObjectFactory(ObjectTypeManager mgr)
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