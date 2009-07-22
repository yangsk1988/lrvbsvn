using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.IO;
using VirtualBicycle.Logic.Traffic;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class TerrainObject : StaticObject
    {
        #region 字段

        TerrainObjectType objType;

        float heightScale = 1f;

        //float currentHS = 0.01f;

        #endregion

        #region 属性

        public string TypeName
        {
            get { return objType.TypeName; }
        }

        public float HeightScale
        {
            get { return heightScale; }
            set
            {
                heightScale = value; 
                isTransformDirty = true;
            }
        }

        #endregion

        #region 构造函数

        public TerrainObject(TerrainObjectType bldType)
        {
            this.objType = bldType;
            this.ModelL0 = bldType.Model;
            this.ModelL1 = bldType.LodModel;
        }

        #endregion

        #region 方法

        public override void UpdateTransform()
        {
            Matrix rotation;
            Matrix.RotationQuaternion(ref orientation, out rotation);

            Matrix.Translation(ref position, out Transformation);
            Matrix.Multiply(ref rotation, ref Transformation, out Transformation);

            if (heightScale != 1f)
            {
                Matrix scale = Matrix.Scaling(1, heightScale, 1);

                Matrix.Multiply(ref scale, ref Transformation, out Transformation);
            }
            BoundingSphere.Center = position + BoundingSphereOffset;

            RequiresUpdate = true;
        }

        //public override void Update(float dt)
        //{
        //    if (currentHS < heightScale) 
        //    {
        //        currentHS += dt * 0.1f;
        //        isTransformDirty = true;
        //    }
        //}

        #endregion

        #region SceneObject 序列化

        public override void Serialize(BinaryDataWriter data)
        {
            TerrainObjectFactory.Serialize(this, data);
        }

        public override bool IsSerializable
        {
            get { return true; }
        }
        public override string TypeTag
        {
            get
            {
                return TerrainObjectFactory.TypeId;
            }
        }
        #endregion
    }
}

