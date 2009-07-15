using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SlimDX;
using VirtualBicycle.CollisionModel;
using VirtualBicycle.Graphics;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using PM = VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.Scene
{
    public interface IPositionedObject 
    {
        Vector3 Position { get; set; }

        void UpdateTransform();

        [Browsable(false)]
        bool EditorMovable { get; }
    }
    public interface IRoatableObject 
    {
        Quaternion Orientation { get; set; }
        void UpdateTransform();

        [Browsable(false)]
        bool EditorMovable { get; }
    }

    /// <summary>
    ///  表示有模型的场景物体
    /// </summary>
    public abstract class Entity : SceneObject, IUpdatable, IPositionedObject, IRoatableObject
    {
        #region 字段

        protected Vector3 position;
        protected Quaternion orientation = Quaternion.Identity;

        protected bool isTransformDirty;

        bool useLodModel;
        bool dontDraw;

        Vector3 boundingSphereOffset;

        #endregion

        #region 构造函数

        protected Entity(bool hasSubObjects)
            : base(hasSubObjects)
        {
        }

        #endregion

        #region 属性

        

        protected bool UseLodModel
        {
            get { return useLodModel; }
            private set { useLodModel = value; }
        }
        /// <summary>
        ///  获取一个四元数，表示物体的朝向
        /// </summary>
        public Quaternion Orientation
        {
            get { return orientation; }
            set
            {
                if (RigidBody != null)
                {
                    RigidBody.Orientation = value;
                }
                orientation = value;
                isTransformDirty = true;
            }
        }

        /// <summary>
        ///  获取物体的位置
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;

                BoundingSphere.Center = value + BoundingSphereOffset;

                if (RigidBody != null)
                {
                    RigidBody.Translate((PM.Vector3)value - RigidBody.CenterOfMassPosition);
                }
                isTransformDirty = true;
            }
        }

        public Vector3 BoundingSphereOffset
        {
            get { return boundingSphereOffset; }
            protected set
            {
                boundingSphereOffset = value;
                BoundingSphere.Center = value + Position;
            }
        }

        public Model Model
        {
            get;
            protected set;
        }

        public Model LodModel
        {
            get;
            protected set;
        }

        [Browsable(false)]
        public virtual bool EditorMovable
        {
            get { return true; }
        }

        #endregion

        #region 方法

        public override void NotifyClusterChanged(Cluster newCluster)
        {
            float ofsX = newCluster.WorldX - OffsetX;
            float ofsY = newCluster.WorldY - OffsetY;
            float ofsZ = newCluster.WorldZ - OffsetZ;

            BoundingSphere.Center.X -= ofsX;
            BoundingSphere.Center.Y -= ofsY;
            BoundingSphere.Center.Z -= ofsZ;

            position.X -= ofsX;
            position.Y -= ofsY;
            position.Z -= ofsZ;
        
        }

        public virtual void UpdateTransform()
        {
            // T = r * t

            Matrix rotation;
            Matrix.RotationQuaternion(ref orientation, out rotation);

            Matrix.Translation(ref position, out Transformation);
            Matrix.Multiply(ref rotation, ref Transformation, out Transformation);


            RequiresUpdate = true;

        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (UseLodModel)
            {
                if (LodModel != null)
                {
                    return LodModel.GetRenderOperation();
                }
            }
            if (Model != null)
            {
                return Model.GetRenderOperation();
            }

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Model = null;
            LodModel = null;
        }

        #endregion

        #region IUpdatable 成员

        public override void Update(float dt)
        {
            if (Model != null)
            {
                Model.Update(dt);
            }

            if (LodModel != null)
            {
                LodModel.Update(dt);
            }

            if (GameScene != null)
            {
                ICamera camera = GameScene.CurrentCamera;

                float dist = Vector3.Distance(camera.Position, BoundingSphere.Center);

                UseLodModel = dist > BoundingSphere.Radius * 15f;

                dontDraw = (dist - BoundingSphere.Radius) > camera.FarPlane * 0.75f;
            }
            else
            {
                UseLodModel = false;
            }

            if (isTransformDirty)
            {
                UpdateTransform();
                isTransformDirty = false;
            }
        }

        #endregion
    }
}
